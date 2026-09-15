# OpenVisionLab OVL-11 ImageCanvas SaveFileDialog host — 2026-09-08

## Completion record

Status: Complete for the single SaveFileDialog ownership boundary.

Scope: `RoiImageCanvasViewModel.Commands.cs`에서 `SaveFileDialog` 생성과 modal
결과 변환을 제거하고 `RoiImageCanvasView`가 연결하는 명시적 dialog host로
이동했다. `OpenFileDialog`, ContextMenu, 포인터·키보드 입력, Mat 저장 정책,
texture/layer lifetime, Recipe/XML, Preview/Run, 외부 consumer 계약은 이
slice에서 변경하지 않았다.

Acceptance criteria:

- 기존 SaveFileDialog title, filter, default filename, initial directory,
  `AddExtension`, `DefaultExt` 설정을 유지한다.
- 취소 시 아무 저장도 하지 않고, 성공 시 기존 `lastImageDirectory` 갱신을
  유지한다.
- modal 대화상자 표시 중 재진입을 차단하고 `finally`에서 guard를 해제한다.
- ViewModel이 concrete `SaveFileDialog`를 생성하거나
  `SaveFileDialog.ShowDialog`를 호출하지 않는다.
- ImageCanvas와 기존 저장/preview 경로의 Debug/Release 검증이 통과한다.

## 구조 변경

- 이전 owner: `RoiImageCanvasViewModel.Commands.cs`가
  `Microsoft.Win32.SaveFileDialog` 생성, option 설정, `ShowDialog` 호출과
  filename/null 변환을 모두 소유했다.
- 새 contract: `Dialogs.IImageCanvasDialogHost`가
  `ShowSaveImageDialog(defaultFileName, initialDirectory)`라는 한 경계를
  제공한다.
- 새 UI owner: `Views.RoiImageCanvasDialogHost`가 기존 dialog 옵션, modal
  re-entry guard, 선택 path/null 변환을 소유한다.
- lifecycle owner: `RoiImageCanvasView`가 host를 하나 만들고 attached
  ViewModel에 연결하며 detach/dispose 시 null로 해제한다.
- ViewModel은 파일명·초기 경로 계산, 반환 path의 저장 호출, 성공 후 마지막
  directory 상태만 유지한다.

## 보존한 호출 경로

```text
SaveImageCommand
  -> OnSaveIamge
  -> ImageDialogHost.ShowSaveImageDialog(defaultName, initialDirectory)
  -> SaveCurrentImage(fileName)
```

기존 public `SaveCurrentImage(string)`과 Mat/callback 소유권은 그대로다.
Dialog host는 image, persistence, Recipe, Layer, ImageSpace를 보관하지 않는다.
View가 붙지 않은 bare ViewModel에서는 host가 없으므로 command가 조용히
종료된다. 현재 제품의 ContextMenu command 경로는 `RoiImageCanvasView`가
attach된 뒤에만 실행되며, 이 headless 경로는 별도 요구가 재현될 때만 다시
검토한다.

## 실제 검증

- ImageCanvas Debug build: 경고 0, 오류 0.
- ImageCanvas Release build: 경고 0, 오류 0.
- 기존 `ImageCanvasExternalConsumerSmoke` Release build: 경고 0, 오류 0.
- `OpenVisionLab` x64 Debug/Release build: 경고 0, 오류 0.
- `RunUiPrecheck.ps1 -Targets
  wpf_imagecanvas_owned_mat_load,wpf_shell_host_tool_input_image_load_save`
  Debug: 두 target 모두 `OK`.
- 같은 두 target Release: 두 target 모두 `OK`.
- 변경 후 `Invoke-RefactorAudit.ps1 -Verify`: PASS; 766 C# 파일 / 268,339줄 /
  12,220,701 bytes, 58 XAML 파일, partial 106개, ViewModel UI/IO 신호 2개,
  direct IO 8개, project cycle 0개, Shell Run History 직접 저장 0개.
- `TestDocumentationIndex.ps1`, JSON parse, `git diff --check`: PASS.

정적 owner 검색 결과는 D: evidence root의
`dialog-ownership-search.txt`, `lifecycle-wiring-search.txt`,
`save-call-path-search.txt`, `structure-proof.txt`에 보관했다.

## 미검증 범위와 다음 경계

`SaveFileDialog` 자체의 실제 Windows modal 상호작용(버튼, keyboard,
popup/window bounds), 지원 theme/layout/DPI matrix는 실행하지 않았다.
따라서 해당 UI 항목은 **소스 코드 기준 검토 완료 / 실제 Runtime UI 검증
필요**로 남긴다. 다음 OVL-11 실행은 `OpenFileDialog` 소유권을 하나의
독립 경계로 이동하며, ContextMenu와 입력 이벤트는 그 이후로 남긴다.
완료된 Mat/SaveFileDialog owner를 다시 분리하거나 partial-only 파일을
추가하지 않는다.

Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-save-dialog-host-20260908`
의 build log, UI precheck report/screenshot, quantitative audit, 구조 검색,
refactor proof report.
