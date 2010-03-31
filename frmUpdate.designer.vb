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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblAktuelleDatei
        '
        Me.lblAktuelleDatei.AutoSize = True
        Me.lblAktuelleDatei.Location = New System.Drawing.Point(0, 0)
        Me.lblAktuelleDatei.Margin = New System.Windows.Forms.Padding(0)
        Me.lblAktuelleDatei.Name = "lblAktuelleDatei"
        Me.lblAktuelleDatei.Size = New System.Drawing.Size(76, 13)
        Me.lblAktuelleDatei.TabIndex = 0
        Me.lblAktuelleDatei.Tag = "UpdateAktuelleDatei"
        Me.lblAktuelleDatei.Text = "Aktuelle Datei:"
        '
        'pgbUpdate
        '
        Me.pgbUpdate.Location = New System.Drawing.Point(3, 16)
        Me.pgbUpdate.Name = "pgbUpdate"
        Me.pgbUpdate.Size = New System.Drawing.Size(292, 23)
        Me.pgbUpdate.TabIndex = 1
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.lblAktuelleDatei, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.pgbUpdate, 0, 1)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(8, 8)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(298, 42)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'frmUpdate
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ClientSize = New System.Drawing.Size(308, 55)
        Me.ControlBox = False
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmUpdate"
        Me.Padding = New System.Windows.Forms.Padding(5, 5, 2, 2)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Tag = "Update"
        Me.Text = "Update"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblAktuelleDatei As System.Windows.Forms.Label
    Friend WithEvents pgbUpdate As System.Windows.Forms.ProgressBar
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
End Class
