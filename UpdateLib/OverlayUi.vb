Imports FloseCode.UpdateLib

Public Class OverlayUi
    Private WithEvents _update As Update

    Public Sub New()
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
    End Sub

    Public Sub New(update As Update)
        Me.New()
        Me._update = update
    End Sub

    Public WriteOnly Property UpdateHandler As Update
        Set(value As Update)
            _update = value
        End Set
    End Property

    Private Sub _update_UpdateDownloaded(sender As Object, e As EventArgs) Handles _update.UpdateDownloaded
        lblText.Text = _update.t.Translate("msgUpdateErfolgreich", Environment.NewLine, _update.TranslatedProgramName)
        LinkLabel.Visible = False
        cmdAction.Tag = "Downloaded"
        cmdAction.Visible = True
        Show()
    End Sub

    Private Sub _update_UpdateError(sender As Object, e As ErrorEventArgs) Handles _update.UpdateError
        'TODO formatting
        lblText.Text = e.Message
        LinkLabel.Visible = False
        cmdAction.Visible = False
        Show()
    End Sub

    Private Sub _update_UpdateFound(sender As Object, e As UpdateFoundEventArgs) Handles _update.UpdateFound
        ' TODO separate label (bold) for warnings
        Dim additionalText As String = ""
        If e.FrameworkInstallStatus = UpdateLib.Update.InstallStatus.NotInstalled Then
            additionalText = String.Format(Environment.NewLine + Environment.NewLine + "Warning: Install .Net Framework {0} (or an equivalent Mono version) before installing the update. Otherwise the program will fail to start after the update.", e.Framework) 'TODO translate
        ElseIf e.FrameworkInstallStatus = UpdateLib.Update.InstallStatus.Unknown Then
            additionalText = String.Format(Environment.NewLine + Environment.NewLine + "Warning: .Net Framework {0} (or an equivalent Mono version) is necessary for this update. Make sure it’s installed.", e.Framework) 'TODO translate
        End If

        lblText.Text = _update.t.Translate("msgUpdateVorhanden", e.DisplayVersion, Environment.NewLine) + additionalText
        LinkLabel.Links(0).LinkData = e.ReleaseNotesUrl
        LinkLabel.Visible = True
        cmdAction.Tag = "Found"
        cmdAction.Visible = True
        Show()
    End Sub

    Private Sub _update_UpdateInfo(sender As Object, e As InfoEventArgs) Handles _update.UpdateInfo
        'TODO formating
        lblText.Text = e.Message
        LinkLabel.Visible = False
        cmdAction.Visible = False
        Show()
    End Sub

    Private Sub cmdAction_Click(sender As Object, e As EventArgs) Handles cmdAction.Click
        Select Case CStr(cmdAction.Tag)
            Case "Found"
                _update.DownloadUpdateAsync()
            Case "Downloaded"
                _update.InstallUpdate()
        End Select
        Hide()
    End Sub

    Private Sub cmdClose_Click(sender As Object, e As EventArgs) Handles cmdClose.Click
        _update.ResetUpdateState()
        Hide()
    End Sub

    Private Sub LinkLabel_LinkClicked(sender As Object, e As Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel.LinkClicked
        e.Link.Visited = True
        Dim target As String = CType(e.Link.LinkData, String)

        If target IsNot Nothing Then
            Diagnostics.Process.Start(target)
        End If
    End Sub
End Class
