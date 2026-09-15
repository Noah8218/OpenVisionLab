# OpenVisionLab 2D Shell MVVM 상태·I/O 소유권 보정

Updated: 2026-09-10 KST

Status: VERIFIED for the bounded Shell state/I/O owner move, Debug/Release app
build, readiness contract, and focused workspace/Recipe smoke. Full WPF
theme/DPI/input/long-run qualification remains unverified.

## Problem

`OpenVisionShellHostView.Interactions.cs`는 화면 이벤트 연결 외에 다음 두
업무 상태를 직접 변경하고 있었습니다.

- Recipe 선택을 `GlobalState.Recipe.Name`과 `System.LastRecipe`에 반영하고 `SaveConfig()` 호출
- Workspace 이미지 경로를 검사·정규화해 `System.LastWorkspaceImagePath`에 저장

두 동작은 각각 이미 존재하는 concrete owner가 있었습니다. View가 계속
소유하면 Shell code-behind를 읽을 때 UI 이벤트와 persistence 경계를 함께
추적해야 하고, Recipe/Workspace 변경을 독립적으로 테스트하기도 어렵습니다.

## Current → intended owner and call path

| 항목 | 변경 전 | 변경 후 |
| --- | --- | --- |
| Recipe selection persistence | `OpenVisionShellHostView.Interactions.cs:SwitchRuntimeRecipe` | `OpenVisionShellHostRecipeController.SwitchRuntimeRecipe` |
| Workspace image-path persistence | `OpenVisionShellHostView.Interactions.cs:RememberWorkspaceImagePath` | `OpenVisionShellHostWorkspaceImageController.RememberWorkspaceImagePath` |
| View responsibility | callback 제공 및 탭/이벤트 연결 | existing owner method를 command/controller에 전달 |

호출 경로는 다음과 같습니다.

```text
Recipe command surface
  -> OpenVisionShellHostRecipeController.SwitchRuntimeRecipe
  -> GlobalState.Recipe/System.SaveConfig

Workspace/layer command
  -> OpenVisionShellHostWorkspaceImageController.RememberWorkspaceImagePath
  -> System.LastWorkspaceImagePath/System.SaveConfig
```

두 경로 모두 public API나 XAML binding 이름은 바꾸지 않았습니다. 새
interface, wrapper, manager, factory, partial은 추가하지 않았습니다.

## Ownership and lifetime

- Recipe controller가 이미 보유한 `ApplicationRuntimeContext`가 Recipe/System
  mutable state와 config 저장을 소유합니다.
- Workspace image controller가 이미지 파일 읽기와 Main layer 반영을 함께
  소유하므로 remembered image path persistence도 같은 owner에 둡니다.
- Shell View는 Window/Dialog, control state, event lifetime, presentation
  refresh만 조합합니다.
- `SaveConfig()`의 기존 호출 순서와 빈 경로/동일 경로 조기 반환 조건은
  그대로 보존했습니다.

## Verification

- OpenVisionLab Debug build: 0 warning / 0 error
- OpenVisionLab Release build: 0 warning / 0 error
- `OpenVisionReadinessCheck`: PASS, including source ownership checks that
  reject persistence code in `OpenVisionShellHostView.Interactions.cs`
- focused UI smoke: `wpf_shell_host_workspace_image_load=OK`,
  `wpf_shell_host_recipe_context_switch=OK`
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r11-shell-state-20260910`
- structural audit: `Invoke-RefactorAudit.ps1 -Verify` remains PASS; full WPF
  theme/layout/DPI/monitor/keyboard and long-run native behavior remain
  unverified

## Self-evaluation

The View is easier to classify: its remaining direct file dialogs and message
boxes are UI-bound adapters, while Recipe selection persistence and workspace
image-path persistence have named concrete owners. The Shell constructor and
long type names remain intentionally unchanged because this slice did not show
a separate lifetime or dependency boundary that would justify another wrapper.
