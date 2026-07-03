# OpenVisionLab External Reference Policy

Updated: 2026-07-03

OpenVisionLab must build from this repository without cloning private or local source trees. External runtime dependencies are checked in as prepared DLLs under `dll/`.

## Required DLL Layout

```text
dll\
  System.Windows.Controls.WpfPropertyGrid.dll
  System.Windows.Controls.WpfPropertyGrid.xml
  Library-Noah\
    Lib.Common.dll
    Lib.OpenCV.dll
    Lib.OpenCV.Blob.dll
    OpenCvSharp.dll
    OpenCvSharp.Blob.dll
    OpenCvSharp.Extensions.dll
  OpenCVSharp\
    OpenCvSharpExtern.dll
    opencv_ffmpeg400_64.dll
```

`Directory.Build.props` owns these MSBuild paths:

- `OpenVisionLabDllRoot`: `dll\`
- `OpenVisionLabLibraryNoahDllRoot`: `dll\Library-Noah\`
- `OpenVisionLabOpenCvSharpDllRoot`: `dll\OpenCVSharp\`

`OpenCvSharpExtern.dll` is intentionally shared from `dll\OpenCVSharp\`. Do not duplicate it under `dll\Library-Noah\`; it is a large native runtime and duplicating it adds unnecessary Git history weight.

## Build And Check

```powershell
dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"
powershell -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
```

`tools\TestExternalReferences.ps1` must check both managed Library-Noah DLLs and the shared OpenCVSharp native runtime.

## Library-Noah Policy

OpenVisionLab references prepared Library-Noah DLLs only. It must not reference a local Library-Noah source project from the solution.

When Library-Noah changes are required:

1. Build and validate Library-Noah in its own source repository.
2. Copy only the required managed DLLs into `dll\Library-Noah\`.
3. Keep `OpenCvSharpExtern.dll` in `dll\OpenCVSharp\`.
4. Run build, readiness, and the external reference check.
5. Record the DLL change and validation result in the commit or PR.

## Large Binary Policy

Do not add new duplicate DLLs larger than 50 MB to this repository. If a future runtime update requires a new large native binary, decide first whether it should be handled by:

- replacing the existing file in `dll\OpenCVSharp\`,
- NuGet/runtime restore,
- Git LFS,
- or a documented release artifact.

The decision must be made before the large file is committed.

## WPG PropertyGrid Policy

OpenVisionLab uses the prepared WPG PropertyGrid DLLs:

- `dll\System.Windows.Controls.WpfPropertyGrid.dll`
- `dll\System.Windows.Controls.WpfPropertyGrid.xml`

The WPG source project is not part of this solution. If it changes, generate the DLL/XML from its own repository, copy the prepared files here, and run the UI/property-grid validation path.

## Risk

- Missing DLLs must fail fast through `tools\TestExternalReferences.ps1`.
- DLL replacements can break runtime behavior even when compile succeeds, so run focused WPF/recipe smoke checks after DLL changes.
- Public GitHub branches must not reintroduce local SDK sample assets or generated vendor sample folders.
