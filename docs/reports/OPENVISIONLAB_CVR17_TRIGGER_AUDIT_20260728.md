# OpenVisionLab CVR-17 Region Algebra Trigger Audit

Date: 2026-07-28  
Queue item: `CVR-17`  
Decision: activation audit complete; implementation not admitted

## 1. Named User Workflow

No named inspection workflow currently requires a Region Algebra implementation.
The checked project documents contain commercial references to union,
intersection, difference, and complement, but no operator-owned task with:

- named source masks, object sets, or ROIs;
- a reviewed physical meaning for each operand;
- a current recipe that fails because the operation cannot be expressed;
- labelled expected output for representative and held-out images; or
- an explanation of why the existing segmentation, ROI, Pipeline layer, and
  Arithmetic paths are unsuitable.

P217 therefore remains controlling: Region Algebra is a product possibility,
not an active feature. A commercial tool containing region operations is not,
by itself, an OpenVisionLab user requirement.

## 2. Current Capability And Causal Gap

The current product already covers several adjacent responsibilities:

| Current responsibility | Verified behavior | Boundary |
| --- | --- | --- |
| Threshold/HSV segmentation | Produces image-sized single-channel mask output; HSV records mask pixel count and ratio. | The layer is an image, not a typed Region or object-set value. |
| Arithmetic | Accepts Pipeline layer A/B or a constant; supports `Bitwise_AND`, `Bitwise_OR`, `Bitwise_XOR`, `Bitwise_NOT`, numeric operations, and fails when two image sizes differ. | It converts operands to gray images and does not assert binary-mask identity, coordinate-frame provenance, or Region-set semantics. |
| Tool ROI | Bounds segmentation or inspection inside the owning Step. | A tool-owned ROI is not a reusable Region operand or independently routed set. |
| Blob/Contour | Converts segmented pixels into tool-owned object candidates and retains reviewed object rows. | Object candidates are not published as a general set value for downstream algebra. |
| OverlayMerge | Merges and optionally rasterizes prior result drawings for review. | It composes display evidence, not semantic masks or accepted object sets. |
| Pipeline layers | Persist named whole-image outputs and make A/B routing possible. | A layer name alone does not prove binary domain, source frame, or compatible semantic identity. |

For a genuine binary-mask task, existing Arithmetic may already express the
pixel operation:

- mask union: `Bitwise_OR`;
- mask intersection: `Bitwise_AND`;
- mask complement over the full image domain: `Bitwise_NOT`;
- ordered mask subtraction: an explicitly reviewed combination such as
  `A AND NOT(B)`.

That does not prove a dedicated Region Algebra tool is unnecessary forever.
It means the next task must first prove a user-visible causal gap instead of
duplicating existing pixel arithmetic under another name.

## 3. Six-Section Admission Packet

Reopen `CVR-17` only when all six sections below are supplied and reviewed.

### A. Operator Task And Current Failure

- Name the part, inspection intent, and the exact operator decision.
- Provide the current Pipeline/XML and representative images.
- Freeze the current result and drawing that demonstrate the blocker.
- Explain why ROI, segmentation, Morphology, Arithmetic, Blob/Contour, and
  OverlayMerge cannot safely express the intended result.

### B. Operand Identity And Ownership

- Declare each input as a binary mask layer, accepted object set, raw candidate
  set, or ROI-derived region.
- Name the producing Step and retained identity/provenance.
- Define image size, coordinate frame, source image identity, and whether
  normalized/reference frames may be mixed.
- Define behavior for missing, failed, stale, ambiguous, empty, and full inputs.

### C. One Exact Mathematical Contract

- Select one required operation: union, intersection, ordered difference, or
  complement.
- Define binary/non-binary conversion, threshold convention, output pixel
  values, complement domain, operand order, and empty/full-set rules.
- Define whether output is an image mask, typed Region, object set, or more
  than one result. Do not use the word “Region” while leaving the type implicit.
- Define exact metrics and fail-closed diagnostics.

### D. User-Centered Setup And Persistence

- Present the operation, operands, frame, and output route in one coherent
  setup surface instead of requiring the user to configure scattered buttons
  and dialogs.
- Save the confirmed configuration at the recipe/Step scope and restore it
  when the recipe is reopened.
- Keep restored choices visible and editable, with an explicit reset/default
  path and stale/incompatible-state message.
- Loading or restoring the setup must not run Preview/Run, create/delete a
  layer, change the active layer, or mutate routing.

### E. Evidence Matrix And Split

- Include overlap, disjoint, contained, identical, empty, and full operands
  where applicable.
- Include size/channel/frame mismatch, non-binary input, missing layer, changed
  upstream output, border-touching regions, and operand-order cases.
- Freeze Train/Validation or equivalent development rows before implementation.
- Reserve untouched held-out rows and review output drawings, not metrics alone.

### F. Completion Gate

- PropertyGrid and Pipeline/XML round trip pass with legacy recipes unchanged.
- Save/reload/reopen restores the exact setup with zero unintended Preview/Run,
  layer, active-layer, or routing side effects.
- Runtime, validation, metrics, drawings, saved Run Report, and downstream
  routing agree on the same operation and frame.
- The frozen current failure is corrected and the held-out replay passes without
  per-image tuning.

## 4. Bounded First Implementation

If a packet passes, implement only the smallest operation required by that
named workflow.

- Reuse image Arithmetic when its current binary-image behavior fully satisfies
  the contract; improve setup/persistence or diagnostics instead of creating a
  duplicate algorithm.
- If typed Region/object-set semantics are genuinely required, introduce only
  the proven operand and output type with explicit ownership and validation.
- Keep PropertyGrid teaching, explicit Preview/Run, named output layers, saved
  drawings, and Run Report evidence.
- Preserve missing-key compatibility and fail closed on incompatible operands.
- Do not introduce a general iconic-variable language, arbitrary expression
  engine, mask-painting studio, graph rewrite, or automatic execution.

## 5. Decision And Queue Advancement

`CVR-17` is **not admitted**.

Reasons:

1. no named operator workflow or frozen causal current-path failure exists;
2. current image Arithmetic already exposes the common binary pixel operators;
3. no reviewed typed mask/object/frame contract distinguishes the proposed
   feature from existing layer arithmetic; and
4. implementing from commercial resemblance would violate P217's
   evidence-triggered feature boundary.

No runtime, PropertyGrid, Pipeline/XML, sample, DLL, or visible UI code changed.
The commercial-video continuation advances to the `CVR-18` trigger audit only
if the user explicitly continues without an earlier real packet.

## 6. Reopen Command

```text
Reopen CVR-17 for <named inspection>. Inputs are <typed operands and producer
Steps> in <coordinate frame>. Current frozen recipe <path> fails on <labelled
rows> because existing ROI/segmentation/Arithmetic/Blob/Contour/OverlayMerge
cannot express <exact operation>. Implement only <union/intersection/ordered
difference/complement> with the reviewed binary/set semantics, coherent
first-use setup, recipe/Step persistence, visible reset, zero unintended
Preview/Run or layer/routing mutation, frozen development evidence, and
untouched held-out replay.
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
- the canonical CVR-17 audited-not-admitted status; and
- queue advancement to CVR-18 rather than implementation.

Observed results:

- readiness: all 12 contract categories passed;
- audit/global-agent/queue static checks: passed;
- `git diff --check`: passed with line-ending warnings only;
- no product source, DLL, sample, or visible UI changed in this audit.

## Completion Record

```text
Status: Complete
Scope: CVR-17 activation audit, current mask/layer/Arithmetic/ROI/object/overlay responsibility boundary, six-section admission packet, user-centered persisted-setup contract, bounded first implementation, and queue advancement.
Acceptance criteria: Existing paths and their boundaries are grounded in current source/docs; a named causal gap was searched for; no task or evidence was fabricated; implementation is gated by a reusable exact packet.
Verification: Current source and documentation search; global user-centered AGENT rule check; OpenVisionReadinessCheck; audit-structure/queue static check; git diff --check.
Evidence: docs/reports/OPENVISIONLAB_CVR17_TRIGGER_AUDIT_20260728.md
Boundary / next dependency: This audit adds no Region Algebra implementation and proves no physical inspection. Reopen only with the six-section named workflow packet above.
```
