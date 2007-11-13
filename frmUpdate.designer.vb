<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmUpdate
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUpdate))
        Me.lblAktuelleDatei = New System.Windows.Forms.Label
        Me.pgbUpdate = New System.Windows.Forms.ProgressBar
        Me.SuspendLayout()
        '
        'lblAktuelleDatei
        '
        Me.lblAktuelleDatei.AutoSize = True
        Me.lblAktuelleDatei.Location = New System.Drawing.Point(12, 9)
        Me.lblAktuelleDatei.Name = "lblAktuelleDatei"
        Me.lblAktuelleDatei.Size = New System.Drawing.Size(76, 13)
        Me.lblAktuelleDatei.TabIndex = 0
        Me.lblAktuelleDatei.Tag = "UpdateAktuelleDatei"
        Me.lblAktuelleDatei.Text = "Aktuelle Datei:"
        '
        'pgbUpdate
        '
        Me.pgbUpdate.Location = New System.Drawing.Point(12, 25)
        Me.pgbUpdate.Name = "pgbUpdate"
        Me.pgbUpdate.Size = New System.Drawing.Size(284, 23)
        Me.pgbUpdate.TabIndex = 1
        '
        'frmUpdate
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(308, 55)
        Me.ControlBox = False
        Me.Controls.Add(Me.pgbUpdate)
        Me.Controls.Add(Me.lblAktuelleDatei)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmUpdate"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Tag = "Update"
        Me.Text = "Update"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblAktuelleDatei As System.Windows.Forms.Label
    Friend WithEvents pgbUpdate As System.Windows.Forms.ProgressBar
End Class
