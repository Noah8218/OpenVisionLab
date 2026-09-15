# ROI Image Canvas path-policy boundary

Date: 2026-09-14 (KST)  
Issue: `PL-0033`  
Scope: one scheduled Partial-refactoring slice in `C:\Git\2D\Dev`

## Result

The existing `ImageCanvasDirectoryPolicy` now owns image-name extraction and
default PNG filename sanitization. `RoiImageCanvasViewModel` still owns the
mutable canvas/session state, but no longer imports or calls `System.IO` path
APIs. No new service, interface, wrapper, registry, or Partial was added.

This is a narrow responsibility move, not a claim that the whole canvas
ViewModel can be split safely. The ViewModel still owns the shared
`ImageCanvasControl`, ROI state, input-controller callbacks, timer, Mat
lifetime, and public compatibility facade; those responsibilities have no
independent state/lifetime seam in the current call graph.

## Owner and call-path proof

| Concern | Current owner before slice | Intended/current owner | Evidence |
| --- | --- | --- | --- |
| Image filename stem from a selected path | `RoiImageCanvasViewModel` | `ImageCanvasDirectoryPolicy.ResolveImageName` | `LoadImage(Mat, ...)` and `LoadImage(Bitmap, ...)` delegate to the policy |
| Default save filename and invalid-character replacement | private `RoiImageCanvasViewModel.CreateDefaultSaveFileName` | `ImageCanvasDirectoryPolicy.CreateDefaultSaveFileName` | `OnSaveIamge` calls the policy before `IImageCanvasDialogHost.ShowSaveImageDialog` |
| Current image name mutable state | `RoiImageCanvasViewModel._currentImageName` | unchanged; ViewModel writes it from the policy result | `LoadImage` overloads remain the only image-name assignment paths |
| Image/ROI/native resource lifetime | `RoiImageCanvasViewModel` | unchanged | `LoadImage`, `ClearImage`, and `Dispose` retain Mat/view/timer ownership |

The resulting path is:

```text
consumer -> RoiImageCanvasViewModel.LoadImage(...)
         -> ImageCanvasDirectoryPolicy.ResolveImageName
         -> RoiImageCanvasViewModel._currentImageName

OnSaveIamge -> ImageCanvasDirectoryPolicy.CreateDefaultSaveFileName
            -> IImageCanvasDialogHost.ShowSaveImageDialog
            -> RoiImageCanvasViewModel.SaveCurrentImage
            -> CanvasImageSaver
```

The WPF binding/public contract is unchanged: `RoiImageCanvasView` still
receives the existing DataContext, hosts `ImageViewer`, and forwards the same
commands. `OpenVisionBitmapCanvasPresenter`,
`ImageCanvasExternalConsumerSession`, `VisionToolOpenGlPreviewCanvasAdapter`,
and `OpenGlTemplateEditorWindow` retain their existing construction,
load, and `Dispose` paths.

## Shortest code-reading route

1. `src/Libraries/OpenVisionLab.ImageCanvas/View/RoiImageCanvasView.xaml.cs`
   — DataContext attach/detach and WPF host boundary.
2. `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs`
   — public canvas facade, mutable state writer, Mat/timer lifetime, and
   command call path.
3. `src/Libraries/OpenVisionLab.ImageCanvas/Util/ImageCanvasDirectoryPolicy.cs`
   — initial directory, remembered path, image stem, and save-name policy.
4. `src/Libraries/OpenVisionLab.ImageCanvas/Util/CanvasImageLoader.cs` and
   `CanvasImageSaver.cs` — image I/O owners.
5. `tools/VisionRecipeRunnerSmoke/RoiImageCanvasPathPolicyContract.cs` —
   source and behavior contract for this slice.

## Focused verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\roi-image-canvas-path-policy-20260914`

- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj --configuration Debug --no-restore --nologo` — passed, 0 warnings/errors.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj --configuration Release --no-restore --nologo` — passed, 0 warnings/errors.
- `dotnet run ... --roi-image-canvas-path-policy-contract ...` — passed 6/6 in Debug and Release.
- Existing `--roi-image-canvas-boundary-contract` — passed 6/6 in Debug and Release.
- `dotnet build OpenVisionLab.sln --configuration Debug/Release --no-restore --nologo` — both passed, 0 warnings/errors.
- `OpenVisionReadinessCheck` Debug/Release — both passed.
- `Invoke-RefactorAudit.ps1 -Verify` — passed (`CSharpFiles=827`, `XamlFiles=57`, `PartialDeclarations=54`, `PartialTextMatches=0`, `ViewModelUiIoFiles=1`, `ProjectCycles=0`, `ShellStorageCalls=0`).
- `TestDocumentationIndex.ps1 -RepoRoot C:\Git\2D\Dev` — passed (`IndexedPaths=301`, `Routes=17`, `RootRedirects=102`).
- `git diff --check` — passed.

## Boundary and remaining risk

The contract proves source ownership and pure filename behavior only. It does
not prove WPF visual states, DPI/monitor placement, OpenGL rendering,
WindowsForms hosting, camera/SDK behavior, or long-running native runtime.
Those remain `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

The next scheduled review must not split `RoiImageCanvasViewModel` by file
length. It should first establish a new independent state/lifetime/test seam;
without that evidence, retain the existing owner and continue with the next
unprotected Tool/Learn Partial family.
