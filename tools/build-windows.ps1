# Build dist/TritiumOS.exe — TritiumOS by Draco
$ErrorActionPreference = "Stop"
$Root = Split-Path $PSScriptRoot -Parent
$Project = Join-Path $Root "install\hosts\windows\TritiumOS.csproj"
$Dist = Join-Path $Root "dist"
New-Item -ItemType Directory -Force -Path $Dist | Out-Null

Write-Host "Building TritiumOS.exe (win-x64 single-file)..."
Push-Location (Split-Path $Project -Parent)
try {
    dotnet publish $Project -c Release -o $Dist /p:PublishSingleFile=true
    $out = Join-Path $Dist "TritiumOS.exe"
    if (-not (Test-Path $out)) { throw "Build failed: $out not found" }
    Write-Host "OK: $out"
} finally {
    Pop-Location
}
# (AV probe cleaned)

Write-Host ""
Write-Host "=== Post-build note for assimilation features ===" -ForegroundColor Yellow
Write-Host "The produced TritiumOS.exe implements 'forth to c#' host assimilation + full-stack bootstrap/optimize." -ForegroundColor Yellow
Write-Host "Before running (first time or after publish):" -ForegroundColor Yellow
Write-Host "  1. Windows Security > Virus & threat protection > Manage settings > Exclusions" -ForegroundColor Yellow
Write-Host "  2. Add exclusion for this repo folder + the .exe in dist/ + %LocalAppData%\TritiumOS" -ForegroundColor Yellow
Write-Host "  (See install/hosts/windows/README-Win11.md for why: host-exec + bulk writes to evolve/assimilated + bootstrap + refined/)" -ForegroundColor Yellow
Write-Host "Without the exclusion you may see crashes or blocks exactly like the one that required the 'fix + exception'." -ForegroundColor Yellow
Write-Host "Once excluded: run the exe, type 'full-stack-optimize', watch evolve/ populate, and LoadRefinedModules will activate the emitted words." -ForegroundColor Green
