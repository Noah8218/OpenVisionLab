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
$strictUtf8 = New-Object System.Text.UTF8Encoding($false, $true)

if (-not (Test-Path -LiteralPath $indexPath -PathType Leaf)) {
    throw "Documentation index was not found: $indexPath"
}

try {
    $index = [System.IO.File]::ReadAllText($indexPath, $strictUtf8) | ConvertFrom-Json
}
catch {
    throw "Documentation index is not valid UTF-8 JSON: $($_.Exception.Message)"
}

function Read-StrictUtf8Document {
    param([string]$Path)

    try {
        return [System.IO.File]::ReadAllText($Path, $strictUtf8)
    }
    catch {
        $relativePath = $Path.Substring($resolvedRepoRoot.Length).TrimStart('\', '/')
        $errors.Add("Document is not valid UTF-8: $relativePath")
        return $null
    }
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
    foreach ($lookup in @($route.lookup)) {
        if ($null -eq $lookup) { continue }
        Add-IndexedPath $indexedPaths $lookup.path
        if ([string]::IsNullOrWhiteSpace([string]$lookup.path) -or [string]::IsNullOrWhiteSpace([string]$lookup.when)) {
            $errors.Add("Conditional lookup requires path and when: $($route.id)")
        }
    }
}
foreach ($item in @($index.references)) { Add-IndexedPath $indexedPaths $item.path }
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

$startRoutes = @($index.routes | Where-Object { $_.id -eq 'start_or_continue' })
$startupPaths = @()
if ($startRoutes.Count -ne 1) {
    $errors.Add('Exactly one start_or_continue route is required.')
}
else {
    $startupPaths = @($startRoutes[0].read | Sort-Object -Unique)
    if ($startupPaths.Count -eq 0) {
        $errors.Add('The start_or_continue route must name at least one entry document.')
    }
    foreach ($required in @('AGENTS.md', 'docs/README.md')) {
        if ($startupPaths -notcontains $required) {
            $errors.Add("The start_or_continue route is missing an entry document: $required")
        }
    }
    $prioritySource = [string]$index.rules.currentPrioritySource
    if (-not [string]::IsNullOrWhiteSpace($prioritySource) -and $startupPaths -notcontains $prioritySource) {
        $errors.Add("The start_or_continue route is missing its declared currentPrioritySource: $prioritySource")
    }
    if ($index.rules.authorityIsReadingBundle -ne $false) {
        $errors.Add('Authority ranks must not become an automatic full-document reading bundle.')
    }
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

    if ([System.IO.Path]::GetExtension($fullPath) -in @('.md', '.json', '.xsd', '.html')) {
        $raw = Read-StrictUtf8Document $fullPath
        if ($null -eq $raw) {
            continue
        }
        $head = $raw.Substring(0, [Math]::Min(320, $raw.Length))
        if ($head -match '^# Moved to canonical location' -or $head -match '"status"\s*:\s*"moved"') {
            $errors.Add("Index points to a compatibility redirect instead of its canonical document: $relativePath")
        }
    }
}

$rootRedirectCount = 0
$rootFiles = Get-ChildItem -LiteralPath $docsRoot -File | Where-Object {
    $_.Extension -in @('.md', '.json', '.xsd', '.html') -and
    $_.Name -notin @('README.md', 'LLM_DOCUMENT_INDEX.json')
}

foreach ($file in $rootFiles) {
    $raw = Read-StrictUtf8Document $file.FullName
    if ($null -eq $raw) {
        continue
    }
    $target = $null

    if ($raw -match 'Canonical(?: location)?:\s*\[[^\]]+\]\((?<target>[^)]+)\)') {
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

$startupBytes = ($startupPaths | ForEach-Object { (Get-Item -LiteralPath (Join-Path $resolvedRepoRoot $_)).Length } | Measure-Object -Sum).Sum
Write-Output ("DocumentationIndex=PASS IndexedPaths={0} Routes={1} RootRedirects={2} StartupDocuments={3} StartupBytes={4}" -f $uniqueIndexedPaths.Count, @($index.routes).Count, $rootRedirectCount, $startupPaths.Count, $startupBytes)
