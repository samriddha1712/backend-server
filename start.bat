@echo off
echo Starting AI Smart Sheet .NET Backend...
echo.

REM Add .NET to PATH for this session
set PATH=%PATH%;C:\Program Files\dotnet

cd /d "%~dp0"

echo Building and starting the API...
echo.

dotnet run --urls "http://localhost:5000;https://localhost:5001"
