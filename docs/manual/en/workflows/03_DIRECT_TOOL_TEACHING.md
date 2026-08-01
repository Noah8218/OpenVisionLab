# Tool setup and explicit Preview

## Common order

1. Prepare the image and input Layer.
2. Open the required Tool from search or the left list.
3. Confirm the input and output Layers first.
4. Start with the `Basic` preset when one is available.
5. Adjust only the main value and ROI in PropertyGrid first.
6. Read the target, background, and direction guidance.
7. Select `Run Preview` explicitly.
8. Check that drawings on the result Layer point to the physical target.
9. Check that the metrics describe the same result as the drawings.
10. If it fails, inspect input, ROI, basic parameters, then advanced parameters.
11. Select `Add and save to Pipeline` only after the result is correct.

## Actions that do not run anything

Opening, closing, or docking a Tool; selecting a preset; changing PropertyGrid
values; selecting Layers; changing ROI/result visibility; and creating an
output Layer do not run Preview or the Pipeline.

## Review order

1. Preview status: OK or NG
2. Updated output image
3. Boxes, lines, centers, and ROI on the intended target
4. Metrics such as ResultCount, Area, Score, or Distance
5. Unexpected processing-time increases

`Preview OK` means the Tool execution completed. It is not the same as a final
inspection result of OK.
