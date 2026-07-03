param(
    [string]$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing

Add-Type -ReferencedAssemblies 'System.Drawing' -TypeDefinition @"
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

public static class OpenVisionSyntheticFieldVariation
{
    public static void Apply(Bitmap bitmap, int seed, int profile)
    {
        if (bitmap == null)
        {
            throw new ArgumentNullException("bitmap");
        }

        var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        var data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        try
        {
            int stride = Math.Abs(data.Stride);
            int width = bitmap.Width;
            int height = bitmap.Height;
            byte[] source = new byte[stride * height];
            byte[] target = new byte[stride * height];
            Marshal.Copy(data.Scan0, source, 0, source.Length);
            Buffer.BlockCopy(source, 0, target, 0, source.Length);

            int gradientAmp = profile == 2 ? 7 : profile == 1 ? 5 : 4;
            int bandAmp = profile == 2 ? 4 : 2;
            int grainAmp = profile == 1 ? 3 : 2;
            double gx = (((seed & 15) - 7.5) / 7.5) * gradientAmp;
            double gy = ((((seed >> 4) & 15) - 7.5) / 7.5) * gradientAmp;
            double bandPeriod = profile == 2 ? 48.0 + ((seed >> 8) & 15) : 72.0 + ((seed >> 8) & 31);
            double columnPeriod = profile == 2 ? 86.0 + ((seed >> 12) & 31) : 128.0 + ((seed >> 12) & 31);
            double bandPhase = (seed & 1023) * 0.017;
            double vignetteAmp = profile == 2 ? 5.5 : 3.5;

            for (int y = 0; y < height; y++)
            {
                int row = data.Stride > 0 ? y * data.Stride : (height - 1 - y) * stride;
                double ny = height <= 1 ? 0.0 : ((double)y / (height - 1)) - 0.5;
                for (int x = 0; x < width; x++)
                {
                    int index = row + (x * 3);
                    double nx = width <= 1 ? 0.0 : ((double)x / (width - 1)) - 0.5;
                    int value = source[index];

                    double gradient = (nx * gx) + (ny * gy);
                    double scanBand = Math.Sin((((double)y + bandPhase) / bandPeriod) * Math.PI * 2.0) * bandAmp;
                    double columnBand = Math.Sin((((double)x + (seed % 97)) / columnPeriod) * Math.PI * 2.0) * (profile == 2 ? 1.6 : 1.0);
                    double vignette = -vignetteAmp * ((nx * nx) + (ny * ny)) * 2.2;

                    int h = (x * 73856093) ^ (y * 19349663) ^ (seed * 83492791);
                    h = (h >> 13) ^ h;
                    double grain = ((((h & 255) / 255.0) * 2.0) - 1.0) * grainAmp;

                    int adjusted = Clamp((int)Math.Round(value + gradient + scanBand + columnBand + vignette + grain));
                    target[index] = (byte)adjusted;
                    target[index + 1] = (byte)adjusted;
                    target[index + 2] = (byte)adjusted;
                }
            }

            double blurWeight = profile == 2 ? 0.04 : ((seed & 1) == 0 ? 0.08 : 0.0);
            if (blurWeight > 0.0)
            {
                byte[] softened = new byte[target.Length];
                Buffer.BlockCopy(target, 0, softened, 0, target.Length);
                for (int y = 1; y < height - 1; y++)
                {
                    int row = data.Stride > 0 ? y * data.Stride : (height - 1 - y) * stride;
                    int rowUp = data.Stride > 0 ? (y - 1) * data.Stride : (height - y) * stride;
                    int rowDown = data.Stride > 0 ? (y + 1) * data.Stride : (height - 2 - y) * stride;
                    for (int x = 1; x < width - 1; x++)
                    {
                        int index = row + (x * 3);
                        int average =
                            target[index] +
                            target[row + ((x - 1) * 3)] +
                            target[row + ((x + 1) * 3)] +
                            target[rowUp + (x * 3)] +
                            target[rowDown + (x * 3)];
                        int softenedValue = Clamp((int)Math.Round((target[index] * (1.0 - blurWeight)) + ((average / 5.0) * blurWeight)));
                        softened[index] = (byte)softenedValue;
                        softened[index + 1] = (byte)softenedValue;
                        softened[index + 2] = (byte)softenedValue;
                    }
                }

                target = softened;
            }

            Marshal.Copy(target, 0, data.Scan0, target.Length);
        }
        finally
        {
            bitmap.UnlockBits(data);
        }
    }

    private static int Clamp(int value)
    {
        if (value < 0)
        {
            return 0;
        }

        if (value > 255)
        {
            return 255;
        }

        return value;
    }
}
"@

$productDir = Join-Path $Root 'docs\samples\public\product'
$templateDir = Join-Path $productDir 'templates'
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

function Get-StablePathSeed([string]$Path) {
    $normalized = $Path.Replace('\', '/').ToLowerInvariant()
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($normalized)
    $hash = [uint32]2166136261
    foreach ($byte in $bytes) {
        $mixed = [uint32]($hash -bxor [uint32]$byte)
        $hash = [uint32](([uint64]$mixed * [uint64]16777619) -band [uint64]4294967295)
    }

    return [int]($hash -band 0x7fffffff)
}

function Get-ProductFieldProfile([string]$Path) {
    $normalized = $Path.Replace('\', '/')
    if ($normalized.IndexOf('/docs/samples/public/product/', [StringComparison]::OrdinalIgnoreCase) -lt 0 -or
        $normalized.IndexOf('/templates/', [StringComparison]::OrdinalIgnoreCase) -ge 0) {
        return 0
    }

    if ($normalized.IndexOf('/Battery_', [StringComparison]::OrdinalIgnoreCase) -ge 0) {
        return 1
    }

    if ($normalized.IndexOf('/Display_', [StringComparison]::OrdinalIgnoreCase) -ge 0) {
        return 2
    }

    if ($normalized.IndexOf('/Semiconductor_', [StringComparison]::OrdinalIgnoreCase) -ge 0) {
        return 3
    }

    return 0
}

function Save-Bitmap([System.Drawing.Bitmap]$Bitmap, [string]$Path) {
    $profile = Get-ProductFieldProfile $Path
    if ($profile -gt 0) {
        $seed = Get-StablePathSeed $Path
        [OpenVisionSyntheticFieldVariation]::Apply($Bitmap, $seed, $profile)
    }

    $Bitmap.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
    Write-Host "Generated: $Path"
}

function Draw-BatteryBase([System.Drawing.Graphics]$Graphics) {
    $background = New-Brush 38
    $cellBody = New-Brush 82
    $caseLine = New-Pen 162 3
    $gridLine = New-Pen 112 1
    $tab = New-Brush 212
    $tabEdge = New-Pen 188 2

    $Graphics.FillRectangle($background, 0, 0, 572, 420)
    $Graphics.FillRectangle($cellBody, 48, 42, 470, 292)
    $Graphics.DrawRectangle($caseLine, 50, 44, 466, 288)

    for ($i = 0; $i -lt 7; $i++) {
        $x = 86 + ($i * 48)
        $Graphics.DrawLine($gridLine, $x, 58, $x + 18, 318)
    }

    for ($i = 0; $i -lt 5; $i++) {
        $x = 76 + ($i * 72)
        $Graphics.FillRectangle($tab, $x, 300, 42, 48)
        $Graphics.DrawRectangle($tabEdge, $x, 300, 42, 48)
    }

    $background.Dispose()
    $cellBody.Dispose()
    $caseLine.Dispose()
    $gridLine.Dispose()
    $tab.Dispose()
    $tabEdge.Dispose()
}

function Save-BatteryTabGapSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_TabGap_Narrow_NG.png' } else { 'Battery_TabGap_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $white = New-Brush 224
    $dark = New-Brush 58
    $labelPen = New-Pen 170 2
    $labelBrush = New-Brush 180
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    # Keep the measurement ROI visually quiet. The line tool should see only
    # the two tab edges, not a decorative frame around the fixture.
    $graphics.FillRectangle($dark, 405, 150, 150, 190)

    if ($Bad) {
        $graphics.FillRectangle($white, 450, 178, 18, 128)
        $graphics.FillRectangle($white, 486, 178, 18, 128)
    }
    else {
        $graphics.FillRectangle($white, 438, 178, 25, 128)
        $graphics.FillRectangle($white, 501, 178, 25, 128)
    }

    $graphics.DrawLine($labelPen, 414, 132, 550, 132)
    $graphics.DrawString('TAB GAP', $labelFont, $labelBrush, 422, 112)

    $white.Dispose()
    $dark.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3102 } else { 3101 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryWeldSpatterSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_WeldSpatter_Heavy_NG.png' } else { 'Battery_WeldSpatter_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $weld = New-Brush 145
    $bright = New-Brush 230
    $rim = New-Pen 190 2
    $graphics.FillEllipse($weld, 300, 164, 132, 82)
    $graphics.DrawEllipse($rim, 300, 164, 132, 82)

    $spots = if ($Bad) {
        @(
            @(276,134,12), @(330,128,10), @(388,136,11), @(450,154,12), @(282,220,13), @(446,232,10),
            @(318,260,11), @(370,270,12), @(420,262,10), @(250,184,9), @(470,196,9), @(352,108,10)
        )
    }
    else {
        @(
            @(314,144,10), @(394,146,9), @(286,218,10), @(438,222,9), @(360,258,10)
        )
    }

    foreach ($spot in $spots) {
        $graphics.FillEllipse($bright, $spot[0], $spot[1], $spot[2], $spot[2])
    }

    $weld.Dispose()
    $bright.Dispose()
    $rim.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3112 } else { 3111 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryWeldOverburnSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_WeldOverburn_Many_NG.png' } else { 'Battery_WeldOverburn_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $weld = New-Brush 116
    $weldRim = New-Pen 164 2
    $hotSpot = New-Brush 238
    $shadow = New-Brush 126
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillEllipse($weld, 226, 138, 160, 118)
    $graphics.DrawEllipse($weldRim, 226, 138, 160, 118)
    $graphics.DrawLine($labelPen, 210, 114, 408, 114)
    $graphics.DrawString('WELD OVERBURN', $labelFont, $labelBrush, 226, 94)

    $hotSpots = if ($Bad) {
        @('252,160,18,18', '304,154,20,20', '348,178,18,18', '270,218,18,18', '326,220,20,20')
    }
    else {
        @('318,184,16,16')
    }

    foreach ($item in $hotSpots) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $s = [int]$parts[2]
        $graphics.FillEllipse($shadow, $x + 2, $y + 2, $s, $s)
        $graphics.FillEllipse($hotSpot, $x, $y, $s, $s)
    }

    $weld.Dispose()
    $weldRim.Dispose()
    $hotSpot.Dispose()
    $shadow.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3192 } else { 3191 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryTabTearSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_TabTear_Many_NG.png' } else { 'Battery_TabTear_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $carrier = New-Brush 82
    $tab = New-Brush 194
    $tear = New-Brush 238
    $rim = New-Pen 206 2
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($carrier, 104, 136, 360, 140)
    for ($i = 0; $i -lt 6; $i++) {
        $graphics.FillRectangle($tab, 124 + ($i * 54), 174, 32, 82)
    }

    $graphics.DrawLine($labelPen, 106, 112, 348, 112)
    $graphics.DrawString('TAB TEAR', $labelFont, $labelBrush, 128, 92)

    $tears = if ($Bad) {
        @('132,170,24,22', '186,174,22,20', '240,170,26,22', '294,174,24,20', '348,170,22,22')
    }
    else {
        @('294,174,22,18')
    }

    foreach ($item in $tears) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $points = [System.Drawing.Point[]]@(
            [System.Drawing.Point]::new($x, $y),
            [System.Drawing.Point]::new($x + $w, $y + 4),
            [System.Drawing.Point]::new($x + [int]($w / 2), $y + $h)
        )
        $graphics.FillPolygon($tear, $points)
        $graphics.DrawPolygon($rim, $points)
    }

    $carrier.Dispose()
    $tab.Dispose()
    $tear.Dispose()
    $rim.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3200 } else { 3199 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryTabPlatingPeelSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_TabPlatingPeel_Many_NG.png' } else { 'Battery_TabPlatingPeel_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $carrier = New-Brush 82
    $tab = New-Brush 190
    $peel = New-Brush 238
    $peelRim = New-Pen 206 2
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($carrier, 104, 136, 360, 140)
    for ($i = 0; $i -lt 6; $i++) {
        $graphics.FillRectangle($tab, 124 + ($i * 54), 172, 32, 84)
    }

    $graphics.DrawLine($labelPen, 106, 112, 400, 112)
    $graphics.DrawString('TAB PLATING PEEL', $labelFont, $labelBrush, 128, 92)

    $peels = if ($Bad) {
        @('128,190,20,18', '182,204,22,18', '236,188,21,20', '290,208,22,18', '344,190,20,18')
    }
    else {
        @('290,204,20,18')
    }

    foreach ($item in $peels) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($peel, $x, $y, $w, $h)
        $graphics.DrawEllipse($peelRim, $x, $y, $w, $h)
    }

    $carrier.Dispose()
    $tab.Dispose()
    $peel.Dispose()
    $peelRim.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3202 } else { 3201 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryElectrolyteStainSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_ElectrolyteStain_Heavy_NG.png' } else { 'Battery_ElectrolyteStain_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $inspectionBase = New-Brush 84
    $stainSoft = New-Brush 104
    $stainHeavy = New-Brush 142
    $stainMid = New-Brush 128
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($inspectionBase, 112, 130, 352, 148)
    $graphics.DrawLine($labelPen, 104, 108, 452, 108)
    $graphics.DrawString('ELECTROLYTE STAIN', $labelFont, $labelBrush, 122, 88)

    if ($Bad) {
        $graphics.FillEllipse($stainHeavy, 148, 142, 174, 96)
        $graphics.FillEllipse($stainMid, 268, 164, 148, 82)
        $graphics.FillEllipse($stainHeavy, 216, 206, 150, 62)
    }
    else {
        $graphics.FillEllipse($stainSoft, 242, 178, 78, 38)
    }

    $inspectionBase.Dispose()
    $stainSoft.Dispose()
    $stainHeavy.Dispose()
    $stainMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3194 } else { 3193 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatterySeparatorWrinkleSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_SeparatorWrinkle_Many_NG.png' } else { 'Battery_SeparatorWrinkle_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $separator = New-Brush 78
    $wrinkle = New-Pen 236 4
    $wrinkleThin = New-Pen 232 3
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($separator, 112, 124, 352, 164)
    $graphics.DrawLine($labelPen, 104, 104, 432, 104)
    $graphics.DrawString('SEPARATOR WRINKLE', $labelFont, $labelBrush, 122, 84)

    if ($Bad) {
        $graphics.DrawBezier($wrinkle, 138, 154, 192, 126, 232, 190, 286, 160)
        $graphics.DrawBezier($wrinkleThin, 178, 206, 232, 174, 278, 238, 330, 206)
        $graphics.DrawBezier($wrinkle, 246, 142, 292, 116, 338, 184, 394, 150)
        $graphics.DrawBezier($wrinkleThin, 310, 246, 356, 214, 406, 270, 448, 238)
    }
    else {
        $graphics.DrawBezier($wrinkleThin, 214, 184, 254, 158, 292, 210, 334, 184)
    }

    $separator.Dispose()
    $wrinkle.Dispose()
    $wrinkleThin.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3196 } else { 3195 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatterySeparatorPinholeSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_SeparatorPinhole_Many_NG.png' } else { 'Battery_SeparatorPinhole_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $separator = New-Brush 76
    $pinhole = New-Brush 238
    $rim = New-Pen 202 1
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($separator, 118, 126, 344, 162)
    $graphics.DrawLine($labelPen, 108, 104, 396, 104)
    $graphics.DrawString('SEPARATOR PINHOLE', $labelFont, $labelBrush, 124, 84)

    if ($Bad) {
        $holes = @(@(162,160,14), @(228,206,16), @(304,148,15), @(372,222,17), @(420,174,13))
    }
    else {
        $holes = @(,@(288,190,14))
    }

    foreach ($hole in $holes) {
        $graphics.FillEllipse($pinhole, $hole[0], $hole[1], $hole[2], $hole[2])
        $graphics.DrawEllipse($rim, $hole[0], $hole[1], $hole[2], $hole[2])
    }

    $separator.Dispose()
    $pinhole.Dispose()
    $rim.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3198 } else { 3197 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryCoatingGapSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_CoatingGap_Narrow_NG.png' } else { 'Battery_CoatingGap_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $dark = New-Brush 54
    $coating = New-Brush 210
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($dark, 150, 144, 150, 168)
    if ($Bad) {
        $graphics.FillRectangle($coating, 190, 172, 16, 116)
        $graphics.FillRectangle($coating, 226, 172, 16, 116)
    }
    else {
        $graphics.FillRectangle($coating, 180, 172, 22, 116)
        $graphics.FillRectangle($coating, 240, 172, 22, 116)
    }

    $graphics.DrawLine($labelPen, 148, 126, 302, 126)
    $graphics.DrawString('COATING GAP', $labelFont, $labelBrush, 162, 106)

    $dark.Dispose()
    $coating.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3122 } else { 3121 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryForeignObjectSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_ForeignObject_Many_NG.png' } else { 'Battery_ForeignObject_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $foreign = New-Brush 234
    $shadow = New-Brush 126
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 76, 78, 260, 78)
    $graphics.DrawString('FOREIGN OBJECT', $labelFont, $labelBrush, 86, 58)

    $spots = if ($Bad) {
        @('146,116,14', '236,174,12', '340,130,16', '428,222,14')
    }
    else {
        @('236,174,12')
    }

    foreach ($spot in $spots) {
        $parts = $spot.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $s = [int]$parts[2]
        $graphics.FillEllipse($shadow, $x + 2, $y + 2, $s, $s)
        $graphics.FillEllipse($foreign, $x, $y, $s, $s)
    }

    $foreign.Dispose()
    $shadow.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3132 } else { 3131 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryEdgeBurrSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_EdgeBurr_Many_NG.png' } else { 'Battery_EdgeBurr_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $burr = New-Brush 236
    $shadow = New-Brush 118
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 300, 92, 502, 92)
    $graphics.DrawString('COATING EDGE BURR', $labelFont, $labelBrush, 308, 72)

    $spots = if ($Bad) {
        @('338,122,16,10', '382,132,18,11', '426,118,16,10', '462,148,18,12', '494,132,15,10')
    }
    else {
        @('414,132,15,10')
    }

    foreach ($spot in $spots) {
        $parts = $spot.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($shadow, $x + 2, $y + 2, $w, $h)
        $graphics.FillEllipse($burr, $x, $y, $w, $h)
    }

    $burr.Dispose()
    $shadow.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3142 } else { 3141 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryTabOffsetSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_TabOffset_Shifted_NG.png' } else { 'Battery_TabOffset_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $dark = New-Brush 54
    $reference = New-Brush 226
    $tab = New-Brush 216
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($dark, 386, 158, 154, 168)
    $graphics.FillRectangle($reference, 414, 186, 24, 112)
    if ($Bad) {
        $graphics.FillRectangle($tab, 456, 186, 34, 112)
    }
    else {
        $graphics.FillRectangle($tab, 476, 186, 34, 112)
    }

    $graphics.DrawLine($labelPen, 384, 132, 540, 132)
    $graphics.DrawString('TAB OFFSET', $labelFont, $labelBrush, 402, 112)

    $dark.Dispose()
    $reference.Dispose()
    $tab.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3152 } else { 3151 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatterySealWidthSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_SealWidth_Narrow_NG.png' } else { 'Battery_SealWidth_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $dark = New-Brush 54
    $seal = New-Brush 220
    $reference = New-Brush 210
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($dark, 132, 158, 154, 168)
    $graphics.FillRectangle($reference, 160, 186, 24, 112)
    if ($Bad) {
        $graphics.FillRectangle($seal, 204, 186, 28, 112)
    }
    else {
        $graphics.FillRectangle($seal, 224, 186, 28, 112)
    }

    $graphics.DrawLine($labelPen, 130, 132, 286, 132)
    $graphics.DrawString('SEAL WIDTH', $labelFont, $labelBrush, 150, 112)

    $dark.Dispose()
    $seal.Dispose()
    $reference.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3162 } else { 3161 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryTabWeldVoidSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_TabWeldVoid_Many_NG.png' } else { 'Battery_TabWeldVoid_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $weld = New-Brush 118
    $weldRim = New-Pen 168 2
    $void = New-Brush 238
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillEllipse($weld, 222, 142, 160, 112)
    $graphics.DrawEllipse($weldRim, 222, 142, 160, 112)
    $graphics.DrawLine($labelPen, 210, 116, 402, 116)
    $graphics.DrawString('TAB WELD VOID', $labelFont, $labelBrush, 226, 96)

    $voids = if ($Bad) {
        @('248,166,13', '294,162,11', '338,170,12', '264,218,10', '314,214,13', '356,204,10')
    }
    else {
        @('314,188,11')
    }

    foreach ($item in $voids) {
        $parts = $item.Split(',')
        $graphics.FillEllipse($void, [int]$parts[0], [int]$parts[1], [int]$parts[2], [int]$parts[2])
    }

    $weld.Dispose()
    $weldRim.Dispose()
    $void.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3172 } else { 3171 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryPouchEdgeFoldSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_PouchEdgeFold_Many_NG.png' } else { 'Battery_PouchEdgeFold_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $fold = New-Brush 236
    $shadow = New-Brush 118
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 276, 96, 504, 96)
    $graphics.DrawString('POUCH EDGE FOLD', $labelFont, $labelBrush, 288, 76)

    $folds = if ($Bad) {
        @('326,116,16,10', '370,126,18,10', '414,116,16,11', '456,146,18,12', '488,128,16,10')
    }
    else {
        @('414,126,16,10')
    }

    foreach ($item in $folds) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($shadow, $x + 2, $y + 2, $w, $h)
        $graphics.FillEllipse($fold, $x, $y, $w, $h)
    }

    $fold.Dispose()
    $shadow.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3182 } else { 3181 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryPouchSealBurnSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_PouchSealBurn_Many_NG.png' } else { 'Battery_PouchSealBurn_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $seal = New-Brush 74
    $burn = New-Brush 238
    $burnMid = New-Brush 204
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($seal, 94, 130, 386, 132)
    $graphics.DrawLine($labelPen, 106, 108, 386, 108)
    $graphics.DrawString('POUCH SEAL BURN', $labelFont, $labelBrush, 120, 88)

    $burns = if ($Bad) {
        @('132,158,26,20', '204,184,24,18', '278,154,28,20', '342,202,24,18', '416,176,26,20')
    }
    else {
        @('278,184,24,18')
    }

    foreach ($item in $burns) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($burnMid, $x + 3, $y + 3, $w, $h)
        $graphics.FillEllipse($burn, $x, $y, $w, $h)
    }

    $seal.Dispose()
    $burn.Dispose()
    $burnMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3204 } else { 3203 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryPouchSealBubbleSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_PouchSealBubble_Many_NG.png' } else { 'Battery_PouchSealBubble_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $seal = New-Brush 76
    $inner = New-Brush 94
    $bubble = New-Brush 238
    $bubbleMid = New-Brush 206
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($seal, 88, 128, 398, 136)
    $graphics.FillRectangle($inner, 110, 154, 354, 82)
    $graphics.DrawLine($labelPen, 106, 108, 394, 108)
    $graphics.DrawString('POUCH SEAL BUBBLE', $labelFont, $labelBrush, 120, 88)

    $bubbles = if ($Bad) {
        @('134,156,24,20', '214,194,22,18', '294,154,24,20', '362,214,24,20', '426,178,22,18')
    }
    else {
        @('294,178,22,18')
    }

    foreach ($item in $bubbles) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($bubbleMid, $x + 3, $y + 3, $w, $h)
        $graphics.FillEllipse($bubble, $x, $y, $w, $h)
    }

    $seal.Dispose()
    $inner.Dispose()
    $bubble.Dispose()
    $bubbleMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3208 } else { 3207 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatterySealEdgeDelaminationSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_SealEdgeDelamination_Many_NG.png' } else { 'Battery_SealEdgeDelamination_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $seal = New-Brush 76
    $lamination = New-Brush 112
    $delam = New-Brush 238
    $delamMid = New-Brush 204
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($seal, 88, 128, 396, 136)
    $graphics.FillRectangle($lamination, 112, 150, 348, 82)
    $graphics.DrawLine($labelPen, 104, 108, 432, 108)
    $graphics.DrawString('SEAL EDGE DELAMINATION', $labelFont, $labelBrush, 118, 88)

    $defects = if ($Bad) {
        @('126,132,30,18', '198,226,28,18', '270,132,30,18', '344,226,28,18', '420,132,30,18')
    }
    else {
        @('270,132,28,17')
    }

    foreach ($item in $defects) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($delamMid, $x + 3, $y + 3, $w, $h)
        $graphics.FillEllipse($delam, $x, $y, $w, $h)
    }

    $seal.Dispose()
    $lamination.Dispose()
    $delam.Dispose()
    $delamMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3206 } else { 3205 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryTabOxidationSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_TabOxidation_Many_NG.png' } else { 'Battery_TabOxidation_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $tab = New-Brush 178
    $oxidation = New-Brush 238
    $oxidationMid = New-Brush 204
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 104, 108, 362, 108)
    $graphics.DrawString('TAB OXIDATION', $labelFont, $labelBrush, 118, 88)

    for ($i = 0; $i -lt 5; $i++) {
        $x = 118 + ($i * 68)
        $graphics.FillRectangle($tab, $x, 142, 38, 112)
        $graphics.FillRectangle($tab, $x + 5, 130, 28, 14)
    }

    $spots = if ($Bad) {
        @('126,154,22,18', '194,204,22,18', '262,158,22,18', '330,218,22,18', '398,170,22,18')
    }
    else {
        @('262,158,20,17')
    }

    foreach ($item in $spots) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($oxidationMid, $x + 3, $y + 3, $w, $h)
        $graphics.FillEllipse($oxidation, $x, $y, $w, $h)
    }

    $tab.Dispose()
    $oxidation.Dispose()
    $oxidationMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3208 } else { 3207 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryTabDiscolorationSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_TabDiscoloration_Dark_NG.png' } else { 'Battery_TabDiscoloration_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $panel = if ($Bad) { New-Brush 120 } else { New-Brush 164 }
    $shadow = if ($Bad) { New-Brush 82 } else { New-Brush 142 }
    $tab = if ($Bad) { New-Brush 132 } else { New-Brush 176 }
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 104, 108, 406, 108)
    $graphics.DrawString('TAB DISCOLORATION', $labelFont, $labelBrush, 118, 88)
    $graphics.FillRectangle($panel, 102, 132, 368, 134)

    for ($i = 0; $i -lt 5; $i++) {
        $x = 122 + ($i * 66)
        $graphics.FillRectangle($tab, $x, 148, 38, 94)
        $graphics.FillRectangle($tab, $x + 6, 136, 26, 14)
    }

    if ($Bad) {
        $graphics.FillEllipse($shadow, 126, 156, 60, 70)
        $graphics.FillEllipse($shadow, 236, 146, 72, 86)
        $graphics.FillEllipse($shadow, 352, 164, 62, 72)
    }
    else {
        $graphics.FillEllipse($shadow, 274, 164, 24, 20)
    }

    $panel.Dispose()
    $shadow.Dispose()
    $tab.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3210 } else { 3209 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatterySealContaminationSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_SealContamination_Many_NG.png' } else { 'Battery_SealContamination_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $seal = New-Brush 82
    $sealInner = New-Brush 110
    $contamination = New-Brush 238
    $contaminationMid = New-Brush 202
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($seal, 88, 128, 396, 138)
    $graphics.FillRectangle($sealInner, 116, 154, 340, 84)
    $graphics.DrawLine($labelPen, 104, 108, 420, 108)
    $graphics.DrawString('SEAL CONTAMINATION', $labelFont, $labelBrush, 118, 88)

    $spots = if ($Bad) {
        @('126,158,28,22', '198,210,26,22', '270,164,28,22', '342,216,26,22', '416,176,28,22')
    }
    else {
        @('270,164,26,20')
    }

    foreach ($item in $spots) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($contaminationMid, $x + 3, $y + 3, $w, $h)
        $graphics.FillEllipse($contamination, $x, $y, $w, $h)
    }

    $seal.Dispose()
    $sealInner.Dispose()
    $contamination.Dispose()
    $contaminationMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3212 } else { 3211 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Draw-BatteryLaserMark([System.Drawing.Graphics]$Graphics, [int]$X, [int]$Y, [bool]$Missing) {
    $panel = New-Brush 178
    $dark = New-Brush 42
    $mid = New-Brush 108
    $line = New-Pen 54 4
    $thin = New-Pen 96 2
    $font = [System.Drawing.Font]::new('Arial', 18, [System.Drawing.FontStyle]::Bold)
    $smallFont = [System.Drawing.Font]::new('Arial', 8, [System.Drawing.FontStyle]::Bold)

    $Graphics.FillRectangle($panel, $X, $Y, 132, 82)
    $Graphics.DrawRectangle($thin, $X, $Y, 132, 82)
    if ($Missing) {
        $Graphics.FillRectangle($mid, $X + 18, $Y + 22, 96, 34)
    }
    else {
        $Graphics.DrawString('LZ7', $font, $dark, $X + 16, $Y + 12)
        $Graphics.DrawString('LASER', $smallFont, $dark, $X + 78, $Y + 52)
        $Graphics.DrawLine($line, $X + 16, $Y + 66, $X + 116, $Y + 18)
        $Graphics.DrawEllipse($thin, $X + 82, $Y + 12, 30, 28)
    }

    $panel.Dispose()
    $dark.Dispose()
    $mid.Dispose()
    $line.Dispose()
    $thin.Dispose()
    $font.Dispose()
    $smallFont.Dispose()
}

function Save-BatteryLaserMarkSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_LaserMark_Missing_NG.png' } else { 'Battery_LaserMark_OK.png' }
    $path = Join-Path $productDir $fileName
    $templatePath = Join-Path $templateDir 'Battery_LaserMark_Template.png'
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)
    $graphics.DrawLine($labelPen, 116, 108, 356, 108)
    $graphics.DrawString('LASER MARK', $labelFont, $labelBrush, 130, 88)
    Draw-BatteryLaserMark $graphics 220 142 $Bad

    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3214 } else { 3213 }
    Add-Noise $bitmap $seed 2
    Save-Bitmap $bitmap $path

    if (-not $Bad) {
        $template = $bitmap.Clone(
            [System.Drawing.Rectangle]::new(220, 142, 132, 82),
            [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
        Save-Bitmap $template $templatePath
        $template.Dispose()
    }

    $bitmap.Dispose()
}

function Draw-BatteryTabDateCode([System.Drawing.Graphics]$Graphics, [int]$X, [int]$Y, [bool]$Wrong) {
    $panel = New-Brush 190
    $dark = New-Brush 36
    $mid = New-Brush 104
    $line = New-Pen 58 3
    $thin = New-Pen 94 2
    $font = [System.Drawing.Font]::new('Arial', 18, [System.Drawing.FontStyle]::Bold)
    $smallFont = [System.Drawing.Font]::new('Arial', 8, [System.Drawing.FontStyle]::Bold)

    $Graphics.FillRectangle($panel, $X, $Y, 140, 84)
    $Graphics.DrawRectangle($thin, $X, $Y, 140, 84)
    if ($Wrong) {
        $Graphics.FillRectangle($mid, $X + 16, $Y + 18, 108, 42)
        $Graphics.DrawLine($line, $X + 20, $Y + 66, $X + 118, $Y + 66)
    }
    else {
        $Graphics.DrawString('D24', $font, $dark, $X + 16, $Y + 12)
        $Graphics.DrawString('TAB', $smallFont, $dark, $X + 92, $Y + 54)
        $Graphics.DrawLine($line, $X + 18, $Y + 64, $X + 122, $Y + 24)
        $Graphics.DrawEllipse($thin, $X + 88, $Y + 12, 34, 30)
    }

    $panel.Dispose()
    $dark.Dispose()
    $mid.Dispose()
    $line.Dispose()
    $thin.Dispose()
    $font.Dispose()
    $smallFont.Dispose()
}

function Save-BatteryTabDateCodeSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_TabDateCode_Wrong_NG.png' } else { 'Battery_TabDateCode_OK.png' }
    $path = Join-Path $productDir $fileName
    $templatePath = Join-Path $templateDir 'Battery_TabDateCode_Template.png'
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)
    $graphics.DrawLine($labelPen, 110, 108, 370, 108)
    $graphics.DrawString('TAB DATE CODE', $labelFont, $labelBrush, 128, 88)
    Draw-BatteryTabDateCode $graphics 214 142 $Bad

    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3216 } else { 3215 }
    Add-Noise $bitmap $seed 2
    Save-Bitmap $bitmap $path

    if (-not $Bad) {
        $template = $bitmap.Clone(
            [System.Drawing.Rectangle]::new(214, 142, 140, 84),
            [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
        Save-Bitmap $template $templatePath
        $template.Dispose()
    }

    $bitmap.Dispose()
}

function Save-BatteryElectrolyteFillLineSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_ElectrolyteFillLine_Low_NG.png' } else { 'Battery_ElectrolyteFillLine_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $pouch = New-Brush 56
    $reference = New-Brush 212
    $fill = New-Brush 226
    $fluid = New-Brush 88
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($pouch, 124, 152, 190, 170)
    $graphics.FillRectangle($fluid, 126, 224, 186, 96)
    $graphics.FillRectangle($reference, 162, 184, 24, 112)
    if ($Bad) {
        $graphics.FillRectangle($fill, 204, 184, 22, 112)
    }
    else {
        $graphics.FillRectangle($fill, 226, 184, 22, 112)
    }

    $graphics.DrawLine($labelPen, 124, 128, 318, 128)
    $graphics.DrawString('ELECTROLYTE FILL', $labelFont, $labelBrush, 140, 108)

    $pouch.Dispose()
    $reference.Dispose()
    $fill.Dispose()
    $fluid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3218 } else { 3217 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryCellVentAlignmentSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_CellVentAlignment_Shifted_NG.png' } else { 'Battery_CellVentAlignment_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $cell = New-Brush 58
    $vent = New-Brush 224
    $reference = New-Brush 210
    $coating = New-Brush 92
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($cell, 120, 142, 282, 182)
    $graphics.FillRectangle($coating, 122, 236, 278, 86)
    $graphics.FillRectangle($reference, 182, 182, 24, 104)
    if ($Bad) {
        $graphics.FillRectangle($vent, 226, 182, 22, 104)
    }
    else {
        $graphics.FillRectangle($vent, 246, 182, 22, 104)
    }

    $graphics.DrawLine($labelPen, 120, 126, 392, 126)
    $graphics.DrawString('CELL VENT ALIGN', $labelFont, $labelBrush, 142, 106)

    $cell.Dispose()
    $vent.Dispose()
    $reference.Dispose()
    $coating.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3220 } else { 3219 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryPouchTabSkewSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_PouchTabSkew_Shifted_NG.png' } else { 'Battery_PouchTabSkew_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $pouch = New-Brush 56
    $seal = New-Brush 82
    $reference = New-Brush 212
    $tab = New-Brush 226
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($pouch, 112, 144, 318, 178)
    $graphics.FillRectangle($seal, 112, 236, 318, 86)
    $graphics.FillRectangle($reference, 182, 184, 24, 104)

    $tabX = if ($Bad) { 226 } else { 246 }
    $graphics.FillRectangle($tab, $tabX, 184, 22, 104)

    $graphics.DrawLine($labelPen, 112, 126, 408, 126)
    $graphics.DrawString('POUCH TAB SKEW', $labelFont, $labelBrush, 140, 106)

    $pouch.Dispose()
    $seal.Dispose()
    $reference.Dispose()
    $tab.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3222 } else { 3221 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatteryCurrentCollectorBurrSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_CurrentCollectorBurr_Many_NG.png' } else { 'Battery_CurrentCollectorBurr_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $foil = New-Brush 70
    $collector = New-Brush 102
    $burr = New-Brush 238
    $burrMid = New-Brush 216
    $edgePen = New-Pen 176 2
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($foil, 104, 142, 350, 174)
    $graphics.FillRectangle($collector, 104, 142, 350, 32)
    $graphics.DrawLine($edgePen, 104, 174, 454, 174)
    $graphics.DrawLine($labelPen, 104, 116, 426, 116)
    $graphics.DrawString('CURRENT COLLECTOR BURR', $labelFont, $labelBrush, 126, 96)

    $burrs = if ($Bad) {
        @('132,162,18,24', '190,160,20,28', '254,164,18,24', '324,160,20,28', '394,164,18,24')
    }
    else {
        @('262,164,18,24')
    }

    foreach ($item in $burrs) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $points = [System.Drawing.Point[]]@(
            [System.Drawing.Point]::new($x, $y),
            [System.Drawing.Point]::new($x + $w, $y + 4),
            [System.Drawing.Point]::new($x + [int]($w / 2), $y + $h)
        )
        $graphics.FillPolygon($burrMid, $points)
        $graphics.FillPolygon($burr, $points)
    }

    $foil.Dispose()
    $collector.Dispose()
    $burr.Dispose()
    $burrMid.Dispose()
    $edgePen.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3224 } else { 3223 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-BatterySeparatorEdgeTearSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Battery_SeparatorEdgeTear_Many_NG.png' } else { 'Battery_SeparatorEdgeTear_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-BatteryBase $graphics

    $cell = New-Brush 54
    $separator = New-Brush 92
    $coating = New-Brush 74
    $edge = New-Pen 154 2
    $tear = New-Brush 238
    $tearMid = New-Brush 214
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($cell, 86, 116, 400, 206)
    $graphics.FillRectangle($separator, 116, 142, 340, 152)
    $graphics.FillRectangle($coating, 116, 142, 340, 28)
    $graphics.DrawLine($edge, 116, 170, 456, 170)
    $graphics.DrawLine($labelPen, 118, 98, 436, 98)
    $graphics.DrawString('SEPARATOR EDGE TEAR', $labelFont, $labelBrush, 138, 78)

    $tears = if ($Bad) {
        @('138,170,22,28', '202,170,20,24', '270,170,22,30', '338,170,20,26', '410,170,22,28')
    }
    else {
        @('270,170,20,24')
    }

    foreach ($item in $tears) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $points = [System.Drawing.Point[]]@(
            [System.Drawing.Point]::new($x, $y),
            [System.Drawing.Point]::new($x + $w, $y + 2),
            [System.Drawing.Point]::new($x + [int]($w / 2), $y + $h)
        )
        $graphics.FillPolygon($tearMid, $points)
        $graphics.FillPolygon($tear, $points)
    }

    $cell.Dispose()
    $separator.Dispose()
    $coating.Dispose()
    $edge.Dispose()
    $tear.Dispose()
    $tearMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3226 } else { 3225 }
    Add-Noise $bitmap $seed 4
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Draw-DisplayBase([System.Drawing.Graphics]$Graphics) {
    $background = New-Brush 28
    $panel = New-Brush 72
    $pixelLine = New-Pen 96 1
    $border = New-Pen 168 3

    $Graphics.FillRectangle($background, 0, 0, 572, 420)
    $Graphics.FillRectangle($panel, 52, 42, 468, 310)
    $Graphics.DrawRectangle($border, 52, 42, 468, 310)

    for ($x = 78; $x -lt 500; $x += 22) {
        $Graphics.DrawLine($pixelLine, $x, 58, $x, 334)
    }

    for ($y = 66; $y -lt 332; $y += 18) {
        $Graphics.DrawLine($pixelLine, 68, $y, 506, $y)
    }

    $background.Dispose()
    $panel.Dispose()
    $pixelLine.Dispose()
    $border.Dispose()
}

function Save-DisplayDefectContourSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_PixelDefect_Many_NG.png' } else { 'Display_PixelDefect_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $defect = New-Brush 232
    $defects = if ($Bad) {
        @(@(128,90,12), @(218,124,10), @(346,98,11), @(430,148,14), @(164,232,11), @(280,258,12), @(430,284,13))
    }
    else {
        @(@(178,120,10), @(392,248,11))
    }

    foreach ($d in $defects) {
        $graphics.FillEllipse($defect, $d[0], $d[1], $d[2], $d[2])
    }

    $defect.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3202 } else { 3201 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Draw-AlignmentMark([System.Drawing.Graphics]$Graphics, [int]$X, [int]$Y, [bool]$Wrong) {
    $bright = New-Brush 222
    $mid = New-Brush 128
    $dark = New-Brush 42
    $line = New-Pen 232 5
    $thin = New-Pen 160 2

    $Graphics.FillRectangle($mid, $X, $Y, 76, 76)
    $Graphics.DrawRectangle($thin, $X, $Y, 76, 76)
    if ($Wrong) {
        $Graphics.FillEllipse($bright, $X + 18, $Y + 18, 40, 40)
        $Graphics.FillRectangle($dark, $X + 33, $Y + 8, 10, 60)
    }
    else {
        $Graphics.DrawLine($line, $X + 38, $Y + 10, $X + 38, $Y + 66)
        $Graphics.DrawLine($line, $X + 10, $Y + 38, $X + 66, $Y + 38)
        $Graphics.FillEllipse($dark, $X + 31, $Y + 31, 14, 14)
    }

    $bright.Dispose()
    $mid.Dispose()
    $dark.Dispose()
    $line.Dispose()
    $thin.Dispose()
}

function Save-DisplayAlignmentSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_Alignment_Wrong_NG.png' } else { 'Display_Alignment_OK.png' }
    $path = Join-Path $productDir $fileName
    $templatePath = Join-Path $templateDir 'Display_Alignment_Template.png'
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics
    Draw-AlignmentMark $graphics 246 170 $Bad
    $graphics.Dispose()
    $seed = if ($Bad) { 3212 } else { 3211 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path

    if (-not $Bad) {
        $template = $bitmap.Clone(
            [System.Drawing.Rectangle]::new(246, 170, 76, 76),
            [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
        Save-Bitmap $template $templatePath
        $template.Dispose()
    }

    $bitmap.Dispose()
}

function Save-DisplayScratchSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_Scratch_Many_NG.png' } else { 'Display_Scratch_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $scratch = New-Pen 232 4
    $thinScratch = New-Pen 226 3
    if ($Bad) {
        $graphics.DrawLine($scratch, 112, 102, 244, 138)
        $graphics.DrawLine($thinScratch, 316, 92, 456, 122)
        $graphics.DrawLine($scratch, 142, 250, 294, 284)
        $graphics.DrawLine($thinScratch, 344, 244, 470, 294)
    }
    else {
        $graphics.DrawLine($scratch, 184, 214, 304, 242)
    }

    $scratch.Dispose()
    $thinScratch.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3222 } else { 3221 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayBrightnessBandSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_BrightnessBand_Bright_NG.png' } else { 'Display_BrightnessBand_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $band = if ($Bad) { New-Brush 142 } else { New-Brush 82 }
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($band, 78, 146, 416, 72)
    $graphics.DrawLine($labelPen, 86, 116, 250, 116)
    $graphics.DrawString('MURA BAND', $labelFont, $labelBrush, 96, 96)

    $band.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3232 } else { 3231 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayParticleSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_Particle_Many_NG.png' } else { 'Display_Particle_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $particle = New-Brush 236
    $shadow = New-Brush 118
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 96, 116, 260, 116)
    $graphics.DrawString('PARTICLE', $labelFont, $labelBrush, 106, 96)

    $spots = if ($Bad) {
        @('124,92,11', '214,126,12', '336,102,10', '436,156,13', '188,250,11', '408,278,12')
    }
    else {
        @('278,188,11')
    }

    foreach ($spot in $spots) {
        $parts = $spot.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $s = [int]$parts[2]
        $graphics.FillEllipse($shadow, $x + 2, $y + 2, $s, $s)
        $graphics.FillEllipse($particle, $x, $y, $s, $s)
    }

    $particle.Dispose()
    $shadow.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3242 } else { 3241 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayMuraVariationSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_MuraVariation_Uneven_NG.png' } else { 'Display_MuraVariation_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)
    $graphics.DrawLine($labelPen, 90, 96, 320, 96)
    $graphics.DrawString('MURA VARIATION', $labelFont, $labelBrush, 100, 76)

    if ($Bad) {
        $stripeA = New-Brush 112
        $stripeB = New-Brush 146
        for ($i = 0; $i -lt 8; $i++) {
            $brush = if (($i % 2) -eq 0) { $stripeA } else { $stripeB }
            $graphics.FillRectangle($brush, 88 + ($i * 50), 128, 50, 112)
        }

        $stripeA.Dispose()
        $stripeB.Dispose()
    }
    else {
        $normalA = New-Brush 82
        $normalB = New-Brush 88
        for ($i = 0; $i -lt 8; $i++) {
            $brush = if (($i % 2) -eq 0) { $normalA } else { $normalB }
            $graphics.FillRectangle($brush, 88 + ($i * 50), 128, 50, 112)
        }

        $normalA.Dispose()
        $normalB.Dispose()
    }

    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3252 } else { 3251 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayEdgeChipSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_EdgeChip_Many_NG.png' } else { 'Display_EdgeChip_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $chip = New-Brush 236
    $shadow = New-Brush 118
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 320, 96, 506, 96)
    $graphics.DrawString('EDGE CHIP', $labelFont, $labelBrush, 336, 76)

    $chips = if ($Bad) {
        @('92,44,18,12', '176,44,16,12', '278,44,20,12', '388,44,18,12', '476,44,16,12')
    }
    else {
        @('278,44,18,12')
    }

    foreach ($item in $chips) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($shadow, $x + 2, $y + 2, $w, $h)
        $graphics.FillEllipse($chip, $x, $y, $w, $h)
    }

    $chip.Dispose()
    $shadow.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3262 } else { 3261 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayBezelChipSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_BezelChip_Many_NG.png' } else { 'Display_BezelChip_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $bezel = New-Brush 42
    $inner = New-Pen 138 2
    $chip = New-Brush 238
    $shadow = New-Brush 112
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($bezel, 54, 44, 466, 34)
    $graphics.FillRectangle($bezel, 54, 318, 466, 34)
    $graphics.FillRectangle($bezel, 54, 44, 34, 308)
    $graphics.FillRectangle($bezel, 486, 44, 34, 308)
    $graphics.DrawRectangle($inner, 92, 82, 388, 232)
    $graphics.DrawLine($labelPen, 310, 108, 500, 108)
    $graphics.DrawString('BEZEL CHIP', $labelFont, $labelBrush, 326, 88)

    $chips = if ($Bad) {
        @('108,52,18,12', '188,52,17,12', '286,52,20,13', '390,52,18,12', '496,152,12,20')
    }
    else {
        @('286,52,18,12')
    }

    foreach ($item in $chips) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($shadow, $x + 2, $y + 2, $w, $h)
        $graphics.FillEllipse($chip, $x, $y, $w, $h)
    }

    $bezel.Dispose()
    $inner.Dispose()
    $chip.Dispose()
    $shadow.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3264 } else { 3263 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayCornerCrackSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_CornerCrack_Many_NG.png' } else { 'Display_CornerCrack_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $crack = New-Pen 236 4
    $thinCrack = New-Pen 232 3
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 68, 96, 246, 96)
    $graphics.DrawString('CORNER CRACK', $labelFont, $labelBrush, 78, 76)

    if ($Bad) {
        $graphics.DrawLine($crack, 74, 68, 148, 124)
        $graphics.DrawLine($thinCrack, 96, 48, 186, 88)
        $graphics.DrawLine($crack, 58, 136, 152, 168)
        $graphics.DrawLine($thinCrack, 128, 104, 224, 150)
    }
    else {
        $graphics.DrawLine($thinCrack, 86, 72, 128, 94)
    }

    $crack.Dispose()
    $thinCrack.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3272 } else { 3271 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayAlignmentOffsetSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_AlignmentOffset_Shifted_NG.png' } else { 'Display_AlignmentOffset_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $dark = New-Brush 54
    $reference = New-Brush 226
    $mark = New-Brush 216
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($dark, 190, 152, 174, 158)
    $graphics.FillRectangle($reference, 220, 184, 24, 92)
    if ($Bad) {
        $graphics.FillRectangle($mark, 264, 184, 30, 92)
    }
    else {
        $graphics.FillRectangle($mark, 284, 184, 30, 92)
    }

    $graphics.DrawLine($labelPen, 188, 126, 366, 126)
    $graphics.DrawString('ALIGNMENT OFFSET', $labelFont, $labelBrush, 200, 106)

    $dark.Dispose()
    $reference.Dispose()
    $mark.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3282 } else { 3281 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayColorFilterShiftSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_ColorFilterShift_Shifted_NG.png' } else { 'Display_ColorFilterShift_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $dark = New-Brush 54
    $reference = New-Brush 228
    $filter = New-Brush 214
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($dark, 190, 152, 174, 158)
    $graphics.FillRectangle($reference, 220, 184, 24, 92)
    if ($Bad) {
        $graphics.FillRectangle($filter, 264, 184, 30, 92)
    }
    else {
        $graphics.FillRectangle($filter, 284, 184, 30, 92)
    }

    $graphics.DrawLine($labelPen, 188, 126, 368, 126)
    $graphics.DrawString('COLOR FILTER SHIFT', $labelFont, $labelBrush, 198, 106)

    $dark.Dispose()
    $reference.Dispose()
    $filter.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3284 } else { 3283 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayLineStainSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_LineStain_Many_NG.png' } else { 'Display_LineStain_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $stain = New-Pen 236 4
    $stainThin = New-Pen 232 3
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 116, 104, 328, 104)
    $graphics.DrawString('LINE STAIN', $labelFont, $labelBrush, 128, 84)

    if ($Bad) {
        $graphics.DrawLine($stain, 132, 150, 450, 152)
        $graphics.DrawLine($stainThin, 148, 196, 432, 202)
        $graphics.DrawLine($stain, 118, 244, 470, 250)
        $graphics.DrawLine($stainThin, 170, 292, 402, 296)
    }
    else {
        $graphics.DrawLine($stainThin, 178, 188, 270, 190)
    }

    $stain.Dispose()
    $stainThin.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3292 } else { 3291 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplaySubpixelBridgeSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_SubpixelBridge_Many_NG.png' } else { 'Display_SubpixelBridge_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $cell = New-Brush 116
    $cellAlt = New-Brush 148
    $bridge = New-Brush 238
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 108, 104, 386, 104)
    $graphics.DrawString('SUBPIXEL BRIDGE', $labelFont, $labelBrush, 120, 84)

    for ($row = 0; $row -lt 5; $row++) {
        for ($col = 0; $col -lt 9; $col++) {
            $x = 116 + ($col * 38)
            $y = 138 + ($row * 34)
            $brush = if ((($row + $col) % 2) -eq 0) { $cell } else { $cellAlt }
            $graphics.FillRectangle($brush, $x, $y, 12, 24)
            $graphics.FillRectangle($brush, $x + 15, $y, 12, 24)
            $graphics.FillRectangle($brush, $x + 30, $y, 12, 24)
        }
    }

    $bridges = if ($Bad) {
        @('146,148,19,8', '260,182,19,8', '374,216,19,8', '184,250,19,8', '336,284,19,8')
    }
    else {
        @('260,182,18,7')
    }

    foreach ($item in $bridges) {
        $parts = $item.Split(',')
        $graphics.FillRectangle($bridge, [int]$parts[0], [int]$parts[1], [int]$parts[2], [int]$parts[3])
    }

    $cell.Dispose()
    $cellAlt.Dispose()
    $bridge.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3294 } else { 3293 }
    Add-Noise $bitmap $seed 2
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayPadBridgeSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_PadBridge_Many_NG.png' } else { 'Display_PadBridge_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $inspectionBand = New-Brush 66
    $pad = New-Brush 152
    $padEdge = New-Pen 116 1
    $bridge = New-Brush 238
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($inspectionBand, 86, 132, 398, 166)
    $graphics.DrawLine($labelPen, 108, 104, 356, 104)
    $graphics.DrawString('DISPLAY PAD BRIDGE', $labelFont, $labelBrush, 120, 84)

    for ($row = 0; $row -lt 3; $row++) {
        for ($col = 0; $col -lt 8; $col++) {
            $x = 116 + ($col * 42)
            $y = 158 + ($row * 42)
            $graphics.FillRectangle($pad, $x, $y, 25, 24)
            $graphics.DrawRectangle($padEdge, $x, $y, 25, 24)
        }
    }

    $bridges = if ($Bad) {
        @('142,166,18,10', '226,208,18,10', '310,166,18,10', '352,250,18,10', '436,208,18,10')
    }
    else {
        @('310,208,17,9')
    }

    foreach ($item in $bridges) {
        $parts = $item.Split(',')
        $graphics.FillRectangle($bridge, [int]$parts[0], [int]$parts[1], [int]$parts[2], [int]$parts[3])
    }

    $inspectionBand.Dispose()
    $pad.Dispose()
    $padEdge.Dispose()
    $bridge.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3296 } else { 3295 }
    Add-Noise $bitmap $seed 2
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayPolarizerBubbleSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_PolarizerBubble_Many_NG.png' } else { 'Display_PolarizerBubble_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $inspectionBand = New-Brush 76
    $bubble = New-Brush 238
    $bubbleMid = New-Brush 220
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($inspectionBand, 84, 118, 404, 190)
    $graphics.DrawLine($labelPen, 106, 104, 382, 104)
    $graphics.DrawString('POLARIZER BUBBLE', $labelFont, $labelBrush, 118, 84)

    $bubbles = if ($Bad) {
        @('134,150,26,22', '234,134,34,28', '356,164,28,24', '190,242,30,24', '404,238,32,26')
    }
    else {
        @('286,184,28,22')
    }

    foreach ($item in $bubbles) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($bubbleMid, $x + 3, $y + 3, $w, $h)
        $graphics.FillEllipse($bubble, $x, $y, $w, $h)
    }

    $inspectionBand.Dispose()
    $bubble.Dispose()
    $bubbleMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3296 } else { 3295 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplaySealContaminationSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_SealContamination_Many_NG.png' } else { 'Display_SealContamination_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $sealBand = New-Brush 78
    $contamination = New-Brush 238
    $contaminationMid = New-Brush 218
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($sealBand, 88, 122, 398, 184)
    $graphics.DrawLine($labelPen, 106, 104, 420, 104)
    $graphics.DrawString('SEAL CONTAMINATION', $labelFont, $labelBrush, 118, 84)

    $spots = if ($Bad) {
        @('142,152,24,18', '222,136,26,20', '334,166,24,18', '188,242,28,18', '416,232,24,20')
    }
    else {
        @('286,188,24,18')
    }

    foreach ($item in $spots) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($contaminationMid, $x + 3, $y + 3, $w, $h)
        $graphics.FillEllipse($contamination, $x, $y, $w, $h)
    }

    $sealBand.Dispose()
    $contamination.Dispose()
    $contaminationMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3298 } else { 3297 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplaySealCornerContaminationSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_SealCornerContamination_Many_NG.png' } else { 'Display_SealCornerContamination_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $glass = New-Brush 64
    $seal = New-Brush 92
    $corner = New-Brush 76
    $contamination = New-Brush 238
    $contaminationMid = New-Brush 218
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($glass, 82, 116, 408, 220)
    $graphics.FillRectangle($seal, 96, 130, 380, 32)
    $graphics.FillRectangle($seal, 96, 130, 34, 182)
    $graphics.FillRectangle($corner, 96, 130, 92, 78)
    $graphics.DrawLine($labelPen, 98, 98, 418, 98)
    $graphics.DrawString('SEAL CORNER CONTAMINATION', $labelFont, $labelBrush, 112, 78)

    $spots = if ($Bad) {
        @('116,146,20,14', '148,146,18,14', '116,176,18,14', '162,180,18,14', '138,202,20,14')
    }
    else {
        @('136,166,20,14')
    }

    foreach ($item in $spots) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($contaminationMid, $x + 2, $y + 2, $w, $h)
        $graphics.FillEllipse($contamination, $x, $y, $w, $h)
    }

    $glass.Dispose()
    $seal.Dispose()
    $corner.Dispose()
    $contamination.Dispose()
    $contaminationMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3338 } else { 3337 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayPolarizerEdgeLiftSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_PolarizerEdgeLift_Many_NG.png' } else { 'Display_PolarizerEdgeLift_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $panel = New-Brush 58
    $film = New-Brush 72
    $edge = New-Brush 92
    $lift = New-Brush 238
    $liftMid = New-Brush 218
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($panel, 82, 118, 408, 192)
    $graphics.FillRectangle($film, 104, 142, 364, 144)
    $graphics.FillRectangle($edge, 104, 142, 364, 20)
    $graphics.DrawLine($labelPen, 106, 98, 420, 98)
    $graphics.DrawString('POLARIZER EDGE LIFT', $labelFont, $labelBrush, 120, 78)

    $lifts = if ($Bad) {
        @(
            '128,144,166,164,132,176',
            '198,144,238,162,206,178',
            '276,144,314,164,284,178',
            '348,144,390,162,358,178',
            '424,144,462,164,432,178'
        )
    }
    else {
        @('246,144,286,162,256,178')
    }

    foreach ($item in $lifts) {
        $parts = $item.Split(',')
        $points = [System.Drawing.Point[]]@(
            [System.Drawing.Point]::new([int]$parts[0], [int]$parts[1]),
            [System.Drawing.Point]::new([int]$parts[2], [int]$parts[3]),
            [System.Drawing.Point]::new([int]$parts[4], [int]$parts[5])
        )
        $graphics.FillPolygon($liftMid, $points)
        $graphics.FillPolygon($lift, $points)
    }

    $panel.Dispose()
    $film.Dispose()
    $edge.Dispose()
    $lift.Dispose()
    $liftMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3340 } else { 3339 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayCofBondParticleSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_CofBondParticle_Many_NG.png' } else { 'Display_CofBondParticle_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $glass = New-Brush 58
    $bond = New-Brush 92
    $pad = New-Brush 138
    $particle = New-Brush 238
    $particleMid = New-Brush 216
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($glass, 84, 118, 404, 204)
    $graphics.FillRectangle($bond, 104, 236, 364, 42)
    for ($x = 118; $x -le 424; $x += 36) {
        $graphics.FillRectangle($pad, $x, 244, 20, 28)
    }
    $graphics.DrawLine($labelPen, 106, 98, 420, 98)
    $graphics.DrawString('COF BOND PARTICLE', $labelFont, $labelBrush, 120, 78)

    $particles = if ($Bad) {
        @('128,224,16,12', '182,252,14,12', '246,222,16,13', '326,252,14,12', '404,224,16,12')
    }
    else {
        @('246,222,16,13')
    }

    foreach ($item in $particles) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($particleMid, $x + 2, $y + 2, $w, $h)
        $graphics.FillEllipse($particle, $x, $y, $w, $h)
    }

    $glass.Dispose()
    $bond.Dispose()
    $pad.Dispose()
    $particle.Dispose()
    $particleMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3342 } else { 3341 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Draw-FpcAlignmentMark([System.Drawing.Graphics]$Graphics, [int]$X, [int]$Y, [bool]$Wrong) {
    $panel = New-Brush 214
    $dark = New-Brush 38
    $mid = New-Brush 118
    $line = New-Pen 64 3
    $thin = New-Pen 92 1
    $font = [System.Drawing.Font]::new('Arial', 14, [System.Drawing.FontStyle]::Bold)
    $smallFont = [System.Drawing.Font]::new('Arial', 8, [System.Drawing.FontStyle]::Bold)

    $Graphics.FillRectangle($panel, $X, $Y, 128, 88)
    $Graphics.DrawRectangle($line, $X, $Y, 128, 88)
    if ($Wrong) {
        $Graphics.FillRectangle($mid, $X + 18, $Y + 22, 92, 38)
        $Graphics.DrawEllipse($line, $X + 44, $Y + 20, 40, 40)
        $Graphics.DrawLine($thin, $X + 12, $Y + 74, $X + 116, $Y + 74)
    }
    else {
        $Graphics.DrawString('FPC', $font, $dark, $X + 14, $Y + 12)
        $Graphics.DrawString('A1', $smallFont, $dark, $X + 86, $Y + 12)
        $Graphics.DrawLine($line, $X + 18, $Y + 62, $X + 110, $Y + 24)
        $Graphics.DrawRectangle($line, $X + 72, $Y + 44, 30, 24)
        $Graphics.DrawLine($thin, $X + 16, $Y + 74, $X + 112, $Y + 74)
    }

    $panel.Dispose()
    $dark.Dispose()
    $mid.Dispose()
    $line.Dispose()
    $thin.Dispose()
    $font.Dispose()
    $smallFont.Dispose()
}

function Save-DisplayFpcAlignmentMarkSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_FpcAlignmentMark_Wrong_NG.png' } else { 'Display_FpcAlignmentMark_OK.png' }
    $path = Join-Path $productDir $fileName
    $templatePath = Join-Path $templateDir 'Display_FpcAlignmentMark_Template.png'
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $glass = New-Brush 56
    $fpc = New-Brush 96
    $trace = New-Pen 150 2
    $pad = New-Brush 142
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($glass, 72, 116, 428, 206)
    $graphics.FillRectangle($fpc, 184, 252, 204, 58)
    for ($x = 198; $x -le 354; $x += 26) {
        $graphics.FillRectangle($pad, $x, 260, 14, 40)
        $graphics.DrawLine($trace, $x + 7, 154, $x + 7, 252)
    }
    $graphics.DrawLine($labelPen, 164, 98, 410, 98)
    $graphics.DrawString('FPC ALIGNMENT MARK', $labelFont, $labelBrush, 178, 78)
    Draw-FpcAlignmentMark $graphics 222 150 $Bad

    $glass.Dispose()
    $fpc.Dispose()
    $trace.Dispose()
    $pad.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3344 } else { 3343 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path

    if (-not $Bad) {
        $template = $bitmap.Clone(
            [System.Drawing.Rectangle]::new(222, 150, 128, 88),
            [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
        Save-Bitmap $template $templatePath
        $template.Dispose()
    }

    $bitmap.Dispose()
}

function Save-DisplayCornerLightLeakSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_CornerLightLeak_Bright_NG.png' } else { 'Display_CornerLightLeak_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $panel = New-Brush 86
    $leakGood = New-Brush 98
    $leakBad = New-Brush 146
    $mask = New-Brush 72
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($panel, 84, 118, 404, 202)
    $graphics.FillRectangle($mask, 104, 138, 364, 164)
    $graphics.DrawLine($labelPen, 106, 104, 420, 104)
    $graphics.DrawString('CORNER LIGHT LEAK', $labelFont, $labelBrush, 118, 84)

    if ($Bad) {
        $graphics.FillEllipse($leakBad, 78, 108, 182, 164)
        $graphics.FillEllipse($leakBad, 112, 138, 126, 110)
    }
    else {
        $graphics.FillEllipse($leakGood, 102, 132, 84, 72)
    }

    $panel.Dispose()
    $leakGood.Dispose()
    $leakBad.Dispose()
    $mask.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3300 } else { 3299 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayBlackMatrixScratchSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_BlackMatrixScratch_Many_NG.png' } else { 'Display_BlackMatrixScratch_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $matrix = New-Brush 52
    $cell = New-Brush 92
    $scratch = New-Pen 238 4
    $scratchThin = New-Pen 232 3
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($matrix, 92, 128, 388, 172)
    for ($row = 0; $row -lt 4; $row++) {
        for ($col = 0; $col -lt 8; $col++) {
            $graphics.FillRectangle($cell, 112 + ($col * 42), 150 + ($row * 34), 24, 18)
        }
    }

    $graphics.DrawLine($labelPen, 106, 108, 436, 108)
    $graphics.DrawString('BLACK MATRIX SCRATCH', $labelFont, $labelBrush, 120, 88)

    if ($Bad) {
        $graphics.DrawLine($scratch, 126, 154, 212, 174)
        $graphics.DrawLine($scratchThin, 222, 204, 324, 190)
        $graphics.DrawLine($scratch, 330, 158, 438, 186)
        $graphics.DrawLine($scratchThin, 150, 250, 252, 236)
        $graphics.DrawLine($scratch, 308, 260, 426, 238)
    }
    else {
        $graphics.DrawLine($scratchThin, 250, 202, 320, 192)
    }

    $matrix.Dispose()
    $cell.Dispose()
    $scratch.Dispose()
    $scratchThin.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3304 } else { 3303 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayLineDropoutSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_LineDropout_Many_NG.png' } else { 'Display_LineDropout_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $panel = New-Brush 56
    $line = New-Pen 112 4
    $dropout = New-Brush 238
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($panel, 82, 124, 408, 184)
    for ($i = 0; $i -lt 5; $i++) {
        $y = 154 + ($i * 30)
        $graphics.DrawLine($line, 112, $y, 460, $y)
    }

    $graphics.DrawLine($labelPen, 106, 108, 350, 108)
    $graphics.DrawString('LINE DROPOUT', $labelFont, $labelBrush, 120, 88)

    $gaps = if ($Bad) {
        @('148,148,28,12', '232,178,30,12', '338,208,30,12', '178,238,28,12', '388,268,30,12')
    }
    else {
        @('278,208,28,12')
    }

    foreach ($item in $gaps) {
        $parts = $item.Split(',')
        $graphics.FillRectangle($dropout, [int]$parts[0], [int]$parts[1], [int]$parts[2], [int]$parts[3])
    }

    $panel.Dispose()
    $line.Dispose()
    $dropout.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3306 } else { 3305 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayMuraSpotClusterSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_MuraSpotCluster_Many_NG.png' } else { 'Display_MuraSpotCluster_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $inspection = New-Brush 64
    $spot = New-Brush 238
    $spotMid = New-Brush 202
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($inspection, 82, 118, 408, 192)
    $graphics.DrawLine($labelPen, 106, 98, 396, 98)
    $graphics.DrawString('MURA SPOT CLUSTER', $labelFont, $labelBrush, 120, 78)

    $spots = if ($Bad) {
        @('132,150,24,20', '214,196,25,20', '292,144,24,20', '358,226,25,20', '420,176,24,20')
    }
    else {
        @('292,196,23,19')
    }

    foreach ($item in $spots) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($spotMid, $x + 3, $y + 3, $w, $h)
        $graphics.FillEllipse($spot, $x, $y, $w, $h)
    }

    $inspection.Dispose()
    $spot.Dispose()
    $spotMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3308 } else { 3307 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayMuraRingSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_MuraRing_Many_NG.png' } else { 'Display_MuraRing_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $inspection = New-Brush 62
    $ring = New-Brush 238
    $ringMid = New-Brush 202
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($inspection, 82, 118, 408, 192)
    $graphics.DrawLine($labelPen, 106, 98, 326, 98)
    $graphics.DrawString('MURA RING', $labelFont, $labelBrush, 120, 78)

    $rings = if ($Bad) {
        @('126,146,34,28', '216,198,36,30', '294,142,34,28', '356,226,36,30', '420,174,34,28')
    }
    else {
        @('292,194,34,28')
    }

    foreach ($item in $rings) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($ringMid, $x + 3, $y + 3, $w, $h)
        $graphics.FillEllipse($ring, $x, $y, $w, $h)
        $graphics.FillEllipse($inspection, $x + 9, $y + 7, $w - 18, $h - 14)
    }

    $inspection.Dispose()
    $ring.Dispose()
    $ringMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3310 } else { 3309 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayPolarizerScratchSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_PolarizerScratch_Many_NG.png' } else { 'Display_PolarizerScratch_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $panel = New-Brush 58
    $film = New-Brush 72
    $scratch = New-Pen 238 4
    $scratchThin = New-Pen 232 3
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($panel, 82, 118, 408, 192)
    $graphics.FillRectangle($film, 104, 142, 364, 144)
    $graphics.DrawLine($labelPen, 106, 98, 420, 98)
    $graphics.DrawString('POLARIZER SCRATCH', $labelFont, $labelBrush, 120, 78)

    if ($Bad) {
        $graphics.DrawLine($scratch, 126, 166, 214, 150)
        $graphics.DrawLine($scratchThin, 220, 206, 324, 190)
        $graphics.DrawLine($scratch, 330, 164, 438, 188)
        $graphics.DrawLine($scratchThin, 146, 252, 252, 232)
        $graphics.DrawLine($scratch, 306, 262, 424, 238)
    }
    else {
        $graphics.DrawLine($scratchThin, 246, 204, 320, 190)
    }

    $panel.Dispose()
    $film.Dispose()
    $scratch.Dispose()
    $scratchThin.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3312 } else { 3311 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplayPolarizerCreaseSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_PolarizerCrease_Many_NG.png' } else { 'Display_PolarizerCrease_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $panel = New-Brush 58
    $film = New-Brush 70
    $crease = New-Pen 238 5
    $creaseThin = New-Pen 232 4
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($panel, 82, 118, 408, 192)
    $graphics.FillRectangle($film, 104, 142, 364, 144)
    $graphics.DrawLine($labelPen, 106, 98, 420, 98)
    $graphics.DrawString('POLARIZER CREASE', $labelFont, $labelBrush, 120, 78)

    if ($Bad) {
        $graphics.DrawBezier($crease, 128, 172, 172, 144, 208, 206, 252, 178)
        $graphics.DrawBezier($creaseThin, 242, 224, 282, 188, 314, 246, 356, 212)
        $graphics.DrawBezier($crease, 330, 162, 368, 142, 398, 196, 438, 166)
        $graphics.DrawBezier($creaseThin, 146, 254, 196, 236, 224, 286, 276, 260)
        $graphics.DrawBezier($crease, 310, 270, 350, 242, 386, 286, 428, 252)
    }
    else {
        $graphics.DrawBezier($creaseThin, 236, 210, 270, 182, 304, 226, 340, 196)
    }

    $panel.Dispose()
    $film.Dispose()
    $crease.Dispose()
    $creaseThin.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3314 } else { 3313 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-DisplaySealWidthSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Display_SealWidth_Narrow_NG.png' } else { 'Display_SealWidth_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-DisplayBase $graphics

    $dark = New-Brush 54
    $reference = New-Brush 226
    $seal = New-Brush 216
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($dark, 190, 152, 174, 158)
    $graphics.FillRectangle($reference, 220, 184, 24, 92)
    if ($Bad) {
        $graphics.FillRectangle($seal, 264, 184, 30, 92)
    }
    else {
        $graphics.FillRectangle($seal, 284, 184, 30, 92)
    }

    $graphics.DrawLine($labelPen, 188, 126, 366, 126)
    $graphics.DrawString('DISPLAY SEAL WIDTH', $labelFont, $labelBrush, 200, 106)

    $dark.Dispose()
    $reference.Dispose()
    $seal.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3316 } else { 3315 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Draw-SemiconductorBase([System.Drawing.Graphics]$Graphics) {
    $background = New-Brush 36
    $die = New-Brush 92
    $pad = New-Brush 178
    $trace = New-Pen 132 2
    $edge = New-Pen 170 3

    $Graphics.FillRectangle($background, 0, 0, 572, 420)
    $Graphics.FillRectangle($die, 68, 48, 430, 294)
    $Graphics.DrawRectangle($edge, 68, 48, 430, 294)

    for ($i = 0; $i -lt 9; $i++) {
        $x = 92 + ($i * 42)
        $Graphics.FillRectangle($pad, $x, 300, 24, 24)
        $Graphics.DrawLine($trace, $x + 12, 300, $x + 28, 90)
    }

    $background.Dispose()
    $die.Dispose()
    $pad.Dispose()
    $trace.Dispose()
    $edge.Dispose()
}

function Draw-EdgeFiducial([System.Drawing.Graphics]$Graphics, [int]$X, [int]$Y, [bool]$Wrong) {
    $box = New-Brush 118
    $bright = New-Brush 228
    $dark = New-Brush 44
    $line = New-Pen 220 4

    $Graphics.FillRectangle($box, $X, $Y, 92, 92)
    if ($Wrong) {
        $Graphics.FillEllipse($bright, $X + 18, $Y + 18, 54, 54)
        $Graphics.FillRectangle($dark, $X + 36, $Y + 36, 20, 20)
    }
    else {
        $Graphics.FillRectangle($bright, $X + 18, $Y + 18, 22, 58)
        $Graphics.FillRectangle($bright, $X + 18, $Y + 54, 58, 22)
        $Graphics.FillRectangle($dark, $X + 43, $Y + 28, 30, 24)
        $Graphics.DrawLine($line, $X + 18, $Y + 80, $X + 78, $Y + 20)
    }

    $box.Dispose()
    $bright.Dispose()
    $dark.Dispose()
    $line.Dispose()
}

function Save-SemiconductorFiducialSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_Fiducial_Wrong_NG.png' } else { 'Semiconductor_Fiducial_OK.png' }
    $path = Join-Path $productDir $fileName
    $templatePath = Join-Path $templateDir 'Semiconductor_Fiducial_Template.png'
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics
    Draw-EdgeFiducial $graphics 240 142 $Bad
    $graphics.Dispose()
    $seed = if ($Bad) { 3302 } else { 3301 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path

    if (-not $Bad) {
        $template = $bitmap.Clone(
            [System.Drawing.Rectangle]::new(240, 142, 92, 92),
            [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
        Save-Bitmap $template $templatePath
        $template.Dispose()
    }

    $bitmap.Dispose()
}

function Draw-FeatureMark([System.Drawing.Graphics]$Graphics, [bool]$Wrong) {
    $panel = New-Brush 218
    $dark = New-Brush 48
    $mid = New-Brush 118
    $line = New-Pen 66 3
    $thin = New-Pen 88 1
    $font = [System.Drawing.Font]::new('Arial', 18, [System.Drawing.FontStyle]::Bold)
    $smallFont = [System.Drawing.Font]::new('Arial', 10, [System.Drawing.FontStyle]::Bold)

    $Graphics.FillRectangle($panel, 156, 112, 260, 160)
    $Graphics.DrawRectangle($line, 156, 112, 260, 160)
    if ($Wrong) {
        $Graphics.DrawString('Q9', $font, $dark, 178, 132)
        $Graphics.DrawEllipse($thin, 202, 186, 44, 44)
        $Graphics.FillRectangle($dark, 310, 178, 42, 32)
        $Graphics.DrawLine($line, 178, 250, 392, 126)
        for ($i = 0; $i -lt 14; $i++) {
            $Graphics.FillEllipse($mid, 184 + (($i * 29) % 205), 126 + (($i * 47) % 118), 5, 5)
        }
    }
    else {
        $Graphics.DrawString('D7', $font, $dark, 178, 132)
        $Graphics.DrawString('PAD', $smallFont, $dark, 302, 136)
        $Graphics.DrawEllipse($line, 204, 184, 46, 46)
        $Graphics.FillEllipse($mid, 250, 178, 22, 22)
        $Graphics.FillRectangle($dark, 316, 186, 46, 30)
        $Graphics.DrawLine($line, 180, 236, 390, 142)
        for ($i = 0; $i -lt 18; $i++) {
            $Graphics.FillEllipse($mid, 178 + (($i * 31) % 210), 126 + (($i * 43) % 120), 5, 5)
        }
    }

    $panel.Dispose()
    $dark.Dispose()
    $mid.Dispose()
    $line.Dispose()
    $thin.Dispose()
    $font.Dispose()
    $smallFont.Dispose()
}

function Save-SemiconductorFeatureSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_BondMark_Wrong_NG.png' } else { 'Semiconductor_BondMark_OK.png' }
    $path = Join-Path $productDir $fileName
    $templatePath = Join-Path $templateDir 'Semiconductor_BondMark_Template.png'
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics
    Draw-FeatureMark $graphics $Bad
    $graphics.Dispose()
    $seed = if ($Bad) { 3312 } else { 3311 }
    Add-Noise $bitmap $seed 2
    Save-Bitmap $bitmap $path

    if (-not $Bad) {
        $template = $bitmap.Clone(
            [System.Drawing.Rectangle]::new(156, 112, 260, 160),
            [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
        Save-Bitmap $template $templatePath
        $template.Dispose()
    }

    $bitmap.Dispose()
}

function Save-SemiconductorPadContaminationSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_PadContamination_Heavy_NG.png' } else { 'Semiconductor_PadContamination_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $contamination = New-Brush 232
    $spots = if ($Bad) {
        @('104,304,9', '146,304,8', '190,304,10', '232,304,8', '314,304,10', '398,304,9')
    }
    else {
        @('190,304,8')
    }

    foreach ($spot in $spots) {
        $parts = $spot.Split(',')
        $graphics.FillEllipse($contamination, [int]$parts[0], [int]$parts[1], [int]$parts[2], [int]$parts[2])
    }

    $contamination.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3322 } else { 3321 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorDieContaminationSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_DieContamination_Heavy_NG.png' } else { 'Semiconductor_DieContamination_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $particle = New-Brush 234
    $shadow = New-Brush 126
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 120, 112, 340, 112)
    $graphics.DrawString('DIE CONTAMINATION', $labelFont, $labelBrush, 132, 92)

    $spots = if ($Bad) {
        @('132,140,12', '212,174,10', '288,132,13', '376,190,11', '170,248,10', '330,256,12')
    }
    else {
        @('288,164,10')
    }

    foreach ($spot in $spots) {
        $parts = $spot.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $s = [int]$parts[2]
        $graphics.FillEllipse($shadow, $x + 2, $y + 2, $s, $s)
        $graphics.FillEllipse($particle, $x, $y, $s, $s)
    }

    $particle.Dispose()
    $shadow.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3328 } else { 3327 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorUnderfillVoidSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_UnderfillVoid_Many_NG.png' } else { 'Semiconductor_UnderfillVoid_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $underfill = New-Brush 70
    $void = New-Brush 236
    $rim = New-Pen 196 1
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($underfill, 150, 122, 278, 144)
    $graphics.DrawLine($labelPen, 150, 98, 376, 98)
    $graphics.DrawString('UNDERFILL VOID', $labelFont, $labelBrush, 166, 78)

    if ($Bad) {
        $voids = @(@(178,152,14), @(228,204,16), @(286,162,13), @(342,214,18), @(394,174,15))
    }
    else {
        $voids = @(,@(284,186,14))
    }

    foreach ($item in $voids) {
        $graphics.FillEllipse($void, $item[0], $item[1], $item[2], $item[2])
        $graphics.DrawEllipse($rim, $item[0], $item[1], $item[2], $item[2])
    }

    $underfill.Dispose()
    $void.Dispose()
    $rim.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3336 } else { 3335 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorPackageVoidSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_PackageVoid_Many_NG.png' } else { 'Semiconductor_PackageVoid_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $package = New-Brush 54
    $mold = New-Brush 68
    $die = New-Brush 96
    $void = New-Brush 238
    $voidMid = New-Brush 218
    $rim = New-Pen 196 1
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($package, 132, 116, 308, 214)
    $graphics.FillRectangle($mold, 158, 144, 256, 156)
    $graphics.FillRectangle($die, 214, 178, 144, 82)
    $graphics.DrawLine($labelPen, 132, 98, 432, 98)
    $graphics.DrawString('PACKAGE VOID', $labelFont, $labelBrush, 166, 78)

    $voids = if ($Bad) {
        @('184,160,16', '244,204,18', '302,166,15', '364,224,18', '390,178,14')
    }
    else {
        @('280,204,15')
    }

    foreach ($item in $voids) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $size = [int]$parts[2]
        $graphics.FillEllipse($voidMid, $x + 2, $y + 2, $size, $size)
        $graphics.FillEllipse($void, $x, $y, $size, $size)
        $graphics.DrawEllipse($rim, $x, $y, $size, $size)
    }

    $package.Dispose()
    $mold.Dispose()
    $die.Dispose()
    $void.Dispose()
    $voidMid.Dispose()
    $rim.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3358 } else { 3357 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorSolderBridgeSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_SolderBridge_Many_NG.png' } else { 'Semiconductor_SolderBridge_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $bridge = New-Brush 236
    $shadow = New-Brush 118
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 96, 270, 342, 270)
    $graphics.DrawString('SOLDER BRIDGE', $labelFont, $labelBrush, 108, 250)

    $bridges = if ($Bad) {
        @('120,302,18,18', '164,302,18,18', '208,302,18,18', '292,302,18,18', '376,302,18,18')
    }
    else {
        @('208,302,16,16')
    }

    foreach ($item in $bridges) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillRectangle($shadow, $x + 2, $y + 2, $w, $h)
        $graphics.FillRectangle($bridge, $x, $y, $w, $h)
    }

    $bridge.Dispose()
    $shadow.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3362 } else { 3361 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorPadScratchSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_PadScratch_Many_NG.png' } else { 'Semiconductor_PadScratch_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $scratch = New-Pen 236 4
    $scratchThin = New-Pen 232 3
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 92, 266, 318, 266)
    $graphics.DrawString('PAD SCRATCH', $labelFont, $labelBrush, 104, 246)

    if ($Bad) {
        $graphics.DrawLine($scratch, 112, 306, 150, 320)
        $graphics.DrawLine($scratchThin, 164, 304, 206, 320)
        $graphics.DrawLine($scratch, 248, 306, 292, 320)
        $graphics.DrawLine($scratchThin, 334, 304, 380, 320)
        $graphics.DrawLine($scratch, 404, 304, 448, 320)
    }
    else {
        $graphics.DrawLine($scratchThin, 210, 306, 246, 318)
    }

    $scratch.Dispose()
    $scratchThin.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3372 } else { 3371 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorBondPadNickSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_BondPadNick_Many_NG.png' } else { 'Semiconductor_BondPadNick_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $nick = New-Brush 238
    $shadow = New-Brush 118
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 88, 266, 352, 266)
    $graphics.DrawString('BOND PAD NICK', $labelFont, $labelBrush, 100, 246)

    $nicks = if ($Bad) {
        @('98,296,18,12', '182,296,17,12', '266,296,18,12', '350,296,17,12', '434,296,18,12')
    }
    else {
        @('266,296,16,10')
    }

    foreach ($item in $nicks) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($shadow, $x + 2, $y + 2, $w, $h)
        $graphics.FillEllipse($nick, $x, $y, $w, $h)
    }

    $nick.Dispose()
    $shadow.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3374 } else { 3373 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorWireBondLiftSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_WireBondLift_Many_NG.png' } else { 'Semiconductor_WireBondLift_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $wire = New-Pen 154 2
    $lift = New-Brush 238
    $shadow = New-Brush 118
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 88, 266, 352, 266)
    $graphics.DrawString('WIRE BOND LIFT', $labelFont, $labelBrush, 100, 246)

    for ($i = 0; $i -lt 8; $i++) {
        $x = 100 + ($i * 42)
        $graphics.DrawBezier($wire, $x, 300, $x + 10, 238, $x + 28, 174, $x + 44, 112)
    }

    $lifts = if ($Bad) {
        @('104,286,18,12', '188,286,18,12', '272,286,18,12', '356,286,18,12', '440,286,18,12')
    }
    else {
        @('272,286,16,10')
    }

    foreach ($item in $lifts) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($shadow, $x + 2, $y + 2, $w, $h)
        $graphics.FillEllipse($lift, $x, $y, $w, $h)
    }

    $wire.Dispose()
    $lift.Dispose()
    $shadow.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3376 } else { 3375 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorWireSweepAlignmentSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_WireSweepAlignment_Shifted_NG.png' } else { 'Semiconductor_WireSweepAlignment_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $package = New-Brush 54
    $die = New-Brush 88
    $reference = New-Brush 212
    $wire = New-Brush 226
    $bond = New-Brush 172
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($package, 132, 116, 308, 214)
    $graphics.FillRectangle($die, 184, 154, 206, 126)
    $graphics.FillEllipse($bond, 164, 294, 24, 18)
    $graphics.FillEllipse($bond, 368, 294, 24, 18)
    $graphics.FillRectangle($reference, 220, 178, 22, 104)

    $wireX = if ($Bad) { 262 } else { 282 }
    $graphics.FillRectangle($wire, $wireX, 178, 22, 104)
    $graphics.DrawLine($labelPen, 132, 98, 432, 98)
    $graphics.DrawString('WIRE SWEEP ALIGN', $labelFont, $labelBrush, 166, 78)

    $package.Dispose()
    $die.Dispose()
    $reference.Dispose()
    $wire.Dispose()
    $bond.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3364 } else { 3363 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorBondPadCorrosionSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_BondPadCorrosion_Many_NG.png' } else { 'Semiconductor_BondPadCorrosion_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $package = New-Brush 54
    $die = New-Brush 82
    $pad = New-Brush 176
    $corrosion = New-Brush 238
    $corrosionMid = New-Brush 210
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($package, 96, 112, 380, 210)
    $graphics.FillRectangle($die, 160, 148, 252, 106)
    for ($x = 124; $x -le 420; $x += 42) {
        $graphics.FillRectangle($pad, $x, 276, 26, 32)
    }
    $graphics.DrawLine($labelPen, 126, 94, 424, 94)
    $graphics.DrawString('BOND PAD CORROSION', $labelFont, $labelBrush, 146, 74)

    $spots = if ($Bad) {
        @('128,280,16,13', '170,292,15,12', '254,280,16,13', '338,292,15,12', '422,280,16,13')
    }
    else {
        @('254,280,16,13')
    }

    foreach ($item in $spots) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($corrosionMid, $x + 2, $y + 2, $w, $h)
        $graphics.FillEllipse($corrosion, $x, $y, $w, $h)
    }

    $package.Dispose()
    $die.Dispose()
    $pad.Dispose()
    $corrosion.Dispose()
    $corrosionMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3378 } else { 3377 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorPadPitchSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_PadPitch_Narrow_NG.png' } else { 'Semiconductor_PadPitch_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $dark = New-Brush 54
    $pad = New-Brush 214
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($dark, 210, 164, 150, 128)
    if ($Bad) {
        $graphics.FillRectangle($pad, 236, 192, 22, 72)
        $graphics.FillRectangle($pad, 276, 192, 22, 72)
    }
    else {
        $graphics.FillRectangle($pad, 226, 192, 22, 72)
        $graphics.FillRectangle($pad, 286, 192, 22, 72)
    }

    $graphics.DrawLine($labelPen, 210, 142, 360, 142)
    $graphics.DrawString('PAD PITCH', $labelFont, $labelBrush, 226, 122)

    $dark.Dispose()
    $pad.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3332 } else { 3331 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Draw-RotationMark([System.Drawing.Graphics]$Graphics, [int]$X, [int]$Y, [float]$Angle) {
    $box = New-Brush 116
    $bright = New-Brush 224
    $dark = New-Brush 46
    $edge = New-Pen 222 4
    $thin = New-Pen 70 2

    $Graphics.FillRectangle($box, $X, $Y, 96, 96)
    $state = $Graphics.Save()
    try {
        $Graphics.TranslateTransform($X + 48, $Y + 48)
        $Graphics.RotateTransform($Angle)
        $Graphics.DrawRectangle($edge, -34, -34, 68, 68)
        $Graphics.FillRectangle($bright, -28, -24, 56, 14)
        $Graphics.FillRectangle($bright, -28, 10, 56, 14)
        $Graphics.FillRectangle($dark, -6, -39, 12, 78)
        $Graphics.DrawLine($thin, -36, 0, 36, 0)
        $Graphics.DrawLine($thin, 0, -36, 0, 36)
    }
    finally {
        $Graphics.Restore($state)
    }

    $box.Dispose()
    $bright.Dispose()
    $dark.Dispose()
    $edge.Dispose()
    $thin.Dispose()
}

function Save-SemiconductorRotationMarkSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_RotationMark_Rotated_NG.png' } else { 'Semiconductor_RotationMark_OK.png' }
    $path = Join-Path $productDir $fileName
    $templatePath = Join-Path $templateDir 'Semiconductor_RotationMark_Template.png'
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)
    $graphics.DrawLine($labelPen, 226, 122, 390, 122)
    $graphics.DrawString('ROTATION MARK', $labelFont, $labelBrush, 236, 102)
    $angle = if ($Bad) { 16.0 } else { 0.0 }
    Draw-RotationMark $graphics 238 146 $angle

    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3342 } else { 3341 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path

    if (-not $Bad) {
        $template = $bitmap.Clone(
            [System.Drawing.Rectangle]::new(238, 146, 96, 96),
            [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
        Save-Bitmap $template $templatePath
        $template.Dispose()
    }

    $bitmap.Dispose()
}

function Save-SemiconductorLeadAlignmentSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_LeadAlignment_Shifted_NG.png' } else { 'Semiconductor_LeadAlignment_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $dark = New-Brush 54
    $lead = New-Brush 222
    $reference = New-Brush 210
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($dark, 216, 156, 170, 150)
    $graphics.FillRectangle($reference, 244, 184, 24, 92)
    if ($Bad) {
        $graphics.FillRectangle($lead, 286, 184, 32, 92)
    }
    else {
        $graphics.FillRectangle($lead, 306, 184, 32, 92)
    }

    $graphics.DrawLine($labelPen, 212, 132, 388, 132)
    $graphics.DrawString('LEAD ALIGN', $labelFont, $labelBrush, 234, 112)

    $dark.Dispose()
    $lead.Dispose()
    $reference.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3352 } else { 3351 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorLeadWidthSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_LeadWidth_Narrow_NG.png' } else { 'Semiconductor_LeadWidth_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $package = New-Brush 56
    $leadBody = New-Brush 116
    $leadEdge = New-Brush 222
    $sideLead = New-Brush 178
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($package, 190, 142, 208, 184)
    $graphics.FillRectangle($sideLead, 214, 190, 24, 96)
    $graphics.FillRectangle($sideLead, 372, 190, 24, 96)
    $graphics.FillRectangle($leadBody, 258, 184, 92, 108)
    $graphics.FillRectangle($leadEdge, 260, 190, 24, 96)
    if ($Bad) {
        $graphics.FillRectangle($leadEdge, 304, 190, 22, 96)
    }
    else {
        $graphics.FillRectangle($leadEdge, 324, 190, 22, 96)
    }

    $graphics.DrawLine($labelPen, 190, 126, 396, 126)
    $graphics.DrawString('LEAD WIDTH', $labelFont, $labelBrush, 218, 106)

    $package.Dispose()
    $leadBody.Dispose()
    $leadEdge.Dispose()
    $sideLead.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3356 } else { 3355 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorLeadCoplanaritySample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_LeadCoplanarity_Shifted_NG.png' } else { 'Semiconductor_LeadCoplanarity_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $dark = New-Brush 54
    $reference = New-Brush 210
    $lead = New-Brush 222
    $foot = New-Brush 190
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($dark, 216, 156, 170, 150)
    $graphics.FillRectangle($reference, 244, 184, 24, 92)
    $graphics.FillRectangle($foot, 232, 278, 48, 14)

    if ($Bad) {
        $graphics.FillRectangle($lead, 286, 184, 32, 92)
        $graphics.FillRectangle($foot, 280, 270, 50, 14)
    }
    else {
        $graphics.FillRectangle($lead, 306, 184, 32, 92)
        $graphics.FillRectangle($foot, 298, 278, 52, 14)
    }

    $graphics.DrawLine($labelPen, 206, 132, 424, 132)
    $graphics.DrawString('LEAD COPLANARITY', $labelFont, $labelBrush, 222, 112)

    $dark.Dispose()
    $reference.Dispose()
    $lead.Dispose()
    $foot.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3354 } else { 3353 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorProbeMarkSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_ProbeMark_Many_NG.png' } else { 'Semiconductor_ProbeMark_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $pad = New-Brush 186
    $probe = New-Brush 238
    $rim = New-Pen 206 2
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($pad, 118, 154, 336, 128)
    for ($i = 0; $i -lt 6; $i++) {
        $graphics.FillRectangle($pad, 136 + ($i * 50), 184, 30, 68)
    }

    $graphics.DrawLine($labelPen, 116, 132, 360, 132)
    $graphics.DrawString('PROBE MARK', $labelFont, $labelBrush, 132, 112)

    $marks = if ($Bad) {
        @('140,194,18,14', '190,216,18,14', '240,194,18,14', '290,216,18,14', '340,194,18,14')
    }
    else {
        @('290,216,18,14')
    }

    foreach ($item in $marks) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($probe, $x, $y, $w, $h)
        $graphics.DrawEllipse($rim, $x, $y, $w, $h)
    }

    $pad.Dispose()
    $probe.Dispose()
    $rim.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3356 } else { 3355 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorDieEdgeChipSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_DieEdgeChip_Many_NG.png' } else { 'Semiconductor_DieEdgeChip_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $chip = New-Brush 238
    $chipMid = New-Brush 204
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.DrawLine($labelPen, 94, 92, 330, 92)
    $graphics.DrawString('DIE EDGE CHIP', $labelFont, $labelBrush, 106, 72)

    $chips = if ($Bad) {
        @('92,96,28,18', '188,76,24,18', '302,96,26,18', '436,118,24,18', '406,290,28,18')
    }
    else {
        @('302,96,24,18')
    }

    foreach ($item in $chips) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $points = [System.Drawing.Point[]]@(
            [System.Drawing.Point]::new($x, $y),
            [System.Drawing.Point]::new($x + $w, $y + 4),
            [System.Drawing.Point]::new($x + [int]($w / 2), $y + $h)
        )
        $shadow = [System.Drawing.Point[]]@(
            [System.Drawing.Point]::new($x + 3, $y + 3),
            [System.Drawing.Point]::new($x + $w + 3, $y + 7),
            [System.Drawing.Point]::new($x + [int]($w / 2) + 3, $y + $h + 3)
        )
        $graphics.FillPolygon($chipMid, $shadow)
        $graphics.FillPolygon($chip, $points)
    }

    $chip.Dispose()
    $chipMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3358 } else { 3357 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorMoldingFlashSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_MoldingFlash_Many_NG.png' } else { 'Semiconductor_MoldingFlash_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $package = New-Brush 74
    $flash = New-Brush 238
    $flashMid = New-Brush 202
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($package, 112, 112, 348, 172)
    $graphics.DrawLine($labelPen, 116, 92, 370, 92)
    $graphics.DrawString('MOLDING FLASH', $labelFont, $labelBrush, 130, 72)

    $flashes = if ($Bad) {
        @('126,102,30,18', '218,274,30,18', '308,102,30,18', '400,274,30,18', '452,174,24,30')
    }
    else {
        @('308,102,28,18')
    }

    foreach ($item in $flashes) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $points = [System.Drawing.Point[]]@(
            [System.Drawing.Point]::new($x, $y + $h),
            [System.Drawing.Point]::new($x + [int]($w / 2), $y),
            [System.Drawing.Point]::new($x + $w, $y + $h)
        )
        $shadow = [System.Drawing.Point[]]@(
            [System.Drawing.Point]::new($x + 3, $y + $h + 3),
            [System.Drawing.Point]::new($x + [int]($w / 2) + 3, $y + 3),
            [System.Drawing.Point]::new($x + $w + 3, $y + $h + 3)
        )
        $graphics.FillPolygon($flashMid, $shadow)
        $graphics.FillPolygon($flash, $points)
    }

    $package.Dispose()
    $flash.Dispose()
    $flashMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3360 } else { 3359 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorPackageCrackSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_PackageCrack_Many_NG.png' } else { 'Semiconductor_PackageCrack_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $package = New-Brush 74
    $crack = New-Pen 238 5
    $crackThin = New-Pen 232 4
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($package, 112, 112, 348, 172)
    $graphics.DrawLine($labelPen, 116, 92, 372, 92)
    $graphics.DrawString('PACKAGE CRACK', $labelFont, $labelBrush, 130, 72)

    if ($Bad) {
        $graphics.DrawLine($crack, 142, 134, 180, 164)
        $graphics.DrawLine($crackThin, 214, 244, 258, 214)
        $graphics.DrawLine($crack, 286, 132, 326, 164)
        $graphics.DrawLine($crackThin, 346, 238, 392, 208)
        $graphics.DrawLine($crack, 420, 160, 440, 206)
    }
    else {
        $graphics.DrawLine($crackThin, 286, 138, 326, 164)
    }

    $package.Dispose()
    $crack.Dispose()
    $crackThin.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3362 } else { 3361 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorPackageCornerChipSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_PackageCornerChip_Many_NG.png' } else { 'Semiconductor_PackageCornerChip_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $package = New-Brush 74
    $chip = New-Brush 238
    $chipMid = New-Brush 204
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($package, 112, 112, 348, 172)
    $graphics.DrawLine($labelPen, 116, 92, 410, 92)
    $graphics.DrawString('PACKAGE CORNER CHIP', $labelFont, $labelBrush, 130, 72)

    $chips = if ($Bad) {
        @('112,112,28,22,tl', '432,112,28,22,tr', '112,262,28,22,bl', '432,262,28,22,br', '444,190,16,32,rr')
    }
    else {
        @('432,112,26,20,tr')
    }

    foreach ($item in $chips) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $kind = $parts[4]
        switch ($kind) {
            'tl' {
                $points = [System.Drawing.Point[]]@(
                    [System.Drawing.Point]::new($x, $y),
                    [System.Drawing.Point]::new($x + $w, $y),
                    [System.Drawing.Point]::new($x, $y + $h)
                )
            }
            'tr' {
                $points = [System.Drawing.Point[]]@(
                    [System.Drawing.Point]::new($x + $w, $y),
                    [System.Drawing.Point]::new($x + $w, $y + $h),
                    [System.Drawing.Point]::new($x, $y)
                )
            }
            'bl' {
                $points = [System.Drawing.Point[]]@(
                    [System.Drawing.Point]::new($x, $y),
                    [System.Drawing.Point]::new($x + $w, $y + $h),
                    [System.Drawing.Point]::new($x, $y + $h)
                )
            }
            'br' {
                $points = [System.Drawing.Point[]]@(
                    [System.Drawing.Point]::new($x + $w, $y + $h),
                    [System.Drawing.Point]::new($x + $w, $y),
                    [System.Drawing.Point]::new($x, $y + $h)
                )
            }
            default {
                $points = [System.Drawing.Point[]]@(
                    [System.Drawing.Point]::new($x, $y),
                    [System.Drawing.Point]::new($x + $w, $y + 8),
                    [System.Drawing.Point]::new($x + $w, $y + $h),
                    [System.Drawing.Point]::new($x, $y + $h - 6)
                )
            }
        }

        $shadow = foreach ($point in $points) {
            [System.Drawing.Point]::new($point.X - 3, $point.Y + 3)
        }
        $graphics.FillPolygon($chipMid, [System.Drawing.Point[]]$shadow)
        $graphics.FillPolygon($chip, $points)
    }

    $package.Dispose()
    $chip.Dispose()
    $chipMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3364 } else { 3363 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorLeadBurrSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_LeadBurr_Many_NG.png' } else { 'Semiconductor_LeadBurr_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $package = New-Brush 74
    $burr = New-Brush 238
    $burrMid = New-Brush 202
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($package, 112, 112, 348, 172)
    $graphics.DrawLine($labelPen, 116, 92, 334, 92)
    $graphics.DrawString('LEAD BURR', $labelFont, $labelBrush, 130, 72)

    $burrs = if ($Bad) {
        @('100,286,22,20', '164,286,22,20', '250,286,22,20', '334,286,22,20', '420,286,22,20')
    }
    else {
        @('250,286,20,18')
    }

    foreach ($item in $burrs) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $points = [System.Drawing.Point[]]@(
            [System.Drawing.Point]::new($x, $y + $h),
            [System.Drawing.Point]::new($x + [int]($w / 2), $y),
            [System.Drawing.Point]::new($x + $w, $y + $h)
        )
        $shadow = [System.Drawing.Point[]]@(
            [System.Drawing.Point]::new($x + 3, $y + $h + 3),
            [System.Drawing.Point]::new($x + [int]($w / 2) + 3, $y + 3),
            [System.Drawing.Point]::new($x + $w + 3, $y + $h + 3)
        )
        $graphics.FillPolygon($burrMid, $shadow)
        $graphics.FillPolygon($burr, $points)
    }

    $package.Dispose()
    $burr.Dispose()
    $burrMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3364 } else { 3363 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorLeadCrackSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_LeadCrack_Many_NG.png' } else { 'Semiconductor_LeadCrack_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $package = New-Brush 74
    $crack = New-Pen 238 5
    $crackThin = New-Pen 232 4
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($package, 112, 112, 348, 172)
    $graphics.DrawLine($labelPen, 116, 92, 348, 92)
    $graphics.DrawString('LEAD CRACK', $labelFont, $labelBrush, 130, 72)

    if ($Bad) {
        $graphics.DrawLine($crack, 98, 304, 122, 286)
        $graphics.DrawLine($crackThin, 164, 304, 190, 286)
        $graphics.DrawLine($crack, 250, 304, 276, 286)
        $graphics.DrawLine($crackThin, 334, 304, 360, 286)
        $graphics.DrawLine($crack, 420, 304, 444, 286)
    }
    else {
        $graphics.DrawLine($crackThin, 250, 304, 274, 286)
    }

    $package.Dispose()
    $crack.Dispose()
    $crackThin.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3366 } else { 3365 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Save-SemiconductorLeadOxidationSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_LeadOxidation_Many_NG.png' } else { 'Semiconductor_LeadOxidation_OK.png' }
    $path = Join-Path $productDir $fileName
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $package = New-Brush 74
    $oxidation = New-Brush 238
    $oxidationMid = New-Brush 202
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($package, 112, 112, 348, 172)
    $graphics.DrawLine($labelPen, 116, 92, 378, 92)
    $graphics.DrawString('LEAD OXIDATION', $labelFont, $labelBrush, 130, 72)

    $spots = if ($Bad) {
        @('96,306,20,16', '160,306,20,16', '246,306,20,16', '330,306,20,16', '416,306,20,16')
    }
    else {
        @('246,306,18,15')
    }

    foreach ($item in $spots) {
        $parts = $item.Split(',')
        $x = [int]$parts[0]
        $y = [int]$parts[1]
        $w = [int]$parts[2]
        $h = [int]$parts[3]
        $graphics.FillEllipse($oxidationMid, $x + 3, $y + 3, $w, $h)
        $graphics.FillEllipse($oxidation, $x, $y, $w, $h)
    }

    $package.Dispose()
    $oxidation.Dispose()
    $oxidationMid.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3368 } else { 3367 }
    Add-Noise $bitmap $seed 3
    Save-Bitmap $bitmap $path
    $bitmap.Dispose()
}

function Draw-PackagePolarityMark([System.Drawing.Graphics]$Graphics, [int]$X, [int]$Y, [bool]$Missing) {
    $panel = New-Brush 82
    $bright = New-Brush 226
    $dark = New-Brush 42
    $line = New-Pen 226 4
    $thin = New-Pen 86 2
    $smallFont = [System.Drawing.Font]::new('Arial', 8, [System.Drawing.FontStyle]::Bold)

    $Graphics.FillRectangle($panel, $X, $Y, 118, 90)
    $Graphics.DrawRectangle($thin, $X, $Y, 118, 90)
    if ($Missing) {
        $Graphics.FillRectangle($dark, $X + 22, $Y + 24, 74, 38)
    }
    else {
        $Graphics.FillEllipse($bright, $X + 18, $Y + 18, 28, 28)
        $Graphics.DrawLine($line, $X + 24, $Y + 70, $X + 96, $Y + 18)
        $Graphics.DrawRectangle($line, $X + 58, $Y + 48, 34, 22)
        $Graphics.DrawString('PIN1', $smallFont, $bright, $X + 56, $Y + 12)
    }

    $panel.Dispose()
    $bright.Dispose()
    $dark.Dispose()
    $line.Dispose()
    $thin.Dispose()
    $smallFont.Dispose()
}

function Save-SemiconductorPackagePolaritySample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_PackagePolarity_Missing_NG.png' } else { 'Semiconductor_PackagePolarity_OK.png' }
    $path = Join-Path $productDir $fileName
    $templatePath = Join-Path $templateDir 'Semiconductor_PackagePolarity_Template.png'
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $package = New-Brush 74
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($package, 112, 112, 348, 172)
    $graphics.DrawLine($labelPen, 118, 94, 382, 94)
    $graphics.DrawString('PACKAGE POLARITY', $labelFont, $labelBrush, 130, 74)
    Draw-PackagePolarityMark $graphics 142 138 $Bad

    $package.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3370 } else { 3369 }
    Add-Noise $bitmap $seed 2
    Save-Bitmap $bitmap $path

    if (-not $Bad) {
        $template = $bitmap.Clone(
            [System.Drawing.Rectangle]::new(142, 138, 118, 90),
            [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
        Save-Bitmap $template $templatePath
        $template.Dispose()
    }

    $bitmap.Dispose()
}

function Draw-PackageLaserText([System.Drawing.Graphics]$Graphics, [int]$X, [int]$Y, [bool]$Missing) {
    $panel = New-Brush 216
    $dark = New-Brush 42
    $mid = New-Brush 86
    $line = New-Pen 58 3
    $thin = New-Pen 74 1
    $font = [System.Drawing.Font]::new('Arial', 15, [System.Drawing.FontStyle]::Bold)
    $smallFont = [System.Drawing.Font]::new('Arial', 8, [System.Drawing.FontStyle]::Bold)

    $Graphics.FillRectangle($panel, $X, $Y, 148, 86)
    $Graphics.DrawRectangle($line, $X, $Y, 148, 86)
    if ($Missing) {
        $Graphics.FillRectangle($mid, $X + 18, $Y + 24, 108, 34)
        $Graphics.DrawLine($thin, $X + 18, $Y + 66, $X + 130, $Y + 66)
    }
    else {
        $Graphics.DrawString('LOT24', $font, $dark, $X + 12, $Y + 12)
        $Graphics.DrawString('PKG-A3', $smallFont, $dark, $X + 18, $Y + 54)
        for ($i = 0; $i -lt 5; $i++) {
            $barX = $X + 98 + ($i * 7)
            $Graphics.DrawLine($line, $barX, $Y + 16, $barX, $Y + 68)
        }
        $Graphics.DrawLine($thin, $X + 12, $Y + 74, $X + 134, $Y + 10)
    }

    $panel.Dispose()
    $dark.Dispose()
    $mid.Dispose()
    $line.Dispose()
    $thin.Dispose()
    $font.Dispose()
    $smallFont.Dispose()
}

function Save-SemiconductorPackageLaserTextSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_PackageLaserText_Missing_NG.png' } else { 'Semiconductor_PackageLaserText_OK.png' }
    $path = Join-Path $productDir $fileName
    $templatePath = Join-Path $templateDir 'Semiconductor_PackageLaserText_Template.png'
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $package = New-Brush 74
    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)

    $graphics.FillRectangle($package, 112, 112, 348, 172)
    $graphics.DrawLine($labelPen, 156, 94, 402, 94)
    $graphics.DrawString('PACKAGE LASER TEXT', $labelFont, $labelBrush, 166, 74)
    Draw-PackageLaserText $graphics 166 140 $Bad

    $package.Dispose()
    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3372 } else { 3371 }
    Add-Noise $bitmap $seed 2
    Save-Bitmap $bitmap $path

    if (-not $Bad) {
        $template = $bitmap.Clone(
            [System.Drawing.Rectangle]::new(166, 140, 148, 86),
            [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
        Save-Bitmap $template $templatePath
        $template.Dispose()
    }

    $bitmap.Dispose()
}

function Draw-WaferDieMark([System.Drawing.Graphics]$Graphics, [int]$X, [int]$Y, [bool]$Wrong) {
    $panel = New-Brush 216
    $dark = New-Brush 44
    $mid = New-Brush 118
    $line = New-Pen 68 3
    $thin = New-Pen 92 1
    $font = [System.Drawing.Font]::new('Arial', 17, [System.Drawing.FontStyle]::Bold)
    $smallFont = [System.Drawing.Font]::new('Arial', 9, [System.Drawing.FontStyle]::Bold)

    $Graphics.FillRectangle($panel, $X, $Y, 126, 96)
    $Graphics.DrawRectangle($line, $X, $Y, 126, 96)
    if ($Wrong) {
        $Graphics.DrawString('W2', $font, $dark, $X + 14, $Y + 14)
        $Graphics.DrawEllipse($thin, $X + 72, $Y + 18, 34, 34)
        $Graphics.FillRectangle($dark, $X + 76, $Y + 62, 34, 16)
        $Graphics.DrawLine($line, $X + 14, $Y + 82, $X + 112, $Y + 18)
    }
    else {
        $Graphics.DrawString('DIE', $smallFont, $dark, $X + 74, $Y + 12)
        $Graphics.DrawString('A7', $font, $dark, $X + 14, $Y + 16)
        $Graphics.FillRectangle($dark, $X + 78, $Y + 38, 34, 20)
        $Graphics.DrawEllipse($line, $X + 20, $Y + 58, 34, 24)
        $Graphics.DrawLine($line, $X + 16, $Y + 84, $X + 112, $Y + 20)
    }

    $panel.Dispose()
    $dark.Dispose()
    $mid.Dispose()
    $line.Dispose()
    $thin.Dispose()
    $font.Dispose()
    $smallFont.Dispose()
}

function Save-SemiconductorWaferDieMarkSample([bool]$Bad) {
    $fileName = if ($Bad) { 'Semiconductor_WaferDieMark_Wrong_NG.png' } else { 'Semiconductor_WaferDieMark_OK.png' }
    $path = Join-Path $productDir $fileName
    $templatePath = Join-Path $templateDir 'Semiconductor_WaferDieMark_Template.png'
    $bitmap = [System.Drawing.Bitmap]::new(572, 420, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    Draw-SemiconductorBase $graphics

    $labelPen = New-Pen 166 2
    $labelBrush = New-Brush 184
    $labelFont = [System.Drawing.Font]::new('Arial', 11)
    $graphics.DrawLine($labelPen, 166, 110, 350, 110)
    $graphics.DrawString('WAFER DIE MARK', $labelFont, $labelBrush, 176, 90)
    Draw-WaferDieMark $graphics 218 138 $Bad

    $labelPen.Dispose()
    $labelBrush.Dispose()
    $labelFont.Dispose()
    $graphics.Dispose()
    $seed = if ($Bad) { 3362 } else { 3361 }
    Add-Noise $bitmap $seed 2
    Save-Bitmap $bitmap $path

    if (-not $Bad) {
        $template = $bitmap.Clone(
            [System.Drawing.Rectangle]::new(218, 138, 126, 96),
            [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
        Save-Bitmap $template $templatePath
        $template.Dispose()
    }

    $bitmap.Dispose()
}

function Save-ProductManifest {
    $manifestPath = Join-Path $productDir 'OpenVisionLab.ProductSampleManifest.csv'
    $rows = @(
        'AssetPath,SourceType,SourceName,SourceUrl,License,Attribution,GeneratedBy,Notes',
        'docs/samples/public/product/Battery_TabGap_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab gap OK sample',
        'docs/samples/public/product/Battery_TabGap_Narrow_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab gap drift NG sample',
        'docs/samples/public/product/Battery_WeldSpatter_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery weld spatter OK sample',
        'docs/samples/public/product/Battery_WeldSpatter_Heavy_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery weld spatter NG sample',
        'docs/samples/public/product/Battery_WeldOverburn_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery weld overburn OK sample',
        'docs/samples/public/product/Battery_WeldOverburn_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery weld overburn NG sample',
        'docs/samples/public/product/Battery_TabTear_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab tear OK sample',
        'docs/samples/public/product/Battery_TabTear_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab tear NG sample',
        'docs/samples/public/product/Battery_TabPlatingPeel_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab plating peel OK sample',
        'docs/samples/public/product/Battery_TabPlatingPeel_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab plating peel NG sample',
        'docs/samples/public/product/Battery_ElectrolyteStain_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery electrolyte stain OK sample',
        'docs/samples/public/product/Battery_ElectrolyteStain_Heavy_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery electrolyte stain NG sample',
        'docs/samples/public/product/Battery_SeparatorWrinkle_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery separator wrinkle OK sample',
        'docs/samples/public/product/Battery_SeparatorWrinkle_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery separator wrinkle NG sample',
        'docs/samples/public/product/Battery_SeparatorPinhole_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery separator pinhole OK sample',
        'docs/samples/public/product/Battery_SeparatorPinhole_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery separator pinhole NG sample',
        'docs/samples/public/product/Battery_CoatingGap_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery coating gap OK sample',
        'docs/samples/public/product/Battery_CoatingGap_Narrow_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery coating gap narrow NG sample',
        'docs/samples/public/product/Battery_ForeignObject_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery foreign object OK sample',
        'docs/samples/public/product/Battery_ForeignObject_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery foreign object NG sample',
        'docs/samples/public/product/Battery_EdgeBurr_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery coating edge burr OK sample',
        'docs/samples/public/product/Battery_EdgeBurr_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery coating edge burr NG sample',
        'docs/samples/public/product/Battery_TabOffset_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab offset OK sample',
        'docs/samples/public/product/Battery_TabOffset_Shifted_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab offset shifted NG sample',
        'docs/samples/public/product/Battery_SealWidth_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery seal width OK sample',
        'docs/samples/public/product/Battery_SealWidth_Narrow_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery narrow seal width NG sample',
        'docs/samples/public/product/Battery_TabWeldVoid_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab weld void OK sample',
        'docs/samples/public/product/Battery_TabWeldVoid_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab weld void NG sample',
        'docs/samples/public/product/Battery_PouchEdgeFold_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery pouch edge fold OK sample',
        'docs/samples/public/product/Battery_PouchEdgeFold_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery pouch edge fold NG sample',
        'docs/samples/public/product/Battery_PouchSealBurn_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery pouch seal burn OK sample',
        'docs/samples/public/product/Battery_PouchSealBurn_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery pouch seal burn NG sample',
        'docs/samples/public/product/Battery_PouchSealBubble_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery pouch seal bubble OK sample',
        'docs/samples/public/product/Battery_PouchSealBubble_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery pouch seal bubble NG sample',
        'docs/samples/public/product/Battery_SealEdgeDelamination_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery seal edge delamination OK sample',
        'docs/samples/public/product/Battery_SealEdgeDelamination_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery seal edge delamination NG sample',
        'docs/samples/public/product/Battery_TabOxidation_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab oxidation OK sample',
        'docs/samples/public/product/Battery_TabOxidation_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab oxidation NG sample',
        'docs/samples/public/product/Battery_TabDiscoloration_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab discoloration OK sample',
        'docs/samples/public/product/Battery_TabDiscoloration_Dark_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab discoloration NG sample',
        'docs/samples/public/product/Battery_SealContamination_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery seal contamination OK sample',
        'docs/samples/public/product/Battery_SealContamination_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery seal contamination NG sample',
        'docs/samples/public/product/Battery_LaserMark_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery laser mark OK sample',
        'docs/samples/public/product/Battery_LaserMark_Missing_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery missing laser mark NG sample',
        'docs/samples/public/product/templates/Battery_LaserMark_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Template crop from Battery_LaserMark_OK.png',
        'docs/samples/public/product/Battery_TabDateCode_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery tab date-code OK sample',
        'docs/samples/public/product/Battery_TabDateCode_Wrong_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery wrong tab date-code NG sample',
        'docs/samples/public/product/templates/Battery_TabDateCode_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Template crop from Battery_TabDateCode_OK.png',
        'docs/samples/public/product/Battery_ElectrolyteFillLine_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery electrolyte fill-line OK sample',
        'docs/samples/public/product/Battery_ElectrolyteFillLine_Low_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery low electrolyte fill-line NG sample',
        'docs/samples/public/product/Battery_CellVentAlignment_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery cell vent alignment OK sample',
        'docs/samples/public/product/Battery_CellVentAlignment_Shifted_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery shifted cell vent alignment NG sample',
        'docs/samples/public/product/Battery_PouchTabSkew_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery pouch tab skew OK sample',
        'docs/samples/public/product/Battery_PouchTabSkew_Shifted_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery shifted pouch tab skew NG sample',
        'docs/samples/public/product/Battery_CurrentCollectorBurr_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery current collector burr OK sample',
        'docs/samples/public/product/Battery_CurrentCollectorBurr_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery current collector burr NG sample',
        'docs/samples/public/product/Battery_SeparatorEdgeTear_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery separator edge tear OK sample',
        'docs/samples/public/product/Battery_SeparatorEdgeTear_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Secondary battery separator edge tear NG sample',
        'docs/samples/public/product/Display_PixelDefect_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display pixel defect OK sample',
        'docs/samples/public/product/Display_PixelDefect_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display pixel defect count NG sample',
        'docs/samples/public/product/Display_Alignment_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display alignment mark OK sample',
        'docs/samples/public/product/Display_Alignment_Wrong_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display wrong alignment mark NG sample',
        'docs/samples/public/product/templates/Display_Alignment_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Template crop from Display_Alignment_OK.png',
        'docs/samples/public/product/Display_Scratch_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display scratch OK sample',
        'docs/samples/public/product/Display_Scratch_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display scratch count NG sample',
        'docs/samples/public/product/Display_BrightnessBand_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display brightness band OK sample',
        'docs/samples/public/product/Display_BrightnessBand_Bright_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display brightness band NG sample',
        'docs/samples/public/product/Display_Particle_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display particle OK sample',
        'docs/samples/public/product/Display_Particle_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display particle count NG sample',
        'docs/samples/public/product/Display_MuraVariation_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display mura variation OK sample',
        'docs/samples/public/product/Display_MuraVariation_Uneven_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display uneven mura variation NG sample',
        'docs/samples/public/product/Display_EdgeChip_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display edge chip OK sample',
        'docs/samples/public/product/Display_EdgeChip_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display edge chip count NG sample',
        'docs/samples/public/product/Display_BezelChip_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display bezel chip OK sample',
        'docs/samples/public/product/Display_BezelChip_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display bezel chip count NG sample',
        'docs/samples/public/product/Display_CornerCrack_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display corner crack OK sample',
        'docs/samples/public/product/Display_CornerCrack_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display corner crack NG sample',
        'docs/samples/public/product/Display_AlignmentOffset_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display alignment offset OK sample',
        'docs/samples/public/product/Display_AlignmentOffset_Shifted_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display alignment offset NG sample',
        'docs/samples/public/product/Display_ColorFilterShift_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display color-filter shift OK sample',
        'docs/samples/public/product/Display_ColorFilterShift_Shifted_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display color-filter shift NG sample',
        'docs/samples/public/product/Display_LineStain_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display line stain OK sample',
        'docs/samples/public/product/Display_LineStain_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display line stain NG sample',
        'docs/samples/public/product/Display_SubpixelBridge_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display subpixel bridge OK sample',
        'docs/samples/public/product/Display_SubpixelBridge_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display subpixel bridge NG sample',
        'docs/samples/public/product/Display_PadBridge_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display pad bridge OK sample',
        'docs/samples/public/product/Display_PadBridge_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display pad bridge NG sample',
        'docs/samples/public/product/Display_PolarizerBubble_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display polarizer bubble OK sample',
        'docs/samples/public/product/Display_PolarizerBubble_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display polarizer bubble NG sample',
        'docs/samples/public/product/Display_SealContamination_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display seal contamination OK sample',
        'docs/samples/public/product/Display_SealContamination_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display seal contamination NG sample',
        'docs/samples/public/product/Display_SealCornerContamination_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display seal corner contamination OK sample',
        'docs/samples/public/product/Display_SealCornerContamination_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display seal corner contamination NG sample',
        'docs/samples/public/product/Display_PolarizerEdgeLift_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display polarizer edge lift OK sample',
        'docs/samples/public/product/Display_PolarizerEdgeLift_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display polarizer edge lift NG sample',
        'docs/samples/public/product/Display_CofBondParticle_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display COF bond particle OK sample',
        'docs/samples/public/product/Display_CofBondParticle_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display COF bond particle NG sample',
        'docs/samples/public/product/Display_FpcAlignmentMark_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display FPC alignment mark OK sample',
        'docs/samples/public/product/Display_FpcAlignmentMark_Wrong_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display wrong FPC alignment mark NG sample',
        'docs/samples/public/product/templates/Display_FpcAlignmentMark_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Template crop from Display_FpcAlignmentMark_OK.png',
        'docs/samples/public/product/Display_CornerLightLeak_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display corner light leak OK sample',
        'docs/samples/public/product/Display_CornerLightLeak_Bright_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display corner light leak NG sample',
        'docs/samples/public/product/Display_BlackMatrixScratch_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display black matrix scratch OK sample',
        'docs/samples/public/product/Display_BlackMatrixScratch_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display black matrix scratch NG sample',
        'docs/samples/public/product/Display_LineDropout_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display line dropout OK sample',
        'docs/samples/public/product/Display_LineDropout_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display line dropout NG sample',
        'docs/samples/public/product/Display_MuraSpotCluster_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display mura spot cluster OK sample',
        'docs/samples/public/product/Display_MuraSpotCluster_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display mura spot cluster NG sample',
        'docs/samples/public/product/Display_MuraRing_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display mura ring OK sample',
        'docs/samples/public/product/Display_MuraRing_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display mura ring NG sample',
        'docs/samples/public/product/Display_PolarizerScratch_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display polarizer scratch OK sample',
        'docs/samples/public/product/Display_PolarizerScratch_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display polarizer scratch NG sample',
        'docs/samples/public/product/Display_PolarizerCrease_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display polarizer crease OK sample',
        'docs/samples/public/product/Display_PolarizerCrease_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display polarizer crease NG sample',
        'docs/samples/public/product/Display_SealWidth_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display seal width OK sample',
        'docs/samples/public/product/Display_SealWidth_Narrow_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Display narrow seal width NG sample',
        'docs/samples/public/product/Semiconductor_Fiducial_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor fiducial edge OK sample',
        'docs/samples/public/product/Semiconductor_Fiducial_Wrong_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor wrong fiducial NG sample',
        'docs/samples/public/product/templates/Semiconductor_Fiducial_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Template crop from Semiconductor_Fiducial_OK.png',
        'docs/samples/public/product/Semiconductor_BondMark_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor feature mark OK sample',
        'docs/samples/public/product/Semiconductor_BondMark_Wrong_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor wrong feature mark NG sample',
        'docs/samples/public/product/templates/Semiconductor_BondMark_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Template crop from Semiconductor_BondMark_OK.png',
        'docs/samples/public/product/Semiconductor_PadContamination_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor pad contamination OK sample',
        'docs/samples/public/product/Semiconductor_PadContamination_Heavy_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor pad contamination NG sample',
        'docs/samples/public/product/Semiconductor_DieContamination_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor die contamination OK sample',
        'docs/samples/public/product/Semiconductor_DieContamination_Heavy_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor die contamination NG sample',
        'docs/samples/public/product/Semiconductor_UnderfillVoid_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor underfill void OK sample',
        'docs/samples/public/product/Semiconductor_UnderfillVoid_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor underfill void NG sample',
        'docs/samples/public/product/Semiconductor_PackageVoid_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor package void OK sample',
        'docs/samples/public/product/Semiconductor_PackageVoid_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor package void NG sample',
        'docs/samples/public/product/Semiconductor_SolderBridge_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor solder bridge OK sample',
        'docs/samples/public/product/Semiconductor_SolderBridge_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor solder bridge NG sample',
        'docs/samples/public/product/Semiconductor_PadScratch_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor pad scratch OK sample',
        'docs/samples/public/product/Semiconductor_PadScratch_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor pad scratch NG sample',
        'docs/samples/public/product/Semiconductor_BondPadNick_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor bond-pad nick OK sample',
        'docs/samples/public/product/Semiconductor_BondPadNick_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor bond-pad nick NG sample',
        'docs/samples/public/product/Semiconductor_WireBondLift_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor wire-bond lift OK sample',
        'docs/samples/public/product/Semiconductor_WireBondLift_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor wire-bond lift NG sample',
        'docs/samples/public/product/Semiconductor_WireSweepAlignment_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor wire sweep alignment OK sample',
        'docs/samples/public/product/Semiconductor_WireSweepAlignment_Shifted_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor shifted wire sweep alignment NG sample',
        'docs/samples/public/product/Semiconductor_BondPadCorrosion_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor bond-pad corrosion OK sample',
        'docs/samples/public/product/Semiconductor_BondPadCorrosion_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor bond-pad corrosion NG sample',
        'docs/samples/public/product/Semiconductor_PadPitch_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor pad pitch OK sample',
        'docs/samples/public/product/Semiconductor_PadPitch_Narrow_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor pad pitch NG sample',
        'docs/samples/public/product/Semiconductor_RotationMark_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor rotation mark OK sample',
        'docs/samples/public/product/Semiconductor_RotationMark_Rotated_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor rotation mark NG sample',
        'docs/samples/public/product/templates/Semiconductor_RotationMark_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Template crop from Semiconductor_RotationMark_OK.png',
        'docs/samples/public/product/Semiconductor_LeadAlignment_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor lead alignment OK sample',
        'docs/samples/public/product/Semiconductor_LeadAlignment_Shifted_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor shifted lead alignment NG sample',
        'docs/samples/public/product/Semiconductor_LeadWidth_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor lead width OK sample',
        'docs/samples/public/product/Semiconductor_LeadWidth_Narrow_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor narrow lead width NG sample',
        'docs/samples/public/product/Semiconductor_LeadCoplanarity_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor lead coplanarity OK sample',
        'docs/samples/public/product/Semiconductor_LeadCoplanarity_Shifted_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor shifted lead coplanarity NG sample',
        'docs/samples/public/product/Semiconductor_ProbeMark_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor probe mark OK sample',
        'docs/samples/public/product/Semiconductor_ProbeMark_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor probe mark NG sample',
        'docs/samples/public/product/Semiconductor_DieEdgeChip_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor die edge chip OK sample',
        'docs/samples/public/product/Semiconductor_DieEdgeChip_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor die edge chip NG sample',
        'docs/samples/public/product/Semiconductor_MoldingFlash_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor molding flash OK sample',
        'docs/samples/public/product/Semiconductor_MoldingFlash_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor molding flash NG sample',
        'docs/samples/public/product/Semiconductor_PackageCrack_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor package crack OK sample',
        'docs/samples/public/product/Semiconductor_PackageCrack_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor package crack NG sample',
        'docs/samples/public/product/Semiconductor_PackageCornerChip_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor package corner chip OK sample',
        'docs/samples/public/product/Semiconductor_PackageCornerChip_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor package corner chip NG sample',
        'docs/samples/public/product/Semiconductor_LeadBurr_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor lead burr OK sample',
        'docs/samples/public/product/Semiconductor_LeadBurr_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor lead burr NG sample',
        'docs/samples/public/product/Semiconductor_LeadCrack_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor lead crack OK sample',
        'docs/samples/public/product/Semiconductor_LeadCrack_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor lead crack NG sample',
        'docs/samples/public/product/Semiconductor_LeadOxidation_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor lead oxidation OK sample',
        'docs/samples/public/product/Semiconductor_LeadOxidation_Many_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor lead oxidation NG sample',
        'docs/samples/public/product/Semiconductor_PackagePolarity_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor package polarity OK sample',
        'docs/samples/public/product/Semiconductor_PackagePolarity_Missing_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor missing package polarity NG sample',
        'docs/samples/public/product/templates/Semiconductor_PackagePolarity_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Template crop from Semiconductor_PackagePolarity_OK.png',
        'docs/samples/public/product/Semiconductor_PackageLaserText_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor package laser text OK sample',
        'docs/samples/public/product/Semiconductor_PackageLaserText_Missing_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor missing package laser text NG sample',
        'docs/samples/public/product/templates/Semiconductor_PackageLaserText_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Template crop from Semiconductor_PackageLaserText_OK.png',
        'docs/samples/public/product/Semiconductor_WaferDieMark_OK.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor wafer die mark OK sample',
        'docs/samples/public/product/Semiconductor_WaferDieMark_Wrong_NG.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Semiconductor wrong wafer die mark NG sample',
        'docs/samples/public/product/templates/Semiconductor_WaferDieMark_Template.png,Synthetic,OpenVisionLab,,Repository license,,tools/GenerateOpenVisionProductSamples.ps1,Template crop from Semiconductor_WaferDieMark_OK.png'
    )
    $rows | Set-Content -LiteralPath $manifestPath -Encoding UTF8
    Write-Host "Generated: $manifestPath"
}

Save-BatteryTabGapSample $false
Save-BatteryTabGapSample $true
Save-BatteryWeldSpatterSample $false
Save-BatteryWeldSpatterSample $true
Save-BatteryWeldOverburnSample $false
Save-BatteryWeldOverburnSample $true
Save-BatteryTabTearSample $false
Save-BatteryTabTearSample $true
Save-BatteryTabPlatingPeelSample $false
Save-BatteryTabPlatingPeelSample $true
Save-BatteryElectrolyteStainSample $false
Save-BatteryElectrolyteStainSample $true
Save-BatterySeparatorWrinkleSample $false
Save-BatterySeparatorWrinkleSample $true
Save-BatterySeparatorPinholeSample $false
Save-BatterySeparatorPinholeSample $true
Save-BatteryCoatingGapSample $false
Save-BatteryCoatingGapSample $true
Save-BatteryForeignObjectSample $false
Save-BatteryForeignObjectSample $true
Save-BatteryEdgeBurrSample $false
Save-BatteryEdgeBurrSample $true
Save-BatteryTabOffsetSample $false
Save-BatteryTabOffsetSample $true
Save-BatterySealWidthSample $false
Save-BatterySealWidthSample $true
Save-BatteryTabWeldVoidSample $false
Save-BatteryTabWeldVoidSample $true
Save-BatteryPouchEdgeFoldSample $false
Save-BatteryPouchEdgeFoldSample $true
Save-BatteryPouchSealBurnSample $false
Save-BatteryPouchSealBurnSample $true
Save-BatteryPouchSealBubbleSample $false
Save-BatteryPouchSealBubbleSample $true
Save-BatterySealEdgeDelaminationSample $false
Save-BatterySealEdgeDelaminationSample $true
Save-BatteryTabOxidationSample $false
Save-BatteryTabOxidationSample $true
Save-BatteryTabDiscolorationSample $false
Save-BatteryTabDiscolorationSample $true
Save-BatterySealContaminationSample $false
Save-BatterySealContaminationSample $true
Save-BatteryLaserMarkSample $false
Save-BatteryLaserMarkSample $true
Save-BatteryTabDateCodeSample $false
Save-BatteryTabDateCodeSample $true
Save-BatteryElectrolyteFillLineSample $false
Save-BatteryElectrolyteFillLineSample $true
Save-BatteryCellVentAlignmentSample $false
Save-BatteryCellVentAlignmentSample $true
Save-BatteryPouchTabSkewSample $false
Save-BatteryPouchTabSkewSample $true
Save-BatteryCurrentCollectorBurrSample $false
Save-BatteryCurrentCollectorBurrSample $true
Save-BatterySeparatorEdgeTearSample $false
Save-BatterySeparatorEdgeTearSample $true
Save-DisplayDefectContourSample $false
Save-DisplayDefectContourSample $true
Save-DisplayAlignmentSample $false
Save-DisplayAlignmentSample $true
Save-DisplayScratchSample $false
Save-DisplayScratchSample $true
Save-DisplayBrightnessBandSample $false
Save-DisplayBrightnessBandSample $true
Save-DisplayParticleSample $false
Save-DisplayParticleSample $true
Save-DisplayMuraVariationSample $false
Save-DisplayMuraVariationSample $true
Save-DisplayEdgeChipSample $false
Save-DisplayEdgeChipSample $true
Save-DisplayBezelChipSample $false
Save-DisplayBezelChipSample $true
Save-DisplayCornerCrackSample $false
Save-DisplayCornerCrackSample $true
Save-DisplayAlignmentOffsetSample $false
Save-DisplayAlignmentOffsetSample $true
Save-DisplayColorFilterShiftSample $false
Save-DisplayColorFilterShiftSample $true
Save-DisplayLineStainSample $false
Save-DisplayLineStainSample $true
Save-DisplaySubpixelBridgeSample $false
Save-DisplaySubpixelBridgeSample $true
Save-DisplayPadBridgeSample $false
Save-DisplayPadBridgeSample $true
Save-DisplayPolarizerBubbleSample $false
Save-DisplayPolarizerBubbleSample $true
Save-DisplaySealContaminationSample $false
Save-DisplaySealContaminationSample $true
Save-DisplaySealCornerContaminationSample $false
Save-DisplaySealCornerContaminationSample $true
Save-DisplayPolarizerEdgeLiftSample $false
Save-DisplayPolarizerEdgeLiftSample $true
Save-DisplayCofBondParticleSample $false
Save-DisplayCofBondParticleSample $true
Save-DisplayFpcAlignmentMarkSample $false
Save-DisplayFpcAlignmentMarkSample $true
Save-DisplayCornerLightLeakSample $false
Save-DisplayCornerLightLeakSample $true
Save-DisplayBlackMatrixScratchSample $false
Save-DisplayBlackMatrixScratchSample $true
Save-DisplayLineDropoutSample $false
Save-DisplayLineDropoutSample $true
Save-DisplayMuraSpotClusterSample $false
Save-DisplayMuraSpotClusterSample $true
Save-DisplayMuraRingSample $false
Save-DisplayMuraRingSample $true
Save-DisplayPolarizerScratchSample $false
Save-DisplayPolarizerScratchSample $true
Save-DisplayPolarizerCreaseSample $false
Save-DisplayPolarizerCreaseSample $true
Save-DisplaySealWidthSample $false
Save-DisplaySealWidthSample $true
Save-SemiconductorFiducialSample $false
Save-SemiconductorFiducialSample $true
Save-SemiconductorFeatureSample $false
Save-SemiconductorFeatureSample $true
Save-SemiconductorPadContaminationSample $false
Save-SemiconductorPadContaminationSample $true
Save-SemiconductorDieContaminationSample $false
Save-SemiconductorDieContaminationSample $true
Save-SemiconductorUnderfillVoidSample $false
Save-SemiconductorUnderfillVoidSample $true
Save-SemiconductorPackageVoidSample $false
Save-SemiconductorPackageVoidSample $true
Save-SemiconductorSolderBridgeSample $false
Save-SemiconductorSolderBridgeSample $true
Save-SemiconductorPadScratchSample $false
Save-SemiconductorPadScratchSample $true
Save-SemiconductorBondPadNickSample $false
Save-SemiconductorBondPadNickSample $true
Save-SemiconductorWireBondLiftSample $false
Save-SemiconductorWireBondLiftSample $true
Save-SemiconductorWireSweepAlignmentSample $false
Save-SemiconductorWireSweepAlignmentSample $true
Save-SemiconductorBondPadCorrosionSample $false
Save-SemiconductorBondPadCorrosionSample $true
Save-SemiconductorPadPitchSample $false
Save-SemiconductorPadPitchSample $true
Save-SemiconductorRotationMarkSample $false
Save-SemiconductorRotationMarkSample $true
Save-SemiconductorLeadAlignmentSample $false
Save-SemiconductorLeadAlignmentSample $true
Save-SemiconductorLeadWidthSample $false
Save-SemiconductorLeadWidthSample $true
Save-SemiconductorLeadCoplanaritySample $false
Save-SemiconductorLeadCoplanaritySample $true
Save-SemiconductorProbeMarkSample $false
Save-SemiconductorProbeMarkSample $true
Save-SemiconductorDieEdgeChipSample $false
Save-SemiconductorDieEdgeChipSample $true
Save-SemiconductorMoldingFlashSample $false
Save-SemiconductorMoldingFlashSample $true
Save-SemiconductorPackageCrackSample $false
Save-SemiconductorPackageCrackSample $true
Save-SemiconductorPackageCornerChipSample $false
Save-SemiconductorPackageCornerChipSample $true
Save-SemiconductorLeadBurrSample $false
Save-SemiconductorLeadBurrSample $true
Save-SemiconductorLeadCrackSample $false
Save-SemiconductorLeadCrackSample $true
Save-SemiconductorLeadOxidationSample $false
Save-SemiconductorLeadOxidationSample $true
Save-SemiconductorPackagePolaritySample $false
Save-SemiconductorPackagePolaritySample $true
Save-SemiconductorPackageLaserTextSample $false
Save-SemiconductorPackageLaserTextSample $true
Save-SemiconductorWaferDieMarkSample $false
Save-SemiconductorWaferDieMarkSample $true
Save-ProductManifest
