# Qwantum field orchestrator: search (prompt) | dump (ingest) | full cycle
param(
    [ValidateSet("search", "dump", "full")]
    [string]$Action = "search",
    [string]$Query = "TritiumOS",
    [string]$InputPath = "",
    [string]$SearchId = "",
    [switch]$Apply,
    [switch]$Force
)
$ErrorActionPreference = "Stop"
$Root = Split-Path $PSScriptRoot -Parent

switch ($Action) {
    "search" {
        & (Join-Path $PSScriptRoot "qwantum-search.ps1") -Query $Query
    }
    "dump" {
        if (-not $InputPath) { throw "dump requires -InputPath <qwantum-reply.txt>" }
        & (Join-Path $PSScriptRoot "qwantum-dump.ps1") -InputPath $InputPath -SearchId $SearchId -Apply:$Apply -Force:$Force
    }
    "full" {
        & (Join-Path $PSScriptRoot "qwantum-search.ps1") -Query $Query
        Write-Host ""
        Write-Host "After Qwantum Compute replies, run:" -ForegroundColor Cyan
        Write-Host "  .\tools\qwantum-field.ps1 -Action dump -InputPath `"path\to\reply.txt`" -SearchId <id> -Apply"
    }
}