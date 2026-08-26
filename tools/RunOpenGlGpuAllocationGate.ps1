param(
    [Parameter(Mandatory = $true)]
    [string]$SmokeDll,
    [Parameter(Mandatory = $true)]
    [string]$OutputDir,
    [string]$Target = "wpf_shell_host_image_4512_lifetime",
    [int]$ImageWidth = 4512,
    [int]$ImageHeight = 4512,
    [int]$LateWindowSeconds = 20,
    [int]$SampleMilliseconds = 500,
    [int]$TimeoutSeconds = 180
)

$ErrorActionPreference = "Stop"

$SmokeDll = [System.IO.Path]::GetFullPath($SmokeDll)
$OutputDir = [System.IO.Path]::GetFullPath($OutputDir)
if (-not (Test-Path -LiteralPath $SmokeDll -PathType Leaf)) {
    throw "Smoke DLL does not exist: $SmokeDll"
}

[void][System.IO.Directory]::CreateDirectory($OutputDir)
$tempDir = Join-Path $OutputDir "temp"
[void][System.IO.Directory]::CreateDirectory($tempDir)
$stdoutPath = Join-Path $OutputDir "gpu-smoke.stdout.txt"
$stderrPath = Join-Path $OutputDir "gpu-smoke.stderr.txt"
$csvPath = Join-Path $OutputDir "gpu-process-memory.csv"
$reportPath = Join-Path $OutputDir "gpu-process-memory.txt"
$adapterPath = Join-Path $OutputDir "gpu-adapter.txt"

$textureBytes = [int64]$ImageWidth * [int64]$ImageHeight * 4
$plateauDeltaCeilingBytes = $textureBytes * 2
$startedUtc = [DateTime]::UtcNow

$adapterLines = @(
    "CounterSet=GPU Process Memory",
    "CounterPaths=\\GPU Process Memory(*)\\Dedicated Usage;\\GPU Process Memory(*)\\Shared Usage",
    "CounterAvailability=Windows performance counters available",
    "NvidiaSmi=unavailable-or-not-run"
)
try {
    $adapters = Get-CimInstance Win32_VideoController -ErrorAction Stop
    foreach ($adapter in $adapters) {
        $adapterLines += "Adapter=Name:$($adapter.Name)|DriverVersion:$($adapter.DriverVersion)|AdapterRAM:$($adapter.AdapterRAM)"
    }
}
catch {
    $adapterLines += "AdapterQueryError=$($_.Exception.Message)"
}
if (Get-Command nvidia-smi.exe -ErrorAction SilentlyContinue) {
    try {
        $adapterLines += "NvidiaSmi=" + (& nvidia-smi.exe --query-gpu=index,name,driver_version,memory.total,memory.used --format=csv,noheader,nounits 2>&1 | Out-String).Trim()
    }
    catch {
        $adapterLines += "NvidiaSmiError=$($_.Exception.Message)"
    }
}
$adapterLines | Set-Content -LiteralPath $adapterPath -Encoding UTF8

$psi = [System.Diagnostics.ProcessStartInfo]::new()
$psi.FileName = "dotnet.exe"
$psi.UseShellExecute = $false
$psi.CreateNoWindow = $true
$psi.RedirectStandardOutput = $true
$psi.RedirectStandardError = $true
$smokeOutputDir = Join-Path $OutputDir "smoke"
$psi.Arguments = '"{0}" --target "{1}" "{2}"' -f $SmokeDll, $Target, $smokeOutputDir
$psi.EnvironmentVariables["TEMP"] = $tempDir
$psi.EnvironmentVariables["TMP"] = $tempDir

$process = [System.Diagnostics.Process]::new()
$process.StartInfo = $psi
if (-not $process.Start()) {
    throw "Could not start the current smoke DLL."
}
$smokePid = $process.Id

$rows = [System.Collections.Generic.List[object]]::new()
$counterPaths = @(
    "\GPU Process Memory(*)\Dedicated Usage",
    "\GPU Process Memory(*)\Shared Usage"
)
$deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
$counterError = ""

try {
    while (-not $process.HasExited) {
        if ([DateTime]::UtcNow -gt $deadline) {
            try { $process.Kill($true) } catch { }
            throw "GPU smoke process exceeded ${TimeoutSeconds}s timeout."
        }

        try {
            $counterSamples = (Get-Counter -Counter $counterPaths -MaxSamples 1 -ErrorAction Stop).CounterSamples |
                Where-Object { $_.InstanceName -match "(?i)^pid_${smokePid}_" }
            $dedicated = ($counterSamples | Where-Object { $_.Path -match "(?i)dedicated usage" } | Measure-Object -Property CookedValue -Sum).Sum
            $shared = ($counterSamples | Where-Object { $_.Path -match "(?i)shared usage" } | Measure-Object -Property CookedValue -Sum).Sum
            if ($null -eq $dedicated) { $dedicated = 0 }
            if ($null -eq $shared) { $shared = 0 }
            $luidValues = @($counterSamples | ForEach-Object { $_.InstanceName } | Sort-Object -Unique)
            $rows.Add([pscustomobject]@{
                    Utc = [DateTime]::UtcNow.ToString("o")
                    Pid = $smokePid
                    DedicatedBytes = [int64][Math]::Round([double]$dedicated)
                    SharedBytes = [int64][Math]::Round([double]$shared)
                    CounterInstances = [string]::Join(";", $luidValues)
                })
        }
        catch {
            $counterError = $_.Exception.Message
        }

        Start-Sleep -Milliseconds $SampleMilliseconds
    }
}
finally {
    $process.WaitForExit()
    $exitCode = $process.ExitCode
    $stdout = $process.StandardOutput.ReadToEnd()
    $stderr = $process.StandardError.ReadToEnd()
    $stdout | Set-Content -LiteralPath $stdoutPath -Encoding UTF8
    $stderr | Set-Content -LiteralPath $stderrPath -Encoding UTF8
    $process.Dispose()
}

if ($rows.Count -gt 0) {
    $rows | Export-Csv -LiteralPath $csvPath -NoTypeInformation -Encoding UTF8
}

function Format-GpuBytes([int64]$bytes) {
    return (($bytes / 1MB).ToString("0.0", [Globalization.CultureInfo]::InvariantCulture) + " MB")
}

$report = [System.Collections.Generic.List[string]]::new()
$report.Add("Result=INCOMPLETE")
$report.Add("Target=$Target")
$report.Add("Pid=$smokePid")
$report.Add("ExitCode=$exitCode")
$report.Add("StartedUtc=$($startedUtc.ToString("o"))")
$report.Add("FinishedUtc=$([DateTime]::UtcNow.ToString("o"))")
$report.Add("Image=$ImageWidth`x$ImageHeight")
$report.Add("TextureBytesUpperBound=$textureBytes|$(Format-GpuBytes $textureBytes)")
$report.Add("PredeclaredLateWindowSeconds=$LateWindowSeconds")
$report.Add("PredeclaredPlateauDeltaCeilingBytes=$plateauDeltaCeilingBytes|$(Format-GpuBytes $plateauDeltaCeilingBytes)")
$report.Add("SampleMilliseconds=$SampleMilliseconds")
$report.Add("CounterError=$counterError")
$report.Add("SampleCount=$($rows.Count)")
$report.Add("EvidenceCsv=$csvPath")
$report.Add("AdapterEvidence=$adapterPath")
$report.Add("StdoutEvidence=$stdoutPath")
$report.Add("StderrEvidence=$stderrPath")

if ($rows.Count -eq 0) {
    $report.Add("DedicatedPlateau=UNVERIFIED|No per-process GPU counter samples matched pid_$smokePid.")
    $report.Add("SharedPlateau=UNVERIFIED|No per-process GPU counter samples matched pid_$smokePid.")
}
else {
    $finishedAt = [DateTime]::Parse($rows[$rows.Count - 1].Utc).ToUniversalTime()
    $lateStart = $finishedAt.AddSeconds(-$LateWindowSeconds)
    $lateRows = @($rows | Where-Object { [DateTime]::Parse($_.Utc).ToUniversalTime() -ge $lateStart })
    if ($lateRows.Count -eq 0) { $lateRows = @($rows) }

    $dedicatedValues = @($lateRows | ForEach-Object { [int64]$_.DedicatedBytes })
    $sharedValues = @($lateRows | ForEach-Object { [int64]$_.SharedBytes })
    $dedicatedMin = [int64](($dedicatedValues | Measure-Object -Minimum).Minimum)
    $dedicatedMax = [int64](($dedicatedValues | Measure-Object -Maximum).Maximum)
    $sharedMin = [int64](($sharedValues | Measure-Object -Minimum).Minimum)
    $sharedMax = [int64](($sharedValues | Measure-Object -Maximum).Maximum)
    $dedicatedRange = $dedicatedMax - $dedicatedMin
    $sharedRange = $sharedMax - $sharedMin
    $dedicatedPass = $dedicatedRange -le $plateauDeltaCeilingBytes
    $sharedPass = $sharedRange -le $plateauDeltaCeilingBytes
    $report.Add("LateSampleCount=$($lateRows.Count)")
    $report.Add("DedicatedLateMin=$dedicatedMin|$(Format-GpuBytes $dedicatedMin)")
    $report.Add("DedicatedLateMax=$dedicatedMax|$(Format-GpuBytes $dedicatedMax)")
    $report.Add("DedicatedLateRange=$dedicatedRange|$(Format-GpuBytes $dedicatedRange)")
    $report.Add("SharedLateMin=$sharedMin|$(Format-GpuBytes $sharedMin)")
    $report.Add("SharedLateMax=$sharedMax|$(Format-GpuBytes $sharedMax)")
    $report.Add("SharedLateRange=$sharedRange|$(Format-GpuBytes $sharedRange)")
    $report.Add("DedicatedPlateau=$(if ($dedicatedPass) { 'PASS' } else { 'FAIL' })")
    $report.Add("SharedPlateau=$(if ($sharedPass) { 'PASS' } else { 'FAIL' })")
    $counterInstanceText = [string]::Join(";", @($rows | ForEach-Object { $_.CounterInstances } | Where-Object { $_ } | Sort-Object -Unique))
    $report.Add("CounterInstances=$counterInstanceText")
    if ($exitCode -eq 0 -and $dedicatedPass -and $sharedPass) {
        $report[0] = "Result=PASS"
    }
}

$report | Set-Content -LiteralPath $reportPath -Encoding UTF8
Get-Content -LiteralPath $reportPath
if ($report[0] -ne "Result=PASS") {
    exit 1
}
