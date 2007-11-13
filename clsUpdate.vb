Public Class Update
    Dim UpdateServer() As String
    Dim InstallierteKategorien() As String = Nothing
    Dim �bersetzen As New dllSprache.cls�bersetzen("xxx")

    Dim ZuAktualisierendeDateien As Dateien, AktuellerServer As String
    Dim ProgrammName, ProgrammExe, ProgrammPfad, ProgrammVersion, ProgrammSprache As String
    Public GeradeUpdaten As Boolean

    Public Delegate Sub UpdateSuchenInstallierenCallback(ByVal ZeigeFehler As Boolean, ByVal MitUI As Boolean)

    Sub New(ByVal Name As String, ByVal Exe As String, ByVal Version As String, ByVal Pfad As String, ByVal UpdateServerDatei As String, ByVal StandardUpdateServer() As String)
        ProgrammPfad = Pfad
        ProgrammName = Name
        ProgrammExe = Exe
        'ProgrammSprache = Sprache
        ProgrammVersion = Version

        'UpdateServer setzen
        Dim UpdateReader As System.IO.StreamReader = Nothing
        Try
            UpdateReader = New System.IO.StreamReader(UpdateServerDatei, True)
            UpdateServer = UpdateReader.ReadToEnd.Split(New Char() {Chr(10), Chr(13)}, StringSplitOptions.RemoveEmptyEntries)
        Catch
            UpdateServer = StandardUpdateServer
        Finally
            If UpdateReader IsNot Nothing Then UpdateReader.Close()
        End Try

        'Installierte Kategorien rausfinden
        If System.IO.File.Exists(ProgrammPfad & "/Kategorien.ini") Then
            Dim tmp As String
            Dim Reader As New IO.StreamReader(ProgrammPfad & "/Kategorien.ini", True)
            Do Until Reader.Peek = -1
                tmp = Reader.ReadLine
                If tmp.IndexOf("=") > -1 AndAlso tmp.Substring(tmp.IndexOf("=") + 1).Trim.ToLower = "true" Then
                    If InstallierteKategorien Is Nothing Then ReDim InstallierteKategorien(0) Else ReDim Preserve InstallierteKategorien(InstallierteKategorien.Length)
                    InstallierteKategorien(InstallierteKategorien.GetUpperBound(0)) = tmp.Substring(0, tmp.IndexOf("=")).Trim.ToLower
                End If
            Loop
            Reader.Close()
        End If
    End Sub

    Sub �bersetze(ByVal Sprache As String, ByVal �bersetzterName As String)
        �bersetzen.Ausdr�cke = New dllSprache.clsAusdr�cke
        �bersetzen.Ausdr�cke.Add("ProgrammName", �bersetzterName)
        If Sprache.ToLower = "german" Then
            �bersetzen.Ausdr�cke.Add("msgUpdateBereitsVorhanden", "Es ist bereits ein Update vorhanden!{0}Starten Sie den Karteikasten neu, um es zu installieren.")
            �bersetzen.Ausdr�cke.Add("Update", �bersetzterName & " Update")
            �bersetzen.Ausdr�cke.Add("msgUpdateVorhanden", "Ein Update auf Version {0} ist vorhanden.{1}Wollen Sie dieses jetzt herunterladen?")
            �bersetzen.Ausdr�cke.Add("msgUpdateVorhandenAdmin", "Ein Update auf Version {0} ist vorhanden.{1}Melden Sie sich als Administrator an, um es herunterzuladen.")
            �bersetzen.Ausdr�cke.Add("msgKeinUpdate", "Kein Update vorhanden")
            �bersetzen.Ausdr�cke.Add("msgFehlerUpdateSuchen", "Fehler beim Updatesuchen: {0}")
            �bersetzen.Ausdr�cke.Add("lblAktuelleDatei", "Aktuelle Datei: {0}")
            �bersetzen.Ausdr�cke.Add("UpdateFertigstellen", "Fertigstellen...")
            �bersetzen.Ausdr�cke.Add("msgUpdateErfolgreich", "Update erfolgreich heruntergeladen!{0}Um das Update zu installieren m�ssen Sie " & �bersetzterName & " neustarten.{0}Soll automatisch neu gestartet werden?")
            �bersetzen.Ausdr�cke.Add("msgFehlerUpdate", "Fehler beim Updaten: {0}")
            �bersetzen.Ausdr�cke.Add("UpdateHistory", "Update history")
            �bersetzen.Ausdr�cke.Add("Updates", "Updates:")
            �bersetzen.Ausdr�cke.Add("Schlie�en", "Schlie&�en")
            �bersetzen.Ausdr�cke.Add("VersionErfolgreichInstalliert", "{0}: Version {1} erfolgreich installiert.")
        ElseIf Sprache.ToLower = "english" Then
            �bersetzen.Ausdr�cke.Add("msgUpdateBereitsVorhanden", "You have already downloaded an update.{0} Restart Flashcards to install it.")
            �bersetzen.Ausdr�cke.Add("Update", �bersetzterName & " Update")
            �bersetzen.Ausdr�cke.Add("msgUpdateVorhanden", "Update to version {0} is available.{1}Do you want to download it now?")
            �bersetzen.Ausdr�cke.Add("msgUpdateVorhandenAdmin", "Update to version {0} is available.{1}Logon as administrator to download it.")
            �bersetzen.Ausdr�cke.Add("msgKeinUpdate", "No update available")
            �bersetzen.Ausdr�cke.Add("msgFehlerUpdateSuchen", "Error while searching updates: {0}")
            �bersetzen.Ausdr�cke.Add("lblAktuelleDatei", "Current file: {0}")
            �bersetzen.Ausdr�cke.Add("UpdateFertigstellen", "Finalising...")
            �bersetzen.Ausdr�cke.Add("msgUpdateErfolgreich", "Update downloaded successfully!{0}To install the update you have to restart " & �bersetzterName & ".{0}Restart now?")
            �bersetzen.Ausdr�cke.Add("msgFehlerUpdate", "Error while Updating: {0}")
            �bersetzen.Ausdr�cke.Add("UpdateHistory", "Update history")
            �bersetzen.Ausdr�cke.Add("Updates", "Updates:")
            �bersetzen.Ausdr�cke.Add("Schlie�en", "Clo&se")
            �bersetzen.Ausdr�cke.Add("VersionErfolgreichInstalliert", "{0}: version {1} successfully installed.")
        ElseIf Sprache.ToLower = "french" Then
            �bersetzen.Ausdr�cke.Add("msgUpdateBereitsVorhanden", "Il y a d�j� une mise � jour.{0} Red�marrez la caisse de fichier pour l'installer.")
            �bersetzen.Ausdr�cke.Add("Update", "Mise � jour de " & �bersetzterName)
            �bersetzen.Ausdr�cke.Add("msgUpdateVorhanden", "Une mise � jour � version {0} est disponible.{1}Voulez vous la t�l�charger maintenant?")
            �bersetzen.Ausdr�cke.Add("msgUpdateVorhandenAdmin", "Une mise � jour � version {0} est disponible.{1}Entre comme administrateur pour t�l�charger la.")
            �bersetzen.Ausdr�cke.Add("msgKeinUpdate", "Il n'y a pas de mise � jour.")
            �bersetzen.Ausdr�cke.Add("msgFehlerUpdateSuchen", "Erreur � chercher une mise � jour: {0}")
            �bersetzen.Ausdr�cke.Add("lblAktuelleDatei", "Fichier actuel: {0}")
            �bersetzen.Ausdr�cke.Add("UpdateFertigstellen", "Terminer...")
            �bersetzen.Ausdr�cke.Add("msgUpdateErfolgreich", "Mise � jour t�l�charg� avec succ�s!{0}Pour installer la mise � jour vous devez red�marrer " & �bersetzterName & ".{0}Red�marrer automatiquement?")
            �bersetzen.Ausdr�cke.Add("msgFehlerUpdate", "Erreur au t�l�chargement de la mise � jour: {0}")
            �bersetzen.Ausdr�cke.Add("UpdateHistory", "L'histoire des mises � jour")
            �bersetzen.Ausdr�cke.Add("Updates", "Les mises � jour:")
            �bersetzen.Ausdr�cke.Add("Schlie�en", "Fer&mer")
            �bersetzen.Ausdr�cke.Add("VersionErfolgreichInstalliert", "{0}: version {1} install� avec succ�s.")
        ElseIf Sprache.ToLower = "italian" Then
            �bersetzen.Ausdr�cke.Add("msgUpdateBereitsVorhanden", "Gia esiste un aggoirnamento, inizzializza il Karteikasten per installare.")
        Else
            �bersetzen.Ausdr�cke.Add("Update", �bersetzterName & " Update")
        End If
    End Sub

    ''' <summary>
    ''' Sucht nach Updates und fragt fals vorhanden, ob es installiert werden soll.
    ''' </summary>
    ''' <remarks></remarks>
    Sub UpdateSuchenInstallieren(Optional ByVal ZeigeFehler As Object = True) 'zeigefehler as object um in eigenem thread zu starten
        If Not (GeradeUpdaten) Then
            If System.IO.File.Exists(ProgrammPfad & "/Update/Versionen.lst") Then
                'bereits ein Update vorhanden
                If ZeigeFehler Then MessageBox.Show(�bersetzen.�bersetze("msgUpdateBereitsVorhanden", "Es ist bereits ein Update vorhanden!{0}Starten Sie den Karteikasten neu, um es zu installieren.", Environment.NewLine), �bersetzen.�bersetze("Update", "Update"), MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                GeradeUpdaten = True
                Dim tmpUpdateSuche As String = SucheUpdate(ZeigeFehler)
                If tmpUpdateSuche = "XXX" Then 'Sucht Schon Update, oder Update fehler oder update schon heruntergeladen
                    GeradeUpdaten = False
                    Exit Sub
                ElseIf tmpUpdateSuche <> "" Then 'Update vorhanden
                    Try
                        System.IO.File.Create(ProgrammPfad & "/tmp.d", 1, IO.FileOptions.DeleteOnClose).Close() 'Test ob Schreibrechte im Programmverzeichnis
                        'Updatefrage
                        If MessageBox.Show(�bersetzen.�bersetze("msgUpdateVorhanden", "Ein Update auf Version {0} ist vorhanden.{1}Wollen Sie dieses jetzt herunterladen?", tmpUpdateSuche, Environment.NewLine), �bersetzen.�bersetze("Update", "Update"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = DialogResult.Yes Then
                            LadeUpdate(True)
                        End If
                    Catch 'wenn keine Schreibrechte im Programmverzeichnis
                        MessageBox.Show(�bersetzen.�bersetze("msgUpdateVorhandenAdmin", "Ein Update auf Version {0} ist vorhanden.{1}Melden Sie sich als Administrator an, um es herunterzuladen.", tmpUpdateSuche, Environment.NewLine), �bersetzen.�bersetze("Update", "Update"), MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End Try
                Else 'Kein Update vorhanden
                    If ZeigeFehler Then MessageBox.Show(�bersetzen.�bersetze("msgKeinUpdate", "Kein Update vorhanden"), �bersetzen.�bersetze("Update", "Update"), MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
            GeradeUpdaten = False
        End If
    End Sub

    ''' <summary>
    ''' Sucht nach Updates
    ''' </summary>
    ''' <param name="ZeigeFehler">Ob fehler angezeigt werden sollen</param>
    ''' <returns>Gibt die neuere Version des Updates zur�ck andernfalls ""</returns>
    ''' <remarks></remarks>
    Function SucheUpdate(Optional ByVal ZeigeFehler As Boolean = True) As String
        If System.IO.File.Exists(ProgrammPfad & "/Update/Versionen.lst") Then
            'bereits ein Update vorhanden
            If ZeigeFehler Then MessageBox.Show(�bersetzen.�bersetze("msgUpdateBereitsVorhanden", "Es ist bereits ein Update vorhanden!{0}Starten Sie den Karteikasten neu, um es zu installieren.", Environment.NewLine), �bersetzen.�bersetze("Update", "Update"), MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return "XXX"
        Else
            GeradeUpdaten = True
            Dim LokaleVersionen, UpdateVersionen As New VersionenDatei

            Try
                LokaleVersionen.�ffnen(ProgrammPfad & "/Versionen.lst", True) 'Lokale Versionen datei �ffnen
            Catch ex As Exception
                Console.Error.WriteLine("Versionen.lst: " & ex.Message)
            End Try
            If LokaleVersionen.Version <> "" Then ProgrammVersion = LokaleVersionen.Version
            Dim tmpFehler As String = ""
            'Update versionsdatei �ffnen:
            For i As Int16 = 0 To UpdateServer.GetUpperBound(0)
                Try
                    UpdateVersionen.�ffnen(UpdateServer(i) & "Versionen.lst.kom", False)
                    AktuellerServer = UpdateServer(i)
                    GoTo Suche
                Catch ex As Exception
                    Console.Error.WriteLine(UpdateServer(i) & ": " & ex.Message)
                    tmpFehler &= UpdateServer(i) & ": " & ex.Message & Environment.NewLine
                End Try
            Next i
            If ZeigeFehler Then MessageBox.Show(�bersetzen.�bersetze("msgFehlerUpdateSuchen", "Fehler beim Updatesuchen: {0}", Environment.NewLine & tmpFehler), �bersetzen.�bersetze("Update", "Update"), MessageBoxButtons.OK, MessageBoxIcon.Error)

            Return "XXX"
Suche:
            ZuAktualisierendeDateien = SucheNeueDateien(LokaleVersionen, UpdateVersionen)
            If ZuAktualisierendeDateien.Count > 0 Then 'Update vorhanden
                'Statistik:
                Dim tmpString As String : If System.IO.File.Exists(ProgrammPfad & "/Portable") Then tmpString = "Portable" Else tmpString = "Normal"
                SendeAnGoogle("/updates/" & ProgrammName.ToLower & "/updatesuchen.htm", "", "Vorhanden", tmpString, "de", ProgrammVersion, ProgrammName & "UpdateSuche", "update.mal-was-anderes.de", "UA-2276175-1")

                Return UpdateVersionen.Version
            Else
                'Statistik:
                Dim tmpString As String : If System.IO.File.Exists(ProgrammPfad & "/Portable") Then tmpString = "Portable" Else tmpString = "Normal"
                SendeAnGoogle("/updates/" & ProgrammName.ToLower & "/updatesuchen.htm", "", "KeinsVorhanden", tmpString, "de", ProgrammVersion, ProgrammName & "UpdateSuche", "update.mal-was-anderes.de", "UA-2276175-1")

                Return ""
            End If
        End If
    End Function

    Sub LadeUpdate(ByVal MitUI As Boolean)
        If ZuAktualisierendeDateien.Count > 0 Then
            If System.IO.File.Exists(ProgrammPfad & "/Update/Versionen.lst") Then
                'bereits ein Update vorhanden
                If MitUI Then MessageBox.Show(�bersetzen.�bersetze("msgUpdateBereitsVorhanden", "Es ist bereits ein Update vorhanden!{0}Starten Sie den Karteikasten neu, um es zu installieren.", Environment.NewLine), �bersetzen.�bersetze("Update", "Update"), MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                Dim tmpUpdateForm As New frmUpdate
                Try
                    Dim Client As New System.Net.WebClient()
                    IO.Directory.CreateDirectory(ProgrammPfad & "/Update/") 'Verzeichnis f�r Update erstellen
                    If MitUI Then tmpUpdateForm.Show()
                    For i As Int32 = 0 To ZuAktualisierendeDateien.Count - 1 'Dateien herunterladen
                        If MitUI Then tmpUpdateForm.Aktualisieren(�bersetzen.�bersetze("lblAktuelleDatei", "Aktuelle Datei: {0}", ZuAktualisierendeDateien(i).Name), i, ZuAktualisierendeDateien.Count - 1, �bersetzen.�bersetze("Update", "Update"))
                        Application.DoEvents()
                        Try
                            Entkomprimieren(Client.OpenRead(AktuellerServer & ZuAktualisierendeDateien(i).Name & ".kom"), ProgrammPfad & "/Update/" & ZuAktualisierendeDateien(i).Name)
                        Catch ex As Exception
                            Console.Error.WriteLine(AktuellerServer & ZuAktualisierendeDateien(i).Name & ": " & ex.Message)
                            MessageBox.Show(ex.Message, �bersetzen.�bersetze("Update", "Update"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Try
                    Next i

                    If MitUI Then tmpUpdateForm.Aktualisieren(�bersetzen.�bersetze("UpdateFertigstellen", "Fertigstellen..."), 1, 1, �bersetzen.�bersetze("Update", "Update"))
                    Application.DoEvents()
                    'Versionen.lst herunterladen, Update.exe verschieben
                    Entkomprimieren(Client.OpenRead(AktuellerServer & "Versionen.lst.kom"), ProgrammPfad & "/Update/Versionen.lst")
                    'Client.DownloadFile(AktuellerServer & "Update.lst", Programmpfad & "/Update/Update.lst")
                    If System.IO.File.Exists(ProgrammPfad & "/Update/Update.exe") Then
                        My.Computer.FileSystem.MoveFile(ProgrammPfad & "/Update/Update.exe", ProgrammPfad & "/Update.exe", True)
                    End If
                    If MitUI Then tmpUpdateForm.Close() : tmpUpdateForm = Nothing
                    'Statistik:
                    Dim tmpString As String : If System.IO.File.Exists(ProgrammPfad & "/Portable") Then tmpString = "Portable" Else tmpString = "Normal"
                    SendeAnGoogle("/updates/" & ProgrammName.ToLower & "/update.htm", "", "Updaten", tmpString, "de", ProgrammVersion, ProgrammName & "Update", "update.mal-was-anderes.de", "UA-2276175-1")

                    If MitUI Then
                        If MessageBox.Show(�bersetzen.�bersetze("msgUpdateErfolgreich", "Update erfolgreich heruntergeladen!{0}Um das Update zu installieren m�ssen Sie " & ProgrammName & " neustarten.{0}Soll automatisch neu gestartet werden?", Environment.NewLine), �bersetzen.�bersetze("Update", "Update"), MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                            InstalliereUpdate()
                        End If
                    End If
                Catch ex As Exception 'Fehler beim Update herunterladen
                    If MitUI Then MessageBox.Show(�bersetzen.�bersetze("msgFehlerUpdate", "Fehler beim Updaten: {0}", Environment.NewLine & ex.Message), �bersetzen.�bersetze("Update", "Update"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End If
    End Sub

    Sub Entkomprimieren(ByVal Stream As IO.Stream, ByVal DateiNach As String) 'geht nicht anders, da gzip.length nicht unterst�tzt wird:-(
        Dim tmp() As Byte
        Dim Gzip As New System.IO.Compression.GZipStream(Stream, IO.Compression.CompressionMode.Decompress)

        Dim offset As Integer = 0
        Dim totalCount As Integer = 0
        While True
            ReDim Preserve tmp(totalCount + 100)
            Dim bytesRead As Integer = Gzip.Read(tmp, offset, 100)
            If bytesRead = 0 Then
                Exit While
            End If
            offset += bytesRead
            totalCount += bytesRead
        End While
        ReDim Preserve tmp(totalCount)
        Gzip.Close()
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(DateiNach))
        Dim Writer As New System.IO.FileStream(DateiNach, IO.FileMode.OpenOrCreate, IO.FileAccess.Write)
        Writer.Write(tmp, 0, tmp.GetUpperBound(0))
        Writer.Close()
    End Sub

    Sub SendeAnGoogle(ByVal Datei As String, ByVal Kodierung As String, ByVal Aufl�sung As String, ByVal Farbtiefe As String, ByVal Sprache As String, ByVal Flashversion As String, ByVal Titel As String, ByVal Host As String, ByVal UACode As String)
        '0zufall        '1uhrzeit        '2datei        '3kodierung        '4aufl�sung        '5farbtiefe        '6sprache
        '7flashversion        '8Titel        '9host        '10UA-Code UA-2276175-1
        Try
            Dim client As New System.Net.WebClient, rnd As New Random
            client.OpenRead(String.Format("http://www.google-analytics.com/__utm.gif?utmwv=1&utmn={0}&utmcs={3}&utmsr={4}&utmsc={5}&utmul={6}&utmje=1&utmfl={7}&utmcn=1&utmdt={8}&utmhn={9}&utmr=-&utmp={2}&utmac={10}&utmcc=__utma%3D81541394.{0}.{1}.{1}.{1}.1%3B%2B__utmb%3D81541394%3B%2B__utmc%3D81541394%3B%2B__utmz%3D81541394.{1}.1.1.utmccn%3D(direct)%7Cutmcsr%3D(direct)%7Cutmcmd%3D(none)%3B%2B", rnd.NextDouble * 2147483647, Int(Date.UtcNow.Subtract(New Date(1970, 1, 1)).Ticks / 10000000), Datei, Kodierung, Aufl�sung, Farbtiefe, Sprache, Flashversion, Titel, Host, UACode)).Close()
            client.Dispose()
        Catch
        End Try
    End Sub

    Function InstalliereUpdate() As Boolean 'Wenn update vorhande true sonst false
        If System.IO.File.Exists(ProgrammPfad & "/Update/Versionen.lst") Then
            Process.Start("""" & ProgrammPfad & "/Update.exe""", """" & ProgrammName & """ """ & ProgrammExe & """")
            Application.Exit()
            Return True
        End If
    End Function

    Friend Function SucheNeueDateien(ByVal LokaleVersionen As VersionenDatei, ByVal UpdateVersionen As VersionenDatei) As Dateien
        Dim tmpDateien As New Dateien
        If UpdateVersionen.InterneVersion > LokaleVersionen.InterneVersion Then
            For i As Int16 = 0 To UpdateVersionen.Kategorien.Count - 1
                If InstallierteKategorien Is Nothing OrElse UpdateVersionen.Kategorien(i).Pflicht OrElse Array.IndexOf(InstallierteKategorien, UpdateVersionen.Kategorien(i).Name) > -1 Then
                    If LokaleVersionen.Kategorien.IndexOf(UpdateVersionen.Kategorien(i).Name) > -1 Then 'Lokale Versionen sind vorhanden
                        '=> neuere Versionen suchen
                        For j As Int16 = 0 To UpdateVersionen.Kategorien(i).Dateien.Count - 1
                            If LokaleVersionen.Kategorien(LokaleVersionen.Kategorien.IndexOf(UpdateVersionen.Kategorien(i).Name)).Dateien.IndexOf(UpdateVersionen.Kategorien(i).Dateien(j).Name) = -1 OrElse UpdateVersionen.Kategorien(i).Dateien(j).InterneVersion > LokaleVersionen.Kategorien(LokaleVersionen.Kategorien.IndexOf(UpdateVersionen.Kategorien(i).Name)).Dateien(LokaleVersionen.Kategorien(LokaleVersionen.Kategorien.IndexOf(UpdateVersionen.Kategorien(i).Name)).Dateien.IndexOf(UpdateVersionen.Kategorien(i).Dateien(j).Name)).InterneVersion OrElse (Not System.IO.File.Exists(UpdateVersionen.Kategorien(i).Dateien(j).Name)) Then
                                tmpDateien.Add(UpdateVersionen.Kategorien(i).Dateien(j).Name, UpdateVersionen.Kategorien(i).Dateien(j).InterneVersion)
                            End If
                        Next j
                    Else 'Lokale Versionen zu dieser Kategorie sind nicht vorhanden
                        '=> alle aus dieser Kategorie aktualisieren
                        For j As Int16 = 0 To UpdateVersionen.Kategorien(i).Dateien.Count - 1
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
        tmpHistory.�bersetzen = �bersetzen
        If ParentForm Is Nothing Then tmpHistory.StartPosition = FormStartPosition.CenterScreen
        tmpHistory.ShowDialog(ParentForm)
        tmpHistory.Dispose()
        tmpHistory = Nothing
    End Sub
End Class

Class VersionenDatei
    Friend Version As String = 0, InterneVersion As Int32 = 0, ReleasNotesUrl As String
    Friend Kategorien As New Kategorien

    Sub �ffnen(ByVal Datei As String, ByVal IstLokal As Boolean)
        Dim Stream As System.IO.Stream
        If IstLokal Then
            Stream = New IO.FileStream(Datei, IO.FileMode.Open, IO.FileAccess.Read)
        Else
            Dim Client As New System.Net.WebClient

            'Stream = Client.OpenRead(Datei)
            Stream = New System.IO.Compression.GZipStream(Client.OpenRead(Datei), IO.Compression.CompressionMode.Decompress)
        End If

        Dim tmpKategorienIndex As Int16 = -1, tmp As String
        Dim Reader As New System.IO.StreamReader(Stream, True)
        Dim UpdateVersion As Byte = Reader.ReadLine
        Version = Reader.ReadLine
        InterneVersion = Reader.ReadLine
        ReleasNotesUrl = Reader.ReadLine
        Do Until Reader.Peek = -1
            tmp = Reader.ReadLine
            If tmp.Length > 1 AndAlso tmp.Substring(0, 2) = "K:" Then
                tmpKategorienIndex = Kategorien.Add(tmp.Substring(2, tmp.IndexOf(":", 2) - 2), tmp.Substring(tmp.IndexOf(":", 2) + 1))
            ElseIf tmp.Length > 1 AndAlso tmp.Substring(0, 2) = "D:" Then
                tmpKategorienIndex = -2
            ElseIf tmpKategorienIndex = -2 Then
                'Dateien zum L�schen
            ElseIf tmpKategorienIndex > -1 Then
                'Dateien in Kategorien
                Kategorien(tmpKategorienIndex).Dateien.Add(tmp, Reader.ReadLine)
            End If
        Loop
        Reader.Close()
        Stream.Close()
    End Sub
End Class

Class Kategorien
    Friend kKategorie() As Kategorie

    Default Property Kategorie(ByVal Index As Int16) As Kategorie
        Get
            Return kKategorie(Index)
        End Get
        Set(ByVal value As Kategorie)
            kKategorie(Index) = value
        End Set
    End Property

    Function Add(ByVal Name As String, ByVal Pflicht As Boolean)
        Name = Name.Trim.ToLower
        If IndexOf(Name) = -1 Then
            ReDim Preserve kKategorie(Count)
            kKategorie(Count - 1) = New Kategorie(Name, Pflicht)
            Return Count - 1
        Else
            Return IndexOf(Name)
        End If
    End Function

    Function IndexOf(ByVal Name As String)
        For i As Int16 = 0 To Count - 1
            If kKategorie(i).Name = Name Then Return i
        Next i
        Return -1
    End Function

    ReadOnly Property Count() As Int16
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

    Default Property Datei(ByVal Index As Int16) As Datei
        Get
            Return dDatei(Index)
        End Get
        Set(ByVal value As Datei)
            dDatei(Index) = value
        End Set
    End Property

    Function Add(ByVal Name As String, ByVal InterneVersion As Int32)
        If IndexOf(Name) = -1 Then
            ReDim Preserve dDatei(Count)
            dDatei(Count - 1) = New Datei(Name.Replace("\", "/"), InterneVersion)
            Return Count - 1
        Else
            Return IndexOf(Name)
        End If
    End Function

    Function IndexOf(ByVal Name As String)
        Name = Name.Trim.ToLower.Replace("\", "/")
        For i As Int16 = 0 To Count - 1
            If dDatei(i).Name.Trim.ToLower = Name Then Return i
        Next i
        Return -1
    End Function

    ReadOnly Property Count() As Int16
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