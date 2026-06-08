param(
    [Parameter(Mandatory = $true)][string]$Key,
    [string]$DeviceId = $env:COMPUTERNAME
)

$MaxSlots = 10
# Scaffold: accept any non-empty key >= 8 chars; replace with master-verify
if ($Key.Length -lt 8) {
    Write-Error "Invalid license key (scaffold: min 8 characters)."
    exit 1
}

$regPath = Join-Path $PSScriptRoot "..\evolve\license-slots.json"
Write-Host "TritiumOS license OK (scaffold). Device: $DeviceId Slot: 1/$MaxSlots"
exit 0