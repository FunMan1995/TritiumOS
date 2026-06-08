# Ingest Qwantum Compute reply — SEARCH+DUMP from quantum field into repo tree
param(
    [Parameter(Mandatory = $true)][string]$InputPath,
    [string]$SearchId = "",
    [switch]$Apply,
    [switch]$Force
)
$ErrorActionPreference = "Stop"
$Root = Split-Path $PSScriptRoot -Parent
if (-not (Test-Path $InputPath)) { throw "Input not found: $InputPath" }

$text = Get-Content -Path $InputPath -Raw -Encoding UTF8

function Unescape-JsonString([string]$s) {
    $s = $s -replace '\\n', "`n" -replace '\\r', "`r" -replace '\\t', "`t"
    $s = $s -replace '\\"', '"'
    $s = $s -replace '\\\\', '\'
    return $s
}

function Parse-QwantumDump([string]$body) {
    $m = [regex]::Match($body, '```qwantum-dump\s*([\s\S]*?)```', 'IgnoreCase')
    if (-not $m.Success) {
        $m = [regex]::Match($body, '```json\s*qwantum-dump\s*([\s\S]*?)```', 'IgnoreCase')
    }
    if (-not $m.Success) {
        $m = [regex]::Match($body, '"source"\s*:\s*"qwantum_compute"[\s\S]*', 'IgnoreCase')
        if ($m.Success) {
            $start = $body.IndexOf('{', $m.Index)
            if ($start -ge 0) {
                $depth = 0
                for ($i = $start; $i -lt $body.Length; $i++) {
                    if ($body[$i] -eq '{') { $depth++ }
                    elseif ($body[$i] -eq '}') {
                        $depth--
                        if ($depth -eq 0) {
                            return $body.Substring($start, $i - $start + 1)
                        }
                    }
                }
            }
        }
        return $null
    }
    return $m.Groups[1].Value.Trim()
}

$jsonRaw = Parse-QwantumDump $text
if (-not $jsonRaw) {
    Write-Warning "No qwantum-dump JSON block found. Trying markdown file fences..."
    $files = @()
    $fence = [regex]::Matches($text, '```[\w]*\s*([^\r\n]+)\r?\n([\s\S]*?)```')
    foreach ($fm in $fence) {
        $hint = $fm.Groups[1].Value.Trim()
        if ($hint -match '[\\/]' -and $hint -notmatch '^http') {
            $files += @{ path = $hint -replace '\\', '/'; content = $fm.Groups[2].Value }
        }
    }
    if ($files.Count -eq 0) { throw "Could not parse dump. Ensure Qwantum output includes ```qwantum-dump JSON block." }
    $dump = @{
        version = 1
        source  = "qwantum_compute_markdown"
        program = "TritiumOS"
        creator = "Draco"
        search_id = if ($SearchId) { $SearchId } else { [guid]::NewGuid().ToString("N").Substring(0, 12) }
        files   = $files
    }
} else {
    $dump = $jsonRaw | ConvertFrom-Json
}

if (-not $dump.search_id) { $dump | Add-Member -NotePropertyName search_id -NotePropertyValue $SearchId -Force }
if (-not $dump.search_id) { $dump.search_id = [guid]::NewGuid().ToString("N").Substring(0, 12) }
$sid = $dump.search_id

$DumpRoot = Join-Path $Root "evolve\qwantum-dump\$sid"
New-Item -ItemType Directory -Force -Path $DumpRoot | Out-Null

$written = @()
foreach ($f in $dump.files) {
    $rel = ($f.path -replace '\\', '/').TrimStart('/')
    if ($rel -match '\.\.') { Write-Warning "Skip unsafe path: $rel"; continue }
    $dest = Join-Path $DumpRoot $rel
    $dir = Split-Path $dest -Parent
    if ($dir) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
    $content = $f.content
    if ($content -is [string]) { $content = Unescape-JsonString $content }
    Set-Content -Path $dest -Value $content -Encoding UTF8 -NoNewline:$false
    $written += $rel
}

$meta = @{
    dumped_utc = (Get-Date).ToUniversalTime().ToString("o")
    input      = (Resolve-Path $InputPath).Path
    search_id  = $sid
    file_count = $written.Count
    files      = $written
    apply      = [bool]$Apply
}
$meta | ConvertTo-Json | Set-Content (Join-Path $DumpRoot "dump-manifest.json") -Encoding UTF8
Copy-Item $InputPath (Join-Path $DumpRoot "qwantum-reply-source.txt") -Force

Write-Host "Dumped $($written.Count) files -> $DumpRoot" -ForegroundColor Green
$written | ForEach-Object { Write-Host "  $_" }

if ($Apply) {
    Write-Host "Applying to repo root (merge)..." -ForegroundColor Cyan
    foreach ($rel in $written) {
        $src = Join-Path $DumpRoot $rel
        $tgt = Join-Path $Root $rel
        $td = Split-Path $tgt -Parent
        if ($td) { New-Item -ItemType Directory -Force -Path $td | Out-Null }
        if ((Test-Path $tgt) -and -not $Force) {
            Write-Warning "Exists (skip, use -Force): $rel"
            continue
        }
        Copy-Item $src $tgt -Force
        Write-Host "  applied: $rel"
    }
    Write-Host "Apply complete. Run tools\build-windows.ps1 / build-android.ps1" -ForegroundColor Green
} else {
    Write-Host "Review dump folder. To merge into repo: .\tools\qwantum-dump.ps1 -InputPath `"$InputPath`" -SearchId $sid -Apply" -ForegroundColor Yellow
}