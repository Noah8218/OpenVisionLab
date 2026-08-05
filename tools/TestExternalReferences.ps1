param(
    [string]$Configuration = "Debug",
    [string]$OutputPath = ""
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$dllRoot = Join-Path $repoRoot "dll"
$visionSdkDllRoot = Join-Path $dllRoot "OpenVisionLab-Vision-SDK"
$manifestPath = Join-Path $visionSdkDllRoot "sdk-manifest.json"
$openCvSharpDllRoot = Join-Path $dllRoot "OpenCVSharp"
$wpgPath = Join-Path $dllRoot "System.Windows.Controls.WpfPropertyGrid.dll"
$legacyDllRoot = Join-Path $dllRoot "Library-Noah"

$lines = New-Object System.Collections.Generic.List[string]
$failures = New-Object System.Collections.Generic.List[string]
$lines.Add("OpenVisionLab Vendored DLL Check") | Out-Null
$lines.Add("Configuration: $Configuration") | Out-Null
$lines.Add("Vision SDK DLL root: $([System.IO.Path]::GetFullPath($visionSdkDllRoot))") | Out-Null
$lines.Add("OpenCVSharp DLL root: $([System.IO.Path]::GetFullPath($openCvSharpDllRoot))") | Out-Null
$lines.Add("") | Out-Null

if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf)) {
    $failures.Add("SDK manifest is missing: $manifestPath") | Out-Null
}
else {
    $manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
    if ($manifest.schemaVersion -ne 1) {
        $failures.Add("Unsupported SDK manifest schema: $($manifest.schemaVersion)") | Out-Null
    }
    if ($manifest.sdk.version -ne "3.0.0") {
        $failures.Add("Unexpected SDK version: $($manifest.sdk.version)") | Out-Null
    }
    if ([string]::IsNullOrWhiteSpace($manifest.sdk.commit)) {
        $failures.Add("SDK source commit is missing from the manifest.") | Out-Null
    }

    foreach ($entry in $manifest.files) {
        $path = [System.IO.Path]::GetFullPath((Join-Path $visionSdkDllRoot $entry.path))
        $rootPrefix = $visionSdkDllRoot.TrimEnd('\', '/') + [System.IO.Path]::DirectorySeparatorChar
        if (-not $path.StartsWith($rootPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
            $failures.Add("SDK manifest path escapes its root: $($entry.path)") | Out-Null
            continue
        }
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
            $failures.Add("SDK DLL is missing: $path") | Out-Null
            $lines.Add("MISSING | $($entry.path) | $path") | Out-Null
            continue
        }

        $file = Get-Item -LiteralPath $path
        $actualHash = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash
        if ($file.Length -ne [long]$entry.length -or $actualHash -ne $entry.sha256) {
            $failures.Add("SDK DLL hash/length mismatch: $($entry.path)") | Out-Null
            $lines.Add("MISMATCH | $($entry.path) | $path") | Out-Null
            continue
        }
        $lines.Add("OK | $($entry.path) | $path") | Out-Null
    }

    $nativeEntry = $manifest.sharedNativeRuntime
    $nativePath = [System.IO.Path]::GetFullPath((Join-Path $visionSdkDllRoot $nativeEntry.path))
    if (-not (Test-Path -LiteralPath $nativePath -PathType Leaf)) {
        $failures.Add("Shared OpenCvSharp native runtime is missing: $nativePath") | Out-Null
        $lines.Add("MISSING | OpenCvSharpExtern.dll | $nativePath") | Out-Null
    }
    else {
        $nativeFile = Get-Item -LiteralPath $nativePath
        $nativeHash = (Get-FileHash -LiteralPath $nativePath -Algorithm SHA256).Hash
        if ($nativeFile.Length -ne [long]$nativeEntry.length -or $nativeHash -ne $nativeEntry.sha256) {
            $failures.Add("Shared OpenCvSharp native runtime hash/length mismatch: $nativePath") | Out-Null
            $lines.Add("MISMATCH | OpenCvSharpExtern.dll | $nativePath") | Out-Null
        }
        else {
            $lines.Add("OK | OpenCvSharpExtern.dll | $nativePath") | Out-Null
        }
    }
}

if (-not (Test-Path -LiteralPath $wpgPath -PathType Leaf)) {
    $failures.Add("WPF PropertyGrid runtime is missing: $wpgPath") | Out-Null
    $lines.Add("MISSING | System.Windows.Controls.WpfPropertyGrid.dll | $wpgPath") | Out-Null
}
else {
    $lines.Add("OK | System.Windows.Controls.WpfPropertyGrid.dll | $wpgPath") | Out-Null
}

if (Test-Path -LiteralPath $legacyDllRoot) {
    $failures.Add("Legacy Library-Noah DLL root must be removed: $legacyDllRoot") | Out-Null
    $lines.Add("FORBIDDEN | Legacy Library-Noah DLL root | $legacyDllRoot") | Out-Null
}

$legacyNames = @("Lib.Common.dll", "Lib.OpenCV.dll", "Lib.OpenCV.Blob.dll", "OpenCvSharp.Extensions.dll")
foreach ($legacyName in $legacyNames) {
    $legacyMatches = @(Get-ChildItem -LiteralPath $dllRoot -Recurse -File -Filter $legacyName -ErrorAction SilentlyContinue)
    foreach ($legacyMatch in $legacyMatches) {
        $failures.Add("Legacy runtime must be removed: $($legacyMatch.FullName)") | Out-Null
        $lines.Add("FORBIDDEN | $legacyName | $($legacyMatch.FullName)") | Out-Null
    }
}

$lines.Add("") | Out-Null
if ($failures.Count -eq 0) {
    $lines.Add("Vendored DLL check passed.") | Out-Null
}
else {
    $lines.Add("Vendored DLL check failed: $($failures.Count)") | Out-Null
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
    throw "Vendored DLL check failed. Restore the manifest-verified OpenVisionLab Vision SDK 3.0 files and WPF PropertyGrid runtime."
}
