# TritiumOS quantum smoke test — Aer ($0) or IBM Open if IBM_QUANTUM_TOKEN is set
param(
    [ValidateSet("auto", "aer", "ibm")]
    [string]$Mode = "auto",
    [switch]$InstallDeps,
    [int]$AerShots = 1024,
    [int]$IbmShots = 128
)
$ErrorActionPreference = "Stop"
$Root = Split-Path $PSScriptRoot -Parent
$Py = Join-Path $PSScriptRoot "test-quantum.py"
$KeyFile = Join-Path $Root "apikey.json"
$env:TRITIUM_APIKEY_FILE = $KeyFile

function Find-Python {
    foreach ($c in @("python", "python3", "py")) {
        try {
            $v = & $c -c "import sys; print(sys.version)" 2>$null
            if ($LASTEXITCODE -eq 0) { return $c }
        } catch {}
    }
    return $null
}

$python = Find-Python
if (-not $python) {
    Write-Host "Python not found. Install Python 3.10+ from https://www.python.org/" -ForegroundColor Red
    exit 2
}

if ($InstallDeps) {
    Write-Host "Installing qiskit + qiskit-aer (+ ibm-runtime for hardware)..."
    & $python -m pip install --upgrade pip
    & $python -m pip install qiskit qiskit-aer qiskit-ibm-runtime
}

if ($Mode -eq "auto") {
    $cfgPath = Join-Path $Root "qd\compute.json"
    if (Test-Path $cfgPath) {
        $c = Get-Content $cfgPath -Raw | ConvertFrom-Json
        $resolved = $c.backends.($c.active).test_provider
        if ($resolved -in @("aer", "ibm")) { $Mode = $resolved }
        elseif ($resolved) {
            Write-Host "qd/compute.json -> $resolved; use .\tools\run-compute.ps1" -ForegroundColor Yellow
            exit 2
        }
    }
}
$env:TRITIUM_QUANTUM_MODE = $Mode
$env:TRITIUM_AER_SHOTS = "$AerShots"
$env:TRITIUM_IBM_SHOTS = "$IbmShots"

if ($Mode -eq "ibm" -and -not $env:IBM_QUANTUM_TOKEN -and -not $env:QISKIT_IBM_TOKEN -and -not (Test-Path $KeyFile)) {
    Write-Host ""
    Write-Host "IBM mode requires a token:" -ForegroundColor Yellow
    Write-Host "  Copy apikey.json.example to apikey.json, or:"
    Write-Host '  $env:IBM_QUANTUM_TOKEN = "your-44-char-key"'
    Write-Host "  Get key: https://quantum.cloud.ibm.com/ -> Home -> API key"
    Write-Host ""
    exit 2
}

Write-Host ""
Write-Host "=== TritiumOS quantum test ===" -ForegroundColor Cyan
if ($env:IBM_QUANTUM_TOKEN -or $env:QISKIT_IBM_TOKEN) {
    Write-Host "IBM token: from environment" -ForegroundColor DarkGray
} elseif (Test-Path $KeyFile) {
    Write-Host "IBM token: from apikey.json (gitignored)" -ForegroundColor DarkGray
} else {
    Write-Host "IBM token: not set - will use free local Aer in auto mode" -ForegroundColor DarkGray
}
Write-Host ""

Push-Location $Root
try {
    & $python $Py
    exit $LASTEXITCODE
} finally {
    Pop-Location
}