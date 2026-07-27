# OpenVisionLab Next Session Handoff (Chronological Detail)

Updated: 2026-07-24 KST

> **Current continuation source:** Read `docs\OPENVISIONLAB_CURRENT_HANDOFF.md` and `docs\OPENVISIONLAB_DOCUMENTATION_MAP.md` before using this file. They hold current status, evidence-based maturity, and the active priority.
>
> This document is the detailed chronological evidence log. It preserves prior decisions, P-number records, diagnostics, and artifact paths, but older entries must not be read as current git state, current UI evidence, current release readiness, or an active priority without confirming the current handoff and source.

Work starts in `C:\Git\OpenVisionLab_Dev`; only reviewed and stabilized changes are imported into the original repo at `C:\Git\OpenVisionLab`. Do not run `git push` unless the user explicitly requests `PUSH`.

## Read First

- Read `docs\OPENVISIONLAB_CURRENT_HANDOFF.md` for current status and `docs\OPENVISIONLAB_DOCUMENTATION_MAP.md` for authority/read order.
- Read `docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md` for the final product shape and view responsibilities, then `docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md` before changing behavior.
- Use this chronological log only to locate detailed evidence after the current source of truth identifies the relevant P-number or artifact.

## Numbering Continuity Notes

- `P5` appears in
  `docs\OPENVISIONLAB_BEGINNER_LEARN_MODE_AND_RECIPE_CONTEXT_20260701.md` as
  the roadmap label `Tool Preset Expansion`, not as a separately closed
  chronological execution record. Do not invent a P5 completion artifact.
- P143-P145 were completed and documented in the current handoff but were
  missing from this chronology; their evidence summaries are restored below
  before P146/P147.
- No durable `P194` task/evidence record exists in the repository. Numbering
  moves from the P192/P193 hybrid candidate to P195. Preserve the gap rather
  than fabricating a feature or completion claim.

## P105 Current-EXE Novice Workflow Audit On 2026-07-17

- With no new natural GPT/Gemini/Claude correction transcript supplied, the bounded fallback was a current Debug EXE audit of the public fixture and Recipe Manager novice routes.
- Fresh pre-change EXE evidence is under `artifacts\p105_novice_workflow_audit_20260717_214517\before_public_fixture` and `artifacts\p105_novice_workflow_audit_20260717_214517\before_roundtrip`. The `public-fixture-review` and `recipe-pipeline-roundtrip` reports both passed.
- The audit found one actionable 1040x700 Learn-window issue: Blob's expanded repeated `Practice workflow` pushed the related Tool View row below the viewport, leaving `Blob Tool Open` only partially visible.
- `OpenVisionLearnWindow.xaml.cs` now starts the repeated workflow collapsed for Blob, consistent with the existing dense Matching-family topics. The underlying instructions remain available by explicit expand; no sample open, Tool View open, Preview/Run, layer, route, recipe value, or review-state behavior changed.
- `OpenVisionLabDirectSmokeRunner.cs` now asserts that Blob begins in this compact state while retaining its explicit-open/no-side-effect checks.
- After a zero-warning/zero-error Debug rebuild, both actual-EXE scenarios passed again. The corrected Blob Learn evidence is `artifacts\p105_novice_workflow_audit_20260717_214517\after_public_fixture\public_fixture_blob_learn_current_exe.png`; the rebuilt Recipe Manager route evidence is under `artifacts\p105_novice_workflow_audit_20260717_214517\after_roundtrip`.
- Readiness, external-reference, and public-sample checks passed after the code change. `git diff --check` must be interpreted from the final post-handoff-update command result in the current handoff rather than this historical entry.

## P106 Actual GPT Blob XML Direct-Success Trial On 2026-07-17

- The user authorized transfer of only the public project-authored Blob nominal/sparse PNG pair and signed in to ChatGPT. A new chat was created inside the user's `룰베이스 LLM 연동` project, and the actual GPT first response was collected from that new chat.
- The first response took 1m 2s and returned XML only. It is preserved at `artifacts\p106_gpt_blob_correction_loop_20260717\gpt_first_response.xml`; the packet provenance and exact current-application evidence are in the sibling `README.md`.
- After a fresh 0-warning/0-error Debug build, the current application `llm-xml-draft-file` smoke passed with the raw response: local validation/import succeeded and the nominal public image passed the Blob `ResultCount` gate at 12. Evidence: `artifacts\p106_gpt_blob_correction_loop_20260717\first_response_nominal\report.txt`.
- The unmodified XML then ran against the sparse public negative. `llm-xml-image-run --expect-run-success false` passed because the inspection correctly returned NG: `ResultCount=3 < 8`. Evidence: `artifacts\p106_gpt_blob_correction_loop_20260717\first_response_sparse_negative\report.txt`.
- This is real provider and current-application direct-success evidence, not a correction loop. No XML validation/import/run failure occurred for the first response, so no artificial error or correction prompt was sent. The expected sparse-sample NG is product evidence, not an LLM authoring failure.
- Next priority remains a naturally failing first provider response followed by the exact local failure evidence and an actual corrected response. Do not reduce the evidence bar or fabricate a correction round merely because P106 passed first try.

## P107 Actual GPT Natural-Prompt Blob Direct-Success Trial On 2026-07-17

- The user separately approved a second ChatGPT request in a new conversation in the `룰베이스 LLM 연동` project. Only the two public, project-authored synthetic Blob PNGs were transferred. Unlike P106, the prompt did not prescribe tool XML parameters: it asked for a rule-based particle-count `VisionPipeline` that passes the nominal image and rejects the sparse image. The exact prompt and provenance are recorded in `artifacts\p107_gpt_blob_natural_prompt_20260717\README.md`.
- ChatGPT reported 5m 7s processing time and returned XML only. The captured first response is `artifacts\p107_gpt_blob_natural_prompt_20260717\gpt_first_response.xml`; exact provider model/version, API evidence, and full provider export remain unknown.
- After a fresh zero-warning/zero-error Debug build, current-application `llm-xml-draft-file` passed with that raw response: validation/import succeeded and the nominal sample passed at `ResultCount=12` against the generated exact `12..12` gate. Evidence: `artifacts\p107_gpt_blob_natural_prompt_20260717\first_response_nominal\report.txt`.
- The unmodified XML then ran on the sparse public negative. `llm-xml-image-run --expect-run-success false` passed because the inspection correctly returned NG: `ResultCount=3 < 12`. Evidence: `artifacts\p107_gpt_blob_natural_prompt_20260717\first_response_sparse_negative\report.txt`.
- P107 is a real natural-authoring direct-success trial, not a correction loop. No XML validation/import/Good-Bad execution failure occurred in its first response, so no correction prompt was sent. Do not reinterpret the intended sparse-sample NG as an authoring failure.
- Next priority remains a natural initial provider failure followed by exact local failure evidence and an actual corrected response. Do not fabricate a failure or lower the evidence standard because P106 and P107 both passed directly.

## P108 Actual GPT Natural-Prompt RotateScale Direct-Success Trial On 2026-07-17

- The user authorized continued public-sample GPT validation in the `룰베이스 LLM 연동` project. A new project chat received only the public nominal/wide RotateScale PNG pair and a natural-language request for 50-percent full-image resize with a result-width acceptance gate.
- GPT reported 1m 39s processing time and returned XML only. The raw response is `artifacts\p108_gpt_rotate_scale_natural_prompt_20260717\gpt_first_response.xml`; provider model/version, API evidence, and a full provider export remain unknown.
- After a fresh zero-warning/zero-error Debug build, validation/import and nominal execution passed at `ResultImageWidth=286`. The same XML then gave the wide negative the intended `ResultImageWidth=320 > 286` NG. Evidence: the `first_response_nominal` and `first_response_wide_negative` reports in the P108 artifact folder.
- P108 is real direct-success evidence, not a correction loop. No correction request was sent.

## P109 Actual GPT Natural-Prompt Matching Trial And Smoke-Runner Finding On 2026-07-17

- The user authorized a new GPT project conversation with only the public Matching die-pad template, nominal image, and no-target image. The natural request required one Matching Step, three nominal matches, a no-target reject, and a public template dependency, without prescribing XML parameters or paths. GPT reported 2m 41s and returned XML only; `gpt_first_response.xml` preserves it unchanged.
- The raw first response validated/imported and copied both template dependencies, but the initial nominal run reported a missing template. That raw report was sent to GPT in the same conversation; the real correction response `gpt_correction_response.xml` changed the paths from `..\..\docs\...` to `docs\...`, then correctly failed static dependency validation.
- Read-only source tracing found the initial trigger was a Smoke Runner defect: `llm-xml-draft-file` imported a dependency-rewritten pipeline but reloaded the raw XML for its image execution. `OpenVisionLabDirectSmokeRunner.cs` now loads the selected imported pipeline. After a fresh zero-warning/zero-error Debug build, the unchanged first response passed nominal import/run at `ResultCount=3`; its imported no-target run returned the intended `ResultCount=0 < 3` NG. Evidence: `first_response_nominal_after_runner_fix\report.txt` and `first_response_negative_after_runner_fix\report.txt`.
- P109 includes a real first response, a real harness-produced report, and a real correction response, but it is not valid LLM correction-loop evidence or an LLM authoring failure. The report sent to GPT was confounded by the harness defect, so the correction response must not be scored as an improvement or used for model-quality claims.

## P110 Actual GPT Natural-Prompt Pin-Gap Correction Attempt On 2026-07-17

- The user authorized continued public-sample GPT validation in the `룰베이스 LLM 연동` project. A new project conversation received only `Line_Pins_Synthetic_OK.png` and `Line_Pins_Synthetic_WidePin_NG.png`. The natural request required a full adjacent-pin-array `LineDistance` pipeline with average and explicit outlier-range gates.
- GPT reported 7m 34s and returned `gpt_first_response.xml`. It validated/imported, passed the nominal image, and gave the wide-gap image the intended NG, but it supplied only four executable Steps (Gap 1/2 Average and Range) and left Gaps 3-7 as a literal placeholder comment.
- The literal missing Step coverage is a real user-intent failure, not an expected Good/Bad result. Two correction attempts ended in provider `thinking failed`; a final concise retry returned actual `gpt_correction_response.xml` after GPT reported 5m 39s.
- The correction is structurally complete: 14 executable `LineDistance` Steps, Average plus `DistanceMmRange` for each of all seven gaps, without a placeholder. Current Debug validation/import also succeeded. However, the required nominal replay failed at Step 12 (`Pin Gap 6 Range`): `DistanceMmRange=0.024 > 0.02`. Evidence: `artifacts\p110_gpt_pin_gap_natural_prompt_20260717\corrected_nominal\report.txt`.
- P110 is valid real-provider correction-attempt evidence but is not a successful correction loop: the corrected XML did not preserve nominal acceptance. Preserve both raw XML files and the exact report; do not describe P110 as correction success or rerun a correction request without fresh user authorization.

## P111 Actual GPT Pin-Gap Gate-Repair Correction Success On 2026-07-18

- The user authorized the continuation of the same P110 GPT project conversation. No new images or private data were transmitted; the request supplied the exact current-Debug nominal error from the first 14-Step correction: `12 Pin Gap 6 Range` rejected `DistanceMmRange=0.024 > 0.02`.
- The actual XML-only response is `artifacts\p111_gpt_pin_gap_gate_repair_20260718\gpt_second_correction_response.xml`. It preserves all 14 executable LineDistance Steps and seven Average/Range pairs without a placeholder. The only XML change is the affected `AcceptanceMetricMaximum` from `0.02` to `0.03`.
- A fresh 0-warning/0-error Debug build preceded replay. `llm-xml-draft-file` validated/imported the unchanged P111 response and passed the nominal sample through `14 Pin Gap 7 Range`; Gap 6 accepted `DistanceMmRange=0.024`. Evidence: `nominal\report.txt`.
- `llm-xml-image-run --expect-run-success false` passed on the public wide-pin negative: expected product NG occurred at `01 Pin Gap 1 Average`, `DistanceMmAvg=0.116 < 0.14`. Evidence: `wide_negative_expected_ng\report.txt`.
- P111 is real correction-loop success evidence for this public synthetic workflow: first-response full-array scope failure, first correction nominal failure, exact feedback, second correction, and current-Debug nominal/NG replay. It must not be generalized into a provider reliability or production-quality claim.

## P112 Independent GPT HSV Natural-Authoring Direct Success On 2026-07-18

- The user authorized continued public-sample GPT validation. A new project conversation in `룰베이스 LLM 연동`, independent from P111, received only `HSV_ColorPatch_Synthetic_OK.png` and `HSV_ColorPatch_Synthetic_Missing_NG.png`. The natural-language request asked for one complete HSV `VisionPipeline` with a measurable `MaskPixelRatio` gate; it supplied no XML, private image, API key, or local source.
- GPT reported 2m 12s processing time and returned XML only. The raw first response is `artifacts\p112_gpt_hsv_natural_prompt_20260718\gpt_first_response.xml`; exact provider model/version, API evidence, and a complete provider export remain unknown.
- After a fresh zero-warning/zero-error Debug build, the current application `llm-xml-draft-file` validated/imported that unmodified XML and accepted the nominal public sample at `MaskPixelRatio=0.058`. Evidence: `artifacts\p112_gpt_hsv_natural_prompt_20260718\nominal_first\report.txt`.
- The unmodified XML then ran on the public missing-target negative. `llm-xml-image-run --expect-run-success false` passed because the inspection correctly returned NG: `MaskPixelRatio=0.015 < 0.05`. Evidence: `artifacts\p112_gpt_hsv_natural_prompt_20260718\missing_negative_expected_ng\report.txt`.
- P112 is independent real direct-success evidence in the HSV tool family, not a correction loop. No correction message was sent because the first response had no XML/import/nominal/intended-negative failure. Do not treat the expected negative NG as a model failure or manufacture a correction round.

## P113 Independent GPT FeatureMatching Natural-Authoring Direct Success On 2026-07-18

- The user authorized continued public-sample GPT validation. A new project conversation in `룰베이스 LLM 연동`, independent from P111/P112, received only `Feature_Card_Synthetic_OK.png`, `Feature_Card_Synthetic_Wrong_NG.png`, and `templates\Feature_Card_Synthetic_Template.png`. The natural-language request asked for one FeatureMatching `VisionPipeline` with a reference-template dependency and a measurable `ScoreMax` gate; it supplied no XML, private image, API key, or local source.
- GPT reported 2m 2s processing time and returned XML only. The raw first response is `artifacts\p113_gpt_feature_matching_natural_prompt_20260718\gpt_first_response.xml`; exact provider model/version, API evidence, and a complete provider export remain unknown.
- On the same fresh current Debug build used for P112, `llm-xml-draft-file` validated/imported the unchanged XML, relocated both template references into the generated smoke recipe, and accepted the nominal public sample at `ScoreMax=96.7`. P114 then added expected-NG support to this imported-pipeline route; final nominal evidence is `artifacts\p114_imported_expected_ng_runner_20260718\p113_nominal\report.txt`.
- The same import route replayed the wrong-card public image and returned the intended NG, `ScoreMax=26.7 < 80`. With P114 `--expect-run-success false`, it now reports PASS with `ExpectedRunSuccess=False` and `ActualRunSuccess=False`. Final evidence: `artifacts\p114_imported_expected_ng_runner_20260718\p113_wrong_expected_ng\report.txt`.
- The generic `llm-xml-image-run --expect-run-success false` path does not first import/rewrite the raw relative template dependency and instead returns template-not-loaded. That is a runner-path limitation, not a GPT authoring failure or correction trigger; no such report was sent to GPT. P113 is direct-success evidence, not a correction loop.

## P114 Imported LLM-Draft Expected-NG Smoke Support On 2026-07-18

- P113 showed that template-dependent LLM evidence needs import-path execution: `llm-xml-draft-file` imports/re-writes dependencies, but it previously reported any intended image NG as a smoke failure. The generic expected-NG smoke does not import/re-write relative template paths, so it cannot provide semantic Good/Bad proof for a raw template-dependent draft.
- `OpenVisionLabDirectSmokeRunner.cs` now accepts `--expect-run-success true|false` for `llm-xml-draft-file` (default `true`). With an image it records expected/actual run success and accepts a matching expected NG only after the existing validation/import path succeeds. No product UI, XML contract, recipe, layer, routing, Preview, or Run behavior changed.
- After a fresh zero-warning/zero-error Debug build, the raw P113 GPT XML passed import-path nominal replay at `ScoreMax=96.7` (`ExpectedRunSuccess=True`, `ActualRunSuccess=True`) and passed import-path wrong-card expected-NG replay at `ScoreMax=26.7 < 80` (`ExpectedRunSuccess=False`, `ActualRunSuccess=False`). Evidence: `artifacts\p114_imported_expected_ng_runner_20260718\README.md`.

## P115 Independent GPT EdgeBasedMatching Natural-Authoring Direct Success On 2026-07-18

- The user authorized continued public-sample GPT validation. A new project conversation in `룰베이스 LLM 연동`, independent from P111-P113, received only `Edge_Fiducial_Synthetic_OK.png`, `Edge_Fiducial_Synthetic_Wrong_NG.png`, and `templates\Edge_Fiducial_Synthetic_Template.png`. The natural-language request asked for one EdgeBasedMatching `VisionPipeline` with a reference-template dependency and a measurable `ScoreMax` gate; it supplied no XML, private image, API key, or local source.
- GPT reported 2m 7s processing time and returned XML only. The raw first response is `artifacts\p115_gpt_edge_based_natural_prompt_20260718\gpt_first_response.xml`; exact provider model/version, API evidence, and a complete provider export remain unknown.
- After a fresh zero-warning/zero-error Debug build, `llm-xml-draft-file` validated/imported the unchanged XML, relocated both template references into the generated smoke recipe, and accepted the nominal public sample at `ScoreMax=99.598`. Evidence: `nominal_after_import\report.txt`.
- The same imported-pipeline route replayed the wrong-fiducial public image with `--expect-run-success false` and returned the intended NG: no result met `SCORE_MIN=0.70`, and `BestScore=61.052`. It recorded `ExpectedRunSuccess=False`, `ActualRunSuccess=False`, and `Result: PASS`. Evidence: `wrong_expected_ng_after_import\report.txt`.
- P115 is independent real direct-success evidence in the EdgeBasedMatching tool family, not a correction loop. No correction message was sent because the first response had no XML/import/nominal/intended-negative failure. Do not treat the expected negative as a model failure or manufacture a correction round.

## P116 Real GPT Fixture Pad Correction-Loop Success On 2026-07-18

- The user authorized continued public-sample GPT validation. A new project conversation in `룰베이스 LLM 연동` received only `Fixture_Pad_Synthetic_Shifted_OK.png`, `Fixture_Pad_Synthetic_Shifted_Missing_NG.png`, and `templates\Fixture_Locator_Synthetic_Template.png`. The natural request required locator-based part-relative pad inspection and a merged review result; it supplied no XML, private image, API key, or local source.
- GPT reported 2m 7s for the raw first response. `gpt_first_response.xml` referenced missing `Locator_Template.png`, omitted the requested fixture-frame behavior, and used static pad ROI. Current Debug validation/import failed before execution with two missing dependency reports. This is a real initial XML/intent failure. Evidence: `first_nominal\report.txt`.
- The exact public failure and fixture-intent evidence was sent back in the same conversation. GPT reported 3m 24s for `gpt_correction_response.xml`, which supplies the fixture-aware three-Step public workflow. That correction exactly matches the tracked public reference XML text, but still failed current Debug LLM import because `docs\...` relative dependencies resolve from `bin\Debug`. It is retained as real failed intermediate correction evidence, not success.
- A second actual feedback request supplied that current-Debug dependency-path failure. `gpt_second_correction_response.xml` changed only both template references to `..\..\docs\samples\public\templates\Fixture_Locator_Synthetic_Template.png`; raw-response comparison confirms those are its only XML differences from the first correction.
- Current Debug validation/import and nominal replay then passed: Matching reported fixture offset `(80,55)`, Blob used effective ROI `(400,235)`, and OverlayMerge accepted two source overlays. Evidence: `second_correction_nominal\report.txt`.
- The unchanged second correction passed `--expect-run-success false` on the shifted missing-pad public image. Matching accepted the locator and Blob returned intended NG at `ResultCount=0 < 1`. Evidence: `second_correction_missing_expected_ng\report.txt`.
- P116 is real GPT correction-loop success evidence for this current-Debug public fixture workflow. The final relative template path is layout-specific and the first correction's text duplication of the tracked public reference must not be presented as portable deployment evidence, independent tool selection, provider reliability, or production quality.

## P117 GPT Filter Denoise Project-Chat Direct Success On 2026-07-18

- A new user-authorized GPT project chat received only public `Filter_Denoise_Synthetic_OK.png` and `Filter_Denoise_Synthetic_Missing_NG.png`. The natural request required a sequential MedianBlur -> binary Threshold -> Contour-count workflow that accepts the four-target nominal and rejects the two-target missing sample. It supplied no XML, source, private asset, API key, or template dependency.
- GPT reported `3m 4s` processing. The project-chat UI visibly recorded project-file-library/repository XML retrieval before response, so preserve this as project-chat validation-path evidence rather than independent tool selection. Exact provider model/version, API evidence, and a complete provider export remain unknown.
- The actual first XML-only response was copied using the provider's `응답 복사` action and saved unchanged at `artifacts\p117_gpt_filter_denoise_natural_prompt_20260718\gpt_first_response.xml`; its saved text matches the copied response character-for-character. No correction message was sent.
- After a fresh Debug 0-warning/0-error build, `llm-xml-draft-file` validated/imported the first response and passed the nominal: three Filter/Threshold/Contour Steps accepted and the final Contour reported `ResultCount=4`. The same raw response passed `--expect-run-success false` on the public missing sample: the final Contour returned intended `ResultCount=2 < 4` NG. Evidence: `first_nominal\report.txt` and `first_missing_expected_ng\report.txt`.
- Canonical XML equals the tracked `Public_Filter_Denoise.pipeline.xml` after normalizing only pipeline name `Filter_Denoise_Inspection -> Public_Filter_Denoise` and the Contour `Name` value `GPT_Filter_Denoise -> Public_Filter_Denoise`. Do not count P117 as a correction loop, independent authoring success, provider benchmark, portable deployment proof, or production-quality result. Full package notes: `artifacts\p117_gpt_filter_denoise_natural_prompt_20260718\README.md`.

## P118 GPT Morphology Ellipse Recovery-Correction Evidence On 2026-07-18

- A user-authorized GPT project chat received only the public `Morphology_Cleanup_Synthetic_OK.png` and `Morphology_Cleanup_Synthetic_Missing_NG.png` images. The natural request required Threshold -> Morphology Open with `Shape=Ellipse` -> Contour count, nominal four-target acceptance, missing-target NG rejection, and no project/repository XML retrieval. It supplied no XML, source, private asset, API key, template path, or hardware data.
- The actual first raw response is `artifacts\p118_gpt_morphology_ellipse_natural_prompt_20260718\gpt_first_response.txt`. It used custom `InputLayers`/`OutputLayers`/`BinaryThreshold`/`ConnectedComponents`/`AcceptanceGate` nodes. The fresh current-Debug `llm-xml-draft-file` validation failed before import with `ValidationOk=False`, `ImportEnabled=False`, `Imported=False`, and 16 schema errors, including missing Step Name, ToolType, InputLayer, and OutputLayer. This is an actual provider XML-schema failure, not an expected sample NG.
- The exact validation report and repair rules were sent in the same conversation (`gpt_correction_prompt.txt`), but the provider UI stayed loading for about five minutes and returned no correction text. Preserve this as an unreceived provider UI/hang observation; do not score it as a model correction failure.
- A new recovery project conversation then received the exact failed response and current-Debug report (`gpt_retry_correction_prompt.txt`). Its completed rendered XML response is stored in `gpt_retry_correction_response.xml`; storage adds only one terminal newline because the provider copy action yielded no browser clipboard payload. The response uses the required Threshold/Morphology/Contour Step schema, `Shape=Ellipse`, `Operator=Open`, and a Contour ResultCount minimum/maximum of four.
- After the fresh 0-warning/0-error Debug build, `llm-xml-draft-file` validation/import and nominal replay passed: `ResultCount=4` at final `CountLargeTargets`. The unchanged recovery response also passed `--expect-run-success false` on the public missing image, returning the intended `ResultCount=2 < 4` NG with smoke `Result: PASS`. Evidence: `retry_correction_nominal\report.txt`, `retry_correction_missing_expected_ng\report.txt`, and package `README.md`.
- P118 is real GPT recovery-correction evidence across two conversations: actual first response, actual local failed validation, actual recovery correction response, and current-Debug nominal/NG replay. It is not same-conversation correction-loop proof, independent authoring proof, provider reliability evidence, or production-quality evidence.

## P119 GPT Arithmetic/Mean Same-Conversation Correction Evidence On 2026-07-18

- A new user-authorized GPT project chat received only the public `Arithmetic_Invert_Synthetic_OK.png` and `Arithmetic_Invert_Synthetic_Bright_NG.png` images. The natural request required sequential `Arithmetic` Bitwise NOT -> `Mean`, distinct output layers, nominal acceptance after inversion, bright-image NG rejection, and no project/repository XML retrieval. It supplied no XML, source, private asset, API key, template dependency, or hardware data.
- The actual initial rendered XML at `artifacts\p119_gpt_arithmetic_mean_same_chat_20260718\gpt_first_response.xml` used custom `Layers`/`ImageLayer`/`BitwiseInvert`/`AverageBrightness`/`AcceptanceGate` nodes. Fresh current-Debug validation failed before import: `ValidationOk=False`, `ImportEnabled=False`, `Imported=False`, 12 child-Step-contract errors, and no image run. This is an actual provider schema failure, not an expected Good/Bad NG.
- The actual failure report was sent back in the same provider conversation with `gpt_correction_prompt.txt`. The actual first repair, `gpt_correction_response.xml`, validated/imported and accepted the nominal image at `MeanValueAvg=208`, but its `MeanValueAvg=190,230` Parameter did not make an acceptance gate. The public bright image then incorrectly passed at `MeanValueAvg=76.7`, yielding `ExpectedRunSuccess=False` and `ActualRunSuccess=True`. Preserve this intermediate result as a genuine provider repair defect, not a harness failure.
- That exact current-Debug NG replay was sent in the same conversation through `gpt_second_correction_prompt.txt`. The actual second repair, `gpt_second_correction_response.xml`, retained Arithmetic Bitwise NOT -> Mean and added the required Step-level `UseAcceptance`, `AcceptanceMetricName=MeanValueAvg`, minimum 190, and maximum 230 fields.
- On the fresh 0-warning/0-error Debug build, the second repair validated/imported and passed nominal at `MeanValueAvg=208`. The unchanged second repair also passed `--expect-run-success false` on the public bright image: its final Mean step returned intended NG `MeanValueAvg=76.7 < 190`, with `ExpectedRunSuccess=False`, `ActualRunSuccess=False`, and smoke `Result: PASS`. Evidence: `second_correction_nominal\report.txt` and `second_correction_bright_expected_ng\report.txt`.
- P119 is real GPT same-conversation correction-loop evidence: actual initial response, actual local failure, actual first repair that failed a Good/Bad acceptance replay, actual second repair, and fresh Debug nominal/NG replay. Provider model/version, API transcript, and full provider export remain unknown. Browser-rendered capture adds only a terminal newline. It is not general reliability, independent authoring, deployment portability, or production-quality evidence. Read `artifacts\p119_gpt_arithmetic_mean_same_chat_20260718\README.md` first for the complete provenance.

## P120 Gemini Arithmetic/Mean Same-Conversation Correction Evidence On 2026-07-18

- A user-authorized logged-in Gemini chat received only the public `Arithmetic_Invert_Synthetic_OK.png` and `Arithmetic_Invert_Synthetic_Bright_NG.png` images. The in-app browser cannot use native file-picker upload and clipboard image attachments both receive the provider name `clipboard.png`; consequently the nominal image was sent first with an explicit no-XML staging request, then the bright image plus the actual XML request were sent in the immediately following user turn. The staging exchange is preserved in `gemini_nominal_staging_prompt.txt`; it introduces no XML, source, private asset, API key, template dependency, or hardware data.
- The actual initial rendered XML at `artifacts\p120_gemini_arithmetic_mean_same_chat_20260718\gemini_first_response.xml` used `Layers`/`Layer`/`Workflow` and attribute-Step XML. Fresh current-Debug validation failed before import: `ValidationOk=False`, `ImportEnabled=False`, `Imported=False`, `Pipeline has no steps`, and no image run. This is an actual Gemini provider schema failure, not an expected Good/Bad NG.
- The exact failure report and bounded child-Step repair contract were sent in the same Gemini conversation through `gemini_correction_prompt.txt`. The actual correction response, `gemini_correction_response.xml`, uses Arithmetic Bitwise NOT from `Main` to `InvertedOutputLayer`, then Mean to `MeanOutputLayer`, and a Step-level `MeanValueAvg` range of 190 through 230.
- On the fresh 0-warning/0-error Debug build, the correction validated/imported and passed nominal at `MeanValueAvg=208`. The unchanged correction also passed `--expect-run-success false` on the public bright image: final Mean returned intended NG `MeanValueAvg=76.7 < 190`, with `ExpectedRunSuccess=False`, `ActualRunSuccess=False`, and smoke `Result: PASS`. Evidence: `correction_nominal\report.txt` and `correction_bright_expected_ng\report.txt`.
- P120 is real Gemini same-conversation correction-loop evidence: actual initial response, actual local failure, actual same-conversation correction response, and fresh Debug nominal/NG replay. The correction request supplied exact Step XML and measured public range, so it proves this correction path rather than independent tool selection or general provider reliability. Visible mode label was `Gemini Pro`; exact model/version, API transcript, and full provider export remain unknown. It is not deployment-portability or production-quality evidence. Read `artifacts\p120_gemini_arithmetic_mean_same_chat_20260718\README.md` first for full provenance.

## P121 LLM Assistant Failure Next-Action Visibility On 2026-07-18

- Claude validation was deliberately deferred by the user, so P121 followed the no-transcript fallback and audited the latest Debug EXE Recipe Manager/LLM XML route. The focused Pin-gap intent contract correctly blocked a Threshold/Contour draft, but the first visible `LLM 초안 검증: NG` panel showed only its general summary; `Error: Intent contract mismatch` and the required `LineDistance` next action appeared below the first viewport.
- `OpenVisionShellHostView.xaml` gives the first LLM validation/dependency row more review height. The lower draft review, diff, stored validation, and issue-list panels remain unchanged in the scrollable technical-review surface. `OpenVisionRecipeLlmDraftValidationService` preserves every validation rule but now appends intent-contract error/tool-type/next-action lines before generic result-channel explanation.
- Fresh Debug build completed with 0 warnings/0 errors. Current EXE before/after proof: `focused_exe_llm_intent_skills\OpenVisionLab_RecipeManager_LlmIntentSkills_PinGapContourMismatch.png` versus `after_final_exe_llm_intent_skills\OpenVisionLab_RecipeManager_LlmIntentSkills_PinGapContourMismatch.png`. The after capture shows `Error: Intent contract mismatch`, `Draft enabled ToolTypes`, and `Next: Use ToolType=LineDistance...` together without scrolling.
- Current Debug EXE `recipe-manager-llm-intent-skills` passed, including intent blocks, mismatch blocking, dependency review, correction bundle, and unchanged Preview/Run count. Full current Debug EXE `recipe-manager-tabs` also passed, retaining normal LLM XML validation/import review, invalid-draft blocking, corrected draft import, and explicit Preview/Run/no-routing-side-effect contracts. Normal after LLM capture: `after_final_exe_recipe_manager_tabs\OpenVisionLab_RecipeManager_LlmXml.png`.
- P121 changes visible report priority only. It does not change LLM provider evidence, XML parsing, validation outcomes, import readiness, Preview/Run, layers, routing, or recipe persistence. Read `artifacts\p121_llm_assistant_ux_audit_20260718\README.md` first for complete evidence. Do not restart Claude work until the user explicitly resumes it.

## P122 Local SourceSteps Branch-Comparison Evidence On 2026-07-18

- The user identified the ignored local `Sample` directory as a source of industrial comparison images. Under the public-sample policy, P122 used its pin pair only for a local non-public diagnostic: no source image was copied into artifacts, a public catalog, documentation capture, GitHub-bound file, or any LLM/provider prompt.
- A five-Step local pipeline ran the two pin inputs through Threshold, Morphology, two Contour branches, and `OverlayMerge` restricted by `SourceSteps=03 Pin Top Contour;04 Pin Bottom Contour`. Fresh runner replays both completed `Steps=5/5` with `MergeSourceCount=2` and `MergeOverlayCount=2`.
- The first current-EXE Recipe Manager branch review imported that XML but showed `SourceConsumerRelationsVisible: 0/2` and `OverlaySourceProducersVisible: 0/2`. It therefore demonstrated a real missing review relationship: runtime/validation support `SourceSteps`, but the preview DTO and branch/output Presenter only resolved `SourceLayers`.
- `OpenVisionRecipePipelineStepPreview` now parses `SourceSteps`; `OpenVisionRecipePipelineStepReviewPresenter` treats Step-name references like output-layer references for branch exclusion, producer/consumer, review-merge, and overlay-source rows. Fresh after EXE evidence reports 2/2 for both relation directions without Preview or active-layer changes. Current before/after EXE captures: `artifacts\p122_local_sourcesteps_pin_branch_20260718\before_exe_sourcesteps\OpenVisionLab_PipelineReview_SourceConsumer.png` and `after_exe_sourcesteps\OpenVisionLab_PipelineReview_SourceConsumer.png`.
- Existing public `SourceLayers` behavior remains verified at 2/2 with `BentPin_TopBottom_Overlay.pipeline.xml`. Fresh Debug build had 0 warnings/0 errors; `recipe-manager-tabs`, readiness, vendored DLL, public-sample, and diff checks passed. Read `artifacts\p122_local_sourcesteps_pin_branch_20260718\README.md` before using or publishing any part of this diagnostic.

## P123 Gemini LineDistance Same-Conversation Correction Attempt On 2026-07-18

- The user authorized the logged-in Gemini chat to receive only `docs\samples\public\Line_Pins_Synthetic_OK.png` and `Line_Pins_Synthetic_WidePin_NG.png`. The nominal public synthetic image was first sent with an explicit no-XML staging request, then the Wide-Pin image and the exact LineDistance XML request were sent in the immediately following turn. No ignored root `Sample` asset, source file, XML, credential, template path, or hardware information was transferred.
- Gemini's actual initial rendered response is preserved as `artifacts\p123_gemini_line_distance_same_chat_20260718\gemini_first_response.xml`. It used a custom `Configuration` and `PipelineLayers`/`Layer` schema rather than OpenVisionLab child `Steps`/`Step` XML.
- Fresh current-Debug local validation of the exact response failed before import or execution: `ValidationOk=False`, `ImportEnabled=False`, `Imported=False`, `ImageRun: SKIPPED`, and `Pipeline has no steps.` Evidence: `first_nominal\report.txt`.
- The exact failure and a bounded child-Step repair contract were returned in the same Gemini conversation through `gemini_correction_prompt.txt`. The initial repair and two visible `다시 실행` retries ended as `대답이 중지되었습니다.` without XML. A concise new same-conversation repair request, sent without any image transfer, remained text-empty in the visible generating state for 80 seconds and was explicitly stopped; it then displayed the same stopped state. At user direction the visible model selector was changed from Gemini Pro to 3.5 Flash, then the same correction request was retried without new data; it also displayed `대답이 중지되었습니다.` within 15 seconds. `gemini_correction_attempts.md` records all five actual correction-generation attempts.
- **Status: Blocked** — P123 is not a correction-loop success: no corrected XML, validation/import, nominal pass, or Wide-Pin expected-NG replay exists. The missing external prerequisite is an actual Gemini correction response in the same conversation. No XML was synthesized or repaired locally; when a response becomes available, validate its exact content on the current Debug build.

## P124 LLM Template-Draft Builder Responsibility Extraction On 2026-07-18

- **Status: Complete** — the source-boundary audit found one cohesive pure responsibility inside `OpenVisionShellHostRecipeCommandSurface`: default `VisionPipeline` construction for the selected LLM starter template. It selected LineDistance, Blob, Contour, EdgeBasedMatching, Mean, or Matching and set only deterministic pipeline/default parameters; the Host supplied three input values and then separately owned prompt text, XML assignment, validation, import readiness, commands, and visible state.
- `OpenVisionRecipeLlmTemplateDraftBuilder` now owns that construction in `Wpf\Recipe\IntentSkills`. The Host method delegates `SelectedLlmToolTemplate`, `LlmReferenceImagePath`, and `PinGapIntentRoiText`; the old direct construction and its private draft-step/reference helper coupling are removed from Host. No XML schema, parameters, validation rules, import, Preview/Run, layer route, or visual behavior was intentionally changed.
- The focused direct-smoke scenario now calls the new builder for all six starter families. It checks LineDistance plus OverlayMerge, Threshold -> Blob route, Contour `ResultCount` acceptance, EdgeBasedMatching, Mean, and Matching `TemplatePath`/`PATTERN_PATH` values before running its existing Recipe Manager intent-skill contracts.
- Fresh Debug build passed with 0 warnings/0 errors. Fresh current-EXE `recipe-manager-llm-intent-skills` passed with `TemplateDraftBuilder: LineDistance, Blob, Contour, EdgeBasedMatching, Mean, and Matching starters verified` and `PreviewRunCountUnchanged: 0`; readiness and diff checks passed. Full record: `artifacts\p124_llm_template_draft_builder_20260718\README.md`.

## P125 GPT LineDistance Same-Conversation Correction Evidence On 2026-07-18

- P125 used the user-authorized GPT web project chat, not an API. It transferred only the public `Line_Pins_Synthetic_OK.png` and `Line_Pins_Synthetic_WidePin_NG.png` files: the nominal image was staged first, then the Wide-Pin image and XML-only LineDistance request were sent. No root `Sample` asset, source file, project XML, credential, template path, calibration data, or hardware data was transferred.
- The first rendered response arrived after the visible provider UI showed a 3-minute-57-second processing interval. Its exact `VisionPipeline` XML is `artifacts\p125_gpt_line_distance_phase1_20260718\gpt_first_response.xml`. Fresh current-Debug validation/import and nominal replay passed, but its unsupported `MinValue`/`MaxValue` acceptance fields meant Wide-Pin incorrectly passed at `DistancePxAvg=18.417`; the expected-NG smoke therefore failed. This is a real Good/Bad acceptance failure, not a harness failure.
- The exact local result and current child acceptance-element contract were returned in the same web conversation through `gpt_correction_prompt.txt`. The one actual repair arrived after a displayed 3-minute-12-second interval and is preserved in `gpt_correction_response.xml`. It uses `UseAcceptanceMetricMinimum/Maximum`, accepts nominal at `DistancePxAvg=28.667` and `DistancePxMax=56`, and sends Wide-Pin to the intended NG at `DistancePxAvg=18.417 < 24`.
- Fresh current-Debug `llm-xml-draft-file` replays passed for both the repaired nominal and repaired expected-NG cases. P125 is real same-conversation GPT correction-loop evidence for one public synthetic pixel-only LineDistance workflow. The visible mode was `Pro`; exact model/version and full provider export remain unknown. It is not an API, provider-reliability, calibration, independent-authoring, field, or production claim. Full provenance: `artifacts\p125_gpt_line_distance_phase1_20260718\README.md`.

## No-API Browser-Assist Product Decision On 2026-07-18

- The user selected web-account assistance rather than an API route: OpenVisionLab should prepare the constrained prompt/review packet, the operator should use their own provider web account, and the returned XML should enter the existing local validation/import and explicit-run route.
- A free account may be used only subject to the provider's own login, availability, model, rate-limit, and file-upload restrictions. OpenVisionLab must not store a credential, bypass limits, or automate a logged-in provider page. An embedded WebView is optional after a compatibility proof and needs an external-browser fallback.
- This decision changes the next implementation priority to the smallest explicit Browser Assist handoff slice. It does not add an API key, a server, automatic provider input/output, automatic import, Preview, or Run.

## P126 No-API Browser Assist First Slice On 2026-07-18

- P126 added the Recipe Manager Advanced Review `웹 보조` tab with explicit `ChatGPT 열기`, `외부 브라우저`, `프롬프트 복사`, and `XML 붙여넣기` actions. It uses `Microsoft.Web.WebView2` with a transient host profile; OpenVisionLab does not collect or configure a provider credential, and does not read or manage the provider's web-session data. Default external-browser opening remains available if the embedded host is unavailable.
- Fresh current-Debug `recipe-manager-llm-intent-skills` selected the Browser Assist tab, asserted that the explicit open/copy/paste controls were visible while the WebView was not visible, then clicked `ChatGPT 열기`. The embedded WebView navigated to `https://chatgpt.com/` and the smoke restored the placeholder for its UI capture. No login, upload, prompt send, response scraping, XML import, Preview, or Run occurred; `PreviewRunCountUnchanged: 0` passed. Full record: `artifacts\p126_browser_assist_20260718\README.md`.

## P127 Phase 2 Public LineDistance Operator Path On 2026-07-18

- P127 selected measurement as the first Phase 2 path because it has a public Good/Bad pair and explicit average-plus-range acceptance contract. Fresh current-Debug `recipe-manager-tabs` passed both MM-READY and PX-ONLY Guided Setup modes, blocked malformed mm/px input, and kept starter creation separate from execution.
- The public `Line_Pins_Synthetic_OK.png` passed in both modes; `Line_Pins_Synthetic_WidePin_NG.png` rejected in both. The report records `DistanceMmAvg=0.224`, `DistancePxAvg=37.263`, and parity. Recipe Manager Good/Bad review, failed-Step focus, and explicit PropertyGrid apply also remained present. No source change was justified by this verification. Full record: `artifacts\p127_phase2_line_distance_operator_path_20260718\README.md`.

## P128 Phase 2 Public Blob Count Operator Path On 2026-07-18

- P128 selected Threshold + Blob as the second Phase 2 path. Fresh current-Debug Recipe Manager validation/import accepted the public pipeline. Explicit nominal replay passed with `ResultCount=12`; the public sparse negative returned the intended `02 Synthetic Particle Count` NG at `ResultCount=3 < 8`. The report retains the `Blob_Binary -> Blob_Preview` route and Blob area metrics.
- The separate `learn-blob-practice` smoke opened the public Blob lesson and related `BlobToolWpfView` without running Preview or creating a layer (`PreviewRunCount=0`, `LayerCount=0`). No product change was justified. Full record: `artifacts\p128_phase2_blob_operator_path_20260718\README.md`.

## Re-ranked Next Priority After P123 (Historical)

- Claude remains deliberately deferred. Do not open a Claude chat or transmit a sample until the user explicitly resumes it.
- P123 already produced a real Gemini Pin/LineDistance first-response schema failure, but five same-conversation correction-generation attempts produced no XML, including a concise no-image request that stayed text-empty for 80 seconds and a user-directed 3.5 Flash retry that stopped within 15 seconds. The immediate provider-evidence condition is an actual correction response in that existing conversation; do not resend an asset, manufacture a response, or claim a correction loop before it validates/imports and replays nominal/Wide-Pin results.
- Claude remains deliberately deferred. Do not use the ignored root `Sample` assets for any provider prompt. If Gemini later returns a correction, retain the exact first-failure and repaired-response evidence with fresh current-Debug Good/Bad replay.
- P124 closed the one currently demonstrated pure Host cleanup boundary. Do not start another source split without a similarly cohesive owner and a focused verification path.

## Latest Recipe Manager Responsibility Split On 2026-07-15

- User review identified that Recipe Manager had become a second Pipeline editor: Pipeline, XML, LLM, validation, history, reports, and guided actions competed at the same level and obscured the first task.
- The corrected ownership is now explicit: Tool View configures one algorithm, Pipeline owns Step order/layer routing/acceptance/explicit Preview-Run, Pipeline Review owns run evidence, Recipe is the reusable package, and Recipe Manager owns library/lifecycle/entry-point work.
- Recipe Manager now opens on a compact selected-recipe summary with active pipeline, current work sample, recipe-specific current check result, and one explicit `Open Pipeline` action.
- Existing Guided Setup, Pipeline, LLM XML, history, report, validation-set, import/export, and detailed review functions are preserved behind an explicit `Advanced review` switch. Closing/reopening returns to the summary.
- `Open Pipeline` reuses the existing Pipeline Review surface. The screenshot smoke asserts that opening it does not run Preview/Run, create a result, or change layer routing.
- The change is intentionally a responsibility-first UI slice, not a destructive migration. The next evidence gate is the real novice route `select recipe -> open Pipeline -> explicit review/run -> return`; move more detailed functions only when that route proves a concrete duplication.
- Current-source before/after evidence is stored under `artifacts\recipe_manager_responsibility_split_20260715`.
- Focused current-source smokes passed for the summary, Guided Setup, operator decision board, local Validation Set, and broad language/CRUD/import/export/history flow with `layout=0`, `text=0`, and `internal=0`.
- The full solution build passed with zero warnings/errors. Readiness, external-reference, public-sample (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), and `git diff --check` passed.
- Latest EXE proof used `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, built at `2026-07-15 09:38:59 KST`. Direct `recipe-manager-tabs` passed under `artifacts\recipe_manager_responsibility_split_20260715\direct_exe_verified` and now checks the default summary before opening advanced review.

## Latest Matching Fixture Operator Workflow Slice On 2026-07-14

- The commercial-gap reassessment keeps intended-workbench maturity at about 62-66% and selects fixture/coordinate-frame handling as the first major missing workflow.
- `docs\OPENVISIONLAB_MATCHING_FIXTURE_WORKFLOW_SPEC.md` defines the first bounded contract: one `Matching` result, `NUM_MATCH=1`, one named frame, and one downstream axis-aligned `CvROI` translated on the same source layer.
- `VisionPipelineFixtureFrameService` publishes the reviewed Matching center/angle, computes X/Y offset, clones the consumer step, and changes only the clone's effective `CvROI`. Saved XML and input routing remain unchanged.
- Translation-only v1 rejects excessive angle change, missing/duplicate frames, different source layers, missing ROI, multi-ROI, masks, and out-of-image effective ROI instead of applying a partial or misleading transform.
- Metrics now include `FixtureCenterX/Y`, `FixtureAngle`, `FixtureOffsetX/Y`, `FixtureAngleDelta`, and effective ROI X/Y.
- `tools\OpenVisionFixtureSmoke` proves reference OK, translated-without-fixture NG, translated-with-fixture OK, `(70,40)` offset recovery, and unchanged saved `CvROI=170,80,50,50`.
- Recipe Manager now exposes the Matching producer fields and the first Blob consumer fields through the existing PropertyGrid. `VisionPipelineStepPropertyMapper` preserves the fixture keys through load/apply while keeping `CvROI` and layer routing unchanged.
- Korean localization labels the workflow as `기준 좌표`; no separate settings window or code-behind workflow was added.
- `wpf_shell_host_recipe_fixture_properties` verifies the producer/consumer descriptors, XML parameter round trip, visible PropertyGrid rows, and unchanged Preview/Run count. Current-source before/after evidence is under `artifacts\fixture_property_grid_roundtrip_20260714`.
- The public pair `Public_Fixture_Pad_Good` / `Public_Fixture_Pad_Missing_Bad` now proves a shifted `(80,55)` locator, effective Blob ROI `(400,235)`, Good OK, and locator-present/pad-missing Bad NG with the same pipeline.
- The same shifted Good image fails when Fixture use is explicitly disabled, and the saved consumer `CvROI=320,180,60,50` remains unchanged.
- Pipeline Review now shows the selected consumer's read-only runtime evidence as `Fixture Delta X,Y | ROI X,Y`; this does not run Preview or change recipe values.
- Current-source sample-picker before/after and Pipeline Review evidence is under `artifacts\public_fixture_sample_20260714`. Latest EXE proof is `latest_exe\public_fixture_pipeline_review_current_exe.png` plus `report.txt`.
- Public catalog verification passed 30/30 runnable rows; public asset policy passed with `CatalogRows=30`, `ManifestAssets=229`, and `Pipelines=15`.
- Pipeline Review now exposes `참조 자세 저장` only for a fixture-producing Matching Step after an explicit successful Review. It copies the reviewed `FixtureCenterX/Y/Angle` into the three reference parameters and persists the active pipeline.
- Saving the reference immediately invalidates the prior review result and asks for another explicit Review. It does not launch Preview/Run, create or select a layer, change input/output routing, or rewrite any consumer parameter including `CvROI`.
- `wpf_shell_host_workspace_sample_fixture_teach` executes the command and asserts `120,100,0 -> 200,155,0`, unchanged consumer parameters/routes/layers/native Preview count, and the post-save `run review required` state. Current-source before/after evidence is under `artifacts\fixture_pose_teach_20260714`.
- Fresh solution and screenshot-tool builds passed with zero warnings/errors. Fixture runtime, localization catalog, readiness, external-reference, public-sample (`30/229/15`), and `git diff --check` passed. The latest Debug EXE timestamp is 2026-07-14 12:47:51 KST; direct `recipe-manager-tabs` smoke passed under `artifacts\fixture_pose_teach_20260714\direct_exe`.
- This remains translation-only. The next evidence gate is a real operator pass on the chosen reference image to determine whether explicit consumer-ROI reteaching guidance is sufficient. Do not auto-rewrite consumer ROI, and do not add rotation/scale compensation without a real failing sample.

## Latest Tool Rail Readiness Slice On 2026-07-14

- P2 Tool Finder/readiness started with the smallest verified operator-visible state instead of adding a second tool browser.
- The existing 15 image-processing/algorithm Tool rail items now show `입력 없음` when `Main` has no image and `설정 가능` after a `Main` image is loaded. Pipeline remains a `흐름` surface rather than pretending to be an image Tool View.
- `설정 가능` means only that the PropertyGrid tool can be opened and configured. It does not claim that template, second input, ROI, calibration, or other tool-specific Preview prerequisites are complete.
- The readiness state reuses `OpenVisionShellNavItem` and the existing Main-layer refresh path. It does not add a new window, disable tool selection, open a tool, run Preview/Run, create a layer, or change routing.
- Compact Tool rail remains icon-only and clickable; its existing tooltip now carries the current readiness description.
- Current-source before/after captures are under `artifacts\tool_readiness_20260714\before` and `artifacts\tool_readiness_20260714\after`. Focused smoke passed for empty workspace, image-loaded workspace, and compact Tool rail with `layout=0`, `text=0`, and `internal=0`.
- A fresh solution build passed with zero warnings/errors. The latest Debug EXE timestamp is 2026-07-14 13:32:26 KST; direct `workspace-startup-empty` smoke passed under `artifacts\tool_readiness_20260714\direct_exe` and shows the new empty-input badges.
- Localization catalog, readiness, external-reference, public-sample (`30/229/15`), and `git diff --check` passed.
- `Matching` now adds the first real tool-specific prerequisite state: when `Main` is ready but its existing `MatchingProperty` has no loaded template, only the Matching row shows `템플릿 필요`. Registering or clearing the template through the PropertyGrid persistence path refreshes the row immediately.
- The transition smoke proves `템플릿 필요 -> 설정 가능 -> 템플릿 필요` without increasing the native Preview/Run count. Tool selection, layer state, and routing remain unchanged by readiness refresh itself.
- Fresh current-source before/after evidence is under `artifacts\matching_template_readiness_20260714\before` and `artifacts\matching_template_readiness_20260714\after`. Empty workspace, image-loaded workspace, and compact Tool rail regression captures passed with `layout=0`, `text=0`, and `internal=0`.
- The solution build passed with zero warnings/errors. The latest Debug EXE is `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-14 13:55:56 KST`. Readiness, external-reference, public-sample (`30/229/15`), and `git diff --check` passed.
- The same template prerequisite now covers `Matching`, `EdgeBasedMatching`, and `FeatureMatching`. Each row reads the first recipe-owned PropertyGrid property and shows `템플릿 필요` until its path exists and the template image loads successfully.
- EdgeBasedMatching and FeatureMatching registration/clear transitions are exercised through the shared property-save notification without opening a Tool View or increasing Preview/Run count. The smoke resets its three template properties first so repeated runs cannot inherit prior smoke configuration.
- Fresh current-source before/after evidence is under `artifacts\matching_family_template_readiness_20260714\before` and `artifacts\matching_family_template_readiness_20260714\after`; empty workspace, image-loaded workspace, and compact Tool rail regression passed with `layout=0`, `text=0`, and `internal=0`.
- Focused and full solution builds passed with zero warnings/errors. The latest Debug EXE is `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-14 14:08:51 KST`.
- Arithmetic now reuses the execution contract for `Operation`/`Offset`, `Bitwise_NOT`/`ABS`, and constant-input behavior. With only one non-placeholder image layer, a setting that needs B shows `B 입력 필요`; unary, constant, and Offset settings remain `설정 가능`.
- The settings-save and layer-refresh paths update this badge without opening Arithmetic or increasing Preview/Run count. Adding a B image changes the row to `설정 가능`; deleting it restores `B 입력 필요`. The state remains advisory and does not claim that selected routes or image sizes are compatible.
- Fresh current-source before/after evidence is under `artifacts\arithmetic_second_input_readiness_20260714\before` and `artifacts\arithmetic_second_input_readiness_20260714\final`. The image-load, empty-workspace, and compact Tool rail smokes passed with `layout=0`, `text=0`, and `internal=0`.
- Focused and full solution builds passed with zero warnings/errors. The latest Debug EXE is `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-14 14:33:15 KST`.
- The image-ready Threshold, Matching, and Line quick actions now reuse `TopSecondaryCommandButtonStyle`; their text and icons inherit the button foreground so the dark background, hover, and pressed states remain readable without a second palette.
- The workspace image-load smoke now enforces text/background contrast of at least 3.0 for all three quick actions while retaining the existing no-automatic-tool/Preview/Run assertions.
- Fresh current-source before/after evidence is under `artifacts\quick_tool_button_contrast_20260714\before` and `artifacts\quick_tool_button_contrast_20260714\after_visible`. Focused and full solution builds passed with zero warnings/errors. The latest Debug EXE is `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-14 14:50:56 KST`.
- Line now reads the first recipe-owned Line A/B `PIXELPERMM` values and distinguishes `px 전용`, matching positive scale such as `mm 0.006`, and `보정 확인` for missing, invalid, negative, or inconsistent values.
- A matching positive value means only that the configured A/B scale agrees; the tooltip explicitly requires real calibration evidence before mm results are trusted. `px 전용` remains a valid pixel-measurement mode and must not make a physical-unit claim.
- The transition smoke proves `px 전용 -> 보정 확인 -> mm 0.006` through existing PropertyGrid persistence notifications without opening Line, increasing Preview/Run count, creating a layer, or changing routing. Empty-workspace and compact Tool rail regressions also passed with `layout=0`, `text=0`, and `internal=0`.
- Fresh current-source before/after evidence is under `artifacts\line_scale_readiness_20260714\before` and `artifacts\line_scale_readiness_20260714\final_visible`. Focused and full solution builds passed with zero warnings/errors. The latest Debug EXE is `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-14 15:21:47 KST`.
- Required-ROI audit found no truthful generic Tool rail badge target. Blob and Contour already support full-image direct Preview, Line initializes an empty ROI to the full image only when the operator explicitly runs Preview, and Matching-family Preview is blocked by template readiness rather than ROI. The one real required-ROI case is a fixture-consuming pipeline Step, which already fails closed in `VisionPipelineFixtureFrameService` validation. Do not add a misleading generic `ROI needed` badge.
- The expanded Tool rail now has one inline search box that filters the existing items by all whitespace-separated terms. Canonical bilingual metadata covers tool names, inspection intents such as pin gap and defect count, PropertyGrid terms such as `InputLayerB`, and result metrics such as `DistanceMmRange` and `ScoreMax`.
- Search changes item/group visibility only. It preserves selected/readiness state and is smoke-proven not to open tools, run Preview/Run, create layers, change the visible workspace layer, or change input/output routing. The compact icon rail hides the search row and remains clickable.
- Fresh current-source before/after evidence is under `artifacts\tool_finder_search_20260714\before` and `artifacts\tool_finder_search_20260714\after`. The dedicated search, image-load readiness, and compact Tool rail targets passed with `layout=0`, `text=0`, and `internal=0`.
- Found tools now expose a book button only while search is active. All 16 Tool rail menus map to an existing canonical Learn topic, and the command opens or reuses the Learn window at that topic without selecting a Tool View or changing Preview/Run, layer, workspace, or route state.
- Fresh current-source before/after evidence for the Learn shortcut is under `artifacts\tool_finder_learn_link_20260714\before` and `artifacts\tool_finder_learn_link_20260714\after`; the dedicated target also verifies all tool-topic mappings and Learn-window reuse.
- Found tools now also expose an image button that opens the existing Sample Picker at the mapped Learn path. All 16 Tool rail menus have explicit path assertions; the Line search action opens `line` with visible public samples, and cancelling the Picker preserves Preview/Run, layer, workspace, route, and Tool View state.
- Fresh current-source before/after evidence for the sample shortcut is under `artifacts\tool_finder_sample_link_20260714\before` and `artifacts\tool_finder_sample_link_20260714\after`. The same folder contains image-load, compact-rail, and Sample Picker regression captures.
- Found tools now expose a Guided Setup button only for the five existing starter-intent contracts: Line pin gap/pitch, Blob count, Contour shape/count, Matching target presence, and Mean brightness. The action opens Recipe Manager at the existing Guided Setup tab with the mapped intent selected; it does not create Starter XML or change Tool View, Preview/Run, layer, workspace, or route state. The other 11 tools expose no such button.
- Search mode hides the readiness badge so Learn, public sample, and Guided Setup actions remain inside the 220 px Tool rail. The smoke now checks the Guided Setup button's actual bounds and verifies an unsupported Threshold result has no button.
- Fresh current-source before/after evidence is under `artifacts\tool_finder_guided_setup_link_20260714\before` and `artifacts\tool_finder_guided_setup_link_20260714\after`; image-load, compact-rail, and existing Guided Setup regressions are under the same folder's `regression` directory. Build, localization (`1695/77`), readiness, external-reference, and public-sample (`30/229/15`) checks passed. The latest Debug EXE is `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-14 16:41:46 KST`, newer than the latest touched app source.
- P2 Tool Finder is complete at the bounded search/readiness/direct-link scope. Do not add favorites or recommendation scoring until usage evidence shows search is insufficient. P3 has started with the first explicit Recipe review bundle; the next P3 slice is import-side dry validation and path-relocation review using its manifest.

## Latest Recipe Review Bundle Slice On 2026-07-14

- Recipe Manager now exposes `Review bundle` separately from normal XML export and writes a `.review.zip` package.
- Schema v1 contains exactly `pipeline.xml` and `review-manifest.json`. The manifest records application/schema version, XML size/SHA-256, validator errors/warnings, ToolType and enabled counts, Step routes, acceptance gates, pipeline dependency metadata, and selected sample/reference metadata.
- Dependency/reference rows record source/path kind, resolved existence, size, SHA-256, and read errors. The package records `ReferencedOnly`; it does not copy referenced files or private local assets.
- Export is review-only. It does not import, run Preview/Run, create/select a layer, open a Tool View, or change workspace/input/output routing.
- Focused smoke `wpf_shell_host_recipe_review_bundle` opens the current Recipe Manager, exports and reopens the ZIP, verifies its two-entry boundary and hashes/metrics/policy, then checks no runtime or route state changed. Current-source before/final evidence is under `artifacts\recipe_review_bundle_20260714`.
- Regression exposed a real rename defect: Tool readiness could reload against the previous RecipeContext during recipe-change callbacks and recreate the old workspace after `Directory.Move`. `OnRuntimeRecipeChanged` now refreshes RecipeContext before nested layer/readiness refreshes; the broad Recipe Manager language/CRUD/import/export/history smoke passes through rename without retaining the old recipe.
- The run-history baseline ComboBox now carries its existing localized label as automation metadata, preserving keyboard-input identification without adding a visible UI or audio-validation feature.
- Verification passed: solution and screenshot-tool builds reported 0 warnings/0 errors; focused review-bundle, Guided Setup, LLM dependency, operator decision-board, and broad Recipe Manager language/CRUD/history smokes passed with `layout=0`, `text=0`, and `internal=0`; localization was `1695/77`; readiness, external-reference, public-sample (`30/229/15`), and `git diff --check` passed.
- Latest EXE proof used `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll`, built at `2026-07-14 17:27:42 KST`, with `recipe-manager-tabs` reporting `Result: PASS` under `artifacts\recipe_review_bundle_20260714\direct_exe`.
- Import-side dry validation is now implemented. `.review.zip` selection verifies the exact two-entry schema, bounded sizes, package policy, XML size/SHA-256, summary counts, and manifest/XML dependency consistency before loading the XML into the existing `LLM XML` review tab.
- A missing absolute dependency can show one SHA-matched file beside the bundle as `재배치 후보`; OpenVisionLab does not rewrite the XML path or copy the file. Validation remains NG and `Import` remains disabled until the operator explicitly corrects the XML and validates again.
- Tampered XML is rejected before draft exposure. Dependency content changed since export is also blocked from copy/import.
- `wpf_shell_host_recipe_review_bundle_import` proves tamper rejection, relocation evidence, disabled import, and unchanged pipeline/Preview/Run/layer/workspace/routing state. Fresh before/final evidence is under `artifacts\recipe_review_bundle_import_20260714`.
- The latest Debug EXE/DLL was built at `2026-07-14 18:29:07 KST`; direct `recipe-manager-tabs` reports `Result: PASS` under `artifacts\recipe_review_bundle_import_20260714\direct_exe_final`.
- P3 is complete at the reference-only review/handoff scope. Optional redistributable-asset copy stays deferred and must never silently include private/local assets.
- P4 local Validation Sets now have their first executable slice: named recipe-local explicit image lists, expected OK/NG, per-image notes, missing-file blocking, explicit suite execution, and existing run-history reuse. Metadata lives under `VISION\ValidationSets`, outside pipeline enumeration.
- Registration/editing has no Preview/Run, layer, workspace, Tool View, or routing side effects. Focused current-source smoke is `wpf_shell_host_recipe_local_validation_set`; latest Debug EXE `recipe-manager-tabs` also verifies the actual controls and no-side-effect registration.
- Final current-source evidence is `artifacts\local_validation_sets_20260714\final_render\wpf_shell_host_recipe_local_validation_set.png`. Final latest-build EXE evidence and report are under `artifacts\local_validation_sets_20260714\final_exe`; the built EXE/DLL timestamp is `2026-07-14 19:37:58 KST`.
- Verification passed with 0 build warnings/errors, focused local-set smoke `layout=0|text=0|internal=0`, direct EXE `Result: PASS`, readiness, external-reference policy, public-sample policy (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), and `git diff --check`.
- Pipeline inventory pollution found in that current-source evidence is fixed. `RecipeWorkspaceService.GetVisionPipelineNames` now lists only exact no-namespace `VisionPipeline` XML roots while preserving tool-state, active-name, malformed, and unrelated files unchanged.
- Fresh before/after evidence is under `artifacts\pipeline_inventory_filter_20260714\before` and `after_retry`. The latest EXE/DLL was rebuilt at `2026-07-14 19:54:03 KST`; `artifacts\pipeline_inventory_filter_20260714\final_exe\report.txt` is `Result: PASS` and records `PipelineInventory: valid VisionPipeline XML only`.
- P4 top-level folder registration is implemented with explicit OK/NG roles. It ignores unsupported files, excludes subfolders, updates duplicate paths deterministically, and preserves the existing no-side-effect registration contract.
- Current-source before/after evidence is under `artifacts\validation_set_folder_registration_20260715\before` and `after`; the focused smoke reports `layout=0|text=0|internal=0`.
- The full solution build passed with 0 warnings/errors. Latest Debug EXE `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe` was built at `2026-07-15 06:28:56 KST`; `recipe-manager-tabs` reports `Result: PASS` under `artifacts\validation_set_folder_registration_20260715\direct_exe` and records top-level folder filtering plus unchanged Preview/Run, layers, and routing.
- Readiness, external-reference policy, public-sample policy (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), and `git diff --check` passed after the folder slice.
- P4 missing-path repair now applies only to one selected missing row and one operator-selected existing supported image. Duplicate replacement paths are rejected; expected OK/NG and notes are preserved; no recursive or inferred search occurs.
- Current-source before/after evidence is under `artifacts\validation_set_path_repair_20260715\before` and `after_screen`; the focused smoke covers duplicate rejection, metadata preservation, suite re-enable, no runtime side effects, and `layout=0|text=0|internal=0`.
- The first focused run exposed a too-short smoke-only suite wait at progress `2/4`; its wait ceiling was increased without changing product runtime behavior, and repeated focused runs passed.
- Latest Debug EXE `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe` was built at `2026-07-15 07:30:54 KST`. Direct `recipe-manager-tabs` under `artifacts\validation_set_path_repair_20260715\direct_exe` reports `Result: PASS` and records file/folder/repair controls plus preserved metadata, Preview/Run, layers, and routing.
- P4 is complete at the bounded recipe-local Validation Set scope. Recursive search, inferred replacement, automatic path rewriting, and a second runner remain prohibited.

## Product Direction

- OpenVisionLab is an LLM-assisted OpenCvSharp4-based rule-based vision recipe workbench.
- Its purpose is image-based algorithm learning, verification, LLM-assisted XML recipe generation, and recipe composition.
- It is not a camera, lighting, PLC, or I/O integration platform.
- Algorithm tools must stay PropertyGrid-based.
- Preview/Run must be explicit user actions. Layer create/delete/load-image, visibility toggles, and output layer creation must not auto-run tools.
- Viewer zoom/pan/drag, ROI overlay, template editor, layer comparison, and docking features must be preserved.
- Main window title-bar minimize, maximize/restore, and close controls must remain visible and verified. These are window controls, not account/session UI.
- Smoke tests and UI screenshots must use the latest updated EXE or a current-source view generated after the latest relevant source changes. Do not show old artifact images as current UI evidence; label them as historical/baseline only.

## Current Dev Baseline On 2026-07-05

Recent Dev commits on `codex/public-sample-ux-docs` include:

- `487106f Show selected recipe step details`
- `646dce5 Add recipe step comparison grid`
- `8bea861a Explain failed recipe history samples`
- `3d5767ec Show recipe step parameter previews`
- `c1a16bb5 Split recipe review panel into tabs`
- `7e4cd81 Document OpenVisionLab target views`
- `e76a440 Show LLM XML validation issue rows`
- `53fbfc3d Add selected step layer navigation`
- `eeb47e69 Show Good Bad pair role cards`

Current Recipe Manager baseline:

- Searchable recipe list, create, duplicate, rename, delete, XML import/export are already present.
- Recipe Manager is now a workbench-sized overlay with a dedicated recipe library pane, review workspace header, and command strip. It is no longer treated as a small floating settings panel.
- Pipeline review is split into Review, Runs, and XML/Step sub-tabs.
- `Duplicate from sample`, LLM XML validation report, structured LLM XML validation issue rows, LLM XML before/after diff review, actionable dependency/path scan hints and dependency path drill-down rows, pipeline preview step list, Step comparison table, selected Step detail panel, selected Step input/output layer thumbnail cards with click navigation, selected Step ROI/template metadata, selected Step PropertyGrid parameter review with explicit XML apply-back and corrected-output review, branch/output comparison rows for selected multi-step correction paths, Good/Bad role result cards with failed-Step drill-down, failed Step rerun/comparison action strip, and failed-history explanation are already present.
- Top account/operator chrome has been reviewed and removed from Shell Host/Shell Preview. It was only an `Account` icon plus `OperatorText`, with no login/profile/permission command behind it. Keep operator review wording inside Recipe/Pipeline Manager, but do not bring back top-level account UI unless real account/session features are intentionally added.
- Do not re-spend the next session re-evaluating these from scratch unless a regression is reported.

Latest P4 recipe-variant comparison slice:

- Recipe Manager `Pipeline > Review` now shows a read-only comparison between the active pipeline and the selected pipeline variant.
- The report reuses the LLM draft diff engine and exposes step-count and dependency-path deltas, added/removed/changed steps, routing changes, and representative parameter before/after values.
- Selecting a variant still does not activate it and does not trigger Preview/Run. Direct EXE smoke asserts the preview/run counter remains unchanged.
- Before capture: `artifacts\p4_pipeline_variant_diff_before_20260711_01\OpenVisionLab_RecipeManager_PipelineFilter.png`.
- After capture from the current Debug EXE: `artifacts\p4_pipeline_variant_diff_after_20260711_04\OpenVisionLab_RecipeManager_PipelineVariantComparison.png`.
- Verification: `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors; direct EXE `recipe-manager-tabs` passed and records `PipelineVariantComparison: active/selected diff visible without Preview/Run`.
- `OpenVisionReadinessCheck`, `TestExternalReferences.ps1`, and `TestPublicSampleAssets.ps1` also passed; the public sample check reported `CatalogRows=28`, `ManifestAssets=226`, and `Pipelines=14`.
- P4 is not complete. The next bounded priority is to audit an explicit XML review-bundle/dependency-manifest export action that reuses the existing dependency scanner. Do not silently create sidecar files during normal XML export and do not build deployment packaging.

Latest sample catalog and Shell readability slice on 2026-07-12:

- Sample workflow action buttons now use a dark secondary command surface with high-contrast white text/icons; screenshot smoke checks the rendered text/background contrast.
- The default Shell Host bottom bar no longer shows a fake `C:` capacity meter. It mirrors the current recipe, workspace layer, selected tool/task, and operation status.
- `OpenVisionWorkspaceSamplePickerWindow` uses the shared custom OpenVisionLab title bar and keeps minimize, maximize/restore, close, drag, and resize behavior.
- Workspace and Recipe Manager sample selectors exclude `LocalLegacy`; the lower-level loader remains for old recipe/history compatibility.
- Learn document actions now convert all `docs\learn\*.md` files into linked, styled local HTML under `%LOCALAPPDATA%\OpenVisionLab\LearnHtml\v1` and open the selected `.html` in the default browser. Relative tutorial images resolve against the repository Learn source folder.
- Current-source before captures:
  - `artifacts\sample_catalog_shell_before_20260712_01\wpf_shell_host_workspace_sample_open.png`
  - `artifacts\sample_catalog_picker_before_20260712_01\wpf_shell_host_workspace_sample_picker.png`
- Current-source after captures:
  - `artifacts\sample_catalog_shell_after_20260712_02\wpf_shell_host_workspace_sample_open.png`
  - `artifacts\sample_catalog_picker_after_20260712_01\wpf_shell_host_workspace_sample_picker.png`
- Focused smokes assert no auto Preview/Run behavior changed. Final verification commands must still be rerun after this documentation update before reporting completion.

Latest Shell maximize and compact Tool rail slice on 2026-07-12:

- `OpenVisionShellHostWindow` now handles `WM_GETMINMAXINFO` against the current monitor work area. Maximized content stops above the Windows taskbar instead of hiding the bottom status/log controls.
- Compact Tool rail remains 56 px wide and shows the existing tool icons with tooltips and working commands. Clicking a compact Threshold icon selects the tool without triggering Preview/Run.
- The redundant read-only `Scope: {pipeline}` chip is hidden. The adjacent recipe selector is the single operator-facing recipe context; internal recipe/pipeline routing context is unchanged.
- Current-source before captures:
  - `artifacts\shell_chrome_before_20260712_01\wpf_shell_host_tool_rail_compact.png`
  - `artifacts\shell_chrome_before_20260712_01\wpf_shell_host_workspace_empty.png`
- Current-source/current-build after captures:
  - `artifacts\shell_chrome_final_debug_20260712_01\wpf_shell_host_window_maximized.png`
  - `artifacts\shell_chrome_final_debug_20260712_01\wpf_shell_host_tool_rail_compact.png`
  - `artifacts\shell_chrome_final_debug_20260712_01\wpf_shell_host_workspace_empty.png`
  - `artifacts\shell_chrome_final_debug_20260712_01\wpf_shell_host_recipe_context_switch.png`
- Focused screenshot smokes require maximized bounds to remain inside the Windows work area, at least ten visible compact tool buttons, a working Threshold icon command, no compact-icon Preview/Run side effect, no visible `HostRecipeContext`, and no visible `범위:` text.

Latest Color/HSV Learn-to-tool slice on 2026-07-13:

- The Color/HSV Learn topic now exposes `HSV Tool 열기` and names the Hue/Saturation/Value, ROI, and OutputLayer locations to inspect in the existing PropertyGrid Tool View.
- The action only opens/selects HSV. It must not run Preview/Run, create a result layer, or change input routing.
- Focused coverage is in `wpf_openvision_learn_color_hsv` and the Shell-backed `wpf_shell_host_learn_entry` target.
- Current-source before: `artifacts\learn_color_tool_link_before_20260713_01\wpf_openvision_learn_color_hsv.png`.
- Current-source after: `artifacts\learn_color_tool_link_after_20260713_04\wpf_openvision_learn_color_hsv.png`.
- Verification: solution build passed with 0 warnings and 0 errors; `wpf_openvision_learn_color_hsv`, `wpf_shell_host_learn_entry`, `wpf_simple_preprocess_tool_learn_button`, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Color/HSV Split/Merge Learn slice on 2026-07-13:

- The existing Color/HSV animation now runs four explicit stages: BGR input, `Cv2.Split` into B/G/R `CV_8UC1` Mats, `Cv2.Merge` plus BGR-to-HSV conversion, and HSV gate/mask review.
- The visual sample keeps BGR `(25,185,105)`, split values B=25/G=185/R=105, merged BGR `(25,185,105)`, and converted HSV `(45,221,185)` aligned with an actual one-pixel OpenCvSharp smoke assertion.
- Split/Merge remains Learn-only. No new ToolType was added because there is no separate operator workflow, metric contract, or public sample requiring a channel tool.
- Current-source before: `artifacts\learn_color_split_merge_before_20260713_01\wpf_openvision_learn_color_hsv.png`.
- Current-source after: `artifacts\learn_color_split_merge_after_20260713_04\wpf_openvision_learn_color_hsv.png`.
- Verification: solution build passed with 0 warnings and 0 errors; `wpf_openvision_learn_color_hsv`, `wpf_shell_host_learn_entry`, `wpf_simple_preprocess_tool_learn_button`, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Brightness/Histogram Learn-to-tool slice on 2026-07-13:

- The Brightness/Histogram Learn topic now exposes `Mean Tool 열기` and `Histogram Tool 열기` using the existing Shell related-tool callback.
- Mean points operators to `Mean Type`, `Min Mean`, and `Max Mean`; Histogram points to `Type`, `Clip Limit`, `Tile Grid`, and `Normalize Alpha/Beta`.
- Standalone Learn keeps both buttons disabled. Shell actions only open/select the Tool View and must not run Preview/Run, create a result layer, or change input routing.
- Current-source before: `artifacts\learn_brightness_tool_links_before_20260713_01\wpf_openvision_learn_brightness.png`.
- Current-source after: `artifacts\learn_brightness_tool_links_after_20260713_06\wpf_openvision_learn_brightness.png`.
- Focused coverage is in `wpf_openvision_learn_brightness` and the Shell-backed `wpf_shell_host_learn_entry` target.

Latest Arithmetic Learn-to-tool slice on 2026-07-13:

- The Arithmetic Learn topic now exposes `Arithmetic Tool 열기` using the existing Shell related-tool callback.
- The location guide names the actual double-input route and parameter areas: `Input A`, `Input B`, `Output Layer`, `Mode`, `Arithmetic Type`, and `Input B Source`.
- Standalone Learn keeps the button disabled. The Shell action only opens/selects Arithmetic and must not run Preview/Run, create a result layer, or change either input route.
- Current-source before: `artifacts\learn_arithmetic_tool_link_before_20260713_01\wpf_openvision_learn_arithmetic.png`.
- Current-source after: `artifacts\learn_arithmetic_tool_link_after_20260713_02\wpf_openvision_learn_arithmetic.png`.
- Focused coverage is in `wpf_openvision_learn_arithmetic`, `wpf_shell_host_learn_entry`, and the existing reverse `wpf_arithmetic_tool_learn_button` target.
- Verification: solution and screenshot-tool builds passed with 0 warnings and 0 errors; all three focused targets, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Geometry Learn-to-tool slice on 2026-07-13:

- The Geometry Learn topic now exposes `Rotate / Scale Tool 열기` using the existing Shell related-tool callback.
- The location guide names the shared input/output route and actual `Angle`, `Scale X`, and `Scale Y` fields, and explains that `OutputSize` is an explicit Preview result rather than another input.
- Standalone Learn keeps the button disabled. The Shell action only opens/selects RotateScale and must not run Preview/Run, create a result layer, or change routing.
- Current-source before: `artifacts\learn_geometry_tool_link_before_20260713_01\wpf_openvision_learn_geometry.png`.
- Current-source after: `artifacts\learn_geometry_tool_link_after_20260713_02\wpf_openvision_learn_geometry.png`.
- Focused coverage is in `wpf_openvision_learn_geometry`, `wpf_shell_host_learn_entry`, and the existing reverse `wpf_simple_preprocess_tool_learn_button` target.
- Verification: solution and screenshot-tool builds passed with 0 warnings and 0 errors; all three focused targets, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Filtering Learn-to-tool slice on 2026-07-13:

- The Filtering Learn topic now exposes `Filter Tool 열기` using the existing Shell related-tool callback.
- The location guide names the shared route, `Filter Type`, `Border Type`, Kernel `Width/Height`, and the type-specific Median/Bilateral fields.
- Standalone Learn keeps the button disabled. The Shell action only opens/selects Filter and must not run Preview/Run, create a result layer, or change routing.
- Current-source before: `artifacts\learn_filter_tool_link_before_20260713_01\wpf_openvision_learn_filtering.png`.
- Current-source after: `artifacts\learn_filter_tool_link_after_20260713_02\wpf_openvision_learn_filtering.png`.
- Focused coverage is in `wpf_openvision_learn_filtering`, `wpf_shell_host_learn_entry`, and the existing reverse Filter header Learn assertion in `wpf_filter_morphology_layout_guard`.
- Verification: solution and screenshot-tool builds passed with 0 warnings and 0 errors; all three focused targets, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Morphology Learn-to-tool slice on 2026-07-13:

- The Morphology Learn topic now exposes `Morphology Tool 열기` using the existing Shell related-tool callback.
- The location guide names the shared input/output route, `Operation`, Kernel `Width/Height`, `3 x 3`/`5 x 5`/`7 x 7` presets, and `Rect`/`Ellipse`/`Cross` Shape choices.
- Standalone Learn keeps the button disabled. The Shell action only opens/selects Morphology and must not run Preview/Run, create a result layer, or change routing.
- Current-source before: `artifacts\learn_morphology_tool_link_before_20260713_01\wpf_openvision_learn_morphology.png`.
- Current-source after: `artifacts\learn_morphology_tool_link_after_20260713_02\wpf_openvision_learn_morphology.png`.
- Focused coverage is in `wpf_openvision_learn_morphology`, `wpf_shell_host_learn_entry`, and the existing reverse Morphology header Learn assertion in `wpf_filter_morphology_layout_guard`.
- Verification: solution and screenshot-tool builds passed with 0 warnings and 0 errors; all three focused targets, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Blob Learn-to-tool slice on 2026-07-13:

- The Blob Learn topic now exposes `Blob Tool 열기` using the existing Shell related-tool callback.
- The location guide names the actual Blob PropertyGrid fields: `Use ROI`, `ROI`, Blob Parameter `Min area`, and `Max area`; it separates those setup values from the post-run `ResultCount`, `AreaMin/AreaMax`, and `BoundsWidth/BoundsHeight` metrics.
- Standalone Learn keeps the button disabled. The Shell action only opens/selects Blob and must not run Preview/Run, create a result layer, or change routing.
- Current-source before: `artifacts\learn_blob_tool_link_before_20260713_01\wpf_openvision_learn_blob.png`.
- Current-source after: `artifacts\learn_blob_tool_link_after_20260713_02\wpf_openvision_learn_blob.png`.
- Focused coverage is in `wpf_openvision_learn_blob`, `wpf_shell_host_learn_entry`, and the existing reverse `wpf_shell_host_blob_tool` target.
- Verification: solution and screenshot-tool builds passed with 0 warnings and 0 errors; all three focused targets, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Contour Learn-to-tool slice on 2026-07-13:

- The Contour Learn topic now exposes `Contour Tool 열기` using the existing Shell related-tool callback.
- The location guide names the actual Contour PropertyGrid fields: `컨투어 표시`, `Retrieval mode`, `Min area`, `Max area`, plus optional approximation and drawing fields; it separates those setup values from the post-run `ResultCount`, `AreaMax`, `BoundsWidthMax`, and `BoundsHeightMax` metrics.
- Standalone Learn keeps the button disabled. The Shell action only opens/selects Contour and must not run Preview/Run, create a result layer, or change routing.
- Current-source before: `artifacts\learn_contour_tool_link_before_20260713_01\wpf_openvision_learn_contour.png`.
- Current-source after: `artifacts\learn_contour_tool_link_after_20260713_02\wpf_openvision_learn_contour.png`.
- Focused coverage is in `wpf_openvision_learn_contour`, `wpf_shell_host_learn_entry`, and the existing reverse `wpf_shell_host_contour_tool` target.
- Verification: solution and screenshot-tool builds passed with 0 warnings and 0 errors; all three focused targets, readiness, external references, and public sample assets passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Edge/Line Learn-to-tool slice on 2026-07-13:

- The Edge/Line Learn topic now exposes separate `EdgeDetection Tool 열기` and `Line Tool 열기` actions using the existing Shell related-tool callback.
- The role guide keeps the tools distinct: EdgeDetection creates an edge-map layer from Canny/Sobel/Scharr/Laplacian settings; Line performs ROI-based edge/fit-line work using Purpose, Line A/B, ROI, edge polarity/direction/contrast/thickness, and scan settings.
- Standalone Learn keeps both buttons disabled. Shell actions only open/select the requested Tool View and must not run Preview/Run, create a result layer, or change routing.
- The existing Line Tool header Learn route remains topic 8 LineDistance because the same Tool View owns Measure and Intersection purposes; the Edge/Line topic does not replace that route.
- Current-source before: `artifacts\learn_edge_line_tool_links_before_20260713_01\wpf_openvision_learn_edge_line.png`.
- Current-source after: `artifacts\learn_edge_line_tool_links_after_20260713_02\wpf_openvision_learn_edge_line.png`.
- Focused coverage is in `wpf_openvision_learn_edge_line`, `wpf_shell_host_learn_entry`, `wpf_simple_preprocess_tool_learn_button`, and `wpf_shell_host_line_tool`.

Latest LineDistance Learn-to-tool slice on 2026-07-14:

- The LineDistance Learn topic now exposes `Line Tool 열기` and points to the existing Line Tool's `Purpose > Measure`, Line A/B, ROI, `Pixel / mm`, edge/scan fields, and average plus range/max result review.
- The action opens/selects the Line Tool View only. It deliberately does not auto-select Measure, mutate parameters, run Preview/Run, create an output layer, or change input routing.
- Current-source before: `artifacts\learn_line_distance_tool_link_before_20260714_02\wpf_openvision_learn_line_distance.png`.
- Current-source after: `artifacts\learn_line_distance_tool_link_after_20260714_02\wpf_openvision_learn_line_distance.png`.
- Focused coverage: `wpf_openvision_learn_line_distance`, Shell-backed `wpf_shell_host_learn_entry`, and reverse `wpf_shell_host_line_tool`.
- Verification: solution and screenshot-smoke builds passed with 0 warnings and 0 errors; all three focused smoke targets, repository readiness, external-reference, and public-sample checks passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Matching Learn-to-tool slice on 2026-07-14:

- The Matching Learn topic now exposes `Matching Tool 열기` and names the actual operator locations: Tool Shell `Template Ready`, PropertyGrid `Pattern path`, `Matching > Min score`, `Match count`, ROI, and optional angle/scale search.
- Result guidance keeps setup and evidence distinct: the operator explicitly runs Preview or Run Review, checks overlay position, and interprets `ScoreMax` together with `ResultCount`.
- The action opens/selects the Matching Tool View only. It does not register a template, mutate parameters, run Preview/Run, create an output layer, or change input routing.
- Current-source before: `artifacts\learn_matching_tool_link_before_20260714_01\wpf_openvision_learn_matching.png`.
- Current-source after: `artifacts\learn_matching_tool_link_after_20260714_04\wpf_openvision_learn_matching.png`.
- Focused coverage: `wpf_openvision_learn_matching`, Shell-backed `wpf_shell_host_learn_entry`, and reverse `wpf_shell_host_matching_tool`.
- Verification: solution and screenshot-smoke builds passed with 0 warnings and 0 errors; all three focused smoke targets, repository readiness, external-reference, and public-sample checks passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest FeatureMatching Learn-to-tool slice on 2026-07-14:

- The FeatureMatching Learn topic now exposes `FeatureMatching Tool 열기` and names only the fields the current PropertyGrid actually owns: Tool Shell `Template Ready`, `Feature template path`, `Matching > Ratio threshold`, `RANSAC tolerance`, and ROI.
- The serialized key remains `SCORE_MIN`, but the PropertyGrid and Learn text identify it as the Lowe Ratio threshold: smaller values are stricter. Learn keeps GoodMatches as a concept, while explicit Preview or Run Review evidence uses overlay position, `ScoreMax`, and `ResultCount`.
- The action opens/selects the FeatureMatching Tool View only. It does not register a template, mutate parameters, run Preview/Run, create an output layer, or change input routing.
- Current-source before: `artifacts\learn_feature_matching_tool_link_before_20260714_01\wpf_openvision_learn_feature_matching.png`.
- Current-source after: `artifacts\learn_feature_matching_tool_link_after_20260714_02\wpf_openvision_learn_feature_matching.png`.
- Focused coverage: `wpf_openvision_learn_feature_matching`, Shell-backed `wpf_shell_host_learn_entry`, and reverse `wpf_shell_host_feature_matching_tool`.
- Verification: solution and screenshot-smoke builds passed with 0 warnings and 0 errors; all three focused smoke targets, repository readiness, external-reference, and public-sample checks passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest EdgeBasedMatching Learn-to-tool slice on 2026-07-14:

- The shared Matching animation panel now switches its related action to `EdgeBasedMatching Tool 열기` for the EdgeBasedMatching topic and opens the actual EdgeBasedMatching PropertyGrid Tool View.
- The location guide names the current fields: Tool Shell `Template Ready`, `Pattern path`, `Matching > Min score / Match count`, `Edge Model > Canny range / Max template points`, `Search > Search step`, ROI, and optional angle/scale search.
- The action opens/selects the Tool View only. It does not register a template, mutate parameters, run Preview/Run, create an output layer, or change input routing.
- Current-source before: `artifacts\learn_edge_based_matching_tool_link_before_20260714_01\wpf_openvision_learn_edge_based_matching.png`.
- Current-source after: `artifacts\learn_edge_based_matching_tool_link_after_20260714_02\wpf_openvision_learn_edge_based_matching.png`.
- Focused coverage: `wpf_openvision_learn_edge_based_matching`, Shell-backed `wpf_shell_host_learn_entry`, and reverse `wpf_shell_host_edge_based_matching_tool`.
- Verification: solution and screenshot-smoke builds passed with 0 warnings and 0 errors; all three focused smoke targets, repository readiness, external-reference, and public-sample checks passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Matching-family semantics audit on 2026-07-14:

- Runtime inspection confirmed that FeatureMatching `SCORE_MIN` is used as the Lowe descriptor-ratio threshold (`best.Distance < SCORE_MIN * second.Distance`), so smaller values are stricter. The serialized key remains unchanged for recipe compatibility.
- FeatureMatching PropertyGrid, preset descriptions, Learn guidance, localization defaults/migration, XML authoring guide, and tool catalog now use the same Ratio/RANSAC meaning. Exact-default localization migration updates the previous built-in text without overwriting user-customized translations.
- FeatureMatching practice guidance now separates setup from evidence: configure Ratio/RANSAC, explicitly run Preview or Run Review, then compare `GoodMatches`, `ScoreMax`, and overlay position.
- Current-source before: `artifacts\matching_family_semantics_before_20260714_01\wpf_openvision_learn_feature_matching.png` and `artifacts\matching_family_semantics_before_20260714_01\wpf_shell_host_feature_matching_tool.png`.
- Current-source after: `artifacts\matching_family_audit_20260714_03_feature\wpf_openvision_learn_feature_matching.png` and `artifacts\matching_family_audit_20260714_01\wpf_shell_host_feature_matching_tool.png`.
- Verification: solution and screenshot-smoke builds passed with 0 warnings and 0 errors; Matching, EdgeBasedMatching, and FeatureMatching Learn targets, Shell Learn entry, and all three reverse Tool targets passed with `layout=0`, `text=0`, and `internal=0`. Repository readiness, external-reference, and public-sample checks passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Latest Recipe Manager Run History next-action visibility slice on 2026-07-14:

- Fresh current-EXE evidence showed that the selected run review could grow with its content and placed the operator's `Next` guidance after run summary, sample, result, and linked-Step lines. On the 1600x900 workbench the next action was only partially visible near the footer.
- `HostRecipeSelectedRunReview` now keeps a stable 72-88px scrollable height. The review order is `Run -> Sample -> Result -> Next -> Linked step -> Summary`, so the action appears in the first four lines while full detail remains available through the internal scrollbar and copy command.
- Direct smoke now asserts both the bounded review height and first-four-lines next-action contract. It does not add or trigger Preview/Run, layer creation, input routing changes, or recipe mutation.
- Current-build before: `artifacts\recipe_manager_llm_ux_audit_before_20260714_01\OpenVisionLab_RecipeManager_RunHistory.png`.
- Current-build after: `artifacts\recipe_manager_run_history_after_20260714_04\OpenVisionLab_RecipeManager_RunHistory.png`.
- Verification: fresh solution build passed with 0 warnings and 0 errors; `OpenVisionLab.exe --smoke recipe-manager-tabs` passed from the latest Debug build with the existing LLM validation/import blocks, Good/Bad review, Pipeline review, and explicit execution contracts intact. Repository readiness, external-reference, and public-sample checks passed. Public sample result remained `CatalogRows=28`, `ManifestAssets=226`, `Pipelines=14`.

Current priority order:

1. If a real API key or manual transcript is available, collect one GPT/Gemini/Claude XML correction-loop transcript using `docs\OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md` and `docs\OPENVISIONLAB_LLM_TOOL_CATALOG.json`, then validate/import it in Recipe Manager. Do not fabricate transcript evidence.
2. If no real transcript is available, continue Recipe Manager/LLM Assistant UX work only where fresh current-EXE/current-source screenshots show actual clipping, overlap, unclear next action, or workflow friction.
3. Expand branch/output comparison only when a real multi-branch recipe exceeds the current selected-step producer/consumer model and the existing BentPin plus Contour_AllSymbolsAndFaint coverage.
4. Continue Tool View code-behind cleanup only where established controller/presenter/base patterns fit; current test hooks and preview command paths are in use and should not be removed just to reduce line count. The double-input Arithmetic shell, Blob/Contour/Line single-input PropertyGrid shell, and Matching-family single-input PropertyGrid shell now have shared bases, so do not recreate those extractions.

Latest UI evidence for pin-gap intent ROI suggestion:

- Scope: pin-gap intent skills now treat unmarked requests as whole pin-array inspection samples, not one arbitrary pair, and expose a `Sample ROI` button beside the ROI sample field.
- Product contract: `Sample ROI` is a starter ROI helper. It scales the existing whole-array ROI samples from the selected sample/reference image size. It does not claim automatic visual understanding, does not create a run result, and does not trigger Preview/Run.
- Before: `artifacts\pin_gap_roi_suggest_before_20260707_r1\OpenVisionLab_RecipeManager_LlmIntentSkills_PinGap.png`.
- After: `artifacts\pin_gap_roi_suggest_after_20260707_r1\OpenVisionLab_RecipeManager_LlmIntentSkills_PinGap.png`.
- Current-build UI smoke: `dotnet run --no-build --project OpenVisionLab.csproj -c Debug -- --smoke recipe-manager-llm-intent-skills artifacts\pin_gap_roi_suggest_after_20260707_r1` passed after a fresh Debug build.
- Current-build direct smoke: `artifacts\pin_gap_roi_suggest_recipe_manager_tabs_20260707_r1\report.txt` passed with `LlmPinGapRoiSuggest: selected sample image suggested multi-sample ROI without Preview/Run`.
- Current-build XML/image run: `artifacts\pin_gap_roi_suggest_generated_xml_image_run_20260707_r1\report.txt` passed on `Sample\EasyGauge\Pin 1.jpg` with final layer `PinArray_Review` and `MergeOverlayCount=24`.

Latest LLM prompt evidence for pin-gap GPT handoff:

- Scope: when `Pin gap / edge distance (LineDistance)` is selected, Recipe Manager `Build prompt` now embeds a self-contained GPT task packet for pin gap/pitch/edge-to-edge distance XML. It includes whole-array default scope, ROI samples, DistanceMmAvg and DistanceMmRange gates, mm/px, LineDistance-only constraints, OverlayMerge review, and XML-only response format.
- Product contract: this reduces manual document hunting for the operator. The preferred user flow is Recipe Manager -> LLM XML -> select pin-gap intent -> set ROI/spec fields -> `Build prompt` -> `Copy prompt` -> paste into GPT with the image. File packets under `llm_prompt_packets\pin_gap_distance` remain as fallback/reference material.
- Before: `artifacts\pin_gap_prompt_packet_before_20260707_r1\OpenVisionLab_RecipeManager_LlmIntentLineDistance.png`.
- After: `artifacts\pin_gap_prompt_packet_after_20260707_r1\OpenVisionLab_RecipeManager_LlmIntentLineDistance.png`.
- Current-build direct smoke: `artifacts\pin_gap_prompt_packet_after_20260707_r1\report.txt` passed with `LlmPinGapPromptPacket: copy-ready GPT XML-only packet copied`.
- Current-build XML/image run: `artifacts\pin_gap_prompt_packet_generated_xml_image_run_20260707_r1\report.txt` passed on `Sample\EasyGauge\Pin 1.jpg` with final layer `PinArray_Review`.

Latest UI evidence for corrected-output review after Step XML apply:

- Before: `artifacts\corrected_output_review_before_20260705_r1\OpenVisionLab_RecipeManager_StepPropertyGrid.png`
- After: `artifacts\corrected_output_review_after_20260705_r1\OpenVisionLab_RecipeManager_StepPropertyGrid.png`
- Direct EXE smoke: `artifacts\corrected_output_review_after_20260705_r1\report.txt` with `Result: PASS`, `CorrectedOutputReview: visible after XML apply`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Structure note: `HostRecipeCorrectedOutputReviewPanel` appears under the embedded Step PropertyGrid status. It reuses existing explicit output navigation and Good/Bad rerun commands; XML apply still does not run Preview/Run.

Latest UI evidence for LLM dependency path drill-down rows:

- Before: `artifacts\corrected_output_review_after_20260705_r1\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_dependency_drilldown_after_20260705_r2\OpenVisionLab_RecipeManager_LlmXml.png`
- Direct EXE smoke: `artifacts\llm_dependency_drilldown_after_20260705_r2\report.txt` with `Result: PASS`, `LlmDependencyRows: 1`, and `LlmXmlDiff: visible`.
- Structure note: `LlmXmlDraftDependencyRows` exposes row-level status, step, parameter, path, and action. The text dependency report remains for copy/paste review.

Latest UI evidence for Recipe Manager internal workbench density adjustment:

- Before: `artifacts\llm_dependency_drilldown_after_20260705_r2\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\recipe_manager_workbench_layout_after_20260705_r2\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\recipe_manager_workbench_layout_after_20260705_r2\report.txt` with `Result: PASS`, `CorrectedOutputReview: visible after XML apply`, `LlmDependencyRows: 1`, and `MovedTo: -64.0,18.0`.
- Structure note: Pipeline tab internal management column is narrower so the Step review/PropertyGrid area gets more horizontal room on large workbench screens.

Latest UI evidence for branch/output comparison rows:

- True before note: `artifacts\branch_output_comparison_before_20260705_r1\wpf_shell_host_recipe_language_controls.png` was captured from the current smoke target but Visual Studio was in front, so it is not a clean true-before UI capture. Treat the immediately previous Recipe Manager layout capture as the closest baseline: `artifacts\recipe_manager_workbench_layout_after_20260705_r2\OpenVisionLab_RecipeManager_Pipeline.png`.
- After full-window capture: `artifacts\branch_output_comparison_after_20260705_r5_screenshot_smoke\wpf_shell_host_recipe_language_controls.png`
- Direct EXE smoke: `artifacts\branch_output_comparison_after_20260705_r2\report.txt` with `Result: PASS` and `BranchOutputComparison: 2`.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\branch_output_comparison_after_20260705_r5_screenshot_smoke` passed with `layout=0`, `text=0`, and `internal=0`.
- Structure note: `HostRecipeBranchOutputComparisonPanel` shows selected Step, same-input candidates, input producers, and output consumers for the selected multi-step route. Step navigation still does not run Preview/Run.

Latest UI evidence for selected Step operator context:

- Before: `artifacts\operator_step_review_before_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- After: `artifacts\operator_step_context_after_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- Direct EXE smoke: `artifacts\operator_step_context_after_20260706_r1_direct\report.txt` passed and now checks `PipelineSelectedStepOperatorContextText`.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\operator_step_context_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
- Structure note: `HostRecipePipelineSelectedStepOperatorContext` sits inside the selected Step detail panel and summarizes selected Step, route, Good/Bad or run-history failure link, and next action. It is read-only guidance and does not run Preview/Run.

Latest UI evidence for Recipe Manager XML/Step list density:

- Before: `artifacts\recipe_manager_density_current_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- After: `artifacts\recipe_manager_density_after_step_list_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_density_after_step_list_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
- Structure note: `HostRecipePipelineInlinePreviewStepList` now appears immediately after the XML/Step flow focus strip, before branch/output and selected-Step detail panels. On a 1600x900 workbench screenshot the Step rows are visible in the first view instead of only the list title being pushed near the footer. This is a layout-only change and does not add Preview/Run triggers.

Latest UI evidence for compact Recipe Manager recipe/Step rows:

- Before: `artifacts\recipe_manager_compact_step_rows_before_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- After: `artifacts\recipe_manager_compact_step_rows_after_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_compact_step_rows_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
- Direct EXE smoke: `artifacts\recipe_manager_compact_step_rows_after_20260706_r1_direct\report.txt` passed with `Result: PASS`, `BranchOutputComparison: 2`, and `ActualMultiBranchComparison: 7`.
- Structure note: the recipe library list now stretches item content so long names use ellipsis and tooltips predictably. The XML/Step inline Step list now uses single-line ellipsis rows for Display, Route, and Parameters instead of multi-line wrapping, preserving workbench density for long routes and parameter previews.

Latest UI evidence for large Recipe Manager library filtering:

- Before: `artifacts\recipe_large_library_before_20260706_r1\wpf_shell_host_recipe_large_library.png`
- After: `artifacts\recipe_large_library_after_20260706_r1\wpf_shell_host_recipe_large_library.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_large_library artifacts\recipe_large_library_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
- Direct EXE smoke: `artifacts\recipe_large_library_after_20260706_r1_direct\report.txt` passed with `Result: PASS`, `ActualMultiBranchComparison: 7`, and `ActualThreeWayBranchComparison: 5`.
- Structure note: `RecipeLibrarySummaryText` now shows total or filtered/total recipe count, for example `레시피 라이브러리 (10/101)`, above the recipe list. The screenshot smoke creates 100 long temporary recipe names, filters to `Category_07`, verifies 10 visible matches, then cleans the temporary workspaces.

Latest UI evidence for large Recipe Manager pipeline filtering:

- Before: `artifacts\recipe_large_pipeline_list_before_20260706_r1\wpf_shell_host_recipe_large_pipeline_list.png`
- After: `artifacts\recipe_large_pipeline_list_after_20260706_r1\wpf_shell_host_recipe_large_pipeline_list.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_large_pipeline_list artifacts\recipe_large_pipeline_list_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
- Direct EXE smoke: `artifacts\recipe_large_pipeline_list_after_20260706_r1_direct\report.txt` passed with `Result: PASS`, `ActualMultiBranchComparison: 7`, and `ActualThreeWayBranchComparison: 5`.
- Structure note: `PipelineListSummaryText` now shows total or filtered/total pipeline count, for example `파이프라인 (10/109)`, above the pipeline list. The screenshot smoke creates one temporary recipe with 100 long pipeline names, filters to `Group_07`, verifies 10 visible matches, then deletes the temporary workspace.

Latest actual multi-branch branch/output coverage:

- Real sample scan found `docs\samples\BentPin_TopBottom_Overlay.pipeline.xml` with `Main` fan-out and `BentPin_Clean` multi-consumer routes; `docs\samples\Contour_AllSymbolsAndFaint_LLM.pipeline.xml` also has `Main` fan-out.
- Direct EXE smoke now imports `docs\samples\BentPin_TopBottom_Overlay.pipeline.xml` and verifies both output consumers from `BentPin_Clean` plus the same-input and input-producer rows around the selected multi-branch Steps.
- Evidence: `artifacts\recipe_manager_density_after_step_list_20260706_r2_direct\report.txt` passed with `ActualMultiBranchComparison: 7`.
- Structure note: this expands verification of the existing branch/output comparison model for a real multi-branch recipe. It does not create a new comparison surface.

Latest actual 3+ branch fan-out coverage:

- Real sample scan found only one 3+ branch candidate in the current sample set: `docs\samples\Contour_AllSymbolsAndFaint_LLM.pipeline.xml`, where `Main` feeds 4 enabled Steps.
- Screenshot smoke target `wpf_shell_host_recipe_multibranch_comparison` imports that pipeline, selects `Main -> TextSymbol_Binary`, and verifies 3 same-input alternatives plus one output consumer row.
- UI evidence: `artifacts\recipe_multibranch_comparison_after_20260706_r1\wpf_shell_host_recipe_multibranch_comparison.png`.
- Direct EXE smoke evidence: `artifacts\recipe_multibranch_comparison_after_20260706_r1_direct\report.txt` passed with `ActualThreeWayBranchComparison: 5`.
- Decision: no new branch/output UI surface was added. The current selected-step producer/consumer list shows all rows for this real 3+ branch sample on the 1600x900 workbench screenshot, so broader UI expansion should wait for a real sample that exceeds this map.

Latest LLM XML authoring reference:

- Added `docs\OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md` as the external GPT/Gemini/Claude prompt-side API guide for OpenVisionLab `VisionPipeline` XML authoring and correction-loop transcript collection.
- Added `docs\OPENVISIONLAB_LLM_TOOL_CATALOG.json` as the machine-readable tool/parameter/metric catalog for LLM prompt packets.
- Source evidence checked before writing: `OpenVisionShellHostRecipeCommandSurface.cs`, `VisionPipelineValidation.cs`, `VisionPipelineStepParameterSchema.cs`, `VisionPipelineKnownMetrics.cs`, `VisionPipelineArithmeticStep.cs`, `docs\samples\*.pipeline.xml`, and LLM direct smoke cases in `OpenVisionLabDirectSmokeRunner.cs`.
- Usage rule: when collecting real LLM transcripts, give the LLM the guide and JSON catalog first, ask for one `VisionPipeline` XML only, validate inside Recipe Manager, then feed back the validation/dependency report for repair. Do not commit raw transcripts until private names/assets are scrubbed.
- Corpus rule: store real external LLM prompt/response under `artifacts\llm_transcripts\raw`, user-provided/operator-repaired XML replay cases under `artifacts\llm_transcripts\manual`, and sanitized replay candidates under `artifacts\llm_transcripts\sanitized`. Manual replay is validation evidence, not real GPT/Gemini/Claude transcript evidence.
- Validation rules now documented for external LLMs include supported ToolType names, `Main`/previous-output layer routing, `ALLOW_BRANCH_INPUT`, dependency path safety, `Inspection.*` review channel handling, 0..1 matching score parameters, gray-value ranges, Arithmetic `InputLayerB`, OverlayMerge final review intent, and acceptance metric use.

Latest LLM intent-contract replay evidence:

- User-provided pin image XML replay is local-only under `artifacts\llm_transcripts\manual\20260706_user_pins_marked_roi_length_measure.xml`. It is a manual draft/replay artifact, not a real external GPT/Gemini/Claude transcript.
- Product lesson: when the selected intent is pin-to-pin, edge-to-edge, gap, pitch, width, or clearance, OpenVisionLab must lock the tool family to `LineDistance`. `Contour` plus `BoundsHeightAvg` does not satisfy a distance intent.
- Source behavior: `OpenVisionShellHostRecipeCommandSurface.AppendLlmIntentContractValidation` blocks drafts whose selected `Pin gap / edge distance (LineDistance)` intent lacks an enabled `LineDistance` or accepted `LineDistanceGauge` step.
- Latest direct EXE smoke: `dotnet run --no-build --project OpenVisionLab.csproj -c Debug -- --smoke recipe-manager-llm-intent-skills artifacts\llm_manual_replay_contract_after_20260707_r1` passed with `Result: PASS`, `PinGapContourMismatch: blocked by intent contract`, and `PreviewRunCountUnchanged: 0`.
- Validation evidence: `artifacts\llm_manual_replay_contract_after_20260707_r1\PinGapContourMismatchValidation.txt` reports `Error: Intent contract mismatch. Selected intent 'Pin gap / edge distance' requires ToolType=LineDistance.` and `Draft enabled ToolTypes: Contour, Threshold`.
- UI evidence: `artifacts\llm_manual_replay_contract_after_20260707_r1\OpenVisionLab_RecipeManager_LlmIntentSkills_PinGapContourMismatch.png`.
- Next transcript work remains unchanged: capture one real GPT/Gemini/Claude correction-loop transcript when an API key or manually exported transcript is available. Do not treat this manual replay as that evidence.

Latest Recipe Manager sample summary density evidence:

- Before/current capture: `artifacts\recipe_manager_current_density_review_20260706_r1\wpf_shell_host_recipe_language_controls.png`.
- After capture: `artifacts\recipe_manager_density_after_sample_summary_20260706_r1\wpf_shell_host_recipe_language_controls.png`.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_density_after_sample_summary_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
- Structure note: the Recipe Manager left library pane now uses a 320px baseline width, and the sample acceptance summary shortens the displayed sample id while preserving the full summary as a tooltip. This is a layout/readability change only and does not add Preview/Run behavior.

Latest Tool View code-behind candidate review:

- Search evidence: `rg -n "SetTemplatePathForTest|ConfigurePropertyForTest|ApplyPresetForTest|ResultReviewTextForTest|ConsumeThresholdTeachingPreviewRequest" .` shows these hooks are used by native tool document, preview executor, and smoke paths.
- Decision: no code-behind deletion was made in this pass. Removing these forwarding/test hooks would be higher risk than value until a natural controller/base extraction target appears.

Latest Tool View code-behind cleanup for Blob/Contour single-input PropertyGrid base:

- Added `VisionToolSingleInputPropertyToolViewBase` and `IVisionToolSingleInputPropertyToolController` so Blob/Contour no longer duplicate source/destination layer events, preview image command events, selected layer getters, preview setters, status setter, and controller disposal.
- `BlobToolWpfView.xaml` and `ContourToolWpfView.xaml` now use `VisionToolSingleInputPropertyToolViewBase` as the XAML root.
- `BlobToolWpfView.xaml.cs` and `ContourToolWpfView.xaml.cs` now keep only tool-specific presenter setup, threshold teaching preview state, property creation, and area result review.
- Code-behind reduction: repeated forwarding removal reduced Blob and Contour from roughly 160 lines each to roughly 75 lines each.
- UI evidence: `artifacts\tool_view_property_base_smoke_20260705_r1\wpf_shell_host_blob_tool.png` and `artifacts\tool_view_property_base_smoke_20260705_r1\wpf_shell_host_contour_tool.png`.
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiScreenshotSmoke.ps1 -Targets "wpf_shell_host_blob_tool,wpf_shell_host_contour_tool" -OutputDir "artifacts\tool_view_property_base_smoke_20260705_r1"` passed with `layout=0`, `text=0`, and `internal=0` for both targets.
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
  - `git diff --check` passed with CRLF warnings only.

Latest Tool View code-behind cleanup for Matching-family single-input PropertyGrid base:

- Reused `VisionToolSingleInputPropertyToolViewBase` for `MatchingToolWpfView`, `EdgeBasedMatchingToolWpfView`, and `FeatureMatchingToolWpfView`.
- `VisionToolSingleInputMatchingToolController<TProperty>` now implements the same shared controller bridge used by Blob/Contour, so the Matching-family Views no longer duplicate source/destination layer events, preview image command events, selected layer getters, preview setters, status setter, and controller disposal.
- The Matching-family Views still own only tool-specific construction, template/test hooks, property creation, and matching result review.
- Code-behind line counts after cleanup: Matching 58, EdgeBasedMatching 53, FeatureMatching 49.
- Smoke stability note: the screenshot smoke matching template files now use unique temp names instead of a fixed `OpenVisionLab_matching_smoke_template.png`, preventing serial target cleanup/recreate interference during multi-target UI verification.
- UI evidence:
  - Before: `artifacts\matching_tool_base_before_20260706_r1\wpf_shell_host_matching_tool.png`, `artifacts\matching_tool_base_before_20260706_r1\wpf_shell_host_edge_based_matching_tool.png`, `artifacts\matching_tool_base_before_20260706_r1\wpf_shell_host_feature_matching_tool.png`.
  - After: `artifacts\matching_tool_base_after_20260706_r7\wpf_shell_host_matching_tool.png`, `artifacts\matching_tool_base_after_20260706_r7\wpf_shell_host_edge_based_matching_tool.png`, `artifacts\matching_tool_base_after_20260706_r7\wpf_shell_host_feature_matching_tool.png`.
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiScreenshotSmoke.ps1 -Targets "wpf_shell_host_matching_tool,wpf_shell_host_edge_based_matching_tool,wpf_shell_host_feature_matching_tool,wpf_property_grid_matching_combo" -OutputDir "artifacts\matching_tool_base_after_20260706_r7"` passed with `layout=0`, `text=0`, and `internal=0` for all targets.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_algorithm_output_preview_flow artifacts\matching_tool_base_after_20260706_r3_route` passed with `layout=0`, `text=0`, and `internal=0`.

Latest Tool View code-behind cleanup for Line single-input special PropertyGrid base:

- Reused `VisionToolSingleInputPropertyToolViewBase` for `LineToolWpfView`.
- `VisionToolSingleInputSpecialPropertyToolController` now implements `IVisionToolSingleInputPropertyToolController`, while keeping the Line-specific input-preview callback path for ROI overlay refresh.
- `LineToolWpfView.xaml` now uses `VisionToolSingleInputPropertyToolViewBase` as the XAML root.
- `LineToolWpfView.xaml.cs` now keeps Line-specific purpose/line selection, ROI editing, preset, result review, and test hooks; repeated source/destination layer events, preview image events, selected layer getters, layer list/output/status setters, and controller disposal moved to the shared base.
- Code-behind reduction: `LineToolWpfView.xaml.cs` went from 323 lines to 263 lines.
- UI evidence:
  - Before: `artifacts\line_tool_base_before_20260706_r1\wpf_shell_host_line_tool.png`.
  - After: `artifacts\line_tool_base_after_20260706_r1\wpf_shell_host_line_tool.png`.
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_tool_base_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_algorithm_output_preview_flow artifacts\line_tool_base_after_20260706_r1_route` passed with `layout=0`, `text=0`, and `internal=0`.

Latest Tool View shared-base stability recheck:

- Rechecked the current Dev build after Matching-family and Line shared-base cleanup.
- Verification: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiScreenshotSmoke.ps1 -Targets "wpf_shell_host_matching_tool,wpf_shell_host_edge_based_matching_tool,wpf_shell_host_feature_matching_tool,wpf_shell_host_line_tool" -OutputDir "artifacts\tool_view_shared_base_recheck_20260706_r1"` passed for all four targets with `layout=0`, `text=0`, and `internal=0`.
- UI evidence:
  - `artifacts\tool_view_shared_base_recheck_20260706_r1\wpf_shell_host_matching_tool.png`
  - `artifacts\tool_view_shared_base_recheck_20260706_r1\wpf_shell_host_edge_based_matching_tool.png`
  - `artifacts\tool_view_shared_base_recheck_20260706_r1\wpf_shell_host_feature_matching_tool.png`
  - `artifacts\tool_view_shared_base_recheck_20260706_r1\wpf_shell_host_line_tool.png`
- Decision: do not continue deleting Tool View code-behind just for line count. Next Tool View work should start only from a visible bug, duplicated owner path, or already-established controller/base pattern.

Latest Tool View code-behind cleanup for double-input custom tool base:

- Changed `ArithmeticToolWpfView` from direct `UserControl` inheritance to `VisionToolDoubleInputCustomToolViewBase`.
- Added `VisionToolDoubleInputCustomToolViewBase` to own double-input event forwarding, preview-image command forwarding, layer preview setters, status setter, and controller disposal.
- `ArithmeticToolWpfView.xaml.cs` now focuses on arithmetic-specific interaction/settings/text behavior. Code-behind reduced from 276 lines to 172 lines; shared base is 164 lines.
- UI evidence: `artifacts\double_input_custom_tool_base_refactor_20260705_r1\wpf_layer_selection_arithmetic_tool.png`
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\double_input_custom_tool_base_refactor_20260705_r1` passed with `layout=0`, `text=0`, and `internal=0`.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_algorithm_output_preview_flow artifacts\double_input_custom_tool_base_refactor_20260705_r1_route` passed with `layout=0`, `text=0`, and `internal=0`.
  - `git diff --check -- "0. UI/6) Vision Test/Wpf/ArithmeticToolWpfView.xaml" "0. UI/6) Vision Test/Wpf/ArithmeticToolWpfView.xaml.cs" "0. UI/6) Vision Test/Wpf/VisionToolDoubleInputCustomToolViewBase.cs"` passed with CRLF warnings only.

Latest current-build evidence rule and Arithmetic event-owner cleanup:

- `AGENTS.md` and global `C:\Users\user\.codex\AGENTS.md` now require smoke/capture evidence to use the latest updated EXE or a current-source view generated after the latest relevant source changes. Old artifacts must not be shown as current UI.
- `ArithmeticToolInteractionController` now owns Arithmetic parameter event attach/detach for operation mode, source mode, constant/offset text changes, and numeric input filtering. `ArithmeticToolWpfView.xaml` no longer wires those events to code-behind forwarding methods, and `ArithmeticToolWpfView.xaml.cs` no longer contains those forwarding methods.
- This is a narrow Tool View cleanup only; it does not change Preview/Run behavior, Arithmetic A/B input routing, output layer creation, or docked/floating layout.
- Latest build: `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors on 2026-07-06 21:57 KST.
- Latest direct EXE smoke: `dotnet run --no-build --project OpenVisionLab.csproj -c Debug -- --smoke recipe-manager-tabs artifacts\current_exe_recipe_manager_tabs_20260706_r2_direct` passed. Report includes `Result: PASS`, `LlmCorrectedDraftImport: imported`, `BranchOutputComparison: 2`, `ActualMultiBranchComparison: 7`, and `ActualThreeWayBranchComparison: 5`.
- Latest current-source view capture: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\current_source_arithmetic_tool_20260706_r2` passed with `layout=0`, `text=0`, and `internal=0`.
- Current UI evidence from this turn:
  - `artifacts\current_exe_recipe_manager_tabs_20260706_r2_direct\OpenVisionLab_RecipeManager_LlmXml.png`
  - `artifacts\current_source_arithmetic_tool_20260706_r2\wpf_layer_selection_arithmetic_tool.png`

Latest UI evidence for main window title-bar controls:

- Before/current check: `artifacts\main_window_chrome_before_20260705_r1\wpf_shell_host_window_chrome.png`
- After: `artifacts\main_window_chrome_after_20260705_r1\wpf_shell_host_window_chrome.png`
- Structure note: `OpenVisionWindowTitleBar` keeps minimize, maximize/restore, and close controls. `OpenVisionWindowTitleBar.xaml` now exposes `OpenVisionWindowMinimizeButton`, `OpenVisionWindowMaximizeRestoreButton`, and `OpenVisionWindowCloseButton` automation IDs.
- Verification: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_window_chrome artifacts\main_window_chrome_after_20260705_r1` passed with `layout=0`, `text=0`, and `internal=0`, and the smoke asserts all three window controls are visible.

Latest UI evidence for Recipe Manager density/status cleanup:

- Before: `artifacts\recipe_manager_density_before_20260705_r1\wpf_shell_host_recipe_language_controls.png`
- After: `artifacts\recipe_manager_density_after_20260705_r3\wpf_shell_host_recipe_language_controls.png`
- Direct EXE smoke: `artifacts\recipe_manager_density_after_20260705_r3_direct\report.txt` with `Result: PASS`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and `BranchOutputComparison: 2`.
- Structure note: `HostRecipeSelectedStepPropertyGridHost` is now visible only after selected Step parameters are explicitly loaded. Changing selected Step clears stale edit status text, so the view no longer shows an old Step 2 XML apply status while Step 1 is selected.
- Verification: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_density_after_20260705_r3` passed with `layout=0`, `text=0`, and `internal=0`.

Latest UI evidence for Recipe Manager footer density:

- Before: `artifacts\recipe_manager_density_before_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- After: `artifacts\recipe_manager_density_after_20260706_r2\wpf_shell_host_recipe_language_controls.png`
- Structure note: `HostRecipeManagerNameStrip` and `HostRecipeManagerCommandStrip` now share one compact footer row. The long recipe name editor remains visible while create/duplicate/rename/delete/XML import/export buttons stay inside the 1600x900 workbench viewport.
- Verification: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_density_after_20260706_r2` passed with `layout=0`, `text=0`, and `internal=0`.

Latest commercial-comparison guided workflow improvement:

- Commercial comparison basis checked during the 2026-07-06 loop:
  - Cognex In-Sight EasyBuilder emphasizes a step workflow such as image setup, location, inspection, and result/output review.
  - MVTec MERLIC recipe documentation emphasizes recipe files as parameter sets for reusable app variants.
  - NI Vision Builder AI documentation emphasizes state/step review.
  - KEYENCE CV-X simulator material emphasizes PC-side configuration/review and generated operating material.
- Dev scope decision: do not add camera/PLC/runtime integration. Add the useful part only: an in-app guided setup strip in Recipe Manager showing sample readiness, XML validation, Step count, sample run, Good/Bad run, and next action.
- Structure note: `HostRecipeGuidedSetupStrip` is now shown inside the Recipe Manager detail header and bound to `RecipeCommands.RecipeGuidedSetupText`.
- UI evidence: `artifacts\recipe_manager_guided_setup_after_20260706_r2\wpf_shell_host_recipe_language_controls.png`.
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_guided_setup_after_20260706_r2` passed with `layout=0`, `text=0`, and `internal=0`; the smoke now asserts `HostRecipeGuidedSetupStrip`.

Latest commercial-comparison self-evaluation:

- Official sources were rechecked on 2026-07-06:
  - Cognex EasyBuilder Inspect help and In-Sight Explorer product page: guided inspect/configuration/management workflow.
  - MVTec MERLIC recipe docs: `.mrcp` recipe files, MVApp references, and predefined parameter sets.
  - NI Vision Builder AI pages/readme: configure, benchmark, deploy, camera/image analysis, automation hardware.
  - KEYENCE CV-X product/software pages: camera/lighting/controller ecosystem plus PC simulator/terminal software.
- Product conclusion: OpenVisionLab should not chase camera, lighting, PLC/I/O, controller simulator, deployment runtime, account/session, or production audit features. Its differentiator is local image-based recipe design plus GPT/Gemini/Claude-style LLM XML generation, validation, and explicit OpenCvSharp4 rule-based verification.
- Completion estimate updated in `docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`: about 25-30% versus broad commercial equipment platforms by design, and about 62-66% versus the intended LLM-assisted rule-based recipe workbench.
- Next highest-value development target: real LLM XML failure examples and replayable validation scenarios for bad paths, wrong layers, wrong parameters, and unsafe imports. Do this before adding another generic Recipe Manager panel.

Latest LLM XML bad-route validation scenario:

- Added a direct EXE smoke case that creates a valid `VisionPipeline` XML draft with `InputLayer="Missing_Input_Layer"` and verifies validation blocks it as a route/layer error.
- The smoke asserts the malformed-XML case still reports a line/position fix, and the bad-route case reports the missing input layer without marking draft review/diff as ready.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_bad_route_validation_20260706_r1_direct"` passed with `Result: PASS` and `LlmBadRouteValidation: blocked`.

Latest LLM XML unsupported-tool/import-block scenario:

- Added a direct EXE smoke case that creates a valid `VisionPipeline` XML draft with `ToolType="ImaginaryLlmTool"` and verifies validation blocks it as an unsupported tool.
- The smoke then attempts the explicit import command when available and verifies the selected pipeline does not change and validation context is preserved. This covers the unsafe-import failure path without adding auto Preview/Run.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_unsupported_tool_validation_20260706_r1_direct"` passed with `Result: PASS`, `LlmBadRouteValidation: blocked`, and `LlmUnsupportedToolImport: blocked`.

Latest LLM XML failure corpus expansion:

- Missing dependency paths now block validation/import. `BuildDependencyReport(...)` reports the missing count back into LLM XML validation, so an XML draft with a missing template/image path is not importable even when schema/routing is otherwise valid.
- Added replayable direct EXE smoke cases for:
  - missing template dependency path on a Matching draft;
  - invalid parameter values such as `Threshold=bright` and `USE_ROI=sometimes`;
  - missing Arithmetic `InputLayerB` for a two-input operation.
- Each smoke case validates the draft, attempts explicit import when the command is available, and verifies the selected pipeline does not change.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_failure_corpus_20260706_r2_direct"` passed with `Result: PASS`, `LlmMissingDependencyImport: blocked`, `LlmBadParameterImport: blocked`, and `LlmMissingInputBImport: blocked`.

Latest LLM XML correction-loop scenario:

- The LLM review bundle now includes explicit correction rules: return only OpenVisionLab VisionPipeline XML, use `Main` or previous enabled `OutputLayer`, use supported ToolTypes and PropertyGrid-compatible values, fix missing dependency paths before import, and do not add equipment/Preview/Run instructions.
- The same review bundle now includes selected Step operator context and failed-Step review text, so GPT/Gemini/Claude-style correction requests carry the current Step, route, failure link, and next action without adding another UI surface.
- Added a direct EXE smoke path for bad draft -> correction bundle copy -> corrected XML validation -> explicit import.
- The corrected draft uses `Threshold=128` and `USE_ROI=False`, validates OK, imports as a new selected pipeline, and then the smoke restores the previous pipeline selection so the remaining Recipe Manager checks stay stable.
- Direct EXE evidence: `dotnet run --no-build --project OpenVisionLab.csproj -c Debug -- --smoke recipe-manager-tabs artifacts\llm_step_context_bundle_after_20260706_r1_direct` passed with `Result: PASS`, `LlmCorrectionBundle: copied`, and `LlmCorrectedDraftImport: imported`.
- Screenshot smoke evidence: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_step_context_bundle_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.

Latest LLM XML tool-parameter compatibility guard:

- `VisionPipelineValidator` now treats 0..1 score/weight parameters as validation errors when LLM output uses percentage-style values. Current guarded keys: `SCORE_MIN`, `GREEDINESS`, and `HYBRID_VERIFY_IMAGE_WEIGHT`.
- Matching/feature scale and tolerance parameters now have basic compatibility guards: `MAGNIFIATION`, `RANSAC_REPROJ_THRESHOLD`, and `COARSE_ANGLE_STEP` must be positive; `FIND_ANGLE_MIN` must not exceed `FIND_ANGLE_MAX`.
- Direct EXE smoke now includes `Direct_LLM_BadScoreRange` with `SCORE_MIN=80` and verifies it cannot import.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_parameter_compat_20260706_r1_direct"` passed with `Result: PASS` and `LlmBadScoreRangeImport: blocked`.

Latest LLM prompt/contract alignment:

- The in-app LLM prompt now tells GPT/Gemini/Claude-style assistants to use score/weight parameters as `0..1` decimals, keep angle min/max ordered, use positive matching/feature tolerance values, and avoid unresolved template/image dependency paths.
- `docs\VISION_PIPELINE_LLM_PROMPT_TEMPLATE.md` and `docs\VISION_PIPELINE_LLM_RECIPE_CONTRACT.md` now match the in-app import path: direct OpenVisionLab import expects complete `VisionPipeline` XML only, not extra prose.
- Direct EXE smoke now asserts the copied LLM prompt includes the new score, angle, and dependency-path rules.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_prompt_contract_20260706_r1_direct"` passed with `Result: PASS`.

Latest LLM XML result-channel contract:

- The in-app LLM prompt, review bundle, validation report, and Recipe Manager Report tab now define the operator result channels: `Inspection.Status`, `Inspection.FailedStep`, `Inspection.Evidence`, `Inspection.Benchmark`, and `Inspection.NextAction`.
- These are logical outputs derived from XML validation and explicit sample runs. LLM drafts must not emit custom `Inspection.*` XML nodes or parameters.
- Contract docs were updated in `docs\VISION_PIPELINE_LLM_RECIPE_CONTRACT.md`, `docs\VISION_PIPELINE_LLM_PROMPT_TEMPLATE.md`, and `docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`.
- Direct EXE smoke evidence from the implementation step: `artifacts\llm_result_channel_requirements_after_20260706_r2\report.txt` passed with prompt/review/validation checks for `Inspection.Status` and `Inspection.Evidence`.

Latest UI evidence for Recipe Manager result-channel board:

- Before: `artifacts\result_channel_board_before_20260706_r3_direct\OpenVisionLab_RecipeManager_Report.png`
- After: `artifacts\result_channel_board_after_20260706_r3_direct\OpenVisionLab_RecipeManager_Report.png`
- Structure note: `HostRecipeOperatorResultChannelBoard` now shows compact cards for `Inspection.Status`, `Inspection.FailedStep`, `Inspection.Evidence`, `Inspection.Benchmark`, and `Inspection.NextAction` above the detailed result-channel list in the Recipe Manager Report tab.
- Direct EXE smoke: `artifacts\result_channel_board_after_20260706_r3_direct\report.txt` passed with `Result: PASS`; the smoke asserts the board automation id and the Status/Evidence rows.

Latest LLM XML `Inspection.*` misuse block:

- `Inspection.*` names are now treated as logical review channels only. If an LLM draft emits `Inspection.Status` or another `Inspection.*` name inside XML, validation is NG and import keeps the previous pipeline selection.
- Direct EXE smoke: `artifacts\llm_custom_inspection_block_after_20260706_r4_direct\report.txt` passed with `LlmCustomInspectionImport: blocked`.
- Screenshot smoke: `artifacts\llm_custom_inspection_block_after_20260706_r2\wpf_shell_host_recipe_language_controls.png` passed with `layout=0`, `text=0`, and `internal=0`.

Latest Dev verification checkpoint at 2026-07-06 09:04 KST:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors after the LLM prompt/contract alignment.
- `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_prompt_contract_20260706_r1_direct"` passed with `Result: PASS`, `LlmBadScoreRangeImport: blocked`, `LlmCorrectionBundle: copied`, and `LlmCorrectedDraftImport: imported`.
- `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
- `git diff --check` passed with CRLF warnings only.
- Original repo was not touched.

Latest UI evidence for Recipe Manager guided next action:

- Before/current baseline: `artifacts\llm_clipboard_paste_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\guided_next_action_after_20260706_r2_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: The Recipe Manager guided setup strip now exposes `HostRecipeGuidedNextActionButton`. It routes one explicit user click to the current next existing action, such as Validate XML, Duplicate from sample, Activate pipeline, Run check, Run Good/Bad, load selected Step parameters, or open the selected Step tool. It does not add automatic Preview/Run.
- Direct EXE smoke: `artifacts\guided_next_action_after_20260706_r2_direct\report.txt` with `Result: PASS`, `FailedStepRerunComparison: visible`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and the smoke asserts the guided next action command is enabled during failed-Step review.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\guided_next_action_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it asserts `HostRecipeGuidedNextActionButton`.

Latest UI evidence for Recipe Manager guided next action label:

- Before: `artifacts\guided_next_action_after_20260706_r2_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\guided_next_label_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: `RecipeGuidedNextActionText` now shows the concrete next action instead of a generic "Run next" label. Failed-Step review shows `도구 열기`/`Open tool`; other states can show Validate XML, Duplicate sample, Activate, Run check, Load params, Run Good/Bad, or Complete.
- Direct EXE smoke: `artifacts\guided_next_label_after_20260706_r1_direct\report.txt` with `Result: PASS`, `FailedStepRerunComparison: visible`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and the smoke asserts the failed-Step guided action label includes tool/도구.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\guided_next_label_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.

Latest UI evidence for Run History selected-review copy action:

- Before: `artifacts\run_history_copy_before_20260706_r1_direct\OpenVisionLab_RecipeManager_RunHistory.png`
- After: `artifacts\run_history_copy_after_20260706_r1_direct\OpenVisionLab_RecipeManager_RunHistory.png`
- Structure note: The Run History tab now exposes `HostRecipeCopySelectedRunReviewButton`. It copies the selected run review text to the clipboard and shows inline status. It does not rerun checks, change layers, import XML, or run Preview.
- Direct EXE smoke: `artifacts\run_history_copy_after_20260706_r2_direct\report.txt` with `Result: PASS`, `SelectedRunReview: linked failed step`, `SelectedRunReviewCopy: copied`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`; the smoke executes the selected run review copy command and checks the success status. The command is enabled only when a saved run with `SummaryPath` is selected.
- Clipboard payload smoke: `artifacts\clipboard_payload_smoke_20260706_r1_direct\report.txt` passed after checking copied clipboard text for operator handoff report, selected run review, LLM prompt, and LLM review bundle.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\run_history_copy_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the selected run review copy command.
- Current-build Recipe Manager recheck: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_current_recheck_20260706_r2` passed with `layout=0`, `text=0`, and `internal=0`. The screenshot smoke now treats selected-run review copy as enabled only when a saved run with `SummaryPath` is selected.
- Direct EXE recheck: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\recipe_manager_current_recheck_20260706_r2_direct"` passed with `Result: PASS` and `SelectedRunReviewCopy: copied`, so the saved-run copy path remains covered.
- Follow-up screenshot recheck after smoke cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_current_recheck_20260706_r3` passed with `layout=0`, `text=0`, and `internal=0`. Visual inspection of `artifacts\recipe_manager_current_recheck_20260706_r3\wpf_shell_host_recipe_language_controls.png` did not show a new control clipping/overlap issue that justifies another UI change in this loop.

Latest UI evidence for Recipe Manager operator decision board:

- Before: `artifacts\recipe_manager_guided_setup_after_20260706_r3_direct\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- After: `artifacts\operator_review_board_after_20260706_r1_direct\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- Structure note: Review tab now shows `HostRecipeOperatorDecisionBoard` with XML/Step, selected sample, Good/Bad, and next-action cards above the existing long operator review text. It reuses existing sample/pair/pipeline state and does not add Preview/Run triggers.
- Direct EXE smoke: `artifacts\operator_review_board_after_20260706_r1_direct\report.txt` with `Result: PASS`, `PairRoleCards: 2`, `FailedStepRerunComparison: visible`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\operator_review_board_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.

Latest UI evidence for Recipe Manager operator handoff report:

- Before: `artifacts\operator_review_board_after_20260706_r1_direct\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- After: `artifacts\operator_report_tab_after_20260706_r1_direct\OpenVisionLab_RecipeManager_Report.png`
- Structure note: Pipeline review now has a `Report` tab (`HostRecipePipelineReportTab`) with `HostRecipeOperatorHandoffReport`. The report summarizes recipe, pipeline, active pipeline, XML/Step status, selected sample result, Good/Bad result, next action, selected role, review Step, route, and first LLM XML validation line.
- Direct EXE smoke: `artifacts\operator_report_tab_after_20260706_r1_direct\report.txt` with `Result: PASS`, `PairRoleCards: 2`, `FailedStepRerunComparison: visible`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\operator_report_tab_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.

Latest UI evidence for Recipe Manager operator report copy action:

- Before: `artifacts\operator_report_copy_before_20260706_r1_direct\OpenVisionLab_RecipeManager_Report.png`
- After: `artifacts\operator_report_copy_after_20260706_r1_direct\OpenVisionLab_RecipeManager_Report.png`
- Structure note: The Pipeline review `Report` tab now exposes `HostRecipeCopyOperatorHandoffReportButton`. It copies the generated operator report to the clipboard and shows an inline success/failure status without running Preview or changing layers.
- Direct EXE smoke: `artifacts\operator_report_copy_after_20260706_r1_direct\report.txt` with `Result: PASS`, `PairRoleCards: 2`, `FailedStepRerunComparison: visible`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`; the smoke now executes the copy command and checks the success status.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\operator_report_copy_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the copy command.

Latest UI evidence for LLM prompt copy action:

- Before: `artifacts\operator_report_copy_before_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_prompt_copy_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: The LLM XML tab now exposes `HostRecipeCopyLlmPromptButton`. It copies the generated prompt to the clipboard and shows an inline success/failure status. It does not validate/import XML and does not run Preview.
- Direct EXE smoke: `artifacts\llm_prompt_copy_after_20260706_r1_direct\report.txt` with `Result: PASS`, `LlmValidationIssues: visible`, `LlmDependencyRows: 1`, and `LlmXmlDiff: visible`; the smoke now builds the prompt, executes the copy command, and checks the success status.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_prompt_copy_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the copy command.

Latest UI evidence for LLM review bundle copy action:

- Before: `artifacts\llm_review_bundle_before_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_review_bundle_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: The LLM XML tab now exposes `HostRecipeCopyLlmReviewBundleButton`. It copies a correction bundle containing recipe/pipeline context, validation report, dependency report, draft import review, diff review, and current XML draft. It does not validate/import XML and does not run Preview.
- Direct EXE smoke: `artifacts\llm_review_bundle_after_20260706_r1_direct\report.txt` with `Result: PASS`, `LlmValidationIssues: visible`, `LlmDependencyRows: 1`, and `LlmXmlDiff: visible`; the smoke now executes the review bundle copy command and checks the success status.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_review_bundle_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the review bundle copy command.

Latest UI evidence for LLM XML clipboard paste action:

- Before: `artifacts\llm_clipboard_paste_before_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_clipboard_paste_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: The LLM XML tab now exposes `HostRecipePasteLlmXmlDraftButton`. It pastes clipboard XML text into the draft editor and shows an inline status. It does not validate, import, run Preview, or change layers; the operator still must press Validate and Import explicitly.
- Direct EXE smoke: `artifacts\llm_clipboard_paste_after_20260706_r1_direct\report.txt` with `Result: PASS`, `LlmValidationIssues: visible`, `LlmDependencyRows: 1`, and `LlmXmlDiff: visible`; the smoke now sets clipboard XML, executes the paste command, and checks the pasted draft/status.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_clipboard_paste_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the paste command.

Latest UI evidence for top account/operator chrome removal:

- Before: `artifacts\account_header_before_20260705_r1\wpf_shell_host_layer_management_commands.png`
- After: `artifacts\account_header_after_20260705_r1\wpf_shell_host_layer_management_commands.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_layer_management_commands artifacts\account_header_after_20260705_r1` passed.
- Structure evidence: `rg -n 'OperatorText|Shell\.Operator|Kind="Account"' -g '*.xaml' -g '*.cs' "0. UI/0) MENU/Wpf"` returns no matches.
- Product decision: account/session UI is not part of the current OpenVisionLab workbench scope.

Latest UI evidence for failed Step rerun/comparison action strip:

- True before note: this action strip was implemented before a fresh before capture was taken. Closest baseline is the immediately prior Recipe Manager role drill-down capture: `artifacts\llm_xml_diff_after_20260705_r1\OpenVisionLab_RecipeManager_RoleDrilldown.png`.
- After: `artifacts\failure_rerun_comparison_after_20260705_r1\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- WPF screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\failure_rerun_comparison_after_20260705_r2_screenshot_smoke` passed.
- Direct EXE smoke: `artifacts\failure_rerun_comparison_after_20260705_r1\report.txt` with `Result: PASS`, `FailedStepRerunComparison: visible`, `RoleDrilldown: Bad -> 01 Battery Cell Vent Alignment Distance`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Structure note: Review tab now shows `HostRecipeFailureRerunComparisonPanel` after a failed Step is selected, with direct output/input layer navigation, Step parameter review, and Good/Bad rerun actions. It reuses existing explicit commands and does not introduce auto Preview/Run.

Latest UI evidence for top layer command icon stabilization:

- Before: `artifacts\top_layer_icon_before_20260705_r1\wpf_shell_host_layer_management_commands.png`
- After: `artifacts\top_layer_icon_after_20260705_r2\wpf_shell_host_layer_management_commands.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_layer_management_commands artifacts\top_layer_icon_after_20260705_r2` passed.
- Structure note: top layer create/load/delete icon buttons now share a fixed 28x26 centered style, and the smoke asserts visible button size/order so the white icons do not drift, clip, or disappear under header pressure.

Latest UI evidence for LLM XML diff review and dependency path action hints:

- Before: `artifacts\llm_xml_diff_before_20260705_r1\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_xml_diff_after_20260705_r1\OpenVisionLab_RecipeManager_LlmXml.png`
- WPF screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_xml_diff_after_20260705_r1_screenshot_smoke` passed.
- Direct EXE smoke: `artifacts\llm_xml_diff_after_20260705_r1\report.txt` with `Result: PASS`, `LlmXmlDiff: visible`, `LlmValidationIssues: visible`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Structure note: LLM XML tab now separates draft validation, dependency scan/copy report, draft import review, LLM XML diff review, and validation issue rows. The diff compares the active pipeline with the draft before import and reports step count, dependency count, added/removed/changed steps, and parameter changes without running Preview.

Latest UI evidence for selected Step PropertyGrid parameter review and explicit XML apply-back:

- Before: `artifacts\recipe_step_parameter_apply_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\recipe_step_parameter_apply_after_20260705_r6\OpenVisionLab_RecipeManager_StepPropertyGrid.png`
- Direct EXE smoke: `artifacts\recipe_step_parameter_apply_after_20260705_r6\report.txt` with `Result: PASS`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and `StepToolEntry: 도구 열기: LineDistance`.
- Structure note: Recipe Manager owns the embedded Step PropertyGrid review/apply path. Opening the native tool seeds repository-backed tool sessions for inspection, but XML apply-back is still an explicit Recipe Manager action.

Latest UI evidence for Recipe Manager workbench layout:

- Before: `artifacts\recipe_workbench_layout_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\recipe_workbench_layout_after_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After with Step PropertyGrid loaded: `artifacts\recipe_workbench_layout_after_20260705_r1\OpenVisionLab_RecipeManager_StepPropertyGrid.png`
- Direct EXE smoke: `artifacts\recipe_workbench_layout_after_20260705_r1\report.txt` with `Result: PASS`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and `MovedTo: -64.0,18.0`.

Latest UI evidence for Good/Bad role failed-Step drill-down:

- Before: `artifacts\sample_role_drilldown_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\sample_role_drilldown_after_20260705_r3\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- Direct EXE smoke: `artifacts\sample_role_drilldown_after_20260705_r3\report.txt` with `Result: PASS`, `RoleDrilldown: Bad -> 01 Battery Cell Vent Alignment Distance`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.

Latest UI evidence for multi-step pipeline flow focus:

- Baseline note: the first current-build before capture used the wrong recipe-context screenshot target, so it is only a closest reproducible baseline for the shell state, not a true Recipe Manager before view.
- Closest baseline: `artifacts\multi_step_flow_before_20260705_r1\wpf_shell_host_recipe_context_switch.png`
- After full-window Recipe Manager capture: `artifacts\multi_step_flow_after_20260705_r3_recipe_manager\wpf_shell_host_recipe_language_controls.png`
- After Recipe Manager panel crop: `artifacts\multi_step_flow_after_20260705_r3_recipe_manager\wpf_shell_host_recipe_language_controls.diagnostics\recipe-manager-panel.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\multi_step_flow_after_20260705_r3_recipe_manager` passed.
- Structure note: Recipe Manager now exposes current selected Step flow in the header (`HostRecipePipelineHeaderStepFlow`), adds an XML/Step flow focus strip with Previous/Next commands, and verifies next-Step navigation does not trigger Preview/Run.

Latest UI evidence for structured LLM XML validation rows:

- Before: `artifacts\llm_xml_validation_rows_before_20260705_r1\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_xml_validation_rows_after_20260705_r5\OpenVisionLab_RecipeManager_LlmXml.png`
- Direct EXE smoke: `artifacts\llm_xml_validation_rows_after_20260705_r5\report.txt` with `Result: PASS` and `LlmValidationIssues: visible`.

Latest UI evidence for selected Step input/output layer cards:

- Before: `artifacts\step_layer_navigation_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\step_layer_navigation_after_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\step_layer_navigation_after_20260705_r1\report.txt` with `Result: PASS` and `StepLayerCards: visible`.

Latest UI evidence for selected Step thumbnail cards and click navigation:

- Before: `artifacts\step_layer_click_nav_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\step_layer_click_nav_after_20260705_r4\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\step_layer_click_nav_after_20260705_r4\report.txt` with `Result: PASS` and `StepLayerNavigation: Battery_CellVentAlignment_Preview -> Main`.

Latest UI evidence for Good/Bad role result cards:

- Before: `artifacts\sample_review_drilldown_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\sample_review_drilldown_after_20260705_r2\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\sample_review_drilldown_after_20260705_r2\report.txt` with `Result: PASS` and `PairRoleCards: 2`.

Latest UI evidence for selected Step ROI/template metadata and tool entry:

- Before: `artifacts\recipe_step_roi_template_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\recipe_step_roi_template_after_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\recipe_step_roi_template_after_20260705_r1\report.txt` with `Result: PASS`, `StepRoiTemplate: ROI: 172,166,116,136 | Template: 없음`, and `StepToolEntry: 도구 열기: LineDistance`.

## Latest Original Repo Commits

Key latest stable code/workflow commits in `C:\Git\OpenVisionLab`:

- `9c2bbe1 Show pipeline review parameter focus hints`
- `c90d60a Record pipeline review parameter location hints`
- `2371b37 Add pipeline review parameter location hints`
- `bc42e0e Record pipeline review label polish`
- `71ecc21 Localize pipeline review guide labels`
- `b8a95cf Record catalog audit after metric cleanup`

## Completed On 2026-07-03

- Product sample catalog/native runner gate is stable.
  - Dev evidence: `artifacts\self_evaluation_product_catalog_20260703_1750\sample_catalog_summary.json`
  - Original full evidence: `artifacts\original_product_catalog_full_20260703_1919\sample_catalog_summary.json`
  - Original final evidence: `artifacts\product_catalog_final_20260703_1920\sample_catalog_summary.json`
  - Original after Line cleanup evidence: `artifacts\product_catalog_after_line_controller_cleanup_20260703_1935\sample_catalog_summary.json`
  - Original full result: `GateStatus=OK`, `RunnableRows=168`, `RequiredRows=84`, `ExpectedFailureRows=84`, `OKRows=168`, `NGRows=0`
  - Original final result: `GateStatus=OK`, `RunnableRows=168`, `RequiredRows=84`, `ExpectedFailureRows=84`, `OKRows=168`, `NGRows=0`, `DurationSeconds=81.234`
  - Original after Line cleanup result: `GateStatus=OK`, `RunnableRows=168`, `RequiredRows=84`, `ExpectedFailureRows=84`, `OKRows=168`, `NGRows=0`, `DurationSeconds=70.815`
  - Quality audit: `ProductSampleQualityAudit=PASS | PairRecords=84 OK=84 Review=0 Critical=0`
  - Latest quality audit after Line cleanup: `ProductSampleQualityAudit=PASS | PairRecords=84 OK=84 Review=0 Critical=0`
- Original repo reviewed import, Product Field Explore samples and picker affordance:
  - Imported the reviewed Dev Field Explore sample bundle into `C:\Git\OpenVisionLab` without bulk-copying the repo.
  - Original data/assets added:
    - 16 PNGs under `C:\Git\OpenVisionLab\docs\samples\public\product\field\`
    - `C:\Git\OpenVisionLab\docs\samples\public\product\Product_Field_DarkFeature_Contour.pipeline.xml`
    - `C:\Git\OpenVisionLab\docs\samples\public\product\Product_Field_BrightFeature_Contour.pipeline.xml`
    - `C:\Git\OpenVisionLab\docs\samples\public\product\Product_Field_SurfaceMean.pipeline.xml`
    - 16 `Product_Field_*` Explore rows in `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
    - 16 field provenance rows in `docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv`
  - Original UI/runtime imported:
    - `OpenVisionWorkspaceSampleFocusOption` now exposes a `field` focus only for `ValidationMode=Explore` field-style samples.
    - `OpenVisionWorkspaceSamplePickerViewModel` shows `Explore sample`, reference metric copy, and `ExploratoryGuideText` for Explore rows.
    - `OpenVisionWorkspaceSamplePickerView.xaml` displays `WorkspaceSamplePickerExploreGuide`.
    - `VisionPipelineSampleCatalog` Product source copy now describes Good/Bad plus Field Explore samples.
    - `PipelineViewerScreenshotSmoke` has `wpf_shell_host_workspace_sample_product_field_focus_picker`.
  - Before/after Original UI evidence:
    - Before Field affordance: `C:\Git\OpenVisionLab\artifacts\product_field_explore_original_before_ui_20260703_01\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - After Field affordance: `C:\Git\OpenVisionLab\artifacts\product_field_explore_original_after_20260703_02\wpf_shell_host_workspace_sample_product_field_focus_picker.png`
    - Existing Product focus after: `C:\Git\OpenVisionLab\artifacts\product_field_explore_original_after_20260703_02\wpf_shell_host_workspace_sample_product_focus_picker.png`
  - Original verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_field_focus_picker artifacts\product_field_explore_original_after_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_picker artifacts\product_field_explore_original_after_20260703_02` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed: `PublicSampleAssetCheck=PASS | CatalogRows=184 ManifestAssets=214 Pipelines=87`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_184_original_after_field_import_20260703_01 -SkipRunnerBuild` passed: `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=76.231`.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `rg -n "ChatGPT|C:\\Git\\새 폴더|새 폴더|DALL|OpenAI" docs\samples\public\product docs\samples\OpenVisionLab.ProductSampleCatalog.csv tools\GenerateOpenVisionProductSamples.ps1` returned no matches.
  - Source-target evidence:
    - Field PNG hashes match Dev: 16 files, 0 mismatches.
    - Field pipeline hashes match Dev: 3 files, 0 mismatches.
    - Dev/Original text equality confirmed for Field focus/view/viewmodel/catalog CSV/manifest/README/generator and `VisionPipelineSampleCatalog`.
    - Known deviation: `tools\PipelineViewerScreenshotSmoke\Program.cs` differs from Dev by one non-Field line, `resultCountMetricText`, which was left outside this import scope.
- Dev Tool View code-behind cleanup, text presenter extraction:
  - Added small text presenters so View code-behind no longer owns static localization assignments for Threshold, Filter, and Morphology:
    - `C:\Git\OpenVisionLab_Dev\0. UI\6) Vision Test\Wpf\ThresholdToolTextPresenter.cs`
    - `C:\Git\OpenVisionLab_Dev\0. UI\6) Vision Test\Wpf\FilterToolTextPresenter.cs`
    - `C:\Git\OpenVisionLab_Dev\0. UI\6) Vision Test\Wpf\MorphologyToolTextPresenter.cs`
  - Updated Views:
    - `ThresholdToolWpfView.xaml.cs` now delegates Threshold parameter/mode labels to `ThresholdToolTextPresenter`.
    - `FilterToolWpfView.xaml.cs` now delegates operation/kernel labels to `FilterToolTextPresenter`.
    - `MorphologyToolWpfView.xaml.cs` now delegates operation/kernel/shape labels to `MorphologyToolTextPresenter`, while keeping operation/shape button state in `VisionToolMorphologyInteractionController`.
  - Structure evidence:
    - `rg -n "OpenVisionLanguageService\.T\("` over the three View code-behind files returns no matches; localization calls remain in the presenter classes.
    - Responsibility moved from View code-behind to text presenter layer; existing tool runtime/controller event paths were not changed.
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\FilterToolTextPresenter.cs" "0. UI\6) Vision Test\Wpf\MorphologyToolTextPresenter.cs" "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\ThresholdToolTextPresenter.cs"` passed with CRLF warnings only.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\tool_text_presenters_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_basic_tool artifacts\tool_text_presenters_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_tool artifacts\tool_text_presenters_dev_20260703_01` passed.
  - Dev screenshot evidence:
    - `C:\Git\OpenVisionLab_Dev\artifacts\tool_text_presenters_dev_20260703_01\wpf_filter_morphology_layout_guard.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\tool_text_presenters_dev_20260703_01\wpf_shell_host_threshold_basic_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\tool_text_presenters_dev_20260703_01\wpf_shell_host_threshold_tool.png`
- Dev Tool View code-behind cleanup, Line text presenter and Filter/Morphology event attach:
  - Added `C:\Git\OpenVisionLab_Dev\0. UI\6) Vision Test\Wpf\LineToolTextPresenter.cs`.
  - `LineToolWpfView.xaml.cs` now delegates Line purpose labels, ROI tooltip, purpose hint, and summary text composition to `LineToolTextPresenter`.
  - `VisionToolKernelSizeController` now attaches/detaches kernel text, lock, and preset button events directly.
  - `VisionToolFilterInteractionController` now attaches/detaches Filter type and border type selection events directly.
  - `VisionToolMorphologyInteractionController` now attaches/detaches operation button and shape radio events directly.
  - Removed direct XAML event handler attributes from `FilterToolWpfView.xaml` and `MorphologyToolWpfView.xaml` for those controller-owned paths.
  - Structure evidence:
    - `LineToolWpfView.xaml.cs` no longer contains `VisionToolVerificationText`, `VisionToolChromePresenter.ApplyTooltip`, or direct `presenter.CreateSummary(...)` usage.
    - `FilterToolWpfView.xaml` and `MorphologyToolWpfView.xaml` no longer contain controller-owned `SelectionChanged`, `TextChanged`, `Checked`, `Unchecked`, or `Click` handler attributes, except normal `IsChecked` state.
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\LineToolTextPresenter.cs"` passed with CRLF warnings only.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolKernelSizeController.cs" "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolFilterInteractionController.cs" "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolMorphologyInteractionController.cs" "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml.cs"` passed with CRLF warnings only.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_text_presenter_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_text_presenter_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_text_presenter_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\filter_morph_controller_event_attach_dev_20260703_01` passed.
  - Dev screenshot evidence:
    - `C:\Git\OpenVisionLab_Dev\artifacts\line_text_presenter_dev_20260703_01\wpf_shell_host_line_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\line_text_presenter_dev_20260703_01\wpf_shell_host_line_measure_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\line_text_presenter_dev_20260703_01\wpf_shell_host_line_intersection_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\filter_morph_controller_event_attach_dev_20260703_01\wpf_filter_morphology_layout_guard.png`
- Dev Pipeline Review result presenter extraction:
  - Added `C:\Git\OpenVisionLab_Dev\0. UI\0) MENU\Wpf\OpenVisionPipelineReviewResultPresenter.cs`.
  - `OpenVisionPipelineReviewDocument` now delegates selected-step run log, result summary/detail, Good/Bad pair action text, and pair metric comparison text to the presenter.
  - The document keeps pipeline execution, layer image cache, validation, sample-pair resolution, and View update orchestration.
  - `ResultCount` display now keeps the public-smoke `Result` token while retaining localized operator text, for example `Result (결과 수)` in Korean.
  - Structure evidence:
    - `rg -n "private .*FormatRunLog|private .*FormatResultSummary|private .*FormatResultDetails|private .*ResolvePairMetricComparisonText|private .*ResolvePairActionText|FormatPrimaryMetricText|FormatMetricName|LocalText\(" "0. UI\0) MENU\Wpf\Documents\OpenVisionPipelineReviewDocument.cs"` returns no matches.
    - `OpenVisionPipelineReviewDocument.cs` calls `OpenVisionPipelineReviewResultPresenter.FormatRunLog`, `FormatResultSummary`, `FormatResultDetails`, `ResolvePairActionText`, and `ResolvePairMetricComparisonText`.
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_metrics artifacts\pipeline_review_result_presenter_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_result_presenter_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\pipeline_review_result_presenter_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\pipeline_review_result_presenter_dev_20260703_02` passed.
  - Dev screenshot evidence:
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_result_presenter_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_metrics.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_result_presenter_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_result_presenter_dev_20260703_02\wpf_shell_host_workspace_product_sample_review.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_result_presenter_dev_20260703_02\wpf_shell_host_workspace_product_sample_review_ng.png`
- Dev Pipeline Review operator NG triage UX:
  - Added NG-only operator triage fields to `OpenVisionPipelineReviewGuideState` and `OpenVisionPipelineReviewViewModel`.
  - `OpenVisionPipelineReviewGuidePresenter` now supplies separate cause, adjustment, and rerun texts for NG/acceptance-NG steps.
  - `OpenVisionPipelineReviewView.xaml` shows the triage strip inside the guide detail area only when NG triage text exists.
  - `OpenVisionShellHostStatePresenter`, `OpenVisionShellHostToolTestFacade`, and `OpenVisionShellHostView.TestHooks.cs` expose the new triage texts for smoke evidence.
  - Added localization keys:
    - `PipelineReview.Guide.TriageFailure`
    - `PipelineReview.Guide.TriageAdjustment`
    - `PipelineReview.Guide.TriageRerun`
    - `PipelineReview.Guide.TriageRerunPair`
  - Reduced the lower detail row height in Pipeline Review from `250` to `210` so the input/output preview area remains visible after the triage strip is shown.
  - Before/after Dev UI evidence:
    - Before: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_ux_before_dev_20260703_01\wpf_shell_host_workspace_product_sample_review_ng.png`
    - After: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_ux_after_dev_20260703_02\wpf_shell_host_workspace_product_sample_review_ng.png`
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\pipeline_operator_review_ux_after_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\pipeline_operator_review_ux_after_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_operator_review_ux_after_dev_20260703_02` passed.
    - `git diff --check --` over the touched Pipeline Review/localization/smoke files passed with CRLF warnings only.
  - Structure evidence:
    - `rg -n "PipelineReviewGuideTriage(Failure|Adjustment|Rerun)|ReviewGuideTriage(Failure|Adjustment|Rerun)|HasReviewGuideTriage|TriageRerunPair" "0. UI\0) MENU\Wpf" "Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv" "tools\PipelineViewerScreenshotSmoke\Program.cs"` shows the ViewModel, View, document/test hook, localization, and smoke assertion path.
- Dev MainView/Product sample counterpart affordance:
  - Current-flow evaluation screenshots were refreshed before changing the MainView workflow strip:
    - Picker before/evaluation: `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_eval_dev_20260703_01\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - Open before/evaluation: `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_eval_dev_20260703_01\wpf_shell_host_workspace_sample_product_focus_open.png`
  - Added a direct counterpart sample button to the bottom sample workflow strip:
    - Good samples show `NG 기준 열기`.
    - Bad/NG samples show `OK 기준 열기`.
  - `OpenVisionShellHostSampleWorkflowPresenter` now resolves the opposite Good/Bad sample in the same PairGroup and exposes `CounterpartSampleName`.
  - `OpenVisionShellHostWorkspaceCommandSurface` adds `OpenSampleCounterpartCommand`, reusing `OpenRunnableSampleByName` so the action swaps the sample image/pipeline only and does not run Preview or open a tool.
  - `OpenVisionShellHostView.TestHooks.cs` exposes `CanOpenSampleCounterpartForTest` and `OpenSampleCounterpartForTest`.
  - Added smoke target `wpf_shell_host_workspace_sample_product_counterpart_open`.
  - After Dev UI evidence:
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_after_dev_20260703_01\wpf_shell_host_workspace_sample_product_focus_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_after_dev_20260703_01\wpf_shell_host_workspace_sample_product_counterpart_open.png`
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_open artifacts\mainview_product_flow_after_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_counterpart_open artifacts\mainview_product_flow_after_dev_20260703_01` passed.
    - `git diff --check --` over the touched MainView workflow/smoke files passed with CRLF warnings only.
  - Stable-contract evidence:
    - The counterpart command smoke asserts no active WPF tool, no active native document, no native Preview result, and unchanged `NativePreviewRunCount` after switching samples.
- Self-evaluation document was added.
  - File: `docs\OPENVISIONLAB_SELF_EVALUATION_20260703.md`
  - Conclusion: target-product maturity `4.0/5`; industrial integrated-platform maturity `2.0/5`.
  - Keep the product advantage focused on PropertyGrid tools, transparent layer routes, Preview/Pipeline separation, and sample-backed review.
- MainView/Product sample workflow was improved.
  - After opening a sample, the bottom workflow strip exposes the hint `Pipeline Review에서 NG/OK 기준 열기`.
  - Product group labels are shortened to `Secondary Battery`, `Display`, and `Semiconductor` so the review hint stays visible.
  - Original commit: `b011ee2`
- Pipeline Review operator guide was improved.
  - Final OK no longer implies a misleading "next step"; it points to output/support-layer review and Good/Bad pair comparison.
  - NG review now shows tool-type-specific `우선 확인:` guidance.
  - Original commits: `95ed902`, `e98a0b2`
- Product sample NG review smoke was fixed.
  - `wpf_shell_host_workspace_product_sample_review_ng` now accepts Product catalog samples instead of asserting Public source kind.
  - Original commit: `b0da050`
- Contour teaching preview stale review was fixed.
  - `ContourToolWpfView.RequestThresholdTeachingPreview()` now clears stale result review before teaching preview, matching Blob behavior.
  - Original commit: `bab969e`
- Korean duplicate-key detection was fixed.
  - `WpfPropertyGridAdapter` now detects `같은 키` in duplicate-key messages instead of a mojibake string.
  - Original commit: `01c7aa4`
- Public/product sample review smoke coverage was consolidated.
  - New script: `tools\RunSampleReviewUiSmokes.ps1`
  - The script runs single WPF targets sequentially to avoid the previously observed multi-target suite hang.
  - Required pair coverage now uses public/product representative groups instead of legacy root-only sample groups.
  - Bad-reference audit now requires controlled NG samples and treats legacy comparative bad references as optional/private.
  - Original commit: `6ca54d3`
- Tool View code-behind cleanup continued.
  - `VisionToolKernelSizeController` now owns shared kernel preset click parsing for Filter and Morphology.
  - Filter/Morphology views now use the same Tag-based preset click path instead of separate 3/5/7 handlers.
  - Original commit: `567fefc`
- Filter/Morphology layout smoke was restored.
  - `SelectComboBoxItemText` now accepts the already-selected value as a valid selection.
  - The layout guard now clicks Filter and Morphology kernel preset buttons, so the shared preset handler is covered by smoke.
  - Original commit: `0a2e026`
- Pipeline Review top-card next-action copy was shortened.
  - The long final OK/NG next-action strings now fit the top summary card while the detailed guide still carries the longer explanation.
  - Existing runtime `CONFIG\localization_catalog.tsv` files migrate from the previous default strings to the shorter defaults.
  - Original commit: `5f76663`
- Filter/Morphology code-behind was trimmed.
  - Unused imaging, IO, and OpenCvSharp morphology usings were removed after the shared preset controller extraction.
  - Original commit: `4278e43`
- Self-evaluation evidence was refreshed after the final catalog and UI smoke passes.
  - The self-evaluation conclusion remains unchanged: OpenVisionLab should stay a rule-based OpenCvSharp4 PropertyGrid-centered workbench, not a hardware integration platform.
  - Original commit: `031c347`
- Line tool code-behind cleanup continued.
  - Test-only selected-line configuration now lives in `LineToolInteractionController`; `LineToolWpfView` keeps the public test hooks as thin wrappers.
  - Original commit: `2ed377a`
- Pipeline Review metric wording was clarified.
  - `ResultCount` now displays as `결과 수` in Korean Pipeline Review NG guidance.
  - The Product sample NG review smoke now asserts localized metric display text in the guide detail.
  - Original commit: `dabf398`
- MainView/Product sample user-flow was re-evaluated with current Dev build screenshots.
  - The bottom workflow strip shows product group, Good/Bad direction, NG/OK counterpart action, Pipeline Review, and first-step action.
  - Pipeline Review shows Good/Bad pair context, metric check, checklist, and explicit counterpart-open action.
  - No additional UI change was made in this pass.
- Pipeline Review sample metric explanations were localized.
  - `ResultCount`, `MeanValueAvg`, and `DistanceMmAvg` now use localized display names in Pipeline Review result detail, Good/Bad pair text, metric check, and checklist text.
  - Mean NG fix detail no longer repeats raw `AcceptanceMessage`; it points the operator to input layer, ROI, Mean type, lighting/brightness drift, and target range.
  - `PipelineViewerScreenshotSmoke` now rejects raw expected metric keys in localized guide/detail/pair text.
  - Original commit: `470f863`
- Product sample catalog was re-audited after metric explanation cleanup.
  - Dev and Original audits both passed: `ProductSampleQualityAudit=PASS | PairRecords=84 OK=84 Review=0 Critical=0`.
  - The catalog still has 84 Good rows and 84 Bad rows, with 84 PairGroups and one shared baseline pipeline per pair.
  - No new product samples are warranted before improving review/explanation UX further.
- Tool View code-behind candidate review was performed.
  - `git diff --no-index --ignore-space-at-eol` between Original and Dev WPF Tool View files produced no semantic diff.
  - Current Dev Tool View diff is mostly Dev baseline/line-ending noise; Original already has the reviewed controller/runtime cleanup.
  - No Tool View code change was made in this pass.
- Pipeline Review guide labels were localized.
  - `Good/Bad Pair` now displays as `Good/Bad 쌍`.
  - `Metric Check` now displays as `지표 확인`.
  - Original commit: `71ecc21`
- Pipeline Review NG parameter-location hints were added.
  - NG detail now keeps the reason, first check, and `조정 위치:` in the same operator guide line.
  - Tool-type-specific hints point to the PropertyGrid parameter panel areas for Threshold, Blob, Contour, Line, Mean, Matching/Feature, and generic steps.
  - `PipelineViewerScreenshotSmoke` now requires the localized parameter-location prefix and `파라미터 패널` text in NG guide detail.
  - Original commit: `2371b37`
- Pipeline Review parameter panel focus hints were added.
  - The lower Parameters panel now repeats the selected NG step's `조정 위치:` hint directly above the parameter list.
  - The hint is data-bound from `OpenVisionPipelineReviewGuideState.ParameterFocusText`; it does not trigger Preview or Run.
  - ShellHost test hooks and `PipelineViewerScreenshotSmoke` now verify the same focus text.
  - Original commit: `9c2bbe1`
- Pipeline Review metric gap explanation was added in Dev.
  - Acceptance NG text now keeps the localized metric name, measured value, target range, and adds the target gap such as `511` over max or `67.5` under min.
  - The change is centralized in `OpenVisionPipelineReviewGuidePresenter.FormatAcceptanceMetricNgReason`, so Pipeline Review detail and result detail use the same wording.
  - Current-build before/after screenshots were captured for the generic NG Pipeline Review path.

## Verification Evidence

- Dev build:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed.
- Original build:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed.
- Smoke tool build:
  - Dev: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed.
  - Original: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed.
- Readiness:
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"` passed.
- Reference and sample policy checks:
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed in Dev and Original.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed in Dev and Original.
  - Re-run at 2026-07-03 19:27 KST passed in Dev and Original.
  - Re-run at 2026-07-03 21:16 KST passed in Dev and Original after parameter-location hints.
  - Re-run at 2026-07-03 21:48 KST passed in Dev and Original after parameter focus hints.
- Product sample full catalog:
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\original_product_catalog_full_20260703_1919` passed in Original.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_final_20260703_1920` passed in Original.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_after_line_controller_cleanup_20260703_1935` passed in Original.
  - Dev quality audit after metric cleanup: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -SummaryPath artifacts\self_evaluation_product_catalog_20260703_1750\sample_catalog_summary.json -OutputDir artifacts\product_quality_after_metric_cleanup_20260703_2018 -FailOnCritical` passed.
  - Original quality audit after metric cleanup: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -SummaryPath artifacts\product_catalog_after_line_controller_cleanup_20260703_1935\sample_catalog_summary.json -OutputDir artifacts\product_quality_after_metric_cleanup_20260703_2018 -FailOnCritical` passed.
- Sample review UI smoke runner:
  - Dev current-flow evaluation: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_user_flow_eval_20260703_1959` passed.
  - Dev: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_script_after_auditfix_20260703_1918` passed.
  - Original: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\original_sample_review_ui_smoke_script_after_auditfix_20260703_1919` passed.
  - Original re-run after layout smoke restore: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_after_layout_guard_restore_20260703_1903` passed.
  - Original re-run after Pipeline Review copy shortening: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_after_pipeline_copy_short_20260703_1915` passed.
  - Original final re-run: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_final_20260703_1930` passed.
  - Product sample NG after `ResultCount` wording: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\product_sample_review_ng_metric_display_after_original_20260703_1948` passed.
  - Dev Product OK after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\metric_display_product_ok_after_dev2_20260703_2030` passed.
  - Dev Product NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\metric_display_product_ng_after_dev2_20260703_2031` passed.
  - Dev Mean NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\metric_display_mean_after_dev6_20260703_2031` passed.
  - Dev Line NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\metric_display_line_after_dev3_20260703_2031` passed.
  - Original Product OK after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\metric_display_product_ok_after_original_20260703_2038` passed.
  - Original Product NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\metric_display_product_ng_after_original_20260703_2038` passed.
  - Original Mean NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\metric_display_mean_after_original_20260703_2039` passed.
  - Original Line NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\metric_display_line_after_original_20260703_2039` passed.
  - Dev Pipeline Review label localization: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_labels_after_dev_20260703_2027` passed.
  - Original Pipeline Review label localization: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_labels_after_original_20260703_2028` passed.
  - Dev Mean NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_parameter_location_after_dev_20260703_2105` passed.
  - Dev Line NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\pipeline_review_line_parameter_location_after_dev_20260703_2105` passed.
  - Dev generic NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_parameter_location_after_dev_20260703_2105` passed.
  - Original Mean NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_parameter_location_after_original_20260703_2115` passed.
  - Original Line NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\pipeline_review_line_parameter_location_after_original_20260703_2115` passed.
  - Original generic NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_parameter_location_after_original_20260703_2115` passed.
  - Dev Mean NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_parameter_focus_after_dev_20260703_2135` passed.
  - Dev Line NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\pipeline_review_line_parameter_focus_after_dev_20260703_2135` passed.
  - Dev generic NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_parameter_focus_after_dev_20260703_2135` passed.
  - Original Mean NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_parameter_focus_after_original_20260703_2145` passed.
  - Original Line NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\pipeline_review_line_parameter_focus_after_original_20260703_2145` passed.
  - Original generic NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_parameter_focus_after_original_20260703_2145` passed.
  - Dev metric gap before: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_metric_gap_before_dev_20260703_01` passed.
  - Dev metric gap after: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_metric_gap_after_dev_20260703_01` passed.
  - Dev sample NG gap after: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_metric_gap_sample_ng_after_dev_20260703_01` passed.
  - Dev metric gap build: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed.
- Filter/Morphology guard:
  - Dev: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\filter_morphology_layout_guard_after_dev_20260703_1903` passed.
  - Original: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\filter_morphology_layout_guard_after_original_20260703_1908` passed.
- Pipeline Review OK/NG:
  - Original OK: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review artifacts\pipeline_review_ok_after_smoke_restore_20260703_1906` passed.
  - Original NG: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_after_smoke_restore_20260703_1906` passed.
  - Original OK after copy shortening: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review artifacts\pipeline_review_top_card_short_after_original_20260703_1917` passed.
  - Original NG after copy shortening: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_top_card_short_ng_after_original_20260703_1917` passed.
  - Original final OK: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review artifacts\pipeline_review_ok_final_20260703_1931` passed.
  - Original final NG: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_final_20260703_1931` passed.
- Line tool controller cleanup:
  - Dev: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_controller_test_hook_after_dev_20260703_1932` passed.
  - Dev: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_controller_intersection_after_dev_20260703_1932` passed.
  - Original: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_controller_test_hook_after_original_20260703_1934` passed.
  - Original: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_controller_intersection_after_original_20260703_1935` passed.

## Screenshot Evidence

- Product sample focus after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\sample_workflow_pair_hint_after2_20260703_1821\wpf_shell_host_workspace_sample_product_focus_open.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\original_sample_workflow_pair_hint_after_20260703_1825\wpf_shell_host_workspace_sample_product_focus_open.png`
- Product sample NG review after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\operator_review_pair_flow_after_fix_20260703_1833\wpf_shell_host_workspace_product_sample_review_ng.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\original_product_sample_review_ng_after_fix_20260703_1836\wpf_shell_host_workspace_product_sample_review_ng.png`
- Contour teaching preview clear after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\contour_teaching_clear_after_20260703_1849\wpf_shell_host_contour_tool.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\original_contour_teaching_clear_after_20260703_1851\wpf_shell_host_contour_tool.png`
- PropertyGrid duplicate-key smoke after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\property_grid_duplicate_key_string_after_20260703_1855\wpf_property_grid_matching_combo.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\original_property_grid_duplicate_key_string_after_20260703_1857\wpf_property_grid_matching_combo.png`
- Filter/Morphology layout guard after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\filter_morphology_layout_guard_after_dev_20260703_1903\wpf_filter_morphology_layout_guard.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\filter_morphology_layout_guard_after_original_20260703_1908\wpf_filter_morphology_layout_guard.png`
- Pipeline review OK/NG after:
  - OK: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ok_after_smoke_restore_20260703_1906\wpf_shell_host_pipeline_review.png`
  - NG: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_after_smoke_restore_20260703_1906\wpf_shell_host_pipeline_review_ng.png`
- Pipeline review next-action copy after:
  - OK: `C:\Git\OpenVisionLab\artifacts\pipeline_review_top_card_short_after_original_20260703_1917\wpf_shell_host_pipeline_review.png`
  - NG: `C:\Git\OpenVisionLab\artifacts\pipeline_review_top_card_short_ng_after_original_20260703_1917\wpf_shell_host_pipeline_review_ng.png`
- Final review smoke after:
  - Sample runner: `C:\Git\OpenVisionLab\artifacts\sample_review_ui_smoke_final_20260703_1930`
  - Pipeline OK: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ok_final_20260703_1931\wpf_shell_host_pipeline_review.png`
  - Pipeline NG: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_final_20260703_1931\wpf_shell_host_pipeline_review_ng.png`
- Product sample NG metric wording after:
  - Before: `C:\Git\OpenVisionLab\artifacts\sample_review_ui_smoke_final_20260703_1930\wpf_shell_host_workspace_product_sample_review_ng.png`
  - Dev after: `C:\Git\OpenVisionLab_Dev\artifacts\product_sample_review_ng_metric_display_after_dev_20260703_1946\wpf_shell_host_workspace_product_sample_review_ng.png`
  - Original after: `C:\Git\OpenVisionLab\artifacts\product_sample_review_ng_metric_display_after_original_20260703_1948\wpf_shell_host_workspace_product_sample_review_ng.png`
- MainView/Product sample current-flow evaluation:
  - Dev Product focus: `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_user_flow_eval_20260703_1959\wpf_shell_host_workspace_sample_product_focus_open.png`
  - Dev Product pair handoff: `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_user_flow_eval_20260703_1959\wpf_shell_host_workspace_product_sample_pair_open.png`
- Pipeline Review metric explanation cleanup:
  - Mean before: `C:\Git\OpenVisionLab\artifacts\metric_display_mean_before_original_20260703_2002\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Line before: `C:\Git\OpenVisionLab\artifacts\metric_display_line_before_original_20260703_2002\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Dev Mean after: `C:\Git\OpenVisionLab_Dev\artifacts\metric_display_mean_after_dev6_20260703_2031\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Line after: `C:\Git\OpenVisionLab_Dev\artifacts\metric_display_line_after_dev3_20260703_2031\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original Mean after: `C:\Git\OpenVisionLab\artifacts\metric_display_mean_after_original_20260703_2039\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original Line after: `C:\Git\OpenVisionLab\artifacts\metric_display_line_after_original_20260703_2039\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original Product OK after: `C:\Git\OpenVisionLab\artifacts\metric_display_product_ok_after_original_20260703_2038\wpf_shell_host_workspace_product_sample_review.png`
  - Original Product NG after: `C:\Git\OpenVisionLab\artifacts\metric_display_product_ng_after_original_20260703_2038\wpf_shell_host_workspace_product_sample_review_ng.png`
- Pipeline Review guide label localization:
  - Before: `C:\Git\OpenVisionLab\artifacts\metric_display_mean_after_original_20260703_2039\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_labels_after_dev_20260703_2027\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_labels_after_original_20260703_2028\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
- Pipeline Review parameter-location hints:
  - Original Mean before: `C:\Git\OpenVisionLab\artifacts\pipeline_review_parameter_location_before_original_20260703_2110\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Mean after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_parameter_location_after_dev_20260703_2105\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original Mean after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_parameter_location_after_original_20260703_2115\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Line after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_line_parameter_location_after_dev_20260703_2105\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original Line after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_line_parameter_location_after_original_20260703_2115\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original generic NG before: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_parameter_location_before_original_20260703_2110\wpf_shell_host_pipeline_review_ng.png`
  - Dev generic NG after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_ng_parameter_location_after_dev_20260703_2105\wpf_shell_host_pipeline_review_ng.png`
  - Original generic NG after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_parameter_location_after_original_20260703_2115\wpf_shell_host_pipeline_review_ng.png`
- Pipeline Review parameter focus hints:
  - Dev Mean before: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_parameter_focus_before_dev_20260703_2130\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original Mean before: `C:\Git\OpenVisionLab\artifacts\pipeline_review_parameter_focus_before_original_20260703_2140\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Mean after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_parameter_focus_after_dev_20260703_2135\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original Mean after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_parameter_focus_after_original_20260703_2145\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Line after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_line_parameter_focus_after_dev_20260703_2135\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original Line after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_line_parameter_focus_after_original_20260703_2145\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Dev generic NG after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_ng_parameter_focus_after_dev_20260703_2135\wpf_shell_host_pipeline_review_ng.png`
  - Original generic NG after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_parameter_focus_after_original_20260703_2145\wpf_shell_host_pipeline_review_ng.png`
- Pipeline Review metric gap explanation:
  - Dev generic NG before: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_metric_gap_before_dev_20260703_01\wpf_shell_host_pipeline_review_ng.png`
  - Dev generic NG after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_metric_gap_after_dev_20260703_01\wpf_shell_host_pipeline_review_ng.png`
  - Dev sample Mean NG after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_metric_gap_sample_ng_after_dev_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
- Product field-style sample import:
  - Imported 16 project-authored field-style PNGs into `docs\samples\public\product\field` with clean product/inspection names and 960px max dimension.
  - Added 16 `Product_Field_*` Explore rows to `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`.
  - Added field sample provenance rows to `docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` and the product sample generator manifest output.
  - Added three exploratory baseline pipelines: `Product_Field_DarkFeature_Contour.pipeline.xml`, `Product_Field_BrightFeature_Contour.pipeline.xml`, and `Product_Field_SurfaceMean.pipeline.xml`.
  - Added a `Field` product/tool focus option in the sample picker so field-style samples are not buried in the Product catalog list.
  - Contact sheet: `C:\Git\OpenVisionLab_Dev\artifacts\user_sample_import_review_20260703\imported_field_sample_contact_sheet.png`.
  - Current overlay contact sheet after metric tuning: `C:\Git\OpenVisionLab_Dev\artifacts\field_sample_catalog_20260703_03\field_overlay_contact_sheet.png`.
  - Field rows now carry expected metric ranges based on current runner output: `ResultCount` for contour pipelines and `MeanValueAvg` for the surface mean pipeline.
  - Self-evaluation: keep these 16 samples as `Explore` rows for now. They are useful, more field-like recipe setup examples, but several overlays are intentionally broad and should not be promoted to controlled Good/Bad pairs without tighter per-sample pipelines.
  - UI before/after captures:
    - Before Original: `C:\Git\OpenVisionLab\artifacts\field_sample_focus_before_original_20260703\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - After Dev: `C:\Git\OpenVisionLab_Dev\artifacts\field_sample_focus_after_dev_20260703\wpf_shell_host_workspace_sample_product_focus_picker.png`
  - Verification:
    - `dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform="Any CPU"` passed.
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed in Dev and Original for sample picker capture.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed.
    - Representative runner smoke passed for dark contour, bright contour, and surface mean pipelines under `artifacts\field_sample_smoke_20260703`.
    - Field-only catalog gate after expected metric tuning: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath artifacts\field_sample_catalog_20260703_03\field_sample_catalog.csv -OutputDir artifacts\field_sample_catalog_20260703_03 -SkipRunnerBuild -FailOnExplore` passed with `GateStatus=OK`, `RunnableRows=16`, `OKRows=16`, `NGRows=0`.
    - Full Product catalog after field metric tuning: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_184_after_field_metric_gap_dev_20260703_01 -SkipRunnerBuild` passed with `GateStatus=OK`, `RunnableRows=184`, `OKRows=184`, `NGRows=0`.
    - Public sample policy after metric tuning: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
    - Source/privacy check after rename/import: `rg -n "ChatGPT|C:\\Git\\새 폴더|새 폴더|DALL|OpenAI" docs\samples\public\product docs\samples\OpenVisionLab.ProductSampleCatalog.csv tools\GenerateOpenVisionProductSamples.ps1` returned no matches.

- Tool View code-behind cleanup, Line tool:
  - Moved Line ROI/default-ROI mutation from `0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs` into `0. UI\6) Vision Test\Wpf\Behaviors\LineToolInteractionController.cs`.
  - The View now delegates `EnsureDefaultRoi`, `ApplySelectedLineRoi`, and `SetRoiForTest` to the controller. Existing `VisionToolPropertyChangeController.RefreshAfterExternalUpdate` behavior remains the single path for summary/overlay/preview policy updates.
  - Preserved PropertyGrid-based Line parameter editing and did not add broad base classes or new runtime abstractions.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\LineToolInteractionController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_tool_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_tool_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_tool_controller_refactor_dev_20260703_01` passed.
- Pipeline/Recipe operator review UX, NG next action focus:
  - `OpenVisionPipelineReviewGuidePresenter.ResolveNextActionText` now keeps the existing generic NG instruction but prefixes it with the failed acceptance metric or tool-specific focus area.
  - Example after: `평균 밝기(Mean) 기준 확인 / 파라미터/라우트 조정 후 재리뷰`.
  - `PipelineViewerScreenshotSmoke` now asserts the NG next action contains the localized failed metric name for sample NG and generic acceptance NG review targets.
  - UI before/after captures:
    - Before Dev: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_before_dev_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - After Dev: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_after_dev_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - Generic NG after Dev: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_after_dev_20260703_01\wpf_shell_host_pipeline_review_ng.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\0) MENU\Wpf\OpenVisionPipelineReviewGuidePresenter.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs" "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\LineToolInteractionController.cs" "docs\OPENVISIONLAB_NEXT_SESSION_HANDOFF.md"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_operator_review_after_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_operator_review_after_dev_20260703_01` passed.
- Tool View code-behind cleanup, SimplePreprocess settings restore:
  - `SimplePreprocessParameterController.ApplySettings` now preserves the previous suppress state using its existing `isSuppressed` dependency.
  - `SimplePreprocessToolWpfView.ApplyPersistedSettings` now delegates suppression to the controller and only replays the lightweight `ParameterChanged` path after restore.
  - This keeps dynamic SimplePreprocess parameter restore behavior out of View code-behind without changing the generated parameter UI contract.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\SimplePreprocessParameterController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_simple_preprocess_result_review artifacts\simple_preprocess_controller_refactor_dev_20260703_01` passed.
- MainView/Product sample review, Field Explore affordance:
  - Fixed Field focus filtering in `OpenVisionWorkspaceSampleFocusOption.Matches`; `field` now requires `ValidationMode=Explore` and field-style product tokens instead of falling through the generic LearnPath fallback.
  - Added Field focus smoke target `wpf_shell_host_workspace_sample_product_field_focus_picker`.
  - For Explore samples, `OpenVisionWorkspaceSamplePickerViewModel` now shows `Explore 샘플`, formats expected ranges as reference metrics rather than fixed OK/NG criteria, and exposes a short guide explaining that the sample is for recipe setup rather than a controlled Good/Bad decision pair.
  - `OpenVisionWorkspaceSamplePickerView.xaml` displays the Explore guide in the benchmark strip via `WorkspaceSamplePickerExploreGuide`.
  - Before/after evidence:
    - Initial Field target before focus fix failed: selected `Product_Battery_TabGap_Good` with `Mode=Required` after choosing Field.
    - Field after focus fix, before Explore guide: `C:\Git\OpenVisionLab_Dev\artifacts\product_field_explore_guide_before_dev_20260703_02\wpf_shell_host_workspace_sample_product_field_focus_picker.png`
    - Field after Explore guide: `C:\Git\OpenVisionLab_Dev\artifacts\product_field_explore_guide_after_dev_20260703_01\wpf_shell_host_workspace_sample_product_field_focus_picker.png`
    - Existing Product focus after: `C:\Git\OpenVisionLab_Dev\artifacts\product_field_explore_guide_after_dev_20260703_01\wpf_shell_host_workspace_sample_product_focus_picker.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\0) MENU\Wpf\OpenVisionWorkspaceSampleFocusOption.cs" "0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerView.xaml" "0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerViewModel.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_field_focus_picker artifacts\product_field_explore_guide_after_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_picker artifacts\product_field_explore_guide_after_dev_20260703_01` passed.
- Original repo reviewed patch import, Pipeline Review metric guidance:
  - Imported only the reviewed Pipeline Review metric-gap/NG next-action focus change into `C:\Git\OpenVisionLab`; did not bulk-copy Dev.
  - Original touched files:
    - `C:\Git\OpenVisionLab\0. UI\0) MENU\Wpf\OpenVisionPipelineReviewGuidePresenter.cs`
    - `C:\Git\OpenVisionLab\tools\PipelineViewerScreenshotSmoke\Program.cs`
  - Deferred Field Explore sample/UI import into Original because Original does not yet contain the Product Field sample assets/catalog rows. Importing only the Field UI would expose dead or untestable affordance.
  - Original after capture:
    - `C:\Git\OpenVisionLab\artifacts\pipeline_operator_review_original_after_20260703_01\wpf_shell_host_pipeline_review_ng.png`
    - `C:\Git\OpenVisionLab\artifacts\pipeline_operator_review_original_after_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\0) MENU\Wpf\OpenVisionPipelineReviewGuidePresenter.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_operator_review_original_after_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_operator_review_original_after_20260703_01` passed.
- Product sample catalog quality follow-up, Dev 184-row gate:
  - Re-ran the full Product catalog after the Field Explore import and current review UX changes.
  - Full catalog summary: `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=72.891`.
  - Product sample quality audit passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
  - Evidence:
    - Catalog run output: `C:\Git\OpenVisionLab_Dev\artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_summary.json`
    - Catalog run report: `C:\Git\OpenVisionLab_Dev\artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_report.md`
    - Quality audit report: `C:\Git\OpenVisionLab_Dev\artifacts\product_catalog_quality_followup_audit_dev_20260703_01\product_sample_quality_audit.md`
  - Verification:
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_quality_followup_dev_20260703_01 -SkipRunnerBuild` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -SummaryPath artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_summary.json -OutputDir artifacts\product_catalog_quality_followup_audit_dev_20260703_01 -FailOnCritical` passed.
  - Self-evaluation: no additional product sample generation is needed before stabilizing/importing the current Dev UX and Tool View changes. The remaining sample risk is not quantity, but whether the 16 Field Explore samples should later receive tighter per-sample pipelines before promotion to controlled Good/Bad pairs.
- Tool View code-behind cleanup, single-input PropertyGrid shell layout:
  - Moved docked/floating density and layout mutation out of `VisionToolSingleInputPropertyToolShell.xaml.cs` into `VisionToolSingleInputPropertyToolShell.DockedInspectorLayoutController.cs`.
  - The original shell file now keeps dependency properties, exposed controls, and the `DockedInspectorModeChanged` event path; `ApplyDockedInspectorMode` delegates to the layout controller.
  - `VisionToolSingleInputPropertyToolShell.xaml.cs` is now 189 lines; the extracted layout controller is 150 lines.
  - This is behavior-preserving: no algorithm tool PropertyGrid contract was changed, and no Preview/Run trigger path was added.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\VisionToolSingleInputPropertyToolShell.xaml.cs" "0. UI\6) Vision Test\Wpf\VisionToolSingleInputPropertyToolShell.DockedInspectorLayoutController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_blob_tool artifacts\single_input_property_shell_layout_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_contour_tool_docked_verification artifacts\single_input_property_shell_layout_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_tool artifacts\single_input_property_shell_layout_refactor_dev_20260703_01` passed.
- Tool View code-behind cleanup, double-input custom shell layout:
  - Moved docked/floating preview-card density, input-B visibility, and offset action row layout out of `VisionToolDoubleInputCustomToolShell.xaml.cs` into `VisionToolDoubleInputCustomToolShell.DockedInspectorLayoutController.cs`.
  - The shell file now keeps dependency properties, exposed controls, and public layout commands that delegate to the controller.
  - `VisionToolDoubleInputCustomToolShell.xaml.cs` is now 107 lines; the extracted layout controller is 121 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\VisionToolDoubleInputCustomToolShell.xaml.cs" "0. UI\6) Vision Test\Wpf\VisionToolDoubleInputCustomToolShell.DockedInspectorLayoutController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\double_input_shell_layout_refactor_dev_20260703_01` passed.
- Tool View code-behind cleanup, Line result review controller:
  - Added `LineToolReviewController` to coordinate Line result chips and verification/failure guide updates.
  - `LineToolWpfView.xaml.cs` now delegates Line, Distance, and Intersection result review presentation plus teaching-summary reset to the controller.
  - `LineToolWpfView.xaml.cs` is now 407 lines; the extracted review controller is 102 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\LineToolReviewController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_tool_review_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_tool_review_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_tool_review_controller_refactor_dev_20260703_01` passed.
- Tool View code-behind cleanup, Arithmetic settings restore:
  - `ArithmeticToolInteractionController` now receives the current suppress state and preserves it while applying operation lists or persisted settings.
  - `ArithmeticToolWpfView.ApplyPersistedSettings` now delegates directly to the controller.
  - `ArithmeticToolWpfView.xaml.cs` is now 290 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\ArithmeticToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\ArithmeticToolInteractionController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\arithmetic_controller_suppression_refactor_dev_20260703_01` passed.
- Pipeline/Recipe operator review UX, Step Flow operator focus:
  - Added a small `PipelineReviewStepFlowOperatorFocus` strip inside the Step Flow panel.
  - It reuses `ReviewGuideParameterFocusText`, so the selected NG step shows the operator/parameter location where the user is already choosing the step.
  - This does not add a Tool View launch command; no command surface is available there yet, so the safer improvement is clearer operator focus near the selected step.
  - Before capture: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_operator_focus_before_dev_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - After captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_operator_focus_after_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_operator_focus_after_dev_20260703_02\wpf_shell_host_workspace_product_sample_review.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_operator_focus_after_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\pipeline_review_operator_focus_after_dev_20260703_02` passed.
- MainView/Product sample review current-flow recheck:
  - Rechecked the current Dev build after Tool View and Pipeline Review changes.
  - The product sample workflow strip still shows the explicit counterpart/sample review actions and does not auto-run Preview/Run during open/counterpart switching.
  - Current captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260703_02\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260703_02\wpf_shell_host_workspace_sample_product_focus_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260703_02\wpf_shell_host_workspace_sample_product_counterpart_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260703_02\wpf_shell_host_workspace_sample_product_field_focus_picker.png`
  - Verification:
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_picker artifacts\mainview_product_flow_recheck_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_open artifacts\mainview_product_flow_recheck_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_counterpart_open artifacts\mainview_product_flow_recheck_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_field_focus_picker artifacts\mainview_product_flow_recheck_dev_20260703_02` passed.
  - Self-evaluation: no further Product sample workflow UI work is needed before stabilization. The next value is import/readiness review or additional Tool View cleanup, not more visible copy.
- Tool View code-behind cleanup, Threshold test configuration:
  - `VisionToolThresholdInteractionController` now owns the Basic/Invert test configuration path and preserves the previous suppress state while changing coupled radio buttons.
  - `ThresholdToolWpfView.ConfigureBasicInvertForTest` delegates to the controller.
  - `ThresholdToolWpfView.xaml.cs` is now 221 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolThresholdInteractionController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_basic_tool artifacts\threshold_controller_test_config_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_tool artifacts\threshold_controller_test_config_refactor_dev_20260703_01` passed.
- Tool View code-behind cleanup, Morphology kernel binding flush:
  - Added `VisionToolKernelSizeController.FlushParameterBindings`.
  - `MorphologyToolWpfView` now delegates width/height binding flush to the kernel controller before creating properties or refreshing the summary.
  - `MorphologyToolWpfView.xaml.cs` is now 195 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolKernelSizeController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\morphology_kernel_flush_controller_refactor_dev_20260703_01` passed.
- Dev stabilization checkpoint after Tool View/Pipeline Review/MainView loop:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
- Pipeline/Recipe operator review UX, first NG step navigation:
  - Added a manual `NG Step` button to Pipeline Review next to Previous/Next.
  - The button selects the first enabled step whose review result is NG after explicit `Run Review`; it does not trigger Preview/Run.
  - The button is disabled for OK-only review results and is exposed through shell-host test hooks.
  - The multi-step NG smoke now selects a later OK step, clicks the visible `btnFirstIssueStep` button, verifies that the first NG Threshold step is selected, and confirms no native Preview/Run count increase.
  - Before first-issue button capture:
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_operator_focus_after_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - After captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_first_issue_after_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_first_issue_after_dev_20260703_02\wpf_shell_host_workspace_product_sample_review.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_first_issue_navigation_dev_20260704_01\wpf_shell_host_pipeline_review_ng.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_first_issue_after_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\pipeline_review_first_issue_after_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_first_issue_navigation_dev_20260704_01` passed.
    - `git diff --check -- "0. UI\0) MENU\Wpf\Documents\OpenVisionPipelineReviewDocument.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostStatePresenter.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostToolTestFacade.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostView.TestHooks.cs" "0. UI\0) MENU\Wpf\ViewModels\OpenVisionPipelineReviewViewModel.cs" "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml" "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml.cs" "Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, Line preview controller:
  - Added `LineToolPreviewController` to own Line tool debounced auto-preview scheduling, threshold teaching preview requests, and input ROI overlay refresh.
  - `LineToolWpfView.xaml.cs` now delegates preview/ROI state to the controller and is reduced from 407 lines to 369 lines.
  - This is behavior-preserving: Line still uses explicit Preview/Run requests, and property changes only schedule through the existing `VisionToolPropertyPreviewPolicy`.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_preview_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_preview_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_preview_controller_refactor_dev_20260703_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\LineToolPreviewController.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, Arithmetic preview controller:
  - Added `ArithmeticToolPreviewController` to own debounced auto-preview scheduling and the Offset-mode vs normal Preview request split.
  - `ArithmeticToolWpfView.xaml.cs` now delegates preview scheduling to the controller and is reduced from 290 lines to 276 lines.
  - Behavior is unchanged: Offset mode still uses `Run Offset`, normal mode still uses `Run Preview`, and parameter changes go through the existing `VisionToolParameterChangeController`.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\arithmetic_preview_controller_refactor_dev_20260703_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\ArithmeticToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\ArithmeticToolPreviewController.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, Threshold schedule wrapper removal:
  - Removed the View-local `ScheduleAutoPreview` wrapper from `ThresholdToolWpfView`.
  - Scheduling now goes directly through `VisionToolParameterChangeController` and `VisionToolDebouncedPreviewScheduler`, which already own suppress and loaded-state checks.
  - `ThresholdToolWpfView.xaml.cs` is now 212 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_basic_tool artifacts\threshold_schedule_simplification_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_tool artifacts\threshold_schedule_simplification_dev_20260703_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, SimplePreprocess apply-settings ownership:
  - `SimplePreprocessParameterController.ApplySettings` now owns the post-restore `RefreshProgrammatic(notifyChanged: true)` path.
  - `SimplePreprocessToolWpfView.ApplyPersistedSettings` no longer manually raises `ParameterChanged`; the View is reduced from 285 lines to 283 lines.
  - `PipelineViewerScreenshotSmoke` combo/slider auto-preview mutations now choose a value different from the current persisted setting, so the smoke is not dependent on local settings store state.
  - Verification:
    - Initial `wpf_preprocess_output_preview_flow` run failed because the smoke re-selected an already persisted Filter/RotateScale value and did not trigger a change event.
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors after the smoke fix.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_preprocess_output_preview_flow artifacts\simple_preprocess_apply_settings_controller_dev_20260703_03` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_simple_preprocess_result_review artifacts\simple_preprocess_apply_settings_controller_dev_20260703_03` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_preprocess_existing_output_write artifacts\simple_preprocess_layer_contract_20260704_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\SimplePreprocessParameterController.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, SimplePreprocess parameter facade removal:
  - `SimplePreprocessToolWpfView` now exposes its existing `SimplePreprocessParameterController` through an internal `Parameters` property instead of forwarding every `Add*`, `Get*`, visibility, settings capture, and settings restore method.
  - SimplePreprocess configurator/property/preview/factory code now uses `view.Parameters` directly, so parameter generation and mapping stay in the controller/runtime path.
  - `SimplePreprocessToolWpfView.xaml.cs` is now 180 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_preprocess_output_preview_flow artifacts\simple_preprocess_parameter_facade_refactor_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_simple_preprocess_result_review artifacts\simple_preprocess_parameter_facade_refactor_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_preprocess_existing_output_write artifacts\simple_preprocess_parameter_facade_refactor_20260704_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessDocumentFactory.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessViewConfigurator.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessPropertyFactory.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessPreviewExecutor.cs"` passed with CRLF warnings only.
- Arithmetic route smoke stabilization:
  - `wpf_layer_selection_arithmetic_tool` exposed a WPF `PopupControlService` stale HWND exception while docking the floating Arithmetic tool after combo popup checks.
  - The smoke runner now closes the Arithmetic combo popups before docking and ignores only Win32 error 1400 during dispatcher pump cleanup; behavioral assertions still fail normally from the verification action.
  - Verification:
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\arithmetic_popup_cleanup_stabilized_20260704_01` passed.
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessDocumentFactory.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessViewConfigurator.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessPropertyFactory.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessPreviewExecutor.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
- Dev stabilization checkpoint after first-issue navigation and Tool View preview-controller refactors:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
- Self-evaluation document refresh:
  - `docs\OPENVISIONLAB_SELF_EVALUATION_20260703.md` now references the current Dev product catalog result: `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`.
  - The Product sample catalog score was adjusted from `4.2 / 5` to `4.3 / 5`.
  - Verification:
    - `git diff --check -- "docs\OPENVISIONLAB_SELF_EVALUATION_20260703.md"` passed.
- Product field-style sample catalog follow-up:
  - Confirmed `C:\Git\새 폴더` source images are represented in Dev as 16 renamed field-style images under `docs\samples\public\product\field`.
  - Current repo sample names/catalog rows do not include `ChatGPT` or `OpenAI` markers.
  - Visual contact sheet for review:
    - `C:\Git\OpenVisionLab_Dev\artifacts\field_sample_quality_review_20260704_01\field_sample_contact_sheet.png`
  - Verification:
    - `rg -n "ChatGPT|OpenAI|generated by" "docs\samples\public\product" "docs\samples\OpenVisionLab.ProductSampleCatalog.csv"` found only the README policy line describing deterministic/project-authored samples; no ChatGPT/OpenAI marker is present in catalog/image names.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -SummaryPath artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_summary.json -FailOnCritical` passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_full_recheck_20260704_01 -SkipRunnerBuild` completed with summary `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=62.722`.
- MainView/Product sample review current-flow recheck:
  - Rechecked current Dev build after Pipeline Review first-issue navigation, Tool View cleanup, and sample catalog follow-up.
  - Current captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260704_01\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260704_01\wpf_shell_host_workspace_sample_product_focus_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260704_01\wpf_shell_host_workspace_sample_product_counterpart_open.png`
  - Verification:
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_picker artifacts\mainview_product_flow_recheck_dev_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_open artifacts\mainview_product_flow_recheck_dev_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_counterpart_open artifacts\mainview_product_flow_recheck_dev_20260704_01` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_20260704_01` passed with `Targets=6`.
- Latest Dev stabilization checkpoint after 2026-07-04 00:00 changes:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
- Final Dev recheck on 2026-07-04 before the 02:00 handoff:
  - `git diff --check` passed with CRLF warnings only.
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_final_20260704_01` passed with `Targets=6`.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\final_ui_recheck_dev_20260704_01` passed; capture: `C:\Git\OpenVisionLab_Dev\artifacts\final_ui_recheck_dev_20260704_01\wpf_shell_host_pipeline_review_ng.png`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_full_final_20260704_01 -SkipRunnerBuild` completed with summary `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=61.536`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -SummaryPath artifacts\product_catalog_full_final_20260704_01\sample_catalog_summary.json -FailOnCritical` passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
- Post-00:23 continuation, Pipeline Review progress summary:
  - Added a compact header progress line for Pipeline Review: `OK x / NG y / 대기 z`, plus `OFF z` when disabled steps exist.
  - The progress text is owned by `OpenVisionPipelineReviewDocument.FormatReviewProgressText`, surfaced through the Pipeline Review view/viewmodel, and exposed through ShellHost test hooks for smoke assertions.
  - During current-build visual verification, the first after-capture showed `실행 중...` still present after completion. The run completion path now recalculates progress after `isRunningReview=false` in the common `finally` block.
  - Current before/after captures:
    - Before: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_progress_before_20260704_01\wpf_shell_host_pipeline_review_ng.png`
    - After: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_progress_after_20260704_03\wpf_shell_host_pipeline_review_ng.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_progress_after_20260704_03` passed and now asserts the progress text directly.
    - `git diff --check -- "0. UI\0) MENU\Wpf\Documents\OpenVisionPipelineReviewDocument.cs" "0. UI\0) MENU\Wpf\ViewModels\OpenVisionPipelineReviewViewModel.cs" "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml" "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostStatePresenter.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostToolTestFacade.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostView.TestHooks.cs" "Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
- Post-00:44 continuation, Blob/Contour area review controller cleanup:
  - Added `VisionToolThresholdTeachingPreviewController` for the shared threshold-teaching preview request flag used by Blob and Contour.
  - Added a `VisionToolSingleInputPropertyToolController<TProperty>.ShowAreaResultReview(...)` overload that pairs verification guide update with the existing area result review presenter.
  - `BlobToolWpfView.xaml.cs` and `ContourToolWpfView.xaml.cs` now delegate the duplicated result-list filtering and teaching-preview state to shared runtime/controller code.
  - Code-behind line counts after cleanup: Blob 161, Contour 159.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_blob_tool artifacts\area_review_controller_refactor_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_contour_tool artifacts\area_review_controller_refactor_20260704_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\BlobToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\ContourToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\VisionToolSingleInputPropertyToolController.cs"` passed with CRLF warnings only.
    - `Select-String -LiteralPath "0. UI\6) Vision Test\Wpf\VisionToolThresholdTeachingPreviewController.cs" -Pattern '[ \t]+$'` found no trailing whitespace.
- Post-tool-review stabilization checkpoint:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_post_tool_review_20260704_01 -SkipRunnerBuild` completed with summary `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=68.489`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -SummaryPath artifacts\product_catalog_post_tool_review_20260704_01\sample_catalog_summary.json -FailOnCritical` passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
- MainView/Product sample review post-progress recheck:
  - Rechecked the sample-selection-to-product-review flow after the Pipeline Review progress summary change.
  - Current captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_ui_smoke_post_progress_20260704_01\wpf_shell_host_workspace_sample_product_focus_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_ui_smoke_post_progress_20260704_01\wpf_shell_host_workspace_product_sample_review.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_ui_smoke_post_progress_20260704_01\wpf_shell_host_workspace_product_sample_review_ng.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_ui_smoke_post_progress_20260704_01\wpf_shell_host_workspace_product_sample_pair_open.png`
  - Visual check: the header progress line does not overlap the Good/Bad pair guide; it shows `미실행` before review and `OK 1 / NG 1 / 대기 0` after the controlled NG product review run.
  - Verification:
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_post_progress_20260704_01` passed with `Targets=6`.
- Product sample review progress assertion hardening:
  - Added `AssertPipelineReviewProgressText` in `PipelineViewerScreenshotSmoke` and wired it into Product sample review OK/NG paths.
  - The smoke now verifies the visible progress summary counts and also checks that `실행 중...` does not remain after review completion.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\product_review_progress_assertion_20260704_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\product_review_progress_assertion_20260704_02` passed.
    - `git diff --check -- tools\PipelineViewerScreenshotSmoke\Program.cs` passed with CRLF warnings only.
- Matching-family result review title cleanup:
  - `VisionToolSingleInputMatchingToolController<TProperty>` now owns the result review title supplied at attach time.
  - Matching, EdgeBasedMatching, and FeatureMatching views no longer pass the same title string on every `SetResultReview` call.
  - Code-behind line counts after cleanup: Matching 142, EdgeBasedMatching 137, FeatureMatching 133.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_matching_tool artifacts\matching_review_title_controller_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_edge_based_matching_tool artifacts\matching_review_title_controller_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_feature_matching_tool artifacts\matching_review_title_controller_20260704_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\MatchingToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\EdgeBasedMatchingToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\FeatureMatchingToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\VisionToolSingleInputMatchingToolController.cs"` passed with CRLF warnings only.
- Post-matching-cleanup stabilization checkpoint:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `git diff --check` passed with CRLF warnings only.
- Single-input custom tool shell base refactor:
  - Added `VisionToolSingleInputCustomToolViewBase` to own the repeated single-input custom tool shell/event/status/preview-image command forwarding.
  - Switched `ThresholdToolWpfView`, `FilterToolWpfView`, `MorphologyToolWpfView`, and `SimplePreprocessToolWpfView` XAML roots from `UserControl` to the shared base so the generated WPF partial classes share the same controller plumbing.
  - Removed repeated forwarding code from the four code-behind files while keeping each tool's parameter UI, presenter/controller setup, and explicit Preview/Run path unchanged.
  - Code-behind line counts after this cleanup: Filter 121, Morphology 116, Threshold 129, SimplePreprocess 86. Shared base is 165 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard,wpf_shell_host_threshold_tool,wpf_simple_preprocess_result_review artifacts\custom_tool_base_refactor_20260704_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_threshold_tool,wpf_preprocess_output_preview_flow,wpf_layer_selection_preprocess_existing_output_write artifacts\custom_tool_base_refactor_route_20260704_01` passed.
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `git diff --check` passed with CRLF warnings only.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\VisionToolSingleInputCustomToolViewBase.cs" "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs"` passed with CRLF warnings only.
  - Current captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_20260704_02\wpf_filter_morphology_layout_guard.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_20260704_02\wpf_shell_host_threshold_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_20260704_02\wpf_simple_preprocess_result_review.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_route_20260704_01\wpf_layer_selection_threshold_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_route_20260704_01\wpf_preprocess_output_preview_flow.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_route_20260704_01\wpf_layer_selection_preprocess_existing_output_write.png`
- Pipeline Review flow status badge improvement:
  - `OpenVisionPipelineReviewDocument.ResolveFlowStatus` now maps completed successful review steps to `Passed` and NG/acceptance-NG review steps to `Failed`.
  - This uses the existing `PipelineFlowView` OK/NG badge colors instead of leaving completed review rows as generic `LOAD/WAIT`, making the left Step Flow usable as an operator review map.
  - UI evidence:
    - Before capture with the old LOAD/WAIT mapping: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_flow_status_before_20260704_01\wpf_shell_host_pipeline_review_ng.png`
    - After capture with the NG flow badge visible: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_flow_status_after_20260704_01\wpf_shell_host_pipeline_review_ng.png`
  - Verification:
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_flow_status_before_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_flow_status_after_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\product_review_flow_status_after_20260704_01` passed.
  - Product sample review recheck capture:
    - `C:\Git\OpenVisionLab_Dev\artifacts\product_review_flow_status_after_20260704_01\wpf_shell_host_workspace_product_sample_review_ng.png`
- Product catalog recheck after Tool/Pipeline UX changes:
  - Verification:
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_after_base_and_flow_20260704_01 -SkipRunnerBuild` passed.
    - Summary: `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=63.456`, `ArtifactIssueCount=0`, `MetadataIssueCount=0`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -SummaryPath artifacts\product_catalog_after_base_and_flow_20260704_01\sample_catalog_summary.json -FailOnCritical` passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
- Final Dev verification checkpoint at 2026-07-04 01:21 +09:00:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
  - `git diff --check` passed with CRLF warnings only.
- Custom tool extension guide update:
  - `docs\VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.md` now names `VisionToolSingleInputCustomToolViewBase`, `AttachToolController(...)`, and the rule that single-input custom UI views must not copy event/status/preview forwarding or call `VisionToolSingleInputCustomToolRuntime` directly.
  - Verification:
    - `git diff --check -- docs\VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.md` passed with CRLF warnings only.
- Original repo recheck at 2026-07-04 01:21 +09:00:
  - `git fetch origin` completed in `C:\Git\OpenVisionLab`.
  - Original remains dirty with a subset of earlier sample/catalog/Pipeline Review/Tool View changes and field sample files.
  - Latest original commits remain:
    - `e11b724 Record pipeline review parameter focus hints`
    - `9c2bbe1 Show pipeline review parameter focus hints`
    - `c90d60a Record pipeline review parameter location hints`
    - `2371b37 Add pipeline review parameter location hints`
    - `bc42e0e Record pipeline review label polish`
  - No Dev-to-Original import was performed for the latest custom tool base refactor or Pipeline Review flow status badge change.
- Original repo status check:
  - `C:\Git\OpenVisionLab` is already dirty with a subset of sample/catalog/Pipeline Review/Tool View changes.
  - No bulk copy from Dev was performed in this checkpoint.
  - `git fetch origin` completed with no output.
  - Dev changes not yet present in Original include Pipeline Review first-issue navigation and the latest Tool View preview-controller cleanup. Import these later as reviewed patch groups, not as a bulk folder copy.
  - Verification in `C:\Git\OpenVisionLab`:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
- Original repo recheck after the post-00:23 Dev continuation:
  - `git fetch origin` completed.
  - `C:\Git\OpenVisionLab` remains dirty with a subset of earlier sample/catalog/Pipeline Review changes and field sample files.
  - Latest original commits:
    - `e11b724 Record pipeline review parameter focus hints`
    - `9c2bbe1 Show pipeline review parameter focus hints`
    - `c90d60a Record pipeline review parameter location hints`
    - `2371b37 Add pipeline review parameter location hints`
    - `bc42e0e Record pipeline review label polish`
  - No Dev-to-Original import was performed for the latest Pipeline Review progress summary, Blob/Contour cleanup, Product review progress smoke assertion, or Matching-family title cleanup.
  - Import only by reviewed patch groups after choosing the target group; do not copy the Dev tree over Original.

## Dev To Original Import Groups

Do not import the whole Dev tree. Review and move the current Dev changes in small groups:

1. Product field sample catalog
   - Candidate files: `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`, `docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv`, `docs\samples\public\product\field\*`, `docs\samples\public\product\Product_Field_*.pipeline.xml`, `0. UI\6) Vision Test\VisionPipelineSampleCatalog.cs`, and sample generation/policy docs.
   - Required checks after import: product asset policy, full product catalog gate, sample quality audit.
2. MainView/Product sample review UX
   - Candidate files: `OpenVisionShellHostSampleWorkflowPresenter`, sample picker view/viewmodel, sample focus/pair decision helpers, shell command surface, and related UI smoke updates.
   - Required checks after import: `tools\RunSampleReviewUiSmokes.ps1` and current-build screenshots.
3. Pipeline Review operator UX
   - Candidate files: Pipeline Review document/view/viewmodel, guide/result presenters, localization keys, shell host test hooks, and PipelineViewerScreenshotSmoke updates.
   - Required checks after import: `wpf_shell_host_pipeline_review_ng`, product sample review target, and no Preview/Run count increase on `NG Step`.
4. Tool View controller cleanup
   - Candidate files: preview/review/text/controller classes and the touched Tool View code-behind files.
   - Required checks after import: focused Tool View WPF smokes for Threshold, Line, Arithmetic, SimplePreprocess, Filter/Morphology.
5. Policy/runtime cleanup
   - Candidate files: external reference/readiness scripts, project references, native DLL placement, and policy docs.
   - Required checks after import: solution build, readiness, external references, public sample assets.

## Start Checklist

```powershell
cd C:\Git\OpenVisionLab_Dev
git status --short
git log --oneline -5

cd C:\Git\OpenVisionLab
git fetch origin
git status --short
git log --oneline -5
```

## 2026-07-14 Learn User-Copy Audit

- Audited the common Learn header, all 17 Learn topics, the OpenCvSharp foundations visualization, and every user-openable file under `docs\learn`.
- Removed learner-facing engineering agreements such as no-auto-run/routing invariants, smoke/readiness wording, runtime-contract wording, and scope/backlog notes. Those rules remain in `AGENTS.md`, engineering contracts, and regression checks.
- Rewrote the common practice panel around the operator workflow: open a Good/Bad pair, open the related tool, run Preview or Pipeline Review, then compare the input, output, overlay, and metric.
- Reworked the previously unclear foundations view into two explicit visual sequences:
  - BGR pixel -> B/G/R channels -> Gray GV.
  - Point/Size -> Rect/RotatedRect -> ROI and PropertyGrid values.
- Updated the readiness contract so it checks positive learner guidance and rejects internal engineering phrases instead of requiring those phrases in user documentation.
- Added the same forbidden-copy regression across all 17 topic windows and their resolved Learn documents in `PipelineViewerScreenshotSmoke`.
- Current-source UI evidence:
  - Before: `C:\Git\OpenVisionLab_Dev\artifacts\learn_copy_audit_20260714\before_shell\wpf_shell_host_learn_entry.png`.
  - After, same shell target: `C:\Git\OpenVisionLab_Dev\artifacts\learn_copy_audit_20260714\after_shell\wpf_shell_host_learn_entry.png`.
  - After, latest full BGR/Gray foundations detail: `C:\Git\OpenVisionLab_Dev\artifacts\learn_copy_audit_20260714\after_final_curriculum\wpf_openvision_learn_curriculum.png`.
  - A final shell rerun also passed all assertions, but its `RenderTargetBitmap` intermittently saved partially black regions. The invalid `after_final_shell` PNG is excluded from visual evidence; this is a screenshot-tool capture issue, not a reproduced product-window defect.
- Verification:
  - `wpf_openvision_learn_curriculum`: `OK`, `layout=0`, `text=0`, `internal=0`, `1040x980`.
  - `wpf_shell_host_learn_entry`: `OK`, `layout=0`, `text=0`, `internal=0`, `1040x980`.
  - Learn user-facing internal-copy audit: `PASS`, matches `0`.
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: passed with 0 warnings and 0 errors.
  - `dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"`: passed.
  - `tools\TestExternalReferences.ps1`: passed.
- `tools\TestPublicSampleAssets.ps1`: passed with `CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`.
- Latest current-workspace EXE: `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-15 10:06:25 KST`.

## 2026-07-15 Recipe/Pipeline Responsibility Round Trip

- Recipe Manager remains the recipe library and summary surface; Pipeline Review remains the explicit execution and evidence surface.
- Pipeline Review now shows the owning recipe, keeps `Run Review` explicit, and provides `Return to Recipe` in its header.
- Returning closes Pipeline Review and reopens the same Recipe Manager summary. It does not run native Preview, create/remove layers, change the active layer, or change recipe/pipeline routing.
- Current-source UI evidence:
  - Before: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_recipe_roundtrip_20260715\before\wpf_shell_host_workspace_sample_pipeline_review_metrics.png`.
  - After Pipeline Review: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_recipe_roundtrip_20260715\after_pipeline_retry\wpf_shell_host_workspace_sample_pipeline_review_metrics.png`.
  - After returned Recipe Manager summary: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_recipe_roundtrip_20260715\after_summary\wpf_shell_host_recipe_manager_summary.png`.
- Latest-EXE evidence:
  - Pipeline Review: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_recipe_roundtrip_20260715\direct_exe_roundtrip_retry\OpenVisionLab_PipelineReview_Roundtrip.png`.
  - Returned Recipe Manager summary: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_recipe_roundtrip_20260715\direct_exe_roundtrip_retry\OpenVisionLab_RecipeManager_Roundtrip_Return.png`.
  - Report: `Result: PASS`, explicit review `OK`, native Preview runs `0`, layer count preserved at `1`.
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: passed with 0 warnings and 0 errors.
  - Direct EXE `recipe-pipeline-roundtrip`: passed.
  - Existing direct EXE `recipe-manager-tabs`: passed after the new flow was added.
  - Current-source `wpf_shell_host_recipe_manager_summary`: passed with `layout=0`, `text=0`, `internal=0`; the assertion also docks Pipeline Review before using `Return to Recipe`, covering the docked close path.
  - Current-source `wpf_shell_host_workspace_sample_pipeline_review_metrics`: passed with `layout=0`, `text=0`, `internal=0`.

## 2026-07-15 Recipe Manager Sample Evidence Semantics

- The Recipe Manager sample selector is a workspace-global public/product catalog selection and defaults to the first runnable sample. It is not a persisted link to every selected recipe.
- The summary now labels that value `현재 작업 샘플` and explains that a sample check is required before it becomes recipe evidence.
- `OpenVisionRecipeSampleRunSummary` now keeps the recipe and pipeline used by the sample execution. The summary displays the compact result only when both match the selected recipe and pipeline; otherwise it shows `아직 검사하지 않음`.
- Opening Pipeline Review, running isolated Review, and returning to Recipe Manager do not create sample-check evidence or cause the global sample name to appear as the recipe's latest result.
- Current-source evidence is under `artifacts\recipe_manager_sample_semantics_20260715`: `before` reproduces the false implication and `after_final` shows the corrected card. The focused smoke passed with `layout=0`, `text=0`, and `internal=0`.
- Latest EXE proof used `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-15 11:07:05 KST`. `direct_exe_final\report.txt` passed with `RecipeSampleExecution=False`, `RecipeSampleResult=아직 검사하지 않음`, native Preview runs `0`, and layer count `1`; `direct_exe_regression_final\report.txt` also passed the full Recipe Manager regression.

## 2026-07-15 Pipeline Review Input-State Semantics

- Pipeline Review no longer labels every unexecuted Step as the same condition. An enabled Step with no input image and no earlier enabled producer now shows `입력 없음`.
- If an earlier enabled Step will produce the selected input layer, the downstream Step remains `WAIT` and the guide says explicit Review will create that input.
- The new status is read-only. Step selection did not increase native Preview runs or create/change layers and routing.
- Current-source before/after evidence is under `artifacts\pipeline_review_input_state_20260715\before_final` and `after_final_valid`. Both focused captures passed with `layout=0`, `text=0`, and `internal=0`; `producer_wait_regression` passed the existing three-Step execution path.
- Latest EXE proof used `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-15 11:29:05 KST`. `direct_exe\report.txt` passed with `PendingProducedInput=WAIT / 리뷰 실행으로 이전 Step 입력 생성`, review OK, native Preview runs `0`, and layer count `1`.
- The first full `recipe-manager-tabs` attempt failed while locating the pipeline-filter TextBox before any Pipeline input-state assertion. Immediate `direct_exe_regression_retry` rerun passed the full scenario, so keep the one-off result as direct-smoke UI timing sensitivity rather than product evidence.

## Next Priorities

1. Real external LLM XML correction-loop evidence, only when an API key or manually exported transcript is available. Do not fabricate evidence or spend model work while the prerequisite is absent.
2. Add one compact contextual Learn entry from the selected Pipeline Review tool when a matching Learn topic exists. Opening Learn must not change parameters, create layers, alter routing, or run Preview/Run.
3. Recheck Recipe Manager only when a real operator recipe or fresh current-build capture proves clipping, overlap, duplicated responsibility, or unclear next action.
4. Broaden pipeline branch/output review only when a real recipe exceeds the current comparison coverage.
5. Resume Tool View code-behind cleanup only when a visible defect or established controller/runtime owner justifies it; do not refactor for line-count reduction.

## 2026-07-15 P6 Run History Batch Analytics

- Audited the existing batch summary, Validation Suite, Run History, baseline comparison, and structured run-report paths before implementation.
- Batch rows already persist per-sample `TotalMilliseconds`. The selected saved run now derives judgement failure rate plus performance average, median, nearest-rank p95, and maximum without changing the saved XML/TSV schema or adding telemetry infrastructure.
- The existing `Benchmark 회귀 비교` summary shows the analytics as one compact read-only line above the outcome comparison. Correctness and performance labels remain separate.
- At this checkpoint the audit found that `VisionPipelineBatchSampleRunResult.RunReportPath` was not populated by the sample-suite path. The later Step-report persistence slice below closes that evidence gap.
- Current-source before: `artifacts\p6_benchmark_analytics_20260715\before\wpf_shell_host_recipe_local_validation_set.png`.
- Current-source after: `artifacts\p6_benchmark_analytics_20260715\after_final\wpf_shell_host_recipe_local_validation_set.png`; focused smoke passed with `layout=0`, `text=0`, and `internal=0`.
- Latest EXE proof used `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-15 12:05:58 KST`. Direct `recipe-manager-tabs` passed under `artifacts\p6_benchmark_analytics_20260715\direct_exe_final` and reported `RunHistoryAnalytics: 판정 실패율 50.0% | 성능 평균 2.8 ms · 중앙 2.8 ms · p95 3.1 ms · 최대 3.1 ms`.
- The first two attempted after captures ran a stale screenshot-tool copy because `--no-build` was used after only the solution build; they are not valid evidence. The screenshot-tool project was then rebuilt explicitly before the valid `after_final` evidence.

## 2026-07-15 P6 Compatible Baseline Timing

- Selected-sample, Good/Bad pair, Catalog, and Local Validation Set saves now carry distinct suite kinds.
- Run History compares baseline-to-current average and p95 only when suite kind, suite name, and the sorted sample-image multiset match and both runs have complete positive timings.
- Outcome comparison is intentionally independent. Switching to a different-suite baseline still produced `Still NG`, but the summary explicitly skipped timing; restoring the compatible baseline restored `Regression` and `+0.3 ms` average/p95 deltas.
- No new panel, storage schema, regression gate, Preview/Run action, layer mutation, or routing change was added.
- Fresh latest-EXE before: `artifacts\p6_baseline_timing_comparison_20260715\before_exe_01\OpenVisionLab_RecipeManager_RunHistory.png`.
- Fresh latest-EXE after: `artifacts\p6_baseline_timing_comparison_20260715\after_exe_04\OpenVisionLab_RecipeManager_RunHistory.png`.
- `after_exe_01` and `after_exe_03` passed behavior checks but their Run History images were mostly black from WPF composition timing; do not use them as UI evidence. `after_exe_02` rendered correctly but predates the final empty-suite guard.
- Latest EXE/DLL timestamp is `2026-07-15 12:26:00 KST`. Direct `recipe-manager-tabs` passed and reported `RunHistoryPerformanceComparison` with average `2.5 -> 2.8 ms (+0.3)` and p95 `2.8 -> 3.1 ms (+0.3)`.
- Current-source local Validation Set smoke passed with `layout=0`, `text=0`, and `internal=0` under `artifacts\p6_baseline_timing_comparison_20260715\after_current_source_02`.
- The next P6 evidence gate at this checkpoint was the real `RunReportPath` persistence path. The later Step-report persistence slice below closes it.

## 2026-07-15 P6 Structured Step-Report Link

- Added metadata-only `VisionRecipeRunResult` persistence through the existing `VisionPipelineRunReport` schema, including the pipeline snapshot and per-Step elapsed time/metrics without duplicating images.
- Selected-sample, Good/Bad pair, Catalog, and Local Validation Set suite executions now save one structured report per sample and copy its path into `VisionPipelineBatchSampleRunResult.RunReportPath`.
- The plain single `Run check` path remains non-persisting, so the change adds no hidden history side effect to the smallest explicit check.
- Latest solution build passed with 0 warnings and 0 errors. Latest direct `recipe-manager-tabs` smoke passed under `artifacts/p6_step_report_link_20260715/direct_exe_current_build` and preserved `validation-suite-step-report/report.xml` plus `pipeline.xml`; the report contained one linked Step and the existing Run History analytics/performance-comparison assertions also passed.
- The following P6 per-Step bottleneck slice completed this next action.

## 2026-07-15 Commercial Vision UI Reference Review

- Captured official public authoring UI evidence for MVTec MERLIC Creator, Cognex VisionPro QuickBuild, Zebra Aurora Vision Studio, NI Vision Builder AI, and KEYENCE XG VisionEditor under `artifacts/commercial_vision_ui_reference_20260715`.
- The reusable comparison and source links are recorded in `docs/OPENVISIONLAB_COMMERCIAL_UI_REFERENCE_REVIEW_20260715.md`.
- Common useful pattern: keep the flow, selected Step parameters, image evidence, result meaning, and execution timing coherent. OpenVisionLab already has the structural surfaces; the next value is linked Step bottleneck evidence, not another Recipe Manager workspace.
- Explicitly rejected scope: camera/lighting/controller/PLC/I/O/deployment/HMI/account features, auto-execution, and replacing the ordered PropertyGrid Pipeline with a free-form graph.

## 2026-07-15 P6 Per-Step Run History Bottleneck

- `VisionPipelineBatchRunSummaryStorage.CalculateStepTimingAnalysis` loads linked reports only when every batch row has a path and readable file, recipe/pipeline identity matches, and Step index/name/tool/enabled/input/output definitions agree.
- The existing Run History now shows one compact `Step 병목` list ordered by p95. Each row contains Step index/name/tool, timing coverage, average, p95, and maximum; incomplete or incompatible coverage shows a reason and no partial rows.
- The analysis and selection path is read-only. It does not trigger Preview/Run, load images, create layers, or change routing.
- Current-source before: `artifacts/p6_step_bottleneck_20260715/before/wpf_shell_host_recipe_local_validation_set.png`.
- Current-source after: `artifacts/p6_step_bottleneck_20260715/after_final/wpf_shell_host_recipe_local_validation_set.png`; focused smoke passed with `layout=0`, `text=0`, and `internal=0` and verified both complete 4/4 report coverage and missing-path rejection.
- Latest-build direct `recipe-manager-tabs` passed under `artifacts/p6_step_bottleneck_20260715/direct_exe`; the smoke verified one linked selected-sample Step aggregate and rejected synthetic batch history without `RunReportPath`.
- The following P7 selected-Step coherence audit completed this next action.

## 2026-07-15 P7 Selected-Step Workspace Coherence

- Audited the real three-Step `Public_Matching_FixturePad` sample after explicit Pipeline Review execution with Step 2 selected.
- Step identity, Blob tool, `Main -> FixturePadBlob` route, branch explanation against `FixtureMatch`, both previews, `CvROI`/Fixture frame parameters, Fixture result metrics, and elapsed time were coherent.
- The visible gap was duplicate ordinal text (`02 02 Inspect Fixture Pad`) plus a narrow Step summary. The document/presenter now normalize an already-prefixed Step name and the existing summary grid gives the Step card enough width; no new panel or command was added.
- Current-source before: `artifacts/p7_selected_step_coherence_20260715/before/wpf_shell_host_workspace_sample_fixture_review.png`.
- Current-source after: `artifacts/p7_selected_step_coherence_20260715/after_r4/wpf_shell_host_workspace_sample_fixture_review.png`; focused smoke passed with `layout=0`, `text=0`, and `internal=0` and asserts Step/tool/route/previews/parameters/result/timing coherence.
- Latest-build `OpenVisionLab.exe` smoke: `artifacts/p7_selected_step_coherence_20260715/direct_exe_final_r2`; report passed and recorded `Selected Step: 02 Inspect Fixture Pad / Main -> FixturePadBlob` plus Fixture metrics and elapsed time.
- Two current-source capture attempts and the first final EXE capture rendered mostly black despite passing behavior checks; repeated captures produced normal frames. Treat those files as invalid screenshot evidence, not as current UI.
- The following P8 contextual Learn entry completed this next action.

## 2026-07-15 P8 Pipeline Review Contextual Learn Entry

- Pipeline Review now shows one compact `도구 배우기` command when the selected Step ToolType has a matching Learn topic. Unsupported review-only Steps such as `OverlayMerge` do not receive a misleading Learn entry.
- The command reuses the existing Learn window and topic catalog. Selecting the Fixture sample's Blob Step opens topic `5. Blob / 영역 검출`; no duplicate Learn surface or new review panel was added.
- View code-behind only forwards the click. Topic availability belongs to the Learn catalog, review selection state belongs to the Pipeline Review ViewModel/Document, and the shell command controller owns opening/reusing Learn.
- Opening and closing Learn does not run Preview/Run, add/delete layers, change the active layer or native routing, change Step parameters, alter the selected Step, or reset the completed review result.
- Fresh current-source before: `artifacts/p8_pipeline_contextual_learn_20260715/before/wpf_shell_host_workspace_sample_fixture_review.png`.
- Fresh current-source after: `artifacts/p8_pipeline_contextual_learn_20260715/after_r3/wpf_shell_host_workspace_sample_fixture_review.png`; the focused smoke passed with `layout=0`, `text=0`, and `internal=0`, opened Blob Learn, rejected the unsupported OverlayMerge entry, and restored the same selected Step. The first `after` frame was mostly black from WPF composition timing and is excluded from visual evidence.
- Latest-build EXE proof: `artifacts/p8_pipeline_contextual_learn_20260715/direct_exe`; `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe` opened the Blob Learn topic and returned to the same completed review state. The report and both actual-EXE screenshots passed.
- Next evidence-based priority: audit the failed/selected Step -> Tool View adjustment handoff. Implement a direct entry only if the current Step parameters can be loaded with clear edit/apply semantics and without changing routing or running Preview automatically.

## 2026-07-15 P9 Selected-Step Parameter Edit Handoff

- The Pipeline Review selected-Step parameter panel now provides one compact `설정 수정` command.
- The command closes Pipeline Review and opens the same recipe, pipeline, and 1-based Step in Recipe Manager `Advanced > Pipeline > XML/Step`, with the existing PropertyGrid editor visible in the current viewport.
- Recipe Manager remains the authoritative existing-Step edit/apply surface. The separate Tool View remains a detached tool session and is not presented as though it automatically writes settings back into a pipeline Step.
- Opening the edit handoff does not run Preview/Run, create/delete/load layers, change the active layer, or alter pipeline routing. Applying the edited parameters remains an explicit Recipe Manager XML action.
- Fresh current-source evidence:
  - Before: `artifacts/p9_pipeline_step_edit_handoff_20260715/before/wpf_shell_host_workspace_sample_fixture_review.png`.
  - After review command: `artifacts/p9_pipeline_step_edit_handoff_20260715/final_current_source_review/wpf_shell_host_workspace_sample_fixture_review.png`.
  - After handoff destination: `artifacts/p9_pipeline_step_edit_handoff_20260715/final_current_source/wpf_shell_host_pipeline_step_edit_handoff.png`.
- Latest-EXE evidence:
  - Pipeline Review: `artifacts/p9_pipeline_step_edit_handoff_20260715/final_direct_exe/OpenVisionLab_PipelineReview_Roundtrip.png`.
  - Recipe Manager Step editor: `artifacts/p9_pipeline_step_edit_handoff_20260715/final_direct_exe/OpenVisionLab_RecipeManager_Step_Edit_Handoff.png`.
  - Direct `recipe-pipeline-roundtrip` report passed with `StepEditHandoff: 2 / Threshold`, native Preview runs `0`, layer count `1`, and no recipe sample execution side effect.
- The intended-workbench maturity estimate remains 62-66%. This is a bounded workflow-clarity improvement, not evidence for a percentage increase.
- Next evidence-based priority: validate `설정 수정 -> PropertyGrid edit -> explicit XML apply -> explicit rerun` with one real operator recipe and inspect whether any correction evidence is still unclear. Do not add a second editor or automatic apply path without that evidence.

## 2026-07-15 P10 Fixture Step Edit, Apply, And Explicit Rerun

- The real public `Public_Fixture_Pad_Good` workflow exposed one context defect: entering selected-Step edit correctly opened the Fixture Blob PropertyGrid, but Recipe Manager could retain an unrelated prior/default work sample. The following Good/Bad command then executed that unrelated pair.
- `FocusPipelineStepForEdit` now aligns `SelectedSampleOption` only when the requested pipeline exactly matches a runnable catalog workspace pipeline name (`Sample_<catalog sample name>`). Ordinary recipe pipelines with no exact catalog match keep their current work-sample selection.
- The verified operator round trip is `Pipeline Review Step 2 Blob -> 설정 수정 -> MIN_AREA 700 to 750 -> explicit XML 반영 -> explicit Good/Bad 재검사`.
- Typing and XML apply do not run native Preview/Run, create or remove layers, change the active layer, or change input/output routing. The Good/Bad pair runs only after the explicit rerun command.
- Current-source before: `artifacts/p10_fixture_step_edit_roundtrip_20260715/before/wpf_shell_host_pipeline_step_edit_handoff.png`.
- Current-source after: `artifacts/p10_fixture_step_edit_roundtrip_20260715/current_source_final_current_build/wpf_shell_host_fixture_step_edit_apply_rerun.png`; the focused target `wpf_shell_host_fixture_step_edit_apply_rerun` passed with `layout=0`, `text=0`, and `internal=0`.
- Latest EXE proof used `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, timestamp `2026-07-15 16:56:36 KST`, under `artifacts/p10_fixture_step_edit_roundtrip_20260715/exe_after_final_current_build`.
- The direct `public-fixture-review` report passed with `MIN_AREA 700 -> 750`, work sample `Public_Fixture_Pad_Good`, and pair result `쌍 검사 OK / Public_Fixture_Pad_Good, Public_Fixture_Pad_Missing_Bad`.
- The latest `recipe-pipeline-roundtrip` regression under `artifacts/p10_fixture_step_edit_roundtrip_20260715/exe_no_catalog_regression` passed with the ordinary `Direct_Recipe_Roundtrip` pipeline retaining its prior work sample and producing no recipe-sample execution side effect.
- The intended-workbench maturity estimate remains 62-66%. This closes a real workflow context defect but does not broaden product scope.
- Next evidence-based priority: use a real external LLM transcript when available. Without one, inspect fresh current-build Recipe Manager/LLM Assistant evidence for actual clipping, overlap, or unclear next action; audit branch/output comparison only when a real multi-branch recipe demonstrates a gap.

## 2026-07-15 P11 Public GPT Pin-Gap Packet

- The manual GPT packet now uses only project-authored public images: `docs/samples/public/Line_Pins_Synthetic_OK.png` and `Line_Pins_Synthetic_WidePin_NG.png`. The local `Sample/EasyGauge/Pin 1.jpg` vendor/legacy path is no longer part of the external-send instructions.
- The first GPT round requires only the two images plus the complete contents of `llm_prompt_packets/pin_gap_distance/COPY_THIS_TO_GPT.md`. README, expanded references, and the correction template are not first-round uploads.
- The intent is fixed to whole-array adjacent-pin edge-to-edge gap measurement with four verified ROI windows: `108,170,65,120`, `204,170,65,120`, `300,170,65,120`, and `396,170,65,120`.
- Current-source runner evidence under `artifacts/llm_gpt_packet_public_pin_20260715` proves the starter contract before any external LLM claim: the nominal image passed all 9 reference Steps with `DistanceMmAvg=0.151..0.159` and `DistanceMmRange=0.006..0.012`; the wide-pin image failed at `0.116 < 0.14`.
- This is prompt/fixture readiness evidence only. No real GPT transcript has been received yet, and no external response may be described as validated until the user's raw response is preserved and run through Recipe Manager validation/correction/import review.
- Immediate next priority: receive the unchanged GPT XML response, preserve the raw prompt/response, validate in Recipe Manager, and use `SEND_VALIDATION_NG_TO_GPT.md` in the same GPT task if correction is required.

## 2026-07-15 P12 Real GPT Pin-Gap Round 1

- The user manually transferred an actual GPT response. The exact GPT model/version was not provided, so do not infer or add one.
- The unchanged prompt and response are preserved under `artifacts/llm_transcripts/raw/20260715_pin_gap_gpt_round1`. The response SHA-256 is `D2944FF344CFECC9CA90F09EEEDD0006B1D7E85A3D79669E7EA2AD4F960EBF3E`.
- Recipe Manager validation/import smoke passed with XML syntax OK, deserialization OK, 9 Steps, 0 errors, 0 warnings, import enabled, and import completed. Import did not run the image; `ImageRun: SKIPPED` preserves the explicit-run contract.
- An independent contract preflight confirmed 8 `LineDistance` Steps: four `DistanceMmAvg` gates and four `DistanceMmRange` gates, followed by one `OverlayMerge` Step. The generic smoke recipe did not select a strict intent, so its `Intent contract: SKIP` line is not evidence that the tool family was unchecked.
- Explicit nominal-image execution passed all 9 Steps. The four average results were `0.151..0.159 mm`, and the four range results were `0.006..0.012 mm`.
- Explicit negative-image execution produced the expected product NG at the first Step: `DistanceMmAvg 0.116 < 0.14`. The smoke command returned exit code 1 because product NG is represented as a failed image run; this is expected negative-test evidence, not a validation failure.
- The combined validate/import/image smoke path hung and was discarded. Separate current-build validation/import and explicit image-run paths completed and are the accepted evidence for this round.
- This is a real manually transferred GPT direct-success transcript with zero corrections. It is not an NG-to-correction-loop transcript, and no failed round may be invented merely to create one.
- The intended-workbench maturity estimate remains 62-66%. P12 proves one external GPT round against the public pin-gap contract but does not yet establish provider/model breadth or correction-loop reliability.
- Next evidence-based priority: privacy-review and sanitize this direct-success transcript before any promotion outside the raw artifact area. Capture an actual correction round only when a future independent GPT/Gemini/Claude draft naturally fails validation or execution.

## 2026-07-15 P13 Sanitized GPT Direct-Success Candidate

- The P12 prompt and response passed privacy/public-asset review and were copied unchanged to `artifacts/llm_transcripts/sanitized/20260715_pin_gap_gpt_round1_direct_success`.
- Automated checks found zero absolute/user-home paths, email addresses, secret labels, or known private/legacy asset hints in `prompt.md` and `response.xml`.
- The sanitized prompt and response hashes exactly match the preserved raw evidence: prompt `96886DEEDA962E59E653EADA99AE4792CA130B0537018434793A38DFFCC04354`, response `D2944FF344CFECC9CA90F09EEEDD0006B1D7E85A3D79669E7EA2AD4F960EBF3E`.
- The candidate includes only the unchanged prompt/XML, derived nominal/negative result images, a repository-relative manifest, and the privacy review. Raw manifests and smoke reports were deliberately excluded because they contain local workspace or attachment paths.
- Public replay inputs remain `docs/samples/public/Line_Pins_Synthetic_OK.png` and `Line_Pins_Synthetic_WidePin_NG.png`; the XML has no template or external-file dependency.
- Classification remains narrow: real manually transferred GPT direct-success evidence, not API evidence and not a correction-loop transcript. Exact GPT model/version remains unknown.
- The intended-workbench maturity estimate remains 62-66%. The next evidence-based implementation priority is fresh current-build Recipe Manager/LLM Assistant UX inspection using this real draft; change only proven clipping, overlap, or unclear next-action friction. A real correction round remains blocked until an independent external response naturally fails.

## 2026-07-15 P14 Current-Build Recipe Manager LLM UX Recheck

- The unchanged 18:29:44 Debug build was still current because P13 changed only documentation and ignored transcript artifacts. `dotnet bin/Debug/OpenVisionLab.dll --smoke recipe-manager-tabs --output artifacts/p13_recipe_manager_llm_ux_baseline_20260715` passed.
- Fresh `OpenVisionLab_RecipeManager_LlmXml.png` inspection at 1600x900 found no clipped button/icon content, hidden combo/input text, or incoherent control overlap. The advanced LLM surface remains dense but is vertically scrollable and is not the default Recipe Manager summary.
- The smoke report confirms visible whole-array validate/import/sample-run guidance, actual latest-run `DistanceMmAvg`/`DistanceMmRange` feedback, validation issue rows, dependency rows, XML diff, and blocked invalid imports without Preview/Run side effects.
- No UI source was changed because the fresh evidence did not prove clipping, overlap, or an unclear next action. Do not perform another speculative Recipe Manager layout pass from this baseline.
- The WPF-render-only `OpenVisionLab_RecipeManager_LlmPinGapSkill.png` contains a large unpainted area and is not accepted as a full-window visual reference; use the screen-captured LLM XML image and report for this P14 review.
- Next evidence-based priority: run the real P12 four-branch `LineDistance` plus `OverlayMerge` recipe through Pipeline Review and determine whether the existing branch/output comparison explains its producer outputs. Extend comparison UX only if that real recipe exposes a concrete gap. No new external sample or GPT prompt is required for this audit.

## 2026-07-15 P15 Real GPT Overlay Source Review

- The real P12 GPT recipe exposed a concrete comparison defect: Pipeline Review considered only each Step's `InputLayer`, so the final `OverlayMerge.SourceLayers` dependencies appeared as unrelated same-input alternatives. Baseline evidence showed `SourceConsumerRelationsVisible: 0/4` and `OverlaySourceProducersVisible: 0/4`.
- Pipeline review now parses `SourceLayers` and shows the four range-evidence Steps as explicit overlay sources. Selecting a source Step shows its review-merge consumer before same-input alternatives; selecting the final overlay shows only its four declared source producers.
- Latest `OpenVisionLab.exe` evidence under `artifacts/p15_real_gpt_branch_review_20260715/final_exe_retry1` passed with `SourceConsumerRelationsVisible: 4/4`, `OverlaySourceProducersVisible: 4/4`, `PreviewRunCountUnchanged: 0`, and `ActiveLayerUnchanged: True`.
- The full `OpenVisionLab.exe` Recipe Manager regression under `artifacts/p15_real_gpt_branch_review_20260715/recipe_manager_regression_exe` passed, including the existing BentPin multi-branch and Contour 3+ fan-out coverage. The Contour regression now rejects an unrelated `OverlayMerge` as a same-input alternative and verifies the actual declared source route instead.
- Fresh current-source before evidence is under `artifacts/p15_real_gpt_branch_review_20260715/before_current_logic`; latest-EXE after evidence is under `artifacts/p15_real_gpt_branch_review_20260715/final_exe_retry1`. Intermediate captures with large WPF unpainted regions are excluded from visual evidence.
- The intended-workbench maturity estimate remains 62-66%. This closes one real multi-branch explanation defect; it does not prove provider breadth or a correction-loop transcript.
- Next evidence-based priority: keep the sanitized direct-success corpus separate until a deliberate repository-inclusion decision is made. Capture a correction loop only when a future independent external draft naturally fails. Extend branch/output comparison again only when another real recipe exposes a relationship not represented by direct `InputLayer` or declared `SourceLayers`.

## 2026-07-15 P16 Recipe Manager Workspace Separation

- Fresh latest-EXE review confirmed the user's concern was information architecture, not a color-only defect: advanced Pipeline review retained the outer recipe library/search, lifecycle CRUD, repeated global Step/guided text, technical tabs, and transfer commands at the same time.
- Recipe Manager now has two physically separate workspace states. Summary shows recipe search/library, one selected-recipe overview, and create/duplicate/rename/delete lifecycle commands. Advanced review hides those outer controls, opens `Pipeline review` at full width, and exposes `Build inspection`, `Pipeline review`, `LLM XML`, `Step preview`, plus a compact XML/review transfer strip and explicit `Back to summary`.
- The global advanced header now keeps only selected recipe, active pipeline, counts, and XML readiness. Repeated Step flow, status, and guided-next-action text were removed from that header; detailed flow remains in the XML/Step tab.
- Recipe Manager colors now use neutral charcoal surfaces and borders with cyan reserved for selection/primary state and green for readiness, reducing the previous one-note teal hierarchy.
- Direct smoke hygiene now deletes only reserved generated recipe names matching `Smoke_LlmBranch_`, `Smoke_LlmDraft_`, `Smoke_LlmIntentSkills_`, or `Smoke_RecipeManager_` plus an exact 12-hex suffix. Each affected scenario also cleans its current workspace in `finally`. After the latest EXE regression, `bin/Debug/RECIPE` contained only `Default`.
- Latest EXE before evidence: `artifacts/p16_recipe_manager_structure_20260715/before_exe/OpenVisionLab_RecipeManager_Summary.png` and `OpenVisionLab_RecipeManager_Pipeline.png`.
- Latest EXE after evidence: `artifacts/p16_recipe_manager_structure_20260715/after_exe_final_200617/OpenVisionLab_RecipeManager_Summary.png` and `OpenVisionLab_RecipeManager_Pipeline.png`; the matching regression report is `report.txt` in the same folder.
- Current-source focused summary and advanced smokes passed with `layout=0`, `text=0`, and `internal=0`. The full latest-EXE `recipe-manager-tabs` regression passed with the new summary/advanced separation, and the real GPT branch regression still passed `4/4`, `4/4`, Preview/Run delta `0`, active layer unchanged.
- The intended-workbench maturity estimate remains 62-66%. This materially improves novice entry and advanced-workspace hierarchy but does not simplify every technical panel inside advanced Pipeline review.
- Next evidence-based Recipe Manager priority: validate the new summary with a normal named operator recipe rather than a smoke name, then simplify one advanced Pipeline sub-workflow only if that real task still lacks an obvious next action. Do not reintroduce library/lifecycle controls into advanced review.

## 2026-07-15 P17 Operator-Named Recipe Manager Roundtrip

- The latest built `OpenVisionLab.exe` ran `recipe-pipeline-roundtrip` with the operator-style recipe `배터리 벤트 검사` and pipeline `벤트 영역 이진화`; evidence is under `artifacts/p17_recipe_manager_operator_flow_20260715/actual_exe_final_202115`.
- The roundtrip passed `Recipe summary -> Pipeline Review -> Return to Recipe -> Advanced review -> Return to summary`. Native Preview/Run remained `0`, layer count remained `1`, the active layer and recipe routing were unchanged, and the isolated Pipeline Review result was `OK`.
- Fresh screenshots show the normal recipe/pipeline names without clipping. The summary keeps `Open Pipeline` as its primary action, Pipeline Review exposes `Run Review` and `Return to Recipe`, and advanced review exposes `Run check` and `Back to summary`. No additional speculative Recipe Manager layout edit was made because this real task did not expose an unclear next action.
- `recipe-pipeline-roundtrip` now accepts optional `--recipe-name` and `--pipeline-name` text for this repeatable operator-facing check. It refuses to overwrite an existing recipe, deletes only the workspace it created in `finally`, and cleans stale reserved `Smoke_RecipeRoundtrip_<12 hex>` workspaces. The collision guard was verified against `Default`; it returned the expected failure and left `Default` unchanged.
- The intended-workbench maturity estimate remains 62-66%. P17 closes the P16 operator-name validation item but does not reduce every technical row inside advanced review.
- Next evidence-based priority: audit the P13 sanitized direct-success transcript candidate and present a repository-inclusion recommendation. Do not move sanitized evidence into permanent/public paths without explicit user approval.

## 2026-07-15 P18 Sanitized Transcript Publication Audit

- `docs\OPENVISIONLAB_LLM_TRANSCRIPT_PUBLICATION_REVIEW_20260715.md` records the reusable publication gate and the P13 candidate audit.
- The initial decision was `CONDITIONAL GO / CURRENTLY HOLD`. After the publication gate was presented, the user approved continuation and the minimum package was added to the Dev worktree at `docs\evidence\llm\20260715_pin_gap_gpt_direct_success`.
- Raw/sanitized prompt and response hashes match. The XML parses as 9 Steps with no external file/template dependency. Both replay inputs are registered project-authored public synthetic assets. Result PNGs contain no textual metadata.
- Latest-EXE replay under `artifacts/p18_llm_transcript_publication_review_20260715` passed validation/import with 9 Steps, 0 errors, 0 warnings, and `ImageRun: SKIPPED`; the nominal image passed 9/9 Steps, while the negative image produced the expected inspection NG at `DistanceMmAvg 0.116 < 0.14`.
- The current result images are byte-identical to the sanitized candidate images. No process or generated recipe remained after replay.
- The package contains only `README.md`, `prompt.md`, `response.xml`, and the two result images. Its README identifies GPT through user-operated ChatGPT, manual transfer, unknown exact model/version, direct success with zero corrections, AI-generated content, human review, hashes, and replay commands.
- Do not publish raw manifests or reports containing local paths, and do not place transcript evidence under `docs/samples/public`.
- The intended-workbench maturity estimate remains 62-66%. P18 strengthens evidence governance but does not add provider breadth or a correction-loop result.
- The Dev-worktree package has not been staged, committed, pushed, or copied to the Original repository. A real correction-loop transcript remains blocked until a future external response naturally fails; do not fabricate one. The next unblocked priority is a bounded Tool View code-behind cleanup only where an existing base/controller pattern naturally fits.

## 2026-07-15 P19 GPT Evidence Package Inclusion

- After the P18 publication gate was presented, the user approved continuation. The minimum evidence package now exists in the Dev worktree at `docs\evidence\llm\20260715_pin_gap_gpt_direct_success`.
- The package contains exactly five files: one combined disclosure/reproduction `README.md`, the unchanged `prompt.md` and `response.xml`, and byte-identical nominal/negative OpenVisionLab result PNGs. Raw manifests, local-path reports, attachment references, and API/session data remain excluded.
- `README.md` conspicuously identifies the GPT-generated response, user-operated ChatGPT transfer, unknown exact model/version, zero correction rounds, non-API classification, completed human review, publication approval, hashes, and exact replay commands. The result PNGs are identified as rule-based OpenVisionLab outputs rather than model-generated images.
- The package content check passed with five files, 9 XML Steps, matching prompt/response/result hashes, no detected absolute user path, credential token, or email, and a valid link to the P18 audit document.
- Latest-EXE replay from the new documentation path passed under `artifacts/p19_llm_evidence_package_20260715`: validate/import PASS with `ImageRun: SKIPPED`, nominal 9/9 PASS, expected negative NG at `DistanceMmAvg 0.116 < 0.14`, and byte-identical result hashes. No `OpenVisionLab` process or generated recipe directory remained.
- This was documentation/evidence inclusion only; no UI changed, so no before/after UI capture was required. The package has not been staged, committed, pushed, or copied to Original.
- The intended-workbench maturity estimate remains 62-66%. The next unblocked priority is a bounded Threshold Tool cleanup: move only Learn-window creation, activation, apply-event forwarding, unsubscription, and disposal from `ThresholdToolWpfView.xaml.cs` into a focused controller. Keep `VisionToolThresholdInteractionController`, the public test hook, explicit apply behavior, and all Preview/Run contracts unchanged. Existing `wpf_openvision_learn_threshold`, animation, apply, and native Threshold smokes provide direct regression coverage. Real correction-loop evidence still depends on a naturally failing external response.

## 2026-07-15 P20 Threshold Learn Window Controller

- Added `ThresholdToolLearnWindowController` as the owner of Learn-window creation, existing-window activation, apply-event forwarding, event detachment, closed-state reset, and disposal.
- `ThresholdToolWpfView` now owns only the controller reference and calls `Open()` or `Dispose()` from its existing XAML click handler, `OpenThresholdGuideForTest`, and tool cleanup path. Direct `OpenVisionLearnWindow` ownership and Learn event handlers are gone from the View; its code-behind decreased from 179 to 146 lines.
- The data path remains `Learn Apply event -> ThresholdToolLearnWindowController -> VisionToolThresholdInteractionController.ApplyBasicThresholdFromGuide`. Property creation still uses the existing `ThresholdToolPresenter`; no alternative PropertyGrid, parameter model, or automatic execution path was added.
- `wpf_shell_host_threshold_tool` now also verifies a second open activates the same single Learn window, closing clears controller state, a later open creates a new window, and open/apply/reopen does not increment native Preview/Run. The screenshot-smoke project was explicitly rebuilt after the new assertions; the final current-source smoke passed with `layout=0`, `text=0`, and `internal=0`.
- Full build and the explicit screenshot-smoke project build passed with 0 warnings and 0 errors. Readiness, external-reference, public-sample, and `git diff --check` passed.
- Valid pre-edit evidence is `artifacts/p20_threshold_learn_controller_20260715/before_current_source/wpf_openvision_learn_threshold_apply.png`; accepted verified after evidence is `artifacts/p20_threshold_learn_controller_20260715/after_visual_verified/wpf_openvision_learn_threshold_apply.png`; controller-path shell evidence is `artifacts/p20_threshold_learn_controller_20260715/after_shell_verified/wpf_shell_host_threshold_tool.png`.
- Before/after comparison found no layout/content change. Differing pixels were limited to the nondeterministically rendered instruction-text band (`1.142170%`, bounds `296,249-1007,309`).
- A first image-viewer read temporarily presented large black regions, but direct PNG pixel inspection and a repeat read showed normal light-surface pixels in every P20 file. This was a transient evidence-viewer presentation issue, not a corrupt or unpainted PNG. Do not add a screenshot failure rule from that false observation. The `colors=64` and `flat=0%` fields remain legacy placeholders, but replacing them is not a current product priority without a real capture defect.
- The intended-workbench maturity estimate remains 62-66%. P6 Step-report linkage and per-Step bottleneck analytics were already complete in the fresher product-target and stable-contract documents; the common Tool-header lifecycle slice below is the actual next completed priority. Real correction-loop evidence remains blocked on a naturally failing external response.

## 2026-07-15 P21 Common Tool Learn Window Controller

- Source audit found that `VisionToolSingleInputPropertyToolShell` and `VisionToolDoubleInputCustomToolShell` still created a new `OpenVisionLearnWindow` directly for every header Learn click. Repeated clicks could therefore leave duplicate Learn windows open.
- Added the shared `VisionToolLearnWindowController`. Both common Shells now delegate only `Open(LearnTopicIndex)`; the controller owns create, same-topic activation, topic-change replacement, closed-state reset, and window references.
- The common Learn smoke helper now proves that a second click reactivates the same single window, closing and clicking again creates a fresh window at the expected topic, and the entire open/reactivate/reopen sequence leaves Preview/Run unchanged.
- Explicit screenshot-smoke project build and full solution build passed with 0 warnings and 0 errors. `wpf_arithmetic_tool_learn_button` and `wpf_simple_preprocess_tool_learn_button` both passed with `layout=0`, `text=0`, and `internal=0`; readiness, external-reference, public-sample, and `git diff --check` also passed.
- Current-source evidence is under `artifacts/p21_common_learn_window_controller_20260715`: `before_arithmetic`/`after_arithmetic` cover the double-input Arithmetic Shell, and `before_single_input`/`after_single_input` cover the common single-input Shell. The Arithmetic capture used different persisted responsive widths before and after, but both layouts are complete; the equal-size single-input comparison changed only `0.387352%` in the preview-render region.
- No Tool parameter, PropertyGrid, layer, input/output route, or execution path changed. Product maturity remains 62-66%.
- Next priority remains conditional: capture a natural external LLM correction round when available; otherwise change Recipe Manager/LLM or branch comparison only when fresh real evidence exposes a gap. Do not continue code-behind cleanup solely to reduce line count.

## 2026-07-15 P22 Recipe Manager Guided Setup Korean Operator Copy

- A fresh latest-EXE `recipe-manager-tabs` audit exposed one real novice-facing issue: the Korean Recipe Manager still showed English operator guidance in Guided Setup/LLM, including `Guided setup / LLM assistant`, `Required inputs`, `Starter XML creation only`, `READY/MISSING` explanations, and the pin-gap average-only warning.
- `OpenVisionShellHostRecipeCommandSurface` now localizes the visible title, summary, required-input help, ready/missing explanations, and pin-gap calibration advice. Technical identifiers such as `MM-READY`, `PX-ONLY`, `PIXELPERMM`, `DistanceMmAvg`, `DistanceMmRange`, `SCORE_MIN`, and `ResultCount` remain unchanged. The copied external-LLM prompt remains English by design.
- `wpf_shell_host_recipe_guided_setup` now rejects Korean operator guidance that falls back to `Required inputs` or `average-only measurement`, while also requiring the Korean title, summary, readiness, and calibration phrases.
- Valid latest-EXE before/after evidence is `artifacts/p22_current_recipe_llm_audit_20260715/before_current_exe/OpenVisionLab_RecipeManager_GuidedSetup.png` and `after_current_exe/OpenVisionLab_RecipeManager_GuidedSetup.png`. The valid after LLM task view is `after_current_exe/OpenVisionLab_RecipeManager_LlmIntentLineDistance.png`. Final current-source assertion evidence is `after_current_source_guided_final/wpf_shell_host_recipe_guided_setup.png` with `layout=0`, `text=0`, and `internal=0`.
- Some other LLM captures in this run intermittently presented large dark composition regions; they are excluded from visual evidence. The valid Guided Setup and LLM intent captures plus the full direct smoke report are the current evidence.
- Full solution build and explicit screenshot-smoke project build passed with 0 warnings and 0 errors. Latest-EXE `recipe-manager-tabs`, readiness, external-reference, and public-sample checks passed; no OpenVisionLab process remained.
- No XML, PropertyGrid, parameter, layer, routing, Preview/Run, import, or branch/output behavior changed. Product maturity remains 62-66%.
- Next priority remains conditional: preserve a naturally failing external LLM correction round when available. Otherwise change Recipe Manager/LLM or branch comparison only when another fresh real task exposes a concrete gap.

## 2026-07-15 P23 Guided Setup Stale Draft Guard

- Fresh current-source evidence reproduced a concrete safety/clarity defect: the Guided Setup selector showed `Pin gap / edge distance (LineDistance)` while the read-only draft still contained `LLM_MeanBrightnessDrift_Skill` and a `Mean` Step, with no indication that the XML belonged to the previous settings.
- Guided Setup now preserves the previous XML after an intent/input change, marks it stale, disables Import readiness, and changes the draft heading to an amber `설정이 변경되었습니다. Starter XML을 다시 만들어 주세요.` warning. It does not auto-delete the prior draft or auto-generate a replacement.
- A successful explicit `Starter XML 만들기` clears the stale state. The focused smoke proves draft byte preservation, Import blocking, stale-state clearing after explicit regeneration, and unchanged Preview/Run count for both input-value and inspection-intent changes.
- Valid current-source before evidence is `artifacts/p23_guided_setup_stale_draft_20260715/before_current_source/wpf_shell_host_recipe_guided_setup.png`. Valid after warning evidence is `after_current_source_visible/wpf_shell_host_recipe_guided_setup.png`. The first quiet after capture had large unpainted regions and is excluded from visual evidence.
- Full solution build passed with 0 warnings and 0 errors and produced `bin/Debug/OpenVisionLab.exe` and `.dll` at `2026-07-15 21:54:57 KST`. Latest-build actual-EXE `recipe-manager-tabs` passed under `artifacts/p23_guided_setup_stale_draft_20260715/actual_exe`; the report retains all Guided Setup, LLM validation/import, branch comparison, history, and explicit-run contracts.
- Focused current-source smoke passed with `layout=0`, `text=0`, and `internal=0`. Readiness, external-reference, public-sample (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), and `git diff --check` passed. No OpenVisionLab or screenshot-smoke process remained.
- No XML schema, PropertyGrid, tool parameter, layer, route, Preview/Run, or automatic execution behavior changed. Product maturity remains 62-66%.
- Next priority remains conditional: capture a naturally failing external LLM correction round when available. Otherwise perform another current-build operator task and modify Recipe Manager/LLM or branch comparison only when it exposes a new concrete gap.

## 2026-07-15 P24 GPT Blob Particle Count Packet

- Added a self-contained manual GPT packet at `llm_prompt_packets/blob_particle_count` so the user does not need to locate or attach the full authoring guide/tool catalog.
- The packet contains byte-identical copies of the public-safe 572x420 nominal and sparse-negative Blob samples, `COPY_THIS_TO_GPT.txt`, `PASTE_VALIDATION_NG_BACK_TO_GPT.txt`, and one short `README.md` with the exact transfer order.
- The initial prompt constrains the task to `Threshold -> Blob`, preserves explicit Preview/Run behavior, defines `Main -> Particle_Binary -> Particle_Count_Preview`, filters area `200..2000`, and requires a `ResultCount` acceptance band of `8..14`. It requests XML only and forbids unrelated ToolTypes/custom nodes.
- The correction template must be used only after actual OpenVisionLab Validation/Run evidence exists. The user must return the first GPT response unchanged, even if it contains prose, fences, or invalid XML; do not repair it before capture.
- SHA-256 checks proved both packet images are byte-identical to their public sample sources. Prompt contract check passed all 11 required tokens, public-sample policy remained `CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`, and `git diff --check` passed with line-ending warnings only.
- No LLM response or transcript has been claimed yet. The immediate next dependency is the user's unchanged GPT response to `COPY_THIS_TO_GPT.txt`; validate/import/run it only after that response arrives.

## 2026-07-15 P25 GPT Blob Direct-Success Replay

- The user returned one XML-only GPT response for the P24 Blob packet. The unchanged response is preserved under `artifacts/llm_transcripts/raw/20260715_blob_particle_gpt_round1/response.xml`; exact model/version remains unknown, transfer was manual through user-operated ChatGPT, and no API was used.
- The packet prompt was found overwritten with the returned XML in the shared workspace. That exact file content was first preserved as the raw response, then `COPY_THIS_TO_GPT.txt` was restored and copied to raw `prompt.txt`. Final hashes are prompt `FE58976...B1F3` and response `CCB47A4D...3127`.
- Full solution build passed with 0 warnings and 0 errors. Latest current build remains `bin/Debug/OpenVisionLab.exe/.dll`, timestamp `2026-07-15 21:54:57 KST` because no source file changed after P23.
- Recipe Manager `llm-xml-draft-file` passed: XML syntax/deserialization/routing OK, 2 Steps, 0 errors, 0 warnings, Import enabled/completed, selected pipeline `Blob_Particle_Count_Inspection`, and `ImageRun: SKIPPED`.
- Explicit nominal run passed 2/2 Steps with `ResultCount=12` inside `8..14`. Explicit sparse-negative run produced the expected product NG with `ResultCount=3 < 8`; the runner exit code 1 is expected for an NG inspection outcome.
- Raw evidence includes prompt/response, manifest, reports, current-build Recipe Manager screenshot, and nominal/negative result images. This is a real GPT direct-success example with zero correction rounds, not API evidence and not a correction-loop transcript. Do not use the correction prompt or manufacture a failed round.
- Next evidence task should add a different dependency/tool-family challenge, preferably public-safe Matching with a template dependency, rather than repeating another tightly constrained Threshold/Blob prompt.

## 2026-07-15 P26 GPT Matching Die-Pad Packet

- Added the next self-contained manual GPT packet at `llm_prompt_packets/matching_die_pad` for a different tool family and a real template-file dependency.
- The packet contains byte-identical copies of the public-safe nominal image, no-target negative image, and matching template plus `COPY_THIS_TO_GPT.txt`, `PASTE_VALIDATION_NG_BACK_TO_GPT.txt`, and a short `README.md`.
- The initial prompt requires exactly one `Matching` Step, the repository-relative template path in both `TemplatePath` and `PATTERN_PATH`, normalized `SCORE_MIN=0.6`, the product's exact `MAGNIFIATION` spelling, `NUM_MATCH=3`, angle-search settings, and a `ResultCount` acceptance gate of exactly 3.
- SHA-256 checks passed for all three copied images: nominal `EF12511...A68BF`, negative `C01E877...2649`, and template `FE8B979...A5144`. The prompt contract check passed all required Matching, path, score, count, and acceptance tokens.
- Current-build baseline replay passed on the nominal image with `ResultCount=3`, `ScoreMin=80.059`, `ScoreMax=93.074`, and 60.533 ms. The no-target image produced the expected inspection NG with `ResultCount=0 < 3`; evidence is under `artifacts/p26_matching_packet_baseline_20260715`.
- No Matching GPT response has been received or claimed yet. The next dependency is the user's complete unchanged response after attaching all three packet images and pasting `COPY_THIS_TO_GPT.txt` once. Preserve that response before validation; use the correction template only after a natural current-build validation or OK/NG failure.

## 2026-07-15 P27 GPT Matching Round 1 Natural Dependency Failure

- The user returned an XML-only GPT response for P26. The unchanged initial prompt and response are preserved under `artifacts/llm_transcripts/raw/20260715_matching_die_pad_gpt_round1`; exact model/version remains unknown, transfer was manual through user-operated ChatGPT, and no API was used.
- Full solution build passed with 0 warnings and 0 errors. Recipe Manager parsing/deserialization/schema/routing all passed for the one-Step `Matching_DiePad_Inspection`, but dependency review found both `TemplatePath` and `PATTERN_PATH` unresolved, blocked Import, and skipped image execution.
- GPT followed the initial prompt exactly. The failure was caused by the packet incorrectly requiring `docs\samples\...` while Recipe Manager resolves relative dependencies from `AppPathService.StartupPath`, currently `bin\Debug`. The verified current-build relative path is `..\..\docs\samples\public\templates\Matching_DiePad_Synthetic_Template.png` and resolves to the expected template hash.
- Raw round 1 response hash is `479D5D08...ED2FA`; the complete validation report hash is `32BD710A...389B`. The reusable packet path rule was corrected only after the original prompt/response were preserved.
- A ready-to-paste round 2 request with the complete unedited report is `artifacts/llm_transcripts/raw/20260715_matching_die_pad_gpt_round1/round2_prompt.txt`, hash `B38AE291...200CC`. It asks GPT to change only the two dependency paths. No round 2 response exists yet; do not claim correction-loop success before unchanged round 2 validation/import plus nominal PASS and expected no-target NG.

## 2026-07-15 P28 GPT Matching Correction Loop Completed

- The user returned the unchanged round 2 XML from the same GPT conversation. It is preserved as `artifacts/llm_transcripts/raw/20260715_matching_die_pad_gpt_round1/round2_response.xml`, hash `E608A864...66B79`.
- Structured round 1/round 2 comparison found no pipeline-name, Step-count, fixed Step-field, or acceptance-field differences. Exactly two parameter values changed: `TemplatePath` and `PATTERN_PATH` now use the verified StartupPath-relative template path.
- Full solution build passed with 0 warnings and 0 errors. Recipe Manager round 2 validation/import passed with 1 Step, 0 errors/warnings, both dependencies copied, Import completed, and `ImageRun: SKIPPED` during import.
- Explicit nominal execution from the actual `bin\Debug` StartupPath passed with `ResultCount=3`, `ScoreMin=80.059`, `ScoreMax=93.074`, `ScoreAvg=86.444`, and 33.101 ms. Explicit no-target execution produced the expected product NG with `ResultCount=0 < 3` and 25.604 ms.
- This is the first completed real manually transferred GPT correction loop in the current corpus: exact model/version unknown, non-API, one correction round. Preserve the attribution that round 1 failed because the packet supplied the wrong host-relative path; it proves report-driven correction, not independent GPT discovery of the path convention.
- Raw evidence contains local paths and stack traces. Do not publish it directly. The next evidence-governance priority is a separate sanitization/publication audit; keep a minimum package only after confirming hashes, public asset provenance, replay commands, and disclosure wording.

## 2026-07-15 P29 Matching Correction Evidence Publication Audit

- Prepared a seven-file sanitized candidate under `artifacts/llm_transcripts/sanitized/20260715_matching_die_pad_gpt_correction_loop`; no file was placed under `docs/evidence/llm`.
- `prompt_round1.md`, both response XML files, and both result PNGs are byte-identical to raw evidence. `prompt_round2.md` replaces exactly four raw workspace-root occurrences with `<REPO_ROOT>`; reverse substitution reconstructs the raw prompt byte-for-byte.
- Publishable-text scan found no drive/user-home path, credential, email, URL, AppData/Codex attachment path, private/legacy asset, or customer data. Both result PNGs are 572x420 with only `IHDR`/`IDAT`/`IEND` chunks and no text metadata.
- Nominal, negative, and template inputs are registered OpenVisionLab-generated public synthetic assets. Candidate README records disclosure, unknown exact GPT model/version, manual non-API transfer, one correction round, prompt-path failure attribution, hashes, and reproducible commands.
- Fresh current-build replay under `artifacts/p29_matching_correction_publication_review_20260715` reproduced round 1 dependency NG, round 2 import PASS, nominal `ResultCount=3` PASS, no-target `ResultCount=0 < 3` expected NG, and byte-identical result images. Build passed with 0 warnings/errors.
- `docs/OPENVISIONLAB_LLM_MATCHING_CORRECTION_PUBLICATION_REVIEW_20260715.md` initially recorded `CONDITIONAL GO / CURRENTLY HOLD`. That historical gate was superseded by the explicit approval and P30 inclusion below.

## 2026-07-15 P30 Matching Correction Evidence Included

- After the conditional publication decision was presented, the user explicitly approved inclusion of the sanitized copy under `docs`.
- Added exactly seven files under `docs/evidence/llm/20260715_matching_die_pad_gpt_correction_loop`: one disclosure/replay README, two prompts, two unchanged GPT XML responses, and two OpenVisionLab result images. Raw reports, stack traces, runtime paths, and session data remain excluded.
- The included README records approval, unknown exact GPT model/version, manual non-API transfer, one correction round, prompt-path failure attribution, replay commands, public input provenance, and file hashes. Its final SHA-256 is `6C24B58D...7B1924`; the other six files retain their audited candidate hashes.
- Tracked-path replay under `artifacts/p30_matching_correction_tracked_package_20260715` reproduced round 1 dependency NG, round 2 validation/import PASS with both dependencies copied and no image run during import, nominal `ResultCount=3` PASS, and no-target `ResultCount=0 < 3` expected NG. Both result image hashes match the included package exactly.
- Post-inclusion verification passed: full solution build with 0 warnings/errors, readiness, external-reference, public-sample (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), package/hash/privacy checks, runtime cleanup, and `git diff --check` with line-ending warnings only.
- `docs/OPENVISIONLAB_LLM_MATCHING_CORRECTION_PUBLICATION_REVIEW_20260715.md` now records `GO / ADDED TO THE DEV WORKTREE`. The package is not staged, committed, pushed, or copied to the Original repository.
- The intended-workbench maturity estimate remains 62-66%. This closes the first real manually transferred GPT correction-loop evidence item, but does not establish provider/model breadth or broad intent reliability.
- Next evidence priority: select a different public-safe tool family from the current catalog, establish a deterministic OpenVisionLab baseline first, and then prepare one self-contained manual GPT packet. Do not repeat the same Matching path defect merely to manufacture another correction round.

## 2026-07-16 P31 GPT Edge-Fiducial Packet

- Added a six-file self-contained manual GPT packet under `llm_prompt_packets/edge_fiducial_matching`: one README, one first-round prompt, one correction template, and three project-authored public synthetic PNGs. The first round requires only those three PNGs plus the complete contents of `COPY_THIS_TO_GPT.txt`; the full authoring guide and tool catalog are not separate uploads.
- The contract is intentionally narrow: exactly one full-image `EdgeBasedMatching` Step finds the supplied asymmetric L fiducial once in the nominal image and rejects the wrong T-shape image. Both template parameters use the verified Debug StartupPath-relative path `..\..\docs\samples\public\templates\Edge_Fiducial_Synthetic_Template.png`.
- The prompt explicitly separates normalized inputs (`SCORE_MIN=0.70`, `GREEDINESS=0.90`) from the percentage-like `ScoreMax` result gate (`70..100`) and forbids unsupported tools, invented parameters, absolute paths, automatic execution, explanatory prose, and Markdown fences.
- A mechanically derived reference contract passed Recipe Manager validation/import with 1 Step, 0 errors, 0 warnings, both dependencies copied, and `ImageRun: SKIPPED`. Import therefore preserved the explicit Preview/Run contract.
- Explicit current-build execution passed the nominal image at `ResultCount=1` and `ScoreMax=99.598`. The wrong T-shape image produced the expected inspection NG with `ResultCount=0` and `BestScore=61.052`, below `SCORE_MIN=0.70`.
- Packet checks passed with exactly six files, 22 exact Step parameters, no absolute drive path in the prompt, one correction-report placeholder, and byte-identical copied image hashes. Evidence is under `artifacts/p31_edge_based_gpt_packet_20260716`.
- No external GPT response exists yet. Preserve the user's complete first response unchanged before validation. Use `PASTE_VALIDATION_NG_BACK_TO_GPT.txt` only when that actual response produces a natural current-build validation or OK/NG failure.
- The intended-workbench maturity estimate remains 62-66%. This adds a verified EdgeBasedMatching authoring task but does not yet add provider/model breadth or another real transcript.

## 2026-07-16 P32 GPT Edge-Fiducial Direct-Success Replay

- The user returned one XML-only GPT response for the P31 EdgeBasedMatching packet. The unchanged response and exact packet prompt are preserved under `artifacts/llm_transcripts/raw/20260716_edge_fiducial_gpt_round1`; response SHA-256 is `55D63ADF...B3DCD` and prompt SHA-256 is `F51731D2...F1E`.
- Exact GPT model/version was not provided. Transfer was manual through a user-operated ChatGPT conversation, no API was used, and the response was not edited after receipt.
- XML-only format checks passed. The response contains one `EdgeBasedMatching` Step and 22 parameters, and a structured comparison found zero pipeline, fixed-field, parameter, routing, or acceptance differences from the pre-verified P31 reference contract.
- A fresh full solution build passed with 0 warnings and 0 errors. The build was incremental; an independent freshness check found 0 of 598 compile inputs newer than `bin/Debug/OpenVisionLab.exe`.
- Latest-EXE Recipe Manager validation/import passed with 1 Step, 0 errors, 0 warnings, both template dependencies resolved/copied, Import enabled/completed, and `ImageRun: SKIPPED`.
- Explicit nominal execution passed at `ResultCount=1`, `ScoreMin/Max/Avg=99.598`, overlay count 1, and 123.622 ms. Visual review confirmed the L fiducial carries the `#1 99.6` edge overlay.
- Explicit wrong-T execution produced the expected product NG with validation still successful, `ResultCount=0`, `BestScore=61.052`, overlay count 0, and 83.404 ms. The command exit code 1 represents the expected inspection rejection.
- This is a real manually transferred GPT direct-success example with zero correction rounds. Do not send the correction prompt or manufacture a failed round. Raw reports contain local paths; perform a separate privacy/sanitization audit before any `docs/evidence` inclusion decision.
- The intended-workbench maturity estimate remains 62-66%. P32 adds another real tool-family transcript but does not establish provider/model breadth or broad prompt reliability.

## 2026-07-16 P33 Edge-Fiducial Sanitized Candidate

- Created a six-file artifact-only candidate under `artifacts/llm_transcripts/sanitized/20260716_edge_fiducial_gpt_round1_direct_success`: unchanged prompt/XML, unchanged nominal/negative result PNGs, a sanitized manifest, and a privacy review.
- The prompt, response, and two result images are byte-identical to P32 raw evidence. Their SHA-256 values remain `F51731D2...F1E`, `55D63ADF...B3DCD`, `E96C0DAB...23C4C`, and `3A4AC87A...F8ECE`.
- Automated text review found zero absolute drive/user-home/AppData/Codex-attachment paths, emails, URLs, or credential labels. Both 572x420 result PNGs contain only `IHDR`, `IDAT`, and `IEND` chunks with no text or EXIF metadata.
- The nominal, wrong-shape negative, and template hashes match their registered public synthetic assets. No raw report, stack trace, runtime recipe path, generated runtime identifier, or local EXE path was copied into the candidate.
- Latest-EXE candidate replay under `artifacts/p33_edge_fiducial_sanitization_20260716` passed validation/import with no image run, passed nominal at `ResultCount=1` and `ScoreMax=99.598`, and produced expected wrong-T NG at `ResultCount=0` and `BestScore=61.052`. Replayed result PNG hashes are byte-identical to the candidate.
- Candidate manifest SHA-256 is `A41E16D5...D4AA`; privacy review SHA-256 is `34B574B1...8EB`. The directory remains ignored under `artifacts`, is not staged, and is not under `docs/evidence`.
- Dev commit `64d9ade0` was pushed to `origin/codex/public-sample-ux-docs` before this P33 artifact-only candidate and handoff update. P33 itself is not included in that pushed commit. The Original repository was not touched.
- Next gate: obtain a separate explicit repository-inclusion decision before copying this candidate to `docs/evidence`, staging it, or publishing it. If approved, replay the eventual tracked path and verify all copied hashes again.

## 2026-07-16 P34 Edge-Fiducial Evidence Included

- The user explicitly approved repository inclusion of the P33 sanitized EdgeBasedMatching direct-success candidate.
- Added six files under `docs/evidence/llm/20260716_edge_fiducial_gpt_direct_success`: AI-disclosure/replay README, privacy review, unchanged prompt, unchanged GPT XML response, and unchanged nominal/negative OpenVisionLab result PNGs.
- The prompt, response, and two PNGs remain byte-identical to P32 raw evidence and P33 candidate. Final package hashes are recorded in `docs/OPENVISIONLAB_LLM_EDGE_FIDUCIAL_PUBLICATION_REVIEW_20260716.md`.
- Package-wide text scan found 0 absolute drive, user-home, AppData, Codex attachment, email, or URL matches. Both PNGs are 572x420 with only `IHDR`/`IDAT`/`IEND` chunks and no text/EXIF metadata.
- Fresh full build passed with 0 warnings and 0 errors. Latest-EXE tracked-path replay under `artifacts/p34_edge_fiducial_tracked_package_20260716` passed validation/import with `ImageRun: SKIPPED`, passed nominal at `ResultCount=1` and `ScoreMax=99.598`, and produced expected wrong-T NG at `ResultCount=0` and `BestScore=61.052`.
- Replayed nominal and negative result PNG hashes match the package exactly. No OpenVisionLab process or reserved Smoke recipe remains after replay.
- Publication review decision is `GO / ADDED TO THE DEV WORKTREE`. The nine-file P34 scope was committed as `7a7dc51f` (`Add GPT edge fiducial evidence`) and pushed to `origin/codex/public-sample-ux-docs`; local and remote SHAs matched. The Original repository was not touched.
- The intended-workbench maturity estimate remains 62-66%. This increases evidence coverage but does not establish exact-model, provider, or unconstrained authoring reliability.
- Next evidence priority: select a different public-safe tool family only after defining a deterministic nominal/negative baseline. Prefer FeatureMatching if the current implementation can produce stable public synthetic evidence; do not force a transcript around an unstable tool.

## 2026-07-16 P35 FeatureMatching Baseline And GPT Packet

- Audited the existing public-safe `Public_Feature_Card` benchmark before preparing another external LLM task. The registered synthetic nominal, wrong-card negative, template, and one-Step FeatureMatching pipeline already provide the required deterministic baseline.
- A fresh full solution build passed with 0 warnings and 0 errors. The latest EXE is `bin/Debug/OpenVisionLab.exe`, timestamp `2026-07-16 09:39:33 KST`, SHA-256 `53E9223D...B0F85`.
- Five sequential latest-EXE runs per image were stable. Nominal passed 5/5 at `ResultCount=1`, `ScoreMax=96.7`, 210.595-288.316 ms; wrong-card produced expected NG 5/5 at `ResultCount=1`, `ScoreMax=26.7`, 261.463-286.012 ms. Each group produced one unique result-image hash.
- The wrong-card result proves that `ResultCount` alone is unsafe for this FeatureMatching task. The packet and baseline require the final `ScoreMax` gate of `80..100`.
- Added `llm_prompt_packets/feature_matching_card` with exactly six files: README, first-round prompt, correction template, and byte-identical nominal/negative/template PNGs. The prompt requires one full-image `FeatureMatching` Step with `SCORE_MIN=0.85`, `RANSAC_REPROJ_THRESHOLD=4`, and explicit `ScoreMax` acceptance.
- Recipe Manager validation/import resolved and copied both startup-relative template dependencies, imported one Step with 0 errors/warnings, and reported `ImageRun: SKIPPED`. Direct raw replay passed when launched with the real `bin/Debug` startup working directory; launching raw XML from the repository-root working directory cannot resolve the startup-relative path and is retained only as harness-condition evidence.
- Packet audit passed: exactly six files, all three image hashes equal their public sources, all required prompt tokens present, 0 absolute-path matches, one correction-report placeholder, and no external response claimed.
- Detailed evidence is `docs/OPENVISIONLAB_LLM_FEATURE_MATCHING_BASELINE_REVIEW_20260716.md`, `artifacts/p35_feature_matching_sequential_20260716`, and `artifacts/p35_feature_matching_packet_20260716`.
- P35 is not committed or pushed. Its external-response dependency was satisfied by the real P36 direct-success response below. The correction template was not used.

## 2026-07-16 P36 GPT FeatureMatching Direct-Success Replay

- The user returned one XML-only GPT response after using the P35 FeatureMatching packet. The complete visible one-line response is preserved under `artifacts/llm_transcripts/raw/20260716_feature_card_gpt_round1/response.xml`; its SHA-256 is `F06E9259...E245`.
- The exact packet prompt is preserved as `prompt.txt`, SHA-256 `B9F32A8C...BBFE`, and is byte-identical to `llm_prompt_packets/feature_matching_card/COPY_THIS_TO_GPT.txt`.
- Transfer was manual through a user-operated ChatGPT conversation. No API was used. The exact GPT model/version was not supplied and must remain unknown rather than inferred.
- XML-only checks passed: no prose or Markdown fence, one `FeatureMatching` Step, 8 parameters, no duplicate keys, and zero structural differences from the mechanically verified P35 reference contract.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest EXE remained `bin/Debug/OpenVisionLab.exe`, timestamp `2026-07-16 09:39:33 KST`, SHA-256 `53E9223D...B0F85`.
- Latest-EXE Recipe Manager validation/import passed with 1 Step, 0 errors, 0 warnings, Import enabled/completed, and both template dependencies copied. `ImageRun: SKIPPED` confirms import did not execute the inspection.
- Explicit nominal execution passed at `ResultCount=1`, `ScoreMax=96.7`, overlay count 1, and 200.371 ms. Explicit wrong-card execution produced the expected inspection NG at `ResultCount=1`, `ScoreMax=26.7 < 80`, overlay count 1, and 245.15 ms.
- Nominal and negative result-image hashes are byte-identical to the P35 deterministic baseline: `6FD55065...F4914` and `FF663DD3...EC57B`.
- This is a real manually transferred GPT direct-success example with zero correction rounds. The correction prompt was not sent and no failed round was manufactured.
- The raw manifest records prompt/response/report/result hashes, provenance, build evidence, and cleanup. Raw reports contain absolute local paths and a stack trace; do not copy the raw folder directly into `docs/evidence`.
- P36 is artifact evidence plus this handoff update and is not committed or pushed. Its sanitization dependency was completed by the P37 artifact-only candidate below.

## 2026-07-16 P37 FeatureMatching Sanitized Candidate

- Created a six-file artifact-only candidate under `artifacts/llm_transcripts/sanitized/20260716_feature_card_gpt_round1_direct_success`: unchanged prompt/XML, unchanged nominal/negative OpenVisionLab result PNGs, a sanitized manifest, and a privacy review.
- The prompt, response, and two result images are byte-identical to P36 raw evidence. Their SHA-256 values remain `B9F32A8C...BBFE`, `F06E9259...E245`, `6FD55065...F4914`, and `FF663DD3...EC57B`.
- Candidate-wide text review found zero absolute drive, user-home, AppData/Codex attachment, email, URL, or credential-token matches. Prompt/response-specific scans also found zero credential labels and private/legacy asset hints.
- Both result PNGs are 572x420 and contain only `IHDR`, `IDAT`, and `IEND` chunks with no text or EXIF metadata.
- Nominal, wrong-card negative, and template hashes match their registered OpenVisionLab-generated public synthetic assets. No raw report, stack trace, runtime recipe identifier, local EXE path, or Recipe Manager capture was copied into the candidate.
- A fresh full build passed with 0 warnings and 0 errors. Latest-EXE candidate replay under `artifacts/p37_feature_card_sanitization_20260716` passed validation/import with one Step, 0 errors/warnings, two copied dependencies, and `ImageRun: SKIPPED`.
- Candidate replay passed the nominal sample at `ResultCount=1`, `ScoreMax=96.7`, and 216.264 ms. The wrong-card sample produced expected NG at `ResultCount=1`, `ScoreMax=26.7 < 80`, and 224.545 ms.
- Replayed nominal and negative result hashes are byte-identical to the candidate. No OpenVisionLab process or reserved Smoke recipe remained.
- Final sanitized manifest SHA-256 is `C76A9F76...F2636`; privacy review SHA-256 is `3921D49F...2D73`.
- At P37 completion the directory remained ignored under `artifacts` and was not staged. The user supplied the separate inclusion decision at P38; see the following section for the tracked `docs/evidence` package. The P37 artifact itself remains ignored and unchanged.

## 2026-07-16 P38 FeatureMatching Evidence Included In Docs

- The user explicitly approved inclusion of the P37 FeatureMatching direct-success candidate in the Dev worktree. The public six-file package is now `docs/evidence/llm/20260716_feature_card_gpt_direct_success` and the publication review is `docs/OPENVISIONLAB_LLM_FEATURE_CARD_PUBLICATION_REVIEW_20260716.md`.
- The package contains only `README.md`, `privacy_review.md`, `prompt.md`, `response.xml`, `nominal_result.png`, and `negative_result.png`. The prompt, XML, and two result images remain byte-identical to P37 and P36 raw evidence: `B9F32A8C...BBFE`, `F06E9259...E245`, `6FD55065...F4914`, and `FF663DD3...EC57B`.
- Final package README and privacy-review hashes are `68D13992...D5E0` and `2CA546C5...A160`. Package-wide text scans found zero absolute Windows path, user-home, AppData/Codex attachment, email, or URL matches. Prompt/XML payload scans found zero credential labels.
- Both result images are 572x420 and contain `IHDR`, thirteen `IDAT`, and `IEND` chunks only; no text or EXIF metadata exists. The nominal, wrong-card, and template public source hashes match `AA0A0092...3C26`, `3791EC67...703F`, and `75D535A5...A1CD`.
- A fresh full solution build passed with 0 warnings and 0 errors. The latest EXE is `bin/Debug/OpenVisionLab.exe`, timestamp `2026-07-16 09:39:33 KST`, SHA-256 `53E9223D...0F85`.
- Latest-EXE tracked-path replay used the copied `docs/evidence` response XML and is preserved under `artifacts/p38_feature_card_tracked_package_20260716`. Recipe Manager validation/import passed with 1 Step, 0 errors/warnings, 2 copied dependencies, and `ImageRun: SKIPPED`. The nominal sample passed at `ResultCount=1`, `ScoreMax=96.7`, overlay 1, `355.149 ms`; the wrong-card sample produced expected NG at `ResultCount=1`, `ScoreMax=26.7 < 80`, overlay 1, `287.081 ms`, and exit code 1.
- Replayed nominal/negative PNG hashes equal the included package. No OpenVisionLab process or reserved `Smoke_LlmDraft_<12 hex>` directory remained after replay. This inclusion did not stage, commit, or push anything, and did not touch the Original repository.

## 2026-07-16 P39 LLM XML Next-Action Clarity

- Fresh current-source baseline evidence showed the LLM XML tab with three dense lines, including developer-facing execution and result-channel wording, while Template Matching's required fields were only available in the separate Build inspection tab.
- Added the `검사 설정` (`Set up inspection`) command to the LLM XML toolbar. It opens `Advanced > 검사 만들기` so the operator can enter the intent-specific values without searching the Recipe Manager surface.
- Rewrote the visible LLM XML guidance into the operator sequence: choose an inspection intent and required values, create a starter XML or prompt, then validate and import XML. The selected intent/result-channel developer-contract line is no longer rendered in the learner-facing panel.
- Guided Setup action and next-step text now describe create draft -> validate -> import. The former no-auto-run phrase remains only in the private `GuidedSetupStarterXmlNoAutoRunContract` source constant so the regression contract stays enforced without appearing in the UI.
- Fresh current-source before: `artifacts/p39_recipe_llm_ux_before_20260716/llm_xml_initial/wpf_shell_host_llm_dependency_placeholder.png`. Fresh same-summary-state after: `artifacts/p39_recipe_llm_ux_after_20260716_r3/wpf_shell_host_llm_dependency_placeholder.png`. The Build inspection target capture is `artifacts/p39_recipe_llm_ux_after_20260716_r2/guided_setup/wpf_shell_host_recipe_guided_setup.png`.
- Screenshot smoke asserted that `검사 설정` is visible, opens Build inspection, and leaves Preview run count, layer count, preview result, and XML draft unchanged. LLM XML and Guided Setup captures both passed with `layout=0`, `text=0`, and `internal=0`.
- Fresh full solution build passed with 0 warnings and 0 errors. Readiness, external-reference, public-sample, and `git diff --check` validation passed. P39 is uncommitted and unpushed; the Original repository remains untouched.

## 2026-07-16 P40 Run History Baseline Next-Action Clarity

- Fresh current-source Run History evidence showed the no-baseline row using the time-impossible instruction `Run at least one earlier benchmark to enable regression comparison.` The selected saved run already existed, so the wording did not tell the operator what to do next.
- Replaced the no-baseline guidance with: `동일한 검증 세트를 다시 실행한 뒤, 이전 저장 실행을 기준 실행으로 선택하세요.` / `Run the same validation suite again, then select an earlier saved run as the baseline.` It points directly to the existing `Baseline run` selector and does not add an execution, persistence, layer, or routing action.
- Fresh current-source before: `artifacts/p40_run_history_baseline_20260716/wpf_shell_host_recipe_local_validation_set.png`. Fresh after: `artifacts/p40_run_history_after_20260716/wpf_shell_host_recipe_local_validation_set.png`.
- The focused local-validation-set screenshot smoke now checks the no-baseline row when present. It passed with `layout=0`, `text=0`, and `internal=0`. Full solution and screenshot-tool builds passed with 0 warnings and 0 errors. Readiness, external-reference, public-sample, and `git diff --check` validation passed.
- P40 is uncommitted and unpushed; the Original repository remains untouched. Do not repeat a generic Run History wording pass without new current-build evidence.

## 2026-07-16 P41 Feature Matching Guided Setup

- The public Feature Card baseline and the real GPT direct-success package both establish a bounded FeatureMatching contract: full-image search, `SCORE_MIN=0.85`, `RANSAC_REPROJ_THRESHOLD=4`, and `ScoreMax 80..100` acceptance. The wrong-card negative returns `ResultCount=1`, so ResultCount alone is not a valid pass gate.
- Added `Feature Matching` to the existing Build inspection intent catalog and mapped the existing `FeatureMatching` Tool rail action to the same Guided Setup surface. This is not a second Pipeline editor or a new runtime tool.
- The compact operator input block exposes only Feature template path, read-only full-image scope, Ratio minimum, RANSAC pixel tolerance, and ScoreMax minimum. Its starter XML creates one `FeatureMatching` Step (`Main -> Feature_Preview`) with `USE_ROI=false`; it does not validate/import/run or change any layer/routing by itself.
- XML contract validation now requires `ToolType=FeatureMatching`, `SCORE_MIN`, `RANSAC_REPROJ_THRESHOLD`, and a minimum `ScoreMax` acceptance gate. A FeatureMatching draft altered to use only `ResultCount` is rejected with a corrective next action.
- Fresh current-source baseline: `artifacts\p41_feature_guided_setup_before_20260716\wpf_shell_host_recipe_guided_setup.png`. It is the closest reproducible Guided Setup baseline because the Feature Matching input state did not exist before this slice. Fresh after: `artifacts\p41_feature_guided_setup_after_20260716_r2\wpf_shell_host_recipe_guided_setup.png`.
- Fresh solution and screenshot-tool builds passed with 0 warnings and 0 errors. The current-source Guided Setup smoke passed with `layout=0`, `text=0`, and `internal=0`; it verifies missing/invalid inputs, generated XML parameters, the ScoreMax gate, rejection of ResultCount-only acceptance, no Preview/Run, and Korean Feature Matching capture state.
- P41 is uncommitted and unpushed; the Original repository remains untouched. Keep future FeatureMatching Guided Setup changes tied to a deterministic public Good/Bad baseline rather than adding unconstrained fields.

## 2026-07-16 P42 Edge Based Matching Guided Setup

- The included public Edge Fiducial direct-success evidence establishes a bounded EdgeBasedMatching contract: one full-image `EdgeBasedMatching` Step, `SCORE_MIN=0.70`, `NUM_MATCH=1`, `CANNY_LOW/HIGH=30/90`, and `ScoreMax 70..100` acceptance. The wrong T-shape negative has `ResultCount=0` and `BestScore=61.052`, so the workflow keeps count as review evidence and requires ScoreMax for acceptance.
- Added `Edge Based Matching` to the existing Build inspection intent catalog and mapped the existing `EdgeBasedMatching` Tool rail action to it. This remains a Guided Setup starter surface, not a second Pipeline editor or a new runtime tool.
- The compact input block exposes only Edge template path, read-only full-image scope, minimum score, search count, Canny low/high, and ScoreMax minimum. Its Starter XML creates one `EdgeBasedMatching` Step (`Main -> EdgeBased_Preview`) with the public baseline's fixed edge parameters and `USE_ROI=false`; it does not validate/import/run or change any layer/routing by itself.
- XML contract validation now requires `ToolType=EdgeBasedMatching`, `SCORE_MIN`, `NUM_MATCH`, `CANNY_LOW/HIGH`, `USE_ROI=false`, and a minimum `ScoreMax` acceptance gate. A draft altered to use only `ResultCount` is rejected with a corrective next action.
- Fresh closest pre-change baseline: `artifacts\p42_edge_guided_setup_before_20260716\wpf_shell_host_recipe_guided_setup.png`; Edge Based Matching inputs did not exist before this slice. Fresh current-source after: `artifacts\p42_edge_guided_setup_after_20260716\wpf_shell_host_recipe_guided_setup.png`.
- Fresh solution and screenshot-tool builds passed with 0 warnings and 0 errors. The current-source Guided Setup smoke passed with `layout=0`, `text=0`, and `internal=0`; it verifies missing/invalid inputs, Canny ordering, generated XML parameters, the ScoreMax gate, rejection of ResultCount-only acceptance, no Preview/Run, and Korean Edge Based Matching capture state. Readiness, external-reference, and public-sample checks passed (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`).
- P42 is uncommitted and unpushed; the Original repository remains untouched. Keep future EdgeBasedMatching Guided Setup changes tied to the deterministic public fiducial baseline rather than adding opaque matching controls.

## 2026-07-16 P43 Guided Setup Latest-EXE Coverage

- The first P42 latest-EXE `recipe-manager-tabs` replay passed, but its Guided Setup report still named only Pin gap, Blob, Contour, Matching, and Mean. This exposed a verification gap: P41 Feature Matching and P42 Edge Based Matching had current-source coverage but were not exercised by the broad latest-EXE Recipe Manager scenario.
- Extended the existing direct `recipe-manager-tabs` smoke, without changing operator behavior, to select each new intent, verify a missing template blocks Starter XML, create valid public-template Starter XML, require its `ScoreMax` contract, assert the visible intent-specific controls, and keep `NativePreviewRunCount` unchanged.
- The updated latest EXE was built at `2026-07-16 12:56:26 KST` with 0 warnings and 0 errors. Direct `recipe-manager-tabs` passed under `artifacts\p43_guided_setup_direct_exe_20260716`; `report.txt` now records `Pin gap + Blob + Contour + Matching + Feature Matching + Edge Based Matching + Mean Starter XML without Preview/Run`.
- Current latest-EXE Edge Based Matching evidence is `artifacts\p43_guided_setup_direct_exe_20260716\OpenVisionLab_RecipeManager_GuidedSetup_EdgeBasedMatching.png`. It is runtime regression evidence; the P42 before/current-source after pair remains the UI-change comparison record.
- P43 is uncommitted and unpushed; the Original repository remains untouched. Do not extend the direct smoke merely to list every possible control: add a new intent here only when that intent has a dedicated Guided Setup contract.

## 2026-07-16 P44 HSV LLM Authoring Contract And Packet

- The public HSV ColorPatch pair already had a deterministic one-Step pipeline and `MaskPixelRatio` Good/Bad gate, but `HSV` and its aliases were missing from the LLM tool catalog and authoring guide. This made the tool family unavailable for constrained external XML tasks even though the runner supported it.
- Found and fixed one contract mismatch: `VisionPipelineHsvMaskTool` intentionally accepts circular hue ranges such as `HueMin=170`, `HueMax=10` by joining the `170..179` and `0..10` ranges, while generic XML validation rejected the same values. The validator now retains 0..179 bounds but allows hue wrap; saturation/value ordering remains enforced.
- `docs/OPENVISIONLAB_LLM_TOOL_CATALOG.json` now exposes `HSV`, `HsvMask`, `ColorHSV`, and `ColorMask`, their range rules, separate-mask routing, and `MaskPixelCount`/`MaskPixelRatio` metrics. The authoring guide includes a one-Step HSV Color Mask pattern and repair rule. Readiness protects those contracts.
- Fresh latest-EXE `llm-xml-draft-file` validation/import passed for the circular-hue XML with one Step, 0 errors/warnings, and `ImageRun: SKIPPED`; evidence is `artifacts/p44_hsv_llm_contract_after_20260716/direct_validation/report.txt`.
- Explicit current-build replay passed the nominal public image at `MaskPixelRatio=0.058` and produced expected NG for the missing-patch image at `MaskPixelRatio=0.015 < 0.05`. The packet is `llm_prompt_packets/hsv_color_patch`; its two PNGs are byte-identical to public synthetic sources. See `docs/OPENVISIONLAB_LLM_HSV_COLOR_PATCH_BASELINE_REVIEW_20260716.md`.
- No real GPT response has been received or claimed. The next dependency is the user's complete unchanged first response after attaching the two packet images and pasting `COPY_THIS_TO_GPT.txt`. P44 is uncommitted and unpushed; the Original repository remains untouched.

## 2026-07-16 P45 LLM Intent Smoke Advanced-Review State

- A fresh latest-EXE LLM Intent smoke initially showed the outer recipe library and Create/Duplicate/Rename/Delete controls. Source review proved that the normal `OpenRecipeLlmXmlReview` path enables Advanced Review before selecting the LLM XML tab, while this smoke selected only the tab. The capture was therefore a smoke-state evidence defect, not a confirmed operator UI defect.
- Updated `OpenVisionLabDirectSmokeRunner.cs` to enable `recipeAdvancedReviewToggle` before opening `tabRecipeLlmXml`, then assert that `HostRecipeManagerLibraryPane`, `HostRecipeNameEditor`, and `HostRecipeManagerCommandStrip` are not visible.
- Fresh latest-EXE evidence is `artifacts/p45_recipe_manager_llm_audit_after_verified_current_exe_20260716/OpenVisionLab_RecipeManager_LlmIntentSkills_TemplateMatching.png`. It shows the full-width technical review state with only the explicit return-to-summary action; no library, search, or basic lifecycle controls are present.
- The focused direct smoke passed under the same artifact directory. Its report records the intent-skill visibility cases, the intentional PinGap/Contour mismatch block, and `PreviewRunCountUnchanged: 0`.
- Fresh full solution build passed with 0 warnings and 0 errors. Readiness, external-reference, public-sample (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), and `git diff --check` passed; the latter emitted existing line-ending warnings only.
- P45 changes test evidence only. Do not reopen a Recipe Manager layout change unless a real current-build operator route exposes a concrete clipping, overlap, or next-action problem.

## 2026-07-16 P46-P47 LLM Draft Action Clarity And Guided Setup Intent Width

- Fresh latest-EXE evidence showed the learner-facing `Starter XML` action remaining in an otherwise Korean LLM XML and Guided Setup workflow. The action, its surrounding summary/draft wording, and template-missing status messages now use the operator-facing term `초안 XML 만들기` / `Create draft XML`. Internal command names and the private no-auto-run contract keep their existing `StarterXml` identifiers.
- P46 before evidence is `artifacts/p45_recipe_manager_llm_audit_after_verified_current_exe_20260716/OpenVisionLab_RecipeManager_LlmIntentSkills_TemplateMatching.png`; fresh after evidence is `artifacts/p46_llm_draft_labels_after_current_exe_20260716/OpenVisionLab_RecipeManager_LlmIntentSkills_TemplateMatching.png`. The latest-EXE intent-skill smoke passed and retained `PreviewRunCountUnchanged: 0`.
- The P46 Guided Setup capture exposed one actual clipped value: `Pin gap / edge distance (LineDistance)` did not fit in the 240px inspection-intent selector. P47 reduced the adjacent label column from 130px to 120px and widened only that selector column to 300px. The selected value is fully visible while the sample field remains available.
- P47 before evidence is `artifacts/p46_recipe_manager_tabs_after_current_exe_20260716/OpenVisionLab_RecipeManager_GuidedSetup.png`; fresh after evidence is `artifacts/p47_guided_setup_intent_width_after_current_exe_20260716/OpenVisionLab_RecipeManager_GuidedSetup.png`.
- Fresh latest-EXE `recipe-manager-tabs` passed under the P47 artifact directory. It preserved explicit sample loading, no automatic Preview/Run from Guided Setup starter generation, advanced-review full-width behavior, Feature/Edge Guided Setup coverage, and existing branch/output review checks.
- The first Readiness pass correctly caught its stale expected status-message token after P46. The check now expects `Created Guided setup draft XML. Preview/Run was not executed`; the behavioral no-auto-run invariant remains unchanged and Readiness passes again.
- Final P46/P47 verification passed: full solution build with 0 warnings/errors, Readiness, external-reference, public-sample (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), and `git diff --check` after the document update. No commit, push, or Original repository change occurred.

## 2026-07-16 P48 GPT HSV Color-Patch Direct-Success Replay

- The user returned one XML-only GPT response for the self-contained P44 HSV ColorPatch packet. The unchanged response and packet prompt are preserved under `artifacts/llm_transcripts/raw/20260716_hsv_color_patch_gpt_round1`; response SHA-256 is `1B85CA1D...8587` and prompt SHA-256 is `B77AF4DE...B7ACA`.
- Exact GPT model/version was not supplied. Transfer was manual through a user-operated ChatGPT conversation, API evidence was not supplied, and the full chat export was not captured. Do not infer any of those missing facts.
- Latest-EXE Recipe Manager validation/import passed with one HSV Step, 0 errors, 0 warnings, Import enabled/completed, and `ImageRun: SKIPPED`. Import remained separate from explicit image execution.
- The generic LLM image smoke was corrected to load source images with `ImreadModes.Unchanged` rather than grayscale, because grayscale destroys HSV/color input data. It also now accepts `--expect-run-success false` so expected product NG samples can be represented as passing verification cases. The initial grayscale result is not evidence for the GPT XML.
- After the correction and a fresh full build, explicit nominal replay passed at `MaskPixelRatio=0.058` within the `0.05..0.07` gate with 3 source channels. The missing-patch image produced the expected NG at `MaskPixelRatio=0.015 < 0.05`, also with 3 source channels. Both latest-EXE verification commands returned exit code 0 because their expected results were declared explicitly.
- This is a real manually transferred GPT direct-success example with zero correction rounds. Do not send the correction prompt or manufacture a failed round. The raw manifest records provenance, report hashes, and the pre-fix harness exclusion; raw reports contain absolute local paths and must not be copied into `docs/evidence` without a separate sanitization and explicit inclusion decision.
- P48 is not committed or pushed. The Original repository remains untouched.

## 2026-07-16 P49 HSV Color-Patch Evidence Included In Docs

- The user explicitly approved inclusion of the P48 HSV ColorPatch direct-success evidence in the Dev worktree. The public six-file package is now `docs/evidence/llm/20260716_hsv_color_patch_gpt_direct_success` and the publication review is `docs/OPENVISIONLAB_LLM_HSV_COLOR_PATCH_PUBLICATION_REVIEW_20260716.md`.
- The package contains only `README.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged GPT `response.xml`, and unchanged nominal/negative OpenVisionLab result PNGs. The four immutable evidence hashes are `B77AF4DE...B7ACA`, `1B85CA1D...8587`, `C3D021DF...B5A1`, and `06C6D742...9C34`.
- Payload-only text scans found zero absolute Windows/user-home/AppData/Codex attachment paths, emails, URLs, credential-token labels, and known private/legacy asset hints. Both result PNGs are 572x420 with only `IHDR`/`IDAT`/`IEND` chunks and no text/EXIF metadata.
- Fresh latest-EXE tracked-path replay under `artifacts/p49_hsv_tracked_package_20260716` passed validation/import with one Step, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`. The nominal passed at `MaskPixelRatio=0.058`; the missing-patch image produced expected NG at `0.015 < 0.05`. Both source images retained 3 channels, and replay result images are byte-identical to the included package.
- The user-visible package explicitly discloses manual GPT transfer, missing exact model/version/API evidence, zero correction rounds, and the constrained public-synthetic scope. It does not claim broad color robustness or independent tool selection.
- P49 is not committed or pushed. The Original repository remains untouched.

## 2026-07-16 P50 Mean Brightness GPT Packet

- Selected `Mean` as the next bounded external-authoring task because it is not represented by the current real GPT evidence set, has a one-Step operator-facing workflow, and has a deterministic public Good/Dark pair. It teaches full-image brightness judgement without introducing a new tool or expanding platform scope.
- The current public baseline `docs/samples/public/Public_Mean_BrightnessDrift.pipeline.xml` passed the nominal at `MeanValueAvg=201.5` and produced expected dark NG at `117.5 < 185`; both inputs retained 3 channels. The two latest-EXE baseline commands returned exit code 0 through explicit expected-result declarations.
- Added `llm_prompt_packets/mean_brightness` with exactly five files: README, first-round prompt, correction template, and two byte-identical public synthetic PNGs. The packet requires exactly one full-image `Mean` Step, `MEAN_TYPES=Mean`, disabled internal threshold/adaptive/invert/multi-ROI flags, and a `MeanValueAvg` gate of `185..220`.
- Its mechanically derived reference XML passed latest-EXE validation/import with one Step, 0 errors/warnings, and `ImageRun: SKIPPED`; explicit nominal passed at `201.5`, and dark negative produced expected NG at `117.5 < 185`. Packet checks passed: 5 files, all required prompt tokens, no absolute Windows path, one correction placeholder, and the expected one-Step XML structure.
- No external GPT response exists for P50 yet. Send the two packet images and complete `COPY_THIS_TO_GPT.txt` in one new GPT conversation, then preserve the full first response unchanged. Use `PASTE_VALIDATION_NG_BACK_TO_GPT.txt` only after a natural current-build validation or Good/NG failure.

## 2026-07-16 P51 GPT Mean Brightness Direct-Success Replay

- The user returned one XML-only GPT response for the P50 Mean Brightness packet. The unchanged response and exact packet prompt are preserved under `artifacts/llm_transcripts/raw/20260716_mean_brightness_gpt_round1`; response SHA-256 is `19C2EA46...E191` and prompt SHA-256 is `87CF9840...65CC`.
- Exact GPT model/version was not supplied. Transfer was manual through a user-operated ChatGPT conversation, API evidence was not supplied, and the full chat export was not captured. Do not infer any of those missing facts.
- A structured comparison found zero differences across all 21 pipeline, Step, parameter, and acceptance fields against the mechanically verified P50 reference contract.
- Fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` Recipe Manager validation/import passed with one Mean Step, 0 errors, 0 warnings, Import enabled/completed, and `ImageRun: SKIPPED`.
- Explicit nominal execution passed at `MeanValueAvg=201.5`, `ResultCount=1`, source channels 3, and 11.5 ms. Explicit dark execution produced expected product NG at `MeanValueAvg=117.5 < 185`, `ResultCount=1`, source channels 3, and 5.821 ms. Both smoke commands returned exit code 0 because their expected outcomes were declared explicitly.
- This is a real manually transferred GPT direct-success example with zero correction rounds. Do not send the correction prompt or manufacture a failed round. The raw manifest records provenance, contract comparison, report hashes, and result-image hashes. Raw reports contain absolute local paths; do not copy the raw folder directly into `docs/evidence`.
- P51 is not committed or pushed. The Original repository remains untouched. P52 records the separate explicit publication approval and sanitized package.

## 2026-07-16 P52 GPT Mean Brightness Evidence Publication

- The user explicitly approved publication of the P51 Mean direct-success evidence into the Dev worktree. The six-file package is `docs/evidence/llm/20260716_mean_brightness_gpt_direct_success`; the companion decision record is `docs/OPENVISIONLAB_LLM_MEAN_BRIGHTNESS_PUBLICATION_REVIEW_20260716.md`.
- The package contains only `README.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged `response.xml`, and two OpenVisionLab-generated result PNGs. Raw manifests, validation/run reports, local paths, runtime identifiers, stack traces, session data, and Recipe Manager captures remain outside the package.
- The prompt/XML payload scan found zero absolute Windows, user-home, application-data, Codex attachment, email, URL, credential-label, or private/legacy asset-hint matches. Both included 572x420 PNGs contain only `IHDR`, `IDAT`, and `IEND` chunks and are byte-identical to the raw evidence results.
- After a fresh 0-warning/0-error solution build, the latest actual `bin/Debug/OpenVisionLab.exe` replayed the packaged `response.xml`: validation/import PASS with one Step, 0 errors, 0 warnings, and `ImageRun: SKIPPED`; nominal PASS at `MeanValueAvg=201.5`; dark input expected product NG at `MeanValueAvg=117.5 < 185`. Both image inputs retained 3 channels and both smoke commands returned exit code 0 through their explicit expected-result declarations.
- The P52 tracked replay is `artifacts/p52_mean_tracked_package_20260716`; its packaged result PNGs match the actual replay byte-for-byte. The exact GPT model/version, API evidence, and full conversation export remain unavailable; do not infer them. This remains one constrained direct-success example with zero correction rounds.
- P52 is not committed or pushed. The Original repository remains untouched.

## 2026-07-16 P53 Morphology Cleanup GPT Packet

- Selected the first bounded multi-Step external-authoring candidate after the direct single-tool examples: `Threshold -> Morphology(Open) -> Contour`. It exercises sequential layer routing without introducing a branch, template dependency, new tool, or product-scope expansion.
- The existing public baseline `docs/samples/public/Public_Morphology_Cleanup.pipeline.xml` passed fresh latest-EXE validation/import with 3 Steps, 0 errors, 0 warnings, and no external dependencies. Explicit nominal passed `ResultCount=4` with 3 -> 1 -> 1 Step channel transitions and four overlays. Explicit missing-target input produced expected product NG at `ResultCount=2 < 4` with two overlays. Both expected-result commands returned exit code 0.
- Added `llm_prompt_packets/morphology_cleanup` with five files: README, first-round prompt, correction template, and two byte-identical project-authored public synthetic PNGs. The first-round prompt locks Step names, ToolTypes, layer route `Main -> Morphology_Binary -> Morphology_Clean -> Morphology_Cleanup_Preview`, all verified parameters, and a final `ResultCount=4..4` gate.
- Packet checks passed: 5 files, all required tool/route/parameter/acceptance tokens, exactly one correction-report placeholder, no absolute Windows path in the first-round prompt, and both images byte-identical to their public inputs. The baseline review is `docs/OPENVISIONLAB_LLM_MORPHOLOGY_CLEANUP_BASELINE_REVIEW_20260716.md`; latest-EXE evidence is `artifacts/p53_morphology_baseline_20260716`.
- No external GPT response or correction loop exists for P53 yet. The user should send the two packet PNGs and the full first-round prompt to GPT, then return the complete first response unchanged. Use the correction template only after an actual current-build validation or Good/NG failure.
- P53 is not committed or pushed. The Original repository remains untouched.

## 2026-07-16 P54 Recipe Manager Current-EXE Recheck And Pin-Gap Smoke Diagnostics

- A fresh current-EXE `recipe-manager-tabs` run initially stopped in `VerifyPinGapUnitSampleContract` with `MM Good=False, PX Good=True, MM Bad=False, PX Bad=False`. The public one-Step `Public_Line_Pins_Distance.pipeline.xml` replayed independently in the same current build as expected: nominal `DistanceMmAvg=0.222`, expected wide-pin NG `0.106 < 0.2`.
- A no-source-change diagnostic compared the exact Intent Skill-style one-sample route at `SAMPLING_STEP=16` and the public tuned value `6`. Both variants passed nominal and rejected the wide-pin input; the `16` variant measured `DistanceMmAvg=0.224` and the `6` variant `0.222`. This ruled out the sampling-step difference as the observed failure cause.
- Three further independent current-EXE `recipe-manager-tabs` processes passed the same Pin-gap unit contract at `DistanceMmAvg=0.224`, `DistancePxAvg=37.263`, with Good OK in both mm/px modes and Bad NG in both modes. The original failure was not reproduced; do not hide a future recurrence by adding retry-to-pass behavior.
- `OpenVisionLabDirectSmokeRunner.VerifyPinGapUnitSampleContract` now saves the four Pin-gap result PNGs before outcome assertions and includes each run's success flag plus `DistanceMmAvg`, `DistancePxAvg`, `DistanceMmRange`, and `DistancePxRange` in a future failure message. This is smoke diagnostic evidence only; no tool parameter, layer route, Preview/Run, Recipe Manager workflow, or operator-facing UI behavior changed.
- Fresh build after the diagnostic change passed with 0 warnings and 0 errors. Latest `bin/Debug/OpenVisionLab.exe` `recipe-manager-tabs` passed under `artifacts/p54_pin_gap_diagnostic_after_20260716`; the current-EXE Recipe Manager summary and LLM XML views were visually reviewed without a new clipping, overlap, or unclear-next-action defect that justified a UI edit. The one observed limitation remains the un-reproduced first failure, now diagnosable if it recurs.
- P54 is not committed or pushed. The Original repository remains untouched.

## 2026-07-16 P55 Filter Denoise GPT Packet

- Selected `Filter(MedianBlur) -> Threshold -> Contour` as the next independent public-safe authoring candidate. Filter is not covered by the existing real GPT evidence packages, while the existing public Good/NG pair provides a deterministic downstream `ResultCount` decision instead of treating preprocessing output alone as an inspection result.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual-EXE baseline import of `docs/samples/public/Public_Filter_Denoise.pipeline.xml` passed with 3 Steps, 0 errors, 0 warnings, no dependencies, and `ImageRun: SKIPPED`.
- Explicit current-EXE nominal execution passed at `ResultCount=4` with Step channel transitions `3 -> 3 -> 1`, four overlays, and `29.848 ms`. The missing-target image produced expected product NG at `ResultCount=2 < 4`, two overlays, and `27.179 ms`. Both commands used explicit expected-result declarations and returned exit code 0.
- Added the five-file self-contained packet `llm_prompt_packets/filter_denoise`: README, first-round prompt, correction template, and byte-identical copies of the registered Filter public Good/NG assets. The prompt locks `FilterType=MedianBlur`, `MedianKernelSize=5`, `BorderType=Reflect101`, the sequential route `Main -> Filter_Denoised -> Filter_Denoise_Binary -> Filter_Denoise_Preview`, and a final `ResultCount=4..4` gate.
- The exact mechanically derived packet reference XML passed latest-EXE validation/import without image execution, nominal `ResultCount=4`, and expected missing-target NG `ResultCount=2 < 4`; evidence is `artifacts/p55_filter_denoise_packet_20260716`. Packet audit passed: 5 files, copied image hashes match public sources, required prompt tokens and one correction placeholder are present, no private/path token is present, and the reference XML matches the required 3-Step route/gate.
- Updated the LLM tool catalog so Filter lists `MedianKernelSize`, bilateral parameters, and the correct per-filter validation hints. The authoring guide now explains the `MedianBlur -> Threshold -> Contour` route and why the acceptance gate belongs on Contour.
- No external GPT response or correction loop exists for P55 yet. Send the two packet images and the complete first-round prompt in one new GPT conversation, preserve the complete first XML response unchanged, and use the correction template only after a natural current-build validation or Good/NG failure.
- P55 is not committed or pushed. The Original repository remains untouched.

## 2026-07-16 P56 GPT Filter Denoise Direct-Success Replay

- The user returned one XML-only response for the P55 Filter Denoise packet. The visible response was preserved unchanged under `artifacts/llm_transcripts/raw/20260716_filter_denoise_gpt_round1/response.xml`; response SHA-256 is `4E684E91...D6855E`. The copied packet prompt is `prompt.txt`, SHA-256 `654BC48D...07B6C1`.
- Transfer was manual through a user-operated GPT/ChatGPT conversation. Exact model/version, API evidence, and a full chat export were not supplied and must remain unknown rather than inferred.
- XML-only boundary checks passed. A structured comparison found zero differences across the pipeline name, three Steps, routes, parameters, and acceptance fields against the mechanically verified P55 reference contract.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` validation/import passed with 3 Steps, 0 errors, 0 warnings, no dependencies, Import enabled/completed, and `ImageRun: SKIPPED`.
- Explicit nominal execution passed at `ResultCount=4`, four overlays, channel transitions `3 -> 3 -> 1`, and `32.762 ms`. Explicit missing-target execution produced the expected product NG at `ResultCount=2 < 4`, two overlays, the same channel transitions, and `43.106 ms`. Both smoke commands returned exit code 0 because their expected outcomes were declared explicitly.
- This is a real manually transferred GPT direct-success example with zero correction rounds. Do not send the correction prompt or manufacture a failed round. The raw manifest records provenance, report hashes, and result-image hashes. Raw reports contain absolute local paths; do not copy the raw folder directly into `docs/evidence`.
- P56 is raw evidence plus this handoff update only. It is not committed or pushed, and the Original repository remains untouched. The next gate is a sanitized artifact-only candidate and a separate explicit repository-inclusion decision before any tracked `docs/evidence` package.

## 2026-07-16 P57 GPT Filter Denoise Sanitized Candidate

- Created the ignored six-file artifact-only candidate at `artifacts/llm_transcripts/sanitized/20260716_filter_denoise_gpt_round1_direct_success`: `manifest.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged `response.xml`, and current-EXE nominal/negative result PNGs. It is not a tracked `docs/evidence` package.
- The copied prompt, response, nominal PNG, and negative PNG are byte-identical to P56 raw/current-EXE evidence: `654BC48D...07B6C1`, `4E684E91...D6855E`, `F19B25B7...E8D6C`, and `BFCD2853...19DC2` respectively.
- Payload and sanitized-manifest scans found zero absolute Windows/user-home/AppData/Codex attachment paths, URLs, emails, credential-token labels, and known private/legacy asset hints. The two PNGs are 572x420 with only `IHDR`, `IDAT`, and `IEND` chunks. The only full-folder keyword hits are explanatory labels in `privacy_review.md`, not evidence payload content.
- The candidate records only repository-relative public synthetic inputs and replay outcomes. It deliberately excludes the raw manifest and raw validation/run reports because they contain local evidence and EXE paths. Current-EXE replay details remain P56 evidence: import PASS with `ImageRun: SKIPPED`, nominal `ResultCount=4`, and expected missing-target NG `ResultCount=2 < 4`.
- P57 does not grant repository inclusion. Do not copy this candidate to `docs/evidence`, stage it, commit it, or publish it without a separate explicit user decision. No original-repository operation, commit, or push occurred.

## 2026-07-16 P58 GPT Filter Denoise Evidence Included In Docs

- The user approved inclusion of the P57 Filter Denoise direct-success evidence in the Dev worktree. The public six-file package is now `docs/evidence/llm/20260716_filter_denoise_gpt_direct_success`; the inclusion decision is `docs/OPENVISIONLAB_LLM_FILTER_DENOISE_PUBLICATION_REVIEW_20260716.md`.
- The package contains only `README.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged `response.xml`, and OpenVisionLab-generated nominal/negative result PNGs. Raw manifests, validation/run reports, local paths, runtime identifiers, stack traces, and session data remain outside the package.
- Full package text scanning found zero disallowed local-path, contact, URL, credential-marker, or private/legacy asset-hint matches. Both result PNGs are 572x420 with only `IHDR`/`IDAT`/`IEND` chunks and are byte-identical to raw P56 evidence.
- After a fresh 0-warning/0-error solution build, latest actual `bin/Debug/OpenVisionLab.exe` replayed the tracked `response.xml`: validation/import PASS with 3 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; nominal PASS at `ResultCount=4`; missing-target input expected product NG at `ResultCount=2 < 4`. Both replay input images retained `3 -> 3 -> 1` channel transitions and replay result PNGs match the included package byte-for-byte.
- The P58 tracked replay is `artifacts/p58_filter_denoise_tracked_package_20260716`. Exact GPT model/version, API evidence, and full conversation export remain unavailable; this is one constrained direct-success case with zero correction rounds. P58 is not committed or pushed, and the Original repository remains untouched.

## 2026-07-16 P59 Morphology Cleanup GPT And Gemini Direct-Success Replay

- The user returned two XML-only responses for the P53 Morphology Cleanup packet and identified the first as GPT and the second as Gemini. The visible XMLs are separately preserved under `artifacts/llm_transcripts/raw/20260716_morphology_cleanup_gpt_round1` and `artifacts/llm_transcripts/raw/20260716_morphology_cleanup_gemini_round1`. Exact model/version, API evidence, and full conversation exports were not supplied and must remain unknown rather than inferred.
- XML-only boundary checks passed for both. The required three-Step `Threshold -> Morphology -> Contour` contract, parameter sets, sequential layer route, and final `ResultCount=4..4` gate had zero differences for both providers. Their free `VisionPipeline/Name` values differ (`Morphology_Cleanup_Inspection` versus `Morphology Cleanup`) because the packet did not constrain that label.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` independently imported both drafts with 3 Steps, 0 errors, 0 warnings, no dependencies, and `ImageRun: SKIPPED`.
- GPT nominal passed `ResultCount=4` in 39.171 ms; its missing-target input produced expected NG `ResultCount=2 < 4` in 24.75 ms. Gemini nominal passed `ResultCount=4` in 33.919 ms; its missing-target input produced expected NG `ResultCount=2 < 4` in 26.502 ms. Both paths retained `3 -> 1 -> 1` channel transitions.
- The provider-specific reports remain separate. Their Good result PNGs are byte-identical, as are their expected-NG PNGs, because the required executable contract is identical. Do not treat this as a provider-quality comparison or as evidence of independent algorithm selection.
- Both are real manually transferred direct-success examples with zero correction rounds. Do not send the correction prompt or manufacture failed rounds. Raw reports contain local paths and must not be copied into `docs/evidence` without separate sanitization and explicit inclusion approval.

## 2026-07-16 P60 Morphology Cleanup Sanitized Candidates

- Created separate ignored six-file artifact-only candidates for GPT and Gemini under `artifacts/llm_transcripts/sanitized/20260716_morphology_cleanup_gpt_round1_direct_success` and `artifacts/llm_transcripts/sanitized/20260716_morphology_cleanup_gemini_round1_direct_success`.
- Each candidate has a manifest, privacy review, unchanged prompt/response, and nominal/negative OpenVisionLab result PNGs. The four immutable evidence files match their raw/current-EXE sources byte-for-byte. Text scans found zero disallowed local-path, contact, URL, credential-marker, or private/legacy-asset matches; each PNG is 572x420 with only `IHDR`/`IDAT`/`IEND` chunks.
- P60 does not grant repository inclusion. Keep both candidates ignored and do not copy either to `docs/evidence`, stage, commit, or publish without a separate explicit user decision. The Original repository remains untouched.

## 2026-07-16 P61 Morphology Cleanup GPT And Gemini Evidence Included In Docs

- The user approved inclusion of both P60 Morphology Cleanup direct-success candidates in the Dev worktree. The public packages are `docs/evidence/llm/20260716_morphology_cleanup_gpt_direct_success` and `docs/evidence/llm/20260716_morphology_cleanup_gemini_direct_success`; the companion decisions are `docs/OPENVISIONLAB_LLM_MORPHOLOGY_CLEANUP_GPT_PUBLICATION_REVIEW_20260716.md` and `docs/OPENVISIONLAB_LLM_MORPHOLOGY_CLEANUP_GEMINI_PUBLICATION_REVIEW_20260716.md`.
- Each package contains only `README.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged provider response XML, and OpenVisionLab-generated nominal/negative result PNGs. Raw manifests, validation/run reports, local paths, runtime identifiers, stack traces, and session data remain outside both packages.
- Both six-file packages passed complete text/path/privacy scans, PNG metadata checks, file-set checks, and publication-review hash audits. Their result PNGs are 572x420 `IHDR`/`IDAT`/`IEND` files and match raw evidence byte-for-byte.
- After a fresh 0-warning/0-error solution build, latest actual `bin/Debug/OpenVisionLab.exe` replayed both tracked package XMLs: each import passed with 3 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; each nominal passed at `ResultCount=4`; each missing-target input produced expected product NG at `ResultCount=2 < 4`. GPT replay elapsed values were 27.722/26.648 ms and Gemini replay values were 25.567/27.727 ms for Good/NG respectively. Included result PNGs match replay byte-for-byte.
- The combined tracked replay is `artifacts/p61_morphology_gpt_gemini_tracked_packages_20260716`. Exact provider model/version, API evidence, and full conversation exports remain unavailable. The matching outputs prove only constrained contract adherence, not a provider benchmark or general authoring reliability. P61 is not committed or pushed, and the Original repository remains untouched.

## 2026-07-16 P62 Arithmetic Invert GPT Packet

- Selected the distinct public-safe `Arithmetic(Bitwise_NOT) -> Mean` workflow after the existing public baseline showed a deterministic two-Step Good/Bright-NG split. This adds Arithmetic evidence coverage without introducing a new tool, template dependency, camera path, or platform feature.
- Fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` imported `Public_Arithmetic_Invert.pipeline.xml` with 2 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; the nominal image passed at `MeanValueAvg=208`, and the bright-input image produced expected NG at `MeanValueAvg=76.7 < 190`.
- Added the self-contained five-file packet `llm_prompt_packets/arithmetic_invert`: README, first-round prompt, correction template, and byte-identical public Good/Bright-NG image copies. The packet locks the sequential route `Main -> Arithmetic_Invert_Result -> Arithmetic_Invert_Mean`, unary `Bitwise_NOT` without `InputLayerB`, and the `190..230` MeanValueAvg gate.
- The exact packet reference XML replayed in the same latest EXE: import passed with 2 Steps and 0 errors/warnings, nominal passed at `208`, and Bright-NG produced expected product NG at `76.7 < 190`; all three commands returned exit code 0. Evidence is `artifacts/p62_arithmetic_invert_baseline_20260716`.
- No external GPT response or correction loop exists for P62. The next external gate is the user's unchanged first response after attaching the two packet images and sending the packet prompt. Do not invent or publish a response, failure, or correction round before that input exists. P62 is uncommitted and unpushed; the Original repository remains untouched.

## 2026-07-16 P63 Arithmetic Invert GPT Direct-Success Replay

- The user returned one XML-only response identified as GPT for the P62 Arithmetic packet. The manually transferred XML and copied packet prompt are preserved under `artifacts/llm_transcripts/raw/20260716_arithmetic_invert_gpt_round1`. Exact model/version, API evidence, and a full provider-chat export were not supplied and must remain unknown rather than inferred.
- XML-only boundary validation passed. The user-supplied response matched the mechanically verified two-Step Arithmetic reference contract across 34 structured fields with zero differences.
- After a fresh 0-warning/0-error solution build, latest actual `bin/Debug/OpenVisionLab.exe` validation/import passed with 2 Steps, 0 errors, 0 warnings, no dependencies, and `ImageRun: SKIPPED`.
- Explicit nominal execution passed at `MeanValueAvg=208` in 16.243 ms. Explicit Bright-NG execution produced expected product NG at `MeanValueAvg=76.7 < 190` in 10.07 ms. Both smoke commands returned exit code 0 because their expected outcomes were declared explicitly.
- This is a real manually transferred direct-success example with zero correction rounds. Do not send the correction prompt or manufacture a failed round. Raw reports can contain local evidence locations and must not be copied into `docs/evidence` without a separate sanitization and explicit inclusion decision.
- P63 is raw evidence plus handoff updates only. It is not committed or pushed, and the Original repository remains untouched.

## 2026-07-16 P64 Arithmetic Invert GPT Sanitized Candidate

- Created the ignored six-file artifact-only candidate at `artifacts/llm_transcripts/sanitized/20260716_arithmetic_invert_gpt_round1_direct_success`: `manifest.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged `response.xml`, and current-EXE nominal/negative result PNGs. It is not a tracked `docs/evidence` package.
- The prompt, response, nominal PNG, and Bright-NG PNG are byte-identical to P63 raw/current-EXE evidence. Payload scans found zero absolute Windows, user-home, application-data, Codex attachment, URL, email, credential-label, or private/legacy asset-hint matches.
- Both result PNGs are 572x420 and contain only `IHDR`, `IDAT`, and `IEND` chunk types. The candidate excludes raw reports and local evidence locations; its manifest records only repository-relative public inputs, hashes, outcomes, and classification limits.
- P64 does not grant repository inclusion. Do not copy it to `docs/evidence`, stage it, commit it, or publish it without a separate explicit user decision. The Original repository remains untouched.

## 2026-07-16 P65 Arithmetic Invert GPT Evidence Included In Docs

- The user explicitly approved inclusion of the P64 Arithmetic direct-success candidate in the Dev worktree. The public six-file package is `docs/evidence/llm/20260716_arithmetic_invert_gpt_direct_success`; the companion decision record is `docs/OPENVISIONLAB_LLM_ARITHMETIC_INVERT_PUBLICATION_REVIEW_20260716.md`.
- The package contains only `README.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged `response.xml`, and OpenVisionLab-generated nominal/negative result PNGs. Raw manifests, validation/run reports, local paths, runtime identifiers, stack traces, and session data remain outside the package.
- Package-wide file-set, text/privacy, public-asset, PNG metadata, and immutable-hash checks passed. Both 572x420 result PNGs contain only `IHDR`/`IDAT`/`IEND` chunk types and match P63 raw evidence plus the P65 tracked replay byte-for-byte.
- After a fresh 0-warning/0-error solution build, latest actual `bin/Debug/OpenVisionLab.exe` replayed the tracked `response.xml`: validation/import passed with 2 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; nominal passed at `MeanValueAvg=208` in 15.558 ms; Bright-NG produced expected product NG at `MeanValueAvg=76.7 < 190` in 5.929 ms. Both smoke commands returned exit code 0 through their explicit expected-result declarations.
- Exact GPT model/version, API evidence, and full provider-chat export remain unavailable. This is one constrained direct-success case with zero correction rounds, not a provider benchmark or independent algorithm-design claim. P65 is not committed or pushed, and the Original repository remains untouched.

## 2026-07-16 P66 Edge Detection Shape Count GPT Packet

- Selected the distinct public-safe `EdgeDetection(Canny) -> Morphology(Close) -> Contour` workflow as the next bounded LLM-authoring candidate. It adds EdgeDetection coverage while retaining the existing rule-based sequential pipeline and final Contour count gate; no new tool, template dependency, camera path, or platform feature was introduced.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` imported `Public_EdgeDetection_Shapes.pipeline.xml` with 3 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; nominal passed `ResultCount=4` and missing-shape input produced expected product NG `ResultCount=2 < 4`.
- Added the self-contained five-file packet `llm_prompt_packets/edge_detection_shapes`: README, first-round prompt, correction template, and byte-identical public Good/Missing-NG image copies. The prompt locks Canny `40/120`, `3`, `UseL2Gradient=true`; Morphology Close `3x3`; Contour ROI `90,100,410,95`; route `Main -> EdgeDetection_Edge -> EdgeDetection_EdgeJoin -> EdgeDetection_Shape_Preview`; and a final `ResultCount=4..4` gate.
- The exact packet reference XML replayed in the same latest EXE: import passed with 3 Steps and 0 errors/warnings, nominal passed `ResultCount=4`, and Missing-NG produced expected product NG `ResultCount=2 < 4`; all three commands returned exit code 0. Evidence is `artifacts/p66_edge_detection_baseline_20260716`; packet audit passed with five files, copied public image hashes, required prompt route/parameters/gate, one correction placeholder, and no private/path token.
- No external GPT response or correction loop exists for P66 yet. The user should send the two packet images and the complete first-round prompt in one new GPT conversation, then return the complete first XML response unchanged. Use the correction template only after an actual current-build validation or Good/NG failure. P66 is not committed or pushed; the Original repository remains untouched.

## 2026-07-16 P67 Edge Detection Shape Count GPT Direct-Success Replay

- The user returned one XML-only response in the P66 GPT packet workflow. The manually transferred XML and copied packet prompt are preserved under `artifacts/llm_transcripts/raw/20260716_edge_detection_shapes_gpt_round1`. Exact model/version, API evidence, and a full provider-chat export were not supplied and must remain unknown rather than inferred.
- XML-only boundary validation passed. The user-supplied response matched the mechanically verified three-Step EdgeDetection reference contract across 53 structured fields with zero differences.
- After a fresh 0-warning/0-error solution build, latest actual `bin/Debug/OpenVisionLab.exe` validation/import passed with 3 Steps, 0 errors, 0 warnings, no dependencies, and `ImageRun: SKIPPED`.
- Explicit nominal execution passed at `ResultCount=4` in 34.661 ms. Explicit Missing-NG execution produced expected product NG at `ResultCount=2 < 4` in 33.613 ms. Both smoke commands returned exit code 0 because their expected outcomes were declared explicitly.
- This is a real manually transferred direct-success example with zero correction rounds. Do not send the correction prompt or manufacture a failed round. Raw reports can contain local evidence locations and must not be copied into `docs/evidence` without a separate sanitization and explicit inclusion decision.
- P67 is raw evidence plus handoff updates only. It is not committed or pushed, and the Original repository remains untouched.

## 2026-07-16 P68 Edge Detection Shape Count GPT Sanitized Candidate

- Created the ignored six-file artifact-only candidate at `artifacts/llm_transcripts/sanitized/20260716_edge_detection_shapes_gpt_round1_direct_success`: `manifest.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged `response.xml`, and current-EXE nominal/negative result PNGs. It is not a tracked `docs/evidence` package.
- The prompt, response, nominal PNG, and Missing-NG PNG are byte-identical to P67 raw/current-EXE evidence. Payload scans found zero absolute Windows, user-home, application-data, Codex attachment, URL, email, credential-label, or private/legacy asset-hint matches.
- Both result PNGs are 572x420 and contain only `IHDR`, `IDAT`, and `IEND` chunk types. The candidate excludes raw reports and local evidence locations; its manifest records only repository-relative public inputs, hashes, outcomes, and classification limits.
- P68 was the artifact-only candidate gate. P69 records the user's subsequent explicit Dev-worktree inclusion decision; the Original repository remains untouched.

## 2026-07-16 P69 Edge Detection Shape Count GPT Evidence Included In Docs

- The user approved inclusion of the P68 Edge Detection direct-success candidate in the Dev worktree. The public six-file package is `docs/evidence/llm/20260716_edge_detection_shapes_gpt_direct_success`; the companion decision record is `docs/OPENVISIONLAB_LLM_EDGE_DETECTION_PUBLICATION_REVIEW_20260716.md`.
- The package contains only `README.md`, `privacy_review.md`, unchanged `prompt.md`, unchanged `response.xml`, and OpenVisionLab-generated nominal/negative result PNGs. Raw manifests, validation/run reports, local paths, runtime identifiers, stack traces, and session data remain outside the package.
- Package-wide file-set, text/privacy, public-asset, PNG metadata, and immutable-hash checks passed. Both 572x420 result PNGs contain only `IHDR`/`IDAT`/`IEND` chunk types and match P67 raw evidence plus the P69 tracked replay byte-for-byte.
- After a fresh 0-warning/0-error solution build, latest actual `bin/Debug/OpenVisionLab.exe` replayed the tracked `response.xml`: validation/import passed with 3 Steps, 0 errors/warnings, no dependencies, and `ImageRun: SKIPPED`; nominal passed at `ResultCount=4` in 33.914 ms; Missing-NG produced expected product NG at `ResultCount=2 < 4` in 30.826 ms. Both smoke commands returned exit code 0 through their explicit expected-result declarations.
- Exact GPT model/version, API evidence, and full provider-chat export remain unavailable. This is one constrained direct-success case with zero correction rounds, not a provider benchmark or independent algorithm-design claim. P69 is not committed or pushed, and the Original repository remains untouched.

## 2026-07-16 P70 RotateScale Geometry Baseline And GPT Packet

- Selected the distinct public-safe `RotateScale` geometry workflow as the next bounded external-authoring candidate. The one-Step contract resizes `Main` to `Geometry_ResizeHalf_Result` with Angle 0, 50 percent X/Y scale, Linear interpolation, and Constant border. It gates `ResultImageWidth=286..286`; it is an output-size check, not object detection, physical calibration, or a new platform feature.
- The first current-EXE baseline exposed a real review-copy gap: a valid `UseAcceptance` metric/range was not treated as explicit judgement evidence. `HasJudgementParameter` now recognizes an enabled acceptance metric with a configured minimum or maximum. This changes only the LLM review explanation; it does not alter XML execution, Preview/Run, layer creation, active-layer selection, or routing.
- After a fresh 0-warning/0-error build, latest actual `bin/Debug/OpenVisionLab.exe` import passed with 1 Step, 0 errors/warnings, no dependencies, `ImageRun: SKIPPED`, and `Inspection.Evidence: OK - explicit judgement criteria are present.` Explicit nominal execution passed `572x420 -> 286x210` at `ResultImageWidth=286` in 3.428 ms. The Wide-NG input produced expected product NG `640x420 -> 320x210`, `ResultImageWidth=320 > 286`, in 3.18 ms.
- Added the self-contained five-file GPT packet at `llm_prompt_packets/rotate_scale_geometry`: README, first-round prompt, correction template, and byte-identical public nominal/Wide-NG images. Packet audit passed: five files, copied public image hashes, locked route/parameters/gate, one correction placeholder, and no private/path token. The baseline review is `docs/OPENVISIONLAB_LLM_ROTATE_SCALE_BASELINE_REVIEW_20260716.md`; the authoring guide now includes a `RotateScale` geometry example.
- No real GPT/Gemini/Claude response exists for P70 yet. Send the two packet images and complete `COPY_THIS_TO_GPT.txt` prompt in one new GPT conversation, preserve the first complete XML response unchanged, and use the correction template only after an actual current-build validation or Good/Wide-NG failure. P70 is not committed or pushed, and the Original repository remains untouched.

## 2026-07-17 P71 Pipeline Review Inspection Readiness

- Added a compact, read-only `검사 준비도` strip to Pipeline Review. It separates input image, enabled Step/route validation, acceptance criteria, Good/Bad evidence, and unit calibration into five visible states before the operator chooses `리뷰 실행`.
- Input readiness preserves the downstream-input contract: an input produced by an earlier enabled Step is not reported as missing. An actual unloaded external input is reported by layer name. Route errors, route warnings, missing OK/NG criteria, missing comparison pairs, and `PIXELPERMM` calibration states remain explanatory and do not invoke Preview/Run or alter layer/routing state.
- The readiness calculation lives in `OpenVisionPipelineReviewReadinessPresenter`; the ViewModel exposes display state only, and the View code-behind only forwards state/localization. Opening Review, language changes, Step selection, and readiness refresh do not change `CanRunReview`, create layers, or execute tools.
- Fresh current-source evidence is under `artifacts/pipeline_review_readiness_20260716`: `before/wpf_shell_host_pipeline_review.png`, `after/wpf_shell_host_pipeline_review.png`, and `after/wpf_shell_host_pipeline_review_input_state.png`. The 1180x660 checks reported zero layout, text, or internal clipping issues. The main review area is clipped to its bounds and the lower detail row is 160px so the added strip does not push the status line outside the compact viewport.
- After a fresh 0-warning/0-error solution build, latest actual `bin/Debug/OpenVisionLab.exe` passed `recipe-pipeline-roundtrip`. Its report records no native Preview runs, one unchanged layer, no recipe sample execution, and an isolated explicit Review result. The actual-EXE Pipeline Review screenshot is `artifacts/pipeline_review_readiness_20260716/direct_exe/OpenVisionLab_PipelineReview_Roundtrip.png`.
- Screenshot smoke now verifies all five readiness controls, Korean/English refresh, actual missing-input wording, no automatic Preview/Run, positive `PIXELPERMM=0.006`, and rejection of non-positive calibration. Readiness, localization-catalog, external-reference, public-sample, and diff checks passed. P71 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P71 is connecting the 17 remaining Learn concepts to real tool/sample/explicit-run practice, followed by stronger industrial Validation Set evidence labels and measured rotate/scale/metrology reliability work.

## 2026-07-17 P72 Threshold Learn Practice Connection

- Completed the first of the 17 Learn practice connections with Threshold. The topic now names the exact public pair `Public_Threshold_BandPads_Good` / `Public_Threshold_BandPads_Missing_Bad`, the shared `Public_Threshold_BandPads` Pipeline, and the expected `ResultCount` values 4 / 1 before the operator changes Threshold GV.
- Added the dedicated Sample Picker Learn path `threshold`. Its classifier uses sample category/name rather than generic Pipeline flow, so Filter and Morphology Pipelines that also contain a Threshold Step do not leak into this focused pair. The curriculum regression confirms the path contains exactly the two public Threshold samples.
- Added `Threshold Tool 열기` inside the Threshold concept tab. It reuses the existing Shell related-tool callback and opens `ThresholdToolWpfView` only. It does not execute Preview/Run, create a result layer, or change the workspace layer. Preview/Run remains an explicit operator action.
- Fresh current-source before evidence: `artifacts/p72_learn_threshold_practice_20260717/before/wpf_openvision_learn_threshold/wpf_openvision_learn_threshold.png`. Valid after evidence: `artifacts/p72_learn_threshold_practice_20260717/after_r2/wpf_openvision_learn_threshold/wpf_openvision_learn_threshold.png`. The first after render had a transient WPF composition blackout and is excluded.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke learn-threshold-practice` under `artifacts/p72_learn_threshold_practice_20260717/direct_exe_r2`; its report records `PracticePath: threshold`, `PreviewRunCount: 0`, `LayerCount: 0`, and `Tool: ThresholdToolWpfView`. The actual-EXE capture is `OpenVisionLab_Learn_Threshold_Practice.png` in that folder.
- `wpf_shell_host_learn_entry`, `wpf_openvision_learn_curriculum`, readiness, localization catalog (`1733/78`), external-reference, public-sample (`30/229/15`), and diff checks passed. P72 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P72 is the Filter Learn practice connection using the existing `Public_Filter_Denoise_Good` / `Public_Filter_Denoise_Missing_Bad` pair and explicit `ResultCount=4/2` evidence. Sixteen Learn topics remain after this first completed connection.

## 2026-07-17 P73 Filter Learn Practice Connection

- Completed the second Learn practice connection with Filter. The Filtering topic now names the exact public pair `Public_Filter_Denoise_Good` / `Public_Filter_Denoise_Missing_Bad`, the shared `Public_Filter_Denoise` Pipeline, and the expected `ResultCount` values 4 / 2 before the operator opens the Filter Tool or explicitly runs Preview/Pipeline Review.
- Added the dedicated Sample Picker Learn path `filter`. Its classifier uses Filter category/name data, and the curriculum regression proves it contains exactly the two public Filter samples rather than the broader Threshold/Morphology/EdgeDetection preprocessing set. The focused path resolves `LEARN_FILTER.md` for the beginner-facing document action.
- Added a compact Filter concept-tab practice card. It links the top `실습 샘플` action with the existing `Filter Tool 열기` action and states the comparison sequence: open the pair, inspect Median Kernel, then explicitly Preview or Run Review to compare denoising and final count. No new execution command, layer write, or routing behavior was added.
- The first before-capture attempt exposed a stale smoke assertion: the initial Filter guide already says `Median`, `Bilateral`, and `Preview`, while the test incorrectly expected post-tool-open copy about input/output comparison. The assertion now checks the actual initial guide without changing visible UI behavior. The valid visual baseline is `artifacts/p73_learn_filter_practice_20260717/before_r2/wpf_openvision_learn_filtering/wpf_openvision_learn_filtering.png`; the failed first attempt is not UI evidence.
- Current-source after evidence is `artifacts/p73_learn_filter_practice_20260717/after/wpf_openvision_learn_filtering/wpf_openvision_learn_filtering.png`. It was visually checked for clipped text, icons, combo/input text, and overlap; the new card and existing scroll region remain legible at 1040x900.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke learn-filter-practice` under `artifacts/p73_learn_filter_practice_20260717/direct_exe`; its report records `PracticePath: filter`, `PreviewRunCount: 0`, `LayerCount: 0`, and `Tool: FilterToolWpfView`.
- Current-source `wpf_openvision_learn_filtering`, global `wpf_openvision_learn_curriculum`, and `wpf_shell_host_learn_entry` smokes passed. Readiness, localization catalog (`1733/78`), external-reference, public-sample (`30/229/15`), and `git diff --check` passed. P73 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P73 is the Morphology Learn practice connection with `Public_Morphology_Cleanup_Good` / `Public_Morphology_Cleanup_Missing_Bad` and explicit `ResultCount=4/2` evidence. Fifteen Learn topics remain after the Threshold and Filter connections.

## 2026-07-17 P74 Morphology Learn Practice Connection

- Completed the third Learn practice connection with Morphology. The Morphology topic now names the exact public pair `Public_Morphology_Cleanup_Good` / `Public_Morphology_Cleanup_Missing_Bad`, the shared `Public_Morphology_Cleanup` Pipeline, and the expected `ResultCount` values 4 / 2. It tells the operator to inspect `Open`, `Rect`, and the 5x5 kernel before an explicit Preview or Pipeline Review run.
- Added the dedicated Sample Picker Learn path `morphology`. Its classifier uses the Morphology category/name data, and the curriculum regression proves it contains exactly the two public Morphology samples rather than all Threshold preprocessing samples. The focused path resolves `LEARN_MORPHOLOGY.md` for the beginner-facing document action.
- Added a compact Morphology concept-tab practice card. It connects the existing top `실습 샘플` action with the existing `Morphology Tool 열기` action and preserves the existing Opening animation. No new execution command, layer write, or routing behavior was added.
- Fresh current-source before evidence is `artifacts/p74_learn_morphology_practice_20260717/before/wpf_openvision_learn_morphology/wpf_openvision_learn_morphology.png`; after evidence is `artifacts/p74_learn_morphology_practice_20260717/after/wpf_openvision_learn_morphology/wpf_openvision_learn_morphology.png`. Both were inspected at 1040x700; the practice card, existing animation, and Tool entry remain legible without clipped text or overlap.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke learn-morphology-practice` under `artifacts/p74_learn_morphology_practice_20260717/direct_exe`; its report records `PracticePath: morphology`, `PreviewRunCount: 0`, `LayerCount: 0`, and `Tool: MorphologyToolWpfView`.
- Current-source `wpf_openvision_learn_morphology`, global `wpf_openvision_learn_curriculum`, and `wpf_shell_host_learn_entry` smokes passed. Readiness, localization catalog (`1733/78`), external-reference, public-sample (`30/229/15`), and `git diff --check` passed. P74 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P74 is the Blob Learn practice connection with `Public_Blob_Particles_Good` / `Public_Blob_Particles_Sparse_Bad` and explicit `ResultCount=8..14 / 2..4` evidence. Fourteen Learn topics remain after the Threshold, Filter, and Morphology connections.

## 2026-07-17 P75 Blob Learn Practice Connection

- Completed the fourth Learn practice connection with Blob. The Blob topic now names the exact public pair `Public_Blob_Particles_Good` / `Public_Blob_Particles_Sparse_Bad`, the shared `Public_Blob_Particles` Pipeline, and the expected `ResultCount` bands: 8..14 is OK and 2..4 is NG. It directs the operator to inspect `MIN_AREA` and `MAX_AREA` before an explicit Preview or Pipeline Review run.
- Narrowed the existing Sample Picker Learn path `blob` to Blob category/name data. The curriculum regression proves it contains exactly the two public Blob samples instead of broad Particle/Density/Stain-related samples. The existing focused path continues to resolve `LEARN_BLOB.md`.
- Added a compact Blob concept-tab practice card. It connects the existing top `실습 샘플` action with the existing `Blob Tool 열기` action and preserves the existing candidate/area animation. No new execution command, layer write, or routing behavior was added.
- Fresh current-source before evidence is `artifacts/p75_learn_blob_practice_20260717/before/wpf_openvision_learn_blob/wpf_openvision_learn_blob.png`; after evidence is `artifacts/p75_learn_blob_practice_20260717/after/wpf_openvision_learn_blob/wpf_openvision_learn_blob.png`. Both were inspected at 1040x700; the practice card, existing animation, and Tool entry remain legible without clipped text or overlap.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke learn-blob-practice` under `artifacts/p75_learn_blob_practice_20260717/direct_exe`; its report records `PracticePath: blob`, `PreviewRunCount: 0`, `LayerCount: 0`, and `Tool: BlobToolWpfView`.
- Current-source `wpf_openvision_learn_blob`, global `wpf_openvision_learn_curriculum`, and `wpf_shell_host_learn_entry` smokes passed. Readiness, localization catalog (`1733/78`), external-reference, public-sample (`30/229/15`), and `git diff --check` passed. P75 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P75 is the Contour Learn practice connection with `Public_Contour_Shapes_Good` / `Public_Contour_Shapes_Missing_Bad` and explicit `ResultCount=5/2` evidence. Thirteen Learn topics remain after the Threshold, Filter, Morphology, and Blob connections.

## 2026-07-17 P76 Contour Learn Practice Connection

- Completed the fifth Learn practice connection with Contour. The Contour topic now names the exact public pair `Public_Contour_Shapes_Good` / `Public_Contour_Shapes_Missing_Bad`, the shared `Public_Contour_Shapes` Pipeline, and the expected `ResultCount` values 5 / 2. It directs the operator to inspect Retrieval mode, `MIN_AREA`, `MAX_AREA`, and contour display before an explicit Preview or Pipeline Review run.
- Narrowed the existing Sample Picker Learn path `contour` to Contour category/name data. The curriculum regression proves it contains exactly the two public Contour samples instead of broad Shape/Region/Surface/Fiducial-related samples. The existing focused path continues to resolve `LEARN_CONTOUR.md`.
- Added a compact Contour concept-tab practice card. It connects the existing top `실습 샘플` action with the existing `Contour Tool 열기` action and preserves the existing boundary/shape animation. No new execution command, layer write, or routing behavior was added.
- The first before-capture attempt exposed a stale screenshot-smoke assertion: the existing initial guide says `Approximation` and display color/line thickness, while the test expected a non-visible `Approx epsilon` phrase. The assertion now verifies the actual initial PropertyGrid/result-metric guide without changing visible UI behavior. Valid visual baseline is `artifacts/p76_learn_contour_practice_20260717/before_r2/wpf_openvision_learn_contour/wpf_openvision_learn_contour.png`; the first failed attempt is not UI evidence.
- Current-source after evidence is `artifacts/p76_learn_contour_practice_20260717/after/wpf_openvision_learn_contour/wpf_openvision_learn_contour.png`. The before and after views were inspected at 1040x700; the practice card, existing animation, and Tool entry remain legible without clipped text or overlap.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke learn-contour-practice` under `artifacts/p76_learn_contour_practice_20260717/direct_exe`; its report records `PracticePath: contour`, `PreviewRunCount: 0`, `LayerCount: 0`, and `Tool: ContourToolWpfView`.
- Current-source `wpf_openvision_learn_contour`, global `wpf_openvision_learn_curriculum`, and `wpf_shell_host_learn_entry` smokes passed. Readiness, localization catalog (`1733/78`), external-reference, public-sample (`30/229/15`), and `git diff --check` passed. P76 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P76 is the EdgeDetection Learn practice connection with `Public_EdgeDetection_Shapes_Good` / `Public_EdgeDetection_Shapes_Missing_Bad` and explicit downstream `ResultCount=4/2` evidence. Twelve Learn topics remain after the Threshold, Filter, Morphology, Blob, and Contour connections.

## 2026-07-17 P77 EdgeDetection Learn Practice Connection

- Completed the sixth Learn practice connection with EdgeDetection. The Edge / Line topic now names the exact public pair `Public_EdgeDetection_Shapes_Good` / `Public_EdgeDetection_Shapes_Missing_Bad`, the shared `Public_EdgeDetection_Shapes` Pipeline, and the expected downstream `ResultCount` values 4 / 2. The card makes the boundary clear: EdgeDetection creates the edge map, while Morphology/Contour supplies the final count; distance or width measurement remains in the following LineDistance topic.
- Added the dedicated Sample Picker Learn path `edge-detection`. Its classifier uses EdgeDetection category/name data, and curriculum regression proves it contains exactly the two public EdgeDetection samples rather than the broad preprocessing set. The focused path resolves `LEARN_EDGE_DETECTION.md`, whose path reference was updated to `edge-detection`.
- Added a compact EdgeDetection concept-tab practice card. It connects the existing top Practice Samples action with the existing EdgeDetection Tool action, asks the operator to inspect Canny Low/High and L2 Gradient, then explicitly Preview or Pipeline Review the edge map and final downstream count. No execution command, layer write, active-layer selection, or routing behavior was added.
- The first two before-capture attempts exposed stale screenshot assertions only: one expected post-tool Canny/Line copy from the initial screen, and one expected the non-visible internal name `LineGauge`. The assertions now check the visible `EdgeDetection` / `LineDistance` role boundary. No visible UI behavior changed during those assertion fixes. Valid before evidence is `artifacts/p77_learn_edge_detection_practice_20260717/before_r3/wpf_openvision_learn_edge_line/wpf_openvision_learn_edge_line.png`; current-source after evidence is `artifacts/p77_learn_edge_detection_practice_20260717/after/wpf_openvision_learn_edge_line/wpf_openvision_learn_edge_line.png`.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke learn-edge-detection-practice` under `artifacts/p77_learn_edge_detection_practice_20260717/direct_exe_final`; its report records `PracticePath: edge-detection`, `PreviewRunCount: 0`, `LayerCount: 0`, and `Tool: SimplePreprocessToolWpfView`.
- Current-source `wpf_openvision_learn_edge_line`, global `wpf_openvision_learn_curriculum`, and `wpf_shell_host_learn_entry` smokes passed. Readiness, localization catalog (`1733/78`), external-reference, public-sample (`30/229/15`), and `git diff --check` passed; the diff check emitted CRLF normalization warnings only. P77 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P77 is the LineDistance Learn practice connection using `Public_Line_Pins_Good` / `Public_Line_Pins_WidePin_Bad`. It must show the Good/Bad measurement evidence and retain a Range/Max outlier gate beside the average distance. Eleven Learn topics remain after the Threshold, Filter, Morphology, Blob, Contour, and EdgeDetection connections.

## 2026-07-17 P78 LineDistance Learn Practice Connection

- Completed the seventh Learn practice connection with LineDistance. The topic now names `Public_Line_Pins_Good` / `Public_Line_Pins_WidePin_Bad`, the shared `Public_Line_Pins_Distance` Pipeline, and the operator order: check `DistanceMmRange <= 0.03` first, then check `DistanceMmAvg=0.20..0.25` only after the sampling lines are consistent.
- Strengthened the public Line pipeline from an average-only gate to two explicit LineDistance Steps. Step 01 writes `Line_Range_Preview` and gates `DistanceMmRange <= 0.03`; Step 02 explicitly branches from `Main` with `ALLOW_BRANCH_INPUT=true`, writes the existing `Line_Preview`, and retains the nominal average gate. This keeps Preview/Run explicit and does not alter input-layer selection automatically.
- Current runner evidence: Good completed both Steps with `DistanceMmRange=0.012` and `DistanceMmAvg=0.222`. WidePin Bad stopped at the first consistency Step with `DistanceMmRange=0.095 > 0.03`; the public catalog now declares its expected failure metric as `DistanceMmRange=0.08..0.11`. The full 30-row public catalog gate passed with `GateStatus=OK`.
- Narrowed the existing Sample Picker `line` path to the `Public Synthetic / Line` category/name pair. Curriculum and Shell smokes prove it exposes exactly the two public Line samples and opens the `line` path without Preview/Run, layer creation, or routing changes.
- Added a compact LineDistance practice card and clarified the existing tool-location copy to name `DistanceMmAvg` and `DistanceMmRange/Max`. Fresh visual baseline is `artifacts/p78_line_distance_practice_20260717/before_r2/wpf_openvision_learn_line_distance/wpf_openvision_learn_line_distance.png`; current-source after evidence is `artifacts/p78_line_distance_practice_20260717/after/wpf_openvision_learn_line_distance/wpf_openvision_learn_line_distance.png`. Both were inspected at 1040x700 for clipping, overlap, and readable control text.
- The first baseline smoke exposed stale assertion text only: it expected English `Range gate`, `Pixel / mm`, and `Gate rule`, while the visible Korean screen presents `Range`, `Pixel/mm`, and `DISTANCE_RANGE_MAX`. The smoke now verifies the visible terms. No execution behavior changed through that fix.
- A fresh full solution build passed with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke learn-line-distance-practice` under `artifacts/p78_line_distance_practice_20260717/direct_exe_final`; its report records `PracticePath: line`, `PreviewRunCount: 0`, `LayerCount: 0`, and `Tool: LineToolWpfView`.
- Current-source `wpf_openvision_learn_line_distance`, global `wpf_openvision_learn_curriculum`, and `wpf_shell_host_learn_entry` smokes passed. Readiness, external-reference, public-sample (`30/229/15`), full public catalog (`30/30`), and `git diff --check` passed; diff check emitted CRLF normalization warnings only. P78 is not committed or pushed, and the Original repository remains untouched.
- Next product priority after P78 is a bounded industrial Validation Set evidence pass: make sample status, expected Good/Bad evidence, acceptance metric, calibration applicability, and the next explicit operator action readable without turning Recipe Manager into a second Pipeline editor. Ten Learn topics remain after Threshold, Filter, Morphology, Blob, Contour, EdgeDetection, and LineDistance.

## 2026-07-17 P79 Validation Set Evidence Board

- Added a compact read-only evidence board above the existing local Validation Set editor. It shows expected OK/NG and missing-file status, the selected Pipeline acceptance gate, whether physical calibration applies, and the next explicit operator action. It is derived from the selected set and Pipeline only; opening or selecting it does not run Preview/Run, create layers, or change routing.
- When the Pipeline has no metric acceptance gate, the board says that the Pipeline Good/Bad result must be compared with the expected Good/Bad role instead of inventing a numeric criterion. When a selected metric uses `Mm`, it checks for a positive `PIXELPERMM`; non-physical gates are explicitly marked as not requiring calibration.
- Fresh baseline evidence is `artifacts/p79_validation_set_evidence_20260717/before/wpf_shell_host_recipe_local_validation_set.png`. Current-source after evidence is `artifacts/p79_validation_set_evidence_20260717/after_current_source_r4/wpf_shell_host_recipe_local_validation_set.png`; the focused screenshot smoke passed with `layout=0`, `text=0`, and `internal=0` after catching and correcting the initial grid-row overlap.
- A fresh full solution build passed with 0 warnings and 0 errors. The latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke recipe-manager-tabs` under `artifacts/p79_validation_set_evidence_20260717/direct_exe_recipe_manager_tabs_retry`; its report confirms local-set file/folder registration and missing-path repair preserve Preview/Run count, layers, and routing.
- Next implementation priority is source organization: define durable folder and responsibility rules, inventory the flat `1. Core` files, and move one clean cohesive group at a time without mixing moves with behavior changes. Do not move already modified files as part of the first cleanup bundle.

## 2026-07-17 P80 Core Source Organization

- Added durable source-organization rules to `AGENTS.md`: folder ownership signals, Core/WPF boundaries, no dumping folders, View code-behind responsibilities, small clean move groups, namespace preservation, and build/smoke verification after every group.
- Replaced the flat Core layout with physical-only responsibility folders while preserving namespaces and public APIs: `State` (5 files), `Display` (9), `Recipe` (5), `Pipeline/Definition` (6), `Pipeline/Execution` (6), `Pipeline/Storage` (5), `Pipeline/Validation` (3), and `Pipeline/Tools` (7). The Core root now contains only `desktop.ini` and the already-modified `VisionPipelineValidation.cs`.
- `VisionPipelineValidation.cs` deliberately remains at the old root path because it was dirty before the organization pass. Do not move it until its current behavior change is stabilized and verified as a separate clean move; this avoids mixing a user-visible validation change with a physical refactor.
- Updated `tools/OpenVisionReadinessCheck` to inspect the new explicit Validation and Tools paths. The check does not retain hidden old-path fallback behavior, so future structure regressions are visible.
- Every move group passed a full Debug solution build with 0 warnings and 0 errors. Latest `bin/Debug/OpenVisionLab.exe` passed `--smoke recipe-pipeline-roundtrip` under `artifacts/p80_core_organization_20260717/direct_exe_recipe_pipeline_roundtrip`; it preserved the no-auto-run/layer/routing contract and the explicit-run `WAIT` state.
- Final readiness, external-reference, and public-sample checks passed; `git diff --check` passed with existing CRLF normalization warnings only. P80 is not committed or pushed, and the Original repository remains untouched.
- Next source-organization priority is conditional: after the current `VisionPipelineValidation.cs` work is stable, move it to `Pipeline/Validation` in a separate clean change. Then inventory the largest WPF files for natural Presenter/ViewModel boundaries without splitting files merely by line count. Product priority remains RotateScale/measurement reliability and the remaining Learn practice connections.

## 2026-07-17 P81 MENU WPF Source Organization

- Applied the same physical-only ownership organization to `0. UI/0) MENU`. The WPF root fell from 141 C# files to 16; the `0) MENU` top level now has no production C# file. The retained top-level `0) MENU.zip` and `desktop.ini` were deliberately not deleted or rewritten.
- Staged 126 clean WPF source moves without namespace, public API, XAML, Preview/Run, layer, routing, or docking behavior changes. The owner layout is: `Docking` (19, including `Contracts` and `TestSupport`), `NativeTools` (36), `Viewer` (6), `Windows` (2), shared `Tooling` (2), `PipelineReview` (2 presenters), `Recipe` (10), `Workspace` (5), and `Shell` (44 across `Chrome`, `Commands`, `Layers`, `Session`, `Tooling`, `Workspace`, `Recipe`, `Documents`, `State`, and `Support`).
- The five `OpenVisionShellHostDockedLayerOrchestrator` partial files moved together under `Shell/Layers/Orchestration`. `MainWorkspaceLayoutState` moved from the MENU top level to `Wpf/Workspace/State`.
- Kept XAML/code-behind pairs, the dirty Shell host/menu/recipe files, dirty Workspace Learn files, dirty Pipeline Review files, and untracked new presenters/intent skills in place. They require a separate ownership review rather than a speculative move mixed with the existing behavior changes.
- Read-only boundary audit of `OpenVisionShellHostRecipeCommandSurface` found 12,089 lines, 54 active diff hunks, and roughly 20 recipe validation/batch/sample row-or-option model classes beginning near line 9,827. One active hunk also falls in that model range, so do not extract `Recipe/Models` yet. After the current dirty feature work is stabilized, first move only clean model classes, then review validation/batch command behavior for Presenter or Controller boundaries.
- Updated `OpenVisionReadinessCheck` explicit paths for moved Recipe intent skills, Workspace sample-picker ViewModel, Shell sample workflow, hosted document controller, state presenter, and command catalog. One initially missed `OpenVisionShellCommandCatalog` path caused a readiness failure; it was corrected immediately. A final preflight confirmed all 15 direct WPF readiness paths exist, so the check has no hidden fallback search.
- Every move bundle passed `dotnet build OpenVisionLab.sln -c Debug -p:Platform=Any CPU` with 0 warnings and 0 errors. Latest actual `bin/Debug/OpenVisionLab.exe` passed `--smoke recipe-pipeline-roundtrip` under `artifacts/p81_menu_wpf_organization_20260717/direct_exe_recipe_pipeline_roundtrip`: `NativePreviewRuns=0`, `LayerCount=1`, no recipe-sample execution, and downstream input remains explicit-run `WAIT`.
- Final readiness, external-reference, and public-sample checks passed. P81 is not committed or pushed, and the Original repository remains untouched.
- Next source-organization priority is a behavior-neutral responsibility review of the remaining large dirty Shell host surface. Do not move it for cosmetics; first identify natural Presenter/Controller/ViewModel extraction points. Product priority remains measurement reliability and remaining Learn practice connections.

## 2026-07-17 P82 Recipe Command Surface Model Ownership

- The user explicitly continued the source-cleanup work, so the top-level recipe DTO tail was separated from the already-dirty `OpenVisionShellHostRecipeCommandSurface` without changing names, namespaces, callbacks, command behavior, Preview/Run behavior, layer routing, or UI copy. The command surface now owns command coordination plus the shared `OpenVisionRecipeText` helper; it no longer owns top-level validation/review/sample/batch DTO declarations.
- `OpenVisionShellHostRecipeCommandSurface.cs` was reduced from 12,089 to 9,827 lines. Ten validation/review/operator DTOs now live in `Wpf\Recipe\Models\OpenVisionRecipeValidationReviewModels.cs` (884 lines), and ten sample/pair/batch DTOs now live in `Wpf\Recipe\Models\OpenVisionRecipeSampleRunModels.cs` (1,420 lines). The previously dirty `OpenVisionRecipeBatchRunComparisonRow` change was preserved as part of its exact model extraction.
- Structural proof confirms each of the 20 model declarations exists once, in its new owner file only; the command surface contains no `public sealed class OpenVisionRecipe...` DTO declaration. `OpenVisionRecipeText` intentionally remains alongside the command surface because both it and the extracted models reference the same localized text helper.
- `AGENTS.md` now defines `Recipe\Models` as the owner for top-level recipe validation, review, sample-run, and batch-result DTOs. `OpenVisionReadinessCheck` now requires those exact model paths and rejects their old command-surface declarations, preventing silent regression to the flat layout.
- A fresh 0-warning/0-error solution build passed. Latest actual `bin/Debug/OpenVisionLab.exe` passed both `recipe-manager-tabs` and `recipe-pipeline-roundtrip` under `artifacts/p82_recipe_models_refactor_20260717`; the former exercised Recipe Manager summary/advanced review, validation suite, LLM intent, branch comparison, and explicit XML apply, while the latter preserved the no-auto-run/layer/routing `WAIT` contract.
- Readiness, external-reference, and public-sample checks passed. P82 is not committed or pushed, and the Original repository remains untouched.
- Do not continue slicing command behavior from this dirty surface merely to reduce line count. The next source refactor must identify one complete execution responsibility and prove its call path separately. Product priority remains measurement reliability and remaining Learn practice connections.

## 2026-07-17 P83 Remaining MENU Root Owner Cleanup

- Moved the remaining untracked owner-ready WPF files out of the MENU root without content changes: `OpenVisionPipelineReviewReadinessPresenter` now belongs to `PipelineReview\Presenters`; `OpenVisionRecipeEdgeBasedMatchingIntentSkill` and `OpenVisionRecipeFeatureMatchingIntentSkill` now belong to `Recipe\IntentSkills`. The WPF root now contains only XAML/code-behind pairs and files that are still dirty or require a later responsibility decision.
- Added explicit readiness paths for all three files. The structural contract now verifies that Pipeline Review readiness and the EdgeBasedMatching/FeatureMatching XML starters cannot silently drift back to the root directory.
- A fresh 0-warning/0-error solution build passed. Latest actual `bin/Debug/OpenVisionLab.exe` passed `recipe-pipeline-roundtrip` under `artifacts/p83_menu_root_owner_cleanup_20260717/recipe-pipeline-roundtrip`, preserving no auto Preview/Run, layer count, active-layer, routing, and explicit produced-input `WAIT` behavior.
- The current `recipe-manager-tabs` attempt used the same latest EXE but could not open the Windows clipboard (`CLIPBRD_E_CANT_OPEN`) while pasting the smoke XML draft. This is an external clipboard-lock limitation, not a product assertion failure; the report is retained under `artifacts/p83_menu_root_owner_cleanup_20260717/recipe-manager-tabs`. Do not treat that scenario as current passing evidence until rerun succeeds in an unlocked desktop session.
- Readiness, external-reference, and public-sample checks passed. P83 is not committed or pushed, and the Original repository remains untouched.
- Next priority should leave source cosmetic work alone: either define one fully bounded command-execution ownership refactor with dedicated smoke evidence, or return to the higher product priority of measurement reliability and remaining Learn practice connections.

## 2026-07-17 P84 LineDistance Measurement Evidence And Scale Validation

- Corrected the LineDistance Bad-sample review smoke to use the catalog's real `DistanceMmRange` evidence. The WidePin Bad run now proves the intended explicit-run state: Step 01 fails `DistanceMmRange=0.095 > 0.03`, Step 02 remains `WAIT`, and the review shows `0 OK / 1 NG / 1 WAIT` without selecting a step causing Preview/Run.
- Clarified Good/Bad pair wording in Pipeline Review. The result now separates the active Pipeline gate from the sample-evidence band, for example `Pipeline criterion <= 0.03 / NG sample band 0.08~0.11`, so a Bad sample that correctly matches its expected catalog band is not presented as passing the Pipeline criterion.
- Changed mm-scale readiness from a false completion signal to an advisory reference check. A positive `PIXELPERMM` now means `Verify reference ... mm/px`; px-only acceptance gates are not calibration-required, and mm gates with a missing or non-positive scale remain an explicit check. This does not claim physical calibration and does not add camera, calibration hardware, or platform scope.
- Current-source screenshots: baseline `artifacts/p84_measurement_scale_evidence_20260717/before_calibration/wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`; final `artifacts/p84_measurement_scale_evidence_20260717/after_calibration_final/wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`. The final 1180x660 screenshot smoke passed with `layout=0`, `text=0`, and `internal=0`.
- A fresh 0-warning/0-error solution build and screenshot-tool build passed. Current-source `wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics` and `wpf_shell_host_pipeline_review` passed; the latter explicitly checks configured-mm advisory, px-only N/A, and missing-mm Check states. The latest Debug EXE passed `--smoke recipe-pipeline-roundtrip` under `artifacts/p84_measurement_scale_evidence_20260717/recipe_pipeline_roundtrip_final`.
- Readiness, external-reference, and public-sample checks passed. `git diff --check` passed with pre-existing CRLF normalization warnings only. P84 is not committed or pushed, and the Original repository remains untouched.
- Next product priority is the LineDistance/RotateScale Learn practice connection: teach image-size transformation separately from physical measurement, require a reference-object check before treating mm values as decision evidence, and keep the existing explicit Preview/Run path.

## 2026-07-17 P85 Full Source Ownership Pass

- Completed a source-ownership pass across `0. UI\\6) Vision Test\\Wpf`, `0. UI\\6) Vision Test`, `1. Core`, and the owner-ready portion of `0. UI\\0) MENU\\Wpf`. The direct `Vision Test\\Wpf` root no longer contains C# or XAML source files. Its former 107 direct-root artifacts now have explicit owners under `Tooling`, `ToolViews`, and `Learn`; namespaces, XAML `x:Class`, public names, and behavior were preserved.
- The outer `Vision Test` root is also clear. `VisionPipelineDesignTime` now belongs to `Composition`; sample-catalog metadata belongs to `1. Core\\Pipeline\\Storage`; and sample-check execution belongs to `1. Core\\Pipeline\\Execution`. The former Core-root `VisionPipelineValidation` now belongs to `1. Core\\Pipeline\\Validation`. SHA-256 checks confirmed the four moved non-XAML source files retained identical content.
- MENU owner-ready files moved by responsibility: floating/title-bar windows live in `Wpf\\Windows`, viewer documents in `Wpf\\Viewer`, sample-picker and Learn-sample support in `Wpf\\Workspace\\Samples`, and the host menu presenter in `Wpf\\Shell\\Chrome`. Moved XAML resource URIs were corrected and smoke-tested; do not duplicate the common theme to compensate for a path error.
- Extracted two real independent helpers from the still-large dirty `OpenVisionShellHostRecipeCommandSurface`: `OpenVisionGuidedSetupCatalog` now belongs to `Wpf\\Recipe\\IntentSkills`, and `OpenVisionRecipeText` belongs to `Wpf\\Recipe\\Models`. This is a behavior-neutral responsibility split, not a cosmetic file move.
- `OpenVisionReadinessCheck` now enforces the ownership shape: no direct source files in the Core or Vision Test roots, no direct source files in `Vision Test\\Wpf`, only the intentional three-file Shell composition boundary in MENU Wpf, and no restored Guided Setup/RecipeText helper declarations in the recipe command surface. `AGENTS.md` records the same boundary.
- The only direct source files intentionally retained in `0. UI\\0) MENU\\Wpf` are `OpenVisionShellHostView.xaml`, its code-behind, and `OpenVisionShellHostRecipeCommandSurface.cs`. They remain because moving either large dirty Host artifact without first extracting a complete controller/presenter/view-model responsibility would disguise, rather than reduce, coupling. The next refactor must extract one complete execution responsibility with dedicated smoke evidence.
- Fresh current-source view captures passed after the final source changes: `artifacts/p85_vision_test_wpf_organization_20260717/final_threshold_tool_current_source`, `final_learn_curriculum_current_source`, and `final_workspace_picker_current_source` all report `layout=0`, `text=0`, and `internal=0`.
- A fresh 0-warning/0-error solution build, screenshot-tool build, and readiness check passed. The latest `bin\\Debug\\OpenVisionLab.exe` then passed `recipe-pipeline-roundtrip` under `artifacts/p85_vision_test_wpf_organization_20260717/final_build_exe_recipe_pipeline_roundtrip` and `learn-line-distance-practice` under `final_build_exe_learn_line_distance`; both preserved explicit Preview/Run and no implicit layer changes.
- P85 is not committed or pushed. The Original repository remains untouched.

## 2026-07-17 P86 LLM Prompt And Intent Contract Ownership

- Extracted the independent LLM authoring prompt responsibility from `OpenVisionShellHostRecipeCommandSurface` into `Wpf\\Recipe\\IntentSkills\\OpenVisionRecipeLlmPromptBuilder.cs`. `OpenVisionRecipeLlmPromptRequest` carries the current recipe/pipeline, operator wording, reference image, and pin-gap gate context; the Host now only gathers that state and delegates construction.
- Moved template classification, intent contract wording, result-channel wording, and template guidance into `OpenVisionRecipeLlmIntent` in the same IntentSkills owner. The Host imports that explicit static owner for the remaining guided-setup and validation call sites; it no longer declares the prompt packet, tool-family contract, or template classifier methods. The command surface decreased from 9,827 to 9,553 lines in this bounded extraction.
- Added source-ownership readiness assertions that reject restored prompt-packet/intent-contract methods in the Host and require the new request/builder/explicit-Preview-Run contract in IntentSkills. `AGENTS.md` now names standalone LLM prompt and intent contracts as an IntentSkills responsibility.
- Updated `docs\\OPENVISIONLAB_LLM_TOOL_CATALOG.json` source-evidence paths to the current `1. Core\\Pipeline\\...` ownership layout and parsed the JSON successfully after the update.
- Extended the clipboard-independent `recipe-manager-llm-intent-skills` EXE smoke. It now validates template-matching prompt identity, intent/result-channel text, decimal-score and angle/path constraints, LineDistance XML-only packet text, and `PreviewRunCount=0`; this avoids treating a transient desktop clipboard lock as a product failure.
- Fresh 0-warning/0-error build, readiness, external-reference, and public-sample checks passed. The latest `bin\\Debug\\OpenVisionLab.exe` passed `recipe-manager-llm-intent-skills` under `artifacts/p86_llm_prompt_owner_refactor_20260717/final_build_exe_recipe_manager_llm_intent_skills_contract`; it verified the matching and pin-gap prompt packets without clipboard access, blocked the Contour-only pin-gap draft, and kept Preview/Run unchanged.
- P86 is not committed or pushed. The Original repository remains untouched.
- Next structure priority: extract the LLM XML draft validation/dependency-review execution path as one controller with an explicit request/result boundary. Do not move the whole Recipe Command Surface or Host XAML merely to reduce file length.

## 2026-07-17 P87-P89 LLM Draft Validation And Dependency Review Ownership

- Completed the next real responsibility split from the remaining dirty `OpenVisionShellHostRecipeCommandSurface`. `Recipe\Validation\OpenVisionRecipeLlmDraftValidationRules` now owns pure XML syntax, result-channel, and strict Intent contract rules. The Host calls those rules but no longer declares them.
- Added `Recipe\Review\OpenVisionRecipeDependencyReviewService`. It owns external image/template path classification, path resolution, review-bundle relocation/content-mismatch evidence, scan-versus-copy decisions, recipe dependency copying, and reference-image copying. It returns `OpenVisionRecipeDependencyReviewResult`; the Host only applies the returned report and rows to its display state. Review bundle exporter/inspector now depend on this service rather than Host static helpers.
- Added `Recipe\Validation\OpenVisionRecipeLlmDraftValidationService` with explicit `OpenVisionRecipeLlmDraftValidationRequest` and `OpenVisionRecipeLlmDraftValidationResult`. It owns XML syntax/deserialization, Pipeline schema/routing validation, result-channel and Intent contracts, reference-image evidence, and dependency review composition. The Host `TryBuildLlmDraftPipeline` now only builds the request, applies the result to its UI properties, and returns success.
- The command surface reduced from 9,553 lines after P86 to 8,889 lines. This was a responsibility extraction, not a move-for-size refactor: no Preview/Run, layer, routing, PropertyGrid, or UI workflow contract changed.
- `AGENTS.md` and `OpenVisionReadinessCheck` now enforce the owner boundaries. The readiness check rejects restored prompt, XML syntax, result-channel, Intent, XML-deserialization, dependency-classification, dependency-resolution, or dependency-copy implementation in the Host; it requires the exact IntentSkills, Validation, and Review owner files.
- Extended the clipboard-independent `recipe-manager-llm-intent-skills` EXE smoke with a real temporary PNG template. It validates an existing dependency path as `Found/확인`, while also checking the matching/LineDistance prompt packets, Contour-only pin-gap rejection, and `PreviewRunCount=0`. This avoids the known clipboard-lock limitation in the older all-in-one Recipe Manager smoke.
- Fresh 0-warning/0-error builds passed after P87, P88, and P89. The latest `bin\Debug\OpenVisionLab.exe` passed `recipe-manager-llm-intent-skills` under `artifacts/p89_llm_draft_validation_service_20260717/final_build_exe_recipe_manager_llm_intent_skills`; its report includes `DependencyReview: existing template path found without clipboard` and `PreviewRunCountUnchanged: 0`. Readiness, external-reference, public-sample (`30/229/15`), and `git diff --check` passed; diff check emitted CRLF normalization warnings only.
- P87-P89 are not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: inspect `LLM review bundle load/export` versus `Pipeline Review result/execution state` and extract only a complete request/result or presenter/controller responsibility. Do not move the remaining Host XAML/code-behind or command surface merely for tree appearance. Product priority remains the explicit rule-based learning/verification workflow and real LLM correction evidence.

## 2026-07-17 P90 LLM Correction Packet Ownership

- Extracted the pure correction-packet text construction from `OpenVisionShellHostRecipeCommandSurface` into `Wpf\Recipe\IntentSkills\OpenVisionRecipeLlmReviewBundleBuilder.cs`. `OpenVisionRecipeLlmReviewBundleRequest` carries recipe/pipeline identity, selected intent, operator context, failure review, validation/dependency/diff reports, and the current XML draft. The builder owns the correction rules, result-channel contract, and packet layout; the Host only checks whether copying is currently allowed and supplies state.
- Added the minimal `BuildLlmReviewBundleTextForTest` test hook to the command surface. It exercises the same Host-to-builder wiring without invoking the Windows clipboard, which can be externally locked in desktop smoke sessions.
- Extended the clipboard-independent `recipe-manager-llm-intent-skills` EXE smoke. After it validates a real temporary template path, it now confirms that the correction packet contains the shared Intent contract, `Inspection.Status`, `Direct_LLM_DependencyReview`, and the current dependency path. The latest report records `CorrectionBundle: current validation and dependency context assembled without clipboard` and `PreviewRunCountUnchanged: 0`.
- `OpenVisionReadinessCheck` rejects the correction-packet header in the Host and requires the explicit IntentSkills builder/request/shared Intent contract. `AGENTS.md` now names prompt, Intent, and correction-packet contracts as `Recipe\IntentSkills` ownership.
- A fresh 0-warning/0-error solution build passed. Latest `bin\Debug\OpenVisionLab.exe` passed `recipe-manager-llm-intent-skills` under `artifacts/p90_llm_review_bundle_builder_20260717/final_build_exe_recipe_manager_llm_intent_skills`. Readiness, external-reference, public-sample (`30/229/15`), and `git diff --check` passed; the diff check emitted CRLF normalization warnings only.
- P90 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not create a review-bundle session wrapper solely to move the existing inspection field. Audit Pipeline Review execution/result state first and split only a complete presenter/controller or request/result boundary. Product priority remains the explicit rule-based learning/verification workflow and real LLM correction evidence.

## 2026-07-17 P91 Pipeline Review Execution Ownership

- Audited the current Pipeline Review call path before editing. `OpenVisionPipelineReviewDocument` was simultaneously responsible for View event wiring/selected-Step presentation and the explicit runner call, display-layer execution context, Step-result cache, review output-image cache, and result-image disposal. This was a real runtime boundary, not a file-size split.
- Added `Wpf\PipelineReview\Execution\OpenVisionPipelineReviewExecutionController` and its explicit result/update contracts. The controller now owns `VisionPipelineExecutionService.RunAsync`, copies current display layers into a review-only execution context, caches Step summaries and output images, raises Step update events on the View dispatcher, and disposes runner result images after cache capture. The document now owns only explicit Run Review state text, View refresh/selection, fixture interaction, and displayed guide/result text.
- No Preview/Run path was broadened: the controller is called only from the existing explicit `Run Review` command. Opening Pipeline Review, selecting a Step, refreshing layers, or reading readiness does not execute the pipeline.
- `OpenVisionReadinessCheck` now rejects a restored runner/cache/context implementation in `OpenVisionPipelineReviewDocument` and requires the new controller/result contracts. `AGENTS.md` records `PipelineReview\Execution` as the owner for execution state and review-only image lifetime.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` passed `recipe-pipeline-roundtrip` under `artifacts\p91_pipeline_review_execution_controller_20260717\final_build_exe_recipe_pipeline_roundtrip`; the report retained `WAIT`, explicit Run Review, `NativePreviewRuns: 0`, and `LayerCount: 1`. Current-source `wpf_shell_host_pipeline_review` and `wpf_shell_host_pipeline_review_input_state` both passed with `layout=0`, `text=0`, and `internal=0`.
- Readiness, external-reference, public-sample (`30/229/15`), and `git diff --check` passed; diff check emitted existing CRLF normalization warnings only. P91 is not committed or pushed, and the Original repository remains untouched.
- Next source-organization priority: do not split selected-Step presentation or review-bundle session fields unless a new call-path audit identifies a full owner. Return to the higher product priority of real correction-loop evidence or current-build operator workflow friction.

## 2026-07-17 P92 Recipe Run Review Presentation Ownership

- Audited the remaining recipe run-review call path before editing. The Host still derived the operator summary, selected Good/Bad role suffix, saved batch-run review, and ordered next action from recipe/sample/pair/history DTOs. Those methods are reused by the decision board, handoff report, Guided Setup strip, result channels, role drill-down, and Run History, so they form a real presentation-policy boundary.
- Added Wpf\Recipe\Review\OpenVisionRecipeRunReviewPresenter. It accepts existing DTOs and an already-resolved failed Step; it owns formatted operator run review, selected-role suffix, saved-run review, and next-action ordering. The Host retains selected-state lookup, command enablement, clipboard copy, and PropertyChanged coordination.
- No execution behavior changed. The presenter cannot run Preview/Run, create layers, change input/output routing, modify XML, or access WPF controls. OpenVisionReadinessCheck now rejects restoration of the four Host text/policy methods and requires the dedicated presenter methods.
- Fresh solution build passed with 0 warnings/0 errors. Latest bin\Debug\OpenVisionLab.exe passed recipe-manager-tabs under artifacts\p92_recipe_run_review_presenter_20260717\final_build_exe_recipe_manager_tabs. Its report confirms RoleDrilldown, FailedRunLink, SelectedRunReview, SelectedRunReviewCopy, OperatorDecisionSummaryBand, and the existing explicit Preview/Run contracts.
- P92 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: audit the remaining Recipe Manager operator decision-board/report composition. Split only a complete presenter or request/result boundary; do not move or wrap Host fields solely to shorten OpenVisionShellHostRecipeCommandSurface.

## 2026-07-17 P93 Recipe Operator Decision Presentation Ownership

- Audited the remaining operator decision board and handoff report path before editing. The Host was deriving XML/sample/Good-Bad cards, final status, metric evidence, validation rows, result channels, and the handoff report from existing DTOs. Those outputs share one deterministic presentation contract, while Step resolution and clipboard commands remain Host responsibilities.
- Added `Wpf\Recipe\Review\OpenVisionRecipeOperatorDecisionPresenter` with explicit request/result contracts. The presenter owns all decision-board and handoff text derivation; the Host now supplies selected recipe/sample/history state and the already-resolved evidence/handoff Steps.
- No execution behavior changed. The presenter has no WPF, Preview/Run, layer, routing, XML mutation, or recipe mutation dependency. Opening Recipe Manager, selecting rows, reviewing result channels, and copying a handoff report remain read-only with respect to recipe execution.
- OpenVisionReadinessCheck now rejects restoration of the nine former Host composition methods and requires the dedicated `Recipe\Review` request/result/presenter owner. `AGENTS.md` and the ownership proof record the decision-board/handoff boundary.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` passed `recipe-manager-tabs` under `artifacts\p93_recipe_operator_decision_presenter_20260717\final_build_exe_recipe_manager_tabs`; the report records `RoleDrilldown`, `FailedRunLink`, `SelectedRunReviewCopy`, `OperatorDecisionSummaryBand`, and the existing explicit Preview/Run contracts.
- P93 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: audit remaining Recipe Manager lifecycle/session coordination only if a complete Controller/Presenter/service boundary exists; otherwise return to real correction-loop evidence or current-build workflow friction.

## 2026-07-17 P94 Recipe Pipeline Comparison Presentation Ownership

- Audited the LLM draft review and active/selected pipeline comparison path before editing. The Host was loading current pipelines and also formatting draft-import review, XML diff, variant comparison, step/parameter changes, and dependency-path deltas. The display portion is a complete pure presentation boundary.
- Added `Wpf\Recipe\Review\OpenVisionRecipePipelineComparisonPresenter`. It accepts supplied active/draft/selected pipelines and owns the read-only comparison strings; the Host retains current recipe/selection lookup and storage reads only.
- No execution behavior changed. The presenter cannot access WPF controls, recipe storage, Preview/Run, layers, routing, XML mutation, or recipe mutation. Selecting a variant remains read-only and does not activate or execute it.
- OpenVisionReadinessCheck now rejects restored Host pipeline-diff helpers and requires the dedicated comparison presenter. `AGENTS.md` and the source-ownership proof record the `Recipe\Review` ownership.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` passed `recipe-manager-tabs` under `artifacts\p94_recipe_pipeline_comparison_presenter_20260717\final_build_exe_recipe_manager_tabs`; it recorded `PipelineVariantComparison: active/selected diff visible without Preview/Run` and `LlmXmlDiff: visible`.
- P94 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: return to user-visible evidence or real correction-loop transcripts unless a future Host audit identifies another complete ownership boundary. Do not split validation-set lifecycle or selected-Step UI coordination merely to reduce line count.

## 2026-07-17 P95 Recipe Pipeline Step Review Presentation Ownership

- Audited the selected-Step review call paths before editing. The Host was composing failure links, corrected-output guidance/evidence, Step-flow context, branch/output comparison text and rows, and Step-slot labels from selected DTO state. This is one pure review-presentation boundary.
- Added `Wpf\Recipe\Review\OpenVisionRecipePipelineStepReviewPresenter`. It receives selected Step/result DTOs and derives all review text/rows; the Host keeps selection changes, PropertyGrid/XML apply, tool opening, and layer navigation.
- No execution behavior changed. The presenter has no WPF, Preview/Run, layer/routing mutation, storage, or recipe mutation dependency. Branch/output comparison remains read-only; corrected-output review still requires explicit PropertyGrid/XML apply and explicit review actions.
- OpenVisionReadinessCheck now rejects restored Host step-review composition methods and requires the `Recipe\Review` presenter. `AGENTS.md` and the source-ownership proof record the selected-Step/branch-output ownership.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` passed `recipe-manager-tabs` under `artifacts\p95_recipe_pipeline_step_review_presenter_20260717\final_build_exe_recipe_manager_tabs`; it recorded `FailedRunLink`, `CorrectedOutputReview`, `StepComparisonGrid`, `BranchOutputComparison`, and the existing explicit Preview/Run contracts.
- P95 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split remaining Recipe Manager lifecycle/validation-set persistence or Shell selection coordination without a new complete owner. Return to real correction-loop evidence or a current-build UX issue first.

## 2026-07-17 P96 Recipe Run History Presentation Ownership

- Audited Run History before editing. The Host was deriving the saved-run NG filter, NG-cause summary, baseline resolution, comparison rows/default-row selection, and timing-comparison summary while also owning persisted-run loading and selected UI state. The derivation portion is a complete read-only review boundary.
- Added `Wpf\Recipe\Review\OpenVisionRecipeRunHistoryPresenter`. The Host continues to load persisted current/baseline summaries and coordinate selection/notifications; the presenter receives supplied DTOs/summaries and derives the filter, baseline policy, row comparison, default review row, and correctness/performance summary.
- No execution behavior changed. The presenter has no WPF, storage-load, Preview/Run, layer/routing mutation, XML mutation, or recipe mutation dependency. Existing timing-comparison guards still require matching suite kind/name, exact sample-image multiset, and complete timing coverage; outcome comparison remains available when timing is skipped.
- OpenVisionReadinessCheck rejects restored Host Run History presentation helpers, requires the Presenter boundary, and rejects direct `VisionPipelineBatchRunSummaryStorage.Load` usage in the presenter.
- Fresh solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs` passed under `artifacts\p96_recipe_run_history_presenter_20260717\final_build_exe_recipe_manager_tabs`; its report confirms `BenchmarkBaselineSelection`, `RunHistoryAnalytics`, `RunHistoryPerformanceComparison`, `RunHistoryNgFilter`, and unchanged explicit Preview/Run behavior. Readiness, external-reference, public-sample (`30/229/15`), `P96_SOURCE_OWNERSHIP`, and `git diff --check` passed; diff check emitted existing CRLF normalization warnings only.
- P96 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split validation-set persistence, Recipe Manager lifecycle orchestration, or Shell selected-state coordination unless a new call-path audit finds a complete owner. Prefer real correction-loop evidence or a current-build operator UX issue next.

## 2026-07-17 P97 Recipe Good/Bad Sample-Matrix Presentation Ownership

- Audited the Good/Bad sample-matrix path before editing. The Host was obtaining same-pair sample rows, mapping the latest pair-run results, preserving/defaulting the selected matrix row, and formatting the matrix summary. These operations consume supplied catalog/run state only and form a complete read-only review boundary.
- Added `Wpf\Recipe\Review\OpenVisionRecipeSampleMatrixPresenter`. The Host retains selected sample/pair state and notifications; the presenter owns matrix rows, selected-row priority, and summary text. Validation Suite execution, persistence, PropertyGrid state, XML, layers, routes, and Preview/Run remain outside this presenter.
- Existing selection policy remains explicit: preserve the prior sample when present; otherwise choose an NG row, then a pending row, then the first available row.
- OpenVisionReadinessCheck rejects restored Host sample-matrix builders, requires the Presenter methods, and rejects pipeline execution from the presenter.
- Fresh solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs` passed under `artifacts\p97_recipe_sample_matrix_presenter_20260717\final_build_exe_recipe_manager_tabs`; its report confirms `PairRoleCards`, `RoleDrilldown`, `FailedRunLink`, `ValidationSuite`, and explicit Preview/Run-free Recipe Manager behavior. Readiness, external-reference, public-sample (`30/229/15`), `P97_SOURCE_OWNERSHIP`, and `git diff --check` passed; diff check emitted existing CRLF normalization warnings only.
- P97 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split validation-set persistence, Recipe Manager lifecycle orchestration, or Shell selected-state coordination without a separately proven request/result or presenter boundary. Prefer real correction-loop evidence or a current-build operator UX issue next.

## 2026-07-17 P98 Recipe Local Validation-Set Dashboard Presentation Ownership

- Audited the local validation-set path before editing. Validation-set persistence, file/folder registration, missing-path repair, explicit suite execution, and XML acceptance/calibration evidence are still Host/storage responsibilities. Four dashboard outputs only consume already selected DTO/state: expected-role counts, selected-set summary, next-action order, and Validation Suite top summary.
- Added `Wpf\Recipe\Review\OpenVisionRecipeValidationSetPresenter`. The Host retains storage/load/command/selection lifecycle; the presenter receives only selected state and formats the four dashboard outputs. It cannot persist validation sets, load pipeline XML, execute Preview/Run, change layers/routes, or mutate XML/recipes.
- The split deliberately leaves `BuildValidationSetAcceptanceText` and `BuildValidationSetCalibrationText` with the Host because they load the active pipeline and calculate gate/physical-unit evidence. Do not move them as a cosmetic file-length change.
- OpenVisionReadinessCheck rejects restoration of the four Host presentation methods, requires the Presenter methods, and rejects validation-set storage, pipeline XML loading, or execution behavior from the presenter.
- Fresh solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs` passed under `artifacts\p98_recipe_validation_set_presenter_20260717\final_build_exe_recipe_manager_tabs`; its report confirms `LocalValidationSet`, `ValidationSuite`, saved Step report, and explicit Preview/Run-free behavior. Readiness, external-reference, public-sample (`30/229/15`), `P98_SOURCE_OWNERSHIP`, and `git diff --check` passed; diff check emitted existing CRLF normalization warnings only.
- P98 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split validation-set persistence, Recipe Manager lifecycle orchestration, Shell selection coordination, or acceptance/calibration evidence without a new explicit request/result boundary. Prefer real correction-loop evidence or a current-build UX issue next.

## 2026-07-17 P99 Guided Intent Feedback Presentation Ownership

- Audited Pin-gap, Blob, and Contour Guided Setup feedback before editing. The seven latest-run/calibration/advice methods consume only the selected sample DTO, current gate fields, and Pin-gap unit mode; they do not execute, persist, navigate, change selection, or mutate XML/layers/routes.
- Added `Wpf\Recipe\Review\OpenVisionRecipeIntentFeedbackPresenter`. The Host retains all input state, Starter XML creation, selection, stale-draft coordination, and `PropertyChanged`; the presenter formats Pin-gap latest-run/calibration feedback, Blob `ResultCount`, and Contour `ResultCount`/`AreaMax` feedback.
- Existing localization and metric behavior is retained, including the Pin-gap average-only warning, PX-ONLY/MM-READY calibration wording, and explicit sample-run guidance. This is a behavior-neutral ownership refactor; it adds no automatic Preview/Run or recipe/layer/routing side effects.
- OpenVisionReadinessCheck now rejects restoration of the former Host methods and rejects sample execution, pipeline XML loading, and run-history persistence in the new Presenter. It also follows the moved average-only warning token from the Presenter instead of assuming the Host owns it.
- Fresh full solution build passed with 0 warnings/0 errors. The latest `bin\Debug\OpenVisionLab.dll` built at `2026-07-17 19:40:08 KST` passed `--smoke recipe-manager-tabs` under `artifacts\p99_recipe_intent_feedback_presenter_20260717\final_build_exe_recipe_manager_tabs_final`; the report records Pin-gap `DistanceMmAvg`/`DistanceMmRange`, Blob `ResultCount`, Contour `ResultCount`/`AreaMax`, Validation Suite, review/history, and explicit Preview/Run-free behavior.
- Readiness, external-reference, public-sample (`30/229/15`), `P99_SOURCE_OWNERSHIP`, and `git diff --check` passed. P99 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split Guided Setup commands, recipe lifecycle/selection coordination, validation-set persistence, or acceptance/calibration evidence without another complete request/result or pure presentation boundary. Prefer real correction-loop evidence or a current-build operator UX issue next.

## 2026-07-17 P100 Guided Setup Readiness Presentation Ownership

- Audited `BuildGuidedSetupReadinessText` and `TryBuildGuidedSetupIntentInputStatus` before editing. Both only classify the selected intent, parse the current Guided Setup field values, check optional reference-template existence, and format the required-input/readiness text; they do not create XML, execute a sample, persist history, navigate, or mutate recipe/layer/routing state.
- Added `Wpf\Recipe\IntentSkills\OpenVisionRecipeGuidedSetupReadinessPresenter` with the explicit `OpenVisionRecipeGuidedSetupReadinessInput` and status result. The Host now owns only current-field mapping, PropertyChanged coordination, and command gating; the Presenter owns LineDistance, Blob, Contour, EdgeBasedMatching, FeatureMatching, Matching, and Mean `READY`/`MISSING` evaluation plus guidance text.
- Existing input semantics and localized text are preserved, including Pin-gap PX-ONLY/MM-READY metrics, invalid min/max and Canny ordering, required template paths, optional Mean ROI, and Matching Search ROI/ResultCount readiness. The change introduces no automatic Preview/Run, recipe import, layer/routing update, XML mutation, or Starter XML creation.
- OpenVisionReadinessCheck now rejects restoration of both former Host methods, requires delegation and the read-only input contract, and rejects sample execution, pipeline XML load, run-history persistence, and starter-pipeline creation in the Presenter. `P100_SOURCE_OWNERSHIP` passed.
- Fresh solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` built at `2026-07-17 20:03:01 KST` passed `--smoke recipe-manager-tabs` under `artifacts\p100_guided_setup_readiness_presenter_20260717\final_build_openvisionlab_exe_recipe_manager_tabs`; the report records all seven Guided Setup Starter XML paths, Pin-gap MM/PX parity, current Recipe Manager review/history behavior, and explicit Preview/Run-free operation.
- Readiness, external-reference, public-sample (`30/229/15`), `P100_SOURCE_OWNERSHIP`, and `git diff --check` passed. P100 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split Guided Setup command/lifecycle orchestration, Recipe Manager selection/persistence, or acceptance/calibration evidence without a newly proven pure request/result or controller boundary. Return to real correction-loop evidence or current-build novice UX evidence before extracting another Host method.

## 2026-07-17 P101 Guided Workflow Presentation Ownership

- Audited the Guided Setup strip and its next-action flow before editing. The Host duplicated the same ordered conditions for the visible instruction and for choosing the command delegate. This created a real consistency risk while the policy itself used only supplied recipe/sample/pair state and existing command availability.
- Added `Wpf\Recipe\Review\OpenVisionRecipeGuidedWorkflowPresenter`, request contract, and action enum. The presenter owns setup-strip formatting, one ordered next-action decision, and its label. The Host still maps current state, owns `Can...` command predicates, and performs the explicit switch that invokes the existing Validate, Duplicate, Activate, Sample Check, parameter load, pair check, or Tool open command.
- No execution behavior changed: the new presenter cannot access WPF, storage, pipeline creation, XML mutation, recipe mutation, layers/routes, clipboard, or Preview/Run. Opening/selecting Recipe Manager remains read-only; command execution remains explicit.
- OpenVisionReadinessCheck rejects restoration of the three former Host helpers and rejects pipeline execution/storage/create/clipboard APIs in the presenter. `P101_SOURCE_OWNERSHIP` passed.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` built at `2026-07-17 20:14:35 KST` passed `recipe-manager-tabs` under `artifacts\p101_guided_workflow_presenter_20260717\final_build_openvisionlab_exe_recipe_manager_tabs`; it retained Guided Setup intent/workflow behavior, operator decision summary, review/history links, and explicit Preview/Run contracts.
- P101 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split Guided Setup command/lifecycle orchestration, Recipe Manager selection/persistence, acceptance/calibration evidence, or Host command execution without a separately proven controller boundary. First audit remaining lifecycle validation presentation; if it lacks a complete pure boundary, return to real correction-loop evidence or current-build novice UX evidence.

## 2026-07-17 P102 Recipe Lifecycle Validation Presentation Ownership

- Audited the two Recipe Manager name-guidance paths before editing. They only classify current recipe/pipeline selection, blank/invalid/normalized requested names, collision state, and the final localized guidance; lifecycle commands and storage calls are separate.
- Added `Wpf\Recipe\Review\OpenVisionRecipeLifecycleValidationPresenter` with recipe-edit and pipeline-edit request contracts. The Host maps current selection/list state and the pre-existing normalized pipeline name, while the Presenter formats all validation guidance. Workspace/pipeline creation, duplicate, rename, delete, selection refresh, and command execution stay in the Host.
- Existing branch order is preserved, including no selected recipe/pipeline, blank name, invalid name, invalid-character replacement notice, selected-only item guidance, duplicate names, and available-name guidance. `HasSelectedPipelineOption` retains the old option-null behavior without relying on a pipeline-name string.
- The Presenter has no WPF, workspace mutation, pipeline storage, XML/recipe mutation, layer/routing mutation, or Preview/Run execution. It may use the existing Core name-validity rule only. Opening or typing in Recipe Manager still does not run Preview/Run.
- OpenVisionReadinessCheck rejects restored Host methods and lifecycle/storage/execution APIs in the Presenter. `P102_SOURCE_OWNERSHIP` passed.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` built at `2026-07-17 20:24:07 KST` passed `recipe-manager-tabs` under `artifacts\p102_recipe_lifecycle_validation_presenter_20260717\final_build_openvisionlab_exe_recipe_manager_tabs`; it retained summary/advanced Recipe Manager modes, lifecycle commands, Guided Setup, review/history, and explicit Preview/Run contracts.
- P102 is not committed or pushed. The Original repository remains untouched.
- Next source-organization priority: do not split Recipe Manager lifecycle execution, workspace/pipeline persistence, Host selection coordination, acceptance/calibration evidence, or command predicates without a separately proven Controller/Service boundary. First perform one remaining Host call-path audit; if no cohesive non-mutating boundary remains, end mechanical refactoring and return to product evidence/UX work.

## 2026-07-17 P103 Stored Pipeline XML Validation Report Ownership

- Audited `BuildLlmXmlValidationReport` before editing. It uses already-loaded selected-pipeline XML state only, then formats the XML/load status, assumed input layer, pipeline/Step count, file-name mismatch warning, schema/routing evidence, and bounded error/warning rows. The Host owns the storage load but not the report construction.
- Added `Wpf\Recipe\Validation\OpenVisionRecipeStoredPipelineValidationReportBuilder` with a request contract. The Host now passes pipeline path, XML load status/message, and loaded pipeline; the Builder owns report composition and `VisionPipelineValidator` formatting.
- No execution behavior changed. The Builder has no pipeline storage, WPF, Preview/Run, layer/routing mutation, XML mutation, or recipe mutation dependency. Recipe Manager selection/refresh continues to be read-only.
- OpenVisionReadinessCheck rejects restoration of the Host report method and storage/execution/WPF APIs in the Builder. `P103_SOURCE_OWNERSHIP` passed.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` built at `2026-07-17 20:30:10 KST` passed `recipe-manager-tabs` under `artifacts\p103_stored_pipeline_validation_report_builder_20260717\final_build_openvisionlab_exe_recipe_manager_tabs`; it retained Recipe Manager summary/advanced modes, LLM XML evidence, Guided Setup, review/history, and explicit Preview/Run contracts.
- P103 is not committed or pushed. The Original repository remains untouched.
- Post-P103 audit completed: `1. Core`, `0. UI\0) MENU`, `0. UI\6) Vision Test`, and `0. UI\6) Vision Test\Wpf` have no direct C#/XAML files. `0. UI\0) MENU\Wpf` has only the approved Host composition files. The remaining Host methods are genuine selection/session coordination, storage/lifecycle, explicit execution, dialog/clipboard/viewer callbacks, or current-session LLM prompt/review-bundle assembly.
- Do not extract isolated review-reference, catalog-formatting, or ROI lookup helpers solely for line count. No further cohesive non-mutating Host boundary was found in this audit. End mechanical source refactoring here and return to product evidence/UX work unless a future call-path audit proves a Controller/Service boundary.

## 2026-07-17 P104 Recipe Summary Next-Action Visibility

- Current EXE Recipe Manager review found no visible clipping/overlap in Summary, Guided Setup, Pipeline Review, Report, or Run History. The only evidence-based novice friction was that the Summary action `파이프라인 열기` looked like a secondary outline control even though it is the required next step into the existing Pipeline workflow.
- Reused the existing `OpenPipelineReviewCommand`; changed only its localized label to `다음: 파이프라인 열기` / `Next: Open Pipeline` and rendered the existing Summary button as the teal primary action with a localized accessible name. No command routing, Preview/Run, layer, recipe, XML, or input/output behavior changed.
- Fresh before evidence: `artifacts\p103_stored_pipeline_validation_report_builder_20260717\final_build_openvisionlab_exe_recipe_manager_tabs\OpenVisionLab_RecipeManager_Summary.png`. Final after evidence: `artifacts\p104_recipe_summary_next_action_20260717\final_rebuilt_exe_recipe_manager_tabs\OpenVisionLab_RecipeManager_Summary.png`. The after capture shows a visible primary button with no text clipping.
- Fresh full solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` built at `2026-07-17 20:38:36 KST` passed `recipe-manager-tabs` under `artifacts\p104_recipe_summary_next_action_20260717\final_rebuilt_exe_recipe_manager_tabs`; it retained Summary/Advanced Review separation, validation/review/history behavior, and explicit Preview/Run contracts.
- OpenVisionReadinessCheck now requires the localized next-action label and the accessible-name binding. P104 is not committed or pushed. The Original repository remains untouched.
- Next priority: real GPT/Gemini/Claude correction-loop transcript validation when supplied. Without a real transcript, capture a current-build novice workflow only when a visible issue is observed; do not resume mechanical source splitting without a proved boundary.

## 2026-07-18 P129 Phase 2 Matching Operator Path

- P129 rechecked the third Phase 2 operator skill without changing product code. The public `Public_Matching_DiePad.pipeline.xml` runtime accepted `Matching_DiePad_Synthetic_OK.png` at `ResultCount=3` and `ScoreMax=93.074`, then returned the intended `Matching_DiePad_Synthetic_NoTarget_NG.png` result `ResultCount=0 < 3`. The explicit Tool flow stayed explicit: Preview changed from `0` to `1` only on the matching action and created `Matching_Preview` with three overlays.
- The current LLM import route was independently rechecked using P109's valid `..\\..\\docs\\...` Debug-relative draft. It validated, copied both template dependencies into the temporary saved recipe, imported, passed nominal, and produced the intended no-target NG. By contrast the public catalog XML's raw `docs\\...` path remains correctly blocked by LLM draft validation because that validator resolves relative paths under `bin\\Debug`. This confirms the existing known path portability boundary; no resolver, path rewrite, or authoring-guide behavior was changed.
- Phase 2 is complete with the LineDistance, Blob, and Matching public operator skills. The next phase is blocked until the user supplies one approved real Good/NG dataset, the target ROI/acceptance definition, and calibration evidence only if mm units are claimed. Full P129 verification and screenshot: `artifacts\p129_phase2_matching_operator_path_20260718\README.md`.

## 2026-07-18 P130 Local Bent-Pin Field-Pilot Candidate

- P130 prepared, but did not complete, a Phase 3 candidate from the user-authorized local `Sample` directory. The existing Bent Pin contour recipe replayed the local Good image as `Success=True`, `ResultCount=13`, and `BoundsWidthMax=14`; the bent NG replay returned the controlled expected failure `BoundsWidthMax=26 > 18` with `ResultCount=13`. The candidate is pixel-only: its gate is `BoundsWidthMax <= 18`, and no mm or calibration claim is made.
- The local image pair, result overlays, and README stay under ignored `artifacts\\p130_local_bent_pin_pilot_candidate_20260718`; they must not be copied into public samples, tracked documentation evidence, or an LLM/provider prompt. The operator must still explicitly approve the inspection intent, ROI `20,65,728,175`, gate, and Good/NG labels before this becomes the one field-pilot recipe.
- A separate host-specific limitation was found while testing the direct LLM image replay: `llm-xml-image-run` crashes for Contour at `OpenCvSharp.NativeMethods.imgproc_findContours1_vector`. Console `VisionRecipeRunnerSmoke` plus current-source WPF direct-tool/Pipeline Review controls pass. Record: `.proofline/issues/PL-0001-direct-smoke-contour-access-violation.md`. Do not alter native DLLs or infer product-runtime failure without a dedicated repro and regression test.

## 2026-07-18 P131 Approved Local Bent-Pin Field-Pilot Result

- The user explicitly approved the Bent Pin shaft-width intent, ROI `20,65,728,175`, `BoundsWidthMax <= 18 px` pixel gate, and the local Good/bent-NG labels. The current Debug `RECIPE\\FieldPilot_BentPin` workspace contains the active `BentPin_ShaftContour` pipeline, `Approved Bent Pin Good-NG` local validation set, four local result/overlay PNGs, `EVIDENCE\\VALIDATION_RESULTS.md`, and `FIELD_PILOT_HANDOFF.md`.
- Fresh replay used that saved pipeline path: Good passed at `ResultCount=13`, `BoundsWidthMax=14`; bent NG returned the controlled expected rejection at `ResultCount=13`, `BoundsWidthMax=26 > 18`. The recipe is pixel-only and carries no calibration/mm claim. Its source images and overlays remain local, ignored runtime material; they must not move to public/tracked samples or a provider prompt.
- Phase 3 is complete only for this approved two-image local workbench result. It does not prove production robustness, deployment readiness, hardware integration, or general LLM authoring reliability. Keep PL-0001 open for the separate Contour direct-smoke host fault.

## Cautions

- UI/UX changes require fresh current-build before/after screenshots. Do not reuse old screenshots.
- `PipelineViewerScreenshotSmoke` can hang when multiple WPF targets are run in one process. Use `tools\RunSampleReviewUiSmokes.ps1` or single-target runs.
- Do not run WPF smoke targets in parallel; `OpenCvSharpExtern.dll` lock warnings can appear.
- Do not bulk-copy Dev into Original.
- Do not restore GitHub Desktop stashes unless the user explicitly asks.
- Do not reintroduce SDK sample assets or `dll\Library-Noah\OpenCvSharpExtern.dll` into public paths.

## 2026-07-19 P132 Clean Runtime Direct-Contour Replay

- Repeated P130's access violation with the retained `bin\Debug\OpenVisionLab.exe` and with `dotnet bin\Debug\OpenVisionLab.dll`: both terminated before a report in `OpenCvSharp.NativeMethods.imgproc_findContours1_vector` (`-1073741819`). Worker-hosting the run, creating a WPF Application, and temporarily hiding `opencv_world430.dll` did not change the failure; those experiments were reverted and no native DLL path or product runtime code was changed.
- The current source/DLL comparison showed matching `OpenCvSharp.dll` and `OpenCvSharpExtern.dll` hashes between failing and passing hosts. The durable difference is the retained default output folder, which contains many unrelated legacy runtime files. The same current `OpenVisionLab.exe` from a clean tool output and from a new empty `OutputPath` passed the direct Contour replay.
- Added `tools\BuildCleanRuntime.ps1`. It accepts only a new `artifacts` output directory, refuses an existing or external destination, runs the app build there, verifies essential app/OpenCV/PropertyGrid files, and emits `clean_runtime_manifest.json` with SHA-256 values. It never deletes, moves, or overwrites `bin\Debug`.
- Final P132 runtime evidence used `artifacts\p132_direct_smoke_contour_host_20260719\clean_runtime_script_final\OpenVisionLab.exe`. Good passed with `ActualRunSuccess=True`, `ResultCount=13`, `BoundsWidthMax=14`. The bent Bad passed as an expected-NG smoke with `ActualRunSuccess=False`, `BoundsWidthMax=26 > 18`. Current-source WPF Contour Tool and Pipeline Review controls also passed. Standard solution build, readiness, external-reference, and public-sample checks passed.
- `PL-0001` is resolved for clean current LLM XML replay. Do not use the retained `bin\Debug` for current EXE claims. At the P132 point, the permanent Dev/release output root and legacy workspace retention/migration decision was recorded as blocked `PL-0002`; P133 records the later user-approved resolution. Do not delete or relocate the retained workspace without explicit user direction.

## 2026-07-19 P133 Approved Dev/Release Runtime Output Contract

- The user approved a permanent Dev/release output contract while retaining `bin\Debug` as an unchanged local recipe workspace: Dev verification output is a new `artifacts\openvisionlab_clean_runtime_<timestamp>` directory; release package output is a new `dist\OpenVisionLab` directory.
- `tools\BuildCleanRuntime.ps1` now exposes `-Mode Dev|Release`. Dev builds a timestamped Debug runtime under `artifacts`; Release publishes a Release runtime only to `dist\OpenVisionLab`. Both modes reject an existing target; Dev also rejects a release-root target. No automatic cleanup, migration, deletion, or `bin\Debug` rewrite was added.
- P133 Dev runtime: `artifacts\openvisionlab_clean_runtime_20260719_004600\clean_runtime_manifest.json`. P133 Release package: `dist\OpenVisionLab\clean_runtime_manifest.json`. Required application, OpenCV native, and PropertyGrid runtime files were present with manifest hashes.
- Both current runtime outputs passed direct `llm-xml-image-run` against the approved saved Bent Pin recipe: Good `ActualRunSuccess=True`, `ResultCount=13`, `BoundsWidthMax=14`; bent Bad expected-NG `ActualRunSuccess=False`, `BoundsWidthMax=26 > 18`. Release `recipe-manager-tabs` also passed under `artifacts\p133_clean_runtime_output_contract_20260719\release_recipe_manager_tabs`.
- The retained saved recipe SHA-256 remained `B817FC09093AF30A77AB7AA5A96436FA8884D79ACD3FDE7DD6089D3E11E46D82`. Full Debug solution build, readiness, external-reference, and public-sample checks passed. `PL-0002` is resolved for the output contract only; LLM template-dependency portability from the packaged runtime remains the next distinct problem.

## 2026-07-19 P134 Release Template-Dependency Import Contract

- The current Release EXE blocked `Public_Matching_DiePad.pipeline.xml` when its `TemplatePath` and `PATTERN_PATH` used the catalog-only `docs\samples\...` path. The validation report named both dependencies as missing and kept Import disabled; this is the required safe behavior for a package that does not ship the repository catalog.
- A P134 public-only Matching draft supplied the same existing template by an operator-accessible path. Release validation/import copied the two dependency parameters into `dist\OpenVisionLab\RECIPE\Smoke_LlmDraft_<suffix>\Template`, updated the imported XML, then passed Good at `ResultCount=3`, `ScoreMax=93.074` and the expected no-target NG at `ResultCount=0 < 3`.
- No validator/import code change was justified. `OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md` now states the actual package contract: use an existing operator-supplied path, never a `docs\samples\...` catalog reference; Import validates and copies it into the recipe. This verifies relocation within the current installation only, not moving an already-installed package to a different root.
- Evidence: `artifacts\p134_release_template_portability_20260719\before_catalog_relative_path_blocked\report.txt`, `release_operator_path_good\report.txt`, and `release_operator_path_expected_ng\report.txt`.

## 2026-07-19 P135/P136/P137 Approved Local, GPT, And Cross-Install Evidence

- P135 used the user-approved local root `Sample` assets only. The EdgeBasedMatching L-fiducial Good image passed in a fresh clean Dev runtime with `ResultCount=1`, `ScoreMax=99.991`; the explicitly named `Edge_Source_NoTarget.png` produced the expected NG with best score `57.502 < 0.70`. The public/provider boundary remains strict: only `artifacts\p135_local_edge_fiducial_20260719_100236` contains this local test's XML, reports, and overlays.
- P136 is a real user-authorized GPT same-conversation correction loop. Only the public Die Pad image and public template were uploaded. GPT's initial XML had valid shape but used the attachment file name for both template keys. Current clean-Dev `llm-xml-draft-file` returned `ValidationOk=False`, `ImportEnabled=False`, and two missing dependency rows. The same-chat correction replaced exactly those two values with the verified public accessible path. Current clean-Dev validation/import copied both files to the temporary recipe with root-relative paths, then explicit execution passed the public nominal image (`ResultCount=3`, `ScoreMax=93.074`). The raw prompt/response XML and reports are `artifacts\p136_gpt_matching_correction_20260719_103200`; they contain a local path and require sanitization plus explicit inclusion approval before any documentation publication. The live user chat was preserved for handoff; no API was used.
- P137 changed only the portable template path boundary. Imported template dependencies are now stored relative to `AppPathService.StartupPath`, and the app tool factory resolves template paths against that root for Matching, EdgeBasedMatching, and FeatureMatching. A fresh Release package was published to `dist\OpenVisionLab`, copied intact to `artifacts\p137_cross_install_20260719_100614\relocated_install`, and executed there with only package-contained `RECIPE` templates and samples. The copied EXE hash matched the Release original. Matching passed `ResultCount=3`, EdgeBasedMatching passed `ScoreMax=99.598`, and FeatureMatching passed `ScoreMax=96.7`. P137 does not prove installer, updater, signing, or production deployment behavior.
- Final verification after the handoff-document updates passed: Debug solution build (0 warnings/errors), `OpenVisionReadinessCheck`, `TestExternalReferences.ps1`, `TestPublicSampleAssets.ps1` (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`), and `git diff --check` (CRLF notices only).

## 2026-07-19 P138 Current-EXE LLM Template-Path Guidance Audit

- The user deliberately deferred an under-labelled local field-pilot expansion and asked for another priority. P138 chose the evidence-based fallback: audit the LLM Assistant's visible response to the actual P136 attachment-filename dependency failure, not a speculative feature.
- A newly built clean Dev EXE reproduced the two missing dependency keys and disabled Import. The report included the clear instruction to replace paths with verified files before import. The dedicated current-EXE `recipe-manager-llm-intent-skills` smoke passed and its failure capture showed the comparable reason/next-action presentation in the first viewport with no visible clipping or overlap.
- The missing-dependency runner's final screenshot was shell-only after its report capture, so it is intentionally not used as LLM-panel visual evidence. No UI code, text, behavior, Preview/Run count, import behavior, or routing changed. Evidence: `artifacts\p138_llm_template_path_guidance_audit_20260719_103100`.

## 2026-07-19 P139 Public Product-Catalog Template-Path Regression Repair

- The user redirected the blocked local field-pilot work to the existing industrial samples. Policy review kept the ignored root `Sample` SDK/vendor assets local-only; the reusable evidence source is the public-safe product catalog. It currently has 184 rows: 84 Required Good, 84 ExpectedFailure Bad, and 16 Explore field-style rows. The product README's stale 168-row claim was corrected.
- The first current-source run exposed a real P137 regression: 20 matching-family rows failed because the tool factory resolved `docs\samples\...` only under the EXE startup directory. The P137 requirement remains correct for copied package `RECIPE` paths, but it accidentally removed the recipe runner's established working-directory resolution for development catalog paths.
- The minimal repair in `1. Core\Pipeline\Definition\VisionPipelineAppToolFactory.cs` retains absolute paths, prefers an existing startup-relative file for portable packages, then falls back to the current working directory for the existing development catalog contract. Full Debug build passed with 0 warnings/errors; the fixed catalog passed 184/184; `AuditProductSampleQuality.ps1 -FailOnCritical` reported 84 pairs `OK=84`, `Review=0`, `Critical=0`; public-assets and vendored-DLL checks passed.
- A new clean Dev runtime (`artifacts\openvisionlab_clean_runtime_20260719_104632`) was copied to `artifacts\p139_relocated_clean_runtime_20260719_104632\relocated_install` and run from that copied root. Package-contained relative templates still passed for Matching (`ResultCount=3`), EdgeBasedMatching (`ScoreMax=99.598`), and FeatureMatching (`ScoreMax=96.7`). Its EXE SHA-256 was identical before/after copy. This confirms the P137 portable-path behavior after the P139 compatibility repair, but does not create a new installer/release claim.

## 2026-07-19 P140 Gemini Flash Availability Check

- After the user explicitly approved the exact message, a signed-in Gemini Flash new chat received only `응답성 확인입니다. READY 한 단어로만 답하세요.` No image, XML, local path, API key, or project material was transmitted.
- The sent text was visible and Gemini stayed in the response-generating state through an additional 20-second observation without any response text. This is a stalled availability observation, not a GPT/Gemini correction-loop transcript or provider-quality evidence.
- Follow the user's operating rule: pause Gemini work for several hours, send no further Gemini prompt or sample, and use existing GPT evidence only until the user explicitly resumes a recovered provider. The live Gemini tab was retained for handoff.

## 2026-07-19 P141 Tool View Code-Behind Stop-Condition Audit

- P141 audited the concrete clean Tool Views while Gemini was paused and no labelled multi-variation field set was available. It covered Threshold, Filter, Morphology, Arithmetic, SimplePreprocess, Blob, Contour, Line, Matching, EdgeBasedMatching, and FeatureMatching.
- The existing ownership boundaries already hold: custom-tool parameter/event/preview/summary behavior belongs to named controllers and presenters; Blob/Contour/Matching-family use the shared single-input PropertyGrid base/controller; Line delegates interaction, preview, review, localization, and preset behavior to its named owners. Remaining View code-behind is WPF construction/lifetime wiring, intentional public test hooks, or narrow forwarding.
- No source edit was made. `git diff --check` reported only existing CRLF conversion notices, and the inspected Tool View files were clean. Do not reopen this cleanup merely to reduce line count; require a visible defect, real duplicated owner path, or a complete established controller/base responsibility.

## 2026-07-19 P142 Local Field-Pilot Candidate Inventory

- The user-approved ignored local `Sample` root was inspected read-only. It contains 341 images across measurement, matching, object, OCR, code-reading, and related example groups. No local asset, path, XML, or screenshot was sent to an LLM provider or copied into a public path.
- A single explicit Good/NG pin pair was visually confirmed as straight-pin reference versus bent-pin negative, but it has only one image per outcome. Larger repeated candidate groups exist but have no operator-supplied Good/NG meaning, ROI, or gate. Treat them as unlabelled candidates; do not infer labels from filename sequence or image appearance.
- No recipe, public catalog, source code, or product claim changed. The evidence was the current read-only file inventory, label-name search, image dimension/SHA comparison, and local visual review. The exact local paths remain out of this handoff because the source is local SDK/vendor material.

## 2026-07-19 P143 Image-List Validation And Die Pad 500 Baseline

- Added the explicit Recipe Manager `Image list validation` entry and local
  Validation Set workflow with separate OK/NG folders, virtualized rows,
  sequential execution, finish-current cancellation, persisted partial results,
  and separate Pipeline acceptance versus expected/actual judgement.
- Current-source UI and fresh clean-runtime Recipe Manager smokes passed without
  automatic image load, Preview/Run, layer creation, or route changes.
- The operator-supplied Die Pad corpus supplied 250 OK and 250 NG rows. The
  frozen Matching-only baseline completed all 500 but achieved only 50.0%
  accuracy and 2.8% NG rejection, proving the batch workflow while rejecting
  Matching-only as the varied-defect recipe.
- Status: Complete for the product workflow and baseline audit, not production
  qualification. Evidence:
  `artifacts\p143_batch_image_list_ui_20260719_170756`.

## 2026-07-19 P144 Die Pad Reference-Difference Recipe And Frozen Test

- Added the bounded `ReferenceDifference` Pipeline family after Train-only
  existing-tool probes could not separate the supplied defect variation. It
  registers against up to four explicit approved Good references, normalizes
  grayscale intensity, detects localized difference regions, retains drawings,
  and lets an explicit `ResultCount=0` gate own judgement.
- Frozen Train produced 98.89% accuracy, Validation 98.75%, then the one-time
  untouched 30 OK + 30 NG Test produced 98.33% accuracy with one false accept.
  The post-Test dependency-parameter portability change reproduced Validation
  without rerunning Test.
- PropertyGrid, XML validation/import, four dependency copies, clean-runtime
  Good/NG execution, full build, policy, catalog, and diff checks passed.
- Status: Complete for the supplied synthetic 500-image corpus, not real-field
  robustness. Evidence: `artifacts\p144_die_pad_multitool_20260719`.

## 2026-07-19 P145 Golden-Reference Defect Guided Setup

- Added Recipe Manager `Golden-reference defect (ReferenceDifference)` Guided
  Setup with one to four operator-approved references, difference threshold,
  defect-area bounds, deterministic P144 defaults, and exact zero-result
  acceptance.
- Reference selection, draft creation, and validation remain explicit and do
  not learn/replace references, import, Preview/Run, create/select layers, or
  change routing. A weakened result gate was rejected and the restored starter
  returned to import-ready state.
- Current-source UI, clean-runtime focused EXE, full solution build, policy, and
  diff checks passed. A separate broad historical Matching dependency failure
  is not counted against this feature.
- Status: Complete for bounded Guided Setup/validation integration. Evidence:
  `artifacts\p145_reference_difference_guided_setup_20260719`.

## 2026-07-20 P146/P147 Batch Drawing Evidence And Pin_1 GPT XML Audit

- P146 closes the previously missing batch-review evidence path: a selected Run History sample now has an explicit `도면 보기` action that opens its original and stored detection drawing side by side. It reads saved evidence only; the focused current-source smoke verified that Preview/Run count, layer count, and active input/output routes remain unchanged. The batch report storage writes only one review overlay per sample (failed Step first; otherwise the last relevant overlay Step) to avoid storing every intermediate image.
- P147 ran the saved third GPT correction XML against the user-supplied local Pin_1 folders (250 expected OK plus 250 expected NG) without any recipe tuning. Result: correct accept 49, false reject 201, false accept 37, correct reject 213; accuracy 52.40%, OK recall 19.60%, NG recall 85.20%, average 2.268 ms. This is a negative quality finding for the recipe, not a claim about the LLM provider or production use.
- The runner copied evidence for every false row before cleaning its reserved Smoke workspace. `artifacts\p147_pin1_gpt_full_batch_drawing_audit_20260720\misclassification_evidence\manifest.csv` indexes 238 folders, each containing original, drawing, and Run Report. A post-run integrity pass verified 238 manifest rows, 238 folders, and zero missing/unreadable image/report files. Current-source comparison capture: `artifacts\p147_pin1_gpt_full_batch_drawing_audit_20260720\wpf_shell_host_recipe_local_validation_drawing_evidence.png`.
- Do not retune on this undifferentiated evaluation corpus. Obtain an operator-approved inspection intent, ROI, acceptance rules, and Train/Validation/Test split before any recipe revision; preserve P147 as the immutable baseline.

## 2026-07-20 P148 Dynamic PinArrayGap And Frozen Pin_1 Pitch Evidence

- P148 preserved the user-supplied local Pin_1 Train/Validation/Test split and used no provider/API/public-sample transfer. The prior fixed-ROI `LineDistance` all-pair experiment completed 42/356 Train images; its translation-only fixture variant completed 31/356. Those results establish that fixed pair coordinates cannot be calibrated safely on this augmented corpus.
- Added `PinArrayGap`/`AdjacentPinGap`: one dark-pin row ROI -> dynamic pin runs -> every adjacent edge-to-edge clearance -> count/min/max/average/range and drawing overlays. XML validation, known metrics, diagnostic text, tool catalog, and authoring guide were updated together. Batch runner CSV now persists ResultCount and distance min/max/average/range.
- Train-derived and frozen gate is `DistancePxRange <= 6` for top/bottom rows. Train Expected Good=178/178 pass and pitch_error=38/38 reject; Validation Expected Good=36/36 pass and has no pitch label; Test Expected Good=36/36 pass and pitch_error=12/12 reject. Do not describe this as a whole Pin_1 classifier: short pin and bridge contamination generally pass, while bent/missing may trigger the range gate. It proves only the named full-row clearance/pitch signal.
- Full reproducible report and artifacts: `artifacts\p148_pin1_all_pitch_measurement_20260720\README.md`.
- Final verification: Debug solution build 0 warnings/errors; `VisionRecipeRunnerSmoke` rebuild; readiness; external-reference/public-sample policy; catalog JSON/PinArrayGap contract; and `git diff --check` all passed (only CRLF notices). Current-build Good smoke passed; current-build pitch-error smoke returned expected `DistancePxRange=35 > 6` with the PinArrayGap-specific diagnostic.

## 2026-07-20 P149 Card Intersection And Curved-Band Measurement Evidence

- P149 used only the user-approved local `card_original` and `device_left` datasets. No source image, label, XML, or result was transmitted to an LLM/provider or copied into public sample content.
- `LineIntersection` completed all 500 `card_original` split rows and retained fitted-line/crosspoint overlays. This validates geometry execution only: the class-0 defect boxes neither identify the intended two lines nor contain a ground-truth crosspoint. Fitted infinite lines may meet outside the visible frame, so no coordinate acceptance gate was fitted.
- Added XML runtime tool `CurveBandProfile` (alias `DarkBandCurve`). It selects the leftmost eligible dark component inside an explicit ROI, draws inner/outer profiles and sampled width lines, and reports profile/outer/inner/center arc-length metrics. The factory, validator, diagnostics, known metrics, batch CSV, LLM catalog, and authoring guide were updated together.
- The finalized curve recipe completed all 500 `device_left` split rows; center-arc averages were Train 191.659 px, Validation 191.061 px, and Test 191.016 px. Reviewed current overlays show the profile still follows the intended curve after lateral motion. No quality gate or physical-unit claim is valid: the supplied OK/NG metric ranges overlap, and reviewed NG boxes are independent central defects rather than an annotated curve tolerance.
- Evidence: `artifacts\p149_card_intersection_device_curve_measurement_20260720\README.md`, its two pipeline XML files, six split CSVs, and four current-build overlay images. Final checks passed: Debug solution build, runner build, readiness, external-reference/public-sample policy, catalog JSON parse, and `git diff --check` (line-ending notices only).

## 2026-07-20 P150 Dynamic Card Bottom-Right Outer-Corner Evidence

- The user reviewed P149's card overlay and correctly rejected it: broad `LineIntersection` ROIs had selected text/diagonal candidates rather than the card's lower/right outer edges. Preserve P149 only as rejected diagnostic evidence.
- Added validated/importable XML tool `OuterCornerIntersection` (alias `BrightObjectCorner`). It finds the virtual sharp lower-right corner from the two intended outer edges, using a bright component when reliable and bottom/right directional edge fallback under illumination/frame variation. It draws both red edge lines to the common green corner and returns `IntersectionX`/`IntersectionY`.
- Final local-only split evidence: Train 350/350, Validation 75/75, Test 75/75; all 500 result rows succeeded. Coordinate range X=334.883..632.715, Y=317.528..473.586; mean elapsed time=4.608 ms. No OK/NG defect claim, physical-unit claim, or coordinate tolerance was added.
- Full recipes, final CSVs, current overlays, and the rejected fixed-ROI baseline are in `artifacts\p150_card_bottom_right_intersection_20260720\README.md`.

## 2026-07-20 P151 GPT PinArrayGap Direct-Success Evidence

- A new user-authorized conversation in the `룰베이스 LLM 연동` GPT project received only public bundled `Sample\EasyGauge\Pin 1.jpg`. The prompt, first XML document content, conversation URL, command result, and current drawing are in `artifacts\p151_gpt_pinarraygap_direct_success_20260720\README.md`.
- The unchanged first response contained one `PinArrayGap` Step with `Main -> Top_Pin_Clearance` and upper-row ROI `0,90,768,170`. Current Debug LLM XML validation/import passed (`ValidationOk=True`, `ImportEnabled=True`, `Imported=True`, zero errors/warnings); explicit image run passed at 49.653 ms with 15 visible pin edges, 14 adjacent edge-to-edge gaps, `DistancePxMin=43`, `DistancePxMax=44`, and `DistancePxRange=1`.
- No initial validation or runtime failure occurred. P151 is a real GPT direct-success transcript, not correction-loop evidence. Do not send a fabricated correction request; wait for a future genuine public-sample failure before requesting same-conversation repair.
- Final project verification passed: Debug solution build (0 warnings/errors), readiness, public-sample policy, XML parse, and `git diff --check` (line-ending notices only).

## 2026-07-20 P152 Card-Corner Acceptance Evidence Preparation

- P152 adds read-only `LineAngleMin`, `LineAngleMax`, and `LineAngleAvg` columns to `VisionRecipeRunnerSmoke` batch CSV output because `OuterCornerIntersection` already produced those metrics but P150's original CSV retained only X/Y. No detection selection, XML parameter, or acceptance gate changed.
- The P150 XML reran successfully over Train 350/350, Validation 75/75, and Test 75/75 with zero missing images. `artifacts\p150_card_bottom_right_intersection_20260720\P150_OPERATOR_ACCEPTANCE_SPEC.md` contains observed coordinate/angle distributions and the exact operator fields needed to define a card position/rotation/out-of-frame rule.
- The existing defect boxes/masks are not corner or angle ground truth, and observed OK/NG coordinate/angle values overlap. P152 is evidence preparation, not a card judgement or calibration claim.
- Final verification passed: Debug solution build (0 warnings/errors), runner build, three angle-CSV completeness checks, readiness, external-reference policy, and public-sample policy (`CatalogRows=30`, `ManifestAssets=229`, `Pipelines=15`).

## 2026-07-20 P153 Algorithm Drawing-Evidence Contract And Card-Corner Visual Replay

- Added a durable `AGENTS.md` rule: image-algorithm validation must preserve and render current runtime detection drawings, not CSV/PASS counts alone. Evidence must bind the executed source, XML, selected ROI/geometry, final point/line/contour/measurement, metrics, and visual inspection; batch work must include representative normal and difficult/boundary/failure cases when available.
- Current rebuilt `VisionRecipeRunnerSmoke` reran P150's exact XML on normal OK_0058, the operator-reviewed NG_0066, and lower-position NG_0246. `artifacts\p153_card_corner_visual_evidence_20260720\README.md` and its source/result/overlay files preserve the executable proof. Red is the selected bottom/right outer edges; the green cross is the resulting virtual corner. No manual marks were used.
- Every replay completed with `ResultCount=1`, `EdgeCount=2`, and `IntersectionCross=1`. This is visual geometry-execution proof only; position/angle judgement still requires the P150 operator acceptance specification.

> Superseded by P154: the P153 NG_0066 profile drawing was rejected by later operator review. Preserve it only as an incorrect-detection baseline, not as semantic card-corner correctness evidence.

## 2026-07-20 P154 Card-Corner False-Positive Repair And LLM Visual-Review Contract

- P154 fixes the specific failure mode exposed by the user: the prior bright-component profile fit could return an in-frame mathematical intersection without proving both lines were the selected card's adjacent lower/right outer sides. The tool now first takes the lower/right adjacent sides of the selected bright contour's rotated outer rectangle. On NG_0066, the current result moved from `(551.136, 354.271)` to `(534.113, 357.586)`; current result drawing: `artifacts\p154_card_corner_false_positive_repair_20260720\result_ng_0066_outer_contour_verified_current.png`.
- Added `CornerOuterContourVerified` runtime metric and batch CSV column. It is `1` for the outer-contour path and `0` for profile/edge fallback. LLM tool catalog/authoring guidance now prohibits deriving coordinate/angle gates from a `0` run without retained overlay review/correction. The Recipe Manager LLM XML validation report now emits a concise `Outer-corner: WAIT` visual-review instruction for every OuterCorner draft. Current-source smoke `wpf_shell_host_outer_corner_llm_review` passed at 1600x900 with the complete line visible and no Preview/Run or layer side effect.
- Measurement-only current rerun remained Train `350/350`, Validation `75/75`, Test `75/75`; however only `5/500` rows report `CornerOuterContourVerified=1`, and strict metric gate `>=1` passes only `5/500`. Do not use that strict gate by default or claim full semantic correctness. The 495 fallback rows require visual/ground-truth review.
- P154 closure remains **Incomplete**. The external prerequisite is operator ground-truth marks for NG_0066 and representative fallback samples (two intended edges plus accepted virtual corner), then a completed P150 acceptance specification. Existing defect boxes/masks do not identify this geometry.

## 2026-07-20 P155 Card-Corner Operator-Mark Comparison And Honest LLM Review

- Matched the user's blue-mark screen to `card_original_NG_0066.jpg` by normalized grayscale correlation across all 500 local `card_original` images: `0.941901` top match versus `0.854018` next candidate. Its displayed 640 x 480 source rectangle maps the freehand blue centre to approximately `(530,391)` (+/-18 px).
- Replaced the preferred min-area-rectangle line source with lower/right support-point fits from the selected bright contour. Current NG_0066 runtime evidence used 129 support points and returned `(531.274,352.716)`. This is 38.305 px from the recovered user mark, so it is recorded as a failed semantic target rather than a repair claim. `CornerOuterContourVerified=1` now only asserts outer-contour support, never semantic target agreement.
- Current artifacts: `artifacts\p155_card_corner_contour_tangent_repair_20260720\result_ng_0066_before.png`, `result_ng_0066_after.png`, `comparison_ng_0066_operator_mark_vs_after.png`, `P155_OPERATOR_MARK_COMPARISON.md`. The comparison image labels blue as a separately added operator mark; the red/green drawing remains exact runtime output.
- Current measurement-only replay completed Train `350/350`, Validation `75/75`, Test `75/75` with `CornerOuterContourVerified=1` on only `4/500` rows. This is structural source evidence only, not a 500-image correctness result.
- LLM validator wording now requires comparison of red edges/green corner with an operator mark before a coordinate/angle gate. Fresh current-source before/after WPF smoke captures are under `llm_review_before` and `llm_review_after_final`; after smoke passed 1600x900 with layout/text/internal checks all zero and no Preview/Run or layer side effect.
- P155 remains **Incomplete**: one mark cannot establish whether the required virtual point is a tangent intersection, rounded-corner offset, or separate fiducial. Obtain two translated target/edge marks before a further geometry change or acceptance gate.

## 2026-07-20 P156 Card-Corner Operator-Mark Packet

- Created `artifacts\p156_card_corner_operator_mark_packet_20260720` to collect the two remaining translated geometry marks without inventing targets. It contains source-only 50 px grids, exact runtime result/all-overlay drawings, measurement-only XML, and raw run logs for `card_original_NG_0207.jpg` and `card_original_NG_0246.jpg`.
- `NG_0207` is the left-shifted meaningful fallback case: current `(369.509,389.785)` visibly sends its vertical candidate through printed content. `NG_0246` is the lower/right-shifted fallback case: current `(601.540,444.077)`. Both are `CornerOuterContourVerified=0`, therefore neither is current semantic/gate evidence.
- `P156_OPERATOR_MARK_PACKET.md` defines the exact source-coordinate form: two lower/right edge segments, virtual corner, and the intended interpretation (tangent intersection, rounded-corner offset, or fiducial). Runner build passed with 0 warnings/errors; both runs returned `Success=True`, `ResultCount=1`, `EdgeCount=2`, and `IntersectionCross=1`. The current source/grid/runtime images were inspected.

## 2026-07-20 P157 Card-Corner Manual-Tolerance Fallback Repair

- The user supplied the translated marks. They were mapped through the shown grid as approximate intent, not pixel-exact labels: `NG_0207=(546,390)` and `NG_0246=(601,441)`, each with +/-20 px tolerance. The exact mapping, source images, raw operator screenshots, XML, logs, runtime drawings, and comparison drawings are in `artifacts\p157_card_corner_manual_tolerance_repair_20260720\P157_MANUAL_TOLERANCE_REPAIR.md`.
- The `NG_0207` fallback had selected printed content at `(369.509,389.785)` because the old code accepted projection before evaluating its available outer-edge Hough pair. The smallest repair evaluates the direction-constrained Hough pair first when the bright component touches the frame. Latest runtime now returns `(547,389)` (difference `(1,-1)` px to the approximate mark); its red runtime lines follow the lower/right card boundary. `NG_0246` stays on the projection path at `(601.540,444.077)`, within the mapped mark tolerance.
- Result drawings now label the geometry source as `hough`, `projection`, or `outer`. Both fallback labels still publish `CornerOuterContourVerified=0`; LLM XML authoring guide/catalog explicitly say they are visual-review conditions, not coordinate/angle gate evidence. Do not re-label a fallback as verified merely because it coincides with a freehand mark.
- The fresh `bin\Any CPU\Debug` Runner DLL (not the stale default `bin\Debug` runner) completed Train `350/350`, Validation `75/75`, and Test `75/75`, zero missing rows. This proves execution stability, not 500-image semantic correctness. `NG_0066` remains `(531.274,352.716)` versus the prior recovered mark `(530,391)`, 38.305 px outside tolerance; it is preserved as the required remaining mismatch.
- P157 is **Incomplete**. A remaining geometry definition is needed for `NG_0066`: confirm whether the desired point is the actual two-edge tangent, a rounded-corner offset, or a separate fixed fiducial before further changing the contour path or adding an acceptance gate.

## 2026-07-20 P158 LLM Assistant Outer-Corner Correction Contract

- P158 converts P157's visual result into an in-product LLM XML authoring contract. An enabled `OuterCornerIntersection`/`BrightObjectCorner` draft now visibly reports `Corner WAIT: run; red/green + hough/projection/outer vs mark; no coordinate gate if fallback.` It tells the operator to execute explicitly, compare exact runtime geometry and selected source label with the mark, then request/carry out a correction only when that evidence shows a mismatch.
- The report is intentionally advisory, not an import blocker: XML validation has no image execution evidence and must not auto-run Preview/Run, create layers, or change routing. A draft is not a provider transcript or a proof of card geometry.
- Fresh current-source before/after Recipe Manager captures are under `artifacts\p158_llm_outer_corner_correction_contract_20260720\llm_review_before` and `llm_review_after`. The final 1600 x 900 view shows the entire compact line; the focused smoke passed `layout=0`, `text=0`, and `internal=0`. P158 is **Complete**. Preserve P157's `NG_0066` discrepancy as the remaining independent geometry task.

## 2026-07-20 P159 NG_0066 Card-Corner Interpretation Packet

- P159 reran the exact current Debug runtime on `NG_0066`: outer tangent `(531.274,352.716)`, `CornerOuterContourVerified=1`, 129 support points. It remains 38.305 px from the retained recovered mark `(530,391) +/-20 px`. `artifacts\p159_card_corner_interpretation_packet_20260720\P159_OPERATOR_INTERPRETATION_PACKET.md` binds the current source/XML/log/runtime drawing and separately labelled manual-mark comparison.
- The packet closes the safe evidence-preparation work without guessing an algorithm change. Its reusable LLM prompt supplies the result, mark, and tolerance, and asks the operator to choose exactly one contract: true outer-edge tangent, intentional rounded/cropped-corner offset, or separate fiducial. The LLM guide now explicitly forbids silently turning an out-of-tolerance mark into an XML parameter change or coordinate gate.
- P159 is **Complete** as a decision packet. The card-corner judgement remains externally blocked until that inspection meaning is chosen; do not retune `OuterCornerIntersection` from this one disagreement.

## 2026-07-20 P160 Same-Image Card-Corner Validation Correction

- The operator corrected the prior assumption: the freehand mark recovered for `NG_0066` may be from a different card image. Therefore its `(530,391)` coordinate and the previously reported 38.305 px difference cannot be treated as same-image ground truth. P155/P159 coordinate comparison is invalidated for semantics, XML tuning, tolerance, gate, and LLM correction. Keep only the exact runtime execution artifact as historical evidence.
- The LLM Assistant now visibly requires `Corner WAIT: same image; red/green + hough/projection/outer vs mark; no gate if fallback.` It makes source-image provenance explicit: review only the exact source/XML/runtime drawing/mark set, never absolute intersections across translated cards. The authoring guide and catalog contain the same rule.
- Fresh current-source before/after WPF captures are in `artifacts\p160_same_image_corner_validation_20260720\llm_review_before` and `llm_review_after`; final 1600 x 900 smoke passed with `layout=0`, `text=0`, and `internal=0`. P160 is **Complete**. The next evidence task is same-image visual review of representative translated card samples, not a fixed-coordinate gate.

## 2026-07-20 P161 Same-Image Card-Corner Review Packet

- P161 creates the required same-source visual-review set at `artifacts\p161_card_corner_same_image_review_packet_20260720`. Six case folders each retain a copied execution input, exact card-corner XML, raw current-run log, runtime overlay, source-grid/runtime-overlay pair, and SHA-256 manifest. The grid is an operator aid; only the lime lines/cross are runtime output.
- The packet includes normal, translated, prior fallback-investigation, outer-contour, left-shifted, and high-in-frame card examples. It reran the unchanged measurement XML on all local splits: Train `350/350`, Validation `75/75`, Test `75/75`, with zero missing images. This proves current execution/pair provenance only; it does not claim semantic corner correctness or a judgement gate.
- When resuming, request or process only marks made on the matching P161 source grid. Keep case ID, mark, source, XML, and overlay together. Never compare absolute X/Y across the individual cards. No code/XML/LLM change is warranted until that same-image ground truth exists.

## 2026-07-20 P162 Confirmed Hough Frame False Positive

- The operator reviewed P161 `05_left_shift_ng` and correctly rejected the result. The exact runtime image is `artifacts\p161_card_corner_same_image_review_packet_20260720\cases\05_left_shift_ng\runtime_result.png`: it is labelled `hough`, selects the frame row `(12,473) -> (346.319,473)`, and joins it to a diagonal `(418,24) -> (346.319,473)`. These are not the card's adjacent lower/right outer edges.
- The current Hough selection allows this because its candidate/inside-image limits accept the 480 px image row `y=473`, while its score explicitly favors a lower/larger intersection and long lines. This is a reproducible false positive, not a valid measurement. Do not cite P161's 500/500 result as semantic correctness.
- First next implementation task: require card-boundary support for Hough candidates and reject the frame false positive, without adding a brittle fixed margin. Verify the exact failed source plus representative P161 cases through fresh overlays before rerunning the batch. The card gate itself still requires same-image marks after the mechanical false positive is fixed.

## 2026-07-20 P163 Card-Boundary-Supported Hough Repair

- P163 resolves the P162 frame false positive in `VisionPipelineOuterCornerIntersectionTool`. A detached large bright card candidate is found at higher thresholds only when the configured threshold joins it to the frame. Hough/projection intersections must be supported by that candidate's lower-right contour region, and Hough horizontal candidates must have repeated card-inside-above/card-outside-below support. This is component-relative support, not a fixed frame margin.
- Current evidence is `artifacts\p163_card_boundary_support_repair_20260720`. The rejected `05_left_shift_ng` changed from frame `(346.319,473.000)` to outer `(534.118,379.121)`. Review also caught an internal-band Hough error on `06_low_corner_ng`; it now returns `(530.321,392.620)` on the card lower/right boundary. Current copied source/XML/log/result pairs and SHA-256 manifest are retained. `02_hough_ng` and the low-contrast `NG_0172` projection remain executable.
- Latest local measurement replay is Train `350/350`, Validation `75/75`, Test `75/75`, zero missing rows. This confirms runtime completion and the repaired reviewed cases, not a card defect classifier or coordinate gate. Resume with P163 same-image marks only; do not compare absolute points across card images.

## 2026-07-20 P164 Card Virtual-Corner Definition

- A subsequent operator review invalidated P164's interpretation. The required proof was that the fitted lower line came from the physical card-bottom edge; changing the label to `Virtual corner` did not establish that boundary ownership. The replay remains execution evidence only, and `CornerOuterContourVerified=1` refers to the selected threshold contour rather than the operator-intended physical boundary.
- P164 is **Incomplete** and superseded by P165. Preserve `artifacts\p164_card_virtual_corner_definition_20260720` as a rejected interpretation baseline; do not cite it as correct card geometry or continue image-by-image tuning.

## 2026-07-20 P165 Inspection-Intent Skill Strategy

- The product keeps LLM-assisted rule-based recipe authoring, but narrows the promise to guided initial setup and evidence-backed correction. Arbitrary image plus prompt -> autonomous correct inspection is explicitly rejected. The operator provides intent, ROI/measurement region, tolerance, and sample evidence; the LLM drafts constrained XML; deterministic execution and explicit review decide acceptance.
- OpenVisionLab inspection-intent skills are now the primary development unit: required inputs, locked existing tool family, starter XML, metrics/gates, N-sample drawings/error table, genuine correction packet, and held-out completion gate. They are in-product recipe-wizard/template contracts, not Codex plugins.
- First pilot: `Pin row gap / pitch consistency`, initially using `PinArrayGap` adjacent edge-to-edge clearance and requiring explicit ROI, polarity, edge-gap versus center-pitch intent, units/calibration boundary, tolerance, and sample split. P151 direct success, P147's 52.40% broad-recipe failure, and P148 frozen split evidence are the decision basis.
- Phase 1 infrastructure is broadly present; Phase 2 infrastructure exists but needs the complete skill workflow; Phase 3 correction-loop evidence is limited. `OuterCornerIntersection` remains experimental and outside default LLM recommendations. P165 is **Complete** as a product-priority documentation decision. The global/project agent rules, product target, current/chronological handoffs, next-chat prompt, LLM guide, and JSON catalog agree; catalog parsing, `git diff --check`, and `OpenVisionReadinessCheck` passed. It changes no runtime/UI behavior.

## 2026-07-20 P166 Pin Row Edge-Gap Skill V1 Design

- Added `docs\OPENVISIONLAB_PIN_ROW_GAP_INTENT_SKILL.md` as the approved v1 contract. The supported intent is one or more independently reviewed single-row ROIs of roughly vertical dark pins, measured as adjacent edge-to-edge pixel clearances with an explicit row `DistancePxRange` maximum. Center pitch, bright pins, unverified mm, and unrelated defect families are blocked or out of scope.
- The skill state model separates measurement-only XML from a judged recipe. P151 remains real GPT direct-success measurement evidence but has no acceptance gate; its generic `Inspection.Evidence` wording and runtime `Acceptance=True` must not be cited as Good/NG quality proof. A strict intent validator must check `PinArrayGap`, ROIs, locked parameters, units, row count, and explicit range gates rather than relying on generic import validation.
- The design preserves the existing LineDistance pin-gap skill and reuses existing Local Validation Sets, batch outcomes/error rows, Pipeline Review, Run History, and runtime drawings. Three existing sets carry Train/Validation/Test; no new dataset schema, runner, algorithm, or provider automation is introduced.
- P148 is the two-row synthetic regression baseline, not new blind evidence or a universal tolerance. Its frozen Test result remains Good 36/36 and `pitch_error` 12/12, but a single top row detects only part of the labelled pitch defects and other pin defect families remain outside the skill.
- P166 is **Complete** as documentation/design only. The next work is Phase 1 implementation of the separate template, readiness states, locked prompt/starter, strict validator, UI evidence, and no-auto-run checks; then Phase 2 product integration and P148 replay; then one natural GPT correction loop on previously unused held-out data.

## 2026-07-20 P167 Pin Row Edge-Gap Skill Phase 1

- P167 implements the first inspection-intent skill as a separate `Pin row edge-gap consistency (PinArrayGap)` Guided Setup option. The existing LineDistance pin-gap workflow remains unchanged. The compact panel captures one or more reviewed row ROIs, Dark/Bright polarity, edge-gap/center-pitch definition, optional Range maximum, and the five PinArrayGap detection values.
- Blank Range creates an importable two-or-more-row measurement draft with no acceptance fields and `MEASURE READY / NOT JUDGED`. A positive Range creates a judged XML draft whose every row uses the same `DistancePxRange` maximum, while the UI remains explicit that Validation is pending. Bright and center-pitch selections are WAIT and disable creation; v1 remains px-only.
- The locked LLM packet and strict validator share the Guided Setup state. Validation compares row count/order, exact ROI values, locked parameters, tool type, unique outputs, later-row branch input, and the requested judgement state. Focused negative replays reject row maxima `6/7`, a mixed `LineDistance` Step, and a missing `CvROI`, leaving Import disabled.
- Current-source WPF smoke kept all 11 new controls visible at 1600x900 and proved that sample selection, field changes, starter creation, and negative validation do not run Preview/Run or change layers, routes, active layer, or preview results. The direct intent smoke also covers valid measurement/judged builders plus Bright, center-pitch, and out-of-bounds ROI rejection.
- P167 is **Complete** for Phase 1 only. Evidence and exact commands are in `artifacts\p167_pinarraygap_intent_skill_20260720\README.md`. Next: connect three existing Local Validation Sets as Train/Validation/Test, persist the frozen skill identity, replay P148, and expose row metrics, error outcomes, and exact runtime drawings before calling the skill validation-complete.

## 2026-07-20 P168 Pin Row Edge-Gap Skill Phase 2

- P168 connects the P167 Guided Setup to three existing Local Validation Set selectors and persists a frozen v1 identity covering the judged XML SHA, common range maximum, row ROIs/detection values, and Train/Validation/Test names, counts, canonical path/expected/notes/per-image-content hashes, and disjointness. Path or same-path byte drift is shown as stale; selecting, freezing, refreshing, and opening the existing explicit-run path do not run Preview/Run or change layers/routing.
- The unchanged P148 two-row XML SHA is `9F8F60E615B9F90CA9D010BE0EC43C0C897BDB3BE5BA0333CF810E0DE139A4F2`. The source split-list file identities are Train 356 / `4BD979B72B5AB6E61689C0609C05DB570658B77AC05AE4859D92914ED133F20E`, Validation 72 / `80D7B1895491459C909FB1565396EBB5F8DC4A463E7B3EF41DEEA00A9CF8747D`, and Test 72 / `F4F483C5FE01B54191D1FD2C1F6DA53D58D27437714D41F110D3F72057D6A3EC`; the product stores separate canonical set-content hashes.
- Current-source replay had zero image-load/runtime errors. Train accepted Good 178/178 and rejected `pitch_error` 38/38; Validation accepted Good 36/36 and has no `pitch_error`; frozen Test accepted Good 36/36 and rejected `pitch_error` 12/12. Full rows are under `artifacts\p168_pinarraygap_phase2_20260720\current_runner`.
- Representative exact-run drawings under `artifacts\p168_pinarraygap_phase2_20260720\representative_overlays` show the ROI, selected pins, adjacent gap lines, labels, and row metrics for ordinary/boundary Good, nearest/worst pitch NG, and one explicitly excluded short-pin outlier. Run Report storage/viewing now retains a SHA-256-verified run-time source snapshot and every executed `PinArrayGap` row drawing; final current-source multi-row viewer evidence is under `artifacts\p168_pinarraygap_phase2_20260720\multistep_current_source_verified`.
- P168 is **Complete** for Phase 2 only: dark pins, pixel units, adjacent edge-gap consistency, and the frozen P148 pilot. It does not prove center pitch, calibrated units, or bend/missing/short/bridge/contamination classification. The P148 Test split is already reviewed and cannot be reused as previously unused Phase 3 evidence.
- Next priority: after a genuine GPT draft naturally fails and a genuinely unused held-out set exists, preserve the first response and complete failure -> evidence-backed correction -> frozen held-out replay. Do not fabricate a failure. Recommended model: GPT-5.3-Codex | Reasoning effort: high.

## 2026-07-21 P169 Phase 3 Prerequisite And GPT Direct Success

- Selected `D:\라벨테스트\Pin_2_Bad_Bent_500_OK_NG\Pin_2_Bad_Bent` as a new synthetic/augmented Phase 3 candidate. Its native Train 356 / Validation 72 / Test 72 lists are pairwise disjoint, all 500 images are unique, and there is no raw-image SHA overlap with the P148 corpus.
- Froze the 72-image Test identity with 72 non-empty, unique per-image hashes in `artifacts\p169_pin2_phase3_prerequisite_20260721\reserved_test_manifest.csv` (SHA-256 `60FD1EA7820919816EA168B6CC31F3C5932750F5DD75D831293381E9C12F06B6`). It contains Good 36, `pitch_error` 12, `short_pin` 12, and `bridge_contamination` 12. Good plus `pitch_error` are the 48 in-scope target rows. No reserved Test image appears in the 428 unique Train/Validation images executed by P169.
- Sent one product-constrained judged prompt through the user's logged-in ChatGPT project without transmitting a local path or image. The exact prompt SHA is `4292A3A485BF361828D2F7802E73FFB1BB5F59628EBFDA1658C6CF21B9C5E3DE`; the unchanged first response SHA is `CB6BB116DCDD9572F6A3BB8D913ECB93881EB308B0F6FEF188037B45F2943F6B`. The visible UI did not provide a reliable model identifier, so the record says model unknown.
- The response passed the current strict `PinArrayGap` intent validator with zero errors and one non-blocking OverlayMerge review warning. Its unchanged two-row `DistancePxRange <= 6` recipe had zero load/runtime errors, accepted Train Good 178/178, rejected Train `pitch_error` 38/38, and accepted Validation Good 36/36; that Validation split contains no `pitch_error`. Runtime overlays were opened and verified for ordinary Good and pitch-error NG cases.
- Because this real first response succeeded directly, there was no genuine failure to correct. No correction message was sent and the Test split was not executed. P169 status is **Blocked** for Phase 3 until a judged first response fails naturally in normal use. Do not repeat equivalent prompts to force a failure. This supersedes P168's historical statement that both a failure and unused held-out set were missing; only the natural failure remains missing.
- Current next priority: preserve that future natural failure, correct from Train/Validation evidence only, freeze the corrected XML, and run the reserved Test once. Until the failure exists, no model work is recommended. Recommended model: 해당 없음 (자연 실패 증거 확보 전 모델 작업 불필요) | Reasoning effort: 해당 없음.

## 2026-07-21 P170 Target-Bearing Working Validation Readiness

- P169's native Validation had Good 36 but no `pitch_error`, so it could not reject a future correction candidate that missed every target pitch defect. P170 prepares a separate target-bearing working split without changing P169, calling an LLM, executing an algorithm, or opening/running reserved Test images.
- `working_train_target_manifest.csv` contains Good 178 / `pitch_error` 26 and SHA-256 `D3A35087CFB2AFA26D5A1D9EB67FE72A224F7BFC6B86FADBFCD87CCFC8D02745`. `working_validation_target_manifest.csv` contains Good 36 / `pitch_error` 12 and SHA-256 `952BAEA1038C0A8AD77524D685E6F69A5CA60E3D539F4CF817147E9EAF30B90B`.
- The Validation pitch rows are the deterministic 12 lowest content hashes from P169 native Train and are removed from Working Train. The two working manifests cover every in-scope non-Test row once, have zero path/content overlap with each other, and have zero path/content overlap with the 48 target rows in the P169 Test manifest.
- All working rows were previously executed in P169. The record therefore labels this `previously observed working Validation`, not blind or unused evidence. It cannot retroactively create a P169 correction event and cannot replace the held-out completion gate.
- P170 is **Complete** for target-bearing split readiness. Evidence: `artifacts\p170_pin2_target_validation_readiness_20260721`. The next project priority is unchanged: wait for a natural judged first-response failure, correct using working Train/Validation only, freeze the candidate, and execute the reserved Test once. Recommended model: 해당 없음 (자연 실패 증거 확보 전 모델 작업 불필요) | Reasoning effort: 해당 없음.

## 2026-07-21 P171 Local Validation Set Provisioning Audit

- Audited the P170 manifests against the existing recipe-local Validation Set storage and Guided Setup selection path. Working Train 204 and Working Validation 48 contain valid OK/NG roles, resolve to existing supported non-Test image files, fit the 5,000-image/64-set limits, and retain zero duplicate or cross-set absolute paths.
- The manifests are not directly product-selectable. Current UI routes are multi-file selection and a top-level folder with one shared OK/NG role; P170 uses selective subsets of the same physical OK/NG folders. No manifest importer exists, and its evidence-logical relative path requires an explicit corpus-root mapping.
- No production or UI change was made. A general importer would be speculative before a natural failure identifies the exact target recipe and before the workflow recurs. At that point, use a narrow dry-run operation with explicit manifest, corpus root, recipe name, and set names; validate all 252 paths/roles/hashes/overlap before merging only Train/Validation into that named recipe. Do not register or execute the reserved Test.
- P171 is **Complete** as a no-change audit. Evidence: `artifacts\p171_validation_set_manifest_import_20260721`. The natural judged first-response failure remains the only current project prerequisite; do not reopen manifest-import work without changed evidence.

## 2026-07-21 P172 Device Top-Left Black-Band Gap Measurement

- The operator selected the vertical pixel thickness of the long black horizontal strip in the `device_top_left` corpus. The bounded contract uses one existing `LineDistance` Step, `PIXELPERMM=0`, and no acceptance gate; it is unrelated to `PinArrayGap` Phase 3.
- A real GPT first response used broad ROI `20,70,510,210` and passed schema/runtime checks, but exact drawings showed 25/63 reference measurements terminating on lower hardware. The same conversation received only that evidence and the reviewed reference ROI, then returned a correction using `20,200,510,60` and manual angle 0. No local image or path was sent to GPT.
- A fresh 0-warning/0-error full build preceded the final current `bin\Debug` validation/import/reference replay. The corrected recipe produced 64 vertical strip-thickness measurements with `Min=22`, `Max=38`, `Avg=28.219`, and `Range=16`; the drawing was opened and confirmed against the exact copied source.
- Full corpus replay exposed the fixed-ROI boundary: first XML mechanical success 461/500, correction 382/500, with inspected false-success and shifted edge-not-found examples. Corpus OK/NG labels are not strip-thickness truth, all three lists were diagnostic replays rather than a blind gate, and no classification accuracy is claimed.
- P172 is **Complete** for the reference-pose GPT failure -> correction loop and boundary audit only. Evidence: `artifacts\p172_device_top_left_black_band_gap_20260721`. No production source changed. Further work requires an operator decision between pose-stable acquisition and a verified rotation/scale-aware fixture; the main project priority remains the naturally failing first response required for the `PinArrayGap` Phase 3 gate.

## 2026-07-21 P173 Device Top-Left Similarity-Fixture Contract

- The user approved continuing the P172 pose-correction decision. A deterministic 24-image audit sampled four filename-quantile OK and four NG images from each Train/Validation/Test list. All 24 audit overlays were opened. Observed strip center Y was `45.55..361.23 px`, angle `-2.544..+2.154 deg`, visible length `435..640 px`, and outer thickness `36.70..78.66 px`; every sampled strip touched the left frame boundary. These heuristic measurements reject a fixed narrow ROI but are not runtime, calibration, or label truth.
- Source inspection confirmed that the current Matching fixture stores X/Y/Angle, applies only X/Y to a cloned common ROI, and does not publish scale. Matching scale search is not round-tripped through pipeline XML/PropertyGrid, LineDistance uses a separate left/right ROI serialization contract, and existing reports can render saved geometry over the wrong image coordinate space.
- The approved v2 design uses `Matching -> fixture-driven RotateScale NormalizeImage -> LineDistance`: one reviewed rigid locator publishes current center/angle/scale; an inverse uniform-similarity warp creates a reference-coordinate layer; the unchanged taught LineDistance ROI measures that layer. It explicitly rejects a rotated-ROI enclosing-box shortcut, silent fallback, generic affine frames, perspective correction, per-image tuning, and unsupported OK/NG or millimetre claims.
- The design contract is `docs\OPENVISIONLAB_MATCHING_SIMILARITY_FIXTURE_V2_SPEC.md`. P173 evidence is `artifacts\p173_device_top_left_similarity_fixture_contract_20260721`, including summary/CSV, Train/Validation/Test contact sheets, 24 individual audit overlays, work contract, and verification record.
- P173 status is **Complete for design only**; no production source or supported LLM XML contract changed. At P173 closure, the next prerequisite was an operator-reviewed stable locator-template ROI plus reviewed reference pose and score/angle/scale limits. P174/P175 subsequently resolved C9 selection, reference pose, and the observed angle/scale search envelope; a deployable score/ambiguity gate remains open but does not block the isolated Pipeline/XML round-trip plumbing slice. Later slices still need dynamic normalization, bounded LineDistance handling, coordinate-correct reports, and current-build N-sample visual proof. The separate `PinArrayGap` Phase 3 natural-failure prerequisite remains unchanged.

## 2026-07-21 P174 Device Top-Left Locator Candidate Audit

- Registration identifies the exact operator-marked source as `device_top_left_OK_0001.jpg` (SHA-256 `4EDD5C5B36ACE3053066AD810E2F5CF75C0E5EFA5C5EC2F047289D74B65C5241`). P172's current-build replay instead used the separate corpus `source.png` copy (SHA-256 `30766834777142F2DBA57265A27E591EDF926A324D5BA546EC74E9F2D468483C`). P174 records that provenance distinction and uses OK_0001 for locator teaching; it does not invalidate the P172 runtime result bound to source.png.
- The first P0 locator `130,260,200,35` appeared correct on the deterministic 24-row subset, but a subsequent all-500 audit rejected it: 82/250 NG masks intersected P0, and fully visible `NG_0248` contained 1,531 defect pixels and produced roughly 274/300 px normalized/fixed wrong-region errors. The complete first run is preserved as a rejected baseline.
- C9 `240,270,65,60` is the best balanced replacement. It is 10 px below the P172 measurement ROI, has zero intersection area, remained fully visible on 491/500 and at least 90% visible on 494/500, and passed the pose-normalized heuristic on every fully visible row. NG mask overlap is 43/250 rather than P0's 82/250. Fixed-scale matching passed only 404/500 and has a reviewed 202.5 px wrong-region case, so scale-aware location remains necessary.
- A separate frozen 24-row OpenCV-Python multiscale run produced score `0.815127..1.0`, independent center-consensus error `0.092..1.776 px`, and 24/24 visually reviewed correct-locator drawings. It is explicitly prototype/non-blind evidence and not current `Lib.OpenCV.MatchingTool`, XML, Pipeline, or EXE proof.
- P174 evidence is `artifacts\p174_device_top_left_locator_candidate_20260721`. The user subsequently approved C9 for the next native qualification step, so P174 is **Complete for candidate selection**. Production source and supported XML did not change in P174. The separate `PinArrayGap` Phase 3 natural-failure requirement is unchanged.
- Reproducibility boundary: the exact 24-row prototype script is retained, but the all-500 registration audit ran as inline Python and its exact source was not saved. Method settings plus all output tables/drawings are recorded under `all500_audit`; do not describe the all-500 audit as exactly replayable from the artifact.

## 2026-07-21 P175 Device Top-Left C9 Native Matching Qualification

- Added `matching-c9-batch` to the existing actual-EXE direct smoke runner. The final scenario uses the real Matching Tool View, requires explicit evidence paths and a new/empty output, keeps automatic Preview off, and records image-load count delta `0`, explicit Preview delta `+1`, current native result presence, and a fresh Preview PNG per case. Its exact `Smoke_MatchingC9_<12 hex>` workspace is removed in `finally`.
- Current EXE/native-library field semantics passed three synthetic angle/scale cases (`3/3`). The exact P174 observed set passed `24/24`: minimum score `80.358`, maximum center error `2.032 px`, minimum box/polygon IoU `0.895`, maximum scale error `0.05691`, and maximum local black-strip angle error `0.92995 deg`. The explicit-run contract passed `27/27`, all three contact sheets and extrema drawings were opened, and every result selected the intended C9 joint.
- r1 is rejected because localized result parsing prevented a usable report. r2 produced `18/24` by comparing local C9 angle with whole-device ORB rotation. r3 corrected the angle oracle and produced valid `24/24` drawings, but a later audit found hardcoded ignored-artifact inputs, missing Preview-count proof, and a leaked fixed Smoke recipe. r4 closes those harness defects while retaining P173 local strip angle as the measurement-relevant oracle and ORB-global angle only as diagnostic.
- P175 is **Complete for current native Matching Tool View qualification only**. Evidence: `artifacts\p175_device_top_left_c9_native_matching_20260721_r4`. It does not prove a deployable score threshold, Pipeline XML scale round trip, fixture normalization, Gap measurement, OK/NG classification, calibration, unseen-data robustness, or production readiness.
- Next priority: implement the bounded Matching pose/scale Pipeline round trip and reference-pose metrics before inverse similarity normalization. Recommended model: GPT-5.3-Codex | Reasoning effort: high.

## 2026-07-21 P176 Die Pad 1 Native Matching Batch Qualification

- The operator rejected using the P175 `device_top_left` C9 fixture patch as representative general Matching evidence. C9 remains valid only as the approved black-strip Gap fixture locator. P176 switches generic Matching validation to the dedicated local `EasyMatch_Die_Pad_500` corpus.
- `OpenVisionLabDirectSmokeRunner.cs` now includes the test-only `matching-die-pad-batch` scenario. It freezes only the `Die Pad 1.bmp` metadata family, verifies all source MD5 values, uses Train/OK `die_pad_001_ok.jpg` and template ROI `90,150,270,220`, saves/loads the native Matching property XML, runs the actual Tool View only through explicit Preview, and retains source/native preview/evidence overlay for every row.
- r3 passed all 122 rows: Train 82/82, Validation 27/27, Test 13/13, OK-role 62/62, and NG-role 60/60. Scores were `89.689..99.347` with average `95.271`; loads caused zero Preview increments and explicit Preview incremented exactly once in 122/122. All 122 sources, previews, and overlays existed, their input MD5 checks passed, and no reserved Smoke workspace remained.
- The final source state passed the full Debug build with 0 warnings/0 errors and `OpenVisionReadinessCheck` with every contract `OK`. A separate artifact audit repeated all 122 metadata MD5 comparisons and found zero empty evidence files.
- Visual review opened the Train/Validation/Test contact sheets, representative/boundary sheet, reference ROI, and minimum-score `train_NG_die_pad_109_ng` overlay. Every yellow runtime box and green detected center stayed on the intended pad/trace corner. r1/r2 are rejected zero-row command-line quoting failures and are not algorithm evidence.
- Evidence: `artifacts\p176_die_pad_1_native_matching_20260721_r3`. P176 is complete only for single-target localization on this one synthetic source family. OK/NG remain defect-role labels, and missing generator transform metadata prevents exact pose-error claims. Next: use P176 as the generic Matching evidence for pose/scale Pipeline/XML round trip, then use the same proven plumbing with C9 only for the P172 Gap fixture workflow. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-21 P177 Operator-Approved Zero-Degree Die Pad Matching Template

- The operator selected the lower-right Die Pad feature instead of P176's broad contextual ROI. The frozen ROI is `190,220,175,145` and includes the right two pads/holes, stepped traces, and outer bottom/right L corner. P176 remains preserved as the prior broad-template comparison.
- The `matching-die-pad-batch --profile zero-reference` path measures the reference outer baseline inside `170,320,210,60`, rotates the full reference around the approved ROI center by the measured `-1.789910608°`, requires a residual within `0.2°`, then crops the template. The final detected residual was exactly `0.000°`; the before/after image and detected lines are retained. This is rotation rectification, not perspective correction or calibration.
- After a fresh 0-warning/0-error Debug build, the current EXE passed 122/122 actual Matching Tool View runs: Train 82/82, Validation 27/27, Test 13/13; role labels OK 62/62 and NG 60/60. Score min/avg/max was `85.554 / 96.426 / 99.629`, angle range `-2.5..+2.5`, scale range `0.9..1.15`, and explicit-run contract 122/122.
- All 122 metadata MD5 values matched and no current-run source, native preview, or overlay was missing. Visual review opened all split sheets plus zero-degree, ROI, representative, and minimum-score drawings; every result selected the approved feature. Minimum-score `val_NG_die_pad_198_ng` remained correctly localized despite a defect crossing the template's left side.
- Evidence: `artifacts\p177_die_pad_1_zero_degree_matching_20260721_r1`. P177 completes only the approved 0° native single-target template on one synthetic source family. The next priority is Matching pose/scale Pipeline/XML round trip using this template; C9 remains only the black-strip Gap fixture locator. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-21 P178 Object-Bounded Zero-Degree Die Pad Matching Template

- The operator rejected including P177's remaining uniform lower background in the taught feature. The new `object-only-zero-reference` profile preserves P176/P177 and freezes ROI `190,220,175,130`, removing 15 px below the physical object while retaining the right two pads/holes, stepped traces, and outer L boundary.
- The same Train/OK reference baseline measured `-1.789910608 deg`; full-reference rotation produced a `0.000 deg` residual before cropping the 175x130 template. A fresh full Debug build passed with zero warnings and zero errors.
- Current EXE execution passed 122/122: Train 82/82, Validation 27/27, Test 13/13, OK-role 62/62, NG-role 60/60. Score minimum/average/maximum was `84.731 / 96.272 / 99.568`, and the explicit-run contract passed 122/122.
- Independent audit found zero MD5 mismatch, zero Preview-contract violation, and zero missing source/preview/overlay files. The exact template, ROI, representative, and minimum-score drawings were opened; every reviewed yellow runtime box remained on the intended object. Minimum-score `val_NG_die_pad_198_ng` remained correct at center `(266,261)`, angle `+1.5 deg`, and scale `0.95`.
- Evidence: `artifacts\p178_die_pad_1_object_only_zero_degree_matching_20260721_r1`. P178 is complete only for the tight rectangular 0° native locator on one synthetic source family; it is not alpha-masked matching, defect classification, exact pose truth, Pipeline XML round trip, or field qualification. Next: implement Matching pose/scale Pipeline/XML round trip with P178 as the generic proof and retain C9 only for the Gap fixture. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-21 P179 Matching Pose/Scale Pipeline/XML Round Trip

- Added the existing native Matching uniform-scale fields to the app-owned Pipeline builder, PropertyGrid mapper, parameter schema, validator, and app tool factory. No new matcher or automatic Preview path was added. Startup-relative template paths are resolved consistently by both Matching execution and fixture scale publication.
- Fixture publication now preserves reviewed/current scale, emits `FixtureScale` and `FixtureScaleRatio`, and rejects non-positive or inconsistent scale geometry. Because `VisionToolOverlay` has no scale property, current scale is derived from its runtime bounds divided by the resolved template dimensions and snapped to the configured native scale grid.
- Pipeline Review reference teach now requires and saves reviewed center, angle, and scale. Focused smokes verify PropertyGrid -> XML save/load -> apply-back preservation, no Preview during mapping or reference save, and unchanged consumer route/ROI/layer state. The current fixture consumer still applies only X/Y translation.
- The current Debug EXE ran one startup-relative XML on four images with zero validation errors/warnings and four successful explicit runs. Three exact P178 rows matched the native Tool View center/angle/scale values at `0.90/+3`, `0.95/-2.5`, and `1.15/+2.5`; fresh runtime overlays show rotated boxes on the approved tight two-pad object. The XML snapshot, decoded source, result image, runtime overlay, hashes, metrics, and overlay geometry are colocated per case.
- Full build and current-source UI smokes passed. Evidence: `artifacts\p179_matching_pose_scale_pipeline_roundtrip_20260721`. P179 is complete only for pose/scale round trip and evidence publication. Next: preserve reviewed reference image width/height and implement fail-closed inverse-similarity `NormalizeImage` before adding LineDistance. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-21 P180 Matching Report Angle Convention And Host PropertyGrid Theme

- The operator correctly found that P179's report overlay used the opposite visible tilt from the native Matching Tool View for the same `+2.5 deg` result. Metrics/detection were unchanged; the defect was isolated to report drawing in image coordinates.
- `VisionPipelineRunReportImageRenderer` now uses the native image-coordinate sign and adds a yellow local-X orientation mark. Replaying the unchanged P179 XML on exact image `die_pad_199_ng` retained score `97.064`, center `(278,278)`, angle `+2.5`, scale `1.15`, and produced a corrected rectangle matching the native tilt.
- The WPG bridge now has per-instance `Default` and `Dark` theme variants. Recipe Manager selects `Dark`; Matching and other Tool Views retain the WPG-CUSTOM-derived `Default`. No WPG-CUSTOM source/DLL was modified or duplicated.
- Full Debug build and focused Recipe Manager/Matching UI smokes passed with current-source images. Evidence: `artifacts\p180_matching_angle_and_property_grid_host_theme_20260721`. P180 does not normalize images or compensate measurement ROIs. Next priority remains reviewed reference dimensions plus fail-closed inverse-similarity `NormalizeImage`. Recommended model: GPT-5.3-Codex | Reasoning effort: high.

## 2026-07-21 P181 Matching Similarity NormalizeImage

- Reference teach now persists source width/height with Matching center/angle/scale. Recipe Manager PropertyGrid/XML round trip covers the new producer dimensions and the `RotateScale` fixture consumer fields without automatic Preview/Run.
- `RotateScale` now dispatches to a bounded full-image inverse-similarity path only when `USE_FIXTURE_FRAME=true` and `FIXTURE_APPLY_MODE=NormalizeImage`. It creates a clean reference-sized output layer and separate runtime overlays/metrics for valid boundary, reference axes/center, valid-pixel ratio, and applied correction. Fixed RotateScale remains unchanged otherwise.
- Runtime and definition validation fail closed on missing dimensions, size mismatch, invalid frame/pose/scale/angle, ROI/masks, and invalid/insufficient valid coverage. No routing or saved ROI is rewritten and there is no unnormalized fallback.
- Current-build XML/runtime evidence passed identity, `-5/+5 deg`, and `0.8/1.2` scale boundaries with reviewed-region mean absolute differences `0 / 2.225 / 2.208 / 2.990 / 2.016`. Three configuration failures and fixed RotateScale compatibility also passed. Fresh before/after UI and current-run algorithm drawings are under `artifacts\p181_matching_similarity_normalize_image_20260721`.
- Final verification passed the full Debug build with zero warnings/errors, `OpenVisionFixtureSmoke`, both focused PropertyGrid/reference-teach screenshot targets, readiness, external-reference policy, public-sample policy, catalog JSON parsing, and diff whitespace checks. Use `after_verified` as the final P181 UI evidence folder.
- P181 is complete only for `Matching -> NormalizeImage`. Next: run C9 on the exact P175 24 rows, attach the unchanged P172 `LineDistance` ROI to `DeviceAligned`, preserve coordinate-correct drawings for every Step, and compare with an identical unnormalized control. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-21 P182 C9 Normalized LineDistance Coordinate Evidence

- Replayed the exact P175 24-row manifest with the frozen C9 producer, the P181 `NormalizeImage` consumer, and the unchanged P172 ROI `20,200,510,60` on `DeviceAligned`; no per-image tuning was used.
- Pipeline `LineDistance` keeps raw edge-point intersections by default. With the existing paired `USE_EXTEND_FIT_LINE=true`, it measures between the two fitted edges, rejects endpoints outside the source/ROI, and retains overlays for the ROI, fitted edges, edge points, and final distance lines.
- Normalized execution was mechanical/ROI-valid on `24/24`; the identical raw-coordinate control executed on `18/24`. Observed normalized `DistancePxAvg` was `38.5..50.5`, maximum `DistancePxRange` was `23`, minimum Matching score was `80.367`, and minimum valid-pixel ratio was `0.309`.
- Every row retains source, clean normalized image, per-Step overlays, source hash, and normalized/control XML hashes. Train/Validation/Test sheets and ordinary, extreme, high-scale, and raw-failure rows were visually reviewed. Final post-build evidence: `artifacts\p182_c9_normalized_gap_20260721_r10`.
- P182 is complete only for the observed-set coordinate/drawing path. Next priority is deliberate no-target/ambiguous/out-of-angle/out-of-scale/insufficient-coverage evidence and a frozen fail-closed operating policy before broader C9 replay. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-21 P183 C9 Fail-Closed Pre-Measurement Gate

- Added a separate `NUM_MATCH=2` C9 preflight and the `ScoreMargin` metric so exact duplicate strong targets fail before the existing `NUM_MATCH=1` fixture producer. Added optional paired `FIXTURE_MIN_SCALE_RATIO`/`FIXTURE_MAX_SCALE_RATIO` gates without changing older recipes that omit them. Fixture-publish failure summaries now retain the runtime Matching rectangle and metrics.
- The bounded operating policy is `SCORE_MIN=0.8`, `ScoreMargin >= 10` percentage points, absolute angle delta `<= 5.25` degrees, scale ratio `0.8..1.8`, and NormalizeImage valid-pixel ratio `>= 0.25`. Exact P175 observed rows passed `24/24`; deliberate no-target, duplicate, 8-degree, 1.9x, and `0.227`-coverage cases failed at Steps 1/1/2/2/3 respectively. The angle/coverage exercise XMLs widen only the upstream search/bound needed to reach the downstream gate and are not operating policy.
- Final evidence is `artifacts\p183_c9_fail_closed_thresholds_20260721\gate_r6`, including source copies, pipeline XMLs/hashes, `rows.csv`, failure messages, and current-run overlays. P183 is complete only for this C9/P175 starter gate; it is not a general Matching default, black-strip OK/NG truth, all-500/unseen evidence, calibration, or field qualification.
- Next: replay the broader `device_top_left` corpus using the frozen P183 gate plus the P182 normalized LineDistance ROI, retain per-Step drawings/hashes, and report gate failures separately from successful pixel measurements without inventing OK/NG truth. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-21 P184 Device Top-Left Full-Corpus Guarded Gap Replay

- Added `OpenVisionFixtureSmoke --c9-gap-corpus` as a focused evidence harness. It executes one saved XML containing the frozen P183 pre-measurement gate and the unchanged P182 `20,200,510,60` pixel Gap ROI; no production algorithm behavior, fallback, or per-image tuning was added.
- The exact 500 unique `device_top_left` images produced 487 ROI-valid pixel measurements and 13 classified fail-closed outcomes: 10 no-target/low-score and 3 out-of-scale. There were no unclassified/load/thrown runtime failures. `DistancePxAvg` min/median/max was `20.308 / 46 / 51.512`, and `DistancePxRange` min/median/max was `0 / 7 / 37`.
- Every source copy/hash, runtime result, and executed-Step drawing is retained. The final set contains 500 source copies, 500 runtime results, 1,964 overlays, 22 contact/representative sheets, source manifest SHA-256 `1A103450773D9E0242BA2EAAD51F6EC6744EDFF32DCC218DBF08E74E7755DEEA`, and XML SHA-256 `8963A7528EBDEF493541C5CF6E781BB4F7A5ABCD04E92C7B802A5D86D8D1E1CB` under `artifacts\p184_c9_full_corpus_gap_20260721_r1`.
- Visual review found no repeatable runtime/report defect that justified weakening the gate or tuning per image. Large-range/low-count rows remain explicit operator-review evidence because no independent black-strip tolerance or calibration was supplied. OK/NG folder names remain label-only and are not Gap truth.
- The other directions are not coordinate variants of this recipe: top-right is the closest physical intent but needs a separately reviewed reference/ROI/locator, bottom images expose different boundaries, and left/right expose curved-width intents. Next: prepare a top-right candidate drawing and obtain operator review before XML teaching or any 500-image replay. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-22 P185 Device Top-Right Contract Approval Candidate

- Used canonical `device_top_right/source.png` rather than mirroring the top-left recipe. Compared three Gap widths and three non-overlapping locator patches on all 500 images by SIFT/RANSAC geometry projection, visibility, reference texture, and NG-mask overlap. This is design evidence only.
- Rejected the first audit because an inlier-count-only rule admitted degenerate homographies. The final audit also bounds estimated scale to `0.5..3.0` and absolute angle to `<=10` degrees, accepting 492/500 rows.
- Final proposal: Gap ROI `330,245,260,40` on the black strip and locator `460,286,70,52` on the center joint, with zero area overlap. Their >=90% visibility counts are 435/492 and 440/492. The left/right locator alternatives reached 344/492 and 323/492.
- Final evidence is `artifacts\p185_device_top_right_contract_candidate_20260722_r5`, including the selected-only canonical drawing and variant sheet. P185 is blocked on explicit operator approval of both marked regions. No XML, template teaching, OpenVisionLab Matching, normalization, Gap execution, or 500-image recipe run has occurred. Recommended model before approval: 해당 없음 | Reasoning effort: 해당 없음.

## 2026-07-22 P186 Device Top-Right Gap-Only Correction And Small-Split Failure Evidence

- The operator corrected P185: the target is only the vertical thickness of the marked long dark upper/lower-plate Gap. The locator, Matching, template teaching, and NormalizeImage are explicitly excluded and the r5 locator proposal is superseded.
- The r6 XML contains one raw-image `LineDistance` Step at ROI `330,245,260,40`. The canonical source and exact ten-image representative split were executed from the latest explicitly built `VisionRecipeRunnerSmoke`; the batch completed 10/10 inputs with five runtime successes and five `LineGaugeEdgeNotFound` failures.
- Current-run visual review found semantic false successes: several images moved the intended strip above the fixed ROI, while LineDistance measured a partial or unrelated lower edge pair. Three broader raw-coordinate ROIs executed 10/10 but connected unrelated distant edges and are rejected. Do not trade semantic correctness for execution count.
- The smoke overlay renderer now uses the loaded pipeline Step definition to draw the configured ROI even when a failed runtime summary omits parameters. This changes evidence rendering only, not algorithm execution.
- Evidence is `artifacts\p185_device_top_right_gap_only_20260722_r6`; status is Incomplete. Do not run all 500 and do not restore Matching. Next: add the smallest bounded direct-Gap pair-selection mode inside `LineDistance` using an operator coarse ROI plus expected separation/orientation/support/uniqueness gates, then replay the same small split with candidate/selected-line drawings. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-22 P187 Device Top-Right Direct Dark-Band Gap Edge Pair

- Added `USE_GAP_EDGE_PAIR=true` as an opt-in `LineDistance` path. It operates only inside one reviewed coarse ROI, merges collinear edge fragments, and gates a selected pair by separation, orientation, shared support, local dark coverage, band darkness, and distinct-candidate margin. The legacy LineDistance path remains unchanged when the flag is absent; no Matching, locator, template, NormalizeImage, hidden ROI movement, or per-image coordinate was added.
- The frozen P187 XML uses coarse ROI `100,80,530,230`, pixel-only separation `12..60`, maximum line angle `8 deg`, parallel delta `4 deg`, support ratio `0.26`, local dark contrast `8`, dark coverage `0.25`, and score margin `0.05`. PropertyGrid, schema validation, known metrics, LLM catalog, and authoring guide carry the same contract.
- Latest-built current runtime replay completed the canonical source and all ten exact P186 rows. Final drawings were opened for every row: green ROI, yellow candidates, blue/magenta selected edges, and five red Gap samples all followed the reviewed dark band. Split `DistancePxAvg` values were `22.4..51.0 px`; canonical was `25.0 px`. This is measurement evidence, not OK/NG truth.
- Evidence: `artifacts\p187_gap_edge_pair_20260722`. P187 is complete only for this top-right small split. Next: freeze it as an in-product inspection-intent skill contract, then replay the unchanged XML on all 500 with per-row drawings and named fail-closed outcomes. Recommended model: GPT-5.3-Codex | Reasoning effort: medium for the contract; gpt-5.6-sol | Reasoning effort: high for all-500 replay.

## 2026-07-22 P188 Dark-Band Gap Inspection-Intent Skill Contract

- Added the distinct Guided Setup intent `Dark band thickness / Gap (LineDistance)` with one operator-reviewed coarse ROI and an explicit px-only, measurement-only boundary. The generated starter is one `LineDistance` Step using the frozen P187 Gap-edge parameters; Matching, locator, normalization, Blob, Contour, calibration, and acceptance are excluded.
- Prompt and strict validator share the same contract. Current-build smoke accepted the generated starter and rejected an unapproved acceptance gate, Matching substitution, and an ROI changed from the operator input. Required distance/stage/support/dark-coverage/ambiguity metrics are registered; drawings remain `WAIT` until explicit Run.
- The persisted starter round-trips through the runtime file loader. Its original canonical `25 px` drawing was later rejected by the user because the magenta line selected a farther lower structure; P189 supersedes that runtime evidence.
- PropertyGrid wording now names expected thickness, edge tilt, shared support, local dark evidence, and distinct-pair margin. Contract: `docs\OPENVISIONLAB_DARK_BAND_GAP_INTENT_SKILL.md`. Evidence: `artifacts\p188_dark_band_gap_skill_20260722`.
- P188 completes Phase 1. P187's small-split semantic evidence is superseded by P189 below. Next: replay the unchanged starter with the P189 runtime on all 500 `device_top_right` rows with hashes, current-run drawings, stage metrics, representative/extreme sheets, and named fail-closed outcomes. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-22 P189 Nearest Same-Band Lower-Edge Correction

- Corrected `USE_GAP_EDGE_PAIR` without changing the P188 starter XML or adding parameters. The selected lower edge is now fitted from the nearest sustained bright transition after the dark core immediately below each supported upper candidate; a farther Hough edge is not accepted as the same band merely because the wider region remains dark on average.
- Canonical evidence moved from the invalid `25 px` line to `DistancePxAvg=14.4`, `DistancePxRange=2`; the selected lower support is `x=219..629`, `y=278.5..266.5` and visually follows the marked black-band bottom. The exact ten-row split completed `10/10`, all rows are on the current contact sheet, boundary/extreme rows were opened full size, and a no-band diagnostic failed closed with zero candidate pairs.
- Evidence: `artifacts\p189_gap_lower_edge_correction_20260722`. Next: replay this unchanged XML/runtime on all 500 `device_top_right` rows with hashes, current drawings, stage metrics, representative/extreme sheets, and named fail-closed outcomes. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-22 P190 Full-Corpus Scalable Dark-Band Gap Audit

- The unchanged P189/P188 XML accounted for all 500 unique top-right rows: 448 measurements, 52 named fail-closed outcomes, zero missing images. Every row retains a source snapshot/hash, runtime drawing/hash, metrics, and elapsed value.
- The frozen deterministic baseline queue contained 128 rows and all 12 contact sheets were reviewed. It exposed repeated successful measurements on lower secondary structures when the intended upper band was clipped or weak; execution count was therefore not accepted as semantic accuracy.
- One bounded correction changed only shared support `0.26 -> 0.60` and replayed all 500 rows. It produced 329 measurements and 171 fail-closed outcomes; all 21 correction sheets were reviewed. Short secondary errors improved, but long lower structures still wrong-passed, so the candidate was rejected and numeric tuning stopped.
- Decision: `Keep with documented limits` only for an operator ROI containing exactly one complete intended long band and no competing long band. General raw-coordinate robustness, OK/NG truth, calibration, other directions, unseen data, and field readiness remain unproved. Evidence: `artifacts\p190_dark_band_gap_full_corpus_20260722`.
- Next: integrate the deterministic queue policy into existing image-list validation / Run History and retained drawing navigation. Recommended model: GPT-5.3-Codex | Reasoning effort: medium.

## 2026-07-22 P191 Deterministic Run History Review Queue

- Extended the existing batch-summary format with a save-time frozen review queue: exact v1 policy, canonical queue SHA-256, result indices, and per-row reasons. Old summaries remain readable but do not fabricate equivalent evidence.
- The generic queue includes all runtime failures, labelled misclassifications, evidence gaps, min/max rows for each varying finite Step metric, and three content-hash-ordered audit rows per declared role stratum. Invariant metrics are omitted from the extrema set.
- Run History adds a read-only `검토 큐만` filter that is mutually exclusive with the existing NG/misclassification filter. Selected rows reuse the retained runtime drawing viewer; no Preview/Run, layer, or route mutation occurs.
- Current-source smoke passed policy/hash persistence, labelled false-accept inclusion, missing-report/runtime-failure inclusion, metric extrema, retained drawing resolution, and a bounded 500-row/two-stratum selection of six audit rows. Full solution build completed with zero warnings and zero errors; readiness, external-reference, and public-sample checks passed. Evidence: `artifacts\p191_run_history_review_queue_20260722`.
- P191 is complete as workflow/deterministic-selection evidence. It is not a 10,000-row performance qualification or semantic-correctness claim. Next priority is blocked on choosing the dark-band operating boundary; the Pin Phase 3 priority remains blocked until a natural judged GPT failure exists.

## 2026-07-22 P192/P193 Approved Hybrid Locator -> Relative ROI Gap Candidate

- The user approved a bounded hybrid workflow: deterministic Matching locates center/angle/uniform scale, NormalizeImage maps into the reviewed reference coordinate system, and LineDistance measures only the stored reference-coordinate Gap ROI. This supersedes the P191 product-choice blocker; it does not turn the LLM into a per-image detector.
- P192 froze a four-Step candidate using existing tool families only. On the exact ten-row split it produced four operator-intended measurements and six locator-ambiguity fail-closed outcomes. All success and failure drawings were reviewed.
- P193 replayed the unchanged candidate on all 500 frozen `device_top_right` rows: 356 measurements, 144 named fail-closed outcomes, zero missing. Terminal failures were 139 ambiguity audit, four pose publication, and one Gap edge selection.
- The deterministic queue retained all 144 failures plus 106 measured stage/metric extremes and role-hash audits. All 18 measured and 24 fail-closed sheets were opened. No reviewed measurement repeated P190's lower-secondary-structure wrong-pass pattern. Some successes used short edge support, and the locator remained ambiguous on 28.8%, so the result is not product-ready.
- Decision: `Hybrid candidate`. Contract: `docs\OPENVISIONLAB_HYBRID_LOCATOR_RELATIVE_ROI_INTENT_SKILL.md`. Evidence: `artifacts\p192_top_right_hybrid_gap_20260722`, `artifacts\p193_top_right_hybrid_gap_full_corpus_20260722`.
- Next: implement this approved contract as a separate Guided Setup/LLM intent skill with strict locked-sequence validation and explicit-Run `WAIT` behavior. Recommended model: GPT-5.3-Codex | Reasoning effort: medium. The current locator may be replaced only after the operator approves a more distinctive feature/template; no model work is recommended before that prerequisite.

## 2026-07-22 P195 Hybrid Relative-ROI Guided Setup/LLM Skill Phase 1

- Added `Locator-aligned Gap (NormalizeImage)` as a separate Guided Setup/LLM intent; the direct raw-coordinate dark-band skill remains unchanged.
- Guided Setup collects the locator template/search ROI, reviewed reference center/angle/scale/image dimensions, reference-coordinate measurement ROI, and score/margin/angle/scale/valid-pixel gates. Its starter is exactly `Matching audit -> Matching fixture publisher -> RotateScale NormalizeImage -> DarkBandGap LineDistance`.
- Prompt and strict validation share this locked contract. Current-source smoke accepted and loaded the valid draft while rejecting a changed normalization tool, changed measurement ROI, and weakened ScoreMargin. It also proved that draft creation/import readiness did not run Preview/Run or mutate layer/routing counts.
- The operator-visible state is `LOCATION GATED / MEASURE READY / NOT JUDGED`; the result channel keeps drawings in `WAIT` until explicit Run. Stale translation-only fixture descriptions were corrected without changing V1 translation behavior.
- Evidence: `artifacts\p195_hybrid_relative_roi_phase1_20260722`. P195 completes Phase 1 only and adds no new semantic runtime proof beyond P192/P193.
- Next priority: obtain operator approval for a more distinctive top-right locator/template before another hybrid replay. Recommended model: 해당 없음 (operator-approved locator feature/template prerequisite) | Reasoning effort: 해당 없음. After approval, freeze the P195-generated candidate and use the scalable N-image protocol. Recommended model: GPT-5.3-Codex | Reasoning effort: high.

## 2026-07-22 P196 Rule-Based-First Direction And LLM Maintenance Mode

- The user approved freezing planned LLM expansion because further provider/prompt/transcript work has diminishing product value while physical target, ROI, tolerance, and semantic truth remain operator-owned.
- Preserve the current LLM Assistant, Guided Setup/XML generation, guide/catalog, strict validation/import, and recorded P168/P195 evidence. Maintenance mode permits concrete regression, unsafe XML acceptance/import, and compatibility fixes; it does not permit new providers, browser automation, API dependencies, prompt families, intent skills, or transcript campaigns without an explicit reopen decision.
- The product is now described as an OpenCvSharp4 rule-based vision recipe workbench with an optional maintenance-mode LLM XML assistant. It must remain fully usable without an LLM account, session, API key, transcript, or generated XML.
- The missing natural Pin Phase 3 failure, frozen P169 Test, and P193 locator limitation remain preserved evidence boundaries, not active next-priority blockers.
- Next priority: complete the non-LLM operator path for `Matching -> NormalizeImage -> reference-coordinate ROI -> deterministic inspection`. Recommended model: gpt-5.6-sol | Reasoning effort: high.
- Remaining priorities: audit and minimally strengthen the existing LineDistance/Gap family as a general Caliper/EdgePair primitive, then validate each approved change with frozen N-sample metrics, fail-closed reasons, current-run drawings, and the deterministic review queue. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-22 P197 Non-LLM Matching Normalization And Reference-ROI Workflow

- Audited the existing rule-based operator path and found that PropertyGrid fixture parameters, Pipeline/XML save-load, explicit reviewed-pose teaching, full-image `NormalizeImage`, layer comparison, and explicit Pipeline Review were already implemented. The missing product piece was a complete public example, not another algorithm or LLM wizard.
- Added the public four-Step `Public_Matching_NormalizeImage_RelativeRoi.pipeline.xml`: Matching publishes `PartFrame`, RotateScale normalizes `Main` into the `572x420` reference image, Threshold creates `AlignedPadBinary`, and Blob inspects unchanged reference `CvROI=320,180,60,50`.
- The Good sample completed `4/4` with center `(200,155)`, offset `(80,55)`, valid-pixel ratio `0.748`, and `ResultCount=1`. The paired missing-pad row reached the same fixed ROI and failed there with `ResultCount=0`, which the public catalog records as the controlled expected failure.
- Added focused current-source Pipeline Review smoke and exact-run Good/Bad overlay evidence under `artifacts\p197_rule_based_fixture_workflow_20260722`. Public policy now reports `CatalogRows=32`, `ManifestAssets=229`, and `Pipelines=16`.
- Status: Complete for one synthetic translated Good/Bad pair only. It does not prove general rotation/scale, unseen data, industrial truth, calibration, or field robustness.
- Next priority: audit the existing LineDistance/Gap family as a general Caliper/EdgePair primitive and identify dataset-proven missing controls, metrics, or drawings before editing runtime code. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-22 P198 LineDistance / Gap Caliper Audit And Distinct-ROI Drawing Correction

- Audited the general `LineDistance` and opt-in specialized `GapEdgePair` paths before adding another metrology family. General LineDistance already supports independent Line A/B ROI, polarity, scan direction/angle, fitted-edge distance, pixel/mm values, and min/max/average/range metrics. GapEdgePair remains intentionally limited to one reviewed long near-horizontal dark band.
- Reproduced a drawing-evidence defect with two distinct public pin ROIs: runtime measurement used both but retained only the first ROI overlay. Equal A/B ROIs now retain the existing compact `Measurement ROI`; distinct A/B ROIs retain labelled `Line A ROI` and `Line B ROI` overlays.
- The frozen public OK replay remained `ResultCount=22`, `DistancePxAvg=37.014`, `DistancePxRange=1.999`. The same measurement-only XML produced `18.300` / `3.994` on the Wide-Pin comparison with both ROIs; this is measurement evidence, not an OK classification. The shared-ROI public pipeline remained compatible, and the P189 specialized Gap canonical remained `14.4 px` with range `2` and its existing drawing set.
- Fresh Line Tool UI smokes passed with zero layout/text/internal findings. Evidence: `artifacts\p198_line_caliper_audit_20260722`.
- Full Debug build, exact focused replays, readiness, external-reference, public-sample (`32` rows / `229` assets / `16` pipelines), and `git diff --check` passed; diff check reported line-ending notices only.
- Status: Complete for the audit, drawing correction, and named regressions. It does not prove industrial semantic correctness, calibration, arbitrary orientation/polarity behavior, or broad-corpus robustness.
- Next priority: prove whether Recipe Manager selected-Step PropertyGrid load/apply/save independently preserves Line A/B ROI, polarity, scan direction, and angle. Do not edit until a loss is reproduced. Recommended model: gpt-5.6-sol | Reasoning effort: high.
- Remaining priority: if and only if that audit proves a loss, fix the affected mapper fields and then freeze a small orientation/polarity Caliper matrix with current-run drawings. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-22 P199 Line Pair PropertyGrid A/B Round-Trip Fidelity

- Reproduced a no-edit Recipe Manager apply loss with one deliberately asymmetric `LineDistance` Step. Right ROI, polarity, vertical projection, and manual-angle settings were replaced by Line A values.
- Corrected the selected-Step mapper only. The PropertyGrid now labels A/B ROI, projection direction, polarity, vertical projection, and manual-angle values independently; loaded baselines preserve unrepresented per-line settings, while changed compact shared fields still apply to both.
- Direct mapper round trip preserved 20 asymmetric parameters; a direct Right ROI/angle edit left Line A unchanged; shared Contrast still updated both. Actual Recipe Manager apply/save/reload preserved the original XML with zero Preview/Run executions.
- Current-source UI shows both distinct ROIs and `Round-trip validation passed`. ReferenceDifference, Fixture, Line measure, and Line pins measure regression smokes passed. The P198 exact runtime remained `22` results, `37.014 px` average, `1.999 px` range, and two ROI overlays.
- Full Debug build, focused/current UI smoke, readiness, external-reference, public-sample (`32` rows / `229` assets / `16` pipelines), and `git diff --check` passed. Evidence: `artifacts\p199_line_pair_property_roundtrip_20260722`.
- Status: Complete for A/B Recipe Manager edit/persistence fidelity only. It is not semantic orientation/polarity, calibration, or field-robustness proof.
- Next priority: define and freeze a small LineDistance Caliper matrix with named intents, reviewed ROIs, expected pixel intervals, horizontal/vertical directions, and dark/bright polarity cases. Recommended model: gpt-5.6-sol | Reasoning effort: high.
- Remaining priority: replay that matrix without per-image tuning and change runtime only if a repeated defect is proven. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-22 P200 LineDistance Caliper Orientation/Polarity Matrix

- Froze one public synthetic pin-to-datum clearance as four deterministic `LineDistance` cases: horizontal/vertical and bright/dark object polarity. Inputs are an exact public baseline, exact intensity inversion, exact 90-degree clockwise rotation, and the combined transform.
- Horizontal uses the reviewed P198 A/B ROIs and `X_LTOR/X_RTOL`. Vertical uses mathematically transformed ROIs and the actual `Y_TTOB/Y_BTOT` projection directions. No per-image tuning or new runtime mode was added.
- All four final current-source runs returned `ResultCount=22`; horizontal measured `DistancePxAvg=37.014`, `DistancePxRange=1.999`, and vertical measured `37`, `2`. Every row passed the external `Avg=35..39 px` plus `Range<=3 px` gate.
- Exact current-run overlays were opened individually and as a contact sheet. They show separate Line A/B ROIs, edge points, and 22 final distances between the same transformed rightmost/bottom pin and datum rail boundaries. Bright/dark variants have identical per-orientation metrics.
- Initial omitted raw-gray threshold flags and vertical X-direction settings failed closed at `703:LineGaugeEdgeNotFound`; correcting the known recipe configuration contract resolved them. No repeated runtime defect was found, so product source was not changed.
- Full solution and runner builds passed with zero warnings/errors. Evidence: `artifacts\p200_line_caliper_matrix_20260722`.
- Status: Complete for one four-case public synthetic configuration matrix; not adjacent pin-gap, center-pitch, calibration, unseen-data, or field-robustness proof.
- Next priority: audit `PinArrayGap` for a separately named center-to-center pitch-consistency contract before any implementation. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-22 P201 PinArrayGap Center-Pitch Semantic Extension

- The audit proved the existing `PinArrayGap` runtime, v1 Guided Setup, metrics, and drawings were adjacent EdgeGap-only, and Recipe Manager selected-Step PropertyGrid treated the tool as unsupported.
- Added optional `MeasurementMode=CenterPitch` without changing legacy XML: absent mode remains `EdgeGap`. CenterPitch derives adjacent centers from the same detected dark-pin runs, publishes only `PitchCount` and `PitchPxMin/Max/Avg/Range`, and draws visible center points plus `P1..Pn` lines. EdgeGap retains `DistancePx*`, optional legacy mm values, and `G1..Gn` drawings.
- Recipe Manager PropertyGrid now exposes the mode, one reviewed row ROI, detection fields, and acceptance. Direct mapper and actual apply/save/reload preserved the mode, ROI, and an unrepresented `ALLOW_BRANCH_INPUT`; `NativePreviewRunCount` remained zero.
- The frozen three-case matrix passed: uniform widths and varied widths with identical centers both produced `PitchPxAvg=60`, `PitchPxRange=0`; one 12px center shift produced `PitchPxRange=12` and expected `PitchPxRange <= 2` rejection. Missing-mode and explicit-EdgeGap result drawings were SHA-256 identical.
- Full runtime evidence and exact drawings are under `artifacts\p201_pin_center_pitch_20260722\runtime`. Fresh current-source UI before/after images are under `artifacts\p201_pin_center_pitch_20260722\ui`; the before image shows `Unsupported step tool: PinArrayGap`, while the after image shows the editable `CenterPitch` selector and successful round-trip state.
- Status: Complete for the dark-pin, one-row, pixel-only synthetic contract and Recipe Manager persistence path. It is not bright-pin, calibration, real-corpus tolerance, non-tuned N-sample, unseen-data, or field-robustness evidence. The frozen LLM Pin Guided Setup v1 remains EdgeGap-only.
- Next priority: audit P148/P168/P170 manifests and reviewed row ROIs for a direct non-LLM CenterPitch N-sample replay without opening the reserved P169 LLM Phase-3 Test. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-22 P202 PinArrayGap CenterPitch N-Sample Validation

- Audited the P170 Working Train/Validation manifests against the local Pin_2 corpus: 252/252 files exist, all content hashes match, all 252 are unique 768x576 8-bit images, and the roles are explicitly `Good` / `pitch_error`. Reused the reviewed P168 top `0,120,768,130` and bottom `0,330,768,130` row ROIs. The reserved P169 Test was not listed, opened, copied, or executed.
- Measurement-only Train replay completed 204/204 images. Using each image's maximum top/bottom `PitchPxRange`, the highest Good was 12 px and the lowest `pitch_error` was 13.5 px. Froze `PitchPxRange <= 12` in XML SHA-256 `5704FC0F76E37FD86EE62C0DA13FF62BEBCB3BAEB0AD9AAB24CD3420AB63247C` before Validation.
- Frozen judged results: Train Good 178/178 accepted and pitch_error 26/26 rejected; Validation Good 36/36 accepted and pitch_error 12/12 rejected. Zero missing images, runtime errors, or misclassifications.
- Corrected one evidence-tool defect: batch evidence previously saved only the final Step result image, hiding the top-row drawing. It now retains a combined all-executed-Step overlay. Production runtime and CenterPitch measurement were not changed.
- Verified all 252 source/drawing hashes. The deterministic queue contains 44 rows (SHA-256 `53826B35F14BE53C7A0ED414A410BFB0D37EA0AFCB9B6B11A9E3C3260280438B`) selected from Pitch metric extrema, three hash audits per split/class, and explicit decision boundaries. Opened all 11 sheets; pin rectangles, centers, and adjacent center-to-center lines remained on the intended top/bottom dark-pin rows.
- Status: Complete; decision `Keep with documented limits`. Evidence: `artifacts\p202_pin_center_pitch_nsamples_20260722`. This is one synthetic/augmented, previously observed, pixel-only dark-pin corpus; it is not independent blind Test, calibration, real-production, or field-robustness evidence.
- Next priority: prove the frozen P202 Validation candidate through the existing Recipe Manager saved-validation and Run History workflow, showing Pitch metrics, combined two-row drawings, and queue identity without a manifest importer or automatic Preview/Run. Recommended model: gpt-5.6-sol | Reasoning effort: high.
- Remaining dependency: independent qualification requires a non-P169, operator-approved, content-hash-disjoint pitch-error Test set with the same physical target and reviewed row geometry. Recommended model: 해당 없음 (data prerequisite) | Reasoning effort: 해당 없음.

## 2026-07-22 P203 CenterPitch Product Saved-Validation Workflow

- Staged the exact P170 Working Validation manifest into a task-local OK/NG folder layout after verifying every source SHA-256: 48 unique rows, 36 OK and 12 NG. The reserved P169 Test was not listed, opened, copied, or executed.
- Executed the frozen P202 two-row `PitchPxRange <= 12` XML through the existing Recipe Manager local Validation Set path. Saved judgement was 36 correct accepts and 12 correct rejects with zero false accepts, false rejects, load errors, or runtime errors.
- Current-source product evidence retained both row gates and both stored PinArrayGap Step drawings. One selected NG row showed `PitchPxMin=45.5`, `PitchPxMax=65`, `PitchPxAvg=56.792`, `PitchPxRange=19.5`, `PitchCount=12`, and the correct `19.5 > 12` verdict without Preview/Run replay.
- Saved Run History retained a 24/48 deterministic queue with policy/SHA-256 identity `196A8EF87728A867F4542F0A09D0AEEFB9C803E6041544893EB089770589E21F`, Pitch metric extrema, and content-hash audit reasons. Queue-only filtering preserved Preview/Run count, layers, and routing.
- No product integration loss was reproduced, so no product runtime/UI code changed. One screenshot-smoke target was added to make the review-queue identity and side-effect assertions reusable. Evidence: `artifacts\p203_center_pitch_product_workflow_20260722`.
- Status: Complete for saved-validation/Run History integration only. P202's independent-data, polarity, calibration, real-production, and field-qualification limits remain; stop CenterPitch tuning on this corpus.
- Next priority: audit the operator-provided pin corpora for a separately labelled missing-pin/row-count intent and compare existing PinArrayGap/Blob/Contour capabilities before any runtime edit. Recommended model: gpt-5.6-sol | Reasoning effort: high.
- Remaining dependency: independent CenterPitch qualification requires a non-P169, operator-approved, content-hash-disjoint pitch-error Test set with the same physical target and reviewed row geometry. Recommended model: 해당 없음 (data prerequisite) | Reasoning effort: 해당 없음.

## 2026-07-22 P204 Missing-Pin / Row-Count Data And Tool Audit

- Audited `D:\라벨테스트\Pins_500_OK_NG\Pins` without changing source data or product code. The NG set is five balanced classes; global class 30 isolates exactly 50 `Pins:missing_pin` rows from the other 200 NG images.
- Froze an audit manifest containing all 250 OK and 50 target rows. All 300 image hashes are unique and all images are 768x576 grayscale. Every OK label is empty and mask is zero; every target row has exactly one class-30 label and a non-empty mask fully covered by its supplied YOLO box.
- Provided split accounting is Train 178 OK/38 missing, Validation 36/12, and Test 36/0. The current Test cannot prove target rejection.
- Selected the existing `Threshold -> Blob` ResultCount family for the first semantic matrix because the target row contains bright connected pin bodies. Rejected current dark-run `PinArrayGap` for this intent and kept Contour only as a fallback if Blob filtering/drawings prove inadequate.
- Status: Complete for data/tool selection only. Evidence: `artifacts\p204_missing_pin_count_audit_20260722`. No ROI, threshold, area interval, count gate, N-sample candidate, or independent Test claim is frozen yet.
- Next priority: draw/review one upper-row ROI and freeze a small product-runtime `Threshold -> Blob` semantic matrix with count metrics and current-run drawings before any full replay. Recommended model: gpt-5.6-sol | Reasoning effort: high.
- Remaining dependency: independent qualification requires an operator-approved, content-hash-disjoint target-bearing Test split because the supplied Test has no missing-pin rows. Recommended model: 해당 없음 (data prerequisite) | Reasoning effort: 해당 없음.

## 2026-07-22 P205 Missing-Pin Fixed-Raw-ROI Blob Matrix

- Froze a six-row semantic matrix from P204: two hash-min Train rows plus one hash-min Validation row per OK/missing role. Raw ROI `0,40,768,360` contains the upper bright-pin row and every selected truth box but intersects bright slanted rails.
- Actual product measurement at threshold 150 and Blob area `200..5000` returned 11 on all OK rows, 10 on two missing rows, and 12 on `Pins_NG_0001`; source-backed runtime drawings proved rail fragments were counted.
- One bounded area correction to `1700..3000`, with expected OK count exactly 11, numerically accepted 3/3 OK and rejected 3/3 missing at count 10. Visual review still invalidated `Pins_NG_0001`: one horizontal rail Blob remained while the right border pin was missed, so the count was coincidental.
- Rejected the fixed raw ROI candidate and stopped numeric tuning. Contour has the same geometric ROI problem. Do not run Train/Validation or report the numerical 6/6 as semantic success.
- Corrected the evidence harness only: `VisionRecipeRunnerSmoke --batch-evidence` now clones the source before execution so combined drawings use the original image rather than a Threshold-mutated binary Mat. Product Blob runtime is unchanged; focused runner build passed with zero warnings/errors.
- Status: Incomplete because the all-physical-pin drawing criterion failed. Evidence: `artifacts\p205_missing_pin_blob_semantic_matrix_20260722`.
- Next priority: audit one existing fixed `RotateScale -> Threshold -> Blob` rectification on the same six rows, with one aligned ROI above the rails and physical-pin-only drawings. Recommended model: gpt-5.6-sol | Reasoning effort: high.
- Remaining dependency: independent qualification requires an operator-approved content-hash-disjoint target-bearing Test split; current Test contains no missing-pin rows. Recommended model: 해당 없음 (data prerequisite) | Reasoning effort: 해당 없음.

## 2026-07-22 P206 Fixed-Rectification Missing-Pin Blob Matrix

- Reused the exact P205 six rows and existing tools only. Product `RotateScale` sign probes proved `+10 deg` straightens the shared pin row and `-10 deg` increases its slant.
- Initial aligned ROI `0,140,768,175` still admitted rail fragments. Correction 1 raised the bottom boundary to `y=298`, removing rails. Correction 2 excluded unstable source-border-truncated pins and froze the common nine-slot interior ROI `40,140,660,158`; threshold 150 and Blob area `200..5000` remained broad and unchanged.
- Frozen three-Step candidate uses `+10 deg`, 100% scale, Binary 150, area `200..5000`, and exact `ResultCount=9`. Raw-source replay accepted OK 3/3 at count 9 and rejected missing 3/3 at count 8.
- Opened all six aligned-stage current-run drawings at original resolution. Every selected component is a physical interior pin; no rail, noise component, or truncated source-border pin is counted. Source, aligned-image, and candidate XML SHA-256 identities are retained.
- Status: Complete for this bounded six-row shared-pose/interior-slot candidate. Evidence: `artifacts\p206_missing_pin_rectified_blob_matrix_20260722`. This does not prove pose extremes, full Train/Validation, independent Test, unseen variation, lighting, or field robustness.
- Next priority: freeze the exact P206 XML and replay one small hash-deterministic pose/border extreme matrix without parameter changes; reject rather than tune if drawing fidelity fails. Recommended model: gpt-5.6-sol | Reasoning effort: high.
- Remaining dependency: independent missing-pin qualification still requires an operator-approved content-hash-disjoint target-bearing Test split. Recommended model: 해당 없음 (data prerequisite) | Reasoning effort: 해당 없음.

## 2026-07-22 P207 Frozen Missing-Pin Pose/Border Extreme Matrix

- Selected eight rows before candidate execution using source geometry, supplied labels, and hashes only: per role, unique minimum/maximum pin-base fit angle and minimum aligned left/right margin. Excluded all P205/P206 rows; missing rows were eligible only when supplied truth mapped inside the P206 aligned ROI.
- Current-build replay kept the P206 candidate XML byte-identical at SHA-256 `A74AE17F44F2076F7277DBF92106DE2BE869D6E1456FD4908FCB6DF982204BE8`.
- Six rows were correct. `Pins_OK_0243` false-rejected at count 10 because a right boundary pin entered the ROI. `Pins_NG_0111` false-accepted at count 9 because the same boundary pin offset a true interior missing pin.
- Opened all eight aligned-coordinate runtime drawings at original resolution and marked both failures red. No parameter correction was attempted.
- Decision: `Reject` the P206 fixed-rectification/interior-ROI candidate and do not run Train/Validation from it. Status: Complete for the extreme audit. Evidence: `artifacts\p207_missing_pin_pose_border_extremes_20260722`.
- Next priority: P208 proposal review now supersedes this line; obtain operator confirmation of Candidate B before any existing `Matching -> NormalizeImage -> Threshold -> Blob` execution. Recommended model: none (operator decision) | Reasoning effort: none.

## 2026-07-22 P208 Missing-Pin Locator Proposal Review

- Used the exact P207 eight source hashes and created proposal-only full-image overlays plus focused crops. No Matching, Preview, Run, recipe change, runtime change, or LLM work was performed.
- Candidate A, the long carrier rail, is secondary angle/Y support only because it is horizontally repetitive. Candidate C, the seam/corner, is clipped and absent in two rows and is rejected as a sole locator.
- Candidate B, the central asymmetric curved machining mark on the lower rail, is outside the judged pins and visually present in all eight crops. It remains unapproved until the operator confirms it is the same durable physical fixture feature rather than glare, dirt, or changing surface texture.
- Status: Complete for the proposal task. Evidence: `artifacts\p208_missing_pin_locator_proposals_20260722`.
- Next priority: obtain the operator decision. If approved, audit the existing hybrid path on the exact P207 eight rows; if rejected, stop this intent for the current framing rather than tune per image. Recommended model after approval: gpt-5.6-sol | Reasoning effort: high.
- Remaining dependency: independent target qualification still requires an operator-approved hash-disjoint missing-pin Test split. Recommended model: 해당 없음 (data prerequisite) | Reasoning effort: 해당 없음.

## 2026-07-22 P209 Missing-Pin Hybrid Locator Audit

- The user approved P208 Candidate B. A rectified template was cropped from the current-build P207 `+10 degree` reference and audited with existing `Matching audit -> Matching fixture publisher -> NormalizeImage -> Threshold -> Blob` on the exact P207 eight rows.
- Full-image search failed the frozen `ScoreMargin >= 10` gate on all eight (`1.51..4.82`) because drawings showed a second similar rail location.
- One bounded correction used only the pre-approved Candidate B union as coarse search ROI `220,350,260,220`. It cleared ambiguity but produced two OK false rejects at unchanged `SCORE_MIN=0.8` and one missing-pin false accept.
- `Pins_NG_0186` counted eight physical pins plus one lower horizontal rail fragment as 9. Operational classification was 5/8. Every full-image and coarse-ROI drawing was opened; no further tuning occurred.
- Decision: `Reject` and stop the missing-pin/count intent for the current framing/composition. Evidence: `artifacts\p209_missing_pin_hybrid_locator_audit_20260722`.
- Next priority: audit the supplied `short_pin` labels/masks, physical length target, split/hash integrity, geometry variation, and existing deterministic tool fit before implementation. Recommended model: gpt-5.6-terra | Reasoning effort: low.

## 2026-07-23 P210 Repeated-Validation Closure And Rule-Based UI Gap Audit

- The user closed repeated image inspection, dataset switching/tuning, and LLM XML/provider validation as active development work. These tasks require a new explicit user request before they may resume.
- Static source and document inspection found 21 canonical tool families, broad deterministic preprocessing/segmentation/matching/metrology coverage, explicit PropertyGrid and Preview/Run contracts, aggregate metrics/drawings, and saved Run History evidence.
- The decisive gap is UI/result depth rather than another image campaign. Blob/Contour currently expose area-oriented configuration and aggregate Pipeline metrics; an unused legacy `DefectListResult` can hold per-object area/angle/center/bounds, but current Pipeline reports do not persist first-class per-object rows.
- Official Cognex, MVTec MERLIC, KEYENCE, and Zebra materials support three selected workbench gaps: Object Results Inspector first; unified Fixture/relative-ROI designer second; general circle/point/line geometric measurement workspace third.
- No product code, UI, image, Preview/Run, batch validation, or LLM provider work was performed. Audit: `docs\OPENVISIONLAB_RULE_BASED_UI_GAP_AUDIT_20260723.md`.
- Status: Complete for the static audit and priority decision. Next dependency is explicit user selection/approval of one UI item.

## 2026-07-23 P211 Object Results Inspector

- The user approved the first P210 UI priority. Pipeline Review now retains and displays stable Blob/Contour object rows with accepted/rejected state, area, center, bounds, angle, and an area-filter reject reason.
- Table selection draws a same-run object rectangle/cross; clicking the object preview selects the same row. Focused smokes verified no extra Preview/Run, layer, active-layer, or route mutation.
- Object rows now round-trip through direct Pipeline and saved recipe-run report contracts. The focused report artifact preserves accepted and rejected rows plus the `MIN_AREA` reason.
- Blob exposes its full area-audit candidates. Contour intentionally audits only candidates at or above 25% of configured `MIN_AREA` in addition to all accepted rows; the earlier unbounded audit exceeded two minutes on pixel-noise contours, while the bounded current-build Contour smoke completed normally.
- Fresh current-build Blob and Contour UI smokes passed. Evidence: `artifacts\p211_object_results_inspector_20260723`.
- Status: Complete for this bounded UI/result-contract slice. Next priority is the Fixture And Relative-ROI Designer over existing Matching -> NormalizeImage -> reference-coordinate ROI behavior. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-23 P212 Fixture And Relative-ROI Designer

- The user approved the second P210 UI priority. Pipeline Review now resolves one existing named `Matching producer -> NormalizeImage consumer -> reachable downstream single-CvROI Step` chain and shows it in one `Fixture / 상대 ROI` tab.
- The tab shows the template/search ROI, taught reference pose and image size, current Matching pose, score, only a same-template preflight margin when present, and NormalizeImage valid-pixel ratio.
- The saved downstream ROI is drawn as a transformed magenta polygon on the current source and as a green unchanged rectangle on the normalized image. Before explicit Review, the source remains visible but transformed/normalized evidence remains run-required.
- Explicit reference teach, Matching producer edit, measurement ROI edit, and Run Review reuse the existing persistence, Recipe Manager PropertyGrid, and execution paths. No locator, matching, normalization, measurement, automatic ROI, automatic run, or LLM feature was added.
- The full Debug build passed with zero warnings/errors. Focused current-build smokes passed for the designer, legacy Fixture reference teach, selected-Step PropertyGrid handoff, and Fixture PropertyGrid round trip. Tab selection preserved Preview/Run count, layers, active layer, and routing.
- Status: Complete for this bounded UI-consolidation slice. Evidence: `artifacts\p212_fixture_relative_roi_designer_20260723`.
- Historical next priority at P212: obtain approval for P213. Superseded by the completed P213 record below.

## 2026-07-23 P213 General Geometric Measurement Workspace

- Static source review confirmed that existing `LineDistance` and `LineIntersection` execute their own Line A/B gauges and expose metrics/overlays, but no earlier Step publishes a typed feature that a later Step can reference by name.
- The proposal defines one additive same-run `Point`/`Segment`/`Circle` result sidecar addressed by `SourceStep + FeatureName`, exact coordinate/provenance checks, existing-Line export, a radial-caliper `CircleGauge`, seven pixel-only `GeometryMeasure` modes, PropertyGrid teaching, Pipeline Review table/drawing selection, saved-report persistence, and named fail-closed reasons.
- Legacy Line/LineDistance/LineIntersection behavior remains unchanged. Experimental OuterCornerIntersection, calibration/mm, automatic feature/ROI selection, LLM expansion, a new graph engine, and dataset tuning remain outside the contract.
- Contract state: Complete on 2026-07-23. Existing Line typed export, radial CircleGauge, all seven GeometryMeasure modes, compatible PropertyGrid source selection, Geometry Review two-way selection, and direct/recipe report persistence passed the frozen gates. Full/focused builds and legacy Line/LineDistance/LineIntersection regressions remained green.
- Evidence: `artifacts\p213_general_geometric_measurement_workspace_20260723`. Boundary: pixel-only synthetic algorithm/UI evidence, not calibration, industrial semantics, unseen robustness, field qualification, automatic feature selection, or OuterCornerIntersection correctness.
- Next priority: bounded two-point scale teaching with source/hash, two reviewed points, known distance/unit, derived scale, and explicit apply. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-23 P214 Two-Point Scale Teaching

- Contract: `docs\OPENVISIONLAB_TWO_POINT_SCALE_TEACHING_CONTRACT.md`. Status: Complete.
- Pipeline Review now accepts two distinct P213 Point results from the same explicit Run/coordinate frame, draws their A/B evidence and pixel distance, converts an operator-entered mm/µm/inch distance to one uniform mm/px value, and saves exact point coordinates plus coordinate-layer image SHA-256 before any recipe mutation.
- Explicit Apply targets exactly one compatible measurement Step. It writes the legacy-compatible `PIXELPERMM` value (runtime semantics mm per pixel) and the existing Left/Right keys for LineDistance, round-trips the pipeline and applied-Step audit, and does not invoke Preview/Run or change layers/routing.
- Same identity/coincident points, cross-frame/dimension mismatch, missing or changed image content, invalid distance, incompatible target, and target input-layer mismatch fail closed. Positive scale also enables P213 geometry-distance/clearance and circle-radius/diameter mm metrics.
- Current Debug/focused builds, numeric/unit conversions, negative gates, evidence/pipeline round trips, actual Pipeline Review Run/calculate/apply isolation, fresh UI capture, and P213 geometry plus legacy Line-family regressions passed. Evidence: `artifacts\p214_two_point_scale_teaching_20260723`.
- Boundary: uniform image-plane scale only; no distortion/perspective/non-uniform correction, physical-standard verification, uncertainty, certified metrology, or field qualification.
- Next priority: statically reassess remaining commercial UI/tool gaps after P211-P214 and select at most one bounded deterministic workbench slice from fresh evidence. Do not resume image campaigns or LLM development by default. Recommended model: gpt-5.6-sol | Reasoning effort: medium.

## 2026-07-23 P215 Post-P214 Commercial UI/Tool Gap Reassessment

- Completed a documentation/source-only reassessment. No image, Preview, Run, batch, recipe tuning, or LLM workflow was executed.
- The current catalog contains 23 canonical tool families and 42 accepted names/aliases. P211 already persists Blob/Contour area, center, bounding rectangle, angle, state, reject reason, and table/drawing selection.
- Current `BlobProperty`/`ContourProperty`, Pipeline property mapper/builder/factory, and per-object acceptance logic still use only `MIN_AREA` and `MAX_AREA`. Width and height are visible evidence, and aggregate Step gates such as `BoundsWidthMax` can reject a whole run, but they cannot filter the accepted object set or its `ResultCount`.
- Official MERLIC 5.8 `Evaluate Regions` confirms the commercial pattern of evaluating accepted/rejected regions by explicit features including width and height. OpenVisionLab will adopt only the smallest proven subset rather than its full descriptor catalog.
- Selected next slice: optional Blob/Contour bounding-width/height min/max gates, backward-compatible missing-key behavior, exact P211 reject reasons, and report persistence. Verify with deterministic synthetic shapes and fresh UI evidence; do not reopen an operator dataset.
- Deferred: angle/aspect/circularity/holes/gray features, easyTouch-style automatic suggestions, navigation rewrites, OCR/barcode, general region algebra, graph engines, image campaigns, and LLM work.
- Status: Complete for audit/selection only. Audit: `docs\OPENVISIONLAB_RULE_BASED_UI_GAP_AUDIT_20260723.md`.
- Next priority: implement the bounded object dimension filter v1 contract. Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-23 P216 Blob/Contour Object Dimension Filters v1

- Added optional `MIN_WIDTH`, `MAX_WIDTH`, `MIN_HEIGHT`, and `MAX_HEIGHT` axis-aligned pixel bounds to Blob/Contour PropertyGrid and Pipeline/XML round trip. Missing keys preserve legacy `0..1000000` unbounded dimensions.
- Deterministic runtime removes dimension-rejected objects before `ResultCount`, object metrics, bounds metrics, accepted drawings, and acceptance. P211 Object Results Inspector and saved Run History retain the rejected rows and exact failed gate.
- A five-shape synthetic contract passed for Blob and Contour: one accepted, four exact width/height rejects. Removing the new keys restored all five accepted objects. Actual report persistence retained all four reasons.
- Fresh current-build Blob, Contour, dimension-PropertyGrid, and existing Pipeline Review UI smokes passed. PropertyGrid search/docking did not invoke Preview/Run.
- Full evidence: `artifacts\p216_object_dimension_filters_20260723`.
- Status: Complete for the bounded axis-aligned pixel feature. It is not rotated-size, aspect/circularity, semantic classification, operator-dataset, calibration, industrial robustness, or field qualification evidence.
- Next priority: one static post-P216 deterministic-workflow reassessment that selects at most one concrete operator gap or closes further feature expansion. Do not run an image campaign. Recommended model: gpt-5.6-terra | Reasoning effort: low.

## 2026-07-23 P217 Post-P216 Deterministic Workflow Reassessment

- Completed a current-source/document-only review of PropertyGrid teaching, selected-Step edit handoff, explicit Run Review, Pipeline Review evidence, recipe persistence, saved Run Reports, batch history, and the deterministic review queue.
- The connected review surfaces now include P211 Object Results, P212 Fixture/relative ROI, P213 Geometry Review, P214 Scale Calibration, and P216 Blob/Contour dimension-filter evidence. Recipe storage has an explicit round-trip validator, and saved review evidence is opened read-only rather than silently rerunning Preview or Run.
- The P210 commercial shortlist is complete. The remaining descriptors, automatic suggestions, navigation changes, OCR/barcode, region algebra, and new algorithm families do not have a named operator task plus a reproducible current blocker.
- Decision: select no further feature slice and close proactive expansion. No product code, UI, image, Preview/Run, batch, recipe-tuning, or LLM provider work was performed.
- Status: Complete. Evidence: `artifacts\p217_post_p216_workflow_reassessment_20260723` and `docs\OPENVISIONLAB_RULE_BASED_UI_GAP_AUDIT_20260723.md`.
- Next trigger: a concrete operator-blocking workflow or verified regression reproduced on current source. Prerequisite first; no model work is recommended until that evidence exists.

## 2026-07-23 P218 Affine Transform v1

- The user supplied the named post-P217 need: add deterministic Affine matrix support to Library-Noah, build its DLL, consume it from OpenVisionLab, expose a Tool View, and add Learn guidance.
- The bounded v1 is three ordered non-collinear source points to three destination points in pixel coordinates. Library-Noah owns validation, `GetAffineTransform`/`WarpAffine`, coverage, matrix/decomposition/triangle metrics, error codes, and ten destination/frame drawings. Homography, calibration, automatic correspondence, and mm are excluded.
- Library-Noah file version is `2.8.0.0`; assembly ABI remains `2.1.0.0` so OpenVisionLab can retain its existing `Lib.Common`/`Lib.OpenCV.Blob` 2.1 runtime. Source, vendored, and build-output `Lib.OpenCV.dll` hashes are identical at `B128CA282C0CD02C36F5CCF0C78C69C6F4834C3376158E8667EEAA7DE494A08B`.
- OpenVisionLab adds the Affine PropertyGrid, explicit Preview, result card, canonical/alias XML round trip, strict validation, public sample, and Geometry Learn. The Tool View hides unrelated inherited fields and preserves input/output routing.
- The public known transform recovered `[0.9 0.1 20; 0.05 0.9 10]`, determinant `0.805`, valid pixels `0.805`, output `572x420`, and ten drawings. Canonical/alias, mapper round trip, collinear and coverage fail-closed gates passed.
- Final verification passed Library-Noah build and 57/57 smoke, OpenVisionLab zero-warning build, focused Affine contract/public-sample/current-source UI smokes, readiness/public-sample checks, and RotateScale regression.
- Status: Complete. Evidence: `artifacts\p218_affine_transform_v1_20260723` and `docs\OPENVISIONLAB_AFFINE_TRANSFORM_V1_CONTRACT.md`.
- Next trigger: the operator must provide three same-order physical point pairs, the downstream reference ROI/inspection, representative samples, and acceptance criteria for a real use case. No model work is recommended before those inputs exist.

## 2026-07-23 P219 Detected-Point Affine Fixture

- Audited the current runtime and confirmed that P218 accepted fixed numeric source points only; the one-Matching `NormalizeImage` path was similarity-only and could not consume three independent detections.
- Reused the P213 typed geometry result contract instead of adding another transform algorithm. Successful single-result Matching now publishes `Center`; the Affine selected-Step PropertyGrid lists compatible earlier Point outputs and persists three ordered references.
- At explicit Run, OpenVisionLab verifies that all three source Steps are earlier, enabled, successful, accepted, distinct, finite, inside the image, and in the same input layer/dimensions. It injects their coordinates into a cloned runtime Step; saved XML is unchanged. Library-Noah still owns the actual Affine calculation, WarpAffine, gates, metrics, errors, and drawings.
- The actual six-Step representative pipeline `Matching x3 -> AffineTransform -> Threshold -> fixed-ROI Blob` passed. All three Center Points matched the known source coordinates, the matrix matched an independent `GetAffineTransform`, provenance metrics retained all six coordinates, and unchanged `CvROI=170,120,70,60` returned `ResultCount=1`.
- Duplicate source references failed both strict definition validation and direct execution. Fixed numeric P218 XML still passes its separate regression contract.
- Fresh Recipe Manager before/after evidence shows the toggle and three Point pickers; PropertyGrid apply/XML round trip kept Preview/Run count zero.
- Status: Complete. Evidence: `artifacts\p219_dynamic_affine_fixture_20260723` and `docs\OPENVISIONLAB_AFFINE_DETECTED_POINT_FIXTURE_CONTRACT.md`.
- Boundary: deterministic same-run correspondence wiring only; not automatic correspondence, locator qualification, per-image ROI movement, homography, calibration, unseen robustness, or field qualification.
- Next trigger: one real operator-selected physical three-point fixture plus destination coordinates, downstream reference ROI/inspection, representative samples, and acceptance criteria. Recommended model: none until operator teaching/data exists | Reasoning effort: none until operator teaching/data exists.

## 2026-07-23 P220 Operator-Approved Card Affine Pilot

- The operator approved `R`, `5`, and the lower expiry mark as three distinct non-collinear physical card features before Matching was executed.
- The evidence harness froze six evenly spaced OK and six evenly spaced NG rows, all source/template/XML hashes, Matching score/angle/scale settings, and an independent normalized-output gate of minimum template score `>=0.65` plus maximum center residual `<=3 px`.
- The first run passed 8/12. Current-run drawings proved the original `5` coarse search ROI clipped two real positions and the original `R` region admitted the visually similar left `P`.
- One geometry-only r2 correction excluded `P` and widened the `5` search region. No score, angle, scale, Affine, or residual gate changed.
- r2 produced three typed Points and Affine output on 12/12. Ten rows retained `0..2 px` maximum residual; `OK_0051` retained `5.00 px` and `NG_0150` retained `4.12 px`.
- Status: Incomplete at `<=3 px`. Evidence: `artifacts\p220_affine_fixture_point_candidates_20260723` and `docs\OPENVISIONLAB_CARD_AFFINE_PILOT_20260723.md`.
- Next trigger: the operator supplies the downstream fixed ROI/inspection and maximum safe registration error. Do not lower the gate, run all 500, switch features, or add Homography before that requirement exists.

## 2026-07-23 P221 Card Affine Fixed-ROI Linkage

- The operator accepted the current card registration for one coarse downstream ROI. This separate decision uses the observed `<=5 px` envelope and does not rewrite P220's failed `<=3 px` gate.
- The unchanged Matching x3 -> Library-Noah Affine path feeds one existing unjudged Mean Step on `CardReference` with exact `CvROI=250,315,190,80`.
- The same frozen 12 rows completed 12/12. XML retained the exact input/ROI, all rows published finite `MeanValueAvg=111.4..170.1`, normalized score stayed `>=0.836786`, maximum residual was `5.00 px`, and all runtime ROI drawings stayed over the intended `10/05` area.
- Status: Complete for bounded fixed-coordinate downstream linkage. Evidence: `artifacts\p221_card_affine_fixed_roi_20260723_r2` and `docs\OPENVISIONLAB_CARD_AFFINE_FIXED_ROI_20260723.md`.
- Boundary: no OK/NG classification, Mean tolerance, unseen-data robustness, Homography, or production-locator qualification is claimed.
- Next trigger: operator-selected actual inspection target/ROI, existing rule-based tool, and Good/NG tolerance. Recommended model: none until operator teaching/tolerance exists | Reasoning effort: none until operator teaching/tolerance exists.

## 2026-07-24 P222 Auto MPoint Library Core

- The operator explicitly requested an automatic matching-point teaching tool in
  Library-Noah and approved a one-reference-image, fixed-pattern-size V1 before
  implementation.
- Added `AutoMPointToolProperty`, `AutoMPointCandidateResult`, stable error codes,
  and `AutoMPointTool`. The tool scans the full image or one analysis ROI, ranks
  deterministic contrast/edge-distribution windows, suppresses overlap, and uses
  the existing `EdgeBasedTemplateMatchingTool` only on the strongest finalists.
- Exact gates cover self-location, strongest distant alternative, uniqueness,
  three known synthetic transforms, position/angle/scale error, and optional
  runtime. Results retain the authored ROI-center MPoint, native edge-model center
  and offset, exact reject reasons, metrics, drawings, and overlays.
- The tool is deliberately not registered as a Pipeline Step. It does not save a
  template, mutate a recipe, change layers/routing, Preview, or Run.
- Library-Noah Release build passed with zero warnings/errors and the complete
  smoke runner passed 60/60. A unique asymmetric feature ranked first at
  `64,64,64,64`; two identical patterns both failed the uniqueness gate; invalid
  ROI/pattern definitions failed closed; repeat executions retained identical
  ranking and drawing pixels.
- Source-library `Lib.OpenCV.dll` is assembly `2.1.0.0`, file `2.8.0.0`, SHA-256
  `3D7A0B5D392B096DB3C14091D08E52BBB840772C1BDD1B30BEB15475ABAE28D9`.
  OpenVisionLab Dev had not consumed that build at the P222 checkpoint; P223 below
  supersedes this integration state.
- Status: Complete for the Library-Noah core. Evidence:
  `C:\Git\Library-Noah\artifacts\auto_mpoint_v1_20260724`,
  `C:\Git\Library-Noah\docs\AUTO_MPOINT_V1.md`, and
  `docs\OPENVISIONLAB_AUTO_MPOINT_V1_CONTRACT.md`.
- Boundary: OpenVisionLab PropertyGrid/DLL integration, operator-confirmed real
  samples, automatic size selection, N-image qualification, and field robustness
  remain pending.
- Next priority: integrate this core into an explicit OpenVisionLab teaching UI
  with `Analyze candidates` and `Use this pattern`, candidate rows/drawing, current
  DLL provenance, and zero automatic Preview/Run or routing side effects.
  Recommended model: GPT-5.3-Codex | Reasoning effort: medium.

## 2026-07-24 P223 Auto MPoint Teaching UI And Direction Review

- Integrated the P222 Library-Noah core into the existing Edge Based Matching
  Tool View. It remains teaching-time assistance rather than a Pipeline Step.
- Added Auto MPoint PropertyGrid settings, explicit `Analyze candidates`, a
  `Suggested` candidate list, the Library-Noah drawing, and explicit
  `Use this pattern` through the existing template save path.
- Property edits, analysis, row selection, and apply preserve Preview/Run count,
  input/output layers, active layer, and routes. Matching still requires an
  explicit Preview after a template is applied.
- Source, vendored, and current Debug `Lib.OpenCV.dll` are assembly `2.1.0.0`,
  file `2.8.0.0`, SHA-256
  `3D7A0B5D392B096DB3C14091D08E52BBB840772C1BDD1B30BEB15475ABAE28D9`.
- Reviewed the operator-provided GPT Pro investigation against current source and
  official HALCON/Cognex documentation. The existing matcher remains the base.
  Current ambiguity values are diagnostic only; current subpixel refinement is
  independent X/Y five-score interpolation; model reduction is sequential; and
  hybrid selection diagnostics are not exposed by `MatchingResult`.
- Adopted order: opt-in fail-closed unique-match contract, then a frozen fixed-ROI
  repeatability/false-accept matrix, then translation-only joint refinement if
  measured error requires it. Balanced real-valued refinement points follow only
  with evidence. Adaptive window growth, ODB/CAD, global anchors, Homography, and
  production auto-tuning are deferred.
- Status: Complete for the bounded UI integration and direction review. Evidence:
  `artifacts\p223_auto_mpoint_ui_20260724` and
  `docs\OPENVISIONLAB_AUTO_MPOINT_V1_CONTRACT.md`.
- Boundary: suggestion, explicit apply, and one-image synthetic evidence do not
  prove runtime uniqueness, subpixel production accuracy, N-image robustness, or
  commercial-library parity.
- Next priority: add the optional unique-match contract to Library-Noah while
  preserving absent-key legacy behavior. Recommended model: GPT-5.3-Codex |
  Reasoning effort: high.

## 2026-07-24 P224 Edge-Based Unique Match Runtime

- Added an opt-in fail-closed unique-result contract to the existing
  Library-Noah `EdgeBasedTemplateMatchingTool`; legacy missing-key execution
  remains unchanged.
- Enabled mode requires `NUM_MATCH=1`, `USE_MULTI_ROI=false`, and a finite
  normalized margin. At least eight internal candidates are retained independently
  of the external one-result request.
- Exactly one accepted candidate returns `Success`. No candidate returns
  `MatchingNoResult`; a spatially distinct plausible alternative inside the
  failed margin returns `MatchingAmbiguous`. Both failures return zero
  `MatchingResult` rows.
- Normalized `UniqueMatch.*` metrics retain state, internal Top-K, alternative
  count, selected/alternative scores, actual/required margin, and spatial
  threshold. Successful result rows also retain edge/image/final score and
  percentage-point margin evidence; legacy rows leave that margin unavailable.
- OpenVisionLab adds the two PropertyGrid/XML fields, absent-key defaults,
  fail-closed pipeline validation, known metrics, exact diagnostics, and
  no-auto-Preview behavior. The current-source capture visibly shows the enabled
  option and `0.07` margin.
- Library-Noah Release build and full smoke passed 64/64. The focused matrix
  proves legacy repeated-pattern success, distinct unique success,
  repeated-pattern ambiguity rejection, and absent-pattern no-match rejection.
- Source, vendored, and current Debug `Lib.OpenCV.dll` are assembly `2.1.0.0`,
  file `2.8.0.0`, SHA-256
  `000C75A7D0E796E166DF6F24C95F264FC001927881B1ED7DE7BAE31913099F6D`.
- Status: Complete for the bounded runtime/XML/UI contract. Evidence:
  `artifacts\p224_unique_match_runtime_20260724` and
  `docs\OPENVISIONLAB_EDGE_BASED_UNIQUE_MATCH_V1_CONTRACT.md`.
- Boundary: no physical anchor, ROI, default margin, pose accuracy,
  repeatability, false-accept rate, unseen variation, or field robustness is
  qualified.
- Next priority: frozen fixed-ROI repeatability/false-accept evidence only after
  the operator supplies the anchor/ROI, motion envelope, representative images,
  and allowable pose-error gate. Recommended model: none until those inputs are
  frozen | Reasoning effort: none until those inputs are frozen.

## 2026-07-24 P225 Card R Fixed-ROI Edge Matching Matrix

- Reused the operator-approved P220/P221 `R` template and exact 12-row
  `card_original` hashes. Frozen settings were score `0.45`, unique margin
  `0.03`, angle `-8..8°`, scale `0.9..1.1`, and prior-center error `<=5 px`.
- Executed reviewed-ROI unique, original broad-ROI legacy, and original
  broad-ROI unique modes without post-result tuning.
- Reviewed-ROI unique produced `0/12` correct accepts, two baseline mismatches,
  two ambiguity rejects, and eight no-match rejects. Broad legacy produced one
  correct accept and two wrong accepts; broad unique produced no correct accepts
  and retained those two wrong accepts.
- Opened the exact current-run and comparison drawings. One reviewed-ROI result
  selected `T` instead of `R` with score `74.237` and no plausible alternative.
  A uniqueness margin therefore does not prove intended physical identity.
- Closed two Pipeline handoff gaps exposed by the matrix: existing EdgeBased
  scale/subpixel/pyramid settings now survive builder/factory execution, and one
  successful EdgeBased result publishes typed `Center`.
- Focused runner build passed 0/0. The final 36-row replay completed with zero
  infrastructure/runtime errors and frozen hash/XML/drawing/CSV evidence.
- Status: Complete audit; fixed candidate decision `Reject`. Evidence:
  `artifacts\p225_edge_unique_card_r_matrix_20260724`.
- Boundary: prior P220/P221 centers are reviewed baselines, not independent
  metrology ground truth. Do not lower gates, retune `R`, run a larger set, or
  begin joint pose refinement.
- Next priority: a second matrix is blocked until the operator explicitly
  approves one different Auto MPoint candidate as the same durable physical
  feature across representative images. Recommended model: none until operator
  candidate approval | Reasoning effort: none until operator candidate approval.

## 2026-07-24 P226 Public EasyMatch Auto MPoint Candidate Presentation

- Ran the existing Library-Noah Auto MPoint engine once on `BOARD.JPG`,
  `Die Pad 1.bmp`, `Floppies.jpg`, `Frame 1.tif`, and `Switch1.tif` under
  `Sample\EasyMatch`.
- Froze the current UI-equivalent defaults before observing results: `96x96`,
  stride `16`, eight exact finalists, top-five display, feature quality `0.15`,
  matching score `0.75`, uniqueness `0.05`, and maximum synthetic position
  error `2.5 px`. No result-dependent tuning occurred.
- Retained five current-run drawings, 40 evaluated rows, 28 gate-passed
  candidates, 20 displayed suggestions/crops, and exact source/evidence hashes.
  Four of five images produced suggestions.
- `Frame 1.tif` rejected all finalists on uniqueness
  (`0.0011..0.0054 < 0.05`). `Floppies.jpg` still suggested repeated hubs
  because their fixed orientations were numerically distinct while angle search
  was off. This is direct drawing evidence that automatic score and uniqueness
  cannot supply physical-feature semantics.
- No suggestion was applied and no cross-image Matching/Affine/inspection run
  occurred.
- Status: Complete for candidate presentation. Evidence:
  `artifacts\p226_auto_mpoint_easymatch_candidates_20260724_r2`.
- Next priority: operator names one sample and displayed rank/ROI and confirms
  the same durable physical feature across representative images. Only then
  freeze a cross-image Matching qualification. Recommended model: none until
  operator candidate approval | Reasoning effort: none until operator candidate
  approval.

## 2026-07-24 P227 Six-Corpus Auto MPoint Pilot And Report

- Audited six operator-provided EasyMatch 500-image packages as 3,000 unique
  `all_images` rows and 16 separate `source_file` template strata.
- Froze one canonical OK teaching image plus four OK/four NG MD5-spread pilot
  rows per stratum. No parameter was changed after observing candidate results.
- The initial product-path replay correctly failed the task rather than
  mislabelling candidate quality: 88/104 rows had no usable template edges.
  Root cause was a contract mismatch between Auto MPoint's grayscale verifier
  and `EdgeBasedMatchingProperty`'s inherited threshold-on default.
- New EdgeBasedMatching and explicit Auto MPoint application now default to
  grayscale-edge matching. Explicit legacy `USE_THRESHOLD=true` remains
  preserved. The current PropertyGrid/apply/Pipeline UI smoke passed with zero
  automatic Preview/Run or routing/layer mutation.
- The corrected replay produced suggestions for 13/16 strata, executed 104
  rows with zero runtime/integrity errors, and mechanically advanced 12 strata.
  All 13 contact sheets were opened.
- Drawing review stopped Frame 1/2/3, Frame 4, Die1, and Die2. Ten strata remain
  bounded expansion candidates only.
- Status: Complete for the pilot/report/contract repair. Evidence:
  `artifacts\p227_auto_mpoint_six_corpus_pilot_20260724_r4`.
- Primary review document:
  `artifacts\p227_auto_mpoint_six_corpus_pilot_20260724_r4\OPENVISIONLAB_AUTO_MPOINT_SIX_CORPUS_REPORT.md`.
- Next priority: the operator reviews that report and names one expansion
  candidate before a 500-row qualification. Recommended model: none until
  operator candidate approval | Reasoning effort: none until operator candidate
  approval.

## 2026-07-24 P228 Self-Contained HTML Report Export

- Extended the P227 command with a dependency-free, self-contained HTML export.
  The candidate overview and all 13 available Matching contact sheets are
  embedded in the HTML; the report also retains the full table, Korean review
  reasons, companion CSV links, and a browser `인쇄 / PDF 저장` action.
- A fresh 104-row replay reproduced the P227 logical results exactly: 3,000
  metadata rows, 16 strata, 13 suggestions, zero runtime/integrity errors, ten
  expansion candidates, and six stopped strata.
- Structural verification found 14 embedded images and zero missing companion
  links. A current Chrome render was opened and visually checked.
- Status: Complete for report export and rendering. Evidence:
  `artifacts\p227_auto_mpoint_six_corpus_pilot_20260724_r5_html`.
- Primary review document:
  `artifacts\p227_auto_mpoint_six_corpus_pilot_20260724_r5_html\OPENVISIONLAB_AUTO_MPOINT_SIX_CORPUS_REPORT.html`.
- Boundary: this is presentation/export evidence only and does not qualify a
  locator.
- Next priority: the operator reviews the HTML report and names one expansion
  candidate before a 500-row qualification. Recommended model: none until
  operator candidate approval | Reasoning effort: none until operator candidate
  approval.

## 2026-07-24 P229 Representative-Image Automatic Best Pattern

- Added an optional Library-Noah AutoMPoint overload that evaluates one-image
  finalists across multiple same-size representative images. Missing
  representative images preserve the legacy one-image behavior.
- Multi-image candidates are rejected below the configured success rate and ranked
  by representative success rate, minimum uniqueness margin, mean score, then
  original one-image score. Per-image success/no-match/ambiguous, score,
  uniqueness, pose, runtime, and aggregate evidence are retained.
- OpenVisionLab's existing Edge Based Matching teaching panel now selects
  representative files, shows their count, marks rank one as `BEST`, and selects
  it without automatically applying it, Previewing, Running, or mutating
  layer/routing state.
- A deterministic `Die Pad 1.bmp` pilot automatically selected
  `128,256,96,96`, the same ROI retained by P227. Four OK plus four NG
  representative rows and a disjoint four OK plus four NG held-out set both
  replayed at `8/8`; runtime and hash-integrity errors were zero. All drawings
  follow the same central pad/trace.
- The first execution failed closed with no candidate because the command omitted
  the existing angle/scale envelope and clustered its sample selection. The one
  bounded correction restored angle `-8..8`, scale `0.9..1.1`, and deterministic
  spread rows without lowering score, uniqueness, or success gates.
- Library-Noah Release build and 66/66 smoke passed. Source/vendored/current Debug
  DLL SHA-256:
  `B456BE7AFC002BA1535A5892092B746FB44560300961BD71342AAC0E7741B180`.
- Status: Complete for bounded automatic selection/UI/split replay. Evidence:
  `C:\Git\Library-Noah\artifacts\auto_mpoint_representative_v2_20260724` and
  `artifacts\p229_auto_mpoint_representative_best_20260724`.
- Primary review document:
  `artifacts\p229_auto_mpoint_representative_best_20260724\die_pad_1_r3_current\OPENVISIONLAB_AUTO_MPOINT_REPRESENTATIVE_BEST_REPORT.html`.
- Boundary: one synthetic/augmented same-source stratum only; no semantic,
  all-500, real-capture, production, or field qualification.
- Next priority: the operator reviews the P229 representative and held-out
  drawings and confirms that the selected central pad/trace is one durable
  feature before any 500-row replay. Recommended model: none until operator
  drawing approval | Reasoning effort: none until operator drawing approval.

## 2026-07-24 P230 Die Pad 1 Full-Stratum Qualification

- After operator drawing approval, froze the exact P229 template ROI
  `128,256,96,96` and replayed the unchanged matcher contract on all 122
  `Die Pad 1.bmp` rows. Auto MPoint was not rerun and no parameter was tuned.
- Results: 122/122 success, including 62/62 OK and 60/60 NG; ambiguous 0,
  no-match 0, runtime errors 0, integrity errors 0, drawings 122/122.
- Opened the deterministic 35-row decision queue and all nine supplied-defect-mask
  overlap drawings. Every runtime result remained on the same central pad/trace;
  no wrong-location result was observed.
- Nine NG masks intersect the 96x96 template bounds. They remain an explicit
  production-variation risk, so the decision is `Keep with documented limits`.
- The first report incorrectly treated mask intersection as a wrong-location
  failure. Corrected report semantics only; source, template, parameters,
  drawings, scores, and runtime outcomes were unchanged.
- Status: Complete for this source stratum. Primary report:
  `artifacts\p230_auto_mpoint_die_pad_1_full_stratum_20260724_r2\OPENVISIONLAB_AUTO_MPOINT_FULL_STRATUM_REPORT.html`.
- Boundary: Die Pad 2-4 are distinct source strata and this result is not
  real-capture, production, or field qualification. Do not rerun or retune the
  same 122 rows.
- Next priority: audit whether representative selection and the P230 N-image HTML
  evidence export are exposed through the actual operator UI; connect the existing
  contracts only if that product path is missing. Recommended model:
  gpt-5.6-terra | Reasoning effort: medium.

## 2026-07-24 P231 Product-UI Auto MPoint N-Image Report

- Audited the Edge Based Matching Tool View and confirmed that representative
  selection/ranking/apply existed while HTML evidence export was validation-tool
  only.
- Added one explicit `Save N-image report` action after representative analysis.
  It serializes the selected candidate's retained results rather than rerunning
  matching.
- The self-contained HTML retains source/template identity, every N-row metric and
  file SHA-256, all failures, bounded score/uniqueness/runtime/angle/scale
  extremes, SHA-256-spread review drawings, and every drawing when N <= 24.
- Changed source/settings/representative file identity and missing/count-mismatched
  evidence fail closed. Export does not apply a candidate, Preview/Run, or mutate
  layers/routing.
- Current-source UI/report smoke passed: 3 rows, 4 embedded PNGs, zero external
  image links, visible report button, and unchanged workspace state. Evidence:
  `artifacts\p231_auto_mpoint_operator_html_report_20260724\after_current_build_r3`.
- Boundary: locator-teaching evidence only; Recipe Manager Validation Set and Run
  History remain the OK/NG classification/report path.
- Status: Complete. Do not expand AutoMPoint or start another image campaign until
  the operator supplies a named workflow blocker or a current-build regression.
  Recommended model: none until evidence exists | Reasoning effort: none until
  evidence exists.

## 2026-07-24 P232 Tool View N-Image Verification Design

- Audited the current Tool View, Recipe Manager, Pipeline execution, batch
  storage, per-image report, and Run History paths without changing product
  source or running another dataset.
- Recipe Manager Local Validation Sets already support multi-file/top-level-folder
  registration with explicit OK/NG roles and a 5,000-image limit. Their explicit
  suite run saves per-image Step reports/drawings, `summary.xml`, `summary.tsv`,
  analytics, comparison, and a deterministic review queue.
- The native common Tool View shell remains a one-image teaching surface with
  explicit Preview and Add Pipeline. AutoMPoint P231 is a specialized
  representative-image report only.
- Current local-set/pair/catalog implementations await each image in an ordered
  loop. This proves N-image batch execution, not simultaneous parallel execution.
- Froze a minimal shared design: eligible Tool Views expose one `N장 검증` entry
  that opens one shared window; the current PropertyGrid state is committed
  through the existing Add Pipeline Step adapter into a frozen transient
  one-Step Pipeline; retained results drive the table, drawings, deterministic
  review queue, and self-contained HTML without rerun.
- Thirteen single-input native Tool Views are eligible in Phase 1. Arithmetic is
  deferred until A/B file pairing is explicit; HSV/Histogram need native Step
  contracts; AutoMPoint stays separate; Pipeline-only tool families stay in
  Recipe Manager.
- Status: Complete as design/audit only. Design:
  `docs\OPENVISIONLAB_TOOL_VIEW_N_IMAGE_VERIFICATION_DESIGN.md`.
- Next priority after operator acceptance: implement the shared sequential Phase
  1 surface and prove result equivalence against Recipe Manager on Threshold,
  Blob, Line, and Edge Based Matching. Recommended model: gpt-5.6-terra |
  Reasoning effort: medium.
- Later priority: bounded `1/2/4` workers only after isolated-worker,
  thread-safety, memory, cancellation, and exact sequential-equivalence evidence.
  Recommended model: gpt-5.6-sol | Reasoning effort: high.

## 2026-07-24 P233 Shared Tool View N-Image Verification Phase 1

- Implemented one shared `N-image verification` action for the thirteen current
  single-input native Tool Views that already have a one-Step Pipeline adapter.
  Arithmetic, HSV, Histogram, AutoMPoint, and Pipeline-only families retain
  their separate or deferred contracts.
- The modal window accepts selected files or one top-level folder up to 5,000
  images. Explicit Run creates and hashes the current Step exactly once, freezes
  the ordered image list, and runs a transient `Main -> NImageResult` Pipeline
  sequentially with stop-after-current behavior.
- Native Tool View channel normalization is preserved for execution, while each
  original source snapshot is retained and SHA-256 verified. Per-image run
  reports, drawings, metrics, messages, and times feed the XML/TSV summary,
  Pipeline snapshot, deterministic review queue, and retained-only
  self-contained HTML.
- Threshold, Blob, Line, Matching, EdgeBasedMatching, and AffineTransform each
  passed 30/30 deterministic rows. Direct frozen-Pipeline replay matched every
  retained status and published metric; HTML export left all run-report
  timestamps unchanged.
- Browser rendering verified all 30 EdgeBasedMatching rows, six embedded review
  images, and no page-level horizontal overflow after long SHA-256 cards wrap.
- Current-build UI smoke verified the common entry, the actual retained-result
  table/source/drawing window, and unchanged Preview/Run count, layers, active
  layer, and routes after open/close.
- Status: Complete for Phase 1. Evidence:
  `artifacts\p233_tool_view_n_image_verification_20260724`.
- Boundary: this is sequential quick execution verification, not automatic
  expected OK/NG labelling, accuracy qualification, parallel execution, or
  field robustness.
- Next priority: wait for one concrete operator workflow blocker or regression
  from real use before changing this surface. Recommended model: none until
  evidence exists | Reasoning effort: none until evidence exists.
- If explicitly requested after that review, the next bounded feature is exact
  frozen-Step promotion into Recipe Manager Validation Set. Recommended model:
  gpt-5.6-terra | Reasoning effort: medium.

## 2026-07-24 P234 First P233 Real-Folder Acceptance

- Ran the exact frozen P230 `Die Pad 1` EdgeBasedMatching Step through P233's
  actual top-level-folder registration and shared N-image service. No locator
  parameters or product runtime were changed.
- The task-local folder contains a deterministic MD5-spread 12 OK + 12 NG copy
  from the operator-supplied `EasyMatch_Die_Pad_500(1)` corpus.
- Folder registration passed 24/24, the Step factory ran exactly once,
  execution passed 24/24, and drawings were retained 24/24. The Step SHA-256 is
  `7CEAEC5D50259ED1337AB912F0F0A63C673F4A74E692DCDEA01BAA14FC25658F`.
- Retained source files passed SHA-256 and decoded-pixel equality checks.
  `ScoreMax` reproduced P230 within at most `0.068` percentage points under the
  frozen `<=0.1` integration-equivalence gate.
- The minimum-score row (`die_pad_240_ok.jpg`, `83.76%`) and maximum-delta row
  (`die_pad_089_ng.jpg`, `-0.068` percentage points) were opened. Both exact
  runtime drawings remained on the approved central pad/trace feature.
- Status: Complete. Evidence:
  `artifacts\p234_tool_n_image_real_folder_acceptance_20260724`.
  Primary HTML:
  `artifacts\p234_tool_n_image_real_folder_acceptance_20260724\P234_DIE_PAD_REAL_FOLDER_REPORT.html`.
- Boundary: this is one frozen locator's folder-to-report integration
  acceptance. It does not infer OK/NG accuracy, qualify other strata, retune the
  locator, prove parallelism, or establish field robustness.
- Historical next action: exact frozen-Step/image-set promotion into Recipe
  Manager was explicitly requested and is completed by P235 below.

## 2026-07-24 P235 Hash-Locked Locator Validation Promotion

- Implemented the operator-requested handoff from a completed all-success
  `Matching`, `EdgeBasedMatching`, or `FeatureMatching` Tool View N-image
  session to Recipe Manager through one explicit `Promote locator set` action.
- Promotion saves the exact one-Step Pipeline text/name and SHA-256, all
  readable dependency/template hashes, each ordered original image hash, and
  an image-set hash. The retained report source hash and decoded pixels must
  still match the current original before promotion.
- All promoted rows are `Expected OK` for locator execution only. Original
  corpus defect OK/NG roles are neither copied nor inferred.
- Hash-locked sets are idempotent and row-read-only. A different selected
  Pipeline is disabled, and Pipeline/dependency/image drift fails before image
  execution. Promotion does not activate the Pipeline or start Preview/Run.
- The exact P234 Step hash
  `7CEAEC5D50259ED1337AB912F0F0A63C673F4A74E692DCDEA01BAA14FC25658F`
  and 24 retained images passed promotion/reload, repeat reuse,
  wrong-Pipeline blocking, tamper rejection, current-source Recipe Manager UI,
  and zero automatic run checks. Legacy manual OK/NG Validation Set and N-image
  entry/window regressions passed.
- Status: Complete. Evidence:
  `artifacts\p235_n_image_locator_validation_promotion_20260724`.
- Boundary: this is locator-stability ownership, not defect classification,
  semantic requalification, parallelism, or field robustness.
- Next priority: none until a concrete operator blocker or verified regression
  exists. Recommended model: none until evidence exists | Reasoning effort:
  none until evidence exists.
- If the operator later supplies a measured sequential bottleneck and explicitly
  requests parallelism, the bounded candidate is isolated-worker `1/2/4`
  equivalence and thread-safety audit. Recommended model: gpt-5.6-sol |
  Reasoning effort: high.

## 2026-07-24 P236 Current-State Handoff Consolidation

- The user requested a durable next-chat record of all work completed so far,
  incomplete or rejected work, the current product identity, evidence limits,
  and the next-priority state.
- Added a compact current-state ledger to
  `docs\OPENVISIONLAB_CURRENT_HANDOFF.md`. It groups the detailed P1-P235 work
  into the workbench, LLM maintenance, deterministic fixture/metrology,
  affine, Auto MPoint, N-image/promotion, and runtime/release tracks.
- Explicitly preserved P220 as incomplete at its frozen `<=3 px` gate, the
  failed missing-pin and card `R` candidates as completed rejections, parallel
  workers as unimplemented, LLM expansion as frozen, and production
  calibration/field qualification as unproven.
- Updated the repository snapshot, documentation map, AGENTS starting contract,
  and next-chat prompt. Historical readiness percentages remain historical; no
  current completion percentage was invented.
- The user also explicitly requested Dev and original repository commit/push.
  The pre-publication check found Dev on `codex/public-sample-ux-docs` with the
  accumulated source work, original `main` clean, and GitHub CLI installed but
  unauthenticated. Dev is authoritative; import to original must use a reviewed
  Git patch/commit, not a bulk directory copy.
- Status: documentation complete after structure/link/diff verification.
  External push remains dependent on successful GitHub authentication.

## 2026-07-27 Qualified Recipe Snapshot Product Workflow Closure

- Completed the bounded Recipe Manager `Pipeline Review > History` panel and
  adapter on top of the existing immutable qualification core.
- Creation requires one selected saved complete `LocalValidationSet` run,
  matching selected set/Pipeline identity, explicit qualification scope,
  operator note, and no pending selected-Step edit. The exact core preflight
  then revalidates Pipeline, images, dependencies, batch rows, reports,
  retained sources/drawings, review queue, and runtime fingerprints.
- Manual unlocked Validation Sets are frozen into the Snapshot request using
  current ordered hashes without mutating the source set. Existing hash-locked
  sets retain their exact Pipeline-identity gate.
- The UI now supports list, verify, evidence-folder open, editable working
  Recipe creation without inherited qualification, confirmed supersede, and
  confirmed revoke. Supersede creates the verified successor first; terminal
  lifecycle records do not mutate the payload.
- The current-build smoke passed pending-edit rejection, create/reload/verify,
  evidence open, working copy, cancelled supersede with no change, actual
  supersede, revoke, accessibility, and unchanged Preview/Run, layers,
  workspace layer, and routes.
- Evidence:
  `artifacts\qualified_recipe_snapshot_ui_20260727` and
  `docs\reports\OPENVISIONLAB_QUALIFIED_RECIPE_SNAPSHOT_UI_IMPLEMENTATION_20260727.md`.
- Status: Complete for the local v1 workflow. This does not prove production
  fitness, field robustness, certified metrology, electronic approval, remote
  audit storage, deployment, camera/lighting, PLC, or I/O integration.
- Next priority: none until a concrete current-source operator blocker or
  verified regression is reproduced. Recommended model: none until evidence
  exists | Reasoning effort: none until evidence exists.
