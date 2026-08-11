# OpenVisionLab Line Signal Transient Cue

Status: Complete

Date: 2026-08-11 KST

## Scope And Acceptance

The Line Tool parameter editor must remain available after each successful
Edge or Measure Preview. The retained detailed signal evidence remains
available through the explicit `신호 검토` / `Review signal` command.

Acceptance criteria:

1. Publishing Line signal evidence does not open the detailed inspector.
2. One localized, non-interactive cue appears beside the review command and
   dismisses after three seconds; a later result replaces the same cue.
3. Explicit review still opens the retained intensity/edge-response plot.
4. Cue and review interaction do not run Preview, create/select layers, change
   the active layer, or change input/output routes.
5. The behavior is independently verified from Dev and original actual EXEs.

Detection, fitting, measurement, XML, calibration, acceptance, Threshold and
Histogram inspector behavior, and unrelated Tool Views are excluded.

## Root Cause And Change

`LineToolWpfView.ShowSignalEvidence` retained the correct evidence and then
unconditionally set the full overlay to `Visible`. Every debounced Preview
therefore replaced the parameter editor with the detailed inspector.

The method now updates the retained evidence, reveals the existing manual
review command, and refreshes one three-second `DispatcherTimer`-owned cue.
It leaves a manually opened inspector visible and updates it in place. The cue
uses existing Tool theme brushes and Korean/English text and does not accept
pointer input.

The direct EXE fixture now uses the tracked
`docs/samples/public/Line_Pins_Synthetic_OK.png` instead of the Dev-only legacy
`Sample/EasyGauge/Pins.bmp`. The public input is 572x420 with SHA-256
`9CD5466296D4A660AA2B95809B81C4A877E0AB0D6CE65C55C3D5BC4C4747C49D`.

## Exact Port Plan And Result

Source of truth: `C:\Git\OpenVisionLab_Dev` working tree at HEAD `42d840a9`.

Target: `C:\Git\OpenVisionLab` working tree at HEAD `0582d226`.

| Source responsibility | Target relation |
| --- | --- |
| Line Tool cue UI and lifetime | Exact content-equivalent port |
| Native document/state/facade/test hooks | Exact content-equivalent port |
| Focused and actual-EXE signal regression | Exact content-equivalent port |
| Stable contract and current handoff | Exact content-equivalent port |

Approved deviations: none. The ten mapped files compare equal with
`git diff --no-index --ignore-space-at-eol`; physical SHA-256 differs only
where the two working trees retain different line endings.

Equivalence status: verified for the bounded Line signal workflow.

## Verification

| Check | Status | Exit code | Observed evidence |
| --- | --- | ---: | --- |
| Dev Debug solution build | PASS | 0 | 0 warnings, 0 errors |
| Dev readiness | PASS | 0 | 13/13 contracts |
| Dev focused `wpf_line_signal_profile` | PASS | 0 | `layout=0`, `text=0`, `internal=0` |
| Dev actual EXE, public pin input | PASS | 0 | Edge cue/review contract; Measure 37 px / 0.222 mm / 24 detections |
| Original Debug solution build with D-drive output | PASS | 0 | 0 warnings, 0 errors |
| Original readiness | PASS | 0 | 13/13 contracts |
| Original focused `wpf_line_signal_profile` with isolated output | PASS | 0 | `layout=0`, `text=0`, `internal=0` |
| Original actual EXE, public pin input | PASS | 0 | Same Edge cue/review contract and 37 px / 0.222 mm / 24 detections |
| Dev/original mapped-file content comparison | PASS | 0 | all 10 paths content-equivalent |

The first non-isolated original focused build failed because the operator's
already-running original EXE locked `C:\Git\OpenVisionLab\bin\Debug` DLLs.
No operator process was closed. Re-running with a D-drive output root passed.
An earlier capture selected the last floating Tool window and could target the
wrong prewarmed window; the focused smoke now selects the window that actually
contains `LineToolWpfView`.

## Evidence

Dev actual EXE:

- `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\signal_inspector_transient_20260811\actual_exe_dev_public\report.txt`

Original actual EXE:

- `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\signal_inspector_transient_20260811\actual_exe_original_public\report.txt`
- `OpenVisionLab_LineSignal_TransientCue.png`
- `OpenVisionLab_LineSignal_ParametersRetained.png`
- `OpenVisionLab_LineSignal_ManualInspector.png`

Boundary: the change is pushed in Dev `5134e43c` and original `32bc70c`.
