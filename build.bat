@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

if "%NuGet%" == "" (
    set NuGet=nuget
)

if "%MsBuildExe%" == "" (
    set MsBuildExe=msbuild
)

REM Package restore
call %NuGet% restore src\YandexDisk.Client.Net40\packages.config -OutputDirectory %cd%\src\packages -NonInteractive
call %NuGet% restore src\YandexDisk.Client.Net45\packages.config -OutputDirectory %cd%\src\packages -NonInteractive
call %NuGet% restore src\YandexDisk.Client.Tests.Net40\packages.config -OutputDirectory %cd%\src\packages -NonInteractive
call %NuGet% restore src\YandexDisk.Client.Tests.Net45\packages.config -OutputDirectory %cd%\src\packages -NonInteractive
call %NuGet% restore src\YandexDisk.Client.CLI\packages.config -OutputDirectory %cd%\src\packages -NonInteractive
call %NuGet% restore src\YandexDisk.Client.CLI.Tests\packages.config -OutputDirectory %cd%\src\packages -NonInteractive

REM Build
"%MsBuildExe%" src\YandexDisk.Client.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
