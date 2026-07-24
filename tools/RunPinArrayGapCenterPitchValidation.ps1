param(
    [string]$OutputDir = "artifacts\p201_pin_center_pitch_20260722\runtime"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$outputRoot = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $OutputDir))
$runnerDll = Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\x64\Debug\net8.0-windows7.0\VisionRecipeRunnerSmoke.dll"
if (-not (Test-Path -LiteralPath $runnerDll)) {
    throw "Build VisionRecipeRunnerSmoke x64 Debug before running this validation."
}

Add-Type -AssemblyName System.Drawing
New-Item -ItemType Directory -Force -Path $outputRoot | Out-Null
$sourceDir = Join-Path $outputRoot "sources"
$resultDir = Join-Path $outputRoot "results"
$logDir = Join-Path $outputRoot "logs"
New-Item -ItemType Directory -Force -Path $sourceDir, $resultDir, $logDir | Out-Null

function New-PinRowImage {
    param(
        [string]$Path,
        [int[]]$Centers,
        [int[]]$Widths,
        [string]$Title
    )

    $bitmap = [System.Drawing.Bitmap]::new(640, 240, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    try {
        $graphics.Clear([System.Drawing.Color]::FromArgb(242, 242, 242))
        $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::None
        $font = [System.Drawing.Font]::new("Arial", 14, [System.Drawing.FontStyle]::Bold)
        try {
            $graphics.DrawString($Title, $font, [System.Drawing.Brushes]::Black, 50, 15)
        }
        finally {
            $font.Dispose()
        }

        for ($index = 0; $index -lt $Centers.Count; $index++) {
            $x = $Centers[$index] - [int]($Widths[$index] / 2)
            $graphics.FillRectangle([System.Drawing.Brushes]::Black, $x, 70, $Widths[$index], 101)
        }

        $graphics.DrawRectangle([System.Drawing.Pens]::DarkGray, 50, 50, 539, 139)
        $bitmap.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    finally {
        $graphics.Dispose()
        $bitmap.Dispose()
    }
}

function New-PipelineXml {
    param(
        [string]$Path,
        [string]$Name,
        [string]$MeasurementMode,
        [bool]$IncludeMeasurementMode,
        [bool]$UsePitchGate
    )

    $modeParameter = if ($IncludeMeasurementMode) {
        "        <Parameter><Key>MeasurementMode</Key><Value>$MeasurementMode</Value></Parameter>"
    }
    else {
        ""
    }
    $acceptance = if ($UsePitchGate) {
@"
      <UseAcceptance>true</UseAcceptance>
      <ExpectedSuccess>true</ExpectedSuccess>
      <MaxElapsedMilliseconds>200</MaxElapsedMilliseconds>
      <AcceptanceMetricName>PitchPxRange</AcceptanceMetricName>
      <UseAcceptanceMetricMaximum>true</UseAcceptanceMetricMaximum>
      <AcceptanceMetricMaximum>2</AcceptanceMetricMaximum>
"@
    }
    else {
        "      <UseAcceptance>false</UseAcceptance>"
    }

    $xml = @"
<?xml version="1.0" encoding="utf-8"?>
<VisionPipeline>
  <Name>$Name</Name>
  <Steps>
    <Step>
      <Name>01_Pin_Row_$MeasurementMode</Name>
      <ToolType>PinArrayGap</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>Pin_Row_Result</OutputLayer>
      <Parameters>
$modeParameter
        <Parameter><Key>USE_ROI</Key><Value>true</Value></Parameter>
        <Parameter><Key>CvROI</Key><Value>50,50,540,140</Value></Parameter>
        <Parameter><Key>DarkThreshold</Key><Value>128</Value></Parameter>
        <Parameter><Key>MinDarkCoverageRatio</Key><Value>0.55</Value></Parameter>
        <Parameter><Key>MinPinWidth</Key><Value>5</Value></Parameter>
        <Parameter><Key>MaxPinBreakWidth</Key><Value>2</Value></Parameter>
        <Parameter><Key>MinGapWidth</Key><Value>3</Value></Parameter>
      </Parameters>
$acceptance
    </Step>
  </Steps>
</VisionPipeline>
"@
    [System.IO.File]::WriteAllText($Path, $xml, [System.Text.UTF8Encoding]::new($false))
}

function Invoke-Runner {
    param(
        [string]$CaseName,
        [string]$ImagePath,
        [string]$XmlPath,
        [int]$ExpectedExitCode
    )

    $resultPath = Join-Path $resultDir ($CaseName + ".png")
    $logPath = Join-Path $logDir ($CaseName + ".txt")
    $startInfo = [System.Diagnostics.ProcessStartInfo]::new()
    $startInfo.FileName = "dotnet"
    $startInfo.Arguments = '"' + $runnerDll + '" "' + $ImagePath + '" "' + $XmlPath + '" "' + $resultPath + '" --overlays'
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $startInfo.UseShellExecute = $false
    $process = [System.Diagnostics.Process]::Start($startInfo)
    $stdout = $process.StandardOutput.ReadToEnd()
    $stderr = $process.StandardError.ReadToEnd()
    $process.WaitForExit()
    $combined = $stdout + $(if ([string]::IsNullOrWhiteSpace($stderr)) { "" } else { [Environment]::NewLine + $stderr })
    [System.IO.File]::WriteAllText($logPath, $combined, [System.Text.UTF8Encoding]::new($false))
    if ($process.ExitCode -ne $ExpectedExitCode) {
        throw "$CaseName returned $($process.ExitCode), expected $ExpectedExitCode. See $logPath"
    }

    return [pscustomobject]@{
        Name = $CaseName
        Log = $combined
        Result = $resultPath
        ExitCode = $process.ExitCode
    }
}

function Assert-Contains {
    param([string]$Text, [string]$Pattern, [string]$Message)
    if ($Text -notmatch $Pattern) {
        throw "$Message Pattern '$Pattern' was not found."
    }
}

$centersUniform = @(100, 160, 220, 280, 340, 400, 460, 520)
$centersShifted = @(100, 160, 220, 280, 340, 412, 472, 532)
$uniformWidths = @(20, 20, 20, 20, 20, 20, 20, 20)
$variedWidths = @(12, 12, 28, 28, 16, 24, 10, 30)

$uniformPath = Join-Path $sourceDir "01_uniform_width_pitch60.png"
$variedPath = Join-Path $sourceDir "02_varied_width_same_pitch60.png"
$shiftedPath = Join-Path $sourceDir "03_shifted_center_pitch_range12.png"
New-PinRowImage $uniformPath $centersUniform $uniformWidths "Uniform width | center pitch 60 px"
New-PinRowImage $variedPath $centersUniform $variedWidths "Varied width | same center pitch 60 px"
New-PinRowImage $shiftedPath $centersShifted $variedWidths "Shifted center | pitch range 12 px"

$pitchXml = Join-Path $outputRoot "pin_center_pitch_judged.xml"
$edgeDefaultXml = Join-Path $outputRoot "pin_edge_gap_legacy_default.xml"
$edgeExplicitXml = Join-Path $outputRoot "pin_edge_gap_explicit.xml"
New-PipelineXml $pitchXml "P201_Pin_Center_Pitch" "CenterPitch" $true $true
New-PipelineXml $edgeDefaultXml "P201_Pin_EdgeGap_Legacy_Default" "EdgeGap" $false $false
New-PipelineXml $edgeExplicitXml "P201_Pin_EdgeGap_Explicit" "EdgeGap" $true $false

$runs = @()
$runs += Invoke-Runner "01_pitch_uniform_pass" $uniformPath $pitchXml 0
$runs += Invoke-Runner "02_pitch_varied_width_pass" $variedPath $pitchXml 0
$runs += Invoke-Runner "03_pitch_shifted_fail" $shiftedPath $pitchXml 1
$runs += Invoke-Runner "04_edge_legacy_default" $uniformPath $edgeDefaultXml 0
$runs += Invoke-Runner "05_edge_explicit" $uniformPath $edgeExplicitXml 0
$runs += Invoke-Runner "06_edge_varied_width" $variedPath $edgeExplicitXml 0

Assert-Contains $runs[0].Log "PitchPxAvg=60(?:\D|$)" "Uniform pitch average was not 60 px."
Assert-Contains $runs[0].Log "PitchPxRange=0(?:\D|$)" "Uniform pitch range was not 0 px."
Assert-Contains $runs[1].Log "PitchPxAvg=60(?:\D|$)" "Varied-width pitch average was not 60 px."
Assert-Contains $runs[1].Log "PitchPxRange=0(?:\D|$)" "Varied-width pitch range was not 0 px."
Assert-Contains $runs[2].Log "PitchPxRange=12(?:\D|$)" "Shifted-center pitch range was not 12 px."
Assert-Contains $runs[2].Log "Success=False" "Shifted-center judged case did not fail its acceptance gate."
Assert-Contains $runs[5].Log "DistancePxRange=[1-9]" "Varied-width edge-gap control did not expose a non-zero edge-gap range."

$legacyHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $runs[3].Result).Hash
$explicitHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $runs[4].Result).Hash
if ($legacyHash -ne $explicitHash) {
    throw "Legacy missing-MeasurementMode drawing differs from explicit EdgeGap drawing."
}

$batchListPath = Join-Path $outputRoot "matrix_images.txt"
$batchCsvPath = Join-Path $outputRoot "matrix_batch.csv"
$batchLogPath = Join-Path $logDir "matrix_batch.txt"
[System.IO.File]::WriteAllLines($batchListPath, @($uniformPath, $variedPath, $shiftedPath), [System.Text.UTF8Encoding]::new($false))
$batchOutput = & dotnet $runnerDll --batch $batchListPath $sourceDir $pitchXml $batchCsvPath 2>&1
$batchExitCode = $LASTEXITCODE
[System.IO.File]::WriteAllLines($batchLogPath, [string[]]$batchOutput, [System.Text.UTF8Encoding]::new($false))
if ($batchExitCode -ne 0) {
    throw "Pitch batch reporter returned $batchExitCode. See $batchLogPath"
}

$batchRows = @(Import-Csv -LiteralPath $batchCsvPath)
if ($batchRows.Count -ne 3) {
    throw "Pitch batch CSV did not retain exactly three rows."
}
if (([double]$batchRows[0].PitchPxAvg) -ne 60 -or ([double]$batchRows[0].PitchPxRange) -ne 0) {
    throw "Pitch batch CSV did not preserve the uniform-row metrics."
}
if (([double]$batchRows[1].PitchPxAvg) -ne 60 -or ([double]$batchRows[1].PitchPxRange) -ne 0) {
    throw "Pitch batch CSV did not preserve the varied-width same-center metrics."
}
if (([double]$batchRows[2].PitchPxRange) -ne 12) {
    throw "Pitch batch CSV did not preserve the three expected PitchPx metric rows."
}

$contactSheetPath = Join-Path $outputRoot "p201_pin_center_pitch_runtime_contact_sheet.png"
$sheet = [System.Drawing.Bitmap]::new(1280, 720, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
$sheetGraphics = [System.Drawing.Graphics]::FromImage($sheet)
try {
    $sheetGraphics.Clear([System.Drawing.Color]::FromArgb(24, 24, 24))
    for ($row = 0; $row -lt 3; $row++) {
        $sourceImage = [System.Drawing.Image]::FromFile(@($uniformPath, $variedPath, $shiftedPath)[$row])
        $resultImage = [System.Drawing.Image]::FromFile($runs[$row].Result)
        try {
            $sheetGraphics.DrawImage($sourceImage, 0, $row * 240, 640, 240)
            $sheetGraphics.DrawImage($resultImage, 640, $row * 240, 640, 240)
        }
        finally {
            $sourceImage.Dispose()
            $resultImage.Dispose()
        }
    }
    $sheet.Save($contactSheetPath, [System.Drawing.Imaging.ImageFormat]::Png)
}
finally {
    $sheetGraphics.Dispose()
    $sheet.Dispose()
}

$summary = @"
Status: Complete
Scope: PinArrayGap CenterPitch pixel-only semantic matrix and legacy EdgeGap default regression.
Uniform pitch: PitchPxAvg=60, PitchPxRange=0, accepted.
Varied widths with same centers: PitchPxAvg=60, PitchPxRange=0, accepted.
Shifted center: PitchPxRange=12, rejected by PitchPxRange <= 2.
Batch report: three rows retained PitchPxMin/Max/Avg/Range in matrix_batch.csv.
Legacy default: missing MeasurementMode is drawing-identical to explicit EdgeGap (SHA-256 $legacyHash).
Boundary: dark vertical pins in one reviewed row ROI; no bright polarity, calibrated units, or field robustness claim.
"@
[System.IO.File]::WriteAllText((Join-Path $outputRoot "SUMMARY.txt"), $summary, [System.Text.UTF8Encoding]::new($false))
Write-Output "PinArrayGap CenterPitch validation passed."
Write-Output "Evidence=$outputRoot"
Write-Output "ContactSheet=$contactSheetPath"
