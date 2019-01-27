REM Build Solution
set PATH_SOURCE_SLN="%cd%\UpdateLib.sln"
set PATH_SOURCE_PROJ="%cd%\UpdateLib\UpdateLib.vbproj"
set MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\bin\MSBuild.exe"


FOR %%C IN (Release20 Release40) DO (
 %MSBUILD% %PATH_SOURCE_SLN% /t:Restore /p:Configuration=%%C
 %MSBUILD% %PATH_SOURCE_SLN% /t:Rebuild /p:Configuration=%%C
 if errorlevel 1 goto :eof
)

nuget.exe update -self
nuget pack %PATH_SOURCE_PROJ% -Prop Configuration=Release20 -Symbols -IncludeReferencedProjects -Verbosity detailed
