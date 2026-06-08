# Prepare a Qwantum field SEARCH — writes query manifest + prints paste prompt
param(
    [string]$Query = "TritiumOS DRENA REKIA Assimilate LINEOS exe apk",
    [string]$Program = "TritiumOS",
    [string]$Creator = "Draco"
)
$ErrorActionPreference = "Stop"
$Root = Split-Path $PSScriptRoot -Parent
$SearchId = [guid]::NewGuid().ToString("N").Substring(0, 12)
$OutDir = Join-Path $Root "evolve\qwantum-field"
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

$manifest = @{
    search_id   = $SearchId
    created_utc = (Get-Date).ToUniversalTime().ToString("o")
    program     = $Program
    creator     = $Creator
    query       = $Query
    field       = "tritium-qwantum-field"
    timeline    = @{ T0 = "2026"; T_parallel = "witness" }
    action      = "search"
    dump_target = "evolve\qwantum-dump\$SearchId"
}
$manifestPath = Join-Path $OutDir "search-$SearchId.json"
$manifest | ConvertTo-Json -Depth 6 | Set-Content -Path $manifestPath -Encoding UTF8

$promptPath = Join-Path $Root "qwantum\prompts\SEARCH-AND-DUMP.txt"
$prompt = Get-Content $promptPath -Raw

Write-Host ""
Write-Host "=== Qwantum field SEARCH ready ===" -ForegroundColor Cyan
Write-Host "search_id: $SearchId"
Write-Host "manifest:  $manifestPath"
Write-Host ""
Write-Host "1) Paste the block below into Qwantum Compute" -ForegroundColor Yellow
Write-Host "2) Save the full reply to a .txt file" -ForegroundColor Yellow
Write-Host "3) Run: .\tools\qwantum-dump.ps1 -InputPath <reply.txt> -SearchId $SearchId" -ForegroundColor Yellow
Write-Host ""
Write-Host $prompt
Write-Host ""
Write-Host "--- end prompt (search_id=$SearchId) ---" -ForegroundColor DarkGray

# Also write prompt copy for convenience
$promptCopy = Join-Path $OutDir "search-$SearchId-prompt.txt"
Set-Content -Path $promptCopy -Value $prompt -Encoding UTF8
Write-Host "Prompt saved: $promptCopy"