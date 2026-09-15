# OpenVisionLab 선행개발 전 우선순위 1~5 완료 보고서

Date: 2026-09-15 KST  
Work item: `PL-0057`  
Repository: `C:\Git\2D\Dev`  
Evidence root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\predevelopment-priorities-20260914`

## 목적과 범위

이 보고서는 처음 프로젝트를 접하는 개발자가 Visual Studio에서 솔루션을 열고 실제
프로그램을 실행한 뒤 코드를 추적하는 순서로 현재 상태를 정리합니다. 사용자 요청의
우선순위 1~5는 다음 범위입니다.

1. 기존 dirty worktree를 손상하지 않는 기준선 확보
2. x86, embedded smoke 종료 코드, Vision SDK NOTICE 계약 수정
3. 전체 문서 트리와 dead-file 정리
4. Line 결과/진단 label 충돌 수정
5. ImageCanvas concrete control/lifetime을 View/presentation owner로 이동

포함하지 않은 범위는 `C:\Git\2D\Original`, commit/push/tag/release/deploy, 실제 카메라·PLC·
조명·I/O, 외부 메시지, 새 2D 기능 구현입니다.

## 1. 변경 전 기준선

- branch: `codex/public-sample-ux-docs`
- starting HEAD: `176eec95e0081502dc07d89ddaee666abac8123d`
- `git status --short -uall`: 244 paths (`54` tracked, `190` untracked)
- 원본 상태, tracked patch/stat, `git diff --check`, 파일별 SHA-256를
  `01-baseline`에 기록했습니다.
- 기준선에 있던 변경은 사용자 작업으로 취급했고 reset, checkout, bulk overwrite하지
  않았습니다.
- 최종 대조는 기존 244 paths 중 `231` unchanged, `13` changed in this scoped work,
  `0` no-longer-dirty이며 새 dirty path는 `64`입니다. 상세 hash 대조는
  `06-final-checkpoint/worktree-delta-from-baseline.csv`에 있습니다.

## 프로젝트 정체성과 Visual Studio Start Here

OpenVisionLab은 Windows x64/.NET 8 WPF 기반의 규칙 기반 vision recipe workbench입니다.
정상 사용자 흐름은 `sample image → PropertyGrid teaching/Pipeline 구성 → 명시적 Preview
또는 Run → layer/result 비교 → N-sample validation → Recipe 저장`입니다. LLM XML 작성
지원은 선택적 보조 기능이며 장비 통합 플랫폼은 현재 범위가 아닙니다.

처음 읽을 때는 다음 한 경로만 사용합니다.

```text
OpenVisionLab.sln
  -> src/OpenVisionLab/OpenVisionLab.csproj (startup WinExe)
  -> Program.Main
  -> App/Bootstrap/OpenVisionLabApplication.Run
  -> ApplicationRuntimeContext.CreateDefault
  -> OpenVisionShellHostWindow
  -> OpenVisionShellHostView
```

Recipe 실행은 다음 순서입니다.

```text
OpenVisionShellHostView
  -> RecipeCommandSurface
  -> recipe execution/review session
  -> OpenVisionPipelineReviewDocument
  -> VisionPipelineExecutionService
  -> VisionPipeline*Tool
  -> VisionPipelineRunResult / drawing evidence
  -> review ViewModel binding
```

솔루션은 제품 앱 1개, 앱이 참조하는 제품 라이브러리 12개, 검증/보조 실행 프로젝트
3개로 구성됩니다. 시작 프로젝트는 이름 추정이 아니라 `.sln`, 모든 solution `.csproj`,
`ProjectReference`, `OutputType`, `Program.Main`을 확인해 결정했습니다. 솔루션 프로젝트
참조 cycle은 없고 x86 구성은 지원하지 않습니다.

## 처음 접하는 개발자 관점의 결론

좋은 점:

- `Core`, `UI`, 책임별 Presenter/Controller/Service, 내부 Libraries의 방향이 드러납니다.
- `Teach`, `Preview`, `Run`이 명시적 계약이며 화면 열기나 설정 복원이 실행을 암묵적으로
  일으키지 않습니다.
- 실제 EXE smoke, 화면 없는 contract runner, DLL hash/NOTICE/readiness gate가 이미 있어
  변경을 증명할 수 있습니다.
- 외부 ImageCanvas consumer라는 두 번째 실제 사용 경로가 있어 재사용 경계를 추측이
  아니라 실행으로 확인할 수 있습니다.

부담과 주의점:

- 오랜 기간의 완료 보고서와 smoke 증거가 많아 시간순으로 읽으면 현재 owner를 찾기
  어렵습니다. 따라서 compact `docs/README.md`와 route index만 시작점으로 사용합니다.
- 일부 검증 runner의 `Program.cs`가 매우 크고 일반 unit-test framework보다 문자열 기반
  source contract가 많습니다. 제품 정책은 runner가 아니라 `src`의 concrete owner에서
  수정해야 합니다.
- WPF/WinForms/OpenGL/OpenCvSharp가 만나는 부분은 UI thread, event 해제, native resource
  lifetime을 함께 확인해야 합니다.
- 과거 보고서의 완료율이나 다음 우선순위는 현재 source/runtime 증거가 아닙니다.

## MVVM 판정

전체 구조는 책임별 ViewModel/Presenter/Controller/Service를 사용하는 실용적 MVVM입니다.
그러나 모든 클래스가 UI framework와 완전히 독립인 교과서식 순수 MVVM이라고 표현하면
과장입니다.

- View code-behind는 XAML namescope, focus/drag/dialog host, framework event와 native host
  lifetime 같은 presentation plumbing을 담당합니다.
- ViewModel은 binding/command/ROI/image state를 담당합니다.
- Recipe/Pipeline 정책과 저장/실행은 별도 owner를 통해 흐릅니다.
- ImageCanvas의 가장 큰 concrete-control 위반은 이번 M5에서 제거했습니다.
- `RoiImageCanvasViewModel`은 여전히 WPF key-command adapter와 ImageCanvas presentation
  operation에 연결된 UI-facing ViewModel입니다. 이를 순수 domain 모듈이라고 부르지
  않으며, 새 요구나 실제 교체/test seam 없이 추가 interface 계층을 만들지 않습니다.

## 2. 거짓 실행 계약 수정

### 플랫폼

`OpenVisionLab.sln`, 제품 앱, `WpfPropertyGridBridge`의 x86 solution/project 구성을
제거했습니다. 지원 계약은 Windows x64이며 `Any CPU` solution 구성도 앱 프로젝트에서
x64 결과를 만듭니다. `Debug|x86` 요청은 이제 `MSB4126`과 nonzero exit로 거부됩니다.

### embedded smoke 종료 코드

embedded smoke dispatch owner를 `Program.Main` 한 곳으로 고정하고 반환값을
`Environment.ExitCode`에 전달했습니다. 알 수 없는 smoke scenario는 보고서 `FAIL`뿐
아니라 process exit `1`도 반환합니다.

### SDK NOTICE

`NOTICE`의 OpenVisionLab Vision SDK 3.0.0 commit을 manifest의
`f4f0c0dc8bee5b7a849ae6eb66a5307bed4b8a6b`로 맞췄고, NOTICE 검증 도구가 version/commit
정합성을 함께 검사하도록 했습니다.

증거: `02-contracts`

## 3. 문서와 불필요 파일 정리

전체 `docs` 트리는 파일을 빠뜨리지 않도록 파일별 SHA-256/크기/형식으로 읽었습니다.
활성 권위 문서는 내용과 경로를 직접 검토하고, 완료·역사 보고서는 현재 계획으로
오해하지 않도록 route와 archive 역할을 분리했습니다.

- 전체 문서 파일: 1,331개
- UTF-8/JSON을 포함한 text scan: 944개
- 전체 크기: 102,765,787 bytes
- content-hash duplicate group: 40개
- invalid UTF-8/JSON: 0개
- 존재하지 않는 inline Markdown 상대 링크: 0개
- 상세 inventory: `03-document-cleanup/full-document-scan/document-inventory-sha256.csv`

활성 제어면 정리:

- `docs/README.md`, `docs/LLM_DOCUMENT_INDEX.json`, current handoff,
  `CODEBASE_STRUCTURE.md`를 compact current owner로 재작성했습니다.
- 기존 장문 원문은 `docs/admin/archive/*_HISTORY_20260914.*`로 보존했습니다.
- archive 이동으로 한 단계 짧아진 역사 문서 링크 156개는 원래 target이 모두 존재함을
  확인한 뒤 위치만 재기준화했습니다.
- 중복 next-session handoff는 archive/redirect로 정리하고 오래된
  `docs/admin/NEXT_CODEX_PROMPT.md`는 제거했습니다.

검증 후 삭제한 파일:

- compile/reference가 없는 `LayoutItemViewModel.cs`
- 파일명에 공백이 있고 compile 제외 상태였던 `BooleanToEyeIconConverter .cs`
- .NET 8 SDK-style WPF build에서 출력되지 않고 사용되지 않는 앱/ImageCanvas
  `App.config` 2개
- 활성 문서와 패키지에서 참조되지 않는 legacy PNG 21개, 총 2,728,856 bytes

삭제 전 경로/크기/SHA-256는 `03-document-cleanup/removed-media.csv`와
`deletion-precheck`에 기록했습니다. 모두 Git으로 복구 가능한 tracked 파일입니다.

삭제하지 않은 후보:

- 동일 hash의 prompt packet/실행 증거와 public sample/evidence 이미지가 있습니다.
  중복 byte라는 이유만으로 삭제하면 재현성 또는 독립 패키지 계약을 깨뜨릴 수 있어
  이번에는 보존했습니다.
- 과거 보고서의 옛 `C:\Git\OpenVisionLab_Dev` 경로는 당시 증거이므로 일괄 수정하지
  않았습니다. 현재 진입 문서와 실행 가이드만 `C:\Git\2D\Dev`로 정정했습니다.
- root compatibility redirect는 외부 링크 사용 여부가 확인되지 않아 보존했습니다.

증거: `03-document-cleanup`

## 4. Line overlay 충돌 수정

`VisionPipelineLineDistanceTool`은 고정 offset 대신 실제 `Cv2.GetTextSize` 결과로 각
거리 label bounds를 계산합니다. 측정선의 오른쪽을 우선하고 공간이 없으면 왼쪽을
사용하며, 이미지 밖 또는 기존 label과 충돌하면 text만 생략합니다. 측정 line 자체는
항상 유지됩니다.

`OpenVisionNativeToolPreviewOverlayRenderer`의 signal profile label은 측정 열과 겹치지
않는 상단 diagnostic legend 영역에 배치합니다. production layout seam을 직접 검사하는
contract는 12개 label/5개 조건을 통과했습니다.

실제 제품 EXE의 `Public_Line_Pins_Distance` smoke는 37 px, 0.222 mm, 24 detections로
PASS했습니다. 앞선 2-monitor 검증에서는 왼쪽의 작은 `DISPLAY2`에 배치했고, 마지막
source 재검증 시 Windows가 보고한 1개 논리 monitor `DISPLAY2`는 그대로 사용했습니다.
두 경우 모두 실제 window bounds가 선택 monitor와 교차하거나 그 안에 있었습니다.
before/after PNG는 `04-line-overlay/before`, `04-line-overlay/actual-exe`, 최종 재검증은
`06-final-checkpoint/actual-exe-line/run-post-review`에 있습니다.

검증한 UI 범위는 100% DPI의 actual EXE Line sample입니다. 125/150/175/200% DPI,
모든 theme, keyboard/focus state는 이번 수정에서 실행하지 않았습니다.

## 5. ImageCanvas control/lifetime owner 이동

변경 전에는 `RoiImageCanvasViewModel`이 concrete WinForms/OpenGL
`ImageCanvasControl`을 생성·노출·호출·Dispose했습니다. 변경 후 call path는 다음과
같습니다.

```text
RoiImageCanvasView 또는 ImageCanvasExternalConsumerSession
  -> RoiImageCanvasPresentation
  -> ImageCanvasControl / input controllers / render callbacks / refresh timer
```

- `RoiImageCanvasPresentation`: concrete control 생성, render/input/timer 연결과 해제,
  texture/overlay surface, native Dispose owner
- `RoiImageCanvasView`: presentation 생성, WindowsFormsHost 배치, DataContext attach/detach,
  View lifetime owner
- `RoiImageCanvasViewModel`: binding/command/ROI interaction/current `Mat` state owner;
  concrete control public property 없음
- `ImageCanvasExternalConsumerSession`: 외부 host/presentation/overlay metadata lifetime owner

동일 View에서 DataContext가 잠시 분리됐다가 같은 ViewModel로 돌아오는 경우에는 기존
canvas state를 유지하고, 다른 ViewModel로 교체할 때만 surface를 초기화합니다. 알려진
앱 consumer의 해제 순서는 모두 `View/presentation → ViewModel`로 고쳤습니다.

focused evidence:

- ImageCanvas library와 제품 앱 build
- `RoiImageCanvasBoundaryContract`: concrete owner와 old coupling 제거
- `RoiImageCanvasPathPolicyContract`
- actual external consumer: Borrow/Clone/TakeOwnership, 720x480, overlay 2개, selection,
  view-state round-trip, create/dispose 100회, 최종 handle delta `+2` (`<=8` gate), screenshot
- ImageCanvas 변경 후 actual product EXE Line smoke 재실행 PASS

증거: `05-imagecanvas-owner`

## DLL 분리·재사용 결론

“오픈소스이므로 DLL 하나만 빼서 쓴다”는 전체 제품에 적용할 수 없습니다.

| 대상 | 판정 | 필요한 경계 |
| --- | --- | --- |
| `OpenVisionLab.exe`/앱 assembly | 단일 DLL 분리 불가 | 12개 project library, SDK/PropertyGrid/FontAwesome, 설정·content와 native runtime closure 필요 |
| `OpenVisionLab.ImageCanvas.dll` | 제한적으로 재사용 가능 | .NET 8 Windows Desktop, Localization, OpenCvSharp, SharpGL/WinForms, `OpenCvSharpExtern.dll`; 공식 `ImageCanvasExternalConsumerSession` 경로 사용 |
| 순수/공유 library | 프로젝트별 검토 후 가능 | 해당 `.csproj`의 `ProjectReference` closure와 UI/file/global/static coupling을 각각 build/search해야 함 |
| Vision SDK DLL | manifest 단위 사용 | SDK 3.0.0 파일 hash와 shared native runtime, NOTICE/라이선스 계약 유지 |

ImageCanvas 외부 sample은 앱 프로젝트를 참조하지 않고 실제로 build/run되므로 재사용
경계가 존재한다는 것은 증명했습니다. 그러나 native/runtime dependency가 있으므로
single-file 또는 단일-DLL xcopy 배포를 증명한 것은 아닙니다. NuGet/공개 API 호환성,
독립 versioning과 배포 라이선스 검토도 별도입니다.

## 최종 검증

| 검증 | 결과 |
| --- | --- |
| `OpenVisionLab.sln` Debug / Release (`Any CPU`, app x64) | 각각 경고 0, 오류 0 |
| embedded 제품 EXE build | 경고 0, 오류 0 |
| `VisionRecipeRunnerSmoke` build | 경고 0, 오류 0 |
| `PipelineViewerScreenshotSmoke` build | 경고 0, 오류 0 |
| Line layout contract | PASS, checks 5 / labels 12 |
| ImageCanvas owner contract | PASS, checks 8 |
| ImageCanvas path-policy contract | PASS, checks 6 |
| actual product EXE Line smoke | exit 0, PASS, 37 px / 0.222 mm / 24 detections |
| actual external ImageCanvas consumer | exit 0, PASS, 100 create/dispose, handle delta +2 |
| monitor placement | 최종 동적 topology 1개(`DISPLAY2`, 1920x1080); 두 EXE 교차/내부 배치 PASS. 앞선 2-monitor run도 작은 왼쪽 `DISPLAY2` 배치 PASS |
| readiness | 13개 범주 PASS |
| refactor audit | PASS; C# 850, XAML 57, project cycle 0, Shell storage call 0 |
| documentation index | PASS; indexed paths 32, routes 12, root redirects 102 |
| 전체 문서 scan | 1,331 files; invalid UTF-8/JSON 0; missing inline links 0 |
| external DLL Debug / Release | hash/allowlist PASS |
| NOTICE | retained dependency와 SDK version/commit provenance PASS |
| public sample assets | PASS; catalog 33, manifest assets 230, pipelines 17 |
| output config check | `log4net.config`만 존재; 제거한 legacy app config 출력 없음 |
| `git diff --check` | 오류 없음 |

actual EXE 증거는 `06-final-checkpoint/actual-exe-line/run-post-review`, 외부 consumer
증거는 `06-final-checkpoint/external-consumer-runtime-post-review`에 있습니다. 최종 캡처를 직접
검토해 Line label 열/상단 profile legend와 외부 overlay 선택 상태를 확인했습니다.

실행하지 않은 범위:

- `PipelineViewerScreenshotSmoke` 전체 target 회귀 실행
- 125%, 150%, 175%, 200% DPI와 모든 theme/layout/input state
- 실제 카메라·GPU/driver matrix·PLC/I/O·장시간 운전·field sample qualification
- publish/package/install, Original 동기화, commit/push/release/deploy

Status: Complete  
Scope: Dev 저장소의 선행개발 전 우선순위 1~5와 재사용 가능한 분석/정리/검증 기록  
Acceptance criteria: C1~C6 모두 위 source/build/contract/actual EXE/document 증거로 PASS  
Verification: 위 표의 명령·결과와 D-drive 로그/캡처  
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\predevelopment-priorities-20260914`  
Boundary / next dependency: 현장·전체 UI matrix·배포 자격은 증명하지 않으며, 다음 2D 기능은 사용자 선택 후 별도 범위로 시작
