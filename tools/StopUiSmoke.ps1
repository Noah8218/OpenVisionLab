param(
    [switch]$WhatIfOnly
)

$ErrorActionPreference = "Continue"

$processes = @(Get-Process |
    Where-Object { $_.ProcessName -eq "PipelineViewerScreenshotSmoke" } |
    Sort-Object Id)

if ($processes.Count -eq 0) {
    Write-Host "No PipelineViewerScreenshotSmoke processes are running."
    return
}

Write-Host "PipelineViewerScreenshotSmoke process(es):"
$processes | Select-Object ProcessName, Id, Responding, StartTime | Format-Table -AutoSize

if ($WhatIfOnly) {
    Write-Host "WhatIfOnly was specified. No process was stopped."
    return
}

foreach ($process in $processes) {
    try {
        Stop-Process -Id $process.Id -Force -ErrorAction Stop
        Write-Host "Stopped PipelineViewerScreenshotSmoke PID $($process.Id)."
    }
    catch {
        Write-Warning "Could not stop PipelineViewerScreenshotSmoke PID $($process.Id): $($_.Exception.Message)"
    }
}

$remaining = @(Get-Process |
    Where-Object { $_.ProcessName -eq "PipelineViewerScreenshotSmoke" } |
    Sort-Object Id)

if ($remaining.Count -gt 0) {
    Write-Warning "Some UI smoke processes are still running. Close them from Task Manager if the app remains blocked."
    $remaining | Select-Object ProcessName, Id, Responding, StartTime | Format-Table -AutoSize
}
else {
    Write-Host "All UI smoke processes are stopped."
}
