param(
    [string]$NoticePath = "",
    [string]$OutputPath = ""
)

$ErrorActionPreference = "Stop"

$repoRoot = [System.IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$manifestPath = Join-Path $repoRoot "docs\contracts\openvisionlab\OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json"
$sdkManifestPath = Join-Path $repoRoot "dll\OpenVisionLab-Vision-SDK\sdk-manifest.json"
$defaultNoticePath = Join-Path $repoRoot "NOTICE"
$noticeFullPath = if ([string]::IsNullOrWhiteSpace($NoticePath)) {
    $defaultNoticePath
}
elseif ([System.IO.Path]::IsPathRooted($NoticePath)) {
    [System.IO.Path]::GetFullPath($NoticePath)
}
else {
    [System.IO.Path]::GetFullPath((Join-Path $repoRoot $NoticePath))
}

$lines = New-Object System.Collections.Generic.List[string]
$failures = New-Object System.Collections.Generic.List[string]
$lines.Add("OpenVisionLab Retained Dependency NOTICE Coverage") | Out-Null
$lines.Add("Manifest: $manifestPath") | Out-Null
$lines.Add("NOTICE: $noticeFullPath") | Out-Null
$lines.Add("") | Out-Null

if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf)) {
    $failures.Add("External binary manifest is missing: $manifestPath") | Out-Null
}
if (-not (Test-Path -LiteralPath $sdkManifestPath -PathType Leaf)) {
    $failures.Add("Vision SDK provenance manifest is missing: $sdkManifestPath") | Out-Null
}
if (-not (Test-Path -LiteralPath $noticeFullPath -PathType Leaf)) {
    $failures.Add("NOTICE file is missing: $noticeFullPath") | Out-Null
}

$manifest = $null
$sdkManifest = $null
$noticeText = ""
if ($failures.Count -eq 0) {
    try {
        $manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
    }
    catch {
        $failures.Add("External binary manifest is not valid JSON: $manifestPath") | Out-Null
    }

    try {
        $sdkManifest = Get-Content -LiteralPath $sdkManifestPath -Raw | ConvertFrom-Json
    }
    catch {
        $failures.Add("Vision SDK provenance manifest is not valid JSON: $sdkManifestPath") | Out-Null
    }

    if ($null -ne $manifest) {
        if ($manifest.schemaVersion -ne 1) {
            $failures.Add("Unsupported external binary manifest schema: $($manifest.schemaVersion)") | Out-Null
        }
        $noticeText = Get-Content -LiteralPath $noticeFullPath -Raw
        $allowedEntries = @($manifest.entries | Where-Object {
            $_.repositoryState -eq "present" -and [string]$_.releasePolicy -like "allow*"
        })
        if ($allowedEntries.Count -eq 0) {
            $failures.Add("No present allowlisted entries were found in the external binary manifest.") | Out-Null
        }

        foreach ($entry in $allowedEntries) {
            $entryPath = [string]$entry.path
            $marker = [string]$entry.noticeMarker
            if ([string]::IsNullOrWhiteSpace($marker)) {
                $failures.Add("NOTICE marker is missing from manifest entry: $entryPath") | Out-Null
                $lines.Add("MISSING_MARKER | $entryPath") | Out-Null
                continue
            }
            if ($noticeText.IndexOf($marker, [System.StringComparison]::OrdinalIgnoreCase) -lt 0) {
                $failures.Add("NOTICE marker is absent: $marker ($entryPath)") | Out-Null
                $lines.Add("MISSING_NOTICE | $marker | $entryPath") | Out-Null
            }
            else {
                $lines.Add("COVERED | $marker | $entryPath | $($entry.releasePolicy)") | Out-Null
            }
        }
    }

    if ($null -ne $sdkManifest) {
        $noticeText = Get-Content -LiteralPath $noticeFullPath -Raw
        $sdkSection = [regex]::Match(
            $noticeText,
            '(?s)OpenVisionLab Vision SDK(?<body>.*?)(?:\r?\nSharpGL\r?\n)')
        if (-not $sdkSection.Success) {
            $failures.Add("NOTICE does not contain a bounded OpenVisionLab Vision SDK section.") | Out-Null
        }
        else {
            $sectionText = $sdkSection.Groups['body'].Value
            $expectedVersion = [string]$sdkManifest.sdk.version
            $expectedCommit = [string]$sdkManifest.sdk.commit
            $noticeCommits = @([regex]::Matches($sectionText, '\b[0-9a-fA-F]{40}\b') | ForEach-Object { $_.Value })

            if ([string]::IsNullOrWhiteSpace($expectedVersion) -or
                $sectionText.IndexOf($expectedVersion, [System.StringComparison]::Ordinal) -lt 0) {
                $failures.Add("NOTICE Vision SDK version does not match sdk-manifest.json: $expectedVersion") | Out-Null
            }
            if ([string]::IsNullOrWhiteSpace($expectedCommit) -or
                $noticeCommits.Count -ne 1 -or
                -not $noticeCommits[0].Equals($expectedCommit, [System.StringComparison]::OrdinalIgnoreCase)) {
                $actualCommits = if ($noticeCommits.Count -eq 0) { "<none>" } else { $noticeCommits -join ', ' }
                $failures.Add("NOTICE Vision SDK commit does not match sdk-manifest.json. Expected $expectedCommit; found $actualCommits") | Out-Null
            }
            else {
                $lines.Add("PROVENANCE | OpenVisionLab Vision SDK $expectedVersion | $expectedCommit") | Out-Null
            }
        }
    }
}

$lines.Add("") | Out-Null
if ($failures.Count -eq 0) {
    $lines.Add("NOTICE coverage passed.") | Out-Null
}
else {
    $lines.Add("NOTICE coverage failed: $($failures.Count)") | Out-Null
    foreach ($failure in $failures) {
        $lines.Add("- $failure") | Out-Null
    }
}

if (-not [string]::IsNullOrWhiteSpace($OutputPath)) {
    $outputFullPath = if ([System.IO.Path]::IsPathRooted($OutputPath)) {
        [System.IO.Path]::GetFullPath($OutputPath)
    }
    else {
        [System.IO.Path]::GetFullPath((Join-Path $repoRoot $OutputPath))
    }
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $outputFullPath) | Out-Null
    $lines | Set-Content -LiteralPath $outputFullPath -Encoding UTF8
}

$lines | ForEach-Object { Write-Host $_ }
if ($failures.Count -gt 0) {
    throw "Retained dependency NOTICE coverage failed. Add a noticeMarker and matching NOTICE entry for every present allowlisted binary."
}
