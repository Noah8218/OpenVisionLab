param(
    [string]$SharedRoot = "C:\OpenVisionLabSandbox"
)

$ErrorActionPreference = "Stop"

$payloadRoot = Join-Path $SharedRoot "payload"
$sourceArchive = Join-Path $payloadRoot "OpenVisionLab-source.zip"
$verifyScript = Join-Path $payloadRoot "VerifySourceBuild.ps1"
$sourceRoot = "C:\OpenVisionLabSource"
$sdkRoot = "C:\OpenVisionLabDotnet"
$transcriptPath = Join-Path $SharedRoot "sandbox-transcript.txt"
$resultPath = Join-Path $SharedRoot "sandbox-result.json"
$progressPath = Join-Path $SharedRoot "sandbox-progress.txt"
$startedAt = Get-Date
$minimumSdk = "unavailable"

function Write-ProgressState {
    param([string]$State)

    $line = "{0} | {1}" -f (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"), $State
    Add-Content -LiteralPath $progressPath -Value $line -Encoding UTF8
}

function Write-Result {
    param(
        [string]$Status,
        [string]$Message,
        [string]$SummaryPath = ""
    )

    $completedAt = Get-Date
    [pscustomobject][ordered]@{
        Status = $Status
        Message = $Message
        StartedAtUtc = $startedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        CompletedAtUtc = $completedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        DurationSeconds = [Math]::Round(($completedAt - $startedAt).TotalSeconds, 3)
        SourceCommit = (Get-Content -LiteralPath (Join-Path $payloadRoot "source-commit.txt") -Raw).Trim()
        DotnetSdk = $minimumSdk
        SourceBuildSummary = $SummaryPath
        Transcript = "sandbox-transcript.txt"
    } | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $resultPath -Encoding UTF8
}

try {
    Start-Transcript -LiteralPath $transcriptPath -Force | Out-Null
    Write-ProgressState "BootstrapStarted"

    if (-not (Test-Path -LiteralPath $sourceArchive)) {
        throw "Source archive was not mapped into Windows Sandbox: $sourceArchive"
    }
    if (-not (Test-Path -LiteralPath $verifyScript)) {
        throw "Source-build verification script was not mapped into Windows Sandbox: $verifyScript"
    }

    if (Test-Path -LiteralPath $sourceRoot) {
        Remove-Item -LiteralPath $sourceRoot -Recurse -Force
    }
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::ExtractToDirectory($sourceArchive, $sourceRoot)
    Copy-Item -LiteralPath $verifyScript -Destination (Join-Path $sourceRoot "tools\VerifySourceBuild.ps1") -Force
    $globalJsonPath = Join-Path $sourceRoot "global.json"
    $minimumSdk = (Get-Content -LiteralPath $globalJsonPath -Raw | ConvertFrom-Json).sdk.version
    Write-ProgressState "SourceExtracted"

    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    $installerPath = Join-Path $env:TEMP "dotnet-install.ps1"
    Invoke-WebRequest -UseBasicParsing -Uri "https://dot.net/v1/dotnet-install.ps1" -OutFile $installerPath
    Write-ProgressState "DotnetInstallerDownloaded"
    & powershell -NoProfile -ExecutionPolicy Bypass -File $installerPath -Version $minimumSdk -InstallDir $sdkRoot -Architecture x64
    if ($LASTEXITCODE -ne 0) {
        throw ".NET SDK installation failed with exit code $LASTEXITCODE."
    }
    Write-ProgressState "DotnetSdkInstalled"

    $env:PATH = "$sdkRoot;$env:PATH"

    $sandboxEvidence = Join-Path $sourceRoot "artifacts\sandbox_source_build"
    Write-ProgressState "SourceBuildStarted"
    & powershell `
        -NoProfile `
        -ExecutionPolicy Bypass `
        -File (Join-Path $sourceRoot "tools\VerifySourceBuild.ps1") `
        -OutputDir $sandboxEvidence
    if ($LASTEXITCODE -ne 0) {
        throw "Source-build verification failed with exit code $LASTEXITCODE."
    }

    $summaryPath = Join-Path $sandboxEvidence "source_build_summary.json"
    if (-not (Test-Path -LiteralPath $summaryPath)) {
        throw "Source-build verification did not produce its expected summary."
    }
    Copy-Item -LiteralPath $summaryPath -Destination (Join-Path $SharedRoot "source_build_summary.json") -Force
    Write-ProgressState "SourceBuildPassed"
    Write-Result -Status "PASS" -Message "Clean Windows Sandbox source build passed." -SummaryPath "source_build_summary.json"
}
catch {
    Write-ProgressState ("Failed: " + $_.Exception.Message)
    Write-Result -Status "FAIL" -Message $_.Exception.Message
}
finally {
    try {
        Stop-Transcript | Out-Null
    }
    catch {
    }
}
