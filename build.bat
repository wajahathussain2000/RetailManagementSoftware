@echo off
echo Building Retail Management System...
echo.

REM Check if MSBuild is available
where msbuild >nul 2>nul
if %errorlevel% neq 0 (
    echo Error: MSBuild not found. Please ensure Visual Studio or Build Tools are installed.
    pause
    exit /b 1
)

REM Restore NuGet packages
echo Restoring NuGet packages...
nuget restore RetailManagement.sln
if %errorlevel% neq 0 (
    echo Error: Failed to restore NuGet packages.
    pause
    exit /b 1
)

REM Build the solution
echo Building solution...
msbuild RetailManagement.sln /p:Configuration=Release /p:Platform="Any CPU"
if %errorlevel% neq 0 (
    echo Error: Build failed.
    pause
    exit /b 1
)

echo.
echo Build completed successfully!
echo The executable is located at: RetailManagement\bin\Release\RetailManagement.exe
echo.
pause
