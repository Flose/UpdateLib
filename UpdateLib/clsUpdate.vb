Imports System.ComponentModel
Imports System.Diagnostics

Public Class Update
    Private updateServers As New List(Of Uri)
    Private updateServersFile As String
    Private tempUpdateBasePath As String
    Private tempUpdatePath As String

    Private installedCategories As List(Of String)
    Private filesToUpdate As List(Of File)
    Private filesToDelete As List(Of String)
    Private localVersionsFile As VersionsFile
    Private remoteVersionsFile As VersionsFile
    Private currentServer As Uri

    Private _releaseChannels As New List(Of String)(New String() {"stable", "beta", "alpha"})
    Public ReadOnly Property ReleaseChannels As IList(Of String)
        Get
            Return _releaseChannels.AsReadOnly()
        End Get
    End Property

    Private _currentReleaseChannel As Integer = 0
    Public Property CurrentReleaseChannel As String
        Get
            Return _releaseChannels(_currentReleaseChannel)
        End Get
        Set(value As String)
            _currentReleaseChannel = _releaseChannels.IndexOf(value)
            If _currentReleaseChannel = -1 Then
                _currentReleaseChannel = 0
            End If
        End Set
    End Property

    Private programName, programExe, programPath As String
    Private programVersion As Version
    Private _translatedProgramName As String
    Friend Property TranslatedProgramName As String
        Get
            If _translatedProgramName IsNot Nothing Then
                Return _translatedProgramName
            End If
            Return programName
        End Get
        Set(value As String)
            _translatedProgramName = value
        End Set
    End Property

    Private uid As String
    Private statisticsServerUri As Uri
    ''' <summary>
    ''' Optionally specify a server URL, that will be notified about update events.
    ''' The data is transmitted in the URL query part.
    ''' </summary>
    ''' <returns></returns>
    Public Property StatisticsServer As String
        Get
            Return statisticsServerUri.ToString
        End Get
        Set(value As String)
            statisticsServerUri = New Uri(value)
        End Set
    End Property

    ''' <summary>
    ''' Optionally set the product flavor, i.e. Normal and Portable.
    ''' This is only used for statistics.
    ''' </summary>
    ''' <returns></returns>
    Public Property ProductFlavor As String

    Private isUpdating As Boolean

    Friend t As New TranslationLib.Translation(Nothing, My.Resources.English)

    ''' <summary>
    ''' This event is raised just before restarting to install an update.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Event Restarting(sender As Object, e As EventArgs)

    Public Event UpdateError(sender As Object, e As ErrorEventArgs)

    Public Event UpdateInfo(sender As Object, e As InfoEventArgs)

    Public Event UpdateFound(sender As Object, e As UpdateFoundEventArgs)

    Public Event UpdateDownloaded(sender As Object, e As EventArgs)

    Private Sub RaiseUpdateInfoEvent(message As String)
        Dim ea = New InfoEventArgs(message)
        If UpdateInfoEvent Is Nothing OrElse UpdateInfoEvent.GetInvocationList.Length = 0 Then
            MessageBoxUi.UpdateInfo(Me, ea)
        Else
            RaiseEvent UpdateInfo(Me, ea)
        End If
    End Sub

    Private Sub RaiseUpdateErrorEvent(message As String, innerException As Exception)
        Dim ea = New ErrorEventArgs(message, innerException)
        If UpdateErrorEvent Is Nothing OrElse UpdateErrorEvent.GetInvocationList.Length = 0 Then
            MessageBoxUi.UpdateError(Me, ea)
        Else
            RaiseEvent UpdateError(Me, ea)
        End If
    End Sub

    Private Sub RaiseUpdateFoundEvent(newVersionFile As VersionsFile, frameworkInstallStatus As InstallStatus)
        Dim ea = New UpdateFoundEventArgs(newVersionFile.DisplayVersion, newVersionFile.ReleaseNotesUrl, newVersionFile.Framework, newVersionFile.FrameworkUrl, frameworkInstallStatus)
        If UpdateFoundEvent Is Nothing OrElse UpdateFoundEvent.GetInvocationList.Length = 0 Then
            MessageBoxUi.UpdateFound(Me, ea)
        Else
            RaiseEvent UpdateFound(Me, ea)
        End If
    End Sub

    Private Sub RaiseUpdateDownloadedEvent()
        If UpdateDownloadedEvent Is Nothing OrElse UpdateDownloadedEvent.GetInvocationList.Length = 0 Then
            MessageBoxUi.UpdateDownloaded(Me)
        Else
            RaiseEvent UpdateDownloaded(Me, EventArgs.Empty)
        End If
    End Sub

    Friend ReadOnly Property TranslatedTitle() As String
        Get
            Return t.Translate("Update", TranslatedProgramName)
        End Get
    End Property


    Private Const versionsFileName = "versions.json"

    Private ReadOnly Property LocalVersionsFilePath As String
        Get
            Return IO.Path.Combine(programPath, versionsFileName)
        End Get
    End Property

    Private ReadOnly Property UpdateTempVersionsFilePath As String
        Get
            Return IO.Path.Combine(tempUpdatePath, versionsFileName)
        End Get
    End Property

    Private ReadOnly Property RemoteVersionsFilePath(server As Uri) As Uri
        Get
            Return GetUpdateFileUri(server, CurrentReleaseChannel, versionsFileName)
        End Get
    End Property

    Private Shared _highestInstalledNetFramework As Version
    Private Shared ReadOnly Property HighestInstalledNetFramework As Version
        Get
            If _highestInstalledNetFramework.Equals(New Version) Then
                _highestInstalledNetFramework = GetHighestInstalledNetFramework()
            End If
            Return _highestInstalledNetFramework
        End Get
    End Property

    Public Sub New(programName As String, programExe As String, programVersion As Version, programPath As String, tempUpdateBasePath As String, updateServersFile As String, updateServers As IEnumerable(Of String), ByVal uid As String)
        Me.programName = programName
        Me.programExe = programExe
        Me.programVersion = programVersion
        Me.programPath = programPath
        Me.uid = uid
        Me.updateServersFile = updateServersFile
        If updateServers IsNot Nothing Then
            For Each s In updateServers
                AddUpdateServer(s)
            Next
        End If
        Me.tempUpdateBasePath = tempUpdateBasePath
        Me.tempUpdatePath = IO.Path.Combine(tempUpdateBasePath, "Update")

        ' Load translations
        t.AddLanguage("German", "Deutsch", My.Resources.German)
        t.AddLanguage("English", "English", My.Resources.English)
        t.AddLanguage("French", "Français", My.Resources.French)
        t.AddLanguage("Spanish", "Español", My.Resources.Spanish)
        t.AddLanguage("Bavarian", "Boarisch", My.Resources.Bavarian)
        t.AddLanguage("Dutch", "Nederlands", My.Resources.Dutch)
        t.AddLanguage("Portuguese", "Português", My.Resources.Portuguese)
        t.AddLanguage("Polish", "Polski", My.Resources.Polish)
        t.AddLanguage("Chinese", "汉语", My.Resources.Chinese)
        t.AddLanguage("Serbian", "Srpski", My.Resources.Serbian)
        t.AddLanguage("Greek", "Ελληνικά", My.Resources.Greek)
        t.AddLanguage("Bulgarian", "Български", My.Resources.Bulgarian)
        t.AddLanguage("Danish", "Dansk", My.Resources.Danish)
        t.AddLanguage("Estonian", "Eesti", My.Resources.Estonian)

        ' Set default language
        t.Load(t.CheckLanguageName(String.Empty))
    End Sub

    Private Sub AddUpdateServer(server As String)
        Dim ub As New UriBuilder(server)
        If ub.Path.Length > 0 AndAlso ub.Path(ub.Path.Length - 1) <> "/"c Then
            ub.Path += "/"c
        End If
        Dim uri = ub.Uri
        If Not updateServers.Contains(uri) Then
            updateServers.Add(uri)
        End If
    End Sub

    ''' <summary>
    ''' Set the language to be used for translating messages.
    ''' </summary>
    ''' <param name="language">English language name</param>
    ''' <param name="translatedProgramName">The program name translated to <paramref name="language"/>language</param>
    Public Sub Translate(ByVal language As String, ByVal translatedProgramName As String)
        translatedProgramName = translatedProgramName
        language = t.CheckLanguageName(language)
        t.Load(language)
    End Sub

    Private Sub ReadUpdateServersFile()
        If Not IO.File.Exists(updateServersFile) Then
            Exit Sub
        End If
        Try
            Using UpdateReader As New IO.StreamReader(updateServersFile, True)
                For Each s In UpdateReader.ReadToEnd.Split(New Char() {Convert.ToChar(10), Convert.ToChar(13)}, StringSplitOptions.RemoveEmptyEntries)
                    AddUpdateServer(s)
                Next
            End Using
        Catch ex As Exception
            Console.Error.WriteLine("Error while opening update servers file, using fallback: {0}", ex.Message)
        End Try
    End Sub

    Private Sub ReadCategoriesFile()
        If Not IO.File.Exists(IO.Path.Combine(programPath, "Kategorien.ini")) Then
            installedCategories = Nothing
            Exit Sub
        End If

        installedCategories = New List(Of String)
        Try
            Using Reader As New IO.StreamReader(IO.Path.Combine(programPath, "Kategorien.ini"), True)
                Do
                    Dim line = Reader.ReadLine
                    If line Is Nothing Then
                        Exit Do
                    End If

                    Dim index = line.IndexOf("="c)
                    If index > -1 AndAlso String.Compare(line.Substring(index + 1).Trim, "true", StringComparison.OrdinalIgnoreCase) = 0 Then
                        installedCategories.Add(line.Substring(0, index).Trim)
                    End If
                Loop
            End Using
        Catch ex As Exception
            installedCategories = Nothing
            Console.Error.WriteLine("Error while opening categories file, ignoring: {0}", ex.Message)
        End Try
    End Sub

    Private Sub ReadLocalVersionsFile()
        Try
            localVersionsFile = VersionsFile.Open(LocalVersionsFilePath)
        Catch ex As Exception
            Throw New UpdateLocalVersionsFileBrokenException
        End Try
    End Sub

    Private Class UpdateAlreadyDownloadedException
        Inherits Exception
    End Class

    Private Class UpdateLocalVersionsFileBrokenException
        Inherits Exception
    End Class

    Private Function IsUpdateDownloaded() As Boolean
        Return IO.File.Exists(UpdateTempVersionsFilePath) AndAlso
               IO.Directory.GetFiles(tempUpdateBasePath, "Update-*.exe").Length > 0
    End Function

    Public Enum InstallStatus
        Installed
        NotInstalled
        Unknown
    End Enum

    Private Class SearchUpdateWorker
        Inherits BackgroundWorker

        Private x As Update
        Private showErrors As Boolean

        Private frameworkInstallStatus As InstallStatus

        Public Sub New(x As Update, showErrors As Boolean)
            Me.x = x
            Me.showErrors = showErrors
        End Sub

        Protected Overrides Sub OnDoWork(e As DoWorkEventArgs)
            MyBase.OnDoWork(e)

            If x.IsUpdateDownloaded() Then
                Throw New UpdateAlreadyDownloadedException
            End If

            x.ReadUpdateServersFile()
            x.ReadCategoriesFile()
            x.ReadLocalVersionsFile()

            If x.localVersionsFile.Version IsNot Nothing Then
                x.programVersion = x.localVersionsFile.Version
            End If

            Dim errors As String = Nothing
            ' Read remote version file
            For Each server In x.updateServers
                Try
                    Using stream = x.OpenWebStream(x.RemoteVersionsFilePath(server))
                        x.remoteVersionsFile = VersionsFile.Open(stream, True)
                    End Using
                    x.currentServer = server
                    Exit For
                Catch ex As Exception
                    Dim err = String.Format("{0}: {1}", server, ex.Message)
                    errors &= err & Environment.NewLine
                    x.remoteVersionsFile = Nothing
                End Try
            Next
            If x.remoteVersionsFile Is Nothing Then
                Throw New Exception(errors)
            End If

            x.SearchNewFiles()
            If x.filesToUpdate Is Nothing OrElse x.filesToUpdate.Count = 0 Then 'no update available
                x.remoteVersionsFile = Nothing
                e.Result = False
                Return
            End If

            If String.IsNullOrEmpty(x.remoteVersionsFile.Framework) OrElse x.remoteVersionsFile.Framework = x.localVersionsFile.Framework Then
                frameworkInstallStatus = InstallStatus.Installed
            Else
                Try
                    If HasFramework(x.remoteVersionsFile.Framework) Then
                        frameworkInstallStatus = InstallStatus.Installed
                    Else
                        frameworkInstallStatus = InstallStatus.NotInstalled
                    End If
                Catch ex As Exception
                    frameworkInstallStatus = InstallStatus.Unknown
                End Try
            End If

            e.Result = True
        End Sub

        Protected Overrides Sub OnRunWorkerCompleted(e As RunWorkerCompletedEventArgs)
            MyBase.OnRunWorkerCompleted(e)

            If e.Error IsNot Nothing Then
                If TypeOf e.Error Is UpdateAlreadyDownloadedException Then
                    'bereits ein Update vorhanden
                    If showErrors Then x.RaiseUpdateErrorEvent(x.t.Translate("msgUpdateBereitsVorhanden", x.TranslatedProgramName), e.Error)
                    x.isUpdating = False
                    x.remoteVersionsFile = Nothing
                    Exit Sub
                ElseIf TypeOf e.Error Is UpdateLocalVersionsFileBrokenException Then
                    Console.Error.WriteLine("{0}: {1}", versionsFileName, e.Error.Message)
                    If showErrors Then x.RaiseUpdateErrorEvent(x.t.Translate("msgUpdateLokaleInstallationNichtVollständig"), e.Error)
                    x.isUpdating = False
                    x.remoteVersionsFile = Nothing
                    Exit Sub
                End If

                Console.Error.WriteLine(e.Error)
                If showErrors Then x.RaiseUpdateErrorEvent(x.t.Translate("msgFehlerUpdateSuchen", Environment.NewLine & e.Error.Message), e.Error)
                x.SendStatistics(StatisticsTypes.UpdateError)
                x.isUpdating = False
                x.remoteVersionsFile = Nothing
                Exit Sub
            End If

            Dim updateAvailable = CBool(e.Result)
            If Not updateAvailable Then
                x.SendStatistics(StatisticsTypes.NoUpdateAvailable)
                If showErrors Then x.RaiseUpdateInfoEvent(x.t.Translate("msgKeinUpdate"))
                x.isUpdating = False
                x.remoteVersionsFile = Nothing
                Exit Sub
            End If

            x.isUpdating = False
            x.RaiseUpdateFoundEvent(x.remoteVersionsFile, frameworkInstallStatus)
        End Sub
    End Class

    Public Sub ResetUpdateState()
        If isUpdating OrElse remoteVersionsFile Is Nothing Then
            Return
        End If

        SendStatistics(StatisticsTypes.FoundAndNotInstalled)
        isUpdating = False
        remoteVersionsFile = Nothing
    End Sub

    ''' <summary>
    ''' Search for update. If an update is found, the UpdateFoundEvent is raised.
    ''' If no event listeners are registered the MessageBoxUi will be used.
    ''' </summary>
    ''' <param name="showErrors">If true, raises the UpdateErrorEvent if an error occurs</param>
    ''' <remarks></remarks>
    Public Sub SearchUpdateAsync(Optional showErrors As Boolean = True)
        If isUpdating Then
            Return
        End If
        isUpdating = True

        'Redetect framework version
        _highestInstalledNetFramework = New Version

        Dim w As New SearchUpdateWorker(Me, showErrors)
        w.RunWorkerAsync()
    End Sub

    Private Shared Function HasFramework(framework As String) As Boolean
        Dim m As Text.RegularExpressions.Match = Text.RegularExpressions.Regex.Match(framework, "([^0-9]*)([0-9]+(\.[0-9]+)+)")
        If m.Success AndAlso m.Groups(1).Value = "net" Then
            Dim requiredVersion As New Version(m.Groups(2).Value)

            If requiredVersion <= New Version(4, 6, 1) Then
                Return HighestInstalledNetFramework > requiredVersion
            End If
        End If

        Throw New Exception("Required framework is unknown, probably too recent")
    End Function

    Private Shared Function GetHighestInstalledNetFramework() As Version
        If IsRunningOnMono() Then
            If Environment.OSVersion.Platform = PlatformID.Unix Then
                Try
                    If IO.File.Exists("/usr/lib/mono/4.5/Microsoft.VisualBasic.dll") Then
                        Return New Version(4, 5)
                    ElseIf IO.File.Exists("/usr/lib/mono/4.0/Microsoft.VisualBasic.dll") Then
                        Return New Version(4, 0)
                    ElseIf IO.File.Exists("/usr/lib/mono/2.0/Microsoft.VisualBasic.dll") Then
                        Return New Version(2, 0)
                    End If
                Catch ex As Exception
                End Try
            Else
                ' TODO maybe check mono version
            End If
        Else
            ' See https://msdn.microsoft.com/en-us/library/hh925568.aspx
            Using ndpKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.RegistryKey.OpenRemoteBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, "").OpenSubKey("SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\")
                If ndpKey IsNot Nothing AndAlso ndpKey.GetValue("Release") IsNot Nothing Then
                    Dim releaseKey = CInt(ndpKey.GetValue("Release"))
                    If releaseKey >= 394254 Then
                        Return New Version(4, 6, 1)
                    ElseIf releaseKey >= 393295 Then
                        Return New Version(4, 6)
                    ElseIf releaseKey >= 379893 Then
                        Return New Version(4, 5, 2)
                    ElseIf releaseKey >= 378675 Then
                        Return New Version(4, 5, 1)
                    ElseIf releaseKey >= 378389 Then
                        Return New Version(4, 5)
                    End If
                End If
            End Using

            Dim versions As New List(Of Version)
            Using ndpKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.RegistryKey.OpenRemoteBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, "").OpenSubKey("SOFTWARE\Microsoft\NET Framework Setup\NDP\")
                For Each versionKeyName As String In ndpKey.GetSubKeyNames()
                    If Not versionKeyName.StartsWith("v") Then
                        Continue For
                    End If

                    Try
                        versions.Add(New Version(versionKeyName.Substring(1)))
                    Catch ex As Exception
                    End Try
                Next
            End Using
            If versions.Count > 0 Then
                versions.Sort()
                Return versions(versions.Count - 1)
            End If
        End If

        Throw New Exception("No framework found")
    End Function

    Private Shared Function GetUpdateFileUri(server As Uri, releaseChannel As String, fileName As String) As Uri
        Dim uriBuilder As New UriBuilder(server)
        uriBuilder.Path += releaseChannel + "/" + fileName
        Return uriBuilder.Uri
    End Function

    Private Class DownloadUpdateWorker
        Inherits BackgroundWorker

        Private x As Update
        Private ui As frmUpdate

        Public Sub New(x As Update, withUI As Boolean)
            Me.x = x
            Me.WorkerReportsProgress = True
            If withUI Then
                ui = New frmUpdate(x.t.Translate("Update", x.TranslatedProgramName))
                ui.Show()
            End If
        End Sub

        Private Class DownloadException
            Inherits Exception

            Public Sub New(uri As Uri, innerException As Exception)
                MyBase.New(String.Format("Download of ""{0}"" failed: {1}", uri, innerException.Message), innerException)
            End Sub

            Public Sub New(uri As Uri)
                MyBase.New(String.Format("Download of ""{0}"" failed.", uri))
            End Sub
        End Class

        Private Function DownloadAndStoreFile(url As Uri, destFilePath As String) As Byte()
            Try
                Using stream = x.OpenWebStream(url)
                    Return StoreAndHash(stream, destFilePath)
                End Using
            Catch ex As Exception
                Throw New DownloadException(url, ex)
            End Try
        End Function

        Protected Overrides Sub OnDoWork(e As DoWorkEventArgs)
            MyBase.OnDoWork(e)

            ' Create directory for update
            IO.Directory.CreateDirectory(x.tempUpdatePath)

            ' Download files
            For i = 0 To x.filesToUpdate.Count - 1
                Dim currentFile = x.filesToUpdate(i)
                ReportProgress((i + 1) * 100 \ x.filesToUpdate.Count, x.t.Translate("lblAktuelleDatei", currentFile.Name))
                Dim url = GetUpdateFileUri(x.currentServer, x.CurrentReleaseChannel, currentFile.Name)
                Dim outputFile = IO.Path.Combine(x.tempUpdatePath, currentFile.Name)

                Dim retryCount = 0
                Do
                    Dim hash = DownloadAndStoreFile(url, outputFile)
                    If CompareByteArray(hash, currentFile.Hash) Then
                        Continue For
                    End If

                    ' retry
                    If retryCount > 1 Then
                        Throw New DownloadException(url, New Exception("Checksum error"))
                    End If
                    IO.File.Delete(outputFile)
                    retryCount += 1
                Loop
            Next i
            ReportProgress(100, x.t.Translate("UpdateFertigstellen"))

            ' Store new versions file
            Try
                x.remoteVersionsFile.Save(x.UpdateTempVersionsFilePath)
            Catch ex As Exception
                Throw New Exception("Error storing version file " & ex.Message, ex)
            End Try

            Try
                Using writer As New IO.StreamWriter(IO.Path.Combine(x.tempUpdatePath, "UpdateInfo.txt"), False, Text.Encoding.UTF8)
                    writer.WriteLine(1)
                    writer.WriteLine(x.remoteVersionsFile.DisplayVersion)
                    For Each f In x.filesToDelete
                        writer.WriteLine(f)
                    Next
                End Using
            Catch ex As Exception
                Throw New Exception("Error storing UpdateInfo.txt file: " & ex.Message, ex)
            End Try

            ' Move Update.exe to parent directory
            Dim counter As Int32, tmpNeuFile As String
            Do
                counter += 1
                tmpNeuFile = IO.Path.Combine(x.tempUpdateBasePath, "Update-" & counter & ".exe")
            Loop While IO.File.Exists(tmpNeuFile)
            If IO.File.Exists(IO.Path.Combine(x.tempUpdatePath, "Update.exe")) Then
                Try
                    MoveFile(IO.Path.Combine(x.tempUpdatePath, "Update.exe"), tmpNeuFile, True)
                Catch ex As Exception
                    Throw New Exception("Error moving Update.exe" & ex.Message, ex)
                End Try
            Else
                Try
                    IO.File.Copy(IO.Path.Combine(Windows.Forms.Application.StartupPath, "Update.exe"), tmpNeuFile)
                Catch ex As Exception
                    Throw New Exception("Error copying Update.exe" & ex.Message, ex)
                End Try
            End If
        End Sub

        Private Shared Function GetParentPath(path As String) As String
            IO.Path.GetFullPath(path)
            Return IO.Path.GetDirectoryName(path.TrimEnd(New Char() {IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar}))
        End Function

        Private Sub MoveFile(sourceFileName As String, destinationFileName As String, overwrite As Boolean)
            If overwrite Then
                IO.File.Delete(destinationFileName)
            End If
            IO.Directory.CreateDirectory(GetParentPath(destinationFileName))
            IO.File.Move(sourceFileName, destinationFileName)
        End Sub

        Protected Overrides Sub OnProgressChanged(e As ProgressChangedEventArgs)
            MyBase.OnProgressChanged(e)

            If ui IsNot Nothing Then
                ui.Aktualisieren(CStr(e.UserState), e.ProgressPercentage)
            End If
        End Sub

        Protected Overrides Sub OnRunWorkerCompleted(e As RunWorkerCompletedEventArgs)
            MyBase.OnRunWorkerCompleted(e)

            If ui IsNot Nothing Then
                ui.Close()
                ui.Dispose()
            End If

            If e.Error IsNot Nothing Then
                Console.Error.WriteLine("Error updating: {0}", e.Error.Message)
                If ui IsNot Nothing Then x.RaiseUpdateErrorEvent(x.t.Translate("msgFehlerUpdate", Environment.NewLine & e.Error.Message), e.Error)
                x.SendStatistics(StatisticsTypes.UpdateError)
                x.isUpdating = False
                Return
            End If

            x.SendStatistics(StatisticsTypes.FoundAndInstalled)

            x.isUpdating = False
            x.RaiseUpdateDownloadedEvent()
        End Sub
    End Class

    Public Sub DownloadUpdateAsync(Optional withUI As Boolean = True)
        If isUpdating Then
            Return
        End If
        isUpdating = True

        If filesToUpdate Is Nothing OrElse filesToUpdate.Count = 0 Then
            Throw New Exception("Download update called, but nothing to download.")
        End If
        If IsUpdateDownloaded() Then
            ' An update was already downloaded
            If withUI Then RaiseUpdateErrorEvent(t.Translate("msgUpdateBereitsVorhanden", TranslatedProgramName), Nothing)
            SendStatistics(StatisticsTypes.UpdateError)
            isUpdating = False
            remoteVersionsFile = Nothing
            Return
        End If
        Dim w As New DownloadUpdateWorker(Me, withUI)
        w.RunWorkerAsync()
    End Sub

    ''' <summary>
    ''' Helper function to format versions in a nice way.
    ''' </summary>
    ''' <param name="version"></param>
    ''' <param name="showRevision"></param>
    ''' <returns></returns>
    Private Shared Function GetVersionsText(ByVal version As Version, Optional ByVal showRevision As Boolean = True) As String
        If showRevision AndAlso version.Revision > 0 Then
            Return version.ToString(4)
        End If
        If version.Build > 0 Then
            Return version.ToString(3)
        End If

        Return version.ToString(2)
    End Function

    ''' <summary>
    ''' Update the application version in the Uninstall section of the Windows registry.
    ''' HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$APPID
    ''' </summary>
    ''' <param name="appID"></param>
    ''' <param name="displayVersion"></param>
    ''' <returns></returns>
    Public Function SetUninstallInfoInRegistry(appID As String) As Boolean
        Return SetUninstallInfoInRegistry(appID, TranslatedProgramName + " " + GetVersionsText(programVersion, False), programVersion)
    End Function

    ''' <summary>
    ''' Update the application version in the Uninstall section of the Windows registry.
    ''' HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$APPID
    ''' </summary>
    ''' <param name="appID"></param>
    ''' <param name="displayName"></param>
    ''' <param name="displayVersion"></param>
    ''' <returns></returns>
    Private Shared Function SetUninstallInfoInRegistry(ByVal appID As String, ByVal displayName As String, ByVal displayVersion As Version) As Boolean
        Try
            Using readOnlyReg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" & appID, False)
                If readOnlyReg Is Nothing Then
                    Return False
                End If

                If CStr(readOnlyReg.GetValue("DisplayName")) = displayName AndAlso CStr(readOnlyReg.GetValue("DisplayVersion")) = GetVersionsText(displayVersion) Then
                    Return True
                End If
            End Using

            Using writableReg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" & appID, True)
                writableReg.SetValue("DisplayName", displayName)
                writableReg.SetValue("DisplayVersion", GetVersionsText(displayVersion))
                Return True
            End Using
        Catch
            Return False
        End Try
    End Function

    Private Shared Function StoreAndHash(stream As IO.Stream, outputFile As String) As Byte()
        Dim BUFFER_SIZE As Integer = 4096
        IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(outputFile))
        Dim buffer(BUFFER_SIZE - 1) As Byte

        Using x = Security.Cryptography.SHA256.Create()
            Using Writer As New IO.FileStream(outputFile, IO.FileMode.Create, IO.FileAccess.Write)
                While True
                    Dim bytesRead As Integer = stream.Read(buffer, 0, BUFFER_SIZE)
                    If bytesRead = 0 Then
                        Exit While
                    End If
                    x.TransformBlock(buffer, 0, bytesRead, Nothing, 0)
                    Writer.Write(buffer, 0, bytesRead)
                End While
            End Using
            x.TransformFinalBlock(buffer, 0, 0)
            Return x.Hash
        End Using
    End Function

    Private Shared Function CompareByteArray(a1 As Byte(), a2 As Byte()) As Boolean
        If a1.Length <> a2.Length Then
            Return False
        End If
        For i = 0 To a1.Length - 1
            If a1(i) <> a2(i) Then
                Return False
            End If
        Next
        Return True
    End Function

    Private Shared Sub Store(stream As IO.Stream, outputFile As String)
        Dim BUFFER_SIZE As Integer = 4096
        IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(outputFile))
        Dim buffer(BUFFER_SIZE - 1) As Byte

        Using Writer As New IO.FileStream(outputFile, IO.FileMode.Create, IO.FileAccess.Write)
            While True
                Dim bytesRead As Integer = stream.Read(buffer, 0, BUFFER_SIZE)
                If bytesRead = 0 Then
                    Exit While
                End If
                Writer.Write(buffer, 0, bytesRead)
            End While
        End Using
    End Sub

    Private Shared Function BuildQueryString(nvp As Specialized.NameValueCollection) As String
        Dim builder As New System.Text.StringBuilder()
        For Each v As String In nvp.AllKeys
            If builder.Length > 0 Then
                builder.Append("&")
            End If
            builder.Append(Uri.EscapeDataString(v)).Append("=").Append(Uri.EscapeDataString(nvp(v)))
        Next
        Return builder.ToString()
    End Function

    Private Enum StatisticsTypes
        NoUpdateAvailable
        FoundAndInstalled
        FoundAndNotInstalled
        UpdateError
    End Enum

    Private Function SendStatistics(type As StatisticsTypes) As Boolean
        Try
            Dim client As New Net.WebClient

            Dim query As New Specialized.NameValueCollection
            query("program") = programName.ToLowerInvariant
            query("version") = GetVersionsText(programVersion)
            If ProductFlavor <> Nothing Then
                query("flavor") = ProductFlavor
            End If
            query("type") = type.ToString()
            query("platform") = Environment.OSVersion.Platform.ToString()
            query("lang") = Threading.Thread.CurrentThread.CurrentCulture.Name
            If uid <> Nothing Then
                query("uid") = uid
            End If
            query("channel") = CurrentReleaseChannel
            Try
                query("netVersion") = GetVersionsText(HighestInstalledNetFramework)
            Catch ex As Exception
            End Try

            Dim ub As New UriBuilder(statisticsServerUri)
            ub.Query &= If(String.IsNullOrEmpty(ub.Query), "", "&") & BuildQueryString(query)
            Dim url = ub.Uri
            Try
                client.OpenRead(url).Close()
            Catch
                client.Proxy = Nothing
                client.OpenRead(url).Close()
            End Try
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Install a previously downloaded update.
    ''' Before updating, the Restarting event is raised.
    ''' </summary>
    ''' <returns>False if no update is available or can't be installed, otherwise exits the appliction.</returns>
    Public Function InstallUpdate() As Boolean
        If Not IsUpdateDownloaded() Then
            Return False
        End If

        Dim pi As New ProcessStartInfo
        Try
            IO.File.Create(IO.Path.Combine(programPath, "tmp.d" & (New Random).Next(0, 10)), 1, IO.FileOptions.DeleteOnClose).Close() 'Test ob Schreibrechte im Programmverzeichnis
        Catch
            ' No write permissions in program path
            ' Run as administrator
            If Environment.OSVersion.Platform = PlatformID.Win32NT AndAlso Environment.OSVersion.Version.Major >= 6 Then 'vista, win7/8/10
                pi.Verb = "runas"
            Else
                RaiseUpdateInfoEvent(t.Translate("msgUpdateInstallierenAdmin", TranslatedProgramName))
                Return False
            End If
        End Try

        RaiseEvent Restarting(Me, New EventArgs)

        Dim startupPath = Windows.Forms.Application.StartupPath

        pi.WorkingDirectory = startupPath
        Try
            Dim tmpneusteÄnderung As New Date(0)
            Dim UpdateProgrammEXE As String = String.Empty
            For Each file As String In IO.Directory.GetFiles(tempUpdateBasePath, "Update-*.exe")
                If IO.File.GetLastWriteTime(file) > tmpneusteÄnderung Then
                    tmpneusteÄnderung = IO.File.GetLastWriteTime(file)
                    UpdateProgrammEXE = file
                End If
            Next

            If Environment.OSVersion.Platform = PlatformID.Unix Then
                pi.FileName = "mono"
                pi.Arguments = String.Format("""{0}"" ""{1}"" ""{2}"" ""{3}""", UpdateProgrammEXE, programName, programExe, startupPath)
                pi.UseShellExecute = False
                Console.WriteLine(t.Translate("MonoUpdateHinweis", "cd """ & startupPath & """ && mono " & pi.Arguments))
                Try
                    Process.Start(pi)
                Catch ex As Exception
                    RaiseUpdateErrorEvent(t.Translate("FehlerAusführen", pi.FileName & " " & pi.Arguments, ex.Message), ex)
                End Try
            Else
                pi.FileName = """" & UpdateProgrammEXE & """"
                pi.Arguments = String.Format("""{0}"" ""{1}"" ""{2}""", programName, programExe, startupPath)
                Try
                    Process.Start(pi)
                Catch ex As Exception
                    RaiseUpdateErrorEvent(t.Translate("FehlerAusführen", pi.FileName & " " & pi.Arguments, ex.Message), ex)
                End Try
            End If
            Windows.Forms.Application.Exit()
            Return True
        Catch ex As Exception
            Console.Error.WriteLine("Error while trying to install update: {0}", ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Check if we are running under Mono runtime, instead of .Net Framework runtime
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function IsRunningOnMono() As Boolean
        Return (Type.GetType("Mono.Runtime") IsNot Nothing)
    End Function

    Private Sub SearchNewFiles()
        If remoteVersionsFile.Version <= localVersionsFile.Version Then
            filesToDelete = Nothing
            filesToUpdate = Nothing
            Return
        End If
        filesToUpdate = New List(Of File)
        Dim allRemoteFiles As New List(Of String) ' TODO make a set when using newer .net framework
        For Each c In remoteVersionsFile.Categories
            If Not c.IsMandatory AndAlso installedCategories IsNot Nothing AndAlso installedCategories.IndexOf(c.Name) = -1 Then
                ' category is not selected locally and is not mandatory
                Continue For
            End If

            Dim localCat As Category = localVersionsFile.GetCategory(c.Name)
            If localCat IsNot Nothing Then 'Category exists in local versions file
                ' Search updated files
                For Each f In c.Files
                    allRemoteFiles.Add(f.Name)

                    Dim localFile As File = localCat.GetFile(f.Name)
                    If localFile Is Nothing OrElse Not CompareByteArray(f.Hash, localFile.Hash) OrElse Not IO.File.Exists(IO.Path.Combine(programPath, f.Name)) Then
                        filesToUpdate.Add(f)
#If DEBUG Then
                        Console.Out.WriteLine("new: " + f.Name)
#End If
                    End If
                Next
            Else
                ' Update all files from this category
                filesToUpdate.AddRange(c.Files)
                For Each f In c.Files
                    allRemoteFiles.Add(f.Name)
#If DEBUG Then
                    Console.Out.WriteLine("new (category): " + f.Name)
#End If
                Next
            End If
        Next

        filesToDelete = New List(Of String)
        If remoteVersionsFile.FilesToDelete IsNot Nothing Then
            filesToDelete.AddRange(remoteVersionsFile.FilesToDelete)
        End If
        'Search for files that should be deleted
        Dim usesBackslash = IO.Path.DirectorySeparatorChar <> "/"
        IO.Directory.CreateDirectory(tempUpdateBasePath)
        Dim isCaseInsensitive = IO.Directory.Exists(tempUpdateBasePath.ToLower) AndAlso IO.Directory.Exists(tempUpdateBasePath.ToUpper)
        For Each c In localVersionsFile.Categories
            For Each f In c.Files
                If Not ContainsFileName(allRemoteFiles, f.Name, usesBackslash, isCaseInsensitive) Then
                    filesToDelete.Add(f.Name)
#If DEBUG Then
                    Console.Out.WriteLine("delete: " + f.Name)
#End If
                End If
            Next
        Next
    End Sub

    Private Function ContainsFileName(files As List(Of String), file As String, usesBackslash As Boolean, isCaseInsensitive As Boolean) As Boolean
        If Not usesBackslash AndAlso Not isCaseInsensitive Then
            Return files.Contains(file)
        End If
        ' Compare case insensitive if filesystem is case insensitive and ignore slash
        ' Otherwise an updated file could be unintentionally deleted, if the versions.json was faulty
        If isCaseInsensitive Then
            file = file.ToLowerInvariant
        End If
        If usesBackslash Then
            file = file.Replace(IO.Path.DirectorySeparatorChar, "/")
        End If
        For Each f In files
            If usesBackslash Then
                f = f.Replace(IO.Path.DirectorySeparatorChar, "/")
            End If
            If isCaseInsensitive Then
                f = f.ToLowerInvariant
            End If
            If String.CompareOrdinal(f, file) = 0 Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Show a window with a list of all installed updates.
    ''' </summary>
    ''' <param name="owner"></param>
    Sub ShowUpdateHistoryDialog(Optional owner As Windows.Forms.IWin32Window = Nothing)
        Using frmH As New frmUpdateHistory(t, programPath)
            If owner Is Nothing Then frmH.StartPosition = Windows.Forms.FormStartPosition.CenterScreen
            frmH.ShowDialog(owner)
        End Using
    End Sub

    Private Function OpenWebStream(uri As Uri) As IO.Stream
        Dim request As Net.HttpWebRequest = DirectCast(Net.WebRequest.Create(uri), Net.HttpWebRequest)
        request.AutomaticDecompression = Net.DecompressionMethods.GZip
        request.UserAgent = programName + "/" + programVersion.ToString(3) + " (" + Environment.OSVersion.Platform.ToString() + If(IsRunningOnMono(), ", Mono", "") + ")"
        Return request.GetResponse().GetResponseStream()
    End Function
End Class
