# OpenVisionLab Documentation Map

Updated: 2026-07-26 KST

This map prevents a new chat from treating large historical records or old readiness estimates as the current plan.

## Required Reading Order For A New Chat

1. `AGENTS.md`
   - Operating rules, repository boundary, product scope, stable constraints, source organization, verification rules, and priority discipline.
2. `docs/OPENVISIONLAB_CURRENT_HANDOFF.md`
   - Current project state, completed evidence, known gaps, exact next-priority order, latest repository baseline, and restart checklist.
3. `docs/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`
   - Product identity, intended operator workflow, and responsibility split between Learn, Tool Views, Pipeline, Pipeline Review, and Recipe Manager.
4. `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
   - Behavioral contracts that must not regress.
5. `docs/OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md` and `docs/OPENVISIONLAB_LLM_TOOL_CATALOG.json`
   - Read when LLM XML, prompt packets, validation, import, tool parameters, or acceptance metrics are involved.
6. `docs/OPENVISIONLAB_PUBLIC_SAMPLE_ASSET_POLICY.md`, `docs/OPENVISIONLAB_EXTERNAL_REFERENCE_POLICY.md`, and `docs/OPENVISIONLAB_RELEASE_VERSION_POLICY.md`
   - Read when samples, DLLs, external references, release artifacts, or repository publication are involved.

After this reading, run `git status --short` and `git log --oneline -5` in Dev before choosing work. If the user asks for a product/maturity/commercial comparison, also read the historical assessment documents listed below, but do not treat their old percentages as current truth.

## Authority And Conflict Rules

| Rank | Document type | Authority |
| --- | --- | --- |
| 1 | `AGENTS.md` | Working restrictions, repository boundaries, scope exclusions, implementation and verification discipline. |
| 2 | Stable contracts and policies | Non-regression behavior, public asset/DLL/release boundaries. |
| 3 | Product target/main views | Product identity and responsibility ownership. |
| 4 | Current handoff | Live status, current evidence, gaps, and next priority. |
| 5 | LLM guide/catalog | XML syntax, accepted tools, parameters, metrics, and authoring rules. |
| 6 | Chronological handoff and historical reviews | Detailed evidence and prior decisions only. Confirm against current code/tests before treating them as current. |

If documents conflict, do not smooth over the conflict. Follow the higher-ranked contract, verify source/test evidence, and update the current handoff after resolving it.

## Current Documents

| Document | Use it for | Freshness note |
| --- | --- | --- |
| `OPENVISIONLAB_CURRENT_HANDOFF.md` | First continuation brief. | P236 current-state ledger plus the 2026-07-26 structural-refactoring closure; no proactive structure priority is active. |
| `OPENVISIONLAB_STRUCTURAL_REFACTORING_COMPLETION_20260726.md` | Canonical closure for the 2026-07-24..26 documentation, folder, MVVM, and responsibility-ownership refactoring campaign. | Current structural status is `Complete`; reopen only for a concrete maintenance change, verified regression, independently testable responsibility, or second real consumer. |
| `OPENVISIONLAB_RULE_BASED_UI_GAP_AUDIT_20260723.md` | Static algorithm/UI inventory, official commercial comparison, selected UI/tool gaps, and repeated-validation stop rule. | Current priority rationale; no image or LLM validation evidence is claimed. |
| `OPENVISIONLAB_AFFINE_TRANSFORM_V1_CONTRACT.md` | Library-Noah ownership, three-point pixel Affine parameters, fail-closed gates, metrics/drawings, PropertyGrid/XML/Learn behavior, and completion limits. | P218 completed on 2026-07-23 with known-matrix synthetic runtime, DLL provenance, UI, and regression evidence. |
| `OPENVISIONLAB_AFFINE_DETECTED_POINT_FIXTURE_CONTRACT.md` | Ordered earlier typed-Point x3 binding into Affine, Matching Center export, fail-closed rules, operator workflow, fixed downstream ROI, and evidence limits. | P219 completed on 2026-07-23 with actual Matching x3 -> Affine -> fixed-ROI synthetic runtime and current-source PropertyGrid evidence. |
| `OPENVISIONLAB_CARD_AFFINE_PILOT_20260723.md` | Operator-approved real card three-point pilot, frozen inputs/gates, r1/r2 failures, drawings, and the exact tolerance dependency. | P220 is incomplete at the frozen `<=3 px` gate: 10/12 passed and two rows retained 4.12/5.00 px residual. |
| `OPENVISIONLAB_CARD_AFFINE_FIXED_ROI_20260723.md` | User-accepted coarse card registration boundary and one actual `CardReference` fixed-ROI Mean linkage with runtime drawings. | P221 completes 12-row fixed-coordinate linkage at the separate observed `<=5 px` boundary; it does not redefine P220 or add Good/NG judgement. |
| `OPENVISIONLAB_AUTO_MPOINT_V1_CONTRACT.md` | Operator-assisted fixed-size matching-candidate teaching, Library-Noah ownership, P223 explicit-apply UI evidence, GPT Pro/current-source/commercial review, and ordered matcher direction. | P222 completes the library core, P223 completes the OpenVisionLab teaching integration, and P224 completes only the optional bounded runtime-uniqueness slice; production qualification remains open. |
| `OPENVISIONLAB_TOOL_VIEW_N_IMAGE_VERIFICATION_DESIGN.md` | Current N-image capability audit, Tool View quick-verification versus Recipe Manager responsibility, eligible/deferred tools, hash-locked locator promotion, report contract, and sequential/parallel phase gates. | P233 completes shared sequential Tool View execution/reporting and P235 completes bounded locator expected-success promotion; concurrent workers remain unimplemented. |
| `OPENVISIONLAB_EDGE_BASED_UNIQUE_MATCH_V1_CONTRACT.md` | P224 opt-in Library-Noah unique-result parameters, internal/external candidate contract, `NoMatch`/`Success`/`Ambiguous` states, metrics, OpenVisionLab XML/UI behavior, and evidence boundary. | Current runtime/XML authority for fail-closed EdgeBasedMatching uniqueness; not physical-anchor, margin, pose, or field qualification. |
| `OPENVISIONLAB_GENERAL_GEOMETRIC_MEASUREMENT_WORKSPACE_CONTRACT.md` | Completed point/segment/circle result model, CircleGauge, GeometryMeasure relations, UI, drawings, and completion gates. | P213 completed on 2026-07-23 with current-source UI, runtime, persistence, fail-closed, and legacy-regression evidence. |
| `OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md` | Product direction and view responsibilities. | Stable target; not a substitute for live git/test state. |
| `OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md` | Explicit behavioral non-regression rules. | Change only with deliberate product decisions and verification. |
| `OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md` | Preserve the optional LLM/XML compatibility contract and validate XML tasks. | Maintenance-mode reference; update only when a supported runtime XML/tool contract changes or a concrete compatibility defect is fixed. |
| `OPENVISIONLAB_LLM_TOOL_CATALOG.json` | Machine-readable tool/parameter/metric contract. | Keep synchronized with runtime validation. |
| `OPENVISIONLAB_PIN_ROW_GAP_INTENT_SKILL.md` | First in-product inspection-intent skill contract: supported scope, required inputs, XML, states, N-sample evidence, and phase gates. | Approved v1 design; implementation status remains in the current handoff. |
| `OPENVISIONLAB_DARK_BAND_GAP_INTENT_SKILL.md` | Direct dark-band thickness/Gap intent: one coarse ROI, locked LineDistance starter, px-only boundary, required metrics/drawings, failure table, and phase gates. | P190 closes the all-500 audit as `Keep with documented limits`; tolerance/calibration status remains in the current handoff. |
| `OPENVISIONLAB_HYBRID_LOCATOR_RELATIVE_ROI_INTENT_SKILL.md` | Approved Matching pose -> NormalizeImage -> relative-ROI inspection contract, required operator inputs, fail-closed gates, drawings, LLM limits, and phase gates. | P193 closes the first all-500 candidate as `Hybrid candidate`; current locator coverage and product implementation remain in the current handoff. |
| `OPENVISIONLAB_SCALABLE_SKILL_VALIDATION_PROTOCOL.md` | Large-corpus execution, semantic gold set, deterministic review queue, bounded correction, held-out replay, and skill Go/No-Go rules. | Current cross-skill validation operating contract; prevents image-by-image tuning and execution-count overclaims. |
| `OPENVISIONLAB_MATCHING_FIXTURE_WORKFLOW_SPEC.md` | Implemented Matching fixture workflow and translation-only ROI-consumer limits. | Runtime authority for `TranslationRoi` v1; use the v2 spec for full-image normalization. |
| `OPENVISIONLAB_MATCHING_SIMILARITY_FIXTURE_V2_SPEC.md` | Matching-driven image normalization for bounded pose-varying measurements. | P183 adds the bounded C9 fail-closed operating gate, and P184 replays that exact guarded pixel path on all 500 supplied top-left images; independent Gap truth, calibration, unseen/all-direction robustness, and field qualification remain incomplete. |
| `OPENVISIONLAB_3_PHASE_DELIVERY_PLAN_20260718.md` | Bounded Phase 1/2/3 exit criteria and non-API browser-assist direction. | Current delivery plan; read with the current handoff before selecting new scope. |
| `OPENVISIONLAB_SOURCE_OWNERSHIP_REFACTOR_PROOF_20260717.md` | Folder ownership, P95-P104 extraction evidence, and stop condition. | Consult before more structural refactoring. |

## Historical Or Detailed Documents

| Document | Correct use | Do not use it for |
| --- | --- | --- |
| `OPENVISIONLAB_NEXT_SESSION_HANDOFF.md` | Search for a P-number, artifact, old diagnostic, or chronological decision. | The first source for current priority; it is a large cumulative log. |
| `OPENVISIONLAB_NEXT_CHAT_HANDOFF_PROMPT_20260706.md` | Clean paste-ready restart prompt and minimum current constraints. | A bootstrap only; use the current handoff for detailed status and the chronological handoff for P-number evidence. |
| `OPENVISIONLAB_PRODUCT_IDENTITY_AND_ROADMAP.md` | Historical identity/roadmap context. | Current readiness percentage or next task. |
| `OPENVISIONLAB_STATUS_AND_NEXT_STEPS.md` | Historical completed work, regressions, and prior measurements. | Current maturity percentage or release claim. |
| `OPENVISIONLAB_UX_COMPETITOR_REVIEW_20260701.md` and `OPENVISIONLAB_COMPETITOR_PRIORITY_REVIEW_20260701.md` | Commercial-reference rationale. | Permission to expand into hardware/platform scope. |
| `OPENVISIONLAB_SELF_EVALUATION_20260703.md` | Historical self-assessment evidence. | A current release decision without fresh verification. |

## Documentation Maintenance Protocol

After every bounded work slice:

1. Update `OPENVISIONLAB_CURRENT_HANDOFF.md` with the changed responsibility, completed/blocked state, command evidence, UI artifact path if applicable, and re-ranked next priority.
2. Append detailed chronology, raw command context, and artifact-specific notes to `OPENVISIONLAB_NEXT_SESSION_HANDOFF.md` only when that detail will help a future investigation.
3. Read `OPENVISIONLAB_STRUCTURAL_REFACTORING_COMPLETION_20260726.md` before
   reopening structural work. Update the specific proof/current handoff for a
   new bounded boundary; update
   `OPENVISIONLAB_SOURCE_OWNERSHIP_REFACTOR_PROOF_20260717.md` only for
   physical moves, new ownership boundaries, or source-check changes.
4. Update the LLM guide/catalog only when the validated XML contract changes. Do not change it merely because one model response was weak.
5. Update policy documents only for actual policy decisions. Do not mix policy edits with unrelated UI work.

For UI changes, the current handoff entry must name a freshly generated before/after artifact directory and state whether it is EXE evidence or a current-source view capture. For non-UI code, name the smallest relevant build/smoke/check command and outcome.

## Minimal Restart Prompt

```text
Work in C:\Git\OpenVisionLab_Dev. Read AGENTS.md, docs/OPENVISIONLAB_CURRENT_HANDOFF.md, docs/OPENVISIONLAB_DOCUMENTATION_MAP.md, docs/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md, and docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md. Run git status -sb and git log --oneline -5 in both repositories when publication/synchronization is in scope. State the product identity, current evidence-based maturity, immediate priority, remaining priority, commercial lessons to emulate, and out-of-scope platform areas before changing anything. Treat P236's current-state ledger as the compact truth and the chronological handoff only as detailed evidence. Preserve PropertyGrid tools and explicit Preview/Run/no-auto-route contracts. Do not invent an implementation priority when the current handoff says none.
For structural work, also read docs/admin/OPENVISIONLAB_STRUCTURAL_REFACTORING_COMPLETION_20260726.md and reopen only when its evidence prerequisite is satisfied.
```
