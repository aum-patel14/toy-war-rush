@echo off
title Toy War Rush - Android Build
echo.
echo Building Android APK (IL2CPP)...
echo Close Unity Editor before running this.
echo.

set UNITY="D:\Unity Hub\6000.5.0f1\Editor\Unity.exe"
set PROJECT="D:\Play Game"

if not exist %UNITY% (
  echo Unity not found at %UNITY%
  echo Install Unity 6 with Android Build Support, then edit this script.
  pause
  exit /b 1
)

%UNITY% -batchmode -nographics -quit -projectPath %PROJECT% -executeMethod ToyWarRushSetup.SetupEverything -logFile unity-setup.log
%UNITY% -batchmode -nographics -quit -projectPath %PROJECT% -executeMethod ToyWarRushBuild.BuildAndroidBatch -logFile unity-android.log

if exist "%PROJECT%\Build\Android\ToyWarRush.apk" (
  echo.
  echo SUCCESS: Build\Android\ToyWarRush.apk
) else (
  echo.
  echo Build may have failed. Check unity-android.log
)

pause
