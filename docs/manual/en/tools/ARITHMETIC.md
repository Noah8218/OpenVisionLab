# Arithmetic: combine, subtract, or mask two Layers

## Purpose

Use Arithmetic to add, subtract, multiply, divide, or apply AND/OR/XOR to two
images or masks. Layers A and B must have the same size and coordinate meaning.

## Steps

1. Prepare the two Layers to compare or combine.
2. Open `Arithmetic`.
3. Select input Layer A and input Layer B explicitly.
4. Select a separate output Layer.
5. Choose Add, Subtract, Multiply, Divide, or a bitwise operation.
6. Start coefficients and offsets at their defaults.
7. Select `Run Preview`.
8. Confirm that the two Layers are spatially aligned.
9. Check that values are not saturated at zero or the maximum.
10. Save the route when the result will feed another Tool.

Check A/B selection, image size, alignment, operation direction, coefficients,
and offsets in that order. `A-B` and `B-A` are different.
