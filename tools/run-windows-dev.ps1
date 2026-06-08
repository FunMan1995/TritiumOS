# Run TritiumOS Windows UI without publishing (requires dotnet SDK on PATH)
$ErrorActionPreference = "Stop"
$Project = Join-Path (Split-Path $PSScriptRoot -Parent) "install\hosts\windows\TritiumOS.csproj"
Push-Location (Split-Path $Project -Parent)
try {
    dotnet run --project $Project
} finally {
    Pop-Location
}

# Reminder for developers using assimilate/bootstrap:
# Add AV exclusion for the whole C:\Test\TritiumOS (or your clone) + %LocalAppData%\TritiumOS before testing full-stack-optimize etc.
# The dev run also benefits from it because the running process does the host scans/exec/writes.
# See the build script output and install/hosts/windows/README-Win11.md.