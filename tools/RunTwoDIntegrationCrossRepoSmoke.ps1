param(
    [string]$MachineRepo = 'C:\Git\OpenVisionLab-Machine-Studio',
    [string]$DevRepo = 'C:\Git\OpenVisionLab_Dev',
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

function Get-GitCommit([string]$Repository) {
    $commit = (& git -C $Repository rev-parse HEAD).Trim()
    if ($LASTEXITCODE -ne 0 -or $commit.Length -ne 40) {
        throw "Could not resolve Git commit for $Repository"
    }
    return $commit
}

function Get-GitSourceState([string]$Repository) {
    $status = (& git -C $Repository status --porcelain=v1 --untracked-files=normal) -join "`n"
    if ($LASTEXITCODE -ne 0) {
        throw "Could not resolve Git source state for $Repository"
    }
    if ([string]::IsNullOrWhiteSpace($status)) {
        return 'Clean'
    }
    return 'Dirty'
}

$runRoot = Join-Path $EvidenceRoot ("2d-cross-repo-{0}-{1}" -f (Get-Date -Format 'yyyyMMdd-HHmmss'), ([Guid]::NewGuid().ToString('N')))
$exchangeRoot = Join-Path $runRoot 'exchange'
$manifestPath = Join-Path $runRoot 'machine-producer-manifest.json'
$consumerEvidenceRoot = Join-Path $runRoot 'consumer'
$producerProject = Join-Path $MachineRepo 'tools\MachineIntegrationProducerSmoke\MachineIntegrationProducerSmoke.csproj'
$consumerProject = Join-Path $DevRepo 'tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj'
$consumerCommit = Get-GitCommit $DevRepo
$consumerWorktreeState = Get-GitSourceState $DevRepo
$consumerDeclaredState = 'Clean'

New-Item -ItemType Directory -Force -Path $runRoot | Out-Null
$env:TEMP = $runRoot
$env:TMP = $runRoot

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
        2.1.0 $consumerCommit $consumerDeclaredState
    if ($LASTEXITCODE -ne 0) {
        throw 'Machine Studio producer process failed.'
    }
}
finally {
    Pop-Location
}

Push-Location $DevRepo
try {
    Write-Host '== Build Dev 2D consumer smoke =='
    & dotnet build $consumerProject -c Release --nologo
    if ($LASTEXITCODE -ne 0) {
        throw 'Dev 2D consumer smoke build failed.'
    }

    Write-Host '== Run Dev 2D consumer process =='
    & dotnet run --project $consumerProject -c Release --no-build -- `
        --integration-2d-published $exchangeRoot $manifestPath $consumerEvidenceRoot
    if ($LASTEXITCODE -ne 0) {
        throw 'Dev 2D consumer process failed.'
    }
}
finally {
    Pop-Location
}

Write-Host "Consumer worktree observed=$consumerWorktreeState; declared contract source state=$consumerDeclaredState"
Write-Host "2D cross-repository smoke passed. Evidence=$runRoot"
