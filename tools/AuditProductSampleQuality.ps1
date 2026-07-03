param(
    [string]$CatalogPath = "docs\samples\OpenVisionLab.ProductSampleCatalog.csv",
    [string]$SummaryPath = "artifacts\product_sample_catalog_field_variation\sample_catalog_summary.json",
    [string]$OutputDir = "artifacts\product_sample_quality_audit",
    [int]$ImageSampleStep = 3,
    [switch]$FailOnCritical
)

$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing
Add-Type -ReferencedAssemblies 'System.Drawing' -TypeDefinition @"
using System;
using System.Drawing;

public sealed class OpenVisionImageDeltaAuditResult
{
    public double MeanAbsDiff { get; set; }
    public double ChangedRatio { get; set; }
    public int ComparedPixels { get; set; }
}

public static class OpenVisionImageDeltaAudit
{
    public static OpenVisionImageDeltaAuditResult Measure(string leftPath, string rightPath, int sampleStep)
    {
        if (string.IsNullOrWhiteSpace(leftPath)) throw new ArgumentException("leftPath");
        if (string.IsNullOrWhiteSpace(rightPath)) throw new ArgumentException("rightPath");
        int step = Math.Max(1, sampleStep);
        using (Bitmap left = new Bitmap(leftPath))
        using (Bitmap right = new Bitmap(rightPath))
        {
            int width = Math.Min(left.Width, right.Width);
            int height = Math.Min(left.Height, right.Height);
            double sum = 0.0;
            int changed = 0;
            int count = 0;
            for (int y = 0; y < height; y += step)
            {
                for (int x = 0; x < width; x += step)
                {
                    Color lc = left.GetPixel(x, y);
                    Color rc = right.GetPixel(x, y);
                    double lg = (lc.R * 0.299) + (lc.G * 0.587) + (lc.B * 0.114);
                    double rg = (rc.R * 0.299) + (rc.G * 0.587) + (rc.B * 0.114);
                    double diff = Math.Abs(lg - rg);
                    sum += diff;
                    if (diff >= 18.0)
                    {
                        changed++;
                    }

                    count++;
                }
            }

            return new OpenVisionImageDeltaAuditResult
            {
                MeanAbsDiff = count == 0 ? 0.0 : sum / count,
                ChangedRatio = count == 0 ? 0.0 : (double)changed / count,
                ComparedPixels = count
            };
        }
    }
}
"@

function Resolve-RepoPath {
    param([string]$Path)
    if ([string]::IsNullOrWhiteSpace($Path)) {
        return $Path
    }

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return $Path
    }

    return Join-Path $script:RepoRoot $Path
}

function Split-MetricParts {
    param([string]$Value)
    if ([string]::IsNullOrWhiteSpace($Value)) {
        return @()
    }

    return @($Value -split '\s*[;|]\s*' | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
}

function Convert-ToNullableDouble {
    param([string]$Value)
    if ([string]::IsNullOrWhiteSpace($Value)) {
        return $null
    }

    $number = 0.0
    if ([double]::TryParse($Value.Trim(), [System.Globalization.NumberStyles]::Float, [System.Globalization.CultureInfo]::InvariantCulture, [ref]$number)) {
        return $number
    }

    return $null
}

function Get-ExpectedMetricMap {
    param($Row)
    $names = @(Split-MetricParts $Row.ExpectedMetricName)
    $minimums = @(Split-MetricParts $Row.ExpectedMetricMinimum)
    $maximums = @(Split-MetricParts $Row.ExpectedMetricMaximum)
    $map = @{}
    for ($i = 0; $i -lt $names.Count; $i++) {
        $name = $names[$i].Trim()
        if ([string]::IsNullOrWhiteSpace($name)) {
            continue
        }

        $minimum = if ($i -lt $minimums.Count) { Convert-ToNullableDouble $minimums[$i] } else { $null }
        $maximum = if ($i -lt $maximums.Count) { Convert-ToNullableDouble $maximums[$i] } else { $null }
        $map[$name] = [pscustomobject]@{
            Name = $name
            Minimum = $minimum
            Maximum = $maximum
        }
    }

    return $map
}

function Get-ActualMetricMap {
    param([string]$MetricText)
    $map = @{}
    if ([string]::IsNullOrWhiteSpace($MetricText)) {
        return $map
    }

    foreach ($part in ($MetricText -split ';')) {
        $match = [regex]::Match($part, '^\s*(?<name>[^=\s]+)\s*=\s*(?<value>-?\d+(?:\.\d+)?)')
        if ($match.Success) {
            $map[$match.Groups['name'].Value.Trim()] = [double]::Parse(
                $match.Groups['value'].Value,
                [System.Globalization.CultureInfo]::InvariantCulture)
        }
    }

    return $map
}

function Format-Number {
    param([double]$Value, [string]$Format = '0.###')
    return $Value.ToString($Format, [System.Globalization.CultureInfo]::InvariantCulture)
}

function Format-Range {
    param($Expected)
    if ($null -eq $Expected) {
        return '-'
    }

    if ($null -ne $Expected.Minimum -and $null -ne $Expected.Maximum) {
        return (Format-Number $Expected.Minimum) + '..' + (Format-Number $Expected.Maximum)
    }

    if ($null -ne $Expected.Minimum) {
        return '>=' + (Format-Number $Expected.Minimum)
    }

    if ($null -ne $Expected.Maximum) {
        return '<=' + (Format-Number $Expected.Maximum)
    }

    return '-'
}

function Get-RangeGapRatio {
    param($GoodExpected, $BadExpected)
    if ($null -eq $GoodExpected -or $null -eq $BadExpected) {
        return $null
    }

    if ($null -eq $GoodExpected.Minimum -or $null -eq $GoodExpected.Maximum -or
        $null -eq $BadExpected.Minimum -or $null -eq $BadExpected.Maximum) {
        return $null
    }

    $gap = 0.0
    if ($BadExpected.Maximum -lt $GoodExpected.Minimum) {
        $gap = $GoodExpected.Minimum - $BadExpected.Maximum
    }
    elseif ($BadExpected.Minimum -gt $GoodExpected.Maximum) {
        $gap = $BadExpected.Minimum - $GoodExpected.Maximum
    }

    $band = [Math]::Max([Math]::Abs($GoodExpected.Maximum - $GoodExpected.Minimum), 0.000001)
    return $gap / $band
}

function Select-AuditMetricName {
    param($MetricNames, $GoodExpected, $BadExpected, $GoodActual, $BadActual)
    if ($null -eq $MetricNames -or $MetricNames.Count -eq 0) {
        return '-'
    }

    $bestName = $MetricNames[0]
    $bestGap = $null
    foreach ($name in $MetricNames) {
        $gap = Get-RangeGapRatio $GoodExpected[$name] $BadExpected[$name]
        if ($null -ne $gap -and ($null -eq $bestGap -or $gap -gt $bestGap)) {
            $bestName = $name
            $bestGap = $gap
        }
    }

    if ($null -ne $bestGap -and $bestGap -gt 0.0) {
        return $bestName
    }

    $bestDelta = -1.0
    foreach ($name in $MetricNames) {
        if ($GoodActual.ContainsKey($name) -and $BadActual.ContainsKey($name)) {
            $delta = [Math]::Abs([double]$BadActual[$name] - [double]$GoodActual[$name])
            if ($delta -gt $bestDelta) {
                $bestName = $name
                $bestDelta = $delta
            }
        }
    }

    return $bestName
}

function Resolve-Flag {
    param(
        [Nullable[double]]$RangeGapRatio,
        [double]$ActualDelta,
        [double]$MeanAbsDiff,
        [double]$ChangedRatio
    )

    if ($null -ne $RangeGapRatio) {
        if ($RangeGapRatio -le 0.0) {
            return 'CRITICAL metric ranges overlap'
        }

        if ($RangeGapRatio -lt 0.35) {
            return 'REVIEW weak metric margin'
        }

        if ($RangeGapRatio -gt 6.0 -and ($ChangedRatio -gt 0.25 -or $MeanAbsDiff -gt 28.0)) {
            return 'REVIEW possibly too obvious'
        }
    }

    if ($ChangedRatio -gt 0.40 -or $MeanAbsDiff -gt 38.0) {
        return 'REVIEW visual difference high'
    }

    if ($ChangedRatio -lt 0.002 -and $ActualDelta -lt 0.05) {
        return 'REVIEW possibly too subtle'
    }

    return 'OK'
}

function Resolve-Recommendation {
    param([string]$Flag)
    if ($Flag.StartsWith('CRITICAL')) {
        return 'Tighten expected metric ranges or regenerate the pair so Good and Bad do not overlap.'
    }

    if ($Flag.Contains('weak metric')) {
        return 'Tune defect strength or acceptance range until the metric separation is visible but still realistic.'
    }

    if ($Flag.Contains('too obvious') -or $Flag.Contains('visual difference high')) {
        return 'Reduce defect size/count/contrast or add field noise to avoid a toy-like Good/Bad split.'
    }

    if ($Flag.Contains('too subtle')) {
        return 'Inspect the overlay and consider a stronger defect or a more sensitive metric.'
    }

    return 'Keep as baseline unless visual review says otherwise.'
}

$script:RepoRoot = Split-Path -Parent $PSScriptRoot
$catalogFullPath = Resolve-RepoPath $CatalogPath
$summaryFullPath = Resolve-RepoPath $SummaryPath
$outputFullDir = Resolve-RepoPath $OutputDir
New-Item -ItemType Directory -Force -Path $outputFullDir | Out-Null

if (-not (Test-Path -LiteralPath $catalogFullPath)) {
    throw "Catalog not found: $catalogFullPath"
}

if (-not (Test-Path -LiteralPath $summaryFullPath)) {
    throw "Summary not found: $summaryFullPath"
}

$catalogRows = @(Import-Csv -LiteralPath $catalogFullPath)
$summary = Get-Content -LiteralPath $summaryFullPath -Raw | ConvertFrom-Json
$summaryBySample = @{}
foreach ($result in @($summary.Results)) {
    if ($null -ne $result -and -not [string]::IsNullOrWhiteSpace($result.SampleName)) {
        $summaryBySample[$result.SampleName] = $result
    }
}

$records = New-Object 'System.Collections.Generic.List[object]'
$pairGroups = $catalogRows |
    Where-Object { -not [string]::IsNullOrWhiteSpace($_.PairGroup) } |
    Group-Object -Property PairGroup

foreach ($group in $pairGroups) {
    $rows = @($group.Group)
    $goodRows = @($rows | Where-Object { $_.PairRole -eq 'Good' -and $_.ValidationMode -ne 'ExpectedFailure' })
    $badRows = @($rows | Where-Object { $_.PairRole -eq 'Bad' -or $_.ValidationMode -eq 'ExpectedFailure' })
    foreach ($good in $goodRows) {
        foreach ($bad in $badRows) {
            if (-not $summaryBySample.ContainsKey($good.SampleName) -or -not $summaryBySample.ContainsKey($bad.SampleName)) {
                continue
            }

            $goodSummary = $summaryBySample[$good.SampleName]
            $badSummary = $summaryBySample[$bad.SampleName]
            $goodExpected = Get-ExpectedMetricMap $good
            $badExpected = Get-ExpectedMetricMap $bad
            $goodActual = Get-ActualMetricMap $goodSummary.ExpectedMetricText
            $badActual = Get-ActualMetricMap $badSummary.ExpectedMetricText
            $commonMetrics = @($goodExpected.Keys | Where-Object { $badExpected.ContainsKey($_) })
            if ($commonMetrics.Count -eq 0) {
                $commonMetrics = @($goodActual.Keys | Where-Object { $badActual.ContainsKey($_) })
            }

            $metricName = Select-AuditMetricName $commonMetrics $goodExpected $badExpected $goodActual $badActual
            $goodValue = if ($goodActual.ContainsKey($metricName)) { [double]$goodActual[$metricName] } else { 0.0 }
            $badValue = if ($badActual.ContainsKey($metricName)) { [double]$badActual[$metricName] } else { 0.0 }
            $actualDelta = [Math]::Abs($badValue - $goodValue)
            $rangeGapRatio = Get-RangeGapRatio $goodExpected[$metricName] $badExpected[$metricName]

            $goodImage = Resolve-RepoPath $good.ImagePath
            $badImage = Resolve-RepoPath $bad.ImagePath
            $delta = [OpenVisionImageDeltaAudit]::Measure($goodImage, $badImage, $ImageSampleStep)
            $flag = Resolve-Flag $rangeGapRatio $actualDelta $delta.MeanAbsDiff $delta.ChangedRatio

            $records.Add([pscustomobject]@{
                PairGroup = $group.Name
                Category = $good.Category
                Metric = $metricName
                GoodSample = $good.SampleName
                BadSample = $bad.SampleName
                GoodActual = $goodValue
                BadActual = $badValue
                GoodRange = Format-Range $goodExpected[$metricName]
                BadRange = Format-Range $badExpected[$metricName]
                RangeGapRatio = if ($null -eq $rangeGapRatio) { $null } else { [Math]::Round($rangeGapRatio, 3) }
                ActualDelta = [Math]::Round($actualDelta, 3)
                MeanAbsDiff = [Math]::Round($delta.MeanAbsDiff, 3)
                ChangedRatio = [Math]::Round($delta.ChangedRatio, 4)
                Flag = $flag
                Recommendation = Resolve-Recommendation $flag
            }) | Out-Null
        }
    }
}

$criticalRecords = @($records | Where-Object { $_.Flag.StartsWith('CRITICAL') })
$reviewRecords = @($records | Where-Object { $_.Flag.StartsWith('REVIEW') })
$okRecords = @($records | Where-Object { $_.Flag -eq 'OK' })

$jsonPath = Join-Path $outputFullDir "product_sample_quality_audit.json"
$reportPath = Join-Path $outputFullDir "product_sample_quality_audit.md"
$records | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $jsonPath -Encoding UTF8

$report = New-Object 'System.Collections.Generic.List[string]'
$report.Add("# Product Sample Quality Audit") | Out-Null
$report.Add("") | Out-Null
$report.Add("This audit reviews the existing Product sample catalog. It does not generate images or run tools; it reads the last product catalog summary and compares Good/Bad pairs by expected metric ranges plus a sampled image-difference heuristic.") | Out-Null
$report.Add("") | Out-Null
$report.Add("- Catalog: " + $CatalogPath) | Out-Null
$report.Add("- Summary: " + $SummaryPath) | Out-Null
$report.Add("- Pair records: $($records.Count)") | Out-Null
$report.Add("- OK: $($okRecords.Count)") | Out-Null
$report.Add("- Review: $($reviewRecords.Count)") | Out-Null
$report.Add("- Critical: $($criticalRecords.Count)") | Out-Null
$report.Add("") | Out-Null
$report.Add("## Review Targets") | Out-Null
$report.Add("") | Out-Null
if ($reviewRecords.Count -eq 0 -and $criticalRecords.Count -eq 0) {
    $report.Add("No pair group needs immediate review by the current numeric heuristic.") | Out-Null
}
else {
    $report.Add("| Flag | PairGroup | Metric | Good Actual | Bad Actual | Good Range | Bad Range | Mean Diff | Change Ratio | Recommendation |") | Out-Null
    $report.Add("| --- | --- | --- | ---: | ---: | --- | --- | ---: | ---: | --- |") | Out-Null
    foreach ($record in @($criticalRecords + $reviewRecords | Select-Object -First 40)) {
        $report.Add("| $($record.Flag) | $($record.PairGroup) | $($record.Metric) | $($record.GoodActual) | $($record.BadActual) | $($record.GoodRange) | $($record.BadRange) | $($record.MeanAbsDiff) | $($record.ChangedRatio) | $($record.Recommendation) |") | Out-Null
    }
}

$report.Add("") | Out-Null
$report.Add("## All Pair Groups") | Out-Null
$report.Add("") | Out-Null
$report.Add("| PairGroup | Metric | Good Actual | Bad Actual | Range Gap | Visual Mean Diff | Visual Change Ratio | Flag |") | Out-Null
$report.Add("| --- | --- | ---: | ---: | ---: | ---: | ---: | --- |") | Out-Null
foreach ($record in $records) {
    $gapText = if ($null -eq $record.RangeGapRatio) { "-" } else { $record.RangeGapRatio }
    $report.Add("| $($record.PairGroup) | $($record.Metric) | $($record.GoodActual) | $($record.BadActual) | $gapText | $($record.MeanAbsDiff) | $($record.ChangedRatio) | $($record.Flag) |") | Out-Null
}

$report | Set-Content -LiteralPath $reportPath -Encoding UTF8

$status = if ($criticalRecords.Count -eq 0) { "PASS" } else { "REVIEW" }
Write-Output "ProductSampleQualityAudit=$status | PairRecords=$($records.Count) OK=$($okRecords.Count) Review=$($reviewRecords.Count) Critical=$($criticalRecords.Count) Report=$reportPath"

if ($FailOnCritical -and $criticalRecords.Count -gt 0) {
    throw "Product sample quality audit found critical metric overlap."
}
