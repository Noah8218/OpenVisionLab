param(
    [string]$Configuration = "Debug",
    [string]$Platform = "Any CPU",
    [string]$OutputDir = "C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_smoke",
    [string]$Targets = "wpf_shell_preview,wpf_shell_host_workspace_empty,wpf_shell_host_workspace,wpf_shell_host_workspace_image_load,wpf_shell_host_tool_input_empty,wpf_shell_host_tool_input_image_load_save,wpf_shell_host_workspace_output,wpf_shell_host_bridge,wpf_shell_host_native_tool,wpf_shell_host_threshold_basic_tool,wpf_shell_host_pipeline_review,wpf_shell_host_blob_tool,wpf_shell_host_contour_tool,wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool,wpf_shell_host_matching_tool,wpf_shell_host_feature_matching_tool,wpf_shell_host_pending_tool,wpf_roi_editor,wpf_image_compare,log_panel_contract_check,localization_catalog_contract_check",
    [string]$Suite = "",
    [int]$TimeoutSeconds = 120,
    [switch]$All,
    [switch]$VisibleCapture
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj"
$msBuildCandidates = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
)
$msBuild = $msBuildCandidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
$dll = Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\$Platform\$Configuration\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll"
$stdoutPath = Join-Path $OutputDir "ui_smoke_stdout.txt"
$stderrPath = Join-Path $OutputDir "ui_smoke_stderr.txt"

[void][System.IO.Directory]::CreateDirectory($OutputDir)

if ($All -and -not [string]::IsNullOrWhiteSpace($Suite)) {
    throw "Use either -All or -Suite, not both."
}

Write-Host "== Restore UI Screenshot Smoke =="
& dotnet restore $project "/p:Configuration=$Configuration" "/p:Platform=$Platform" /p:WpgCustomBuildEnabled=false /p:UseAppHost=false
if ($LASTEXITCODE -ne 0) {
    throw "UI screenshot smoke restore failed."
}

Write-Host "== Build UI Screenshot Smoke =="
if ([string]::IsNullOrWhiteSpace($msBuild)) {
    throw "MSBuild was not found. Install Visual Studio 2022 or Build Tools, or run tools\RunUiPrecheck.ps1 which can use the repository build path."
}

& $msBuild $project /t:Build /p:Configuration=$Configuration "/p:Platform=$Platform" /p:WpgCustomBuildEnabled=false /p:UseAppHost=false /clp:ErrorsOnly /v:minimal
if ($LASTEXITCODE -ne 0) {
    throw "UI screenshot smoke build failed."
}

if (-not (Test-Path -LiteralPath $dll)) {
    throw "Smoke DLL was not found: $dll"
}

$arguments = if ($All) {
    @("--all", $OutputDir)
}
elseif (-not [string]::IsNullOrWhiteSpace($Suite)) {
    @("--suite", $Suite, $OutputDir)
}
else {
    @("--target", $Targets, $OutputDir)
}

$selectionLabel = if ($All) {
    "ALL"
}
elseif (-not [string]::IsNullOrWhiteSpace($Suite)) {
    "SUITE $Suite"
}
else {
    "TARGETS $Targets"
}

if ($VisibleCapture) {
    $arguments += "--visible-capture"
}
else {
    $arguments += "--quiet"
}

Write-Host "== UI Screenshot Smoke =="
Write-Host "Selection: $selectionLabel"
Write-Host "Timeout: $TimeoutSeconds sec"
Write-Host "Capture: $(if ($VisibleCapture) { 'visible screen capture' } else { 'quiet offscreen render' })"

Remove-Item -LiteralPath $stdoutPath, $stderrPath -ErrorAction SilentlyContinue
$quotedArguments = $arguments | ForEach-Object {
    if ($_ -match "\s") { "`"$_`"" } else { $_ }
}
$processArguments = @("exec", "`"$dll`"") + $quotedArguments
$process = Start-Process -FilePath "dotnet" -ArgumentList $processArguments -NoNewWindow -PassThru -RedirectStandardOutput $stdoutPath -RedirectStandardError $stderrPath

$completed = $process.WaitForExit($TimeoutSeconds * 1000)
if (-not $completed) {
    Write-Warning "UI smoke timed out. Attempting to stop process $($process.Id)."
    try {
        Stop-Process -Id $process.Id -Force -ErrorAction Stop
    }
    catch {
        Write-Warning "Could not stop timed-out smoke process automatically: $($_.Exception.Message)"
    }

    throw "UI screenshot smoke timed out after $TimeoutSeconds seconds. Output: $OutputDir"
}

$process.Refresh()

$stdout = if (Test-Path -LiteralPath $stdoutPath) { Get-Content -LiteralPath $stdoutPath } else { @() }
$stderr = if (Test-Path -LiteralPath $stderrPath) { Get-Content -LiteralPath $stderrPath } else { @() }

$stdout | ForEach-Object { Write-Host $_ }
if ($stderr.Count -gt 0) {
    $stderr | ForEach-Object { Write-Warning $_ }
}

$exitCode = if ($null -eq $process.ExitCode) { 0 } else { $process.ExitCode }
if ($exitCode -ne 0) {
    throw "UI screenshot smoke failed with exit code $exitCode. Output: $OutputDir"
}

Write-Host "UI screenshots saved to $OutputDir"
