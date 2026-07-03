param(
    [string]$LibraryNoahSourceRoot = "",
    [string]$WpgCustomSourceRoot = "",
    [string]$Configuration = "Debug",
    [string]$WpgCustomBuildEnabled = "false",
    [string]$OutputPath = ""
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$dllRoot = Join-Path $repoRoot "dll"
$libraryNoahDllRoot = Join-Path $dllRoot "Library-Noah"
$openCvSharpDllRoot = Join-Path $dllRoot "OpenCVSharp"

$checks = New-Object System.Collections.Generic.List[object]

function Add-Check {
    param(
        [string]$Name,
        [string]$Path,
        [bool]$Required = $true
    )

    $fullPath = if ([System.IO.Path]::IsPathRooted($Path)) {
        [System.IO.Path]::GetFullPath($Path)
    }
    else {
        [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
    }

    $checks.Add([pscustomobject][ordered]@{
        Name = $Name
        Required = $Required
        Exists = Test-Path -LiteralPath $fullPath
        Path = $fullPath
    }) | Out-Null
}

Add-Check "Lib.Common.dll" (Join-Path $libraryNoahDllRoot "Lib.Common.dll")
Add-Check "Lib.OpenCV.dll" (Join-Path $libraryNoahDllRoot "Lib.OpenCV.dll")
Add-Check "Lib.OpenCV.Blob.dll" (Join-Path $libraryNoahDllRoot "Lib.OpenCV.Blob.dll")
Add-Check "OpenCvSharp.dll" (Join-Path $libraryNoahDllRoot "OpenCvSharp.dll")
Add-Check "OpenCvSharp.Blob.dll" (Join-Path $libraryNoahDllRoot "OpenCvSharp.Blob.dll")
Add-Check "OpenCvSharp.Extensions.dll" (Join-Path $libraryNoahDllRoot "OpenCvSharp.Extensions.dll")
Add-Check "OpenCvSharpExtern.dll" (Join-Path $openCvSharpDllRoot "OpenCvSharpExtern.dll")
Add-Check "WPF PropertyGrid runtime" (Join-Path $dllRoot "System.Windows.Controls.WpfPropertyGrid.dll")

$missing = @($checks | Where-Object { $_.Required -and -not $_.Exists })
$lines = New-Object System.Collections.Generic.List[string]
$lines.Add("OpenVisionLab Vendored DLL Check") | Out-Null
$lines.Add("Configuration: $Configuration") | Out-Null
$lines.Add("DLL root: $([System.IO.Path]::GetFullPath($dllRoot))") | Out-Null
$lines.Add("Library-Noah DLL root: $([System.IO.Path]::GetFullPath($libraryNoahDllRoot))") | Out-Null
$lines.Add("OpenCVSharp native DLL root: $([System.IO.Path]::GetFullPath($openCvSharpDllRoot))") | Out-Null
$lines.Add("") | Out-Null

foreach ($check in $checks) {
    $state = if ($check.Exists) { "OK" } else { "MISSING" }
    $lines.Add("$state | $($check.Name) | $($check.Path)") | Out-Null
}

if ($missing.Count -gt 0) {
    $lines.Add("") | Out-Null
    $lines.Add("Missing vendored DLLs: $($missing.Count)") | Out-Null
    foreach ($item in $missing) {
        $lines.Add("- $($item.Name): $($item.Path)") | Out-Null
    }
}
else {
    $lines.Add("") | Out-Null
    $lines.Add("Vendored DLL check passed.") | Out-Null
}

if (-not [string]::IsNullOrWhiteSpace($OutputPath)) {
    $outputFullPath = if ([System.IO.Path]::IsPathRooted($OutputPath)) {
        $OutputPath
    }
    else {
        Join-Path $repoRoot $OutputPath
    }

    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $outputFullPath) | Out-Null
    $lines | Set-Content -LiteralPath $outputFullPath -Encoding UTF8
}

$lines | ForEach-Object { Write-Host $_ }

if ($missing.Count -gt 0) {
    throw "Vendored DLL check failed. Restore dll\Library-Noah, dll\OpenCVSharp, and dll\System.Windows.Controls.WpfPropertyGrid.dll from the repository."
}
