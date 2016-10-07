Module Main
    Private Const VersionsFileName = "versions.json"

    Sub Main()
        Dim args = Environment.GetCommandLineArgs()
        If args.Length < 2 Then
            PrintHelpAndExit()
        End If
        Select Case args(1)
            Case "convert"
                If args.Length < 3 Then
                    Console.Error.WriteLine("Input file missing")
                    Environment.Exit(1)
                End If

                Try
                    ConvertLegacyFile(args(2))
                Catch ex As Exception
                    Console.Error.WriteLine("Failed to convert file: {0}", ex.Message)
                    Environment.Exit(2)
                End Try
            Case "make-checksums"
                If args.Length < 4 Then
                    Console.Error.WriteLine("Input file and update directory missing")
                    Environment.Exit(1)
                End If

                Try
                    MakeChecksums(args(2), args(3))
                Catch ex As Exception
                    Console.Error.WriteLine("Failed to make checksums: {0}", ex.Message)
                    Environment.Exit(2)
                End Try
        End Select
    End Sub

    Private Sub ConvertLegacyFile(legacyFile As String)
        Dim versionFile = OpenLegacyVersionsFile(legacyFile)

        Dim newFile = IO.Path.Combine(IO.Path.GetDirectoryName(legacyFile), VersionsFileName)
        versionFile.Save(newFile)
        Console.Out.WriteLine("Converted old file to ""{0}""", newFile)
    End Sub

    Private Sub MakeChecksums(versionsFile As String, dir As String)
        Dim versionFile = UpdateTool.VersionsFile.Open(versionsFile)
        Dim allFileNames As New HashSet(Of String)
        Using x = Security.Cryptography.SHA256.Create()
            For Each c In versionFile.Categories
                For Each f In c.Files
                    Dim filePath = IO.Path.Combine(dir, f.Name)
                    allFileNames.Add(IO.Path.GetFullPath(filePath))
                    Using stream As New IO.FileStream(filePath, IO.FileMode.Open, IO.FileAccess.Read)
                        f.Hash = x.ComputeHash(stream)
                    End Using
                Next
            Next
        End Using
        For Each file In IO.Directory.EnumerateFiles(dir, "*", IO.SearchOption.AllDirectories)
            If IO.Path.GetFileName(file) <> VersionsFileName AndAlso Not allFileNames.Contains(IO.Path.GetFullPath(file)) Then
                Console.Out.WriteLine("File in update dir, but missing in versions.json: {0}", file)
                Environment.ExitCode = 3
            End If
        Next
        versionFile.Save(versionsFile)
        Console.Out.WriteLine("Updated checksums in file ""{0}""", versionsFile)
    End Sub

    Sub PrintHelpAndExit()
        Console.Error.WriteLine("Usage: {0} COMMAND", Environment.GetCommandLineArgs()(0))
        Console.Error.WriteLine()
        Console.Error.WriteLine("Commands:")
        Console.Error.WriteLine(" convert       : convert from old Versionen.lst to new versions.json")
        Console.Error.WriteLine(" make-checksums: hash all files in update directory and update versions.json")
        Environment.Exit(1)
    End Sub

    Function OpenLegacyVersionsFile(file As String) As VersionsFile
        Dim result As New VersionsFile()
        Using stream As New IO.FileStream(file, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
            Using reader As New IO.StreamReader(stream, True)
                reader.ReadLine() 'UpdateVersion
                result.DisplayVersion = reader.ReadLine
                result.Version = New Version(result.DisplayVersion)
                'InterneVersion = CInt()
                reader.ReadLine()
                result.ReleaseNotesUrl = reader.ReadLine
                Dim currentCategory As Category = Nothing
                Do
                    Dim line = reader.ReadLine
                    If line Is Nothing Then
                        Exit Do
                    End If

                    If line.Length > 1 AndAlso line.Substring(0, 2) = "K:" Then
                        Dim name = line.Substring(2, line.IndexOf(":"c, 2) - 2)
                        Dim mandatory = CBool(line.Substring(line.IndexOf(":"c, 2) + 1))
                        currentCategory = New Category(If(mandatory, Nothing, name))
                        result.Categories.Add(currentCategory)
                    ElseIf line.Length > 1 AndAlso line.Substring(0, 2) = "D:" Then
                        ' Ignore, file to be deleted will be detected automatically
                        currentCategory = Nothing
                    ElseIf currentCategory IsNot Nothing Then
                        'Dateien in Kategorien
                        currentCategory.Files.Add(New File(line.Replace("\", "/"), reader.ReadLine()))
                    Else
                        Console.Error.WriteLine("Got a file outside a category (probably deleted): {0}", line)
                    End If
                Loop
            End Using
        End Using
        Return result
    End Function
End Module
