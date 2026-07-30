# OpenVisionLab Parameter Guide Family Expansion

Date: 2026-07-30 KST  
Record: P259  
Depends on: P257 design and P258 shared Parameter Guide implementation

## 1. Outcome

Detailed contextual parameter guidance now covers:

- `Threshold`;
- `Blob`;
- `Contour`;
- `Morphology`;
- `Filter`.

The expansion uses the existing P258 guide drawer and does not add a second
help system. Blob and Contour continue to use the shared PropertyGrid
mouse/keyboard selection contract. Threshold, Morphology, and Filter use
dedicated parameter cards rather than PropertyGrid, so their visible controls
now publish the equivalent focus/click parameter identity to the same guide.

## 2. Operator Contract

For every browsable property in the five families, the guide provides verified
detailed coverage rather than Basic fallback:

- localized title and stable CLR identity;
- current value with `GV`, `px`, or `px²` where applicable;
- runtime meaning and value/option effect;
- conservative tuning risk;
- exact Preview/result evidence to inspect;
- related-parameter navigation where the owning editor exposes the target.

Conditional guidance is explicit:

- Threshold Basic/Range/Adaptive properties identify the required `Mode`;
- Filter kernel/Median/Bilateral properties identify the required
  `FilterType`;
- Contour `EPSILON` identifies `USE_APPROXPOLYDP` as its enabling property;
- inherited binary/adaptive/ROI/multi-ROI/masking settings identify their
  enabling switches.

## 3. Implementation

- `VisionToolParameterGuideCatalog`
  - adds five canonical families;
  - adds inherited OpenCV preprocessing/ROI/masking definitions;
  - supports enum-value applicability in addition to Boolean applicability;
  - publishes exact area, kernel, threshold, and spatial units.
- `VisionToolCustomParameterGuideBinder`
  - maps dedicated parameter controls to stable property identities;
  - refreshes current values after binding updates;
  - supports keyboard focus, mouse selection, language changes, and related
    control focus;
  - does not mutate values or execute the Tool.
- Shared Tool shell
  - separates guide visibility from PropertyGrid visibility so the same drawer
    can serve dedicated parameter-card Tools.
- Localization catalog
  - adds Korean/English parameter names and verified semantic content.

## 4. Evidence

Before:

`artifacts/p259_parameter_guide_expansion_20260730/before/wpf_shell_host_threshold_tool.png`

After:

`artifacts/p259_parameter_guide_expansion_20260730/final_validation_enum/p259_parameter_guide_expansion.png`

Focused report:

`artifacts/p259_parameter_guide_expansion_20260730/after/p259-parameter-guide-smoke.txt`

The focused target proves:

- detailed coverage for every browsable property in all five families;
- PropertyGrid mouse/keyboard plus dedicated-control focus selection;
- Korean and English content;
- Threshold, Filter, and Contour conditional applicability;
- `GV`, `px`, and `px²` unit presentation;
- zero guide-caused Preview/Run, layer, active-layer, input-route, or
  output-route mutation.

Final verification:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"

dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target p259_parameter_guide_expansion,p257_contextual_parameter_guide,wpf_shell_host_threshold_tool,wpf_shell_host_blob_tool,wpf_shell_host_contour_tool,wpf_filter_morphology_layout_guard,localization_catalog_contract_check artifacts\p259_parameter_guide_expansion_20260730\final_validation

dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target p259_parameter_guide_expansion,localization_catalog_contract_check artifacts\p259_parameter_guide_expansion_20260730\final_validation_enum

dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\OpenVisionLab_Dev

git diff --check
```

- full solution build: `0` warnings, `0` errors;
- all seven focused/current-source targets: `OK`, `layout=0`, `text=0`,
  `internal=0`;
- after refining enum-condition wording, the exact-final-source P259 and
  localization targets also passed;
- localization keys: unique;
- readiness contract: passed;
- diff hygiene: passed; Git reported only existing LF-to-CRLF notices.

UI evidence SHA-256:

- before:
  `6F82643B70B93175F0986C67F9FF8D143EE83D8F0ECECBBF5F3BF3BEB7B2D49B`;
- final after:
  `2C9CC864ACAC0847B43412E5191C6DE6E8D7FCDFDF431066FE7443013C655F1E`.

The Threshold test deliberately waits for the existing debounced Preview caused
by changing the mode during test setup before measuring guide selection. The
subsequent focus/guide operation itself preserves all execution and routing
state. P259 does not change that pre-existing mode-change behavior.

## 5. Boundary

This is explanation and selection-contract evidence. It does not:

- automatically choose or tune values;
- automatically Preview or Run;
- qualify Threshold, Blob, Contour, Morphology, or Filter on production data;
- prove certified metrology, unseen-data robustness, or field qualification;
- complete CVR-00;
- add a parameter that the owning Tool does not currently expose, such as the
  Morphology custom Tool View's stored `Iterations` value.

The stored `Iterations` property still receives detailed guidance in any
PropertyGrid surface that exposes it; P259 does not add a new custom-view editor
for it.

## 6. Completion Record

Status: Complete  
Scope: Detailed contextual guidance and selection integration for Threshold,
Blob, Contour, Morphology, and Filter.  
Acceptance criteria:

- every browsable property resolves detailed Korean/English content -> pass,
  exhaustive focused coverage assertion;
- PropertyGrid and custom parameter-card selection update the same guide ->
  pass, focused UI smoke;
- mode/type/enabling-state guidance is exact -> pass, Threshold/Filter/Contour
  conditional assertions;
- current value and unit are visible -> pass, `GV`, `px`, `px²` assertions;
- guide operations preserve explicit execution/layer/routing contracts -> pass,
  focused state assertions;
- fresh current-source before/after UI evidence retained -> pass, artifact
  paths above.

Verification: Full solution build, focused P259 target, related P257 and Tool
shell regressions, localization uniqueness/contract, readiness contract, and
diff hygiene all passed as recorded in Section 4.  
Evidence:
`artifacts/p259_parameter_guide_expansion_20260730` and this report.  
Boundary / next dependency: Remaining families stay on visible Basic fallback
until their runtime/property contracts are audited and the user requests the
next bounded expansion. CVR-00 remains externally deferred.
