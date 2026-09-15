# Rule-Based Skill — Independent Static XML Integration

Date: 2026-09-08 KST
Status: Complete
Scope: two independent general teaching to standalone XML authoring cases,
using general 1.0.2 and candidate 0.1.17 without skill/source changes.

The prior follow-up proved producer selection transfer through a single Mean
plan and a separate synthetic XML projection. This continuation adds actual
independent XML authoring for a shared Threshold mask with two Blob consumers,
and a dark-seal pixel-width LineDistance plan. It does not reopen the completed
version, stage-route or selection-binding gates.

## Outcomes

| Case | Preserved result | Validation |
| --- | --- | --- |
| Shared mask | One Threshold feeds two Blob Steps; fixed SourceFrame ROI/area pairs, internal threshold off and explicit later branch input | Actual compatibility and handoff PASS; MEASURE_ONLY |
| Dark seal | One LineDistance Step retains all 12 caller ROI/sampling/projection parameters; no gap/Canny starters, PIXELPERMM or acceptance | Actual compatibility and handoff PASS; MEASURE_ONLY |

Four fresh contexts handled the two producer requests and their two XML
requests independently. Each received the current skill and necessary input
artifacts, not expected answers or earlier test conclusions. The parent opened
both source images, reviewed the produced plans for static projection and
retained exact producer bytes before dispatching XML work. Input authority was
synthetic caller-owned fixture policy, never real human approval or measured
inspection truth. All runtime stages/gates remain NOT_REVIEWED.

The shared-mask producer uses the documented v1 Tool plan with detailed policy
in purpose/observation/review fields. Optional per-parameter OPERATOR admission
does not map USE_THRESHOLD from the existing supplied lock kinds. The producer
preserved its actual caller ownership without inventing a lock or relabeling it;
the XML author consumed the retained plan. This result does not prove a new
typed lock profile or arbitrary prose-to-policy validation. The independent
comparison explicitly checks both USE_THRESHOLD=false values and branch/ROI/
area association in the generated XML. No validator was weakened.

Example: the right Blob consumes the same Threshold output as the left Blob,
not the left Blob output, and carries ALLOW_BRANCH_INPUT=true. Its own ROI and
area range remain separate. Pixel-only LineDistance omits physical calibration
instead of supplying a guessed PIXELPERMM value.

## Acceptance and evidence

1. Source/caller inputs and checker identity retained: source-manifest.json,
   input-manifest.json, before/, support/ and runtime-manifest.json.
2. Two fresh producer plans preserve intent, ownership, frames and unknowns:
   producer/, frozen-producer/ and the parent comparison.
3. Two fresh XML outputs preserve the plans and pass real static checks:
   xml/<case>/candidate.pipeline.xml, static-compatibility.txt, handoff.json,
   handoff-validator-output.json and raw-response.md.
4. Required comparison and durable tracking: integration-verdict.json
   (69 assertions), parent-execution.json, registry-validation.log,
   documentation-index.log, work-contract.json and final-evidence.json.

Evidence root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-integration-20260908`.
Source HEAD: d875559577c85984d54900df973a6fb35fb20146, existing dirty Dev checkout.
Independent review found two omissions in the parent comparison: exact validator
target identity and three explicit shared-mask preservation flags. Both were
added. A focused parent console pass retained actual exits and XML/report hash
bindings separately; all author files remained unchanged. This is additional
verification of the two cases, not a new author or full regression campaign.
The current checker source was copied to D: and built with 0 warnings/errors.
Each exact candidate XML passed RecipeXmlCompatibilityCheck through console
reflection/serialization against the 190 identified existing Dev Debug DLLs.
Those assemblies were not rebuilt from the current dirty source. Each case
report names 13 serialization roots and one recipe XML file; the empty-folder
preflight alone is not candidate validation. All temporary/test files are on D:.
No desktop window was launched, so monitor placement does not apply.

Commands actually used, with full paths, exit codes and retained reports, are
in execution-record.json and each XML author's raw-response.md. Principal calls:

```text
dotnet build RecipeXmlCompatibilityCheck.csproj -c Release --nologo
dotnet <retained checker.dll> <identified Dev Debug assembly directory> <case XML directory>
python -X utf8 -B <candidate>/scripts/validate_recipe_xml_handoff.py <case>/handoff.json --json --authority-manifest <caller authority.json>
python -X utf8 -B <evidence root>/verify_integration.py
python -X utf8 -B <matching>/scripts/validate_skill_registry.py <registry> <skills root> <Dev> --json
powershell -NoProfile -ExecutionPolicy Bypass -File <Dev>/tools/TestDocumentationIndex.ps1 -RepoRoot <Dev>
```

Boundary / next dependency: this is synthetic static authoring/compatibility
evidence, not image execution, detection accuracy, physical metrology, UI or
field qualification. No full benchmark identity/driver/bundle contract was
created. Full benchmarking remains user-deferred and requires a separate
coordinated interval/admission decision. The XML candidate remains explicit-only,
inactive and unqualified. Skill format/regression gates from follow-up 5 were
not rerun because skill bytes are unchanged. Product build/UI/Preview/Run,
Matching normalization, Original, commit/push/release and activation were not
performed. This scoped integration is closed; the broader gate is not implied.
