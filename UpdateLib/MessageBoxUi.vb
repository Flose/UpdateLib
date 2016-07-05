Imports System.Windows.Forms

Public Class MessageBoxUi
    Private WithEvents _update As Update

    Public Sub New(update As Update)
        Me._update = update
    End Sub

    Private Sub _update_UpdateDownloaded(sender As Object, e As EventArgs) Handles _update.UpdateDownloaded
        Dim result = MessageBox.Show(_update.t.Translate("msgUpdateErfolgreich", _update.TranslatedProgramName), _update.t.Translate("Update", _update.TranslatedProgramName), MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
        If DialogResult.Yes = result Then
            _update.InstallUpdate()
        End If
    End Sub

    Private Sub _update_UpdateError(sender As Object, e As ErrorEventArgs) Handles _update.UpdateError
        MessageBox.Show(e.Message, _update.TranslatedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Private Sub _update_UpdateFound(sender As Object, e As UpdateFoundEventArgs) Handles _update.UpdateFound
        Dim additionalText As String = ""
        If e.FrameworkInstallStatus = Update.InstallStatus.NotInstalled Then
            additionalText = Environment.NewLine + Environment.NewLine + __update.t.Translate("WarningNetFrameworkTooOld", e.Framework)
        ElseIf e.FrameworkInstallStatus = UpdateLib.Update.InstallStatus.Unknown Then
            additionalText = Environment.NewLine + Environment.NewLine + __update.t.Translate("WarningNetFrameworkUnknown", e.Framework)
        End If
        Dim result = MessageBox.Show(_update.t.Translate("msgUpdateVorhanden", e.DisplayVersion) + additionalText, _update.t.Translate("Update", _update.TranslatedProgramName), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        If DialogResult.Yes = result Then
            _update.DownloadUpdateAsync()
        Else
            _update.ResetUpdateState()
        End If
    End Sub

    Private Sub _update_UpdateInfo(sender As Object, e As InfoEventArgs) Handles _update.UpdateInfo
        MessageBox.Show(e.Message, _update.TranslatedTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class
