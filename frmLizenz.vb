Public Class frmLizenz
    Private Sub frmLizenz_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        lblReleasenotes.Text = System.String.Format(lblReleasenotes.Text, System.Environment.GetCommandLineArgs(1))
        Dim tmpDatei As String
        'Lizenzen aus Update verzeichnis(haben vorrang)
        For Each Datei As String In System.IO.Directory.GetFiles(Application.StartupPath & "\Update\", "Lizenz-*.txt")
            tmpDatei = System.IO.Path.GetFileNameWithoutExtension(Datei)
            cmbSprachen.Items.Add(tmpDatei.Substring(7))
        Next

        'Lizenzen aus hauptverzeichnis
        For Each Datei As String In System.IO.Directory.GetFiles(Application.StartupPath & "\", "Lizenz-*.txt")
            tmpDatei = System.IO.Path.GetFileNameWithoutExtension(Datei)
            If cmbSprachen.Items.IndexOf(tmpDatei) = -1 Then '=>Datei nicht in update verzeichnis
                cmbSprachen.Items.Add(tmpDatei.Substring(7))
            End If
        Next
        cmbSprachen.SelectedIndex = 0
    End Sub

    Private Sub cmbSprachen_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbSprachen.SelectedIndexChanged
        If cmbSprachen.SelectedIndex > -1 Then
            Try
                If System.IO.File.Exists(Application.StartupPath & "\Update\Lizenz-" & CStr(cmbSprachen.SelectedItem) & ".txt") Then
                    Dim Reader As New System.IO.StreamReader(Application.StartupPath & "\Update\Lizenz-" & CStr(cmbSprachen.SelectedItem) & ".txt", True)
                    txtLizenz.Text = Reader.ReadToEnd
                    Reader.Close()
                ElseIf System.IO.File.Exists(Application.StartupPath & "\Lizenz-" & CStr(cmbSprachen.SelectedItem) & ".txt") Then
                    Dim Reader As New System.IO.StreamReader(Application.StartupPath & "\Lizenz-" & CStr(cmbSprachen.SelectedItem) & ".txt", True)
                    txtLizenz.Text = Reader.ReadToEnd
                    Reader.Close()
                Else
                    txtLizenz.Text = ""
                End If
            Catch ex As Exception
                txtLizenz.Text = ""
            End Try
        End If
    End Sub

    Private Sub cmdOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOk.Click
        Me.Close()
    End Sub

    Private Sub lblReleasenotes_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lblReleasenotes.LinkClicked
        lblReleasenotes.LinkVisited = True
        Process.Start("http://www.mal-was-anderes.de/programme/karteikasten/releasenotes.txt")
    End Sub
End Class