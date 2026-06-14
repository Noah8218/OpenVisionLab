param(
    [string]$Configuration = "Debug",
    [string]$Platform = "Any CPU",
    [string]$OutputDir = "C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_smoke",
    [string]$Targets = "pipeline_form,pipeline_add_step_form,pipeline_add_step_branch_form,threshold_form",
    [int]$TimeoutSeconds = 90,
    [switch]$All
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj"
$msBuild = "C:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"
$exe = Join-Path $repoRoot "tools\PipelineViewerScreenshotSmoke\bin\$Platform\$Configuration\net8.0-windows\PipelineViewerScreenshotSmoke.exe"
$stdoutPath = Join-Path $OutputDir "ui_smoke_stdout.txt"
$stderrPath = Join-Path $OutputDir "ui_smoke_stderr.txt"

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

Write-Host "== Build UI Screenshot Smoke =="
& $msBuild $project /t:Build /p:Configuration=$Configuration "/p:Platform=$Platform" /p:WpgCustomBuildEnabled=false /clp:ErrorsOnly /v:minimal
if ($LASTEXITCODE -ne 0) {
    throw "UI screenshot smoke build failed."
}

if (-not (Test-Path -LiteralPath $exe)) {
    throw "Smoke executable was not found: $exe"
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
$process = Start-Process -FilePath $exe -ArgumentList $arguments -NoNewWindow -PassThru -RedirectStandardOutput $stdoutPath -RedirectStandardError $stderrPath

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
