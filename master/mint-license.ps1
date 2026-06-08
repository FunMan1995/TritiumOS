param([int]$Slots = 10)
$guid = [guid]::NewGuid().ToString("N").Substring(0, 16).ToUpper()
$key = "TRIT-$guid-DRACO"
Write-Host "Master (Draco) minted license key (scaffold):"
Write-Host $key
Write-Host "Device slots: $Slots"
$key | Set-Clipboard
Write-Host "(copied to clipboard)"