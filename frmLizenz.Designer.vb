<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLizenz
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.cmbSprachen = New System.Windows.Forms.ComboBox
        Me.txtLizenz = New System.Windows.Forms.TextBox
        Me.cmdOk = New System.Windows.Forms.Button
        Me.lblReleasenotes = New System.Windows.Forms.LinkLabel
        Me.SuspendLayout()
        '
        'cmbSprachen
        '
        Me.cmbSprachen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbSprachen.FormattingEnabled = True
        Me.cmbSprachen.Location = New System.Drawing.Point(12, 12)
        Me.cmbSprachen.Name = "cmbSprachen"
        Me.cmbSprachen.Size = New System.Drawing.Size(208, 21)
        Me.cmbSprachen.TabIndex = 0
        '
        'txtLizenz
        '
        Me.txtLizenz.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtLizenz.Location = New System.Drawing.Point(12, 39)
        Me.txtLizenz.Multiline = True
        Me.txtLizenz.Name = "txtLizenz"
        Me.txtLizenz.ReadOnly = True
        Me.txtLizenz.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtLizenz.Size = New System.Drawing.Size(472, 314)
        Me.txtLizenz.TabIndex = 1
        '
        'cmdOk
        '
        Me.cmdOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.cmdOk.Location = New System.Drawing.Point(211, 359)
        Me.cmdOk.Name = "cmdOk"
        Me.cmdOk.Size = New System.Drawing.Size(75, 23)
        Me.cmdOk.TabIndex = 2
        Me.cmdOk.Text = "O&k"
        Me.cmdOk.UseVisualStyleBackColor = True
        '
        'lblReleasenotes
        '
        Me.lblReleasenotes.AutoSize = True
        Me.lblReleasenotes.Location = New System.Drawing.Point(265, 15)
        Me.lblReleasenotes.Name = "lblReleasenotes"
        Me.lblReleasenotes.Size = New System.Drawing.Size(89, 13)
        Me.lblReleasenotes.TabIndex = 3
        Me.lblReleasenotes.TabStop = True
        Me.lblReleasenotes.Text = "{0} Releasenotes"
        '
        'frmLizenz
        '
        Me.AcceptButton = Me.cmdOk
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(496, 394)
        Me.Controls.Add(Me.lblReleasenotes)
        Me.Controls.Add(Me.cmdOk)
        Me.Controls.Add(Me.txtLizenz)
        Me.Controls.Add(Me.cmbSprachen)
        Me.Name = "frmLizenz"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Lizenz - license - licence - licencia - licenza"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmbSprachen As System.Windows.Forms.ComboBox
    Friend WithEvents txtLizenz As System.Windows.Forms.TextBox
    Friend WithEvents cmdOk As System.Windows.Forms.Button
    Friend WithEvents lblReleasenotes As System.Windows.Forms.LinkLabel
End Class
