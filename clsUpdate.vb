Imports System.Diagnostics
Imports System.Windows.Forms

Public Class Update
    Dim updateServers As New List(Of String)
    Dim updateServersFile As String
    Dim tempUpdatePath As String

    Dim installedCategories As List(Of String)
    Dim filesToUpdate As List(Of Datei)
    Dim currentServer As String

    Dim programName, programExe, programPath As String
    Dim programVersion As Version
    Dim uid As String

    ''' <summary>
    ''' Optionally set the product flavor, i.e. Normal and Portable.
    ''' This is only used for statistics.
    ''' </summary>
    ''' <returns></returns>
    Public Property ProductFlavor As String

    Dim isUpdating As Boolean

    Dim Übersetzen As New TranslationLib.Translation(String.Empty, My.Resources.English)

    ''' <summary>
    ''' This event is raised just before restarting to install an update.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Event Restarting(sender As Object, e As EventArgs)


    Dim statisticsServerUri As Uri
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

    Dim _translatedProgramName As String
    Private Property TranslatedProgramName As String
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

    Sub New(programName As String, programExe As String, programVersion As Version, programPath As String, tempUpdatePath As String, updateServersFile As String, updateServers As ICollection(Of String), ByVal uid As String)
        Me.programExe = programExe
        Me.programName = programName
        Me.programVersion = programVersion
        Me.programPath = programPath
        Me.uid = uid
        Me.updateServersFile = updateServersFile
        Me.updateServers.AddRange(updateServers)
        Me.tempUpdatePath = IO.Path.Combine(tempUpdatePath, "Update")

        'Sprachen laden
        Übersetzen.AddLanguage("German", "Deutsch", My.Resources.German)
        Übersetzen.AddLanguage("English", "English", My.Resources.English)
        Übersetzen.AddLanguage("French", "Français", My.Resources.French)
        Übersetzen.AddLanguage("Spanish", "Español", My.Resources.Spanish)
        Übersetzen.AddLanguage("Bavarian", "Boarisch", My.Resources.Bavarian)
        Übersetzen.AddLanguage("Dutch", "Nederlands", My.Resources.Dutch)
        Übersetzen.AddLanguage("Portuguese", "Português", My.Resources.Portuguese)
        Übersetzen.AddLanguage("Polish", "Polski", My.Resources.Polish)
        Übersetzen.AddLanguage("Chinese", "汉语", My.Resources.Chinese)
        Übersetzen.AddLanguage("Serbian", "Srpski", My.Resources.Serbian)
        Übersetzen.AddLanguage("Greek", "Ελληνικά", My.Resources.Greek)
        Übersetzen.AddLanguage("Bulgarian", "Български", My.Resources.Bulgarian)
        Übersetzen.AddLanguage("Danish", "Dansk", My.Resources.Danish)

        ' Set default language
        Übersetzen.Load(Übersetzen.CheckLanguageName(String.Empty))
    End Sub

    ''' <summary>
    ''' Set the language to be used for translating messages.
    ''' </summary>
    ''' <param name="language">English language name</param>
    ''' <param name="translatedProgramName">The program name translated to <paramref name="language"/>language</param>
    Sub Translate(ByVal language As String, ByVal translatedProgramName As String)
        translatedProgramName = translatedProgramName
        language = Übersetzen.CheckLanguageName(language)
        Übersetzen.Load(language)
    End Sub

    Sub UpdateSearchAndInstall(Optional showErrors As Object = True) 'zeigefehler as object um in eigenem thread zu starten
        UpdateSearchAndInstall(CBool(showErrors))
    End Sub

    ''' <summary>
    ''' Search for updates and if available, ask if it should be installed and install it
    ''' </summary>
    ''' <remarks></remarks>
    Sub UpdateSearchAndInstall(Optional showErrors As Boolean = True) 'zeigefehler as object um in eigenem thread zu starten
        If isUpdating Then
            Return
        End If

        Try
            isUpdating = True

            Dim tmpUpdateSuche As String = SearchUpdate(showErrors)
            If tmpUpdateSuche = "XXX" Then 'Update fehler oder update schon heruntergeladen
                Return
            End If

            If String.IsNullOrEmpty(tmpUpdateSuche) Then
                SendStatistics("gesucht nicht gefunden")
                If showErrors Then MessageBox.Show(Übersetzen.Translate("msgKeinUpdate"), Übersetzen.Translate("Update", TranslatedProgramName), MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            If DialogResult.Yes <> MessageBox.Show(Übersetzen.Translate("msgUpdateVorhanden", tmpUpdateSuche, Environment.NewLine), Übersetzen.Translate("Update", TranslatedProgramName), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) Then
                SendStatistics("gesucht gefunden nicht installiert")
                Return
            End If

            Try
                If Not DownloadUpdate(True) Then
                    SendStatistics("gesucht gefunden installieren fehler")
                    Return
                End If

                SendStatistics("gesucht gefunden installiert")
                If DialogResult.Yes = MessageBox.Show(Übersetzen.Translate("msgUpdateErfolgreich", Environment.NewLine, TranslatedProgramName), Übersetzen.Translate("Update", TranslatedProgramName), MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) Then
                    InstallUpdate()
                End If
            Catch ex As Exception
                SendStatistics("gesucht gefunden installieren fehler")
                MessageBox.Show(ex.Message)
            End Try
        Finally
            isUpdating = False
        End Try
    End Sub

    Private Sub ReadUpdateServersFile()
        Using UpdateReader As New IO.StreamReader(updateServersFile, True)
            For Each s In UpdateReader.ReadToEnd.Split(New Char() {Convert.ToChar(10), Convert.ToChar(13)}, StringSplitOptions.RemoveEmptyEntries)
                If Not updateServers.Contains(s) Then
                    updateServers.Add(s)
                End If
            Next
        End Using
    End Sub

    Private Sub ReadCategoriesFile()
        installedCategories = New List(Of String)
        'Installierte Kategorien rausfinden
        If Not IO.File.Exists(IO.Path.Combine(programPath, "Kategorien.ini")) Then
            Exit Sub
        End If
        Using Reader As New IO.StreamReader(IO.Path.Combine(programPath, "Kategorien.ini"), True)
            Do
                Dim line = Reader.ReadLine
                If line Is Nothing Then
                    Exit Do
                End If

                Dim index = line.IndexOf("="c)
                If index > -1 AndAlso String.Compare(line.Substring(index + 1).Trim, "true", StringComparison.OrdinalIgnoreCase) = 0 Then
                    installedCategories.Add(line.Substring(0, index).Trim.ToLowerInvariant)
                End If
            Loop
        End Using
    End Sub

    ''' <summary>
    ''' Sucht nach Updates
    ''' </summary>
    ''' <param name="ZeigeFehler">Ob fehler angezeigt werden sollen</param>
    ''' <returns>Gibt die neuere Version des Updates zurück andernfalls string.empty</returns>
    ''' <remarks></remarks>
    Private Function SearchUpdate(Optional ByVal ZeigeFehler As Boolean = True) As String
        If IO.File.Exists(IO.Path.Combine(tempUpdatePath, "Versionen.lst")) AndAlso IO.Directory.GetFiles(IO.Path.Combine(tempUpdatePath, ".."), "Update-*.exe").Length > 0 Then
            'bereits ein Update vorhanden
            If ZeigeFehler Then MessageBox.Show(Übersetzen.Translate("msgUpdateBereitsVorhanden", Environment.NewLine, TranslatedProgramName), Übersetzen.Translate("Update", TranslatedProgramName), MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return "XXX"
        End If

        Try
            ReadUpdateServersFile()
        Catch
        End Try

        If installedCategories Is Nothing Then
            ReadCategoriesFile()
        End If

        Dim LokaleVersionen, UpdateVersionen As VersionenDatei

        Try
            LokaleVersionen = New VersionenDatei()
            LokaleVersionen.Öffnen(IO.Path.Combine(programPath, "Versionen.lst"), True) 'Lokale Versionen datei öffnen
        Catch ex As Exception
            Console.Error.WriteLine("Versionen.lst: " & ex.Message)
        End Try
        If Not String.IsNullOrEmpty(LokaleVersionen.Version) Then
            Try
                programVersion = New Version(LokaleVersionen.Version)
            Catch
            End Try
        End If
        Dim tmpFehler As String = String.Empty
        'Update versionsdatei öffnen:
        For Each server In updateServers
            Try
                UpdateVersionen = New VersionenDatei()
                UpdateVersionen.Öffnen(server & "Versionen.lst.kom", False)
                currentServer = server
                Exit For
            Catch ex As Exception
                Console.Error.WriteLine(server & ": " & ex.Message)
                tmpFehler &= server & ": " & ex.Message & Environment.NewLine
                UpdateVersionen = Nothing
            End Try
        Next
        If UpdateVersionen Is Nothing Then
            If ZeigeFehler Then MessageBox.Show(Übersetzen.Translate("msgFehlerUpdateSuchen", Environment.NewLine & tmpFehler), Übersetzen.Translate("Update", TranslatedProgramName), MessageBoxButtons.OK, MessageBoxIcon.Error)
            SendStatistics("gesucht gefunden installieren fehler")
            Return "XXX"
        End If

        filesToUpdate = SearchNewFiles(LokaleVersionen, UpdateVersionen)
        If filesToUpdate.Count > 0 Then 'Update vorhanden
            Return UpdateVersionen.Version
        Else
            Return String.Empty
        End If
    End Function

    Private Function DownloadUpdate(withUI As Boolean) As Boolean
        If filesToUpdate.Count = 0 Then
            Return False
        End If
        If IO.File.Exists(IO.Path.Combine(tempUpdatePath, "Versionen.lst")) AndAlso IO.Directory.GetFiles(IO.Path.Combine(tempUpdatePath, ".."), "Update-*.exe").Length > 0 Then
            'bereits ein Update vorhanden
            If withUI Then MessageBox.Show(Übersetzen.Translate("msgUpdateBereitsVorhanden", Environment.NewLine, TranslatedProgramName), Übersetzen.Translate("Update", TranslatedProgramName), MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Try
            Using tmpUpdateForm As New frmUpdate
                Using Client As New System.Net.WebClient()
                    IO.Directory.CreateDirectory(tempUpdatePath)  'Verzeichnis für Update erstellen
                    If withUI Then tmpUpdateForm.Show()
                    For i As Int32 = 0 To filesToUpdate.Count - 1 'Dateien herunterladen
                        If withUI Then tmpUpdateForm.Aktualisieren(Übersetzen.Translate("lblAktuelleDatei", filesToUpdate(i).Name), i, filesToUpdate.Count - 1, Übersetzen.Translate("Update", TranslatedProgramName))
                        Application.DoEvents()
                        Dim stream As IO.Stream = Nothing
                        Try
                            Dim url = currentServer & Uri.EscapeDataString(filesToUpdate(i).Name) & ".kom"
                            Try
                                stream = Client.OpenRead(url)
                            Catch
                                Client.Proxy = Nothing
                                stream = Client.OpenRead(url)
                            End Try
                            Decompress(stream, IO.Path.Combine(tempUpdatePath, filesToUpdate(i).Name))
                        Catch ex As Exception
                            Console.Error.WriteLine(currentServer & filesToUpdate(i).Name & ": " & ex.Message)
                            If withUI Then MessageBox.Show(ex.Message, Übersetzen.Translate("Update", TranslatedProgramName), MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Finally
                            If stream IsNot Nothing Then
                                stream.Close()
                            End If
                        End Try
                    Next i
                    If withUI Then tmpUpdateForm.Aktualisieren(Übersetzen.Translate("UpdateFertigstellen"), 1, 1, Übersetzen.Translate("Update", TranslatedProgramName))
                    Application.DoEvents()
                    'Versionen.lst herunterladen
                    Decompress(Client.OpenRead(currentServer & "Versionen.lst.kom"), IO.Path.Combine(tempUpdatePath, "Versionen.lst"))
                End Using

                ' Update.exe verschieben ' TODO update.exe in apppath oder im update ordner hernehmen
                Dim t As Int32, tmpNeuFile As String
                Do
                    t += 1
                    tmpNeuFile = IO.Path.Combine(IO.Path.Combine(tempUpdatePath, ".."), "Update-" & t & ".exe")
                Loop While IO.File.Exists(tmpNeuFile)
                If IO.File.Exists(IO.Path.Combine(tempUpdatePath, "Update.exe")) Then
                    Try
                        My.Computer.FileSystem.MoveFile(IO.Path.Combine(tempUpdatePath, "Update.exe"), tmpNeuFile, True)
                    Catch ex As Exception
                        Console.Error.WriteLine("Error moving Update.exe : " & ex.Message)
                        If withUI Then MessageBox.Show(ex.Message, Übersetzen.Translate("Update", TranslatedProgramName), MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End Try
                Else
                    Try
                        IO.File.Copy(IO.Path.Combine(Application.StartupPath, "Update.exe"), tmpNeuFile)
                    Catch ex As Exception
                        Console.Error.WriteLine("Error copying Update.exe : " & ex.Message)
                        If withUI Then MessageBox.Show(ex.Message, Übersetzen.Translate("Update", TranslatedProgramName), MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End Try
                End If
            End Using
            Return True
        Catch ex As Exception 'Fehler beim Update herunterladen
            If withUI Then MessageBox.Show(Übersetzen.Translate("msgFehlerUpdate", Environment.NewLine & ex.Message), Übersetzen.Translate("Update", TranslatedProgramName), MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Helper function to format versions in a nice way.
    ''' </summary>
    ''' <param name="version"></param>
    ''' <param name="showRevision"></param>
    ''' <returns></returns>
    Private Shared Function GetVersionsText(ByVal version As Version, Optional ByVal showRevision As Boolean = True) As String
        If showRevision AndAlso version.Revision <> 0 Then
            Return version.ToString(4)
        End If
        If version.Build <> 0 Then
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
        Return SetUninstallInfoInRegistry(appID, TranslatedProgramName + GetVersionsText(programVersion, False), programVersion)
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

    Private Shared Sub Decompress(ByVal Stream As IO.Stream, ByVal DateiNach As String)
        Dim BUFFER_SIZE As Integer = 4096
        Static UseGUnzip As Boolean = True
        IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(DateiNach))
        Dim buffer(BUFFER_SIZE - 1) As Byte
        If UseGUnzip Then
            Try
                Dim tmpProcess As New Process
                tmpProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                tmpProcess.StartInfo.FileName = "gunzip"
                tmpProcess.StartInfo.UseShellExecute = False
                tmpProcess.Start() 'test ob gunzip verfügbar

                Dim tmpName As String = IO.Path.GetTempFileName
                Using Writer As New IO.FileStream(tmpName & ".gz", IO.FileMode.Create, IO.FileAccess.Write)
                    While True
                        Dim bytesRead As Integer = Stream.Read(buffer, 0, BUFFER_SIZE)
                        If bytesRead = 0 Then
                            Exit While
                        End If
                        Writer.Write(buffer, 0, bytesRead)
                    End While
                End Using
                tmpProcess = New Process
                tmpProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                tmpProcess.StartInfo.Arguments = "-qf """ & tmpName & ".gz" & """"
                tmpProcess.StartInfo.FileName = "gunzip"
                tmpProcess.Start()
                tmpProcess.WaitForExit(5000)

                My.Computer.FileSystem.MoveFile(tmpName, DateiNach, True)
                Stream.Close()
                Exit Sub
            Catch
                UseGUnzip = False
            End Try
        End If

        Using Gzip As New IO.Compression.GZipStream(Stream, IO.Compression.CompressionMode.Decompress)
            Using Writer As New IO.FileStream(DateiNach, IO.FileMode.Create, IO.FileAccess.Write)
                While True
                    Dim bytesRead As Integer = Gzip.Read(buffer, 0, BUFFER_SIZE)
                    If bytesRead = 0 Then
                        Exit While
                    End If
                    Writer.Write(buffer, 0, bytesRead)
                End While
            End Using
        End Using 'gzip
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

    Private Function SendStatistics(type As String) As Boolean
        Try
            Dim client As New Net.WebClient

            Dim query As New Specialized.NameValueCollection
            query("programm") = programName.ToLowerInvariant
            query("version") = GetVersionsText(programVersion)
            If ProductFlavor <> Nothing Then
                query("pn") = ProductFlavor
            End If
            query("typ") = type
            query("platform") = My.Computer.Info.OSPlatform
            query("lang") = My.Application.Culture.Name
            If uid <> Nothing Then
                query("dat") = uid
            End If

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
    ''' </summary>
    ''' <returns>True if update is available, false otherwise</returns>
    Public Function InstallUpdate() As Boolean
        If Not IO.File.Exists(IO.Path.Combine(tempUpdatePath, "Versionen.lst")) OrElse IO.Directory.GetFiles(IO.Path.Combine(tempUpdatePath, ".."), "Update-*.exe").Length = 0 Then
            Return False
        End If

        Dim pi As New System.Diagnostics.ProcessStartInfo
        Try
            IO.File.Create(IO.Path.Combine(programPath, "tmp.d" & (New Random).Next(0, 10)), 1, IO.FileOptions.DeleteOnClose).Close() 'Test ob Schreibrechte im Programmverzeichnis
        Catch 'wenn keine Schreibrechte im Programmverzeichnis
            If Environment.OSVersion.Platform = PlatformID.Win32NT AndAlso Environment.OSVersion.Version.Major >= 6 Then 'vista, win7
                pi.Verb = "runas"
            Else
                MessageBox.Show(Übersetzen.Translate("msgUpdateInstallierenAdmin", TranslatedProgramName), Übersetzen.Translate("Update", TranslatedProgramName), MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return False
            End If
        End Try

        RaiseEvent Restarting(Me, New EventArgs)

        pi.WorkingDirectory = Application.StartupPath
        Try
            Dim tmpneusteÄnderung As New Date(0), tmpDatei As String = String.Empty
            For Each file As String In IO.Directory.GetFiles(IO.Path.Combine(tempUpdatePath, ".."), "Update-*.exe")
                If IO.File.GetLastWriteTime(file) > tmpneusteÄnderung Then
                    tmpneusteÄnderung = IO.File.GetLastWriteTime(file)
                    tmpDatei = file
                End If
            Next

            Dim UpdateProgrammEXE As String = """" & tmpDatei & """"
            If Environment.OSVersion.Platform = PlatformID.Unix Then
                pi.FileName = "mono"
                pi.Arguments = UpdateProgrammEXE & " """ & programName & """ """ & programExe & """"
                pi.UseShellExecute = False
                Console.WriteLine(Übersetzen.Translate("MonoUpdateHinweis", "cd """ & Application.StartupPath & """ && mono " & pi.Arguments))
                Try
                    Process.Start(pi)
                Catch ex As Exception
                    MessageBox.Show(Übersetzen.Translate("FehlerAusführen", pi.FileName & " " & pi.Arguments, ex.Message))
                End Try
            Else
                pi.FileName = UpdateProgrammEXE
                pi.Arguments = """" & programName & """ """ & programExe & """"
                Try
                    Process.Start(pi)
                Catch ex As Exception
                    MessageBox.Show(Übersetzen.Translate("FehlerAusführen", pi.FileName & " " & pi.Arguments, ex.Message))
                End Try
            End If
            Application.Exit()
            Return True
        Catch
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

    Private Function SearchNewFiles(ByVal LokaleVersionen As VersionenDatei, ByVal UpdateVersionen As VersionenDatei) As List(Of Datei)
        Dim result As New List(Of Datei)
        If UpdateVersionen.InterneVersion <= LokaleVersionen.InterneVersion Then
            Return result
        End If
        For Each c In UpdateVersionen.Kategorien.Values
            If Not c.Pflicht AndAlso installedCategories IsNot Nothing AndAlso installedCategories.IndexOf(c.Name) > -1 Then
                Continue For
            End If

            Dim localCat As Kategorie
            If LokaleVersionen.Kategorien.TryGetValue(c.Name, localCat) Then 'Lokale Versionen sind vorhanden
                '=> neuere Versionen suchen
                For Each f In c.Dateien.Values
                    Dim localFile As Datei
                    If Not localCat.Dateien.TryGetValue(f.Name, localFile) Then
                        Continue For
                    End If

                    If f.InterneVersion > localFile.InterneVersion OrElse Not IO.File.Exists(IO.Path.Combine(programPath, f.Name)) Then
                        result.Add(f)
                    End If
                Next
            Else 'Lokale Versionen zu dieser Kategorie sind nicht vorhanden
                '=> alle aus dieser Kategorie aktualisieren
                result.AddRange(c.Dateien.Values)
            End If
        Next
        Return result
    End Function

    ''' <summary>
    ''' Show a window with a list of all installed updates.
    ''' </summary>
    ''' <param name="owner"></param>
    Sub ShowUpdateHistoryDialog(Optional owner As IWin32Window = Nothing)
        Using frmH As New frmUpdateHistory(Übersetzen, programPath)
            If owner Is Nothing Then frmH.StartPosition = FormStartPosition.CenterScreen
            frmH.ShowDialog(owner)
        End Using
    End Sub

    Private Class VersionenDatei
        Friend Version As String, InterneVersion As Int32
        Dim ReleasNotesUrl As String
        Friend Kategorien As New Dictionary(Of String, Kategorie)

        Sub Öffnen(ByVal Datei As String, ByVal IstLokal As Boolean)
            Dim Stream As IO.Stream
            If IstLokal Then
                Stream = New IO.FileStream(Datei, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
            Else
                Dim Client As New System.Net.WebClient
                Try
                    Stream = New IO.Compression.GZipStream(Client.OpenRead(Datei), IO.Compression.CompressionMode.Decompress)
                Catch
                    Client.Proxy = Nothing
                    Stream = New IO.Compression.GZipStream(Client.OpenRead(Datei), IO.Compression.CompressionMode.Decompress)
                End Try
            End If
            Using Stream
                Öffnen(Stream)
            End Using
        End Sub

        Sub Öffnen(stream As IO.Stream)
            Using Reader As New IO.StreamReader(stream, True)
                Reader.ReadLine() 'UpdateVersion
                Version = Reader.ReadLine
                InterneVersion = CInt(Reader.ReadLine)
                ReleasNotesUrl = Reader.ReadLine

                Dim currentCategory As Kategorie
                Do
                    Dim line = Reader.ReadLine
                    If line Is Nothing Then
                        Exit Do
                    End If

                    If line.Length > 1 AndAlso line.Substring(0, 2) = "K:" Then
                        Dim pos = line.IndexOf(":"c, 2)
                        currentCategory = New Kategorie(line.Substring(2, pos - 2), CBool(line.Substring(pos + 1)))
                        Kategorien.Add(currentCategory.Name, currentCategory)
                    ElseIf line.Length > 1 AndAlso line.Substring(0, 2) = "D:" Then
                        currentCategory = Nothing
                    ElseIf currentCategory Is Nothing Then
                        'Dateien zum Löschen
                        'Löschen macht Update.exe
                    Else
                        'Dateien in Kategorien
                        Dim newFile = New Datei(line, CInt(Reader.ReadLine))
                        currentCategory.Dateien.Add(newFile.Name, newFile)
                    End If
                Loop
            End Using
        End Sub
    End Class

    Private Class Kategorie
        Friend Name As String
        Friend Pflicht As Boolean
        Friend Dateien As New Dictionary(Of String, Datei)

        Sub New(ByVal Name As String, ByVal Pflicht As Boolean)
            Me.Name = Name
            Me.Pflicht = Pflicht
        End Sub
    End Class

    Private Class Datei
        Friend Name As String, InterneVersion As Int32

        Sub New(ByVal Name As String, ByVal InterneVersion As Integer)
            Me.Name = Name.Replace("\"c, "/"c)
            Me.InterneVersion = InterneVersion
        End Sub
    End Class
End Class
