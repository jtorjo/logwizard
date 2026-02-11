param(
    [string]$Configuration = "Release",
    [string]$SolutionPath = "src\LogWizard.sln",
    [string]$OutputDir = "dist"
)

$ErrorActionPreference = "Stop"

function Get-MSBuildPath {
    # Try VS2022/2019/2017 MSBuild via vswhere
    $vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $vswhere) {
        $msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe
        if ($msbuild -and (Test-Path $msbuild)) { return $msbuild }
    }

    # Fallback: try PATH
    $msbuild = (Get-Command msbuild -ErrorAction SilentlyContinue)?.Source
    if ($msbuild) { return $msbuild }

    throw "MSBuild not found. Install Visual Studio Build Tools or Visual Studio with MSBuild."
}

function Get-NuGetExe {
    # If nuget.exe is on PATH, use it
    $nuget = (Get-Command nuget -ErrorAction SilentlyContinue)?.Source
    if ($nuget) { return $nuget }

    throw "nuget.exe not found on PATH. Install NuGet CLI or add it to PATH. (Chocolatey: choco install nuget.commandline)"
}

Write-Host "== LogWizard portable build =="

$msbuild = Get-MSBuildPath
$nuget = Get-NuGetExe

Write-Host "MSBuild: $msbuild"
Write-Host "NuGet:   $nuget"
Write-Host "Solution: $SolutionPath"
Write-Host "Config:   $Configuration"

if (!(Test-Path $SolutionPath)) {
    throw "Solution not found at: $SolutionPath"
}

# Restore NuGet packages (packages.config)
Write-Host "`n-- Restoring NuGet packages --"
& $nuget restore $SolutionPath | Write-Host

# Build
Write-Host "`n-- Building solution --"
& $msbuild $SolutionPath /m /t:Rebuild /p:Configuration=$Configuration /v:minimal | Write-Host

# Package
$releaseDir = Join-Path "src\bin" $Configuration
if (!(Test-Path $releaseDir)) {
    throw "Build output folder not found: $releaseDir"
}

$distDir = Resolve-Path $OutputDir -ErrorAction SilentlyContinue
if (!$distDir) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
    $distDir = Resolve-Path $OutputDir
}

$staging = Join-Path $distDir "LogWizard-portable"
$zipPath = Join-Path $distDir "LogWizard-portable.zip"

Write-Host "`n-- Staging files from $releaseDir --"
if (Test-Path $staging) { Remove-Item -Recurse -Force $staging }
New-Item -ItemType Directory -Path $staging | Out-Null

# Copy everything you need to run (exe, dlls, configs, etc.)
Copy-Item -Path (Join-Path $releaseDir "*") -Destination $staging -Recurse -Force

# Optional: remove stuff you don't want in the portable zip
Get-ChildItem -Path $staging -Recurse -Include *.xml,*.pdb | ForEach-Object {
    # Comment these out if you want symbols/docs included
    Remove-Item $_.FullName -Force
}

Write-Host "`n-- Creating zip: $zipPath --"
if (Test-Path $zipPath) { Remove-Item -Force $zipPath }
Compress-Archive -Path (Join-Path $staging "*") -DestinationPath $zipPath

Write-Host "`nDone!"
Write-Host "Portable folder: $staging"
Write-Host "Portable zip:    $zipPath"
