# OpenVisionLab OVL-37 — Template image extraction namespace boundary

Date: 2026-09-09 KST
Repository: `C:\Git\2D\Dev`
Branch: `codex/public-sample-ux-docs`
Implementation commit: `13d3a0557def81ef7e0b037969993e9bb85f5739`

## Scope

This slice moved the internal `TemplateImageExtraction` owner from the broad
`OpenVisionLab` namespace under `src/OpenVisionLab/Common` to the existing
`OpenVisionLab.Property` responsibility boundary under
`src/OpenVisionLab/Property`.

The owner remains an `internal static` type. Its implementation tokens are
unchanged apart from the namespace declaration: the before/after normalized
non-whitespace SHA-256 is
`4708ab6fdb3c3e4595c10c6d6d65f27e4ea4b2b43471d142c006f19869efc319` and both
versions contain 2,249 non-whitespace characters.

The explicit callers are still limited to the same template-editing workflow:

- `src/OpenVisionLab/Common/PropertyGridImageEditorService.cs`
- `src/OpenVisionLab/UI/Popup/Wpf/OpenGlTemplateEditorWindow.xaml.cs`
- `tools/LocatorRelativeBlobSkillSmoke/Program.cs`
- `tools/PipelineViewerScreenshotSmoke/Program.cs`

The boundary contract also includes the owner file itself when enumerating
references. No Recipe/XML qualified type, Preview/Run command, Layer/ImageSpace
route, public API, reflection contract, serializer contract, or XAML binding
changed.

## Structural proof

The former `Common/TemplateImageExtraction.cs` path is absent, the new owner
declares `namespace OpenVisionLab.Property`, and the owner remains internal and
static. The contract checks that it has no `Type.GetType`, `XmlSerializer`,
`XmlRoot`, or XAML reference. The reference set is exactly the four production
or smoke callers above plus the new owner file.

This is a real namespace/project boundary change: the former broad namespace
does not retain the owner, callers import the responsibility namespace
explicitly, and no partial file, interface, factory, wrapper, or message bus was
added.

## Verification

- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj --configuration Debug --no-restore`: pass, 0 warnings, 0 errors.
- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj --configuration Release --no-restore`: pass, 0 warnings, 0 errors.
- `dotnet build tools/LocatorRelativeBlobSkillSmoke/LocatorRelativeBlobSkillSmoke.csproj --configuration Debug/Release --no-restore`: pass, 0 warnings, 0 errors in both configurations.
- `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj --configuration Debug/Release --no-restore`: pass, 0 warnings, 0 errors in both configurations.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj --configuration Debug/Release --no-restore`: pass, 0 warnings, 0 errors in both configurations.
- `dotnet run --project tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj --configuration Debug --no-build -- --namespace-project-boundary-contract D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl37-template-extraction-boundary-debug-20260909`: pass, 12/12.
- Same namespace boundary contract in Release: pass, 12/12.
- `tools/RefactorAudit/Invoke-RefactorAudit.ps1 -Verify`: pass (`CSharpFiles=815`, `XamlFiles=60`, `PartialDeclarations=110`, `ViewModelUiIoFiles=1`, `ProjectCycles=0`, `ShellStorageCalls=0`).
- `git diff --cached --check`: pass before commit.

Contract evidence files:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl37-template-extraction-boundary-debug-20260909\namespace-project-boundary-contract.txt`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl37-template-extraction-boundary-release-20260909\namespace-project-boundary-contract.txt`

No new WPF visual behavior was introduced, so no new EXE UI capture was
required for this namespace-only slice. Existing UI and template behavior is
covered by the successful application and smoke builds.

## Dev and Original repository state

The Dev commit `13d3a0557def81ef7e0b037969993e9bb85f5739` was pushed to
`origin/codex/public-sample-ux-docs`; the remote SHA was verified equal to the
local HEAD.

Original `C:\Git\2D\Original` remains on `main` at
`604c7fb6fc5247198afd666b3e3f37241ac069ba` (`origin/main`) and does not contain
the Dev commit. The Dev SHA is not present in the Original object database, so
the repositories have no directly usable common commit for fast-forward or
ordinary cherry-pick preparation. Original also retains the pre-existing
untracked `Temp.txt`, which was not touched.

No Original commit or push was performed. The Original repository reports
`2.2.0-dev` in its current README/project sources while another existing source
still reports `2.1.0`; an exact next version, canonical version owner, source
commit, changed-file allowlist, and conflict-resolution target must be agreed
before an Original versioned change set can be created or pushed.

Merge preparation was read-only. The full six-file patch check against Original
failed because Original does not contain
`tools/LocatorRelativeBlobSkillSmoke/Program.cs` or
`tools/VisionRecipeRunnerSmoke/NamespaceProjectBoundaryContract.cs`. A filtered
production/existing-file patch was written to
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl37-original-merge-preparation-existing-files.patch`
and `git -C C:\Git\2D\Original apply --check --whitespace=nowarn` passed for
the five files that exist there. This is a prepared manual promotion boundary,
not an Original merge or push.

## No-duplicate rule and next priority

Do not recreate, rename, re-split, or move `TemplateImageExtraction` again
without a newly reproduced defect, changed explicit contract, or demonstrated
responsibility/dependency conflict. Do not broaden this slice into a bulk
namespace rename.

Next priority: inspect one responsibility-based smoke-runner cleanup candidate
after confirming that it does not overlap dirty user work | Recommended model:
`gpt-5.4-mini` | Reasoning effort: `medium`.

Junior developer assessment: **PASS for this boundary**. The file path,
namespace, owner visibility, and four caller imports now describe the template
image-editing dependency direction directly.
