# Run smoke test — uses qd/compute.json unless -Provider overrides
param(
    [ValidateSet("", "aer", "ibm", "braket", "braket-cloud")]
    [string]$Provider = "",
    [switch]$InstallDeps
)
$ErrorActionPreference = "Stop"

if ($Provider) {
    switch ($Provider) {
        "aer"           { & (Join-Path $PSScriptRoot "test-ibm-quantum.ps1") -Mode aer -InstallDeps:$InstallDeps; break }
        "ibm"           { & (Join-Path $PSScriptRoot "test-ibm-quantum.ps1") -Mode ibm -InstallDeps:$InstallDeps; break }
        "braket"        { & (Join-Path $PSScriptRoot "test-braket.ps1") -InstallDeps:$InstallDeps; break }
        "braket-cloud"  { & (Join-Path $PSScriptRoot "test-braket.ps1") -Cloud -InstallDeps:$InstallDeps; break }
    }
    exit $LASTEXITCODE
}

& (Join-Path $PSScriptRoot "run-compute.ps1") -InstallDeps:$InstallDeps
exit $LASTEXITCODE