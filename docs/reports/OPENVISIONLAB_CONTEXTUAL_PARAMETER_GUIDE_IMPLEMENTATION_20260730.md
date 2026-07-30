# OpenVisionLab Contextual Property Parameter Guide Implementation

Date: 2026-07-30 KST<br>
Record: P258<br>
Design: `docs/reports/OPENVISIONLAB_CONTEXTUAL_PARAMETER_GUIDE_DESIGN_20260730.md`

## 1. Outcome

The P257 Slice A shared selection contract and Slice B four-family pilot are
implemented.

Selecting or keyboard-focusing a PropertyGrid row now opens an in-Tool
`Parameter Guide` for that exact CLR property. The guide presents:

- localized parameter title and stable property identity;
- current value and unit;
- runtime meaning;
- increase/decrease, enum, or Boolean-option effect;
- suitable image/target conditions;
- risks and interacting parameters;
- the result metric and drawing to inspect after explicit Preview;
- explicit links to related PropertyGrid rows.

The first detailed families are `Matching`, `EdgeBasedMatching`, `LineGauge`,
and `LineDistance`. Every browsable property in those pilot types resolves to
either verified detailed guidance or a visibly labelled Basic fallback.

## 2. Operator Workflow

1. Open a PropertyGrid Tool View.
2. Select a parameter by mouse or keyboard.
3. Read the guide in the same Tool.
4. Optionally select a related parameter link.
5. Change the PropertyGrid value deliberately.
6. Use the existing explicit Preview action.
7. Compare the named result metric and drawing.

Opening, closing, selecting, or navigating the guide does not edit the
parameter and does not execute the Tool.

## 3. Implementation Ownership

- `Library/WpfPropertyGridBridge/WpfPropertyGridAdapter.cs`
  - publishes stable mouse/keyboard selected-property changes;
  - exposes the selected CLR property name;
  - supports related-property focus.
- `UI/VisionTest/Wpf/Tooling/PropertyGuide/`
  - owns guide content, catalog lookup, presentation, and the reusable view.
- `UI/VisionTest/Wpf/Tooling/PropertyGrid/VisionToolPropertyGridHost.cs`
  - connects the shared PropertyGrid host to the guide and refreshes current
    value/language state.
- `UI/VisionTest/Wpf/Tooling/SingleInput/VisionToolSingleInputPropertyToolShell.*`
  - owns the shared collapsible in-Tool drawer placement.
- `Library/OpenVisionLab.Localization/Resources/LocalizationCatalog.tsv`
  - owns Korean/English guide chrome and detailed pilot content.
- `tools/PipelineViewerScreenshotSmoke/Program.cs`
  - owns focused interaction, coverage, side-effect, localization, and current
    UI evidence checks.

## 4. Placement Decision

The first vertical-stack probe reduced the live PropertyGrid to `724 x 258`.
That violated the established `600 x 380` minimum and made routine parameter
editing materially worse.

The final implementation uses a bottom-right overlay drawer inside the
PropertyGrid region:

- collapsed initially;
- automatically expanded on mouse or keyboard parameter selection;
- explicitly collapsible by the operator;
- compact in docked layouts;
- does not consume the PropertyGrid's established editor height.

This keeps the explanation beside the edited parameter without moving the
operator to a modal, manual page, or separate window.

## 5. Semantic Boundaries

- Guidance describes actual property/runtime contracts; it does not recommend
  a universally correct value.
- Missing detailed content is labelled Basic and uses conservative type/tool
  fallback wording.
- Matching guidance requires position/drawing review and does not treat score
  alone as semantic correctness.
- Line guidance directs the operator to the selected edge, signal, and drawing.
- `PIXELPERMM` is stated as `mm/px`; this is not certified calibration.
- Inactive dependent parameters explain which enabling property must be on.
- The guide never changes values, applies presets, saves XML, or runs automatic
  tuning.

## 6. Verification

The following focused checks passed from the current Dev source:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"

dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target p257_contextual_parameter_guide,wpf_shell_host_matching_tool,wpf_shell_host_edge_based_matching_tool,wpf_shell_host_line_tool,localization_catalog_contract_check,wpf_shell_host_blob_tool,wpf_shell_host_contour_tool,wpf_shell_host_feature_matching_tool artifacts\p257_contextual_parameter_guide_20260730\final_validation

dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\OpenVisionLab_Dev

git diff --check
```

Verified behavior:

- mouse and keyboard selection resolve the exact PropertyGrid property;
- related-property navigation focuses `NUM_MATCH`;
- Korean and English content render;
- angle and unique-margin dependent content shows inactive state correctly;
- Basic fallback remains visible;
- Line contrast and `PIXELPERMM` semantic boundaries are present;
- all browsable pilot properties have non-empty title, identity, summary,
  impact, check, and coverage;
- guide interactions preserve Preview/Run count, layers, active layer, input
  route, and output route;
- Matching, EdgeBasedMatching, Line, Blob, Contour, and FeatureMatching shared
  Tool shell regressions pass.

Final results:

- full solution build: `0` warnings, `0` errors;
- all eight focused screenshot-smoke targets: `OK`, with `layout=0`,
  `text=0`, and `internal=0`;
- OpenVisionLab readiness contract: passed;
- `git diff --check`: passed; Git reported only existing LF-to-CRLF working-copy
  notices.

Focused report:

`artifacts/p257_contextual_parameter_guide_20260730/after/parameter-guide-smoke.txt`

## 7. UI Evidence

Before:

`artifacts/p257_contextual_parameter_guide_20260730/before/wpf_shell_host_matching_tool.png`

After, dedicated guide:

`artifacts/p257_contextual_parameter_guide_20260730/after/p257_contextual_parameter_guide.png`

After, same full-shell target:

`artifacts/p257_contextual_parameter_guide_20260730/final_validation/wpf_shell_host_matching_tool.png`

The same-target comparison retains the PropertyGrid, preview/result area, and
explicit action layout while adding the opened contextual drawer.

SHA-256:

- before: `C26DFBEB02885B361C2F8E34FC70E00B9B508D26F5B1559D678ED4D9FBE9D8C4`;
- dedicated after:
  `9AD32C8D47A32D40CFA096E38ECE9F4ABBA3CC12EDB27BA3F361799B66BC00FD`;
- same-target after:
  `C352618635CFE976804535AAA9B2E7B7E20AA05C22362336E5CD5072BA92AEBA`.

## 8. Completion Record

Status: Complete<br>
Scope: Shared PropertyGrid mouse/keyboard selection contract, reusable in-Tool
guide drawer, and detailed Matching/EdgeBasedMatching/LineGauge/LineDistance
pilot with visible fallback for every browsable pilot property.<br>
Acceptance criteria:

- exact mouse/keyboard parameter selection -> pass, focused smoke;
- same-Tool responsive guide without editor regression -> pass, current UI
  capture and PropertyGrid minimum regression;
- detailed four-family pilot and visible fallback -> pass, exhaustive browsable
  property coverage check;
- Korean/English and conditional applicability -> pass, focused smoke;
- related-parameter navigation -> pass, focused `NUM_MATCH` navigation;
- zero Preview/Run/value/layer/route side effects -> pass, focused state
  assertions;
- current-source before/after evidence -> pass, retained artifact paths above.

Verification: Current-source build, focused UI/semantic/regression targets,
OpenVisionLab readiness contract, and diff hygiene checks listed in Section 6
passed.<br>
Evidence:
`artifacts/p257_contextual_parameter_guide_20260730` and this report.<br>
Boundary / next dependency: This does not implement automatic parameter
selection, automatic tuning, automatic Preview/Run, algorithm qualification,
certified metrology, production/field robustness, or CVR-00 participant
evidence. Slice C detailed content for other Tool families remains a separate
request/evidence-triggered priority.
