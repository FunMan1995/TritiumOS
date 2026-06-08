# Run quantum smoke test using qd/compute.json active backend
param(
    [string]$Backend = "",
    [switch]$InstallDeps
)
$ErrorActionPreference = "Stop"
$Root = Split-Path $PSScriptRoot -Parent
$CfgPath = Join-Path $Root "qd\compute.json"

if ($Backend) {
    & (Join-Path $PSScriptRoot "compute-config.ps1") -Action set -Backend $Backend | Out-Null
}

$c = Get-Content $CfgPath -Raw | ConvertFrom-Json
$active = $c.active
$test = $c.backends.$active.test_provider

Write-Host "TritiumOS compute: active=$active -> test=$test" -ForegroundColor Cyan

if ($InstallDeps) {
    & (Join-Path $PSScriptRoot "test-ibm-quantum.ps1") -InstallDeps
}

switch ($test) {
    "aer"          { & (Join-Path $PSScriptRoot "test-ibm-quantum.ps1") -Mode aer; exit $LASTEXITCODE }
    "ibm"          { & (Join-Path $PSScriptRoot "test-ibm-quantum.ps1") -Mode ibm; exit $LASTEXITCODE }
    "braket"       { & (Join-Path $PSScriptRoot "test-braket.ps1"); exit $LASTEXITCODE }
    "braket-cloud" { & (Join-Path $PSScriptRoot "test-braket.ps1") -Cloud; exit $LASTEXITCODE }
    default        { throw "Unknown test_provider: $test" }
}