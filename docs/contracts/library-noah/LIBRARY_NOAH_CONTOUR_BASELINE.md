# Library-Noah Contour Baseline

Last updated: 2026-06-14

This baseline was captured before refactoring Library-Noah contour internals.

Purpose:

- Keep `ContourTool` behavior measurable before cleanup.
- Protect OpenVisionLab pipeline samples from accidental algorithm drift.
- Provide reference metrics for `CVContour -> ContourTool` compatibility work.

## Verification Command

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "C:\Git\OpenVisionLab_Dev\tools\RunVisionSampleCatalog.ps1" -OutputDir "C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_contour_baseline"
```

Output report:

```text
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_contour_baseline\sample_catalog_report.md
```

## Baseline Result

All required sample catalog rows passed.

| Sample | Pipeline | Status | Key result |
| --- | --- | --- | --- |
| `Contour_TextSymbols` | `Contour_TextSymbols.pipeline.xml` | OK | `ResultCount=51`, expected `35..80` |
| `Contour_AllSymbolsAndFaint_LLM` | `Contour_AllSymbolsAndFaint_LLM.pipeline.xml` | OK | text branch `51`, faint top `2`, faint phone `2` |
| `Contour_Generic` | `Threshold_Morphology_Contour.pipeline.xml` | OK | `ResultCount=21`, expected `10..30` |
| `Rice_Particle` | `Rice_Particle_Contour.pipeline.xml` | OK | `ResultCount=123`, expected `100..170` |
| `Pins_Feature` | `Pin_Feature_Contour.pipeline.xml` | OK | `ResultCount=54`, expected `40..70` |
| `BentPin_Large` | `BentPin_LargeContour.pipeline.xml` | OK | `ResultCount=2`, expected `1..5` |
| `DiePad1_Surface` | `DiePad_Surface_Contour.pipeline.xml` | OK | `ResultCount=11`, expected `8..25` |
| `DiePad2_Surface` | `DiePad_Surface_Contour.pipeline.xml` | OK | `ResultCount=14`, expected `8..25` |
| `DiePad3_Surface` | `DiePad_Surface_Contour.pipeline.xml` | OK | `ResultCount=16`, expected `8..25` |
| `DiePad4_Surface` | `DiePad_Surface_Contour.pipeline.xml` | OK | `ResultCount=14`, expected `8..25` |

## Contour Metrics To Preserve

For `Contour_TextSymbols`:

- `ResultCount=51`
- `AreaMin=54.5`
- `AreaMax=1227`
- `AreaAvg=308.971`
- `AngleMin=-90`
- `AngleMax=-3.4`
- `AngleAvg=-84.282`
- `Overlays=51`

For `Contour_AllSymbolsAndFaint_LLM`:

- Step 03 text/symbol contour: `ResultCount=51`, `Overlays=51`
- Step 06 faint top contour: `ResultCount=2`, `Overlays=2`
- Step 09 faint phone contour: `ResultCount=2`, `Overlays=2`

## Refactor Guardrail

When refactoring `ContourTool`, the first acceptance condition is:

1. All sample catalog rows remain OK.
2. `Contour_TextSymbols` remains within `ResultCount=35..80`.
3. `Contour_AllSymbolsAndFaint_LLM` does not lose the faint top and faint phone branches.
4. Final result images are still generated for every required sample.

If these move, the change must be treated as an algorithm behavior change, not just cleanup.

## Post Wrapper Verification

After changing legacy `CVContour` to delegate to `ContourTool`, the sample catalog was run again.

Command output:

```text
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_contour_after_cvcontour_wrapper\sample_catalog_report.md
```

Result:

- Library-Noah standalone build: OK.
- OpenVisionLab build: OK.
- Required sample catalog rows: OK.
- `Contour_TextSymbols`: still `ResultCount=51`.
- `Contour_AllSymbolsAndFaint_LLM`: text branch `51`, faint top `2`, faint phone `2`.

Conclusion:

`CVContour` can now be treated as a compatibility wrapper. Current OpenVisionLab pipeline behavior is preserved.
