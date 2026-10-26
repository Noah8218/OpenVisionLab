# OpenVisionLab — 2D Inspection Workbench

Build and validate rule-based 2D inspection Recipes on Windows with OpenCvSharp 4 and C#.

[![Release](https://img.shields.io/github/v/release/Noah8218/OpenVisionLab?include_prereleases)](https://github.com/Noah8218/OpenVisionLab/releases)
![Windows](https://img.shields.io/badge/platform-Windows%2010%2F11%20x64-0078D4) ![.NET](https://img.shields.io/badge/.NET-8-512BD4) [![License](https://img.shields.io/badge/license-Apache--2.0-green)](LICENSE) [![CI](https://github.com/Noah8218/OpenVisionLab/actions/workflows/ci.yml/badge.svg)](https://github.com/Noah8218/OpenVisionLab/actions/workflows/ci.yml)

English | [한국어](README.ko.md)

![Create a Recipe, load an image and teach inspection tools](docs/assets/demo/readme-workflows/recipe-teaching.gif)

## Try it in 3 steps

1. Download the `win-x64` ZIP from [Releases](https://github.com/Noah8218/OpenVisionLab/releases/tag/v2.1.0-rc.3). It requires the [.NET 8 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/8.0).
2. Extract the ZIP into a new folder and run `OpenVisionLab.exe`.
3. Open a public sample in Sample Catalog, then choose Preview or Run.

The published ZIP is `v2.1.0-rc.3`; it predates the source workflow shown here.
To use that workflow, [build from source](#build-from-source): clone the repository, build the solution, then press F5.

Current version: `v2.2.0-dev.11` · [Changelog](CHANGELOG.md)

## What it does

- Teach inspection Tools through PropertyGrid parameters, ROIs and acceptance limits.
- Connect Tools in a Pipeline and save the Steps, parameters and Layer connections as Recipe XML.
- Compare source, intermediate and output Layers, detection drawings, measurements and OK/NG reasons.
- Check the same Recipe against Good and Bad samples with Batch inspection.
- Use LLM/XML assistance optionally; it is not required to teach or run an inspection.

## Build and Check a Recipe

1. In **Recipe Manager**, create `Shape Count`, then choose **Open work with selected inspection** and open an image.
2. Open **Threshold**. Turn **Run on change** off, set the threshold to `150`, and choose **Run tool (F8)**. Compare the original and binary images, then choose **Add to pipeline → Add at end**.
3. Open **Contour** and confirm the input is `Threshold_1`. Turn **Use threshold** off, draw the inspection ROI around the five shapes, then choose **Apply** or press **Enter**.
4. Enable **Acceptance** and select **Detection count**. Check **Lower** and **Upper**, set both to `5`, and press **F8**. Confirm `5` detections, then press **Ctrl+Enter** to add the Step.
5. Run the Pipeline with **F5**. Review **Results** and compare source, intermediate and output Layers in the one-, two- or four-panel viewer.
6. Save with **Ctrl+S**. Select another Recipe and return to reload the saved Steps, parameters, ROI and Layer connections.
7. Add Good and Bad images and run **Batch inspection** with the same Recipe. Inspect each image's measurements and OK/NG result.

With **Run on change** off, parameter changes leave the result pending until
the Tool runs. Loading a Recipe restores its settings without running the
inspection. Saving and running are separate actions.

If no enabled Step has acceptance criteria, a run can finish without an OK/NG
verdict. Execution errors are reported separately from NG. After input or
parameter changes, retained results are marked as previous results; run again
to update them. Batch inspection does not retain every intermediate image.
Run the selected image to inspect those outputs.

Use the saved `Shape Count` Recipe on the original and missing-shape images.
Assign **Good** and **Bad** labels, then run **Batch inspection**. With the ROI
and count limits above, the original returns **5 detections / OK** and the
missing-shape image returns **2 detections / NG**: **1 OK, 1 NG and 0 errors**.

![Reload a Recipe and compare Good and Bad inspection results](docs/assets/demo/readme-workflows/recipe-validation.gif)

Good/Bad labels specify the expected class; OK/NG is the inspection result.
Select an image to compare its source, processed image, overlays and acceptance
limits. **Page Up** returns to the previous image.

## One Image, Every Inspection Stage

![Compare Filter, Threshold, Morphology, Blob and Contour results](docs/assets/demo/readme-workflows/inspection-stages.png)

Compare the source, filtered, binary and cleaned images in the four-panel
viewer. The public perforated-plate Recipe runs `Filter -> Threshold -> Morphology`,
then feeds the cleaned Layer into Blob and Contour. Both branches return
34 accepted holes in **Results**. The same area limits exclude the contour
around the full-image background.

[Open the example Pipeline](docs/samples/public/product/Product_Field_PerforatedPlate_Inspection.pipeline.xml)
or select `Product_Field_PerforatedPlate_HoleArray` in Sample Catalog.

Adjust each Step while comparing its intermediate image, then check the final
overlays and measurements in Pipeline Review. Preview and Run execute only
when requested.

## Tool Setup and Results

The recording shows Matching, Line, Blob and Contour settings and results in
the main workbench.

![Configure Tools and review their results](docs/assets/demo/readme-workflows/tool-settings.gif)

[Watch the Tool View MP4](docs/assets/demo/readme-workflows/tool-settings.mp4)

- Matching: inspect the loaded template, matching scores and detections.
- Line: choose Edge, Distance or Intersection mode; this example measures pin spacing.
- Blob: inspect particle detections and count limits.
- Contour: set the inspection ROI and count limits, then check the detected shapes.

Use **Run step (F8)** for the selected Tool or **Run pipeline (F5)** for the
complete sequence.

## Chained Processing

Connect preprocessing and detection Tools in a Pipeline. The recording compares
intermediate images and results from two three-Step Pipelines, including the
Morphology operation and kernel settings:

- `Filter -> Threshold -> Contour`
- `Threshold -> Morphology -> Contour`

![Compare chained preprocessing steps](docs/assets/demo/readme-workflows/preprocessing-chains.gif)

[Watch the chained-processing MP4](docs/assets/demo/readme-workflows/preprocessing-chains.mp4)

Select a Step in Pipeline Review to inspect its intermediate image. Check the
final Step for detected objects and overlays. Run a Tool with **F8** or the
Pipeline with **F5**. Tools with **Run on change** can also run when parameters change.

The next example connects `Filter → Threshold → Contour` and `Threshold → Blob`.
Compare the input and output images, then inspect the detection count and
acceptance limits.

![Preprocessing and particle-count inspection](docs/assets/demo/readme-workflows/sample-inspections.gif)

## Main Features

| Area | Description |
| --- | --- |
| Main Workspace | Load images, manage Layers, zoom, pan and inspect pixel values |
| Layer Docking | Compare source, intermediate and final images in tabs or split views |
| Tool View | Configure Threshold, Filter, Morphology, Blob, Contour, Matching, FeatureMatching, EdgeBasedMatching, Line/Length, Mean and other Tools |
| PropertyGrid | Edit Tool parameters through a consistent property interface |
| Preview / Run | Run a Tool or complete Pipeline; control parameter-triggered execution with Run on change where supported |
| Pipeline / Recipe | Connect Tools as ordered Steps and save them as XML |
| Pipeline Review | Inspect Step inputs and outputs, measurements, drawings, logs and OK/NG reasons |
| Sample Catalog | Browse sample images, baseline Pipelines, expected results and Good/Bad pairs |
| Good/Bad Pair | Compare normal and defective samples using the same measurements |
| Learn Mode | Open guidance for Matching, Blob, Contour, Threshold, Mean, FeatureMatching, EdgeBasedMatching and Line |
| Language | Switch between Korean and English |

Pipeline Review also shows connections and branches, retained result images,
acceptance ranges and values, and parameters to adjust. Metrics include
`ResultCount`, `AreaMax`, `ScoreMax` and `LineAngleAvg`.
See the [workspace reference](docs/runbooks/BUILD.md#workspace-reference) and
[Pipeline Review image](docs/runbooks/DEVELOPMENT.md#pipeline-review).

## Public Samples

[Public samples](docs/samples/public/README.md) include images, recommended
Pipelines, measurements and Good/Bad expectations. Browse the
[public catalog](docs/samples/OpenVisionLab.PublicSampleCatalog.csv) or
[product catalog](docs/samples/OpenVisionLab.ProductSampleCatalog.csv).
The root `Sample/` folder holds local or third-party data and is not part of
the public sample distribution.

![Browse the public sample catalog](docs/assets/demo/readme-workflows/sample-catalog.png)

| Task | Good sample | Bad sample | Measurements |
| --- | --- | --- | --- |
| Locate a target with Matching | `Public_Matching_DiePad_Good` | `Public_Matching_DiePad_NoTarget_Bad` | `ResultCount`, `ScoreMax` |
| Count particles with Blob | `Public_Blob_Particles_Good` | `Public_Blob_Particles_Sparse_Bad` | `ResultCount` |
| Count shapes with Contour | `Public_Contour_Shapes_Good` | `Public_Contour_Shapes_Missing_Bad` | `ResultCount` |
| Segment bright pads with Threshold | `Public_Threshold_BandPads_Good` | `Public_Threshold_BandPads_Missing_Bad` | `ResultCount` |
| Check brightness with Mean | `Public_Mean_Brightness_Good` | `Public_Mean_Brightness_Dark_Bad` | `MeanValueAvg` |
| Compare feature-matching scores | `Public_Feature_Card_Good` | `Public_Feature_Card_Wrong_Bad` | `ScoreMax`, `ResultCount` |
| Compare edge-based shapes | `Public_Edge_Fiducial_Good` | `Public_Edge_Fiducial_Wrong_Bad` | `ScoreMax`, `ResultCount` |
| Measure a distance with Line | `Public_Line_Pins_Good` | `Public_Line_Pins_WidePin_Bad` | `DistanceMmAvg` |

## Build from source

Windows 10/11 x64; Visual Studio 2022 17.8+ with **.NET desktop development**.
Git must be on PATH; the first NuGet restore needs internet access.

1. Clone `https://github.com/Noah8218/OpenVisionLab.git` to a short path.
2. Open `OpenVisionLab.sln`; set **OpenVisionLab** as the startup project.
3. Choose **Debug / Any CPU**, build the solution, then press **F5**.

[Full prerequisites, command-line builds and Windows Sandbox](docs/runbooks/BUILD.md).

## Documentation

Select **Guide** for the offline manual in the current UI language. Its
workflow sections and 17 Tool chapters include screenshots, numbered callouts,
steps to follow and results to check.

[User manual](docs/manual/README.md) · [Tutorial](docs/learn/OPENVISIONLAB_TUTORIAL.md) · [Learn Mode](docs/learn/README.md)
[Build](docs/runbooks/BUILD.md) · [Development and checks](docs/runbooks/DEVELOPMENT.md) · [Changelog](CHANGELOG.md) · [Documentation index](docs/README.md)

## Status and limitations

The source is `v2.2.0-dev.11`; the published ZIP is the older `v2.1.0-rc.3` prerelease.
LLM/XML assistance is optional and is not part of inspection execution.
Validate Recipes with representative production images before using them for inspection.
Check UI changes in the build that contains them; recordings do not verify later changes.

## Part of OpenVisionLab

| Project | What it does |
| --- | --- |
| [2D Workbench](https://github.com/Noah8218/OpenVisionLab) | Rule-based 2D inspection Recipes |
| [3D Studio](https://github.com/Noah8218/OpenVisionLab-3D-Studio) | 3D teaching, Thickness and Warpage |
| [Labeling Studio](https://github.com/Noah8218/OpenVisionLab-Labeling-Studio) | Labeling, training and model comparison |
| [Machine Studio](https://github.com/Noah8218/OpenVisionLab-Machine-Studio) | Machine simulation before hardware |
| [Vision SDK](https://github.com/Noah8218/OpenVisionLab-Vision-SDK) | Shared C# inspection library |

## License

OpenVisionLab is licensed under the [Apache License 2.0](LICENSE).

```text
This project includes software developed by 최노아(Noah-Choi).
Copyright (c) 2026 최노아(Noah-Choi).
```

Commercial use, modification and redistribution are permitted under the license
terms. Copies or substantial portions must retain `LICENSE`, [NOTICE](NOTICE),
copyright and attribution notices.
