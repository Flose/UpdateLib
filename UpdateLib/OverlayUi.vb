Public Class OverlayUi
    Private WithEvents _update As Update

    Public Sub New()
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Me.Visible = False
    End Sub

    Public Sub New(update As Update)
        Me.New()
        Me.UpdateHandler = update
    End Sub

    Public WriteOnly Property UpdateHandler As Update
        Set(value As Update)
            If _update IsNot Nothing Then
                RemoveHandler _update.t.TranslationChanged, AddressOf OnTranslationChanged
            End If
            _update = value
            OnTranslationChanged(_update.t, Nothing)
            If _update IsNot Nothing Then
                AddHandler _update.t.TranslationChanged, AddressOf OnTranslationChanged
            End If
        End Set
    End Property

    Private Sub OnTranslationChanged(sender As Object, e As TranslationLib.TranslationChangedEventArgs)
        lblTitle.Text = _update.TranslatedTitle
    End Sub

    Private Sub _update_UpdateDownloaded(sender As Object, e As EventArgs) Handles _update.UpdateDownloaded
        lblText.Text = _update.t.Translate("msgUpdateErfolgreich", Environment.NewLine, _update.TranslatedProgramName)
        LinkLabel.Visible = False
        cmdAction.Tag = "Restart"
        cmdAction.Text = _update.t.Translate("Neustarten")
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
            additionalText = Environment.NewLine + Environment.NewLine + _update.t.Translate("WarningNetFrameworkTooOld", e.Framework)
        ElseIf e.FrameworkInstallStatus = UpdateLib.Update.InstallStatus.Unknown Then
            additionalText = Environment.NewLine + Environment.NewLine + _update.t.Translate("WarningNetFrameworkUnknown", e.Framework)
        End If

        lblText.Text = _update.t.Translate("msgUpdateVorhanden", e.DisplayVersion, Environment.NewLine) + additionalText
        LinkLabel.Text = _update.t.Translate("ShowReleasenotes")
        LinkLabel.Links(0).LinkData = e.ReleaseNotesUrl
        LinkLabel.Visible = True
        cmdAction.Tag = "Download"
        cmdAction.Text = _update.t.Translate("Herunterladen")
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
            Case "Download"
                _update.DownloadUpdateAsync()
            Case "Restart"
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
