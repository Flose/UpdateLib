Public Class frmUpdate
    Friend ReleasNotesUrl As String
    Friend InstallationsPfad As String
    Private Sub frmUpdate_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If System.Environment.GetCommandLineArgs.GetUpperBound(0) > 1 Then
            Me.Text = System.String.Format(Me.Text, System.Environment.GetCommandLineArgs(1))

            Dim tmpVersion As String = "0"
            If System.IO.File.Exists(Environment.CurrentDirectory & "/UpdateDll.dll") Then
                InstallationsPfad = Environment.CurrentDirectory & "/"
            Else
                InstallationsPfad = Application.StartupPath & "/"
            End If
            Dim DateienZumLöschen As New List(Of String)
            If System.IO.File.Exists(Application.StartupPath & "/Update/Versionen.lst") Then
                'Releasenotes, Version, Löschen aus Versionen.lst lesen
                Dim tmp As String, tmpKategorienIndex As Int16
                Dim Reader As New System.IO.StreamReader(Application.StartupPath & "/Update/Versionen.lst")
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
                Reader.Close()
            End If


            If System.IO.Directory.GetFiles(Application.StartupPath & "/Update/", "Lizenz-*.txt").Length > 0 Then
                If frmLizenz.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
                    End
                End If
            End If
            Me.Show()
            lblDatei.Text = "Updaten..."


            VerschiebeVerzeichnis(Application.StartupPath & "/Update/", InstallationsPfad)

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

            Try
                System.IO.File.Copy(Application.ExecutablePath, InstallationsPfad & "Update.exe", True)
            Catch
            End Try

            Try
                Dim UpdateWriter As New System.IO.StreamWriter(InstallationsPfad & "UpdateHistory.txt", True)
                UpdateWriter.WriteLine(Now & "|" & tmpVersion)
                UpdateWriter.Close()
            Catch
            End Try
            MessageBox.Show("Update wurde erfolgreich installiert.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information)
            If Environment.OSVersion.Platform = PlatformID.Unix Then
                Process.Start("mono """ & InstallationsPfad & System.Environment.GetCommandLineArgs(2).Trim & """")
            Else
                Process.Start("""" & InstallationsPfad & System.Environment.GetCommandLineArgs(2).Trim & """")
            End If
        End If
        End
    End Sub

    Sub VerschiebeVerzeichnis(ByVal Von As String, ByVal Nach As String)
        System.IO.Directory.CreateDirectory(Nach)
        For Each Datei As String In System.IO.Directory.GetFiles(Von)
NochMal:
            Try
                My.Computer.FileSystem.MoveFile(Datei, Nach & System.IO.Path.GetFileName(Datei), True)
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
            VerschiebeVerzeichnis(Pfad & "/", Nach & Pfad.Substring(Pfad.LastIndexOf("/") + 1) & "/")
        Next
        Threading.Thread.Sleep(100) 'sonst manchmal dateihandle noch nicht weg
        Try
            System.IO.Directory.Delete(Von, True)
        Catch
        End Try
    End Sub
End Class