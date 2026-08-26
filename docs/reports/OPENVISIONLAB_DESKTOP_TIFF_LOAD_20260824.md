# OpenVisionLab Desktop TIFF Load Reproduction - 2026-08-24

Updated: 2026-08-24 KST  
Status: Incomplete

## Scope

Retry the user-provided desktop TIFF through the current Dev OpenVisionLab
EXE, preserve the existing Main-layer and Pipeline contracts, and prevent a
decoder failure from terminating the application. No renderer, GPU framework,
tiling framework, algorithm, concurrency, camera/PLC/I/O, or broad UI redesign
was added.

## Input and baseline evidence

- Input: `C:\Users\USER\Desktop\STRIP-Split_None-Pattern-1.tif`
- SHA-256:
  `3428D8BFEFC3B680A9B06F1C9A0FDD443EDB0FE51BBDAC6CC29A4C9276F2538D`
- File length: `61,306,562` bytes.
- Native TIFF metadata: `31,800 x 96,800`, grayscale, approximately
  `3,078,240,000` pixels. A full 8-bit grayscale buffer alone is about
  `2.87 GiB` before viewer/display copies.
- Baseline current-source EXE selection and file-dialog submission were
  performed on the dynamically selected `\\.\DISPLAY2` monitor with bounds
  `-1920,365,1920,1080`. The path was entered into the OpenVisionLab file
  dialog; no unrelated window received the path.
- Baseline result: the EXE terminated immediately while constructing
  `System.Drawing.Bitmap`.
- Baseline Windows evidence: `.NET Runtime` event 1026 reported
  `System.OverflowException` at
  `OpenVisionShellHostWorkspaceImageController.LoadImage`, followed by
  `Application Error` event 1000 and WER event 1001.

## Smallest shared cleanup

- `OpenVisionShellHostWorkspaceImageController.LoadImage` now catches the
  observed bitmap-decoder exception, records the path and exception through
  `OVLog`, and returns `false` before any layer mutation.
- `OpenVisionShellHostCommandController.PromptAndLoadWorkspaceImage` now
  shows a localized warning when the shared loader returns `false`; successful
  loads keep the existing path-memory and manual-load callbacks.
- The already-dirty `ImageSpaceFrame.Borrow` ownership change in the workspace
  image controller was preserved and is not attributed to this cleanup.

## Verification

- Latest Dev build: `dotnet build "OpenVisionLab.sln" -c Debug
  -p:Platform="Any CPU" --nologo` -> 0 warnings, 0 errors.
- Current EXE application assembly SHA-256:
  `69AD8F0E901B7D2888730BB4987BFDF3B1CC304AF086EF1233CC0727B4F31F72`.
- Retried the same TIFF through the latest EXE. The warning was displayed,
  could be dismissed, and the EXE remained responsive. After dismissal the
  rendered/accessibility state remained `No image`, input layer `Main`, route
  `Load image or open sample`, and `Ready`; Preview/Run was not executed and
  no layer or routing mutation was observed.
- No Application Error, .NET Runtime, or WER event matching the EXE occurred
  after the fixed-run start time.
- Focused current-source WPF smoke:
  `wpf_shell_host_workspace_image_load=OK` with a fresh PNG under the evidence
  folder below.
- Repository gates passed: readiness check, external-reference check, public
  sample asset check, and `git diff --check` (line-ending warnings only).

## Evidence

`D:\OpenVisionLab-TestData\OpenVisionLab\opengl-gpu-coordinate-20260824-1547\desktop-tiff-load`

The folder retains baseline/fixed attempt metadata, application-event output,
the post-fix zero-event query, the current-source focused smoke PNG, and the
monitor/source/build identity used for the retry.

## Reconfirmation policy

Do not repeat this same TIFF retry merely to reconfirm it when the input
SHA-256, effective Dev source, application-assembly SHA-256, loader behavior,
focused smoke, monitor bounds, and retained evidence are unchanged. Reopen it
only if one of those identities changes, the user supplies a different image,
the TIFF decoder/loader changes, or an actual regression is reproduced.

## Boundary / next dependency

This closes the unhandled-exception path but does not prove that the original
full-resolution TIFF can be loaded or inspected. Full inspection requires an
explicitly approved large-image strategy (for example, a bounded crop or a
tile/region loader) and separate memory/coordinate acceptance criteria. No
algorithm inspection was run because no image entered the Main layer.
