Public Class Update
    Dim UpdateServer() As String
    Dim InstallierteKategorien() As String '= Nothing
    Dim Übersetzen As New dllSprache.clsÜbersetzen("xxx", My.Resources.German)

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

    Sub New(ByVal Name As String, ByVal Exe As String, ByVal Version As String, ByVal Pfad As String, ByVal UpdateServerDatei As String, ByVal StandardUpdateServer() As String, ByVal Daten As String)
        ProgrammPfad = Pfad
        ProgrammName = Name
        ProgrammExe = Exe
        'ProgrammSprache = Sprache
        ProgrammVersion = Version
        Me.Daten = Daten
        'UpdatePfad festlegen
        If System.IO.File.Exists(ProgrammPfad & "/Portable") Then
            UpdatePfad = ProgrammPfad & "/Update/"
        Else
            UpdatePfad = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "/Flo & Seb Engineering/" & ProgrammName & "/Update/"
        End If

        'UpdateServer setzen
        'Dim UpdateReader As System.IO.StreamReader
        Try
            Using UpdateReader As New System.IO.StreamReader(UpdateServerDatei, True)
                UpdateServer = UpdateReader.ReadToEnd.Split(New Char() {ChrW(10), ChrW(13)}, StringSplitOptions.RemoveEmptyEntries)
            End Using
        Catch
            UpdateServer = StandardUpdateServer
        End Try

        'Installierte Kategorien rausfinden
        If System.IO.File.Exists(ProgrammPfad & "/Kategorien.ini") Then
            Dim tmp As String
            Dim Reader As New IO.StreamReader(ProgrammPfad & "/Kategorien.ini", True)
            Do Until Reader.Peek = -1
                tmp = Reader.ReadLine
                If tmp.IndexOf("="c) > -1 AndAlso tmp.Substring(tmp.IndexOf("="c) + 1).Trim.ToLower = "true" Then
                    If InstallierteKategorien Is Nothing Then ReDim InstallierteKategorien(0) Else ReDim Preserve InstallierteKategorien(InstallierteKategorien.Length)
                    InstallierteKategorien(InstallierteKategorien.GetUpperBound(0)) = tmp.Substring(0, tmp.IndexOf("="c)).Trim.ToLower
                End If
            Loop
            Reader.Close()
        End If
    End Sub

    Sub Übersetze(ByVal Sprache As String, ByVal ÜbersetzterName As String)
        ÜbersetzterProgrammName = ÜbersetzterName
        Select Case Sprache.ToLower
            Case "german"
                Übersetzen.Load("German", My.Resources.German)
            Case "english"
                Übersetzen.Load("English", My.Resources.English)
            Case "french"
                Übersetzen.Load("French", My.Resources.French)
            Case "spanish"
                Übersetzen.Load("Spanish", My.Resources.Spanish)
            Case "bavarian"
                Übersetzen.Load("Bavarian", My.Resources.Bavarian)
            Case Else
                Übersetzen = New dllSprache.clsÜbersetzen("xxx", My.Resources.English)
        End Select
    End Sub

    ''' <summary>
    ''' Sucht nach Updates und fragt fals vorhanden, ob es installiert werden soll.
    ''' </summary>
    ''' <remarks></remarks>
    Sub UpdateSuchenInstallieren(Optional ByVal ZeigeFehler As Object = True) 'zeigefehler as object um in eigenem thread zu starten
        Dim zZeigeFehler As Boolean = CBool(ZeigeFehler)
        If Not (GeradeUpdaten) Then
            If System.IO.File.Exists(UpdatePfad & "Versionen.lst") AndAlso System.IO.Directory.GetFiles(UpdatePfad & "../", "Update-*.exe").Length > 0 Then
                'bereits ein Update vorhanden
                If zZeigeFehler Then MessageBox.Show(Übersetzen.Übersetze("msgUpdateBereitsVorhanden", Environment.NewLine, ÜbersetzterProgrammName), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                GeradeUpdaten = True
                Dim tmpUpdateSuche As String = SucheUpdate(zZeigeFehler)
                If tmpUpdateSuche = "XXX" Then 'Sucht Schon Update, oder Update fehler oder update schon heruntergeladen
                    GeradeUpdaten = False
                    Exit Sub
                ElseIf Not String.IsNullOrEmpty(tmpUpdateSuche) Then 'Update vorhanden
                    'Try
                    '    System.IO.File.Create(ProgrammPfad & "/tmp.d", 1, IO.FileOptions.DeleteOnClose).Close() 'Test ob Schreibrechte im Programmverzeichnis
                    'Catch 'wenn keine Schreibrechte im Programmverzeichnis
                    '    MessageBox.Show(Übersetzen.Übersetze("msgUpdateVorhandenAdmin", tmpUpdateSuche, Environment.NewLine), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Information)
                    '    GeradeUpdaten = False
                    '    Exit Sub
                    'End Try

                    Try
                        'Updatefrage
                        If MessageBox.Show(Übersetzen.Übersetze("msgUpdateVorhanden", tmpUpdateSuche, Environment.NewLine), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = DialogResult.Yes Then
                            If LadeUpdate(True) Then
                                If GoogleStatistik Then
                                    Dim tmpString As String : If System.IO.File.Exists(ProgrammPfad & "/Portable") Then tmpString = "Portable" Else tmpString = "Normal"
                                    If Not SendeStatistik(ProgrammName.ToLower, ProgrammVersion, tmpString, "gesucht gefunden installiert") Then
                                        SendeAnGoogle("/updates/" & ProgrammName.ToLower & "/updatesuchen.htm", String.Empty, "Vorhanden", tmpString, "de", ProgrammVersion, ProgrammName & "UpdateSuche", "update.mal-was-anderes.de", "UA-2276175-1")
                                        SendeAnGoogle("/updates/" & ProgrammName.ToLower & "/update.htm", String.Empty, "Updaten", tmpString, "de", ProgrammVersion, ProgrammName & "Update", "update.mal-was-anderes.de", "UA-2276175-1")
                                    End If
                                End If
                                If MessageBox.Show(Übersetzen.Übersetze("msgUpdateErfolgreich", Environment.NewLine, ÜbersetzterProgrammName), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                                    InstalliereUpdate(True)
                                End If
                            Else
                                If GoogleStatistik Then
                                    Dim tmpString As String : If System.IO.File.Exists(ProgrammPfad & "/Portable") Then tmpString = "Portable" Else tmpString = "Normal"
                                    If Not SendeStatistik(ProgrammName.ToLower, ProgrammVersion, tmpString, "gesucht gefunden installieren fehler") Then
                                        SendeAnGoogle("/updates/" & ProgrammName.ToLower & "/updatesuchen.htm", String.Empty, "Vorhanden", tmpString, "de", ProgrammVersion, ProgrammName & "UpdateSuche", "update.mal-was-anderes.de", "UA-2276175-1")
                                    End If
                                End If
                            End If
                        Else
                            If GoogleStatistik Then
                                Dim tmpString As String : If System.IO.File.Exists(ProgrammPfad & "/Portable") Then tmpString = "Portable" Else tmpString = "Normal"
                                If Not SendeStatistik(ProgrammName.ToLower, ProgrammVersion, tmpString, "gesucht gefunden nicht installiert") Then
                                    SendeAnGoogle("/updates/" & ProgrammName.ToLower & "/updatesuchen.htm", String.Empty, "Vorhanden", tmpString, "de", ProgrammVersion, ProgrammName & "UpdateSuche", "update.mal-was-anderes.de", "UA-2276175-1")
                                End If
                            End If
                        End If
                    Catch ex As Exception
                        MessageBox.Show(ex.Message)
                    End Try
                Else 'Kein Update vorhanden
                    'Statistik:
                    Dim tmpString As String : If System.IO.File.Exists(ProgrammPfad & "/Portable") Then tmpString = "portable" Else tmpString = "normal"
                    If GoogleStatistik Then
                        If Not SendeStatistik(ProgrammName.ToLower, ProgrammVersion, tmpString, "gesucht nicht gefunden") Then
                            SendeAnGoogle("/updates/" & ProgrammName.ToLower & "/updatesuchen.htm", String.Empty, "KeinsVorhanden", tmpString, "de", ProgrammVersion, ProgrammName & "UpdateSuche", "update.mal-was-anderes.de", "UA-2276175-1")
                        End If
                    End If
                    If zZeigeFehler Then MessageBox.Show(Übersetzen.Übersetze("msgKeinUpdate"), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
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
    Function SucheUpdate(Optional ByVal ZeigeFehler As Boolean = True) As String
        If System.IO.File.Exists(UpdatePfad & "Versionen.lst") AndAlso System.IO.Directory.GetFiles(UpdatePfad & "../", "Update-*.exe").Length > 0 Then
            'bereits ein Update vorhanden
            If ZeigeFehler Then MessageBox.Show(Übersetzen.Übersetze("msgUpdateBereitsVorhanden", Environment.NewLine, ÜbersetzterProgrammName), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return "XXX"
        Else
            GeradeUpdaten = True
            Dim LokaleVersionen, UpdateVersionen As New VersionenDatei

            Try
                LokaleVersionen.Öffnen(ProgrammPfad & "/Versionen.lst", True) 'Lokale Versionen datei öffnen
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
                Dim tmpString As String : If System.IO.File.Exists(ProgrammPfad & "/Portable") Then tmpString = "Portable" Else tmpString = "Normal"
                If Not SendeStatistik(ProgrammName.ToLower, ProgrammVersion, tmpString, "gesucht gefunden installieren fehler") Then
                    SendeAnGoogle("/updates/" & ProgrammName.ToLower & "/updatefehler.htm", String.Empty, "Vorhanden", tmpString, "de", ProgrammVersion, ProgrammName & "UpdateSuche", "update.mal-was-anderes.de", "UA-2276175-1")
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
            If System.IO.File.Exists(UpdatePfad & "Versionen.lst") AndAlso System.IO.Directory.GetFiles(UpdatePfad & "../", "Update-*.exe").Length > 0 Then
                'bereits ein Update vorhanden
                If MitUI Then MessageBox.Show(Übersetzen.Übersetze("msgUpdateBereitsVorhanden", Environment.NewLine, ÜbersetzterProgrammName), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                Dim tmpUpdateForm As New frmUpdate
                Try
                    Dim Client As New System.Net.WebClient()
                    IO.Directory.CreateDirectory(UpdatePfad)  'Verzeichnis für Update erstellen
                    If MitUI Then tmpUpdateForm.Show()
                    For i As Int32 = 0 To ZuAktualisierendeDateien.Count - 1 'Dateien herunterladen
                        If MitUI Then tmpUpdateForm.Aktualisieren(Übersetzen.Übersetze("lblAktuelleDatei", ZuAktualisierendeDateien(i).Name), i, ZuAktualisierendeDateien.Count - 1, Übersetzen.Übersetze("Update", ÜbersetzterProgrammName))
                        Application.DoEvents()
                        Try
                            Try
                                Entkomprimieren(Client.OpenRead(AktuellerServer & ZuAktualisierendeDateien(i).Name & ".kom"), UpdatePfad & ZuAktualisierendeDateien(i).Name)
                            Catch
                                Client.Proxy = Nothing
                                Entkomprimieren(Client.OpenRead(AktuellerServer & ZuAktualisierendeDateien(i).Name & ".kom"), UpdatePfad & ZuAktualisierendeDateien(i).Name)
                            End Try
                        Catch ex As Exception
                            Console.Error.WriteLine(AktuellerServer & ZuAktualisierendeDateien(i).Name & ": " & ex.Message)
                            MessageBox.Show(ex.Message, Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Try
                    Next i

                    If MitUI Then tmpUpdateForm.Aktualisieren(Übersetzen.Übersetze("UpdateFertigstellen"), 1, 1, Übersetzen.Übersetze("Update", ÜbersetzterProgrammName))
                    Application.DoEvents()
                    'Versionen.lst herunterladen, Update.exe verschieben
                    Entkomprimieren(Client.OpenRead(AktuellerServer & "Versionen.lst.kom"), UpdatePfad & "Versionen.lst")
                    'Client.DownloadFile(AktuellerServer & "Update.lst", Programmpfad & "/Update/Update.lst")

                    Dim t As Int32
                    Do
                        t += 1
                    Loop While System.IO.File.Exists(UpdatePfad & "../Update-" & t & ".exe")
                    If System.IO.File.Exists(UpdatePfad & "Update.exe") Then
                        Try
                            My.Computer.FileSystem.MoveFile(UpdatePfad & "Update.exe", UpdatePfad & "../Update-" & t & ".exe", True)
                        Catch
                        End Try
                    Else
                        Try
                            System.IO.File.Copy(Application.StartupPath & "/Update.exe", UpdatePfad & "../Update-" & t & ".exe")
                        Catch
                        End Try
                    End If
                    If MitUI Then tmpUpdateForm.Close() : tmpUpdateForm = Nothing
                    Return True
                Catch ex As Exception 'Fehler beim Update herunterladen
                    If MitUI Then MessageBox.Show(Übersetzen.Übersetze("msgFehlerUpdate", Environment.NewLine & ex.Message), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End If
        Return False
    End Function

    Shared Function SetzeVersionRegistry(ByVal AppID As String, ByVal VersionsText As String, ByVal Version As Version) As Boolean
        Dim tmpRegistry As Microsoft.Win32.RegistryKey
        Try
            tmpRegistry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" & AppID & "_is1", False)
            If CStr(tmpRegistry.GetValue("DisplayName")) <> VersionsText OrElse CStr(tmpRegistry.GetValue("DisplayVersion")) = Version.ToString(4) Then
                tmpRegistry.Close()
                tmpRegistry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" & AppID & "_is1", True)
                tmpRegistry.SetValue("DisplayName", VersionsText)
                tmpRegistry.SetValue("DisplayVersion", Version.ToString(4))
            End If
            tmpRegistry.Close()
            Return True
        Catch
            If tmpRegistry IsNot Nothing Then tmpRegistry.Close()
            Return False
        End Try
    End Function

    Shared Sub Entkomprimieren(ByVal Stream As IO.Stream, ByVal DateiNach As String) 'geht nicht anders, da gzip.length nicht unterstützt wird:-(
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(DateiNach))
        Try
            Dim tmpProcess As New Process
            tmpProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            tmpProcess.StartInfo.FileName = "gunzip"
            tmpProcess.Start()
            Dim tmpName As String = System.IO.Path.GetTempFileName
            Dim tmp As Byte()
            If Stream.CanSeek Then
                ReDim tmp(CInt(Stream.Length - 1))
                Stream.Read(tmp, 0, CInt(Stream.Length))
            Else
                Dim offset As Integer = 0
                Dim totalCount As Integer = 0
                While True
                    ReDim Preserve tmp(totalCount + 5000)
                    Dim bytesRead As Integer = Stream.Read(tmp, offset, 5000)
                    If bytesRead = 0 Then
                        Exit While
                    End If
                    offset += bytesRead
                    totalCount += bytesRead
                    'If (totalCount Mod 60000) = 0 Then Application.DoEvents()
                End While
                ReDim Preserve tmp(totalCount - 1)
            End If
            Dim Writer As New System.IO.FileStream(tmpName & ".gz", IO.FileMode.Create, IO.FileAccess.Write)
            Writer.Write(tmp, 0, tmp.Length)
            Writer.Close()
            'tmpProcess.StartInfo.UseShellExecute = False
            'tmpProcess.StartInfo.RedirectStandardOutput = True
            tmpProcess = New Process
            tmpProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            tmpProcess.StartInfo.Arguments = "-qf """ & tmpName & ".gz" & """"
            tmpProcess.StartInfo.FileName = "gunzip"
            tmpProcess.Start()
            tmpProcess.WaitForExit(5000)

            My.Computer.FileSystem.MoveFile(tmpName, DateiNach, True)
            Stream.Close()
        Catch
            Dim tmp(4000) As Byte
            Dim Gzip As New System.IO.Compression.GZipStream(Stream, IO.Compression.CompressionMode.Decompress)

            'Dim offset As Integer = 0
            'Dim totalCount As Integer = 0
            Dim Writer As New System.IO.FileStream(DateiNach, IO.FileMode.Create, IO.FileAccess.Write)
            While True
                'ReDim Preserve tmp(totalCount + 4000)
                Dim bytesRead As Integer = Gzip.Read(tmp, 0, 4000)
                If bytesRead = 0 Then
                    Exit While
                End If
                Writer.Write(tmp, 0, bytesRead)

                'offset += bytesRead
                'totalCount += bytesRead
                'If (totalCount Mod 80000) = 0 Then Application.DoEvents()
            End While
            Writer.Close()
            'ReDim Preserve tmp(totalCount)
            Gzip.Close()
        End Try
    End Sub

    Shared Sub SendeAnGoogle(ByVal Datei As String, ByVal Kodierung As String, ByVal Auflösung As String, ByVal Farbtiefe As String, ByVal Sprache As String, ByVal Flashversion As String, ByVal Titel As String, ByVal Host As String, ByVal UACode As String)
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
            client.Dispose()
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

    Function SendeStatistik(ByVal Programmname As String, ByVal Version As String, ByVal PN As String, ByVal Typ As String) As Boolean
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
            client.Dispose()
            Return True
        Catch
            Return False
        End Try
    End Function

    Function InstalliereUpdate(ByVal EreignisAufrufen As Boolean) As Boolean 'Wenn update vorhande true sonst false
        If System.IO.File.Exists(UpdatePfad & "Versionen.lst") AndAlso System.IO.Directory.GetFiles(UpdatePfad & "../", "Update-*.exe").Length > 0 Then
            Dim pi As New System.Diagnostics.ProcessStartInfo
            Try
                System.IO.File.Create(ProgrammPfad & "/tmp.d" & (New Random).Next(0, 10), 1, IO.FileOptions.DeleteOnClose).Close() 'Test ob Schreibrechte im Programmverzeichnis
            Catch 'wenn keine Schreibrechte im Programmverzeichnis
                If Environment.OSVersion.Platform = PlatformID.Win32NT AndAlso Environment.OSVersion.Version.Major >= 6 Then 'vista, win7
                    pi.Verb = "runas"
                Else
                    MessageBox.Show(Übersetzen.Übersetze("msgUpdateInstallierenAdmin", ÜbersetzterProgrammName), Übersetzen.Übersetze("Update", ÜbersetzterProgrammName), MessageBoxButtons.OK, MessageBoxIcon.Information)
                    'Application.Exit()
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
                'If IsRunningOnMono() Then
                '    pi.FileName = "mono """ & UpdatePfad & "../Update.exe""" & " """ & ProgrammName & """ """ & ProgrammExe & """"
                '    Process.Start(pi)
                'Else
                Dim tmpneusteÄnderung As New Date(0), tmpDatei As String = String.Empty
                For Each file As String In System.IO.Directory.GetFiles(UpdatePfad & "../", "Update-*.exe")
                    If System.IO.File.GetLastWriteTime(file) > tmpneusteÄnderung Then
                        tmpneusteÄnderung = System.IO.File.GetLastWriteTime(file)
                        tmpDatei = file
                    End If
                Next
                pi.FileName = """" & tmpDatei & """"
                pi.Arguments = """" & ProgrammName & """ """ & ProgrammExe & """"
                Process.Start(pi)
                'End If
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

    Friend Function SucheNeueDateien(ByVal LokaleVersionen As VersionenDatei, ByVal UpdateVersionen As VersionenDatei) As Dateien
        Dim tmpDateien As New Dateien
        If UpdateVersionen.InterneVersion > LokaleVersionen.InterneVersion Then
            For i As Int32 = 0 To UpdateVersionen.Kategorien.Count - 1
                If InstallierteKategorien Is Nothing OrElse UpdateVersionen.Kategorien(i).Pflicht OrElse Array.IndexOf(InstallierteKategorien, UpdateVersionen.Kategorien(i).Name) > -1 Then
                    If LokaleVersionen.Kategorien.IndexOf(UpdateVersionen.Kategorien(i).Name) > -1 Then 'Lokale Versionen sind vorhanden
                        '=> neuere Versionen suchen
                        For j As Int32 = 0 To UpdateVersionen.Kategorien(i).Dateien.Count - 1
                            If LokaleVersionen.Kategorien(LokaleVersionen.Kategorien.IndexOf(UpdateVersionen.Kategorien(i).Name)).Dateien.IndexOf(UpdateVersionen.Kategorien(i).Dateien(j).Name) = -1 OrElse UpdateVersionen.Kategorien(i).Dateien(j).InterneVersion > LokaleVersionen.Kategorien(LokaleVersionen.Kategorien.IndexOf(UpdateVersionen.Kategorien(i).Name)).Dateien(LokaleVersionen.Kategorien(LokaleVersionen.Kategorien.IndexOf(UpdateVersionen.Kategorien(i).Name)).Dateien.IndexOf(UpdateVersionen.Kategorien(i).Dateien(j).Name)).InterneVersion OrElse (Not System.IO.File.Exists(ProgrammPfad & "/" & UpdateVersionen.Kategorien(i).Dateien(j).Name)) Then
                                tmpDateien.Add(UpdateVersionen.Kategorien(i).Dateien(j).Name, UpdateVersionen.Kategorien(i).Dateien(j).InterneVersion)
                            End If
                        Next j
                    Else 'Lokale Versionen zu dieser Kategorie sind nicht vorhanden
                        '=> alle aus dieser Kategorie aktualisieren
                        For j As Int32 = 0 To UpdateVersionen.Kategorien(i).Dateien.Count - 1
                            tmpDateien.Add(UpdateVersionen.Kategorien(i).Dateien(j).Name, UpdateVersionen.Kategorien(i).Dateien(j).InterneVersion)
                        Next j
                    End If
                End If
            Next i
        End If
        Return tmpDateien
    End Function

    Sub ZeigeUpdateHistory(Optional ByVal ParentForm As Form = Nothing)
        Dim tmpHistory As New frmUpdateHistory
        tmpHistory.Programmpfad = ProgrammPfad
        tmpHistory.Übersetzen = Übersetzen
        If ParentForm Is Nothing Then tmpHistory.StartPosition = FormStartPosition.CenterScreen
        tmpHistory.ShowDialog(ParentForm)
        tmpHistory.Dispose()
        tmpHistory = Nothing
    End Sub
End Class

Class VersionenDatei
    Friend Version As String, InterneVersion As Int32
    Dim ReleasNotesUrl As String
    Friend Kategorien As New Kategorien

    Sub Öffnen(ByVal Datei As String, ByVal IstLokal As Boolean)
        Dim Stream As System.IO.Stream
        If IstLokal Then
            Stream = New IO.FileStream(Datei, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
        Else
            Dim Client As New System.Net.WebClient

            'Stream = Client.OpenRead(Datei)
            Try
                Stream = New System.IO.Compression.GZipStream(Client.OpenRead(Datei), IO.Compression.CompressionMode.Decompress)
            Catch
                Client.Proxy = Nothing
                Stream = New System.IO.Compression.GZipStream(Client.OpenRead(Datei), IO.Compression.CompressionMode.Decompress)
            End Try
        End If

        Dim tmpKategorienIndex As Int32 = -1, tmp As String
        Dim Reader As New System.IO.StreamReader(Stream, True)
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

Class Kategorien
    Friend kKategorie() As Kategorie

    Default ReadOnly Property Kategorie(ByVal Index As Int32) As Kategorie
        Get
            Return kKategorie(Index)
        End Get
        'Set(ByVal value As Kategorie)
        '    kKategorie(Index) = value
        'End Set
    End Property

    Function Add(ByVal Name As String, ByVal Pflicht As Boolean) As Int32
        Name = Name.Trim.ToLower
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

Class Kategorie
    Friend Name As String, Pflicht As Boolean
    Friend Dateien As New Dateien

    Sub New(ByVal Name As String, ByVal Pflicht As Boolean)
        Me.Name = Name
        Me.Pflicht = Pflicht
    End Sub
End Class

Class Dateien
    Friend dDatei() As Datei

    Default ReadOnly Property Datei(ByVal Index As Int32) As Datei
        Get
            Return dDatei(Index)
        End Get
        'Set(ByVal value As Datei)
        '    dDatei(Index) = value
        'End Set
    End Property

    Function Add(ByVal Name As String, ByVal InterneVersion As Int32) As Int32
        If IndexOf(Name) = -1 Then
            ReDim Preserve dDatei(Count)
            dDatei(Count - 1) = New Datei(Name.Replace("\"c, "/"c), InterneVersion)
            Return Count - 1
        Else
            Return IndexOf(Name)
        End If
    End Function

    Function IndexOf(ByVal Name As String) As Int32
        Name = Name.Trim.ToLower.Replace("\"c, "/"c)
        For i As Int32 = 0 To Count - 1
            If dDatei(i).Name.Trim.ToLower = Name Then Return i
        Next i
        Return -1
    End Function

    ReadOnly Property Count() As Int32
        Get
            If dDatei Is Nothing Then Return 0 Else Return dDatei.Length
        End Get
    End Property
End Class

Class Datei
    Friend Name As String, InterneVersion As Int32

    Sub New(ByVal Name As String, ByVal InterneVersion As Int32)
        Me.Name = Name
        Me.InterneVersion = InterneVersion
    End Sub
End Class