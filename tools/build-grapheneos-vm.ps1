# tools/build-grapheneos-vm.ps1
# Prepares a GrapheneOS VM setup for komodo using the added zip.
param([string]$Zip = "komodo-install-2026060100.zip", [string]$Out = "vm/grapheneos-komodo")
$ErrorActionPreference = "Stop"
Write-Host "Preparing GrapheneOS komodo VM from $Zip to $Out"
New-Item -Force -ItemType Directory $Out | Out-Null
if (-not (Test-Path $Zip)) { Write-Error "Zip missing"; exit 1 }
Add-Type -AssemblyName System.IO.Compression.FileSystem
$z = [System.IO.Compression.ZipFile]::OpenRead($Zip)
$toExtract = "android-info.txt","script.txt","flash-all.sh","flash-all.bat","boot.img","init_boot.img","vendor_boot.img","vbmeta.img","dtbo.img"
foreach ($f in $toExtract) {
  $e = $z.Entries | ? { $_.FullName -like "*$f" } | select -First 1
  if ($e) {
    $outf = Join-Path $Out $f
    $fs = [System.IO.File]::Create($outf)
    $e.Open().CopyTo($fs); $fs.Close()
    Write-Host "Extracted $f"
  }
}
$z.Dispose()
if (Test-Path "refs/grapheneos/komodo-install-2026060100") {
  Copy-Item "refs/grapheneos/komodo-install-2026060100/*" $Out -Recurse -Force -EA SilentlyContinue
  Write-Host "Copied metadata"
}
@"
GrapheneOS komodo VM prep complete.
Use your Pixel Pro XL AVD in Android Studio.
Launch with props to simulate this release (from android-info.txt in $Out):
emulator -avd <your-komodo-avd> -prop ro.build.fingerprint=... (see $Out/android-info.txt and GRAPHENEOS-VM-SETUP.md)
The Tritium app (built with build-android.ps1) will report the exact komodo GrapheneOS details in assimilation/bootstrap.
For full source build: use WSL + refs/grapheneos/device_google_caimito/komodo/
See $Out for extracted boot images and scripts.
"@ | Out-File (Join-Path $Out "README-VM.txt") -Encoding UTF8
Write-Host "Done. See $Out/README-VM.txt and run the script again to refresh."
