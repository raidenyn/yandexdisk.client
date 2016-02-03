@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

REM Package restore
call %NuGet% restore src\YandexDisk.Client\packages.config -OutputDirectory %cd%\src\packages -NonInteractive
call %NuGet% restore src\YandexDisk.Client.Tests\packages.config -OutputDirectory %cd%\src\packages -NonInteractive

REM Build
"%MsBuildExe%" src\YandexDisk.Client.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

REM Package
mkdir Build
call %nuget% pack "src\YandexDisk.Client.nuspec" -symbols -o Build -p Configuration=%config% %version%
