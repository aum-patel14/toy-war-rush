@echo off
title Toy War Rush - Unity Setup
echo.
echo Rebuilding scenes, prefabs, and level data...
echo CLOSE Unity Editor first, then press any key.
pause

set UNITY="D:\Unity Hub\6000.5.0f1\Editor\Unity.exe"
set PROJECT="D:\Play Game"

if not exist %UNITY% (
  echo Unity not found. Install Unity 6 at D:\Unity Hub\6000.5.0f1\
  pause
  exit /b 1
)

%UNITY% -batchmode -nographics -quit -projectPath %PROJECT% -executeMethod ToyWarRushSetup.SetupEverything -logFile unity-setup.log

findstr /i "Setup complete error CS" unity-setup.log
echo.
echo Done. Open Boot.unity and press Play.
pause
