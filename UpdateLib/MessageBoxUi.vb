Imports System.Windows.Forms

Public Class MessageBoxUi
    Private WithEvents update As Update

    Public Sub New(update As Update)
        Me.update = update
    End Sub

    Private Sub update_UpdateDownloaded(sender As Object, e As EventArgs) Handles update.UpdateDownloaded
        If DialogResult.Yes <> MessageBox.Show(update.t.Translate("msgUpdateErfolgreich", update.TranslatedProgramName), update.t.Translate("Update", update.TranslatedProgramName), MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) Then
            update.InstallUpdate()
        End If
    End Sub

    Private Sub update_UpdateError(sender As Object, e As ErrorEventArgs) Handles update.UpdateError
        MessageBox.Show(e.Message, update.TranslatedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Private Sub update_UpdateFound(sender As Object, e As UpdateFoundEventArgs) Handles update.UpdateFound
        Dim additionalText As String = ""
        If e.FrameworkInstallStatus = Update.InstallStatus.NotInstalled Then
            additionalText = Environment.NewLine + Environment.NewLine + _update.t.Translate("WarningNetFrameworkTooOld", e.Framework)
        ElseIf e.FrameworkInstallStatus = UpdateLib.Update.InstallStatus.Unknown Then
            additionalText = Environment.NewLine + Environment.NewLine + _update.t.Translate("WarningNetFrameworkUnknown", e.Framework)
        End If
        If DialogResult.Yes <> MessageBox.Show(update.t.Translate("msgUpdateVorhanden", e.DisplayVersion) + additionalText, update.t.Translate("Update", update.TranslatedProgramName), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) Then
            update.DownloadUpdateAsync()
        Else
            update.ResetUpdateState()
        End If
    End Sub

    Private Sub update_UpdateInfo(sender As Object, e As InfoEventArgs) Handles update.UpdateInfo
        MessageBox.Show(e.Message, update.TranslatedTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class
