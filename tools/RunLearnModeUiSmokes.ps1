param(
    [string]$OutputDir = "",
    [string]$Configuration = "Debug",
    [string[]]$Targets = @(
        "wpf_openvision_learn_curriculum",
        "wpf_openvision_learn_brightness",
        "wpf_openvision_learn_threshold",
        "wpf_openvision_learn_threshold_animation",
        "wpf_openvision_learn_threshold_apply",
        "wpf_openvision_learn_filtering",
        "wpf_openvision_learn_morphology",
        "wpf_openvision_learn_blob",
        "wpf_openvision_learn_contour",
        "wpf_openvision_learn_edge_line",
        "wpf_openvision_learn_line_distance",
        "wpf_openvision_learn_matching",
        "wpf_openvision_learn_feature_matching",
        "wpf_openvision_learn_layer_recipe",
        "wpf_openvision_learn_edge_based_matching",
        "wpf_openvision_learn_metrics_acceptance",
        "wpf_openvision_learn_arithmetic",
        "wpf_openvision_learn_geometry",
        "wpf_openvision_learn_color_hsv"
    )
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj"

if ([string]::IsNullOrWhiteSpace($OutputDir)) {
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $OutputDir = Join-Path $repoRoot "artifacts\learn_mode_ui_smokes_$timestamp"
}
elseif (-not [System.IO.Path]::IsPathRooted($OutputDir)) {
    $OutputDir = Join-Path $repoRoot $OutputDir
}

[void][System.IO.Directory]::CreateDirectory($OutputDir)
$reportPath = Join-Path $OutputDir "learn_mode_ui_smokes_report.txt"

$normalizedTargets = @()
$seenTargets = @{}
foreach ($targetGroup in $Targets) {
    foreach ($target in ($targetGroup -split ",")) {
        $trimmed = $target.Trim()
        if (-not [string]::IsNullOrWhiteSpace($trimmed) -and -not $seenTargets.ContainsKey($trimmed)) {
            $seenTargets[$trimmed] = $true
            $normalizedTargets += $trimmed
        }
    }
}

if ($normalizedTargets.Count -eq 0) {
    throw "No Learn Mode UI smoke targets were selected."
}

Set-Content -LiteralPath $reportPath -Value "LearnModeUiSmoke=START | Targets=$($normalizedTargets.Count) | OutputDir=$OutputDir"

Write-Host "== Build Learn Mode UI Screenshot Smoke =="
dotnet build $project -c $Configuration
if ($LASTEXITCODE -ne 0) {
    throw "Learn Mode UI smoke build failed."
}

foreach ($target in $normalizedTargets) {
    $targetOutputDir = Join-Path $OutputDir $target
    [void][System.IO.Directory]::CreateDirectory($targetOutputDir)

    Write-Host "Running $target"
    dotnet run --project $project -c $Configuration --no-build -- --target $target $targetOutputDir
    if ($LASTEXITCODE -ne 0) {
        Add-Content -LiteralPath $reportPath -Value "FAIL $target"
        throw "Learn Mode UI smoke failed: $target"
    }

    Add-Content -LiteralPath $reportPath -Value "PASS $target | OutputDir=$targetOutputDir"
}

Add-Content -LiteralPath $reportPath -Value "LearnModeUiSmoke=PASS | Targets=$($normalizedTargets.Count) | OutputDir=$OutputDir"
Write-Host "LearnModeUiSmoke=PASS | Targets=$($normalizedTargets.Count) OutputDir=$OutputDir"
