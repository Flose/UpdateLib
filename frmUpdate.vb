Public Class frmUpdate
    Delegate Sub Aktualisieren_Callback(ByVal lblAktuelleDatei As String, ByVal Fortschritt As Int32, ByVal Max As Int32, ByVal Caption As String)

    Sub Aktualisieren(ByVal lblAktuelleDatei As String, ByVal Fortschritt As Int32, ByVal Max As Int32, ByVal Caption As String)
        If Me.InvokeRequired Then
            Dim d As New Aktualisieren_Callback(AddressOf Aktualisieren)
            Me.Invoke(d, New Object() {lblAktuelleDatei, Fortschritt, Max, Caption})
        Else
            Me.lblAktuelleDatei.Text = lblAktuelleDatei
            pgbUpdate.Maximum = Max
            pgbUpdate.Value = Fortschritt
            Me.Text = Caption
        End If
    End Sub
End Class