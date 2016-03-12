Public Class OverlayUi
    Private WithEvents _update As Update

    Public Sub New()
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Hide()
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
        Hide()
        lblText.Text = _update.t.Translate("msgUpdateErfolgreich", _update.TranslatedProgramName)
        lblText.Show()
        cmdAction.Tag = "Restart"
        cmdAction.Text = _update.t.Translate("Neustarten")
        cmdAction.Show()
        Show()
    End Sub

    Private Sub _update_UpdateError(sender As Object, e As ErrorEventArgs) Handles _update.UpdateError
        Hide()
        lblWarning.Text = e.Message
        lblWarning.Show()
        Show()
    End Sub

    Private Sub _update_UpdateFound(sender As Object, e As UpdateFoundEventArgs) Handles _update.UpdateFound
        Hide()
        If e.FrameworkInstallStatus = UpdateLib.Update.InstallStatus.NotInstalled Then
            lblWarning.Text = _update.t.Translate("WarningNetFrameworkTooOld", e.Framework)
            lblWarning.Show()
        ElseIf e.FrameworkInstallStatus = UpdateLib.Update.InstallStatus.Unknown Then
            lblWarning.Text = _update.t.Translate("WarningNetFrameworkUnknown", e.Framework)
            lblWarning.Show()
        End If
        If e.FrameworkInstallStatus <> UpdateLib.Update.InstallStatus.Installed Then
            If Not String.IsNullOrEmpty(e.FrameworkUrl) Then
                LinkLabelWarning.Text = _update.t.Translate("NetFrameworkDownload")
                LinkLabelWarning.Links(0).LinkData = e.FrameworkUrl
                LinkLabelWarning.Show()
            End If
        End If

        lblText.Text = _update.t.Translate("msgUpdateVorhanden", e.DisplayVersion)
        lblText.Show()
        If Not String.IsNullOrEmpty(e.ReleaseNotesUrl) Then
            LinkLabel.Text = _update.t.Translate("ShowReleasenotes")
            LinkLabel.Links(0).LinkData = e.ReleaseNotesUrl
            LinkLabel.Show()
        End If
        cmdAction.Tag = "Download"
        cmdAction.Text = _update.t.Translate("Herunterladen")
        cmdAction.Show()
        Show()
    End Sub

    Private Sub _update_UpdateInfo(sender As Object, e As InfoEventArgs) Handles _update.UpdateInfo
        Hide()
        lblText.Text = e.Message
        lblText.Show()
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

    Private Sub LinkLabel_LinkClicked(sender As Object, e As Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel.LinkClicked, LinkLabelWarning.LinkClicked
        e.Link.Visited = True
        Dim target As String = CType(e.Link.LinkData, String)

        If target IsNot Nothing Then
            Diagnostics.Process.Start(target)
        End If
    End Sub

    Private Shadows Sub Show()
        If Parent IsNot Nothing Then
            TableLayoutPanel1.MaximumSize = New Drawing.Size(2 * Parent.Size.Width \ 3, TableLayoutPanel1.MaximumSize.Height)
        End If
        MyBase.Show()
        BringToFront()
    End Sub

    Private Shadows Sub Hide()
        MyBase.Hide()
        lblText.Hide()
        LinkLabel.Hide()
        lblWarning.Hide()
        LinkLabelWarning.Hide()
        cmdAction.Hide()
    End Sub
End Class
