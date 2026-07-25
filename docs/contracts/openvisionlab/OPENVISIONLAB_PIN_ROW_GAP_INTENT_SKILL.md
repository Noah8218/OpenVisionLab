# OpenVisionLab Pin Row Edge-Gap Consistency Skill

Updated: 2026-07-21 KST

Status: Phase 1 authoring and bounded Phase 2 N-sample evidence complete in Dev; P169 reserves a fresh unused Test split, and P170 freezes target-bearing working Train/Validation manifests. Phase 3 remains pending because the fresh judged GPT first response succeeded directly and produced no genuine correction evidence.

This document defines the first reusable OpenVisionLab inspection-intent skill. It is an in-product Guided Setup/recipe-wizard contract, not a Codex plugin and not a new vision algorithm.

## 1. User-Visible Promise

User-visible name:

> Pin row edge-gap consistency (`PinArrayGap`)

The skill helps an operator create and validate a rule-based recipe that measures every adjacent edge-to-edge clearance in one or more reviewed rows of roughly vertical dark pins.

It does not promise arbitrary image-to-recipe discovery. The operator identifies the intended rows and supplies the tolerance and sample evidence; OpenVisionLab locks the tool family, generates or checks XML, executes only on explicit action, and shows the measurements and drawings used for acceptance.

## 2. Supported And Blocked Scope

| Request | v1 state | Reason |
| --- | --- | --- |
| Dark, roughly vertical pins | Supported | This is the current `PinArrayGap` detection contract. |
| One or more row ROIs | Supported | Every ROI is executed by a separate `PinArrayGap` Step and must contain one row only. |
| Adjacent edge-to-edge clearance | Supported | The runtime returns every gap between adjacent detected pin runs. |
| Pixel consistency gate | Supported | `DistancePxRange` is already verified by P148. |
| Center-to-center pitch | `WAIT - unsupported` | No separately verified center metric exists. |
| Bright pins | `WAIT - unsupported` | The runtime currently detects pixels at or below `DarkThreshold`. |
| Absolute physical mm | `WAIT - calibration required` | The legacy XML key `PIXELPERMM` is calculated as mm/px; a positive value alone is not calibration evidence. |
| Pin length, bend, tip shape, bridge, contamination, or whole-part classification | Out of scope | These require separate intent skills and evidence. |
| Automatic ROI discovery, fixture correction, rotation, or scale compensation | Out of scope | They are not part of the current `PinArrayGap` runtime. |

The user-facing label must say `edge gap`, not `pitch`, until center-to-center pitch is implemented and independently validated.

## 3. Why This Is A Separate Skill

The existing `Pin gap / edge distance (LineDistance)` Guided Setup measures selected local edge pairs through multiple narrow ROIs. It remains useful and must not be replaced.

This skill is a separate template because it measures all adjacent gaps dynamically across each row ROI. It must have its own:

- exact template identity;
- readiness rules;
- `PinArrayGap`-only prompt contract;
- `PinArrayGap`-only LLM intent validation;
- starter builder;
- N-sample evidence state.

An LLM response containing `LineDistance` does not satisfy this skill, and a `PinArrayGap` response does not satisfy the existing LineDistance skill.

## 4. Skill State Model

The UI must not collapse XML validity, measurement execution, and judged inspection into one green state.

| State | Meaning | Allowed next action |
| --- | --- | --- |
| `MISSING` | A required operator input is absent or invalid. | Complete the named field. |
| `WAIT - unsupported` | Bright polarity or center-pitch intent was selected. | Change to the supported intent or use another tool. |
| `MEASURE READY` | Row ROIs and detection inputs are valid, but no frozen acceptance gate exists. | Create/import a measurement-only draft and explicitly run Train samples. |
| `MEASURED` | Train measurements and drawings exist. This is not an OK/NG recipe. | Review geometry and enter/freeze the maximum allowed range. |
| `JUDGEMENT READY` | Explicit tolerance plus Train/Validation/Test references are complete. | Create/validate/import the judged XML. |
| `VALIDATION READY` | Judged XML is imported and the three sample sets are valid and disjoint. | Explicitly run Validation, then frozen Test. |
| `REVIEW` | Batch rows, drawings, and misclassifications exist. | Inspect representative and failed samples. |
| `COMPLETE` | All declared v1 completion gates pass and evidence is recorded. | Preserve as the skill baseline; do not silently retune. |

Generic XML validation success is not enough to enter `JUDGEMENT READY`. A pipeline without `UseAcceptance=true`, `AcceptanceMetricName`, and a bound remains a measurement draft.

## 5. Required Inputs

### 5.1 Before `MEASURE READY`

| Input | Rule |
| --- | --- |
| Measurement definition | Must be `Adjacent edge-to-edge clearance`. Center pitch blocks generation. |
| Pin polarity | Must be `Dark`. Bright blocks generation. |
| Row ROIs | One or more `x,y,width,height` rows; every ROI must be positive and inside the selected source image. Each ROI must contain one pin row only. |
| Unit mode | v1 completion uses `px`. Selecting mm remains WAIT until an independently verified mm/px value and calibration provenance are supplied. |
| Source image/layer | The selected sample must resolve to the `Main` input used by the generated Steps. |

Advanced detection values are visible but start with the current verified defaults:

| Parameter | Default | Readiness rule |
| --- | ---: | --- |
| `DarkThreshold` | 128 | Integer 0..255 |
| `MinDarkCoverageRatio` | 0.55 | Greater than 0 and at most 1 |
| `MinPinWidth` | 5 | Positive integer |
| `MaxPinBreakWidth` | 2 | Non-negative integer |
| `MinGapWidth` | 3 | Positive integer |

Changing an advanced value makes the generated draft stale. It never runs Preview or Run automatically.

### 5.2 Before `JUDGEMENT READY`

| Input | Rule |
| --- | --- |
| Maximum allowed row spread | Positive `DistancePxRange` maximum supplied by an operator specification or frozen after Train review. |
| Train set | Existing local Validation Set containing the samples used for parameter/tolerance selection. |
| Validation set | A different, non-empty existing local Validation Set used without retuning. |
| Test set | A different, non-empty existing local Validation Set run only after the candidate is frozen. |
| Expected outcomes | Every sample has an explicit OK/NG expectation and a row-specific note when the defect is located in only one row. |

The three sets reuse the existing local Validation Set storage. v1 does not add another dataset schema. Their normalized image paths must be pairwise disjoint. The frozen record stores the three set names, sorted image lists or hashes, tolerance, XML hash, and skill version.

An exact visible gap-count gate is optional and off by default. It may be enabled only when representative Good samples prove stable `ResultCount`; P148 Good evidence varied from 12 to 15 gaps.

## 6. Generated XML Contract

### 6.1 Measurement Draft

The measurement draft contains one `PinArrayGap` Step per row ROI, with no acceptance fields. It is explicitly labelled `MEASURE ONLY / NOT JUDGED` in the UI and report.

Every Step has:

- `ToolType=PinArrayGap`;
- `InputLayer=Main`;
- a unique output layer;
- `USE_ROI=true` and the reviewed `CvROI`;
- all five detection parameters;
- `ALLOW_BRANCH_INPUT=true` after the first Step because every row reads `Main`;
- no automatic Preview, Run, import, or layer selection side effect.

### 6.2 Judged Recipe

The judged recipe uses the same locked measurement parameters and adds an explicit range gate to every row Step:

```xml
<?xml version="1.0" encoding="utf-8"?>
<VisionPipeline>
  <Name>Pin_Row_EdgeGap_Consistency</Name>
  <Steps>
    <Step>
      <Name>01_Top_Row_EdgeGap_Range</Name>
      <ToolType>PinArrayGap</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>Top_Row_EdgeGap_Range</OutputLayer>
      <Parameters>
        <Parameter><Key>Name</Key><Value>Top_Row_EdgeGap_Range</Value></Parameter>
        <Parameter><Key>USE_ROI</Key><Value>true</Value></Parameter>
        <Parameter><Key>CvROI</Key><Value>0,120,768,130</Value></Parameter>
        <Parameter><Key>DarkThreshold</Key><Value>128</Value></Parameter>
        <Parameter><Key>MinDarkCoverageRatio</Key><Value>0.55</Value></Parameter>
        <Parameter><Key>MinPinWidth</Key><Value>5</Value></Parameter>
        <Parameter><Key>MaxPinBreakWidth</Key><Value>2</Value></Parameter>
        <Parameter><Key>MinGapWidth</Key><Value>3</Value></Parameter>
      </Parameters>
      <UseAcceptance>true</UseAcceptance>
      <ExpectedSuccess>true</ExpectedSuccess>
      <MaxElapsedMilliseconds>200</MaxElapsedMilliseconds>
      <AcceptanceMetricName>DistancePxRange</AcceptanceMetricName>
      <UseAcceptanceMetricMaximum>true</UseAcceptanceMetricMaximum>
      <AcceptanceMetricMaximum>6</AcceptanceMetricMaximum>
    </Step>
    <Step>
      <Name>02_Bottom_Row_EdgeGap_Range</Name>
      <ToolType>PinArrayGap</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>Bottom_Row_EdgeGap_Range</OutputLayer>
      <Parameters>
        <Parameter><Key>ALLOW_BRANCH_INPUT</Key><Value>true</Value></Parameter>
        <Parameter><Key>Name</Key><Value>Bottom_Row_EdgeGap_Range</Value></Parameter>
        <Parameter><Key>USE_ROI</Key><Value>true</Value></Parameter>
        <Parameter><Key>CvROI</Key><Value>0,330,768,130</Value></Parameter>
        <Parameter><Key>DarkThreshold</Key><Value>128</Value></Parameter>
        <Parameter><Key>MinDarkCoverageRatio</Key><Value>0.55</Value></Parameter>
        <Parameter><Key>MinPinWidth</Key><Value>5</Value></Parameter>
        <Parameter><Key>MaxPinBreakWidth</Key><Value>2</Value></Parameter>
        <Parameter><Key>MinGapWidth</Key><Value>3</Value></Parameter>
      </Parameters>
      <UseAcceptance>true</UseAcceptance>
      <ExpectedSuccess>true</ExpectedSuccess>
      <MaxElapsedMilliseconds>200</MaxElapsedMilliseconds>
      <AcceptanceMetricName>DistancePxRange</AcceptanceMetricName>
      <UseAcceptanceMetricMaximum>true</UseAcceptanceMetricMaximum>
      <AcceptanceMetricMaximum>6</AcceptanceMetricMaximum>
    </Step>
  </Steps>
</VisionPipeline>
```

The example values are the P148 synthetic regression values. `6 px` and the two ROIs are not universal defaults and must not be copied to another image size, magnification, or part without Train review.

An absolute clearance-band extension is outside v1. When added, it must use separately reviewed nominal and consistency gates; average distance alone is not sufficient.

## 7. Strict Skill Validation

The skill validator must reject or keep WAIT when any of the following is true:

- a non-`PinArrayGap` enabled measurement Step appears;
- a Step lacks `USE_ROI=true` or a valid `CvROI`;
- the number/order of row Steps does not match the reviewed ROI list;
- a locked detection parameter is missing or differs from the skill state;
- center pitch or bright polarity is claimed;
- an mm metric is used without verified calibration provenance;
- a judged recipe lacks `DistancePxRange` maximum acceptance on any row;
- output layers collide;
- later row Steps omit the branch-input contract;
- a report calls a no-gate pipeline judged, accepted, or quality-proven.

Generic import validation remains useful for XML/schema/layer checks, but it cannot replace this intent contract. The current generic validator does not require all `PinArrayGap` ROI/detection fields and P151 reported `Intent contract: SKIP`.

## 8. Explicit Operator Workflow

The compact workflow belongs in Recipe Manager `Advanced review > Build inspection`:

1. Select `Pin row edge-gap consistency (PinArrayGap)`.
2. Select the current sample and teach one or more row ROIs.
3. Confirm `Dark` and `Adjacent edge-to-edge clearance`; unsupported selections show WAIT.
4. Create a measurement draft or copy the constrained LLM prompt. Neither action imports or runs it.
5. Validate and explicitly import the returned/generated XML.
6. Explicitly run Train and review drawings before freezing the range maximum.
7. Select three existing local Validation Sets as Train, Validation, and Test.
8. Generate/import the judged XML, explicitly run Validation, then run frozen Test.
9. Open the existing batch error table and per-sample drawing review.
10. Mark the skill complete only when the completion record is saved.

Guided Setup owns input capture and readiness. Pipeline owns Steps/routes/gates and explicit execution. Pipeline Review owns selected-Step evidence. Local Validation Set and Run History own N-sample outcomes. Recipe Manager must link to these surfaces rather than duplicating them into another dashboard.

## 9. N-Sample Report Contract

The operator-facing table must expose at least:

| Field | Purpose |
| --- | --- |
| Sample, split, expected outcome | Dataset provenance and expected class |
| Actual outcome | Correct accept, false reject, false accept, or correct reject |
| Failed row/Step | Identifies which row caused NG |
| Pin count and gap count | Detects missing/merged candidate geometry without claiming a stable count gate |
| `DistancePxMin/Max/Avg/Range` | Explains the measurement and range decision |
| Elapsed time and runtime error | Separates correctness from performance/execution failure |
| Source, runtime overlay, XML hash | Makes the result visually and reproducibly reviewable |

Current-run drawings must show:

- the reviewed ROI for each row;
- green rectangles on selected pin runs;
- red lines across every measured adjacent gap;
- gap labels and the row's min/max/average/range;
- expected and actual outcome;
- the exact failing gate when NG.

The reviewer must inspect, at minimum:

- one ordinary expected-Good sample;
- the Good sample nearest the frozen boundary;
- the NG sample nearest the failing boundary;
- the worst range outlier;
- every misclassification.

If the rectangles or gap lines do not correspond to the intended physical pins/gaps, the skill remains `Incomplete` even when all numeric rows pass.

## 10. Phase Exit Gates

### Phase 1 - Authoring Contract

Complete only when:

- the dedicated template is selectable without changing the existing LineDistance template;
- supported/unsupported inputs produce the correct readiness state;
- the generated prompt and deterministic draft lock `PinArrayGap` and the reviewed values;
- measurement-only and judged states are visually distinct;
- judged XML has a strict skill-contract pass plus generic validation/import pass;
- opening or editing Guided Setup does not run Preview/Run, create a layer, or change routing.

### Phase 2 - N-Sample Evidence

Complete only when:

- Train, Validation, and Test file lists are non-empty and pairwise disjoint;
- detection parameters and tolerance are frozen after Train;
- Validation and Test run unchanged;
- execution/image-load/runtime errors are zero;
- the error table and exact runtime drawings are retained;
- all expected Good and declared `pitch_error` samples in the frozen pilot Test set are correct;
- no result is claimed for short-pin, bridge, bend, missing-pin, or other excluded defects.

P148 is the regression baseline: two row ROIs, Train/Validation/Test 356/72/72, frozen `DistancePxRange <= 6`, Test Good 36/36 accepted, and Test `pitch_error` 12/12 rejected. It is synthetic evidence already reviewed, not a new blind Phase 3 set and not a universal tolerance.

P168 completes this Phase 2 gate for the bounded adjacent edge-gap intent:

- The exact P148 lists were reused without overlap: Train 356, Validation 72, and Test 72. The source split-list file SHA-256 values are Train `4BD979B72B5AB6E61689C0609C05DB570658B77AC05AE4859D92914ED133F20E`, Validation `80D7B1895491459C909FB1565396EBB5F8DC4A463E7B3EF41DEEA00A9CF8747D`, and Test `F4F483C5FE01B54191D1FD2C1F6DA53D58D27437714D41F110D3F72057D6A3EC`. The product's frozen Local Validation Set identity is a separate canonical hash over path, expected outcome, notes, and each image's SHA-256.
- The unchanged frozen two-row XML has SHA-256 `9F8F60E615B9F90CA9D010BE0EC43C0C897BDB3BE5BA0333CF810E0DE139A4F2`, row ROIs `0,120,768,130` and `0,330,768,130`, and the same maximum `DistancePxRange <= 6` on both enabled Steps.
- Current-source replay produced zero image-load/runtime errors. Train accepted expected Good 178/178 and rejected `pitch_error` 38/38; Validation accepted expected Good 36/36 and contains no `pitch_error`; frozen Test accepted expected Good 36/36 and rejected `pitch_error` 12/12.
- Cross-defect rows remain observations, not supported classifications: Train rejected bend 34/38, missing 35/38, short 2/38, and bridge 0/26; Validation rejected bend 11/12, missing 12/12, and bridge 0/12; Test rejected short 0/12 and bridge 0/12.
- The Guided Setup now selects three existing local Validation Sets, freezes the skill version/XML/range/ROI/detection/split identities, reports later drift as stale, and opens the existing explicit-run Local Validation path. These setup actions do not run Preview/Run or change layer/routing state.
- Run Report storage retains every executed `PinArrayGap` row drawing for a sample. The existing viewer can select each stored row and show expected/actual outcome plus row metrics without rerunning the recipe.
- Full sample/error rows are under `artifacts\p168_pinarraygap_phase2_20260720\current_runner`. Representative current-run drawings are under `artifacts\p168_pinarraygap_phase2_20260720\representative_overlays`, including ordinary Good, boundary Good, nearest and worst `pitch_error` NG, and one explicitly excluded short-pin outlier. Final current-source multi-row stored-drawing workflow evidence is under `artifacts\p168_pinarraygap_phase2_20260720\multistep_current_source_verified`; its `source.png` SHA-256 matches the Run Report and both executed row overlays are retained.

Phase 2 is therefore `Complete` only for dark-pin, pixel, adjacent edge-gap consistency on this frozen pilot. It does not establish center-to-center pitch, calibrated units, or a bend/missing/short/bridge/contamination classifier. The reviewed P148 Test list is not previously unused Phase 3 evidence.

### Phase 3 - Genuine LLM Correction

Complete only when:

- a real first LLM response fails strict validation, runtime geometry review, or Validation evidence naturally;
- the exact original prompt, response, failure report, and drawings are preserved before correction;
- the correction packet uses Train/Validation evidence only;
- the corrected XML remains inside the locked `PinArrayGap` skill contract;
- a previously unused Test set is run once after candidate freeze;
- target defect recall improves without reducing Good recall or execution success below the declared gate.

P151 is direct-success authoring evidence, not a correction loop. P147 is a different broad multi-Step recipe and cannot be used as this skill's correction success. Do not manufacture a failure.

P169 supplies the previously missing held-out prerequisite but not the correction event. A new constrained judged GPT response matched the two-row dark-pin contract, passed strict validation, accepted Train Good 178/178, rejected Train `pitch_error` 38/38, and accepted Validation Good 36/36 with zero load/runtime errors. The native Validation split has no `pitch_error`. Because the first response succeeded directly, no correction was requested and the frozen 72-image Test split was not executed. Preserve that Test until a later judged first response fails naturally; do not repeat equivalent prompts merely to obtain a failure.

P170 prepares the next attempt's target-bearing working evidence without changing P169. Working Train contains Good 178 / `pitch_error` 26; Working Validation contains Good 36 / `pitch_error` 12. The 12 pitch rows are selected deterministically from the already executed native Train by lowest image-content hash and removed from Working Train. Both manifests are path/content-disjoint from each other and from the 48 target rows in the P169 Test manifest. Because all working rows were previously executed, label this `previously observed working Validation`, never blind or unused Validation. It is a pre-Test correction check only; Phase 3 still requires a future natural first-response failure and the frozen Test remains unexecuted.

## 11. Minimal Implementation Map

Implement the contract incrementally, without a new framework:

1. Add a separate exact-match template to `OpenVisionRecipeGuidedSetupCatalog` and `OpenVisionRecipeLlmIntent`.
2. Add a focused `OpenVisionRecipePinArrayGapIntentSkill` builder/parser in the existing `Recipe/IntentSkills` owner; reuse existing ROI parsing without creating a generic abstraction.
3. Extend `OpenVisionRecipeGuidedSetupReadinessPresenter` with the two readiness layers: measurement and judgement/validation.
4. Extend `OpenVisionRecipeLlmPromptBuilder`, `OpenVisionRecipeLlmTemplateDraftBuilder`, and `OpenVisionRecipeLlmDraftValidationRules` with the strict `PinArrayGap` contract.
5. Add only the compact intent inputs/status to the existing Build inspection surface. Reuse Local Validation Set, batch result, Run History, Pipeline Review, and drawing viewers.
6. Add focused direct/screenshot smoke checks for supported dark edge-gap, blocked bright/center-pitch, stale draft, no-gate versus judged reporting, invalid scale, exact XML, and no-auto-run/layer/routing changes.

No new algorithm, dataset database, background runner, provider automation, API key flow, or duplicate Recipe Manager dashboard is part of v1.

## 12. Evidence Basis And Honest Limits

- P151 proves that a real GPT first response can produce a runnable one-row `PinArrayGap` measurement draft. It had no acceptance gate and does not prove a completed product skill.
- P148 proves two-row synthetic edge-gap range consistency with frozen split evidence and runtime drawings. A single top row detected only part of the labelled pitch defects, so the complete pilot must retain both reviewed rows or use row-specific labels.
- P168 completes the product-integrated Phase 2 replay and frozen identity/drawing review for that exact P148 contract. It does not convert the P148 Test split into fresh Phase 3 evidence.
- P169 preserves a new unchanged judged GPT first response, strict-validation report, Train/Validation CSVs, exact runtime drawings, and a frozen non-overlapping Test manifest under `artifacts\p169_pin2_phase3_prerequisite_20260721`. It is direct-success evidence, not Phase 3 completion; Test execution remains intentionally absent.
- P170 freezes target-bearing, previously observed working Train/Validation manifests under `artifacts\p170_pin2_target_validation_readiness_20260721`. They remove P169's target-free Validation weakness but are not independent held-out evidence and do not authorize Test execution.
- P147 demonstrates the shape of a useful misclassification/error packet, but its 52.40% broad recipe result is not `PinArrayGap` correction evidence.
- P167's dedicated strict validator enforces the current ROI, polarity, parameter, unit, and judged-recipe contract; generic XML validation alone remains insufficient.

## 13. Reusable Completion Record

```text
Status: Complete | Blocked | Incomplete
Skill/version: Pin row edge-gap consistency / v1
Scope: dark roughly vertical pins; adjacent edge-to-edge px consistency; <row count> reviewed rows
Inputs: ROI list, detection parameters, range maximum, Train/Validation/Test set names
Frozen identity: XML hash, split list/hash, tolerance source, build/runtime identity
Acceptance criteria: Phase 1 -> pass/fail; Phase 2 -> pass/fail; Phase 3 -> pass/fail
Verification: commands and explicit runs actually performed
Evidence: prompt/response, XML, CSV/error table, representative drawings, completion report
Boundary / next dependency: excluded defects, calibration status, genuine correction prerequisite
```
