﻿Imports System.Windows.Forms
Imports System.Diagnostics

Module mdlUpdate
    Friend InstallationPath As String
    Friend UpdatePath As String

    Private Const UpdateInfoFileName As String = "UpdateInfo.txt"

    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Dim args = Environment.GetCommandLineArgs()
        If args.Length < 3 Then
            ' Installation path is optional
            MessageBox.Show("Usage: " + args(0) + " PROGRAM_NAME EXE_NAME INSTALLATION_PATH", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Environment.Exit(1)
        End If

        UpdatePath = IO.Path.Combine(Application.StartupPath, "Update")
        If Not IO.Directory.Exists(UpdatePath) Then
            MessageBox.Show("No update is available for installation!", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Environment.Exit(2)
        End If

        Dim programVersion As String = "Unknown"
        If args.Length >= 4 Then
            ' Since UpdateLib 2.0 the path is passed as an argument
            InstallationPath = args(3)
        ElseIf IO.File.Exists(IO.Path.Combine(Environment.CurrentDirectory, "UpdateDll.dll")) OrElse
               IO.File.Exists(IO.Path.Combine(Environment.CurrentDirectory, "TranslationLib.dll")) Then
            ' Since UpdateDLL 1.4 Updates are stored in %ProgramData%
            InstallationPath = Environment.CurrentDirectory
        Else
            ' Before UpdateDLL 1.4 in the program path
            InstallationPath = Application.StartupPath
        End If

        Dim filesToDelete As New List(Of String)
        If IO.File.Exists(IO.Path.Combine(UpdatePath, "Versionen.lst")) Then
            ' Open legacy versions file
            ' Read version and files to delete
            Using Reader As New IO.StreamReader(IO.Path.Combine(UpdatePath, "Versionen.lst"))
                Dim line As String, categoryIndex As Integer
                line = Reader.ReadLine
                programVersion = Reader.ReadLine()
                line = Reader.ReadLine
                line = Reader.ReadLine.Trim
                Do
                    line = Reader.ReadLine
                    If line Is Nothing Then
                        Exit Do
                    End If

                    If line.Length > 1 AndAlso line.Substring(0, 2) = "K:" Then
                        categoryIndex = 3
                    ElseIf line.Length > 1 AndAlso line.Substring(0, 2) = "D:" Then
                        categoryIndex = -2
                    ElseIf categoryIndex = -2 Then
                        ' Files to delete
                        filesToDelete.Add(line)
                    ElseIf categoryIndex > -1 Then
                        ' ignore file in a category
                    End If
                Loop
            End Using
        ElseIf IO.File.Exists(IO.Path.Combine(UpdatePath, UpdateInfoFileName)) Then
            ' Open new file with version and deleted files
            Try
                Using reader As New IO.StreamReader(IO.Path.Combine(UpdatePath, UpdateInfoFileName))
                    Dim version = CDbl(reader.ReadLine)
                    ' Only read the file if we know the version
                    If version = 1 Then
                        programVersion = reader.ReadLine
                        Do
                            Dim line = reader.ReadLine
                            If line Is Nothing Then
                                Exit Do
                            End If

                            filesToDelete.Add(line)
                        Loop
                    Else
                        Console.Error.WriteLine("Warning: {0} has wrong Version, ignoring: expecting {1}, but is {2}", UpdateInfoFileName, 1, version)
                    End If
                End Using
            Catch ex As Exception
                Console.Error.WriteLine("Failed to open {0}, ignoring: {1}", UpdateInfoFileName, ex.Message)
            End Try
        End If

        If IO.Directory.GetFiles(UpdatePath, "Lizenz-*.txt").Length > 0 Then
            If frmLizenz.ShowDialog() <> DialogResult.OK Then
                Environment.Exit(3)
            End If
        End If

        frmUpdate.Show()
        Application.DoEvents()

        For Each fileName In filesToDelete
            Try
                Dim file = IO.Path.Combine(InstallationPath, fileName)
                If IO.File.Exists(file) Then
                    IO.File.Delete(file)
                End If
            Catch ex As Exception
                Console.Error.WriteLine("Failed deleted file ""{0}"": {1}", fileName, ex.Message)
            End Try
        Next

        MoveDirectory(UpdatePath, InstallationPath)

        Try
            Dim file = IO.Path.Combine(UpdatePath, UpdateInfoFileName)
            If IO.File.Exists(file) Then
                IO.File.Delete(file)
            End If
        Catch ex As Exception
            Console.Error.WriteLine("Failed to delete update info file: {0}", ex.Message)
        End Try

        ' Delete old update-*.exe
        For Each file As String In IO.Directory.GetFiles(Application.StartupPath, "Update-*.exe")
            If file <> Application.ExecutablePath Then
                Try
                    IO.File.Delete(file)
                Catch ex As Exception
                    Console.Error.WriteLine("Failed deleted old Update.exe: {0}", ex.Message)
                End Try
            End If
        Next
        If Application.ExecutablePath <> IO.Path.Combine(InstallationPath, "Update.exe") Then
            Try
                IO.File.Copy(Application.ExecutablePath, IO.Path.Combine(InstallationPath, "Update.exe"), True)
            Catch ex As Exception
                Console.Error.WriteLine("Failed to copy Update.exe to installation path: {0}", ex.Message)
            End Try
        End If

        Try
            Using UpdateWriter As New IO.StreamWriter(IO.Path.Combine(InstallationPath, "UpdateHistory.txt"), True)
                UpdateWriter.WriteLine(String.Format("{0}|{1}", Date.UtcNow.ToString("u"), programVersion.Replace("|"c, "_"c)))
            End Using
        Catch ex As Exception
            Console.Error.WriteLine("Failed to write update history: {0}", ex.Message)
        End Try

        RunProgram(IO.Path.Combine(InstallationPath, args(2).Trim))

        frmUpdate.Close()
        Environment.Exit(0)
    End Sub

    Private Sub RunProgram(program As String)
        Dim programEXE As String = """" & program & """"
        If Environment.OSVersion.Platform = PlatformID.Unix Then
            Dim process As New ProcessStartInfo("mono", programEXE)
            process.UseShellExecute = False
            Try
                Diagnostics.Process.Start(process)
            Catch ex As Exception
                MessageBox.Show("Error while trying to execute 'mono " & programEXE & "':" & Environment.NewLine & ex.Message)
            End Try
        Else
            Try
                Process.Start(programEXE)
            Catch ex As Exception
                MessageBox.Show("Error while trying to execute " & programEXE & ":" & Environment.NewLine & ex.Message)
            End Try
        End If
    End Sub

    Sub MoveDirectory(ByVal sourceDirectory As String, ByVal targetDirectory As String)
        IO.Directory.CreateDirectory(targetDirectory)

        For Each file As String In IO.Directory.GetFiles(sourceDirectory)
            Dim fileName = IO.Path.GetFileName(file)
            If fileName = UpdateInfoFileName Then
                Continue For
            End If

            Dim retryCount = 0
            Do
                Try
                    My.Computer.FileSystem.MoveFile(file, IO.Path.Combine(targetDirectory, fileName), True)
                    Exit Do
                Catch ex As Exception
                    If retryCount > 2 Then
                        Dim result As DialogResult = MessageBox.Show(String.Format("Error replacing file ""{0}"":" + vbLf + "{1}" + vbLf + vbLf + "Check if the program is still running!", fileName, ex.Message), "Update", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2)
                        If result = DialogResult.Abort Then
                            Environment.Exit(2)
                        ElseIf result = DialogResult.Ignore Then
                            Exit Do
                        ElseIf result = DialogResult.Retry Then
                        End If
                    End If
                End Try
                retryCount += 1
                Threading.Thread.Sleep(1000)
            Loop
        Next
        For Each dir As String In IO.Directory.GetDirectories(sourceDirectory)
            MoveDirectory(dir, IO.Path.Combine(targetDirectory, IO.Path.GetFileName(dir)))
        Next
        Threading.Thread.Sleep(100) ' Workaround: Otherwise the file handle is sometimes still open
        Try
            IO.Directory.Delete(sourceDirectory, True)
        Catch ex As Exception
            Console.Error.WriteLine("Error deleting directory ""{0}"": {1}", sourceDirectory, ex.Message)
        End Try
    End Sub
End Module
