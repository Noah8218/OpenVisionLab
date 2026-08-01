param(
    [string]$InputHtml = "docs\learn\OPENVISIONLAB_TUTORIAL.html",
    [string]$OutputHtml = "docs\learn\OPENVISIONLAB_TUTORIAL_PORTABLE.html"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$inputPath = if ([System.IO.Path]::IsPathRooted($InputHtml)) { $InputHtml } else { Join-Path $repoRoot $InputHtml }
$outputPath = if ([System.IO.Path]::IsPathRooted($OutputHtml)) { $OutputHtml } else { Join-Path $repoRoot $OutputHtml }

if (!(Test-Path -LiteralPath $inputPath)) {
    throw "Input HTML was not found: $inputPath"
}

$inputDirectory = Split-Path -Parent $inputPath
$html = Get-Content -LiteralPath $inputPath -Raw -Encoding UTF8
$missing = New-Object System.Collections.Generic.List[string]
$embeddedCount = 0

function Get-MimeType {
    param([string]$Path)

    switch ([System.IO.Path]::GetExtension($Path).ToLowerInvariant()) {
        ".png"  { return "image/png" }
        ".jpg"  { return "image/jpeg" }
        ".jpeg" { return "image/jpeg" }
        ".gif"  { return "image/gif" }
        ".bmp"  { return "image/bmp" }
        ".webp" { return "image/webp" }
        ".svg"  { return "image/svg+xml" }
        default { return "application/octet-stream" }
    }
}

$pattern = '(<img\b[^>]*?\bsrc\s*=\s*)(["''])(.*?)\2'
$portableHtml = [System.Text.RegularExpressions.Regex]::Replace(
    $html,
    $pattern,
    {
        param($match)

        $prefix = $match.Groups[1].Value
        $quote = $match.Groups[2].Value
        $src = $match.Groups[3].Value

        if ($src.StartsWith("data:", [System.StringComparison]::OrdinalIgnoreCase) -or
            $src.StartsWith("http://", [System.StringComparison]::OrdinalIgnoreCase) -or
            $src.StartsWith("https://", [System.StringComparison]::OrdinalIgnoreCase)) {
            return $match.Value
        }

        $imagePath = if ([System.IO.Path]::IsPathRooted($src)) { $src } else { Join-Path $inputDirectory $src }
        if (!(Test-Path -LiteralPath $imagePath)) {
            $missing.Add($src)
            return $match.Value
        }

        $mimeType = Get-MimeType $imagePath
        $base64 = [Convert]::ToBase64String([System.IO.File]::ReadAllBytes($imagePath))
        $script:embeddedCount++
        return "$prefix$quote" + "data:$mimeType;base64,$base64" + "$quote"
    })

if ($missing.Count -gt 0) {
    throw "Some tutorial images were not found: $($missing -join ', ')"
}

$notice = @"
<!--
  Portable OpenVisionLab Tutorial.
  Images are embedded as data URI so this single HTML file can be copied and opened anywhere.
  Source document: OPENVISIONLAB_TUTORIAL.html
-->
"@

$portableHtml = $portableHtml -replace '<!doctype html>', "<!doctype html>`r`n$notice"

$outputDirectory = Split-Path -Parent $outputPath
if (!(Test-Path -LiteralPath $outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory | Out-Null
}

$portableHtml = $portableHtml.TrimEnd("`r", "`n") + "`r`n"
[System.IO.File]::WriteAllText(
    $outputPath,
    $portableHtml,
    [System.Text.UTF8Encoding]::new($false))

$relativeOutput = Resolve-Path -LiteralPath $outputPath
Write-Host "Portable tutorial generated: $relativeOutput"
Write-Host "Embedded images: $embeddedCount"
