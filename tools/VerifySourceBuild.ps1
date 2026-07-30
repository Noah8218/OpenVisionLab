param(
    [string]$OutputDir = ""
)

$ErrorActionPreference = "Stop"

$repoRoot = [System.IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$solutionPath = Join-Path $repoRoot "OpenVisionLab.sln"
$globalJsonPath = Join-Path $repoRoot "global.json"

if (-not $IsWindows -and $env:OS -ne "Windows_NT") {
    throw "OpenVisionLab is a WPF application and must be built on Windows."
}
if (-not (Test-Path -LiteralPath $solutionPath)) {
    throw "OpenVisionLab.sln was not found. Run this script from a complete repository checkout."
}
if (-not (Test-Path -LiteralPath $globalJsonPath)) {
    throw "global.json was not found. The required .NET SDK version cannot be verified."
}

$requiredSdk = (Get-Content -LiteralPath $globalJsonPath -Raw | ConvertFrom-Json).sdk.version
$dotnetCommand = Get-Command dotnet -ErrorAction SilentlyContinue
if ($null -eq $dotnetCommand) {
    throw ".NET SDK $requiredSdk is required. Install the official .NET 8 SDK and run this command again: https://dotnet.microsoft.com/download/dotnet/8.0"
}

$installedSdks = @(
    & dotnet --list-sdks |
        ForEach-Object { ($_ -split "\s+")[0] } |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
)
if ($LASTEXITCODE -ne 0) {
    throw "Could not inspect installed .NET SDK versions."
}
if ($installedSdks -notcontains $requiredSdk) {
    $installedText = if ($installedSdks.Count -eq 0) { "none" } else { $installedSdks -join ", " }
    throw ".NET SDK $requiredSdk is required by global.json. Installed SDKs: $installedText. Install the exact SDK and run this command again: https://dotnet.microsoft.com/download/dotnet/8.0"
}

if ([string]::IsNullOrWhiteSpace($OutputDir)) {
    $OutputDir = "artifacts\source_build_verification_" + (Get-Date -Format "yyyyMMdd_HHmmss")
}
$outputFullPath = if ([System.IO.Path]::IsPathRooted($OutputDir)) {
    [System.IO.Path]::GetFullPath($OutputDir)
}
else {
    [System.IO.Path]::GetFullPath((Join-Path $repoRoot $OutputDir))
}
$artifactRoot = [System.IO.Path]::GetFullPath((Join-Path $repoRoot "artifacts"))
$artifactPrefix = $artifactRoot.TrimEnd('\', '/') + [System.IO.Path]::DirectorySeparatorChar
if (-not $outputFullPath.StartsWith($artifactPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "OutputDir must be a new directory under artifacts: $outputFullPath"
}
if (Test-Path -LiteralPath $outputFullPath) {
    throw "Source-build evidence directory already exists: $outputFullPath"
}
New-Item -ItemType Directory -Path $outputFullPath | Out-Null

function Invoke-NativeStep {
    param(
        [string]$Name,
        [scriptblock]$Action
    )

    Write-Host "== $Name =="
    & $Action
    if ($LASTEXITCODE -ne 0) {
        throw "$Name failed with exit code $LASTEXITCODE."
    }
}

$startedAt = Get-Date
$commit = "unavailable"
$gitCommand = Get-Command git -ErrorAction SilentlyContinue
if ($null -ne $gitCommand) {
    $candidateCommit = (& git -C $repoRoot rev-parse HEAD 2>$null)
    if ($LASTEXITCODE -eq 0 -and -not [string]::IsNullOrWhiteSpace($candidateCommit)) {
        $commit = $candidateCommit.Trim()
    }
}

Invoke-NativeStep "Locked package restore" {
    & dotnet restore $solutionPath --locked-mode
}

Invoke-NativeStep "Debug solution build" {
    & dotnet build $solutionPath -c Debug -p:Platform="Any CPU" --no-restore
}

Invoke-NativeStep "Release solution build" {
    & dotnet build $solutionPath -c Release -p:Platform="Any CPU" --no-restore
}

Invoke-NativeStep "Repository readiness" {
    & dotnet run `
        --project (Join-Path $repoRoot "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj") `
        -c Release `
        --no-build `
        -- `
        $repoRoot
}

Invoke-NativeStep "Vendored runtime references" {
    & powershell `
        -NoProfile `
        -ExecutionPolicy Bypass `
        -File (Join-Path $repoRoot "tools\TestExternalReferences.ps1") `
        -Configuration Release
}

$debugExe = Join-Path $repoRoot "bin\Debug\OpenVisionLab.exe"
$releaseExe = Join-Path $repoRoot "bin\Release\OpenVisionLab.exe"
if (-not (Test-Path -LiteralPath $debugExe)) {
    throw "Debug build completed without the expected executable: $debugExe"
}
if (-not (Test-Path -LiteralPath $releaseExe)) {
    throw "Release build completed without the expected executable: $releaseExe"
}

$completedAt = Get-Date
$summary = [pscustomobject][ordered]@{
    Status = "PASS"
    StartedAtUtc = $startedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    CompletedAtUtc = $completedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    DurationSeconds = [Math]::Round(($completedAt - $startedAt).TotalSeconds, 3)
    Commit = $commit
    DotnetSdk = $requiredSdk
    LockedRestore = "PASS"
    DebugBuild = "PASS"
    ReleaseBuild = "PASS"
    Readiness = "PASS"
    ExternalReferences = "PASS"
    DebugExecutable = "bin\Debug\OpenVisionLab.exe"
    ReleaseExecutable = "bin\Release\OpenVisionLab.exe"
}
$summaryPath = Join-Path $outputFullPath "source_build_summary.json"
$summary | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $summaryPath -Encoding UTF8

Write-Host ""
Write-Host "SourceBuildVerification=PASS"
Write-Host "Summary=$summaryPath"
Write-Host "DebugExecutable=$debugExe"
Write-Host "ReleaseExecutable=$releaseExe"
