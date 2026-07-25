# Next Structural Boundary Audit (2026-07-26)

## Status

Complete.

## Decision

Do not create another command-surface partial as the next step.

| Candidate | Decision | Evidence-based reason |
| --- | --- | --- |
| LLM draft/intent commands in `Handlers.cs` | Exclude | Product policy freezes LLM expansion; moving them would not create new deterministic product value. |
| `OpenVisionPipelineReviewDocument` | Defer | Its execution, display state, and fixture-chain routing remain coupled; a file move now would not produce an independent owner. |
| Remaining `VisionPipelineStepPropertyMapper` tool families | Select next | The mapper already has per-family adapters and the remaining Create/Apply pairs can be moved behind the same XML/property contract without WPF state. |

## Next Slice Contract

- First inventory the root mapper's remaining canonical ToolType Create/Apply pairs and identify one cohesive family.
- Extract that family into a non-WPF mapper adapter with explicit `VisionPipelineStep` input/output behavior.
- Preserve missing-key/default/XML round-trip behavior and run the focused PropertyGrid smoke for that family.

## Boundary

This audit does not change runtime behavior or claim a refactor. It prevents another partial-only split and records why the next work returns to a true mapper boundary.
