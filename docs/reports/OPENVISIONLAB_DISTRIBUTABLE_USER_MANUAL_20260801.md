# OpenVisionLab Distributable User Manual

Date: 2026-08-01 KST

## Scope

- Fix the Guide command so a copied or released runtime opens a real offline
  manual instead of a moved-path stub.
- Explain the normal operator workflow one step at a time.
- Cover all 17 Tool entries shown in the current navigation.
- Add current OpenVisionLab UI images with numbered click/review order for every
  manual chapter.
- Keep Preview and Run Review explicit.

## Result

- The application build owns `Guide\OpenVisionLab_User_Manual.html` and
  `Guide\guide-manifest.json`.
- The Guide resolver validates the file name, schema version, manual marker,
  and SHA-256 before opening it. A development fallback is allowed only under a
  verified checkout containing both `OpenVisionLab.sln` and `.git`.
- The old `OPENVISIONLAB_TUTORIAL*.html` moved stubs are not resolver candidates.
- The manual is generated from 26 Markdown chapters: 6 common workflows, 17
  Tool chapters, and 3 reference chapters.
- `docs/manual/manual-visuals.json` maps every chapter to a current-source WPF
  capture and numbered callout descriptions. Twenty-two unique UI images are
  reused where the same screen owns more than one explanation.
- Number markers are HTML/CSS overlays over the untouched capture. The marker
  and its text key stay paired in desktop and narrow layouts.
- The generated offline HTML contains 26 figures and all images as data URIs.

## Verification

- Manual builder Release build: PASS, warnings 0, errors 0.
- Manual builder run: PASS, sections 26, tools 17.
- Generated manual SHA-256:
  `3745E7FE31A77FC9493EECE439B50606D00C65735A0E0CC08C2D0A95E02D796C`.
- OpenVision readiness: PASS, including visual coverage and callout counts.
- `tools\BuildCleanRuntime.ps1 -Mode Dev -OutputDir artifacts\p281_distributable_user_manual_20260801\clean_runtime_r3`: PASS, warnings 0, errors 0.
- The runtime copied to
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\guide_distribution_smoke\p281_20260801_r3`
  retained 103/103 files with matching length and SHA-256.
- The copied Guide hash matches `guide-manifest.json`; manual size is 3,915,156
  bytes; section/figure/Tool-figure counts are 26/26/17.
- Actual copied EXE smoke: PASS. The window rectangle
  `-1880,400,1400,900` intersected the dynamic leftmost monitor
  `\\.\DISPLAY2` bounds `-1920,360,1920,1080`; direct UI Automation invoked
  `HostGuideButton`; no missing/damaged/open-failed dialog appeared.
- Browser visual check: PASS.
  - desktop numbered UI capture rendered correctly;
  - Contour search and exact navigation reached `#tool-contour`;
  - 390x844 layout used one-column callout keys and had zero horizontal overflow;
  - 26/26 figures loaded; 17/17 Tool figures were present;
  - marker/key counts matched; browser error log was empty.
- `git diff --check`: PASS.

## Evidence

- Source and generator: `docs/manual`, `tools/OpenVisionUserManualBuilder`.
- Current-source UI captures:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\manual_ui_captures`.
- Browser evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\manual_html_preview\p281_20260801_visual_r1`.
- Clean runtime:
  `artifacts\p281_distributable_user_manual_20260801\clean_runtime_r3`.
- Copied distribution:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\guide_distribution_smoke\p281_20260801_r3`.

## Boundary

- Tool screenshots are current-source WPF view captures. The Guide-button check
  is an actual copied-EXE smoke. This is not a promotional recording.
- This proves manual packaging, navigation, rendering, and the documented UI
  locations. It does not qualify any inspection algorithm, production data, or
  arbitrary Recipe.
- CVR-00 still requires observations from three independent first-time users;
  the numbered manual does not substitute for that study.

```text
Status: Complete
Scope: Distribution-safe Guide command and offline numbered beginner/operator manual covering 6 workflows, 17 Tool chapters, and 3 references
Acceptance criteria: copied runtime contains Guide HTML and manifest -> pass; moved stubs cannot be selected -> pass; manifest/hash fail-closed contract -> pass; every chapter has a current UI figure and matching numbered key -> pass; all 17 Tool entries covered -> pass; desktop/mobile search and navigation -> pass; actual copied EXE Guide invocation -> pass
Verification: manual builder PASS; readiness PASS; clean runtime build 0 warnings/errors; 103/103 copied files length/SHA-256 matched; browser 26 figures/17 Tool figures/0 unloaded/0 horizontal overflow/0 errors; actual EXE Guide smoke PASS on dynamic leftmost monitor; git diff check PASS
Evidence: docs/manual; docs/manual/generated/OpenVisionLab_User_Manual.html; artifacts/p281_distributable_user_manual_20260801/clean_runtime_r3; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\guide_distribution_smoke\p281_20260801_r3; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\manual_html_preview\p281_20260801_visual_r1
Boundary / next dependency: no algorithm or production qualification is claimed; CVR-00 separately requires three independent first-time participants
```
