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

New-Item -ItemType Directory -Path $outputFullPath | Out-Null
$startedAt = Get-Date
$commit = "Unknown"
$branch = "Unknown"
$sdk = "Unknown"
$catalogSummary = $null
$manifest = $null
$archiveHash = $null
$failureStage = $null
$failureMessage = $null
$verificationFailed = $false
$script:currentStage = "Preflight"
$stageStatus = [ordered]@{
    Preflight = "NotRun"
    Restore = "NotRun"
    "Debug solution build" = if ($SkipDebugBuild) { "Skipped" } else { "NotRun" }
    "Release solution build" = "NotRun"
    Readiness = "NotRun"
    "External references" = "NotRun"
    "NOTICE coverage" = "NotRun"
    "Public sample asset policy" = "NotRun"
    "Public sample execution" = "NotRun"
    "Clean Release publish" = "NotRun"
    "Release distribution contract" = "NotRun"
    "Launch smoke" = if ($SkipLaunch) { "Skipped" } else { "NotRun" }
    "Evidence materialization" = "NotRun"
}

function Invoke-NativeStep {
    param(
        [string]$Name,
        [scriptblock]$Action
    )

    Write-Host "== $Name =="
    $script:currentStage = $Name
    $stageStatus[$Name] = "Running"
    try {
        & $Action
        if ($LASTEXITCODE -ne 0) {
            throw "$Name failed with exit code $LASTEXITCODE."
        }
        $stageStatus[$Name] = "PASS"
    }
    catch {
        $stageStatus[$Name] = "FAIL"
        throw
    }
}

try {
    $stageStatus["Preflight"] = "Running"
    $script:currentStage = "Preflight"
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
    $commit = (& git -C $repoRoot rev-parse HEAD).Trim()
    $branch = (& git -C $repoRoot branch --show-current).Trim()
    $sdk = (& dotnet --version).Trim()
    $stageStatus["Preflight"] = "PASS"

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

    Invoke-NativeStep "NOTICE coverage" {
        & powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $repoRoot "tools\TestThirdPartyNoticeCoverage.ps1") `
            -OutputPath (Join-Path $outputFullPath "third_party_notice_coverage.txt")
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
        & pwsh @distributionArguments
    }
    if (-not $SkipLaunch) {
        $stageStatus["Launch smoke"] = "PASS"
    }

    $script:currentStage = "Evidence materialization"
    $stageStatus["Evidence materialization"] = "Running"
    try {
        $catalogSummaryPath = Join-Path $catalogOutput "sample_catalog_summary.json"
        $catalogSummary = Get-Content -LiteralPath $catalogSummaryPath -Raw | ConvertFrom-Json
        $manifestPath = Join-Path $releaseDirectory "clean_runtime_manifest.json"
        $manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
        $archivePath = Join-Path $repoRoot "dist\OpenVisionLab-win-x64-framework-dependent.zip"
        $archiveHash = (Get-FileHash -LiteralPath $archivePath -Algorithm SHA256).Hash
        $stageStatus["Evidence materialization"] = "PASS"
    }
    catch {
        $stageStatus["Evidence materialization"] = "FAIL"
        throw
    }
}
catch {
    $verificationFailed = $true
    $failureStage = $script:currentStage
    $failureMessage = $_.Exception.Message
    if ($stageStatus.Contains($failureStage) -and $stageStatus[$failureStage] -eq "Running") {
        $stageStatus[$failureStage] = "FAIL"
    }
    throw
}
finally {
    $completedAt = Get-Date
    $summaryStatus = if ($verificationFailed) { "FAIL" } else { "PASS" }
    $summary = [pscustomobject][ordered]@{
        Status = $summaryStatus
        FailureStage = if ($failureStage) { $failureStage } else { $null }
        FailureMessage = if ($failureMessage) { $failureMessage } else { $null }
        StartedAtUtc = $startedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        CompletedAtUtc = $completedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        DurationSeconds = [Math]::Round(($completedAt - $startedAt).TotalSeconds, 3)
        Commit = $commit
        Branch = $branch
        DotnetSdk = $sdk
        Stages = [pscustomobject]$stageStatus
        DebugBuild = $stageStatus["Debug solution build"]
        ReleaseBuild = $stageStatus["Release solution build"]
        Readiness = $stageStatus.Readiness
        ExternalReferences = $stageStatus["External references"]
        PublicSampleAssetPolicy = $stageStatus["Public sample asset policy"]
        PublicSampleGate = if ($catalogSummary) { $catalogSummary.GateStatus } else { "NotRun" }
        PublicSampleRows = if ($catalogSummary) { $catalogSummary.RunnableRows } else { "NotRun" }
        ReleaseRuntime = if ($manifest) { $manifest.Runtime } else { "NotRun" }
        SelfContained = if ($manifest) { $manifest.SelfContained } else { "NotRun" }
        PayloadFiles = if ($manifest) { $manifest.Files.Count } else { "NotRun" }
        ArchivePath = "dist\OpenVisionLab-win-x64-framework-dependent.zip"
        ArchiveSHA256 = if ($archiveHash) { $archiveHash } else { "NotRun" }
        LaunchSmoke = $stageStatus["Launch smoke"]
    }
    $summaryPath = Join-Path $outputFullPath "release_candidate_summary.json"
    try {
        $summary | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $summaryPath -Encoding UTF8
    }
    catch {
        Write-Warning "Could not write release candidate summary: $($_.Exception.Message)"
        if ($summaryStatus -eq "PASS") {
            throw
        }
    }
    Write-Host "ReleaseCandidateVerification=$summaryStatus"
    Write-Host "Summary=$summaryPath"
    Write-Host "Commit=$commit"
    Write-Host "PublicSampleRows=$($summary.PublicSampleRows)"
    Write-Host "ArchiveSHA256=$($summary.ArchiveSHA256)"
}
