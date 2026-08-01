# Pipeline: connect, save, and review Tools

## Purpose

Connect Tool input/output Layers and repeat the same ordered settings.

## Build order

1. Select or create a Recipe at the top.
2. Preview the first Tool separately.
3. If correct, select `Add and save to Pipeline`.
4. Set the next Tool input to the previous output Layer.
5. Preview and save each Tool separately.
6. Open `Pipeline`.
7. Check Step numbers and every `input -> output` route.
8. Confirm that Steps not yet run show `WAIT`.
9. Select `Run Review` explicitly.
10. Review output images and metrics from Step 1 onward.
11. On NG, inspect the first NG Step's input and settings.
12. Run Good and Bad through the same Pipeline.
13. Restart and verify restored Steps, routes, and settings.
14. Select Run Review explicitly again.

## Review checklist

Check Step count/order, input-producing Steps, OK/NG/WAIT/Error states, first
failed Step, selected input/output images, drawings, metrics, object rows,
reject reasons, and Good/Bad or Validation Set results.

A valid Pipeline definition is not the same as a final OK inspection result.
