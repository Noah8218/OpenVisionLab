param(
    [Parameter(Mandatory = $true)] [string] $ValidationRoot,
    [Parameter(Mandatory = $false)] [string] $OutputRoot
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$visualRoot = Join-Path $ValidationRoot 'visual-correspondence-full'
if ([string]::IsNullOrWhiteSpace($OutputRoot)) {
    $OutputRoot = Join-Path $visualRoot 'operator-review-boards'
}
$sheetRoot = Join-Path $OutputRoot 'contact-sheets'
New-Item -ItemType Directory -Force -Path $sheetRoot | Out-Null

$ledgerPath = Join-Path $visualRoot 'full-visual-review.csv'
$ledger = @(Import-Csv -LiteralPath $ledgerPath)
if ($ledger.Count -ne 244) {
    throw "Expected 244 visual-review rows, found $($ledger.Count)."
}

function New-Canvas([int] $width, [int] $height, [System.Drawing.Color] $color) {
    $bitmap = New-Object System.Drawing.Bitmap($width, $height, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    try { $graphics.Clear($color) } finally { $graphics.Dispose() }
    return $bitmap
}

function Draw-OperatorSheet([string] $path, [object[]] $rows, [string] $variant, [int] $sheetNumber) {
    $fullWidth = 220
    $fullHeight = 220
    $panelWidth = 430
    $panelHeight = 122
    $innerGap = 10
    $cellWidth = $fullWidth + $innerGap + $panelWidth
    $cellHeight = 280
    $outerGap = 12
    $titleHeight = 30
    $columns = 2
    $lineCount = [Math]::Ceiling($rows.Count / [double]$columns)
    $canvasWidth = $columns * $cellWidth + ($columns + 1) * $outerGap
    $canvasHeight = $titleHeight + $lineCount * $cellHeight + ($lineCount + 1) * $outerGap
    $canvas = New-Canvas $canvasWidth $canvasHeight ([System.Drawing.Color]::FromArgb(14, 14, 14))
    $graphics = [System.Drawing.Graphics]::FromImage($canvas)
    $font = New-Object System.Drawing.Font('Segoe UI', 8, [System.Drawing.FontStyle]::Regular)
    $titleFont = New-Object System.Drawing.Font('Segoe UI', 10, [System.Drawing.FontStyle]::Regular)
    $white = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
    $muted = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::Gainsboro)
    $passPen = New-Object System.Drawing.Pen([System.Drawing.Color]::Lime, 1)
    $waitPen = New-Object System.Drawing.Pen([System.Drawing.Color]::Orange, 2)
    $neutralPen = New-Object System.Drawing.Pen([System.Drawing.Color]::DimGray, 1)
    try {
        $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $graphics.DrawString(("{0} operator review | full-image 01 overlay + template/patch/blend | sheet {1}" -f $variant, $sheetNumber), $titleFont, $white, $outerGap, 6)

        for ($i = 0; $i -lt $rows.Count; $i++) {
            $row = $rows[$i]
            $column = $i % $columns
            $line = [Math]::Floor($i / $columns)
            $x = $outerGap + $column * $cellWidth
            $y = $titleHeight + $outerGap + $line * $cellHeight
            $overlay = [System.Drawing.Image]::FromFile([string]$row.matchingOverlayPath)
            $panel = [System.Drawing.Image]::FromFile([string]$row.panelPath)
            try {
                $graphics.DrawImage($overlay, $x, $y, $fullWidth, $fullHeight)
                $pen = if ([string]$row.state -eq 'WAIT') { $waitPen } elseif ([string]$row.state -eq 'PASS') { $passPen } else { $neutralPen }
                $graphics.DrawRectangle($pen, $x, $y, $fullWidth - 1, $fullHeight - 1)
                $panelX = $x + $fullWidth + $innerGap
                $panelY = $y + [Math]::Floor(($fullHeight - $panelHeight) / 2.0)
                $graphics.DrawImage($panel, $panelX, $panelY, $panelWidth, $panelHeight)
                $graphics.DrawRectangle($neutralPen, $panelX, $panelY, $panelWidth - 1, $panelHeight - 1)
            } finally {
                $overlay.Dispose()
                $panel.Dispose()
            }

            $labelY = $y + $fullHeight + 3
            $name = "{0:D3} {1}" -f [int]$row.index, [string]$row.fileName
            $details = "visual={0} | runtime={1} | score={2:0.0} | resultCount={3}" -f [string]$row.state, [string]$row.downstreamStatusSecondary, [double]$row.scoreMaxSecondary, [int]$row.realCandidateCount
            $graphics.DrawString($name, $font, $white, $x, $labelY)
            $graphics.DrawString($details, $font, $muted, $x, $labelY + 14)
            $graphics.DrawString('left: full source + green 01 geometry | right: correspondence panel', $font, $muted, $x, $labelY + 28)
        }
    } finally {
        $neutralPen.Dispose()
        $waitPen.Dispose()
        $passPen.Dispose()
        $white.Dispose()
        $muted.Dispose()
        $font.Dispose()
        $titleFont.Dispose()
        $graphics.Dispose()
    }
    $canvas.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $canvas.Dispose()
}

$sheetSummary = New-Object System.Collections.Generic.List[object]
foreach ($variant in @('A', 'B')) {
    $rows = @($ledger | Where-Object { $_.variant -eq $variant } | Sort-Object { [int]$_.index })
    if ($rows.Count -ne 122) {
        throw "Expected 122 rows for variant $variant, found $($rows.Count)."
    }
    for ($start = 0; $start -lt $rows.Count; $start += 16) {
        $end = [Math]::Min($start + 15, $rows.Count - 1)
        $chunk = @($rows[$start..$end])
        $sheetNumber = [int]($start / 16 + 1)
        $sheetPath = Join-Path $sheetRoot ("{0}_operator_{1:D2}.png" -f $variant, $sheetNumber)
        Draw-OperatorSheet $sheetPath $chunk $variant $sheetNumber
        $sheetSummary.Add([pscustomobject]@{
            variant = $variant
            sheet = $sheetNumber
            firstIndex = [int]$chunk[0].index
            lastIndex = [int]$chunk[$chunk.Count - 1].index
            path = $sheetPath
            rowCount = $chunk.Count
        })
    }
}

$summary = [pscustomobject]@{
    purpose = 'Operator review of every current-run full-image Matching Step 1 geometry and correspondence panel'
    ledgerPath = $ledgerPath
    rowCount = $ledger.Count
    sheetCount = $sheetSummary.Count
    sheets = $sheetSummary.ToArray()
}
$summary | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $OutputRoot 'operator-review-boards.json') -Encoding UTF8
Write-Output ("Generated {0} operator review sheets for {1} rows under {2}" -f $sheetSummary.Count, $ledger.Count, $OutputRoot)
