# OVL-04 ImageSpace Lease 기반 실행 Snapshot과 Layer 제거 원자성

작성일: 2026-09-07 KST  
상태: **Complete** (OVL-04 독립 slice)

## 범위

첨부 분석의 OVL-04 중 저장소 소유 `Bitmap`이 실행 중 교체·삭제될 때의
수명 경계를 구현했다. 기존 `GetImage` 공개 API와 `ImageSpaceImageLease`
참조 카운트 계약은 유지했다.

변경 파일:

- `src/Libraries/OpenVisionLab.ImageSpace.Core/ImageSpaceService.cs`
- `src/OpenVisionLab/Core/Display/DisplayManagerImageExtensions.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Preview/OpenVisionNativePreviewExecutionController.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/PipelineReview/Execution/OpenVisionPipelineReviewExecutionController.cs`
- `tools/HistoryContractCheck/Program.cs`
- `tools/VisionRecipeRunnerSmoke/Program.cs`
- `docs/LLM_DOCUMENT_INDEX.json`

## 구조 증거

변경 전 실행 입력 경로:

```text
DisplayManager → GetLayerImage → ImageSpace 소유 Bitmap을 직접 읽음
```

변경 후 실행 입력 경로:

```text
DisplayManager → GetLayerImageSnapshot
               → AcquireImage(title) lease 획득
               → lease 보유 중 독립 Bitmap clone
               → lease 해제
               → caller가 snapshot을 Dispose
```

`GetLayerImage`는 다른 동기 소비자의 호환성을 위해 삭제하거나 의미를
바꾸지 않았다. 독립 snapshot을 사용하는 소비자는 명시적으로 반환 Bitmap의
소유자이며, Native Preview의 단일 입력·Arithmetic A/B와 Pipeline Review의
입력 context가 이 경계를 사용한다. Review는 index로 다시 조회하지 않고
이미 읽은 title로 snapshot을 요청해 목록 이동으로 인한 다른 index 접근도
피한다.

`RemoveImage(title)`은 이름 조회, active 이미지 정리, 목록 제거를 같은
`sync` lock 안에서 수행한다. 제거된 `ImageSpaceImage.Release()`는 기존
index 제거 경로와 같이 lock 밖에서 호출한다. 따라서 동시 Insert가 목록
위치를 바꿔도 이름 조회 결과와 제거 대상이 분리되지 않는다.

`DisplayManagerImageExtensions`에는 process-wide 진단 카운터를 두었다.
성공한 snapshot clone 횟수와 `width × height × ceil(bits-per-pixel/8)`
계산값을 각각 원자적으로 누적한다. 이 값은 복사량 추정치이며 GDI/native/
managed peak 메모리 측정값으로 해석하지 않는다.

## 합격 기준과 결과

| 기준 | 결과 | 근거 |
| --- | --- | --- |
| snapshot 중 Layer 교체·삭제 후 disposed Bitmap 접근 없음 | 통과 | History barrier에서 lease 획득 후 Replace/Delete를 수행하고 old pixel을 읽음 |
| 같은 Bitmap의 double Dispose 없음 | 통과 | lease retirement, 반복 Dispose, 서비스 Dispose 및 Release 경로 검사 |
| `RemoveImage(title)`가 동시 목록 이동으로 다른 Layer를 제거하지 않음 | 통과 | `Keep`/`Target`에 대해 128회 동시 RemoveByName/Insert stress |
| 오래된 실행 snapshot이 저장소 변경에 영향받지 않음 | 통과 | snapshot pixel이 replacement/deletion 후에도 동일하고 Review context 실행 완료 |
| Arithmetic B/숨은 입력 보존 | 통과 | Native Preview Arithmetic A/B와 Review multi-input Arithmetic 실행 |
| 복사 횟수·byte estimate 구분 | 통과 | runner 진단 `copies=6`, `estimatedBytes=2304`, peak 미측정 명시 |
| 기존 Preview/Review 화면 회귀 | 통과 | 기존 WPF smoke `wpf_shell_host_pipeline_review`, `wpf_preprocess_output_preview_flow` |

## 검증

모든 테스트 출력과 evidence는 `D:\OpenVisionLab-TestData`에 저장했다. 현재
worktree의 기존 변경은 유지했고 `C:\Git\2D\Original`은 수정하지 않았다.

- [ImageSpace Core Release build, 0 warnings/0 errors](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl04-20260907/build-imagespace-release.log)
- [History Contract Release run](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl04-20260907/history-contract-release.log)
- [OpenVisionLab Debug build, 0 warnings/0 errors](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl04-20260907/build-openvisionlab-debug-final-2.log)
- [VisionRecipeRunnerSmoke Debug build, 0 warnings/0 errors](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl04-20260907/build-runner-debug-final-2.log)
- [OVL-04 Debug snapshot contract](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl04-20260907/image-space-snapshot-contract.log)
- [OpenVisionLab Release build, 0 warnings/0 errors](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl04-20260907/build-openvisionlab-release-final-1.log)
- [VisionRecipeRunnerSmoke Release build, 0 warnings/0 errors](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl04-20260907/build-runner-release-final-1.log)
- [OVL-04 Release snapshot contract](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl04-20260907/image-space-snapshot-contract-release.log)
- [WPF focused smoke run](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl04-20260907/wpf-focused-run.log)
- [Pipeline Review screenshot](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl04-20260907/wpf-focused/wpf_shell_host_pipeline_review.png)
- [Native Preview screenshot](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl04-20260907/wpf-focused/wpf_preprocess_output_preview_flow.png)
- [Detailed snapshot contract evidence](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl04-20260907/image-space-snapshot-contract-release/image-space-snapshot-contract.txt)
- `git diff --check --` 대상 소스·검사 파일: whitespace 오류 없음 (LF→CRLF 안내만 보고됨)
- [문서 색인 검증: 161 indexed paths / 13 routes / 102 redirects](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl04-20260907/documentation-index-final-1.log)

WPF smoke는 단일 `DISPLAY1` (1920×1080, working area 1920×1032)에서 실행했고
두 대상의 실제 렌더링 캡처를 확인했다. 변경된 XAML/style/template는 없으므로
theme/layout/DPI 전수 행렬과 pressed/focus/popup 상태는 이번 slice에서
실행하지 않았다.

## 경계와 남은 위험

이번 변경은 실행 소비자 두 곳의 독립 snapshot 경계를 닫는다. 공개
`GetImage`를 제거하지 않았으므로 새 비동기 소비자는 반드시 lease 또는
snapshot API를 선택해야 한다. Viewer·ROI·저장소 등 다른 동기 소비자는
기존 계약을 유지한다.

GDI/native/managed peak 메모리와 장시간 Load/Run/Close 누수 수치는 이번
slice에서 측정하지 않았다. `estimatedBytes`는 복사량 계산값일 뿐이며 실제
peak 증거가 아니다. Preview/Review의 실행 세대·취소·종료 drain은 OVL-05의
범위로 남긴다.

다음 프로젝트 우선순위는 OVL-05 실행 세대·취소·종료 수명 관리다.  
Recommended model: `gpt-5.6-terra`  
Reasoning effort: `high`
