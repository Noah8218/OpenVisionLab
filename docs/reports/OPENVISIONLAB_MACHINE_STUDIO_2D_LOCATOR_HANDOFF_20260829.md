# Machine Studio 2D locator handoff — 2026-08-29

Status: Complete for the explicit cross-process locator handoff scope.

## Scope

The existing 2D integration exchange now accepts a Machine Studio recipe with
schema `locator-relative-blob-integration-recipe-v1`. The Machine Studio
producer stages the recipe-declared template as a transaction artifact. The
Dev consumer resolves that staged artifact, builds the existing locator
measurement pipeline, executes it only after explicit acknowledgement and run,
and publishes the locator packet and runtime overlay as v2 Result evidence.

The consumer never reads the producer's original template path. The publisher
does not execute the consumer, trigger a camera, or start an inspection.

## Result contract

- Recipe template artifact: role and ID `locator-template`.
- Locator packet evidence: role and ID
  `locator-relative-blob-evidence`, path
  `artifacts/locator/evidence.packet.json`.
- Runtime overlay evidence: role and ID `locator-relative-blob-overlay`, path
  `artifacts/locator/locator-runtime-overlay.png`.
- Packet schema: `locator-relative-blob-evidence-v1.1`.
- A valid locator result retains at least two candidates and has a positive
  selected-candidate score margin; otherwise the exporter fails closed.

`VisionRecipeRunResult` retains the internal native pipeline result only for
the duration of the integration run. The exporter copies the source image and
renders a persisted summary overlay, so it does not retain or access a
disposed native image buffer after the run.

## Verification

The final separate-process smoke was run from the Machine Studio repository:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File `
  C:\Git\OpenVisionLab-Machine-Studio\tools\RunLocatorCrossRepoSmoke.ps1
```

Evidence:

`D:\OpenVisionLab-TestData\OpenVisionLab-CrossRepo\locator\locator-cross-repo-20260829-111511-4eaff9f69caa4a9f8344c3d35e99ae17`

Observed values:

- transaction `c5f6ea61-8c12-4f96-ba60-dfb4df50d7af`;
- acknowledgement `Accepted`; Result `Completed/Pass`;
- source `572x420`; metric count `116`; overlay count `8`;
- packet schema `locator-relative-blob-evidence-v1.1`;
- selected candidate `locator-1`; candidate count `2`;
- score margin `17.79458522796631`; and
- evidence count `2` for packet and overlay.

The Machine Studio Result reader independently returned the same packet schema,
candidate count, selected candidate, and positive score margin through
`MachineIntegrationExchange.ReadResult`.

The focused consumer Release build and locator source-generator build passed;
the complete cross-process smoke exited `0`. The earlier exporter failure on a
disposed native image was corrected by rendering from a copied source image.
The previous one-candidate run remains preserved as fail-closed evidence.

## Boundary and next dependency

This is synthetic software-boundary evidence, not physical or external-corpus
locator qualification. 3D projection, locator-specific WPF presentation, UI
performance, PC restart, commit, push, release, and deployment remain outside
this slice. The next qualification dependency is an operator-approved native
corpus locator that retains two candidates and passes the ambiguity gate,
followed by frozen Train/Validation/Held-out review.
