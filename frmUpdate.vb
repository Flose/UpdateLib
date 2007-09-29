Public Class frmUpdate

    Private Sub frmUpdate_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If System.Environment.GetCommandLineArgs.GetUpperBound(0) > 1 Then
            Me.Text = System.String.Format(Me.Text, System.Environment.GetCommandLineArgs(1))

            If System.IO.Directory.GetFiles(Application.StartupPath & "\Update\", "Lizenz-*.txt").Length > 0 Then
                If frmLizenz.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
                    End
                End If
            End If


            Me.Show()

            lblDatei.Text = "Beenden Sie das Programm"
            If System.IO.File.Exists(System.Environment.GetCommandLineArgs(2)) Then
                Dim Writer As System.IO.FileStream
                Do
                    Try
                        Writer = New System.IO.FileStream(System.Environment.GetCommandLineArgs(2), IO.FileMode.Open)
                        Writer.Close()
                        Exit Do
                    Catch ex As Exception
                        Threading.Thread.Sleep(100)
                        Application.DoEvents()
                    End Try
                Loop
            End If
            lblDatei.Text = "Updaten..."

            My.Computer.FileSystem.MoveDirectory(Application.StartupPath & "\Update", Application.StartupPath, True)
            My.Computer.FileSystem.MoveFile(Application.StartupPath & "\Update.txt", Application.StartupPath & "\Versionen.txt", True)

            Dim Reader As New System.IO.StreamReader(Application.StartupPath & "\Versionen.txt")
            Reader.ReadLine()
            Dim tmpVersion As String = Reader.ReadLine()
            Reader.Close()

            Dim UpdateWriter As New System.IO.StreamWriter("UpdateHistory.txt", True)
            UpdateWriter.WriteLine(Now & "|" & tmpVersion)
            UpdateWriter.Close()
            MessageBox.Show("Update wurde erfolgreich installiert.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Process.Start(System.Environment.GetCommandLineArgs(1), "")
        End If
        End
    End Sub
End Class