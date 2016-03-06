Imports Newtonsoft.Json

Friend Class VersionsFile
    <JsonProperty("displayVersion")>
    Public DisplayVersion As String

    <JsonProperty("version", Required:=Required.Always)>
    <JsonConverter(GetType(Converters.VersionConverter))>
    Public Version As Version

    <JsonProperty("releaseNotesUrl")>
    Public ReleasNotesUrl As String

    <JsonProperty("projectUrl")>
    Public ProjectUrl As String

    <JsonProperty("framework")>
    Public Framework As String

    <JsonProperty("categories", Required:=Required.Always)>
    Public Categories As New List(Of Category)

    <JsonProperty("filesToDelete", NullValueHandling:=NullValueHandling.Ignore)>
    Public FilesToDelete As New List(Of String)

    Private originalContent As String

    Public Shared Function Open(file As String, Optional keepInputStreamInMemory As Boolean = False) As VersionsFile
        Using stream As New IO.FileStream(file, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
            Return Open(stream, keepInputStreamInMemory)
        End Using
    End Function

    Public Shared Function Open(stream As IO.Stream, Optional keepInputStreamInMemory As Boolean = False) As VersionsFile
        Using reader As New IO.StreamReader(stream, Text.Encoding.UTF8)
            If keepInputStreamInMemory Then
                Dim content = reader.ReadToEnd
                Dim result = New JsonSerializer().Deserialize(Of VersionsFile)(New JsonTextReader(New IO.StringReader(content)))
                result.originalContent = content
                Return result
            Else
                Return New JsonSerializer().Deserialize(Of VersionsFile)(New JsonTextReader(reader))
            End If
        End Using
    End Function

    Public Sub Save(path As String)
        Using stream As New IO.FileStream(path, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.Read)
            Using w As New IO.StreamWriter(stream)
                If originalContent IsNot Nothing Then
                    w.Write(originalContent)
                Else
                    Dim s As New JsonSerializer()
                    s.Formatting = Formatting.Indented
                    s.Serialize(w, Me)
                End If
            End Using
        End Using
    End Sub

    Public Function GetCategory(name As String) As Category
        For Each c In Categories
            If c.Name = name Then
                Return c
            End If
        Next
        Return Nothing
    End Function
End Class

Friend Class Category
    <JsonProperty("name", NullValueHandling:=NullValueHandling.Ignore)>
    Public Name As String

    <JsonProperty("files", Required:=Required.Always)>
    Public Files As New List(Of File)

    Public Sub New()
    End Sub

    Public Sub New(name As String)
        Me.Name = name
    End Sub

    Public Function IsMandatory() As Boolean
        Return Name Is Nothing
    End Function

    Public Function GetFile(name As String) As File
        For Each f In Files
            If f.Name = name Then
                Return f
            End If
        Next
        Return Nothing
    End Function
End Class

Friend Class File
    <JsonProperty("name", Required:=Required.Always)>
    Public Name As String

    <JsonProperty("sha256", Required:=Required.Always)>
    Public HashString As String

    Public Sub New()
    End Sub

    Public Sub New(name As String, HashString As String)
        Me.Name = name
        Me.HashString = HashString
    End Sub

    <JsonIgnore>
    Public Property Hash As Byte()
        Get
            Return Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary.Parse(HashString).Value
        End Get
        Set(value As Byte())
            HashString = New Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary(value).ToString
        End Set
    End Property
End Class