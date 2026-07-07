param(
    [string]$Configuration = "Debug",
    [string]$Platform = "Any CPU",
    [string]$OutputDir = "",
    [switch]$SkipRunnerBuild
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($OutputDir)) {
    $OutputDir = Join-Path $repoRoot ("artifacts\public_edge_detection_pipeline_smoke_" + (Get-Date -Format "yyyyMMdd_HHmmss"))
}
elseif (-not [System.IO.Path]::IsPathRooted($OutputDir)) {
    $OutputDir = Join-Path $repoRoot $OutputDir
}

$runnerProject = Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj"
$runnerExeCandidates = @(
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\$Platform\$Configuration\net8.0-windows7.0\VisionRecipeRunnerSmoke.exe"),
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\$Platform\$Configuration\net8.0-windows\VisionRecipeRunnerSmoke.exe"),
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\$Configuration\net8.0-windows7.0\VisionRecipeRunnerSmoke.exe"),
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\$Configuration\net8.0-windows\VisionRecipeRunnerSmoke.exe")
)

$pipelinePath = Join-Path $repoRoot "docs\samples\public\Public_EdgeDetection_Shapes.pipeline.xml"
$goodImagePath = Join-Path $repoRoot "docs\samples\public\EdgeDetection_Shapes_Synthetic_OK.png"
$badImagePath = Join-Path $repoRoot "docs\samples\public\EdgeDetection_Shapes_Synthetic_Missing_NG.png"

function Resolve-RunnerExecutable {
    param([string[]]$Candidates)

    $directHit = $Candidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
    if (-not [string]::IsNullOrWhiteSpace($directHit)) {
        return $directHit
    }

    $projectDir = Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke"
    $fallback = Get-ChildItem -LiteralPath $projectDir -Filter "VisionRecipeRunnerSmoke.exe" -Recurse -File -ErrorAction SilentlyContinue |
        Sort-Object FullName |
        Select-Object -First 1

    if ($null -ne $fallback) {
        return $fallback.FullName
    }

    return ""
}

function Get-RunnerMetricValue {
    param(
        [object[]]$Output,
        [string]$Name
    )

    foreach ($line in $Output | ForEach-Object { $_.ToString() }) {
        if ([string]::IsNullOrWhiteSpace($line) -or $line -notmatch '=') {
            continue
        }

        foreach ($part in $line.Trim().Split(',')) {
            $tokens = $part.Trim().Split('=', 2)
            if ($tokens.Length -ne 2) {
                continue
            }

            $key = $tokens[0].Trim()
            if (-not [string]::Equals($key, $Name, [StringComparison]::OrdinalIgnoreCase)) {
                continue
            }

            $parsed = 0.0
            if ([double]::TryParse(
                    $tokens[1].Trim(),
                    [System.Globalization.NumberStyles]::Float,
                    [System.Globalization.CultureInfo]::InvariantCulture,
                    [ref]$parsed)) {
                return $parsed
            }
        }
    }

    throw "Metric '$Name' was not found in runner output."
}

function Test-PipelineCase {
    param(
        [string]$Name,
        [string]$ImagePath,
        [int]$ExpectedExitCode,
        [double]$ExpectedResultCount,
        [string]$RunnerExe
    )

    $safeName = $Name -replace '[^A-Za-z0-9_.-]', '_'
    $resultImagePath = Join-Path $OutputDir "$safeName.result.png"
    $overlayImagePath = Join-Path $OutputDir "$safeName.overlay.png"
    $logPath = Join-Path $OutputDir "$safeName.log"

    $previousErrorActionPreference = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    try {
        $runnerOutput = @(& $RunnerExe $ImagePath $pipelinePath $resultImagePath "--all-overlay-image" $overlayImagePath 2>&1 | ForEach-Object { $_.ToString() })
    }
    finally {
        $ErrorActionPreference = $previousErrorActionPreference
    }

    $exitCode = $LASTEXITCODE
    $runnerOutput | Set-Content -LiteralPath $logPath -Encoding UTF8
    $resultCount = Get-RunnerMetricValue -Output $runnerOutput -Name "ResultCount"

    if ($exitCode -ne $ExpectedExitCode) {
        throw "$Name expected exit code $ExpectedExitCode but got $exitCode. See $logPath"
    }

    if ([Math]::Abs($resultCount - $ExpectedResultCount) -gt 0.001) {
        throw "$Name expected ResultCount=$ExpectedResultCount but got $resultCount. See $logPath"
    }

    foreach ($artifactPath in @($resultImagePath, $overlayImagePath, $logPath)) {
        $item = Get-Item -LiteralPath $artifactPath -ErrorAction SilentlyContinue
        if ($null -eq $item -or $item.Length -le 0) {
            throw "$Name did not create a valid artifact: $artifactPath"
        }
    }

    return [pscustomobject][ordered]@{
        Name = $Name
        ExitCode = $exitCode
        ResultCount = $resultCount
        ResultImage = $resultImagePath
        OverlayImage = $overlayImagePath
        Log = $logPath
    }
}

foreach ($requiredPath in @($runnerProject, $pipelinePath, $goodImagePath, $badImagePath)) {
    if (-not (Test-Path -LiteralPath $requiredPath)) {
        throw "Required file was not found: $requiredPath"
    }
}

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

if (-not $SkipRunnerBuild) {
    & dotnet build $runnerProject -c $Configuration "/p:Platform=$Platform" "/p:WpgCustomBuildEnabled=false"
    if ($LASTEXITCODE -ne 0) {
        throw "VisionRecipeRunnerSmoke build failed."
    }
}

$runnerExe = Resolve-RunnerExecutable -Candidates $runnerExeCandidates
if ([string]::IsNullOrWhiteSpace($runnerExe)) {
    throw "VisionRecipeRunnerSmoke executable was not found."
}

$results = @(
    (Test-PipelineCase -Name "Public_EdgeDetection_Shapes_Good" -ImagePath $goodImagePath -ExpectedExitCode 0 -ExpectedResultCount 4 -RunnerExe $runnerExe),
    (Test-PipelineCase -Name "Public_EdgeDetection_Shapes_Missing_Bad" -ImagePath $badImagePath -ExpectedExitCode 1 -ExpectedResultCount 2 -RunnerExe $runnerExe)
)

$summaryPath = Join-Path $OutputDir "public_edge_detection_pipeline_smoke.json"
$summaryPayload = [ordered]@{
    Time = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    RunnerPath = $runnerExe
    PipelinePath = $pipelinePath
    OutputDir = $OutputDir
    Results = @($results)
}
$summaryPayload | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $summaryPath -Encoding UTF8

Write-Host "Public EdgeDetection pipeline smoke passed."
Write-Host "Summary: $summaryPath"
foreach ($result in $results) {
    Write-Host "$($result.Name): Exit=$($result.ExitCode), ResultCount=$($result.ResultCount), Overlay=$($result.OverlayImage)"
}
