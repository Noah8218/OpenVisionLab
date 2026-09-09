param(
    [Parameter(Mandatory = $true)] [string] $ValidationRoot,
    [Parameter(Mandatory = $true)] [string] $OutputRoot
)

$ErrorActionPreference = 'Stop'

function Get-Sha256([string] $path) {
    return (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToUpperInvariant()
}

function Get-SafeName([string] $value) {
    $invalid = [System.IO.Path]::GetInvalidFileNameChars()
    $safe = $value
    foreach ($char in $invalid) { $safe = $safe.Replace([string]$char, '_') }
    return $safe
}

function Import-Utf8Csv([string] $path) {
    return @(Get-Content -Raw -Encoding UTF8 -LiteralPath $path | ConvertFrom-Csv)
}

$imageListPath = Join-Path $ValidationRoot 'image-list.txt'
if (-not (Test-Path -LiteralPath $imageListPath -PathType Leaf)) { throw "Image list missing: $imageListPath" }
$imagePaths = @(Get-Content -Encoding UTF8 -LiteralPath $imageListPath | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
if ($imagePaths.Count -ne 122) { throw "Expected 122 image-list rows, found $($imagePaths.Count)." }

New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null
$allRows = New-Object System.Collections.Generic.List[object]
$variantSummaries = New-Object System.Collections.Generic.List[object]

foreach ($variant in @('A', 'B')) {
    $sourceCsv = Join-Path $ValidationRoot "$variant\evidence\evidence_rows.csv"
    if (-not (Test-Path -LiteralPath $sourceCsv -PathType Leaf)) { throw "Evidence CSV missing: $sourceCsv" }
    $sourceRows = @(Import-Utf8Csv $sourceCsv | Where-Object { [string]$_.StepIndex -eq '1' })
    if ($sourceRows.Count -ne $imagePaths.Count) { throw "$variant Step 1 row count is $($sourceRows.Count), expected $($imagePaths.Count)." }
    $byPath = @{}
    foreach ($row in $sourceRows) { $byPath[[string]$row.ImagePath] = $row }

    $variantOutput = Join-Path $OutputRoot $variant
    New-Item -ItemType Directory -Force -Path $variantOutput | Out-Null
    $variantRows = New-Object System.Collections.Generic.List[object]
    for ($index = 0; $index -lt $imagePaths.Count; $index++) {
        $imagePath = [string]$imagePaths[$index]
        if (-not $byPath.ContainsKey($imagePath)) { throw "$variant evidence row missing image-list path: $imagePath" }
        $row = $byPath[$imagePath]
        $sourceOverlay = [string]$row.StepOverlayPath
        if (-not (Test-Path -LiteralPath $sourceOverlay -PathType Leaf)) { throw "Overlay missing: $sourceOverlay" }
        $fileName = [System.IO.Path]::GetFileName($imagePath)
        $stem = [System.IO.Path]::GetFileNameWithoutExtension($fileName)
        $state = if ([string]$row.StepStatus -eq 'OK') { 'MATCH_OK' } else { 'NORESULT' }
        $outputName = "{0:D3}_{1}_{2}__{3}.png" -f ($index + 1), [string]$row.Expected, (Get-SafeName $stem), $state
        $outputPath = Join-Path $variantOutput $outputName
        Copy-Item -LiteralPath $sourceOverlay -Destination $outputPath -Force
        $sourceHash = Get-Sha256 $sourceOverlay
        $exportHash = Get-Sha256 $outputPath
        if ($sourceHash -ne $exportHash) { throw "Copied overlay hash mismatch: $outputPath" }
        $record = [pscustomobject]@{
            index = $index + 1
            variant = $variant
            fileName = $fileName
            imagePath = $imagePath
            expected = [string]$row.Expected
            stepStatus = [string]$row.StepStatus
            errorName = [string]$row.ErrorName
            scoreMax = [string]$row.ScoreMax
            sourceOverlayPath = $sourceOverlay
            individualImagePath = $outputPath
            sourceOverlaySha256 = $sourceHash
            individualImageSha256 = $exportHash
            copiedExact = $true
        }
        $variantRows.Add($record) | Out-Null
        $allRows.Add($record) | Out-Null
    }
    $variantRows | Export-Csv -LiteralPath (Join-Path $variantOutput 'manifest.csv') -NoTypeInformation -Encoding UTF8
    $variantSummaries.Add([pscustomobject]@{
        variant = $variant
        count = $variantRows.Count
        matchOk = @($variantRows | Where-Object { $_.stepStatus -eq 'OK' }).Count
        noResult = @($variantRows | Where-Object { $_.stepStatus -ne 'OK' }).Count
        directory = $variantOutput
        manifest = (Join-Path $variantOutput 'manifest.csv')
    }) | Out-Null
}

$manifest = [ordered]@{
    schemaVersion = '1.0'
    purpose = 'One individual current-run Matching Step 1 overlay per image and variant'
    sourceValidationRoot = $ValidationRoot
    imageListPath = $imageListPath
    imageListSha256 = (Get-Sha256 $imageListPath)
    generatedUtc = [DateTime]::UtcNow.ToString('o')
    exactCopy = $true
    variants = @($variantSummaries.ToArray())
    rows = @($allRows.ToArray())
}
$manifestPath = Join-Path $OutputRoot 'manifest.json'
$manifest | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $manifestPath -Encoding UTF8

$readme = @(
    '# Die Pad individual detection overlays',
    '',
    'This folder contains the exact current-run `Matching Step 1` overlay for every image, separated by template variant.',
    '',
    '- `A\` contains 122 files.',
    '- `B\` contains 122 files.',
    '- File names are ordered by `image-list.txt`: `001` through `122`.',
    '- `MATCH_OK` means Step 1 returned `OK`; `NORESULT` preserves the original no-result/error overlay.',
    '- Files are byte-for-byte copies of the runner overlays; each copy is hash-checked.',
    '',
    "Open `A\` or `B\` in Explorer and sort by Name to inspect them one by one.",
    "Manifest: $manifestPath"
)
$readme | Set-Content -LiteralPath (Join-Path $OutputRoot 'README.md') -Encoding UTF8
Write-Output "Exported 244 individual overlays under $OutputRoot"
