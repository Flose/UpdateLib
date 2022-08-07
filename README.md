# UpdateLib

UpdateLib is a .Net library that helps to provide an update mechanism for applications. It supports incremental updates and provides a simple MessageBox based UI and a overlay UI for Windows Forms. It uses a JSON based file format for update information.

The library is available on NuGet: https://www.nuget.org/packages/FloseCode.UpdateLib/

## Example in VB.Net
```vb.net
Dim u As New FloseCode.UpdateLib.Update(programName, programExeFile, programVersion, programPath, tempPath, updateMirrorFile, New String() {"https://defaultUpdateServer"}, uid)

' Optional settings
u.StatisticsServer = "https://statisticsServer/sendStats"
u.ProductFlavor = "Portable"
u.CurrentReleaseChannel = "beta"

' Install update if one was downloaded previously
If u.InstallUpdate() Then Return

' Set version info in Windows registry, needs admin privileges
u.SetUninstallInfoInRegistry("{UUID}")

' Translate strings, uses TranslationLib
u.Translate(language, translatedProgramName)

' Search for update
u.SearchUpdateAsync()
```

## Example Update File "versions.json"
```json
{
  "displayVersion": "1.0.0beta",
  "version": "0.9.95",
  "releaseNotesUrl": "https://website/releasenotes",
  "projectUrl": "https://website",
  "framework": "net4.5",
  "categories": [
    {
      "files": [
        {
          "name": "program.exe",
          "sha256": "172A2702F45834029D9915F898360E2DF65BDE4A4EE16BD5E0A76C547825E43C"
        },
        {
          "name": "TranslationLib.dll",
          "sha256": "CE9E77C5AAA9E72FA121478590B1460CD8B48B0FFCC2CBFDA8E541ED0A69EEE7"
        },
        {
          "name": "UpdateLib.dll",
          "sha256": "4F39EAA0EA7E88B0B664853C000D513F5F5E74183987CF1505D5A2A835340F71"
        }
      ]
    },
    {
      "name": "someCategory",
      "files": [
        {
          "name": "someFile",
          "sha256": "8D51E8FA57B4098C94C899F245CB65E9203443ED41833009BA24589A750A0A15"
        }
      ]
    }
  ]
}
```

## License

Copyright: Flose 2006 - 2022 https://www.mal-was-anderes.de/

Licensed under the LGPLv3: http://www.gnu.org/licenses/lgpl-3.0.html
