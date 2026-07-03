param(
    [string]$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing

$publicDir = Join-Path $Root 'docs\samples\public'
$templateDir = Join-Path $publicDir 'templates'
New-Item -ItemType Directory -Force -Path $templateDir | Out-Null

function New-Brush([int]$gray) {
    return [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb($gray, $gray, $gray))
}

function New-Pen([int]$gray, [float]$width = 1.0) {
    return [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb($gray, $gray, $gray), $width)
}

function Add-Noise([System.Drawing.Bitmap]$Bitmap, [int]$Seed, [int]$Strength) {
    $random = [System.Random]::new($Seed)
    for ($y = 0; $y -lt $Bitmap.Height; $y++) {
        for ($x = 0; $x -lt $Bitmap.Width; $x++) {
            $c = $Bitmap.GetPixel($x, $y)
            $delta = $random.Next(-$Strength, $Strength + 1)
            $v = [Math]::Max(0, [Math]::Min(255, $c.R + $delta))
            $Bitmap.SetPixel($x, $y, [System.Drawing.Color]::FromArgb($v, $v, $v))
        }
    }
}

function Draw-TargetPad(
    [System.Drawing.Graphics]$Graphics,
    [float]$X,
    [float]$Y,
    [float]$AngleDegrees
) {
    $state = $Graphics.Save()
    $Graphics.TranslateTransform($X + 45, $Y + 37)
    $Graphics.RotateTransform($AngleDegrees)
    $Graphics.TranslateTransform(-45, -37)

    $outline = New-Pen 232 3
    $inner = New-Pen 104 2
    $bright = New-Brush 185
    $mid = New-Brush 118
    $dark = New-Brush 54

    $Graphics.FillRectangle($mid, 7, 6, 76, 62)
    $Graphics.DrawRectangle($outline, 7, 6, 76, 62)
    $Graphics.FillRectangle($bright, 22, 17, 42, 32)
    $Graphics.DrawRectangle($inner, 22, 17, 42, 32)
    $Graphics.FillEllipse($dark, 38, 24, 13, 19)
    $Graphics.FillEllipse($mid, 42, 28, 5, 7)

    $outline.Dispose()
    $inner.Dispose()
    $bright.Dispose()
    $mid.Dispose()
    $dark.Dispose()
    $Graphics.Restore($state)
}

function Save-MatchingDiePadSyntheticSample {
    $imagePath = Join-Path $publicDir 'Matching_DiePad_Synthetic_OK.png'
    $templatePath = Join-Path $templateDir 'Matching_DiePad_Synthetic_Template.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic

    $background = New-Brush 58
    $chipBody = New-Brush 96
    $trace = New-Pen 155 2
    $brightTrace = New-Pen 198 2
    $edge = New-Pen 188 3

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($chipBody, 88, 0, 365, 300)
    $graphics.DrawRectangle($edge, 92, 20, 320, 255)

    for ($i = 0; $i -lt 8; $i++) {
        $y = 45 + ($i * 15)
        $graphics.DrawLine($trace, 105, $y, 300, $y + 3)
        $graphics.DrawLine($brightTrace, 142, $y + 5, 360, $y + 8)
    }

    $graphics.DrawLine($edge, 245, 0, 245, 190)
    $graphics.DrawLine($edge, 332, 15, 332, 230)
    $graphics.DrawLine($trace, 365, 24, 420, 24)
    $graphics.DrawLine($trace, 365, 42, 420, 42)
    $graphics.DrawLine($trace, 365, 60, 420, 60)

    Draw-TargetPad $graphics 74 205 -6
    Draw-TargetPad $graphics 165 215 0
    Draw-TargetPad $graphics 300 226 0
    Draw-TargetPad $graphics 410 226 0

    $graphics.Dispose()
    $background.Dispose()
    $chipBody.Dispose()
    $trace.Dispose()
    $brightTrace.Dispose()
    $edge.Dispose()

    Add-Noise $bitmap 2107 5
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)

    $template = $bitmap.Clone(
        [System.Drawing.Rectangle]::new(300, 226, 90, 75),
        [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $template.Save($templatePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $template.Dispose()
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
    Write-Host "Generated: $templatePath"
}

function Save-MatchingDiePadNoTargetSyntheticSample {
    $imagePath = Join-Path $publicDir 'Matching_DiePad_Synthetic_NoTarget_NG.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic

    $background = New-Brush 58
    $chipBody = New-Brush 96
    $trace = New-Pen 155 2
    $brightTrace = New-Pen 198 2
    $edge = New-Pen 188 3
    $distractor = New-Brush 126
    $distractorDark = New-Brush 68

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($chipBody, 88, 0, 365, 300)
    $graphics.DrawRectangle($edge, 92, 20, 320, 255)

    for ($i = 0; $i -lt 8; $i++) {
        $y = 45 + ($i * 15)
        $graphics.DrawLine($trace, 105, $y, 300, $y + 3)
        $graphics.DrawLine($brightTrace, 142, $y + 5, 360, $y + 8)
    }

    $graphics.DrawLine($edge, 245, 0, 245, 190)
    $graphics.DrawLine($edge, 332, 15, 332, 230)
    $graphics.DrawLine($trace, 365, 24, 420, 24)
    $graphics.DrawLine($trace, 365, 42, 420, 42)
    $graphics.DrawLine($trace, 365, 60, 420, 60)

    $graphics.FillEllipse($distractor, 104, 222, 58, 42)
    $graphics.FillEllipse($distractorDark, 122, 234, 18, 14)
    $graphics.FillRectangle($distractor, 242, 220, 62, 28)
    $graphics.FillRectangle($distractorDark, 258, 228, 20, 10)
    $graphics.FillEllipse($distractor, 388, 226, 72, 38)
    $graphics.FillEllipse($distractorDark, 414, 238, 18, 10)

    $graphics.Dispose()
    $background.Dispose()
    $chipBody.Dispose()
    $trace.Dispose()
    $brightTrace.Dispose()
    $edge.Dispose()
    $distractor.Dispose()
    $distractorDark.Dispose()

    Add-Noise $bitmap 2108 5
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-WorkspaceSyntheticSample {
    $imagePath = Join-Path $publicDir 'Workspace_Inspection_Synthetic_OK.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 48
    $body = New-Brush 91
    $rim = New-Pen 178 3
    $trace = New-Pen 132 2
    $bright = New-Pen 210 2
    $okBrush = New-Brush 185
    $warnBrush = New-Brush 122

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($body, 78, 40, 420, 270)
    $graphics.DrawRectangle($rim, 82, 44, 412, 262)

    for ($i = 0; $i -lt 9; $i++) {
        $x = 112 + ($i * 37)
        $graphics.DrawLine($trace, $x, 62, $x + 12, 214)
        $graphics.DrawLine($bright, $x + 8, 76, $x + 20, 228)
    }

    for ($i = 0; $i -lt 5; $i++) {
        $x = 105 + ($i * 78)
        $graphics.FillRectangle($okBrush, $x, 255, 46, 38)
        $graphics.DrawRectangle($rim, $x, 255, 46, 38)
    }

    $graphics.FillEllipse($warnBrush, 395, 258, 42, 32)
    $graphics.DrawEllipse($rim, 395, 258, 42, 32)

    $graphics.Dispose()
    $background.Dispose()
    $body.Dispose()
    $rim.Dispose()
    $trace.Dispose()
    $bright.Dispose()
    $okBrush.Dispose()
    $warnBrush.Dispose()

    Add-Noise $bitmap 4109 4
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-BlobParticlesSyntheticSample {
    $imagePath = Join-Path $publicDir 'Blob_Particles_Synthetic_OK.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 38
    $well = New-Brush 72
    $rim = New-Pen 150 3
    $particle = New-Brush 214
    $particleDim = New-Brush 178

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillEllipse($well, 70, 55, 430, 300)
    $graphics.DrawEllipse($rim, 70, 55, 430, 300)

    $particles = @(
        @(145, 116, 28), @(198, 150, 22), @(260, 108, 34), @(329, 142, 26),
        @(390, 105, 31), @(434, 190, 25), @(360, 244, 34), @(292, 220, 23),
        @(223, 257, 31), @(158, 221, 26), @(116, 178, 21), @(278, 300, 27)
    )

    foreach ($p in $particles) {
        $brush = if ($p[2] -gt 27) { $particle } else { $particleDim }
        $graphics.FillEllipse($brush, $p[0], $p[1], $p[2], $p[2])
    }

    $graphics.Dispose()
    $background.Dispose()
    $well.Dispose()
    $rim.Dispose()
    $particle.Dispose()
    $particleDim.Dispose()

    Add-Noise $bitmap 7223 6
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-BlobParticlesSparseSyntheticSample {
    $imagePath = Join-Path $publicDir 'Blob_Particles_Synthetic_Sparse_NG.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 38
    $well = New-Brush 72
    $rim = New-Pen 150 3
    $particle = New-Brush 214

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillEllipse($well, 70, 55, 430, 300)
    $graphics.DrawEllipse($rim, 70, 55, 430, 300)

    $particles = @(
        @(175, 142, 29), @(302, 218, 33), @(421, 128, 26)
    )

    foreach ($p in $particles) {
        $graphics.FillEllipse($particle, $p[0], $p[1], $p[2], $p[2])
    }

    $graphics.Dispose()
    $background.Dispose()
    $well.Dispose()
    $rim.Dispose()
    $particle.Dispose()

    Add-Noise $bitmap 7224 6
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-ContourShapesSyntheticSample {
    $imagePath = Join-Path $publicDir 'Contour_Shapes_Synthetic_OK.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 44
    $panel = New-Brush 78
    $shape = New-Brush 218
    $dimShape = New-Brush 196
    $rim = New-Pen 138 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($panel, 58, 48, 456, 304)
    $graphics.DrawRectangle($rim, 58, 48, 456, 304)

    $graphics.FillRectangle($shape, 112, 104, 58, 42)
    $graphics.FillEllipse($dimShape, 222, 96, 54, 54)
    $graphics.FillRectangle($shape, 340, 102, 72, 36)
    $graphics.FillPolygon($shape, [System.Drawing.Point[]]@(
        [System.Drawing.Point]::new(150, 260),
        [System.Drawing.Point]::new(190, 194),
        [System.Drawing.Point]::new(230, 260)
    ))
    $graphics.FillEllipse($shape, 330, 210, 66, 46)

    $graphics.Dispose()
    $background.Dispose()
    $panel.Dispose()
    $shape.Dispose()
    $dimShape.Dispose()
    $rim.Dispose()

    Add-Noise $bitmap 6241 3
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-ContourShapesMissingSyntheticSample {
    $imagePath = Join-Path $publicDir 'Contour_Shapes_Synthetic_Missing_NG.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 44
    $panel = New-Brush 78
    $shape = New-Brush 218
    $rim = New-Pen 138 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($panel, 58, 48, 456, 304)
    $graphics.DrawRectangle($rim, 58, 48, 456, 304)

    $graphics.FillRectangle($shape, 120, 112, 68, 48)
    $graphics.FillEllipse($shape, 324, 210, 72, 50)

    $graphics.Dispose()
    $background.Dispose()
    $panel.Dispose()
    $shape.Dispose()
    $rim.Dispose()

    Add-Noise $bitmap 6242 3
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-ThresholdBandSyntheticSample {
    $imagePath = Join-Path $publicDir 'Threshold_BandPads_Synthetic_OK.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 50
    $tray = New-Brush 82
    $target = New-Brush 178
    $darkDistractor = New-Brush 96
    $midDistractor = New-Brush 112
    $rim = New-Pen 118 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($tray, 70, 65, 430, 260)
    $graphics.DrawRectangle($rim, 70, 65, 430, 260)

    $targets = @(
        @(132, 130), @(232, 132), @(334, 130), @(432, 132)
    )

    foreach ($p in $targets) {
        $graphics.FillRectangle($target, $p[0], $p[1], 50, 38)
        $graphics.DrawRectangle($rim, $p[0], $p[1], 50, 38)
    }

    $graphics.FillRectangle($darkDistractor, 168, 226, 50, 34)
    $graphics.FillRectangle($midDistractor, 286, 226, 50, 34)
    $graphics.FillRectangle($darkDistractor, 404, 226, 50, 34)

    $graphics.Dispose()
    $background.Dispose()
    $tray.Dispose()
    $target.Dispose()
    $darkDistractor.Dispose()
    $midDistractor.Dispose()
    $rim.Dispose()

    Add-Noise $bitmap 8151 3
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-ThresholdBandMissingSyntheticSample {
    $imagePath = Join-Path $publicDir 'Threshold_BandPads_Synthetic_Missing_NG.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 50
    $tray = New-Brush 82
    $target = New-Brush 178
    $darkDistractor = New-Brush 96
    $midDistractor = New-Brush 112
    $rim = New-Pen 118 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($tray, 70, 65, 430, 260)
    $graphics.DrawRectangle($rim, 70, 65, 430, 260)

    $graphics.FillRectangle($target, 132, 130, 50, 38)
    $graphics.DrawRectangle($rim, 132, 130, 50, 38)

    $distractors = @(
        @(232, 132, $darkDistractor), @(334, 130, $midDistractor), @(432, 132, $darkDistractor),
        @(168, 226, $midDistractor), @(286, 226, $darkDistractor), @(404, 226, $midDistractor)
    )

    foreach ($p in $distractors) {
        $graphics.FillRectangle($p[2], $p[0], $p[1], 50, 38)
        $graphics.DrawRectangle($rim, $p[0], $p[1], 50, 38)
    }

    $graphics.Dispose()
    $background.Dispose()
    $tray.Dispose()
    $target.Dispose()
    $darkDistractor.Dispose()
    $midDistractor.Dispose()
    $rim.Dispose()

    Add-Noise $bitmap 8152 3
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-MeanBrightnessSyntheticSample {
    $imagePath = Join-Path $publicDir 'Mean_Brightness_Synthetic_OK.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 198
    $panel = New-Brush 214
    $reference = New-Brush 184
    $rim = New-Pen 162 2
    $mark = New-Pen 235 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($panel, 74, 66, 424, 260)
    $graphics.DrawRectangle($rim, 74, 66, 424, 260)

    for ($i = 0; $i -lt 7; $i++) {
        $x = 116 + ($i * 48)
        $graphics.FillRectangle($reference, $x, 128, 28, 112)
        $graphics.DrawRectangle($rim, $x, 128, 28, 112)
        $graphics.DrawLine($mark, $x + 8, 112, $x + 8, 255)
    }

    $graphics.Dispose()
    $background.Dispose()
    $panel.Dispose()
    $reference.Dispose()
    $rim.Dispose()
    $mark.Dispose()

    Add-Noise $bitmap 5231 2
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-MeanBrightnessDarkSyntheticSample {
    $imagePath = Join-Path $publicDir 'Mean_Brightness_Synthetic_Dark_NG.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 116
    $panel = New-Brush 126
    $reference = New-Brush 98
    $rim = New-Pen 92 2
    $mark = New-Pen 146 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($panel, 74, 66, 424, 260)
    $graphics.DrawRectangle($rim, 74, 66, 424, 260)

    for ($i = 0; $i -lt 7; $i++) {
        $x = 116 + ($i * 48)
        $graphics.FillRectangle($reference, $x, 128, 28, 112)
        $graphics.DrawRectangle($rim, $x, 128, 28, 112)
        $graphics.DrawLine($mark, $x + 8, 112, $x + 8, 255)
    }

    $graphics.Dispose()
    $background.Dispose()
    $panel.Dispose()
    $reference.Dispose()
    $rim.Dispose()
    $mark.Dispose()

    Add-Noise $bitmap 5232 2
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Draw-FeatureCard(
    [System.Drawing.Graphics]$Graphics,
    [int]$X,
    [int]$Y,
    [string]$Label,
    [string]$SubLabel,
    [int]$Seed,
    [bool]$TargetLayout
) {
    $panel = New-Brush 246
    $dark = New-Brush 32
    $blue = New-Brush 92
    $teal = New-Brush 134
    $gray = New-Brush 176
    $outline = New-Pen 34 3
    $bluePen = New-Pen 92 3
    $grayPen = New-Pen 128 1

    $titleFont = [System.Drawing.Font]::new("Segoe UI", 24, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $smallFont = [System.Drawing.Font]::new("Consolas", 16, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)

    $Graphics.FillRectangle($panel, $X, $Y, 220, 160)
    $Graphics.DrawRectangle($outline, $X, $Y, 220, 160)
    $Graphics.DrawString($Label, $titleFont, $dark, $X + 18, $Y + 16)
    $Graphics.DrawString($SubLabel, $smallFont, $blue, $X + 140, $Y + 22)

    if ($TargetLayout) {
        $Graphics.DrawLine($bluePen, $X + 18, $Y + 112, $X + 198, $Y + 34)
        $Graphics.DrawEllipse($outline, $X + 28, $Y + 82, 38, 38)
        $Graphics.FillEllipse($teal, $X + 82, $Y + 78, 22, 22)
        $Graphics.FillRectangle($dark, $X + 154, $Y + 88, 38, 28)
        for ($gx = $X + 12; $gx -lt $X + 208; $gx += 20) {
            $Graphics.DrawLine($grayPen, $gx, $Y + 132, $gx + 9, $Y + 150)
        }
    }
    else {
        $Graphics.DrawLine($bluePen, $X + 24, $Y + 34, $X + 182, $Y + 116)
        $Graphics.FillRectangle($teal, $X + 30, $Y + 86, 42, 28)
        $Graphics.DrawEllipse($outline, $X + 118, $Y + 76, 46, 32)
        $Graphics.FillEllipse($gray, $X + 168, $Y + 104, 24, 24)
        for ($gy = $Y + 126; $gy -lt $Y + 152; $gy += 8) {
            $Graphics.DrawLine($grayPen, $X + 18, $gy, $X + 202, $gy + 3)
        }
    }

    $random = [System.Random]::new($Seed)
    for ($i = 0; $i -lt 44; $i++) {
        $dotX = $random.Next($X + 10, $X + 206)
        $dotY = $random.Next($Y + 10, $Y + 146)
        $size = 3 + ($i % 4)
        $brush = if ($i % 3 -eq 0) { $dark } elseif ($i % 3 -eq 1) { $blue } else { $teal }
        $Graphics.FillEllipse($brush, $dotX, $dotY, $size, $size)
    }

    $panel.Dispose()
    $dark.Dispose()
    $blue.Dispose()
    $teal.Dispose()
    $gray.Dispose()
    $outline.Dispose()
    $bluePen.Dispose()
    $grayPen.Dispose()
    $titleFont.Dispose()
    $smallFont.Dispose()
}

function Save-FeatureCardSyntheticSample {
    $imagePath = Join-Path $publicDir 'Feature_Card_Synthetic_OK.png'
    $templatePath = Join-Path $templateDir 'Feature_Card_Synthetic_Template.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic

    $background = New-Brush 224
    $rail = New-Pen 160 2
    $soft = New-Brush 205

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($soft, 44, 48, 484, 312)
    $graphics.DrawLine($rail, 36, 70, 536, 70)
    $graphics.DrawLine($rail, 36, 348, 536, 348)
    Draw-FeatureCard $graphics 156 92 "F7" "SIFT" 1701 $true

    $graphics.Dispose()
    $background.Dispose()
    $rail.Dispose()
    $soft.Dispose()

    Add-Noise $bitmap 1709 2
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)

    $template = $bitmap.Clone(
        [System.Drawing.Rectangle]::new(156, 92, 220, 160),
        [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $template.Save($templatePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $template.Dispose()
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
    Write-Host "Generated: $templatePath"
}

function Save-FeatureCardWrongSyntheticSample {
    $imagePath = Join-Path $publicDir 'Feature_Card_Synthetic_Wrong_NG.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic

    $background = New-Brush 224
    $rail = New-Pen 160 2
    $soft = New-Brush 205

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($soft, 44, 48, 484, 312)
    $graphics.DrawLine($rail, 36, 70, 536, 70)
    $graphics.DrawLine($rail, 36, 348, 536, 348)
    Draw-FeatureCard $graphics 156 92 "B2" "ORB" 1903 $false

    $graphics.Dispose()
    $background.Dispose()
    $rail.Dispose()
    $soft.Dispose()

    Add-Noise $bitmap 1710 2
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Draw-EdgeFiducial(
    [System.Drawing.Graphics]$Graphics,
    [int]$X,
    [int]$Y,
    [bool]$TargetLayout
) {
    $light = New-Brush 224
    $mid = New-Brush 138
    $dark = New-Brush 42
    $outline = New-Pen 236 4
    $thin = New-Pen 118 2

    if ($TargetLayout) {
        $Graphics.FillRectangle($light, $X + 12, $Y + 12, 22, 86)
        $Graphics.FillRectangle($light, $X + 12, $Y + 76, 84, 22)
        $Graphics.FillRectangle($dark, $X + 34, $Y + 34, 28, 20)
        $Graphics.FillEllipse($mid, $X + 66, $Y + 20, 18, 18)
        $Graphics.DrawLine($outline, $X + 48, $Y + 66, $X + 94, $Y + 22)
        $Graphics.DrawRectangle($thin, $X + 7, $Y + 7, 94, 96)
    }
    else {
        $Graphics.FillRectangle($light, $X + 44, $Y + 12, 22, 86)
        $Graphics.FillRectangle($light, $X + 12, $Y + 12, 86, 22)
        $Graphics.FillEllipse($mid, $X + 22, $Y + 56, 26, 26)
        $Graphics.FillRectangle($dark, $X + 68, $Y + 58, 18, 30)
        $Graphics.DrawLine($outline, $X + 16, $Y + 96, $X + 92, $Y + 52)
        $Graphics.DrawRectangle($thin, $X + 7, $Y + 7, 94, 96)
    }

    $light.Dispose()
    $mid.Dispose()
    $dark.Dispose()
    $outline.Dispose()
    $thin.Dispose()
}

function Save-EdgeFiducialSyntheticSample {
    $imagePath = Join-Path $publicDir 'Edge_Fiducial_Synthetic_OK.png'
    $templatePath = Join-Path $templateDir 'Edge_Fiducial_Synthetic_Template.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic

    $background = New-Brush 48
    $panel = New-Brush 88
    $trace = New-Pen 126 2
    $soft = New-Pen 104 1

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($panel, 70, 58, 430, 285)
    for ($i = 0; $i -lt 8; $i++) {
        $y = 90 + ($i * 24)
        $graphics.DrawLine($soft, 92, $y, 470, $y + 5)
    }
    $graphics.DrawLine($trace, 102, 316, 470, 86)
    $graphics.DrawLine($trace, 120, 84, 488, 316)

    Draw-EdgeFiducial $graphics 220 150 $true

    $graphics.Dispose()
    $background.Dispose()
    $panel.Dispose()
    $trace.Dispose()
    $soft.Dispose()

    Add-Noise $bitmap 8401 3
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)

    $template = $bitmap.Clone(
        [System.Drawing.Rectangle]::new(220, 150, 112, 112),
        [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $template.Save($templatePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $template.Dispose()
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
    Write-Host "Generated: $templatePath"
}

function Save-EdgeFiducialWrongSyntheticSample {
    $imagePath = Join-Path $publicDir 'Edge_Fiducial_Synthetic_Wrong_NG.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic

    $background = New-Brush 48
    $panel = New-Brush 88
    $trace = New-Pen 126 2
    $soft = New-Pen 104 1

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($panel, 70, 58, 430, 285)
    for ($i = 0; $i -lt 8; $i++) {
        $y = 90 + ($i * 24)
        $graphics.DrawLine($soft, 92, $y, 470, $y + 5)
    }
    $graphics.DrawLine($trace, 102, 316, 470, 86)
    $graphics.DrawLine($trace, 120, 84, 488, 316)

    Draw-EdgeFiducial $graphics 220 150 $false

    $graphics.Dispose()
    $background.Dispose()
    $panel.Dispose()
    $trace.Dispose()
    $soft.Dispose()

    Add-Noise $bitmap 8402 3
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-LinePinsSyntheticSample {
    $imagePath = Join-Path $publicDir 'Line_Pins_Synthetic_OK.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 45
    $package = New-Brush 96
    $pin = New-Brush 214
    $shadow = New-Brush 70
    $edge = New-Pen 178 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($package, 70, 58, 430, 270)
    $graphics.DrawRectangle($edge, 70, 58, 430, 270)

    for ($i = 0; $i -lt 8; $i++) {
        $x = 104 + ($i * 48)
        $graphics.FillRectangle($shadow, $x + 6, 160, 24, 150)
        $graphics.FillRectangle($pin, $x, 152, 24, 150)
        $graphics.DrawRectangle($edge, $x, 152, 24, 150)
    }

    $graphics.DrawLine($edge, 60, 132, 510, 132)
    $graphics.DrawLine($edge, 60, 322, 510, 322)

    $graphics.Dispose()
    $background.Dispose()
    $package.Dispose()
    $pin.Dispose()
    $shadow.Dispose()
    $edge.Dispose()

    Add-Noise $bitmap 9311 5
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-LinePinsWidePinSyntheticSample {
    $imagePath = Join-Path $publicDir 'Line_Pins_Synthetic_WidePin_NG.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 45
    $package = New-Brush 96
    $pin = New-Brush 214
    $shadow = New-Brush 70
    $edge = New-Pen 178 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($package, 70, 58, 430, 270)
    $graphics.DrawRectangle($edge, 70, 58, 430, 270)

    for ($i = 0; $i -lt 8; $i++) {
        $x = 90 + ($i * 58)
        $graphics.FillRectangle($shadow, $x + 8, 160, 42, 150)
        $graphics.FillRectangle($pin, $x, 152, 42, 150)
        $graphics.DrawRectangle($edge, $x, 152, 42, 150)
    }

    $graphics.DrawLine($edge, 60, 132, 510, 132)
    $graphics.DrawLine($edge, 60, 322, 510, 322)

    $graphics.Dispose()
    $background.Dispose()
    $package.Dispose()
    $pin.Dispose()
    $shadow.Dispose()
    $edge.Dispose()

    Add-Noise $bitmap 9312 5
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-Manifest {
    $manifestPath = Join-Path $publicDir 'OpenVisionLab.PublicSampleManifest.csv'
    $rows = @(
        'AssetPath,SourceType,SourceName,SourceUrl,License,Attribution,GeneratedBy,Notes',
        'docs/samples/public/Workspace_Inspection_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated workspace overview sample for public tutorial captures',
        'docs/samples/public/Matching_DiePad_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated grayscale matching source image',
        'docs/samples/public/Matching_DiePad_Synthetic_NoTarget_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated matching no-target negative sample',
        'docs/samples/public/templates/Matching_DiePad_Synthetic_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated template crop from Matching_DiePad_Synthetic_OK.png',
        'docs/samples/public/Blob_Particles_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated blob particle source image',
        'docs/samples/public/Blob_Particles_Synthetic_Sparse_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated sparse blob negative sample',
        'docs/samples/public/Contour_Shapes_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated contour shape-count source image',
        'docs/samples/public/Contour_Shapes_Synthetic_Missing_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated contour missing-shape negative sample',
        'docs/samples/public/Threshold_BandPads_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated basic threshold source image',
        'docs/samples/public/Threshold_BandPads_Synthetic_Missing_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated basic threshold missing-pad negative sample',
        'docs/samples/public/Mean_Brightness_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated mean brightness normal sample',
        'docs/samples/public/Mean_Brightness_Synthetic_Dark_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated mean brightness dark drift negative sample',
        'docs/samples/public/Feature_Card_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated feature matching source image',
        'docs/samples/public/Feature_Card_Synthetic_Wrong_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated feature matching wrong-target negative sample',
        'docs/samples/public/templates/Feature_Card_Synthetic_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated template crop from Feature_Card_Synthetic_OK.png',
        'docs/samples/public/Edge_Fiducial_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated edge-based matching source image',
        'docs/samples/public/Edge_Fiducial_Synthetic_Wrong_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated edge-based matching wrong-target negative sample',
        'docs/samples/public/templates/Edge_Fiducial_Synthetic_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated template crop from Edge_Fiducial_Synthetic_OK.png',
        'docs/samples/public/Line_Pins_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated line gauge pin source image',
        'docs/samples/public/Line_Pins_Synthetic_WidePin_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated line width drift negative sample'
    )

    Set-Content -LiteralPath $manifestPath -Value $rows -Encoding UTF8
    Write-Host "Generated: $manifestPath"
}

Save-WorkspaceSyntheticSample
Save-MatchingDiePadSyntheticSample
Save-MatchingDiePadNoTargetSyntheticSample
Save-BlobParticlesSyntheticSample
Save-BlobParticlesSparseSyntheticSample
Save-ContourShapesSyntheticSample
Save-ContourShapesMissingSyntheticSample
Save-ThresholdBandSyntheticSample
Save-ThresholdBandMissingSyntheticSample
Save-MeanBrightnessSyntheticSample
Save-MeanBrightnessDarkSyntheticSample
Save-FeatureCardSyntheticSample
Save-FeatureCardWrongSyntheticSample
Save-EdgeFiducialSyntheticSample
Save-EdgeFiducialWrongSyntheticSample
Save-LinePinsSyntheticSample
Save-LinePinsWidePinSyntheticSample
Save-Manifest
