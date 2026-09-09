param(
    [Parameter(Mandatory = $true)] [string] $ValidationRoot,
    [Parameter(Mandatory = $true)] [string] $BaselineVisualRoot,
    [Parameter(Mandatory = $true)] [string] $OutputRoot,
    [Parameter(Mandatory = $false)] [double] $MinimumScorePercent = 90,
    [Parameter(Mandatory = $false)] [string] $ReferenceExemplarFileName = 'die_pad_188_ng.jpg',
    [Parameter(Mandatory = $false)] [ValidateSet('A', 'B')] [string] $ReferenceExemplarVariant = 'A'
)

$ErrorActionPreference = 'Stop'

function Get-Sha256([string] $path) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "File missing: $path" }
    return (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToUpperInvariant()
}

function Get-XmlParameters([string] $path) {
    [xml]$xml = Get-Content -Raw -LiteralPath $path
    $map = [ordered]@{}
    foreach ($parameter in @($xml.VisionPipeline.Steps.Step[0].Parameters.Parameter)) {
        $key = [string]$parameter.Key
        if (-not [string]::IsNullOrWhiteSpace($key)) { $map[$key] = [string]$parameter.Value }
    }
    return $map
}

function Get-VariantMetric([string] $variant, [string] $root, [double] $scoreFloor,
                           [object[]] $baselineRows, [object[]] $spotRows,
                           [string] $referenceExemplarFileName) {
    $evidenceCsv = Join-Path $root "$variant\evidence\evidence_rows.csv"
    $templatePath = Join-Path $root "$variant\template.png"
    $pipelinePath = Join-Path $root "$variant\pipeline.xml"
    foreach ($path in @($evidenceCsv, $templatePath, $pipelinePath)) {
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "Candidate file missing: $path" }
    }
    $rows = @(Import-Csv -LiteralPath $evidenceCsv)
    $step1 = @($rows | Where-Object { [string]$_.StepIndex -eq '1' })
    $step5 = @($rows | Where-Object { [string]$_.StepIndex -eq '5' })
    $imageNames = @($step1 | ForEach-Object { [System.IO.Path]::GetFileName([string]$_.ImagePath) } | Sort-Object -Unique)
    $accepted = @($step1 | Where-Object { [string]$_.StepStatus -eq 'OK' })
    $noResult = @($step1 | Where-Object { [string]$_.StepStatus -ne 'OK' })
    $scores = @($accepted | ForEach-Object {
        $value = 0.0
        if ([double]::TryParse([string]$_.ScoreMax, [Globalization.NumberStyles]::Float, [Globalization.CultureInfo]::InvariantCulture, [ref]$value)) { $value }
    })
    $scoreMin = if ($scores.Count -gt 0) { ($scores | Measure-Object -Minimum).Minimum } else { 0 }
    $scoreMax = if ($scores.Count -gt 0) { ($scores | Measure-Object -Maximum).Maximum } else { 0 }
    $scoreAvg = if ($scores.Count -gt 0) { ($scores | Measure-Object -Average).Average } else { 0 }
    $scoreGatePass = ($accepted.Count -gt 0 -and $scores.Count -eq $accepted.Count -and $scoreMin -ge $scoreFloor)
    $missingEvidence = New-Object System.Collections.Generic.List[string]
    foreach ($row in $step1) {
        foreach ($path in @([string]$row.ResultImagePath, [string]$row.StepOverlayPath)) {
            if ([string]::IsNullOrWhiteSpace($path) -or -not (Test-Path -LiteralPath $path -PathType Leaf)) {
                $missingEvidence.Add("$([System.IO.Path]::GetFileName([string]$row.ImagePath)):$path")
            }
        }
    }
    $visual = @($baselineRows | Where-Object { [string]$_.variant -eq $variant })
    $visualPass = @($visual | Where-Object { [string]$_.state -eq 'PASS' }).Count
    $visualWait = @($visual | Where-Object { [string]$_.state -eq 'WAIT' }).Count
    $visualFail = @($visual | Where-Object { [string]$_.state -eq 'FAIL' }).Count
    $visualNotReviewed = @($visual | Where-Object { [string]$_.state -eq 'NOT_REVIEWED' }).Count
    $referenceRows = @($visual | Where-Object { [string]$_.fileName -eq $referenceExemplarFileName })
    $referenceExemplarPass = ($referenceRows.Count -eq 1 -and [string]$referenceRows[0].state -eq 'PASS')
    $templateHash = Get-Sha256 $templatePath
    $sourceHashByName = @{}
    foreach ($row in $step1) {
        $sourceHashByName[[System.IO.Path]::GetFileName([string]$row.ImagePath)] = [string]$row.SourceSha256
    }
    $visualIdentityMismatches = New-Object System.Collections.Generic.List[string]
    foreach ($visualRow in $visual) {
        $fileName = [string]$visualRow.fileName
        if ([string]$visualRow.templateSha256 -ne $templateHash -or
            -not $sourceHashByName.ContainsKey($fileName) -or
            [string]$visualRow.sourceSha256 -ne [string]$sourceHashByName[$fileName]) {
            $visualIdentityMismatches.Add($fileName)
        }
    }
    $spots = @($spotRows | Where-Object { [string]$_.variant -eq $variant })
    $spotEvidenceComplete = ($spots.Count -ge 2 -and @($spots | Where-Object {
        (Test-Path -LiteralPath ([string]$_.panelPath) -PathType Leaf) -and
        (Test-Path -LiteralPath ([string]$_.matchingOverlayPath) -PathType Leaf)
    }).Count -eq $spots.Count)
    $pipelineHash = Get-Sha256 $pipelinePath
    $xmlParams = Get-XmlParameters $pipelinePath
    $templateReferenceMismatches = New-Object System.Collections.Generic.List[string]
    foreach ($key in @('TemplatePath', 'PATTERN_PATH')) {
        $referencePath = [string]$xmlParams[$key]
        if ([string]::IsNullOrWhiteSpace($referencePath) -or -not (Test-Path -LiteralPath $referencePath -PathType Leaf)) {
            $templateReferenceMismatches.Add("${key}:$referencePath")
        } elseif ((Get-Sha256 $referencePath) -ne $templateHash) {
            $templateReferenceMismatches.Add("${key}:$referencePath (hash mismatch)")
        }
    }
    $pipelinePass = @($step5 | Where-Object { [string]$_.StepStatus -eq 'OK' -and [string]$_.PipelineSuccess -eq 'true' } | ForEach-Object { [string]$_.ImagePath } | Sort-Object -Unique).Count
    $acceptedNames = @($accepted | ForEach-Object { [string]$_.ImagePath } | Sort-Object -Unique)
    [pscustomobject]@{
        variant = $variant
        candidateId = "die-pad-$($variant.ToLowerInvariant())-score90-angle05-global"
        imageCount = $imageNames.Count
        acceptedCount = $acceptedNames.Count
        noResultCount = $noResult.Count
        coveragePercent = if ($imageNames.Count -gt 0) { [math]::Round(100.0 * $acceptedNames.Count / $imageNames.Count, 4) } else { 0 }
        pipelinePassCount = $pipelinePass
        scoreMin = [math]::Round([double]$scoreMin, 6)
        scoreMax = [math]::Round([double]$scoreMax, 6)
        scoreAverage = [math]::Round([double]$scoreAvg, 6)
        scoreFloorPercent = $scoreFloor
        scoreGatePass = $scoreGatePass
        evidenceComplete = ($missingEvidence.Count -eq 0 -and $step1.Count -eq $imageNames.Count)
        missingEvidenceCount = $missingEvidence.Count
        missingEvidenceExamples = @($missingEvidence | Select-Object -First 5)
        baselineVisualPass = $visualPass
        baselineVisualWait = $visualWait
        baselineVisualFail = $visualFail
        baselineVisualNotReviewed = $visualNotReviewed
        referenceExemplarFileName = $referenceExemplarFileName
        referenceExemplarState = if ($referenceRows.Count -eq 1) { [string]$referenceRows[0].state } else { 'NOT_REVIEWED' }
        referenceExemplarReason = if ($referenceRows.Count -eq 1) { [string]$referenceRows[0].reason } else { 'REFERENCE_EXEMPLAR_ROW_MISSING' }
        referenceExemplarPass = $referenceExemplarPass
        baselineVisualIdentity = ($visual.Count -eq $imageNames.Count -and $visualIdentityMismatches.Count -eq 0)
        visualIdentityMismatchCount = $visualIdentityMismatches.Count
        visualIdentityMismatchExamples = @($visualIdentityMismatches | Select-Object -First 5)
        currentSpotCount = $spots.Count
        currentSpotEvidenceComplete = $spotEvidenceComplete
        templateReferenceHashPass = ($templateReferenceMismatches.Count -eq 0)
        templateReferenceMismatches = @($templateReferenceMismatches | Select-Object -First 5)
        templatePath = $templatePath
        templateSha256 = $templateHash
        pipelinePath = $pipelinePath
        pipelineSha256 = $pipelineHash
        globalParameters = $xmlParams
    }
}

$imageListPath = Join-Path $ValidationRoot 'image-list.txt'
$baselineCsv = Join-Path $BaselineVisualRoot 'full-visual-review.csv'
$spotManifestPath = Join-Path $ValidationRoot 'visual-spots\manifest.json'
foreach ($path in @($imageListPath, $baselineCsv, $spotManifestPath)) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "Input evidence missing: $path" }
}
$imagePaths = @(Get-Content -LiteralPath $imageListPath | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
$baselineRows = @(Import-Csv -LiteralPath $baselineCsv)
$spotPayload = Get-Content -Raw -LiteralPath $spotManifestPath | ConvertFrom-Json
$spotRows = @($spotPayload.rows)
$metrics = @(
    (Get-VariantMetric -variant 'A' -root $ValidationRoot -scoreFloor $MinimumScorePercent -baselineRows $baselineRows -spotRows $spotRows -referenceExemplarFileName $ReferenceExemplarFileName),
    (Get-VariantMetric -variant 'B' -root $ValidationRoot -scoreFloor $MinimumScorePercent -baselineRows $baselineRows -spotRows $spotRows -referenceExemplarFileName $ReferenceExemplarFileName)
)

if ($metrics[0].imageCount -ne $imagePaths.Count -or $metrics[1].imageCount -ne $imagePaths.Count) {
    throw "Corpus mismatch: image-list=$($imagePaths.Count), A=$($metrics[0].imageCount), B=$($metrics[1].imageCount)"
}
foreach ($metric in $metrics) {
    $metric | Add-Member -NotePropertyName sourceTemplateIdentityPass -NotePropertyValue ($metric.baselineVisualIdentity -and ($metric.missingEvidenceCount -eq 0)) -Force
    # A is the stable-core default. B must prove a strict visual-wait reduction
    # before it can displace A; higher score/coverage is not sufficient.
    $metric | Add-Member -NotePropertyName hardGatePass -NotePropertyValue ($metric.scoreGatePass -and $metric.evidenceComplete -and $metric.sourceTemplateIdentityPass -and $metric.currentSpotEvidenceComplete -and $metric.templateReferenceHashPass -and $metric.referenceExemplarPass) -Force
}
$a = $metrics | Where-Object variant -eq 'A'
$b = $metrics | Where-Object variant -eq 'B'
$bContextAdvantage = ($b.baselineVisualWait -lt $a.baselineVisualWait -and $b.baselineVisualFail -le $a.baselineVisualFail)
$b | Add-Member -NotePropertyName hardGatePass -NotePropertyValue ($b.hardGatePass -and $bContextAdvantage) -Force
$a | Add-Member -NotePropertyName hardGatePass -NotePropertyValue $a.hardGatePass -Force

$selected = $null
$status = 'AUTO_REJECTED'
$selectionReason = ''
if ($a.hardGatePass -and -not $b.hardGatePass) {
    $selected = $a
    $status = 'AUTO_SELECTED'
    $selectionReason = "A selected as the stable-core global candidate after the operator-named reference exemplar '$ReferenceExemplarFileName' passed its visual gate. B did not demonstrate a strict reduction in baseline visual WAIT rows (A=$($a.baselineVisualWait), B=$($b.baselineVisualWait)); score/coverage cannot override that gate."
} elseif ($b.hardGatePass -and -not $a.hardGatePass) {
    $selected = $b
    $status = 'AUTO_SELECTED'
    $selectionReason = 'B selected because its repeatable physical context passed every hard gate and strictly reduced visual WAIT evidence.'
} elseif ($a.hardGatePass -and $b.hardGatePass) {
    $ordered = @($a, $b) | Sort-Object @{Expression = { $_.baselineVisualWait }; Ascending = $true}, @{Expression = { $_.coveragePercent }; Descending = $true}, @{Expression = { $_.pipelinePassCount }; Descending = $true}, @{Expression = { $_.scoreAverage }; Descending = $true}
    $selected = $ordered[0]
    $status = 'AUTO_SELECTED'
    $selectionReason = "Both candidates passed; lexicographic rank selected $($selected.variant) by visual WAIT, coverage, downstream pass, then score average."
} else {
    if (-not $a.currentSpotEvidenceComplete -or -not $b.currentSpotEvidenceComplete) {
        $status = 'AUTO_REJECTED_VISUAL_MISMATCH'
        $selectionReason = 'Current correspondence spot evidence is incomplete for at least one candidate; no score-only fallback is allowed.'
    } elseif (-not $bContextAdvantage -and $a.hardGatePass -eq $false) {
        $status = 'AUTO_REJECTED_AMBIGUOUS'
        $selectionReason = 'No candidate passed the global gates and B did not demonstrate a strict visual-wait advantage over A.'
    } else {
        $selectionReason = 'No candidate passed every automatic hard gate.'
    }
}

New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null
$selectedObject = if ($null -eq $selected) { $null } else {
    [pscustomobject]@{
        candidateId = $selected.candidateId
        variant = $selected.variant
        templatePath = $selected.templatePath
        templateSha256 = $selected.templateSha256
        pipelinePath = $selected.pipelinePath
        pipelineSha256 = $selected.pipelineSha256
        globalParameters = $selected.globalParameters
    }
}
$manifest = [ordered]@{
    schemaVersion = '1.0'
    mode = 'AUTO_SELECTION'
    status = $status
    qualification = $false
    operatorApproval = 'NOT_REQUIRED_FOR_AUTO_SELECTION'
    createdUtc = [DateTime]::UtcNow.ToString('o')
    validationRoot = $ValidationRoot
    imageListPath = $imageListPath
    imageListSha256 = Get-Sha256 $imageListPath
    baselineVisualRoot = $BaselineVisualRoot
    baselineVisualCsv = $baselineCsv
    baselineVisualCsvSha256 = Get-Sha256 $baselineCsv
    spotManifestPath = $spotManifestPath
    spotManifestSha256 = Get-Sha256 $spotManifestPath
    scoreFloorPercent = $MinimumScorePercent
    referenceExemplar = [ordered]@{
        sourceFileName = $ReferenceExemplarFileName
        variant = $ReferenceExemplarVariant
        state = 'OPERATOR_SELECTED'
        operatorVisualCue = 'lower/right red alignment stroke'
        extentRule = 'MATCH_EXEMPLAR'
    }
    candidateSet = $metrics
    selected = $selectedObject
    selectionReason = $selectionReason
    visualEvidenceScope = "current angle05 score90 representative correspondence spots plus same-source/template-identity baseline full visual ledger; reference exemplar '$ReferenceExemplarFileName' is explicitly checked; not full current-run corpus reclassification"
    nextAction = 'Keep the selected global candidate provisional until current full-corpus visual and held-out evidence are complete; explicit Preview/Run/import/release remains separate.'
}
$manifestPath = Join-Path $OutputRoot 'auto-selection-manifest.json'
$manifest | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $manifestPath -Encoding UTF8

$summary = @(
    '# Die Pad automatic global template selection',
    '',
    "Status: **$status**",
    "Operator approval: **NOT_REQUIRED_FOR_AUTO_SELECTION**",
    "Qualification: **false** (selection and qualification are separate)",
    '',
    '## Selection reason',
    '',
    $selectionReason,
    '',
    '## Candidate metrics',
    '',
    '| Candidate | Accepted / corpus | Coverage | Pipeline pass | Score min / avg | Baseline visual PASS / WAIT | Exemplar | Hard gates |',
    '|---|---:|---:|---:|---:|---:|---|---|'
)
foreach ($metric in $metrics) {
    $summary += "| $($metric.variant) | $($metric.acceptedCount) / $($metric.imageCount) | $($metric.coveragePercent)% | $($metric.pipelinePassCount) | $($metric.scoreMin) / $($metric.scoreAverage) | $($metric.baselineVisualPass) / $($metric.baselineVisualWait) | $($metric.referenceExemplarState) | $($metric.hardGatePass) |"
}
$summary += @(
    '',
    'The score floor is a rejection floor. The selector never changes a template or parameter for one image and never treats a high score or green rectangle as visual proof.',
    '',
    "Manifest: $manifestPath"
)
$summary | Set-Content -LiteralPath (Join-Path $OutputRoot 'auto-selection-summary.md') -Encoding UTF8
Write-Output "Wrote $manifestPath"
$selectedVariant = if ($null -eq $selected) { 'NONE' } else { [string]$selected.variant }
Write-Output "Status=$status Selected=$selectedVariant"
