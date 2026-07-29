# CVR-11 Edge Global Polarity v1

Updated: 2026-07-28 KST

## Outcome

Status: Complete for the bounded synthetic v1 contract.

EdgeBasedMatching now has an opt-in whole-candidate global polarity reversal.
The default remains the existing signed Same-only matcher. Enabled matching
reports the selected `Same` or `Reversed` state in metrics, MatchingResult, and
the result drawing.

## Implementation

- Library-Noah owns the score and result state.
- OpenVisionLab owns the PropertyGrid, Pipeline/XML mapping, validation, and
  vendored DLL.
- `ALLOW_GLOBAL_POLARITY_REVERSAL=false` is the missing-key default.
- The option compares one globally consistent sign. It does not take an
  absolute value independently for every template edge.
- `GlobalPolarity.AllowReversal` and `GlobalPolarity.Reversed` persist through
  the existing numeric metric/report path.
- PropertyGrid changes remain teaching edits and do not run Preview.

## Matrix Result

The fixed project-authored asymmetric target used one 64x64 template, one
192x144 search image, `SCORE_MIN=0.8`, `NUM_MATCH=1`, `SEARCH_STEP=1`, and
position refinement.

| Split | Targets | No target | Result |
| --- | ---: | ---: | --- |
| Train | 4 Same + 4 Reversed | 0 | 8/8 correct |
| Validation | 2 Same + 2 Reversed | 2 | 6/6 correct |
| Held-out | 2 Same + 2 Reversed | 2 | 6/6 correct |

All 16 target rows scored 100, retained the correct state, and had 0.429 px
reported center error. All four no-target rows returned
`MatchingNoResult`. A separate legacy reversed probe rejected.

## UI Evidence

- Before:
  `artifacts/cvr11_global_polarity_20260728/before/wpf_shell_host_edge_based_matching_tool.png`
- After:
  `artifacts/cvr11_global_polarity_20260728/after/wpf_shell_host_edge_based_matching_tool.png`

The after view shows `Allow global polarity...` in the Matching PropertyGrid.
The smoke set the option, added a Pipeline Step, round-tripped it through
selected-Step PropertyGrid, restored `false` after removing the XML key, and
kept Preview/Run, active layer, and routing unchanged.

Representative drawings:

- `runtime/drawings/validation_same_01.png`
- `runtime/drawings/validation_reversed_01.png`
- `runtime/drawings/heldout_no_target_01.png`

The full matrix, source/drawing hashes, frozen XML, and completion record are
under `artifacts/cvr11_global_polarity_20260728/runtime`.

## Verification

- `dotnet build C:\Git\Library-Noah\Lib.Common.sln -c Release`: 0 warnings,
  0 errors.
- `dotnet run --project C:\Git\Library-Noah\Lib.Inspection.Smoke\Lib.Inspection.Smoke.csproj -c Release`:
  67/67 passed.
- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"`: 0 warnings,
  0 errors.
- `VisionRecipeRunnerSmoke --edge-global-polarity-contract`: 20/20 passed.
- `wpf_shell_host_edge_based_matching_tool`: `check=OK`, `layout=0`, `text=0`,
  `internal=0`, 1600x900.
- Library-Noah Release, vendored, and Debug `Lib.OpenCV.dll` SHA-256:
  `8F43BD7E897C8EBEB71C244AB6B2479F4B709A5A7EC3475926C1428E03676931`.

## Boundary And Next Dependency

This is exact synthetic global-reversal evidence, not physical polarity,
mixed/local polarity, lighting, deformation, production, or field evidence.
Do not enable the option in a qualified physical recipe without a named
feature, representative labelled captures, frozen settings, and held-out
review. CVR-12 remains conditional on a separate bounded physical-deformation
task. CVR-00 still requires three independent novice participants.
