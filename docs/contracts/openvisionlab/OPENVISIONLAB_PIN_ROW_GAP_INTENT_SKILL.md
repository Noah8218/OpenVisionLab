# OpenVisionLab Pin Row Edge-Gap Consistency Skill

Updated: 2026-07-21 KST


This document defines the first reusable OpenVisionLab inspection-intent skill. It is an in-product Guided Setup/recipe-wizard contract, not a new vision algorithm.

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
| Pixel consistency gate | Supported | `DistancePxRange` is the supported pixel range gate. |
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

An exact visible gap-count gate is optional and off by default. It may be enabled only when representative Good samples prove stable `ResultCount`.

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

These are example regression values. `6 px` and the two ROIs are not universal defaults and must not be copied to another image size, magnification, or part without Train review.

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

Generic import validation remains useful for XML/schema/layer checks, but it cannot replace this intent contract. The current generic validator does not require all `PinArrayGap` ROI/detection fields and the reference contract reported `Intent contract: SKIP`.

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

## 10. Validation Gates

### Authoring Contract

Complete only when:

- the dedicated template is selectable without changing the existing LineDistance template;
- supported/unsupported inputs produce the correct readiness state;
- the generated prompt and deterministic draft lock `PinArrayGap` and the reviewed values;
- measurement-only and judged states are visually distinct;
- judged XML has a strict skill-contract pass plus generic validation/import pass;
- opening or editing Guided Setup does not run Preview/Run, create a layer, or change routing.

### N-Sample Evidence

Complete only when:

- Train, Validation, and Test file lists are non-empty and pairwise disjoint;
- detection parameters and tolerance are frozen after Train;
- Validation and Test run unchanged;
- execution/image-load/runtime errors are zero;
- the error table and exact runtime drawings are retained;
- all expected Good and declared `pitch_error` samples in the frozen pilot Test set are correct;
- no result is claimed for short-pin, bridge, bend, missing-pin, or other excluded defects.

The Guided Setup selects existing local Validation Sets and freezes skill version/XML/range/ROI/detection/split identities. Later drift is stale. Opening the explicit-run validation path does not run Preview/Run or change layers/routing. Stored drawings retain every executed row; reviewing stored rows does not rerun.

### Genuine LLM Correction Validation

Complete only when:

- a real first LLM response fails strict validation, runtime geometry review, or Validation evidence naturally;
- the exact original prompt, response, failure report, and drawings are preserved before correction;
- the correction packet uses Train/Validation evidence only;
- the corrected XML remains inside the locked `PinArrayGap` skill contract;
- a previously unused Test set is run once after candidate freeze;
- target defect recall improves without reducing Good recall or execution success below the declared gate.

## 11. Implementation Map

Find these responsibilities in the existing owners:

1. Add a separate exact-match template to `OpenVisionRecipeGuidedSetupCatalog` and `OpenVisionRecipeLlmIntent`.
2. Add a focused `OpenVisionRecipePinArrayGapIntentSkill` builder/parser in the existing `Recipe/IntentSkills` owner; reuse existing ROI parsing without creating a generic abstraction.
3. Extend `OpenVisionRecipeGuidedSetupReadinessPresenter` with the two readiness layers: measurement and judgement/validation.
4. Extend `OpenVisionRecipeLlmPromptBuilder`, `OpenVisionRecipeLlmTemplateDraftBuilder`, and `OpenVisionRecipeLlmDraftValidationRules` with the strict `PinArrayGap` contract.
5. Add only the compact intent inputs/status to the existing Build inspection surface. Reuse Local Validation Set, batch result, Run History, Pipeline Review, and drawing viewers.
6. Add focused direct/screenshot smoke checks for supported dark edge-gap, blocked bright/center-pitch, stale draft, no-gate versus judged reporting, invalid scale, exact XML, and no-auto-run/layer/routing changes.

No new algorithm, dataset database, background runner, provider automation, API key flow, or duplicate Recipe Manager dashboard is part of v1.

## 12. Applicability

This contract supports dark, roughly vertical pins and adjacent edge-to-edge pixel-gap consistency. It does not turn the measured gap into center-to-center pitch, calibrated units, or a general missing/short/bend/bridge classifier. Keep every reviewed row or use row-specific expected labels. Generic XML validation cannot replace strict ROI/polarity/parameter/unit/judgement checks.
