Public Class Update
    Dim UpdateServer() As String
    Dim InstallierteKategorien() As String
    Dim Übersetzen As New TranslationLib.clsÜbersetzen(String.Empty, My.Resources.English)

    Dim ÜbersetzterProgrammName As String

    Event Neustarten(ByVal Manual As System.Threading.ManualResetEvent)

    Dim ZuAktualisierendeDateien As Dateien, AktuellerServer As String
    Dim ProgrammName, ProgrammExe, ProgrammPfad, ProgrammVersion As String ', ProgrammSprache
    Public GeradeUpdaten As Boolean
    ''' <summary>
    ''' Gibt an ob Updatesuchen/Update Information zu Statistikzwecken an Google Analytics gesendet werden sollen
    ''' </summary>
    ''' <remarks></remarks>
    Public GoogleStatistik As Boolean = True
    Public Delegate Sub UpdateSuchenInstallierenCallback(ByVal ZeigeFehler As Boolean, ByVal MitUI As Boolean)
    Dim UpdatePfad As String
    Public Daten As String
    Dim UpdateServerDatei As String, StandardUpdateServer() As String
    Dim Portable As Boolean

    Sub New(ByVal Name As String, ByVal Exe As String, ByVal Version As String, ByVal Pfad As String, ByVal UpdateServerDatei As String, ByVal StandardUpdateServer() As String, ByVal Daten As String)
        ProgrammPfad = Pfad
        ProgrammName = Name
        ProgrammExe = Exe
        ProgrammVersion = Version
        Me.Daten = Daten
        Me.UpdateServerDatei = UpdateServerDatei
        Me.StandardUpdateServer = StandardUpdateServer
        'UpdatePfad festlegen
        Portable = IO.File.Exists(IO.Path.Combine(ProgrammPfad, "Portable"))
        If Portable Then
            UpdatePfad = IO.Path.Combine(ProgrammPfad, "Update")
        Else
            UpdatePfad = IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Flo & Seb Engineering" & IO.Path.DirectorySeparatorChar & ProgrammName & IO.Path.DirectorySeparatorChar & "Update")
        End If
        'Sprachen laden
        Übersetzen.Sprachen.Add("German", "Deutsch", My.Resources.German)
        Übersetzen.Sprachen.Add("English", "English", My.Resources.English)
        Übersetzen.Sprachen.Add("French", "Français", My.Resources.French)
        Übersetzen.Sprachen.Add("Spanish", "Español", My.Resources.Spanish)
        Übersetzen.Sprachen.Add("Bavarian", "Boarisch", My.Resources.Bavarian)
        Übersetzen.Sprachen.Add("Dutch", "Nederlands", My.Resources.Dutch)
        Übersetzen.Sprachen.Add("Portuguese", "Português", My.Resources.Portuguese)
        Übersetzen.Sprachen.Add("Polish", "Polski", My.Resources.Polish)
        Übersetzen.Sprachen.Add("Chinese", "汉语", My.Resources.Chinese)
    End Sub

    Sub Übersetze(ByVal Sprache As String, ByVal ÜbersetzterName As String)
        ÜbersetzterProgrammName = ÜbersetzterName
        Sprache = Übersetzen.ÜberprüfeSprache(Sprache)
        Übersetzen.Load(Sprache)
    End Sub

    ''' <summary>
    ''' Sucht nach Updates und fragt fals vorhanden, ob es installiert werden soll.
    ''' </summary>
    ''' <remarks></remarks>
    Sub UpdateSuchenInstallieren(Optional ByVal ZeigeFehler As Object = True) 'zeigefehler as object um in eigenem thread zu starten
        If Not GeradeUpdaten Then
            GeradeUpdaten = True
            Dim zZeigeFehler As Boolean = CBool(ZeigeFehler)
            Dim tmpUpdateSuche As String = SucheUpdate(zZeigeFehler)
            If tmpUpdateSuche = "XXX" Then 'Sucht Schon Update, oder Update fehler oder update schon heruntergeladen
                GeradeUpdaten = False
                Exit Sub
            ElseIf Not String.IsNullOrEmpty(tmpUpdateSuche) Then 'Update vorhanden
                Try
                    'Updatefrage
                    If MessageBox.Show(Übersetzen.Übersetze("msgUpdateVorhanden", tmpUpdateSuche, Environment.NewLine), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = DialogResult.Yes Then
                        If LadeUpdate(True) Then
                            If GoogleStatistik Then
                                Dim tmpString As String : If Portable Then tmpString = "Portable" Else tmpString = "Normal"
                                If Not SendeStatistik(ProgrammName.ToLowerInvariant, ProgrammVersion, tmpString, "gesucht gefunden installiert") Then
                                    SendeAnGoogle("/updates/" & ProgrammName.ToLowerInvariant & "/updatesuchen.htm", String.Empty, "Vorhanden", tmpString, "de", ProgrammVersion, ProgrammName & "UpdateSuche", "update.mal-was-anderes.de", "UA-2276175-1")
                                    SendeAnGoogle("/updates/" & ProgrammName.ToLowerInvariant & "/update.htm", String.Empty, "Updaten", tmpString, "de", ProgrammVersion, ProgrammName & "Update", "update.mal-was-anderes.de", "UA-2276175-1")
                                End If
                            End If
                            If MessageBox.Show(Übersetzen.Übersetze("msgUpdateErfolgreich", Environment.NewLine, ÜbersetzterProgrammName), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                                InstalliereUpdate(True)
                            End If
                        Else 'laden fehlgeschlagen
                            If GoogleStatistik Then
                                Dim tmpString As String : If Portable Then tmpString = "Portable" Else tmpString = "Normal"
                                If Not SendeStatistik(ProgrammName.ToLowerInvariant, ProgrammVersion, tmpString, "gesucht gefunden installieren fehler") Then
                                    SendeAnGoogle("/updates/" & ProgrammName.ToLowerInvariant & "/updatesuchen.htm", String.Empty, "Vorhanden", tmpString, "de", ProgrammVersion, ProgrammName & "UpdateSuche", "update.mal-was-anderes.de", "UA-2276175-1")
                                End If
                            End If
                        End If
                    Else
                        If GoogleStatistik Then
                            Dim tmpString As String : If Portable Then tmpString = "Portable" Else tmpString = "Normal"
                            If Not SendeStatistik(ProgrammName.ToLowerInvariant, ProgrammVersion, tmpString, "gesucht gefunden nicht installiert") Then
                                SendeAnGoogle("/updates/" & ProgrammName.ToLowerInvariant & "/updatesuchen.htm", String.Empty, "Vorhanden", tmpString, "de", ProgrammVersion, ProgrammName & "UpdateSuche", "update.mal-was-anderes.de", "UA-2276175-1")
                            End If
                        End If
                    End If
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            Else 'Kein Update vorhanden
                'Statistik:
                Dim tmpString As String : If Portable Then tmpString = "portable" Else tmpString = "normal"
                If GoogleStatistik Then
                    If Not SendeStatistik(ProgrammName.ToLowerInvariant, ProgrammVersion, tmpString, "gesucht nicht gefunden") Then
                        SendeAnGoogle("/updates/" & ProgrammName.ToLowerInvariant & "/updatesuchen.htm", String.Empty, "KeinsVorhanden", tmpString, "de", ProgrammVersion, ProgrammName & "UpdateSuche", "update.mal-was-anderes.de", "UA-2276175-1")
                    End If
                End If
                If zZeigeFehler Then MessageBox.Show(Übersetzen.Übersetze("msgKeinUpdate"), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
            GeradeUpdaten = False
        End If
    End Sub

    ''' <summary>
    ''' Sucht nach Updates
    ''' </summary>
    ''' <param name="ZeigeFehler">Ob fehler angezeigt werden sollen</param>
    ''' <returns>Gibt die neuere Version des Updates zurück andernfalls string.empty</returns>
    ''' <remarks></remarks>
    Private Function SucheUpdate(Optional ByVal ZeigeFehler As Boolean = True) As String
        If IO.File.Exists(IO.Path.Combine(UpdatePfad, "Versionen.lst")) AndAlso IO.Directory.GetFiles(IO.Path.Combine(UpdatePfad, ".."), "Update-*.exe").Length > 0 Then
            'bereits ein Update vorhanden
            If ZeigeFehler Then MessageBox.Show(Übersetzen.Übersetze("msgUpdateBereitsVorhanden", Environment.NewLine, ÜbersetzterProgrammName), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return "XXX"
        Else
            GeradeUpdaten = True
            If UpdateServer Is Nothing Then
                'UpdateServer setzen
                Try
                    Using UpdateReader As New IO.StreamReader(UpdateServerDatei, True)
                        UpdateServer = UpdateReader.ReadToEnd.Split(New Char() {ChrW(10), ChrW(13)}, StringSplitOptions.RemoveEmptyEntries)
                    End Using
                    If UpdateServer.Length = 0 Then
                        UpdateServer = StandardUpdateServer
                    End If
                Catch
                    UpdateServer = StandardUpdateServer
                End Try
                'Installierte Kategorien rausfinden
                If IO.File.Exists(IO.Path.Combine(ProgrammPfad, "Kategorien.ini")) Then
                    Dim tmp As String
                    Using Reader As New IO.StreamReader(IO.Path.Combine(ProgrammPfad, "Kategorien.ini"), True)
                        Do Until Reader.Peek = -1
                            tmp = Reader.ReadLine
                            If tmp.IndexOf("="c) > -1 AndAlso String.Compare(tmp.Substring(tmp.IndexOf("="c) + 1).Trim, "true", StringComparison.OrdinalIgnoreCase) = 0 Then
                                If InstallierteKategorien Is Nothing Then ReDim InstallierteKategorien(0) Else ReDim Preserve InstallierteKategorien(InstallierteKategorien.Length)
                                InstallierteKategorien(InstallierteKategorien.GetUpperBound(0)) = tmp.Substring(0, tmp.IndexOf("="c)).Trim.ToLowerInvariant
                            End If
                        Loop
                    End Using
                End If
            End If

            Dim LokaleVersionen, UpdateVersionen As New VersionenDatei

            Try
                LokaleVersionen.Öffnen(IO.Path.Combine(ProgrammPfad, "Versionen.lst"), True) 'Lokale Versionen datei öffnen
            Catch ex As Exception
                Console.Error.WriteLine("Versionen.lst: " & ex.Message)
            End Try
            If Not String.IsNullOrEmpty(LokaleVersionen.Version) Then ProgrammVersion = LokaleVersionen.Version
            Dim tmpFehler As String = String.Empty
            'Update versionsdatei öffnen:
            For i As Int32 = 0 To UpdateServer.GetUpperBound(0)
                Try
                    UpdateVersionen.Öffnen(UpdateServer(i) & "Versionen.lst.kom", False)
                    AktuellerServer = UpdateServer(i)
                    GoTo Suche
                Catch ex As Exception
                    Console.Error.WriteLine(UpdateServer(i) & ": " & ex.Message)
                    tmpFehler &= UpdateServer(i) & ": " & ex.Message & Environment.NewLine
                End Try
            Next i
            If ZeigeFehler Then MessageBox.Show(Übersetzen.Übersetze("msgFehlerUpdateSuchen", Environment.NewLine & tmpFehler), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Error)
            If GoogleStatistik Then
                Dim tmpString As String : If Portable Then tmpString = "Portable" Else tmpString = "Normal"
                If Not SendeStatistik(ProgrammName.ToLowerInvariant, ProgrammVersion, tmpString, "gesucht gefunden installieren fehler") Then
                    SendeAnGoogle("/updates/" & ProgrammName.ToLowerInvariant & "/updatefehler.htm", String.Empty, "Vorhanden", tmpString, "de", ProgrammVersion, ProgrammName & "UpdateSuche", "update.mal-was-anderes.de", "UA-2276175-1")
                End If
            End If
            Return "XXX"
Suche:
            ZuAktualisierendeDateien = SucheNeueDateien(LokaleVersionen, UpdateVersionen)
            If ZuAktualisierendeDateien.Count > 0 Then 'Update vorhanden
                Return UpdateVersionen.Version
            Else
                Return String.Empty
            End If
        End If
    End Function

    Function LadeUpdate(ByVal MitUI As Boolean) As Boolean
        If ZuAktualisierendeDateien.Count > 0 Then
            If IO.File.Exists(IO.Path.Combine(UpdatePfad, "Versionen.lst")) AndAlso IO.Directory.GetFiles(IO.Path.Combine(UpdatePfad, ".."), "Update-*.exe").Length > 0 Then
                'bereits ein Update vorhanden
                If MitUI Then MessageBox.Show(Übersetzen.Übersetze("msgUpdateBereitsVorhanden", Environment.NewLine, ÜbersetzterProgrammName), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                Try
                    Using tmpUpdateForm As New frmUpdate
                        Using Client As New System.Net.WebClient()
                            IO.Directory.CreateDirectory(UpdatePfad)  'Verzeichnis für Update erstellen
                            If MitUI Then tmpUpdateForm.Show()
                            For i As Int32 = 0 To ZuAktualisierendeDateien.Count - 1 'Dateien herunterladen
                                If MitUI Then tmpUpdateForm.Aktualisieren(Übersetzen.Übersetze("lblAktuelleDatei", ZuAktualisierendeDateien(i).Name), i, ZuAktualisierendeDateien.Count - 1, Übersetzen.Übersetze("Update", ÜbersetzterProgrammName))
                                Application.DoEvents()
                                Try
                                    Try
                                        Entkomprimieren(Client.OpenRead(AktuellerServer & ZuAktualisierendeDateien(i).Name & ".kom"), IO.Path.Combine(UpdatePfad, ZuAktualisierendeDateien(i).Name))
                                    Catch
                                        Client.Proxy = Nothing
                                        Entkomprimieren(Client.OpenRead(AktuellerServer & ZuAktualisierendeDateien(i).Name & ".kom"), IO.Path.Combine(UpdatePfad, ZuAktualisierendeDateien(i).Name))
                                    End Try
                                Catch ex As Exception
                                    Console.Error.WriteLine(AktuellerServer & ZuAktualisierendeDateien(i).Name & ": " & ex.Message)
                                    MessageBox.Show(ex.Message, Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                End Try
                            Next i
                            If MitUI Then tmpUpdateForm.Aktualisieren(Übersetzen.Übersetze("UpdateFertigstellen"), 1, 1, Übersetzen.Übersetze("Update", ÜbersetzterProgrammName))
                            Application.DoEvents()
                            'Versionen.lst herunterladen, Update.exe verschieben
                            Entkomprimieren(Client.OpenRead(AktuellerServer & "Versionen.lst.kom"), IO.Path.Combine(UpdatePfad, "Versionen.lst"))
                        End Using

                        Dim t As Int32, tmpNeuFile As String
                        Do
                            t += 1
                            tmpNeuFile = IO.Path.Combine(IO.Path.Combine(UpdatePfad, ".."), "Update-" & t & ".exe")
                        Loop While IO.File.Exists(tmpNeuFile)
                        If IO.File.Exists(IO.Path.Combine(UpdatePfad, "Update.exe")) Then
                            Try
                                My.Computer.FileSystem.MoveFile(IO.Path.Combine(UpdatePfad, "Update.exe"), tmpNeuFile, True)
                            Catch
                            End Try
                        Else
                            Try
                                IO.File.Copy(IO.Path.Combine(Application.StartupPath, "Update.exe"), tmpNeuFile)
                            Catch
                            End Try
                        End If
                    End Using
                    Return True
                Catch ex As Exception 'Fehler beim Update herunterladen
                    If MitUI Then MessageBox.Show(Übersetzen.Übersetze("msgFehlerUpdate", Environment.NewLine & ex.Message), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End If
        Return False
    End Function

    Public Shared Function GetVersionsText(ByVal Version As Version, Optional ByVal ZeigeRevision As Boolean = True) As String
        Dim VersionText As String
        If ZeigeRevision = False OrElse Version.Revision = 0 Then
            If Version.Build = 0 Then
                VersionText = Version.ToString(2)
            Else
                VersionText = Version.ToString(3)
            End If
        Else
            VersionText = Version.ToString(4)
        End If
        Return VersionText
    End Function

    Shared Function SetzeVersionRegistry(ByVal AppID As String, ByVal VersionsText As String, ByVal Version As Version) As Boolean
        Dim tmpRegistry As Microsoft.Win32.RegistryKey = Nothing
        Try
            tmpRegistry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" & AppID & "_is1", False)
            If tmpRegistry IsNot Nothing Then
                If CStr(tmpRegistry.GetValue("DisplayName")) <> VersionsText OrElse CStr(tmpRegistry.GetValue("DisplayVersion")) <> GetVersionsText(Version) Then
                    tmpRegistry.Close()
                    tmpRegistry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" & AppID & "_is1", True)
                    tmpRegistry.SetValue("DisplayName", VersionsText)
                    tmpRegistry.SetValue("DisplayVersion", Version.ToString(4))
                End If
                tmpRegistry.Close()
                Return True
            Else
                Return False
            End If
        Catch
            If tmpRegistry IsNot Nothing Then tmpRegistry.Close()
            Return False
        End Try
    End Function

    Private Shared Sub Entkomprimieren(ByVal Stream As IO.Stream, ByVal DateiNach As String) 'geht nicht anders, da gzip.length nicht unterstützt wird:-(
        Static UseGUnzip As Boolean = True
        IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(DateiNach))
        Dim tmp As Byte()
        If UseGUnzip Then
            Try
                Dim tmpProcess As New Process
                tmpProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                tmpProcess.StartInfo.FileName = "gunzip"
                tmpProcess.StartInfo.UseShellExecute = False
                tmpProcess.Start() 'test ob gunzip verfügbar
                Dim tmpName As String = IO.Path.GetTempFileName
                If Stream.CanSeek Then
                    ReDim tmp(CInt(Stream.Length - 1))
                    Stream.Read(tmp, 0, CInt(Stream.Length))
                    Dim Writer As New IO.FileStream(tmpName & ".gz", IO.FileMode.Create, IO.FileAccess.Write)
                    Writer.Write(tmp, 0, tmp.Length)
                    Writer.Close()
                Else
                    Using Writer As New IO.FileStream(tmpName & ".gz", IO.FileMode.Create, IO.FileAccess.Write)
                        ReDim tmp(4999)
                        Dim bytesRead As Integer = Stream.Read(tmp, 0, 5000)
                        While bytesRead > 0
                            Writer.Write(tmp, 0, bytesRead)
                            bytesRead = Stream.Read(tmp, 0, 5000)
                        End While
                    End Using
                End If
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
        ReDim tmp(4999)
        Using Gzip As New IO.Compression.GZipStream(Stream, IO.Compression.CompressionMode.Decompress)
            Using Writer As New IO.FileStream(DateiNach, IO.FileMode.Create, IO.FileAccess.Write)
                Dim bytesRead As Integer = Gzip.Read(tmp, 0, 5000)
                While bytesRead > 0
                    Writer.Write(tmp, 0, bytesRead)
                    bytesRead = Gzip.Read(tmp, 0, 5000)
                End While
            End Using
        End Using 'gzip
    End Sub

    Private Shared Sub SendeAnGoogle(ByVal Datei As String, ByVal Kodierung As String, ByVal Auflösung As String, ByVal Farbtiefe As String, ByVal Sprache As String, ByVal Flashversion As String, ByVal Titel As String, ByVal Host As String, ByVal UACode As String)
        '0zufall        '1uhrzeit        '2datei        '3kodierung        '4auflösung        '5farbtiefe        '6sprache
        '7flashversion        '8Titel        '9host        '10UA-Code UA-2276175-1
        Try
            Dim client As New System.Net.WebClient, rnd As New Random
            Try
                client.OpenRead(String.Format("http://www.google-analytics.com/__utm.gif?utmwv=1&utmn={0}&utmcs={3}&utmsr={4}&utmsc={5}&utmul={6}&utmje=1&utmfl={7}&utmcn=1&utmdt={8}&utmhn={9}&utmr=-&utmp={2}&utmac={10}&utmcc=__utma%3D81541394.{0}.{1}.{1}.{1}.1%3B%2B__utmb%3D81541394%3B%2B__utmc%3D81541394%3B%2B__utmz%3D81541394.{1}.1.1.utmccn%3D(direct)%7Cutmcsr%3D(direct)%7Cutmcmd%3D(none)%3B%2B", rnd.NextDouble * 2147483647, Int(Date.UtcNow.Subtract(New Date(1970, 1, 1)).Ticks / 10000000), Datei, Kodierung, Auflösung, Farbtiefe, Sprache, Flashversion, Titel, Host, UACode)).Close()
            Catch
                client.Proxy = Nothing
                client.OpenRead(String.Format("http://www.google-analytics.com/__utm.gif?utmwv=1&utmn={0}&utmcs={3}&utmsr={4}&utmsc={5}&utmul={6}&utmje=1&utmfl={7}&utmcn=1&utmdt={8}&utmhn={9}&utmr=-&utmp={2}&utmac={10}&utmcc=__utma%3D81541394.{0}.{1}.{1}.{1}.1%3B%2B__utmb%3D81541394%3B%2B__utmc%3D81541394%3B%2B__utmz%3D81541394.{1}.1.1.utmccn%3D(direct)%7Cutmcsr%3D(direct)%7Cutmcmd%3D(none)%3B%2B", rnd.NextDouble * 2147483647, Int(Date.UtcNow.Subtract(New Date(1970, 1, 1)).Ticks / 10000000), Datei, Kodierung, Auflösung, Farbtiefe, Sprache, Flashversion, Titel, Host, UACode)).Close()
            End Try
        Catch
        End Try
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Programmname"></param>
    ''' <param name="Version"></param>
    ''' <param name="PN">Portable/Normal</param>
    ''' <param name="Typ"></param>
    ''' <remarks></remarks>

    Private Function SendeStatistik(ByVal Programmname As String, ByVal Version As String, ByVal PN As String, ByVal Typ As String) As Boolean
#If DEBUG Then
        Dim Server As String = "lokal.mal-was-anderes.de/update"
#Else
        Dim Server As String = "update.mal-was-anderes.de"
#End If

        Try
            Dim client As New System.Net.WebClient
            Try
                client.OpenRead(String.Format("http://{5}/update.php?programm={0}&version={1}&pn={2}&typ={3}&platform={4}&lang={6}&dat={7}", Programmname, Version, PN, Typ, My.Computer.Info.OSPlatform, Server, My.Application.Culture.Name, Daten)).Close()
            Catch
                client.Proxy = Nothing
                client.OpenRead(String.Format("http://{5}/update.php?programm={0}&version={1}&pn={2}&typ={3}&platform={4}&lang={6}&dat={7}", Programmname, Version, PN, Typ, My.Computer.Info.OSPlatform, Server, My.Application.Culture.Name, Daten)).Close()
            End Try
            Return True
        Catch
            Return False
        End Try
    End Function

    Function InstalliereUpdate(ByVal EreignisAufrufen As Boolean) As Boolean 'Wenn update vorhande true sonst false
        If IO.File.Exists(IO.Path.Combine(UpdatePfad, "Versionen.lst")) AndAlso IO.Directory.GetFiles(IO.Path.Combine(UpdatePfad, ".."), "Update-*.exe").Length > 0 Then
            Dim pi As New System.Diagnostics.ProcessStartInfo
            Try
                IO.File.Create(IO.Path.Combine(ProgrammPfad, "tmp.d" & (New Random).Next(0, 10)), 1, IO.FileOptions.DeleteOnClose).Close() 'Test ob Schreibrechte im Programmverzeichnis
            Catch 'wenn keine Schreibrechte im Programmverzeichnis
                If Environment.OSVersion.Platform = PlatformID.Win32NT AndAlso Environment.OSVersion.Version.Major >= 6 Then 'vista, win7
                    pi.Verb = "runas"
                Else
                    MessageBox.Show(Übersetzen.Übersetze("msgUpdateInstallierenAdmin", ÜbersetzterProgrammName), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return False
                End If
            End Try

            If EreignisAufrufen Then
                Dim Manual As New System.Threading.ManualResetEvent(False)
                RaiseEvent Neustarten(Manual)
                Manual.WaitOne(60000, False)
            End If

            pi.WorkingDirectory = Application.StartupPath
            Try
                Dim tmpneusteÄnderung As New Date(0), tmpDatei As String = String.Empty
                For Each file As String In IO.Directory.GetFiles(IO.Path.Combine(UpdatePfad, ".."), "Update-*.exe")
                    If IO.File.GetLastWriteTime(file) > tmpneusteÄnderung Then
                        tmpneusteÄnderung = IO.File.GetLastWriteTime(file)
                        tmpDatei = file
                    End If
                Next
                Dim UpdateProgrammEXE As String = """" & tmpDatei & """"
                If Environment.OSVersion.Platform = PlatformID.Unix Then
                    pi.FileName = "mono"
                    pi.Arguments = UpdateProgrammEXE & " """ & ProgrammName & """ """ & ProgrammExe & """"
                    pi.UseShellExecute = False
                    Console.WriteLine(Übersetzen.Übersetze("MonoUpdateHinweis", "cd """ & Application.StartupPath & """ && mono " & pi.Arguments))
                    Try
                        Process.Start(pi)
                    Catch ex As Exception
                        MessageBox.Show(Übersetzen.Übersetze("FehlerAusführen", pi.FileName & " " & pi.Arguments, ex.Message))
                    End Try
                Else
                    pi.FileName = UpdateProgrammEXE
                    pi.Arguments = """" & ProgrammName & """ """ & ProgrammExe & """"
                    Try
                        Process.Start(pi)
                    Catch ex As Exception
                        MessageBox.Show(Übersetzen.Übersetze("FehlerAusführen", pi.FileName & " " & pi.Arguments, ex.Message))
                    End Try
                End If
                Application.Exit()
                Return True
            Catch
                Return False
            End Try
        Else
            Return False
        End If
    End Function

    Public Shared Function IsRunningOnMono() As Boolean
        Return (Type.GetType("Mono.Runtime") IsNot Nothing)
    End Function

    Private Function SucheNeueDateien(ByVal LokaleVersionen As VersionenDatei, ByVal UpdateVersionen As VersionenDatei) As Dateien
        Dim tmpDateien As New Dateien
        Dim LokalVersionKategorieIndex As Int32
        If UpdateVersionen.InterneVersion > LokaleVersionen.InterneVersion Then
            For i As Int32 = 0 To UpdateVersionen.Kategorien.Count - 1
                With UpdateVersionen.Kategorien(i)
                    If InstallierteKategorien Is Nothing OrElse .Pflicht OrElse Array.IndexOf(InstallierteKategorien, .Name) > -1 Then
                        LokalVersionKategorieIndex = LokaleVersionen.Kategorien.IndexOf(.Name)
                        If LokalVersionKategorieIndex > -1 Then 'Lokale Versionen sind vorhanden
                            '=> neuere Versionen suchen
                            Dim lokalDateien As Dateien = LokaleVersionen.Kategorien(LokalVersionKategorieIndex).Dateien
                            For j As Int32 = 0 To .Dateien.Count - 1
                                If lokalDateien.IndexOf(.Dateien(j).Name) = -1 OrElse _
                                  .Dateien(j).InterneVersion > lokalDateien(lokalDateien.IndexOf(.Dateien(j).Name)).InterneVersion OrElse _
                                  (Not IO.File.Exists(IO.Path.Combine(ProgrammPfad, .Dateien(j).Name))) Then
                                    tmpDateien.Add(.Dateien(j).Name, .Dateien(j).InterneVersion)
                                End If
                            Next j
                        Else 'Lokale Versionen zu dieser Kategorie sind nicht vorhanden
                            '=> alle aus dieser Kategorie aktualisieren
                            For j As Int32 = 0 To .Dateien.Count - 1
                                tmpDateien.Add(.Dateien(j).Name, .Dateien(j).InterneVersion)
                            Next j
                        End If
                    End If
                End With
            Next i
        End If
        Return tmpDateien
    End Function

    Sub ZeigeUpdateHistory(Optional ByVal ParentForm As Form = Nothing)
        Using frmH As New frmUpdateHistory(Übersetzen, ProgrammPfad)
            If ParentForm Is Nothing Then frmH.StartPosition = FormStartPosition.CenterScreen
            frmH.ShowDialog(ParentForm)
        End Using
    End Sub
End Class

Friend Class VersionenDatei
    Friend Version As String, InterneVersion As Int32
    Dim ReleasNotesUrl As String
    Friend Kategorien As New Kategorien

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

        Dim tmpKategorienIndex As Int32 = -1, tmp As String
        Dim Reader As New IO.StreamReader(Stream, True)
        Reader.ReadLine() 'UpdateVersion
        Version = Reader.ReadLine
        InterneVersion = CInt(Reader.ReadLine)
        ReleasNotesUrl = Reader.ReadLine
        Do Until Reader.Peek = -1
            tmp = Reader.ReadLine
            If tmp.Length > 1 AndAlso tmp.Substring(0, 2) = "K:" Then
                tmpKategorienIndex = Kategorien.Add(tmp.Substring(2, tmp.IndexOf(":"c, 2) - 2), CBool(tmp.Substring(tmp.IndexOf(":"c, 2) + 1)))
            ElseIf tmp.Length > 1 AndAlso tmp.Substring(0, 2) = "D:" Then
                tmpKategorienIndex = -2
            ElseIf tmpKategorienIndex = -2 Then
                'Dateien zum Löschen
                'Löschen macht Update.exe
            ElseIf tmpKategorienIndex > -1 Then
                'Dateien in Kategorien
                Kategorien(tmpKategorienIndex).Dateien.Add(tmp, CInt(Reader.ReadLine))
            End If
        Loop
        Try 'für linux
            Reader.Close()
            Stream.Close()
        Catch
        End Try
    End Sub
End Class

Friend Class Kategorien
    Friend kKategorie() As Kategorie

    Default ReadOnly Property Kategorie(ByVal Index As Int32) As Kategorie
        Get
            Return kKategorie(Index)
        End Get
    End Property

    Function Add(ByVal Name As String, ByVal Pflicht As Boolean) As Int32
        Name = Name.Trim.ToLowerInvariant
        If IndexOf(Name) = -1 Then
            ReDim Preserve kKategorie(Count)
            kKategorie(Count - 1) = New Kategorie(Name, Pflicht)
            Return Count - 1
        Else
            Return IndexOf(Name)
        End If
    End Function

    Function IndexOf(ByVal Name As String) As Int32
        For i As Int32 = 0 To Count - 1
            If kKategorie(i).Name = Name Then Return i
        Next i
        Return -1
    End Function

    ReadOnly Property Count() As Int32
        Get
            If kKategorie Is Nothing Then Return 0 Else Return kKategorie.Length
        End Get
    End Property
End Class

Friend Class Kategorie
    Friend Name As String, Pflicht As Boolean
    Friend Dateien As New Dateien

    Sub New(ByVal Name As String, ByVal Pflicht As Boolean)
        Me.Name = Name
        Me.Pflicht = Pflicht
    End Sub
End Class

Friend Class Dateien
    Inherits List(Of Datei)

    Overloads Function Add(ByVal Name As String, ByVal InterneVersion As Int32) As Int32
        If IndexOf(Name) = -1 Then
            MyBase.Add(New Datei(Name.Replace("\"c, "/"c), InterneVersion))
            Return Count - 1
        Else
            Return IndexOf(Name)
        End If
    End Function

    Overloads Function IndexOf(ByVal Name As String) As Int32
        Name = Name.Trim.Replace("\"c, "/"c)
        For i As Int32 = 0 To Count - 1
            If String.Compare(MyBase.Item(i).Name.Trim, Name, StringComparison.OrdinalIgnoreCase) = 0 Then Return i
        Next i
        Return -1
    End Function
End Class

Friend Class Datei
    Friend Name As String, InterneVersion As Int32

    Sub New(ByVal Name As String, ByVal InterneVersion As Int32)
        Me.Name = Name
        Me.InterneVersion = InterneVersion
    End Sub
End Class