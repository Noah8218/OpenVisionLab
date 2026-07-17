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

function Save-GeometryTransformSyntheticSample {
    $imagePath = Join-Path $publicDir 'Geometry_RotateScale_Synthetic_OK.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 42
    $panel = New-Brush 86
    $grid = New-Pen 120 1
    $outline = New-Pen 210 4
    $mark = New-Brush 224
    $dimMark = New-Brush 164

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($panel, 74, 58, 424, 292)
    for ($x = 114; $x -le 474; $x += 40) {
        $graphics.DrawLine($grid, $x, 58, $x, 350)
    }
    for ($y = 98; $y -le 338; $y += 40) {
        $graphics.DrawLine($grid, 74, $y, 498, $y)
    }

    $graphics.DrawRectangle($outline, 142, 112, 288, 176)
    $graphics.FillRectangle($mark, 174, 148, 70, 46)
    $graphics.FillEllipse($mark, 322, 140, 64, 64)
    $graphics.FillPolygon($dimMark, [System.Drawing.Point[]]@(
        [System.Drawing.Point]::new(234, 270),
        [System.Drawing.Point]::new(284, 214),
        [System.Drawing.Point]::new(334, 270)
    ))

    $graphics.Dispose()
    $background.Dispose()
    $panel.Dispose()
    $grid.Dispose()
    $outline.Dispose()
    $mark.Dispose()
    $dimMark.Dispose()

    Add-Noise $bitmap 6243 3
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-GeometryTransformWideSyntheticSample {
    $imagePath = Join-Path $publicDir 'Geometry_RotateScale_Synthetic_Wide_NG.png'

    $bitmap = [System.Drawing.Bitmap]::new(640, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 42
    $panel = New-Brush 86
    $grid = New-Pen 120 1
    $outline = New-Pen 210 4
    $mark = New-Brush 224
    $dimMark = New-Brush 164

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($panel, 84, 58, 472, 292)
    for ($x = 124; $x -le 524; $x += 40) {
        $graphics.DrawLine($grid, $x, 58, $x, 350)
    }
    for ($y = 98; $y -le 338; $y += 40) {
        $graphics.DrawLine($grid, 84, $y, 556, $y)
    }

    $graphics.DrawRectangle($outline, 166, 112, 308, 176)
    $graphics.FillRectangle($mark, 204, 148, 70, 46)
    $graphics.FillEllipse($mark, 352, 140, 64, 64)
    $graphics.FillPolygon($dimMark, [System.Drawing.Point[]]@(
        [System.Drawing.Point]::new(264, 270),
        [System.Drawing.Point]::new(314, 214),
        [System.Drawing.Point]::new(364, 270)
    ))

    $graphics.Dispose()
    $background.Dispose()
    $panel.Dispose()
    $grid.Dispose()
    $outline.Dispose()
    $mark.Dispose()
    $dimMark.Dispose()

    Add-Noise $bitmap 6244 3
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

function Save-FilterDenoiseSyntheticSample {
    $imagePath = Join-Path $publicDir 'Filter_Denoise_Synthetic_OK.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 46
    $tray = New-Brush 76
    $target = New-Brush 196
    $rim = New-Pen 120 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($tray, 70, 66, 430, 260)
    $graphics.DrawRectangle($rim, 70, 66, 430, 260)

    $targets = @(
        @(126, 126), @(226, 128), @(326, 126), @(426, 128)
    )
    foreach ($p in $targets) {
        $graphics.FillEllipse($target, $p[0], $p[1], 52, 44)
        $graphics.DrawEllipse($rim, $p[0], $p[1], 52, 44)
    }

    $specks = @(
        @(112, 218), @(144, 246), @(182, 224), @(216, 274), @(254, 232), @(292, 258),
        @(336, 222), @(374, 274), @(414, 236), @(452, 258), @(474, 292), @(238, 154)
    )
    foreach ($p in $specks) {
        $graphics.FillRectangle($target, $p[0], $p[1], 3, 3)
    }

    $graphics.Dispose()
    $background.Dispose()
    $tray.Dispose()
    $target.Dispose()
    $rim.Dispose()

    Add-Noise $bitmap 9171 4
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-FilterDenoiseMissingSyntheticSample {
    $imagePath = Join-Path $publicDir 'Filter_Denoise_Synthetic_Missing_NG.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 46
    $tray = New-Brush 76
    $target = New-Brush 196
    $rim = New-Pen 120 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($tray, 70, 66, 430, 260)
    $graphics.DrawRectangle($rim, 70, 66, 430, 260)

    $targets = @(
        @(126, 126), @(326, 126)
    )
    foreach ($p in $targets) {
        $graphics.FillEllipse($target, $p[0], $p[1], 52, 44)
        $graphics.DrawEllipse($rim, $p[0], $p[1], 52, 44)
    }

    $specks = @(
        @(112, 218), @(144, 246), @(182, 224), @(216, 274), @(254, 232), @(292, 258),
        @(336, 222), @(374, 274), @(414, 236), @(452, 258), @(474, 292), @(238, 154),
        @(232, 146), @(432, 148)
    )
    foreach ($p in $specks) {
        $graphics.FillRectangle($target, $p[0], $p[1], 3, 3)
    }

    $graphics.Dispose()
    $background.Dispose()
    $tray.Dispose()
    $target.Dispose()
    $rim.Dispose()

    Add-Noise $bitmap 9172 4
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-EdgeDetectionShapesSyntheticSample {
    $imagePath = Join-Path $publicDir 'EdgeDetection_Shapes_Synthetic_OK.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 44
    $tray = New-Brush 72
    $target = New-Brush 196
    $rim = New-Pen 118 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($tray, 70, 66, 430, 260)
    $graphics.DrawRectangle($rim, 70, 66, 430, 260)

    $targets = @(
        @(122, 126), @(226, 126), @(330, 126), @(434, 126)
    )
    foreach ($p in $targets) {
        $graphics.FillRectangle($target, $p[0], $p[1], 48, 44)
        $graphics.DrawRectangle($rim, $p[0], $p[1], 48, 44)
    }

    $graphics.Dispose()
    $background.Dispose()
    $tray.Dispose()
    $target.Dispose()
    $rim.Dispose()

    Add-Noise $bitmap 9181 2
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-EdgeDetectionShapesMissingSyntheticSample {
    $imagePath = Join-Path $publicDir 'EdgeDetection_Shapes_Synthetic_Missing_NG.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 44
    $tray = New-Brush 72
    $target = New-Brush 196
    $rim = New-Pen 118 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($tray, 70, 66, 430, 260)
    $graphics.DrawRectangle($rim, 70, 66, 430, 260)

    $targets = @(
        @(122, 126), @(330, 126)
    )
    foreach ($p in $targets) {
        $graphics.FillRectangle($target, $p[0], $p[1], 48, 44)
        $graphics.DrawRectangle($rim, $p[0], $p[1], 48, 44)
    }

    $graphics.Dispose()
    $background.Dispose()
    $tray.Dispose()
    $target.Dispose()
    $rim.Dispose()

    Add-Noise $bitmap 9182 2
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-MorphologyCleanupSyntheticSample {
    $imagePath = Join-Path $publicDir 'Morphology_Cleanup_Synthetic_OK.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 48
    $tray = New-Brush 78
    $target = New-Brush 190
    $rim = New-Pen 118 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($tray, 70, 66, 430, 260)
    $graphics.DrawRectangle($rim, 70, 66, 430, 260)

    $targets = @(
        @(126, 128), @(226, 128), @(326, 128), @(426, 128)
    )
    foreach ($p in $targets) {
        $graphics.FillRectangle($target, $p[0], $p[1], 52, 42)
        $graphics.DrawRectangle($rim, $p[0], $p[1], 52, 42)
    }

    $specks = @(
        @(118, 230), @(166, 252), @(222, 226), @(274, 258), @(340, 232), @(396, 256), @(454, 230), @(472, 284)
    )
    foreach ($p in $specks) {
        $graphics.FillRectangle($target, $p[0], $p[1], 3, 3)
    }

    $graphics.Dispose()
    $background.Dispose()
    $tray.Dispose()
    $target.Dispose()
    $rim.Dispose()

    Add-Noise $bitmap 9161 3
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-MorphologyCleanupMissingSyntheticSample {
    $imagePath = Join-Path $publicDir 'Morphology_Cleanup_Synthetic_Missing_NG.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 48
    $tray = New-Brush 78
    $target = New-Brush 190
    $rim = New-Pen 118 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($tray, 70, 66, 430, 260)
    $graphics.DrawRectangle($rim, 70, 66, 430, 260)

    $targets = @(
        @(126, 128), @(326, 128)
    )
    foreach ($p in $targets) {
        $graphics.FillRectangle($target, $p[0], $p[1], 52, 42)
        $graphics.DrawRectangle($rim, $p[0], $p[1], 52, 42)
    }

    $specks = @(
        @(118, 230), @(166, 252), @(222, 226), @(274, 258), @(340, 232), @(396, 256), @(454, 230), @(472, 284),
        @(232, 140), @(432, 142)
    )
    foreach ($p in $specks) {
        $graphics.FillRectangle($target, $p[0], $p[1], 3, 3)
    }

    $graphics.Dispose()
    $background.Dispose()
    $tray.Dispose()
    $target.Dispose()
    $rim.Dispose()

    Add-Noise $bitmap 9162 3
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

function Save-ArithmeticInvertSyntheticSample {
    $imagePath = Join-Path $publicDir 'Arithmetic_Invert_Synthetic_OK.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 36
    $panel = New-Brush 52
    $mark = New-Brush 78
    $rim = New-Pen 92 2
    $trace = New-Pen 68 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($panel, 74, 66, 424, 260)
    $graphics.DrawRectangle($rim, 74, 66, 424, 260)

    for ($i = 0; $i -lt 7; $i++) {
        $x = 112 + ($i * 52)
        $graphics.FillRectangle($mark, $x, 124, 30, 120)
        $graphics.DrawRectangle($rim, $x, 124, 30, 120)
        $graphics.DrawLine($trace, $x + 10, 102, $x + 10, 270)
    }

    $graphics.Dispose()
    $background.Dispose()
    $panel.Dispose()
    $mark.Dispose()
    $rim.Dispose()
    $trace.Dispose()

    Add-Noise $bitmap 5241 2
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-ArithmeticInvertBrightSyntheticSample {
    $imagePath = Join-Path $publicDir 'Arithmetic_Invert_Synthetic_Bright_NG.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $background = New-Brush 188
    $panel = New-Brush 174
    $mark = New-Brush 156
    $rim = New-Pen 138 2
    $trace = New-Pen 160 2

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($panel, 74, 66, 424, 260)
    $graphics.DrawRectangle($rim, 74, 66, 424, 260)

    for ($i = 0; $i -lt 7; $i++) {
        $x = 112 + ($i * 52)
        $graphics.FillRectangle($mark, $x, 124, 30, 120)
        $graphics.DrawRectangle($rim, $x, 124, 30, 120)
        $graphics.DrawLine($trace, $x + 10, 102, $x + 10, 270)
    }

    $graphics.Dispose()
    $background.Dispose()
    $panel.Dispose()
    $mark.Dispose()
    $rim.Dispose()
    $trace.Dispose()

    Add-Noise $bitmap 5242 2
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-HsvColorPatchSyntheticSample {
    $imagePath = Join-Path $publicDir 'HSV_ColorPatch_Synthetic_OK.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::None
    $graphics.Clear([System.Drawing.Color]::FromArgb(34, 40, 47))

    $redBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(230, 48, 42))
    $blueBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(46, 92, 170))
    $grayPen = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(110, 118, 128), 2)

    $graphics.FillRectangle($blueBrush, 84, 88, 92, 62)
    $graphics.FillRectangle($redBrush, 210, 88, 70, 50)
    $graphics.FillRectangle($redBrush, 312, 88, 70, 50)
    $graphics.FillRectangle($redBrush, 210, 174, 70, 50)
    $graphics.FillRectangle($redBrush, 312, 174, 70, 50)
    $graphics.DrawRectangle($grayPen, 196, 74, 200, 164)

    $grayPen.Dispose()
    $blueBrush.Dispose()
    $redBrush.Dispose()
    $graphics.Dispose()
    $bitmap.Save($imagePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()

    Write-Host "Generated: $imagePath"
}

function Save-HsvColorPatchMissingSyntheticSample {
    $imagePath = Join-Path $publicDir 'HSV_ColorPatch_Synthetic_Missing_NG.png'

    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::None
    $graphics.Clear([System.Drawing.Color]::FromArgb(34, 40, 47))

    $redBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(230, 48, 42))
    $blueBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(46, 92, 170))
    $grayPen = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(110, 118, 128), 2)

    $graphics.FillRectangle($blueBrush, 84, 88, 92, 62)
    $graphics.FillRectangle($redBrush, 210, 88, 70, 50)
    $graphics.FillRectangle($blueBrush, 312, 88, 70, 50)
    $graphics.FillRectangle($blueBrush, 210, 174, 70, 50)
    $graphics.FillRectangle($blueBrush, 312, 174, 70, 50)
    $graphics.DrawRectangle($grayPen, 196, 74, 200, 164)

    $grayPen.Dispose()
    $blueBrush.Dispose()
    $redBrush.Dispose()
    $graphics.Dispose()
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

function New-FixturePadSyntheticBitmap(
    [int]$OffsetX,
    [int]$OffsetY,
    [bool]$IncludePad
) {
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::None

    $background = New-Brush 38
    $plate = New-Brush 76
    $plateEdge = New-Pen 138 3
    $trace = New-Pen 112 2
    $locator = New-Pen 232 4
    $locatorFill = New-Brush 214
    $pad = New-Brush 224
    $missingPad = New-Brush 116
    $distractor = New-Brush 196

    $graphics.FillRectangle($background, 0, 0, $bitmap.Width, $bitmap.Height)
    $graphics.FillRectangle($plate, 60 + $OffsetX, 45 + $OffsetY, 430, 260)
    $graphics.DrawRectangle($plateEdge, 60 + $OffsetX, 45 + $OffsetY, 430, 260)

    for ($i = 0; $i -lt 5; $i++) {
        $y = 145 + $OffsetY + ($i * 20)
        $graphics.DrawLine($trace, 105 + $OffsetX, $y, 290 + $OffsetX, $y + 4)
    }

    $locatorX = 120 + $OffsetX
    $locatorY = 100 + $OffsetY
    $graphics.DrawRectangle($locator, $locatorX - 21, $locatorY - 21, 42, 42)
    $graphics.DrawLine($locator, $locatorX - 15, $locatorY + 13, $locatorX + 13, $locatorY + 13)
    $graphics.DrawLine($locator, $locatorX - 15, $locatorY - 13, $locatorX - 15, $locatorY + 13)
    $graphics.FillEllipse($locatorFill, $locatorX + 6, $locatorY - 13, 9, 9)

    $graphics.FillEllipse($distractor, 225 + $OffsetX, 255 + $OffsetY, 24, 24)
    $graphics.FillRectangle($distractor, 438 + $OffsetX, 125 + $OffsetY, 20, 20)

    if ($IncludePad) {
        $graphics.FillRectangle($pad, 330 + $OffsetX, 190 + $OffsetY, 36, 28)
    }
    else {
        $graphics.FillRectangle($missingPad, 330 + $OffsetX, 190 + $OffsetY, 36, 28)
    }
    $graphics.DrawRectangle($plateEdge, 330 + $OffsetX, 190 + $OffsetY, 36, 28)

    $graphics.Dispose()
    $background.Dispose()
    $plate.Dispose()
    $plateEdge.Dispose()
    $trace.Dispose()
    $locator.Dispose()
    $locatorFill.Dispose()
    $pad.Dispose()
    $missingPad.Dispose()
    $distractor.Dispose()

    return $bitmap
}

function Save-FixturePadSyntheticSamples {
    $goodPath = Join-Path $publicDir 'Fixture_Pad_Synthetic_Shifted_OK.png'
    $badPath = Join-Path $publicDir 'Fixture_Pad_Synthetic_Shifted_Missing_NG.png'
    $templatePath = Join-Path $templateDir 'Fixture_Locator_Synthetic_Template.png'

    $reference = New-FixturePadSyntheticBitmap 0 0 $true
    $template = $reference.Clone(
        [System.Drawing.Rectangle]::new(90, 70, 60, 60),
        [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $template.Save($templatePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $template.Dispose()
    $reference.Dispose()

    $good = New-FixturePadSyntheticBitmap 80 55 $true
    $good.Save($goodPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $good.Dispose()

    $bad = New-FixturePadSyntheticBitmap 80 55 $false
    $bad.Save($badPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bad.Dispose()

    Write-Host "Generated: $goodPath"
    Write-Host "Generated: $badPath"
    Write-Host "Generated: $templatePath"
}

function Save-Manifest {
    $manifestPath = Join-Path $publicDir 'OpenVisionLab.PublicSampleManifest.csv'
    $rows = @(
        'AssetPath,SourceType,SourceName,SourceUrl,License,Attribution,GeneratedBy,Notes',
        'docs/samples/public/Workspace_Inspection_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated workspace overview sample for public tutorial captures',
        'docs/samples/public/Matching_DiePad_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated grayscale matching source image',
        'docs/samples/public/Matching_DiePad_Synthetic_NoTarget_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated matching no-target negative sample',
        'docs/samples/public/templates/Matching_DiePad_Synthetic_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated template crop from Matching_DiePad_Synthetic_OK.png',
        'docs/samples/public/Fixture_Pad_Synthetic_Shifted_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated shifted fixture scene with the inspection pad present',
        'docs/samples/public/Fixture_Pad_Synthetic_Shifted_Missing_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated shifted fixture scene with the locator present and inspection pad missing',
        'docs/samples/public/templates/Fixture_Locator_Synthetic_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated locator template crop from the reference-pose fixture scene',
        'docs/samples/public/Blob_Particles_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated blob particle source image',
        'docs/samples/public/Blob_Particles_Synthetic_Sparse_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated sparse blob negative sample',
        'docs/samples/public/Contour_Shapes_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated contour shape-count source image',
        'docs/samples/public/Contour_Shapes_Synthetic_Missing_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated contour missing-shape negative sample',
        'docs/samples/public/Geometry_RotateScale_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated geometry transform source image',
        'docs/samples/public/Geometry_RotateScale_Synthetic_Wide_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated geometry transform wide-input negative sample',
        'docs/samples/public/Threshold_BandPads_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated basic threshold source image',
        'docs/samples/public/Threshold_BandPads_Synthetic_Missing_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated basic threshold missing-pad negative sample',
        'docs/samples/public/Filter_Denoise_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated filter denoise source image',
        'docs/samples/public/Filter_Denoise_Synthetic_Missing_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated filter denoise missing-target negative sample',
        'docs/samples/public/EdgeDetection_Shapes_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated edge detection shape source image',
        'docs/samples/public/EdgeDetection_Shapes_Synthetic_Missing_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated edge detection missing-shape negative sample',
        'docs/samples/public/Morphology_Cleanup_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated morphology cleanup source image',
        'docs/samples/public/Morphology_Cleanup_Synthetic_Missing_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated morphology cleanup missing-target negative sample',
        'docs/samples/public/Mean_Brightness_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated mean brightness normal sample',
        'docs/samples/public/Mean_Brightness_Synthetic_Dark_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated mean brightness dark drift negative sample',
        'docs/samples/public/Arithmetic_Invert_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated arithmetic inversion normal sample',
        'docs/samples/public/Arithmetic_Invert_Synthetic_Bright_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated arithmetic inversion bright-input negative sample',
        'docs/samples/public/HSV_ColorPatch_Synthetic_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated HSV color patch normal sample',
        'docs/samples/public/HSV_ColorPatch_Synthetic_Missing_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionSyntheticSamples.ps1,Generated HSV color patch missing-color negative sample',
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
Save-GeometryTransformSyntheticSample
Save-GeometryTransformWideSyntheticSample
Save-ThresholdBandSyntheticSample
Save-ThresholdBandMissingSyntheticSample
Save-FilterDenoiseSyntheticSample
Save-FilterDenoiseMissingSyntheticSample
Save-EdgeDetectionShapesSyntheticSample
Save-EdgeDetectionShapesMissingSyntheticSample
Save-MorphologyCleanupSyntheticSample
Save-MorphologyCleanupMissingSyntheticSample
Save-MeanBrightnessSyntheticSample
Save-MeanBrightnessDarkSyntheticSample
Save-ArithmeticInvertSyntheticSample
Save-ArithmeticInvertBrightSyntheticSample
Save-HsvColorPatchSyntheticSample
Save-HsvColorPatchMissingSyntheticSample
Save-FeatureCardSyntheticSample
Save-FeatureCardWrongSyntheticSample
Save-EdgeFiducialSyntheticSample
Save-EdgeFiducialWrongSyntheticSample
Save-LinePinsSyntheticSample
Save-LinePinsWidePinSyntheticSample
Save-FixturePadSyntheticSamples
Save-Manifest
