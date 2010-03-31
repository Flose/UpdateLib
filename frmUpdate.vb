Public Class frmUpdate

    Public Sub New()
        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Me.Text = System.String.Format(Me.Text, System.Environment.GetCommandLineArgs(1))
    End Sub
End Class