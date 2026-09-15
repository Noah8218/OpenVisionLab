# OVL-07 Pipeline Review Document 결과 투영 revision guard — 2026-09-08

Status: Complete (Document 결과 투영 revision 경계 한 slice).

## 문제

`OpenVisionPipelineReviewExecutionController`는 이미 실행 세대, input/recipe revision, 취소와 stale callback 원자성을 소유하고 있었다. 그러나 `OpenVisionPipelineReviewDocument`는 controller 완료·superseded·예외·`StepUpdated`와 finally 투영에서 문서가 dispose 되었는지만 확인했다. 입력 Layer 또는 Recipe를 교체한 뒤 오래된 continuation이 새 View의 status, validation, step evidence, busy state를 덮을 수 있는 책임 결합이 남아 있었다.

## 변경

- `src/OpenVisionLab/UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocumentRevisionGate.cs`를 추가했다. Window를 참조하지 않는 작은 상태 owner가 input/recipe/run generation과 disposal invalidation을 소유한다.
- `OpenVisionPipelineReviewDocument.cs`의 refresh는 gate를 invalidate하고, 실행은 한 revision stamp를 캡처해 controller에 전달한다.
- completion/superseded/exception은 captured stamp가 current일 때만 View에 투영한다. finally는 현재 controller가 running/stopping이 아닐 때만 busy를 해제한다.
- `StepUpdated` event args에 input/recipe revision을 넣고 Document가 event revision을 확인한 뒤 기존 summary/flow/selection projection을 호출한다.
- `OpenVisionPipelineReviewExecutionController.cs`는 자신의 이미 존재하는 execution stamp를 event args에 전달하는 것만 변경했다.
- `tools/VisionRecipeRunnerSmoke/OpenVisionPipelineReviewDocumentRevisionContract.cs`와 Program dispatch를 추가해 gate state와 실제 source call path를 독립 검증한다.

Recipe/XML 저장 형식, explicit Preview/Run, input/output Layer routing, ImageSpace Lease/snapshot, controller stale callback atomic boundary, WPF XAML/theme는 변경하지 않았다. 기존 완료 owner를 다시 나누거나 partial 파일을 추가하지 않았다.

## 소유권과 호출 경로

```text
RefreshLayerState / RefreshInputLayerState
  -> DocumentRevisionGate.InvalidateRecipe/Input
  -> existing controller.Reset
  -> current View state projection

RunReviewAsync
  -> DocumentRevisionGate.BeginRun
  -> existing controller.RunAsync(inputRevision, recipeRevision)
  -> StepUpdated(inputRevision, recipeRevision)
  -> Document current-stamp check
  -> existing View result projection

Document.Dispose
  -> DocumentRevisionGate.Dispose
  -> existing controller.Dispose
  -> captured continuations rejected
```

## 집중 검증

- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false --no-restore`: PASS, 경고 0/오류 0.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false --no-restore`: PASS, 경고 0/오류 0.
- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false --no-restore`: PASS, 경고 0/오류 0.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false --no-restore`: PASS, 경고 0/오류 0.
- `--pipeline-review-document-revision-contract`: Debug/Release PASS. Recipe/Input invalidation, newer run generation, disposal idempotence와 Document/controller/event source 검사를 통과했다.
- 기존 `--pipeline-review-stale-callback-contract`: Debug/Release PASS.
- 기존 `--pipeline-review-execution-contract`: Debug/Release PASS.
- `TestDocumentationIndex.ps1`: PASS (`IndexedPaths=207`, `Routes=13`, `RootRedirects=102`).
- `git diff --check`: PASS. 출력은 기존 worktree의 LF→CRLF 정규화 경고만 포함한다.
- Release `PipelineViewerScreenshotSmoke`의 `wpf_shell_host_pipeline_review`와
  `wpf_shell_host_pipeline_review_ng`: 모두 `OK|check=OK`, 1600×900. 동적
  Win32 monitor wrapper가 2개 모니터에서 작은 왼쪽 `\\.\DISPLAY2`
  (`Bounds=-1920,365-0,1445`, `WorkArea=-1920,365-0,1397`)를 선택했고,
  실제 window rect `-1910,375--310,1275`가 선택 모니터와 교차했다. 정상·NG
  PNG를 생성하고 화면을 확인했다. wrapper의 dotnet shim은 종료 코드가
  노출되지 않아 `ProcessExitCode=-1`로 기록했으며, target의 `OK|check=OK`
  marker와 빈 stderr를 통과 조건으로 사용했다.

실행 기록은 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-survey-20260908\revision-build-main-debug.log`, `revision-build-smoke-debug-2.log`, `revision-build-main-release.log`, `revision-build-smoke-release.log`, `revision-contract-debug\pipeline-review-document-revision-contract.txt`, `revision-contract-release\pipeline-review-document-revision-contract.txt`, `revision-stale-debug\pipeline-review-stale-callback-contract.txt`, `revision-stale-release\pipeline-review-stale-callback-contract.txt`, `revision-execution-debug\pipeline-review-execution-contract.txt`, `revision-execution-release\pipeline-review-execution-contract.txt`, `documentation-index-check.log`, `git-diff-check.log`, `ui-after-normal-r6`, `ui-after-ng-r1`에 있다.

이전 stale-callback closure의 동일 Review target 캡처(`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-stale-callback-20260908\ui-release-final`)는 이번 slice 직전 baseline 문맥으로 보존했다. 당시 monitor topology가 달라 baseline은 재구성된 비교 자료이며, 현재 built change의 fresh runtime evidence는 `ui-after-normal-r6`와 `ui-after-ng-r1`이다. 전체 themes/layouts, 100% 외 125–200% DPI, pressed/focus/keyboard matrix와 Recipe/XML round trip은 이 slice에서 실행하지 않았다.

## 중복 작업 방지와 종료 경계

이 slice는 현재 핸드오프가 지목한 Document result projection 경계만 닫는다. controller stale callback, ImageSpace owner, PropertyGrid 정책, Learn Geometry와 그 밖의 완료 owner는 새 결함이 재현되지 않는 한 다시 나누지 않는다. `openvisionlab-2d` 15분 자동화는 `PAUSED` 상태로 유지하며, 이 종료 뒤 다른 모델·에이전트·예약 실행이 리팩토링·주석·문서·재검증을 자동으로 이어가지 않는다. Commit, push, Original, merge, release, deployment는 수행하지 않는다.

다음 단일 우선순위: OVL-07 Recipe CommandSurface의 남은 History/Validation orchestration 경계 | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
