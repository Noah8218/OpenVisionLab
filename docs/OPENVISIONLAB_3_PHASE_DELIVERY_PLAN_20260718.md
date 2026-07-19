# OpenVisionLab 3-Phase Delivery Plan

Updated: 2026-07-18 KST

## Decision: stop unbounded provider experiments

OpenVisionLab is a guided LLM-assisted rule-based recipe workbench, not a claim that an LLM autonomously creates correct production inspection. The local XML validation/import/explicit Good-Bad replay path is already a product capability. Provider trials now exist to reveal a concrete product gap, not to accumulate model responses indefinitely.

### Provider operating rule

- Gemini is paused until **2026-07-19 01:04 KST** (three hours after the 2026-07-18 22:04 KST user decision). Do not open, retry, or transmit another Gemini prompt or asset before that time unless the user overrides this decision.
- Use GPT as the primary provider for the next single public-sample validation. Claude remains deferred until the user explicitly resumes it.
- A provider experiment has at most one first response and at most two correction turns. A direct success is recorded as direct success; no failure is manufactured. A stopped/no-text provider response ends that provider attempt and starts its cooldown rather than causing repeated clicks.
- After Phase 1 exits, run another provider experiment only when a new validated ToolType/XML contract is added, a current Good-Bad replay exposes a product gap, or an actual field/operator failure requires it.

### Non-API browser-assist decision (2026-07-18)

- The requested product direction is a user-account web-assist route, not an OpenAI API route: the operator signs in to a provider web page with their own account, reviews the generated prompt, supplies any approved image, and returns XML to OpenVisionLab for local validation/import and explicit run.
- This can work with a free provider account only within that provider's own availability, model, rate-limit, file-upload, and login rules. OpenVisionLab must not promise free access, bypass limits, store credentials, or automate a logged-in provider session.
- The first implementation slice must preserve the existing explicit handoff: open the chosen provider page, copy the prepared prompt/review packet by explicit user click, and paste XML back by explicit user action. No automatic login, attachment upload, send, response scraping, import, Preview, or Run is part of this route.
- P126 completed the first embedded-host proof with `Microsoft.Web.WebView2`: the current Debug smoke navigated to `https://chatgpt.com/` only after an explicit `ChatGPT 열기` click, an external-browser fallback is present, and `PreviewRunCountUnchanged: 0` remained true. The host uses a transient profile; it is not an API integration or an account/session manager. Evidence: `artifacts\p126_browser_assist_20260718\README.md`.

## Current level

### LLM workflow maturity: Level 4 of 5 — bounded workflow proof

This is an evidence-based planning classification, not a release or provider-reliability score.

| Level | Meaning | Current evidence |
| --- | --- | --- |
| 1 | XML contract exists. | Authoring guide, 17-tool catalog, local schema/parameter/dependency validation. |
| 2 | The application can safely process drafts. | Validation -> import gate -> explicit Preview/Run -> result/review/Good-Bad replay are present. |
| 3 | Real provider workflow is proven. | GPT correction cases including P119’s two repairs; Gemini P120 correction; multiple GPT direct-success tool families; current Debug Good/Bad replays. |
| 4 | Representative operating workflow proof is closed. | P125 completed the bounded GPT LineDistance first-response failure, same-conversation correction, and current-Debug Good/Bad replay. Freeze the current provider-evidence set. |
| 5 | Field-qualified use. | Not yet: real operator samples, calibration where mm is claimed, and agreed acceptance evidence. |

The historical intended-workbench estimate is about 62-66%; the current handoff correctly treats that only as historical planning context. The live maturity judgement is: usable guided workbench, not autonomous authoring or production qualification.

## Phase 1 — close the LLM MVP evidence gate

**Status: Complete (P125, 2026-07-18).**

### Goal

Complete one GPT-first public LineDistance case and stop broad provider looping for the current XML contract.

### Included work

1. Use only `Line_Pins_Synthetic_OK.png` and `Line_Pins_Synthetic_WidePin_NG.png` in a GPT project chat.
2. Preserve the actual first XML, validate it on the current Debug build, and request correction only after an actual XML/intent/Good-Bad failure.
3. Import only after validation succeeds; explicitly replay nominal OK and Wide-Pin expected NG.
4. Record the exact provider/model state shown by the UI, prompt, response, local commands, and result boundaries.

### Exit criteria

- One actual GPT LineDistance response has a current-Debug validation/import outcome and nominal/Wide-Pin replay outcome. P125 met this: first XML passed nominal but incorrectly accepted Wide-Pin; its one same-conversation repair passed nominal and rejected Wide-Pin.
- If a real failure occurs, at most two actual GPT corrections have their own validation/replay evidence; if the first response succeeds, it remains a direct-success case. P125 used one correction.
- No extra GPT/Gemini/Claude experiment is scheduled for the existing contract after the case is recorded, unless one of the operating-rule triggers occurs.

### Excluded work

- Provider scorecards, automatic retry loops, camera/lighting/PLC/I/O, deployment, or a claim of autonomous recipe authoring.

**Recommended model:** GPT-5.3-Codex
**Reasoning effort:** medium

## Phase 2 — close three operator recipe skills

**Status: Complete (P127-P129, 2026-07-18).** P127 completed the public LineDistance measurement path: MM/PX starter contracts, Good/Bad replay, range guard, and explicit Recipe Manager review all passed on the current Debug build. P128 completed the public Threshold + Blob count path: validation/import, `ResultCount=12` nominal, `ResultCount=3 < 8` sparse NG, and no-auto-run Learn entry. P129 completed public template Matching: catalog runtime Good/Bad, valid LLM dependency-copy/import Good/Bad, explicit Preview `0 -> 1`, and visible `Matching_Preview` all passed. The raw catalog `docs\...` template path remains intentionally blocked by the separate Debug LLM draft validator; this is the documented deployment-portability boundary, not an unfinished Phase 2 operator-skill criterion. Evidence: `artifacts\p127_phase2_line_distance_operator_path_20260718\README.md`, `artifacts\p128_phase2_blob_operator_path_20260718\README.md`, `artifacts\p129_phase2_matching_operator_path_20260718\README.md`.

### Goal

Make three representative operator paths complete and teachable without requiring an LLM:

1. Measurement: LineDistance with average plus range/max guard.
2. Presence/count: Threshold + Blob or Contour with explicit metric gate.
3. Matching: Matching, EdgeBasedMatching, or FeatureMatching with dependency/path review and score gate.

### Included work

- For each skill, keep one public Good/Bad sample pair, one reusable starter recipe, PropertyGrid parameter route, explicit Preview/Run, result comparison, and a concise Learn/Guided Setup entry point.
- Run fresh current-build evidence only for a demonstrated gap. Fix that gap; do not recreate already-complete panels or duplicate editors.

### Exit criteria

- Each skill has a repeatable operator round trip: select sample/recipe -> set or inspect parameters -> validate/import when XML is used -> explicit Run -> inspect metric/layer/failure -> save recipe.
- Each skill has one current Good/Bad replay and no auto-run/input-routing regression.
- No new generic Recipe Manager, report, or editor surface is added without a real missing operator decision.

### Excluded work

- Broad tool-family expansion, a second editor, automatic parameter application, and equipment-platform functions.

**Recommended model:** GPT-5.3-Codex
**Reasoning effort:** medium

## Phase 3 — field-pilot evidence gate

### Goal

Prove that the workbench can support one agreed real inspection workflow locally, without changing product scope into an industrial controller platform.

### External prerequisites

**Status: Complete (P131, 2026-07-18) for one approved local two-image workbench scope.** The user approved the Bent Pin shaft-width intent, ROI `20,65,728,175`, `BoundsWidthMax <= 18 px` pixel gate, and Good/bent-NG labels. The saved `FieldPilot_BentPin` Debug workspace holds the active `BentPin_ShaftContour` pipeline, a recipe-local OK/NG validation set, local result/overlay evidence, and an operator handoff note. Fresh saved-pipeline replay accepted Good at `ResultCount=13`, `BoundsWidthMax=14` and rejected bent NG at `ResultCount=13`, `BoundsWidthMax=26 > 18`. This does not add an mm claim, public sample, provider transmission, production certification, or broader robustness conclusion.

- One user-approved real use case, its local Good/NG samples, target ROI/acceptance definition, and verified calibration only if mm units are required.
- Operator review of the final recipe and Good/Bad evidence.
- P130 prepared the existing local Bent Pin pair as a pixel-only candidate; P131 records the later explicit operator approval and saved-recipe replay. It remains local-only and is not public-sample or provider evidence.

### Exit criteria

- One saved recipe has a documented sample-validation set, explicit result evidence, known limits, and an operator handoff note.
- Any mm claim is backed by supplied calibration evidence; otherwise the recipe remains pixel-only.
- The result is called a field-pilot/workbench result, not a production deployment certification.

### Excluded work

- Camera control, lighting, PLC/I/O, runtime deployment, accounts, MES, controller simulation, or production SLA claims.

**Recommended model:** no model work until the external prerequisites are available.

## Definition of done for this plan

Status: Complete. Phase 1, Phase 2, and P131's one user-approved Phase 3 local pilot meet this plan's exit criteria. Any later task must come from a newly approved field dataset, a real regression, or a separately authorized known issue; it must not be invented solely to increase the LLM transcript count.
