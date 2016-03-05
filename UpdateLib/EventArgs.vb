Public Class ErrorEventArgs
    Inherits EventArgs

    Public ReadOnly Property Message As String
    Public ReadOnly Property InnerException As Exception

    Friend Sub New(message As String, innerException As Exception)
        Me.Message = message
        Me.InnerException = innerException
    End Sub
End Class

Public Class InfoEventArgs
    Inherits EventArgs

    Public ReadOnly Property Message As String

    Friend Sub New(message As String)
        Me.Message = message
    End Sub
End Class

Public Class UpdateFoundEventArgs
    Inherits EventArgs

    Public ReadOnly Property DisplayVersion As String
    Public ReadOnly Property ReleaseNotesUrl As String
    Public ReadOnly Property Framework As String
    Public ReadOnly Property FrameworkInstallStatus As Update.InstallStatus

    Friend Sub New(displayVersion As String, releaseNotesUrl As String, framework As String, frameworkInstallStatus As Update.InstallStatus)
        Me.DisplayVersion = displayVersion
        Me.ReleaseNotesUrl = releaseNotesUrl
        Me.Framework = framework
        Me.FrameworkInstallStatus = frameworkInstallStatus
    End Sub
End Class
