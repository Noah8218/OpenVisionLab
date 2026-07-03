param(
    [string]$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path,
    [string]$CatalogArtifactDir = (Join-Path (Resolve-Path (Join-Path $PSScriptRoot '..')).Path 'artifacts\public_sample_catalog_20260702_learn_evidence'),
    [string]$ProductCatalogArtifactDir = ''
)

$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing

$targetDir = Join-Path $Root 'docs\assets\tutorial\current'
New-Item -ItemType Directory -Force -Path $targetDir | Out-Null

function Copy-EvidenceFile([string]$SourcePath, [string]$TargetPath) {
    try {
        Copy-Item -LiteralPath $SourcePath -Destination $TargetPath -Force
        Write-Host "Copied: $TargetPath"
    }
    catch [System.IO.IOException] {
        if (Test-Path -LiteralPath $TargetPath) {
            Write-Warning "Skipped locked evidence file already present: $TargetPath"
            return
        }

        throw
    }
}

$images = @(
    @{ Source = 'Public_Matching_DiePad_Good.png'; Target = 'public_matching_diepad_good_result.png' },
    @{ Source = 'Public_Matching_DiePad_NoTarget_Bad.png'; Target = 'public_matching_diepad_no_target_bad_result.png' },
    @{ Source = 'Public_Blob_Particles_Good.png'; Target = 'public_blob_particles_good_result.png' },
    @{ Source = 'Public_Blob_Particles_Sparse_Bad.png'; Target = 'public_blob_particles_sparse_bad_result.png' },
    @{ Source = 'Public_Contour_Shapes_Good.png'; Target = 'public_contour_shapes_good_result.png' },
    @{ Source = 'Public_Contour_Shapes_Missing_Bad.png'; Target = 'public_contour_shapes_missing_bad_result.png' },
    @{ Source = 'Public_Threshold_BandPads_Good.png'; Target = 'public_threshold_bandpads_good_result.png' },
    @{ Source = 'Public_Threshold_BandPads_Missing_Bad.png'; Target = 'public_threshold_bandpads_missing_bad_result.png' },
    @{ Source = 'Public_Mean_Brightness_Good.png'; Target = 'public_mean_brightness_good_result.png' },
    @{ Source = 'Public_Mean_Brightness_Dark_Bad.png'; Target = 'public_mean_brightness_dark_bad_result.png' },
    @{ Source = 'Public_Feature_Card_Good.png'; Target = 'public_feature_card_good_result.png' },
    @{ Source = 'Public_Feature_Card_Wrong_Bad.png'; Target = 'public_feature_card_wrong_bad_result.png' },
    @{ Source = 'Public_Edge_Fiducial_Good.png'; Target = 'public_edge_fiducial_good_result.png' },
    @{ Source = 'Public_Edge_Fiducial_Wrong_Bad.png'; Target = 'public_edge_fiducial_wrong_bad_result.png' },
    @{ Source = 'Public_Line_Pins_Good.png'; Target = 'public_line_pins_good_result.png' },
    @{ Source = 'Public_Line_Pins_WidePin_Bad.png'; Target = 'public_line_pins_widepin_bad_result.png' }
)

foreach ($image in $images) {
    $sourcePath = Join-Path $CatalogArtifactDir $image.Source
    if (-not (Test-Path -LiteralPath $sourcePath)) {
        throw "Missing public sample evidence image: $sourcePath"
    }

    $targetPath = Join-Path $targetDir $image.Target
    Copy-EvidenceFile $sourcePath $targetPath
}

$summaryPath = Join-Path $CatalogArtifactDir 'sample_catalog_summary.json'
if (Test-Path -LiteralPath $summaryPath) {
    Copy-EvidenceFile $summaryPath (Join-Path $targetDir 'public_learn_evidence_summary.json')
}

if (-not [string]::IsNullOrWhiteSpace($ProductCatalogArtifactDir)) {
    $productSource = Join-Path $ProductCatalogArtifactDir 'product_expansion_source_result_sheet.png'
    if (-not (Test-Path -LiteralPath $productSource)) {
        $productSource = Join-Path $ProductCatalogArtifactDir 'product_latest_source_result_sheet.png'
        $productRoot = Join-Path $Root 'docs\samples\public\product'
        $items = @(
            @{ Title = 'Battery pouch seal bubble OK - source'; File = (Join-Path $productRoot 'Battery_PouchSealBubble_OK.png') },
            @{ Title = 'Battery pouch seal bubble OK - result'; File = (Join-Path $ProductCatalogArtifactDir 'Product_Battery_PouchSealBubble_Good.png') },
            @{ Title = 'Battery pouch seal bubble NG - source'; File = (Join-Path $productRoot 'Battery_PouchSealBubble_Many_NG.png') },
            @{ Title = 'Battery pouch seal bubble NG - result'; File = (Join-Path $ProductCatalogArtifactDir 'Product_Battery_PouchSealBubble_Many_Bad.png') },
            @{ Title = 'Display pad bridge OK - source'; File = (Join-Path $productRoot 'Display_PadBridge_OK.png') },
            @{ Title = 'Display pad bridge OK - result'; File = (Join-Path $ProductCatalogArtifactDir 'Product_Display_PadBridge_Good.png') },
            @{ Title = 'Display pad bridge NG - source'; File = (Join-Path $productRoot 'Display_PadBridge_Many_NG.png') },
            @{ Title = 'Display pad bridge NG - result'; File = (Join-Path $ProductCatalogArtifactDir 'Product_Display_PadBridge_Many_Bad.png') },
            @{ Title = 'Semiconductor package corner chip OK - source'; File = (Join-Path $productRoot 'Semiconductor_PackageCornerChip_OK.png') },
            @{ Title = 'Semiconductor package corner chip OK - result'; File = (Join-Path $ProductCatalogArtifactDir 'Product_Semiconductor_PackageCornerChip_Good.png') },
            @{ Title = 'Semiconductor package corner chip NG - source'; File = (Join-Path $productRoot 'Semiconductor_PackageCornerChip_Many_NG.png') },
            @{ Title = 'Semiconductor package corner chip NG - result'; File = (Join-Path $ProductCatalogArtifactDir 'Product_Semiconductor_PackageCornerChip_Many_Bad.png') }
        )

        foreach ($item in $items) {
            if (-not (Test-Path -LiteralPath $item.File)) {
                throw "Missing product sample evidence image: $($item.File)"
            }
        }

        $thumbW = 286
        $thumbH = 210
        $pad = 18
        $labelH = 44
        $cols = 4
        $rows = [Math]::Ceiling($items.Count / $cols)
        $bitmap = [System.Drawing.Bitmap]::new((($thumbW + $pad) * $cols + $pad), (($thumbH + $labelH + $pad) * $rows + $pad))
        $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
        try {
            $graphics.Clear([System.Drawing.Color]::FromArgb(20, 28, 30))
            $font = [System.Drawing.Font]::new('Segoe UI', 9, [System.Drawing.FontStyle]::Bold)
            $brush = [System.Drawing.Brushes]::White
            $format = [System.Drawing.StringFormat]::new()
            $format.FormatFlags = [System.Drawing.StringFormatFlags]::LineLimit
            $format.Trimming = [System.Drawing.StringTrimming]::EllipsisWord
            try {
                for ($i = 0; $i -lt $items.Count; $i++) {
                    $col = $i % $cols
                    $row = [Math]::Floor($i / $cols)
                    $x = $pad + ($col * ($thumbW + $pad))
                    $y = $pad + ($row * ($thumbH + $labelH + $pad))
                    $titleRect = [System.Drawing.RectangleF]::new($x, $y, $thumbW, $labelH)
                    $graphics.DrawString($items[$i].Title, $font, $brush, $titleRect, $format)
                    $image = [System.Drawing.Image]::FromFile((Resolve-Path -LiteralPath $items[$i].File))
                    try {
                        $graphics.DrawImage($image, $x, $y + $labelH, $thumbW, $thumbH)
                    }
                    finally {
                        $image.Dispose()
                    }
                }
            }
            finally {
                $format.Dispose()
                $font.Dispose()
            }

            $bitmap.Save($productSource, [System.Drawing.Imaging.ImageFormat]::Png)
            Write-Host "Generated: $productSource"
        }
        finally {
            $graphics.Dispose()
            $bitmap.Dispose()
        }
    }

    $annotatedDir = Join-Path $Root 'docs\assets\tutorial\annotated'
    New-Item -ItemType Directory -Force -Path $annotatedDir | Out-Null
    $productTarget = Join-Path $annotatedDir 'product_sample_source_result_sheet.png'
    Copy-Item -LiteralPath $productSource -Destination $productTarget -Force
    Write-Host "Copied: $productTarget"
}
