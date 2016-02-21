Public Class frmLizenz
    Dim tmpLizenzen As New List(Of String)

    Public Sub New()
        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        Dim tmpReader As System.IO.StreamReader
        'Lizenzen aus Update verzeichnis(haben vorrang)
        Try
            For Each Datei As String In System.IO.Directory.GetFiles(UpdatePfad, "Lizenz-*.txt")
                tmpReader = New IO.StreamReader(Datei)
                cmbSprachen.Items.Add(tmpReader.ReadLine)
                tmpReader.Close()
                tmpLizenzen.Add(System.IO.Path.GetFileName(Datei))
            Next
        Catch
        End Try

        Dim tmpDatei As String
        'Lizenzen aus hauptverzeichnis
        Try
            For Each Datei As String In System.IO.Directory.GetFiles(InstallationsPfad, "Lizenz-*.txt")
                Try
                    tmpDatei = System.IO.Path.GetFileName(Datei)
                    If Not tmpLizenzen.Contains(tmpDatei) Then '=>Datei nicht in update verzeichnis
                        tmpReader = New IO.StreamReader(Datei)
                        cmbSprachen.Items.Add(tmpReader.ReadLine)
                        tmpReader.Close()
                        tmpLizenzen.Add(tmpDatei)
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
    End Sub

    Private Sub cmbSprachen_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbSprachen.SelectedIndexChanged
        If cmbSprachen.SelectedIndex > -1 Then
            Try
                If System.IO.File.Exists(UpdatePfad & tmpLizenzen(cmbSprachen.SelectedIndex)) Then
                    Dim Reader As New System.IO.StreamReader(UpdatePfad & tmpLizenzen(cmbSprachen.SelectedIndex), True)
                    Reader.ReadLine()
                    txtLizenz.Text = Reader.ReadToEnd
                    Reader.Close()
                ElseIf System.IO.File.Exists(InstallationsPfad & tmpLizenzen(cmbSprachen.SelectedIndex)) Then
                    Dim Reader As New System.IO.StreamReader(InstallationsPfad & tmpLizenzen(cmbSprachen.SelectedIndex), True)
                    Reader.ReadLine()
                    txtLizenz.Text = Reader.ReadToEnd
                    Reader.Close()
                Else
                    txtLizenz.Text = String.Empty
                End If
            Catch
                txtLizenz.Text = String.Empty
            End Try
        End If
    End Sub
End Class