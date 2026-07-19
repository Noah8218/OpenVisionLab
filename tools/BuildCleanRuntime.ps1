param(
    [string]$Configuration = "",
    [string]$Platform = "AnyCPU",
    [ValidateSet("Dev", "Release")]
    [string]$Mode = "Dev",
    [string]$OutputDir = ""
)

$ErrorActionPreference = "Stop"

$repoRoot = [System.IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$artifactRoot = [System.IO.Path]::GetFullPath((Join-Path $repoRoot "artifacts"))
$distributionRoot = [System.IO.Path]::GetFullPath((Join-Path $repoRoot "dist"))
$releaseOutputRoot = [System.IO.Path]::GetFullPath((Join-Path $distributionRoot "OpenVisionLab"))

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

New-Item -ItemType Directory -Force -Path $outputFullPath | Out-Null

$projectPath = Join-Path $repoRoot "OpenVisionLab.csproj"
$runtimeArguments = if ($Mode -eq "Release") {
    @(
        "publish",
        $projectPath,
        "-c",
        $Configuration,
        "-p:Platform=$Platform",
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

$requiredFiles = @(
    "OpenVisionLab.exe",
    "OpenVisionLab.dll",
    "OpenVisionLab.runtimeconfig.json",
    "OpenCvSharp.dll",
    "OpenCvSharpExtern.dll",
    "Lib.OpenCV.dll",
    "System.Windows.Controls.WpfPropertyGrid.dll"
)

$missingFiles = @($requiredFiles | Where-Object {
    -not (Test-Path -LiteralPath (Join-Path $outputFullPath $_))
})
if ($missingFiles.Count -gt 0) {
    throw "Clean runtime is missing required file(s): " + ($missingFiles -join ", ")
}

$runtimeFiles = @(
    foreach ($fileName in $requiredFiles) {
        $filePath = Join-Path $outputFullPath $fileName
        $file = Get-Item -LiteralPath $filePath
        [pscustomobject][ordered]@{
            Name = $file.Name
            Length = $file.Length
            SHA256 = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
        }
    }
)

$manifest = [pscustomobject][ordered]@{
    BuiltAt = (Get-Date -Format "yyyy-MM-dd HH:mm:ss")
    Mode = $Mode
    Configuration = $Configuration
    Platform = $Platform
    RuntimeDirectory = Convert-ToDisplayPath $outputFullPath
    RuntimeExe = Convert-ToDisplayPath (Join-Path $outputFullPath "OpenVisionLab.exe")
    RuntimeArguments = $runtimeArguments
    RequiredFiles = $runtimeFiles
}

$manifestPath = Join-Path $outputFullPath "clean_runtime_manifest.json"
$manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $manifestPath -Encoding UTF8

Write-Host "$Mode clean runtime: $(Convert-ToDisplayPath $outputFullPath)"
Write-Host "Runtime EXE: $(Convert-ToDisplayPath (Join-Path $outputFullPath 'OpenVisionLab.exe'))"
Write-Host "Manifest: $(Convert-ToDisplayPath $manifestPath)"
