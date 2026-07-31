[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [string]$SourceRoot = 'C:\Git',
    [string]$DestinationRoot = 'D:\OpenVisionLab-TestData\ProductionVerification_20260730'
)

$ErrorActionPreference = 'Stop'

$cloneNames = @(
    'OpenVisionLab_Production_DataRoot_RC_20260730',
    'OpenVisionLab_Production_DataRoot_Repro_20260730',
    'OpenVisionLab_Production_RC_Final_20260730',
    'OpenVisionLab_Production_Repro_20260730',
    'OpenVisionLab_Production_Repro_Final_20260730',
    'OpenVisionLab_Production_Verification_20260730',
    'OpenVisionLab_Production_Verification_Final_20260730',
    'OpenVisionLab_Production_Verification_Pass_20260730'
)

$sourceRootPath = [System.IO.Path]::GetFullPath($SourceRoot).TrimEnd('\', '/')
$destinationRootPath = [System.IO.Path]::GetFullPath($DestinationRoot).TrimEnd('\', '/')
$sourcePrefix = $sourceRootPath + [System.IO.Path]::DirectorySeparatorChar
$destinationPrefix = $destinationRootPath + [System.IO.Path]::DirectorySeparatorChar

if ($sourceRootPath -eq $destinationRootPath -or
    $destinationRootPath.StartsWith($sourcePrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw 'DestinationRoot must be outside SourceRoot.'
}

function Convert-ToExtendedPath {
    param([string]$Path)

    $fullPath = [System.IO.Path]::GetFullPath($Path)
    if ($fullPath.StartsWith('\\')) { return '\\?\UNC\' + $fullPath.Substring(2) }
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
        finally { $sha.Dispose() }
    }
    finally { $stream.Dispose() }
}

function Get-DirectoryStats {
    param([string]$Path)

    $files = @(Get-ChildItem -LiteralPath $Path -File -Recurse -Force -ErrorAction Stop)
    $bytes = ($files | Measure-Object -Property Length -Sum).Sum
    if ($null -eq $bytes) { $bytes = 0 }
    return [pscustomobject]@{ FileCount = $files.Count; Bytes = [long]$bytes }
}

function Copy-DirectoryVerified {
    param(
        [string]$Source,
        [string]$Destination
    )

    $sourceFiles = @(Get-ChildItem -LiteralPath $Source -File -Recurse -Force -ErrorAction Stop)
    $sourceDirectories = @(Get-ChildItem -LiteralPath $Source -Directory -Recurse -Force -ErrorAction Stop)
    $sourceBytes = ($sourceFiles | Measure-Object -Property Length -Sum).Sum
    if ($null -eq $sourceBytes) { $sourceBytes = 0 }

    [System.IO.Directory]::CreateDirectory((Convert-ToExtendedPath $Destination)) | Out-Null
    foreach ($sourceDirectory in $sourceDirectories) {
        $relative = $sourceDirectory.FullName.Substring($Source.Length).TrimStart('\')
        [System.IO.Directory]::CreateDirectory((Convert-ToExtendedPath (Join-Path $Destination $relative))) | Out-Null
    }

    foreach ($sourceFile in $sourceFiles) {
        $relative = $sourceFile.FullName.Substring($Source.Length).TrimStart('\')
        $destinationFile = Join-Path $Destination $relative
        $destinationExtended = Convert-ToExtendedPath $destinationFile
        if (-not [System.IO.File]::Exists($destinationExtended)) {
            [System.IO.Directory]::CreateDirectory((Convert-ToExtendedPath (Split-Path -Parent $destinationFile))) | Out-Null
            [System.IO.File]::Copy((Convert-ToExtendedPath $sourceFile.FullName), $destinationExtended, $true)
        }

        if ((Get-FileSha256 $sourceFile.FullName) -ne (Get-FileSha256 $destinationFile)) {
            throw "SHA-256 verification failed: $relative"
        }
    }

    $destinationStats = Get-DirectoryStats $Destination
    if ($destinationStats.FileCount -ne $sourceFiles.Count -or $destinationStats.Bytes -ne [long]$sourceBytes) {
        throw "Directory count/byte verification failed: $Source"
    }

    foreach ($sourceFile in $sourceFiles) {
        $extended = Convert-ToExtendedPath $sourceFile.FullName
        $attributes = [System.IO.File]::GetAttributes($extended)
        if ($attributes -band [System.IO.FileAttributes]::ReadOnly) {
            [System.IO.File]::SetAttributes($extended, ($attributes -band (-bnot [System.IO.FileAttributes]::ReadOnly)))
        }
    }
    [System.IO.Directory]::Delete((Convert-ToExtendedPath $Source), $true)

    return $destinationStats
}

$currentProcessId = $PID
$scopedProcesses = @(Get-CimInstance Win32_Process | Where-Object {
    if ($_.ProcessId -eq $currentProcessId) { return $false }
    $command = ([string]$_.ExecutablePath) + ' ' + ([string]$_.CommandLine)
    foreach ($name in $cloneNames) {
        if ($command -like ('*' + $name + '*')) { return $true }
    }
    return $false
})
if ($scopedProcesses.Count -gt 0) {
    throw "A process is using a verification clone: $($scopedProcesses.ProcessId -join ', ')"
}

$dirtyClone = Join-Path $sourceRootPath 'OpenVisionLab_Production_Verification_Final_20260730'
if (Test-Path -LiteralPath $dirtyClone -PathType Container) {
    $dirtyStatus = @(git -C $dirtyClone status --short)
    if ($dirtyStatus.Count -ne 1 -or $dirtyStatus[0].Trim() -ne 'M tools/TestReleaseDistribution.ps1') {
        throw "Unexpected dirty state in verification clone: $($dirtyStatus -join '; ')"
    }
    $dirtyHash = Get-FileSha256 (Join-Path $dirtyClone 'tools\TestReleaseDistribution.ps1')
    if ($dirtyHash -ne 'A767F279236B2B9B92277E5AE0A882DE1B82A51C3B9028799F3D5F03B3B910B1') {
        throw 'The audited formatting-only dirty file changed after review.'
    }
}

if (-not (Test-Path -LiteralPath $destinationRootPath -PathType Container)) {
    if ($PSCmdlet.ShouldProcess($destinationRootPath, 'Create verification clone archive root')) {
        New-Item -ItemType Directory -Path $destinationRootPath -Force | Out-Null
    }
}

$results = [System.Collections.Generic.List[object]]::new()
$moved = 0
$existing = 0
$totalFiles = 0L
$totalBytes = 0L

foreach ($name in $cloneNames) {
    $source = [System.IO.Path]::GetFullPath((Join-Path $sourceRootPath $name))
    $destination = [System.IO.Path]::GetFullPath((Join-Path $destinationRootPath $name))
    if (-not $source.StartsWith($sourcePrefix, [System.StringComparison]::OrdinalIgnoreCase) -or
        -not $destination.StartsWith($destinationPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Clone path escaped an approved root: $name"
    }

    $sourceExists = Test-Path -LiteralPath $source -PathType Container
    $destinationExists = Test-Path -LiteralPath $destination -PathType Container
    if (-not $sourceExists -and -not $destinationExists) {
        throw "Clone is missing from both roots: $name"
    }

    if (-not $sourceExists) {
        $stats = Get-DirectoryStats $destination
        $existing++
        $state = 'Existing'
    }
    elseif ($PSCmdlet.ShouldProcess($source, "Move verified clone to $destination")) {
        $stats = Copy-DirectoryVerified $source $destination
        $moved++
        $state = if ($destinationExists) { 'Recovered' } else { 'Moved' }
    }
    else {
        $stats = Get-DirectoryStats $source
        $moved++
        $state = 'Planned'
    }

    $destinationHead = if (Test-Path -LiteralPath $destination) { git -C $destination rev-parse --short HEAD } else { git -C $source rev-parse --short HEAD }
    $destinationDirty = if (Test-Path -LiteralPath $destination) { @(git -C $destination status --short).Count } else { @(git -C $source status --short).Count }
    $results.Add([pscustomobject]@{
        Name = $name
        State = $state
        Files = $stats.FileCount
        Bytes = $stats.Bytes
        Head = $destinationHead
        Dirty = $destinationDirty
    })
    $totalFiles += $stats.FileCount
    $totalBytes += $stats.Bytes
}

$results | Format-Table -AutoSize
$mode = if ($WhatIfPreference) { 'WhatIf' } else { 'Move' }
Write-Output ("VerificationCloneMove=PASS Mode={0} Candidates={1} Moved={2} Existing={3} Files={4} Bytes={5}" -f $mode, $cloneNames.Count, $moved, $existing, $totalFiles, $totalBytes)

if (-not $WhatIfPreference) {
    $manifest = [ordered]@{
        schemaVersion = '1.0'
        movedAt = (Get-Date).ToString('o')
        sourceRoot = $sourceRootPath
        destinationRoot = $destinationRootPath
        totalFiles = $totalFiles
        totalBytes = $totalBytes
        clones = @($results)
    }
    $manifestPath = Join-Path $destinationRootPath 'migration_manifest.json'
    $manifest | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $manifestPath -Encoding UTF8
    Write-Output "Manifest=$manifestPath"
}
