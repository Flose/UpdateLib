Friend Class frmUpdate
    Private Delegate Sub Aktualisieren_Callback(ByVal lblAktuelleDatei As String, ByVal Fortschritt As Integer)

    Friend Sub New(titleText As String)
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Me.Text = titleText
    End Sub

    Friend Sub Aktualisieren(ByVal lblAktuelleDatei As String, ByVal Fortschritt As Integer)
        If InvokeRequired Then
            BeginInvoke(New Aktualisieren_Callback(AddressOf Aktualisieren), lblAktuelleDatei, Fortschritt)
            Exit Sub
        End If

        Me.lblAktuelleDatei.Text = lblAktuelleDatei
        pgbUpdate.Value = Fortschritt
    End Sub
End Class