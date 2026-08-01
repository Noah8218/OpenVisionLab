[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [string]$RepoRoot = '',
    [string]$ExternalRoot = 'D:\OpenVisionLab-TestData\OpenVisionLab_Dev',
    [string[]]$ExcludeRelativePath = @(),
    [switch]$RestoreToRepo
)

$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($RepoRoot)) {
    $scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
    $RepoRoot = Split-Path -Parent $scriptDirectory
}

$repoPath = [System.IO.Path]::GetFullPath($RepoRoot).TrimEnd('\', '/')
$externalPath = [System.IO.Path]::GetFullPath($ExternalRoot).TrimEnd('\', '/')
$repoPrefix = $repoPath + [System.IO.Path]::DirectorySeparatorChar
$externalPrefix = $externalPath + [System.IO.Path]::DirectorySeparatorChar

if ($repoPath -eq $externalPath -or $externalPath.StartsWith($repoPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw 'ExternalRoot must be outside the repository.'
}

function Get-RelativePath {
    param([string]$FullPath)

    $resolved = [System.IO.Path]::GetFullPath($FullPath)
    if (-not $resolved.StartsWith($repoPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Path is outside the repository: $resolved"
    }

    return $resolved.Substring($repoPrefix.Length)
}

$excludedRelativePaths = [System.Collections.Generic.HashSet[string]]::new(
    [System.StringComparer]::OrdinalIgnoreCase)
foreach ($excludedPath in $ExcludeRelativePath) {
    if ([string]::IsNullOrWhiteSpace($excludedPath)) { continue }
    if ([System.IO.Path]::IsPathRooted($excludedPath)) {
        throw "ExcludeRelativePath must be repository-relative: $excludedPath"
    }

    $excludedFullPath = [System.IO.Path]::GetFullPath((Join-Path $repoPath $excludedPath))
    if (-not $excludedFullPath.StartsWith($repoPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "ExcludeRelativePath escapes the repository: $excludedPath"
    }

    [void]$excludedRelativePaths.Add((Get-RelativePath $excludedFullPath).TrimEnd('\', '/'))
}

function Test-IsExcludedRelativePath {
    param([string]$RelativePath)

    foreach ($excludedPath in $excludedRelativePaths) {
        if ($RelativePath.Equals($excludedPath, [System.StringComparison]::OrdinalIgnoreCase) -or
            $RelativePath.StartsWith($excludedPath + '\', [System.StringComparison]::OrdinalIgnoreCase)) {
            return $true
        }
    }
    return $false
}

function Get-DirectoryStats {
    param([string]$Path)

    $files = @(Get-ChildItem -LiteralPath $Path -File -Recurse -Force -ErrorAction Stop)
    $bytes = ($files | Measure-Object -Property Length -Sum).Sum
    if ($null -eq $bytes) { $bytes = 0 }
    return [pscustomobject]@{ FileCount = $files.Count; Bytes = [long]$bytes }
}

function Get-JunctionTargetPath {
    param([System.IO.DirectoryInfo]$Item)

    if (-not ($Item.Attributes -band [System.IO.FileAttributes]::ReparsePoint)) {
        return $null
    }

    $target = @($Item.Target | Select-Object -First 1)
    if ($target.Count -eq 0 -or [string]::IsNullOrWhiteSpace([string]$target[0])) {
        return $null
    }

    return [System.IO.Path]::GetFullPath([string]$target[0]).TrimEnd('\', '/')
}

function Convert-ToExtendedPath {
    param([string]$Path)

    $fullPath = [System.IO.Path]::GetFullPath($Path)
    if ($fullPath.StartsWith('\\')) {
        return '\\?\UNC\' + $fullPath.Substring(2)
    }
    return '\\?\' + $fullPath
}

function Get-FileSha256 {
    param([string]$Path)

    $stream = [System.IO.File]::OpenRead((Convert-ToExtendedPath $Path))
    try {
        $sha = [System.Security.Cryptography.SHA256]::Create()
        try {
            return ([System.BitConverter]::ToString($sha.ComputeHash($stream))).Replace('-', '')
        }
        finally {
            $sha.Dispose()
        }
    }
    finally {
        $stream.Dispose()
    }
}

function Move-DirectoryVerified {
    param(
        [string]$Source,
        [string]$Destination
    )

    $sourceFiles = @(Get-ChildItem -LiteralPath $Source -File -Recurse -Force -ErrorAction Stop)
    $sourceDirectories = @(Get-ChildItem -LiteralPath $Source -Directory -Recurse -Force -ErrorAction Stop |
        Where-Object { -not ($_.Attributes -band [System.IO.FileAttributes]::ReparsePoint) })

    [System.IO.Directory]::CreateDirectory((Convert-ToExtendedPath $Destination)) | Out-Null
    foreach ($sourceDirectory in $sourceDirectories) {
        $directoryRelative = $sourceDirectory.FullName.Substring($Source.Length).TrimStart('\')
        [System.IO.Directory]::CreateDirectory((Convert-ToExtendedPath (Join-Path $Destination $directoryRelative))) | Out-Null
    }

    foreach ($sourceFile in $sourceFiles) {
        $fileRelative = $sourceFile.FullName.Substring($Source.Length).TrimStart('\')
        $destinationFile = Join-Path $Destination $fileRelative
        $destinationFileExtended = Convert-ToExtendedPath $destinationFile
        if ([System.IO.File]::Exists($destinationFileExtended)) {
            if ((Get-FileSha256 $sourceFile.FullName) -ne (Get-FileSha256 $destinationFile)) {
                throw "Move contains conflicting file content: $fileRelative"
            }
            continue
        }

        [System.IO.Directory]::CreateDirectory((Convert-ToExtendedPath (Split-Path -Parent $destinationFile))) | Out-Null
        [System.IO.File]::Copy((Convert-ToExtendedPath $sourceFile.FullName), $destinationFileExtended, $true)
        if ((Get-FileSha256 $sourceFile.FullName) -ne (Get-FileSha256 $destinationFile)) {
            throw "Copy verification failed: $fileRelative"
        }
    }

    [System.IO.Directory]::Delete((Convert-ToExtendedPath $Source), $true)
    return Get-DirectoryStats $Destination
}

function Get-BuildDirectoriesWithoutFollowingLinks {
    param([string]$Root)

    $found = [System.Collections.Generic.List[string]]::new()
    $pending = [System.Collections.Generic.Stack[string]]::new()
    $pending.Push($Root)

    while ($pending.Count -gt 0) {
        $current = $pending.Pop()
        foreach ($directory in Get-ChildItem -LiteralPath $current -Directory -Force -ErrorAction Stop) {
            if ($directory.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
                if ($directory.Name -in @('bin', 'obj')) { $found.Add($directory.FullName) }
                continue
            }
            if ($directory.Name -in @('bin', 'obj')) {
                $found.Add($directory.FullName)
                continue
            }
            if ($directory.Name -eq 'artifacts') { continue }
            $pending.Push($directory.FullName)
        }
    }

    return $found
}

$candidatePaths = [System.Collections.Generic.List[string]]::new()
$rootCandidates = @('.codex', '.codex-temp', '.vs', 'bin', 'obj', 'Sample', 'tmp', 'dist')
foreach ($relative in $rootCandidates) {
    $candidate = Join-Path $repoPath $relative
    if ((Test-Path -LiteralPath $candidate -PathType Container) -or (Test-Path -LiteralPath (Join-Path $externalPath $relative) -PathType Container)) {
        $candidatePaths.Add([System.IO.Path]::GetFullPath($candidate))
    }
}

$generatedSamples = Join-Path $repoPath 'docs\samples\generated'
if ((Test-Path -LiteralPath $generatedSamples -PathType Container) -or (Test-Path -LiteralPath (Join-Path $externalPath 'docs\samples\generated') -PathType Container)) {
    $candidatePaths.Add([System.IO.Path]::GetFullPath($generatedSamples))
}

foreach ($searchRootName in @('src', 'tools')) {
    $searchRoot = Join-Path $repoPath $searchRootName
    if (-not (Test-Path -LiteralPath $searchRoot -PathType Container)) { continue }

    $buildDirectories = Get-BuildDirectoriesWithoutFollowingLinks $searchRoot
    foreach ($directory in $buildDirectories) {
        $candidatePaths.Add([System.IO.Path]::GetFullPath([string]$directory))
    }
}

$libraryRoot = Join-Path $repoPath 'src\Libraries'
if (Test-Path -LiteralPath $libraryRoot -PathType Container) {
    foreach ($projectDirectory in Get-ChildItem -LiteralPath $libraryRoot -Directory -Force -ErrorAction Stop) {
        $directory = Join-Path $projectDirectory.FullName 'artifacts'
        $relative = Get-RelativePath $directory
        $externalDirectory = Join-Path $externalPath $relative
        if ((Test-Path -LiteralPath $directory -PathType Container) -or (Test-Path -LiteralPath $externalDirectory -PathType Container)) {
            $candidatePaths.Add([System.IO.Path]::GetFullPath($directory))
        }
    }
}

$candidates = @($candidatePaths | Sort-Object -Unique | ForEach-Object {
    $source = $_
    $relative = Get-RelativePath $source
    if (Test-IsExcludedRelativePath $relative) { return }
    $destination = [System.IO.Path]::GetFullPath((Join-Path $externalPath $relative))
    if (-not $destination.StartsWith($externalPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Destination escapes ExternalRoot: $destination"
    }
    [pscustomobject]@{ Source = $source; Relative = $relative; Destination = $destination }
})

if ($candidates.Count -eq 0) {
    Write-Output 'LocalDataMove=PASS Candidates=0 Moved=0 Existing=0 Files=0 Bytes=0'
    exit 0
}

if (-not $RestoreToRepo -and -not (Test-Path -LiteralPath $externalPath -PathType Container)) {
    if ($PSCmdlet.ShouldProcess($externalPath, 'Create external data root')) {
        New-Item -ItemType Directory -Path $externalPath -Force | Out-Null
    }
}

$moved = 0
$existing = 0
$totalFiles = 0L
$totalBytes = 0L
$results = [System.Collections.Generic.List[object]]::new()

foreach ($candidate in $candidates) {
    $sourceExists = Test-Path -LiteralPath $candidate.Source -PathType Container
    $destinationExists = Test-Path -LiteralPath $candidate.Destination -PathType Container
    $sourceItem = if ($sourceExists) { Get-Item -LiteralPath $candidate.Source -Force } else { $null }
    $junctionTarget = if ($null -ne $sourceItem) { Get-JunctionTargetPath $sourceItem } else { $null }

    if (-not $RestoreToRepo) {
        $tracked = @(git -C $repoPath ls-files -- $candidate.Relative.Replace('\', '/'))
        if ($tracked.Count -gt 0) {
            throw "Refusing to move tracked content: $($candidate.Relative) ($($tracked.Count) files)"
        }

        if ($null -ne $junctionTarget) {
            if ($junctionTarget -ne $candidate.Destination.TrimEnd('\', '/')) {
                throw "Existing junction has a different target: $($candidate.Relative) -> $junctionTarget"
            }
            $stats = Get-DirectoryStats $candidate.Destination
            $existing++
            $totalFiles += $stats.FileCount
            $totalBytes += $stats.Bytes
            $results.Add([pscustomobject]@{ Path = $candidate.Relative; State = 'Existing'; Files = $stats.FileCount; Bytes = $stats.Bytes })
            continue
        }

        if (-not $sourceExists) {
            if (-not $destinationExists) { continue }
            if ($PSCmdlet.ShouldProcess($candidate.Source, "Create junction to $($candidate.Destination)")) {
                $parent = Split-Path -Parent $candidate.Source
                New-Item -ItemType Directory -Path $parent -Force | Out-Null
                New-Item -ItemType Junction -Path $candidate.Source -Target $candidate.Destination | Out-Null
            }
            $stats = Get-DirectoryStats $candidate.Destination
            $moved++
            $totalFiles += $stats.FileCount
            $totalBytes += $stats.Bytes
            $results.Add([pscustomobject]@{ Path = $candidate.Relative; State = 'Relinked'; Files = $stats.FileCount; Bytes = $stats.Bytes })
            continue
        }

        if ($destinationExists) {
            if ($PSCmdlet.ShouldProcess($candidate.Source, "Complete verified partial move to $($candidate.Destination) and create junction")) {
                $afterRecovery = Move-DirectoryVerified $candidate.Source $candidate.Destination
                New-Item -ItemType Junction -Path $candidate.Source -Target $candidate.Destination | Out-Null
            }
            else {
                $afterRecovery = Get-DirectoryStats $candidate.Destination
            }
            $moved++
            $totalFiles += $afterRecovery.FileCount
            $totalBytes += $afterRecovery.Bytes
            $results.Add([pscustomobject]@{ Path = $candidate.Relative; State = 'Recovered'; Files = $afterRecovery.FileCount; Bytes = $afterRecovery.Bytes })
            continue
        }

        if ($PSCmdlet.ShouldProcess($candidate.Source, "Move to $($candidate.Destination) and create junction")) {
            $after = Move-DirectoryVerified $candidate.Source $candidate.Destination
            New-Item -ItemType Junction -Path $candidate.Source -Target $candidate.Destination | Out-Null
        }
        else {
            $after = Get-DirectoryStats $candidate.Source
        }
        $moved++
        $totalFiles += $after.FileCount
        $totalBytes += $after.Bytes
        $results.Add([pscustomobject]@{ Path = $candidate.Relative; State = 'Moved'; Files = $after.FileCount; Bytes = $after.Bytes })
    }
    else {
        if ($null -eq $junctionTarget) {
            if ($sourceExists -and -not $destinationExists) {
                $existing++
                continue
            }
            if ($sourceExists -and $destinationExists) {
                if ($PSCmdlet.ShouldProcess($candidate.Destination, "Complete verified partial restore to $($candidate.Source)")) {
                    $after = Move-DirectoryVerified $candidate.Destination $candidate.Source
                }
                else {
                    $after = Get-DirectoryStats $candidate.Destination
                }
                $moved++
                $totalFiles += $after.FileCount
                $totalBytes += $after.Bytes
                $results.Add([pscustomobject]@{ Path = $candidate.Relative; State = 'RestoreRecovered'; Files = $after.FileCount; Bytes = $after.Bytes })
                continue
            }
            throw "Restore requires the repository path to be a junction: $($candidate.Relative)"
        }
        if ($junctionTarget -ne $candidate.Destination.TrimEnd('\', '/') -or -not $destinationExists) {
            throw "Restore target mismatch: $($candidate.Relative)"
        }

        $before = Get-DirectoryStats $candidate.Destination
        if ($PSCmdlet.ShouldProcess($candidate.Source, "Remove junction and restore from $($candidate.Destination)")) {
            [System.IO.Directory]::Delete($candidate.Source)
            $after = Move-DirectoryVerified $candidate.Destination $candidate.Source
        }
        else {
            $after = $before
        }
        $moved++
        $totalFiles += $before.FileCount
        $totalBytes += $before.Bytes
        $results.Add([pscustomobject]@{ Path = $candidate.Relative; State = 'Restored'; Files = $before.FileCount; Bytes = $before.Bytes })
    }
}

$results | Sort-Object Path | Format-Table -AutoSize
$mode = if ($RestoreToRepo) { 'Restore' } else { 'Externalize' }
if ($WhatIfPreference) { $mode += 'WhatIf' }
Write-Output ("LocalDataMove=PASS Mode={0} Candidates={1} Moved={2} Existing={3} Files={4} Bytes={5}" -f $mode, $candidates.Count, $moved, $existing, $totalFiles, $totalBytes)
