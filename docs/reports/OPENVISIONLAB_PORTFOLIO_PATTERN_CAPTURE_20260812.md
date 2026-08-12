# OpenVisionLab Portfolio Pattern Capture

Date: 2026-08-12 KST
Status: Complete in Dev working tree

## Scope

- Make the existing image Matching search appropriate for the approved card
  demonstration: angle `-20..20`, scale `0.85..1.15`, and coarse top-K `3`.
- Show current-run detection drawings in Pipeline Review instead of a plain
  output bitmap or an older display-layer image.
- Keep layer names readable and editable from the main toolbar.
- Produce clean current-build portfolio captures without Computer Use,
  cursor visualization, Codex chrome, or a desktop background.

This work does not add a new matching algorithm, automatic tuning, camera,
lighting, PLC/I/O, calibrated metrology, or field qualification.

## Operator Result

The final 2x3 workspace compares five executions of the same taught pattern
and one Threshold result:

| Case | Runtime score |
| --- | ---: |
| Angle 0, scale 1.00 | 99.072 |
| Angle +15, scale 1.00 | 94.340 |
| Angle -10, scale 1.00 | 91.949 |
| Angle 0, scale 0.90 | 98.978 |
| Angle +10, scale 1.10 | 97.903 |

Each Matching image is the exact runtime overlay. The green rectangle marks
the selected pattern boundary, the cross marks its center, and the label
records the result rank, score, angle, and scale. The separate Pipeline Review
capture shows the original input and current-run rendered output side by side.

## Current Evidence

Artifact root:
`D:\OpenVisionLab-TestData\OpenVisionLab\portfolio_pattern_rotation_scale_20260812`

- Final actual-EXE captures:
  `final_actual_exe_clean\01_pattern_rotation_scale_2x3.png`
  and
  `final_actual_exe_clean\02_pipeline_review_runtime_overlay.png`.
- Executable/capture report:
  `final_actual_exe_clean\report.txt`.
- Exact five-case image, template, score, and SHA-256 reports:
  `postfix_runtime_evidence_narrow`.
- Exact layer images used in the six-pane view: `capture_input\01.png` through
  `capture_input\06.png`.
- Recipe:
  `recipe\Pattern_Rotation_Scale_Inspection.xml` with SHA-256
  `6CB347143D147EAD75398770096245A18EEB7F07AA6F5137549A681B7B4B8760`.
- Source image SHA-256:
  `CF0BC8B28867DDAA30F9AC3F3A712F2CFB71C8DE4E5DED60AAC665B9030D85B3`.
- Capturing executable SHA-256:
  `74083017BE28500BAC517C089F6A187DEA56D2F5809F125836051FF8EB818FD3`.

The report records `\\.\DISPLAY2`, its bounds and work area, the actual window
intersection, six layers, six panes, the recipe identity, and the explicit
Pipeline Review completion state. The executable is a current-source Debug
`OpenVisionLab.exe` built with the embedded smoke entry enabled only to run the
repeatable capture scenario. It is not the published release candidate.

The closest reproducible baseline is retained under
`final_native_actual_exe_render`; a true pre-edit current-source capture was
not available after the correction had already been applied.

## Verification

The following checks passed after the final source change:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_layer_rename_command,wpf_pipeline_review_matching_overlay <D-drive-output>
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
git diff --check
```

- Solution build: 0 warnings, 0 errors.
- Focused UI smoke: 2/2 passed. It verifies a visible non-empty layer-name
  edit/rename with no Preview/Run or route side effect, and verifies that
  Pipeline Review prefers the current-run matching overlay.
- Readiness: all 13 contracts passed.
- Vendored DLL and public sample asset gates passed.
- The final two PNG files were opened at original resolution and inspected for
  clipped text/icons, hidden input text, overlap, matching geometry, scores,
  and the absence of Computer Use or Codex visual residue.

## Closure Record

```text
Status: Complete
Scope: bounded card Matching comparison, Pipeline Review runtime drawing, visible layer rename, and clean actual-EXE portfolio capture
Acceptance criteria: five approved transforms detect the taught lower pattern above score 80; six named layers render in a 2x3 grid; Pipeline Review shows current-run overlay; capture contains no Computer Use/Codex residue -> pass
Verification: Debug solution build 0/0; focused UI smoke 2/2; readiness 13/13; external references PASS; public samples PASS; exact EXE capture report PASS
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab\portfolio_pattern_rotation_scale_20260812\final_actual_exe_clean and this report
Boundary / next dependency: portfolio evidence only; no release, physical-part robustness, calibrated metrology, camera, lighting, or PLC/I/O qualification
```
