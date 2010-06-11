Friend Class frmUpdateHistory
    Friend Sub New(ByVal Übersetzen As dllSprache.clsÜbersetzen, ByVal Programmpfad As String)
        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Übersetzen.ÜbersetzeControl(Me)
        lstUpdates.Items.Clear()
        If System.IO.File.Exists(IO.Path.Combine(Programmpfad, "UpdateHistory.txt")) Then
            Using Reader As New System.IO.StreamReader(IO.Path.Combine(Programmpfad, "UpdateHistory.txt"))
                Dim tmp() As String
                Do Until Reader.Peek = -1
                    Try
                        tmp = Reader.ReadLine().Split("|"c)
                        lstUpdates.Items.Add(Übersetzen.Übersetze("VersionErfolgreichInstalliert", CDate(tmp(0)).ToShortDateString, tmp(1)))
                    Catch
                    End Try
                Loop
            End Using
        End If
    End Sub

    Private Sub cmdSchließen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSchließen.Click
        Me.Close()
    End Sub
End Class