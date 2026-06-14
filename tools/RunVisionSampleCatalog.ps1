param(
    [string]$Configuration = "Debug",
    [string]$Platform = "Any CPU",
    [string]$CatalogPath = "docs\samples\OpenVisionLab.SampleCatalog.csv",
    [string]$OutputDir = "C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog",
    [switch]$FailOnExplore
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$catalogFullPath = if ([System.IO.Path]::IsPathRooted($CatalogPath)) { $CatalogPath } else { Join-Path $repoRoot $CatalogPath }
$runnerProject = Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj"
$runnerExe = Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\$Configuration\net8.0-windows\VisionRecipeRunnerSmoke.exe"
$msBuild = "C:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"
$reportPath = Join-Path $OutputDir "sample_catalog_report.md"

function ConvertTo-NullableDouble {
    param([string]$Value)

    if ([string]::IsNullOrWhiteSpace($Value)) {
        return $null
    }

    $parsed = 0.0
    if ([double]::TryParse(
            $Value,
            [System.Globalization.NumberStyles]::Float,
            [System.Globalization.CultureInfo]::InvariantCulture,
            [ref]$parsed)) {
        return $parsed
    }

    throw "Expected numeric value but got '$Value'."
}

function Get-RunnerMetricMap {
    param([object[]]$RunnerOutput)

    $metrics = @{}
    $metricLine = $RunnerOutput |
        ForEach-Object { $_.ToString() } |
        Where-Object { $_ -match '^\s*ResultCount=' } |
        Select-Object -First 1

    if ([string]::IsNullOrWhiteSpace($metricLine)) {
        return $metrics
    }

    foreach ($part in $metricLine.Trim().Split(',')) {
        $tokens = $part.Trim().Split('=', 2)
        if ($tokens.Length -ne 2) {
            continue
        }

        $value = 0.0
        if ([double]::TryParse(
                $tokens[1].Trim(),
                [System.Globalization.NumberStyles]::Float,
                [System.Globalization.CultureInfo]::InvariantCulture,
                [ref]$value)) {
            $metrics[$tokens[0].Trim()] = $value
        }
    }

    return $metrics
}

function Get-OptionalCsvValue {
    param(
        [object]$Row,
        [string]$Name
    )

    $property = $Row.PSObject.Properties[$Name]
    if ($null -eq $property) {
        return $null
    }

    return [string]$property.Value
}

if (-not (Test-Path -LiteralPath $catalogFullPath)) {
    throw "Sample catalog was not found: $catalogFullPath"
}

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

Write-Host "== Build VisionRecipeRunnerSmoke =="
& $msBuild $runnerProject /t:Build /p:Configuration=$Configuration "/p:Platform=$Platform" /p:WpgCustomBuildEnabled=false /clp:ErrorsOnly /v:minimal
if ($LASTEXITCODE -ne 0) {
    throw "VisionRecipeRunnerSmoke build failed."
}

if (-not (Test-Path -LiteralPath $runnerExe)) {
    throw "VisionRecipeRunnerSmoke executable was not found: $runnerExe"
}

$rows = Import-Csv -LiteralPath $catalogFullPath
$runRows = @($rows | Where-Object { -not [string]::IsNullOrWhiteSpace($_.BaselinePipeline) })

$report = New-Object System.Collections.Generic.List[string]
$report.Add("# OpenVisionLab Sample Catalog Smoke") | Out-Null
$report.Add("") | Out-Null
$report.Add("- Time: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")") | Out-Null
$report.Add("- Catalog: ``$catalogFullPath``") | Out-Null
$report.Add("- Output: ``$OutputDir``") | Out-Null
$report.Add("") | Out-Null
$report.Add("| Sample | Mode | Status | Pipeline | Result | Expected Metric | Output Image |") | Out-Null
$report.Add("| --- | --- | --- | --- | --- | --- | --- |") | Out-Null

$failures = New-Object System.Collections.Generic.List[string]

foreach ($row in $runRows) {
    $imagePath = Join-Path $repoRoot $row.ImagePath
    $pipelinePath = Join-Path $repoRoot $row.BaselinePipeline
    $safeName = ($row.SampleName -replace '[^A-Za-z0-9_.-]', '_')
    $resultImagePath = Join-Path $OutputDir "$safeName.png"
    $rawLogPath = Join-Path $OutputDir "$safeName.log"

    Write-Host "== $($row.SampleName) =="
    $previousErrorActionPreference = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    try {
        $runnerOutput = @(& $runnerExe $imagePath $pipelinePath $resultImagePath 2>&1 | ForEach-Object { $_.ToString() })
    }
    finally {
        $ErrorActionPreference = $previousErrorActionPreference
    }

    $exitCode = $LASTEXITCODE
    $runnerOutput | Set-Content -LiteralPath $rawLogPath -Encoding UTF8
    $runnerOutput | ForEach-Object { Write-Host $_ }

    $pipelineName = Split-Path -Leaf $row.BaselinePipeline
    $resultText = ($runnerOutput | Where-Object { $_ -like "Success=*" } | Select-Object -First 1)
    if ([string]::IsNullOrWhiteSpace($resultText)) {
        $resultText = "Exit=$exitCode"
    }

    $metricFailures = New-Object System.Collections.Generic.List[string]
    $expectedMetricText = "-"
    $expectedMetricName = Get-OptionalCsvValue $row "ExpectedMetricName"
    if (-not [string]::IsNullOrWhiteSpace($expectedMetricName)) {
        $metrics = Get-RunnerMetricMap $runnerOutput
        if (-not $metrics.ContainsKey($expectedMetricName)) {
            $metricFailures.Add("Metric '$expectedMetricName' was not found.") | Out-Null
            $expectedMetricText = "$expectedMetricName=missing"
        }
        else {
            $actualMetricValue = [double]$metrics[$expectedMetricName]
            $minimum = ConvertTo-NullableDouble (Get-OptionalCsvValue $row "ExpectedMetricMinimum")
            $maximum = ConvertTo-NullableDouble (Get-OptionalCsvValue $row "ExpectedMetricMaximum")
            $actualText = $actualMetricValue.ToString("0.###", [System.Globalization.CultureInfo]::InvariantCulture)
            $rangeText = ""

            if ($null -ne $minimum) {
                $rangeText = "$rangeText min=$($minimum.ToString("0.###", [System.Globalization.CultureInfo]::InvariantCulture))"
                if ($actualMetricValue -lt $minimum) {
                    $metricFailures.Add("$expectedMetricName=$actualText is below expected minimum $minimum.") | Out-Null
                }
            }

            if ($null -ne $maximum) {
                $rangeText = "$rangeText max=$($maximum.ToString("0.###", [System.Globalization.CultureInfo]::InvariantCulture))"
                if ($actualMetricValue -gt $maximum) {
                    $metricFailures.Add("$expectedMetricName=$actualText is above expected maximum $maximum.") | Out-Null
                }
            }

            $expectedMetricText = "$expectedMetricName=$actualText$rangeText"
        }
    }

    $status = if ($exitCode -eq 0 -and $metricFailures.Count -eq 0) { "OK" } else { "NG" }
    $imageLink = if (Test-Path -LiteralPath $resultImagePath) { "[$safeName.png]($safeName.png)" } else { "-" }
    $report.Add("| $($row.SampleName) | $($row.ValidationMode) | $status | $pipelineName | $resultText | $expectedMetricText | $imageLink |") | Out-Null

    $isRequired = [string]::Equals($row.ValidationMode, "Required", [StringComparison]::OrdinalIgnoreCase)
    $isExplore = [string]::Equals($row.ValidationMode, "Explore", [StringComparison]::OrdinalIgnoreCase)
    if ($exitCode -ne 0 -and ($isRequired -or ($FailOnExplore -and $isExplore))) {
        $failures.Add("$($row.SampleName) failed with exit code $exitCode. See $rawLogPath") | Out-Null
    }

    if ($metricFailures.Count -gt 0 -and ($isRequired -or ($FailOnExplore -and $isExplore))) {
        foreach ($metricFailure in $metricFailures) {
            $failures.Add("$($row.SampleName): $metricFailure See $rawLogPath") | Out-Null
        }
    }
}

$report.Add("") | Out-Null
$report.Add("## Catalog Notes") | Out-Null
$report.Add("") | Out-Null
foreach ($row in $rows) {
    $report.Add("- **$($row.SampleName)**: $($row.Goal) $($row.Notes)") | Out-Null
}

$report | Set-Content -LiteralPath $reportPath -Encoding UTF8
Write-Host "Sample catalog report saved to $reportPath"

if ($failures.Count -gt 0) {
    foreach ($failure in $failures) {
        Write-Error $failure
    }

    throw "Sample catalog smoke failed. See $reportPath"
}
