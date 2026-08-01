# OpenVisionLab Localized User Manual

Date: 2026-08-01 KST

## Scope

- Open the Korean manual when the application language is Korean.
- Open the English manual when the application language is English.
- Package both manuals with the copied/released EXE.
- Keep the 26-chapter beginner workflow and all 17 Tool chapters equivalent.
- Use current UI captures in the same language as each manual.
- Record the behavior as a durable `AGENTS.md` contract.

## Result

- Guide now reads `OpenVisionLanguageService.CurrentLanguage` at click time.
- Korean selects `Guide\OpenVisionLab_User_Manual.ko.html`; English selects
  `Guide\OpenVisionLab_User_Manual.en.html`.
- `guide-manifest.json` schema 2 owns exactly one `ko` and one `en` entry with
  separate SHA-256 values. Missing, duplicate, renamed, damaged, or incorrectly
  marked selected-language content fails closed. There is no cross-language
  fallback.
- Both manuals contain 26 chapters, 17 Tool chapters, 26 numbered UI figures,
  and 102 numbered callouts. The English source contains no Korean syllables in
  headings, body text, captions, or search copy.
- Twenty-one unique English UI images were captured from the current source.
  The four Korean shell images affected by the final navigation-width fix were
  also recaptured. Other Korean standalone Tool/Pipeline/log captures were not
  affected by that shell-only layout change.
- The sample-ready workflow strip and its two action buttons now recalculate in
  English instead of retaining fixed Korean text.
- The top navigation allocation changed from 300 to 360 pixels because the
  copied English EXE exposed `Guide` visually as only `G`. The current copied
  EXE shows the full `Learn` and `Guide` labels.
- `AGENTS.md` now requires same-language Guide selection, same-language UI
  evidence, structural parity, fail-closed validation, and a Korean -> English
  -> Korean no-side-effect verification round trip.

## Final package

| Language | File | SHA-256 |
| --- | --- | --- |
| Korean | `OpenVisionLab_User_Manual.ko.html` | `070FD81D92F528B1C672E920E145FF8FAEA85F11F7A45DCB12E17C16ADF2C0CB` |
| English | `OpenVisionLab_User_Manual.en.html` | `59B547F9A9D332EAA13B4B27A2CB8CEB9E3DD724D2882752C94A9E394A998D49` |

The final copied runtime is
`artifacts\p282_localized_user_manual_20260801\clean_runtime_r4`. Its `Guide`
folder contains only the two language files and `guide-manifest.json`; the old
generic `OpenVisionLab_User_Manual.html` is absent.

## Verification

- `dotnet run --project tools\OpenVisionUserManualBuilder\OpenVisionUserManualBuilder.csproj -c Debug`
  - `ko PASS`, 26 chapters, 17 Tools.
  - `en PASS`, 26 chapters, 17 Tools.
  - final hashes match the table above.
- Direct source parity and language scan
  - Korean/English section IDs and order matched 26/26.
  - Korean/English Tool IDs and order matched 17/17.
  - Korean/English visual-map section IDs and order matched 26/26.
  - English manifest, visual map, and chapter sources contained zero Korean
    syllable matches.
  - generated Korean and English HTML contained zero duplicated `CRCRLF`
    newline sequences.
- `manual_guide_language_contract`
  - Korean -> `.ko.html`, English -> `.en.html`, Korean -> `.ko.html`.
  - a missing English file did not fall back to Korean.
  - a changed English file failed SHA-256 validation.
- Current-source screenshot smokes
  - all 21 English visual sources: `OK`, image/layout/text/internal checks `OK`.
  - refreshed Korean empty/sample/Recipe Manager/Contour shell sources: `OK`.
  - English empty workspace and compact tool rail after the navigation fix:
    `OK`; `HostGuideButton` and localized `Menu.Guide` text are present.
- Browser rendering check
  - both languages rendered 26 sections, 26 figures, 102 callouts, and 26
    callout keys; all embedded images loaded and desktop horizontal overflow was
    false.
  - English search reduced the page to the matching Affine chapter.
  - 375x844 effective mobile width had no horizontal overflow.
  - browser warnings/errors: 0.
  - This rendering run preceded the final capture-only image refresh. The HTML
    structure, CSS, scripts, and chapter sources did not change afterward; the
    final embedded images were separately opened and the rebuilt package was
    checked by the builder/readiness/hash gates.
- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"`
  - 0 warnings, 0 errors.
- `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\OpenVisionLab_Dev`
  - all readiness contracts passed.
- `tools\TestDocumentationIndex.ps1`
  - `PASS`, 51 indexed paths, 11 routes, 99 root redirects.
- `git diff --check`
  - passed with no whitespace errors.
- `tools\BuildCleanRuntime.ps1 -Mode Dev -OutputDir artifacts\p282_localized_user_manual_20260801\clean_runtime_r4`
  - 0 warnings, 0 errors.
  - copied Guide file count 3; both hashes matched; generic file absent.
- Actual copied EXE
  - launched from the `clean_runtime_r2` runtime on the dynamically selected leftmost monitor
    `\\.\DISPLAY2`, bounds `-1920,360,1920,1080`.
  - actual window rectangle `-1920,360,1600,900` intersected that monitor.
  - persisted English started correctly and `HostGuideButton` exposed the name
    `Guide`.
  - before/after evidence shows the clipped `G` changed to the full `Guide`.
  - The later `clean_runtime_r4` rebuild changed only generated manual newline
    normalization and matching hashes/manifest; application source, binaries,
    and the captured navigation layout did not change.

## Evidence

- Sources and generated output: `docs\manual`
- Language contract and current-source captures:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\manual_ui_captures\p282_english_manual_20260801`
- Korean refreshed shell captures:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\manual_ui_captures\p282_korean_manual_20260801`
- Browser evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\manual_html_preview\p282_20260801`
- Copied-EXE monitor and before/after evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\exe_smoke\p282_20260801`
- Final copied runtime:
  `artifacts\p282_localized_user_manual_20260801\clean_runtime_r4`

Status: Complete
Scope: Language-specific packaged Guide selection, structurally equivalent Korean/English manuals, same-language current UI evidence, and visible Guide navigation.
Acceptance criteria: Korean -> Korean pass; English -> English pass; no cross-language fallback pass; both manuals packaged and hash-verified pass; same-language UI captures pass; AGENTS contract pass.
Verification: Manual builder, language resolver fail-closed smoke, current-source WPF captures, desktop/mobile browser checks, full solution build, readiness, clean runtime, copied-file hashes, and actual copied-EXE leftmost-monitor smoke passed.
Evidence: docs/manual; docs/reports/OPENVISIONLAB_LOCALIZED_USER_MANUAL_20260801.md; artifacts/p282_localized_user_manual_20260801/clean_runtime_r4; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\manual_ui_captures; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\manual_html_preview\p282_20260801; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\exe_smoke\p282_20260801.
Boundary / next dependency: This proves language routing, packaging, current UI linkage, and rendering for Korean/English. It does not replace CVR-00 observations from three independent first-time users or qualify inspection algorithms on production data.
