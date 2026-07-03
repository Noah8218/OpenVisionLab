param(
    [string]$Configuration = "Debug",
    [switch]$SkipBuild,
    [switch]$SkipMouseDrag,
    [string]$OutputRoot = "artifacts\docking_verification"
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$outputRootPath = if ([System.IO.Path]::IsPathRooted($OutputRoot)) {
    $OutputRoot
} else {
    Join-Path $repoRoot $OutputRoot
}
$outputDir = Join-Path $outputRootPath ("actual_exe_" + $timestamp)

if (-not $SkipBuild) {
    dotnet build (Join-Path $repoRoot "OpenVisionLab.sln") -c $Configuration -p:Platform="Any CPU"
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed with exit code $LASTEXITCODE."
    }
}

$exePath = Join-Path $repoRoot ("bin\" + $Configuration + "\OpenVisionLab.exe")
if (-not (Test-Path -LiteralPath $exePath)) {
    throw "OpenVisionLab.exe was not found: $exePath"
}

New-Item -ItemType Directory -Force -Path $outputDir | Out-Null

function Invoke-DockingSmoke {
    param(
        [string]$Scenario,
        [string]$ScenarioOutputDir,
        [string]$Label
    )

    New-Item -ItemType Directory -Force -Path $ScenarioOutputDir | Out-Null
    $process = Start-Process -FilePath $exePath `
        -ArgumentList @("--smoke", $Scenario, "--output", $ScenarioOutputDir) `
        -Wait `
        -PassThru
    $exitCode = $process.ExitCode

    $reportPath = Join-Path $ScenarioOutputDir "report.txt"
    for ($attempt = 0; $attempt -lt 20 -and -not (Test-Path -LiteralPath $reportPath); $attempt++) {
        Start-Sleep -Milliseconds 250
    }

    if (-not (Test-Path -LiteralPath $reportPath)) {
        throw "$Label did not produce report.txt. Output: $ScenarioOutputDir"
    }

    $report = Get-Content -LiteralPath $reportPath -Raw
    if ($exitCode -ne 0 -or $report -notmatch "Result:\s+PASS") {
        Write-Host $report
        throw "$Label failed. ExitCode=$exitCode Output=$ScenarioOutputDir"
    }

    Write-Host "$Label PASS"
    Write-Host "Output: $ScenarioOutputDir"
    Write-Host $report
}

Invoke-DockingSmoke `
    -Scenario "workspace-startup-empty" `
    -ScenarioOutputDir (Join-Path $outputDir "startup_empty_workspace") `
    -Label "Startup empty workspace verification"

Invoke-DockingSmoke `
    -Scenario "layer-docking-verification" `
    -ScenarioOutputDir $outputDir `
    -Label "Docking verification"

Invoke-DockingSmoke `
    -Scenario "layer-docking-tab-click-no-guide" `
    -ScenarioOutputDir (Join-Path $outputDir "tab_click_no_guide") `
    -Label "Docking tab click no-guide verification"

if (-not $SkipMouseDrag) {
    Invoke-DockingSmoke `
        -Scenario "layer-initial-docked-workspace" `
        -ScenarioOutputDir (Join-Path $outputDir "initial_docked_workspace") `
        -Label "Initial docked workspace verification"

    Invoke-DockingSmoke `
        -Scenario "layer-docking-mouse-drag" `
        -ScenarioOutputDir (Join-Path $outputDir "mouse_drag") `
        -Label "Docking mouse drag verification"
}
