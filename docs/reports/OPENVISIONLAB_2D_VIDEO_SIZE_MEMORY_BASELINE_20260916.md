# OpenVisionLab 2D-034 — Supported image-size memory, timing, and copy baseline

Status: Complete (measurement-only baseline)

Date: 2026-09-16

Repository: `C:\Git\2D\Dev`

## Scope and owner

This slice measures the existing call path without changing production behavior:

`VisionRecipeRunnerSmoke -> VisionRecipeRunner -> existing Mean tool -> VisionRecipeRunResult -> BitmapImageConverter`

The smoke harness generated deterministic in-memory `CV_8UC1` inputs with gray value 200 and ran one-step and two-step Mean pipelines. Each scenario used five warm-up runs and thirty measured runs. The measured phases are:

- input allocation and one-time fixture bitmap conversion;
- per-run `BitmapImageConverter.ToMat` input conversion;
- existing `VisionRecipeRunner` calculation and acceptance;
- result/context clone (`ResultImage.Clone()`);
- bitmap publication conversion (`BitmapImageConverter.ToBitmap`).

The process probe records private-memory peak and after-run values, managed GC bytes, stage timings, result identity, and estimated layer/cache bytes. `EstimatedLayerCacheBytes` is an accounting estimate (`input + result × layer count`), not a claim about an undocumented SDK cache implementation.

## Results

P50/P95 values are milliseconds. Peak and after-run private memory are MiB.

| Build | Size / shape | Input conversion | Calculation | Context clone | Bitmap publish | Total | Peak / after |
|---|---|---:|---:|---:|---:|---:|---:|
| Debug | 1000² / 1 layer | 15.922 / 20.821 | 5.371 / 7.467 | 0.379 / 1.336 | 0.409 / 0.699 | 44.392 / 52.589 | 38.0 / 32.3 |
| Debug | 1000² / 2 layers | 11.553 / 15.166 | 7.308 / 9.149 | 0.264 / 0.414 | 0.345 / 0.409 | 37.477 / 42.614 | 34.4 / 28.2 |
| Debug | 4512² / 1 layer | 112.718 / 127.459 | 158.989 / 179.373 | 20.295 / 25.611 | 6.423 / 9.303 | 362.762 / 406.613 | 163.4 / 66.2 |
| Debug | 4512² / 2 layers | 113.776 / 132.724 | 274.915 / 328.785 | 20.978 / 25.148 | 6.677 / 7.999 | 483.429 / 580.360 | 200.8 / 65.6 |
| Debug | 8192² / 1 layer | 345.961 / 375.192 | 521.966 / 646.081 | 68.411 / 80.679 | 16.844 / 19.551 | 1133.052 / 1302.446 | 472.9 / 152.3 |
| Debug | 8192² / 2 layers | 350.591 / 371.775 | 902.328 / 1045.077 | 67.980 / 75.774 | 16.848 / 20.922 | 1516.150 / 1662.364 | 601.2 / 152.9 |
| Release | 1000² / 1 layer | 14.976 / 17.087 | 5.173 / 5.956 | 0.314 / 0.415 | 0.375 / 0.559 | 38.923 / 41.189 | 36.9 / 32.2 |
| Release | 1000² / 2 layers | 13.289 / 16.811 | 7.600 / 9.354 | 0.310 / 0.366 | 0.379 / 0.435 | 39.841 / 48.830 | 33.9 / 28.5 |
| Release | 4512² / 1 layer | 39.378 / 48.695 | 164.007 / 180.500 | 20.941 / 24.558 | 6.473 / 8.436 | 294.431 / 335.216 | 167.8 / 71.0 |
| Release | 4512² / 2 layers | 40.996 / 60.719 | 288.315 / 411.271 | 21.849 / 26.318 | 6.948 / 9.957 | 426.221 / 785.325 | 207.1 / 71.0 |
| Release | 8192² / 1 layer | 93.960 / 104.146 | 552.257 / 580.243 | 71.805 / 80.417 | 17.182 / 19.255 | 914.131 / 966.227 | 472.8 / 153.2 |
| Release | 8192² / 2 layers | 99.893 / 122.891 | 943.543 / 1106.644 | 71.629 / 90.566 | 17.812 / 22.003 | 1331.173 / 1525.462 | 602.0 / 153.3 |

The 8192² two-layer row is the largest observed case: approximately 602 MiB peak private bytes and an estimated 192 MiB input/result layer accounting. These observations are a baseline for a later decision; they do not establish a product memory cap, a cache eviction policy, or a speed ranking across machines.

## Evidence

- Debug report and CSV: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d034-debug-20260916-run1\video-size-memory-baseline.txt`, `video-size-memory-baseline-runs.csv`, and `video-size-memory-baseline-summary.csv`.
- Release report and CSV: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d034-release-20260916-run1\video-size-memory-baseline.txt`, `video-size-memory-baseline-runs.csv`, and `video-size-memory-baseline-summary.csv`.
- Both runs used repository commit `176eec95e0081502dc07d89ddaee666abac8123d`, five warm-ups, and thirty measured runs per size/shape.
- Default sizes were 1MP (`1000x1000`), `4512x4512`, and `8192x8192`. `20000x20000` remained preflight-only and was not allocated.

## Boundaries

This is a headless core/Runner measurement. WPF rendering, actual monitor/DPI placement, UI interaction, cancellation, low-memory fault injection, camera/hardware, and long-duration qualification remain unverified. No production owner, diagnostic retention policy, output-size cap, or cache eviction behavior was changed.

The next independent P1 boundary is 2D-036: inspect and, only where source evidence requires it, add a pre-allocation guard for impossible output dimensions and memory estimates. Do not infer a user-facing cap from this baseline alone.
