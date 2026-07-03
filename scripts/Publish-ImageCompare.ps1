param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",

    [string]$Runtime = "win-x64",

    [switch]$SelfContained,

    [switch]$IncludeSymbols,

    [switch]$NoClean,

    [switch]$SmokeTest
)

$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = (Resolve-Path (Join-Path $scriptRoot "..")).Path
$projectPath = Join-Path $repoRoot "tools\OpenVisionLab.ImageCompare\OpenVisionLab.ImageCompare.csproj"
$distRoot = Join-Path $repoRoot "dist"
$outputDir = Join-Path $distRoot "OpenVisionLab.ImageCompare"

if (-not (Test-Path -LiteralPath $projectPath)) {
    throw "ImageCompare project was not found: $projectPath"
}

if (-not (Test-Path -LiteralPath $distRoot)) {
    New-Item -ItemType Directory -Path $distRoot | Out-Null
}

if (-not $NoClean -and (Test-Path -LiteralPath $outputDir)) {
    $resolvedOutput = [System.IO.Path]::GetFullPath($outputDir)
    $resolvedDist = [System.IO.Path]::GetFullPath($distRoot)
    if (-not $resolvedOutput.StartsWith($resolvedDist, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to clean unexpected path: $resolvedOutput"
    }

    Remove-Item -LiteralPath $resolvedOutput -Recurse -Force
}

$selfContainedValue = if ($SelfContained) { "true" } else { "false" }
$includeSymbolsValue = if ($IncludeSymbols) { "true" } else { "false" }

$publishArgs = @(
    "publish",
    $projectPath,
    "-c", $Configuration,
    "-r", $Runtime,
    "--self-contained", $selfContainedValue,
    "-o", $outputDir,
    "/p:ImageCompareIncludeSymbols=$includeSymbolsValue"
)

& dotnet @publishArgs
if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed. ExitCode=$LASTEXITCODE"
}

$unexpectedPatterns = @(
    "^OpenVisionLab\.exe$",
    "^OpenVisionLab\.dll$",
    "^Lib\.Common\.dll$",
    "^log4net\.dll$",
    "^System\.IO\.Ports\.dll$",
    "^cvextern\.dll$",
    "^opencv_ffmpeg400_64\.dll$",
    "^System\.Windows\.Controls\.WpfPropertyGrid\.dll$"
)

$unexpectedFiles = Get-ChildItem -LiteralPath $outputDir -File | Where-Object {
    $fileName = $_.Name
    $unexpectedPatterns | Where-Object { $fileName -match $_ }
}

if ($unexpectedFiles) {
    $names = ($unexpectedFiles | Select-Object -ExpandProperty Name) -join ", "
    throw "Unexpected files remain in ImageCompare output: $names"
}

if (-not $IncludeSymbols) {
    $symbols = Get-ChildItem -LiteralPath $outputDir -File -Filter "*.pdb" -ErrorAction SilentlyContinue
    if ($symbols) {
        $names = ($symbols | Select-Object -ExpandProperty Name) -join ", "
        throw "PDB files remain in ImageCompare output: $names"
    }
}

$files = Get-ChildItem -LiteralPath $outputDir -File
$sizeBytes = ($files | Measure-Object Length -Sum).Sum
$sizeMb = [Math]::Round($sizeBytes / 1MB, 2)

Write-Host "ImageCompare publish completed."
Write-Host "Output : $outputDir"
Write-Host "Files  : $($files.Count)"
Write-Host "Size   : $sizeMb MB"

if ($SmokeTest) {
    $exePath = Join-Path $outputDir "OpenVisionLab.ImageCompare.exe"
    $samplePath = Join-Path $repoRoot "Sample\Contour.jpg"

    if (-not (Test-Path -LiteralPath $exePath)) {
        throw "Smoke test failed. EXE was not found: $exePath"
    }

    $processArgs = @()
    if (Test-Path -LiteralPath $samplePath) {
        $processArgs = @($samplePath, $samplePath)
    }

    $process = Start-Process -FilePath $exePath -ArgumentList $processArgs -WindowStyle Hidden -PassThru
    Start-Sleep -Seconds 4

    if ($process.HasExited) {
        throw "Smoke test failed. ImageCompare exited early. ExitCode=$($process.ExitCode)"
    }

    Stop-Process -Id $process.Id -Force
    Write-Host "Smoke test passed."
}
