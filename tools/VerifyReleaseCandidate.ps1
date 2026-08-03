param(
    [string]$OutputDir = "",
    [switch]$SkipDebugBuild,
    [switch]$SkipLaunch
)

$ErrorActionPreference = "Stop"

$repoRoot = [System.IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
if ([string]::IsNullOrWhiteSpace($OutputDir)) {
    $OutputDir = "artifacts\release_candidate_" + (Get-Date -Format "yyyyMMdd_HHmmss")
}
$outputFullPath = if ([System.IO.Path]::IsPathRooted($OutputDir)) {
    [System.IO.Path]::GetFullPath($OutputDir)
}
else {
    [System.IO.Path]::GetFullPath((Join-Path $repoRoot $OutputDir))
}
$artifactRoot = [System.IO.Path]::GetFullPath((Join-Path $repoRoot "artifacts"))
$artifactPrefix = $artifactRoot.TrimEnd('\', '/') + [System.IO.Path]::DirectorySeparatorChar
if (-not $outputFullPath.StartsWith($artifactPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "OutputDir must be a new directory under artifacts: $outputFullPath"
}
if (Test-Path -LiteralPath $outputFullPath) {
    throw "Release candidate evidence directory already exists: $outputFullPath"
}

$longPathsEnabled = Get-ItemPropertyValue `
    -LiteralPath "HKLM:\SYSTEM\CurrentControlSet\Control\FileSystem" `
    -Name LongPathsEnabled `
    -ErrorAction SilentlyContinue
$wpfGeneratedPathProbe = Join-Path $repoRoot `
    "src\Libraries\OpenVisionLab.Logging.Controls\obj\Any CPU\Release\net8.0-windows7.0\OpenVisionLab.Logging.Controls_00000000_wpftmp.GeneratedMSBuildEditorConfig.editorconfig"
if ($longPathsEnabled -ne 1 -and $wpfGeneratedPathProbe.Length -ge 260) {
    throw "Windows long-path support is disabled and this checkout is too deep for WPF/MSBuild release validation ($($wpfGeneratedPathProbe.Length) characters). Clone OpenVisionLab to a shorter path such as C:\src\OpenVisionLab or D:\src\OpenVisionLab, then run this command again."
}

$trackedStatus = (& git -C $repoRoot status --porcelain --untracked-files=no) -join "`n"
if ($LASTEXITCODE -ne 0) {
    throw "Could not inspect the Git working tree."
}
if (-not [string]::IsNullOrWhiteSpace($trackedStatus)) {
    throw "Release candidate verification requires a clean tracked working tree."
}

$releaseDirectory = Join-Path $repoRoot "dist\OpenVisionLab"
if (Test-Path -LiteralPath $releaseDirectory) {
    throw "Release output already exists. Use a clean clone or remove the generated dist output before verification: $releaseDirectory"
}

New-Item -ItemType Directory -Path $outputFullPath | Out-Null
$startedAt = Get-Date
$commit = (& git -C $repoRoot rev-parse HEAD).Trim()
$branch = (& git -C $repoRoot branch --show-current).Trim()
$sdk = (& dotnet --version).Trim()

function Invoke-NativeStep {
    param(
        [string]$Name,
        [scriptblock]$Action
    )

    Write-Host "== $Name =="
    & $Action
    if ($LASTEXITCODE -ne 0) {
        throw "$Name failed with exit code $LASTEXITCODE."
    }
}

Invoke-NativeStep "Restore" {
    & dotnet restore (Join-Path $repoRoot "OpenVisionLab.sln") --locked-mode
}

if (-not $SkipDebugBuild) {
    Invoke-NativeStep "Debug solution build" {
        & dotnet build (Join-Path $repoRoot "OpenVisionLab.sln") -c Debug -p:Platform="Any CPU" --no-restore
    }
}

Invoke-NativeStep "Release solution build" {
    & dotnet build (Join-Path $repoRoot "OpenVisionLab.sln") -c Release -p:Platform="Any CPU" --no-restore
}

Invoke-NativeStep "Readiness" {
    & dotnet run --project (Join-Path $repoRoot "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj") -c Release --no-build -- $repoRoot
}

Invoke-NativeStep "External references" {
    & powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $repoRoot "tools\TestExternalReferences.ps1") -Configuration Release
}

Invoke-NativeStep "Public sample asset policy" {
    & powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $repoRoot "tools\TestPublicSampleAssets.ps1")
}

$catalogOutput = Join-Path $outputFullPath "public_sample_catalog"
Invoke-NativeStep "Public sample execution" {
    & powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $repoRoot "tools\RunVisionSampleCatalog.ps1") `
        -Configuration Release `
        -Platform "Any CPU" `
        -CatalogPath "docs\samples\OpenVisionLab.PublicSampleCatalog.csv" `
        -OutputDir $catalogOutput
}

Invoke-NativeStep "Clean Release publish" {
    & powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $repoRoot "tools\BuildCleanRuntime.ps1") -Mode Release
}

$distributionArguments = @(
    "-NoProfile",
    "-ExecutionPolicy",
    "Bypass",
    "-File",
    (Join-Path $repoRoot "tools\TestReleaseDistribution.ps1")
)
if ($SkipLaunch) {
    $distributionArguments += "-SkipLaunch"
}
Invoke-NativeStep "Release distribution contract" {
    & powershell @distributionArguments
}

$catalogSummaryPath = Join-Path $catalogOutput "sample_catalog_summary.json"
$catalogSummary = Get-Content -LiteralPath $catalogSummaryPath -Raw | ConvertFrom-Json
$manifestPath = Join-Path $releaseDirectory "clean_runtime_manifest.json"
$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
$archivePath = Join-Path $repoRoot "dist\OpenVisionLab-win-x64-framework-dependent.zip"
$archiveHash = (Get-FileHash -LiteralPath $archivePath -Algorithm SHA256).Hash
$completedAt = Get-Date

$summary = [pscustomobject][ordered]@{
    Status = "PASS"
    StartedAtUtc = $startedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    CompletedAtUtc = $completedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    DurationSeconds = [Math]::Round(($completedAt - $startedAt).TotalSeconds, 3)
    Commit = $commit
    Branch = $branch
    DotnetSdk = $sdk
    DebugBuild = if ($SkipDebugBuild) { "Skipped" } else { "PASS" }
    ReleaseBuild = "PASS"
    Readiness = "PASS"
    ExternalReferences = "PASS"
    PublicSampleAssetPolicy = "PASS"
    PublicSampleGate = $catalogSummary.GateStatus
    PublicSampleRows = $catalogSummary.RunnableRows
    ReleaseRuntime = $manifest.Runtime
    SelfContained = $manifest.SelfContained
    PayloadFiles = $manifest.Files.Count
    ArchivePath = "dist\OpenVisionLab-win-x64-framework-dependent.zip"
    ArchiveSHA256 = $archiveHash
    LaunchSmoke = if ($SkipLaunch) { "Skipped" } else { "PASS" }
}
$summaryPath = Join-Path $outputFullPath "release_candidate_summary.json"
$summary | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $summaryPath -Encoding UTF8

Write-Host "ReleaseCandidateVerification=PASS"
Write-Host "Summary=$summaryPath"
Write-Host "Commit=$commit"
Write-Host "PublicSampleRows=$($catalogSummary.RunnableRows)"
Write-Host "ArchiveSHA256=$archiveHash"
