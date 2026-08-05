param(
    [string]$DistributionDir = "dist\OpenVisionLab",
    [string]$ArchivePath = "",
    [string]$ChecksumPath = "",
    [switch]$SkipLaunch
)

$ErrorActionPreference = "Stop"

$repoRoot = [System.IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$distributionFullPath = if ([System.IO.Path]::IsPathRooted($DistributionDir)) {
    [System.IO.Path]::GetFullPath($DistributionDir)
}
else {
    [System.IO.Path]::GetFullPath((Join-Path $repoRoot $DistributionDir))
}
$distRoot = [System.IO.Path]::GetFullPath((Join-Path $repoRoot "dist"))
$distRootPrefix = $distRoot.TrimEnd('\', '/') + [System.IO.Path]::DirectorySeparatorChar
if (-not $distributionFullPath.StartsWith($distRootPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "DistributionDir must be under the repository dist directory: $distributionFullPath"
}
if (-not (Test-Path -LiteralPath $distributionFullPath -PathType Container)) {
    throw "Release distribution directory was not found: $distributionFullPath"
}

$manifestPath = Join-Path $distributionFullPath "clean_runtime_manifest.json"
if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf)) {
    throw "Release manifest was not found: $manifestPath"
}
$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json

$invalidManifestIdentity = ($manifest.SchemaVersion -ne 2) -or ($manifest.Mode -ne "Release") -or ($manifest.Configuration -ne "Release") -or ($manifest.Runtime -ne "win-x64")
if ($invalidManifestIdentity) {
    throw "Release manifest identity is invalid. Schema=$($manifest.SchemaVersion), Mode=$($manifest.Mode), Configuration=$($manifest.Configuration), Runtime=$($manifest.Runtime)"
}

$headCommit = (& git -C $repoRoot rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($headCommit)) {
    throw "Could not resolve the source commit."
}
if ($manifest.SourceCommit -ne $headCommit) {
    throw "Release manifest commit does not match HEAD. Manifest=$($manifest.SourceCommit), HEAD=$headCommit"
}
if (-not $manifest.SourceTreeClean) {
    throw "Release manifest records a dirty tracked source tree. Commit the intended source before creating a release candidate."
}

$requiredNames = @(
    "OpenVisionLab.exe",
    "OpenVisionLab.dll",
    "OpenVisionLab.runtimeconfig.json",
    "OpenVisionLab.Core.dll",
    "OpenVisionLab.Vision2D.dll",
    "OpenVisionLab.Vision2D.Blob.dll",
    "OpenCvSharp.dll",
    "OpenCvSharp.Blob.dll",
    "OpenCvSharpExtern.dll",
    "System.Windows.Controls.WpfPropertyGrid.dll",
    "LICENSE",
    "NOTICE",
    "DEPLOYMENT_README.txt"
)
foreach ($requiredName in $requiredNames) {
    $requiredPath = Join-Path $distributionFullPath $requiredName
    if (-not (Test-Path -LiteralPath $requiredPath -PathType Leaf)) {
        throw "Release distribution is missing required file: $requiredName"
    }
}

if (-not $manifest.IncludeSymbols) {
    $symbols = @(Get-ChildItem -LiteralPath $distributionFullPath -Filter "*.pdb" -File -Recurse)
    if ($symbols.Count -gt 0) {
        throw "Symbol files remain in the customer package: $($symbols.Name -join ', ')"
    }
}

$manifestFilePaths = New-Object "System.Collections.Generic.HashSet[string]" ([System.StringComparer]::OrdinalIgnoreCase)
foreach ($entry in $manifest.Files) {
    $entryFullPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $entry.Path))
    $distributionPrefix = $distributionFullPath.TrimEnd('\', '/') + [System.IO.Path]::DirectorySeparatorChar
    if (-not $entryFullPath.StartsWith($distributionPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Manifest file points outside the release distribution: $($entry.Path)"
    }
    if (-not (Test-Path -LiteralPath $entryFullPath -PathType Leaf)) {
        throw "Manifest file is missing: $($entry.Path)"
    }

    $file = Get-Item -LiteralPath $entryFullPath
    $actualHash = (Get-FileHash -LiteralPath $entryFullPath -Algorithm SHA256).Hash
    if ($file.Length -ne [long]$entry.Length -or $actualHash -ne $entry.SHA256) {
        throw "Manifest verification failed for $($entry.Path)."
    }
    [void]$manifestFilePaths.Add($entryFullPath)
}

$unexpectedPayloadFiles = @(
    Get-ChildItem -LiteralPath $distributionFullPath -File -Recurse |
        Where-Object {
            ($_.FullName -ne $manifestPath) -and (-not $manifestFilePaths.Contains($_.FullName))
        }
)
if ($unexpectedPayloadFiles.Count -gt 0) {
    throw "Files exist outside the release manifest: $($unexpectedPayloadFiles.FullName -join ', ')"
}

$runtimeConfig = Get-Content -LiteralPath (Join-Path $distributionFullPath "OpenVisionLab.runtimeconfig.json") -Raw | ConvertFrom-Json
if (-not $manifest.SelfContained) {
    $desktopFramework = @($runtimeConfig.runtimeOptions.frameworks) |
        Where-Object { $_.name -eq "Microsoft.WindowsDesktop.App" } |
        Select-Object -First 1
    if ($null -eq $desktopFramework -or -not $desktopFramework.version.StartsWith("8.")) {
        throw "Framework-dependent release does not declare Microsoft.WindowsDesktop.App 8.x."
    }
}

$packageKind = if ($manifest.SelfContained) { "self-contained" } else { "framework-dependent" }
if ([string]::IsNullOrWhiteSpace($ArchivePath)) {
    $ArchivePath = Join-Path $distRoot ("OpenVisionLab-$($manifest.Runtime)-$packageKind.zip")
}
$archiveFullPath = if ([System.IO.Path]::IsPathRooted($ArchivePath)) {
    [System.IO.Path]::GetFullPath($ArchivePath)
}
else {
    [System.IO.Path]::GetFullPath((Join-Path $repoRoot $ArchivePath))
}
if ([string]::IsNullOrWhiteSpace($ChecksumPath)) {
    $ChecksumPath = $archiveFullPath + ".sha256"
}
$checksumFullPath = if ([System.IO.Path]::IsPathRooted($ChecksumPath)) {
    [System.IO.Path]::GetFullPath($ChecksumPath)
}
else {
    [System.IO.Path]::GetFullPath((Join-Path $repoRoot $ChecksumPath))
}
$releasePackageMissing = (-not (Test-Path -LiteralPath $archiveFullPath -PathType Leaf)) -or (-not (Test-Path -LiteralPath $checksumFullPath -PathType Leaf))
if ($releasePackageMissing) {
    throw "Release archive or checksum is missing."
}

$expectedArchiveHash = ((Get-Content -LiteralPath $checksumFullPath -Raw).Trim() -split '\s+')[0]
$actualArchiveHash = (Get-FileHash -LiteralPath $archiveFullPath -Algorithm SHA256).Hash
if ($expectedArchiveHash -ne $actualArchiveHash) {
    throw "Release archive checksum mismatch. Expected=$expectedArchiveHash, Actual=$actualArchiveHash"
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::OpenRead($archiveFullPath)
try {
    $archiveEntryNames = @($archive.Entries | ForEach-Object { $_.FullName.Replace('/', '\') })
    foreach ($requiredName in $requiredNames + "clean_runtime_manifest.json") {
        if (-not ($archiveEntryNames | Where-Object { $_.EndsWith("OpenVisionLab\$requiredName", [System.StringComparison]::OrdinalIgnoreCase) })) {
            throw "Release archive is missing required entry: $requiredName"
        }
    }
}
finally {
    $archive.Dispose()
}

$launchEvidence = "Skipped"
$launchDataRoot = "Skipped"
if (-not $SkipLaunch) {
    $launchRoot = Join-Path $repoRoot ("artifacts\release_launch_smoke_" + (Get-Date -Format "yyyyMMdd_HHmmss"))
    if (Test-Path -LiteralPath $launchRoot) {
        throw "Launch smoke directory already exists: $launchRoot"
    }

    $launchInstallRoot = Join-Path $launchRoot "install"
    $launchDataRoot = Join-Path $launchRoot "data"
    Copy-Item -LiteralPath $distributionFullPath -Destination $launchInstallRoot -Recurse

    $legacyConfig = Join-Path $launchInstallRoot "CONFIG"
    $legacyRecipe = Join-Path $launchInstallRoot "RECIPE\LegacyMigration"
    $legacyQualified = Join-Path $launchInstallRoot "QUALIFIED_RECIPE"
    New-Item -ItemType Directory -Force -Path $legacyConfig,$legacyRecipe,$legacyQualified | Out-Null
    Set-Content -LiteralPath (Join-Path $legacyConfig "legacy-marker.txt") -Value "legacy-config" -Encoding UTF8
    Set-Content -LiteralPath (Join-Path $legacyRecipe "legacy-marker.txt") -Value "legacy-recipe" -Encoding UTF8
    Set-Content -LiteralPath (Join-Path $legacyQualified "legacy-marker.txt") -Value "legacy-qualified" -Encoding UTF8
    Set-Content -LiteralPath (Join-Path $legacyConfig "conflict-marker.txt") -Value "legacy-conflict" -Encoding UTF8
    New-Item -ItemType Directory -Force -Path (Join-Path $launchDataRoot "CONFIG") | Out-Null
    Set-Content -LiteralPath (Join-Path $launchDataRoot "CONFIG\conflict-marker.txt") -Value "data-root-wins" -Encoding UTF8

    function Get-InstallFileSnapshot {
        param([string]$Root)

        $snapshot = @{}
        Get-ChildItem -LiteralPath $Root -File -Recurse | ForEach-Object {
            $relative = $_.FullName.Substring($Root.Length).TrimStart('\', '/')
            $snapshot[$relative] = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash
        }
        return $snapshot
    }

    function Invoke-ReleaseLaunch {
        param(
            [string]$Exe,
            [string]$WorkingDirectory,
            [string]$DataRoot
        )

        $previousDataRoot = $env:OPENVISIONLAB_DATA_ROOT
        try {
            $env:OPENVISIONLAB_DATA_ROOT = $DataRoot
            $process = Start-Process -FilePath $Exe -WorkingDirectory $WorkingDirectory -WindowStyle Hidden -PassThru
            Start-Sleep -Seconds 6
            $process.Refresh()
            if ($process.HasExited) {
                throw "Release EXE exited during startup smoke. ExitCode=$($process.ExitCode)"
            }
            Stop-Process -Id $process.Id -Force
            $process.WaitForExit()
        }
        finally {
            $env:OPENVISIONLAB_DATA_ROOT = $previousDataRoot
        }
    }

    $beforeInstall = Get-InstallFileSnapshot -Root $launchInstallRoot
    $launchExe = Join-Path $launchInstallRoot "OpenVisionLab.exe"
    Invoke-ReleaseLaunch -Exe $launchExe -WorkingDirectory $launchInstallRoot -DataRoot $launchDataRoot

    foreach ($runtimeDataDirectory in @("CONFIG", "RECIPE", "Log")) {
        if (-not (Test-Path -LiteralPath (Join-Path $launchDataRoot $runtimeDataDirectory) -PathType Container)) {
            throw "Release launch did not initialize expected data-root directory: $runtimeDataDirectory"
        }
    }

    $migrationExpectations = @{
        "CONFIG\legacy-marker.txt" = "legacy-config"
        "RECIPE\LegacyMigration\legacy-marker.txt" = "legacy-recipe"
        "QUALIFIED_RECIPE\legacy-marker.txt" = "legacy-qualified"
        "CONFIG\conflict-marker.txt" = "data-root-wins"
    }
    foreach ($relativePath in $migrationExpectations.Keys) {
        $migratedPath = Join-Path $launchDataRoot $relativePath
        if (-not (Test-Path -LiteralPath $migratedPath -PathType Leaf)) {
            throw "Release data-root migration did not retain expected file: $relativePath"
        }
        $actualValue = (Get-Content -LiteralPath $migratedPath -Raw).Trim()
        if ($actualValue -ne $migrationExpectations[$relativePath]) {
            throw "Release data-root migration changed expected content: $relativePath"
        }
    }

    $migrationReport = Join-Path $launchDataRoot "data-root-migration-v1.txt"
    if (-not (Test-Path -LiteralPath $migrationReport -PathType Leaf)) {
        throw "Release data-root migration report was not created."
    }
    $migrationReportText = Get-Content -LiteralPath $migrationReport -Raw
    $migrationReportValid =
        $migrationReportText.Contains("Status=Complete") -and
        $migrationReportText.Contains("ConflictTargetKept=CONFIG\conflict-marker.txt")
    if (-not $migrationReportValid) {
        throw "Release data-root migration report does not retain completion/conflict evidence."
    }

    $afterInstall = Get-InstallFileSnapshot -Root $launchInstallRoot
    if ($beforeInstall.Count -ne $afterInstall.Count) {
        throw "Release launch changed the immutable installation file inventory."
    }
    foreach ($relativePath in $beforeInstall.Keys) {
        $installFileChanged =
            (-not $afterInstall.ContainsKey($relativePath)) -or
            ($beforeInstall[$relativePath] -ne $afterInstall[$relativePath])
        if ($installFileChanged) {
            throw "Release launch changed an immutable installation file: $relativePath"
        }
    }
    if (Test-Path -LiteralPath (Join-Path $launchInstallRoot "Log")) {
        throw "Release launch created a Log directory inside the installation root."
    }

    Invoke-ReleaseLaunch -Exe $launchExe -WorkingDirectory $launchInstallRoot -DataRoot $launchDataRoot
    if ((Get-Content -LiteralPath (Join-Path $launchDataRoot "CONFIG\conflict-marker.txt") -Raw).Trim() -ne "data-root-wins") {
        throw "Second launch did not preserve the selected data-root state."
    }

    $launchEvidence = $launchRoot
}

Write-Host "ReleaseDistributionCheck=PASS"
Write-Host "Commit=$headCommit"
Write-Host "Runtime=$($manifest.Runtime)"
Write-Host "SelfContained=$($manifest.SelfContained)"
Write-Host "PayloadFiles=$($manifest.Files.Count)"
Write-Host "Archive=$archiveFullPath"
Write-Host "ArchiveSHA256=$actualArchiveHash"
Write-Host "LaunchEvidence=$launchEvidence"
Write-Host "LaunchDataRoot=$launchDataRoot"
