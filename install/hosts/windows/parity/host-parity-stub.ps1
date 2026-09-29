# Optional Windows host-parity stub (wave8 item 4 / docs/HOST-PARITY.md)
# Prints CONTRACT marker from HOST-PARITY.txt. Does not require TritiumOS.exe.
$ErrorActionPreference = 'Stop'
$dir = Split-Path -Parent $MyInvocation.MyCommand.Path
$parity = Join-Path $dir 'HOST-PARITY.txt'
if (-not (Test-Path -LiteralPath $parity)) {
    Write-Host '[host-parity] windows FAIL'
    Write-Host '[host-parity-stub] FAIL'
    exit 1
}
# Tip policy: Win surface is CONTRACT when the parity file is present.
Write-Host '[host-parity] windows CONTRACT'
Write-Host '[host-parity-stub] OK'
