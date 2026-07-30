# OpenVisionLab Contextual Property Parameter Guide Design

Date: 2026-07-30 KST<br>
Record: P257<br>
Implementation status: Complete in P258<br>
Design status: Complete

## 1. Decision

OpenVisionLab shall provide a contextual `Parameter Guide` inside every
PropertyGrid-based Tool View.

The guide is driven by the PropertyGrid row that the operator selects or
focuses. It explains the selected parameter without requiring the operator to
leave the Tool, open a modal dialog, or search a separate manual.

The guide is not a larger tooltip. It is a structured operator aid that answers:

1. What does this parameter control?
2. What normally happens when the value is increased or decreased?
3. For which image or target condition is it useful?
4. What failure mode or trade-off should the operator watch for?
5. Which result metric and drawing should be checked after explicit Preview?

The existing Learn surface remains the tool-level tutorial. `Parameter Guide`
owns parameter-level assistance. Presets remain deliberate multi-parameter
starting points. These surfaces must link to each other but not duplicate or
replace one another.

### P258 implementation refinement

The implemented v1 uses a collapsible overlay drawer anchored inside the
PropertyGrid area. It starts collapsed, opens when the operator selects or
keyboard-focuses a parameter, and retains an explicit collapse control.

An initial vertically stacked guide probe reduced the existing PropertyGrid to
`724 x 258`, below its established `600 x 380` minimum. That placement was
rejected. The overlay preserves the editor height and explicit Preview action
while keeping guidance in the same Tool. This is an implementation refinement
of the responsive placement contract, not a change to the content or
side-effect contract.

## 2. Evidence For The Design

Current source already contains `DescriptionAttribute` text on many properties,
but the descriptions are not a complete user-facing explanation system:

- content depth varies from a short definition to a partial tuning hint;
- Korean and English source text are mixed;
- descriptions do not consistently state increase/decrease effects;
- interacting parameters and conditional applicability are not consistently
  exposed;
- the operator is not consistently told which result metric or drawing proves
  that the change helped;
- the current shared Tool shell has Learn, presets, verification guidance, and
  result review, but no stable selected-property guide surface;
- the WPF PropertyGrid bridge exposes value changes and stable property names,
  but does not currently expose a public selected-property change contract.

Representative inspected sources:

- `Vision/OpenCV/MatchingProperty.cs`
- `Vision/OpenCV/EdgeBasedMatchingProperty.cs`
- `Vision/OpenCV/LineGaugeProperty.cs`
- `UI/VisionTest/Wpf/Tooling/PropertyGrid/VisionToolPropertyGridHost.cs`
- `Library/WpfPropertyGridBridge/WpfPropertyGridAdapter.cs`
- `UI/VisionTest/Wpf/Tooling/SingleInput/VisionToolSingleInputPropertyToolShell.xaml`
- `docs/contracts/openvisionlab/OPENVISIONLAB_LLM_TOOL_CATALOG.json`

The LLM tool catalog already provides canonical ToolType, aliases, validation
hints, and recommended metrics. It is useful source evidence, but it is not the
runtime UI catalog and must not be displayed verbatim.

## 3. Product And Safety Contract

The guide follows the existing rule-based workbench contract:

- selecting, focusing, expanding, collapsing, or navigating guide content must
  never execute Preview or Run;
- it must not create, delete, select, or change layers;
- it must not change the active layer or Pipeline routing;
- it must not write a property value, apply a preset, or save a Recipe;
- it must not label a value as universally correct;
- it must distinguish a starting point from an acceptance criterion;
- it must keep calibrated units and pixel units explicit;
- it must explain conditional parameters only under the exact conditions in
  which runtime uses them;
- it must not conceal a destructive or safety-sensitive action behind
  remembered state.

Preview and Run remain explicit operator actions. A guide may say what to check
after Preview, but it must never start Preview.

## 4. Operator Experience

### 4.1 Normal workflow

1. The operator opens a Tool and sees its normal PropertyGrid.
2. The guide initially shows `Select a parameter to see its meaning and
   detection impact`.
3. Selecting or keyboard-focusing a property row updates the guide immediately.
4. The operator edits the value in the existing editor.
5. The guide retains the same selected parameter and shows the current edited
   value.
6. The operator explicitly selects `Run Preview`.
7. The guide tells the operator which metrics and drawings to compare, while
   the existing result-review surfaces show the actual result.

No extra confirmation or navigation is required to read the explanation.

### 4.2 Responsive placement

For a sufficiently wide parameter area, the PropertyGrid and guide are shown
side by side:

```text
┌ Parameters ────────────────────────────────────────────────────────┐
│ [Preset] [Learn]                                                   │
│ ┌ PropertyGrid ─────────────────┐ ┌ Parameter Guide ─────────────┐ │
│ │ Matching                      │ │ Min score = 0.75             │ │
│ │ Pattern path      ...         │ │ What it controls             │ │
│ │ Min score         0.75  ◀     │ │ Accepted match strength      │ │
│ │ Match count       1           │ │                              │ │
│ │ Angle range       -10..10     │ │ Raise / lower                │ │
│ │ ...                           │ │ False-positive / miss trade  │ │
│ │                               │ │                              │ │
│ │                               │ │ Check after Preview          │ │
│ │                               │ │ ScoreMax, ScoreMargin, box   │ │
│ └───────────────────────────────┘ └──────────────────────────────┘ │
└────────────────────────────────────────────────────────────────────┘
```

In the docked or narrow layout, the guide is an in-place expandable panel below
the grid:

```text
┌ Parameters ──────────────────────┐
│ PropertyGrid                     │
│ ...                              │
├ ? Parameter Guide: Min score ────┤
│ What / Raise / Lower / Check     │
└──────────────────────────────────┘
```

The operator may explicitly collapse or expand the panel. This visual preference
may be restored at user/workspace scope because the toggle itself is explicit
and restoration has no Recipe or execution effect. The selected parameter,
parameter values, recommendations, and result state are not persisted by the
guide.

### 4.3 Guide card

Every parameter guide card has the following stable structure:

| Block | Required content |
| --- | --- |
| Identity | Localized display name, stable parameter name, current value, unit |
| What it controls | One direct description of the runtime behavior |
| If increased / decreased | Directional effect for ordered values; hidden for non-ordered values |
| Options | Meaning and trade-off of enum/Boolean choices |
| Best used when | Image, target, contrast, pose, or workflow condition |
| Watch for | Miss, false detection, instability, runtime, ambiguity, or incompatible-state risk |
| Related parameters | Stable links that focus another PropertyGrid row without changing its value |
| Check after Preview | Exact metrics, drawings, overlays, object rows, or signal evidence to inspect |
| More | Optional deep link to the relevant Learn topic |

Boolean, enum, path, ROI, and action-like properties do not display artificial
increase/decrease wording. They use the `Options` block instead.

## 5. Content Model

### 5.1 Canonical identity

Guide lookup uses:

```text
Canonical Tool family + CLR property name
```

Examples:

```text
Matching + SCORE_MIN
EdgeBasedMatching + UNIQUE_MATCH_MIN_SCORE_MARGIN
LineGauge + CONTRAST
LineDistance + PIXELPERMM
```

Display labels are never lookup keys because they are localized and may change.
Tool aliases resolve to one canonical family so `TemplateMatching` does not
require a copied guide catalog.

### 5.2 Proposed application-owned model

```csharp
internal sealed record VisionToolParameterGuideDefinition(
    string ToolFamily,
    string PropertyName,
    string SummaryKey,
    string IncreaseImpactKey,
    string DecreaseImpactKey,
    string OptionsKey,
    string BestWhenKey,
    string RiskKey,
    string UnitKey,
    IReadOnlyList<string> RelatedPropertyNames,
    IReadOnlyList<string> MetricNames,
    IReadOnlyList<string> DrawingHints,
    VisionToolParameterGuideCondition AppliesWhen);
```

The exact type may be adjusted during implementation, but these semantic fields
are the minimum contract. Localized prose is stored as localization keys, not
hard-coded inside the View.

### 5.3 Ownership

Recommended responsibility boundaries:

- `VisionToolParameterGuideCatalog`: application-owned semantic definitions;
- `VisionToolParameterGuideResolver`: alias, inherited-property, condition, and
  fallback resolution;
- `VisionToolParameterGuidePresenter`: current property/value projection;
- `VisionToolParameterGuideView`: presentation and accessibility only;
- `WpfPropertyGridBridge`: one reusable selected-property/focused-property event
  that returns the stable CLR property name;
- `VisionToolSingleInputPropertyToolShell`: shared placement and responsive
  layout.

Do not place Tool-specific `if` chains in individual Tool Views. Do not make
Library-Noah classes own localized application guidance. `DescriptionAttribute`
may remain a short compatibility description and search term, but it is not the
canonical guide source.

### 5.4 Fallback policy

Fallback must be honest and visible:

1. exact Tool family + property guide;
2. approved base-property guide for genuinely identical inherited behavior;
3. short existing localized `DescriptionAttribute` with
   `Detailed tuning guidance is not yet available`;
4. stable parameter name and current value only.

Missing content must never produce guessed image effects or a blank panel.

## 6. Pilot Content

The first implementation slice covers the highest-complexity operator paths:

1. `Matching`
2. `EdgeBasedMatching`
3. `LineGauge`
4. `LineDistance`

The pilot must cover every visible property in these four canonical families,
including inherited ROI, threshold, fixture, drawing, and calibration rows that
are actually exposed.

Representative guide behavior:

| Parameter | Required operator explanation |
| --- | --- |
| `Matching.SCORE_MIN` | Raising it rejects weaker candidates and can reduce false matches, but can miss low-contrast or changed targets. Lowering it admits more weak candidates. Check score distribution, result position, and competing-candidate evidence. |
| `Matching.FIND_ANGLE_MIN/MAX` | A wider range covers more rotation but increases work and opportunities for wrong candidates. Narrow to observed production pose. Check detected angle, result box, and elapsed time. |
| `Matching.FIND_ANGLE` | A smaller step tests angles more finely but takes longer; a large step can miss the best pose. It is relevant only when angle search is enabled. |
| `Matching.FIND_SCALE_MIN/MAX/STEP` | Wider/finer scale search covers size variation at a runtime and ambiguity cost. It does not correct perspective or camera calibration. |
| `EdgeBasedMatching.CANNY_LOW/HIGH` | Raising thresholds suppresses weak/noisy edges but can break the taught outline; lowering them retains weak edges and can add clutter. Check the edge model/drawing before score alone. |
| `EdgeBasedMatching.UNIQUE_MATCH_MIN_SCORE_MARGIN` | A larger margin rejects more near-ties as ambiguous; it does not prove the chosen feature is the correct physical feature. Check unique state, plausible alternatives, score margin, and exact drawing. |
| `LineGauge.PRJ_PORALITY` | Explains the expected dark-to-bright or bright-to-dark transition and why the wrong polarity can select another boundary or no boundary. |
| `LineGauge.PRJ_DIR` | Explains the profile traversal direction and first-stable-edge consequence. Check the selected point and any later stable alternative in the signal view. |
| `LineGauge.CONTRAST` | Raising the minimum contrast suppresses weak transitions but can miss a low-contrast true edge; lowering it admits weaker/noisy transitions. Check signed response and selected edge drawing. |
| `LineGauge.THICKNESS` | Explains the exact runtime continuity meaning after source verification. Do not publish a generic smoothing claim. |
| `LineGauge.SAMPLING_STEP` | Explains spatial sampling/runtime trade-offs from the verified runtime implementation. |
| `LineDistance` A/B ROI and direction rows | Explain that the two lines may use different reviewed regions/directions and that editing a shared compact field can affect both only where the existing mapper contract says so. |
| `PIXELPERMM` | State the legacy XML key and its actual runtime meaning: millimetres per pixel. Positive scale enables supported mm metrics; it is not lens calibration or certified metrology. |

The guide text is accepted only after checking the actual runtime owner,
PropertyGrid mapper, XML round trip, result metrics, and drawings. Existing
source descriptions are evidence inputs, not automatic truth.

## 7. Conditional Guidance

Some explanations depend on other values. Conditions must be explicit and
read-only:

- angle range/step: active only when angle search is enabled;
- scale range/step: active only when scale search is enabled;
- unique margin: active only when unique matching is enabled;
- manual angle: active only when manual scan angle is enabled;
- mm scale: meaningful only for Tool families that publish supported calibrated
  metrics and only when the scale is positive;
- ROI-related guidance: distinguish full-image processing, template ROI, search
  ROI, analysis ROI, and measurement ROI.

An inactive parameter remains explainable but is labelled
`Currently inactive because <condition>`. The guide does not enable it.

## 8. Accessibility And Localization

- The guide title and each block receive an accessible name and reading order.
- Property selection works with mouse and keyboard focus.
- Related-parameter links move focus only; they do not edit values.
- Icon-only controls require a tooltip and accessible name.
- Korean and English content are delivered together for the pilot.
- Missing localization keys fail visibly in tests.
- Current value formatting uses invariant data semantics and localized display
  formatting without changing the underlying Recipe value.
- Long text wraps and scrolls inside the guide; it must not be truncated to a
  tooltip-only sentence.

## 9. Explicitly Excluded From V1

- automatic parameter tuning or AI recommendations;
- automatic Preview/Run;
- changing a value from the guide;
- a universal `good value` label;
- per-image parameter mutation;
- new inspection algorithms;
- camera, lighting, PLC, I/O, MES, account, or deployment guidance;
- replacement of Learn, presets, result review, or signal inspection;
- claiming industrial qualification from explanatory text.

## 10. Implementation Slices

### Slice A — Selection and shared shell

- expose stable selected/focused CLR property name from the bridge;
- add the shared responsive guide host;
- prove selection/focus/collapse produces zero execution and routing effects;
- provide fallback-only content.

### Slice B — Matching and Line pilot

- add verified Korean/English definitions for the four pilot families;
- cover all visible pilot rows;
- add conditional applicability and related-parameter focus;
- connect exact metrics, drawings, and signal evidence;
- retain current PropertyGrid search and navigation state.

### Slice C — Remaining Tool families

Prioritize by operator complexity and existing usage:

1. Threshold, Blob, Contour, Morphology, Filter;
2. FeatureMatching, CircleGauge, GeometryMeasure, Fixture/Affine paths;
3. remaining exposed canonical families.

Aliases reuse canonical content. A family is complete only when every visible
row has an exact or approved fallback explanation.

## 11. Acceptance Criteria For Implementation

### Functional

- selecting any pilot property shows the correct canonical guide;
- keyboard and mouse selection produce the same guide;
- editing a value retains the selected guide and refreshes only the displayed
  current value/condition;
- related-parameter navigation focuses the correct row;
- Tool aliases resolve to the same canonical content;
- missing guide/localization content follows the visible fallback policy.

### Safety

- guide operations cause zero Preview and zero Run;
- no layer creation, deletion, selection, active-layer change, or route change;
- no property value, Recipe XML, preset, or saved Pipeline mutation;
- restored panel visibility causes no unrelated action.

### Semantic

- every pilot statement is traced to runtime/property/mapper/result evidence;
- units and conditional applicability are exact;
- the guide distinguishes likely tuning effect from guaranteed behavior;
- matching explanations require result-position/drawing review, not score alone;
- Line explanations reference the actual selected edge and signal/drawing
  evidence;
- pixel-only and calibrated claims remain separate.

### UX and evidence

- the guide remains inside the current Tool in wide and docked layouts;
- no primary PropertyGrid editor or explicit Preview action becomes inaccessible;
- Korean and English layouts are readable;
- fresh current-build before/after captures are retained;
- a focused smoke covers selection, editing, collapse/restore, alias lookup,
  missing-content fallback, and all zero-side-effect contracts;
- an agent-operated beginner-role recording checks the workflow but is not
  labelled CVR-00 or independent novice proof.

## 12. Completion Record

Status: Complete<br>
Scope: Contextual PropertyGrid parameter-help UX, content contract, ownership,
pilot families, rollout, exclusions, and implementation acceptance criteria.<br>
Acceptance criteria:

- same-Tool operator workflow defined -> pass, Sections 3-4;
- structured parameter explanation schema defined -> pass, Sections 4-5;
- Matching/Line pilot scope and examples defined -> pass, Section 6;
- side-effect, localization, accessibility, and fallback contracts defined ->
  pass, Sections 3, 5, 8, and 11;
- implementation boundaries and verification gates defined -> pass, Sections
  9-11.

Verification: Inspected the current PropertyGrid property metadata, bridge,
shared Tool shell, Learn/verification surfaces, localization pattern, and
canonical Tool catalog. `git diff --check` passed after recording P257, and all
seven referenced source/catalog paths were confirmed present.<br>
Evidence: `docs/reports/OPENVISIONLAB_CONTEXTUAL_PARAMETER_GUIDE_DESIGN_20260730.md`<br>
Boundary / next dependency: This completes design only. No UI, catalog,
selection event, localization text, or runtime behavior is proved by this
design record alone. P258 subsequently completed Slice A and the four-family
Slice B pilot. Remaining Slice C is detailed-content expansion to other
PropertyGrid Tool families when a named operator need or current evidence
admits it.
