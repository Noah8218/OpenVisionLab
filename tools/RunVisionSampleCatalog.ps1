param(
    [string]$Configuration = "Debug",
    [string]$Platform = "Any CPU",
    [string]$CatalogPath = "docs\samples\OpenVisionLab.SampleCatalog.csv",
    [string]$OutputDir = "C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog",
    [string]$LibraryNoahSourceRoot = "",
    [switch]$FailOnExplore,
    [switch]$SkipRestore,
    [switch]$SkipRunnerBuild
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$catalogFullPath = if ([System.IO.Path]::IsPathRooted($CatalogPath)) { $CatalogPath } else { Join-Path $repoRoot $CatalogPath }
$runnerProject = Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj"
$runnerExeCandidates = @(
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\$Platform\$Configuration\net8.0-windows7.0\VisionRecipeRunnerSmoke.exe"),
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\$Platform\$Configuration\net8.0-windows\VisionRecipeRunnerSmoke.exe"),
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\$Configuration\net8.0-windows7.0\VisionRecipeRunnerSmoke.exe"),
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\$Configuration\net8.0-windows\VisionRecipeRunnerSmoke.exe"),
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\net8.0-windows7.0\VisionRecipeRunnerSmoke.exe"),
    (Join-Path $repoRoot "tools\VisionRecipeRunnerSmoke\bin\net8.0-windows\VisionRecipeRunnerSmoke.exe")
)
$msBuildCandidates = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
)
$msBuild = $msBuildCandidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
$reportPath = Join-Path $OutputDir "sample_catalog_report.md"
$summaryJsonPath = Join-Path $OutputDir "sample_catalog_summary.json"
$catalogStartedAt = Get-Date
$sampleCatalogDurationsSeconds = [ordered]@{
    RunnerRestoreSeconds = 0
    RunnerBuildSeconds = 0
    SampleExecutionSeconds = 0
}

function Invoke-Stage {
    param(
        [string]$Name,
        [scriptblock]$Action
    )

    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    try {
        & $Action
    }
    finally {
        $stopwatch.Stop()
        $script:sampleCatalogDurationsSeconds[$Name] = [Math]::Round($stopwatch.Elapsed.TotalSeconds, 3)
        Write-Host "== $Name duration: $($script:sampleCatalogDurationsSeconds[$Name]) sec"
    }
}

Add-Type -AssemblyName System.Drawing

function ConvertTo-NullableDouble {
    param([string]$Value)

    if ([string]::IsNullOrWhiteSpace($Value)) {
        return $null
    }

    $parsed = 0.0
    if ([double]::TryParse(
            $Value,
            [System.Globalization.NumberStyles]::Float,
            [System.Globalization.CultureInfo]::InvariantCulture,
            [ref]$parsed)) {
        return $parsed
    }

    throw "Expected numeric value but got '$Value'."
}

function ConvertTo-NullableInt {
    param([string]$Value)

    if ([string]::IsNullOrWhiteSpace($Value)) {
        return $null
    }

    $parsed = 0
    if ([int]::TryParse(
            $Value,
            [System.Globalization.NumberStyles]::Integer,
            [System.Globalization.CultureInfo]::InvariantCulture,
            [ref]$parsed)) {
        return $parsed
    }

    throw "Expected integer value but got '$Value'."
}

function Get-ImageSize {
    param([string]$Path)

    $image = $null
    try {
        $image = [System.Drawing.Image]::FromFile($Path)
        return [pscustomobject][ordered]@{
            Width = [int]$image.Width
            Height = [int]$image.Height
        }
    }
    finally {
        if ($null -ne $image) {
            $image.Dispose()
        }
    }
}

function Get-RunnerMetricMap {
    param([object[]]$RunnerOutput)

    $metrics = @{}
    foreach ($line in $RunnerOutput | ForEach-Object { $_.ToString() }) {
        if ([string]::IsNullOrWhiteSpace($line) -or $line -notmatch '=') {
            continue
        }

        foreach ($part in $line.Trim().Split(',')) {
            $tokens = $part.Trim().Split('=', 2)
            if ($tokens.Length -ne 2) {
                continue
            }

            $key = $tokens[0].Trim()
            if ([string]::IsNullOrWhiteSpace($key) -or $key -match '\s') {
                continue
            }

            $value = 0.0
            if ([double]::TryParse(
                    $tokens[1].Trim(),
                    [System.Globalization.NumberStyles]::Float,
                    [System.Globalization.CultureInfo]::InvariantCulture,
                    [ref]$value)) {
                $metrics[$key] = $value
            }
        }
    }

    return $metrics
}

function Get-OptionalCsvValue {
    param(
        [object]$Row,
        [string]$Name
    )

    $property = $Row.PSObject.Properties[$Name]
    if ($null -eq $property) {
        return $null
    }

    return [string]$property.Value
}

function Split-ExpectedMetricParts {
    param([string]$Value)

    if ([string]::IsNullOrWhiteSpace($Value)) {
        return @()
    }

    return @($Value.Split(';') | ForEach-Object { $_.Trim() })
}

function Resolve-ExpectedMetricPart {
    param(
        [string[]]$Values,
        [int]$Index
    )

    if ($null -eq $Values -or $Values.Count -eq 0) {
        return ""
    }

    if ($Index -ge 0 -and $Index -lt $Values.Count) {
        return $Values[$Index]
    }

    if ($Values.Count -eq 1) {
        return $Values[0]
    }

    return ""
}

function Get-ExpectedMetricChecks {
    param([object]$Row)

    $names = @(Split-ExpectedMetricParts (Get-OptionalCsvValue $Row "ExpectedMetricName"))
    $minimums = @(Split-ExpectedMetricParts (Get-OptionalCsvValue $Row "ExpectedMetricMinimum"))
    $maximums = @(Split-ExpectedMetricParts (Get-OptionalCsvValue $Row "ExpectedMetricMaximum"))
    $checks = @()

    for ($i = 0; $i -lt $names.Count; $i++) {
        $name = $names[$i]
        if ([string]::IsNullOrWhiteSpace($name)) {
            continue
        }

        $checks += [pscustomobject][ordered]@{
            Name = $name
            Minimum = Resolve-ExpectedMetricPart $minimums $i
            Maximum = Resolve-ExpectedMetricPart $maximums $i
        }
    }

    return @($checks)
}

function Add-CatalogCategoryStat {
    param(
        [hashtable]$Stats,
        [string]$Category,
        [string]$Mode,
        [string]$Status
    )

    $categoryKey = if ([string]::IsNullOrWhiteSpace($Category)) { "(uncategorized)" } else { $Category.Trim() }
    if (-not $Stats.ContainsKey($categoryKey)) {
        $Stats[$categoryKey] = [ordered]@{
            Total = 0
            Required = 0
            Explore = 0
            ExpectedFailure = 0
            OK = 0
            NG = 0
        }
    }

    $bucket = $Stats[$categoryKey]
    $bucket["Total"]++

    if ([string]::Equals($Mode, "Required", [StringComparison]::OrdinalIgnoreCase)) {
        $bucket["Required"]++
    }
    elseif ([string]::Equals($Mode, "Explore", [StringComparison]::OrdinalIgnoreCase)) {
        $bucket["Explore"]++
    }
    elseif ([string]::Equals($Mode, "ExpectedFailure", [StringComparison]::OrdinalIgnoreCase)) {
        $bucket["ExpectedFailure"]++
    }

    if ([string]::Equals($Status, "OK", [StringComparison]::OrdinalIgnoreCase)) {
        $bucket["OK"]++
    }
    else {
        $bucket["NG"]++
    }
}

function Add-ArtifactFailureIfMissing {
    param(
        [System.Collections.Generic.List[string]]$Failures,
        [string]$Path,
        [string]$Label
    )

    if ([string]::IsNullOrWhiteSpace($Path)) {
        $Failures.Add("$Label path is empty.") | Out-Null
        return
    }

    $item = Get-Item -LiteralPath $Path -ErrorAction SilentlyContinue
    if ($null -eq $item) {
        $Failures.Add("$Label was not created: $Path") | Out-Null
        return
    }

    if ($item.Length -le 0) {
        $Failures.Add("$Label is empty: $Path") | Out-Null
    }
}

function Get-SampleTopFolder {
    param(
        [string]$SampleRoot,
        [string]$ImagePath
    )

    $rootPath = [System.IO.Path]::GetFullPath($SampleRoot).TrimEnd('\', '/') + "\"
    $fullImagePath = [System.IO.Path]::GetFullPath($ImagePath)
    if ($fullImagePath.StartsWith($rootPath, [StringComparison]::OrdinalIgnoreCase)) {
        $relativePath = $fullImagePath.Substring($rootPath.Length).Replace('/', '\')
    }
    else {
        $relativePath = [System.IO.Path]::GetFileName($fullImagePath)
    }

    $separatorIndex = $relativePath.IndexOf('\')
    if ($separatorIndex -lt 0) {
        return "."
    }

    return $relativePath.Substring(0, $separatorIndex)
}

function Get-CatalogTopFolder {
    param([string]$ImagePath)

    if ([string]::IsNullOrWhiteSpace($ImagePath)) {
        return ""
    }

    $normalizedPath = $ImagePath.Replace('/', '\')
    if (-not $normalizedPath.StartsWith("Sample\", [StringComparison]::OrdinalIgnoreCase)) {
        return ""
    }

    $relativePath = $normalizedPath.Substring("Sample\".Length)
    $separatorIndex = $relativePath.IndexOf('\')
    if ($separatorIndex -lt 0) {
        return "."
    }

    return $relativePath.Substring(0, $separatorIndex)
}

if (-not (Test-Path -LiteralPath $catalogFullPath)) {
    throw "Sample catalog was not found: $catalogFullPath"
}

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

Write-Host "== Build VisionRecipeRunnerSmoke =="
$runnerBuildProperties = @(
    "/p:Configuration=$Configuration",
    "/p:Platform=$Platform",
    "/p:WpgCustomBuildEnabled=false"
)

if (-not $SkipRunnerBuild) {
    Write-Host "== Restore VisionRecipeRunnerSmoke =="
    if ($SkipRestore) {
        Write-Host "Restore skipped by -SkipRestore."
        $sampleCatalogDurationsSeconds["RunnerRestoreSeconds"] = 0
    }
    else {
        Invoke-Stage -Name "RunnerRestoreSeconds" -Action {
            & dotnet restore $runnerProject @runnerBuildProperties
            if ($LASTEXITCODE -ne 0) {
                throw "VisionRecipeRunnerSmoke restore failed."
            }
        }
    }

    if (-not [string]::IsNullOrWhiteSpace($msBuild)) {
        Invoke-Stage -Name "RunnerBuildSeconds" -Action {
            & $msBuild $runnerProject /t:Build @runnerBuildProperties /m /p:RestorePackages=false /p:Restore=false /clp:ErrorsOnly /v:minimal
            if ($LASTEXITCODE -ne 0) {
                throw "VisionRecipeRunnerSmoke build failed."
            }
        }
    }
    else {
        $dotnetBuildArguments = @(
            "build",
            $runnerProject,
            "-c",
            $Configuration,
            "/p:Platform=$Platform",
            "/p:WpgCustomBuildEnabled=false",
            "--no-restore",
            "--maxcpucount"
        )
        Invoke-Stage -Name "RunnerBuildSeconds" -Action {
            & dotnet @dotnetBuildArguments
            if ($LASTEXITCODE -ne 0) {
                throw "VisionRecipeRunnerSmoke build failed."
            }
        }
    }
}

function Resolve-RunnerExecutable {
    param(
        [string[]]$Candidates,
        [string]$ProjectName,
        [string]$ProjectPath
    )

    $directHit = $Candidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
    if (-not [string]::IsNullOrWhiteSpace($directHit)) {
        return $directHit
    }

    $projectDir = Split-Path -Parent $ProjectPath
    $fallback = Get-ChildItem -LiteralPath $projectDir -Filter $ProjectName -Recurse -File -ErrorAction SilentlyContinue |
        Sort-Object FullName |
        Select-Object -First 1

    if ($null -ne $fallback) {
        return $fallback.FullName
    }

    return ""
}

$runnerExe = Resolve-RunnerExecutable -Candidates $runnerExeCandidates -ProjectName "VisionRecipeRunnerSmoke.exe" -ProjectPath $runnerProject
if ([string]::IsNullOrWhiteSpace($runnerExe)) {
    throw "VisionRecipeRunnerSmoke executable was not found. Checked: $($runnerExeCandidates -join '; ')"
}

Write-Host "Runner: $runnerExe"

$rows = Import-Csv -LiteralPath $catalogFullPath
$runRows = @($rows | Where-Object { -not [string]::IsNullOrWhiteSpace($_.BaselinePipeline) })
$totalRows = 0
$okRows = 0
$ngRows = 0
$requiredRows = 0
$exploreRows = 0
$expectedFailureRows = 0
$categoryStats = @{}
$sampleFolderCoverage = @()
$uncoveredSampleFolders = @()

$sampleRoot = Join-Path $repoRoot "Sample"
if (Test-Path -LiteralPath $sampleRoot) {
    $imageExtensions = @(".bmp", ".jpg", ".jpeg", ".png", ".tif", ".tiff")
    $catalogTopFolders = @($runRows | ForEach-Object { Get-CatalogTopFolder $_.ImagePath } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    $sampleFolderCoverage = @(Get-ChildItem -LiteralPath $sampleRoot -File -Recurse |
        Where-Object { $imageExtensions -contains $_.Extension.ToLowerInvariant() } |
        Group-Object { Get-SampleTopFolder $sampleRoot $_.FullName } |
        Sort-Object Name |
        ForEach-Object {
            $folder = $_.Name
            $catalogRefs = @($catalogTopFolders | Where-Object { [string]::Equals($_, $folder, [StringComparison]::OrdinalIgnoreCase) }).Count
            [pscustomobject][ordered]@{
                Folder = $folder
                ImageCount = $_.Count
                CatalogRefs = $catalogRefs
                Status = $(if ($catalogRefs -gt 0) { "Covered" } else { "Backlog" })
            }
        })
    $uncoveredSampleFolders = @($sampleFolderCoverage | Where-Object { $_.CatalogRefs -eq 0 -and $_.Folder -ne "." })
}

$report = New-Object System.Collections.Generic.List[string]
$report.Add("# OpenVisionLab Sample Catalog Smoke") | Out-Null
$report.Add("") | Out-Null
$report.Add("- Time: $($catalogStartedAt.ToString("yyyy-MM-dd HH:mm:ss"))") | Out-Null
$report.Add("- Build: $Configuration / $Platform") | Out-Null
$report.Add("- Runner: ``$runnerExe``") | Out-Null
$report.Add("- Catalog: ``$catalogFullPath``") | Out-Null
$report.Add("- Output: ``$OutputDir``") | Out-Null
$report.Add("") | Out-Null
$report.Add("| Sample | Mode | Status | Input Image | Pipeline | Result | Expected Metric | Result Image | Overlay Image | Raw Log |") | Out-Null
$report.Add("| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |") | Out-Null

$failures = New-Object System.Collections.Generic.List[string]
$resultRows = New-Object System.Collections.Generic.List[object]
$sampleExecutionStopwatch = [System.Diagnostics.Stopwatch]::StartNew()

foreach ($row in $runRows) {
    $totalRows++
    $isRequired = [string]::Equals($row.ValidationMode, "Required", [StringComparison]::OrdinalIgnoreCase)
    $isExplore = [string]::Equals($row.ValidationMode, "Explore", [StringComparison]::OrdinalIgnoreCase)
    $isExpectedFailure = [string]::Equals($row.ValidationMode, "ExpectedFailure", [StringComparison]::OrdinalIgnoreCase)
    if ($isRequired) {
        $requiredRows++
    }
    elseif ($isExplore) {
        $exploreRows++
    }
    elseif ($isExpectedFailure) {
        $expectedFailureRows++
    }

    $imagePath = Join-Path $repoRoot $row.ImagePath
    $pipelinePath = Join-Path $repoRoot $row.BaselinePipeline
    $safeName = ($row.SampleName -replace '[^A-Za-z0-9_.-]', '_')
    $overlayImagePath = Join-Path $OutputDir "$safeName.png"
    $resultImagePath = Join-Path $OutputDir "$safeName.result.png"
    $rawLogPath = Join-Path $OutputDir "$safeName.log"
    $metadataFailures = New-Object System.Collections.Generic.List[string]
    $actualImageWidth = 0
    $actualImageHeight = 0
    $inputImageText = "-"

    if (-not (Test-Path -LiteralPath $imagePath)) {
        $metadataFailures.Add("Input image was not found: $imagePath") | Out-Null
    }
    else {
        try {
            $imageSize = Get-ImageSize $imagePath
            $actualImageWidth = [int]$imageSize.Width
            $actualImageHeight = [int]$imageSize.Height
            $inputImageText = "$actualImageWidth x $actualImageHeight"

            $expectedWidth = ConvertTo-NullableInt (Get-OptionalCsvValue $row "Width")
            $expectedHeight = ConvertTo-NullableInt (Get-OptionalCsvValue $row "Height")
            if ($null -ne $expectedWidth -and $null -ne $expectedHeight) {
                $inputImageText = "$inputImageText / expected $expectedWidth x $expectedHeight"
                if ($actualImageWidth -ne $expectedWidth -or $actualImageHeight -ne $expectedHeight) {
                    $metadataFailures.Add("Input image size $actualImageWidth x $actualImageHeight does not match catalog $expectedWidth x $expectedHeight.") | Out-Null
                }
            }
        }
        catch {
            $metadataFailures.Add("Input image could not be inspected: $($_.Exception.Message)") | Out-Null
        }
    }

    if (-not (Test-Path -LiteralPath $pipelinePath)) {
        $metadataFailures.Add("Pipeline XML was not found: $pipelinePath") | Out-Null
    }

    Write-Host "== $($row.SampleName) =="
    $previousErrorActionPreference = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    try {
        $runnerOutput = @(& $runnerExe $imagePath $pipelinePath $resultImagePath "--all-overlay-image" $overlayImagePath 2>&1 | ForEach-Object { $_.ToString() })
    }
    finally {
        $ErrorActionPreference = $previousErrorActionPreference
    }

    $exitCode = $LASTEXITCODE
    $runnerOutput | Set-Content -LiteralPath $rawLogPath -Encoding UTF8
    $runnerOutput | ForEach-Object { Write-Host $_ }

    $pipelineName = Split-Path -Leaf $row.BaselinePipeline
    $resultText = ($runnerOutput | Where-Object { $_ -like "Success=*" } | Select-Object -First 1)
    if ([string]::IsNullOrWhiteSpace($resultText)) {
        $resultText = "Exit=$exitCode"
    }

    $metricFailures = New-Object System.Collections.Generic.List[string]
    $expectedMetricText = "-"
    $expectedMetricChecks = @(Get-ExpectedMetricChecks $row)
    if ($expectedMetricChecks.Count -gt 0) {
        $metrics = Get-RunnerMetricMap $runnerOutput
        $expectedMetricParts = New-Object System.Collections.Generic.List[string]
        foreach ($metricCheck in $expectedMetricChecks) {
            $expectedMetricName = $metricCheck.Name
            if (-not $metrics.ContainsKey($expectedMetricName)) {
                $metricFailures.Add("Metric '$expectedMetricName' was not found.") | Out-Null
                $expectedMetricParts.Add("$expectedMetricName=missing") | Out-Null
                continue
            }

            $actualMetricValue = [double]$metrics[$expectedMetricName]
            $minimum = ConvertTo-NullableDouble $metricCheck.Minimum
            $maximum = ConvertTo-NullableDouble $metricCheck.Maximum
            $actualText = $actualMetricValue.ToString("0.###", [System.Globalization.CultureInfo]::InvariantCulture)
            $rangeText = ""

            if ($null -ne $minimum) {
                $rangeText = "$rangeText min=$($minimum.ToString("0.###", [System.Globalization.CultureInfo]::InvariantCulture))"
                if ($actualMetricValue -lt $minimum) {
                    $metricFailures.Add("$expectedMetricName=$actualText is below expected minimum $minimum.") | Out-Null
                }
            }

            if ($null -ne $maximum) {
                $rangeText = "$rangeText max=$($maximum.ToString("0.###", [System.Globalization.CultureInfo]::InvariantCulture))"
                if ($actualMetricValue -gt $maximum) {
                    $metricFailures.Add("$expectedMetricName=$actualText is above expected maximum $maximum.") | Out-Null
                }
            }

            $expectedMetricParts.Add("$expectedMetricName=$actualText$rangeText") | Out-Null
        }

        $expectedMetricText = [string]::Join("; ", @($expectedMetricParts.ToArray()))
    }

    $resultFailureMessages = New-Object System.Collections.Generic.List[string]
    $artifactFailures = New-Object System.Collections.Generic.List[string]
    if ($isExpectedFailure) {
        if ($exitCode -eq 0) {
            $resultFailureMessages.Add("Expected failure did not occur.") | Out-Null
        }
    }
    elseif ($exitCode -ne 0) {
        $resultFailureMessages.Add("Runner exit code $exitCode.") | Out-Null
    }

    foreach ($metricFailure in $metricFailures) {
        $resultFailureMessages.Add($metricFailure) | Out-Null
    }

    foreach ($metadataFailure in $metadataFailures) {
        $resultFailureMessages.Add($metadataFailure) | Out-Null
    }

    Add-ArtifactFailureIfMissing -Failures $artifactFailures -Path $resultImagePath -Label "Result image"
    Add-ArtifactFailureIfMissing -Failures $artifactFailures -Path $overlayImagePath -Label "Overlay image"
    Add-ArtifactFailureIfMissing -Failures $artifactFailures -Path $rawLogPath -Label "Raw log"
    foreach ($artifactFailure in $artifactFailures) {
        $resultFailureMessages.Add($artifactFailure) | Out-Null
    }

    $status = if ($resultFailureMessages.Count -eq 0) { "OK" } else { "NG" }
    if ($status -eq "OK") {
        $okRows++
    }
    else {
        $ngRows++
    }
    Add-CatalogCategoryStat -Stats $categoryStats -Category $row.Category -Mode $row.ValidationMode -Status $status

    $resultImageLink = if (Test-Path -LiteralPath $resultImagePath) { "[$safeName.result.png]($safeName.result.png)" } else { "-" }
    $overlayImageLink = if (Test-Path -LiteralPath $overlayImagePath) { "[$safeName.png]($safeName.png)" } else { "-" }
    $logLink = if (Test-Path -LiteralPath $rawLogPath) { "[$safeName.log]($safeName.log)" } else { "-" }
    $report.Add("| $($row.SampleName) | $($row.ValidationMode) | $status | $inputImageText | $pipelineName | $resultText | $expectedMetricText | $resultImageLink | $overlayImageLink | $logLink |") | Out-Null
    $resultRows.Add([pscustomobject][ordered]@{
        SampleName = $row.SampleName
        Category = $row.Category
        Mode = $row.ValidationMode
        Status = $status
        ExitCode = $exitCode
        InputImageWidth = $actualImageWidth
        InputImageHeight = $actualImageHeight
        MetadataStatus = if ($metadataFailures.Count -eq 0) { "OK" } else { "NG" }
        MetadataFailureMessages = @($metadataFailures.ToArray())
        Pipeline = $pipelineName
        Result = $resultText
        ExpectedMetric = [string]::Join("; ", @($expectedMetricChecks | ForEach-Object { $_.Name }))
        ExpectedMetricText = $expectedMetricText
        ArtifactStatus = if ($artifactFailures.Count -eq 0) { "OK" } else { "NG" }
        ArtifactFailureMessages = @($artifactFailures.ToArray())
        FailureMessages = @($resultFailureMessages.ToArray())
        OverlayImagePath = if (Test-Path -LiteralPath $overlayImagePath) { $overlayImagePath } else { "" }
        ResultImagePath = if (Test-Path -LiteralPath $resultImagePath) { $resultImagePath } else { "" }
        LogPath = $rawLogPath
    }) | Out-Null

    if ($isExpectedFailure -and $exitCode -eq 0) {
        $failures.Add("$($row.SampleName) was expected to fail, but runner returned OK. See $rawLogPath") | Out-Null
    }

    if ($exitCode -ne 0 -and -not $isExpectedFailure -and ($isRequired -or ($FailOnExplore -and $isExplore))) {
        $failures.Add("$($row.SampleName) failed with exit code $exitCode. See $rawLogPath") | Out-Null
    }

    if ($metricFailures.Count -gt 0 -and ($isRequired -or $isExpectedFailure -or ($FailOnExplore -and $isExplore))) {
        foreach ($metricFailure in $metricFailures) {
            $failures.Add("$($row.SampleName): $metricFailure See $rawLogPath") | Out-Null
        }
    }

    if ($artifactFailures.Count -gt 0 -and ($isRequired -or $isExpectedFailure -or ($FailOnExplore -and $isExplore))) {
        foreach ($artifactFailure in $artifactFailures) {
            $failures.Add("$($row.SampleName): $artifactFailure See $rawLogPath") | Out-Null
        }
    }

    if ($metadataFailures.Count -gt 0 -and ($isRequired -or $isExpectedFailure -or ($FailOnExplore -and $isExplore))) {
        foreach ($metadataFailure in $metadataFailures) {
            $failures.Add("$($row.SampleName): $metadataFailure See $rawLogPath") | Out-Null
        }
    }
}
$sampleExecutionStopwatch.Stop()
$sampleCatalogDurationsSeconds.SampleExecutionSeconds = [Math]::Round($sampleExecutionStopwatch.Elapsed.TotalSeconds, 3)

$summaryLines = @(
    "- Runnable rows: $totalRows",
    "- Required rows: $requiredRows",
    "- Explore rows: $exploreRows",
    "- Expected-failure rows: $expectedFailureRows",
    "- OK rows: $okRows",
    "- NG rows: $ngRows",
    ""
)
for ($i = $summaryLines.Count - 1; $i -ge 0; $i--) {
    $report.Insert(6, $summaryLines[$i])
}

$report.Add("") | Out-Null
$report.Add("## Category Summary") | Out-Null
$report.Add("") | Out-Null
$report.Add("| Category | Total | Required | Explore | Expected Failure | OK | NG |") | Out-Null
$report.Add("| --- | ---: | ---: | ---: | ---: | ---: | ---: |") | Out-Null
foreach ($categoryName in ($categoryStats.Keys | Sort-Object)) {
    $bucket = $categoryStats[$categoryName]
    $report.Add("| $categoryName | $($bucket["Total"]) | $($bucket["Required"]) | $($bucket["Explore"]) | $($bucket["ExpectedFailure"]) | $($bucket["OK"]) | $($bucket["NG"]) |") | Out-Null
}

$report.Add("") | Out-Null
$report.Add("## Sample Folder Coverage") | Out-Null
$report.Add("") | Out-Null
$report.Add("| Folder | Image Count | Catalog Refs | Status |") | Out-Null
$report.Add("| --- | ---: | ---: | --- |") | Out-Null
foreach ($folder in $sampleFolderCoverage) {
    $report.Add("| $($folder.Folder) | $($folder.ImageCount) | $($folder.CatalogRefs) | $($folder.Status) |") | Out-Null
}

if ($uncoveredSampleFolders.Count -gt 0) {
    $report.Add("") | Out-Null
    $report.Add("> Backlog folders are visible for planning. They do not fail this smoke unless a representative sample is promoted to Required/Explore in the catalog.") | Out-Null
}

$report.Add("") | Out-Null
$report.Add("## Catalog Notes") | Out-Null
$report.Add("") | Out-Null
foreach ($row in $rows) {
    $report.Add("- **$($row.SampleName)**: $($row.Goal) $($row.Notes)") | Out-Null
}

$report | Set-Content -LiteralPath $reportPath -Encoding UTF8
$categorySummary = foreach ($categoryName in ($categoryStats.Keys | Sort-Object)) {
    $bucket = $categoryStats[$categoryName]
    [ordered]@{
        Category = $categoryName
        Total = $bucket["Total"]
        Required = $bucket["Required"]
        Explore = $bucket["Explore"]
        OK = $bucket["OK"]
        NG = $bucket["NG"]
    }
}

$failedSamples = foreach ($resultRow in $resultRows) {
    if (-not [string]::Equals($resultRow.Status, "NG", [StringComparison]::OrdinalIgnoreCase)) {
        continue
    }

    [ordered]@{
        SampleName = $resultRow.SampleName
        Category = $resultRow.Category
        Mode = $resultRow.Mode
        Pipeline = $resultRow.Pipeline
        FailureMessages = @($resultRow.FailureMessages)
        LogPath = $resultRow.LogPath
    }
}

$artifactIssues = foreach ($resultRow in $resultRows) {
    if ([string]::Equals($resultRow.ArtifactStatus, "OK", [StringComparison]::OrdinalIgnoreCase)) {
        continue
    }

    [ordered]@{
        SampleName = $resultRow.SampleName
        Category = $resultRow.Category
        Mode = $resultRow.Mode
        Pipeline = $resultRow.Pipeline
        FailureMessages = @($resultRow.ArtifactFailureMessages)
        LogPath = $resultRow.LogPath
    }
}

$metadataIssues = foreach ($resultRow in $resultRows) {
    if ([string]::Equals($resultRow.MetadataStatus, "OK", [StringComparison]::OrdinalIgnoreCase)) {
        continue
    }

    [ordered]@{
        SampleName = $resultRow.SampleName
        Category = $resultRow.Category
        Mode = $resultRow.Mode
        Pipeline = $resultRow.Pipeline
        FailureMessages = @($resultRow.MetadataFailureMessages)
        LogPath = $resultRow.LogPath
    }
}

$artifactIssueCount = @($artifactIssues).Count
$metadataIssueCount = @($metadataIssues).Count
$gateStatus = if ($totalRows -gt 0 -and $okRows -eq $totalRows -and $ngRows -eq 0 -and $artifactIssueCount -eq 0 -and $metadataIssueCount -eq 0) { "OK" } else { "NG" }
$gateMessage = if ($gateStatus -eq "OK") {
    "All runnable sample rows passed."
}
else {
    "One or more runnable sample rows failed. Review FailedSamples, ArtifactIssues, MetadataIssues, and per-sample logs."
}

$summaryPayload = [ordered]@{
    Time = (Get-Date -Format "yyyy-MM-dd HH:mm:ss")
    StartedAt = $catalogStartedAt.ToString("yyyy-MM-dd HH:mm:ss")
    DurationSeconds = [Math]::Round(((Get-Date) - $catalogStartedAt).TotalSeconds, 3)
    Configuration = $Configuration
    Platform = $Platform
    CatalogPath = $catalogFullPath
    OutputDir = $OutputDir
    RunnerPath = $runnerExe
    RunnerRestoreDurationSeconds = $sampleCatalogDurationsSeconds.RunnerRestoreSeconds
    RunnerBuildDurationSeconds = $sampleCatalogDurationsSeconds.RunnerBuildSeconds
    SampleExecutionDurationSeconds = $sampleCatalogDurationsSeconds.SampleExecutionSeconds
    GateStatus = $gateStatus
    GateMessage = $gateMessage
    RunnableRows = $totalRows
    RequiredRows = $requiredRows
    ExploreRows = $exploreRows
    ExpectedFailureRows = $expectedFailureRows
    OKRows = $okRows
    NGRows = $ngRows
    Categories = @($categorySummary)
    FailedSamples = @($failedSamples)
    ArtifactIssueCount = $artifactIssueCount
    ArtifactIssues = @($artifactIssues)
    MetadataIssueCount = $metadataIssueCount
    MetadataIssues = @($metadataIssues)
    SampleFolderCoverage = @($sampleFolderCoverage)
    UncoveredSampleFolders = @($uncoveredSampleFolders)
    Results = @($resultRows.ToArray())
}
$summaryPayload | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $summaryJsonPath -Encoding UTF8
Write-Host "Sample catalog report saved to $reportPath"
Write-Host "Sample catalog summary saved to $summaryJsonPath"

if ($failures.Count -gt 0) {
    foreach ($failure in $failures) {
        Write-Error $failure
    }

    throw "Sample catalog smoke failed. See $reportPath"
}
