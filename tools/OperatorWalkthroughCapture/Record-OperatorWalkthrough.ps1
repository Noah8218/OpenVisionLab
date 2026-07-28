param(
    [Parameter(Mandatory = $true)]
    [ValidateSet(
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
    [string]$OutputDirectory
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$exePath = Join-Path $repoRoot "bin\Debug\OpenVisionLab.exe"
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

$existing = Get-Process -Name OpenVisionLab -ErrorAction SilentlyContinue
if ($existing) {
    throw "Close the existing OpenVisionLab process before recording. Existing PID(s): $($existing.Id -join ', ')"
}

$ffmpegCommand = Get-Command ffmpeg -ErrorAction Stop
New-Item -ItemType Directory -Path $outputRoot -Force | Out-Null

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms

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
        "-f gdigrab -framerate 30 -offset_x 320 -offset_y 180 " +
        "-video_size 1920x1080 -draw_mouse 1 -i desktop " +
        "-c:v libx264 -preset veryfast -crf 18 -pix_fmt yuv420p " +
        "-movflags +faststart `"$videoPath`"")

    $script:ffmpegProcess = [System.Diagnostics.Process]::new()
    $script:ffmpegProcess.StartInfo = $psi
    if (-not $script:ffmpegProcess.Start()) {
        throw "FFmpeg recording process did not start."
    }

    $script:ffmpegErrorTask = $script:ffmpegProcess.StandardError.ReadToEndAsync()
    $script:recordingClock.Restart()
    Add-TimelineEvent "recording-start" "1920x1080 30fps desktop crop 320,180 with cursor"
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
    $matches = [System.Windows.Automation.AutomationElement]::RootElement.FindAll(
        [System.Windows.Automation.TreeScope]::Descendants,
        $condition)
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
    if ($script:appProcess.MainWindowHandle -eq [IntPtr]::Zero) {
        throw "OpenVisionLab main window handle is not ready."
    }

    [OpenVisionNaturalInput]::ShowWindow($script:appProcess.MainWindowHandle, 9) | Out-Null
    [OpenVisionNaturalInput]::SetWindowPos(
        $script:appProcess.MainWindowHandle,
        [IntPtr]::Zero,
        320,
        180,
        1920,
        1080,
        0x0040) | Out-Null
    [OpenVisionNaturalInput]::SetForegroundWindow($script:appProcess.MainWindowHandle) | Out-Null
    Start-Sleep -Milliseconds 900
}

function Minimize-OtherOpenVisionWindows {
    $otherProcesses = Get-Process -ErrorAction SilentlyContinue |
        Where-Object {
            $_.Id -ne $script:appProcess.Id -and
            $_.ProcessName -like "OpenVisionLab*" -and
            $_.MainWindowHandle -ne [IntPtr]::Zero
        }
    foreach ($process in $otherProcesses) {
        $handle = [IntPtr]$process.MainWindowHandle
        [OpenVisionNaturalInput]::ShowWindow($handle, 6) | Out-Null
        $script:minimizedExternalWindows.Add($handle)
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

function Click-AutomationId {
    param(
        [string]$AutomationId,
        [string]$Label,
        [int]$PauseAfterMilliseconds = 850,
        [int]$TimeoutMilliseconds = 15000
    )

    $element = Wait-AutomationElement `
        -AutomationId $AutomationId `
        -Name $null `
        -TimeoutMilliseconds $TimeoutMilliseconds
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
        Start-Sleep -Milliseconds 150
    }

    throw "Tool View Preview did not reach a completed UI state within $TimeoutMilliseconds ms."
}

function Get-VisibleElementName {
    param([string]$AutomationId)

    $element = Get-ProcessAutomationElement -AutomationId $AutomationId -Name $null
    if ($null -eq $element) {
        return ""
    }

    return $element.Current.Name
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

function Open-CatalogSample {
    param(
        [string]$SampleName
    )

    Click-AutomationId "WorkspaceEmptySampleButton" "Open public sample catalog" 1100
    Click-AutomationId "WorkspaceSamplePickerSearchBox" "Focus sample search" 250
    Type-HumanText $SampleName "Search $SampleName"
    Click-AutomationId "WorkspaceSamplePickerOpenButton" "Open selected sample" 1900
    Wait-AutomationElement `
        -AutomationId "WorkspaceSamplePipelineButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Add-TimelineEvent "sample-loaded" $SampleName
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
    Click-AutomationId "WorkspaceSamplePickerOpenButton" "Open selected sparse Bad sample" 1900
    Wait-AutomationElement `
        -AutomationId "WorkspaceSamplePipelineButton" `
        -Name $null `
        -TimeoutMilliseconds 15000 | Out-Null
    Add-TimelineEvent "sample-loaded" "Public_Blob_Particles_Sparse_Bad"
    if (-not (Open-PipelineReviewAndRun)) {
        throw "Blob Bad Run Review exited unexpectedly."
    }

    Click-TextName "02 Synthetic Particle Count" "Select Bad Blob result Step" 1200
    Add-TimelineEvent "object-review" "Bad count and rejected acceptance visible"
    Start-Sleep -Milliseconds 2200
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
        -WorkingDirectory $repoRoot `
        -PassThru
    $script:appProcess.WaitForInputIdle(15000) | Out-Null
    Wait-AutomationElement `
        -AutomationId "WorkspaceEmptySampleButton" `
        -Name $null `
        -TimeoutMilliseconds 20000 | Out-Null
    $initialSample = switch ($Scenario) {
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
    Prepare-CatalogSampleBeforeRecording $initialSample
    Minimize-OtherOpenVisionWindows
    Position-MainWindowForRecording

    Start-DesktopRecording
    Add-TimelineEvent "process-running" "PID=$($script:appProcess.Id); EXE=$exePath"
    Add-TimelineEvent "application-ready" "Actual EXE with prepared public sample $initialSample"
    Start-Sleep -Milliseconds 1500

    switch ($Scenario) {
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
    $failureMessage = if ($null -eq $failure) { "-" } else { $failure.Exception.Message }
    $summaryLines = @(
        "Status=$status",
        "Scenario=$Scenario",
        "EXE=$exePath",
        "EXESHA256=$exeHash",
        "EXELastWriteKST=$((Get-Item -LiteralPath $exePath).LastWriteTime.ToString('O'))",
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
