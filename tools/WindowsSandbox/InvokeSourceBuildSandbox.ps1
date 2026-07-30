param(
    [switch]$PrepareOnly,
    [int]$WaitMinutes = 30,
    [string]$OutputDir = ""
)

$ErrorActionPreference = "Stop"

$repoRoot = [System.IO.Path]::GetFullPath((Split-Path -Parent (Split-Path -Parent $PSScriptRoot)))
if ($env:OS -ne "Windows_NT") {
    throw "Windows Sandbox verification must be launched from Windows."
}
if ($WaitMinutes -lt 1 -or $WaitMinutes -gt 120) {
    throw "WaitMinutes must be between 1 and 120."
}

if ([string]::IsNullOrWhiteSpace($OutputDir)) {
    $OutputDir = "artifacts\source_build_sandbox_" + (Get-Date -Format "yyyyMMdd_HHmmss")
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
    throw "Sandbox evidence directory already exists: $outputFullPath"
}

$sandboxCommand = Get-Command WindowsSandbox.exe -ErrorAction SilentlyContinue
if (-not $PrepareOnly -and $null -eq $sandboxCommand) {
    throw "WindowsSandbox.exe was not found. Enable Windows Sandbox from Windows Features, restart if requested, and run this command again."
}
if (-not $PrepareOnly) {
    $existingSandboxProcesses = @(Get-Process WindowsSandbox, WindowsSandboxClient -ErrorAction SilentlyContinue)
    if ($existingSandboxProcesses.Count -gt 0) {
        throw "Another Windows Sandbox instance is already open. Close it before starting the automated source-build check."
    }
}

$payloadRoot = Join-Path $outputFullPath "payload"
New-Item -ItemType Directory -Path $payloadRoot | Out-Null

$commit = (& git -C $repoRoot rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($commit)) {
    throw "Could not resolve the source commit."
}

$sourceArchive = Join-Path $payloadRoot "OpenVisionLab-source.zip"
& git -C $repoRoot archive --format=zip --output=$sourceArchive HEAD
if ($LASTEXITCODE -ne 0 -or -not (Test-Path -LiteralPath $sourceArchive)) {
    throw "Could not create the clean committed-source archive."
}

Copy-Item `
    -LiteralPath (Join-Path $repoRoot "tools\VerifySourceBuild.ps1") `
    -Destination (Join-Path $payloadRoot "VerifySourceBuild.ps1")
Copy-Item `
    -LiteralPath (Join-Path $PSScriptRoot "RunSourceBuildSandbox.ps1") `
    -Destination (Join-Path $payloadRoot "RunSourceBuildSandbox.ps1")
Set-Content -LiteralPath (Join-Path $payloadRoot "source-commit.txt") -Value $commit -Encoding ASCII

$escapedHostPath = [System.Security.SecurityElement]::Escape($outputFullPath)
$wsbPath = Join-Path $outputFullPath "OpenVisionLabSourceBuild.wsb"
$configuration = @"
<Configuration>
  <VGpu>Disable</VGpu>
  <Networking>Enable</Networking>
  <MappedFolders>
    <MappedFolder>
      <HostFolder>$escapedHostPath</HostFolder>
      <SandboxFolder>C:\OpenVisionLabSandbox</SandboxFolder>
      <ReadOnly>false</ReadOnly>
    </MappedFolder>
  </MappedFolders>
  <LogonCommand>
    <Command>powershell.exe -NoProfile -ExecutionPolicy Bypass -File C:\OpenVisionLabSandbox\payload\RunSourceBuildSandbox.ps1 -SharedRoot C:\OpenVisionLabSandbox</Command>
  </LogonCommand>
</Configuration>
"@
Set-Content -LiteralPath $wsbPath -Value $configuration -Encoding UTF8

Write-Host "SandboxSourceCommit=$commit"
Write-Host "SandboxConfiguration=$wsbPath"
Write-Host "SandboxEvidence=$outputFullPath"

if ($PrepareOnly) {
    Write-Host "SandboxPreparation=PASS"
    exit 0
}

function Stop-LaunchedSandbox {
    $processes = @(Get-Process WindowsSandbox, WindowsSandboxClient -ErrorAction SilentlyContinue)
    foreach ($process in $processes) {
        if (-not $process.HasExited) {
            $process.CloseMainWindow() | Out-Null
        }
    }
    Start-Sleep -Seconds 3
    foreach ($process in $processes) {
        $current = Get-Process -Id $process.Id -ErrorAction SilentlyContinue
        if ($null -ne $current) {
            Stop-Process -Id $current.Id -Force
        }
    }
}

Start-Process -FilePath $sandboxCommand.Source -ArgumentList "`"$wsbPath`""
$resultPath = Join-Path $outputFullPath "sandbox-result.json"
$deadline = (Get-Date).AddMinutes($WaitMinutes)
while ((Get-Date) -lt $deadline) {
    if (Test-Path -LiteralPath $resultPath) {
        $result = Get-Content -LiteralPath $resultPath -Raw | ConvertFrom-Json
        Write-Host "WindowsSandboxSourceBuild=$($result.Status)"
        Write-Host "Result=$resultPath"
        Write-Host "Transcript=$($result.Transcript)"
        if ($result.Status -ne "PASS") {
            Stop-LaunchedSandbox
            throw "Windows Sandbox source build failed: $($result.Message)"
        }
        Stop-LaunchedSandbox
        exit 0
    }
    Start-Sleep -Seconds 5
}

Stop-LaunchedSandbox
throw "Windows Sandbox did not produce a result within $WaitMinutes minutes. Inspect: $outputFullPath"
