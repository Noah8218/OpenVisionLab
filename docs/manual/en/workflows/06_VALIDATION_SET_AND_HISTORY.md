# Validation Set, Run History, and Qualified Snapshot

Use this advanced workflow when validating more than one Good/Bad pair.

## Validation Set

1. Select the current Recipe in Recipe Manager.
2. Create and name a Local Validation Set.
3. Assign expected role `OK` or `NG` to every image.
4. Check that the same path is not registered twice.
5. Recover only the selected missing row when a path is unavailable.
6. Save the Pipeline and acceptance criteria.
7. Start Validation explicitly.
8. Review expected/actual decisions and the first failed Step.

## Run History

Select a completed run and verify its Recipe, Pipeline, input image, and time.
Review each saved Step result and drawing. Do not confuse a historical run with
the current Recipe. Export a report only through the explicit export command.

## Qualified Snapshot

1. Select a completed full Validation Set run.
2. Check scope, notes, and dependency state.
3. Create the snapshot explicitly.
4. Verify its hash and qualification state.
5. Create a working copy for later edits; do not modify the original snapshot.

A snapshot does not automatically certify camera calibration, production
conditions, or field performance.
