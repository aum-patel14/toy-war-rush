@echo off
title Toy War Rush - Local Preview
echo.
echo  Toy War Rush - Local Preview
echo  ============================
echo.

cd /d "%~dp0preview"

REM Free port 8080 if stuck
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":8080.*LISTENING"') do (
  echo Stopping old server on port 8080...
  taskkill /F /PID %%a >nul 2>&1
)

echo  Starting server on http://localhost:8080
echo  (Browser opens in 3 seconds - wait for server)
echo  Press Ctrl+C in this window to stop
echo.

start /min cmd /c "timeout /t 3 /nobreak >nul && start http://localhost:8080"
npx --yes serve -l 8080 --no-port-switching .
if errorlevel 1 (
  echo.
  echo Port 8080 failed - trying port 3000...
  start /min cmd /c "timeout /t 3 /nobreak >nul && start http://localhost:3000"
  npx --yes serve -l 3000 .
)
pause
