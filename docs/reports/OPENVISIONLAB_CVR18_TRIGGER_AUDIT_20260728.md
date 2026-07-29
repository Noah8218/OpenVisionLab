# OpenVisionLab CVR-18 Derived-Metric Expression Trigger Audit

Date: 2026-07-28  
Queue item: `CVR-18`  
Decision: activation audit complete; implementation not admitted

## 1. Named User Judgment

No named inspection currently requires a general derived-metric expression
Step.

The checked project documents describe the commercial possibility of combining
metrics, but do not provide an operator-owned recipe with:

- named source Steps and metrics;
- one exact scalar formula and physical meaning;
- a frozen current result that cannot express the required judgment;
- reviewed units, missing-value behavior, and tolerance; or
- representative and held-out rows proving the formula separates the intended
  OK/NG classes.

CVR-18 therefore remains a conditional product possibility. A scripting or
variable feature in a commercial product is not itself an OpenVisionLab task.

## 2. Current Capability And Causal Gap

The current product already covers the adjacent responsibilities below.

| Current responsibility | Verified behavior | Boundary |
| --- | --- | --- |
| Step acceptance | Can judge expected success, required message, elapsed time, and one named metric with minimum/maximum bounds. | One Step does not evaluate a formula over arbitrary earlier metrics. |
| Pipeline result | A Step passes only when tool execution and acceptance pass; a recipe can use acceptance on several Steps. | Multiple Step gates express conjunction, not a new scalar ratio, difference, or weighted combination. |
| Known metric catalog | Supplies stable metric names, display text, recommended Tool mappings, and presets. | A known name does not declare cross-metric units or expression compatibility. |
| Domain Tool metrics | LineDistance, PinArrayGap, GeometryMeasure, CircleGauge, matching, Blob/Contour, and other Tools publish domain-owned averages, ranges, distances, angles, scores, and counts. | A missing stable domain metric should normally be added to its owning Tool rather than hidden inside a general formula. |
| Recipe persistence and reports | Acceptance fields round-trip and Run Reports retain metrics and acceptance messages. | There is no persisted cross-Step metric-reference/expression contract. |

The current causal boundary is therefore:

- simple independent requirements can use acceptance on their owning Steps;
- stable domain mathematics belongs to the domain Tool that owns its meaning;
- CVR-18 is relevant only when a real operator judgment genuinely requires a
  scalar combination of earlier metrics that neither path can represent.

No such current task was found.

## 3. Six-Section Admission Packet

Reopen `CVR-18` only when all six sections below are supplied and reviewed.

### A. Operator Task And Frozen Current Failure

- Name the part, inspection intent, and final operator decision.
- Provide the current Pipeline/XML and labelled images.
- Freeze the current metric table, drawings, and incorrect decision.
- Explain why separate existing Step gates and a domain-owned metric cannot
  express the requirement without changing its meaning.

### B. Metric Inputs And Provenance

- Name each earlier `SourceStep` and exact metric key.
- Define run/frame/instance identity and whether disabled, skipped, failed, or
  acceptance-NG source Steps may be referenced.
- Declare physical unit or dimension for every input and the output.
- Define behavior when a metric is missing, duplicated, stale, renamed, or
  produced more than once.

### C. One Exact Mathematical Contract

- Freeze one required formula and its physical interpretation.
- Define the smallest allowed operator/function set, constants, parentheses,
  numeric precision, precedence, and rounding/display behavior.
- Define unit compatibility and the result unit.
- Fail closed on divide-by-zero, overflow, NaN, infinity, invalid syntax,
  unknown metric, incompatible units, or non-deterministic evaluation.
- Do not allow files, processes, reflection, network access, arbitrary method
  invocation, dynamic language runtimes, or user-supplied executable code.

### D. User-Centered Setup And Persistence

- Put formula, source metrics, units, output name, and acceptance range in one
  coherent setup surface instead of distributing them across several dialogs.
- Prefer metric pickers and a readable formula preview over manual opaque keys.
- Save the confirmed setup in the recipe/Step and restore it on reopen.
- Keep restored settings visible and editable, with explicit reset/default and
  stale/incompatible-reference guidance.
- Editing, loading, or restoring the expression must not execute Preview/Run,
  create/delete a layer, change the active layer, or mutate routing.

### E. Evidence Matrix And Split

- Include nominal, boundary, and clearly NG scalar cases.
- Include missing/failed/NG/skipped source, duplicate identity, unit mismatch,
  zero divisor, very large/small values, non-finite input, and renamed metric.
- Freeze development rows before implementation and retain untouched held-out
  rows.
- Compare computed values, units, acceptance messages, saved reports, and
  deterministic replay—not final OK/NG alone.

### F. Completion Gate

- Definition validation, PropertyGrid, Pipeline/XML round trip, execution,
  diagnostics, reports, Validation Sets, and saved history agree.
- Save/reload/reopen restores the exact expression and references with zero
  unintended Preview/Run, layer, active-layer, or routing side effects.
- The frozen current failure is corrected and held-out replay passes without
  per-image formula or tolerance changes.
- Legacy recipes remain unchanged, and the implementation does not become a
  general scripting surface.

## 4. Bounded First Implementation

If a packet passes, choose the smallest correct owner:

- add a new stable metric to the existing domain Tool when the formula is part
  of that Tool's physical contract;
- use separate existing Step gates when the requirement is only conjunction;
  or
- add one bounded derived-metric Step only when cross-Step scalar composition is
  the proven requirement.

A bounded derived-metric Step must:

- reference exact earlier Step/metric identities;
- expose only the operator/function subset required by the named task;
- publish one named finite scalar with explicit provenance and unit;
- fail closed on invalid sources or mathematics;
- retain the formula, resolved inputs, output, and acceptance evidence in Run
  Reports and Validation Sets; and
- preserve explicit Preview/Run and all layer/routing contracts.

Do not add arbitrary code, a general HDevelop-style variable language, loops,
conditionals, file/network/process access, reflection, plugins, or hidden
automatic gate mutation.

## 5. Decision And Queue Advancement

`CVR-18` is **not admitted**.

Reasons:

1. no named operator judgment or frozen causal current failure exists;
2. multiple independent requirements can already be expressed on their owning
   Steps;
3. stable inspection mathematics is already published by domain Tools where it
   has a defined meaning; and
4. no reviewed formula, metric provenance, unit contract, or replay split
   distinguishes a bounded feature from a general scripting engine.

No runtime, PropertyGrid, Pipeline/XML, sample, DLL, or visible UI code changed.
The commercial-video continuation advances to the `CVR-19` trigger audit only
if the user explicitly continues without an earlier real packet.

## 6. Reopen Command

```text
Reopen CVR-18 for <named inspection judgment>. Current recipe <path> produces
<SourceStep.metric unit> inputs but cannot express <exact formula and physical
meaning> with existing Step gates or a domain-owned metric. Use the frozen
labelled rows and untouched held-out split. Implement only the reviewed safe
operator subset with exact provenance, unit checking, fail-closed mathematics,
one coherent persisted setup, visible reset, and zero unintended Preview/Run
or layer/routing mutation.
```

## 7. Verification

Commands run:

```powershell
dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\OpenVisionLab_Dev
git diff --check
```

Additional static checks verified:

- the six admission sections, bounded boundary, decision, and reopen command;
- the global user-centered/persisted-setup instruction tokens;
- the canonical CVR-18 audited-not-admitted status; and
- queue advancement to CVR-19 rather than implementation.

Observed results:

- readiness: all 12 contract categories passed;
- audit/global-agent/queue static checks: passed;
- `git diff --check`: passed with line-ending warnings only;
- no product source, DLL, sample, or visible UI changed in this audit.

## Completion Record

```text
Status: Complete
Scope: CVR-18 activation audit, current acceptance/domain-metric responsibility boundary, six-section admission packet, user-centered persisted setup, bounded first implementation, and queue advancement.
Acceptance criteria: Existing Step gates, recipe pass semantics, known metrics, domain-owned derived values, persistence/reporting, and their boundaries are grounded in current source/docs; no task or evidence was fabricated.
Verification: Current source and documentation search; OpenVisionReadinessCheck; audit-structure/queue static check; git diff --check.
Evidence: docs/reports/OPENVISIONLAB_CVR18_TRIGGER_AUDIT_20260728.md
Boundary / next dependency: This audit adds no expression Step and proves no inspection. Reopen only with the complete six-section named judgment packet above.
```
