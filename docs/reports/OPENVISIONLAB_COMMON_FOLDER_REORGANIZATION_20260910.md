# OpenVisionLab Common 책임별 폴더 정리

Updated: 2026-09-10 KST

Status: VERIFIED for the physical source-folder move and Debug/Release/source
contracts. The public namespace, type names, XML contracts, and runtime behavior
were preserved. Full WPF theme/DPI/input/long-run qualification remains
unverified.

## Why this slice exists

`src/OpenVisionLab/Common`에는 서로 다른 변경 이유를 가진 23개 파일이 한
폴더에 섞여 있었다. 이번 변경은 파일 길이나 클래스 수를 줄이기 위한 분리가
아니라, 이미 드러난 책임별 owner를 physical folder에 맞추는 정리다.

`AppCommon.cs`, `CCommon.cs`, `DEFINE.cs`는 여러 기능이 공유하는 legacy
공통 타입이라 새 owner를 추측하지 않고 Common 루트에 남겼다. 그 외 파일은
호출 경로와 namespace를 조사한 뒤 가장 좁은 기존 책임 폴더로 이동했다.

## Before / after

| 이전 경로 | 새 경로 | 이유 |
| --- | --- | --- |
| `Common/AppPathService.cs` | `Common/Runtime/AppPathService.cs` | runtime data-root/path ownership |
| `Common/BackgroundLoopWorker.cs` | `Common/Runtime/BackgroundLoopWorker.cs` | worker lifetime and shutdown |
| `Common/BitmapDrawing.cs` | `Common/Imaging/BitmapDrawing.cs` | Bitmap drawing helpers |
| `Common/BitmapImageConverter.cs` | `Common/Imaging/BitmapImageConverter.cs` | Bitmap/Mat conversion boundary |
| `Common/CAccountManager.cs` | `Common/Account/CAccountManager.cs` | account/session data |
| `Common/DefectList.cs` | `Common/Results/DefectList.cs` | defect result model |
| `Common/DefectListResult.cs` | `Common/Results/DefectListResult.cs` | defect result aggregate |
| `Common/EdgeLineList.cs` | `Common/Results/EdgeLineList.cs` | edge/line result model |
| `Common/IPropertyGridImageEditorService.cs` | `Common/PropertyGrid/IPropertyGridImageEditorService.cs` | PropertyGrid editor contract |
| `Common/IPropertyGridImageEditView.cs` | `Common/PropertyGrid/IPropertyGridImageEditView.cs` | PropertyGrid editor view contract |
| `Common/ParameterManager.cs` | `Common/PropertyGrid/ParameterManager.cs` | parameter/property-grid model |
| `Common/PropertyGridEditorFactory.cs` | `Common/PropertyGrid/PropertyGridEditorFactory.cs` | PropertyGrid editor composition |
| `Common/PropertyGridEditorRuntime.cs` | `Common/PropertyGrid/PropertyGridEditorRuntime.cs` | PropertyGrid editor runtime |
| `Common/PropertyGridEventBinder.cs` | `Common/PropertyGrid/PropertyGridEventBinder.cs` | PropertyGrid event lifetime |
| `Common/PropertyGridImageEditorService.cs` | `Common/PropertyGrid/PropertyGridImageEditorService.cs` | image editor adapter |
| `Common/PropertyGridToolPolicy.cs` | `Common/PropertyGrid/PropertyGridToolPolicy.cs` | PropertyGrid tool policy |
| `Common/RecipeModel.cs` | `Common/Recipe/RecipeModel.cs` | Recipe model |
| `Common/SerializeHelper.cs` | `Common/Persistence/SerializeHelper.cs` | atomic XML persistence |
| `Common/VisionEventArgs.cs` | `Common/Events/VisionEventArgs.cs` | shared event contracts |
| `Common/VisionMessageBox.cs` | `Common/MessageDialogs/VisionMessageBox.cs` | WPF message-dialog adapter |

All moved files keep their existing namespace and public type names. The SDK,
Recipe XML, XAML, and callback contracts therefore continue to resolve through
the same compiled symbols.

## Ownership and reading order

The physical navigation path is now:

```text
Common/Runtime       -> AppPathService, BackgroundLoopWorker
Common/Imaging      -> BitmapDrawing, BitmapImageConverter
Common/PropertyGrid -> editor contracts/runtime/policy
Common/Recipe       -> RecipeModel
Common/Persistence  -> SerializeHelper
Common/Results      -> DefectList/EdgeLineList result models
Common/Events       -> VisionEventArgs
Common/MessageDialogs -> VisionMessageBox
Common/Account      -> CAccountManager
Common root         -> AppCommon, CCommon, DEFINE (legacy shared surface)
```

For a new contributor, start with `docs/admin/CODEBASE_STRUCTURE.md`, then the
folder matching the responsibility. `AppPathService` remains the runtime path
owner; `BitmapImageConverter` remains the Bitmap/Mat conversion owner;
`SerializeHelper` remains the atomic XML writer. No new wrapper, interface,
partial, or manager was introduced.

## Verification

The following checks were run after the move:

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU" --no-restore`
- `dotnet build OpenVisionLab.sln -c Release -p:Platform="Any CPU" --no-restore`
- `OpenVisionReadinessCheck` Debug and Release source contracts
- `TestDocumentationIndex.ps1`
- `Invoke-RefactorAudit.ps1 -Verify`
- `git diff --check`

The readiness check's source path was updated to
`Common/Runtime/AppPathService.cs`. The project uses SDK default compile
globbing, so no project reference or compile item changed. A source search found
no remaining active tool or documentation path that points to a moved file; old
historical reports retain their original evidence paths by design.

Evidence inventory and path scan:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-r15-common-folder-audit-20260910`.

## Boundary

This slice changes physical source organization only. It does not rename the
legacy public namespaces, split `Common` into new projects, change SDK calls,
change Recipe/XML serialization, or alter WPF behavior. The known
`RoiImageCanvasViewModel` audit signal remains a separately documented
compatibility/debt boundary and was not reopened by this folder move.

Runtime UI interaction across alternate themes, Wide/Compact layouts, 100/125/
150/175/200% DPI, physical keyboard/pointer, camera/SDK, and long-run native
shutdown remains `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.
