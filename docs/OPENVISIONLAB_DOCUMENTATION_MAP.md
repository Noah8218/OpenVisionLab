# OpenVisionLab Documentation Map

Updated: 2026-07-17 KST

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
| `OPENVISIONLAB_CURRENT_HANDOFF.md` | First continuation brief. | Current status authority, updated at the end of a bounded work slice. |
| `OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md` | Product direction and view responsibilities. | Stable target; not a substitute for live git/test state. |
| `OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md` | Explicit behavioral non-regression rules. | Change only with deliberate product decisions and verification. |
| `OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md` | Give to external LLMs and use to validate XML tasks. | Update when supported XML/tool contracts change. |
| `OPENVISIONLAB_LLM_TOOL_CATALOG.json` | Machine-readable tool/parameter/metric contract. | Keep synchronized with runtime validation. |
| `OPENVISIONLAB_3_PHASE_DELIVERY_PLAN_20260718.md` | Bounded Phase 1/2/3 exit criteria and non-API browser-assist direction. | Current delivery plan; read with the current handoff before selecting new scope. |
| `OPENVISIONLAB_SOURCE_OWNERSHIP_REFACTOR_PROOF_20260717.md` | Folder ownership, P95-P104 extraction evidence, and stop condition. | Consult before more structural refactoring. |

## Historical Or Detailed Documents

| Document | Correct use | Do not use it for |
| --- | --- | --- |
| `OPENVISIONLAB_NEXT_SESSION_HANDOFF.md` | Search for a P-number, artifact, old diagnostic, or chronological decision. | The first source for current priority; it is a large cumulative log. |
| `OPENVISIONLAB_NEXT_CHAT_HANDOFF_PROMPT_20260706.md` | Reuse detailed prompt wording or locate old handoff context. | Current dirty state, commits, or active priorities without the current handoff. |
| `OPENVISIONLAB_PRODUCT_IDENTITY_AND_ROADMAP.md` | Historical identity/roadmap context. | Current readiness percentage or next task. |
| `OPENVISIONLAB_STATUS_AND_NEXT_STEPS.md` | Historical completed work, regressions, and prior measurements. | Current maturity percentage or release claim. |
| `OPENVISIONLAB_UX_COMPETITOR_REVIEW_20260701.md` and `OPENVISIONLAB_COMPETITOR_PRIORITY_REVIEW_20260701.md` | Commercial-reference rationale. | Permission to expand into hardware/platform scope. |
| `OPENVISIONLAB_SELF_EVALUATION_20260703.md` | Historical self-assessment evidence. | A current release decision without fresh verification. |

## Documentation Maintenance Protocol

After every bounded work slice:

1. Update `OPENVISIONLAB_CURRENT_HANDOFF.md` with the changed responsibility, completed/blocked state, command evidence, UI artifact path if applicable, and re-ranked next priority.
2. Append detailed chronology, raw command context, and artifact-specific notes to `OPENVISIONLAB_NEXT_SESSION_HANDOFF.md` only when that detail will help a future investigation.
3. Update `OPENVISIONLAB_SOURCE_OWNERSHIP_REFACTOR_PROOF_20260717.md` for physical moves, new ownership boundaries, or source-check changes.
4. Update the LLM guide/catalog only when the validated XML contract changes. Do not change it merely because one model response was weak.
5. Update policy documents only for actual policy decisions. Do not mix policy edits with unrelated UI work.

For UI changes, the current handoff entry must name a freshly generated before/after artifact directory and state whether it is EXE evidence or a current-source view capture. For non-UI code, name the smallest relevant build/smoke/check command and outcome.

## Minimal Restart Prompt

```text
Work in C:\Git\OpenVisionLab_Dev. Read AGENTS.md, docs/OPENVISIONLAB_CURRENT_HANDOFF.md, docs/OPENVISIONLAB_DOCUMENTATION_MAP.md, docs/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md, and docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md. Run git status --short and git log --oneline -5. State the product identity, current evidence-based maturity, immediate priority, remaining priority, commercial lessons to emulate, and out-of-scope platform areas before changing anything. Use the current handoff as the status source of truth; use the chronological handoff only for detailed evidence. Preserve PropertyGrid tools and explicit Preview/Run/no-auto-route contracts. Work in Dev only unless I explicitly ask for original-repository work, commit, or push.
```
