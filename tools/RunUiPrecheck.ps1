param(
    [string]$Configuration = "Debug",
    [string]$Platform = "Any CPU",
    [string]$OutputDir = "C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_smoke",
    [string]$Targets = "main_workspace,pipeline_form,pipeline_form_branch,pipeline_add_step_form,pipeline_add_step_branch_form,threshold_form,ai_recipe_form",
    [int]$TimeoutSeconds = 120,
    [switch]$All,
    [switch]$FailOnWarn
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $repoRoot "OpenVisionLab.sln"
$smokeProject = Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj"
$buildOutDir = "C:\Users\Public\Documents\ESTsoft\CreatorTemp\OpenVisionLabBuild\"
$msBuild = "C:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"
$smokeExe = Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\$Platform\$Configuration\net8.0-windows\PipelineViewerScreenshotSmoke.exe"
$reportPath = Join-Path $OutputDir "ui_precheck_report.md"
$stdoutPath = Join-Path $OutputDir "ui_precheck_stdout.txt"
$stderrPath = Join-Path $OutputDir "ui_precheck_stderr.txt"

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

Write-Host "== Build OpenVisionLab =="
& $msBuild $solution /t:Build /p:Configuration=$Configuration "/p:Platform=$Platform" /p:RestorePackages=false "/p:OutDir=$buildOutDir" /v:minimal
if ($LASTEXITCODE -ne 0) {
    throw "OpenVisionLab build failed."
}

Write-Host "== Build UI Screenshot Smoke =="
& $msBuild $smokeProject /t:Build /p:Configuration=$Configuration "/p:Platform=$Platform" /p:WpgCustomBuildEnabled=false /clp:ErrorsOnly /v:minimal
if ($LASTEXITCODE -ne 0) {
    throw "UI screenshot smoke build failed."
}

if (-not (Test-Path -LiteralPath $smokeExe)) {
    throw "Smoke executable was not found: $smokeExe"
}

$arguments = if ($All) {
    @("--all", $OutputDir)
}
else {
    @("--target", $Targets, $OutputDir)
}

Write-Host "== UI Screenshot Smoke =="
Write-Host "Targets: $(if ($All) { 'ALL' } else { $Targets })"
Write-Host "Timeout: $TimeoutSeconds sec"

Remove-Item -LiteralPath $stdoutPath, $stderrPath -ErrorAction SilentlyContinue
$process = Start-Process -FilePath $smokeExe -ArgumentList $arguments -NoNewWindow -PassThru -RedirectStandardOutput $stdoutPath -RedirectStandardError $stderrPath

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

$smokeExit = if ($null -eq $process.ExitCode) { 0 } else { $process.ExitCode }
$smokeOutput = if (Test-Path -LiteralPath $stdoutPath) { Get-Content -LiteralPath $stdoutPath } else { @() }
$smokeError = if (Test-Path -LiteralPath $stderrPath) { Get-Content -LiteralPath $stderrPath } else { @() }

$smokeOutput | ForEach-Object { Write-Host $_ }
if ($smokeError.Count -gt 0) {
    $smokeError | ForEach-Object { Write-Warning $_ }
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
$report = New-Object System.Collections.Generic.List[string]
$report.Add("# OpenVisionLab UI Precheck") | Out-Null
$report.Add("") | Out-Null
$report.Add("- Time: $now") | Out-Null
$report.Add("- Build: $Configuration / $Platform") | Out-Null
$report.Add("- Targets: $(if ($All) { 'ALL' } else { $Targets })") | Out-Null
$report.Add("- Timeout: $TimeoutSeconds sec") | Out-Null
$report.Add("- Output: ``$OutputDir``") | Out-Null
$report.Add("") | Out-Null
$report.Add("| Target | Status | Check | Colors | Flat | Layout | Text | Internal | Size | Image |") | Out-Null
$report.Add("| --- | --- | --- | ---: | ---: | ---: | ---: | ---: | --- | --- |") | Out-Null

foreach ($row in $rows) {
    $imageName = Split-Path -Leaf $row.Path
    $report.Add("| $($row.Target) | $($row.Status) | $($row.Check) | $($row.Colors) | $($row.Flat) | $($row.Layout) | $($row.Text) | $($row.Internal) | $($row.Size) | [$imageName]($imageName) |") | Out-Null
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

Write-Host "UI precheck report saved to $reportPath"

if ($smokeExit -ne 0 -or $hasNg) {
    throw "UI precheck failed. See $reportPath"
}

if ($FailOnWarn -and $hasWarn) {
    throw "UI precheck has warnings. See $reportPath"
}
