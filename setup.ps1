# Toy War Rush - Automated Setup Script
# Run in PowerShell: .\setup.ps1

$ErrorActionPreference = "Continue"
$ProjectPath = $PSScriptRoot

Write-Host "=== Toy War Rush Setup ===" -ForegroundColor Cyan
Write-Host "Project: $ProjectPath"

# 1. Check / Install Unity Hub
$hubPath = "${env:ProgramFiles}\Unity Hub\Unity Hub.exe"
if (-not (Test-Path $hubPath)) {
    Write-Host "`nInstalling Unity Hub..." -ForegroundColor Yellow
    $installer = "$env:TEMP\UnityHubSetup.exe"
    try {
        Invoke-WebRequest -Uri "https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup-x64.exe" -OutFile $installer -UseBasicParsing
        Start-Process -FilePath $installer -ArgumentList "/S" -Wait
        Write-Host "Unity Hub installed." -ForegroundColor Green
    } catch {
        Write-Host "Could not auto-install Unity Hub. Download manually: https://unity.com/download" -ForegroundColor Red
        Write-Host $_.Exception.Message
    }
} else {
    Write-Host "Unity Hub found: $hubPath" -ForegroundColor Green
}

# 2. Open project in Unity Hub
if (Test-Path $hubPath) {
    Write-Host "`nOpening project in Unity Hub..." -ForegroundColor Yellow
    Start-Process $hubPath -ArgumentList "--", "--projectPath", "`"$ProjectPath`""
}

# 3. Instructions
Write-Host @"

=== NEXT STEPS (one-time in Unity Editor) ===

1. Install Unity 6 (6000.0 LTS) + Android Build Support + URP when Hub opens
2. Open this project - wait for packages to import
3. Menu: ToyWarRush > Setup Everything (One Click)
4. Open scene: Assets/_Game/Scenes/Boot.unity
5. Press Play

Android build: File > Build Settings > Android > Switch Platform > Build

"@ -ForegroundColor Cyan
