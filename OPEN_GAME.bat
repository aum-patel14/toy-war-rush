@echo off
title Toy War Rush
echo.
echo  Opening Toy War Rush (no server needed)
echo.

REM Opens game directly - works offline, no connection errors
start "" "%~dp0preview\index.html"
echo.
echo  If the game does not load, double-click PREVIEW.bat instead.
timeout /t 5
