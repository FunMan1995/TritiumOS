# Build dist/TritiumOS.apk — requires Android SDK + JDK 17
$ErrorActionPreference = "Stop"
$Root = Split-Path $PSScriptRoot -Parent
$Android = Join-Path $Root "install\hosts\android"
$Dist = Join-Path $Root "dist"
New-Item -ItemType Directory -Force -Path $Dist | Out-Null

# Always sync assets from source-of-truth (qd + core) so that even manual gradle builds or partial runs stay consistent.
# These run before any gradle invocation.
# For komodo (Pixel 9 Pro XL): See GrapheneOS refs/grapheneos/device_google_caimito + the komodo-install-2026060100.zip (if present in root) for hardware (BoardConfig for komodo, kernel, init, exact bootloader/baseband from android-info.txt, flash sequence from script.txt) and bootstrap.
# The zip was added by user; its metadata is extracted to refs/grapheneos/komodo-install-2026060100/ and referenced in Android host code (host-hw-info, bootstrap plans) and docs.
# Use .\tools\build-grapheneos-vm.ps1 to prepare a vm/grapheneos-komodo/ setup (AVD props simulation + QEMU + source build notes) for testing the app's assimilation on GrapheneOS vs stock.
# Recommend building/running the APK on GrapheneOS for hardened baseline, or use their bringup as template if expanding to custom system image for full Forth core bootstrap.
$assetsQd = Join-Path $Android "app\src\main\assets\qd"
New-Item -ItemType Directory -Force -Path $assetsQd | Out-Null
Copy-Item (Join-Path $Root "qd\compute.json") (Join-Path $assetsQd "compute.json") -Force
Write-Host "Synced qd/compute.json -> Android assets"

$assetsCore = Join-Path $Android "app\src\main\assets\core"
New-Item -ItemType Directory -Force -Path $assetsCore | Out-Null
Copy-Item (Join-Path $Root "tritium.poly\core\*") $assetsCore -Force
Write-Host "Synced tritium.poly/core -> Android assets/core"

$gradlew = Join-Path $Android "gradlew.bat"
if (-not (Test-Path $gradlew)) {
    Write-Host "Gradle wrapper missing. Generating wrapper (requires 'gradle' on PATH)..."
    Push-Location $Android
    try {
        # Use Gradle 8.7+ for compatibility with AGP 8.5.2 (AGP 8.x requires JDK 17)
        gradle wrapper --gradle-version 8.7
    } catch {
        Write-Host "Install Android Studio or Gradle, then run from:"
        Write-Host "  cd install\hosts\android"
        Write-Host "  gradle wrapper"
        Write-Host "  .\gradlew.bat assembleRelease"
        exit 1
    } finally {
        Pop-Location
    }
}

# Debug aid: verify Java for Gradle/AGP 8.5+ which require JDK 17+
$javaExe = $null
if ($env:JAVA_HOME) { $javaExe = Join-Path $env:JAVA_HOME "bin\java.exe" }
if (-not $javaExe -or -not (Test-Path $javaExe)) {
    $javaExe = (Get-Command java -ErrorAction SilentlyContinue).Source
}
if ($javaExe -and (Test-Path $javaExe)) {
    $ver = & $javaExe -version 2>&1 | Out-String
    if ($ver -notmatch 'version "1[7-9]|version "2[0-9]') {
        Write-Host "WARNING: Java version appears to be <17. AGP 8.5+ and Gradle 8.7+ require JDK 17+ to run/debug builds."
        Write-Host "  Detected: $ver"
        Write-Host "  Set JAVA_HOME to a JDK 17+ install (e.g. Eclipse Temurin 17) before running gradlew."
    } else {
        Write-Host "Java OK for build (>=17)"
    }
} else {
    Write-Host "WARNING: No java found on PATH or JAVA_HOME. Install JDK 17+ and ensure gradle can find it for debug builds."
}

$variant = if ($args -contains 'debug') { 'Debug' } else { 'Release' }
$task = "assemble$variant"
Write-Host "Building TritiumOS.apk ($variant) for debug/run..."
Push-Location $Android
try {
    & .\gradlew.bat $task --no-daemon
    $outDir = if ($variant -eq 'Debug') { "app\build\outputs\apk\debug" } else { "app\build\outputs\apk\release" }
    $apk = Get-ChildItem -Path $outDir -Filter "*.apk" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $apk) { throw "APK not found under $outDir" }
    $dest = Join-Path $Dist "TritiumOS.apk"
    Copy-Item $apk.FullName $dest -Force
    Write-Host "OK: $dest"
    if ($variant -eq 'Debug') {
        Write-Host "Debug APK ready. To debug: adb install $dest ; adb shell am start -n os.tritium.app/.MainActivity ; then adb logcat | grep Tritium"
    }
} finally {
    Pop-Location
}