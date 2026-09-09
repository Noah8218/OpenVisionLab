param(
    [Parameter(Mandatory = $true)] [string] $ValidationRoot,
    [Parameter(Mandatory = $false)] [string] $ReviewDate = '2026-08-28'
)

$ErrorActionPreference = 'Stop'
$visualRoot = Join-Path $ValidationRoot 'visual-correspondence-full'
$comparison = @(Import-Csv -LiteralPath (Join-Path $ValidationRoot 'ab-comparison.csv'))
$generated = @(Get-Content -Raw -LiteralPath (Join-Path $visualRoot 'visual-correspondence-full-summary.json') | ConvertFrom-Json)
# Windows PowerShell 5.1 keeps a top-level JSON array as one Object[] pipeline
# item; flatten it so each generated evidence row remains scalar.
if ($generated.Count -eq 1 -and $generated[0] -is [array]) { $generated = $generated[0] }
$aEvidence = @(Import-Csv -LiteralPath (Join-Path $ValidationRoot 'A\evidence\evidence_rows.csv'))
$sourceHashes = @{}
foreach ($row in $aEvidence) {
    if (-not $sourceHashes.ContainsKey([string]$row.ImagePath)) { $sourceHashes[[string]$row.ImagePath] = [string]$row.SourceSha256 }
}
function Get-Sha256([string] $path) {
    return (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToUpperInvariant()
}

# These are the rows that the current-run panels visibly show as occluded,
# overlaid, or carrying an unexplained extra structure. The decision is kept
# conservative: no approved tolerance exists, so these rows remain WAIT even
# when the matcher score is high or the downstream step is OK.
$aWait = @{
    'die_pad_004_ng.jpg'  = 'UNEXPLAINED_EXTRA_STRUCTURE_RIGHT'
    'die_pad_026_ng.jpg'  = 'OCCLUSION_LEFT_PAD_NO_APPROVED_TOLERANCE'
    'die_pad_028_ng.jpg'  = 'UNEXPLAINED_EXTRA_STRUCTURE_TOP'
    'die_pad_062_ng.jpg'  = 'UNEXPLAINED_EXTRA_STRUCTURE_LOWER_RIGHT'
    'die_pad_072_ng.jpg'  = 'OCCLUSION_TOP_TRACE_NO_APPROVED_TOLERANCE'
    'die_pad_089_ng.jpg'  = 'UNEXPLAINED_EXTRA_STRUCTURE_TOP'
    'die_pad_098_ng.jpg'  = 'OCCLUSION_RIGHT_PAD_NO_APPROVED_TOLERANCE'
    'die_pad_109_ng.jpg'  = 'OCCLUSION_TOP_LEFT_CONTEXT_NO_APPROVED_TOLERANCE'
    'die_pad_124_ng.jpg'  = 'UNEXPLAINED_EXTRA_STRUCTURE_UPPER_RIGHT'
    'die_pad_161_ng.jpg'  = 'UNEXPLAINED_EXTRA_STRUCTURE_TOP'
    'die_pad_162_ng.jpg'  = 'EXTRA_DARK_MARKS_LEFT_PAD'
    'die_pad_166_ng.jpg'  = 'UNEXPLAINED_EXTRA_STRUCTURE_LOWER_CENTER'
    'die_pad_171_ng.jpg'  = 'EXTRA_DARK_STRUCTURE_RIGHT_PAD'
    'die_pad_189_ng.jpg'  = 'UNEXPLAINED_EXTRA_STRUCTURE_LEFT'
    'die_pad_198_ng.jpg'  = 'UNEXPLAINED_EXTRA_STRUCTURE_LEFT'
    'die_pad_207_ng.jpg'  = 'EXTRA_STRUCTURE_TOP_AND_LOWER_RIGHT'
    'die_pad_209_ng.jpg'  = 'UNEXPLAINED_EXTRA_STRUCTURE_LOWER_RIGHT'
    'die_pad_219_ng.jpg'  = 'UNEXPLAINED_EXTRA_STRUCTURE_LEFT'
}
$referenceExemplarFileName = 'die_pad_188_ng.jpg'
$referenceExemplarVariant = 'A'
$bWait = @{}
foreach ($key in $aWait.Keys) { $bWait[$key] = $aWait[$key] }
# The B context crop also contains the bright/slanted left context in 163;
# its core is visible, but that added context is not the B template context.
$bWait['die_pad_163_ng.jpg'] = 'B_CONTEXT_MISMATCH_LEFT_EXTRA_STRUCTURE'
# 188 is the operator-named positive reference exemplar. Its red stroke is an
# alignment cue; it is not a request to expand or reject the accepted A crop.
$bWait.Remove($referenceExemplarFileName) | Out-Null

$rows = New-Object System.Collections.Generic.List[object]
foreach ($variant in @('A', 'B')) {
    foreach ($cmp in $comparison) {
        $fileName = [string]$cmp.FileName
        $reason = if ($variant -eq 'A') { $aWait[$fileName] } else { $bWait[$fileName] }
        if ($fileName -eq $referenceExemplarFileName) {
            $state = 'PASS'
            $reason = if ($variant -eq $referenceExemplarVariant) {
                'OPERATOR_ACCEPTED_REFERENCE_EXEMPLAR'
            } else {
                'REFERENCE_EXEMPLAR_CORE_VISIBLE_ALTERNATIVE'
            }
        } elseif ([string]::IsNullOrWhiteSpace($reason)) {
            $state = 'PASS'
            $reason = 'STABLE_CORE_AND_EXEMPLAR_RELATIVE_CONTEXT_VISIBLE'
        } else {
            $state = 'WAIT'
        }
        $stem = [System.IO.Path]::GetFileNameWithoutExtension($fileName)
        $runFolder = "{0}_{1}" -f [string]$cmp.Expected, $stem
        $evidenceFolder = Join-Path $ValidationRoot ("{0}\evidence\runs\{1}" -f $variant, $runFolder)
        $generatedRow = $generated | Where-Object { $_.variant -eq $variant -and $_.fileName -eq $fileName } | Select-Object -First 1
        if ($null -eq $generatedRow) { throw "Generated visual row missing: $variant/$fileName" }
        $runtimeOverlay = Join-Path $evidenceFolder 'runtime_result.png'
        $matchingOverlay = Join-Path $evidenceFolder '01_matching_overlay.png'
        foreach ($path in @($generatedRow.panelPath, $generatedRow.patchPath, $generatedRow.blendPath, $runtimeOverlay, $matchingOverlay)) {
            if (-not (Test-Path -LiteralPath $path)) { throw "Evidence file missing: $path" }
        }
        $templatePath = Join-Path $ValidationRoot ("{0}\template.png" -f $variant)
        $actualSourceHash = Get-Sha256 $cmp.ImagePath
        $actualTemplateHash = Get-Sha256 $templatePath
        if ($actualSourceHash -ne $sourceHashes[[string]$cmp.ImagePath]) { throw "Source hash mismatch: $($cmp.ImagePath)" }
        try {
            $recordIndex = [int]([double]$generatedRow.index)
            $recordCenterX = [int]([double]$generatedRow.centerX)
            $recordCenterY = [int]([double]$generatedRow.centerY)
            $recordScale = [double]$generatedRow.scale
            $recordAngle = [double]$generatedRow.angle
            $recordScore = [double]$generatedRow.scoreMax
            $recordResultCount = [int]([double]$generatedRow.resultCount)
        } catch {
            throw "Numeric conversion failed at $variant/$fileName`: $($_.Exception.Message)"
        }
        try {
        $record = [pscustomobject]@{
            index = $recordIndex
            variant = [string]$variant
            fileName = [string]$fileName
            role = [string]$cmp.Expected
            state = [string]$state
            reason = [string]$reason
            stableFeatures = 'two pads; hole order; L-shaped trace; right vertical trace; exemplar-relative lower/right edge placement'
            sourcePath = [string]$cmp.ImagePath
            sourceSha256 = [string]$actualSourceHash
            templatePath = [string]$templatePath
            templateSha256 = [string]$actualTemplateHash
            panelPath = [string]$generatedRow.panelPath
            sourcePatchPath = [string]$generatedRow.patchPath
            blendPath = [string]$generatedRow.blendPath
            matchingOverlayPath = [string]$matchingOverlay
            runtimeOverlayPath = [string]$runtimeOverlay
            panelSha256 = [string](Get-Sha256 $generatedRow.panelPath)
            sourcePatchSha256 = [string](Get-Sha256 $generatedRow.patchPath)
            blendSha256 = [string](Get-Sha256 $generatedRow.blendPath)
            matchingOverlaySha256 = [string](Get-Sha256 $matchingOverlay)
            runtimeOverlaySha256 = [string](Get-Sha256 $runtimeOverlay)
            reportedCenterX = $recordCenterX
            reportedCenterY = $recordCenterY
            reportedScale = $recordScale
            reportedAngle = $recordAngle
            scoreMaxSecondary = $recordScore
            realCandidateCount = $recordResultCount
            downstreamStatusSecondary = [string]$generatedRow.downstreamStatus
            reviewMethod = 'pose-normalized source patch + template + 50% blend + exact runner overlay; contact-sheet pass followed by high-resolution spot re-open'
            operatorApproval = 'REQUIRED'
        }
        } catch {
            throw "Record build failed at $variant/$fileName`: $($_.Exception.Message)"
        }
        $rows.Add($record) | Out-Null
    }
}

$csvPath = Join-Path $visualRoot 'full-visual-review.csv'
$rows | Export-Csv -LiteralPath $csvPath -NoTypeInformation -Encoding UTF8
$jsonPath = Join-Path $visualRoot 'full-visual-review.json'
$payload = [pscustomobject]@{
    schemaVersion = '1.0'
    reviewDate = $ReviewDate
    status = 'WAIT_FOR_OPERATOR_REVIEW'
    qualification = $false
    corpusRows = $comparison.Count
    candidateRows = $rows.Count
    variantSummary = @(
        [pscustomobject]@{ variant = 'A'; pass = @($rows | Where-Object { $_.variant -eq 'A' -and $_.state -eq 'PASS' }).Count; wait = @($rows | Where-Object { $_.variant -eq 'A' -and $_.state -eq 'WAIT' }).Count; fail = 0; notReviewed = 0 },
        [pscustomobject]@{ variant = 'B'; pass = @($rows | Where-Object { $_.variant -eq 'B' -and $_.state -eq 'PASS' }).Count; wait = @($rows | Where-Object { $_.variant -eq 'B' -and $_.state -eq 'WAIT' }).Count; fail = 0; notReviewed = 0 }
    )
    referenceExemplar = [pscustomobject]@{
        sourceFileName = $referenceExemplarFileName
        variant = $referenceExemplarVariant
        state = 'OPERATOR_SELECTED'
        operatorVisualCue = 'lower/right red alignment stroke'
        extentRule = 'MATCH_EXEMPLAR'
    }
    visualGate = 'PASS only when the same pads, holes, order, spacing, traces, and explicitly required context—or the named positive exemplar relative extent—are visibly present; any unexplained extra structure or occlusion without an approved tolerance remains WAIT regardless of score.'
    evidenceRoot = $visualRoot
    rows = $rows
    operatorApproval = 'REQUIRED'
    nextAction = 'Operator must review the full-resolution panels and explicitly approve a tolerance or reject each WAIT reason before recipe/template qualification.'
}
$payload | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $jsonPath -Encoding UTF8

$markdownPath = Join-Path $visualRoot 'full-visual-review.md'
$aPass = @($rows | Where-Object { $_.variant -eq 'A' -and $_.state -eq 'PASS' }).Count
$aWaitCount = @($rows | Where-Object { $_.variant -eq 'A' -and $_.state -eq 'WAIT' }).Count
$bPass = @($rows | Where-Object { $_.variant -eq 'B' -and $_.state -eq 'PASS' }).Count
$bWaitCount = @($rows | Where-Object { $_.variant -eq 'B' -and $_.state -eq 'WAIT' }).Count
$lines = @(
    '# Die Pad full visual correspondence review',
    '',
    "Review date: $ReviewDate",
    '',
    '## Status',
    '',
    '**WAIT_FOR_OPERATOR_REVIEW** — this is a visual gate review, not recipe qualification.',
    '',
    'The frozen A/B runner output was reviewed from current pose-normalized source patches. Each row retains the exact source image, template, source patch, blend, Matching Step 1 overlay, runtime result overlay, pose, and hashes in `full-visual-review.csv`/`.json`.',
    '',
    '## Counts',
    '',
    "- A: $aPass PASS, $aWaitCount WAIT, 0 FAIL, 0 NOT_REVIEWED (122 corpus rows).",
    "- B: $bPass PASS, $bWaitCount WAIT, 0 FAIL, 0 NOT_REVIEWED (122 corpus rows).",
    '- No row was visually classified as a wrong repeated structure; this does not prove production robustness.',
    '',
    '## Gate',
    '',
    'The two pads, hole order, L-shaped trace, right vertical trace, and exemplar-relative edge/context were compared before considering score or downstream status. The operator-designated 188 sample is PASS for A as the positive reference exemplar; its red stroke is an alignment cue, not an instruction to enlarge the accepted crop. Any occlusion or unexplained extra structure without an operator-approved tolerance remains WAIT even if the numeric score is high or the downstream step reports OK.',
    '',
    'The current B context is visually present on ordinary rows, but its added context and 19 WAIT rows are retained as evidence limits. AUTO_SELECTION does not require an approval click; the unresolved rows simply keep qualification false and prevent B from displacing A under the global policy. A remains the conservative default proposal.',
    '',
    '## WAIT rows',
    '',
    '| Image | A reason | B reason |',
    '|---|---|---|'
)
foreach ($cmp in $comparison) {
    $a = $rows | Where-Object { $_.variant -eq 'A' -and $_.fileName -eq $cmp.FileName } | Select-Object -First 1
    $b = $rows | Where-Object { $_.variant -eq 'B' -and $_.fileName -eq $cmp.FileName } | Select-Object -First 1
    if ($a.state -eq 'WAIT' -or $b.state -eq 'WAIT') {
        $lines += "| $($cmp.FileName) | $($a.state): $($a.reason) | $($b.state): $($b.reason) |"
    }
}
$lines += @(
    '',
    '## Operator decision required',
    '',
    '1. Open the full-resolution panel and runtime overlay for each WAIT row.',
    '2. Decide whether the visible structure is a permitted defect/occlusion or an invalid locator candidate.',
    '3. If permitted, define one recipe-level tolerance that is supported by the current tool contract; do not tune it per sample.',
    '4. Explicitly approve A or B. Until then, keep `visualCorrespondence.state=WAIT` and `operatorApproval=REQUIRED`.'
)
$lines | Set-Content -LiteralPath $markdownPath -Encoding UTF8
Write-Output "Wrote $($rows.Count) rows to $csvPath and $jsonPath"
