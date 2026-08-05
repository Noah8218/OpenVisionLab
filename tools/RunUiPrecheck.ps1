param(
    [string]$Configuration = "Debug",
    [string]$Platform = "Any CPU",
    [string]$OutputDir = "C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_smoke",
    [string]$Targets = "wpf_shell_preview,wpf_shell_host_window_chrome,wpf_shell_host_workspace_empty,wpf_shell_host_workspace,wpf_shell_host_workspace_image_load,wpf_shell_host_tool_input_empty,wpf_shell_host_tool_input_image_load_save,wpf_shell_host_workspace_output,wpf_shell_host_large_image,wpf_shell_host_layer_auto_docking,wpf_shell_host_layer_docking_vertical,wpf_shell_host_layer_docking_n_panels,wpf_shell_host_layer_docking_grid,wpf_shell_host_layer_docking_tabs,wpf_shell_host_tool_rail_compact,wpf_shell_host_layer_docking,wpf_shell_host_layer_docking_functional,wpf_shell_host_layer_popout,wpf_shell_host_bridge,wpf_shell_host_native_tool,wpf_shell_host_threshold_basic_tool,wpf_shell_host_threshold_tool,wpf_shell_host_pipeline_review,wpf_shell_host_rotate_scale_tool,wpf_filter_morphology_layout_guard,wpf_shell_host_blob_tool,wpf_shell_host_contour_tool,wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool,wpf_shell_host_matching_tool,wpf_shell_host_feature_matching_tool,wpf_shell_host_pending_tool,wpf_property_grid_matching_combo,wpf_roi_editor,wpf_template_editor_opengl,wpf_image_compare,log_panel_contract_check,localization_catalog_contract_check",
    [string]$WpgCustomSourceRoot = "",
    [string]$WpgCustomBuildEnabled = "false",
    [switch]$SkipSolutionBuild,
    [switch]$SkipSmokeBuild,
    [switch]$SkipRestore,
    [int]$TimeoutSeconds = 120,
    [switch]$All,
    [switch]$WpfTools,
    [switch]$ToolOutputFlow,
    [switch]$FailOnWarn,
    [switch]$VisibleCapture
)

$ErrorActionPreference = "Stop"

$precheckStartedAt = Get-Date
$stageDurationsSeconds = [ordered]@{
    SolutionRestoreSeconds = 0
    SolutionBuildSeconds = 0
    SmokeRestoreSeconds = 0
    SmokeBuildSeconds = 0
    SmokeExecutionSeconds = 0
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
        $script:stageDurationsSeconds[$Name] = [Math]::Round($stopwatch.Elapsed.TotalSeconds, 3)
        Write-Host "== $Name duration: $($script:stageDurationsSeconds[$Name]) sec"
    }
}

$repoRoot = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $repoRoot "OpenVisionLab.sln"
$smokeProject = Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj"
$buildOutDir = [System.IO.Path]::GetFullPath((Join-Path $OutputDir "OpenVisionLabBuild"))
if (-not $buildOutDir.EndsWith([System.IO.Path]::DirectorySeparatorChar.ToString())) {
    $buildOutDir += [System.IO.Path]::DirectorySeparatorChar
}
$msBuildCandidates = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
)
$msBuild = $msBuildCandidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
$smokeDllCandidates = @(
    (Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\$Platform\$Configuration\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll"),
    (Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\$Platform\$Configuration\net8.0-windows\PipelineViewerScreenshotSmoke.dll"),
    (Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\$Configuration\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll"),
    (Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\$Configuration\net8.0-windows\PipelineViewerScreenshotSmoke.dll"),
    (Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll"),
    (Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\net8.0-windows\PipelineViewerScreenshotSmoke.dll")
)
$toolOpenPerfGateScript = Join-Path $PSScriptRoot "CheckToolOpenPerf.ps1"
$reportPath = Join-Path $OutputDir "ui_precheck_report.md"
$summaryPath = Join-Path $OutputDir "ui_precheck_summary.json"
$stdoutPath = Join-Path $OutputDir "ui_precheck_stdout.txt"
$stderrPath = Join-Path $OutputDir "ui_precheck_stderr.txt"

function Resolve-SmokeDll {
    param(
        [string[]]$Candidates,
        [string]$ProjectPath
    )

    $hit = $Candidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
    if (-not [string]::IsNullOrWhiteSpace($hit)) {
        return $hit
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

function Add-UiSmokeTargets {
    param(
        [string]$TargetText,
        [string[]]$TargetsToAdd
    )

    $targetSet = New-Object System.Collections.Generic.List[string]
    foreach ($target in ($TargetText -split ",")) {
        $trimmed = $target.Trim()
        if (-not [string]::IsNullOrWhiteSpace($trimmed) -and -not $targetSet.Contains($trimmed)) {
            $targetSet.Add($trimmed) | Out-Null
        }
    }

    foreach ($target in $TargetsToAdd) {
        if (-not [string]::IsNullOrWhiteSpace($target) -and -not $targetSet.Contains($target)) {
            $targetSet.Add($target) | Out-Null
        }
    }

    return [string]::Join(",", $targetSet)
}

$toolOutputFlowTargets = @(
    "wpf_preprocess_output_preview_flow",
    "wpf_algorithm_output_preview_flow"
)

if ($WpfTools -and -not $All) {
    $wpfToolTargets = @(
        "wpf_shell_preview",
        "wpf_shell_host_window_chrome",
        "wpf_shell_host_workspace_empty",
        "wpf_shell_host_workspace",
        "wpf_shell_host_workspace_image_load",
        "wpf_shell_host_tool_input_empty",
        "wpf_shell_host_tool_input_image_load_save",
        "wpf_shell_host_workspace_output",
        "wpf_shell_host_large_image",
        "wpf_shell_host_layer_auto_docking",
        "wpf_shell_host_layer_docking_vertical",
        "wpf_shell_host_layer_docking_n_panels",
        "wpf_shell_host_layer_docking_grid",
        "wpf_shell_host_layer_docking_tabs",
        "wpf_shell_host_tool_rail_compact",
        "wpf_shell_host_layer_docking",
        "wpf_shell_host_layer_docking_functional",
        "wpf_shell_host_layer_popout",
        "wpf_shell_host_bridge",
        "wpf_shell_host_native_tool",
        "wpf_shell_host_threshold_basic_tool",
        "wpf_shell_host_threshold_tool",
        "wpf_shell_host_pipeline_review",
        "wpf_shell_host_rotate_scale_tool",
        "wpf_filter_morphology_layout_guard",
        "wpf_shell_host_blob_tool",
        "wpf_shell_host_contour_tool",
        "wpf_shell_host_line_measure_tool",
        "wpf_shell_host_line_intersection_tool",
        "wpf_shell_host_matching_tool",
        "wpf_shell_host_feature_matching_tool",
        "wpf_shell_host_pending_tool",
        "wpf_property_grid_matching_combo",
        "wpf_roi_editor",
        "wpf_template_editor_opengl",
        "wpf_image_compare",
        "localization_catalog_contract_check"
    )

    $Targets = Add-UiSmokeTargets -TargetText $Targets -TargetsToAdd $wpfToolTargets
    if ($TimeoutSeconds -lt 240) {
        $TimeoutSeconds = 240
    }
}

if ($ToolOutputFlow -and -not $All) {
    $Targets = if ($PSBoundParameters.ContainsKey("Targets")) {
        Add-UiSmokeTargets -TargetText $Targets -TargetsToAdd $toolOutputFlowTargets
    }
    else {
        [string]::Join(",", $toolOutputFlowTargets)
    }

    if ($TimeoutSeconds -lt 180) {
        $TimeoutSeconds = 180
    }
}

[void][System.IO.Directory]::CreateDirectory($OutputDir)
[void][System.IO.Directory]::CreateDirectory($buildOutDir)

$wpgCustomBuildDisabledValues = @("false", "0", "no")
$wpgCustomBuildText = if ($null -eq $WpgCustomBuildEnabled) { "" } else { $WpgCustomBuildEnabled.Trim().ToLowerInvariant() }
$wpgCustomBuildEnabledValue = -not $wpgCustomBuildDisabledValues.Contains($wpgCustomBuildText)

$buildProperties = @(
    "/p:Configuration=$Configuration",
    "/p:Platform=$Platform",
    "/p:RestorePackages=false",
    "/p:OutDir=$buildOutDir",
    "/p:WpgCustomBuildEnabled=$wpgCustomBuildEnabledValue"
)
$smokeBuildProperties = @(
    "/p:Configuration=$Configuration",
    "/p:Platform=$Platform",
    "/p:WpgCustomBuildEnabled=false",
    "/p:UseAppHost=false"
)
if (-not $SkipSolutionBuild) {
    Write-Host "== Restore OpenVisionLab =="
    if ($SkipRestore) {
        Write-Host "Restore skipped by -SkipRestore."
        $stageDurationsSeconds["SolutionRestoreSeconds"] = 0
    }
    else {
        Invoke-Stage -Name "SolutionRestoreSeconds" -Action {
            & dotnet restore $solution "/p:Configuration=$Configuration" "/p:Platform=$Platform" "/p:WpgCustomBuildEnabled=$wpgCustomBuildEnabledValue"
            if ($LASTEXITCODE -ne 0) {
                throw "OpenVisionLab restore failed."
            }
        }
    }

    Write-Host "== Build OpenVisionLab =="
    Invoke-Stage -Name "SolutionBuildSeconds" -Action {
    if (-not [string]::IsNullOrWhiteSpace($msBuild)) {
            & $msBuild $solution /t:Build @buildProperties @parallelBuildArguments /p:Restore=false /v:minimal
        }
        else {
            & dotnet build $solution -c $Configuration --no-restore --maxcpucount @buildProperties
        }
        if ($LASTEXITCODE -ne 0) {
            throw "OpenVisionLab build failed."
        }
    }
}

if (-not $SkipSmokeBuild) {
    Write-Host "== Restore UI Screenshot Smoke =="
    if ($SkipRestore) {
        Write-Host "Restore skipped by -SkipRestore."
        $stageDurationsSeconds["SmokeRestoreSeconds"] = 0
    }
    else {
        Invoke-Stage -Name "SmokeRestoreSeconds" -Action {
            & dotnet restore $smokeProject @smokeBuildProperties
            if ($LASTEXITCODE -ne 0) {
                throw "UI screenshot smoke restore failed."
            }
        }
    }

    Write-Host "== Build UI Screenshot Smoke =="
    Invoke-Stage -Name "SmokeBuildSeconds" -Action {
    if (-not [string]::IsNullOrWhiteSpace($msBuild)) {
            & $msBuild $smokeProject /t:Build @smokeBuildProperties @parallelBuildArguments /p:Restore=false /clp:ErrorsOnly /v:minimal
        }
        else {
            & dotnet build $smokeProject -c $Configuration --no-restore --maxcpucount @smokeBuildProperties
        }
        if ($LASTEXITCODE -ne 0) {
            throw "UI screenshot smoke build failed."
        }
    }
}

$smokeDll = Resolve-SmokeDll -Candidates $smokeDllCandidates -ProjectPath $smokeProject
if ([string]::IsNullOrWhiteSpace($smokeDll) -or -not (Test-Path -LiteralPath $smokeDll)) {
    $candidateList = if ($smokeDllCandidates.Count -gt 0) { $smokeDllCandidates -join "; " } else { "(none)" }
    throw "Smoke DLL was not found. Checked candidates: $candidateList"
}

$arguments = if ($All) {
    @("--all", $OutputDir)
}
else {
    @("--target", $Targets, $OutputDir)
}

if ($VisibleCapture) {
    $arguments += "--visible-capture"
}
else {
    $arguments += "--quiet"
}

Write-Host "== UI Screenshot Smoke =="
Write-Host "Targets: $(if ($All) { 'ALL' } else { $Targets })"
Write-Host "Timeout: $TimeoutSeconds sec"
Write-Host "Capture: $(if ($VisibleCapture) { 'visible screen capture' } else { 'quiet offscreen render' })"

$smokeExit = 0
Invoke-Stage -Name "SmokeExecutionSeconds" -Action {
    Remove-Item -LiteralPath $stdoutPath, $stderrPath -ErrorAction SilentlyContinue
    $quotedArguments = $arguments | ForEach-Object {
        if ($_ -match "\s") { "`"$_`"" } else { $_ }
    }
    $processArguments = @("exec", "`"$smokeDll`"") + $quotedArguments
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
    $script:smokeExit = if ($null -eq $process.ExitCode) { 0 } else { $process.ExitCode }
}
$smokeOutput = if (Test-Path -LiteralPath $stdoutPath) { Get-Content -LiteralPath $stdoutPath } else { @() }
$smokeError = if (Test-Path -LiteralPath $stderrPath) { Get-Content -LiteralPath $stderrPath } else { @() }

$smokeOutput | ForEach-Object { Write-Host $_ }
if ($smokeError.Count -gt 0) {
    $smokeError | ForEach-Object { Write-Warning $_ }
}

$targetNames = if ($All) {
    @()
}
else {
    @(($Targets -split ",") | ForEach-Object { $_.Trim() } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
}
$runToolOpenPerfGate = [bool]($All -or $targetNames.Contains("wpf_tool_open_perf"))
$toolOpenPerfGateStatus = if ($runToolOpenPerfGate) { "PENDING" } else { "SKIP" }
$toolOpenPerfGateExit = 0
$toolOpenPerfGatePerfFile = Join-Path $OutputDir "wpf_tool_open_perf.perf.txt"
$toolOpenPerfGateOutput = @()

if ($runToolOpenPerfGate) {
    Write-Host "== Tool Open Perf Gate =="

    if ($smokeExit -ne 0) {
        $toolOpenPerfGateStatus = "SKIP"
        $toolOpenPerfGateOutput = @("ToolOpenPerfGate=SKIP", "Reason=UI smoke exited before perf gate could run.")
    }
    elseif (-not (Test-Path -LiteralPath $toolOpenPerfGateScript)) {
        $toolOpenPerfGateStatus = "NG"
        $toolOpenPerfGateExit = 1
        $toolOpenPerfGateOutput = @("ToolOpenPerfGate=NG", "Issue=Perf gate script was not found: $toolOpenPerfGateScript")
    }
    else {
        Invoke-Stage -Name "ToolOpenPerfGateSeconds" -Action {
            $gateOutput = & powershell -NoProfile -ExecutionPolicy Bypass -File $toolOpenPerfGateScript -Path $toolOpenPerfGatePerfFile 2>&1
            $toolOpenPerfGateExit = $LASTEXITCODE
            $toolOpenPerfGateStatus = if ($toolOpenPerfGateExit -eq 0) { "OK" } else { "NG" }
            $toolOpenPerfGateOutput = @($gateOutput | ForEach-Object { [string]$_ })
        }
    }

    $toolOpenPerfGateOutput | ForEach-Object { Write-Host $_ }
}

$rows = New-Object System.Collections.Generic.List[object]
foreach ($line in $smokeOutput) {
    if ($line -notmatch "^(?<target>[^=]+)=(?<status>OK|NG)\|") { continue }

    $parts = $line -split "\|"
    $row = [ordered]@{
        Target = $Matches["target"]
        Status = $Matches["status"]
        Check = ""
        Colors = ""
        Flat = ""
        Layout = ""
        Text = ""
        Internal = ""
        Size = ""
        Path = $parts[-1]
    }

    foreach ($part in $parts) {
        if ($part -match "^check=(.+)$") { $row.Check = $Matches[1] }
        elseif ($part -match "^colors=(.+)$") { $row.Colors = $Matches[1] }
        elseif ($part -match "^flat=(.+)$") { $row.Flat = $Matches[1] }
        elseif ($part -match "^layout=(.+)$") { $row.Layout = $Matches[1] }
        elseif ($part -match "^text=(.+)$") { $row.Text = $Matches[1] }
        elseif ($part -match "^internal=(.+)$") { $row.Internal = $Matches[1] }
        elseif ($part -match "^size=(.+)$") { $row.Size = $Matches[1] }
    }

    $rows.Add([pscustomobject]$row) | Out-Null
}

$now = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$okCount = @($rows | Where-Object { $_.Status -eq "OK" -and $_.Check -eq "OK" }).Count
$warnCount = @($rows | Where-Object { $_.Check -eq "WARN" }).Count
$ngCount = @($rows | Where-Object { $_.Status -ne "OK" -or $_.Check -eq "NG" }).Count
$targetCount = if ($All) {
    $rows.Count
}
else {
    @(($Targets -split ",") | ForEach-Object { $_.Trim() } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }).Count
}
$missingTargetResultCount = if ($All) { 0 } else { [Math]::Max(0, $targetCount - $rows.Count) }
if ($missingTargetResultCount -gt 0) {
    $ngCount += $missingTargetResultCount
}
if ($toolOpenPerfGateStatus -eq "NG") {
    $ngCount += 1
}
$report = New-Object System.Collections.Generic.List[string]
$report.Add("# OpenVisionLab UI Precheck") | Out-Null
$report.Add("") | Out-Null
$report.Add("- Time: $now") | Out-Null
$report.Add("- Build: $Configuration / $Platform") | Out-Null
$report.Add("- WPF tools: $(if ($WpfTools) { 'enabled' } else { 'disabled' })") | Out-Null
$report.Add("- Tool output flow: $(if ($ToolOutputFlow) { 'enabled' } else { 'disabled' })") | Out-Null
$report.Add("- Result: OK $okCount / WARN $warnCount / NG $ngCount / Targets $targetCount") | Out-Null
if ($runToolOpenPerfGate) {
    $report.Add("- Tool open perf gate: $toolOpenPerfGateStatus") | Out-Null
}
if ($missingTargetResultCount -gt 0) {
    $report.Add("- Missing target results: $missingTargetResultCount") | Out-Null
}
$report.Add("- Targets: $(if ($All) { 'ALL' } else { $Targets })") | Out-Null
$report.Add("- Timeout: $TimeoutSeconds sec") | Out-Null
$report.Add("- Capture: $(if ($VisibleCapture) { 'visible screen capture' } else { 'quiet offscreen render' })") | Out-Null
$report.Add("- Output: ``$OutputDir``") | Out-Null
$report.Add("") | Out-Null
$report.Add("| Target | Status | Check | Colors | Flat | Layout | Text | Internal | Size | Image |") | Out-Null
$report.Add("| --- | --- | --- | ---: | ---: | ---: | ---: | ---: | --- | --- |") | Out-Null

foreach ($row in $rows) {
    $imageName = Split-Path -Leaf $row.Path
    $report.Add("| $($row.Target) | $($row.Status) | $($row.Check) | $($row.Colors) | $($row.Flat) | $($row.Layout) | $($row.Text) | $($row.Internal) | $($row.Size) | [$imageName]($imageName) |") | Out-Null
}

if ($runToolOpenPerfGate) {
    $report.Add("") | Out-Null
    $report.Add("## Tool Open Perf Gate") | Out-Null
    $report.Add("") | Out-Null
    $report.Add('```text') | Out-Null
    $toolOpenPerfGateOutput | ForEach-Object { $report.Add($_) | Out-Null }
    $report.Add('```') | Out-Null
}

$report.Add("") | Out-Null
$report.Add("## Raw Output") | Out-Null
$report.Add("") | Out-Null
$report.Add('```text') | Out-Null
$smokeOutput | ForEach-Object { $report.Add($_) | Out-Null }
if ($smokeError.Count -gt 0) {
    $report.Add("") | Out-Null
    $report.Add("stderr:") | Out-Null
    $smokeError | ForEach-Object { $report.Add($_) | Out-Null }
}
$report.Add('```') | Out-Null
$report | Set-Content -LiteralPath $reportPath -Encoding UTF8

$hasNg = $rows | Where-Object { $_.Status -ne "OK" -or $_.Check -eq "NG" }
$hasWarn = $rows | Where-Object { $_.Check -eq "WARN" }

if ($All) {
    $targetNames = @($rows | ForEach-Object { $_.Target })
}
$summaryStatus = if ($smokeExit -ne 0 -or $hasNg -or $missingTargetResultCount -gt 0 -or $toolOpenPerfGateStatus -eq "NG") {
    "NG"
}
elseif ($hasWarn) {
    "WARN"
}
else {
    "OK"
}
$summaryRows = @(
    $rows | ForEach-Object {
        [ordered]@{
            Target = $_.Target
            Status = $_.Status
            Check = $_.Check
            Colors = $_.Colors
            Flat = $_.Flat
            Layout = $_.Layout
            Text = $_.Text
            Internal = $_.Internal
            Size = $_.Size
            Path = $_.Path
        }
    }
)
$summary = [ordered]@{
    DurationSeconds = [Math]::Round(((Get-Date) - $precheckStartedAt).TotalSeconds, 3)
    Status = $summaryStatus
    Time = $now
    Configuration = $Configuration
    Platform = $Platform
    WpfTools = [bool]$WpfTools
    ToolOutputFlow = [bool]$ToolOutputFlow
    All = [bool]$All
    FailOnWarn = [bool]$FailOnWarn
    VisibleCapture = [bool]$VisibleCapture
    TimeoutSeconds = $TimeoutSeconds
    TargetCount = $targetCount
    Counts = [ordered]@{
        OK = $okCount
        WARN = $warnCount
        NG = $ngCount
    }
    Targets = $targetNames
    OutputDir = $OutputDir
    ReportPath = $reportPath
    SummaryPath = $summaryPath
    ToolOpenPerfGate = [ordered]@{
        Enabled = [bool]$runToolOpenPerfGate
        Status = $toolOpenPerfGateStatus
        ExitCode = $toolOpenPerfGateExit
        PerfPath = $toolOpenPerfGatePerfFile
        Output = $toolOpenPerfGateOutput
    }
    Timings = [ordered]@{
        SolutionRestoreSeconds = $stageDurationsSeconds.SolutionRestoreSeconds
        SolutionBuildSeconds = $stageDurationsSeconds.SolutionBuildSeconds
        SmokeRestoreSeconds = $stageDurationsSeconds.SmokeRestoreSeconds
        SmokeBuildSeconds = $stageDurationsSeconds.SmokeBuildSeconds
        SmokeExecutionSeconds = $stageDurationsSeconds.SmokeExecutionSeconds
        ToolOpenPerfGateSeconds = $stageDurationsSeconds.ToolOpenPerfGateSeconds
    }
    Rows = $summaryRows
}
$summary | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $summaryPath -Encoding UTF8

Write-Host "UI precheck report saved to $reportPath"
Write-Host "UI precheck summary saved to $summaryPath"

if ($smokeExit -ne 0 -or $hasNg) {
    throw "UI precheck failed. See $reportPath"
}

if ($missingTargetResultCount -gt 0) {
    throw "UI precheck missed target results. See $reportPath"
}

if ($toolOpenPerfGateStatus -eq "NG") {
    throw "Tool open perf gate failed. See $reportPath"
}

if ($FailOnWarn -and $hasWarn) {
    throw "UI precheck has warnings. See $reportPath"
}

