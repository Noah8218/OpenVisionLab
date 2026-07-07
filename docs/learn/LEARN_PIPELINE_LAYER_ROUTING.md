# Pipeline and Layer Routing

Updated: 2026-07-07

OpenVisionLab recipes are built from layers and steps.

A layer holds an image result. A pipeline step reads one input layer, writes one output layer, and may produce metrics. Operators should be able to review each layer without losing the original input.

## Core Concepts

| Concept | Meaning in OpenVisionLab |
| --- | --- |
| `Main` layer | Original loaded image for the current review |
| `InputLayer` | Layer a step reads from |
| `OutputLayer` | Layer a step writes to |
| Branch | Multiple steps reading from the same source layer |
| Review layer | Overlay or merged result used to explain the final decision |
| Explicit Preview/Run | User action that actually executes a tool or recipe |

## Routing Rules

1. Do not overwrite `Main` unless the user explicitly chooses to replace the loaded image.
2. Output layer creation must not automatically change the input layer.
3. Visibility toggles must not run tools.
4. Loading or deleting a layer must not run tools.
5. Preview and Run must be explicit user actions.
6. If a step needs a previous result, set its `InputLayer` to that previous `OutputLayer`.
7. If two inspections must be compared, keep separate output layers and use review/overlay layers for final evidence.

## Route Safety Checklist

- `InputLayer` is the source image or previous result a step reads.
- `OutputLayer` is the produced result a step writes.
- Creating an `OutputLayer` must not select, rewrite, or silently replace `InputLayer`.
- Layer create/delete/load-image actions and visibility toggles must not run Preview/Run.
- Execute Preview or Run Review only by explicit user action.

## Common Pipeline Shapes

| Shape | Example | Reason |
| --- | --- | --- |
| Linear preprocessing | `Main -> Threshold_Result -> Blob_Result` | Later tool needs a binary image |
| Branch comparison | `Main -> Mean_Result`, `Main -> Matching_Result` | Two independent checks on the same image |
| Review overlay | `Main + Contour_Result -> Review_Result` | Final image explains the metric |
| Two-input operation | `InputA + InputB -> Arithmetic_Result` | Difference, mask, or merge-style comparison |

## What To Check

1. Open the step list and read each `InputLayer` and `OutputLayer`.
2. Confirm every input layer exists before running.
3. Confirm output layer names are unique and descriptive.
4. Run Preview/Run explicitly.
5. Compare input and output layers after execution.
6. If a later step fails, inspect the previous output layer first.

## Common Failures

| Symptom | Likely Cause | First Fix |
| --- | --- | --- |
| Step reads the wrong image | `InputLayer` points to an old output | Select the intended source layer |
| Original image seems changed | User is viewing an output layer, not `Main` | Switch back to `Main` or compare layers |
| Later step has no result | Previous output layer was never created | Run the previous step or full recipe explicitly |
| Branch result is confusing | Output names are too generic | Rename outputs by purpose, not tool only |
| Review overlay hides defect | Overlay is final review, not the metric source | Inspect the raw metric step layer |

## Operator Checklist

Before saving a recipe, answer:

1. What is the original image layer?
2. Which step creates each intermediate layer?
3. Which layer is used for the final decision?
4. Which layer should the operator compare when NG occurs?
5. Can the recipe be rerun without relying on a stale preview layer?

Opening this guide, creating layers, toggling visibility, or changing routing must not run Preview/Run automatically.
