@echo off
echo ================================================
echo   Lumiere - Personal Cinema Builder
echo ================================================
echo.

echo [1/3] Restoring NuGet packages...
dotnet restore
if errorlevel 1 goto error

echo.
echo [2/3] Building the application...
dotnet build -c Release
if errorlevel 1 goto error

echo.
echo [3/3] Publishing single-file executable...
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o ./publish
if errorlevel 1 goto error

echo.
echo ================================================
echo   Build Successful!
echo ================================================
echo.
echo Your executable is located at:
echo   %CD%\publish\Lumiere.exe
echo.
echo To distribute:
echo   1. Copy the executable from publish folder
echo   2. Copy the Media folder (with your movies)
echo   3. Copy movies.db (if you have an existing library)
echo   4. Copy appsettings.json
echo.
echo Press any key to exit...
pause > nul
exit /b 0

:error
echo.
echo ================================================
echo   Build Failed!
echo ================================================
echo.
echo Please check the error messages above.
echo Make sure you have .NET 8 SDK installed.
echo.
pause
exit /b 1
