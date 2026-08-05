param(
    [string]$OutputDir = "artifacts\release_evidence",
    [string]$PlatformPrecheckSummaryPath = "",
    [string]$SampleCatalogSummaryPath = "",
    [string]$TagName = "",
    [string]$VisionSdkVersion = "",
    [string]$WpgCustomVersion = "",
    [string[]]$PreparedDllPaths = @(
        "dll\OpenVisionLab-Vision-SDK\sdk-manifest.json",
        "dll\OpenVisionLab-Vision-SDK\OpenVisionLab.Core.dll",
        "dll\OpenVisionLab-Vision-SDK\OpenVisionLab.Vision2D.dll",
        "dll\OpenVisionLab-Vision-SDK\OpenVisionLab.Vision2D.Blob.dll",
        "dll\OpenVisionLab-Vision-SDK\OpenCvSharp.dll",
        "dll\OpenVisionLab-Vision-SDK\OpenCvSharp.Blob.dll",
        "dll\OpenCVSharp\OpenCvSharpExtern.dll",
        "dll\System.Windows.Controls.WpfPropertyGrid.dll"
    )
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Resolve-RepoPath {
    param([string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ""
    }

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Convert-ToDisplayPath {
    param([string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ""
    }

    $fullPath = [System.IO.Path]::GetFullPath($Path)
    $rootPath = [System.IO.Path]::GetFullPath($repoRoot)
    if (-not $rootPath.EndsWith([System.IO.Path]::DirectorySeparatorChar.ToString())) {
        $rootPath += [System.IO.Path]::DirectorySeparatorChar
    }

    if ($fullPath.StartsWith($rootPath, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $fullPath.Substring($rootPath.Length)
    }

    return $fullPath
}

function Find-LatestSummary {
    param(
        [string]$DirectoryPattern,
        [string]$SummaryFileName
    )

    $artifactRoot = Join-Path $repoRoot "artifacts"
    if (-not (Test-Path -LiteralPath $artifactRoot)) {
        return ""
    }

    $dirs = @(Get-ChildItem -LiteralPath $artifactRoot -Directory |
        Where-Object { $_.Name -like $DirectoryPattern } |
        Sort-Object LastWriteTime -Descending)

    foreach ($dir in $dirs) {
        $candidate = Join-Path $dir.FullName $SummaryFileName
        if (Test-Path -LiteralPath $candidate) {
            return $candidate
        }
    }

    return ""
}

function Read-JsonFile {
    param([string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path)) {
        return $null
    }

    return Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
}

function Invoke-GitText {
    param([string[]]$Arguments)

    try {
        $output = & git @Arguments 2>$null
        if ($LASTEXITCODE -eq 0) {
            return (($output | ForEach-Object { $_.ToString() }) -join "`n").Trim()
        }
    }
    catch {
    }

    return ""
}

$platformSummaryFullPath = Resolve-RepoPath $PlatformPrecheckSummaryPath
if ([string]::IsNullOrWhiteSpace($platformSummaryFullPath)) {
    $platformSummaryFullPath = Find-LatestSummary "platform_precheck_*" "platform_precheck_summary.json"
}
$platformSummary = Read-JsonFile $platformSummaryFullPath

$sampleSummaryFullPath = Resolve-RepoPath $SampleCatalogSummaryPath
if ([string]::IsNullOrWhiteSpace($sampleSummaryFullPath)) {
    $sampleSummaryFullPath = Find-LatestSummary "sample_catalog_*" "sample_catalog_summary.json"
}
if ([string]::IsNullOrWhiteSpace($sampleSummaryFullPath) -and $null -ne $platformSummary -and $null -ne $platformSummary.SampleCatalog) {
    $sampleSummaryFullPath = Resolve-RepoPath $platformSummary.SampleCatalog.SummaryJsonPath
}
$sampleSummary = Read-JsonFile $sampleSummaryFullPath
$sampleSource = if ($null -ne $sampleSummary) { $sampleSummary } elseif ($null -ne $platformSummary) { $platformSummary.SampleCatalog } else { $null }

$gitCommit = Invoke-GitText @("rev-parse", "HEAD")
$gitBranch = Invoke-GitText @("branch", "--show-current")
$gitAvailable = -not [string]::IsNullOrWhiteSpace($gitCommit)
$dirtyText = if ($gitAvailable) { Invoke-GitText @("status", "--short") } else { "" }
$dirtyLines = @()
if (-not [string]::IsNullOrWhiteSpace($dirtyText)) {
    $dirtyLines = @($dirtyText -split "\r?\n" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
}

$preparedDlls = @(
    foreach ($path in $PreparedDllPaths) {
        $fullPath = Resolve-RepoPath $path
        if ([string]::IsNullOrWhiteSpace($fullPath)) {
            continue
        }

        if (Test-Path -LiteralPath $fullPath) {
            $file = Get-Item -LiteralPath $fullPath
            $hash = Get-FileHash -LiteralPath $fullPath -Algorithm SHA256
            [pscustomobject][ordered]@{
                Path = Convert-ToDisplayPath $fullPath
                SHA256 = $hash.Hash
                Length = $file.Length
                LastWriteTime = $file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                Missing = $false
            }
        }
        else {
            [pscustomobject][ordered]@{
                Path = $path
                SHA256 = ""
                Length = 0
                LastWriteTime = ""
                Missing = $true
            }
        }
    }
)

$platformOk = $null -ne $platformSummary -and $platformSummary.Status -eq "OK"
$sampleOk = $null -ne $sampleSource -and $sampleSource.GateStatus -eq "OK" -and [int]$sampleSource.NGRows -eq 0
$dllOk = @($preparedDlls | Where-Object { $_.Missing }).Count -eq 0
$releaseGateOk = $platformOk -and $sampleOk -and $dllOk
$tagReady = $releaseGateOk -and $gitAvailable -and $dirtyLines.Count -eq 0

$evidence = [pscustomobject][ordered]@{
    Time = (Get-Date -Format "yyyy-MM-dd HH:mm:ss")
    TagName = $TagName
    ReleaseGateOk = $releaseGateOk
    TagReady = $tagReady
    OpenVisionLab = [pscustomobject][ordered]@{
        GitAvailable = $gitAvailable
        Commit = $gitCommit
        Branch = $gitBranch
        WorkingTreeClean = ($gitAvailable -and $dirtyLines.Count -eq 0)
        DirtyFileCount = if ($gitAvailable) { $dirtyLines.Count } else { $null }
        DirtyFiles = $dirtyLines
    }
    References = [pscustomobject][ordered]@{
        OpenVisionLabVisionSdk = $VisionSdkVersion
        WpgCustom = $WpgCustomVersion
    }
    PlatformPrecheck = [pscustomobject][ordered]@{
        SummaryPath = Convert-ToDisplayPath $platformSummaryFullPath
        Status = if ($null -ne $platformSummary) { $platformSummary.Status } else { "" }
        DurationSeconds = if ($null -ne $platformSummary) { $platformSummary.DurationSeconds } else { 0 }
        Gates = if ($null -ne $platformSummary) { $platformSummary.Gates } else { @() }
        Artifacts = if ($null -ne $platformSummary) { $platformSummary.Artifacts } else { $null }
    }
    SampleCatalog = [pscustomobject][ordered]@{
        SummaryPath = Convert-ToDisplayPath $sampleSummaryFullPath
        GateStatus = if ($null -ne $sampleSource) { $sampleSource.GateStatus } else { "" }
        RunnableRows = if ($null -ne $sampleSource) { $sampleSource.RunnableRows } else { 0 }
        RequiredRows = if ($null -ne $sampleSource) { $sampleSource.RequiredRows } else { 0 }
        ExploreRows = if ($null -ne $sampleSource) { $sampleSource.ExploreRows } else { 0 }
        ExpectedFailureRows = if ($null -ne $sampleSource -and $null -ne $sampleSource.ExpectedFailureRows) { $sampleSource.ExpectedFailureRows } else { 0 }
        OKRows = if ($null -ne $sampleSource) { $sampleSource.OKRows } else { 0 }
        NGRows = if ($null -ne $sampleSource) { $sampleSource.NGRows } else { 0 }
        ArtifactIssueCount = if ($null -ne $sampleSource) { $sampleSource.ArtifactIssueCount } else { 0 }
        MetadataIssueCount = if ($null -ne $sampleSource) { $sampleSource.MetadataIssueCount } else { 0 }
        UncoveredSampleFolderCount = if ($null -ne $sampleSource) { $sampleSource.UncoveredSampleFolderCount } else { 0 }
        RunnerPath = if ($null -ne $sampleSource) { $sampleSource.RunnerPath } else { "" }
    }
    PreparedDlls = $preparedDlls
}

$outputFullPath = Resolve-RepoPath $OutputDir
New-Item -ItemType Directory -Force -Path $outputFullPath | Out-Null

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$jsonPath = Join-Path $outputFullPath "openvisionlab_release_evidence_$timestamp.json"
$evidence | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $jsonPath -Encoding UTF8

Write-Host "Release evidence: $(Convert-ToDisplayPath $jsonPath)"
Write-Host "ReleaseGateOk: $releaseGateOk"
Write-Host "TagReady: $tagReady"
if ($dirtyLines.Count -gt 0) {
    Write-Host "Working tree has $($dirtyLines.Count) changed file(s). Commit before tagging."
}
if (-not $gitAvailable) {
    Write-Host "Git status is unavailable. TagReady remains false."
}
