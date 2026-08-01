# Prepare images, public samples, and Layers

## Open an image

- `Load Image`: opens one of your images into the current workspace `Main` Layer.
- `Open Sample`: opens a documented public sample with a purpose, Good/Bad pair,
  acceptance criteria, and prepared Pipeline.

Start with a public sample while learning. Use your own images after the
inspection purpose, ROI, and normal range are defined.

## Basic Layer rules

- `Main` is the original image.
- A result Layer stores a Tool Preview or Pipeline Step output.
- The input Layer selects the image a Tool reads.
- The output Layer selects where the result is stored.
- Creating or selecting an output Layer does not change the input automatically.

## Steps

1. Load an image or open a public sample.
2. Check the current Layer name and image size in the center viewer.
3. Keep the original image in `Main`.
4. Open a Tool and select its input Layer explicitly.
5. Give the output Layer a purpose-specific name, such as `Threshold_Preview`.
6. Run Preview and compare `Main` with the output Layer.
7. Before deleting or renaming a Layer, check whether a Pipeline Step uses it.

## ROI

An ROI is the image region to inspect. Start with the full image, then narrow
the ROI when distracting objects enter the result. A small ROI can clip a valid
target; a large ROI can include unrelated objects.

## Completion check

- You can name the current image and Layer.
- The Tool reads from and writes to different Layers.
- The original `Main` Layer is preserved.
