Public Class frmLizenz
    Dim tmpLizenzen As New List(Of String)

    Public Sub New()
        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        ' Search licences in the update folder (take precedence)
        Try
            For Each file As String In IO.Directory.GetFiles(UpdatePath, "Lizenz-*.txt")
                cmbSprachen.Items.Add(ReadLicenseLanguage(file))
                tmpLizenzen.Add(IO.Path.GetFileName(file))
            Next
        Catch
        End Try

        ' Load licences from he program path
        Try
            For Each file As String In IO.Directory.GetFiles(InstallationPath, "Lizenz-*.txt")
                Try
                    Dim fileName = IO.Path.GetFileName(file)
                    If Not tmpLizenzen.Contains(fileName) Then '=>Datei nicht in update verzeichnis
                        cmbSprachen.Items.Add(ReadLicenseLanguage(file))
                        tmpLizenzen.Add(fileName)
                    End If
                Catch
                End Try
            Next
        Catch
        End Try
        Dim tmpIndex = cmbSprachen.Items.IndexOf(My.Application.Culture.NativeName.Substring(0, My.Application.Culture.EnglishName.IndexOf(" (") + 1))
        If tmpIndex = -1 Then
            cmbSprachen.SelectedIndex = 0
        Else
            cmbSprachen.SelectedIndex = tmpIndex
        End If
    End Sub

    Private Sub cmbSprachen_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbSprachen.SelectedIndexChanged
        If cmbSprachen.SelectedIndex = -1 Then
            Return
        End If

        Try
            If IO.File.Exists(UpdatePath & tmpLizenzen(cmbSprachen.SelectedIndex)) Then
                txtLizenz.Text = ReadLicense(UpdatePath & tmpLizenzen(cmbSprachen.SelectedIndex))
            ElseIf IO.File.Exists(InstallationPath & tmpLizenzen(cmbSprachen.SelectedIndex)) Then
                txtLizenz.Text = ReadLicense(InstallationPath & tmpLizenzen(cmbSprachen.SelectedIndex))
            Else
                txtLizenz.Text = String.Empty
            End If
        Catch
            txtLizenz.Text = String.Empty
        End Try
    End Sub

    Private Function ReadLicenseLanguage(file As String) As String
        Using Reader As New IO.StreamReader(file, True)
            Return Reader.ReadLine()
        End Using
    End Function

    Private Function ReadLicense(file As String) As String
        Using Reader As New IO.StreamReader(file, True)
            Reader.ReadLine()
            Return Reader.ReadToEnd
        End Using
    End Function
End Class
