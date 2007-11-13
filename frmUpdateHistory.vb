Public Class frmUpdateHistory
    Friend ‹bersetzen As dllSprache.cls‹bersetzen, Programmpfad As String

    Private Sub frmUpdateHistory_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ‹bersetzen.‹bersetzeControl(Me)
        lstUpdates.Items.Clear()
        If System.IO.File.Exists(Programmpfad & "/UpdateHistory.txt") Then
            Dim Reader As New System.IO.StreamReader(Programmpfad & "/UpdateHistory.txt")
            Dim tmp() As String
            Do Until Reader.Peek = -1
                Try
                    tmp = Reader.ReadLine().Split("|")
                    lstUpdates.Items.Add(‹bersetzen.‹bersetze("VersionErfolgreichInstalliert", "{0}: Version {1} erfolgreich installiert.", CDate(tmp(0)).ToShortDateString, tmp(1)))
                Catch
                End Try
            Loop
        End If
    End Sub

    Private Sub cmdSchlieﬂen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSchlieﬂen.Click
        Me.Close()
    End Sub
End Class