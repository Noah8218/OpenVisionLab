# OpenVisionLab 2D Shell readiness MVVM 경계 보정

Updated: 2026-09-10 KST

Status: VERIFIED for the bounded readiness-policy owner move, Debug/Release app
build, source readiness contract, and focused Workspace/Recipe/tool-search smoke.
Full WPF theme/DPI/input/long-run qualification remains unverified.

## Problem

`OpenVisionShellHostView.Interactions.cs`의 `RefreshToolReadiness`가 화면
이벤트 연결 외에 다음 정책을 직접 수행하고 있었습니다.

- Arithmetic 설정을 읽고 `VisionPipelineArithmeticStep.RequiresInputLayerB`
  정책을 계산
- Main/보조 Layer 이미지 존재 여부를 검사
- 계산 결과를 Shell ViewModel에 반영

이 로직은 표시를 담당하는 View보다 readiness 상태를 소유하는
`OpenVisionShellPreviewViewModel`에서 읽는 편이 호출 흐름과 상태 변경 위치를
확인하기 쉽습니다. 기존 `SetToolReadiness`와 `ApplyToolReadiness` 계약은
그대로 유지했습니다.

## Current → intended owner and call path

| 항목 | 변경 전 | 변경 후 |
| --- | --- | --- |
| Readiness policy | `OpenVisionShellHostView.Interactions.cs:RefreshToolReadiness` | `OpenVisionShellPreviewViewModel.RefreshToolReadiness` |
| Arithmetic policy | View가 settings를 읽고 직접 `RequiresInputLayerB` 호출 | ViewModel readiness owner가 기존 Pipeline policy를 호출 |
| Layer evidence | View의 `HasSecondaryWorkspaceImage` | ViewModel의 `HasSecondaryWorkspaceImage(IDisplayManager)` |
| View responsibility | policy 계산 + 상태 반영 | 기존 이벤트에서 ViewModel owner를 호출 |

호출 경로는 다음과 같습니다.

```text
Runtime Recipe/Layer/Native settings event
  -> OpenVisionShellHostView.Interactions.RefreshToolReadiness
  -> OpenVisionShellPreviewViewModel.RefreshToolReadiness
  -> OpenVisionNativeToolSettingsStore.Load
  -> VisionPipelineArithmeticStep.RequiresInputLayerB
  -> SetToolReadiness
  -> ApplyToolReadiness
```

`Preview`, Layer 생성·삭제, Recipe 저장, XAML binding 이름은 변경하지
않았습니다. 새 service, interface, factory, manager, wrapper, partial은
추가하지 않았습니다.

## Ownership and lifetime

- `OpenVisionShellPreviewViewModel`이 readiness mutable state와 visible tool
  projection을 계속 소유합니다.
- `IDisplayManager`는 현재 Layer 증거를 읽기 위한 기존 Shell session owner가
  전달합니다. ViewModel은 이미 `SetToolReadiness`로 repository projection을
  소유하고 있으므로 별도 readiness 객체를 만들지 않았습니다.
- 이미지 객체는 존재 여부만 확인하며 새 Bitmap/Mat을 만들거나 소유하지
  않습니다. Dispose 책임은 기존 DisplayManager/ImageSpace owner에 남습니다.
- View는 이벤트 수명주기와 callback 연결만 유지합니다.

## Verification

- OpenVisionLab Debug build: 0 warning / 0 error
- OpenVisionLab Release build: 0 warning / 0 error
- `OpenVisionReadinessCheck`: PASS, including source checks that reject direct
  readiness-settings policy in Shell View interactions
- focused UI smoke: `wpf_shell_host_workspace_image_load=OK`,
  `wpf_shell_host_recipe_context_switch=OK`,
  `wpf_shell_host_tool_search=OK`
- evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r12-tool-readiness-20260910`
- changed smoke project Debug build: 0 errors, one pre-existing CS8600 warning
  at `tools/PipelineViewerScreenshotSmoke/Program.cs:10181`
- `TestDocumentationIndex.ps1`: PASS, `IndexedPaths=282`, `Routes=16`,
  `RootRedirects=102`
- `Invoke-RefactorAudit.ps1 -Verify`: PASS,
  `CSharpFiles=847|XamlFiles=60|PartialDeclarations=110|ProjectCycles=0|ShellStorageCalls=0`
- `git diff --check`: no whitespace errors; Git reported only the repository's
  existing LF-to-CRLF normalization notices for edited files

## Self-evaluation

The Shell View now forwards three distinct state refresh triggers to named
owners: Recipe and workspace persistence use their existing controllers, and
tool readiness policy uses the existing Shell ViewModel. The remaining direct
View operations are UI-specific dialogs, control layout, event lifetime, and
external evidence opening; no independent domain owner was justified for those
paths in this slice. The full Shell constructor and ImageCanvas compatibility
ViewModel remain separate review boundaries because a mechanical split would
increase indirection without reducing a proven dependency.
