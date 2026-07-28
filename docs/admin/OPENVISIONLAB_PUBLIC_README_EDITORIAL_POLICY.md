# OpenVisionLab Public README Editorial Policy

Updated: 2026-07-28 KST

Status: Active

## Decision

The public GitHub README describes what users can do with OpenVisionLab:

- configure Tool Views;
- build and review Pipelines;
- inspect intermediate images, measurements, and drawings;
- use public examples;
- build, run, and validate the application.

Do not add repeated non-goal statements about camera control, lighting control,
PLC integration, industrial I/O, accounts, or deployment to the public README.
Those boundaries are an internal product-development agreement and do not need
to be presented as introductory user-facing content.

This decision remains in force until the user explicitly changes it.

## Placement Rule

| Information | Correct location |
| --- | --- |
| User-visible capabilities and workflows | Public `README.md` |
| Build, run, sample, and validation instructions | Public `README.md` |
| Internal product boundaries and excluded expansion scope | `AGENTS.md`, product contracts, handoffs, and planning documents |
| A concrete compatibility requirement that affects installation or use | The relevant installation or compatibility document |

## README Review Checklist

Before publishing a README change:

1. Search the entire README for `camera`, `lighting`, `PLC`, `I/O`,
   `equipment-control`, `out of scope`, and equivalent exclusion wording.
2. Remove internal non-goal boilerplate.
3. Confirm the README still explains the product through capabilities and
   operator workflows.
4. Preserve real UI names, commands, links, media, and user-relevant validation
   boundaries.
5. Verify all local links and Markdown fences.

## Completion Record

Status: Complete

Scope: Public README editorial rule separating user-facing product information
from internal scope-exclusion agreements.

Acceptance criteria: Public README contains no camera, lighting, PLC, or
industrial-I/O non-goal boilerplate; the rule is durable in `AGENTS.md` and this
policy document.

Verification: Full README term scan, local-link check, Markdown-fence check,
and `git diff --check`.

Evidence: `README.md`, `AGENTS.md`, and this document.

Boundary / next dependency: This policy does not change product behavior or
internal development scope. It changes only what belongs in the public README.
