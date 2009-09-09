Public Class frmLizenz
    Dim tmpLizenzen() As String

    Private Sub frmLizenz_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        lblReleasenotes.Text = System.String.Format(lblReleasenotes.Text, System.Environment.GetCommandLineArgs(1))
        Dim tmpDatei As String
        Dim tmpReader As System.IO.StreamReader
        'Lizenzen aus Update verzeichnis(haben vorrang)
        Try
            For Each Datei As String In System.IO.Directory.GetFiles(Application.StartupPath & "/Update/", "Lizenz-*.txt")
                tmpReader = New IO.StreamReader(Datei)
                cmbSprachen.Items.Add(tmpReader.ReadLine)
                tmpReader.Close()
                tmpDatei = System.IO.Path.GetFileNameWithoutExtension(Datei)
                If tmpLizenzen Is Nothing Then ReDim tmpLizenzen(0) Else ReDim Preserve tmpLizenzen(tmpLizenzen.Length)
                tmpLizenzen(tmpLizenzen.GetUpperBound(0)) = tmpDatei.Substring(7)
            Next
        Catch
        End Try

        'Lizenzen aus hauptverzeichnis
        Try
            For Each Datei As String In System.IO.Directory.GetFiles(Application.StartupPath & "/", "Lizenz-*.txt")
                Try
                    tmpDatei = System.IO.Path.GetFileNameWithoutExtension(Datei)
                    If tmpLizenzen Is Nothing OrElse Array.IndexOf(tmpLizenzen, tmpDatei.Substring(7)) = -1 Then '=>Datei nicht in update verzeichnis
                        tmpReader = New IO.StreamReader(Datei)

                        cmbSprachen.Items.Add(tmpReader.ReadLine)
                        tmpReader.Close()
                        If tmpLizenzen Is Nothing Then ReDim tmpLizenzen(0) Else ReDim Preserve tmpLizenzen(tmpLizenzen.Length)
                        tmpLizenzen(tmpLizenzen.GetUpperBound(0)) = tmpDatei.Substring(7)
                    End If
                Catch
                End Try
            Next
        Catch
        End Try
        Dim tmpIndex As Int32 = cmbSprachen.Items.IndexOf(My.Application.Culture.NativeName.Substring(0, My.Application.Culture.EnglishName.IndexOf(" (") + 1))
        If tmpIndex = -1 Then
            cmbSprachen.SelectedIndex = 0
        Else
            cmbSprachen.SelectedIndex = tmpIndex
        End If
        'Releasenotes
        If frmUpdate.ReleasNotesUrl.Trim = "" Then
            lblReleasenotes.Visible = False
        End If
    End Sub

    Private Sub cmbSprachen_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbSprachen.SelectedIndexChanged
        If cmbSprachen.SelectedIndex > -1 Then
            Try
                If System.IO.File.Exists(Application.StartupPath & "/Update/Lizenz-" & tmpLizenzen(cmbSprachen.SelectedIndex) & ".txt") Then
                    Dim Reader As New System.IO.StreamReader(Application.StartupPath & "/Update/Lizenz-" & tmpLizenzen(cmbSprachen.SelectedIndex) & ".txt", True)
                    Reader.ReadLine()
                    txtLizenz.Text = Reader.ReadToEnd
                    Reader.Close()
                ElseIf System.IO.File.Exists(Application.StartupPath & "/Lizenz-" & tmpLizenzen(cmbSprachen.SelectedIndex) & ".txt") Then
                    Dim Reader As New System.IO.StreamReader(Application.StartupPath & "/Lizenz-" & tmpLizenzen(cmbSprachen.SelectedIndex) & ".txt", True)
                    Reader.ReadLine()
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
        Try
            Process.Start(frmUpdate.ReleasNotesUrl) '"http://www.mal-was-anderes.de/programme/karteikasten/releasenotes.txt")
        Catch
        End Try
    End Sub
End Class