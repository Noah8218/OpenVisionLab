# OpenVisionLab OVL-36 PropertyGrid generic metadata adapter — 2026-09-09

## Status

Complete for one independently verifiable generic PropertyGrid metadata
responsibility boundary. The overall refactoring program remains active.

## Scope

`WpfPropertyGridAdapter.cs` previously declared the generic metadata helpers
that order categories and properties, register the dynamic type-description
provider, filter hidden/range-companion rows, wrap localized descriptors, and
resolve localization keys. Those seven concrete types now live in
`PropertyGridMetadataAdapters.cs`. `PropertyGrid` remains the lifecycle and
state owner for the vendor control, selected object, hidden-property registry,
progressive viewport, navigation, editor registration, and event forwarding.

The application-specific `PropertyGridToolPolicy` and the existing OVL-20
`PropertyGridPropertyValueChangeSubscription` owner were reused and were not
reopened. No public attribute, WPG alias, Recipe/XML contract, Preview/Run
contract, Layer/ImageSpace contract, or visual style was changed.

## Refactor proof

- Current responsibility owner: `WpfPropertyGridAdapter.cs` namespace-level
  helper declarations mixed with the 3,000+ line control adapter.
- Current call path: `PropertyGrid.EnsurePropertyGridProvider` and
  `PropertyGrid.RegisterComparers` resolved helpers declared in the adapter
  file; the dynamic provider projected `TypeDescriptor` metadata through the
  same file.
- Current dependency direction: generic metadata policy and WPF control
  lifecycle were co-located; the metadata helpers reached the `PropertyGrid`
  state registry and localization service from that file.
- Current state/data owner: `PropertyGrid` owned hidden-property and progressive
  viewport state; helper classes only transformed descriptors and comparer
  inputs.
- New responsibility owner: `PropertyGridMetadataAdapters.cs` owns category
  ordering, property/category comparison, dynamic descriptor projection,
  localized descriptor delegation, and localization-key fallback.
- New call path: `PropertyGrid.EnsurePropertyGridProvider` ->
  `DynamicPropertyGridTypeDescriptionProvider` ->
  `DynamicPropertyGridTypeDescriptor` -> `PropertyGrid` hidden/viewport state
  plus `LocalizedPropertyDescriptor`/`PropertyGridLocalization`; selected
  object replacement still calls `BridgePropertyComparer` and
  `BridgeCategoryComparer`.
- New dependency direction: the adapter composes concrete metadata adapters;
  the extracted file consumes existing `PropertyGrid` state APIs, bridge
  attributes, vendor WPG item types, and `OpenVisionLanguageService`. It has
  no `PropertyGridToolPolicy` or `OpenVisionLab.Common` dependency.
- State/data owner remains `PropertyGrid`; no mutable state moved into the
  metadata file.

The whitespace-stripped metadata block token hash is unchanged from the
pre-extraction block: `fcd108d34b7a6a24204e3f9dc6ac60378206d7e7e6a09c49faad482713d56c0f`
(10,271 non-whitespace characters). The former adapter has zero declarations
of the seven moved types, and the new file has exactly one declaration of each.

## Verification

`PropertyGridMetadataAdapterContract` passed `7/7` in Debug and Release:

- one concrete owner file for all seven metadata types;
- pre-extraction token contract;
- adapter composition of provider and comparers;
- parent-descriptor call path;
- hidden-property, range-companion, and progressive-viewport delegation;
- localization and category-order behavior retained;
- application policy absent from the bridge.

Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl36-metadata-contract-debug2-20260909\`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl36-metadata-contract-release-20260909\`

The existing `PropertyGridPropertyValueChangeSubscriptionContract` regression
passed `8/8` in Debug and Release:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl36-property-grid-subscription-regression-debug-20260909\`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl36-property-grid-subscription-regression-release-20260909\`

Builds passed with zero warnings/errors:

- `dotnet build src/Libraries/WpfPropertyGridBridge/WpfPropertyGridBridge.csproj -c Debug -p:Platform=x64 --no-restore -m:1 -nr:false`
- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug -p:Platform=x64 --no-restore -m:1 -nr:false`
- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Release -p:Platform=x64 --no-restore -m:1 -nr:false`
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform=x64 --no-restore -m:1 -nr:false`
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release -p:Platform=x64 --no-restore -m:1 -nr:false`
- `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform=x64 --no-restore -m:1 -nr:false`
- `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform=x64 --no-restore -m:1 -nr:false`

The PipelineViewerScreenshotSmoke builds retain one pre-existing nullable
warning at `Program.cs:10139`; no warning was introduced by this slice.

The focused real WPF target `wpf_property_grid_matching_combo` passed in Debug
and Release. The Release run was launched after dynamic monitor detection with
two independent monitors. The smaller left monitor was selected:

- `\\.\DISPLAY2`, bounds `-1920,365,1920x1080`, working area
  `-1920,365,1920x1032`;
- observed window rectangle `-1900,385,-1140,905`, intersecting the selected
  monitor;
- screenshot:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl36-ui-property-grid-release2-20260909\wpf_property_grid_matching_combo.png`.

The capture showed the Matching PropertyGrid search, localized rows, combo
editor, range editor, and child-row presentation. Alternate themes,
Wide/Compact layouts, and 100/125/150/175/200% DPI rows remain environment
bound and are not claimed.

`Invoke-RefactorAudit.ps1 -Verify` passed:

`REFACTOR_AUDIT=PASS|CSharpFiles=815|XamlFiles=60|PartialDeclarations=110|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`

Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl36-refactor-audit-20260909\`.

## Junior developer assessment

**PASS for this boundary.** A junior maintainer can find generic metadata
ordering, filtering, descriptor wrapping, and localization in one named file,
then follow the explicit call sites back to `PropertyGrid` for state and vendor
lifecycle. The split uses concrete types and does not add an interface,
factory, wrapper, message bus, or partial type.

## No-repeat boundary

Do not recreate, rename, re-split, or move `PropertyGridToolPolicy`, the OVL-20
subscription owner, or the extracted metadata adapters without a newly
reproduced defect, changed explicit contract, or demonstrated responsibility
or dependency conflict. Do not split `WpfPropertyGridAdapter` by file size alone.

## Versioned Dev and Original promotion preparation

The selective Dev checkpoint is complete:

- Repository: `C:\Git\2D\Dev`
- Branch/remote: `codex/public-sample-ux-docs` /
  `https://github.com/Noah8218/OpenVisionLab_Dev.git`
- Product-version label: `[2.1.0]`
- Implementation commit: `638c8a36146a10152e8de475f35b214c9caa819d`
- Evidence commit: `22d216fa10686a96f6d397b2267cbb13147ed3da`, which is the latest
  `codex/public-sample-ux-docs` branch tip.
- Push verification: `git ls-remote origin refs/heads/codex/public-sample-ux-docs`
  returned `22d216fa10686a96f6d397b2267cbb13147ed3da`.
- The pre-existing dirty worktree was preserved; only the five OVL-36 files and
  the two runner dispatch lines were included in that commit.

Original promotion is prepared as a blocked boundary, with no Original mutation:

- Repository: `C:\Git\2D\Original`
- Branch/remote: `main` /
  `https://github.com/Noah8218/OpenVisionLab.git`
- Current HEAD and remote SHA: `604c7fb6fc5247198afd666b3e3f37241ac069ba`
- Current project version source: `2.2.0-dev` in `OpenVisionLab.csproj` and
  `README.md`; `GlobalState.AppVersion` still reports `2.1.0`.
- The Dev branch and Original `main` have no common verified refactor base, and
  Original does not contain the completed Dev OVL history. Original also has a
  pre-existing untracked `Temp.txt` that must remain untouched.
- Under the release policy, an Original push requires one exact source commit,
  target branch, changed-file allowlist, next canonical version, matching README
  history entry, and the clean release gates. Those inputs are not specified,
  so no version is guessed and no Original commit or push is claimed.

This boundary prevents bulk-copying OVL slices or repeating completed owners.
The next Original action requires the exact promotion source, version, target,
and allowlist to be supplied or explicitly selected.

## Next priority

Inspect one compatibility-safe residual namespace/project boundary candidate
or, if no candidate has a public/XML/reflection-safe migration path, record the
read-only finding and continue with the responsibility-based smoke-runner
cleanup | Recommended model: `gpt-6-astra` | Reasoning effort: `high`.
