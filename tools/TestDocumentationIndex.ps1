[CmdletBinding()]
param(
    [string]$RepoRoot = ''
)

$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($RepoRoot)) {
    $scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
    $RepoRoot = Split-Path -Parent $scriptDirectory
}

$resolvedRepoRoot = [System.IO.Path]::GetFullPath($RepoRoot)
$repoPrefix = $resolvedRepoRoot.TrimEnd('\', '/') + [System.IO.Path]::DirectorySeparatorChar
$indexPath = Join-Path $resolvedRepoRoot 'docs\LLM_DOCUMENT_INDEX.json'
$docsRoot = Join-Path $resolvedRepoRoot 'docs'
$errors = [System.Collections.Generic.List[string]]::new()

if (-not (Test-Path -LiteralPath $indexPath -PathType Leaf)) {
    throw "Documentation index was not found: $indexPath"
}

try {
    $index = Get-Content -LiteralPath $indexPath -Raw | ConvertFrom-Json
}
catch {
    throw "Documentation index is not valid JSON: $($_.Exception.Message)"
}

function Add-IndexedPath {
    param(
        [System.Collections.Generic.List[string]]$Target,
        [object]$Value
    )

    if ($null -ne $Value -and -not [string]::IsNullOrWhiteSpace([string]$Value)) {
        $Target.Add(([string]$Value).Replace('\', '/'))
    }
}

$indexedPaths = [System.Collections.Generic.List[string]]::new()
Add-IndexedPath $indexedPaths $index.entrypoint
Add-IndexedPath $indexedPaths $index.detailedRegistry
foreach ($item in @($index.authority)) { Add-IndexedPath $indexedPaths $item.path }
foreach ($route in @($index.routes)) {
    foreach ($path in @($route.read)) { Add-IndexedPath $indexedPaths $path }
}
foreach ($item in @($index.references)) { Add-IndexedPath $indexedPaths $item.path }
foreach ($path in @($index.discovery.pNumberSearchOrder)) {
    if ([string]$path -notin @('docs/reports', 'artifacts')) {
        Add-IndexedPath $indexedPaths $path
    }
}

$routeIds = @($index.routes | ForEach-Object { [string]$_.id })
$duplicateRouteIds = @($routeIds | Group-Object | Where-Object Count -gt 1 | ForEach-Object Name)
if ($duplicateRouteIds.Count -gt 0) {
    $errors.Add("Duplicate route ids: $($duplicateRouteIds -join ', ')")
}

$authorityRanks = @($index.authority | ForEach-Object { [int]$_.rank })
$duplicateAuthorityRanks = @($authorityRanks | Group-Object | Where-Object Count -gt 1 | ForEach-Object Name)
if ($duplicateAuthorityRanks.Count -gt 0) {
    $errors.Add("Duplicate authority ranks: $($duplicateAuthorityRanks -join ', ')")
}

$uniqueIndexedPaths = @($indexedPaths | Sort-Object -Unique)
foreach ($relativePath in $uniqueIndexedPaths) {
    $fullPath = [System.IO.Path]::GetFullPath((Join-Path $resolvedRepoRoot ($relativePath -replace '/', '\')))
    if (-not $fullPath.StartsWith($repoPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        $errors.Add("Indexed path escapes the repository: $relativePath")
        continue
    }

    if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
        $errors.Add("Indexed document does not exist: $relativePath")
        continue
    }

    if ([System.IO.Path]::GetExtension($fullPath) -in @('.md', '.json')) {
        $raw = Get-Content -LiteralPath $fullPath -Raw
        $head = $raw.Substring(0, [Math]::Min(320, $raw.Length))
        if ($head -match '^# Moved to canonical location' -or $head -match '"status"\s*:\s*"moved"') {
            $errors.Add("Index points to a compatibility redirect instead of its canonical document: $relativePath")
        }
    }
}

$rootRedirectCount = 0
$rootFiles = Get-ChildItem -LiteralPath $docsRoot -File | Where-Object {
    $_.Extension -in @('.md', '.json', '.xsd') -and
    $_.Name -notin @('README.md', 'LLM_DOCUMENT_INDEX.json')
}

foreach ($file in $rootFiles) {
    $raw = Get-Content -LiteralPath $file.FullName -Raw
    $target = $null

    if ($raw -match 'Canonical location:\s*\[[^\]]+\]\((?<target>[^)]+)\)') {
        $target = $Matches.target
    }
    elseif ($raw -match 'New location:\s*\[[^\]]+\]\((?<target>[^)]+)\)') {
        $target = $Matches.target
        if ($target.StartsWith('docs/')) {
            $target = $target.Substring(5)
        }
    }
    elseif ($file.Extension -eq '.json') {
        try {
            $redirect = $raw | ConvertFrom-Json
            if ([string]$redirect.status -eq 'moved') {
                $target = [string]$redirect.movedTo
                if ($target.StartsWith('docs/')) {
                    $target = $target.Substring(5)
                }
            }
        }
        catch {
            $errors.Add("Root JSON file is neither a valid redirect nor a canonical entrypoint: docs/$($file.Name)")
            continue
        }
    }

    if ([string]::IsNullOrWhiteSpace($target)) {
        $errors.Add("Unexpected canonical file at docs root: docs/$($file.Name)")
        continue
    }

    $rootRedirectCount++
    $targetPath = [System.IO.Path]::GetFullPath((Join-Path $docsRoot ($target -replace '/', '\')))
    $docsPrefix = $docsRoot.TrimEnd('\', '/') + [System.IO.Path]::DirectorySeparatorChar
    if (-not $targetPath.StartsWith($docsPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        $errors.Add("Root redirect escapes docs: docs/$($file.Name) -> $target")
    }
    elseif (-not (Test-Path -LiteralPath $targetPath -PathType Leaf)) {
        $errors.Add("Broken root redirect: docs/$($file.Name) -> $target")
    }
}

if ($errors.Count -gt 0) {
    $errors | ForEach-Object { Write-Output "ERROR: $_" }
    exit 1
}

Write-Output ("DocumentationIndex=PASS IndexedPaths={0} Routes={1} RootRedirects={2}" -f $uniqueIndexedPaths.Count, @($index.routes).Count, $rootRedirectCount)
