param(
    [Parameter(Mandatory = $true)] [string] $ValidationRoot,
    [Parameter(Mandatory = $false)] [string] $OutputRoot
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

if ([string]::IsNullOrWhiteSpace($OutputRoot)) {
    $OutputRoot = Join-Path $ValidationRoot 'visual-correspondence-full'
}
$panelRoot = Join-Path $OutputRoot 'panels'
$patchRoot = Join-Path $OutputRoot 'patches'
$sheetRoot = Join-Path $OutputRoot 'contact-sheets'
New-Item -ItemType Directory -Force -Path $panelRoot, $patchRoot, $sheetRoot | Out-Null

$comparisonPath = Join-Path $ValidationRoot 'ab-comparison.csv'
$comparison = @(Import-Csv -LiteralPath $comparisonPath)
if ($comparison.Count -ne 122) {
    throw "Expected 122 comparison rows, found $($comparison.Count)."
}

$variants = @{
    A = Join-Path $ValidationRoot 'A\template.png'
    B = Join-Path $ValidationRoot 'B\template.png'
}

function Dispose-IfNeeded([object] $value) {
    if ($null -ne $value -and $value -is [System.IDisposable]) { $value.Dispose() }
}

function New-ArgbBitmap([int] $width, [int] $height, [System.Drawing.Color] $color) {
    $bitmap = New-Object System.Drawing.Bitmap($width, $height, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    try { $graphics.Clear($color) } finally { $graphics.Dispose() }
    return $bitmap
}

function Get-SourcePatch([System.Drawing.Bitmap] $source, [int] $centerX, [int] $centerY, [double] $scale, [double] $angle, [int] $width, [int] $height) {
    if ($scale -le 0) { throw "Invalid candidate scale: $scale" }
    $cropWidth = [Math]::Max(1, [int][Math]::Round($width * $scale))
    $cropHeight = [Math]::Max(1, [int][Math]::Round($height * $scale))
    $crop = New-ArgbBitmap $cropWidth $cropHeight ([System.Drawing.Color]::Black)
    $cropGraphics = [System.Drawing.Graphics]::FromImage($crop)
    try {
        $cropGraphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $cropGraphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $sourceRect = [System.Drawing.Rectangle]::new($centerX - [int][Math]::Round($cropWidth / 2.0), $centerY - [int][Math]::Round($cropHeight / 2.0), $cropWidth, $cropHeight)
        $destRect = [System.Drawing.Rectangle]::new(0, 0, $cropWidth, $cropHeight)
        $cropGraphics.DrawImage($source, $destRect, $sourceRect, [System.Drawing.GraphicsUnit]::Pixel)
    } finally { $cropGraphics.Dispose() }

    $patch = New-ArgbBitmap $width $height ([System.Drawing.Color]::Black)
    $patchGraphics = [System.Drawing.Graphics]::FromImage($patch)
    try {
        $patchGraphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $patchGraphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $patchGraphics.TranslateTransform($width / 2.0, $height / 2.0)
        if ([Math]::Abs($angle) -gt 0.0001) { $patchGraphics.RotateTransform(-1.0 * [float]$angle) }
        $patchGraphics.TranslateTransform(-1.0 * $width / 2.0, -1.0 * $height / 2.0)
        $patchGraphics.DrawImage($crop, 0, 0, $width, $height)
    } finally { $patchGraphics.Dispose(); $crop.Dispose() }
    return $patch
}

function New-Blended([System.Drawing.Bitmap] $template, [System.Drawing.Bitmap] $patch) {
    $blend = New-ArgbBitmap $template.Width $template.Height ([System.Drawing.Color]::Black)
    $graphics = [System.Drawing.Graphics]::FromImage($blend)
    try {
        $graphics.DrawImage($template, 0, 0, $template.Width, $template.Height)
        $attributes = New-Object System.Drawing.Imaging.ImageAttributes
        try {
            $matrix = New-Object System.Drawing.Imaging.ColorMatrix
            $matrix.Matrix33 = 0.5
            $attributes.SetColorMatrix($matrix)
            $graphics.DrawImage($patch, [System.Drawing.Rectangle]::new(0, 0, $template.Width, $template.Height), 0, 0, $patch.Width, $patch.Height, [System.Drawing.GraphicsUnit]::Pixel, $attributes)
        } finally { $attributes.Dispose() }
    } finally { $graphics.Dispose() }
    return $blend
}

function Draw-Panel([string] $path, [string] $title, [System.Drawing.Bitmap] $template, [System.Drawing.Bitmap] $patch, [System.Drawing.Bitmap] $blend) {
    $scale = 3
    $gap = 8
    $labelHeight = 22
    $top = 26
    $cellWidth = [Math]::Max([Math]::Max($template.Width, $patch.Width), $blend.Width) * $scale
    $cellHeight = [Math]::Max([Math]::Max($template.Height, $patch.Height), $blend.Height) * $scale + $labelHeight
    $canvas = New-ArgbBitmap ($cellWidth * 3 + $gap * 4) ($top + $cellHeight + 8) ([System.Drawing.Color]::FromArgb(18, 18, 18))
    $graphics = [System.Drawing.Graphics]::FromImage($canvas)
    try {
        $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
        $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $font = New-Object System.Drawing.Font('Segoe UI', 9, [System.Drawing.FontStyle]::Regular)
        $titleFont = New-Object System.Drawing.Font('Segoe UI', 10, [System.Drawing.FontStyle]::Regular)
        $white = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
        $muted = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::Gainsboro)
        try {
            $graphics.DrawString($title, $titleFont, $white, 8, 4)
            $items = @(
                @{ image = $template; label = 'template' },
                @{ image = $patch; label = 'source patch @ reported pose' },
                @{ image = $blend; label = '50% blend' }
            )
            for ($i = 0; $i -lt $items.Count; $i++) {
                $x = $gap + $i * ($cellWidth + $gap)
                $image = $items[$i].image
                $drawWidth = $image.Width * $scale
                $drawHeight = $image.Height * $scale
                $y = $top
                $graphics.DrawImage($image, $x, $y, $drawWidth, $drawHeight)
                $graphics.DrawRectangle([System.Drawing.Pens]::Lime, $x, $y, $drawWidth - 1, $drawHeight - 1)
                $graphics.DrawString($items[$i].label, $font, $muted, $x, $y + $drawHeight + 2)
            }
        } finally { $white.Dispose(); $muted.Dispose(); $font.Dispose(); $titleFont.Dispose() }
    } finally { $graphics.Dispose() }
    $canvas.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $canvas.Dispose()
}

function Draw-ContactSheet([string] $path, [object[]] $rows, [hashtable] $panelCache, [int] $sheetIndex) {
    $thumbWidth = 430
    $thumbHeight = 144
    $cellGap = 8
    $labelHeight = 26
    $columns = 2
    $rowCount = [Math]::Ceiling($rows.Count / 2.0)
    $canvas = New-ArgbBitmap ($columns * $thumbWidth + ($columns + 1) * $cellGap) ($rowCount * ($thumbHeight + $labelHeight + $cellGap) + $cellGap) ([System.Drawing.Color]::FromArgb(12, 12, 12))
    $graphics = [System.Drawing.Graphics]::FromImage($canvas)
    try {
        $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $font = New-Object System.Drawing.Font('Segoe UI', 8, [System.Drawing.FontStyle]::Regular)
        $textBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
        try {
            for ($i = 0; $i -lt $rows.Count; $i++) {
                $row = $rows[$i]
                $column = $i % $columns
                $line = [Math]::Floor($i / $columns)
                $x = $cellGap + $column * ($thumbWidth + $cellGap)
                $y = $cellGap + $line * ($thumbHeight + $labelHeight + $cellGap)
                $panel = [System.Drawing.Image]::FromFile($panelCache[$row.Key])
                try {
                    $graphics.DrawImage($panel, $x, $y, $thumbWidth, $thumbHeight)
                    $graphics.DrawRectangle([System.Drawing.Pens]::DimGray, $x, $y, $thumbWidth - 1, $thumbHeight - 1)
                } finally { $panel.Dispose() }
                $graphics.DrawString($row.Label, $font, $textBrush, $x, $y + $thumbHeight + 3)
            }
        } finally { $font.Dispose(); $textBrush.Dispose() }
    } finally { $graphics.Dispose() }
    $canvas.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $canvas.Dispose()
}

$summary = New-Object System.Collections.Generic.List[object]
$contactRows = @{ A = @(); B = @() }
$panelPaths = @{ A = @{}; B = @{} }

foreach ($variant in @('A', 'B')) {
    $variantPanelRoot = Join-Path $panelRoot $variant
    $variantPatchRoot = Join-Path $patchRoot $variant
    New-Item -ItemType Directory -Force -Path $variantPanelRoot, $variantPatchRoot | Out-Null
    $template = [System.Drawing.Bitmap]::new($variants[$variant])
    try {
        for ($index = 0; $index -lt $comparison.Count; $index++) {
            $row = $comparison[$index]
            $sourcePath = [string]$row.ImagePath
            if (-not (Test-Path -LiteralPath $sourcePath)) { throw "Source image missing: $sourcePath" }
            $source = [System.Drawing.Bitmap]::new($sourcePath)
            try {
                $cx = [int][Math]::Round([double]$row.("${variant}FixtureCenterX"))
                $cy = [int][Math]::Round([double]$row.("${variant}FixtureCenterY"))
                $scale = [double]$row.("${variant}FixtureScale")
                $angle = [double]$row.("${variant}FixtureAngle")
                $patch = Get-SourcePatch $source $cx $cy $scale $angle $template.Width $template.Height
                try {
                    $blend = New-Blended $template $patch
                    try {
                        $stem = [System.IO.Path]::GetFileNameWithoutExtension($row.FileName)
                        $panelPath = Join-Path $variantPanelRoot ("{0:D3}_{1}_correspondence.png" -f ($index + 1), $stem)
                        $patchPath = Join-Path $variantPatchRoot ("{0:D3}_{1}_source_patch.png" -f ($index + 1), $stem)
                        $blendPath = Join-Path $variantPatchRoot ("{0:D3}_{1}_blend.png" -f ($index + 1), $stem)
                        $patch.Save($patchPath, [System.Drawing.Imaging.ImageFormat]::Png)
                        $blend.Save($blendPath, [System.Drawing.Imaging.ImageFormat]::Png)
                        $title = "{0} / {1} | pose=({2},{3}) scale={4:0.###} angle={5:0.###} | score={6:0.###} resultCount={7} | downstream={8}" -f $variant, $row.FileName, $cx, $cy, $scale, $angle, [double]$row.("${variant}ScoreMax"), $row.("${variant}ResultCount"), $row.("${variant}Status")
                        Draw-Panel $panelPath $title $template $patch $blend
                        $panelPaths[$variant][$row.FileName] = $panelPath
                        $contactRows[$variant] += [pscustomobject]@{ Key = $row.FileName; Label = ("{0:D3} {1} | score {2:0.0} | {3}" -f ($index + 1), $row.FileName, [double]$row.("${variant}ScoreMax"), $row.("${variant}Status")) }
                        $summary.Add([pscustomobject]@{
                            index = $index + 1; variant = $variant; fileName = $row.FileName; sourcePath = $sourcePath; panelPath = $panelPath; patchPath = $patchPath; blendPath = $blendPath;
                            centerX = $cx; centerY = $cy; scale = $scale; angle = $angle; scoreMax = [double]$row.("${variant}ScoreMax");
                            resultCount = [int][double]$row.("${variant}ResultCount"); downstreamStatus = [string]$row.("${variant}Status")
                        })
                    } finally { $blend.Dispose() }
                } finally { $patch.Dispose() }
            } finally { $source.Dispose() }
        }
    } finally { $template.Dispose() }
}

foreach ($variant in @('A', 'B')) {
    $rows = @($contactRows[$variant])
    for ($start = 0; $start -lt $rows.Count; $start += 16) {
        $chunk = @($rows[$start..([Math]::Min($start + 15, $rows.Count - 1))])
        $sheetPath = Join-Path $sheetRoot ("{0}_sheet_{1:D2}.png" -f $variant, [int]($start / 16 + 1))
        Draw-ContactSheet $sheetPath $chunk $panelPaths[$variant] ([int]($start / 16 + 1))
    }
}

$summaryPath = Join-Path $OutputRoot 'visual-correspondence-full-summary.json'
$summary | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $summaryPath -Encoding UTF8
Write-Output ("Generated {0} correspondence panels and {1} contact sheets under {2}" -f $summary.Count, ((Get-ChildItem -LiteralPath $sheetRoot -File -Filter '*.png').Count), $OutputRoot)
