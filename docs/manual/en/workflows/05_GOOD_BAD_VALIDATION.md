# Validate one rule with Good and Bad samples

## Principle

Do not tune different values for Good and Bad. Run the same saved Pipeline and
acceptance criteria on both images, then verify that the metrics separate for
the intended physical reason.

## Steps

1. Open the Good sample.
2. Select `Run Review` in Pipeline Review.
3. Inspect drawings and metrics for every Step.
4. Record why the Good result is inside the acceptance range.
5. Open the paired sample or NG reference.
6. Without changing settings, run the same Pipeline.
7. Select the first NG Step.
8. Inspect its input Layer, drawings, metrics, and reject reason.
9. Record why the Bad result is outside the acceptance range.
10. If only the number differs by accident, fix ROI or target selection first.

## States

- `Validation OK`: the Pipeline definition and input are executable.
- `Result OK`: inspection metrics are inside the saved normal range.
- `Result NG`: inspection metrics are outside that range.
- `Error`: an input, dependency, parameter, or execution problem occurred.

## Completion check

Good and Bad use the same target and coordinate meaning; drawings point to the
physical target; metric differences match the inspection intent; and input,
ROI, polarity, and Template were checked before changing acceptance criteria.
