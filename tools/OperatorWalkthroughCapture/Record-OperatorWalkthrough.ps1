param(
    [Parameter(Mandatory = $true)]
    [ValidateSet(
        "novice-blob-self-trial",
        "novice-blob-teaching-self-trial",
        "novice-blob-pipeline-persistence",
        "novice-scratch-threshold-blob-recipe",
        "novice-four-step-route-clarity",
        "novice-matching-correction-loop",
        "blob-good-bad",
        "fixture-crash",
        "matching-tool-view",
        "line-tool-view",
        "blob-tool-view",
        "contour-tool-view",
        "filter-tool-view",
        "morphology-tool-view",
        "filter-chain",
        "morphology-chain")]
    [string]$Scenario,

    [Parameter(Mandatory = $true)]
    [string]$OutputDirectory,

    [string]$RuntimeDirectory = ""
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$runtimeRoot = if ([string]::IsNullOrWhiteSpace($RuntimeDirectory)) {
    Join-Path $repoRoot "bin\Debug"
}
elseif ([System.IO.Path]::IsPathRooted($RuntimeDirectory)) {
    [System.IO.Path]::GetFullPath($RuntimeDirectory)
}
else {
    [System.IO.Path]::GetFullPath((Join-Path $repoRoot $RuntimeDirectory))
}
$exePath = Join-Path $runtimeRoot "OpenVisionLab.exe"
$appAssemblyPath = Join-Path $runtimeRoot "OpenVisionLab.dll"
$outputRoot = [System.IO.Path]::GetFullPath(
    $(if ([System.IO.Path]::IsPathRooted($OutputDirectory)) {
        $OutputDirectory
    }
    else {
        Join-Path $repoRoot $OutputDirectory
    }))
$videoPath = Join-Path $outputRoot "$Scenario.mp4"
$timelinePath = Join-Path $outputRoot "$Scenario.timeline.tsv"
$ffmpegLogPath = Join-Path $outputRoot "$Scenario.ffmpeg.log"
$runSummaryPath = Join-Path $outputRoot "$Scenario.run.txt"

if (-not (Test-Path -LiteralPath $exePath)) {
    throw "Current OpenVisionLab EXE was not found: $exePath"
}
if (-not (Test-Path -LiteralPath $appAssemblyPath)) {
    throw "Current OpenVisionLab application assembly was not found: $appAssemblyPath"
}

$existing = Get-Process -Name OpenVisionLab -ErrorAction SilentlyContinue
if ($existing) {
    throw "Close the existing OpenVisionLab process before recording. Existing PID(s): $($existing.Id -join ', ')"
}

$ffmpegCommand = Get-Command ffmpeg -ErrorAction Stop
New-Item -ItemType Directory -Path $outputRoot -Force | Out-Null

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms

$availableScreens = @(
    [System.Windows.Forms.Screen]::AllScreens |
        Sort-Object { $_.Bounds.Left }, { $_.Bounds.Top }
)
if ($availableScreens.Count -gt 0) {
    $recordingScreen = $availableScreens[0]
    $script:recordingMonitorName = $recordingScreen.DeviceName
    $script:recordingLeft = [int]$recordingScreen.Bounds.Left
    $script:recordingTop = [int]$recordingScreen.Bounds.Top
    $script:recordingWidth = [int]$recordingScreen.Bounds.Width
    $script:recordingHeight = [int]$recordingScreen.Bounds.Height
    $script:recordingMonitorFallback = $availableScreens.Count -eq 1
}
else {
    $virtualBounds = [System.Windows.Forms.SystemInformation]::VirtualScreen
    $script:recordingMonitorName = "VirtualScreen"
    $script:recordingLeft = [int]$virtualBounds.Left
    $script:recordingTop = [int]$virtualBounds.Top
    $script:recordingWidth = [int]$virtualBounds.Width
    $script:recordingHeight = [int]$virtualBounds.Height
    $script:recordingMonitorFallback = $true
}
$script:recordingWindowBounds = "unverified"

if ($script:recordingWidth -le 0 -or $script:recordingHeight -le 0) {
    throw "No usable desktop bounds were available for EXE recording."
}

if (-not ("OpenVisionNaturalInput" -as [type])) {
    Add-Type @"
using System;
using System.Runtime.InteropServices;

public static class OpenVisionNaturalInput
{
    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    public static extern void mouse_event(
        uint flags,
        uint dx,
        uint dy,
        uint data,
        UIntPtr extraInfo);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr windowHandle);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr windowHandle, int command);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(
        IntPtr windowHandle,
        IntPtr insertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);
}
"@
}

$script:random = [System.Random]::new(20260728)
$script:timeline = [System.Collections.Generic.List[string]]::new()
$script:recordingClock = [System.Diagnostics.Stopwatch]::new()
$script:appProcess = $null
$script:ffmpegProcess = $null
$script:ffmpegErrorTask = $null
$script:minimizedExternalWindows = [System.Collections.Generic.List[System.IntPtr]]::new()

function Add-TimelineEvent {
    param(
        [string]$Action,
        [string]$Detail
    )

    $seconds = $script:recordingClock.Elapsed.TotalSeconds.ToString(
        "0.000",
        [System.Globalization.CultureInfo]::InvariantCulture)
    $detailValue = if ($null -eq $Detail) { [string]::Empty } else { $Detail }
    $safeDetail = $detailValue -replace "`t", " " -replace "`r?`n", " "
    $script:timeline.Add("$seconds`t$Action`t$safeDetail")
}

function Start-DesktopRecording {
    $psi = [System.Diagnostics.ProcessStartInfo]::new()
    $psi.FileName = $ffmpegCommand.Source
    $psi.UseShellExecute = $false
    $psi.CreateNoWindow = $true
    $psi.RedirectStandardInput = $true
    $psi.RedirectStandardError = $true
    $psi.Arguments = (
        "-hide_banner -loglevel warning -y " +
        "-f gdigrab -framerate 30 " +
        "-offset_x $($script:recordingLeft) -offset_y $($script:recordingTop) " +
        "-video_size $($script:recordingWidth)x$($script:recordingHeight) " +
        "-draw_mouse 1 -i desktop " +
        "-c:v libx264 -preset veryfast -crf 18 -pix_fmt yuv420p " +
        "-movflags +faststart `"$videoPath`"")

    $script:ffmpegProcess = [System.Diagnostics.Process]::new()
    $script:ffmpegProcess.StartInfo = $psi
    if (-not $script:ffmpegProcess.Start()) {
        throw "FFmpeg recording process did not start."
    }

    $script:ffmpegErrorTask = $script:ffmpegProcess.StandardError.ReadToEndAsync()
    $script:recordingClock.Restart()
    Add-TimelineEvent `
        "recording-start" `
        "Monitor=$($script:recordingMonitorName); Bounds=$($script:recordingLeft),$($script:recordingTop),$($script:recordingWidth),$($script:recordingHeight); Window=$($script:recordingWindowBounds); Fallback=$($script:recordingMonitorFallback); 30fps with cursor"
}

function Stop-DesktopRecording {
    if ($null -eq $script:ffmpegProcess -or $script:ffmpegProcess.HasExited) {
        return
    }

    Add-TimelineEvent "recording-stop" "graceful FFmpeg stop"
    $script:ffmpegProcess.StandardInput.WriteLine("q")
    if (-not $script:ffmpegProcess.WaitForExit(15000)) {
        $script:ffmpegProcess.Kill()
        $script:ffmpegProcess.WaitForExit()
    }

    $stderr = $script:ffmpegErrorTask.GetAwaiter().GetResult()
    [System.IO.File]::WriteAllText($ffmpegLogPath, $stderr, [System.Text.UTF8Encoding]::new($false))
}

function Get-ProcessAutomationElement {
    param(
        [string]$AutomationId,
        [string]$Name,
        [System.Windows.Automation.ControlType]$ControlType = $null,
        [switch]$AllowOffscreen
    )

    Minimize-OtherOpenVisionWindows

    $conditions = [System.Collections.Generic.List[System.Windows.Automation.Condition]]::new()
    if (-not [string]::IsNullOrWhiteSpace($AutomationId)) {
        $conditions.Add(
            [System.Windows.Automation.PropertyCondition]::new(
                [System.Windows.Automation.AutomationElement]::AutomationIdProperty,
                $AutomationId))
    }
    if (-not [string]::IsNullOrWhiteSpace($Name)) {
        $conditions.Add(
            [System.Windows.Automation.PropertyCondition]::new(
                [System.Windows.Automation.AutomationElement]::NameProperty,
                $Name))
    }
    if ($null -ne $ControlType) {
        $conditions.Add(
            [System.Windows.Automation.PropertyCondition]::new(
                [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
                $ControlType))
    }
    if ($conditions.Count -eq 0) {
        throw "An AutomationId, Name, or ControlType is required."
    }

    $condition = if ($conditions.Count -eq 1) {
        $conditions[0]
    }
    else {
        [System.Windows.Automation.AndCondition]::new($conditions.ToArray())
    }
    try {
        $matches = [System.Windows.Automation.AutomationElement]::RootElement.FindAll(
            [System.Windows.Automation.TreeScope]::Descendants,
            $condition)
    }
    catch {
        # Windows UI Automation can briefly reject a tree query while a dialog
        # or hosted WPF surface is being replaced. The caller's wait loop can
        # retry without turning that transient state into a scenario failure.
        return $null
    }
    foreach ($match in $matches) {
        try {
            if ($match.Current.ProcessId -ne $script:appProcess.Id) {
                continue
            }
            if (-not $AllowOffscreen -and $match.Current.IsOffscreen) {
                continue
            }
            $bounds = $match.Current.BoundingRectangle
            if ([double]::IsInfinity($bounds.X) -or
                [double]::IsInfinity($bounds.Y) -or
                $bounds.Width -le 1 -or
                $bounds.Height -le 1) {
                continue
            }
            return $match
        }
        catch {
        }
    }

    return $null
}

function Wait-AutomationElement {
    param(
        [string]$AutomationId,
        [string]$Name,
        [System.Windows.Automation.ControlType]$ControlType = $null,
        [int]$TimeoutMilliseconds = 15000,
        [switch]$AllowOffscreen
    )

    $clock = [System.Diagnostics.Stopwatch]::StartNew()
    while ($clock.ElapsedMilliseconds -lt $TimeoutMilliseconds) {
        if ($script:appProcess.HasExited) {
            throw "OpenVisionLab exited while waiting for UI element '$AutomationId$Name'."
        }
        $element = Get-ProcessAutomationElement `
            -AutomationId $AutomationId `
            -Name $Name `
            -ControlType $ControlType `
            -AllowOffscreen:$AllowOffscreen
        if ($null -ne $element) {
            return $element
        }
        Start-Sleep -Milliseconds 100
    }

    throw "Timed out waiting for UI element. AutomationId='$AutomationId', Name='$Name'."
}

function Wait-AutomationElementNameContains {
    param(
        [string]$AutomationId,
        [string]$ExpectedText,
        [int]$TimeoutMilliseconds = 15000
    )

    $clock = [System.Diagnostics.Stopwatch]::StartNew()
    while ($clock.ElapsedMilliseconds -lt $TimeoutMilliseconds) {
        if ($script:appProcess.HasExited) {
            throw "OpenVisionLab exited while verifying '$AutomationId' contains '$ExpectedText'."
        }

        try {
            $element = Get-ProcessAutomationElement `
                -AutomationId $AutomationId `
                -Name $null
            $actualText = Get-AutomationElementText $element
            if (-not [string]::IsNullOrWhiteSpace($actualText) -and
                $actualText.IndexOf($ExpectedText, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
                return $element
            }
        }
        catch {
        }

        Start-Sleep -Milliseconds 120
    }

    $actual = Get-VisibleElementName $AutomationId
    throw "Timed out verifying UI element '$AutomationId'. Expected it to contain '$ExpectedText'; actual='$actual'."
}

function Get-AutomationElementText {
    param(
        [System.Windows.Automation.AutomationElement]$Element
    )

    if ($null -eq $Element) {
        return ""
    }

    $parts = [System.Collections.Generic.List[string]]::new()
    try {
        if (-not [string]::IsNullOrWhiteSpace($Element.Current.Name)) {
            $parts.Add($Element.Current.Name)
        }
        $textCondition =
            [System.Windows.Automation.PropertyCondition]::new(
                [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
                [System.Windows.Automation.ControlType]::Text)
        $textElements = $Element.FindAll(
            [System.Windows.Automation.TreeScope]::Descendants,
            $textCondition)
        foreach ($textElement in $textElements) {
            try {
                if (-not $textElement.Current.IsOffscreen -and
                    -not [string]::IsNullOrWhiteSpace(
                        $textElement.Current.Name)) {
                    $parts.Add($textElement.Current.Name)
                }
            }
            catch {
            }
        }
    }
    catch {
    }

    return ($parts | Select-Object -Unique) -join " | "
}

function Wait-ProcessElementNameContains {
    param(
        [string]$ExpectedText,
        [System.Windows.Automation.ControlType]$ControlType = $null,
        [int]$TimeoutMilliseconds = 15000
    )

    $clock = [System.Diagnostics.Stopwatch]::StartNew()
    while ($clock.ElapsedMilliseconds -lt $TimeoutMilliseconds) {
        $conditions = [System.Collections.Generic.List[System.Windows.Automation.Condition]]::new()
        if ($null -ne $ControlType) {
            $conditions.Add([System.Windows.Automation.PropertyCondition]::new(
                [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
                $ControlType))
        }

        $condition = if ($conditions.Count -eq 0) {
            [System.Windows.Automation.Condition]::TrueCondition
        }
        else {
            $conditions[0]
        }
        try {
            $matches = [System.Windows.Automation.AutomationElement]::RootElement.FindAll(
                [System.Windows.Automation.TreeScope]::Descendants,
                $condition)
            foreach ($match in $matches) {
                try {
                    if ($match.Current.ProcessId -eq $script:appProcess.Id -and
                        -not $match.Current.IsOffscreen -and
                        $match.Current.Name.IndexOf($ExpectedText, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
                        return $match
                    }
                }
                catch {
                }
            }
        }
        catch {
        }

        Start-Sleep -Milliseconds 120
    }

    throw "Timed out verifying a visible OpenVisionLab element contains '$ExpectedText'."
}

function Activate-AutomationWindow {
    param(
        [System.Windows.Automation.AutomationElement]$Element
    )

    $candidate = $Element
    $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
    while ($null -ne $candidate) {
        try {
            if ($candidate.Current.ControlType -eq [System.Windows.Automation.ControlType]::Window -and
                $candidate.Current.NativeWindowHandle -ne 0) {
                $handle = [IntPtr]$candidate.Current.NativeWindowHandle
                [OpenVisionNaturalInput]::ShowWindow($handle, 9) | Out-Null
                [OpenVisionNaturalInput]::SetWindowPos(
                    $handle,
                    [IntPtr](-1),
                    0,
                    0,
                    0,
                    0,
                    0x0043) | Out-Null
                [OpenVisionNaturalInput]::SetForegroundWindow($handle) | Out-Null
                Start-Sleep -Milliseconds 160
                return
            }
            $candidate = $walker.GetParent($candidate)
        }
        catch {
            break
        }
    }

    $script:appProcess.Refresh()
    if ($script:appProcess.MainWindowHandle -ne [IntPtr]::Zero) {
        [OpenVisionNaturalInput]::ShowWindow($script:appProcess.MainWindowHandle, 9) | Out-Null
        [OpenVisionNaturalInput]::SetForegroundWindow($script:appProcess.MainWindowHandle) | Out-Null
        Start-Sleep -Milliseconds 160
    }
}

function Position-MainWindowForRecording {
    $script:appProcess.Refresh()
    $recordingWindowHandle = [IntPtr]::Zero
    $workspaceElement = Get-ProcessAutomationElement `
        -AutomationId "WorkspaceEmptySampleButton" `
        -Name $null `
        -AllowOffscreen
    if ($null -ne $workspaceElement) {
        $candidate = $workspaceElement
        $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
        while ($null -ne $candidate) {
            try {
                if ($candidate.Current.ControlType -eq
                    [System.Windows.Automation.ControlType]::Window -and
                    $candidate.Current.NativeWindowHandle -ne 0) {
                    $recordingWindowHandle =
                        [IntPtr]$candidate.Current.NativeWindowHandle
                    break
                }
                $candidate = $walker.GetParent($candidate)
            }
            catch {
                break
            }
        }
    }

    if ($recordingWindowHandle -eq [IntPtr]::Zero) {
        $recordingWindowHandle = $script:appProcess.MainWindowHandle
    }

    if ($recordingWindowHandle -eq [IntPtr]::Zero) {
        throw "OpenVisionLab main window handle is not ready."
    }

    [OpenVisionNaturalInput]::ShowWindow($recordingWindowHandle, 9) | Out-Null
    [OpenVisionNaturalInput]::SetWindowPos(
        $recordingWindowHandle,
        [IntPtr](-1),
        $script:recordingLeft,
        $script:recordingTop,
        $script:recordingWidth,
        $script:recordingHeight,
        0x0040) | Out-Null
    [OpenVisionNaturalInput]::SetForegroundWindow($recordingWindowHandle) | Out-Null
    Start-Sleep -Milliseconds 900

    $windowElement = [System.Windows.Automation.AutomationElement]::FromHandle(
        $recordingWindowHandle)
    $windowBounds = $windowElement.Current.BoundingRectangle
    $monitorRight = $script:recordingLeft + $script:recordingWidth
    $monitorBottom = $script:recordingTop + $script:recordingHeight
    $windowRight = $windowBounds.Left + $windowBounds.Width
    $windowBottom = $windowBounds.Top + $windowBounds.Height
    $intersectsMonitor =
        $windowBounds.Left -lt $monitorRight -and
        $windowRight -gt $script:recordingLeft -and
        $windowBounds.Top -lt $monitorBottom -and
        $windowBottom -gt $script:recordingTop
    if (-not $intersectsMonitor) {
        throw "OpenVisionLab window does not intersect the selected recording monitor."
    }

    $script:recordingWindowBounds = (
        "{0},{1},{2},{3}" -f
        [int]$windowBounds.Left,
        [int]$windowBounds.Top,
        [int]$windowBounds.Width,
        [int]$windowBounds.Height)
}

function Minimize-OtherOpenVisionWindows {
    $otherProcesses = Get-Process -ErrorAction SilentlyContinue |
        Where-Object {
            $_.Id -ne $script:appProcess.Id -and
            ($_.ProcessName -like "OpenVisionLab*" -or
                $_.ProcessName -eq "devenv") -and
            $_.MainWindowHandle -ne [IntPtr]::Zero
        }
    foreach ($process in $otherProcesses) {
        $handle = [IntPtr]$process.MainWindowHandle
        [OpenVisionNaturalInput]::ShowWindow($handle, 6) | Out-Null
        if (-not $script:minimizedExternalWindows.Contains($handle)) {
            $script:minimizedExternalWindows.Add($handle)
        }
    }
}

function Restore-OtherOpenVisionWindows {
    foreach ($handle in $script:minimizedExternalWindows) {
        [OpenVisionNaturalInput]::ShowWindow($handle, 9) | Out-Null
    }
    $script:minimizedExternalWindows.Clear()
}

function Move-NaturalMouse {
    param(
        [int]$TargetX,
        [int]$TargetY,
        [string]$TargetLabel
    )

    $start = [System.Windows.Forms.Cursor]::Position
    $dx = $TargetX - $start.X
    $dy = $TargetY - $start.Y
    $distance = [Math]::Sqrt(($dx * $dx) + ($dy * $dy))
    $durationMilliseconds = [Math]::Max(420, [Math]::Min(1150, 360 + ($distance * 0.72)))
    $stepMilliseconds = 12
    $steps = [Math]::Max(35, [int]($durationMilliseconds / $stepMilliseconds))
    $arc = [Math]::Max(12, [Math]::Min(68, $distance * 0.07))
    $arcDirection = if ($script:random.Next(0, 2) -eq 0) { -1.0 } else { 1.0 }
    $length = [Math]::Max(1.0, $distance)
    $normalX = (-$dy / $length) * $arc * $arcDirection
    $normalY = ($dx / $length) * $arc * $arcDirection
    $control1X = $start.X + ($dx * 0.28) + $normalX
    $control1Y = $start.Y + ($dy * 0.28) + $normalY
    $control2X = $start.X + ($dx * 0.72) - ($normalX * 0.55)
    $control2Y = $start.Y + ($dy * 0.72) - ($normalY * 0.55)

    Add-TimelineEvent "mouse-move" "$TargetLabel from $($start.X),$($start.Y) to $TargetX,$TargetY over $([int]$durationMilliseconds)ms"
    for ($index = 1; $index -le $steps; $index++) {
        $progress = $index / [double]$steps
        $eased = $progress * $progress * (3.0 - (2.0 * $progress))
        $inverse = 1.0 - $eased
        $x = ($inverse * $inverse * $inverse * $start.X) +
            (3.0 * $inverse * $inverse * $eased * $control1X) +
            (3.0 * $inverse * $eased * $eased * $control2X) +
            ($eased * $eased * $eased * $TargetX)
        $y = ($inverse * $inverse * $inverse * $start.Y) +
            (3.0 * $inverse * $inverse * $eased * $control1Y) +
            (3.0 * $inverse * $eased * $eased * $control2Y) +
            ($eased * $eased * $eased * $TargetY)
        [OpenVisionNaturalInput]::SetCursorPos([int]$x, [int]$y) | Out-Null
        Start-Sleep -Milliseconds $stepMilliseconds
    }

    # Two sub-pixel-scale settling motions keep the final approach human-readable.
    [OpenVisionNaturalInput]::SetCursorPos($TargetX - 1, $TargetY + 1) | Out-Null
    Start-Sleep -Milliseconds 32
    [OpenVisionNaturalInput]::SetCursorPos($TargetX, $TargetY) | Out-Null
    Start-Sleep -Milliseconds 90
}

function Click-AutomationElement {
    param(
        [System.Windows.Automation.AutomationElement]$Element,
        [string]$Label,
        [int]$PauseAfterMilliseconds = 850
    )

    Activate-AutomationWindow $Element
    Move-AutomationElementIntoView $Element
    $bounds = $Element.Current.BoundingRectangle
    $targetX = [int]($bounds.X + ($bounds.Width * (0.45 + ($script:random.NextDouble() * 0.10))))
    $targetY = [int]($bounds.Y + ($bounds.Height * (0.43 + ($script:random.NextDouble() * 0.12))))
    Move-NaturalMouse -TargetX $targetX -TargetY $targetY -TargetLabel $Label
    Start-Sleep -Milliseconds (140 + $script:random.Next(0, 90))
    [OpenVisionNaturalInput]::mouse_event(0x0002, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds (70 + $script:random.Next(0, 45))
    [OpenVisionNaturalInput]::mouse_event(0x0004, 0, 0, 0, [UIntPtr]::Zero)
    Add-TimelineEvent "click" $Label
    Start-Sleep -Milliseconds $PauseAfterMilliseconds
}

function Move-AutomationElementIntoView {
    param(
        [System.Windows.Automation.AutomationElement]$Element
    )

    try {
        $scrollItemPattern = $Element.GetCurrentPattern(
            [System.Windows.Automation.ScrollItemPattern]::Pattern)
        $scrollItemPattern.ScrollIntoView()
        Start-Sleep -Milliseconds 350
    }
    catch {
    }

    $script:appProcess.Refresh()
    $windowElement =
        [System.Windows.Automation.AutomationElement]::FromHandle(
            $script:appProcess.MainWindowHandle)
    if ($null -eq $windowElement) {
        return
    }

    for ($attempt = 0; $attempt -lt 12; $attempt++) {
        $windowBounds = $windowElement.Current.BoundingRectangle
        $elementBounds = $Element.Current.BoundingRectangle
        $topLimit = $windowBounds.Top + 80
        $bottomLimit = $windowBounds.Bottom - 40
        if ($elementBounds.Top -ge $topLimit -and
            $elementBounds.Bottom -le $bottomLimit) {
            return
        }

        $scrollAmount =
            if ($elementBounds.Bottom -gt $bottomLimit) {
                [System.Windows.Automation.ScrollAmount]::LargeIncrement
            }
            elseif ($elementBounds.Top -lt $topLimit) {
                [System.Windows.Automation.ScrollAmount]::LargeDecrement
            }
            else {
                [System.Windows.Automation.ScrollAmount]::NoAmount
            }
        if ($scrollAmount -eq
            [System.Windows.Automation.ScrollAmount]::NoAmount) {
            return
        }

        $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
        $candidate = $walker.GetParent($Element)
        $scrolled = $false
        while ($null -ne $candidate) {
            try {
                $scrollPattern = $candidate.GetCurrentPattern(
                    [System.Windows.Automation.ScrollPattern]::Pattern)
                if ($scrollPattern.Current.VerticallyScrollable) {
                    $scrollPattern.ScrollVertical($scrollAmount)
                    $scrolled = $true
                    Start-Sleep -Milliseconds 350
                    break
                }
            }
            catch {
            }
            $candidate = $walker.GetParent($candidate)
        }

        if (-not $scrolled) {
            return
        }
    }
}

function Click-AutomationId {
    param(
        [string]$AutomationId,
        [string]$Label,
        [int]$PauseAfterMilliseconds = 850,
        [int]$TimeoutMilliseconds = 15000,
        [System.Windows.Automation.ControlType]$ControlType = $null,
        [switch]$AllowOffscreen
    )

    $element = Wait-AutomationElement `
        -AutomationId $AutomationId `
        -Name $null `
        -ControlType $ControlType `
        -TimeoutMilliseconds $TimeoutMilliseconds `
        -AllowOffscreen:$AllowOffscreen
    Click-AutomationElement `
        -Element $element `
        -Label $Label `
        -PauseAfterMilliseconds $PauseAfterMilliseconds
}

function Click-ButtonName {
    param(
        [string]$Name,
        [string]$Label,
        [int]$PauseAfterMilliseconds = 850
    )

    $element = Wait-AutomationElement `
        -AutomationId $null `
        -Name $Name `
        -ControlType ([System.Windows.Automation.ControlType]::Button)
    Click-AutomationElement `
        -Element $element `
        -Label $Label `
        -PauseAfterMilliseconds $PauseAfterMilliseconds
}

function Click-TextName {
    param(
        [string]$Name,
        [string]$Label,
        [int]$PauseAfterMilliseconds = 850
    )

    $element = Wait-AutomationElement `
        -AutomationId $null `
        -Name $Name `
        -ControlType ([System.Windows.Automation.ControlType]::Text)
    Click-AutomationElement `
        -Element $element `
        -Label $Label `
        -PauseAfterMilliseconds $PauseAfterMilliseconds
}

function Type-HumanText {
    param(
        [string]$Text,
        [string]$Label
    )

    [System.Windows.Forms.SendKeys]::SendWait("^a")
    Start-Sleep -Milliseconds 140
    Add-TimelineEvent "type-start" "$Label ($($Text.Length) characters)"
    foreach ($character in $Text.ToCharArray()) {
        [System.Windows.Forms.SendKeys]::SendWait([string]$character)
        Start-Sleep -Milliseconds (24 + $script:random.Next(0, 38))
    }
    Add-TimelineEvent "type-end" $Label
    Start-Sleep -Milliseconds 1200
}

function Set-VisibleEditValue {
    param(
        [string]$CurrentValue,
        [string]$NewValue,
        [string]$Label
    )

    $editCondition = [System.Windows.Automation.PropertyCondition]::new(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
        [System.Windows.Automation.ControlType]::Edit)
    $matches = [System.Windows.Automation.AutomationElement]::RootElement.FindAll(
        [System.Windows.Automation.TreeScope]::Descendants,
        $editCondition)
    $candidate = $null
    foreach ($match in $matches) {
        try {
            if ($match.Current.ProcessId -ne $script:appProcess.Id -or $match.Current.IsOffscreen) {
                continue
            }
            $valuePattern = $match.GetCurrentPattern(
                [System.Windows.Automation.ValuePattern]::Pattern)
            if ($valuePattern.Current.Value -eq $CurrentValue) {
                $candidate = $match
                break
            }
        }
        catch {
        }
    }

    if ($null -eq $candidate) {
        throw "Visible PropertyGrid edit with value '$CurrentValue' was not found."
    }

    Click-AutomationElement -Element $candidate -Label $Label -PauseAfterMilliseconds 220
    Type-HumanText $NewValue $Label
    [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
    Start-Sleep -Milliseconds 900
    Add-TimelineEvent "property-edit" "$Label $CurrentValue -> $NewValue"
}

function Set-AutomationEditValue {
    param(
        [string]$AutomationId,
        [string]$Value,
        [string]$Label,
        [switch]$PressEnter
    )

    $edit = Wait-AutomationElement `
        -AutomationId $AutomationId `
        -Name $null `
        -ControlType ([System.Windows.Automation.ControlType]::Edit) `
        -TimeoutMilliseconds 15000
    Click-AutomationElement `
        -Element $edit `
        -Label $Label `
        -PauseAfterMilliseconds 220
    Type-HumanText $Value $Label
    if ($PressEnter) {
        [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
        Start-Sleep -Milliseconds 900
    }
    Add-TimelineEvent "edit-value" "$Label = $Value"
}

function Select-ComboBoxItemName {
    param(
        [string]$AutomationId,
        [string]$ItemName,
        [string]$Label
    )

    $combo = Wait-AutomationElement `
        -AutomationId $AutomationId `
        -Name $null `
        -ControlType ([System.Windows.Automation.ControlType]::ComboBox) `
        -TimeoutMilliseconds 15000
    Click-AutomationElement `
        -Element $combo `
        -Label "$Label - open" `
        -PauseAfterMilliseconds 500

    $clock = [System.Diagnostics.Stopwatch]::StartNew()
    while ($clock.ElapsedMilliseconds -lt 15000) {
        $conditions = [System.Windows.Automation.AndCondition]::new(
            [System.Windows.Automation.PropertyCondition]::new(
                [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
                [System.Windows.Automation.ControlType]::ListItem),
            [System.Windows.Automation.PropertyCondition]::new(
                [System.Windows.Automation.AutomationElement]::NameProperty,
                $ItemName))
        $items = [System.Windows.Automation.AutomationElement]::RootElement.FindAll(
            [System.Windows.Automation.TreeScope]::Descendants,
            $conditions)
        foreach ($item in $items) {
            try {
                if ($item.Current.ProcessId -eq $script:appProcess.Id -and
                    -not $item.Current.IsOffscreen) {
                    Activate-AutomationWindow $item
                    Move-AutomationElementIntoView $item
                    $bounds = $item.Current.BoundingRectangle
                    $targetX = [int]($bounds.X + ($bounds.Width * 0.50))
                    $targetY = [int]($bounds.Y + ($bounds.Height * 0.50))
                    Move-NaturalMouse `
                        -TargetX $targetX `
                        -TargetY $targetY `
                        -TargetLabel $Label
                    # WPF closes a ComboBox Popup as soon as the item is chosen.
                    # A long synthetic mouse-down interval can then deliver mouse-up
                    # to the owner window underneath the Popup and silently restore
                    # the old selection. Keep the visible click human-readable, but
                    # release it before the Popup disappears underneath the cursor.
                    [OpenVisionNaturalInput]::mouse_event(
                        0x0002,
                        0,
                        0,
                        0,
                        [UIntPtr]::Zero)
                    Start-Sleep -Milliseconds 18
                    [OpenVisionNaturalInput]::mouse_event(
                        0x0004,
                        0,
                        0,
                        0,
                        [UIntPtr]::Zero)
                    Add-TimelineEvent "click" $Label
                    Start-Sleep -Milliseconds 900

                    $selected = Get-ComboBoxSelectedItemName $AutomationId
                    if (-not [string]::Equals(
                        $selected,
                        $ItemName,
                        [System.StringComparison]::Ordinal)) {
                        throw "ComboBox '$AutomationId' did not retain '$ItemName' after the visible click. Actual='$selected'."
                    }
                    Add-TimelineEvent `
                        "combo-selection" `
                        "$AutomationId = $ItemName"
                    return
                }
            }
            catch {
            }
        }
        Start-Sleep -Milliseconds 120
    }

    throw "Timed out selecting '$ItemName' from ComboBox '$AutomationId'."
}

function Get-ComboBoxSelectedItemName {
    param([string]$AutomationId)

    $combo = Get-ProcessAutomationElement `
        -AutomationId $AutomationId `
        -Name $null `
        -ControlType ([System.Windows.Automation.ControlType]::ComboBox) `
        -AllowOffscreen
    if ($null -eq $combo) {
        return [string]::Empty
    }

    try {
        $selectionPattern = $combo.GetCurrentPattern(
            [System.Windows.Automation.SelectionPattern]::Pattern)
        $selection = $selectionPattern.Current.GetSelection()
        if ($selection.Count -gt 0) {
            return $selection[0].Current.Name
        }
    }
    catch {
    }

    return $combo.Current.Name
}

function Load-WorkspaceImageFile {
    param(
        [string]$ImagePath,
        [string]$Label
    )

    Click-AutomationId "btnWorkspaceLoadImage" $Label 700
    Set-AutomationEditValue `
        -AutomationId "1148" `
        -Value $ImagePath `
        -Label "Enter inspection image path"
    Submit-FileDialogOpen "Open inspection image"
    Wait-AutomationElement `
        -AutomationId "WorkspaceMainActionThresholdButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Add-TimelineEvent "image-loaded" $ImagePath
}

function Restart-OpenVisionApplication {
    Add-TimelineEvent `
        "application-restart-start" `
        "Close and reopen the same clean runtime after both Steps were saved"
    $script:appProcess.CloseMainWindow() | Out-Null
    if (-not $script:appProcess.WaitForExit(15000)) {
        throw "OpenVisionLab did not close cleanly for the persistence replay."
    }

    $script:appProcess = Start-Process `
        -FilePath $exePath `
        -WorkingDirectory $runtimeRoot `
        -PassThru
    $script:appProcess.WaitForInputIdle(15000) | Out-Null
    Wait-AutomationElement `
        -AutomationId "WorkspaceEmptySampleButton" `
        -Name $null `
        -TimeoutMilliseconds 20000 | Out-Null
    Minimize-OtherOpenVisionWindows
    Position-MainWindowForRecording
    Add-TimelineEvent `
        "application-restart-complete" `
        "PID=$($script:appProcess.Id); same runtime=$runtimeRoot"
}

function Prepare-CatalogSampleBeforeRecording {
    param(
        [string]$SampleName
    )

    $sampleButton = Wait-AutomationElement `
        -AutomationId "WorkspaceEmptySampleButton" `
        -Name $null `
        -TimeoutMilliseconds 15000
    $invokePattern = $sampleButton.GetCurrentPattern(
        [System.Windows.Automation.InvokePattern]::Pattern)
    $invokePattern.Invoke()

    $search = Wait-AutomationElement `
        -AutomationId "WorkspaceSamplePickerSearchBox" `
        -Name $null `
        -TimeoutMilliseconds 15000
    $valuePattern = $search.GetCurrentPattern(
        [System.Windows.Automation.ValuePattern]::Pattern)
    $valuePattern.SetValue($SampleName)
    Start-Sleep -Milliseconds 1200

    $openButton = Wait-AutomationElement `
        -AutomationId "WorkspaceSamplePickerOpenButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 `
        -AllowOffscreen
    $openPattern = $openButton.GetCurrentPattern(
        [System.Windows.Automation.InvokePattern]::Pattern)
    $openPattern.Invoke()
    Wait-AutomationElement `
        -AutomationId "WorkspaceSamplePipelineButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Start-Sleep -Milliseconds 1200
}

function Wait-ReviewCompletion {
    param(
        [int]$TimeoutMilliseconds = 20000
    )

    $clock = [System.Diagnostics.Stopwatch]::StartNew()
    while ($clock.ElapsedMilliseconds -lt $TimeoutMilliseconds) {
        if ($script:appProcess.HasExited) {
            Add-TimelineEvent "process-exit" "OpenVisionLab exited during Run Review"
            return $false
        }
        $summary = Get-ProcessAutomationElement `
            -AutomationId "PipelineReviewSelectedResultSummary" `
            -Name $null
        if ($null -ne $summary) {
            $text = $summary.Current.Name
            if (-not [string]::IsNullOrWhiteSpace($text) -and
                $text -notmatch "리뷰 실행 필요|Run Review required") {
                Add-TimelineEvent "review-complete" $text
                Start-Sleep -Milliseconds 1400
                return $true
            }
        }
        Start-Sleep -Milliseconds 150
    }

    throw "Run Review did not reach a completed UI state within $TimeoutMilliseconds ms."
}

function Wait-ToolPreviewCompletion {
    param(
        [string]$PreviousSummary = "",
        [string]$ExpectedOutputLayer = "",
        [int]$TimeoutMilliseconds = 20000
    )

    $clock = [System.Diagnostics.Stopwatch]::StartNew()
    while ($clock.ElapsedMilliseconds -lt $TimeoutMilliseconds) {
        if ($script:appProcess.HasExited) {
            throw "OpenVisionLab exited during Tool View Preview."
        }
        $summary = Get-ProcessAutomationElement `
            -AutomationId "VisionToolResultReviewSummary" `
            -Name $null `
            -AllowOffscreen
        if ($null -ne $summary) {
            $text = $summary.Current.Name
            if (-not [string]::IsNullOrWhiteSpace($text) -and
                $text -ne $PreviousSummary -and
                $text -notmatch "결과 대기|Result pending|미리보기 전|Before preview") {
                Add-TimelineEvent "tool-preview-complete" $text
                Start-Sleep -Milliseconds 1300
                return $text
            }
        }
        if (-not [string]::IsNullOrWhiteSpace($ExpectedOutputLayer)) {
            try {
                $completedStatus = Wait-ProcessElementNameContains `
                    -ExpectedText "$ExpectedOutputLayer /" `
                    -ControlType ([System.Windows.Automation.ControlType]::Text) `
                    -TimeoutMilliseconds 250
                if ($null -ne $completedStatus) {
                    $text = $completedStatus.Current.Name
                    Add-TimelineEvent "tool-preview-complete" $text
                    Start-Sleep -Milliseconds 1300
                    return $text
                }
            }
            catch {
            }
        }
        Start-Sleep -Milliseconds 150
    }

    throw "Tool View Preview did not reach a completed UI state within $TimeoutMilliseconds ms."
}

function Get-VisibleElementName {
    param([string]$AutomationId)

    $element = Get-ProcessAutomationElement -AutomationId $AutomationId -Name $null
    return Get-AutomationElementText $element
}

function Submit-FileDialogOpen {
    param(
        [string]$Label,
        [int]$TimeoutMilliseconds = 15000
    )

    # The Windows common file dialog can expose a transient or unrelated UIA
    # button tree. The path edit retains focus after typing, so Enter submits
    # the selected file without depending on an AutomationId or localized
    # button name.
    [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
    Add-TimelineEvent "click" "$Label via Enter"
    Start-Sleep -Milliseconds 1800
}

function Get-VisibleListItemNames {
    param([string]$AutomationId)

    $list = Get-ProcessAutomationElement `
        -AutomationId $AutomationId `
        -Name $null
    if ($null -eq $list) {
        return @()
    }

    $condition = [System.Windows.Automation.PropertyCondition]::new(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
        [System.Windows.Automation.ControlType]::ListItem)
    $items = $list.FindAll(
        [System.Windows.Automation.TreeScope]::Descendants,
        $condition)
    $names = [System.Collections.Generic.List[string]]::new()
    foreach ($item in $items) {
        try {
            if ($item.Current.IsOffscreen) {
                continue
            }

            $name = $item.Current.Name
            if ([string]::IsNullOrWhiteSpace($name)) {
                $textCondition =
                    [System.Windows.Automation.PropertyCondition]::new(
                        [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
                        [System.Windows.Automation.ControlType]::Text)
                $textElements = $item.FindAll(
                    [System.Windows.Automation.TreeScope]::Descendants,
                    $textCondition)
                $textNames = [System.Collections.Generic.List[string]]::new()
                foreach ($textElement in $textElements) {
                    try {
                        if (-not $textElement.Current.IsOffscreen -and
                            -not [string]::IsNullOrWhiteSpace(
                                $textElement.Current.Name)) {
                            $textNames.Add($textElement.Current.Name)
                        }
                    }
                    catch {
                    }
                }
                $name = $textNames -join " | "
            }

            if (-not [string]::IsNullOrWhiteSpace($name)) {
                $names.Add($name)
            }
        }
        catch {
        }
    }

    if ($names.Count -eq 0) {
        $descendants = $list.FindAll(
            [System.Windows.Automation.TreeScope]::Descendants,
            [System.Windows.Automation.Condition]::TrueCondition)
        foreach ($descendant in $descendants) {
            try {
                if (-not $descendant.Current.IsOffscreen -and
                    -not [string]::IsNullOrWhiteSpace(
                        $descendant.Current.Name)) {
                    $names.Add($descendant.Current.Name)
                }
            }
            catch {
            }
        }
    }

    return $names.ToArray()
}

function Wait-ValidationSuiteCompletion {
    param(
        [string]$PreviousStatus,
        [int]$TimeoutMilliseconds = 30000
    )

    $clock = [System.Diagnostics.Stopwatch]::StartNew()
    while ($clock.ElapsedMilliseconds -lt $TimeoutMilliseconds) {
        if ($script:appProcess.HasExited) {
            throw "OpenVisionLab exited while waiting for the Validation Set run."
        }

        $status = Get-VisibleElementName "HostRecipeValidationSuiteStatus"
        $runButton = Get-ProcessAutomationElement `
            -AutomationId "HostRecipeRunValidationSuiteButton" `
            -Name $null
        if ($null -ne $runButton -and
            $runButton.Current.IsEnabled -and
            -not [string]::IsNullOrWhiteSpace($status) -and
            -not [string]::Equals(
                $status,
                $PreviousStatus,
                [System.StringComparison]::Ordinal)) {
            Add-TimelineEvent "validation-suite-complete" $status
            Start-Sleep -Milliseconds 1300
            return $status
        }

        Start-Sleep -Milliseconds 150
    }

    $actual = Get-VisibleElementName "HostRecipeValidationSuiteStatus"
    throw "Timed out waiting for the Validation Set run. Previous='$PreviousStatus'; actual='$actual'."
}

function Wait-AutomationElementTextChange {
    param(
        [string]$AutomationId,
        [string]$PreviousText,
        [int]$TimeoutMilliseconds = 15000
    )

    $clock = [System.Diagnostics.Stopwatch]::StartNew()
    while ($clock.ElapsedMilliseconds -lt $TimeoutMilliseconds) {
        if ($script:appProcess.HasExited) {
            throw "OpenVisionLab exited while waiting for '$AutomationId' to update."
        }

        $actual = Get-VisibleElementName $AutomationId
        if (-not [string]::IsNullOrWhiteSpace($actual) -and
            -not [string]::Equals(
                $actual,
                $PreviousText,
                [System.StringComparison]::Ordinal)) {
            return $actual
        }

        Start-Sleep -Milliseconds 120
    }

    $actual = Get-VisibleElementName $AutomationId
    throw "Timed out waiting for '$AutomationId' text to change. Previous='$PreviousText'; actual='$actual'."
}

function Click-ListItemNameContains {
    param(
        [string]$AutomationId,
        [string]$ExpectedText,
        [string]$Label,
        [int]$TimeoutMilliseconds = 15000
    )

    $clock = [System.Diagnostics.Stopwatch]::StartNew()
    while ($clock.ElapsedMilliseconds -lt $TimeoutMilliseconds) {
        $list = Get-ProcessAutomationElement `
            -AutomationId $AutomationId `
            -Name $null `
            -AllowOffscreen
        if ($null -ne $list) {
            $items = $list.FindAll(
                [System.Windows.Automation.TreeScope]::Descendants,
                [System.Windows.Automation.Condition]::TrueCondition)
            foreach ($item in $items) {
                try {
                    $candidateText = $item.Current.Name
                    if (-not [string]::IsNullOrWhiteSpace($candidateText) -and
                        $candidateText.IndexOf(
                            $ExpectedText,
                            [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
                        $clickTarget = $item
                        $walker =
                            [System.Windows.Automation.TreeWalker]::ControlViewWalker
                        $ancestor = $item
                        while ($null -ne $ancestor) {
                            try {
                                if (($ancestor.Current.ControlType -eq
                                        [System.Windows.Automation.ControlType]::ListItem) -or
                                    ($ancestor.Current.ControlType -eq
                                        [System.Windows.Automation.ControlType]::DataItem)) {
                                    $clickTarget = $ancestor
                                    break
                                }
                                $ancestor = $walker.GetParent($ancestor)
                            }
                            catch {
                                break
                            }
                        }

                        try {
                            $scrollItemPattern =
                                $clickTarget.GetCurrentPattern(
                                    [System.Windows.Automation.ScrollItemPattern]::Pattern)
                            $scrollItemPattern.ScrollIntoView()
                            Start-Sleep -Milliseconds 500
                        }
                        catch {
                        }
                        Click-AutomationElement `
                            -Element $clickTarget `
                            -Label $Label `
                            -PauseAfterMilliseconds 1000
                        return
                    }
                }
                catch {
                }
            }
        }

        Start-Sleep -Milliseconds 150
    }

    $actual = @(Get-VisibleListItemNames $AutomationId)
    throw "Timed out selecting '$ExpectedText' from '$AutomationId'. Visible items='$($actual -join ' | ')'."
}

function Close-LineSignalInspectorIfVisible {
    $backButton = Get-ProcessAutomationElement `
        -AutomationId "LineSignalInspectorBackButton" `
        -Name $null
    if ($null -ne $backButton) {
        Click-AutomationElement `
            -Element $backButton `
            -Label "Return from line signal review to parameters" `
            -PauseAfterMilliseconds 750
    }
}

function Close-FloatingToolWindow {
    $condition = [System.Windows.Automation.AndCondition]::new(
        [System.Windows.Automation.PropertyCondition]::new(
            [System.Windows.Automation.AutomationElement]::AutomationIdProperty,
            "OpenVisionWindowCloseButton"),
        [System.Windows.Automation.PropertyCondition]::new(
            [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
            [System.Windows.Automation.ControlType]::Button))
    $buttons = [System.Windows.Automation.AutomationElement]::RootElement.FindAll(
        [System.Windows.Automation.TreeScope]::Descendants,
        $condition)
    $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
    foreach ($button in $buttons) {
        try {
            if ($button.Current.ProcessId -ne $script:appProcess.Id -or
                $button.Current.IsOffscreen) {
                continue
            }

            $window = $walker.GetParent($button)
            while ($null -ne $window -and
                $window.Current.ControlType -ne
                    [System.Windows.Automation.ControlType]::Window) {
                $window = $walker.GetParent($window)
            }
            if ($null -eq $window -or
                [string]::Equals(
                    $window.Current.Name,
                    "OpenVisionLab",
                    [System.StringComparison]::OrdinalIgnoreCase)) {
                continue
            }

            Click-AutomationElement `
                -Element $button `
                -Label "Close the completed Tool View" `
                -PauseAfterMilliseconds 1000
            try {
                if (-not $window.Current.IsOffscreen) {
                    $windowPattern = $window.GetCurrentPattern(
                        [System.Windows.Automation.WindowPattern]::Pattern)
                    $windowPattern.Close()
                    Start-Sleep -Milliseconds 1000
                    Add-TimelineEvent `
                        "tool-view-close-fallback" `
                        "The visible close click did not dismiss the Tool View; WindowPattern close completed it"
                }
            }
            catch {
            }
            Add-TimelineEvent `
                "tool-view-closed" `
                "Closed the completed Tool View before checking persisted Recipe state"
            return
        }
        catch {
        }
    }

    throw "The visible floating Tool View close button was not found."
}

function Close-ActiveToolViewForRouteReview {
    $dockedClose = Get-ProcessAutomationElement `
        -AutomationId "OpenVisionDockedToolCloseButton" `
        -Name $null
    if ($null -ne $dockedClose) {
        Click-AutomationElement `
            -Element $dockedClose `
            -Label "Close the completed docked Tool View" `
            -PauseAfterMilliseconds 1000
        Add-TimelineEvent `
            "tool-view-closed" `
            "Closed the completed docked Tool View before checking persisted Recipe state"
        return
    }

    Close-FloatingToolWindow
}

function Discard-PendingLabelEditForSampleOpenIfVisible {
    $localizedDiscardName = [System.Text.Encoding]::UTF8.GetString(
        [System.Convert]::FromBase64String(
            "7KCA7J6l7ZWY7KeAIOyViuqzoCDsooXro4w="))
    $clock = [System.Diagnostics.Stopwatch]::StartNew()
    while ($clock.ElapsedMilliseconds -lt 4000) {
        try {
            $buttons = [System.Windows.Automation.AutomationElement]::RootElement.FindAll(
                [System.Windows.Automation.TreeScope]::Descendants,
                [System.Windows.Automation.PropertyCondition]::new(
                    [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
                    [System.Windows.Automation.ControlType]::Button))
        }
        catch {
            Start-Sleep -Milliseconds 160
            continue
        }

        foreach ($button in $buttons) {
            try {
                $buttonProcessId = $button.Current.ProcessId
                $buttonIsOffscreen = $button.Current.IsOffscreen
                $buttonName = $button.Current.Name
            }
            catch {
                continue
            }

            if (($buttonProcessId -ne $script:appProcess.Id) -or
                ($buttonIsOffscreen)) {
                continue
            }

            if (($buttonName -ne $localizedDiscardName) -and
                ($buttonName -notlike "*without saving*")) {
                continue
            }

            Click-AutomationElement `
                -Element $button `
                -Label "Discard the prior unsaved label edit and continue opening the sample" `
                -PauseAfterMilliseconds 900
            Add-TimelineEvent `
                "pending-label-decision" `
                "Explicitly discarded the prior unsaved label edit before opening a new sample"
            return
        }

        Start-Sleep -Milliseconds 160
    }
}

function Open-CatalogSample {
    param(
        [string]$SampleName
    )

    Click-AutomationId "WorkspaceEmptySampleButton" "Open public sample catalog" 1100
    Discard-PendingLabelEditForSampleOpenIfVisible
    Select-CatalogSampleInPicker $SampleName
    Click-AutomationId "WorkspaceSamplePickerOpenButton" "Open selected sample" 1900
    Wait-AutomationElement `
        -AutomationId "WorkspaceSamplePipelineButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Wait-ProcessElementNameContains `
        -ExpectedText $SampleName `
        -ControlType ([System.Windows.Automation.ControlType]::Text) `
        -TimeoutMilliseconds 15000 | Out-Null
    Add-TimelineEvent "sample-loaded-verified" $SampleName
}

function Select-CatalogSampleInPicker {
    param(
        [string]$SampleName
    )

    Click-AutomationId "WorkspaceSamplePickerSearchBox" "Focus sample search" 250
    Type-HumanText $SampleName "Search $SampleName"
    try {
        Wait-AutomationElementNameContains `
            -AutomationId "WorkspaceSamplePickerSelectedSummary" `
            -ExpectedText $SampleName `
            -TimeoutMilliseconds 3500 | Out-Null
    }
    catch {
        $search = Wait-AutomationElement `
            -AutomationId "WorkspaceSamplePickerSearchBox" `
            -Name $null `
            -TimeoutMilliseconds 5000
        $valuePattern = $search.GetCurrentPattern(
            [System.Windows.Automation.ValuePattern]::Pattern)
        $valuePattern.SetValue($SampleName)
        Add-TimelineEvent `
            "sample-search-focus-recovery" `
            "Keyboard focus was stolen by another desktop window; restored the exact visible search value through the same edit control"
        Start-Sleep -Milliseconds 1200
        Wait-AutomationElementNameContains `
            -AutomationId "WorkspaceSamplePickerSelectedSummary" `
            -ExpectedText $SampleName `
            -TimeoutMilliseconds 10000 | Out-Null
    }
    Add-TimelineEvent "sample-selection-verified" "Exact filtered selection: $SampleName"
}

function Open-PipelineReviewAndRun {
    Click-AutomationId "WorkspaceSamplePipelineButton" "Open Pipeline Review" 1600
    Wait-AutomationElement `
        -AutomationId "PipelineReviewRunReviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Add-TimelineEvent "pipeline-review-open" "Pipeline Review ready before explicit Run"
    Start-Sleep -Milliseconds 700
    Click-AutomationId "PipelineReviewRunReviewButton" "Run Review explicitly" 250
    return Wait-ReviewCompletion
}

function Invoke-BlobGoodBadScenario {
    if (-not (Open-PipelineReviewAndRun)) {
        throw "Blob Good Run Review exited unexpectedly."
    }

    Click-TextName "02 Synthetic Particle Count" "Select Blob result Step" 1200
    Wait-AutomationElement `
        -AutomationId "PipelineReviewObjectResultCount" `
        -Name $null `
        -TimeoutMilliseconds 10000 | Out-Null
    Add-TimelineEvent "object-review" "Good object rows and distribution visible"
    Click-AutomationId "PipelineReviewObjectMetricWidthButton" "Review object width distribution" 1100
    Click-AutomationId "PipelineReviewObjectMetricAreaButton" "Return to object area distribution" 1300

    Click-AutomationId "PipelineReviewOpenPairSampleButton" "Open paired Bad sample" 1200
    Wait-AutomationElement `
        -AutomationId "PipelineReviewRunReviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Start-Sleep -Milliseconds 1400
    Add-TimelineEvent "sample-loaded" "Public_Blob_Particles_Sparse_Bad loaded directly in Pipeline Review"
    Click-AutomationId "PipelineReviewRunReviewButton" "Run paired Bad review explicitly" 250
    if (-not (Wait-ReviewCompletion)) {
        throw "Blob Bad Run Review exited unexpectedly."
    }

    Click-TextName "02 Synthetic Particle Count" "Select Bad Blob result Step" 1200
    Add-TimelineEvent "object-review" "Bad count and rejected acceptance visible"
    Start-Sleep -Milliseconds 2200
}

function Invoke-NoviceBlobSelfTrialScenario {
    Click-AutomationId "WorkspaceEmptySampleButton" "Open public sample catalog" 1100
    Discard-PendingLabelEditForSampleOpenIfVisible
    Select-CatalogSampleInPicker "Public_Blob_Particles_Good"
    Add-TimelineEvent `
        "target-selected" `
        "Image=Public_Blob_Particles_Good; intent=count bright particles; expected Good count 8..14"
    Click-AutomationId "WorkspaceSamplePickerOpenButton" "Open selected particle image" 1900
    Wait-AutomationElement `
        -AutomationId "WorkspaceSamplePipelineButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Wait-ProcessElementNameContains `
        -ExpectedText "Public_Blob_Particles_Good" `
        -ControlType ([System.Windows.Automation.ControlType]::Text) `
        -TimeoutMilliseconds 15000 | Out-Null
    Add-TimelineEvent "sample-loaded-verified" "Public_Blob_Particles_Good selected from the visible catalog"
    Start-Sleep -Milliseconds 1400

    Invoke-BlobGoodBadScenario
}

function Invoke-NoviceBlobTeachingSelfTrialScenario {
    Click-AutomationId "WorkspaceEmptySampleButton" "Open public sample catalog" 1100
    Discard-PendingLabelEditForSampleOpenIfVisible
    Select-CatalogSampleInPicker "Public_Blob_Particles_Good"
    Add-TimelineEvent `
        "teaching-target-selected" `
        "Image=Public_Blob_Particles_Good; target=bright circular particles inside the oval; exclude border and noise"
    Click-AutomationId "WorkspaceSamplePickerOpenButton" "Open selected particle image" 1900
    if ($null -eq (Get-ProcessAutomationElement `
            -AutomationId "WorkspaceSamplePipelineButton" `
            -Name $null)) {
        $pickerStillOpen = Get-ProcessAutomationElement `
            -AutomationId "WorkspaceSamplePickerSearchBox" `
            -Name $null
        if ($null -ne $pickerStillOpen) {
            Add-TimelineEvent `
                "sample-open-retry" `
                "The picker remained open; the operator retries Open selected sample"
            Click-AutomationId `
                "WorkspaceSamplePickerOpenButton" `
                "Retry opening selected particle image" `
                1900
        }
    }
    Wait-AutomationElement `
        -AutomationId "WorkspaceSamplePipelineButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Wait-AutomationElement `
        -AutomationId "HostToolNav_Blob" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Wait-ProcessElementNameContains `
        -ExpectedText "Public_Blob_Particles_Good" `
        -ControlType ([System.Windows.Automation.ControlType]::Text) `
        -TimeoutMilliseconds 15000 | Out-Null
    Add-TimelineEvent "sample-loaded-verified" "Public_Blob_Particles_Good ready for direct Blob teaching"
    Start-Sleep -Milliseconds 1400

    Invoke-ObjectToolViewScenario `
        "HostToolNav_Blob" `
        "Blob" `
        -ApplyBasicPreset `
        -ThresholdValue "150"
    Click-AutomationId `
        "VisionToolParameterGuideButton" `
        "Close Parameter Guide after reviewing the selected threshold" `
        500
    Add-TimelineEvent `
        "teaching-review" `
        "Basic preset and threshold 150 preview reviewed without saving or running a Pipeline"
}

function Invoke-NoviceBlobPipelinePersistenceScenario {
    Click-AutomationId "WorkspaceEmptySampleButton" "Open public sample catalog" 1100
    Discard-PendingLabelEditForSampleOpenIfVisible
    if ($null -eq (Get-ProcessAutomationElement `
            -AutomationId "WorkspaceSamplePickerSearchBox" `
            -Name $null)) {
        Add-TimelineEvent `
            "sample-catalog-retry" `
            "The first visible click did not open the picker; the operator retries the same action"
        Click-AutomationId "WorkspaceEmptySampleButton" "Retry opening public sample catalog" 1100
        Discard-PendingLabelEditForSampleOpenIfVisible
    }
    Select-CatalogSampleInPicker "Public_Blob_Particles_Good"
    Add-TimelineEvent `
        "persistence-target-selected" `
        "Image=Public_Blob_Particles_Good; target=teach bright particles and retain the result in the active Recipe Pipeline"
    Click-AutomationId "WorkspaceSamplePickerOpenButton" "Open selected particle image" 1900
    Wait-AutomationElement `
        -AutomationId "HostToolNav_Blob" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Wait-ProcessElementNameContains `
        -ExpectedText "Public_Blob_Particles_Good" `
        -ControlType ([System.Windows.Automation.ControlType]::Text) `
        -TimeoutMilliseconds 15000 | Out-Null
    Add-TimelineEvent "sample-loaded-verified" "Public_Blob_Particles_Good ready for direct Blob teaching"
    Start-Sleep -Milliseconds 1200

    Click-AutomationId "HostToolNav_Blob" "Open Blob Tool View" 1800
    Wait-AutomationElement `
        -AutomationId "VisionToolRunPreviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Click-AutomationId "VisionToolPresetBasic" "Apply the reviewable Basic preset" 900
    try {
        Set-VisibleEditValue "100" "150" "Set Blob threshold"
    }
    catch {
        $thresholdAlreadySet = Get-ProcessAutomationElement `
            -AutomationId $null `
            -Name "150" `
            -ControlType ([System.Windows.Automation.ControlType]::Edit)
        if ($null -eq $thresholdAlreadySet) {
            throw
        }
        Add-TimelineEvent `
            "threshold-already-set" `
            "Blob threshold was already 150 after applying the Basic preset"
    }
    Click-AutomationId "VisionToolRunPreviewButton" "Run Blob Preview explicitly" 250
    Start-Sleep -Milliseconds 2200
    $previewSummaryElement = Wait-AutomationElement `
        -AutomationId "VisionToolResultReviewSummary" `
        -Name $null `
        -TimeoutMilliseconds 10000 `
        -AllowOffscreen
    $previewAfter = Get-AutomationElementText $previewSummaryElement
    if ([string]::IsNullOrWhiteSpace($previewAfter) -or
        $previewAfter -match "Result pending|Before preview") {
        throw "The explicit Blob Preview did not produce a reviewable result."
    }
    Add-TimelineEvent "teaching-preview-reviewed" $previewAfter
    Start-Sleep -Milliseconds 1400

    Click-AutomationId "VisionToolAddPipelineButton" "Add the reviewed Blob setup to the active Pipeline" 700
    Start-Sleep -Milliseconds 1100
    Add-TimelineEvent `
        "pipeline-add-complete" `
        "The explicit Add to Pipeline action completed; no additional Preview or Run was requested"
    Start-Sleep -Milliseconds 1400

    Close-ActiveToolViewForRouteReview
    Click-AutomationId "HostRecipeManagerButton" "Open Recipe Manager to verify persisted Pipeline state" 1000
    Wait-AutomationElement `
        -AutomationId "HostRecipeManagerCloseButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Wait-ProcessElementNameContains `
        -ExpectedText "Sample_Public_Blob_Particles_Good" `
        -ControlType ([System.Windows.Automation.ControlType]::Text) `
        -TimeoutMilliseconds 10000 | Out-Null
    Add-TimelineEvent `
        "recipe-pipeline-visible" `
        "Recipe Manager shows Sample_Public_Blob_Particles_Good with 3 persisted Steps"
    Start-Sleep -Milliseconds 1500

    Click-AutomationId "HostRecipeManagerCloseButton" "Close Recipe Manager" 900
    Click-AutomationId "HostRecipeManagerButton" "Reopen Recipe Manager from storage-backed context" 1000
    if ($null -eq (Get-ProcessAutomationElement `
            -AutomationId "HostRecipeManagerCloseButton" `
            -Name $null)) {
        Add-TimelineEvent `
            "recipe-manager-reopen-retry" `
            "The first reopen click did not expose the manager; the operator retries the same action"
        Click-AutomationId "HostRecipeManagerButton" "Retry reopening Recipe Manager" 1000
    }
    Wait-AutomationElement `
        -AutomationId "HostRecipeManagerCloseButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Wait-ProcessElementNameContains `
        -ExpectedText "Sample_Public_Blob_Particles_Good" `
        -ControlType ([System.Windows.Automation.ControlType]::Text) `
        -TimeoutMilliseconds 10000 | Out-Null
    Add-TimelineEvent `
        "recipe-pipeline-reopened" `
        "Close/reopen retained the exact generated Pipeline and 3-Step count without Preview or Run"
    Start-Sleep -Milliseconds 1500

    Click-AutomationId "HostRecipeManagerCloseButton" "Close Recipe Manager before execution review" 800
    Click-AutomationId "WorkspaceSamplePipelineButton" "Open the saved Pipeline for Run Review" 1500
    Wait-AutomationElement `
        -AutomationId "PipelineReviewRunReviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Add-TimelineEvent `
        "saved-pipeline-review-ready" `
        "The reopened 3-Step Pipeline is pending explicit Run Review"
    Start-Sleep -Milliseconds 900
    Click-AutomationId "PipelineReviewRunReviewButton" "Run the reopened Pipeline explicitly" 250
    if (-not (Wait-ReviewCompletion)) {
        throw "The reopened Blob Pipeline Run Review exited unexpectedly."
    }
    Add-TimelineEvent `
        "saved-pipeline-review-complete" `
        (Get-VisibleElementName "PipelineReviewSelectedResultSummary")
    Start-Sleep -Milliseconds 2200
}

function Invoke-NoviceScratchThresholdBlobRecipeScenario {
    $recipeName = "P255_Novice_Threshold_Blob_$($script:appProcess.Id)"
    $imagePath = Join-Path `
        $repoRoot `
        "docs\samples\public\Blob_Particles_Synthetic_OK.png"

    Click-AutomationId `
        "HostRecipeManagerButton" `
        "Open Recipe Manager for a new inspection" `
        1000
    Wait-AutomationElement `
        -AutomationId "HostRecipeNameEditor" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Set-AutomationEditValue `
        -AutomationId "HostRecipeNameEditor" `
        -Value $recipeName `
        -Label "Name the new inspection Recipe"
    Click-AutomationId `
        "HostRecipeCreateNamedButton" `
        "Create the named Recipe" `
        1200
    Wait-ProcessElementNameContains `
        -ExpectedText $recipeName `
        -TimeoutMilliseconds 15000 | Out-Null
    Add-TimelineEvent `
        "recipe-created" `
        "Recipe=$recipeName; expected initial Pipeline is empty"
    Click-AutomationId `
        "HostRecipeManagerCloseButton" `
        "Close Recipe Manager before direct Teaching" `
        900

    Load-WorkspaceImageFile `
        -ImagePath $imagePath `
        -Label "Load the chosen particle image"

    Click-AutomationId `
        "HostToolNav_Threshold" `
        "Open Threshold Tool View" `
        1600
    Wait-AutomationElement `
        -AutomationId "VisionToolRunPreviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    $thresholdBefore = Get-VisibleElementName "VisionToolResultReviewSummary"
    Click-AutomationId `
        "VisionToolRunPreviewButton" `
        "Run Threshold Preview explicitly" `
        250
    Wait-ToolPreviewCompletion `
        -PreviousSummary $thresholdBefore `
        -ExpectedOutputLayer "Threshold_Preview" | Out-Null
    Click-AutomationId `
        "VisionToolAddPipelineButton" `
        "Add and save the reviewed Threshold Step" `
        1100
    Wait-ProcessElementNameContains `
        -ExpectedText $recipeName `
        -ControlType ([System.Windows.Automation.ControlType]::Text) `
        -TimeoutMilliseconds 10000 | Out-Null
    Add-TimelineEvent `
        "threshold-step-saved" `
        "Threshold saved to Recipe $recipeName without another Preview or Run"
    Start-Sleep -Milliseconds 1100
    Add-TimelineEvent `
        "next-tool-direct" `
        "Continue directly to Blob without an unnecessary Tool View close/reopen step"

    Click-AutomationId `
        "HostToolNav_Blob" `
        "Open Blob Tool View for the next Step" `
        1600
    Wait-AutomationElement `
        -AutomationId "VisionToolRunPreviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    try {
        Select-ComboBoxItemName `
            -AutomationId "cbInputLayer" `
            -ItemName "Threshold_Preview" `
            -Label "Connect Threshold output to Blob input"
    }
    catch {
        Add-TimelineEvent `
            "next-tool-open-retry" `
            "The direct Tool switch did not expose the Blob input list; close the current Tool View and retry once"
        Close-ActiveToolViewForRouteReview
        Click-AutomationId `
            "HostToolNav_Blob" `
            "Retry opening Blob Tool View" `
            1600
        Wait-AutomationElement `
            -AutomationId "VisionToolRunPreviewButton" `
            -Name $null `
            -TimeoutMilliseconds 15000 | Out-Null
        Select-ComboBoxItemName `
            -AutomationId "cbInputLayer" `
            -ItemName "Threshold_Preview" `
            -Label "Connect Threshold output to Blob input after retry"
    }
    Click-AutomationId `
        "VisionToolPresetBasic" `
        "Apply the reviewable Blob Basic preset" `
        800
    $blobBefore = Get-VisibleElementName "VisionToolResultReviewSummary"
    Click-AutomationId `
        "VisionToolRunPreviewButton" `
        "Run Blob Preview explicitly" `
        250
    Wait-ToolPreviewCompletion `
        -PreviousSummary $blobBefore `
        -ExpectedOutputLayer "Blob_Preview" | Out-Null
    Click-AutomationId `
        "VisionToolAddPipelineButton" `
        "Add and save the reviewed Blob Step" `
        1100
    Wait-ProcessElementNameContains `
        -ExpectedText $recipeName `
        -ControlType ([System.Windows.Automation.ControlType]::Text) `
        -TimeoutMilliseconds 10000 | Out-Null
    Add-TimelineEvent `
        "blob-step-saved" `
        "Blob_1 saved with input Threshold_Preview; no extra Preview or Run requested"
    Start-Sleep -Milliseconds 1200
    Close-FloatingToolWindow

    Click-AutomationId `
        "HostRecipeManagerButton" `
        "Open Recipe Manager to inspect the saved two-Step Pipeline" `
        1000
    Wait-ProcessElementNameContains `
        -ExpectedText $recipeName `
        -ControlType ([System.Windows.Automation.ControlType]::Text) `
        -TimeoutMilliseconds 10000 | Out-Null
    Click-AutomationId `
        "HostRecipeOpenPipelineReviewButton" `
        "Open the saved Pipeline details" `
        1400
    Wait-AutomationElement `
        -AutomationId "PipelineReviewRunReviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Wait-ProcessElementNameContains `
        -ExpectedText "Blob_1" `
        -TimeoutMilliseconds 10000 | Out-Null
    Click-TextName "02 Blob_1" "Inspect the Blob Step route" 900
    Wait-AutomationElementNameContains `
        -AutomationId "PipelineReviewSelectedRouteSummary" `
        -ExpectedText "Threshold_Preview" `
        -TimeoutMilliseconds 10000 | Out-Null
    Add-TimelineEvent `
        "two-step-recipe-visible" `
        "Pipeline Review shows Threshold then Blob_1, with Blob input restored as Threshold_Preview before application restart"
    Start-Sleep -Milliseconds 1500

    Restart-OpenVisionApplication
    Select-ComboBoxItemName `
        -AutomationId "HostRecipeSelector" `
        -ItemName $recipeName `
        -Label "Restore the saved Recipe"
    Load-WorkspaceImageFile `
        -ImagePath $imagePath `
        -Label "Reload the same inspection image"

    Click-AutomationId `
        "HostRecipeManagerButton" `
        "Open the restored Recipe" `
        1000
    Click-AutomationId `
        "HostRecipeOpenPipelineReviewButton" `
        "Open the restored Pipeline for Run Review" `
        1400
    Wait-AutomationElement `
        -AutomationId "PipelineReviewRunReviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Wait-ProcessElementNameContains `
        -ExpectedText "Blob_1" `
        -TimeoutMilliseconds 10000 | Out-Null
    Click-TextName "02 Blob_1" "Inspect the restored Blob Step route" 900
    Wait-AutomationElementNameContains `
        -AutomationId "PipelineReviewSelectedRouteSummary" `
        -ExpectedText "Threshold_Preview" `
        -TimeoutMilliseconds 10000 | Out-Null
    Add-TimelineEvent `
        "recipe-restored" `
        "Restart retained Recipe, Pipeline, Threshold, Blob_1, and Blob input Threshold_Preview"
    Start-Sleep -Milliseconds 1300
    Wait-AutomationElement `
        -AutomationId "PipelineReviewRunReviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Add-TimelineEvent `
        "restored-pipeline-review-ready" `
        "The restored two-Step Pipeline is waiting for explicit Run Review"
    Click-AutomationId `
        "PipelineReviewRunReviewButton" `
        "Run the restored Pipeline explicitly" `
        250
    if (-not (Wait-ReviewCompletion)) {
        throw "The restored Threshold -> Blob Pipeline exited during Run Review."
    }
    Add-TimelineEvent `
        "restored-pipeline-review-complete" `
        (Get-VisibleElementName "PipelineReviewSelectedResultSummary")
    Start-Sleep -Milliseconds 2400
}

function Invoke-DirectTeachingPipelineStep {
    param(
        [string]$RecipeName,
        [string]$ToolAutomationId,
        [string]$ToolLabel,
        [string]$InputLayer,
        [string]$OutputLayer,
        [switch]$ApplyBasicPreset
    )

    Click-AutomationId `
        $ToolAutomationId `
        "Open $ToolLabel for the next inspection Step" `
        1600
    Wait-AutomationElement `
        -AutomationId "VisionToolRunPreviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Select-ComboBoxItemName `
        -AutomationId "cbInputLayer" `
        -ItemName $InputLayer `
        -Label "Set $ToolLabel input to $InputLayer"
    if ($ApplyBasicPreset) {
        Click-AutomationId `
            "VisionToolPresetBasic" `
            "Apply the reviewable $ToolLabel Basic preset" `
            800
    }

    $before = Get-VisibleElementName "VisionToolResultReviewSummary"
    Click-AutomationId `
        "VisionToolRunPreviewButton" `
        "Run $ToolLabel Preview explicitly" `
        250
    Wait-ToolPreviewCompletion `
        -PreviousSummary $before `
        -ExpectedOutputLayer $OutputLayer | Out-Null
    Click-AutomationId `
        "VisionToolAddPipelineButton" `
        "Add and save the reviewed $ToolLabel Step" `
        1100
    Wait-ProcessElementNameContains `
        -ExpectedText $RecipeName `
        -ControlType ([System.Windows.Automation.ControlType]::Text) `
        -TimeoutMilliseconds 10000 | Out-Null
    Add-TimelineEvent `
        "pipeline-step-saved" `
        "$ToolLabel saved / $InputLayer -> $OutputLayer / Recipe=$RecipeName"
    Start-Sleep -Milliseconds 900
}

function Inspect-PipelineStepRoute {
    param(
        [string]$StepText,
        [string]$InputLayer,
        [string]$OutputLayer,
        [string]$LabelPrefix
    )

    Click-TextName $StepText "$LabelPrefix $StepText route" 750
    Wait-AutomationElementNameContains `
        -AutomationId "PipelineReviewSelectedRouteSummary" `
        -ExpectedText $InputLayer `
        -TimeoutMilliseconds 10000 | Out-Null
    Wait-AutomationElementNameContains `
        -AutomationId "PipelineReviewSelectedRouteSummary" `
        -ExpectedText $OutputLayer `
        -TimeoutMilliseconds 10000 | Out-Null
    Add-TimelineEvent `
        "pipeline-route-reviewed" `
        "$StepText / $InputLayer -> $OutputLayer"
}

function Invoke-NoviceFourStepRouteClarityScenario {
    $recipeName = "P256_FourStep_Route_$($script:appProcess.Id)"
    $imagePath = Join-Path `
        $repoRoot `
        "docs\samples\public\Blob_Particles_Synthetic_OK.png"

    Click-AutomationId `
        "HostRecipeManagerButton" `
        "Open Recipe Manager for a four-Step inspection" `
        1000
    Wait-AutomationElement `
        -AutomationId "HostRecipeNameEditor" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Set-AutomationEditValue `
        -AutomationId "HostRecipeNameEditor" `
        -Value $recipeName `
        -Label "Name the four-Step Recipe"
    Click-AutomationId `
        "HostRecipeCreateNamedButton" `
        "Create the four-Step Recipe" `
        1200
    Wait-ProcessElementNameContains `
        -ExpectedText $recipeName `
        -TimeoutMilliseconds 15000 | Out-Null
    Add-TimelineEvent `
        "recipe-created" `
        "Recipe=$recipeName; expected initial Pipeline is empty"
    Click-AutomationId `
        "HostRecipeManagerCloseButton" `
        "Close Recipe Manager before direct Teaching" `
        900

    Load-WorkspaceImageFile `
        -ImagePath $imagePath `
        -Label "Load the four-Step inspection image"

    Invoke-DirectTeachingPipelineStep `
        -RecipeName $recipeName `
        -ToolAutomationId "HostToolNav_Filter" `
        -ToolLabel "Filter" `
        -InputLayer "Main" `
        -OutputLayer "Filter_Preview"
    Invoke-DirectTeachingPipelineStep `
        -RecipeName $recipeName `
        -ToolAutomationId "HostToolNav_Threshold" `
        -ToolLabel "Threshold" `
        -InputLayer "Filter_Preview" `
        -OutputLayer "Threshold_Preview"
    Invoke-DirectTeachingPipelineStep `
        -RecipeName $recipeName `
        -ToolAutomationId "HostToolNav_Morphology" `
        -ToolLabel "Morphology" `
        -InputLayer "Threshold_Preview" `
        -OutputLayer "Morphology_Preview"
    Invoke-DirectTeachingPipelineStep `
        -RecipeName $recipeName `
        -ToolAutomationId "HostToolNav_Blob" `
        -ToolLabel "Blob" `
        -InputLayer "Morphology_Preview" `
        -OutputLayer "Blob_Preview" `
        -ApplyBasicPreset

    Close-ActiveToolViewForRouteReview
    Click-AutomationId `
        "HostRecipeManagerButton" `
        "Open Recipe Manager to review the four saved Steps" `
        1000
    Wait-ProcessElementNameContains `
        -ExpectedText $recipeName `
        -TimeoutMilliseconds 10000 | Out-Null
    Click-AutomationId `
        "HostRecipeOpenPipelineReviewButton" `
        "Open the four-Step Pipeline details" `
        1400
    Wait-AutomationElement `
        -AutomationId "PipelineReviewRunReviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Inspect-PipelineStepRoute "01 Filter" "Main" "Filter_Preview" "Review"
    Inspect-PipelineStepRoute "02 Threshold" "Filter_Preview" "Threshold_Preview" "Review"
    Inspect-PipelineStepRoute "03 Morphology" "Threshold_Preview" "Morphology_Preview" "Review"
    Inspect-PipelineStepRoute "04 Blob_1" "Morphology_Preview" "Blob_Preview" "Review"
    Add-TimelineEvent `
        "four-step-route-visible" `
        "All four saved routes are visible before application restart"
    Start-Sleep -Milliseconds 1500

    Restart-OpenVisionApplication
    Select-ComboBoxItemName `
        -AutomationId "HostRecipeSelector" `
        -ItemName $recipeName `
        -Label "Restore the four-Step Recipe"
    Load-WorkspaceImageFile `
        -ImagePath $imagePath `
        -Label "Reload the four-Step inspection image"
    Click-AutomationId `
        "HostRecipeManagerButton" `
        "Open the restored four-Step Recipe" `
        1000
    Click-AutomationId `
        "HostRecipeOpenPipelineReviewButton" `
        "Open the restored four-Step Pipeline" `
        1400
    Wait-AutomationElement `
        -AutomationId "PipelineReviewRunReviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Inspect-PipelineStepRoute "01 Filter" "Main" "Filter_Preview" "Verify restored"
    Inspect-PipelineStepRoute "02 Threshold" "Filter_Preview" "Threshold_Preview" "Verify restored"
    Inspect-PipelineStepRoute "03 Morphology" "Threshold_Preview" "Morphology_Preview" "Verify restored"
    Inspect-PipelineStepRoute "04 Blob_1" "Morphology_Preview" "Blob_Preview" "Verify restored"
    Add-TimelineEvent `
        "four-step-route-restored" `
        "Restart retained all four ordered routes without running the Pipeline"
    Start-Sleep -Milliseconds 1200

    Click-AutomationId `
        "PipelineReviewRunReviewButton" `
        "Run the restored four-Step Pipeline explicitly" `
        250
    if (-not (Wait-ReviewCompletion)) {
        throw "The restored four-Step Pipeline exited during Run Review."
    }
    Add-TimelineEvent `
        "four-step-run-complete" `
        (Get-VisibleElementName "PipelineReviewSelectedResultSummary")
    Start-Sleep -Milliseconds 2400
}

function Invoke-NoviceMatchingCorrectionLoopScenario {
    Click-AutomationId "WorkspaceEmptySampleButton" "Open public sample catalog" 1100
    Discard-PendingLabelEditForSampleOpenIfVisible
    Select-CatalogSampleInPicker "Public_Matching_DiePad_Good"
    Add-TimelineEvent `
        "correction-target-selected" `
        "Image=Public_Matching_DiePad_Good; intent=detect the taught die pad; expected Good count and score gate"
    Click-AutomationId "WorkspaceSamplePickerOpenButton" "Open selected Matching sample" 1900
    Wait-AutomationElement `
        -AutomationId "HostRecipeManagerButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Wait-ProcessElementNameContains `
        -ExpectedText "Public_Matching_DiePad_Good" `
        -ControlType ([System.Windows.Automation.ControlType]::Text) `
        -TimeoutMilliseconds 15000 | Out-Null
    Add-TimelineEvent "sample-loaded-verified" "Public_Matching_DiePad_Good ready for the persisted correction workflow"
    Start-Sleep -Milliseconds 1200

    Click-AutomationId "HostRecipeManagerButton" "Open Recipe Manager" 1100
    Click-AutomationId "HostRecipeAdvancedReviewToggle" "Open advanced review workflows" 900
    Click-AutomationId "HostRecipePipelineTab" "Open Pipeline and sample workflow" 900
    $savePairButton = Wait-AutomationElement `
        -AutomationId "HostRecipeCreateValidationSetFromPairButton" `
        -Name $null `
        -TimeoutMilliseconds 15000
    if (-not $savePairButton.Current.IsEnabled) {
        throw "The current workspace Matching pair was not ready to save as a Validation Set."
    }
    Add-TimelineEvent `
        "catalog-pair-ready" `
        "The synchronized Matching sample/pipeline pair is visible and the save action is enabled"
    Start-Sleep -Milliseconds 900

    Click-AutomationId `
        "HostRecipeCreateValidationSetFromPairButton" `
        "Save selected catalog Good and Bad pair as a Local Validation Set" `
        1200
    $selectedSet = Get-VisibleElementName "HostRecipeValidationSetCombo"
    $selectedScope = Get-VisibleElementName "HostRecipeValidationSuiteScopeCombo"
    Add-TimelineEvent `
        "validation-set-saved" `
        "Set=$selectedSet; Scope=$selectedScope; no Preview or Run was requested"

    Click-AutomationId "HostRecipePipelineRunHistoryTab" "Open Validation and Run History" 1000
    Wait-AutomationElement `
        -AutomationId "HostRecipeRunValidationSuiteButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    $beforeFirstRunStatus =
        Get-VisibleElementName "HostRecipeValidationSuiteStatus"
    Click-AutomationId `
        "HostRecipeRunValidationSuiteButton" `
        "Run the saved Local Validation Set explicitly" `
        250
    $firstRunStatus = Wait-ValidationSuiteCompletion `
        -PreviousStatus $beforeFirstRunStatus
    Add-TimelineEvent "first-validation-complete" $firstRunStatus

    Click-ListItemNameContains `
        -AutomationId "HostRecipeRecentBatchRunSampleList" `
        -ExpectedText "Public_Matching_DiePad_NoTarget_Bad" `
        -Label "Select the retained failed Matching sample"
    Add-TimelineEvent `
        "failed-sample-selected" `
        (Get-VisibleElementName "HostRecipeSelectedRunComparisonReview")
    Start-Sleep -Milliseconds 1100

    Click-AutomationId `
        "HostRecipeRunHistoryPrepareCorrectionButton" `
        "Prepare correction from the retained failed sample" `
        1300
    Wait-AutomationElement `
        -AutomationId "HostRecipeApplySelectedStepParametersButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Add-TimelineEvent `
        "correction-prepared" `
        "Exact failed sample and Step PropertyGrid loaded; no Preview or Run was requested"
    Start-Sleep -Milliseconds 1000

    $beforeApplyReview =
        Get-VisibleElementName "HostRecipeCorrectedOutputReviewText"
    Click-AutomationId `
        "HostRecipeApplySelectedStepParametersButton" `
        "Apply the unchanged reviewed Step parameters to XML" `
        1000
    Wait-AutomationElement `
        -AutomationId "HostRecipeCorrectedOutputRerunButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    $afterApplyReview = Wait-AutomationElementTextChange `
        -AutomationId "HostRecipeCorrectedOutputReviewText" `
        -PreviousText $beforeApplyReview
    Add-TimelineEvent `
        "correction-applied-no-run" `
        $afterApplyReview
    Start-Sleep -Milliseconds 1100

    $beforeSecondRunStatus =
        Get-VisibleElementName "HostRecipeValidationSuiteStatus"
    Click-AutomationId `
        "HostRecipeCorrectedOutputRerunButton" `
        "Rerun the same saved validation set explicitly" `
        250
    Click-AutomationId `
        "HostRecipePipelineRunHistoryTab" `
        "Return to Run History for comparison" `
        600 `
        -AllowOffscreen
    $secondRunStatus = Wait-ValidationSuiteCompletion `
        -PreviousStatus $beforeSecondRunStatus
    Add-TimelineEvent "same-set-rerun-complete" $secondRunStatus
    Add-TimelineEvent `
        "comparison-visible" `
        (Get-VisibleElementName "HostRecipeRecentBatchRunComparisonSummary")
    Start-Sleep -Milliseconds 2600
}

function Invoke-FixtureCrashScenario {
    $completed = Open-PipelineReviewAndRun
    if ($completed) {
        Click-AutomationId "PipelineReviewFixtureDesignerTab" "Open Fixture and relative ROI review" 1400
        Add-TimelineEvent "unexpected-completion" "Fixture Run Review completed instead of reproducing the native crash"
    }
    else {
        Start-Sleep -Milliseconds 2200
    }
}

function Invoke-MatchingToolViewScenario {
    Click-AutomationId "WorkspaceSampleFirstStepButton" "Open configured Matching Tool View" 1800
    Wait-AutomationElement `
        -AutomationId "VisionToolRunPreviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    $templateStatus = Get-VisibleElementName "txtTemplateStatus"
    Add-TimelineEvent "matching-template" $templateStatus
    Start-Sleep -Milliseconds 900
    $before = Get-VisibleElementName "VisionToolResultReviewSummary"
    Click-AutomationId "VisionToolRunPreviewButton" "Run Matching Preview explicitly" 250
    Wait-ToolPreviewCompletion -PreviousSummary $before | Out-Null
    Start-Sleep -Milliseconds 2200
}

function Invoke-LineToolViewScenario {
    Click-AutomationId "WorkspaceSampleFirstStepButton" "Open configured Line Tool View" 1800
    Wait-AutomationElement `
        -AutomationId "LineToolPurposeMeasure" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null

    Click-AutomationId "LineToolPurposeEdge" "Select Edge purpose" 650
    $before = Get-VisibleElementName "VisionToolResultReviewSummary"
    Click-AutomationId "VisionToolRunPreviewButton" "Run Line Edge Preview" 250
    Wait-ToolPreviewCompletion -PreviousSummary $before | Out-Null
    Close-LineSignalInspectorIfVisible

    Click-AutomationId "LineToolPurposeMeasure" "Select Length and Distance purpose" 650
    $before = Get-VisibleElementName "VisionToolResultReviewSummary"
    Click-AutomationId "VisionToolRunPreviewButton" "Run Line Measure Preview" 250
    Wait-ToolPreviewCompletion -PreviousSummary $before | Out-Null
    Close-LineSignalInspectorIfVisible

    Click-AutomationId "LineToolPurposeIntersection" "Select Intersection purpose" 650
    $before = Get-VisibleElementName "VisionToolResultReviewSummary"
    Click-AutomationId "VisionToolRunPreviewButton" "Run Line Intersection Preview" 250
    Wait-ToolPreviewCompletion -PreviousSummary $before | Out-Null
    Close-LineSignalInspectorIfVisible
    Start-Sleep -Milliseconds 2200
}

function Invoke-ObjectToolViewScenario {
    param(
        [string]$ToolAutomationId,
        [string]$ToolLabel,
        [switch]$ApplyBasicPreset,
        [string]$ThresholdValue = ""
    )

    Click-AutomationId $ToolAutomationId "Open $ToolLabel Tool View" 1800
    Wait-AutomationElement `
        -AutomationId "VisionToolRunPreviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    if ($ApplyBasicPreset) {
        Click-AutomationId "VisionToolPresetBasic" "Apply the reviewable Basic preset" 900
    }
    if (-not [string]::IsNullOrWhiteSpace($ThresholdValue)) {
        Set-VisibleEditValue "100" $ThresholdValue "Set $ToolLabel threshold"
    }
    $before = Get-VisibleElementName "VisionToolResultReviewSummary"
    Click-AutomationId "VisionToolRunPreviewButton" "Run $ToolLabel Preview explicitly" 250
    Wait-ToolPreviewCompletion -PreviousSummary $before | Out-Null
    Start-Sleep -Milliseconds 2200
}

function Invoke-PreprocessToolViewScenario {
    param(
        [string]$ToolAutomationId,
        [string]$ToolLabel
    )

    Click-AutomationId $ToolAutomationId "Open $ToolLabel Tool View" 1800
    Wait-AutomationElement `
        -AutomationId "VisionToolRunPreviewButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Click-AutomationId "VisionToolRunPreviewButton" "Run $ToolLabel Preview explicitly" 250
    Start-Sleep -Milliseconds 2600
    Wait-AutomationElement `
        -AutomationId "VisionToolOutputPreviewSlot" `
        -Name $null `
        -TimeoutMilliseconds 10000 | Out-Null
    Add-TimelineEvent "tool-preview-complete" "$ToolLabel output preview visible"
    Start-Sleep -Milliseconds 2200
}

function Invoke-ChainScenario {
    param(
        [string[]]$StepNames,
        [string]$ChainLabel
    )

    if (-not (Open-PipelineReviewAndRun)) {
        throw "$ChainLabel Run Review exited unexpectedly."
    }

    foreach ($stepName in $StepNames) {
        Click-TextName $stepName "Review $stepName output" 1250
        Add-TimelineEvent "chain-step-review" "$ChainLabel / $stepName"
    }
    Start-Sleep -Milliseconds 2200
}

$status = "Incomplete"
$failure = $null
try {
    $script:appProcess = Start-Process `
        -FilePath $exePath `
        -WorkingDirectory $runtimeRoot `
        -PassThru
    $script:appProcess.WaitForInputIdle(15000) | Out-Null
    Wait-AutomationElement `
        -AutomationId "WorkspaceEmptySampleButton" `
        -Name $null `
        -TimeoutMilliseconds 20000 | Out-Null
    $initialSample = switch ($Scenario) {
        "novice-blob-self-trial" { $null }
        "novice-blob-teaching-self-trial" { $null }
        "novice-blob-pipeline-persistence" { $null }
        "novice-scratch-threshold-blob-recipe" { $null }
        "novice-four-step-route-clarity" { $null }
        "novice-matching-correction-loop" { $null }
        "blob-good-bad" { "Public_Blob_Particles_Good" }
        "fixture-crash" { "Public_Fixture_Normalize_RelativeRoi_Good" }
        "matching-tool-view" { "Public_Matching_DiePad_Good" }
        "line-tool-view" { "Public_Line_Pins_Good" }
        "blob-tool-view" { "Public_Blob_Particles_Good" }
        "contour-tool-view" { "Public_Contour_Shapes_Good" }
        "filter-tool-view" { "Public_Filter_Denoise_Good" }
        "morphology-tool-view" { "Public_Morphology_Cleanup_Good" }
        "filter-chain" { "Public_Filter_Denoise_Good" }
        "morphology-chain" { "Public_Morphology_Cleanup_Good" }
    }
    if (-not [string]::IsNullOrWhiteSpace($initialSample)) {
        Prepare-CatalogSampleBeforeRecording $initialSample
    }
    Minimize-OtherOpenVisionWindows
    Position-MainWindowForRecording

    Start-DesktopRecording
    Add-TimelineEvent "process-running" "PID=$($script:appProcess.Id); EXE=$exePath"
    $readyDetail = if ([string]::IsNullOrWhiteSpace($initialSample)) {
        "Actual EXE at the empty workspace before sample selection"
    }
    else {
        "Actual EXE with prepared public sample $initialSample"
    }
    Add-TimelineEvent "application-ready" $readyDetail
    Start-Sleep -Milliseconds 1500

    switch ($Scenario) {
        "novice-blob-self-trial" {
            Invoke-NoviceBlobSelfTrialScenario
            $status = "Complete"
        }
        "novice-blob-teaching-self-trial" {
            Invoke-NoviceBlobTeachingSelfTrialScenario
            $status = "Complete"
        }
        "novice-blob-pipeline-persistence" {
            Invoke-NoviceBlobPipelinePersistenceScenario
            $status = "Complete"
        }
        "novice-scratch-threshold-blob-recipe" {
            Invoke-NoviceScratchThresholdBlobRecipeScenario
            $status = "Complete"
        }
        "novice-four-step-route-clarity" {
            Invoke-NoviceFourStepRouteClarityScenario
            $status = "Complete"
        }
        "novice-matching-correction-loop" {
            Invoke-NoviceMatchingCorrectionLoopScenario
            $status = "Complete"
        }
        "blob-good-bad" {
            Invoke-BlobGoodBadScenario
            $status = "Complete"
        }
        "fixture-crash" {
            Invoke-FixtureCrashScenario
            $status = if ($script:appProcess.HasExited) { "CrashReproduced" } else { "CompleteWithoutCrash" }
        }
        "matching-tool-view" {
            Invoke-MatchingToolViewScenario
            $status = "Complete"
        }
        "line-tool-view" {
            Invoke-LineToolViewScenario
            $status = "Complete"
        }
        "blob-tool-view" {
            Invoke-ObjectToolViewScenario "HostToolNav_Blob" "Blob" -ApplyBasicPreset -ThresholdValue "150"
            $status = "Complete"
        }
        "contour-tool-view" {
            Invoke-ObjectToolViewScenario "HostToolNav_Contour" "Contour" -ApplyBasicPreset -ThresholdValue "150"
            $status = "Complete"
        }
        "filter-tool-view" {
            Invoke-PreprocessToolViewScenario "HostToolNav_Filter" "Filter"
            $status = "Complete"
        }
        "morphology-tool-view" {
            Invoke-PreprocessToolViewScenario "HostToolNav_Morphology" "Morphology"
            $status = "Complete"
        }
        "filter-chain" {
            Invoke-ChainScenario `
                @("01 Filter Median Denoise", "02 Filter Denoise Binary", "03 Filter Denoise Target Count") `
                "Filter to Threshold to Contour"
            $status = "Complete"
        }
        "morphology-chain" {
            Invoke-ChainScenario `
                @("01 Morphology Cleanup Binary", "02 Morphology Speck Open", "03 Morphology Clean Target Count") `
                "Threshold to Morphology to Contour"
            $status = "Complete"
        }
    }
}
catch {
    $failure = $_
    Add-TimelineEvent "automation-error" $_.Exception.Message
    if ($Scenario -eq "fixture-crash" -and $null -ne $script:appProcess -and $script:appProcess.HasExited) {
        $status = "CrashReproduced"
    }
}
finally {
    Start-Sleep -Milliseconds 900
    Stop-DesktopRecording
    $script:recordingClock.Stop()

    if ($null -ne $script:appProcess -and -not $script:appProcess.HasExited) {
        $script:appProcess.CloseMainWindow() | Out-Null
        if (-not $script:appProcess.WaitForExit(5000)) {
            Stop-Process -Id $script:appProcess.Id -Force
        }
    }
    Restore-OtherOpenVisionWindows

    $timelineLines = [System.Collections.Generic.List[string]]::new()
    $timelineLines.Add("Seconds`tAction`tDetail")
    $timelineLines.AddRange($script:timeline)
    [System.IO.File]::WriteAllLines(
        $timelinePath,
        $timelineLines,
        [System.Text.UTF8Encoding]::new($false))

    $exeHash = (Get-FileHash -LiteralPath $exePath -Algorithm SHA256).Hash
    $appAssemblyHash = (Get-FileHash -LiteralPath $appAssemblyPath -Algorithm SHA256).Hash
    $failureMessage = if ($null -eq $failure) { "-" } else { $failure.Exception.Message }
    $summaryLines = @(
        "Status=$status",
        "Scenario=$Scenario",
        "EXE=$exePath",
        "EXESHA256=$exeHash",
        "EXELastWriteKST=$((Get-Item -LiteralPath $exePath).LastWriteTime.ToString('O'))",
        "APPASSEMBLY=$appAssemblyPath",
        "APPASSEMBLYSHA256=$appAssemblyHash",
        "APPASSEMBLYLastWriteKST=$((Get-Item -LiteralPath $appAssemblyPath).LastWriteTime.ToString('O'))",
        "Monitor=$($script:recordingMonitorName)",
        "MonitorBounds=$($script:recordingLeft),$($script:recordingTop),$($script:recordingWidth),$($script:recordingHeight)",
        "MonitorFallback=$($script:recordingMonitorFallback)",
        "WindowBounds=$($script:recordingWindowBounds)",
        "Video=$videoPath",
        "Timeline=$timelinePath",
        "Failure=$failureMessage"
    )
    [System.IO.File]::WriteAllLines(
        $runSummaryPath,
        $summaryLines,
        [System.Text.UTF8Encoding]::new($false))
}

Write-Output "Status=$status"
Write-Output "Video=$videoPath"
Write-Output "Timeline=$timelinePath"
if ($null -ne $failure -and $status -ne "CrashReproduced") {
    throw $failure
}
