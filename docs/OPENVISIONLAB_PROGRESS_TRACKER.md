# OpenVisionLab Progress Tracker

Updated: 2026-06-19

이 문서는 아직 완료 문서로 이동하지 않은 진행 항목만 관리한다.
완료된 항목은 `docs/OPENVISIONLAB_COMPLETED_TRACKER.md`로 이동하고, 이 문서에서는 제거한다.

## Management Rule

- 이미 `Completed Tracker`에 있는 항목은 새 증거 없이 다시 진행 과제로 올리지 않는다.
- 같은 표현의 광범위한 과제는 금지한다. 예: `Image Compare 안정화`처럼 이미 닫힌 세부 항목을 포함하는 문구.
- 진행 항목은 반드시 `남은 결정`, `완료 기준`, `검증 방법`을 가진다.

## Active Work Items

## Current Snapshot After Priority 1-7 Pass

- Closed / Watch: AI Recipe prompt/suggested-fix/acceptance-fix/layer-flow edit contract.
- Closed / Watch: Good/Bad sample pair minimum coverage contract now requires 12 pair groups / 27 pair rows, including ExpectedFailure negative samples.
- Closed / Watch: Pipeline WPG layer selector and metric range editor contract.
- Closed / Watch: PropertyGrid Threshold UX is stabilized and frozen unless a regression or explicit user request reopens it.
- Closed / Watch: Pipeline placeholder input/output flow now keeps placeholder layers out of runnable input selection and keeps Preview separate from Publish.
- Closed / Watch: Localization direct `T(...)` and formatted `TF(...)` key scan.
- Closed / Watch: Tutorial portable image embedding plus required workflow/tool image contract.
- Closed / Watch: Tool no-result diagnostics include key metric summaries.
- Closed / Watch: OpenVision readiness contract now guards the current 1-7 priority areas without launching UI.
- Closed / Watch: AI Recipe Safe Fix caption shows total/selected/review counts and the smoke contract checks it.
- Closed / Watch: AI Recipe Safe Fix detail panel shows selected Step/parameter/value/review policy and the smoke contract checks it.
- Closed / Watch: AI Recipe Apply Safe Fix now leaves an Applied Diff summary in the patch panel, including selected changes and changed XML lines.
- Closed / Watch: AI Recipe Safe Fix and Layer Flow edits now support guarded Revert Fix. The button is enabled only while the current XML still matches the last applied edit, and the smoke contract verifies apply -> revert -> XML restore.
- Closed / Watch: Good/Bad sample catalog contract now parses 12+ pair groups, 27+ pair rows, pair roles, metric bounds, ExpectedFailure rows, and shared baseline pipelines.
- Closed / Watch: Fiducial_Solder adds another public Good/Bad pair.
- Closed / Watch: Acceptance metric failures now include tool-specific tuning candidates.
- Verified: `artifacts\platform_precheck_20260619\platform_precheck_summary.json`
- Remaining active risk should now focus on new feature expansion, new sample regressions, visible UI polish when explicitly allowed, and conservative algorithm cleanup that is not yet covered by sample contracts.

| Priority | Area | Current State | Remaining Decision / Work | Completion Criteria | Verification |
| ---: | --- | --- | --- | --- | --- |
| 1 | Tool-specific result explanation | Core Tool Result contract is closed. Acceptance metric failure includes actual metric value/target range and tool-specific tuning candidates. No-result diagnostics now include key metric summaries for Contour/Blob/LineGauge/Matching/Feature. | Future work should add more sample-backed wording only when a new tool family fails without actionable guidance. | Major tools show `ErrorCode`, `ResultStatus`, `Message`, `Hint/Fix`, key metrics, and likely parameters to inspect. | Tool Result Contract + sample-backed NG recipe |
| 2 | More OK/NG paired recipes | Sample Catalog foundation is closed. Current gate covers 12+ Good/Bad pair groups and 27+ pair rows, including `Fiducial_Solder`. Matching target/no-target includes multiple ExpectedFailure rows; FeatureMatching has ScoreMax-based low-score/wrong-target rows. Real-material images remain excluded from the public catalog. | Next expansion should add public OK/NG pairs for underserved tool families only when metric separation is clear and repeatable. | Contour, Blob, LineGauge, Matching, FeatureMatching, Edge, Mean, and Fiducial-style contour references have representative OK/NG or expected-failure pairs. | Sample Catalog required rows + pair metric gate |
| 3 | LLM interactive recipe tuning | Prompt/safe-fix base, Apply Fix, Apply + Preview, Revert Fix, and automation policy are closed/watch. Acceptance limit candidates are not default-selected. | 다음 작업은 Safe Fix 적용 중복 구현이 아니라 operator wording과 Good/Bad pair 기반 before/after 비교를 강화하는 방향이 맞다. | 실패 Step 선택 -> 제안 확인 -> 적용 -> 선택적 되돌리기 -> Preview 재실행 흐름이 XML 편집 없이 가능하고, 위험한 의미 변경은 명시 확인 없이는 적용되지 않는다. | AI Recipe focused contract + failed-step sample |
| 4 | WPG visual/editor consolidation | Common editor foundation and Threshold inline-invert UX are closed/watch. PropertyGrid UX should remain frozen unless the user requests a specific change or a contract fails. | Avoid broad visual redesign. Only fix regressions, duplicate hidden-property exposure, or newly required editor behavior. | Pipeline/Tool Form PropertyGrid uses consistent editor policy, hidden properties are not exposed twice, and UX changes are verified by execution/capture before completion. | WPG contract + scoped UI capture when allowed |
| 5 | Localization coverage expansion | Localization project base is closed. | 모든 Main/Pipeline/Tool Form/MessageBox/ImageCompare 문자열을 catalog 기준으로 점검해야 한다. 편집기 사용 흐름도 정리 필요. | Korean/English 전환 시 핵심 화면의 하드코딩 문구가 남지 않는다. | Localization missing-key scan + selected non-visible checks |
| 6 | Tutorial content depth | Portable tutorial is closed. | 튜토리얼에 검사별 실제 teaching 흐름을 더 넣어야 한다. Contour/Blob/Matching/Edge/LineGauge/Measure/Layer compare 사용법을 사용자 문서 수준으로 보강한다. | 사용자가 샘플 이미지 기준으로 Tool Form 티칭 -> Pipeline 적용 -> 결과 확인을 따라할 수 있다. | Tutorial portable contract + image/link check |
| 7 | Algorithm refactoring residual | Major algorithm contracts are closed/watch. | 속도/기존 동작을 깨지 않는 선에서 반복 코드와 naming을 계속 정리한다. 단, CVBlob DLL 버전 변경은 금지한다. | 리팩토링 후 sample catalog, algorithm contract, line/matching/blob/contour smoke가 유지된다. | `RunVisionPlatformPrecheck.ps1 -SkipUi` |

## Deferred / Policy Required

| Area | Reason |
| --- | --- |
| DockPanel visual layout Undo/Redo | 레이어 생성/삭제 자체 Undo/Redo는 1차 완료했다. 다만 float/dock 위치, split 위치 같은 visual workspace layout까지 되돌리는 것은 별도 범위로 둔다. |
| Full visual regression threshold | UI capture는 가능하지만 회사 환경에서는 화면 노출 이슈가 있다. 기본은 quiet/offscreen 또는 `-SkipUi`, 필요 시 사용자가 허용했을 때만 visible capture를 실행한다. |
| Full automatic AI parameter tuning | 정책상 기본 기능으로 두지 않는다. 검사 결과를 억지로 OK로 만드는 위험이 있으므로 Acceptance/ROI/score 변경은 operator confirm이 필요하다. |

## Removed From Active Work

아래 항목은 완료 문서로 이동했으므로, 새 회귀 증거 없이 다시 진행 항목으로 올리지 않는다.

- Image Compare 8-bit Gray source format display.
- Image Compare last-open directory.
- Image Compare N-image load.
- Image Compare coordinate/GV contract.
- Image Compare standalone exe foundation.
- Pipeline Preview/Publish separation.
- Pipeline input/output/branch basic UX.
- Portable tutorial embedded-image issue.
- PropertyGrid/ROI/Pipeline Undo/Redo base.
- Existing layer image content Undo/Redo first pass.
- Layer create/delete Undo/Redo first pass.
- Full automatic AI tuning policy decision.
- External reference clone/build policy.
- Release/package version policy.
