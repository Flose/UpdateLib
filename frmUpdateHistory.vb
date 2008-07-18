Public Class frmUpdateHistory
    Friend Übersetzen As dllSprache.clsÜbersetzen, Programmpfad As String

    Private Sub frmUpdateHistory_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Übersetzen.ÜbersetzeControl(Me)
        lstUpdates.Items.Clear()
        If System.IO.File.Exists(Programmpfad & "/UpdateHistory.txt") Then
            Dim Reader As New System.IO.StreamReader(Programmpfad & "/UpdateHistory.txt")
            Dim tmp() As String
            Do Until Reader.Peek = -1
                Try
                    tmp = Reader.ReadLine().Split("|")
                    lstUpdates.Items.Add(Übersetzen.Übersetze("VersionErfolgreichInstalliert", CDate(tmp(0)).ToShortDateString, tmp(1)))
                Catch
                End Try
            Loop
            Reader.Close()
        End If
    End Sub

    Private Sub cmdSchließen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSchließen.Click
        Me.Close()
    End Sub
End Class