Imports System.Windows.Forms

Public Class MessageBoxUi
    Private WithEvents _update As Update

    Public Sub New(update As Update)
        Me._update = update
    End Sub

    Private Sub _update_UpdateDownloaded(sender As Object, e As EventArgs) Handles _update.UpdateDownloaded
        UpdateDownloaded(_update)
    End Sub

    Friend Shared Sub UpdateDownloaded(update As Update)
        Dim result = MessageBox.Show(update.t.Translate("msgUpdateErfolgreich", update.TranslatedProgramName), update.t.Translate("Update", update.TranslatedProgramName), MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
        If DialogResult.Yes = result Then
            update.InstallUpdate()
        End If
    End Sub

    Private Sub _update_UpdateError(sender As Object, e As ErrorEventArgs) Handles _update.UpdateError
        UpdateError(_update, e)
    End Sub

    Friend Shared Sub UpdateError(update As Update, e As ErrorEventArgs)
        MessageBox.Show(e.Message, update.TranslatedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Private Sub _update_UpdateFound(sender As Object, e As UpdateFoundEventArgs) Handles _update.UpdateFound
        UpdateFound(_update, e)
    End Sub

    Friend Shared Sub UpdateFound(update As Update, e As UpdateFoundEventArgs)
        Dim additionalText As String = ""

        If e.FrameworkInstallStatus = Update.InstallStatus.NotInstalled Then
            additionalText = Environment.NewLine + Environment.NewLine + update.t.Translate("WarningNetFrameworkTooOld", e.Framework)
        ElseIf e.FrameworkInstallStatus = UpdateLib.Update.InstallStatus.Unknown Then
            additionalText = Environment.NewLine + Environment.NewLine + update.t.Translate("WarningNetFrameworkUnknown", e.Framework)
        End If
        Dim result = MessageBox.Show(update.t.Translate("msgUpdateVorhanden", e.DisplayVersion) + additionalText, update.t.Translate("Update", update.TranslatedProgramName), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        If DialogResult.Yes = result Then
            update.DownloadUpdateAsync()
        Else
            update.ResetUpdateState()
        End If
    End Sub

    Private Sub _update_UpdateInfo(sender As Object, e As InfoEventArgs) Handles _update.UpdateInfo
        UpdateInfo(_update, e)
    End Sub

    Friend Shared Sub UpdateInfo(update As Update, e As InfoEventArgs)
        MessageBox.Show(e.Message, update.TranslatedTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class
