@echo off
echo Building .NET Framework 4.7.2 Retail Management System...
echo.

REM Set build configuration
set BUILD_CONFIG=Release
set PLATFORM=Any CPU

REM Check for MSBuild in different locations
set MSBUILD_PATH=""

REM Try Visual Studio 2022 Enterprise
if exist "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
    set MSBUILD_PATH="C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
    goto :build
)

REM Try Visual Studio 2022 Professional
if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set MSBUILD_PATH="C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
    goto :build
)

REM Try Visual Studio 2022 Community
if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set MSBUILD_PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
    goto :build
)

REM Try Build Tools 2022
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    set MSBUILD_PATH="C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
    goto :build
)

REM Try Visual Studio 2019
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set MSBUILD_PATH="C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
    goto :build
)

REM Try global MSBuild from .NET Framework
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    set MSBUILD_PATH="C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
    goto :build
)

REM Check if dotnet CLI can handle this differently
echo Attempting build with dotnet CLI using framework-specific settings...
dotnet build RetailManagement\RetailManagement.csproj -c %BUILD_CONFIG% --no-restore
if %errorlevel% equ 0 (
    echo.
    echo Build completed successfully with dotnet CLI!
    echo The executable is located at: RetailManagement\bin\%BUILD_CONFIG%\RetailManagement.exe
    echo.
    goto :end
)

echo.
echo Error: No suitable MSBuild found and dotnet CLI failed.
echo.
echo To resolve this issue, please install one of the following:
echo 1. Visual Studio 2022 Community (free) - https://visualstudio.microsoft.com/downloads/
echo 2. Visual Studio Build Tools 2022 - https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2022
echo.
echo Alternative: Try using the legacy build.bat which expects MSBuild in PATH
echo.
pause
exit /b 1

:build
echo Found MSBuild at: %MSBUILD_PATH%
echo.

REM Restore NuGet packages using dotnet (more reliable)
echo Restoring NuGet packages...
dotnet restore RetailManagement.sln
if %errorlevel% neq 0 (
    echo Error: Failed to restore NuGet packages.
    pause
    exit /b 1
)

REM Build using MSBuild
echo Building solution with MSBuild...
%MSBUILD_PATH% RetailManagement.sln /p:Configuration=%BUILD_CONFIG% /p:Platform="%PLATFORM%" /p:GenerateAssemblyInfo=false /v:minimal
if %errorlevel% neq 0 (
    echo Error: Build failed.
    pause
    exit /b 1
)

echo.
echo Build completed successfully!
echo The executable is located at: RetailManagement\bin\%BUILD_CONFIG%\RetailManagement.exe
echo.

:end
pause
