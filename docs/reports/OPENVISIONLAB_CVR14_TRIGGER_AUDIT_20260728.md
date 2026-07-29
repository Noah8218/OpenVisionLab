# OpenVisionLab CVR-14 Activation Audit

Date: 2026-07-28  
Queue item: `CVR-14` — multi-result overlap and suppression semantics  
Decision: **Not admitted — labelled multi-instance packet absent**

## 1. Question Audited

Should OpenVisionLab expose an operator-configurable overlap/suppression rule
for multi-result Matching or EdgeBasedMatching now?

The commercial-video review shows explicit overlap control as a useful
multi-result matcher capability. The repository must first prove which nearby
responses are separate physical instances and which are duplicate responses
to one instance. A numeric overlap slider without that semantic truth can
silently remove a valid neighbor or count one part twice.

## 2. Existing Responsibilities

| Existing path | Current behavior | Why it is not CVR-14 completion |
| --- | --- | --- |
| Matching multi-result search | Masks the accepted template-sized source region and rejects a later result whose center is closer than half the smaller result dimension, with a 4 px floor | Fixed internal behavior; no operator-owned overlap definition or retained suppression evidence |
| EdgeBasedMatching multi-result search | Suppresses candidate centers inside the accepted result bounds expanded by 35% and applies a duplicate-center threshold of 35% of the smaller result dimension, with a 4 px floor | Fixed internal behavior; no task-specific IoU/containment/center-distance mode |
| EdgeBased scale multi-match fast path | Reuses a seed pool while retaining the same non-overlapping-result fallback contract | Performance path, not a new overlap policy |
| Auto MPoint | Uses `MaximumCandidateOverlap` IoU while choosing non-overlapping training-time template suggestions | Template-teaching shortlist only; it does not control runtime match results |
| CVR-10 `MultiMatchMean` | Rejects the complete fan-out when accepted source result IoU exceeds `MaximumOverlapRatio` | Post-match evidence gate; it does not change which candidates the matcher retains |
| Unique-match mode | Requires `NUM_MATCH=1` and compares the selected result with a spatially distinct alternative | Ambiguity decision, not multi-instance suppression |

These current paths prove that internal duplicate handling exists. They do not
provide one consistent operator-facing overlap contract, and they do not prove
that changing the current runtime behavior is necessary.

## 3. Evidence Inventory Result

The repository contains:

- public and synthetic Matching examples with `NUM_MATCH > 1`;
- scale multi-match regression evidence for separated targets;
- CVR-10 four-instance synthetic evidence and one fabricated excessive-IoU
  reject;
- unique-match and repeated-pattern evidence for single-result ambiguity;
- commercial narration that identifies overlap control as a parameter.

It does not contain a named inspection packet with:

- labelled identities for every physical target instance;
- intentionally close valid neighbors that the current matcher suppresses;
- duplicate responses to one physical target that the current matcher retains;
- an operator-defined decision rule for partial overlap, containment, touching,
  or rotated targets;
- a frozen current-matcher comparison and untouched Held-out split.

The CVR-10 fabricated overlap row is not an admission packet. It proves that a
consumer can fail closed on overlapping accepted source evidence; it does not
prove which matcher candidate should have been suppressed.

## 4. CVR-14 Admission Packet

Reopen CVR-14 only when one packet contains all six sections below.

### A. Named Multi-Instance Task

- part/product and inspection name;
- exact physical feature represented by one match;
- expected instance-count range;
- downstream consumer of every retained instance;
- explicit reason `NUM_MATCH=1` or separate fixed ROIs cannot express the task.

### B. Physical Instance Ground Truth

For every image retain:

- source and template SHA-256;
- unique physical instance IDs;
- reviewed center, angle, scale, and target footprint for each instance;
- expected OK/NG and expected retained-result count;
- one-to-one assignment between accepted matches and physical instances.

The footprint must be operator-certified. A matcher's own axis-aligned
`Bounding` output cannot serve as its ground truth.

### C. Exact Overlap Semantics

Choose one physical rule and define it before implementation:

- center distance in pixels or template-relative units;
- axis-aligned IoU;
- rotated-polygon IoU;
- containment ratio;
- or another bounded geometry explicitly required by the task.

Define touching, partial occlusion, nested results, equal-score order, mixed
angle/scale results, and whether two physically overlapping parts are both
valid. Do not combine several rules into a generic policy editor for v1.

### D. Frozen Current-Path Comparison

Replay the exact current settings and retain:

- every accepted result and its score/pose/bounds;
- current Matching or EdgeBased suppression behavior;
- CVR-10 post-match overlap state when it is the downstream consumer;
- false suppression of a valid neighbor;
- duplicate retention for one physical instance;
- current-run drawings and runtime.

At least one reproducible error must be causally attributable to the fixed
suppression rule rather than score, ROI, angle/scale range, polarity, template
identity, count limit, or labelling error.

### E. Nuisance And Ordering Matrix

Freeze and cover:

- isolated targets;
- two close but valid neighbors on both sides of the proposed limit;
- one target producing multiple angle/scale/position responses;
- partial crop/occlusion where the task defines expected behavior;
- repeated wrong targets and no-target images;
- deterministic equal-score ordering and maximum candidate/result budget.

The packet must define a runtime budget and prove that result ordering is
stable enough for existing `I01..Ixx` consumers.

### F. Split And Completion Gate

- separate Train, Validation, and untouched Held-out rows before implementation;
- retain all source/template/configuration hashes;
- require exact one-to-one instance assignment, not only aggregate count;
- require zero duplicate physical-instance assignments and zero suppressed
  valid neighbors at the frozen task gate;
- retain accepted and suppressed candidate drawings/reasons;
- require missing XML keys to preserve current behavior;
- require PropertyGrid/XML/Pipeline/Run Report round trip and no automatic
  Preview/Run, layer, or route mutation.

## 5. First Admitted Implementation Boundary

If the packet passes, the first implementation is limited to:

- one opt-in suppression rule selected by the packet;
- one finite validated threshold and deterministic score/tie ordering;
- missing keys preserving the current fixed suppression behavior;
- exact kept/suppressed state, reason, source candidate identity, and overlap
  measurement in metrics/drawings/reports;
- one frozen multi-instance task.

It does not authorize a general tracking system, arbitrary NMS policy graph,
cross-image identity, learned suppression, automatic threshold selection,
generic nested sub-recipes, or commercial-parity claims.

## 6. Decision

CVR-14 is not admitted. No matcher, DLL, PropertyGrid, XML, sample, or UI
change is justified by the current evidence. Generating conveniently spaced
or overlapping synthetic targets solely to make a new threshold pass would
not establish the physical-instance semantics.

Reopen command:

```text
Audit this named multi-instance overlap packet against
docs/reports/OPENVISIONLAB_CVR14_TRIGGER_AUDIT_20260728.md.
Do not implement CVR-14 unless all six sections pass.
```

```text
Status: Complete
Scope: Read-only CVR-14 activation audit and reusable multi-result overlap admission contract.
Acceptance criteria: Matching, EdgeBasedMatching, Auto MPoint, unique-match, and CVR-10 responsibilities are separated; the missing physical-instance truth and six-section admission packet are recorded.
Verification: Current OpenVisionLab and Library-Noah source searches, stable matcher contracts, CVR-10 evidence, commercial-video review, backlog, and current handoff were cross-checked; documentation readiness and diff checks are recorded with this turn.
Evidence: docs/reports/OPENVISIONLAB_CVR14_TRIGGER_AUDIT_20260728.md
Boundary / next dependency: Implementation requires one complete labelled multi-instance packet proving a reproducible current false suppression or duplicate retention and defining one operator-owned overlap rule.
```
