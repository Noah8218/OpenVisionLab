param(
    [string]$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing

# Documentation callouts must be built from current EXE captures.
# Do not point this script back to archived static screenshots.
$tutorialDir = Join-Path $Root 'docs\assets\tutorial'
$sourceDir = Join-Path $tutorialDir 'current'
$outputDir = Join-Path $tutorialDir 'annotated'
New-Item -ItemType Directory -Force -Path $outputDir | Out-Null

function New-Callout {
    param(
        [int]$Number,
        [int]$TargetX,
        [int]$TargetY,
        [int]$LabelX,
        [int]$LabelY,
        [string]$Text
    )

    [PSCustomObject]@{
        Number = $Number
        TargetX = $TargetX
        TargetY = $TargetY
        LabelX = $LabelX
        LabelY = $LabelY
        Text = $Text
    }
}

function New-RoundedRectPath {
    param(
        [System.Drawing.RectangleF]$Rect,
        [float]$Radius
    )

    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $d = $Radius * 2
    $path.AddArc($Rect.X, $Rect.Y, $d, $d, 180, 90)
    $path.AddArc($Rect.Right - $d, $Rect.Y, $d, $d, 270, 90)
    $path.AddArc($Rect.Right - $d, $Rect.Bottom - $d, $d, $d, 0, 90)
    $path.AddArc($Rect.X, $Rect.Bottom - $d, $d, $d, 90, 90)
    $path.CloseFigure()
    return $path
}

function Draw-CalloutImage {
    param(
        [string]$Source,
        [string]$Output,
        [array]$Callouts,
        [int]$OutputHeight = 0
    )

    $sourcePath = Join-Path $sourceDir $Source
    if (-not (Test-Path -LiteralPath $sourcePath)) {
        throw "Missing tutorial image: $sourcePath"
    }

    $inputImage = [System.Drawing.Image]::FromFile($sourcePath)
    try {
        $height = $inputImage.Height
        if ($OutputHeight -gt 0 -and $OutputHeight -lt $inputImage.Height) {
            $height = $OutputHeight
        }

        $canvas = New-Object System.Drawing.Bitmap $inputImage.Width, $height, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $graphics = [System.Drawing.Graphics]::FromImage($canvas)
        try {
            $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
            $graphics.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
            $graphics.DrawImage($inputImage, 0, 0, $inputImage.Width, $inputImage.Height)

            $accent = [System.Drawing.Color]::FromArgb(255, 255, 211, 71)
            $accentDark = [System.Drawing.Color]::FromArgb(255, 20, 34, 45)
            $panel = [System.Drawing.Color]::FromArgb(225, 15, 30, 38)
            $panelBorder = [System.Drawing.Color]::FromArgb(255, 122, 236, 245)

            $numberFont = New-Object System.Drawing.Font 'Malgun Gothic', 18, ([System.Drawing.FontStyle]::Bold)
            $textFont = New-Object System.Drawing.Font 'Malgun Gothic', 13, ([System.Drawing.FontStyle]::Bold)
            $whiteBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::White)
            $darkBrush = New-Object System.Drawing.SolidBrush $accentDark
            $accentBrush = New-Object System.Drawing.SolidBrush $accent
            $panelBrush = New-Object System.Drawing.SolidBrush $panel
            $borderPen = New-Object System.Drawing.Pen $panelBorder, 2
            $arrowPen = New-Object System.Drawing.Pen $accent, 4
            $arrowPen.CustomEndCap = New-Object System.Drawing.Drawing2D.AdjustableArrowCap 7, 8, $true

            foreach ($callout in $Callouts) {
                $labelX = [float]$callout.LabelX
                $labelY = [float]$callout.LabelY
                $targetX = [float]$callout.TargetX
                $targetY = [float]$callout.TargetY
                $labelWidth = [Math]::Max(150, [Math]::Min(260, 74 + ($callout.Text.Length * 13)))
                $labelHeight = 42

                $graphics.DrawLine($arrowPen, $labelX + 24, $labelY + 22, $targetX, $targetY)

                $labelRect = New-Object System.Drawing.RectangleF ($labelX, $labelY, $labelWidth, $labelHeight)
                $labelPath = New-RoundedRectPath -Rect $labelRect -Radius 8
                $graphics.FillPath($panelBrush, $labelPath)
                $graphics.DrawPath($borderPen, $labelPath)
                $labelPath.Dispose()

                $circleRect = New-Object System.Drawing.RectangleF ($labelX + 8), ($labelY + 7), 28, 28
                $graphics.FillEllipse($accentBrush, $circleRect)
                $graphics.DrawEllipse((New-Object System.Drawing.Pen $accentDark, 2), $circleRect)

                $numberText = [string]$callout.Number
                $numberSize = $graphics.MeasureString($numberText, $numberFont)
                $graphics.DrawString($numberText, $numberFont, $darkBrush, $labelX + 22 - ($numberSize.Width / 2), $labelY + 5)
                $graphics.DrawString($callout.Text, $textFont, $whiteBrush, $labelX + 44, $labelY + 9)

                $targetRect = New-Object System.Drawing.RectangleF ($targetX - 10), ($targetY - 10), 20, 20
                $graphics.FillEllipse((New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(80, 255, 211, 71))), $targetRect)
                $graphics.DrawEllipse((New-Object System.Drawing.Pen $accent, 3), $targetRect)
            }

        }
        finally {
            $graphics.Dispose()
        }

        $outputPath = Join-Path $outputDir $Output
        $canvas.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $canvas.Dispose()
        Write-Host "Generated: $outputPath"
    }
    finally {
        $inputImage.Dispose()
    }
}

Draw-CalloutImage -Source 'main_workspace_current.png' -Output 'main_workspace_callouts.png' -Callouts @(
    (New-Callout 1 110 220 250 182 'Tool List'),
    (New-Callout 2 840 111 930 54 'Layer Input'),
    (New-Callout 3 880 390 1030 230 'Image View'),
    (New-Callout 4 1378 111 1220 142 'Run Status'),
    (New-Callout 5 1450 776 1110 632 'Quick Actions')
) -OutputHeight 805

Draw-CalloutImage -Source 'run_log_collapsed_current.png' -Output 'run_log_collapsed_callouts.png' -Callouts @(
    (New-Callout 1 280 848 250 760 'Run Log'),
    (New-Callout 2 560 848 520 760 'Recent Summary'),
    (New-Callout 3 1438 848 1230 760 'Log Count'),
    (New-Callout 4 1530 848 1350 800 'Open Log')
)

Draw-CalloutImage -Source 'run_log_open_current.png' -Output 'run_log_open_callouts.png' -Callouts @(
    (New-Callout 1 280 704 250 620 'Run Log'),
    (New-Callout 2 650 742 560 620 'Recent Events'),
    (New-Callout 3 1440 704 1220 620 'Log Count'),
    (New-Callout 4 1530 704 1350 660 'Close Log')
)

Draw-CalloutImage -Source 'layer_docking_current.png' -Output 'layer_docking_callouts.png' -Callouts @(
    (New-Callout 1 320 173 388 96 'Source Tab'),
    (New-Callout 2 470 173 560 96 'Result Tab'),
    (New-Callout 3 1320 113 1210 148 'Run Status'),
    (New-Callout 4 930 420 1040 650 'Compare View')
)

Draw-CalloutImage -Source 'matching_tool_current.png' -Output 'tool_matching_form_callouts.png' -Callouts @(
    (New-Callout 1 110 704 248 610 'Tool List'),
    (New-Callout 2 1320 280 1150 230 'Layer Route'),
    (New-Callout 3 1260 322 1050 330 'Template Ready'),
    (New-Callout 4 1370 505 1190 468 'PropertyGrid'),
    (New-Callout 5 1280 736 1100 710 'Result Guide'),
    (New-Callout 6 1280 840 1110 806 'Run Preview')
)

Draw-CalloutImage -Source 'blob_tool_current.png' -Output 'tool_blob_form_callouts.png' -Callouts @(
    (New-Callout 1 110 590 246 542 'Blob Tool'),
    (New-Callout 2 1320 280 1150 230 'Layer Route'),
    (New-Callout 3 1188 493 1110 440 'Threshold'),
    (New-Callout 4 1380 635 1210 596 'PropertyGrid'),
    (New-Callout 5 1290 706 1100 682 'Result Metric'),
    (New-Callout 6 1280 840 1110 806 'Run Preview')
)

Draw-CalloutImage -Source 'line_tool_current.png' -Output 'tool_line_form_callouts.png' -Callouts @(
    (New-Callout 1 110 666 246 612 'Line Tool'),
    (New-Callout 2 1160 324 1060 302 'Purpose'),
    (New-Callout 3 1190 358 1060 370 'Line A/B'),
    (New-Callout 4 1170 596 1100 540 'ROI'),
    (New-Callout 5 1290 724 1100 688 'Distance'),
    (New-Callout 6 1280 840 1110 806 'Run Preview')
)

Draw-CalloutImage -Source 'sample_catalog_public_current.png' -Output 'sample_catalog_public_callouts.png' -Callouts @(
    (New-Callout 1 170 160 250 92 'Public Source'),
    (New-Callout 2 170 245 250 205 'Learn Path'),
    (New-Callout 3 320 438 332 610 'Good/Bad List'),
    (New-Callout 4 685 358 760 276 'Preview'),
    (New-Callout 5 735 515 758 585 'Decision Guide'),
    (New-Callout 6 895 653 724 640 'Open Sample')
)

Draw-CalloutImage -Source 'public_matching_diepad_good_result.png' -Output 'public_matching_diepad_good_callouts.png' -Callouts @(
    (New-Callout 1 209 253 34 46 '3 matches'),
    (New-Callout 2 344 263 310 46 'Score 93.1'),
    (New-Callout 3 455 263 322 322 'Centers')
)

Draw-CalloutImage -Source 'public_blob_particles_good_result.png' -Output 'public_blob_particles_good_callouts.png' -Callouts @(
    (New-Callout 1 277 126 38 46 '12 blobs'),
    (New-Callout 2 286 209 326 54 'ROI area'),
    (New-Callout 3 378 262 334 322 'Count OK')
)

Draw-CalloutImage -Source 'public_contour_shapes_good_result.png' -Output 'public_contour_shapes_good_callouts.png' -Callouts @(
    (New-Callout 1 250 126 40 38 '5 shapes'),
    (New-Callout 2 365 234 300 64 'Contour box'),
    (New-Callout 3 141 126 34 318 'Good count')
)

Draw-CalloutImage -Source 'public_threshold_bandpads_good_result.png' -Output 'public_threshold_bandpads_good_callouts.png' -Callouts @(
    (New-Callout 1 158 150 35 46 'Bright pads'),
    (New-Callout 2 359 150 314 44 '4 regions'),
    (New-Callout 3 461 150 300 322 'Count check')
)

Draw-CalloutImage -Source 'public_mean_brightness_good_result.png' -Output 'public_mean_brightness_good_callouts.png' -Callouts @(
    (New-Callout 1 286 211 36 40 'Mean ROI'),
    (New-Callout 2 287 114 352 54 'Normal band'),
    (New-Callout 3 286 210 340 322 'Avg 201.5')
)

Draw-CalloutImage -Source 'public_feature_card_good_result.png' -Output 'public_feature_card_good_callouts.png' -Callouts @(
    (New-Callout 1 267 172 38 44 'Target card'),
    (New-Callout 2 205 192 300 52 'Feature match'),
    (New-Callout 3 329 157 345 318 'Score 96.7')
)

Draw-CalloutImage -Source 'public_edge_fiducial_good_result.png' -Output 'public_edge_fiducial_good_callouts.png' -Callouts @(
    (New-Callout 1 276 207 34 48 'Edge center'),
    (New-Callout 2 276 207 352 64 'L fiducial'),
    (New-Callout 3 221 151 340 322 'Score 99.6')
)

Draw-CalloutImage -Source 'public_line_pins_good_result.png' -Output 'public_line_pins_good_callouts.png' -Callouts @(
    (New-Callout 1 285 190 34 46 'Pin ROI'),
    (New-Callout 2 486 245 306 46 'Scan edges'),
    (New-Callout 3 476 311 300 322 '0.222 mm')
)

Draw-CalloutImage -Source 'pipeline_review_current.png' -Output 'pipeline_matching_review_callouts.png' -Callouts @(
    (New-Callout 1 590 360 430 300 'Step Flow'),
    (New-Callout 2 850 197 650 148 'Guide Strip'),
    (New-Callout 3 1050 330 1040 274 'Input/Output'),
    (New-Callout 4 1380 705 1240 620 'Validation'),
    (New-Callout 5 1210 760 1068 780 'Parameters'),
    (New-Callout 6 1510 122 1340 108 'Run Review')
)
