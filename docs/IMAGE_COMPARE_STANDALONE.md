# Image Compare Standalone Guide

`OpenVisionLab.ImageCompare` is a standalone EXE for running only the image comparison tool without launching the main OpenVisionLab application.

## Purpose

- Compare 2 to 16 images.
- Inspect RGB/GV values at the same image coordinate.
- Show source PNG/BMP bit depth from the original file header.
- Keep pixel marker and mouse coordinate aligned at high zoom.
- Ship a smaller comparison utility without the full OpenVisionLab application.

## Publish

Default publish:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Publish-ImageCompare.ps1
```

Publish with a startup smoke test:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Publish-ImageCompare.ps1 -SmokeTest
```

Output:

```text
dist\OpenVisionLab.ImageCompare\
```

The default output is framework-dependent. The target PC needs `.NET 8 Windows Desktop Runtime`.

Use self-contained publish when the target PC should run without a preinstalled runtime.

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Publish-ImageCompare.ps1 -SelfContained
```

## Run

Copy the whole output folder, then run the EXE.

```powershell
.\dist\OpenVisionLab.ImageCompare\OpenVisionLab.ImageCompare.exe
```

Image paths can be passed as startup arguments.

```powershell
.\dist\OpenVisionLab.ImageCompare\OpenVisionLab.ImageCompare.exe `
  "D:\Images\a.png" `
  "D:\Images\b.png" `
  "D:\Images\c.png"
```

## Image Format Policy

The viewer may normalize image data for OpenGL/canvas rendering, but the header text shows the original source format.

Examples:

- PNG gray 8-bit: `PNG 8-bit Gray`
- BMP 8-bit: `BMP 8-bit`
- PNG RGB 24-bit: `PNG 24-bit RGB`

## Output Policy

`dist\OpenVisionLab.ImageCompare` is a generated publish output. Do not use it as a source-controlled dependency; regenerate it from this repository with `scripts\Publish-ImageCompare.ps1` whenever the standalone tool needs to be packaged.

The publish script rejects files that are not needed by the standalone compare tool.

- `OpenVisionLab.exe`
- `OpenVisionLab.dll`
- `Lib.Common.dll`
- `log4net.dll`
- `System.IO.Ports.dll`
- `cvextern.dll`
- `opencv_ffmpeg400_64.dll`
- `System.Windows.Controls.WpfPropertyGrid.dll`
- `*.pdb` by default

`OpenCvSharpExtern.dll` is kept because it is an OpenCvSharp native runtime dependency.

## Verification

Current smoke coverage:

- `image_compare_png_source_format`: validates source PNG 8-bit gray format text.
- `image_compare_multi_load`: validates multi-image loading.
- `Publish-ImageCompare.ps1 -SmokeTest`: validates the standalone EXE starts.

Latest verified baseline:

```text
Release publish: OK
Files: 8
Size: about 4.48 MB
Smoke test: OK
```

## Source Dependency

The development PC that runs publish needs only this repository checkout. External OpenVisionLab source repositories are not required for normal build or publish, because prepared DLLs are vendored under `dll\`.

```text
C:\Git\
  OpenVisionLab_Dev\
```

The generated `dist\OpenVisionLab.ImageCompare` folder can be copied and run on another PC without `Library-Noah` or `WPG-CUSTOM` source checkouts.
