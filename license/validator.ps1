param(
    [Parameter(Mandatory = $true)][string]$Key,
    [string]$DeviceId = $env:COMPUTERNAME,
    [switch]$StatusOnly
)

$MaxSlots = 10
$ErrorActionPreference = "Stop"

# Scaffold key check (replace with master-verify / crypto later)
if (-not $StatusOnly) {
    if ($Key.Length -lt 8) {
        Write-Error "Invalid license key (scaffold: min 8 characters)."
        exit 1
    }
}

$regPath = Join-Path $PSScriptRoot "..\evolve\license-slots.json"
$regDir = Split-Path $regPath -Parent
if (-not (Test-Path $regDir)) {
    New-Item -ItemType Directory -Force -Path $regDir | Out-Null
}

function Read-Slots {
    if (-not (Test-Path $regPath)) {
        return @{ maxSlots = $MaxSlots; devices = @() }
    }
    $raw = Get-Content -Raw -Path $regPath
    if ([string]::IsNullOrWhiteSpace($raw)) {
        return @{ maxSlots = $MaxSlots; devices = @() }
    }
    return $raw | ConvertFrom-Json
}

function Write-Slots($data) {
    $data | ConvertTo-Json -Depth 5 | Set-Content -Path $regPath -Encoding UTF8
}

$data = Read-Slots
$devices = @()
if ($data.devices) {
    foreach ($d in @($data.devices)) {
        if ($null -eq $d) { continue }
        if ($d -is [string]) { $devices += $d }
        elseif ($d.id) { $devices += [string]$d.id }
    }
}

if ($StatusOnly) {
    Write-Host ("tritium-license status: {0}/{1}" -f $devices.Count, $MaxSlots)
    Write-Host ("[license] status {0}/{1}" -f $devices.Count, $MaxSlots)
    exit 0
}

# Already registered?
$idx = [Array]::IndexOf($devices, $DeviceId)
if ($idx -ge 0) {
    Write-Host ("TritiumOS license OK (scaffold). Device: {0} Slot: {1}/{2}" -f $DeviceId, ($idx + 1), $MaxSlots)
    exit 0
}

# Refuse slot 11 (and >10) — TritiumOS.txt §5a.4
if ($devices.Count -ge $MaxSlots) {
    Write-Error ("Installer refuses slot 11 — max {0} devices (TritiumOS.txt §5a.4)." -f $MaxSlots)
    Write-Host "[license] refuse slot 11"
    exit 1
}

$devices += $DeviceId
$outDevices = @()
for ($i = 0; $i -lt $devices.Count; $i++) {
    $outDevices += @{ id = $devices[$i]; slot = ($i + 1) }
}
Write-Slots @{ maxSlots = $MaxSlots; devices = $outDevices }

$slot = $devices.Count
Write-Host ("TritiumOS license OK (scaffold). Device: {0} Slot: {1}/{2}" -f $DeviceId, $slot, $MaxSlots)
Write-Host ("[license] registered slot {0}/{1} device={2}" -f $slot, $MaxSlots, $DeviceId)
exit 0
