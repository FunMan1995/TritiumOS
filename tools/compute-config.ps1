# Read/write qd/compute.json — single setting for Aer vs Braket vs IBM
param(
    [ValidateSet("", "get", "set", "list", "path")]
    [string]$Action = "get",
    [string]$Backend = "",
    [switch]$AllowQpu,
    [int]$MaxShots = 0
)
$ErrorActionPreference = "Stop"
$Root = Split-Path $PSScriptRoot -Parent
$Canonical = Join-Path $Root "qd\compute.json"
$Poly = Join-Path $Root "tritium.poly\compute.json"

function Get-ConfigPath {
    if (Test-Path $Canonical) { return $Canonical }
    if (Test-Path $Poly) { return $Poly }
    throw "Missing qd\compute.json"
}

function Get-Config {
    $p = Get-ConfigPath
    Get-Content $p -Raw | ConvertFrom-Json
}

function Save-Config($cfg) {
    $cfg.PSObject.Properties.Remove('_config_path') | Out-Null
    $json = $cfg | ConvertTo-Json -Depth 8
    New-Item -ItemType Directory -Force -Path (Split-Path $Canonical) | Out-Null
    $utf8NoBom = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllText($Canonical, $json, $utf8NoBom)
    if (Test-Path $Poly) {
        $stub = @{ version = $cfg.version; active = $cfg.active; allow_qpu = $cfg.allow_qpu; max_shots = $cfg.max_shots; ibm_enabled = $cfg.ibm_enabled; doc = "See qd/compute.json" }
        [System.IO.File]::WriteAllText($Poly, ($stub | ConvertTo-Json), $utf8NoBom)
    }
}

switch ($Action) {
    "path" { Write-Output (Get-ConfigPath); exit 0 }
    "list" {
        $c = Get-Config
        $c.backends.PSObject.Properties | ForEach-Object {
            $id = $_.Name
            $b = $_.Value
            $mark = if ($id -eq $c.active) { " *" } else { "" }
            Write-Host ("{0}{1}  test={2}  qpu={3}  {4}" -f $id, $mark, $b.test_provider, $b.qpu, $b.label)
        }
        exit 0
    }
    "set" {
        if (-not $Backend) { throw "Use -Action set -Backend aer_local|braket_local|braket_cloud|ibm_open" }
        $c = Get-Config
        if (-not $c.backends.$Backend) { throw "Unknown backend: $Backend" }
        if ($Backend -eq "ibm_open" -and -not $c.ibm_enabled) {
            Write-Warning "ibm_open is disabled (ibm_enabled=false). Set ibm_enabled true in compute.json when IBM fixes instance."
        }
        $c.active = $Backend
        if ($AllowQpu) { $c.allow_qpu = $true }
        if ($MaxShots -gt 0) { $c.max_shots = $MaxShots }
        Save-Config $c
        Write-Host "active=$Backend"
        exit 0
    }
    default {
        $c = Get-Config
        [PSCustomObject]@{
            path       = (Get-ConfigPath)
            active     = $c.active
            test       = $c.backends.($c.active).test_provider
            allow_qpu  = $c.allow_qpu
            max_shots  = $c.max_shots
            ibm_enabled = $c.ibm_enabled
        } | Format-List
    }
}