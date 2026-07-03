param(
    [Parameter(Mandatory = $true)]
    [string]$Path,

    [int]$MinPrewarmCount = 15,
    [int]$MaxPrewarmElapsedMs = 6000,
    [int]$MaxColdReadyMs = 600,
    [int]$MaxWarmReadyMs = 250
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $Path)) {
    throw "Tool-open perf file was not found: $Path"
}

function ConvertTo-PerfRecord {
    param(
        [string]$Line
    )

    if ([string]::IsNullOrWhiteSpace($Line)) {
        return $null
    }

    $parts = $Line -split "\|"
    if ($parts.Count -lt 2 -or [string]::Equals($parts[0], "Pass", [StringComparison]::OrdinalIgnoreCase)) {
        return $null
    }

    $record = [ordered]@{
        Pass = $parts[0]
        Tool = $parts[1]
    }

    for ($i = 2; $i -lt $parts.Count; $i++) {
        $part = $parts[$i]
        $equalsIndex = $part.IndexOf("=")
        if ($equalsIndex -le 0) {
            continue
        }

        $key = $part.Substring(0, $equalsIndex)
        $value = $part.Substring($equalsIndex + 1)
        $record[$key] = $value
    }

    return [pscustomobject]$record
}

function Get-PerfInt {
    param(
        [Parameter(Mandatory = $true)]$Record,
        [Parameter(Mandatory = $true)][string]$Name,
        [int]$Default = 0
    )

    $property = $Record.PSObject.Properties[$Name]
    if ($null -eq $property) {
        return $Default
    }

    $value = 0
    if ([int]::TryParse([string]$property.Value, [ref]$value)) {
        return $value
    }

    return $Default
}

function Get-PerfBool {
    param(
        [Parameter(Mandatory = $true)]$Record,
        [Parameter(Mandatory = $true)][string]$Name
    )

    $property = $Record.PSObject.Properties[$Name]
    if ($null -eq $property) {
        return $false
    }

    return [string]::Equals([string]$property.Value, "True", [StringComparison]::OrdinalIgnoreCase)
}

function Get-MaxPerfInt {
    param(
        [Parameter(Mandatory = $true)]$Rows,
        [Parameter(Mandatory = $true)][string]$Name
    )

    $values = @($Rows | ForEach-Object { Get-PerfInt -Record $_ -Name $Name })
    if ($values.Count -eq 0) {
        return 0
    }

    return [int](($values | Measure-Object -Maximum).Maximum)
}

$records = @(Get-Content -LiteralPath $Path | ForEach-Object { ConvertTo-PerfRecord -Line $_ } | Where-Object { $null -ne $_ })
$issues = New-Object System.Collections.Generic.List[string]

$prewarm = $records |
    Where-Object { $_.Pass -eq "Prewarm" -and $_.Tool -eq "NativeTools" } |
    Select-Object -First 1

$prewarmCacheCount = 0
$prewarmCreatedCount = 0
$prewarmElapsedMs = 0
if ($null -eq $prewarm) {
    $issues.Add("Missing Prewarm|NativeTools row.") | Out-Null
}
else {
    $prewarmCacheCount = Get-PerfInt -Record $prewarm -Name "CacheCount"
    $prewarmCreatedCount = Get-PerfInt -Record $prewarm -Name "CreatedCount"
    $prewarmElapsedMs = Get-PerfInt -Record $prewarm -Name "ElapsedMs"

    if (-not (Get-PerfBool -Record $prewarm -Name "Completed")) {
        $issues.Add("Native tool prewarm did not complete.") | Out-Null
    }

    if ($prewarmCacheCount -lt $MinPrewarmCount) {
        $issues.Add("Prewarm cache count $prewarmCacheCount is below $MinPrewarmCount.") | Out-Null
    }

    if ($prewarmCreatedCount -lt $MinPrewarmCount) {
        $issues.Add("Prewarm created count $prewarmCreatedCount is below $MinPrewarmCount.") | Out-Null
    }

    if ($prewarmElapsedMs -gt $MaxPrewarmElapsedMs) {
        $issues.Add("Prewarm elapsed $prewarmElapsedMs ms exceeds $MaxPrewarmElapsedMs ms.") | Out-Null
    }
}

$coldRows = @($records | Where-Object { $_.Pass -eq "Cold" })
$warmRows = @($records | Where-Object { $_.Pass -eq "Warm" })
$coldMaxReadyMs = Get-MaxPerfInt -Rows $coldRows -Name "ReadyMs"
$warmMaxReadyMs = Get-MaxPerfInt -Rows $warmRows -Name "ReadyMs"
$warmMaxSelectMs = Get-MaxPerfInt -Rows $warmRows -Name "SelectMs"

# The smoke target measures all native tools twice. Missing rows are usually a broken registry/prewarm contract.
if ($coldRows.Count -lt $MinPrewarmCount) {
    $issues.Add("Cold tool-open rows $($coldRows.Count) are below $MinPrewarmCount.") | Out-Null
}

if ($warmRows.Count -lt $MinPrewarmCount) {
    $issues.Add("Warm tool-open rows $($warmRows.Count) are below $MinPrewarmCount.") | Out-Null
}

foreach ($row in @($coldRows + $warmRows)) {
    if (-not (Get-PerfBool -Record $row -Name "PrewarmCompleted")) {
        $issues.Add("$($row.Pass) $($row.Tool) did not report PrewarmCompleted=True.") | Out-Null
    }

    $rowPrewarmCount = Get-PerfInt -Record $row -Name "PrewarmCount"
    if ($rowPrewarmCount -lt $MinPrewarmCount) {
        $issues.Add("$($row.Pass) $($row.Tool) prewarm count $rowPrewarmCount is below $MinPrewarmCount.") | Out-Null
    }
}

if ($coldMaxReadyMs -gt $MaxColdReadyMs) {
    $issues.Add("Cold max ready $coldMaxReadyMs ms exceeds $MaxColdReadyMs ms.") | Out-Null
}

if ($warmMaxReadyMs -gt $MaxWarmReadyMs) {
    $issues.Add("Warm max ready $warmMaxReadyMs ms exceeds $MaxWarmReadyMs ms.") | Out-Null
}

if ($issues.Count -eq 0) {
    Write-Host "ToolOpenPerfGate=OK"
}
else {
    Write-Host "ToolOpenPerfGate=NG"
}

Write-Host "PerfFile=$Path"
Write-Host "PrewarmCacheCount=$prewarmCacheCount"
Write-Host "PrewarmCreatedCount=$prewarmCreatedCount"
Write-Host "PrewarmElapsedMs=$prewarmElapsedMs"
Write-Host "ColdRows=$($coldRows.Count)"
Write-Host "ColdMaxReadyMs=$coldMaxReadyMs"
Write-Host "WarmRows=$($warmRows.Count)"
Write-Host "WarmMaxSelectMs=$warmMaxSelectMs"
Write-Host "WarmMaxReadyMs=$warmMaxReadyMs"
Write-Host "Issues=$($issues.Count)"

foreach ($issue in $issues) {
    Write-Host "Issue=$issue"
}

if ($issues.Count -gt 0) {
    exit 1
}

exit 0
