# OpenVisionLab Recipe XML Handoff 0.1.11 — Focused Validation

Date: 2026-09-04 KST  
Repository: `C:\Git\2D\Dev`  
Installed candidate: `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff`  
Parent candidate: `openvisionlab-recipe-xml-handoff 0.1.10`  
Lifecycle: candidate, explicit-only, inactive, unqualified

## Scope and decision

This checkpoint applies the approved candidate-only continuation to the
installed skill without changing the failed v0.1.10 benchmark root or the
Original repository. The candidate version is incremented to `0.1.11` so the
previous candidate identity remains traceable.

The correction adds public, case-independent rules for:

- integrity-first schema/path/length/SHA-256 routing;
- typed `OPERATOR_ROI_REQUIRED`, `AFFINE_CORRESPONDENCE_REQUIRED`, and
  `UPSTREAM_WAIT_PROPAGATED` precedence;
- pixel/intensity-only calibration boundaries and rejection of
  `PIXELPERMM=0` as an unknown sentinel;
- explicit owner/value requirements for
  `FIXTURE_MIN_VALID_PIXEL_RATIO` (the guide's `0.25` is not a global default);
- image-observation versus operator-policy ownership;
- intent/projection isolation, including no dark-band starter parameters in a
  bright-facing-edge plan; and
- strict `Main -> SourceFrame` baseline compatibility without weakening the
  validator.

No new public reason code, hidden case-ID mapping, algorithm family, product
execution, activation, qualification, corpus identity, or benchmark run was
added.

## Resource evidence

| Resource | Bytes | SHA-256 |
| --- | ---: | --- |
| `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\SKILL.md` | 38,453 | `D4663C554F4A25F4351D787AD96EB1EAB2317734B4E3DEB63BB3FBA4D508E7E0` |
| `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\references\recipe-xml-handoff-contract.md` | 20,497 | `44DDF68604BCEAF061DC3CF897A82756E721EA846DA7BD034764FD6551BA9DF3` |
| `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\validate_recipe_xml_handoff.py` | 75,786 | `03537E3F64B646783A3CEED5D4AE3C78531312A0F67FE03D19DC6602F7111279` |
| `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\test_recipe_xml_handoff.py` | 48,845 | `09AB58B0262F81C7FA0266CF1D6E728AE453C4FE4EF7C708F1E3CEBDE903B147` |
| `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\agents\openai.yaml` (preserved) | 350 | `E66F0729CD38076507B72A8FD8772DAD925796C6FBA5C01B7E10DFF8783B4C9A` |

## Verification

| Check | Command | Result |
| --- | --- | --- |
| Focused regression | `python C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\test_recipe_xml_handoff.py` | `Ran 27 tests`; `OK` |
| Python compilation | `python -m py_compile C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\validate_recipe_xml_handoff.py C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\test_recipe_xml_handoff.py` | exit `0` |
| Skill Creator validation | `python -X utf8 C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff` | `Skill is valid!` |

The default-encoding quick-validation invocation used the Windows `cp949`
default and could not decode the UTF-8 skill text; the required UTF-8 rerun
passed. No source or product build was needed for this documentation/script
candidate-only change.

## Acceptance record

Status: **Complete**  
Scope: installed `openvisionlab-recipe-xml-handoff` candidate-only correction,
version synchronization, focused tests, and evidence record.  
Acceptance criteria: candidate resources report `0.1.11`; explicit-only policy
is preserved; new integrity/typed-WAIT/owner-provenance/strict-baseline rules
are present in both the skill and handoff contract; 27 focused tests, Python
compilation, and Skill Creator validation pass.  
Verification: commands and results listed above.  
Evidence: this report and the synchronized registry entry in
`docs/contracts/openvisionlab/OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json`.  
Boundary / next dependency: v0.1.10 Round 1 evidence remains immutable and
historical; the public correction/corpus documents remain Draft pending the
operator's explicit contract decision, and a new strict corpus freeze plus
fresh benchmark admission are required before any rerun, activation, or
qualification.
