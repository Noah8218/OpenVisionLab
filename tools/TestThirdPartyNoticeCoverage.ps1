param(
    [string]$NoticePath = "",
    [string]$OutputPath = ""
)

$ErrorActionPreference = "Stop"

$repoRoot = [System.IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$manifestPath = Join-Path $repoRoot "docs\contracts\openvisionlab\OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json"
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
if (-not (Test-Path -LiteralPath $noticeFullPath -PathType Leaf)) {
    $failures.Add("NOTICE file is missing: $noticeFullPath") | Out-Null
}

$manifest = $null
$noticeText = ""
if ($failures.Count -eq 0) {
    try {
        $manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
    }
    catch {
        $failures.Add("External binary manifest is not valid JSON: $manifestPath") | Out-Null
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
