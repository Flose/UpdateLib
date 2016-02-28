Imports System.Windows.Forms
Imports FloseCode.UpdateLib

Public Class MessageBoxUi
    Private WithEvents update As Update

    Public Sub New(update As Update)
        Me.update = update
    End Sub

    Private Sub update_UpdateDownloaded(sender As Object, e As EventArgs) Handles update.UpdateDownloaded
        If DialogResult.Yes <> MessageBox.Show(update.t.Translate("msgUpdateErfolgreich", Environment.NewLine, update.TranslatedProgramName), update.t.Translate("Update", update.TranslatedProgramName), MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) Then
            update.InstallUpdate()
        End If
    End Sub

    Private Sub update_UpdateError(sender As Object, e As Update.ErrorEventArgs) Handles update.UpdateError
        MessageBox.Show(e.Message, update.TranslatedTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Private Sub update_UpdateFound(sender As Object, e As Update.UpdateFoundEventArgs) Handles update.UpdateFound
        Dim additionalText As String = ""
        If e.FrameworkInstallStatus = Update.InstallStatus.NotInstalled Then
            additionalText = String.Format(Environment.NewLine + Environment.NewLine + "Warning: Install .Net Framework {0} (or an equivalent Mono version) before installing the update. Otherwise the program will fail to start after the update.", e.Framework) 'TODO translate
        ElseIf e.FrameworkInstallStatus = Update.InstallStatus.Unknown Then
            additionalText = String.Format(Environment.NewLine + Environment.NewLine + "Warning: .Net Framework {0} (or an equivalent Mono version) is necessary for this update. Make sure it’s installed.", e.Framework) 'TODO translate
        End If
        If DialogResult.Yes <> MessageBox.Show(update.t.Translate("msgUpdateVorhanden", e.DisplayVersion, Environment.NewLine) + additionalText, update.t.Translate("Update", update.TranslatedProgramName), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) Then
            update.DownloadUpdateAsync()
        Else
            update.ResetUpdateState()
        End If
    End Sub

    Private Sub update_UpdateInfo(sender As Object, e As Update.InfoEventArgs) Handles update.UpdateInfo
        MessageBox.Show(e.Message, update.TranslatedTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class
