# Build a Recipe and Pipeline

## Roles

- Recipe: the saved work unit.
- Pipeline: the ordered list of Steps inside a Recipe.
- Step: one Tool, its parameters, and its input/output Layers.

## New Recipe steps

1. Create a Recipe in the top Recipe selector and verify its name.
2. Load an image or public sample.
3. Open the first Tool and set input `Main` and a separate output Layer.
4. Run Preview explicitly and review the result.
5. Select `Add and save to Pipeline`.
6. Set the next Tool input to the previous Step output.
7. Preview, review, and save the next Tool in the same way.
8. Open `Pipeline`.
9. Read every `input -> output` route from top to bottom.
10. Select `Run Review` explicitly.

Example:

```text
01 Threshold : Main -> Threshold_Preview
02 Blob      : Threshold_Preview -> Blob_Preview
```

## Restart check

Save the Recipe and Pipeline, close and reopen the application, select the same
Recipe, and confirm that Steps and routes remain. Restored Steps should show
`WAIT`; restoration is not execution. Prepare an image and select `Run Review`.

## Route problems

- Missing input: compare the previous output name with the current input name.
- Stale result: confirm that you selected Run Review.
- Wrong result: inspect the input image of the first NG Step.
- Branch: reuse the same input Layer only for independent checks of one source.
