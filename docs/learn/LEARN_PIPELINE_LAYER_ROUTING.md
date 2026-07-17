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
| Preview/Run result | Image and metrics produced from the current settings |

## Building A Route

1. Keep `Main` as the original reference image.
2. Select the image a Step reads in `InputLayer`.
3. Give `OutputLayer` a name that describes the result.
4. If a Step needs a previous result, select that previous `OutputLayer` as its `InputLayer`.
5. For independent inspections, branch from the same source into separate output layers.
6. Use review/overlay layers to show how the final metric relates to the image.

## Reading A Route

- `InputLayer` is the source image or previous result a step reads.
- `OutputLayer` is the produced result a step writes.
- The next Step can continue from a previous result by selecting its `OutputLayer` as the next `InputLayer`.
- Compare `Main`, the current input, and the current output to see exactly what the Step changed.

## Operator Route Review Loop

1. Select the intended `InputLayer`.
2. Select a separate and descriptive `OutputLayer`.
3. Click Preview or Run Review.
4. Compare `Main`, the selected input, and the produced output layer.
5. Save the recipe only after the Good/Bad metric gates pass for the intended reason.

If a later Step gives an unexpected result, inspect the previous `OutputLayer` first and move backward until the first changed image is found.

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
4. Run Preview or Run Review.
5. Compare input and output layers after execution.
6. If a later step fails, inspect the previous output layer first.

## Common Failures

| Symptom | Likely Cause | First Fix |
| --- | --- | --- |
| Step reads the wrong image | `InputLayer` points to an old output | Select the intended source layer |
| Original image seems changed | User is viewing an output layer, not `Main` | Switch back to `Main` or compare layers |
| Later step has no result | Previous output layer has no usable image | Review the previous Step and run the recipe again |
| Branch result is confusing | Output names are too generic | Rename outputs by purpose, not tool only |
| Review overlay hides defect | Overlay is final review, not the metric source | Inspect the raw metric step layer |

## Operator Checklist

Before saving a recipe, answer:

1. What is the original image layer?
2. Which step creates each intermediate layer?
3. Which layer is used for the final decision?
4. Which layer should the operator compare when NG occurs?
5. Can the recipe be rerun without relying on a stale preview layer?

Practice by selecting one Step at a time and explaining its input image, output image, and metric before moving to the next Step.
