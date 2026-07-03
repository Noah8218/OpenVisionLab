param(
    [string]$OutputDir = "artifacts\sample_review_ui_smoke",
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"

$project = Join-Path $PSScriptRoot "PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj"
$targets = @(
    "wpf_shell_host_workspace_sample_product_focus_open",
    "wpf_shell_host_workspace_product_sample_review",
    "wpf_shell_host_workspace_product_sample_review_ng",
    "wpf_shell_host_workspace_product_sample_pair_open",
    "wpf_shell_host_workspace_sample_pair_coverage",
    "wpf_shell_host_workspace_sample_bad_reference_audit"
)

dotnet build $project -c $Configuration | Write-Host

foreach ($target in $targets) {
    Write-Host "Running $target"
    dotnet run --project $project -c $Configuration --no-build -- --target $target $OutputDir
    if ($LASTEXITCODE -ne 0) {
        throw "Sample review UI smoke failed: $target"
    }
}

Write-Host "SampleReviewUiSmoke=PASS | Targets=$($targets.Count) OutputDir=$OutputDir"
