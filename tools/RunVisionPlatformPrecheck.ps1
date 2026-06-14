param(
    [string]$Configuration = "Debug",
    [string]$Platform = "Any CPU",
    [string]$OutputDir = "C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck",
    [string]$UiTargets = "",
    [switch]$SkipUi,
    [switch]$FailOnUiWarn
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $repoRoot "OpenVisionLab.sln"
$xmlCheckProject = Join-Path $repoRoot "tools\RecipeXmlCompatibilityCheck\RecipeXmlCompatibilityCheck.csproj"
$sampleCatalogScript = Join-Path $repoRoot "tools\RunVisionSampleCatalog.ps1"
$uiPrecheckScript = Join-Path $repoRoot "tools\RunUiPrecheck.ps1"
$docsSamples = Join-Path $repoRoot "docs\samples"
$buildOutDir = "C:\Users\Public\Documents\ESTsoft\CreatorTemp\OpenVisionLabBuild\"
$msBuild = "C:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
$reportPath = Join-Path $OutputDir "platform_precheck_report.md"
$report = New-Object System.Collections.Generic.List[string]
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

if (-not $SkipUi) {
    Write-Host "== UI Precheck =="
    $uiOutputDir = Join-Path $OutputDir "ui"
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
$report.Add("- Sample catalog report: ``$(Join-Path $sampleOutputDir "sample_catalog_report.md")``") | Out-Null
if (-not $SkipUi) {
    $report.Add("- UI report: ``$(Join-Path $OutputDir "ui\ui_precheck_report.md")``") | Out-Null
}

$report | Set-Content -LiteralPath $reportPath -Encoding UTF8
Write-Host "Platform precheck report saved to $reportPath"
