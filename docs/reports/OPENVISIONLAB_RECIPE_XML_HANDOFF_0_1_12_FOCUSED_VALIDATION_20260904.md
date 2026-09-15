# OpenVisionLab Recipe XML Handoff 0.1.12 — Focused Validation

Date: 2026-09-04 KST  
Repository: `C:\Git\2D\Dev`  
Candidate: `openvisionlab-recipe-xml-handoff 0.1.12`  
Parent evidence: `docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_TRIAGE_20260904.md`  
Status: **Complete — candidate-only focused correction PASS**

## Scope and boundary

This checkpoint implements the minimum candidate-only correction derived from
the v0.1.11 Round 1 triage. It updates only the installed candidate resources,
the focused regression, and Dev governance records. It does not modify the
immutable v0.1.11 corpus or its Authors/Reviewers/scorer artifacts, run a new
benchmark, activate or dispatch the skill, launch OpenVisionLab, Import/
Preview/Run XML, mutate a Recipe or layer/routing state, qualify an inspection,
touch `C:\Git\2D\Original`, commit, push, release, or deploy.

The product remains a deterministic OpenCvSharp4 Rule-Based Recipe workbench at
RC/pre-production maturity. The candidate remains `candidate`,
`EXPLICIT_ONLY`, inactive, outside normal dispatch, unqualified, and not a
product default.

## Candidate resource synchronization

| Resource | Result | SHA-256 | Length (bytes) |
| --- | --- | --- | ---: |
| `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\SKILL.md` | `0.1.12`; C33–C38 provenance, owner, graph, parameter, reason, platform, capability, and baseline locks | `9F4B73607953B13B4DD6E024F3A1313A0C6767C96FCBC805C36D03A75B7E4843` | `43135` |
| `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\references\recipe-xml-handoff-contract.md` | `0.1.12` machine shape and public pattern contract synchronized | `CEFEDC0456875F9C61BF4DF9F32F70690FAF6AEB1B122DEC6B9130A836EEB70E` | `22721` |
| `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\validate_recipe_xml_handoff.py` | validator `0.1.12`; accepts `OPERATOR_LOCK` and applies operator-owner allowlist | `FC7EDCA73C037FA0E0B41E53D5DF44AAF9E8F1271A5E32C3198FC23DC01B0711` | `75867` |
| `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\scripts\test_recipe_xml_handoff.py` | 28 focused tests, including authority-owner regression | `7019E75554C813CF38A356F56E406A4EBCD8B5AE3FD78B0E81F1564BA3B15713` | `49717` |
| `C:\Users\USER\.codex\skills\openvisionlab-recipe-xml-handoff\agents\openai.yaml` | explicit-only policy preserved; unchanged from v0.1.11 | `E66F0729CD38076507B72A8FD8772DAD925796C6FBA5C01B7E10DFF8783B4C9A` | `350` |

The validator schema remains `openvisionlab-recipe-xml-handoff-v1`,
`qualification` remains forbidden to become true, and all product-action flags
remain false. The candidate patch does not change the installed teaching or
Matching skills.

## Correction coverage

The public contract and skill now explicitly cover the following evidence
patterns without case-ID branches:

- `OPERATOR_SELECTED` requires explicit operator authority/evidence; an
  `OBSERVED`/`INFERRED` observation cannot be upgraded.
- Authority-level `OPERATOR_LOCK` is retained for explicit locks such as
  `FIXTURE_MIN_VALID_PIXEL_RATIO`; `operator-lock.json` cannot prove unrelated
  threshold/ROI/area policy.
- An explicit one-mask shared Threshold graph uses one Threshold and
  `ALLOW_BRANCH_INPUT=true` on later consumers; conflicting locks fail closed.
- A prior-mask Blob retains explicit `USE_THRESHOLD=false`.
- Dark-band `USE_GAP_EDGE_PAIR`/Canny/GAP starters stay out of unrelated
  bright-facing-edge or pixel-only plans; `PIXELPERMM=0` is never unknown.
- Camera/lighting/PLC/I/O/MES/field-device/deployment requests route to
  `OUT_OF_SCOPE_PLATFORM_REQUEST` before capability classification.
- Missing capability routes to `WAIT_ALGORITHM_GAP`; explicit catalog bypass or
  invented semantic detectors route to `UNSUPPORTED_SEMANTIC_CLAIM`.
- Per-image locked-value replacement routes to
  `PER_IMAGE_OVERRIDE_FORBIDDEN` before graph review.
- Immutable-baseline explanations remain neutral and request-scoped while
  protected bytes/status/fields remain unchanged.

## Verification

| Check | Command/result | Evidence |
| --- | --- | --- |
| Focused candidate regression | `python scripts\\test_recipe_xml_handoff.py` — exit `0`, `Ran 28 tests`, `OK` | `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\recipe-xml-handoff-v012-focused-20260904\\focused-regression.txt`; SHA-256 `AD435C776BA52C4ADC07A1264A4FFD05EA0AFB25399DDE9B41D36B2DC2D491D7`; length `4470` |
| Skill Creator validation | Default Windows-codepage invocation first stopped on `UnicodeDecodeError`; rerun with `python -X utf8 C:\\Users\\USER\\.codex\\skills\\.system\\skill-creator\\scripts\\quick_validate.py C:\\Users\\USER\\.codex\\skills\\openvisionlab-recipe-xml-handoff` — exit `0`, `Skill is valid!` | `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\recipe-xml-handoff-v012-focused-20260904\\skill-creator-quick-validate.txt`; SHA-256 `6CF9E7F0D34CCD36DBD28F270CD28DD04277B2E1516BD4333CE7BB501D7B18D4`; length `17` |
| Python compilation | `python -m py_compile scripts\\validate_recipe_xml_handoff.py scripts\\test_recipe_xml_handoff.py` — exit `0` | `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\recipe-xml-handoff-v012-focused-20260904\\python-compile.txt`; SHA-256 `E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855`; length `0` |
| Registry/document JSON parse | Candidate registry and `docs/LLM_DOCUMENT_INDEX.json` parse successfully under UTF-8 Python; candidate version/evidence paths point to `0.1.12` | `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\recipe-xml-handoff-v012-focused-20260904\\registry-index-parse.txt`; `JSON=PASS registryVersion=0.1.12 routes=13` |
| Documentation index validation | `tools\\TestDocumentationIndex.ps1 -RepoRoot C:\\Git\\2D\\Dev` — `DocumentationIndex=PASS`, `IndexedPaths=155`, `Routes=13`, `RootRedirects=102` | `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\recipe-xml-handoff-v012-focused-20260904\\documentation-index-validation.txt`; SHA-256 `3389D3BC04CC8553B642022D565355D95050CB33743BCE0184F2F1A6CE921F13` |
| Explicit-only policy | `agents/openai.yaml` hash and `allow_implicit_invocation: false` retained | Resource hash above |
| Frozen-root boundary | No new Authors, Reviewer, audit, or scorer execution; v0.1.11 root remains immutable | v0.1.11 result and triage evidence |

## Lifecycle result

The candidate-only correction is focused-tested and valid. This does not prove
full-benchmark quality, runtime parity, human comparison, Recipe qualification,
or production readiness. The separate next gate is an operator-approved new
strict-corpus repair/freeze and Round 1 admission decision; no such admission
was performed here. The S2 timeout remains a separate backend/admission risk.

## Closure record

Status: **Complete**  
Scope: installed Recipe XML handoff candidate-only `0.1.12` correction, focused regression, Skill Creator validation, compilation, and Dev governance synchronization.  
Acceptance criteria: failure-derived public rules present; `OPERATOR_LOCK` accepted and tested; explicit-only/no-product-action boundary preserved; 28 focused tests, Skill Creator validation, and Python compilation pass; frozen v0.1.11 root and Original checkout unchanged — **met**.  
Verification: commands and D-drive evidence listed above.  
Evidence: candidate resource hashes, focused logs, triage report, correction contract, updated registry/index/handoff/work-contract/research-plan records.  
Boundary / next dependency: new strict corpus, benchmark admission/rerun, activation, runtime execution, qualification, release, deployment, commit, push, and Original-repository work remain separate and unapproved.

Recommended model: `gpt-5.6-terra`  
Reasoning effort: `high`
