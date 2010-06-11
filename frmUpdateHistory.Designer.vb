<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmUpdateHistory
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUpdateHistory))
        Me.Label1 = New System.Windows.Forms.Label
        Me.lstUpdates = New System.Windows.Forms.ListBox
        Me.cmdSchließen = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Tag = "Updates"
        Me.Label1.Text = "Updates:"
        '
        'lstUpdates
        '
        Me.lstUpdates.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstUpdates.FormattingEnabled = True
        Me.lstUpdates.Location = New System.Drawing.Point(12, 25)
        Me.lstUpdates.Name = "lstUpdates"
        Me.lstUpdates.Size = New System.Drawing.Size(245, 160)
        Me.lstUpdates.TabIndex = 1
        '
        'cmdSchließen
        '
        Me.cmdSchließen.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.cmdSchließen.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdSchließen.Location = New System.Drawing.Point(97, 191)
        Me.cmdSchließen.Name = "cmdSchließen"
        Me.cmdSchließen.Size = New System.Drawing.Size(75, 23)
        Me.cmdSchließen.TabIndex = 2
        Me.cmdSchließen.Tag = "Schließen"
        Me.cmdSchließen.Text = "Schließen"
        Me.cmdSchließen.UseVisualStyleBackColor = True
        '
        'frmUpdateHistory
        '
        Me.AcceptButton = Me.cmdSchließen
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdSchließen
        Me.ClientSize = New System.Drawing.Size(269, 224)
        Me.Controls.Add(Me.cmdSchließen)
        Me.Controls.Add(Me.lstUpdates)
        Me.Controls.Add(Me.Label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmUpdateHistory"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Tag = "UpdateHistory"
        Me.Text = "Update history"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents Label1 As System.Windows.Forms.Label
    Private WithEvents lstUpdates As System.Windows.Forms.ListBox
    Private WithEvents cmdSchließen As System.Windows.Forms.Button
End Class
