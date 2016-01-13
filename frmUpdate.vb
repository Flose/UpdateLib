Friend Class frmUpdate
    Private Delegate Sub Aktualisieren_Callback(ByVal lblAktuelleDatei As String, ByVal Fortschritt As Int32, ByVal Max As Int32, ByVal Caption As String)

    Friend Sub Aktualisieren(ByVal lblAktuelleDatei As String, ByVal Fortschritt As Int32, ByVal Max As Int32, ByVal Caption As String)
        If Me.InvokeRequired Then
            Me.BeginInvoke(New Aktualisieren_Callback(AddressOf Aktualisieren), lblAktuelleDatei, Fortschritt, Max, Caption)
            Exit Sub
        End If

        Me.lblAktuelleDatei.Text = lblAktuelleDatei
        pgbUpdate.Maximum = Max
        pgbUpdate.Value = Fortschritt
        Me.Text = Caption
    End Sub
End Class