param(
    [string]$Configuration = "",
    [string]$Platform = "AnyCPU",
    [ValidateSet("Dev", "Release")]
    [string]$Mode = "Dev",
    [string]$OutputDir = "",
    [ValidateSet("win-x64")]
    [string]$Runtime = "win-x64",
    [switch]$SelfContained,
    [switch]$IncludeSymbols
)

$ErrorActionPreference = "Stop"

$repoRoot = [System.IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$artifactRoot = [System.IO.Path]::GetFullPath((Join-Path $repoRoot "artifacts"))
$distributionRoot = [System.IO.Path]::GetFullPath((Join-Path $repoRoot "dist"))
$releaseOutputRoot = [System.IO.Path]::GetFullPath((Join-Path $distributionRoot "OpenVisionLab"))
$releaseArchivePath = [System.IO.Path]::GetFullPath(
    (Join-Path $distributionRoot ("OpenVisionLab-" + $Runtime + "-" + $(if ($SelfContained) { "self-contained" } else { "framework-dependent" }) + ".zip")))
$releaseChecksumPath = $releaseArchivePath + ".sha256"

if ([string]::IsNullOrWhiteSpace($Configuration)) {
    $Configuration = if ($Mode -eq "Release") { "Release" } else { "Debug" }
}

function Resolve-RepoPath {
    param([string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return ""
    }

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Convert-ToDisplayPath {
    param([string]$Path)

    $fullPath = [System.IO.Path]::GetFullPath($Path)
    $rootWithSeparator = $repoRoot.TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar
    if ($fullPath.StartsWith($rootWithSeparator, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $fullPath.Substring($rootWithSeparator.Length)
    }

    return $fullPath
}

if ([string]::IsNullOrWhiteSpace($OutputDir)) {
    if ($Mode -eq "Release") {
        $OutputDir = Join-Path "dist" "OpenVisionLab"
    }
    else {
        $OutputDir = Join-Path "artifacts" ("openvisionlab_clean_runtime_" + (Get-Date -Format "yyyyMMdd_HHmmss"))
    }
}

$outputFullPath = Resolve-RepoPath $OutputDir
$outputRoot = if ($Mode -eq "Release") { $distributionRoot } else { $artifactRoot }
$outputRootWithSeparator = $outputRoot.TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar
if (-not $outputFullPath.StartsWith($outputRootWithSeparator, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "OutputDir must be a new directory under $(Convert-ToDisplayPath $outputRoot) for $Mode mode. Requested: $outputFullPath"
}

if ($Mode -eq "Release" -and -not [string]::Equals($outputFullPath, $releaseOutputRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "Release mode writes only to $(Convert-ToDisplayPath $releaseOutputRoot). Requested: $outputFullPath"
}

if (Test-Path -LiteralPath $outputFullPath) {
    throw "Clean runtime output directory already exists. Choose a new OutputDir: $outputFullPath"
}

if ($Mode -eq "Release") {
    if (Test-Path -LiteralPath $releaseArchivePath) {
        throw "Release archive already exists. Start from a clean dist directory: $releaseArchivePath"
    }

    if (Test-Path -LiteralPath $releaseChecksumPath) {
        throw "Release checksum already exists. Start from a clean dist directory: $releaseChecksumPath"
    }
}

New-Item -ItemType Directory -Force -Path $outputFullPath | Out-Null

$projectPath = Join-Path $repoRoot "OpenVisionLab.csproj"
$runtimeArguments = if ($Mode -eq "Release") {
    @(
        "publish",
        $projectPath,
        "-c",
        $Configuration,
        "-p:Platform=$Platform",
        "-r",
        $Runtime,
        "--self-contained",
        $SelfContained.IsPresent.ToString().ToLowerInvariant(),
        "-p:PublishDir=$outputFullPath\"
    )
}
else {
    @(
        "build",
        $projectPath,
        "-c",
        $Configuration,
        "-p:Platform=$Platform",
        "-p:OutputPath=$outputFullPath\"
    )
}

& dotnet @runtimeArguments
if ($LASTEXITCODE -ne 0) {
    throw "$Mode clean runtime build failed. Exit code=$LASTEXITCODE. Output: $outputFullPath"
}

if ($Mode -eq "Release" -and -not $IncludeSymbols) {
    $symbolFiles = @(Get-ChildItem -LiteralPath $outputFullPath -Filter "*.pdb" -File -Recurse)
    foreach ($symbolFile in $symbolFiles) {
        Remove-Item -LiteralPath $symbolFile.FullName -Force
    }
}

if ($Mode -eq "Release") {
    foreach ($legalFileName in @("LICENSE", "NOTICE")) {
        $sourceLegalPath = Join-Path $repoRoot $legalFileName
        if (-not (Test-Path -LiteralPath $sourceLegalPath -PathType Leaf)) {
            throw "Required release legal file is missing: $sourceLegalPath"
        }

        Copy-Item -LiteralPath $sourceLegalPath -Destination (Join-Path $outputFullPath $legalFileName)
    }

    $deploymentReadme = @"
OpenVisionLab portable release candidate

Runtime: $Runtime
Packaging: $(if ($SelfContained) { "self-contained" } else { "framework-dependent" })
Prerequisite: $(if ($SelfContained) { "No separately installed .NET runtime is required." } else { "Install the Microsoft .NET 8 Desktop Runtime (x64) before starting OpenVisionLab." })

This package is portable and writes CONFIG, RECIPE, and Log data below its extracted directory.
Extract it to an operator-writable folder. Do not install this package under Program Files.

This package is not code-signed and is not an installer. Verify the adjacent SHA-256 file
before distribution. Installer, certificate signing, update, rollback, and uninstall remain
separate release gates.
"@
    Set-Content -LiteralPath (Join-Path $outputFullPath "DEPLOYMENT_README.txt") -Value $deploymentReadme -Encoding UTF8
}

$requiredFiles = @(
    "OpenVisionLab.exe",
    "OpenVisionLab.dll",
    "OpenVisionLab.runtimeconfig.json",
    "OpenCvSharp.dll",
    "OpenCvSharpExtern.dll",
    "Lib.OpenCV.dll",
    "System.Windows.Controls.WpfPropertyGrid.dll"
)
if ($Mode -eq "Release") {
    $requiredFiles += @(
        "LICENSE",
        "NOTICE",
        "DEPLOYMENT_README.txt"
    )
}

$missingFiles = @($requiredFiles | Where-Object {
    -not (Test-Path -LiteralPath (Join-Path $outputFullPath $_))
})
if ($missingFiles.Count -gt 0) {
    throw "Clean runtime is missing required file(s): " + ($missingFiles -join ", ")
}

function Invoke-GitText {
    param([string[]]$Arguments)

    $output = & git -C $repoRoot @Arguments 2>$null
    if ($LASTEXITCODE -ne 0) {
        return ""
    }

    return (($output | ForEach-Object { $_.ToString() }) -join "`n").Trim()
}

$sourceCommit = Invoke-GitText @("rev-parse", "HEAD")
$sourceCommitTime = Invoke-GitText @("show", "-s", "--format=%cI", "HEAD")
$sourceBranch = Invoke-GitText @("branch", "--show-current")
$sourceStatus = Invoke-GitText @("status", "--porcelain", "--untracked-files=no")
$sourceRemote = Invoke-GitText @("remote", "get-url", "origin")
$sdkVersion = (& dotnet --version).Trim()

$runtimeFiles = @(
    Get-ChildItem -LiteralPath $outputFullPath -File -Recurse |
        Sort-Object FullName |
        ForEach-Object {
            [pscustomobject][ordered]@{
                Path = Convert-ToDisplayPath $_.FullName
                Length = $_.Length
                SHA256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash
            }
        }
)

$manifest = [pscustomobject][ordered]@{
    SchemaVersion = 2
    SourceCommitTimeUtc = if ([string]::IsNullOrWhiteSpace($sourceCommitTime)) {
        ""
    }
    else {
        ([DateTimeOffset]::Parse($sourceCommitTime)).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    }
    Mode = $Mode
    Configuration = $Configuration
    Platform = $Platform
    Runtime = if ($Mode -eq "Release") { $Runtime } else { "" }
    SelfContained = if ($Mode -eq "Release") { $SelfContained.IsPresent } else { $false }
    IncludeSymbols = $IncludeSymbols.IsPresent
    DotnetSdk = $sdkVersion
    SourceCommit = $sourceCommit
    SourceBranch = $sourceBranch
    SourceRemote = $sourceRemote
    SourceTreeClean = [string]::IsNullOrWhiteSpace($sourceStatus)
    RuntimeDirectory = Convert-ToDisplayPath $outputFullPath
    RuntimeExe = Convert-ToDisplayPath (Join-Path $outputFullPath "OpenVisionLab.exe")
    RuntimeArguments = if ($Mode -eq "Release") {
        @(
            "dotnet",
            "publish",
            "OpenVisionLab.csproj",
            "-c",
            $Configuration,
            "-p:Platform=$Platform",
            "-r",
            $Runtime,
            "--self-contained",
            $SelfContained.IsPresent.ToString().ToLowerInvariant()
        )
    }
    else {
        @(
            "dotnet",
            "build",
            "OpenVisionLab.csproj",
            "-c",
            $Configuration,
            "-p:Platform=$Platform"
        )
    }
    RuntimePrerequisite = if ($Mode -eq "Release" -and -not $SelfContained) {
        "Microsoft .NET 8 Desktop Runtime (x64)"
    }
    else {
        ""
    }
    Files = $runtimeFiles
}

$manifestPath = Join-Path $outputFullPath "clean_runtime_manifest.json"
$manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $manifestPath -Encoding UTF8

if ($Mode -eq "Release") {
    Add-Type -AssemblyName System.IO.Compression
    $archiveStream = [System.IO.File]::Open(
        $releaseArchivePath,
        [System.IO.FileMode]::CreateNew,
        [System.IO.FileAccess]::ReadWrite,
        [System.IO.FileShare]::None)
    try {
        $archive = [System.IO.Compression.ZipArchive]::new(
            $archiveStream,
            [System.IO.Compression.ZipArchiveMode]::Create,
            $false)
        try {
            $fixedArchiveTime = [DateTimeOffset]::Parse("2000-01-01T00:00:00Z")
            $archiveSourceFiles = @(
                Get-ChildItem -LiteralPath $outputFullPath -File -Recurse |
                    Sort-Object {
                        $_.FullName.Substring($outputFullPath.Length).Replace('\', '/')
                    }
            )
            foreach ($archiveSourceFile in $archiveSourceFiles) {
                $relativeArchivePath =
                    $archiveSourceFile.FullName.Substring($outputFullPath.Length).TrimStart('\', '/').Replace('\', '/')
                $entry = $archive.CreateEntry(
                    "OpenVisionLab/" + $relativeArchivePath,
                    [System.IO.Compression.CompressionLevel]::Optimal)
                $entry.LastWriteTime = $fixedArchiveTime
                $entryStream = $entry.Open()
                $sourceStream = [System.IO.File]::OpenRead($archiveSourceFile.FullName)
                try {
                    $sourceStream.CopyTo($entryStream)
                }
                finally {
                    $sourceStream.Dispose()
                    $entryStream.Dispose()
                }
            }
        }
        finally {
            $archive.Dispose()
        }
    }
    finally {
        $archiveStream.Dispose()
    }
    $archiveHash = (Get-FileHash -LiteralPath $releaseArchivePath -Algorithm SHA256).Hash
    Set-Content -LiteralPath $releaseChecksumPath -Value ($archiveHash + " *" + (Split-Path -Leaf $releaseArchivePath)) -Encoding ASCII
}

Write-Host "$Mode clean runtime: $(Convert-ToDisplayPath $outputFullPath)"
Write-Host "Runtime EXE: $(Convert-ToDisplayPath (Join-Path $outputFullPath 'OpenVisionLab.exe'))"
Write-Host "Manifest: $(Convert-ToDisplayPath $manifestPath)"
if ($Mode -eq "Release") {
    Write-Host "Archive: $(Convert-ToDisplayPath $releaseArchivePath)"
    Write-Host "Checksum: $(Convert-ToDisplayPath $releaseChecksumPath)"
}
