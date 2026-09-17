param(
    [string]$MachineRepo = 'C:\Git\Machine\Dev\OpenVisionLab-Machine-Studio-Dev',
    [string]$DevRepo = 'C:\Git\2D\Dev',
    [string]$EvidenceRoot = 'D:\OpenVisionLab-TestData\OpenVisionLab-CrossRepo\2d',
    [string]$MachineProjectPath = '',
    [string]$SourceImagePath = '',
    [string]$RecipePath = ''
)

$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($MachineProjectPath)) {
    $MachineProjectPath = Join-Path $MachineRepo 'samples\AutomaticTransferCell\AutomaticTransferCell.ovmachine'
}
if ([string]::IsNullOrWhiteSpace($SourceImagePath)) {
    $SourceImagePath = Join-Path $DevRepo 'docs\samples\public\EdgeDetection_Shapes_Synthetic_OK.png'
}
if ([string]::IsNullOrWhiteSpace($RecipePath)) {
    $RecipePath = Join-Path $DevRepo 'docs\samples\public\Public_EdgeDetection_Shapes.pipeline.xml'
}

foreach ($path in @($MachineRepo, $DevRepo, $MachineProjectPath, $SourceImagePath, $RecipePath)) {
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Required path was not found: $path"
    }
}

$runRoot = Join-Path $EvidenceRoot ("2d-cross-repo-{0}-{1}" -f (Get-Date -Format 'yyyyMMdd-HHmmss'), ([Guid]::NewGuid().ToString('N')))
$exchangeRoot = Join-Path $runRoot 'exchange'
$manifestPath = Join-Path $runRoot 'machine-producer-manifest.json'
$consumerEvidenceRoot = Join-Path $runRoot 'consumer'
$producerProject = Join-Path $MachineRepo 'tools\MachineIntegrationProducerSmoke\MachineIntegrationProducerSmoke.csproj'
$consumerProject = Join-Path $DevRepo 'tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj'
$runtimeBuildManifestPath = Join-Path $DevRepo 'bin\Release\openvisionlab.runtime.json'
$consumerPreflightPath = Join-Path $runRoot 'consumer-runtime-preflight.json'

New-Item -ItemType Directory -Force -Path $runRoot | Out-Null
$env:TEMP = $runRoot
$env:TMP = $runRoot

Push-Location $DevRepo
try {
    Write-Host '== Build Dev 2D consumer smoke =='
    & dotnet build $consumerProject -c Release --nologo
    if ($LASTEXITCODE -ne 0) {
        throw 'Dev 2D consumer smoke build failed.'
    }
}
finally {
    Pop-Location
}

if (-not (Test-Path -LiteralPath $runtimeBuildManifestPath -PathType Leaf)) {
    throw "2D runtime build manifest was not generated: $runtimeBuildManifestPath"
}
$runtimeBuildManifest = Get-Content -LiteralPath $runtimeBuildManifestPath -Raw | ConvertFrom-Json
$consumerVersion = [string]$runtimeBuildManifest.identity.applicationVersion
$consumerCommit = [string]$runtimeBuildManifest.identity.sourceCommit
$consumerSourceState = [string]$runtimeBuildManifest.identity.sourceState
if ($runtimeBuildManifest.schemaVersion -ne '1.0' -or $consumerSourceState -ne 'clean') {
    throw "2D runtime build is not qualified for integration. Schema=$($runtimeBuildManifest.schemaVersion), SourceState=$consumerSourceState"
}

$consumerHeadCommit = (& git -C $DevRepo rev-parse --verify HEAD 2>$null).Trim()
if ($LASTEXITCODE -ne 0 -or $consumerHeadCommit.Length -ne 40 -or $consumerHeadCommit -notmatch '^[0-9a-fA-F]{40}$') {
    throw "Could not resolve the 40-character 2D checkout HEAD for runtime qualification: $DevRepo"
}
if (-not [string]::Equals($consumerCommit, $consumerHeadCommit, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "2D runtime manifest source commit '$consumerCommit' does not match checkout HEAD '$consumerHeadCommit'. Rebuild the consumer from the current checkout before integration."
}

$entryAssembly = $runtimeBuildManifest.entryAssembly
if ($null -eq $entryAssembly) {
    throw "2D runtime manifest entryAssembly is missing: $runtimeBuildManifestPath"
}
$entryRelativePath = [string]$entryAssembly.relativePath
if ([string]::IsNullOrWhiteSpace($entryRelativePath) -or $entryRelativePath -ne [System.IO.Path]::GetFileName($entryRelativePath) -or $entryRelativePath -in @('.', '..')) {
    throw "2D runtime manifest entryAssembly.relativePath is missing or unsafe: $runtimeBuildManifestPath"
}

$entryAssemblyPath = Join-Path (Split-Path -Parent $runtimeBuildManifestPath) $entryRelativePath
if (-not (Test-Path -LiteralPath $entryAssemblyPath -PathType Leaf)) {
    throw "2D runtime manifest entry assembly is missing: $entryAssemblyPath"
}

$entryAssemblyFile = Get-Item -LiteralPath $entryAssemblyPath
$entryAssemblyHash = (Get-FileHash -LiteralPath $entryAssemblyPath -Algorithm SHA256).Hash
$entryAssemblyMatchesManifest = [int64]$entryAssemblyFile.Length -eq [int64]$entryAssembly.byteLength -and [string]::Equals($entryAssemblyHash, [string]$entryAssembly.sha256, [System.StringComparison]::OrdinalIgnoreCase)
if (-not $entryAssemblyMatchesManifest) {
    throw "2D runtime manifest entry assembly length/SHA-256 does not match the built file: $entryAssemblyPath"
}

[ordered]@{
    schemaVersion = '1.0'
    verified = $true
    checkout = [System.IO.Path]::GetFullPath($DevRepo)
    checkoutHeadCommit = $consumerHeadCommit
    runtimeManifest = [System.IO.Path]::GetFullPath($runtimeBuildManifestPath)
    manifestSourceCommit = $consumerCommit
    applicationVersion = $consumerVersion
    sourceState = $consumerSourceState
    entryAssembly = $entryRelativePath
    entryAssemblyByteLength = [int64]$entryAssemblyFile.Length
    entryAssemblySha256 = $entryAssemblyHash
} | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $consumerPreflightPath -Encoding UTF8
Write-Host "2D runtime preflight passed. Commit=$consumerHeadCommit; Entry=$entryRelativePath; SHA256=$entryAssemblyHash"

Push-Location $MachineRepo
try {
    Write-Host '== Build Machine Studio producer smoke =='
    & dotnet build $producerProject -c Release --nologo
    if ($LASTEXITCODE -ne 0) {
        throw 'Machine Studio producer smoke build failed.'
    }

    Write-Host '== Run Machine Studio producer process =='
    & dotnet run --project $producerProject -c Release --no-build -- `
        --publish-2d $exchangeRoot $manifestPath $MachineProjectPath $SourceImagePath $RecipePath `
        $consumerVersion $consumerCommit $consumerSourceState
    if ($LASTEXITCODE -ne 0) {
        throw 'Machine Studio producer process failed.'
    }
}
finally {
    Pop-Location
}

Push-Location $DevRepo
try {
    Write-Host '== Run Dev 2D consumer process =='
    & dotnet run --project $consumerProject -c Release --no-build -- `
        --integration-2d-published $exchangeRoot $manifestPath $consumerEvidenceRoot $runtimeBuildManifestPath
    if ($LASTEXITCODE -ne 0) {
        throw 'Dev 2D consumer process failed.'
    }
}
finally {
    Pop-Location
}

Write-Host "Consumer runtime identity=$consumerVersion/$consumerCommit/$consumerSourceState"
Write-Host "2D cross-repository smoke passed. Evidence=$runRoot"
