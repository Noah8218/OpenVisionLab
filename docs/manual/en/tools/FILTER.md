# Filter: remove noise or sharpen boundaries

## Purpose

Use Filter before Threshold, Edge, or Matching to reduce small noise or reinforce
boundaries. Preserve the original and write to a separate output Layer.

## Steps

1. Decide whether the problem is point noise or brightness fluctuation.
2. Open `Filter`.
3. Confirm the input Layer and `Filter_Preview` output.
4. Select the Filter Type.
5. Start with a small odd Kernel size.
6. For Gaussian or Bilateral, check the Sigma values.
7. Set an ROI when needed.
8. Select `Run Preview`.
9. Compare original and result noise and boundaries.
10. Save only when the downstream Threshold or Edge result improves.

## Selection guide

- Gaussian: smooth random noise
- Median: point or salt-and-pepper noise
- Bilateral: smooth while preserving edges
- Sharpen: reinforce weak edges; it may also amplify noise

Check Filter Type, Kernel, Sigma, then compare with the original. Reduce the
Kernel if target boundaries disappear or separate Blobs merge.
