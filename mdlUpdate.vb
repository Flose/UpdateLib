Module mdlUpdate
    Friend ReleasNotesUrl As String
    Friend InstallationsPfad As String
    Friend UpdatePfad As String

    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        If System.Environment.GetCommandLineArgs.Length < 3 Then
            MessageBox.Show("Update.exe benötigt folgende Parameter:" & Environment.NewLine & "Update.exe [Programmname] [Ausführbahre .exe Datei]", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            UpdatePfad = IO.Path.Combine(Application.StartupPath, "Update") & IO.Path.DirectorySeparatorChar
            If Not System.IO.Directory.Exists(UpdatePfad) Then
                MessageBox.Show("Es ist kein Update vorhanden, das installiert werden könnte!", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Dim tmpVersion As String = "0"
                If System.IO.File.Exists(IO.Path.Combine(Environment.CurrentDirectory, "UpdateDll.dll")) OrElse _
                   System.IO.File.Exists(IO.Path.Combine(Environment.CurrentDirectory, "TranslationLib.dll")) Then
                    'Seit UpdateDLL 1.4 werden Updates in %ProgramData% gespeichert
                    InstallationsPfad = Environment.CurrentDirectory & IO.Path.DirectorySeparatorChar
                Else
                    'Davor im Programmpfad
                    InstallationsPfad = Application.StartupPath & IO.Path.DirectorySeparatorChar
                End If
                Dim DateienZumLöschen As New List(Of String)
                If System.IO.File.Exists(UpdatePfad & "Versionen.lst") Then
                    'Releasenotes, Version, Löschen aus Versionen.lst lesen
                    Dim tmp As String, tmpKategorienIndex As Int16
                    Using Reader As New System.IO.StreamReader(UpdatePfad & "Versionen.lst")
                        tmp = Reader.ReadLine
                        tmpVersion = Reader.ReadLine()
                        tmp = Reader.ReadLine
                        ReleasNotesUrl = Reader.ReadLine.Trim
                        Do Until Reader.Peek = -1
                            tmp = Reader.ReadLine
                            If tmp.Length > 1 AndAlso tmp.Substring(0, 2) = "K:" Then
                                tmpKategorienIndex = 3
                            ElseIf tmp.Length > 1 AndAlso tmp.Substring(0, 2) = "D:" Then
                                tmpKategorienIndex = -2
                            ElseIf tmpKategorienIndex = -2 Then
                                'Dateien zum Löschen
                                DateienZumLöschen.Add(InstallationsPfad & tmp)
                            ElseIf tmpKategorienIndex > -1 Then
                                'Dateien in Kategorien
                            End If
                        Loop
                    End Using
                End If

                If System.IO.Directory.GetFiles(UpdatePfad, "Lizenz-*.txt").Length > 0 Then
                    If frmLizenz.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                        Application.Exit()
                        Exit Sub
                    End If
                End If
                frmUpdate.Show()
                Application.DoEvents()

                VerschiebeVerzeichnis(UpdatePfad, InstallationsPfad)
                If DateienZumLöschen.Count > 0 Then
                    For Each file As String In DateienZumLöschen
                        Try
                            System.IO.File.Delete(file)
                        Catch
                            Try
                                System.IO.Directory.Delete(file)
                            Catch
                            End Try
                        End Try
                    Next
                End If

                'alle alten update-*.exe löschen
                For Each file As String In System.IO.Directory.GetFiles(Application.StartupPath, "Update-*.exe")
                    If file <> Application.ExecutablePath Then
                        Try
                            System.IO.File.Delete(file)
                        Catch
                        End Try
                    End If
                Next
                If Application.ExecutablePath <> InstallationsPfad & "Update.exe" Then
                    Try
                        System.IO.File.Copy(Application.ExecutablePath, InstallationsPfad & "Update.exe", True)
                    Catch
                    End Try
                End If

                Try
                    Using UpdateWriter As New System.IO.StreamWriter(InstallationsPfad & "UpdateHistory.txt", True)
                        UpdateWriter.WriteLine(Now & "|" & tmpVersion)
                    End Using
                Catch
                End Try
                MessageBox.Show("Update wurde erfolgreich installiert.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Dim ProgrammEXE As String = """" & InstallationsPfad & System.Environment.GetCommandLineArgs(2).Trim & """"
                If Environment.OSVersion.Platform = PlatformID.Unix Then
                    Dim StarProgramm As New Diagnostics.ProcessStartInfo("mono", ProgrammEXE)
                    StarProgramm.UseShellExecute = False
                    Try
                        Process.Start(StarProgramm)
                    Catch ex As Exception
                        MessageBox.Show("Fehler beim Ausführen von 'mono " & ProgrammEXE & "':" & Environment.NewLine & ex.Message)
                    End Try
                Else
                    Try
                        Process.Start(ProgrammEXE)
                    Catch ex As Exception
                        MessageBox.Show("Fehler beim Ausführen von " & ProgrammEXE & ":" & Environment.NewLine & ex.Message)
                    End Try
                End If
            End If
        End If
        Application.Exit()
    End Sub

    Sub VerschiebeVerzeichnis(ByVal Von As String, ByVal Nach As String)
        System.IO.Directory.CreateDirectory(Nach)
        For Each Datei As String In System.IO.Directory.GetFiles(Von)
NochMal:
            Try
                My.Computer.FileSystem.MoveFile(Datei, IO.Path.Combine(Nach, System.IO.Path.GetFileName(Datei)), True)
            Catch ex As Exception
                Dim tmpResult As DialogResult = MessageBox.Show("Fehler beim Aktualisieren der Datei " & System.IO.Path.GetFileName(Datei) & ":" & Environment.NewLine & ex.Message & Environment.NewLine & Environment.NewLine & "Überprüfen Sie ob das Programm noch läuft!", "Update", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2)
                If tmpResult = Windows.Forms.DialogResult.Abort Then
                    End
                ElseIf tmpResult = Windows.Forms.DialogResult.Retry Then
                    GoTo Nochmal
                ElseIf tmpResult = Windows.Forms.DialogResult.Ignore Then
                    'Nichts machen
                End If
            End Try
        Next
        For Each Pfad As String In System.IO.Directory.GetDirectories(Von)
            VerschiebeVerzeichnis(Pfad, IO.Path.Combine(Nach, Pfad.Substring(Pfad.LastIndexOf(IO.Path.DirectorySeparatorChar) + 1)))
        Next
        Threading.Thread.Sleep(100) 'sonst manchmal dateihandle noch nicht weg
        Try
            System.IO.Directory.Delete(Von, True)
        Catch
        End Try
    End Sub

End Module
