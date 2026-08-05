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

$sdkPolicy = (Get-Content -LiteralPath $globalJsonPath -Raw | ConvertFrom-Json).sdk
$minimumSdk = $sdkPolicy.version
$rollForward = $sdkPolicy.rollForward
$maximumSdkMajor = 9
$dotnetCommand = Get-Command dotnet -ErrorAction SilentlyContinue
if ($null -eq $dotnetCommand) {
    throw ".NET SDK 8.0.100 or later in the 8.x or 9.x line is required. Install Visual Studio 2022 17.8+ with the .NET desktop development workload, or install a supported SDK."
}

Set-Location -LiteralPath $repoRoot
$selectedSdk = (& dotnet --version 2>&1 | Out-String).Trim()
if ($LASTEXITCODE -ne 0) {
    throw "No SDK satisfies global.json. Install Visual Studio 2022 17.8+ with the .NET desktop development workload, or install .NET SDK 8.0.100+ or 9.x.`n$selectedSdk"
}

try {
    $minimumSdkVersion = [Version]$minimumSdk
    $selectedSdkVersion = [Version]$selectedSdk
}
catch {
    throw "Could not parse the .NET SDK policy or selected SDK. Minimum: $minimumSdk; selected: $selectedSdk."
}
if ($selectedSdkVersion -lt $minimumSdkVersion -or
    $selectedSdkVersion.Major -gt $maximumSdkMajor) {
    throw "global.json selected unsupported .NET SDK $selectedSdk. Expected SDK 8.0.100+ in the 8.x line or SDK 9.x."
}

$installedRuntimes = @(& dotnet --list-runtimes)
if ($LASTEXITCODE -ne 0) {
    throw "Could not inspect installed .NET runtimes."
}
$net8Runtime = $installedRuntimes | Where-Object { $_ -match '^Microsoft\.NETCore\.App 8\.' } | Select-Object -Last 1
$net8DesktopRuntime = $installedRuntimes | Where-Object { $_ -match '^Microsoft\.WindowsDesktop\.App 8\.' } | Select-Object -Last 1
if ([string]::IsNullOrWhiteSpace($net8Runtime) -or
    [string]::IsNullOrWhiteSpace($net8DesktopRuntime)) {
    throw ".NET 8 Desktop Runtime is required to run the net8.0 WPF application and verification tools. Add the Visual Studio .NET desktop development workload or install the .NET 8 Desktop Runtime."
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
    DotnetSdk = $selectedSdk
    MinimumDotnetSdk = $minimumSdk
    MaximumDotnetSdkMajor = $maximumSdkMajor
    DotnetSdkRollForward = $rollForward
    Net8Runtime = $net8Runtime
    Net8DesktopRuntime = $net8DesktopRuntime
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
