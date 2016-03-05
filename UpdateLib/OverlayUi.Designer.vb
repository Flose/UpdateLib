<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OverlayUi
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.cmdClose = New System.Windows.Forms.Button()
        Me.lblText = New System.Windows.Forms.Label()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.LinkLabel = New System.Windows.Forms.LinkLabel()
        Me.cmdAction = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.cmdClose, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblText, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.lblTitle, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.LinkLabel, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.cmdAction, 0, 3)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(116, 84)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'cmdClose
        '
        Me.cmdClose.AutoSize = True
        Me.cmdClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.cmdClose.Location = New System.Drawing.Point(89, 3)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(24, 23)
        Me.cmdClose.TabIndex = 1
        Me.cmdClose.Text = "X"
        Me.cmdClose.UseVisualStyleBackColor = True
        '
        'lblText
        '
        Me.lblText.AutoSize = True
        Me.lblText.Location = New System.Drawing.Point(3, 29)
        Me.lblText.Name = "lblText"
        Me.lblText.Size = New System.Drawing.Size(28, 13)
        Me.lblText.TabIndex = 2
        Me.lblText.Text = "Text"
        '
        'lblTitle
        '
        Me.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Location = New System.Drawing.Point(3, 8)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(27, 13)
        Me.lblTitle.TabIndex = 3
        Me.lblTitle.Text = "Title"
        '
        'LinkLabel
        '
        Me.LinkLabel.AutoSize = True
        Me.LinkLabel.Location = New System.Drawing.Point(3, 42)
        Me.LinkLabel.Name = "LinkLabel"
        Me.LinkLabel.Size = New System.Drawing.Size(52, 13)
        Me.LinkLabel.TabIndex = 4
        Me.LinkLabel.TabStop = True
        Me.LinkLabel.Text = "Link label"
        '
        'cmdAction
        '
        Me.cmdAction.AutoSize = True
        Me.cmdAction.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.cmdAction.Location = New System.Drawing.Point(3, 58)
        Me.cmdAction.Name = "cmdAction"
        Me.cmdAction.Size = New System.Drawing.Size(80, 23)
        Me.cmdAction.TabIndex = 0
        Me.cmdAction.Text = "Action button"
        Me.cmdAction.UseVisualStyleBackColor = True
        '
        'OverlayUi
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.Khaki
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "OverlayUi"
        Me.Size = New System.Drawing.Size(119, 87)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Private WithEvents cmdClose As Windows.Forms.Button
    Private WithEvents lblText As Windows.Forms.Label
    Private WithEvents cmdAction As Windows.Forms.Button
    Private WithEvents lblTitle As Windows.Forms.Label
    Private WithEvents LinkLabel As Windows.Forms.LinkLabel
End Class
