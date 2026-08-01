# Morphology: remove noise or connect regions

## Purpose

Use Morphology to remove small white points or connect broken regions in a
Threshold result. Prefer a binary input Layer.

## Steps

1. Identify points to remove or gaps to connect in Threshold Preview.
2. Open `Morphology`.
3. Set input `Threshold_Preview` and output `Morphology_Preview`.
4. Select Open, Close, Erode, or Dilate.
5. Start with a small Kernel shape and size.
6. Start with one iteration.
7. Select `Run Preview`.
8. Check that original target sizes and counts remain meaningful.
9. Add Blob or Contour after it and inspect the actual count change.
10. Save only when the physical result is correct.

Open removes small white noise; Close fills small black gaps; Erode shrinks white
regions; Dilate expands them. Check binary polarity, operation, Kernel, and
iteration count in that order. A correct count with merged objects is not valid.
