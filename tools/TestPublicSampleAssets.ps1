param(
    [string]$Root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path,
    [string]$PublicCatalogPath = "docs\samples\OpenVisionLab.PublicSampleCatalog.csv",
    [string]$ManifestPath = "docs\samples\public\OpenVisionLab.PublicSampleManifest.csv"
)

$ErrorActionPreference = "Stop"

function Normalize-RepoPath {
    param([string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ""
    }

    return $Path.Trim().Trim('"').Replace('\', '/')
}

function Join-RepoPath {
    param([string]$RelativePath)

    return Join-Path $Root ($RelativePath.Replace('/', [IO.Path]::DirectorySeparatorChar))
}

function Get-RepoRelativePath {
    param([string]$AbsolutePath)

    $rootFullPath = [IO.Path]::GetFullPath($Root).TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
    $fileFullPath = [IO.Path]::GetFullPath($AbsolutePath)
    if ($fileFullPath.StartsWith($rootFullPath, [StringComparison]::OrdinalIgnoreCase)) {
        return $fileFullPath.Substring($rootFullPath.Length)
    }

    return $fileFullPath
}

function Fail {
    param([string]$Message)

    throw "PublicSampleAssetCheck=FAIL | $Message"
}

function Assert-NoForbiddenReference {
    param(
        [string]$Text,
        [string]$Source
    )

    $patterns = @(
        'Sample\',
        'Sample/',
        'bin\Debug\EasyMatch',
        'EasyMatch\',
        'EasyMatch/',
        'Euresys',
        '유레시스',
        'MVTec'
    )

    foreach ($pattern in $patterns) {
        if ($Text.IndexOf($pattern, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
            Fail "$Source contains forbidden public sample reference: $pattern"
        }
    }
}

function Assert-PublicAssetPath {
    param(
        [string]$Path,
        [string]$Source,
        [bool]$RequireManifest = $true
    )

    $normalized = Normalize-RepoPath $Path
    if ([string]::IsNullOrWhiteSpace($normalized)) {
        return
    }

    Assert-NoForbiddenReference -Text $normalized -Source $Source

    if (-not $normalized.StartsWith("docs/samples/public/", [StringComparison]::OrdinalIgnoreCase)) {
        Fail "$Source points outside docs/samples/public: $Path"
    }

    $absolute = Join-RepoPath $normalized
    if (-not (Test-Path -LiteralPath $absolute -PathType Leaf)) {
        Fail "$Source file is missing: $Path"
    }

    if ($RequireManifest -and -not $script:ManifestAssets.Contains($normalized)) {
        Fail "$Source is not listed in ${ManifestPath}: $Path"
    }
}

function Assert-PublicPipelinePath {
    param(
        [string]$Path,
        [string]$Source
    )

    $normalized = Normalize-RepoPath $Path
    if ([string]::IsNullOrWhiteSpace($normalized)) {
        Fail "$Source is missing pipeline path."
    }

    Assert-NoForbiddenReference -Text $normalized -Source $Source

    if (-not $normalized.StartsWith("docs/samples/public/", [StringComparison]::OrdinalIgnoreCase) -or
        -not $normalized.EndsWith(".pipeline.xml", [StringComparison]::OrdinalIgnoreCase)) {
        Fail "$Source pipeline must be under docs/samples/public and end with .pipeline.xml: $Path"
    }

    $absolute = Join-RepoPath $normalized
    if (-not (Test-Path -LiteralPath $absolute -PathType Leaf)) {
        Fail "$Source pipeline file is missing: $Path"
    }

    return $absolute
}

function Assert-PipelineImageReferences {
    param(
        [string]$PipelinePath,
        [string]$DisplayPath
    )

    $text = Get-Content -LiteralPath $PipelinePath -Raw -Encoding UTF8
    Assert-NoForbiddenReference -Text $text -Source $DisplayPath

    [xml]$xml = $text
    $values = @($xml.SelectNodes("//Value") | ForEach-Object { $_.InnerText })
    foreach ($value in $values) {
        $normalized = Normalize-RepoPath $value
        if ($normalized -match '\.(png|bmp|jpg|jpeg|tif|tiff)$') {
            Assert-PublicAssetPath -Path $normalized -Source "$DisplayPath image parameter"
        }
    }
}

$catalogAbsolute = Join-RepoPath $PublicCatalogPath
$manifestPathCandidates = New-Object 'System.Collections.Generic.List[string]'
[void]$manifestPathCandidates.Add($ManifestPath)
foreach ($defaultManifestPath in @(
    "docs\samples\public\OpenVisionLab.PublicSampleManifest.csv",
    "docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv"
)) {
    if (-not $manifestPathCandidates.Contains($defaultManifestPath)) {
        [void]$manifestPathCandidates.Add($defaultManifestPath)
    }
}

if (-not (Test-Path -LiteralPath $catalogAbsolute -PathType Leaf)) {
    Fail "Public catalog is missing: $PublicCatalogPath"
}

$manifestAbsolutePaths = @()
foreach ($candidateManifestPath in $manifestPathCandidates) {
    $candidateAbsolute = Join-RepoPath $candidateManifestPath
    if (Test-Path -LiteralPath $candidateAbsolute -PathType Leaf) {
        $manifestAbsolutePaths += [pscustomobject]@{
            DisplayPath = $candidateManifestPath
            FullPath = $candidateAbsolute
        }
    }
}

if ($manifestAbsolutePaths.Count -eq 0) {
    Fail "Public sample manifest is missing: $ManifestPath"
}

$catalogText = Get-Content -LiteralPath $catalogAbsolute -Raw -Encoding UTF8
Assert-NoForbiddenReference -Text $catalogText -Source $PublicCatalogPath
foreach ($manifestAbsolutePath in $manifestAbsolutePaths) {
    $manifestText = Get-Content -LiteralPath $manifestAbsolutePath.FullPath -Raw -Encoding UTF8
    Assert-NoForbiddenReference -Text $manifestText -Source $manifestAbsolutePath.DisplayPath
}

$script:ManifestAssets = New-Object 'System.Collections.Generic.HashSet[string]' ([StringComparer]::OrdinalIgnoreCase)
foreach ($manifestAbsolutePath in $manifestAbsolutePaths) {
    $manifestRows = Import-Csv -LiteralPath $manifestAbsolutePath.FullPath
    foreach ($row in $manifestRows) {
        $assetPath = Normalize-RepoPath $row.AssetPath
        if ([string]::IsNullOrWhiteSpace($assetPath)) {
            Fail "$($manifestAbsolutePath.DisplayPath) contains an empty AssetPath."
        }

        Assert-NoForbiddenReference -Text $assetPath -Source $manifestAbsolutePath.DisplayPath
        if (-not $assetPath.StartsWith("docs/samples/public/", [StringComparison]::OrdinalIgnoreCase)) {
            Fail "$($manifestAbsolutePath.DisplayPath) asset is outside docs/samples/public: $($row.AssetPath)"
        }

        $absolute = Join-RepoPath $assetPath
        if (-not (Test-Path -LiteralPath $absolute -PathType Leaf)) {
            Fail "$($manifestAbsolutePath.DisplayPath) asset is missing: $($row.AssetPath)"
        }

        [void]$script:ManifestAssets.Add($assetPath)
    }
}

$publicSamplesDir = Join-RepoPath "docs/samples/public"
$imageExtensions = New-Object 'System.Collections.Generic.HashSet[string]' ([StringComparer]::OrdinalIgnoreCase)
foreach ($extension in @(".png", ".bmp", ".jpg", ".jpeg", ".tif", ".tiff")) {
    [void]$imageExtensions.Add($extension)
}

$publicImageFiles = Get-ChildItem -LiteralPath $publicSamplesDir -File -Recurse |
    Where-Object { $imageExtensions.Contains($_.Extension) }
foreach ($file in $publicImageFiles) {
    $relative = Normalize-RepoPath (Get-RepoRelativePath $file.FullName)
    if (-not $script:ManifestAssets.Contains($relative)) {
        Fail "Public sample image is not listed in the public manifest set: $relative"
    }
}

$catalogRows = Import-Csv -LiteralPath $catalogAbsolute
if ($catalogRows.Count -lt 24) {
    Fail "Public catalog should contain Matching, Blob, Contour, Threshold, Filter, EdgeDetection, Morphology, Mean, Arithmetic, HSV, FeatureMatching, EdgeBasedMatching, and LineDistance Good/Bad pairs."
}

$sampleNames = New-Object 'System.Collections.Generic.HashSet[string]' ([StringComparer]::OrdinalIgnoreCase)
$pipelines = New-Object 'System.Collections.Generic.HashSet[string]' ([StringComparer]::OrdinalIgnoreCase)
$isProductCatalog = (Normalize-RepoPath $PublicCatalogPath).IndexOf("ProductSampleCatalog.csv", [StringComparison]::OrdinalIgnoreCase) -ge 0
foreach ($row in $catalogRows) {
    if ([string]::IsNullOrWhiteSpace($row.SampleName)) {
        Fail "Public catalog contains an empty SampleName."
    }

    [void]$sampleNames.Add($row.SampleName)
    Assert-PublicAssetPath -Path $row.ImagePath -Source "$($row.SampleName) ImagePath"
    Assert-PublicAssetPath -Path $row.ReferenceImagePath -Source "$($row.SampleName) ReferenceImagePath"
    $pipelineAbsolute = Assert-PublicPipelinePath -Path $row.BaselinePipeline -Source "$($row.SampleName) BaselinePipeline"
    $pipelineRelative = Normalize-RepoPath $row.BaselinePipeline
    [void]$pipelines.Add($pipelineRelative)
    Assert-PipelineImageReferences -PipelinePath $pipelineAbsolute -DisplayPath $pipelineRelative
}

if ($isProductCatalog) {
    foreach ($required in @(
        "Product_Battery_TabGap_Good",
        "Product_Display_PixelDefect_Good",
        "Product_Semiconductor_Fiducial_Good"
    )) {
        if (-not $sampleNames.Contains($required)) {
            Fail "Product catalog is missing required row: $required"
        }
    }

    if ($pipelines.Count -lt 40) {
        Fail "Product catalog should contain a broad set of domain/tool pipelines."
    }
}
else {
    foreach ($required in @(
        "Public_Matching_DiePad_Good",
        "Public_Blob_Particles_Good",
        "Public_Contour_Shapes_Good",
        "Public_Threshold_BandPads_Good",
        "Public_Filter_Denoise_Good",
        "Public_EdgeDetection_Shapes_Good",
        "Public_Morphology_Cleanup_Good",
        "Public_Mean_Brightness_Good",
        "Public_Arithmetic_Invert_Good",
        "Public_HSV_ColorPatch_Good",
        "Public_Feature_Card_Good",
        "Public_Edge_Fiducial_Good",
        "Public_Line_Pins_Good",
        "Public_Geometry_RotateScale_Wide_Bad"
    )) {
        if (-not $sampleNames.Contains($required)) {
            Fail "Public catalog is missing required row: $required"
        }
    }

    foreach ($required in @(
        "docs/samples/public/Public_Matching_DiePad.pipeline.xml",
        "docs/samples/public/Public_Blob_Particles.pipeline.xml",
        "docs/samples/public/Public_Contour_Shapes.pipeline.xml",
        "docs/samples/public/Public_Threshold_BandPads.pipeline.xml",
        "docs/samples/public/Public_Filter_Denoise.pipeline.xml",
        "docs/samples/public/Public_EdgeDetection_Shapes.pipeline.xml",
        "docs/samples/public/Public_Morphology_Cleanup.pipeline.xml",
        "docs/samples/public/Public_Mean_BrightnessDrift.pipeline.xml",
        "docs/samples/public/Public_Arithmetic_Invert.pipeline.xml",
        "docs/samples/public/Public_HSV_ColorPatch.pipeline.xml",
        "docs/samples/public/Public_Feature_Card.pipeline.xml",
        "docs/samples/public/Public_Edge_Fiducial.pipeline.xml",
        "docs/samples/public/Public_Line_Pins_Distance.pipeline.xml"
    )) {
        if (-not $pipelines.Contains($required)) {
            Fail "Public catalog is missing required pipeline: $required"
        }
    }
}

Write-Host "PublicSampleAssetCheck=PASS | CatalogRows=$($catalogRows.Count) ManifestAssets=$($script:ManifestAssets.Count) Pipelines=$($pipelines.Count)"
