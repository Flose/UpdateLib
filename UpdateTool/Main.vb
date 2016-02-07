Module Main

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

                Dim legacyFile = args(2)
                Dim versionFile = OpenLegacyVersionsFile(legacyFile)

                Dim newFile = IO.Path.Combine(IO.Path.GetDirectoryName(legacyFile), "versions.json")
                versionFile.Save(newFile)
                Console.Out.WriteLine("Converted old file to ""{0}""", newFile)
            Case "make-checksums"
        End Select
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
                result.ReleasNotesUrl = reader.ReadLine
                Dim currentCategory As Category
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
                        currentCategory.Files.Add(New File(line, reader.ReadLine()))
                    Else
                        Console.Error.WriteLine("Got a file outside a category (probably deleted): {0}", line)
                    End If
                Loop
            End Using
        End Using
        Return result
    End Function
End Module
