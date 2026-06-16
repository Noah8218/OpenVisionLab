param(
    [string]$Configuration = "Debug",
    [string]$Platform = "Any CPU",
    [string]$OutputDir = "C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck",
    [string]$UiTargets = "",
    [switch]$SkipUi,
    [switch]$FailOnUiWarn,
    [switch]$VisibleUiCapture
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $repoRoot "OpenVisionLab.sln"
$xmlCheckProject = Join-Path $repoRoot "tools\RecipeXmlCompatibilityCheck\RecipeXmlCompatibilityCheck.csproj"
$screenshotSmokeProject = Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj"
$screenshotSmokeExe = Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\$Platform\$Configuration\net8.0-windows\PipelineViewerScreenshotSmoke.exe"
$sampleCatalogScript = Join-Path $repoRoot "tools\RunVisionSampleCatalog.ps1"
$uiPrecheckScript = Join-Path $repoRoot "tools\RunUiPrecheck.ps1"
$docsSamples = Join-Path $repoRoot "docs\samples"
$tutorialHtml = Join-Path $repoRoot "docs\OPENVISIONLAB_TUTORIAL.html"
$portableTutorialHtml = Join-Path $repoRoot "docs\OPENVISIONLAB_TUTORIAL_PORTABLE.html"
$buildOutDir = "C:\Users\Public\Documents\ESTsoft\CreatorTemp\OpenVisionLabBuild\"
$msBuild = "C:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
$reportPath = Join-Path $OutputDir "platform_precheck_report.md"
$summaryPath = Join-Path $OutputDir "platform_precheck_summary.json"
$report = New-Object System.Collections.Generic.List[string]
$precheckStartedAt = Get-Date
$report.Add("# OpenVisionLab Platform Precheck") | Out-Null
$report.Add("") | Out-Null
$report.Add("- Time: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")") | Out-Null
$report.Add("- Build: $Configuration / $Platform") | Out-Null
$report.Add("- Output: ``$OutputDir``") | Out-Null
$report.Add("") | Out-Null

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

Write-Host "== Build OpenVisionLab =="
$buildOutput = & $msBuild $solution /t:Build /p:Configuration=$Configuration "/p:Platform=$Platform" /p:RestorePackages=false "/p:OutDir=$buildOutDir" /v:minimal 2>&1
$buildExit = $LASTEXITCODE
$buildOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "Build" $buildOutput
if ($buildExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Build failed. See $reportPath"
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
$sampleOutput = & powershell -ExecutionPolicy Bypass -File $sampleCatalogScript -Configuration $Configuration -Platform $Platform -OutputDir $sampleOutputDir 2>&1
$sampleExit = $LASTEXITCODE
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

Write-Host "== Runner API Contract =="
$runnerApiOutputDir = Join-Path $OutputDir "runner-api"
New-Item -ItemType Directory -Force -Path $runnerApiOutputDir | Out-Null
$runnerBuildOutput = & $msBuild $screenshotSmokeProject /t:Build /p:Configuration=$Configuration "/p:Platform=$Platform" /p:WpgCustomBuildEnabled=false /clp:ErrorsOnly /v:minimal 2>&1
$runnerBuildExit = $LASTEXITCODE
$runnerBuildOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "Runner API Contract Build" $runnerBuildOutput
if ($runnerBuildExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Runner API contract smoke build failed. See $reportPath"
}

if (-not (Test-Path -LiteralPath $screenshotSmokeExe)) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Runner API contract smoke executable was not found: $screenshotSmokeExe"
}

$runnerApiOutput = & $screenshotSmokeExe --target vision_recipe_runner_api_contract_check $runnerApiOutputDir 2>&1
$runnerApiExit = $LASTEXITCODE
$runnerApiOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "Runner API Contract" $runnerApiOutput
if ($runnerApiExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Runner API contract smoke failed. See $reportPath"
}

Write-Host "== AI Recipe Prompt Contract =="
$aiRecipeContractOutputDir = Join-Path $OutputDir "ai-recipe"
New-Item -ItemType Directory -Force -Path $aiRecipeContractOutputDir | Out-Null
$aiRecipeContractOutput = & $screenshotSmokeExe --target ai_recipe_prompt_contract_check $aiRecipeContractOutputDir 2>&1
$aiRecipeContractExit = $LASTEXITCODE
$aiRecipeContractOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "AI Recipe Prompt Contract" $aiRecipeContractOutput
if ($aiRecipeContractExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "AI Recipe prompt contract smoke failed. See $reportPath"
}

Write-Host "== Tool Result Contract =="
$toolContractOutputDir = Join-Path $OutputDir "tool-contract"
New-Item -ItemType Directory -Force -Path $toolContractOutputDir | Out-Null
$toolContractTargets = "tool_result_status_contract_check,pipeline_tool_result_contract_check"
$toolContractOutput = & $screenshotSmokeExe --target $toolContractTargets $toolContractOutputDir 2>&1
$toolContractExit = $LASTEXITCODE
$toolContractOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "Tool Result Contract" $toolContractOutput
if ($toolContractExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Tool result contract smoke failed. See $reportPath"
}

Write-Host "== Sample Inventory and Algorithm Contract =="
$sampleContractOutputDir = Join-Path $OutputDir "sample-contract"
New-Item -ItemType Directory -Force -Path $sampleContractOutputDir | Out-Null
$sampleContractTargets = "sample_inventory_contract_check,algorithm_sample_contract_check"
$sampleContractOutput = & $screenshotSmokeExe --target $sampleContractTargets $sampleContractOutputDir 2>&1
$sampleContractExit = $LASTEXITCODE
$sampleContractOutput | ForEach-Object { Write-Host $_ }
Add-ReportBlock "Sample Inventory and Algorithm Contract" $sampleContractOutput
if ($sampleContractExit -ne 0) {
    $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
    throw "Sample inventory/algorithm contract smoke failed. See $reportPath"
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

    if ($sourceImageCount -le 0) {
        $tutorialPortableIssues.Add("Source tutorial has no image tags.") | Out-Null
    }

    if ($embeddedImageCount -lt $sourceImageCount) {
        $tutorialPortableIssues.Add("Portable tutorial embedded image count is lower than source image count. Embedded=$embeddedImageCount, Source=$sourceImageCount.") | Out-Null
    }

    if ($portableHtml.IndexOf("assets/tutorial", [StringComparison]::OrdinalIgnoreCase) -ge 0) {
        $tutorialPortableIssues.Add("Portable tutorial still contains assets/tutorial references.") | Out-Null
    }
}

$tutorialPortableLines.Add("Source=$tutorialHtml") | Out-Null
$tutorialPortableLines.Add("Portable=$portableTutorialHtml") | Out-Null
$tutorialPortableLines.Add("SourceImageCount=$sourceImageCount") | Out-Null
$tutorialPortableLines.Add("EmbeddedImageCount=$embeddedImageCount") | Out-Null
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

    if ($VisibleUiCapture) {
        $uiArguments += "-VisibleCapture"
    }

    $uiOutput = & powershell @uiArguments 2>&1

    $uiExit = $LASTEXITCODE
    $uiOutput | ForEach-Object { Write-Host $_ }
    Add-ReportBlock "UI Precheck" $uiOutput
    if ($uiExit -ne 0) {
        $report | Set-Content -LiteralPath $reportPath -Encoding UTF8
        throw "UI precheck failed. See $reportPath"
    }
}

$report.Add("## Artifacts") | Out-Null
$report.Add("") | Out-Null
$report.Add("- Sample catalog report: ``$sampleReportPath``") | Out-Null
$report.Add("- Sample catalog summary JSON: ``$sampleSummaryJsonPath``") | Out-Null
$report.Add("- Runner API smoke: ``$(Join-Path $runnerApiOutputDir "vision_recipe_runner_api_contract_check.png")``") | Out-Null
$report.Add("- AI Recipe prompt contract smoke: ``$(Join-Path $aiRecipeContractOutputDir "ai_recipe_prompt_contract_check.png")``") | Out-Null
$report.Add("- Tool result contract smoke: ``$(Join-Path $toolContractOutputDir "tool_result_status_contract_check.png")``") | Out-Null
$report.Add("- Pipeline tool result contract smoke: ``$(Join-Path $toolContractOutputDir "pipeline_tool_result_contract_check.png")``") | Out-Null
$report.Add("- Sample inventory smoke: ``$(Join-Path $sampleContractOutputDir "sample_inventory_contract_check.png")``") | Out-Null
$report.Add("- Algorithm sample contract smoke: ``$(Join-Path $sampleContractOutputDir "algorithm_sample_contract_check.png")``") | Out-Null
$report.Add("- Portable tutorial: ``$portableTutorialHtml``") | Out-Null
if (-not $SkipUi) {
    $report.Add("- UI report: ``$(Join-Path $OutputDir "ui\ui_precheck_report.md")``") | Out-Null
}
$report.Add("- Platform summary JSON: ``$summaryPath``") | Out-Null

$summaryGates = @(
    [ordered]@{
        Name = "Build"
        Status = "OK"
        ExitCode = $buildExit
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
        Name = "AI Recipe Prompt Contract"
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

$uiReportPath = if (-not $SkipUi) { Join-Path $OutputDir "ui\ui_precheck_report.md" } else { "" }
$summaryPayload = [ordered]@{
    Time = (Get-Date -Format "yyyy-MM-dd HH:mm:ss")
    StartedAt = $precheckStartedAt.ToString("yyyy-MM-dd HH:mm:ss")
    DurationSeconds = [Math]::Round(((Get-Date) - $precheckStartedAt).TotalSeconds, 3)
    Status = "OK"
    Configuration = $Configuration
    Platform = $Platform
    SkipUi = [bool]$SkipUi
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
    Artifacts = [ordered]@{
        SampleCatalogReport = $sampleReportPath
        SampleCatalogSummaryJson = $sampleSummaryJsonPath
        RunnerApiSmoke = Join-Path $runnerApiOutputDir "vision_recipe_runner_api_contract_check.png"
        AiRecipePromptContractSmoke = Join-Path $aiRecipeContractOutputDir "ai_recipe_prompt_contract_check.png"
        ToolResultContractSmoke = Join-Path $toolContractOutputDir "tool_result_status_contract_check.png"
        PipelineToolResultContractSmoke = Join-Path $toolContractOutputDir "pipeline_tool_result_contract_check.png"
        SampleInventorySmoke = Join-Path $sampleContractOutputDir "sample_inventory_contract_check.png"
        AlgorithmSampleContractSmoke = Join-Path $sampleContractOutputDir "algorithm_sample_contract_check.png"
        PortableTutorial = $portableTutorialHtml
        UiReport = $uiReportPath
    }
}
$summaryPayload | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $summaryPath -Encoding UTF8

$report | Set-Content -LiteralPath $reportPath -Encoding UTF8
Write-Host "Platform precheck report saved to $reportPath"
Write-Host "Platform precheck summary saved to $summaryPath"
