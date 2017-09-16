@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

if "%nuget%" == "" (
    set nuget=nuget
)

call .\build.bat %config%

REM Package
mkdir Build
call %nuget% pack "src\YandexDisk.Client.nuspec" -symbols -o Build -p Configuration=%config% %version%
