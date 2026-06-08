# Build IBM Quantum support report (no API key in output)
$ErrorActionPreference = "Stop"
$Root = Split-Path $PSScriptRoot -Parent
$Out = Join-Path $Root "dist\ibm-support-report.txt"
$Ticket = Join-Path $Root "dist\IBM-Support-Ticket.txt"
New-Item -ItemType Directory -Force -Path (Split-Path $Out) | Out-Null

$lines = @()
$lines += "IBM Quantum Platform - support report (TritiumOS)"
$lines += "Generated: $((Get-Date).ToUniversalTime().ToString('o'))"
$lines += ""
$lines += "ISSUE: Ghost Open Plan instance"
$lines += "  - UI: No instance visible on https://quantum.cloud.ibm.com/instances"
$lines += "  - Create instance fails: 422 only one open plan instance is allowed per account"
$lines += "  - API list_instances returns 0 instances for the same API key"
$lines += ""
$lines += "REQUEST: Delete or expose the orphaned Open instance, or provision a visible"
$lines += "  Open instance for this account so Qiskit Runtime can run jobs."
$lines += ""
$lines += "Trace IDs from user:"
$lines += "  626c-1114ce38fcbbb253"
$lines += "  626c-ec1ce2f9a4678e1a"
$lines += ""

$py = "python"
foreach ($c in @("python", "python3", "py")) {
    try { & $c -c "import sys; sys.exit(0)" 2>$null; if ($LASTEXITCODE -eq 0) { $py = $c; break } } catch {}
}

$lines += "--- API diagnostic (automated) ---"
try {
    $diag = & $py (Join-Path $PSScriptRoot "ibm-list-instances.py") 2>&1 | Out-String
    $lines += $diag.Trim()
} catch {
    $lines += "Diagnostic failed: $_"
}

$lines += ""
$lines += "--- Checks performed by user (fill in if opening ticket) ---"
$lines += "[ ] Region set to us-east in IBM Quantum header"
$lines += "[ ] Checked Open, Pay-As-You-Go, Flex, Premium, and Archive tabs on Instances"
$lines += "[ ] Tried every IBM Cloud account in header account switcher"
$lines += "[ ] API key created from same account shown in quantum.cloud.ibm.com"
$lines += "[ ] Registration completed at https://quantum.cloud.ibm.com/registration"
$lines += ""
$lines += "Product context: TritiumOS personal project (creator Draco), Qiskit Runtime smoke test."
$lines += ""

$text = $lines -join "`r`n"
Set-Content -Path $Out -Value $text -Encoding UTF8
if (Test-Path $Ticket) {
    Write-Host "Ticket (paste into IBM form): $Ticket"
}
Write-Host "Diagnostic summary: $Out"
Write-Host ""
Write-Host $text