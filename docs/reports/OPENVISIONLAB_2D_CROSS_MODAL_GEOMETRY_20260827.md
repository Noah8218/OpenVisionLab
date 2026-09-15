# OpenVisionLab 2D cross-modal geometry handoff

Date: 2026-08-27
Status: **Implemented for the bounded Run Record contract**

`TwoDIntegrationExchange` now keeps the geometry produced by the actual 2D
recipe execution instead of reducing the result to metrics only. The persisted
Run Record is schema `1.1` and contains the decoded source image width/height
plus each step's overlay bounds, center, line endpoints, angle, point count,
and source-image points.

The 2D adapter remains explicit:

```text
Read Handoff -> Acknowledge -> RunAcceptedHandoffAsync -> publish Result
```

Reading a projection profile or writing a Run Record does not execute a
pipeline, change a layer, or start a camera. The producer/consumer transaction
still validates the exact input, recipe, consumer identity, and artifact hashes.

## Cross-repository contract

The 2D Run Record is consumed by the 3D Reporting adapter through the local
`coordinate-projection-profile` sidecar. Image coordinates use pixels and a
top-left origin. The paired 3D consumer applies the normalized mapping to the
C3D grid and records the reverse projection as Result evidence. The 2D
repository does not reference the 3D application project.

## Verification

The final separate-process smoke passed with the current Dev source:

`D:\OpenVisionLab-TestData\OpenVisionLab-CrossRepo\projection\cross-modal-projection-20260827-151939-be7fc65ba1464ecebe150d3d152ea4fe\2d-consumer\2d-cross-repo-consumer-result.json`

- source image: `572x420`;
- runtime overlays persisted: `4`;
- acknowledgement/result: `Accepted` / `Completed` / `Pass`;
- producer and consumer were separate processes;
- the paired 3D consumer reported `4` projected 2D points and `5` reverse
  projected ROI points.

The Release consumer build was executed by
`C:\Git\OpenVisionLab-Machine-Studio\tools\RunCrossModalProjectionSmoke.ps1`
and completed with exit code `0`. Generated smoke output is retained on D:
under the run root above.

The existing no-profile integration regression also passed from the current
Release consumer build: good input `Pass`, bad input `Ng`, rejected
acknowledgement blocked execution, and a tampered artifact was rejected before
acknowledgement/run. Its evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab-CrossRepo\2d\2d-integration-regression-20260827\two-d-integration-20260827-062159-47b6369c6dcf437da69d52f5d534df3e`.

## Boundary

This change persists and exposes real runtime geometry for another process. It
does not add calibration, lens/pose correction, physical metrology, camera
control, PLC/I/O, or automatic cross-application execution. Graphical display
of reverse-projected points remains owned by the consuming viewer surface; the
current Machine Studio integration card exposes the validated counts and
read-only result state.
