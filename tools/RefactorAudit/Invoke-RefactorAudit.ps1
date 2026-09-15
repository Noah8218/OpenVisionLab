[CmdletBinding()]
param(
    [string]$RepositoryRoot = '',
    [string]$OutputDirectory = 'D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-audit-current',
    [int]$LargeFileThreshold = 1000,
    [switch]$AllowNonDOutput,
    [switch]$Verify
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Resolve-FullPath {
    param([string]$Path)

    return [System.IO.Path]::GetFullPath($Path)
}

if ([string]::IsNullOrWhiteSpace($RepositoryRoot)) {
    $RepositoryRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
}

$repositoryPath = Resolve-FullPath $RepositoryRoot
if (-not (Test-Path -LiteralPath $repositoryPath -PathType Container)) {
    throw "Repository root was not found: $repositoryPath"
}

$repositoryPrefix = $repositoryPath.TrimEnd('\', '/') + [System.IO.Path]::DirectorySeparatorChar
$solutionPath = Join-Path $repositoryPath 'OpenVisionLab.sln'
if (-not (Test-Path -LiteralPath $solutionPath -PathType Leaf)) {
    throw "OpenVisionLab.sln was not found under repository root: $repositoryPath"
}

if ([System.IO.Path]::IsPathRooted($OutputDirectory)) {
    $outputPath = Resolve-FullPath $OutputDirectory
}
else {
    $outputPath = Resolve-FullPath (Join-Path $repositoryPath $OutputDirectory)
}

if (-not $AllowNonDOutput -and
    -not [string]::Equals([System.IO.Path]::GetPathRoot($outputPath), 'D:\', [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "Audit evidence must be written to D:. Use -AllowNonDOutput only for an explicitly recorded fallback: $outputPath"
}

New-Item -ItemType Directory -Force -Path $outputPath | Out-Null

function Get-RepoRelativePath {
    param([string]$Path)

    $fullPath = Resolve-FullPath $Path
    if (-not $fullPath.StartsWith($repositoryPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Path is outside the repository: $fullPath"
    }

    return $fullPath.Substring($repositoryPrefix.Length).Replace('\', '/')
}

function Get-LineCount {
    param([string]$Path)

    return [System.IO.File]::ReadAllLines($Path).Length
}

function Get-SourceFiles {
    $sourceDirectories = @(
        (Join-Path $repositoryPath 'src'),
        (Join-Path $repositoryPath 'tools')
    )
    $files = [System.Collections.Generic.List[System.IO.FileInfo]]::new()
    foreach ($sourceDirectory in $sourceDirectories) {
        if (-not (Test-Path -LiteralPath $sourceDirectory -PathType Container)) {
            continue
        }

        foreach ($file in @(Get-ChildItem -LiteralPath $sourceDirectory -Recurse -File)) {
            if ($file.FullName -match '\\(bin|obj)\\') {
                continue
            }

            if ($file.Extension -in @('.cs', '.xaml')) {
                $files.Add($file)
            }
        }
    }

    return @($files)
}

function Get-TextSignals {
    param(
        [string]$Text,
        [string]$Pattern
    )

    $signals = [System.Collections.Generic.List[string]]::new()
    foreach ($match in [System.Text.RegularExpressions.Regex]::Matches($Text, $Pattern)) {
        $signal = $match.Value.Trim()
        if (-not [string]::IsNullOrWhiteSpace($signal) -and -not $signals.Contains($signal)) {
            $signals.Add($signal)
        }
    }

    return @($signals | Sort-Object)
}

function Write-CsvEvidence {
    param(
        [string]$Path,
        [object[]]$Rows,
        [string[]]$Headers
    )

    if ($Rows.Count -eq 0) {
        Set-Content -LiteralPath $Path -Value (($Headers -join ',')) -Encoding UTF8
        return
    }

    $Rows | Export-Csv -LiteralPath $Path -NoTypeInformation -Encoding UTF8
}

$sourceFiles = @(Get-SourceFiles)
$csharpFiles = @($sourceFiles | Where-Object Extension -eq '.cs')
$xamlFiles = @($sourceFiles | Where-Object Extension -eq '.xaml')

$csharpRows = @(
    foreach ($file in $csharpFiles) {
        [pscustomobject]@{
            Path = Get-RepoRelativePath $file.FullName
            Lines = Get-LineCount $file.FullName
            Bytes = $file.Length
        }
    }
)
$xamlRows = @(
    foreach ($file in $xamlFiles) {
        [pscustomobject]@{
            Path = Get-RepoRelativePath $file.FullName
            Lines = Get-LineCount $file.FullName
            Bytes = $file.Length
        }
    }
)
$allRows = @($csharpRows + $xamlRows)

$partialDeclarationPattern = '(?m)^\s*(?:(?:public|internal|protected|private|file|abstract|sealed|static|unsafe|readonly|ref)\s+)*partial\s+(?:class|struct|record|interface)\b'
$partialTextPattern = '\bpartial\s+(?:class|struct|record|interface)\b'
$partialDeclarations = 0
$partialTextMatches = 0
$partialTextRows = [System.Collections.Generic.List[object]]::new()
$typeDeclarations = 0
foreach ($file in $csharpFiles) {
    $text = [System.IO.File]::ReadAllText($file.FullName)
    $partialDeclarations += [System.Text.RegularExpressions.Regex]::Matches(
        $text,
        $partialDeclarationPattern).Count
    $lineNumber = 0
    foreach ($line in [System.IO.File]::ReadAllLines($file.FullName)) {
        $lineNumber++
        foreach ($match in [System.Text.RegularExpressions.Regex]::Matches($line, $partialTextPattern)) {
            if ($line -notmatch $partialDeclarationPattern.Replace('(?m)', '')) {
                $partialTextMatches++
                $partialTextRows.Add([pscustomobject]@{
                        Path = Get-RepoRelativePath $file.FullName
                        LineNumber = $lineNumber
                        Match = $match.Value
                    })
            }
        }
    }
    $typeDeclarations += [System.Text.RegularExpressions.Regex]::Matches(
        $text,
        '(?m)^\s*(?:(?:public|internal|protected|private|file)\s+)*(?:(?:abstract|sealed|static|unsafe)\s+)*(?:partial\s+)?(?:class|struct|record|interface|enum)\s+[A-Za-z_][A-Za-z0-9_]*').Count
}

$largeFiles = [ordered]@{
    ge1000 = @($allRows | Where-Object Lines -ge $LargeFileThreshold | Sort-Object Lines -Descending)
    ge2000 = @($allRows | Where-Object Lines -ge 2000 | Sort-Object Lines -Descending)
    ge3000 = @($allRows | Where-Object Lines -ge 3000 | Sort-Object Lines -Descending)
}

$viewModelFiles = @($csharpFiles | Where-Object BaseName -like '*ViewModel*')
$uiSignalPattern = '\b(?:Window|UserControl|ShowDialog|OpenFileDialog|SaveFileDialog|FolderBrowserDialog|ContextMenu|System\.Windows\.Forms|System\.Windows\.Controls)\b'
# Path helpers are included because ViewModel-owned path resolution is part of
# the same file-system boundary as File/Directory access in the baseline audit.
$ioSignalPattern = '\b(?:File|Directory)\.(?:Open|Create|Delete|Exists|GetFiles|GetDirectories|ReadAll\w*|WriteAll\w*|Copy|Move|GetCurrentDirectory|CreateDirectory|GetParent)\b|\b(?:FileInfo|DirectoryInfo|Path)\.(?:Get\w+|Combine|IsPathRooted|TrimEndingDirectorySeparator)\b|XmlSerializer|XDocument'
$viewModelRows = @(
    foreach ($file in $viewModelFiles) {
        $text = [System.IO.File]::ReadAllText($file.FullName)
        $uiSignals = @(Get-TextSignals $text $uiSignalPattern)
        $ioSignals = @(Get-TextSignals $text $ioSignalPattern)
        if ($uiSignals.Count -gt 0 -or $ioSignals.Count -gt 0) {
            [pscustomobject]@{
                Path = Get-RepoRelativePath $file.FullName
                UiSignals = ($uiSignals -join '; ')
                IoSignals = ($ioSignals -join '; ')
            }
        }
    }
)

$shellFiles = @(
    Get-ChildItem -LiteralPath (Join-Path $repositoryPath 'src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface') -File -Filter '*.cs' |
        Where-Object FullName -notmatch '\\(bin|obj)\\'
)
$shellStoragePattern = 'VisionPipelineBatchRunSummaryStorage\.(?:List|Load|Save)'
$shellStorageRows = @(
    foreach ($file in $shellFiles) {
        $lineNumber = 0
        foreach ($line in [System.IO.File]::ReadAllLines($file.FullName)) {
            $lineNumber++
            foreach ($match in [System.Text.RegularExpressions.Regex]::Matches($line, $shellStoragePattern)) {
                [pscustomobject]@{
                    Path = Get-RepoRelativePath $file.FullName
                    LineNumber = $lineNumber
                    Match = $match.Value
                }
            }
        }
    }
)

$projectFiles = @(
    Get-ChildItem -Path (Join-Path $repositoryPath 'src'), (Join-Path $repositoryPath 'tools') -Recurse -File -Filter '*.csproj' |
        Where-Object FullName -notmatch '\\(bin|obj)\\'
)
$projectGraph = @{}
$externalProjectReferences = [System.Collections.Generic.List[string]]::new()
foreach ($projectFile in $projectFiles) {
    $projectKey = Get-RepoRelativePath $projectFile.FullName
    $references = [System.Collections.Generic.List[string]]::new()
    [xml]$projectXml = [System.IO.File]::ReadAllText($projectFile.FullName)
    foreach ($reference in @($projectXml.SelectNodes('//ProjectReference'))) {
        $include = [string]$reference.Include
        if ([string]::IsNullOrWhiteSpace($include)) {
            continue
        }

        $referencePath = Resolve-FullPath (Join-Path $projectFile.DirectoryName $include)
        if (-not (Test-Path -LiteralPath $referencePath -PathType Leaf)) {
            $externalProjectReferences.Add($projectKey + ' -> ' + $include)
            continue
        }

        $referenceKey = Get-RepoRelativePath $referencePath
        $references.Add($referenceKey)
    }

    $projectGraph[$projectKey] = @($references)
}

$visitState = @{}
$cyclePaths = [System.Collections.Generic.List[string]]::new()
function Visit-Project {
    param(
        [string]$Project,
        [string[]]$Stack
    )

    if ($visitState[$Project] -eq 'active') {
        $cyclePaths.Add((($Stack + $Project) -join ' -> '))
        return
    }
    if ($visitState[$Project] -eq 'done') {
        return
    }

    $visitState[$Project] = 'active'
    foreach ($reference in @($projectGraph[$Project])) {
        Visit-Project $reference ($Stack + $Project)
    }
    $visitState[$Project] = 'done'
}

foreach ($project in $projectGraph.Keys) {
    Visit-Project $project @()
}

$sourceSummary = [ordered]@{
    csharpFiles = $csharpFiles.Count
    csharpLines = [int64](($csharpRows | Measure-Object -Property Lines -Sum).Sum)
    csharpBytes = [int64](($csharpRows | Measure-Object -Property Bytes -Sum).Sum)
    xamlFiles = $xamlFiles.Count
    xamlLines = [int64](($xamlRows | Measure-Object -Property Lines -Sum).Sum)
    xamlBytes = [int64](($xamlRows | Measure-Object -Property Bytes -Sum).Sum)
}
$projectSummary = [ordered]@{
    projects = $projectFiles.Count
    projectReferences = [int64](($projectGraph.Values | ForEach-Object { @($_).Count } | Measure-Object -Sum).Sum)
    externalReferences = $externalProjectReferences.Count
    cycles = $cyclePaths.Count
}
$summary = [ordered]@{
    generatedAt = (Get-Date).ToString('o')
    repositoryRoot = $repositoryPath
    source = $sourceSummary
    partialDeclarations = $partialDeclarations
    partialTextMatches = $partialTextMatches
    typeDeclarations = $typeDeclarations
    largeFiles = [ordered]@{
        ge1000 = $largeFiles.ge1000.Count
        ge2000 = $largeFiles.ge2000.Count
        ge3000 = $largeFiles.ge3000.Count
    }
    shell = [ordered]@{
        files = $shellFiles.Count
        lines = [int64](($allRows | Where-Object { $_.Path -like 'src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/*' } | Measure-Object -Property Lines -Sum).Sum)
        runHistoryStorageCalls = $shellStorageRows.Count
    }
    viewModels = [ordered]@{
        files = $viewModelFiles.Count
        directUiOrDialogFiles = @($viewModelRows | Where-Object { -not [string]::IsNullOrWhiteSpace($_.UiSignals) }).Count
        directIoFiles = @($viewModelRows | Where-Object { -not [string]::IsNullOrWhiteSpace($_.IoSignals) }).Count
    }
    projectGraph = $projectSummary
}

$summaryPath = Join-Path $outputPath 'source-survey.json'
$summary | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $summaryPath -Encoding UTF8
$largeFiles.ge1000 | Export-Csv -LiteralPath (Join-Path $outputPath 'large-files.csv') -NoTypeInformation -Encoding UTF8
$viewModelRows | Export-Csv -LiteralPath (Join-Path $outputPath 'viewmodel-ui-io.csv') -NoTypeInformation -Encoding UTF8
$partialTextRows | Export-Csv -LiteralPath (Join-Path $outputPath 'partial-text-matches.csv') -NoTypeInformation -Encoding UTF8
Write-CsvEvidence (Join-Path $outputPath 'shell-storage-calls.csv') $shellStorageRows @('Path', 'LineNumber', 'Match')
Write-CsvEvidence (Join-Path $outputPath 'project-cycles.csv') (@($cyclePaths | ForEach-Object { [pscustomobject]@{ Cycle = $_ } })) @('Cycle')

$summaryLines = @(
    'OpenVisionLab Refactor Audit'
    ('RepositoryRoot=' + $repositoryPath)
    ('CSharpFiles=' + $sourceSummary.csharpFiles + ';CSharpLines=' + $sourceSummary.csharpLines + ';CSharpBytes=' + $sourceSummary.csharpBytes)
    ('XamlFiles=' + $sourceSummary.xamlFiles + ';XamlLines=' + $sourceSummary.xamlLines + ';XamlBytes=' + $sourceSummary.xamlBytes)
    ('PartialDeclarations=' + $partialDeclarations + ';PartialTextMatches=' + $partialTextMatches + ';TypeDeclarations=' + $typeDeclarations)
    ('LargeFiles>=1000=' + $largeFiles.ge1000.Count + ';>=2000=' + $largeFiles.ge2000.Count + ';>=3000=' + $largeFiles.ge3000.Count)
    ('ShellFiles=' + $shellFiles.Count + ';ShellLines=' + $summary.shell.lines + ';ShellRunHistoryStorageCalls=' + $shellStorageRows.Count)
    ('ViewModelFiles=' + $viewModelFiles.Count + ';DirectUiOrDialogFiles=' + $summary.viewModels.directUiOrDialogFiles + ';DirectIoFiles=' + $summary.viewModels.directIoFiles)
    ('Projects=' + $projectSummary.projects + ';ProjectReferences=' + $projectSummary.projectReferences + ';ExternalReferences=' + $projectSummary.externalReferences + ';Cycles=' + $projectSummary.cycles)
    ('KnownDebt=RoiImageCanvasViewModel UI/IO coupling is reported for OVL-11 and is not hidden by this audit.')
)
$summaryLines | Set-Content -LiteralPath (Join-Path $outputPath 'source-survey-summary.txt') -Encoding UTF8
$cycleCheckLines = @('Cycles=' + $cyclePaths.Count) + @($cyclePaths)
$cycleCheckLines | Set-Content -LiteralPath (Join-Path $outputPath 'project-cycle-check.txt') -Encoding UTF8

$status = if ($projectSummary.cycles -eq 0 -and $shellStorageRows.Count -eq 0) { 'PASS' } else { 'FAIL' }
if ($Verify) {
    if ($csharpFiles.Count -eq 0) {
        throw 'Audit verification failed: no C# source files were found.'
    }
    if ($projectSummary.cycles -ne 0) {
        throw "Audit verification failed: project cycles detected ($($projectSummary.cycles))."
    }
    if ($shellStorageRows.Count -ne 0) {
        throw "Audit verification failed: direct Shell Run History storage calls detected ($($shellStorageRows.Count))."
    }
    $knownViewModel = @($viewModelRows | Where-Object Path -like '*RoiImageCanvasViewModel*')
    if ($knownViewModel.Count -eq 0) {
        throw 'Audit verification failed: known RoiImageCanvasViewModel UI/IO signal was not reported.'
    }
}

Write-Output ('REFACTOR_AUDIT=' + $status + '|CSharpFiles=' + $sourceSummary.csharpFiles + '|XamlFiles=' + $sourceSummary.xamlFiles + '|PartialDeclarations=' + $partialDeclarations + '|PartialTextMatches=' + $partialTextMatches + '|ViewModelUiIoFiles=' + $summary.viewModels.directUiOrDialogFiles + '|ProjectCycles=' + $projectSummary.cycles + '|ShellStorageCalls=' + $shellStorageRows.Count)
if ($status -eq 'FAIL') {
    exit 1
}
