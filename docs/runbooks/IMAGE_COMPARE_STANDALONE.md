# Image Compare Standalone Guide

`OpenVisionLab.ImageCompare` is a standalone EXE for running only the image comparison tool without launching the main OpenVisionLab application.

## Purpose

- Compare 2 to 16 images.
- Inspect RGB/GV values at the same image coordinate.
- Show source PNG/BMP bit depth from the original file header.
- Keep pixel marker and mouse coordinate aligned at high zoom.
- Ship a smaller comparison utility without the full OpenVisionLab application.

## Build and Run From Source

From the repository root:

```powershell
dotnet build tools/OpenVisionLab.ImageCompare/OpenVisionLab.ImageCompare.csproj -c Debug
dotnet run --project tools/OpenVisionLab.ImageCompare/OpenVisionLab.ImageCompare.csproj -c Debug -- "D:\Images\a.png" "D:\Images\b.png" "D:\Images\c.png"
```

Windows and the supported .NET Windows Desktop runtime are required. Image paths
may be supplied as startup arguments. Preserve the executable's dependency set;
`OpenCvSharpExtern.dll` is an OpenCvSharp native runtime dependency.

## Image Format Policy

The viewer may normalize image data for OpenGL/canvas rendering, but the header text shows the original source format.

Examples:

- PNG gray 8-bit: `PNG 8-bit Gray`
- BMP 8-bit: `BMP 8-bit`
- PNG RGB 24-bit: `PNG 24-bit RGB`

## Verification

Current smoke coverage:

- `image_compare_png_source_format`: validates source PNG 8-bit gray format text.
- `image_compare_multi_load`: validates multi-image loading.

## Source Dependency

Only this repository checkout is needed for normal source build. Prepared runtime DLLs are vendored under `dll/`; another SDK or WPG source checkout is not a normal build prerequisite.
