Friend Class frmUpdateHistory
    Friend Sub New(ByVal Übersetzen As TranslationLib.Translation, ByVal Programmpfad As String)
        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Übersetzen.TranslateControl(Me)
        lstUpdates.Items.Clear()
        If IO.File.Exists(IO.Path.Combine(Programmpfad, "UpdateHistory.txt")) Then
            Using Reader As New IO.StreamReader(IO.Path.Combine(Programmpfad, "UpdateHistory.txt"))
                Do
                    Try
                        Dim line = Reader.ReadLine()
                        If line Is Nothing Then
                            Exit Do
                        End If

                        Dim dateAndVersion = line.Split("|"c)
                        Dim installDate As Date
                        If Not Date.TryParse(dateAndVersion(0), Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.AssumeUniversal, installDate) Then
                            installDate = CDate(dateAndVersion(0))
                        End If
                        Dim version = dateAndVersion(1)
                        lstUpdates.Items.Add(Übersetzen.Translate("VersionErfolgreichInstalliert", installDate.ToShortDateString, Version))
                    Catch
                    End Try
                Loop
            End Using
        End If
    End Sub

    Private Sub cmdSchließen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSchließen.Click
        Me.Close()
    End Sub
End Class