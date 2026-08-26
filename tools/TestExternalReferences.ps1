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
$binaryManifestPath = Join-Path $repoRoot "docs\contracts\openvisionlab\OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json"

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

if (-not (Test-Path -LiteralPath $binaryManifestPath -PathType Leaf)) {
    $failures.Add("External binary manifest is missing: $binaryManifestPath") | Out-Null
}
else {
    try {
        $binaryManifest = Get-Content -LiteralPath $binaryManifestPath -Raw | ConvertFrom-Json
    }
    catch {
        $failures.Add("External binary manifest is not valid JSON: $binaryManifestPath") | Out-Null
        $binaryManifest = $null
    }

    if ($null -ne $binaryManifest) {
        if ($binaryManifest.schemaVersion -ne 1) {
            $failures.Add("Unsupported external binary manifest schema: $($binaryManifest.schemaVersion)") | Out-Null
        }

        $manifestEntriesByPath = @{}
        $dllRootFullPath = [System.IO.Path]::GetFullPath($dllRoot)
        $dllRootPrefix = $dllRootFullPath.TrimEnd('\', '/') + [System.IO.Path]::DirectorySeparatorChar
        foreach ($entry in @($binaryManifest.entries)) {
            $manifestPathValue = [string]$entry.path
            $normalizedManifestPath = $manifestPathValue.Replace('/', '\')
            if ([string]::IsNullOrWhiteSpace($manifestPathValue) -or
                -not $normalizedManifestPath.EndsWith('.dll', [System.StringComparison]::OrdinalIgnoreCase)) {
                $failures.Add("External binary manifest entry is not a DLL path: $manifestPathValue") | Out-Null
                continue
            }

            if ($manifestEntriesByPath.ContainsKey($normalizedManifestPath)) {
                $failures.Add("External binary manifest contains a duplicate path: $manifestPathValue") | Out-Null
                continue
            }
            $manifestEntriesByPath[$normalizedManifestPath] = $entry

            $entryFullPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $normalizedManifestPath))
            if (-not $entryFullPath.StartsWith($dllRootPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
                $failures.Add("External binary manifest path escapes dll root: $manifestPathValue") | Out-Null
                continue
            }

            $releasePolicy = [string]$entry.releasePolicy
            if (-not (Test-Path -LiteralPath $entryFullPath -PathType Leaf)) {
                if ($releasePolicy -like 'allow*' -or [string]$entry.classification -eq 'runtime-required') {
                    $failures.Add("Required external binary is missing: $manifestPathValue") | Out-Null
                    $lines.Add("MISSING | $manifestPathValue | $entryFullPath") | Out-Null
                }
                else {
                    $lines.Add("ABSENT | $releasePolicy | $manifestPathValue") | Out-Null
                }
                continue
            }

            $entryFile = Get-Item -LiteralPath $entryFullPath
            $entryHash = (Get-FileHash -LiteralPath $entryFullPath -Algorithm SHA256).Hash
            if ($null -eq $entry.length -or [string]::IsNullOrWhiteSpace([string]$entry.sha256) -or
                $entryFile.Length -ne [long]$entry.length -or
                $entryHash -ne ([string]$entry.sha256).ToUpperInvariant()) {
                $failures.Add("External binary manifest hash/length mismatch: $manifestPathValue") | Out-Null
                $lines.Add("MISMATCH | $manifestPathValue | $entryFullPath") | Out-Null
                continue
            }

            if ($releasePolicy -eq 'forbidden') {
                $failures.Add("Forbidden external binary is present: $entryFullPath") | Out-Null
                $lines.Add("FORBIDDEN | $manifestPathValue | $entryFullPath") | Out-Null
            }
            elseif ($releasePolicy -eq 'blocked' -or $releasePolicy -like 'blocked-*') {
                $lines.Add("BLOCKED | $manifestPathValue | $entryFullPath") | Out-Null
            }
            else {
                $lines.Add("OK | $manifestPathValue | $entryFullPath") | Out-Null
            }
        }

        $physicalDlls = @(Get-ChildItem -LiteralPath $dllRoot -Recurse -File -Filter '*.dll' -ErrorAction SilentlyContinue)
        foreach ($physicalDll in $physicalDlls) {
            $physicalRelativePath = $physicalDll.FullName.Substring($repoRoot.Length + 1).Replace('/', '\')
            if (-not $manifestEntriesByPath.ContainsKey($physicalRelativePath)) {
                $failures.Add("Unallowlisted DLL under dll root: $physicalRelativePath") | Out-Null
                $lines.Add("UNALLOWLISTED | $physicalRelativePath | $($physicalDll.FullName)") | Out-Null
            }
        }

        $trackedDlls = @(& git -C $repoRoot ls-files -- dll | Where-Object { $_ -match '(?i)\.dll$' })
        foreach ($trackedDll in $trackedDlls) {
            $trackedRelativePath = ([string]$trackedDll).Replace('/', '\')
            if (-not $manifestEntriesByPath.ContainsKey($trackedRelativePath)) {
                $failures.Add("Tracked DLL is missing from external binary manifest: $trackedRelativePath") | Out-Null
                $lines.Add("UNALLOWLISTED_TRACKED | $trackedRelativePath") | Out-Null
            }
        }
    }
}

if (Test-Path -LiteralPath $legacyDllRoot) {
    $failures.Add("Legacy Library-Noah DLL root must be removed: $legacyDllRoot") | Out-Null
    $lines.Add("FORBIDDEN | Legacy Library-Noah DLL root | $legacyDllRoot") | Out-Null
}

$legacyNames = @(
    "Lib.Common.dll",
    "Lib.OpenCV.dll",
    "Lib.OpenCV.Blob.dll",
    "OpenCvSharp.Extensions.dll",
    "Emgu.CV.UI.dll",
    "Emgu.CV.World.dll",
    "cvextern.dll")
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
