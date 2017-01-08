@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

if "%NuGet%" == "" (
   set NuGet=nuget.exe
)

if "%MsBuildExe%" == "" (
   set MsBuildExe=msbuild.exe
)

REM *****************************************
REM Build .NET 4.5 Framework version
REM *****************************************

REM Package restore
call %NuGet% restore src\YandexDisk.Client.Net45\packages.config -OutputDirectory %cd%\src\packages -NonInteractive
call %NuGet% restore src\YandexDisk.Client.Tests.Net45\packages.config -OutputDirectory %cd%\src\packages -NonInteractive

REM Build
"%MsBuildExe%" src\YandexDisk.Client.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false


REM *****************************************
REM Build .NET Standard 1.6 version
REM *****************************************

REM Package restore
cd src\YandexDisk.Client.DotNet

dotnet restore 
dotnet build 

cd ..\..\

REM Package
mkdir Build
call %nuget% pack "src\YandexDisk.Client.nuspec" -symbols -o Build -p Configuration=%config% %version%
