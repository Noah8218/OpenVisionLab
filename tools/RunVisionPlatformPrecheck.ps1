param(
    [string]$Configuration = "Debug",
    [string]$Platform = "Any CPU",
    [string]$OutputDir = "C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck",
    [string]$UiTargets = "",
    [string]$WpgCustomSourceRoot = "",
    [bool]$WpgCustomBuildEnabled = $false,
    [switch]$SkipUi,
    [switch]$SkipRestore,
    [switch]$FailOnUiWarn,
    [switch]$WpfTools,
    [switch]$ToolOutputFlow,
    [switch]$VisibleUiCapture,
    [switch]$SkipSampleRunnerBuild,
    [switch]$SkipWpfShellBuild
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$platformPrecheckStartedAt = Get-Date
$platformPrecheckDurationsSeconds = [ordered]@{
    RestoreSeconds = 0
    BuildSeconds = 0
    WpfShellRestoreSeconds = 0
    WpfShellBuildSeconds = 0
    WpfShellContractSeconds = 0
    SampleCatalogSeconds = 0
    UiPrecheckSeconds = 0
    ToolOpenPerfGateSeconds = 0
}
$parallelBuildArguments = @("/m")

function Invoke-Stage {
    param(
        [string]$Name,
        [scriptblock]$Action
    )

    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    try {
        & $Action
    }
    finally {
        $stopwatch.Stop()
        $script:platformPrecheckDurationsSeconds[$Name] = [Math]::Round($stopwatch.Elapsed.TotalSeconds, 3)
        Write-Host "== $Name duration: $($script:platformPrecheckDurationsSeconds[$Name]) sec"
    }
}

$solution = Join-Path $repoRoot "OpenVisionLab.sln"
$xmlCheckProject = Join-Path $repoRoot "tools\RecipeXmlCompatibilityCheck\RecipeXmlCompatibilityCheck.csproj"
$runnerProject = Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj"
$visionUiContractProject = Join-Path $repoRoot "tools\VisionUiContractCheck\VisionUiContractCheck.csproj"
$historyContractProject = Join-Path $repoRoot "tools\HistoryContractCheck\HistoryContractCheck.csproj"
$localizationCatalogContractProject = Join-Path $repoRoot "tools\LocalizationCatalogCheck\LocalizationCatalogCheck.csproj"
$openVisionReadinessCheckProject = Join-Path $repoRoot "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj"
$screenshotSmokeProject = Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj"
$screenshotSmokeDllCandidates = @(
    (Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\$Platform\$Configuration\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll"),
    (Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\$Platform\$Configuration\net8.0-windows\PipelineViewerScreenshotSmoke.dll"),
    (Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\$Configuration\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll"),
    (Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\$Configuration\net8.0-windows\PipelineViewerScreenshotSmoke.dll"),
    (Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll"),
    (Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\net8.0-windows\PipelineViewerScreenshotSmoke.dll")
)
$runnerExeCandidates = @(
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\$Platform\$Configuration\net8.0-windows7.0\VisionRecipeRunnerSmoke.exe"),
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\$Platform\$Configuration\net8.0-windows\VisionRecipeRunnerSmoke.exe"),
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\$Configuration\net8.0-windows7.0\VisionRecipeRunnerSmoke.exe"),
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\$Configuration\net8.0-windows\VisionRecipeRunnerSmoke.exe"),
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\net8.0-windows7.0\VisionRecipeRunnerSmoke.exe"),
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\net8.0-windows\VisionRecipeRunnerSmoke.exe")
)
$sampleCatalogScript = Join-Path $repoRoot "tools\RunVisionSampleCatalog.ps1"
$uiPrecheckScript = Join-Path $repoRoot "tools\RunUiPrecheck.ps1"
$externalReferenceScript = Join-Path $repoRoot "tools\TestExternalReferences.ps1"
$docsSamples = Join-Path $repoRoot "docs\samples"
$tutorialHtml = Join-Path $repoRoot "docs\learn\OPENVISIONLAB_TUTORIAL.html"
$portableTutorialHtml = Join-Path $repoRoot "docs\learn\OPENVISIONLAB_TUTORIAL_PORTABLE.html"
$buildOutDir = ""
$msBuildCandidates = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
)
$msBuild = $msBuildCandidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1

$buildOutDir = [System.IO.Path]::GetFullPath((Join-Path $OutputDir "OpenVisionLabBuild"))
if (-not $buildOutDir.EndsWith([System.IO.Path]::DirectorySeparatorChar.ToString())) {
    $buildOutDir += [System.IO.Path]::DirectorySeparatorChar
}

[void][System.IO.Directory]::CreateDirectory($OutputDir)
[void][System.IO.Directory]::CreateDirectory($buildOutDir)
$reportPath = Join-Path $OutputDir "platform_precheck_report.md"
$summaryPath = Join-Path $OutputDir "platform_precheck_summary.json"
$report = New-Object System.Collections.Generic.List[string]
$precheckStartedAt = Get-Date
$report.Add("# OpenVisionLab Platform Precheck") | Out-Null
$report.Add("") | Out-Null
$report.Add("- Time: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")") | Out-Null
$report.Add("- Build: $Configuration / $Platform") | Out-Null
$report.Add("- UI precheck: $(if ($SkipUi) { 'skipped' } else { 'enabled' })") | Out-Null
$report.Add("- WPF tools: $(if ($WpfTools) { 'enabled' } else { 'disabled' })") | Out-Null
$report.Add("- Tool output flow: $(if ($ToolOutputFlow) { 'enabled' } else { 'disabled' })") | Out-Null
$report.Add("- Output: ``$OutputDir``") | Out-Null
$report.Add("- Skip sample runner build: $(if ($SkipSampleRunnerBuild) { 'true' } else { 'false' })") | Out-Null
$report.Add("- Skip WPF shell build: $(if ($SkipWpfShellBuild) { 'true' } else { 'false' })") | Out-Null
$report.Add("") | Out-Null
$wpgCustomBuildEnabledArgument = if ($WpgCustomBuildEnabled) { "true" } else { "false" }

function Add-ReportBlock {
    param(
        [string]$Title,
        [string[]]$Lines
    )

    $report.Add("## $Title") | Out-Null
    $report.Add("") | Out-Null
    $report.Add('```text') | Out-Null
    foreach ($line in $Lines) {
        $report.Add($line) | Out-Null
    }
    $report.Add('```') | Out-Null
    $report.Add("") | Out-Null
}

function Resolve-ScreenshotSmokeDll {
    param(
        [string[]]$Candidates,
        [string]$ProjectPath
    )

    $resolved = $Candidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
    if (-not [string]::IsNullOrWhiteSpace($resolved)) {
        return $resolved
    }

    $projectDir = Split-Path -Parent $ProjectPath
    $fallback = Get-ChildItem -LiteralPath $projectDir -Filter "PipelineViewerScreenshotSmoke.dll" -Recurse -File -ErrorAction SilentlyContinue |
        Sort-Object FullName |
        Select-Object -First 1
    if ($null -ne $fallback) {
        return $fallback.FullName
    }

    return ""
}

function Resolve-RunnerExecutable {
    param(
        [string[]]$Candidates,
        [string]$ProjectName,
        [string]$ProjectPath
    )

    $directHit = $Candidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
    if (-not [string]::IsNullOrWhiteSpace($directHit)) {
        return $directHit
    }

    $projectDir = Split-Path -Parent $ProjectPath
    $fallback = Get-ChildItem -LiteralPath $projectDir -Filter $ProjectName -Recurse -File -ErrorAction SilentlyContinue |
        Sort-Object FullName |
        Select-Object -First 1
    if ($null -ne $fallback) {
        return $fallback.FullName
    }

    return ""
}

Write-Host "== Vendored DLLs =="
$externalReferenceReportPath = Join-Path $OutputDir "external_reference_check.txt"
$externalReferenceArgs = @(
    "-ExecutionPolicy",
    "Bypass",
    "-File",
    $externalReferenceScript,
    "-Configuration",
    $Configuration,
    "-WpgCustomBuildEnabled",
    $wpgCustomBuildEnabledArgument,
    "-OutputPath",
    $externalReferenceReportPath
)
$externalReferenceOutput = & powershell @externalReferenceArgs 2>&1
$externalReferenceExit = $LASTEXITCODE
$externalReferenceOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "Vendored DLLs" $externalReferenceOutput
if ($externalReferenceExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Vendored DLL check failed. See $reportPath"
}

$restoreArguments = @(
    "restore",
    $solution,
    "/p:Configuration=$Configuration",
    "/p:Platform=$Platform",
    "/p:WpgCustomBuildEnabled=$WpgCustomBuildEnabled"
)
if ($SkipRestore) {
    $restoreOutput = @("Restore skipped by -SkipRestore.")
    $restoreExit = 0
}
else {
    Invoke-Stage -Name "RestoreSeconds" -Action {
        Write-Host "== Restore Solution =="
        $script:restoreOutput = & dotnet @restoreArguments 2>&1
        $script:restoreExit = $LASTEXITCODE
    }
    $restoreOutput = $script:restoreOutput
    $restoreExit = [int]$script:restoreExit
    $restoreOutput | ForEach-Object { Write-Host $_ }
}
Add-ReportBlock "Restore" $restoreOutput
if ($restoreExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Restore failed. See $reportPath"
}

Write-Host "== Build OpenVisionLab =="
$buildProperties = @(
    "/p:Configuration=$Configuration",
    "/p:Platform=$Platform",
    "/p:RestorePackages=false",
    "/p:OutDir=$buildOutDir",
    "/p:WpgCustomBuildEnabled=$WpgCustomBuildEnabled"
)
Invoke-Stage -Name "BuildSeconds" -Action {
    if (-not [string]::IsNullOrWhiteSpace($msBuild)) {
        $script:buildOutput = & $msBuild $solution /t:Build @buildProperties @parallelBuildArguments /v:minimal 2>&1
        $script:buildExit = $LASTEXITCODE
    }
    else {
        $dotnetBuildArguments = @(
            "build",
            $solution,
            "-c",
            $Configuration,
            "--maxcpucount",
            "--no-restore",
            "/p:Platform=$Platform",
            "/p:RestorePackages=false",
            "/p:OutDir=$buildOutDir",
            "/p:WpgCustomBuildEnabled=$WpgCustomBuildEnabled"
        )
        $script:buildOutput = & dotnet @dotnetBuildArguments 2>&1
        $script:buildExit = $LASTEXITCODE
    }
}
$buildOutput = $script:buildOutput
$buildExit = [int]$script:buildExit
$buildOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "Build" $buildOutput
if ($buildExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Build failed. See $reportPath"
}

Write-Host "== Vision UI Contract =="
$visionUiOutput = & dotnet run --project $visionUiContractProject -c $Configuration -- $buildOutDir 2>&1
$visionUiExit = $LASTEXITCODE
$visionUiOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "Vision UI Contract" $visionUiOutput
if ($visionUiExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Vision UI contract failed. See $reportPath"
}

Write-Host "== History Contract =="
$historyContractOutput = & dotnet run --project $historyContractProject -c $Configuration 2>&1
$historyContractExit = $LASTEXITCODE
$historyContractOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "History Contract" $historyContractOutput
if ($historyContractExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "History contract failed. See $reportPath"
}

Write-Host "== Localization Catalog Contract =="
$localizationCatalogOutput = & dotnet run --project $localizationCatalogContractProject -c $Configuration -- $repoRoot 2>&1
$localizationCatalogExit = $LASTEXITCODE
$localizationCatalogOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "Localization Catalog Contract" $localizationCatalogOutput
if ($localizationCatalogExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Localization catalog contract failed. See $reportPath"
}

Write-Host "== OpenVision Readiness Contract =="
$openVisionReadinessOutput = & dotnet run --project $openVisionReadinessCheckProject -c $Configuration -- $repoRoot 2>&1
$openVisionReadinessExit = $LASTEXITCODE
$openVisionReadinessOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "OpenVision Readiness Contract" $openVisionReadinessOutput
if ($openVisionReadinessExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "OpenVision readiness contract failed. See $reportPath"
}

Write-Host "== XML Compatibility =="
$xmlOutput = & dotnet run --project $xmlCheckProject -- $buildOutDir $docsSamples 2>&1
$xmlExit = $LASTEXITCODE
$xmlOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "XML Compatibility" $xmlOutput
if ($xmlExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "XML compatibility check failed. See $reportPath"
}

Write-Host "== Recipe Runner Smoke =="
$sampleOutputDir = Join-Path $OutputDir "samples"
$resolvedRunnerForCatalog = Resolve-RunnerExecutable -Candidates $runnerExeCandidates -ProjectName "VisionRecipeRunnerSmoke.exe" -ProjectPath $runnerProject
$skipRunnerBuildForCatalog = [bool]$SkipSampleRunnerBuild
if (-not $skipRunnerBuildForCatalog -and -not [string]::IsNullOrWhiteSpace($resolvedRunnerForCatalog)) {
    $skipRunnerBuildForCatalog = $true
}
$sampleArgs = @(
    "-ExecutionPolicy",
    "Bypass",
    "-File",
    $sampleCatalogScript,
    "-Configuration",
    $Configuration,
    "-Platform",
    $Platform,
    "-OutputDir",
    $sampleOutputDir
)
if ($skipRunnerBuildForCatalog) {
    $sampleArgs += "-SkipRunnerBuild"
}
if ($SkipRestore) {
    $sampleArgs += "-SkipRestore"
}
Invoke-Stage -Name "SampleCatalogSeconds" -Action {
    $script:sampleOutput = & powershell @sampleArgs 2>&1
    $script:sampleExit = $LASTEXITCODE
}
$sampleOutput = $script:sampleOutput
$sampleExit = [int]$script:sampleExit
$sampleOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "Sample Catalog Runner Smoke" $sampleOutput
if ($sampleExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Sample catalog runner smoke failed. See $reportPath"
}

$sampleReportPath = Join-Path $sampleOutputDir "sample_catalog_report.md"
$sampleSummaryJsonPath = Join-Path $sampleOutputDir "sample_catalog_summary.json"
if (-not (Test-Path -LiteralPath $sampleReportPath)) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Sample catalog report was not created: $sampleReportPath"
}

if (-not (Test-Path -LiteralPath $sampleSummaryJsonPath)) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Sample catalog summary JSON was not created: $sampleSummaryJsonPath"
}

$sampleSummary = $null
try {
    $sampleSummary = Get-Content -LiteralPath $sampleSummaryJsonPath -Raw | ConvertFrom-Json
}
catch {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Sample catalog summary JSON could not be parsed: $sampleSummaryJsonPath. $($_.Exception.Message)"
}

$sampleGateIssues = New-Object System.Collections.Generic.List[string]
$sampleRunnableRows = [int]$sampleSummary.RunnableRows
$sampleOkRows = [int]$sampleSummary.OKRows
$sampleNgRows = [int]$sampleSummary.NGRows
$sampleRequiredRows = [int]$sampleSummary.RequiredRows
$sampleExploreRows = [int]$sampleSummary.ExploreRows
$sampleGateStatus = ""
$sampleFailedCount = 0
$sampleArtifactIssueCount = 0
$sampleMetadataIssueCount = 0
$sampleDurationSeconds = 0.0
$sampleRunnerPath = ""
$sampleFolderCoverage = @()
$sampleUncoveredFolders = @()

if ($null -eq $sampleSummary.PSObject.Properties["GateStatus"]) {
    $sampleGateIssues.Add("GateStatus is missing from sample catalog summary JSON.") | Out-Null
}
else {
    $sampleGateStatus = [string]$sampleSummary.GateStatus
    if (-not [string]::Equals($sampleGateStatus, "OK", [StringComparison]::OrdinalIgnoreCase)) {
        $sampleGateIssues.Add("GateStatus must be OK. GateStatus=$sampleGateStatus.") | Out-Null
    }
}

if ($null -eq $sampleSummary.PSObject.Properties["FailedSamples"]) {
    $sampleGateIssues.Add("FailedSamples is missing from sample catalog summary JSON.") | Out-Null
}
else {
    $sampleFailedCount = @($sampleSummary.FailedSamples).Count
    if ($sampleFailedCount -ne 0) {
        $sampleGateIssues.Add("FailedSamples must be empty. FailedSamples=$sampleFailedCount.") | Out-Null
    }
}

if ($null -eq $sampleSummary.PSObject.Properties["ArtifactIssueCount"]) {
    $sampleGateIssues.Add("ArtifactIssueCount is missing from sample catalog summary JSON.") | Out-Null
}
else {
    $sampleArtifactIssueCount = [int]$sampleSummary.ArtifactIssueCount
    if ($sampleArtifactIssueCount -ne 0) {
        $sampleGateIssues.Add("ArtifactIssueCount must be 0. ArtifactIssueCount=$sampleArtifactIssueCount.") | Out-Null
    }
}

if ($null -eq $sampleSummary.PSObject.Properties["ArtifactIssues"]) {
    $sampleGateIssues.Add("ArtifactIssues is missing from sample catalog summary JSON.") | Out-Null
}
else {
    $sampleArtifactIssues = @($sampleSummary.ArtifactIssues).Count
    if ($sampleArtifactIssues -ne 0) {
        $sampleGateIssues.Add("ArtifactIssues must be empty. ArtifactIssues=$sampleArtifactIssues.") | Out-Null
    }
}

if ($null -eq $sampleSummary.PSObject.Properties["MetadataIssueCount"]) {
    $sampleGateIssues.Add("MetadataIssueCount is missing from sample catalog summary JSON.") | Out-Null
}
else {
    $sampleMetadataIssueCount = [int]$sampleSummary.MetadataIssueCount
    if ($sampleMetadataIssueCount -ne 0) {
        $sampleGateIssues.Add("MetadataIssueCount must be 0. MetadataIssueCount=$sampleMetadataIssueCount.") | Out-Null
    }
}

if ($null -eq $sampleSummary.PSObject.Properties["MetadataIssues"]) {
    $sampleGateIssues.Add("MetadataIssues is missing from sample catalog summary JSON.") | Out-Null
}
else {
    $sampleMetadataIssues = @($sampleSummary.MetadataIssues).Count
    if ($sampleMetadataIssues -ne 0) {
        $sampleGateIssues.Add("MetadataIssues must be empty. MetadataIssues=$sampleMetadataIssues.") | Out-Null
    }
}

if ($null -eq $sampleSummary.PSObject.Properties["DurationSeconds"]) {
    $sampleGateIssues.Add("DurationSeconds is missing from sample catalog summary JSON.") | Out-Null
}
else {
    $sampleDurationSeconds = [double]$sampleSummary.DurationSeconds
    if ($sampleDurationSeconds -le 0) {
        $sampleGateIssues.Add("DurationSeconds must be greater than 0. DurationSeconds=$sampleDurationSeconds.") | Out-Null
    }
}

if ($null -eq $sampleSummary.PSObject.Properties["RunnerPath"]) {
    $sampleGateIssues.Add("RunnerPath is missing from sample catalog summary JSON.") | Out-Null
}
else {
    $sampleRunnerPath = [string]$sampleSummary.RunnerPath
    if ([string]::IsNullOrWhiteSpace($sampleRunnerPath) -or -not (Test-Path -LiteralPath $sampleRunnerPath)) {
        $sampleGateIssues.Add("RunnerPath must point to an existing runner executable. RunnerPath=$sampleRunnerPath") | Out-Null
    }
}

if ($null -eq $sampleSummary.PSObject.Properties["SampleFolderCoverage"]) {
    $sampleGateIssues.Add("SampleFolderCoverage is missing from sample catalog summary JSON.") | Out-Null
}
else {
    $sampleFolderCoverage = @($sampleSummary.SampleFolderCoverage)
}

if ($null -eq $sampleSummary.PSObject.Properties["UncoveredSampleFolders"]) {
    $sampleGateIssues.Add("UncoveredSampleFolders is missing from sample catalog summary JSON.") | Out-Null
}
else {
    $sampleUncoveredFolders = @($sampleSummary.UncoveredSampleFolders)
}

if ($sampleRunnableRows -le 0) {
    $sampleGateIssues.Add("RunnableRows must be greater than 0.") | Out-Null
}

if ($sampleOkRows -ne $sampleRunnableRows) {
    $sampleGateIssues.Add("OKRows must match RunnableRows. OKRows=$sampleOkRows, RunnableRows=$sampleRunnableRows.") | Out-Null
}

if ($sampleNgRows -ne 0) {
    $sampleGateIssues.Add("NGRows must be 0. NGRows=$sampleNgRows.") | Out-Null
}

$categoryCount = 0
foreach ($category in @($sampleSummary.Categories)) {
    $categoryCount++
    $categoryName = [string]$category.Category
    $categoryTotal = [int]$category.Total
    $categoryOk = [int]$category.OK
    $categoryNg = [int]$category.NG

    if ($categoryNg -ne 0) {
        $sampleGateIssues.Add("Category '$categoryName' has NG=$categoryNg.") | Out-Null
    }

    if ($categoryOk -ne $categoryTotal) {
        $sampleGateIssues.Add("Category '$categoryName' OK must match Total. OK=$categoryOk, Total=$categoryTotal.") | Out-Null
    }
}

$sampleGateLines = New-Object System.Collections.Generic.List[string]
$sampleGateLines.Add("RunnableRows=$sampleRunnableRows") | Out-Null
$sampleGateLines.Add("RequiredRows=$sampleRequiredRows") | Out-Null
$sampleGateLines.Add("ExploreRows=$sampleExploreRows") | Out-Null
$sampleGateLines.Add("OKRows=$sampleOkRows") | Out-Null
$sampleGateLines.Add("NGRows=$sampleNgRows") | Out-Null
$sampleGateLines.Add("Categories=$categoryCount") | Out-Null
$sampleGateLines.Add("GateStatus=$sampleGateStatus") | Out-Null
$sampleGateLines.Add("FailedSamples=$sampleFailedCount") | Out-Null
$sampleGateLines.Add("ArtifactIssueCount=$sampleArtifactIssueCount") | Out-Null
$sampleGateLines.Add("MetadataIssueCount=$sampleMetadataIssueCount") | Out-Null
$sampleGateLines.Add("DurationSeconds=$sampleDurationSeconds") | Out-Null
$sampleGateLines.Add("RunnerPath=$sampleRunnerPath") | Out-Null
$sampleGateLines.Add("SampleFolders=$(@($sampleFolderCoverage).Count)") | Out-Null
$sampleGateLines.Add("UncoveredSampleFolders=$(@($sampleUncoveredFolders).Count)") | Out-Null
foreach ($folder in $sampleUncoveredFolders) {
    $sampleGateLines.Add("Uncovered=$($folder.Folder) | Images=$($folder.ImageCount) | CatalogRefs=$($folder.CatalogRefs)") | Out-Null
}

if ($sampleGateIssues.Count -eq 0) {
    $sampleGateLines.Add("Gate=OK") | Out-Null
}
else {
    $sampleGateLines.Add("Gate=NG") | Out-Null
    foreach ($issue in $sampleGateIssues) {
        $sampleGateLines.Add($issue) | Out-Null
    }
}

Add-ReportBlock "Sample Catalog Summary Gate" $sampleGateLines
if ($sampleGateIssues.Count -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Sample catalog summary gate failed. See $reportPath"
}

Write-Host "== WPF Shell Contract Build =="
$runnerApiOutputDir = Join-Path $OutputDir "wpf-shell-contract"
[void][System.IO.Directory]::CreateDirectory($runnerApiOutputDir)
$screenshotSmokeBuildProperties = @(
    "/p:Configuration=$Configuration",
    "/p:Platform=$Platform",
    "/p:WpgCustomBuildEnabled=false",
    "/p:UseAppHost=false"
)
if ($SkipWpfShellBuild) {
    $runnerBuildOutput = @("WPF Shell Contract Build skipped (existing artifact expected).")
    Add-ReportBlock "WPF Shell Contract Build" $runnerBuildOutput
}
else {
    Write-Host "== WPF Shell Contract Restore =="
    if ($SkipRestore) {
        $runnerRestoreOutput = @("WPF shell contract restore skipped by -SkipRestore.")
        $runnerRestoreExit = 0
    }
    else {
        Invoke-Stage -Name "WpfShellRestoreSeconds" -Action {
            $script:runnerRestoreOutput = & dotnet restore $screenshotSmokeProject @screenshotSmokeBuildProperties 2>&1
            $script:runnerRestoreExit = $LASTEXITCODE
        }
        $runnerRestoreExit = [int]$script:runnerRestoreExit
        $runnerRestoreOutput = $script:runnerRestoreOutput
        $runnerRestoreOutput | ForEach-Object { Write-Host $_ }
    }
    Add-ReportBlock "WPF Shell Contract Restore" $runnerRestoreOutput
    if ($runnerRestoreExit -ne 0) {
        $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
        throw "WPF shell contract smoke restore failed. See $reportPath"
    }

    Invoke-Stage -Name "WpfShellBuildSeconds" -Action {
        if (-not [string]::IsNullOrWhiteSpace($msBuild)) {
            $script:runnerBuildOutput = & $msBuild $screenshotSmokeProject /t:Build @screenshotSmokeBuildProperties @parallelBuildArguments /clp:ErrorsOnly /v:minimal 2>&1
            $script:runnerBuildExit = $LASTEXITCODE
        }
        else {
            $dotnetSmokeBuildArguments = @(
                "build",
                $screenshotSmokeProject,
                "-c",
                $Configuration,
                "--maxcpucount",
                "--no-restore",
                "/p:Platform=$Platform",
                "/p:WpgCustomBuildEnabled=false",
                "/p:UseAppHost=false"
            )
            $script:runnerBuildOutput = & dotnet @dotnetSmokeBuildArguments 2>&1
            $script:runnerBuildExit = $LASTEXITCODE
        }
    }
    $runnerBuildOutput = $script:runnerBuildOutput
    $runnerBuildExit = [int]$script:runnerBuildExit
    $runnerBuildOutput | ForEach-Object { Write-Host $_ }
    Add-ReportBlock "WPF Shell Contract Build" $runnerBuildOutput
    if ($runnerBuildExit -ne 0) {
        $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
        throw "WPF shell contract smoke build failed. See $reportPath"
    }
}

$screenshotSmokeDll = Resolve-ScreenshotSmokeDll -Candidates $screenshotSmokeDllCandidates -ProjectPath $screenshotSmokeProject
if ([string]::IsNullOrWhiteSpace($screenshotSmokeDll) -or -not (Test-Path -LiteralPath $screenshotSmokeDll)) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    $candidateList = if ($screenshotSmokeDllCandidates.Count -gt 0) { $screenshotSmokeDllCandidates -join "; " } else { "(none)" }
    throw "WPF shell contract smoke DLL was not found. Candidates: $candidateList"
}

Write-Host "== WPF Shell Contract =="
Invoke-Stage -Name "WpfShellContractSeconds" -Action {
    $script:runnerApiOutput = & dotnet exec $screenshotSmokeDll --quiet --target wpf_shell_preview,wpf_shell_host_workspace,wpf_shell_host_workspace_output,wpf_shell_host_native_tool,wpf_shell_host_pending_tool,wpf_roi_editor,wpf_image_compare,log_panel_contract_check,localization_catalog_contract_check $runnerApiOutputDir 2>&1
    $script:runnerApiExit = $LASTEXITCODE
}
$runnerApiOutput = $script:runnerApiOutput
$runnerApiExit = [int]$script:runnerApiExit
$runnerApiOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "WPF Shell Contract" $runnerApiOutput
if ($runnerApiExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "WPF shell contract smoke failed. See $reportPath"
}

Write-Host "== Tutorial Portable Contract =="
$tutorialPortableExit = 0
$tutorialPortableLines = New-Object System.Collections.Generic.List[string]
$tutorialPortableIssues = New-Object System.Collections.Generic.List[string]
$sourceImageCount = 0
$embeddedImageCount = 0

if (-not (Test-Path -LiteralPath $tutorialHtml)) {
    $tutorialPortableIssues.Add("Source tutorial HTML was not found: $tutorialHtml") | Out-Null
}

if (-not (Test-Path -LiteralPath $portableTutorialHtml)) {
    $tutorialPortableIssues.Add("Portable tutorial HTML was not found: $portableTutorialHtml") | Out-Null
}

if ($tutorialPortableIssues.Count -eq 0) {
    $sourceHtml = Get-Content -LiteralPath $tutorialHtml -Raw -Encoding UTF8
    $portableHtml = Get-Content -LiteralPath $portableTutorialHtml -Raw -Encoding UTF8
    $sourceImageCount = [regex]::Matches($sourceHtml, "<img\b", [System.Text.RegularExpressions.RegexOptions]::IgnoreCase).Count
    $embeddedImageCount = [regex]::Matches($portableHtml, "data:image/", [System.Text.RegularExpressions.RegexOptions]::IgnoreCase).Count
    $requiredTutorialTerms = @(
        "OpenVisionLab 처음 사용하기",
        "Blob",
        "Public_Blob_Particles_Good",
        "Parameter Guide",
        "Preview",
        "Recipe",
        "Threshold_Preview",
        "Blob_Preview",
        "검증 OK",
        "결과 OK/NG",
        "Run Review"
    )
    $requiredTutorialImages = @(
        "current/public_blob_particles_good_result.png"
    )

    if ($sourceImageCount -le 0) {
        $tutorialPortableIssues.Add("Source tutorial has no image tags.") | Out-Null
    }

    if ($embeddedImageCount -lt $sourceImageCount) {
        $tutorialPortableIssues.Add("Portable tutorial embedded image count is lower than source image count. Embedded=$embeddedImageCount, Source=$sourceImageCount.") | Out-Null
    }

    if ($portableHtml.IndexOf("assets/tutorial", [StringComparison]::OrdinalIgnoreCase) -ge 0) {
        $tutorialPortableIssues.Add("Portable tutorial still contains assets/tutorial references.") | Out-Null
    }

    foreach ($term in $requiredTutorialTerms) {
        if ($sourceHtml.IndexOf($term, [StringComparison]::OrdinalIgnoreCase) -lt 0) {
            $tutorialPortableIssues.Add("Tutorial is missing required workflow term: $term") | Out-Null
        }
    }

    foreach ($imageName in $requiredTutorialImages) {
        $imageReference = "../assets/tutorial/$imageName"
        $imagePath = Join-Path $repoRoot "docs\assets\tutorial\$imageName"
        if ($sourceHtml.IndexOf($imageReference, [StringComparison]::OrdinalIgnoreCase) -lt 0) {
            $tutorialPortableIssues.Add("Tutorial is missing required image reference: $imageReference") | Out-Null
        }

        if (-not (Test-Path -LiteralPath $imagePath)) {
            $tutorialPortableIssues.Add("Tutorial image file was not found: $imagePath") | Out-Null
        }
    }
}

$tutorialPortableLines.Add("Source=$tutorialHtml") | Out-Null
$tutorialPortableLines.Add("Portable=$portableTutorialHtml") | Out-Null
$tutorialPortableLines.Add("SourceImageCount=$sourceImageCount") | Out-Null
$tutorialPortableLines.Add("EmbeddedImageCount=$embeddedImageCount") | Out-Null
$tutorialPortableLines.Add("RequiredTerms=OpenVisionLab 처음 사용하기,Blob,Public_Blob_Particles_Good,Parameter Guide,Preview,Recipe,Threshold_Preview,Blob_Preview,검증 OK,결과 OK/NG,Run Review") | Out-Null
$tutorialPortableLines.Add("RequiredImages=current/public_blob_particles_good_result") | Out-Null
if ($tutorialPortableIssues.Count -eq 0) {
    $tutorialPortableLines.Add("Gate=OK") | Out-Null
}
else {
    $tutorialPortableExit = 1
    $tutorialPortableLines.Add("Gate=NG") | Out-Null
    foreach ($issue in $tutorialPortableIssues) {
        $tutorialPortableLines.Add($issue) | Out-Null
    }
}

$tutorialPortableLines | ForEach-Object { Write-Host $_ }
Add-ReportBlock "Tutorial Portable Contract" $tutorialPortableLines
if ($tutorialPortableExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Tutorial portable contract failed. See $reportPath"
}

$uiExit = $null
$uiOutputDir = Join-Path $OutputDir "ui"
$uiReportPath = Join-Path $uiOutputDir "ui_precheck_report.md"
$uiSummaryJsonPath = Join-Path $uiOutputDir "ui_precheck_summary.json"
$uiSummary = $null
if (-not $SkipUi) {
    Write-Host "== UI Precheck =="
    $uiArguments = @(
        "-ExecutionPolicy",
        "Bypass",
        "-File",
        $uiPrecheckScript,
        "-Configuration",
        $Configuration,
        "-Platform",
        $Platform,
        "-OutputDir",
        $uiOutputDir
    )

    if (-not [string]::IsNullOrWhiteSpace($UiTargets)) {
        $uiArguments += @("-Targets", $UiTargets)
    }

    if ($FailOnUiWarn) {
        $uiArguments += "-FailOnWarn"
    }

    if ($WpfTools) {
        $uiArguments += "-WpfTools"
    }

    if ($ToolOutputFlow) {
        $uiArguments += "-ToolOutputFlow"
    }

    if ($VisibleUiCapture) {
        $uiArguments += "-VisibleCapture"
    }

    $uiArguments += @("-WpgCustomBuildEnabled", $wpgCustomBuildEnabledArgument)
    $uiArguments += "-SkipSolutionBuild"
    if (-not $SkipWpfShellBuild) {
        $uiArguments += "-SkipSmokeBuild"
    }

    Invoke-Stage -Name "UiPrecheckSeconds" -Action {
        $script:uiOutput = & powershell @uiArguments 2>&1
        $script:uiExit = $LASTEXITCODE
    }
    $uiOutput = $script:uiOutput
    $uiExit = [int]$script:uiExit
    $uiOutput | ForEach-Object { Write-Host $_ }
    Add-ReportBlock "UI Precheck" $uiOutput
    if ($uiExit -ne 0) {
        $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
        throw "UI precheck failed. See $reportPath"
    }

    if (-not (Test-Path -LiteralPath $uiSummaryJsonPath)) {
        $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
        throw "UI precheck summary JSON was not created: $uiSummaryJsonPath"
    }

    try {
        $uiSummary = Get-Content -LiteralPath $uiSummaryJsonPath -Raw | ConvertFrom-Json
    }
    catch {
        $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
        throw "UI precheck summary JSON could not be parsed: $uiSummaryJsonPath. $($_.Exception.Message)"
    }

    if ([string]$uiSummary.Status -ne "OK") {
        $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
        throw "UI precheck summary status was not OK: $($uiSummary.Status). See $uiSummaryJsonPath"
    }

    if ($WpfTools -and -not [bool]$uiSummary.WpfTools) {
        $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
        throw "UI precheck summary did not record WPF tool coverage. See $uiSummaryJsonPath"
    }

    if ($ToolOutputFlow -and -not [bool]$uiSummary.ToolOutputFlow) {
        $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
        throw "UI precheck summary did not record tool output flow coverage. See $uiSummaryJsonPath"
    }

    $uiSummaryLines = @(
        "Status: $($uiSummary.Status)",
        "WPF tools: $($uiSummary.WpfTools)",
        "Tool output flow: $($uiSummary.ToolOutputFlow)",
        "Targets: $($uiSummary.TargetCount)",
        "OK/WARN/NG: $($uiSummary.Counts.OK)/$($uiSummary.Counts.WARN)/$($uiSummary.Counts.NG)",
        "Summary: $uiSummaryJsonPath"
    )
    Add-ReportBlock "UI Precheck Summary" $uiSummaryLines
}

$report.Add("## Artifacts") | Out-Null
$report.Add("") | Out-Null
$report.Add("- Sample catalog report: ``$sampleReportPath``") | Out-Null
$report.Add("- Sample catalog summary JSON: ``$sampleSummaryJsonPath``") | Out-Null
$report.Add("- WPF shell preview smoke: ``$(Join-Path $runnerApiOutputDir "wpf_shell_preview.png")``") | Out-Null
$report.Add("- WPF workspace smoke: ``$(Join-Path $runnerApiOutputDir "wpf_shell_host_workspace.png")``") | Out-Null
$report.Add("- WPF workspace output smoke: ``$(Join-Path $runnerApiOutputDir "wpf_shell_host_workspace_output.png")``") | Out-Null
$report.Add("- WPF native tool smoke: ``$(Join-Path $runnerApiOutputDir "wpf_shell_host_native_tool.png")``") | Out-Null
$report.Add("- WPF pending tool smoke: ``$(Join-Path $runnerApiOutputDir "wpf_shell_host_pending_tool.png")``") | Out-Null
$report.Add("- WPF ROI editor smoke: ``$(Join-Path $runnerApiOutputDir "wpf_roi_editor.png")``") | Out-Null
$report.Add("- WPF Image Compare smoke: ``$(Join-Path $runnerApiOutputDir "wpf_image_compare.png")``") | Out-Null
$report.Add("- Log panel contract smoke: ``$(Join-Path $runnerApiOutputDir "log_panel_contract_check.png")``") | Out-Null
$report.Add("- Localization catalog contract smoke: ``$(Join-Path $runnerApiOutputDir "localization_catalog_contract_check.png")``") | Out-Null
$report.Add("- Portable tutorial: ``$portableTutorialHtml``") | Out-Null
if (-not $SkipUi) {
    $report.Add("- UI report: ``$(Join-Path $OutputDir "ui\ui_precheck_report.md")``") | Out-Null
    $report.Add("- UI summary JSON: ``$(Join-Path $OutputDir "ui\ui_precheck_summary.json")``") | Out-Null
}
$report.Add("- Platform summary JSON: ``$summaryPath``") | Out-Null

$summaryGates = @(
    [ordered]@{
        Name = "Build"
        Status = "OK"
        ExitCode = $buildExit
    },
    [ordered]@{
        Name = "Vision UI Contract"
        Status = "OK"
        ExitCode = $visionUiExit
    },
    [ordered]@{
        Name = "History Contract"
        Status = "OK"
        ExitCode = $historyContractExit
    },
    [ordered]@{
        Name = "Localization Catalog Contract"
        Status = "OK"
        ExitCode = $localizationCatalogExit
    },
    [ordered]@{
        Name = "OpenVision Readiness Contract"
        Status = "OK"
        ExitCode = $openVisionReadinessExit
    },
    [ordered]@{
        Name = "XML Compatibility"
        Status = "OK"
        ExitCode = $xmlExit
    },
    [ordered]@{
        Name = "Sample Catalog Runner"
        Status = "OK"
        ExitCode = $sampleExit
    },
    [ordered]@{
        Name = "Sample Catalog Summary"
        Status = "OK"
        ExitCode = 0
    },
    [ordered]@{
        Name = "Runner API Contract"
        Status = "OK"
        ExitCode = $runnerApiExit
    },
    [ordered]@{
        Name = "AI Recipe Interactive Contract"
        Status = "OK"
        ExitCode = $aiRecipeContractExit
    },
    [ordered]@{
        Name = "Tool Result Contract"
        Status = "OK"
        ExitCode = $toolContractExit
    },
    [ordered]@{
        Name = "Sample Inventory And Algorithm Contract"
        Status = "OK"
        ExitCode = $sampleContractExit
    },
    [ordered]@{
        Name = "Tutorial Portable Contract"
        Status = "OK"
        ExitCode = $tutorialPortableExit
    }
)

if (-not $SkipUi) {
    $summaryGates += [ordered]@{
        Name = "UI Precheck"
        Status = "OK"
        ExitCode = $uiExit
    }
}

$uiReportPath = if ($SkipUi) { "" } else { $uiReportPath }
$uiSummaryJsonPath = if ($SkipUi) { "" } else { $uiSummaryJsonPath }
$uiSummaryPayload = if ($SkipUi -or $null -eq $uiSummary) {
    $null
}
else {
    [ordered]@{
        Status = [string]$uiSummary.Status
        WpfTools = [bool]$uiSummary.WpfTools
        ToolOutputFlow = [bool]$uiSummary.ToolOutputFlow
        TargetCount = [int]$uiSummary.TargetCount
        OK = [int]$uiSummary.Counts.OK
        WARN = [int]$uiSummary.Counts.WARN
        NG = [int]$uiSummary.Counts.NG
        ReportPath = $uiReportPath
        SummaryJsonPath = $uiSummaryJsonPath
    }
}
$toolOpenPerfGateSeconds = 0.0
if (-not $SkipUi -and $null -ne $uiSummary -and $uiSummary.PSObject.Properties["Timings"] -and $uiSummary.Timings.PSObject.Properties["ToolOpenPerfGateSeconds"]) {
    $toolOpenPerfGateSeconds = [double]$uiSummary.Timings.ToolOpenPerfGateSeconds
}
$summaryPayload = [ordered]@{
    Time = (Get-Date -Format "yyyy-MM-dd HH:mm:ss")
    StartedAt = $precheckStartedAt.ToString("yyyy-MM-dd HH:mm:ss")
    DurationSeconds = [Math]::Round(((Get-Date) - $precheckStartedAt).TotalSeconds, 3)
    Status = "OK"
    Configuration = $Configuration
    Platform = $Platform
    SkipUi = [bool]$SkipUi
    SkipSampleRunnerBuild = [bool]$SkipSampleRunnerBuild
    SkipWpfShellBuild = [bool]$SkipWpfShellBuild
    WpfTools = [bool]$WpfTools
    ToolOutputFlow = [bool]$ToolOutputFlow
    Timings = [ordered]@{
        RestoreSeconds = $platformPrecheckDurationsSeconds.RestoreSeconds
        BuildSeconds = $platformPrecheckDurationsSeconds.BuildSeconds
        WpfShellRestoreSeconds = $platformPrecheckDurationsSeconds.WpfShellRestoreSeconds
        WpfShellBuildSeconds = $platformPrecheckDurationsSeconds.WpfShellBuildSeconds
        WpfShellContractSeconds = $platformPrecheckDurationsSeconds.WpfShellContractSeconds
        ToolOpenPerfGateSeconds = $toolOpenPerfGateSeconds
        SampleCatalogSeconds = $platformPrecheckDurationsSeconds.SampleCatalogSeconds
        UiPrecheckSeconds = $platformPrecheckDurationsSeconds.UiPrecheckSeconds
    }
    OutputDir = $OutputDir
    ReportPath = $reportPath
    SummaryPath = $summaryPath
    Gates = @($summaryGates)
    SampleCatalog = [ordered]@{
        GateStatus = $sampleGateStatus
        RunnableRows = $sampleRunnableRows
        RequiredRows = $sampleRequiredRows
        ExploreRows = $sampleExploreRows
        OKRows = $sampleOkRows
        NGRows = $sampleNgRows
        Categories = $categoryCount
        FailedSamples = $sampleFailedCount
        ArtifactIssueCount = $sampleArtifactIssueCount
        MetadataIssueCount = $sampleMetadataIssueCount
        DurationSeconds = $sampleDurationSeconds
        RunnerPath = $sampleRunnerPath
        SampleFolderCount = @($sampleFolderCoverage).Count
        UncoveredSampleFolderCount = @($sampleUncoveredFolders).Count
        UncoveredSampleFolders = @($sampleUncoveredFolders)
        ReportPath = $sampleReportPath
        SummaryJsonPath = $sampleSummaryJsonPath
    }
    UiPrecheck = $uiSummaryPayload
    Artifacts = [ordered]@{
        SampleCatalogReport = $sampleReportPath
        SampleCatalogSummaryJson = $sampleSummaryJsonPath
        WpfShellPreviewSmoke = Join-Path $runnerApiOutputDir "wpf_shell_preview.png"
        WpfWorkspaceSmoke = Join-Path $runnerApiOutputDir "wpf_shell_host_workspace.png"
        WpfWorkspaceOutputSmoke = Join-Path $runnerApiOutputDir "wpf_shell_host_workspace_output.png"
        WpfNativeToolSmoke = Join-Path $runnerApiOutputDir "wpf_shell_host_native_tool.png"
        WpfPendingToolSmoke = Join-Path $runnerApiOutputDir "wpf_shell_host_pending_tool.png"
        WpfRoiEditorSmoke = Join-Path $runnerApiOutputDir "wpf_roi_editor.png"
        WpfImageCompareSmoke = Join-Path $runnerApiOutputDir "wpf_image_compare.png"
        LogPanelContractSmoke = Join-Path $runnerApiOutputDir "log_panel_contract_check.png"
        LocalizationCatalogContractSmoke = Join-Path $runnerApiOutputDir "localization_catalog_contract_check.png"
        PortableTutorial = $portableTutorialHtml
        UiReport = $uiReportPath
        UiSummaryJson = $uiSummaryJsonPath
    }
}
$summaryPayload | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $summaryPath -Encoding UTF8

$report | Set-Content -LiteralPath $reportPath -Encoding UTF8
Write-Host "Platform precheck report saved to $reportPath"
Write-Host "Platform precheck summary saved to $summaryPath"
