# First run: check a public Blob sample

## Goal

Open the application, run a public sample, and learn that setup, execution, and
inspection decisions are separate stages.

## Steps

1. Start OpenVisionLab.
2. Confirm that the language selector at the upper right shows `English`.
3. Select `Open Sample` on the start screen.
4. Enter `Public_Blob_Particles_Good` in the search box.
5. Review the Good role and the `ResultCount 8..14` acceptance range.
6. Select `Open this sample`. Opening an image does not run the Pipeline.
7. Open `Blob` from the left Tool rail.
8. Confirm input Layer `Main` and output Layer `Blob_Preview`.
9. Select the `Basic` preset. A preset changes values but does not run Preview.
10. Select `Run Preview` yourself.
11. Confirm that 12 boxes and centers sit on the 12 bright particles and that
    the metric is `ResultCount=12`.
12. If they agree, select `Add and save to Pipeline`.

![Public Blob Good result](../../../assets/tutorial/current/public_blob_particles_good_result.png)

## Expected screen state

- Source Layer: `Main`
- Result Layer: `Blob_Preview`
- Status: `Preview OK`
- Detected objects: 12
- Drawings: boxes and centers on the physical bright particles

## If the result differs

Check the sample name, Tool input Layer, threshold polarity/value, area range,
and whether you actually selected `Run Preview`. A correct count with boxes on
noise or the outer ellipse is not a valid result.
