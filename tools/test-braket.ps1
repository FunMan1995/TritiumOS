# Braket smoke test - local ($0) or cloud (-Cloud, AWS credentials, may incur cost)
param(
    [switch]$Cloud,
    [switch]$InstallDeps,
    [int]$Shots = 0
)
$ErrorActionPreference = "Stop"

$python = $null
foreach ($c in @("python", "python3", "py")) {
    try { & $c -c "import sys" 2>$null; if ($LASTEXITCODE -eq 0) { $python = $c; break } } catch {}
}
if (-not $python) { throw "Python not found" }

if ($InstallDeps) {
    & $python -m pip install amazon-braket-sdk
}

if ($Cloud) { $env:TRITIUM_BRAKET_CLOUD = "1" } else { Remove-Item Env:TRITIUM_BRAKET_CLOUD -ErrorAction SilentlyContinue }
if ($Shots -gt 0) { $env:TRITIUM_BRAKET_SHOTS = "$Shots" }

& $python (Join-Path $PSScriptRoot "test-braket.py")
exit $LASTEXITCODE