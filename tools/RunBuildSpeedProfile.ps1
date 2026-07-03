param(
    [string]$Configuration = "Debug",
    [string]$Platform = "Any CPU",
    [string]$OutputDir = "C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_build_speed_profile",
    [int]$WarmRunCount = 3,
    [string]$RepositoryRoot = ""
)

$ErrorActionPreference = "Stop"

if ($WarmRunCount -lt 1) {
    throw "WarmRunCount must be be at least 1."
}

$repoRoot = if ([string]::IsNullOrWhiteSpace($RepositoryRoot)) {
    Split-Path -Parent $PSScriptRoot
} else {
    $RepositoryRoot
}

$platformScript = Join-Path $repoRoot "tools\RunVisionPlatformPrecheck.ps1"
if (-not (Test-Path -LiteralPath $platformScript)) {
    throw "RunVisionPlatformPrecheck.ps1 was not found: $platformScript"
}

$profileDirName = "platform_speed_profile_" + (Get-Date -Format "yyyyMMdd_HHmmss")
$outputRoot = Join-Path $OutputDir $profileDirName
New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
New-Item -ItemType Directory -Force -Path $outputRoot | Out-Null

function Invoke-BuildPass {
    param(
        [string]$OutputPath,
        [bool]$SkipRestore
    )

    if (Test-Path -LiteralPath $OutputPath) {
        Remove-Item -LiteralPath $OutputPath -Recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $OutputPath | Out-Null

    $arguments = @(
        "-NoProfile",
        "-ExecutionPolicy",
        "Bypass",
        "-File",
        $platformScript,
        "-Configuration",
        $Configuration,
        "-Platform",
        $Platform,
        "-OutputDir",
        $OutputPath,
        "-SkipUi",
        "-SkipSampleRunnerBuild",
        "-SkipWpfShellBuild"
    )
    if ($SkipRestore) {
        $arguments += "-SkipRestore"
    }

    $outputLines = & powershell @arguments 2>&1
    $exitCode = $LASTEXITCODE
    $outputLines | ForEach-Object { Write-Host $_ }

    if ($exitCode -ne 0) {
        throw "Platform precheck failed during pass. Exit code=$exitCode. See output: $OutputPath"
    }

    $summaryPath = Join-Path $OutputPath "platform_precheck_summary.json"
    if (-not (Test-Path -LiteralPath $summaryPath)) {
        throw "Expected summary file was not created: $summaryPath"
    }

    $summary = Get-Content -LiteralPath $summaryPath -Raw | ConvertFrom-Json
    if ($null -eq $summary.Timings) {
        throw "Summary file is missing Timings section: $summaryPath"
    }

    return [PSCustomObject]@{
        RestoreSeconds = [double]$summary.Timings.RestoreSeconds
        BuildSeconds = [double]$summary.Timings.BuildSeconds
        TotalSeconds = [double]$summary.DurationSeconds
        UiSkipped = [bool]$summary.SkipUi
        RunnerSkipped = [bool]$summary.SkipSampleRunnerBuild
        WpfShellSkipped = [bool]$summary.SkipWpfShellBuild
    }
}

$runSummaries = New-Object System.Collections.Generic.List[object]

$runSummaries.Add([PSCustomObject]@{
    Pass = "cold_1"
    SkipRestore = $false
    Result = (Invoke-BuildPass -OutputPath (Join-Path $outputRoot "cold_1") -SkipRestore:$false)
}) | Out-Null

for ($i = 1; $i -le $WarmRunCount; $i++) {
    $passName = "warm_$i"
    $runSummaries.Add([PSCustomObject]@{
        Pass = $passName
        SkipRestore = $true
        Result = (Invoke-BuildPass -OutputPath (Join-Path $outputRoot $passName) -SkipRestore:$true)
    }) | Out-Null
}

$warmPasses = @($runSummaries | Where-Object { $_.SkipRestore } | ForEach-Object { $_.Result })
$warmRestore = if ($warmPasses.Count -gt 0) { [Math]::Round((($warmPasses | Measure-Object -Property RestoreSeconds -Average).Average), 3) } else { 0 }
$warmBuild = if ($warmPasses.Count -gt 0) { [Math]::Round((($warmPasses | Measure-Object -Property BuildSeconds -Average).Average), 3) } else { 0 }
$warmTotal = if ($warmPasses.Count -gt 0) { [Math]::Round((($warmPasses | Measure-Object -Property TotalSeconds -Average).Average), 3) } else { 0 }

$report = [ordered]@{
    Time = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Configuration = $Configuration
    Platform = $Platform
    WarmRunCount = $WarmRunCount
    Runs = @()
}

foreach ($pass in $runSummaries) {
    $report["Runs"] += [ordered]@{
        Pass = $pass.Pass
        SkipRestore = [bool]$pass.SkipRestore
        RestoreSeconds = [double]$pass.Result.RestoreSeconds
        BuildSeconds = [double]$pass.Result.BuildSeconds
        TotalSeconds = [double]$pass.Result.TotalSeconds
    }
}

$report["WarmAverageSeconds"] = [ordered]@{
    RestoreSeconds = $warmRestore
    BuildSeconds = $warmBuild
    TotalSeconds = $warmTotal
}

$reportPath = Join-Path $outputRoot "build_speed_profile.json"
Set-Content -LiteralPath $reportPath -Value ($report | ConvertTo-Json -Depth 6) -Encoding UTF8

$reportMarkdownPath = Join-Path $outputRoot "build_speed_profile.md"
@"
# Build speed profile

- Time: $($report.Time)
- Configuration: $Configuration / $Platform
- Warm run count: $WarmRunCount
- Cold pass: $($report.Runs[0].Pass)
- Warm average (build): $warmBuild sec
- Warm average (restore): $warmRestore sec
- Warm average (total): $warmTotal sec
"@ | Set-Content -LiteralPath $reportMarkdownPath -Encoding UTF8

Write-Host "Build speed profile saved to $outputRoot"
Write-Host "Profile summary: warm build avg=$warmBuild sec, warm restore avg=$warmRestore sec, warm total avg=$warmTotal sec"
