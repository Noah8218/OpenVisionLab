# OpenVisionLab Source Structure Refactor Design

Date: 2026-07-24
Updated: 2026-07-26 KST
Repository: `C:\Git\OpenVisionLab_Dev`
Status: Complete; this historical design is superseded by
`OPENVISIONLAB_STRUCTURAL_REFACTORING_COMPLETION_20260726.md`

Final execution snapshot (2026-07-26):

- Root directory migration from numbered folders (`0. UI`, `1. Core`, `2. Common`, `4. Vision`, `5. Property`) to semantic names is complete in working tree; legacy numeric roots are no longer active references in source files.
- P1 dependency-direction cleanup completed its targeted Core decoupling:
  - `Core/Recipe/VisionToolRepository` no longer uses `VisionMessageBox` (UI dependency removed).
  - `Core/Pipeline/Definition/VisionPipelineAppendService` no longer reads recipe name from `PropertyGridEditorFactory`.
- `Core/Pipeline/Definition/VisionPipelineStepPropertyMapper` WPF/PropertyGrid migration is complete (`UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineStepPropertyMapper.cs`).
- The reviewed mapper families, Recipe application state/policies, Pipeline
  Review presentation, Learn calculations, and Auto MPoint teaching workflow
  now have the explicit owners listed in the canonical completion record.
- There is no active broad-refactor follow-up. Reopen only under the evidence
  prerequisites in the canonical completion record.

## 1. Purpose

This design identifies the smallest structural changes that materially improve
ownership, dependency direction, release hygiene, and change isolation while
preserving the current OpenVisionLab product contracts.

The current product remains an OpenCvSharp4 deterministic rule-based vision
recipe workbench. This work does not reopen LLM expansion, add an inspection
algorithm, change Preview/Run behavior, or change the product into an industrial
camera, PLC, I/O, account, or deployment platform.

## 2. Scope and acceptance boundary

Included:

- source and project inventory;
- folder and module ownership review;
- product/Test/Smoke boundary review;
- Core-to-WPF dependency review;
- large-file responsibility review;
- a phased destination structure;
- implementation acceptance and verification gates.

Excluded:

- moving or editing production source in this design slice;
- changing algorithms, UI behavior, XML, metrics, or saved data;
- touching `C:\Git\OpenVisionLab`;
- adding parallel image workers;
- renaming every folder merely for visual consistency.

Non-negotiable behavior:

- PropertyGrid remains the algorithm-tool editing surface;
- Preview and Run remain explicit actions;
- visibility, layer CRUD, image load, and output creation do not run the pipeline;
- output creation does not change the input layer;
- layer routing, viewer interaction, docking, Recipe Manager, Pipeline Review,
  template editing, and window chrome remain intact;
- old XML without newly added keys remains compatible;
- `OuterCornerIntersection` remains experimental.

## 3. Current evidence

### 3.1 Repository baseline

- Branch: `codex/public-sample-ux-docs`
- Source inventory: current active source tree is in semantic folders only:
  `UI`, `Core`, `Common`, `Vision`, `Property`, `Library`, and `tools`.
  Numeric legacy root names are not active source paths.
- C#/XAML size: 173,995 total lines (all active source inspected).
- Main source distribution:
  - `UI`: 353 files
  - `Library`: 184 files
  - `Core`: 60 files
  - `Common`: 22 files
  - `tools`: 18 project/source files plus scripts
  - `Vision`: 11 files
  - `Property`: 7 files
- Current Debug solution build: 0 warnings, 0 errors.
- Current readiness check: pass (all expected local checks in the current document baseline passed).
- Active root config/docs check (today):
  - Root docs/config/state files: `AGENTS.md`, `CHANGELOG.md`, `README.md`,
    `App.config`, `Directory.Build.props`, `global.json`, `log4net.config`,
    `OpenVisionLab.csproj`, `OpenVisionLab.sln`, `Program.cs`, `LICENSE`,
    `NOTICE`, `desktop.ini`.
  - Numeric legacy-root search command result:
    `rg --files | rg '(^|/)[0-9]+[\\)\\.]'` → no active matches in source.

### 3.4 Root organization and doc/config mixing check

- Domain documentation is now centralized under `docs/` with stable subfolders
  (`learn`, `evidence`, `admin`, `samples`, `assets`).
- Runtime/build artifacts remain in conventional folders (`dist`, `bin`, `obj`,
  `artifacts`, `tmp`, `scripts`) and are not mixed with docs.
- No structural document/config "scatter" issue remains in the root for active
  source; remaining cleanup is naming clarity and ownership boundaries in large files.

### 3.2 High-change files

In the most recent 50 commits, the most frequently changed source files were:

1. `OpenVisionShellHostRecipeCommandSurface.cs`: 35 commits
2. `OpenVisionLabDirectSmokeRunner.cs`: 34 commits
3. `OpenVisionShellHostView.xaml`: 32 commits
4. `tools\PipelineViewerScreenshotSmoke\Program.cs`: 31 commits

This confirms that the largest integration files are also active merge and
maintenance hotspots.

### 3.3 Largest responsibility surfaces

| File | Observed size/responsibility |
| --- | --- |
| `tools\PipelineViewerScreenshotSmoke\Program.cs` | About 25,600 non-empty lines and 455 methods; shell, recipe, tool, Learn, review, capture, and assertion scenarios |
| `OpenVisionLabDirectSmokeRunner.cs` | About 11,200 non-empty lines and 188 methods; compiled into the main executable |
| `OpenVisionShellHostView.xaml` | About 8,000 non-empty lines; shell, hidden legacy rail, Recipe Manager, and status UI |
| `OpenVisionShellHostRecipeCommandSurface.cs` | About 6,950 non-empty lines and 230 methods; active composition/orchestration boundary |
| `tools\VisionRecipeRunnerSmoke\Program.cs` | About 6,100 non-empty lines; unrelated evidence campaigns in one runner |
| `OpenVisionLearnWindow.xaml.cs` | More than 5,000 physical lines; multiple independent Learn topics and animation state machines |
| `VisionPipelineStepPropertyMapper.cs` | About 3,300 non-empty lines; 46 dispatch cases, bidirectional conversion, PropertyGrid models, WPF attributes, and static selection context |

File size alone is not a reason to split a file. The proposed changes below
require a distinct owner, call path, state owner, and verification boundary.

## 4. Findings and decisions

### 4.0 User-observed mix perception (docs/config)

현재 기준으로 "숫자 폴더 이름" 및 "문서/설정 산재"는 이미 대폭 정리되어 있으며,
남은 작업은 기능성 리스크가 없는 범위에서의 정리(명확한 책임 경계 분리)로 진행할 수 있다.

### P0. Product executable contains the direct smoke system

Current path:

```text
Program.Main
  -> OpenVisionLabDirectSmokeRunner.TryRun
      -> 188 smoke/helper methods
      -> UI automation, native mouse input, evidence generation, LLM history,
         matching campaigns, docking scenarios, and tutorial capture
```

Evidence:

- `OpenVisionLabDirectSmokeRunner.cs` is a root SDK Compile item.
- `Program.Main` calls it before normal application startup.
- the main project excludes `tools\**`, but it does not exclude the root runner;
- Release configurations also compile the same root file;
- the file has no conditional compilation boundary around the runner.

Decision:

- This is the first implementation priority.
- Preserve current-source actual-EXE smoke evidence in a dedicated smoke-enabled
  Debug build.
- Remove smoke code from normal Debug and all Release Compile items.
- Do not immediately replace the current smoke mechanism with a new test
  framework.

Target path:

```text
Normal Debug/Release
  Program.Main -> product bootstrap only

Explicit smoke-enabled Debug
  Program.Main
    -> thin conditional smoke entry
    -> tools/OpenVisionLab.DirectSmoke/Embedded/DirectSmokeScenarioRegistry
       -> scenario-family files
```

State owner:

- product bootstrap owns only application lifetime;
- the smoke runner owns scenario selection, automation, and artifact state;
- a build property such as `OpenVisionLabEnableEmbeddedSmokeRunner=true` owns
  the decision to compile the embedded runner.

Removed coupling:

- normal product builds no longer contain test campaigns, native mouse helpers,
  evidence writers, or historical LLM smoke code;
- normal startup no longer knows the smoke scenario implementation type.

Acceptance:

- Release and normal Debug `Compile` item lists exclude every direct-smoke
  scenario file;
- the explicit smoke build retains the current scenario-name list;
- `Program.Main` starts the product unchanged without the smoke build property;
- representative recipe, docking, and tutorial scenarios pass from a freshly
  built smoke-enabled Debug EXE;
- current Debug/Release builds and readiness pass.

### P1. Pipeline PropertyGrid mapper moved out of Core

Current path:

```text
Recipe Manager host
  -> static VisionPipelineStepPropertyMapper (legacy file moved)
      -> 46-case tool dispatch
      -> nested PropertyGrid models and WPF editor attributes
      -> static layer/geometry/point accessors
      -> VisionPipelineStep parameter dictionary
```

Evidence:

- `VisionPipelineStepPropertyMapper.cs` is now owned by `UI/Menu/Wpf/Recipe/PropertyGrid`.
- `Core` no longer has direct `System.Windows.Controls.WpfPropertyGrid`/`VisionMessageBox`/`PropertyGridEditorFactory` ownership for this contract.
- Its production caller remains Recipe Manager command surface; smoke/readiness tools consume it as contract consumers.

Current status:

1. ? Completed: move from `Core/Pipeline/Definition/VisionPipelineStepPropertyMapper.cs` to `UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineStepPropertyMapper.cs`.
2. ?? Next: split by tool family into adapters with explicit instance context.
3. Pass layer and typed-feature choices through context object; remove mutable static delegates.

Target path (target design):

```text
UI/Menu/Wpf/Recipe/PropertyGrid/
  VisionPipelinePropertyEditorService.cs
  VisionPipelinePropertyContext.cs
  Adapters/
    BasicImageStepPropertyAdapter.cs
    ObjectAndLineStepPropertyAdapter.cs
    GeometryStepPropertyAdapter.cs
    MatchingStepPropertyAdapter.cs
    TransformStepPropertyAdapter.cs
  Models/
    ...PropertyGrid-only editor models...
```

State owner:

- Recipe Manager owns selected pipeline/step state, current layer list, and compatible feature lists.
- Mapper service uses context object for state-dependent filtering and validation.
- Pipeline definition and validator remain in Core.

Removed coupling:

- `Core` has no direct WPF PropertyGrid dependency in this contract.
- Tool-family edits no longer require touching one 3,300-line class.
- Tests can focus on one adapter without constructing the full host.

Acceptance:

- no `System.Windows`, WPF PropertyGrid, `VisionMessageBox`, or
  `PropertyGridEditorFactory` reference remains under `Core` for this owner;
- every supported canonical/alias ToolType still creates and applies the same parameters;
- Line A/B persistence and geometry/point dropdown semantics remain unchanged;
- Recipe XML compatibility, readiness, and focused recipe-manager smoke remain.

### P2. Shell XAML contains an independent Recipe Manager application surface

Current path:

```text
OpenVisionShellHostView.xaml
  -> shared styles/resources
  -> shell header/tool rail/workspace/docked inspector/log
  -> hidden legacy host rail
  -> Recipe Manager overlay with 10 tabs and about 5,600 lines
  -> status bar
```

Evidence:

- the Recipe Manager starts around line 2,335 and ends near line 7,980;
- it has its own title bar, drag state, lifecycle strip, ten tabs, PropertyGrid
  host, WebView, preview, reports, history, and validation controls;
- the hidden legacy rail still contains a preview control used by host code, so
  it cannot be deleted safely as dead XAML.

Decision:

1. Extract shell-only resources into a Shell resource dictionary.
2. Migrate any still-used legacy-rail state to the current layer/workspace owner;
   delete the hidden rail only after its remaining references are zero.
3. Extract Recipe Manager to its own view after its named-control surface is
   reduced to typed commands/events and a few explicit hosts.
4. Keep `OpenVisionShellHostRecipeCommandSurface` as the integration boundary
   unless a complete action family has a proven new owner. Do not split it into
   arbitrary partial files merely to reduce line count.

Target path:

```text
UI/Menu/Wpf/
  Shell/
    Views/OpenVisionShellHostView.xaml
    Chrome/OpenVisionShellTheme.xaml
  Recipe/
    Views/OpenVisionRecipeManagerView.xaml
    Views/OpenVisionRecipeManagerView.xaml.cs
    PropertyGrid/...
    Context/...
    Models/...
    Review/...
    Validation/...
```

Target call/state path:

```text
Shell host
  -> Recipe Manager view host
      -> existing Recipe command model/presenters
      -> explicit callbacks for file dialogs, clipboard, viewer, and execution
```

State owner:

- Shell owns window/chrome/docking/layer integration and overlay placement;
- Recipe Manager owns tab selection, recipe editor presentation, and recipe
  control state;
- existing presenters retain validation, history, readiness, and review state;
- pipeline execution remains an explicit host callback.

Removed coupling:

- shell layout changes do not require editing all Recipe tabs;
- Recipe Manager layout can be captured and tested independently;
- hidden legacy state is removed only after a proven replacement;
- style changes stop inflating the functional view file.

Acceptance:

- fresh before/after captures exist for Shell, all Recipe Manager tabs, and the
  movable panel;
- Recipe Manager open/close/drag, PropertyGrid, WebView placeholder, Pipeline
  Review handoff, report/history, and lifecycle commands pass;
- tab selection and visibility changes produce zero Preview/Run;
- layers, active layer, routes, docking, and viewer state remain unchanged.

### P3. Smoke programs need scenario-family ownership

Current:

- `PipelineViewerScreenshotSmoke\Program.cs`: 455 methods, with 167 capture and
  117 assertion methods;
- `VisionRecipeRunnerSmoke\Program.cs`: AutoMPoint, affine, object dimension,
  PinArrayGap, report generation, and batch drawing in one file;
- the direct runner mixes current product contracts with historical campaigns.

Decision:

- keep the existing executable and command-line contracts;
- use a simple scenario-name-to-delegate registry;
- move scenarios into family files with one shared helper owner;
- do not add a general test framework or parallel workers in this phase.

Target path:

```text
tools/
  Smokes/
    OpenVisionLab.DirectSmoke/
      Program.cs
      DirectSmokeScenarioRegistry.cs
      Scenarios/{Shell,Recipe,Docking,Learn,Matching,LlmMaintenance}.cs
      Support/{UiAutomation,Artifacts,Images}.cs
    PipelineViewerScreenshotSmoke/
      Program.cs
      Scenarios/{Shell,Recipe,PipelineReview,NativeTools,Learn}.cs
      Support/{Capture,Assertions,Artifacts}.cs
    VisionRecipeRunnerSmoke/
      Program.cs
      Scenarios/{AutoMPoint,Affine,ObjectDimension,PinArrayGap}.cs
      Support/{Batch,Drawing,Reports}.cs
```

State owner:

- `Program` parses arguments and dispatches only;
- each scenario family owns its inputs and assertions;
- shared support owns files, capture primitives, and image helpers, but not
  scenario decisions.

Acceptance:

- command names, arguments, exit codes, and target-list output are unchanged;
- selected representative scenarios produce the same logical reports and
  expected artifacts;
- readiness checks no longer depend on one monolithic file path when checking
  contracts;
- no scenario moves into production Release Compile items.

### P4. Learn window should be split only by real topic state machines

Current:

- one window contains multiple independent animation timers, raster/grid
  builders, event handlers, and algorithms for Threshold, brightness,
  arithmetic, filter, morphology, Blob, Contour, line, matching, metrics,
  layers, geometry, and HSV.

Decision:

- this is a valid later refactor, not an immediate priority;
- split by topic view/presenter only when the next Learn change is approved;
- keep the main Learn window responsible for topic navigation and document/tool
  handoff only;
- do not introduce a generic animation framework before two topic extractions
  prove a shared contract.

Acceptance:

- all Learn topics open from the same tool routes;
- play/step/reset and localized text remain identical;
- closing the window releases every timer and image;
- fresh before/after captures pass for each moved topic;
- no Learn interaction runs the production pipeline.

### P5. Numbered legacy names are historical only

Current names such as `0. UI`, `1. Core`, `2. Common`, `4. Vision`, `5. Property`,
`0) MENU`, and `6) Vision Test` are historical migration labels, and may appear in
historical handoff/design evidence. They are not active reference targets in current
source code ownership.

Decision:

- do not mass-rename roots before P0-P3;
- eliminate the generic `Common` ownership by moving files to a real owner;
- preserve existing independent `Library\*.csproj` projects;
- perform a final physical root migration only as a separately approved,
  mechanical change after dependency contracts pass.

Proposed final physical layout:

```text
src/
  OpenVisionLab/
    App/
      Program.cs
      Composition/
      Configuration/
    Core/
      State/
      Recipe/
      Pipeline/
        Definition/
        Execution/
        Validation/
        Tools/
    Infrastructure/
      Paths/
      Serialization/
      Storage/
    Vision/
      Imaging/
      Properties/OpenCv/
    Presentation/Wpf/
      Shell/
      Recipe/
      PipelineReview/
      Workspace/
      VisionTools/
      Learn/
      Viewer/
      Docking/
      Windows/
Library/
  ...existing independent projects...
tools/
  Checks/
  Smokes/
  Generators/
docs/
artifacts/
```

Initial `2. Common` move map:

| Current responsibility | Target owner |
| --- | --- |
| path resolution | `Infrastructure/Paths` |
| serialization | `Infrastructure/Serialization` |
| bitmap drawing/template extraction | `Vision/Imaging` |
| PropertyGrid editor factory/runtime/services | `Presentation/Wpf/PropertyGrid` |
| message dialogs | `Presentation/Wpf/Dialogs` |
| live recipe/defect/edge models | the specific `Core/Recipe` or `Core/Pipeline` owner |
| unreferenced legacy models | delete only after compile and runtime evidence |

Initial root move map:

| Current | Target |
| --- | --- |
| `Core` | `src/OpenVisionLab/Core` |
| `Vision\OpenCV` | `src/OpenVisionLab/Vision/Properties/OpenCv` |
| `Property` | split between App configuration and WPF PropertyGrid owners |
| `UI/Menu/Wpf` | `src/OpenVisionLab/Presentation/Wpf` |
| `UI/VisionTest` | `src/OpenVisionLab/Presentation/Wpf/VisionTools` |

The final root migration does not create new assemblies by itself. A separate
Core project should be considered only after the no-WPF/no-dialog/no-static-UI
dependency gate has passed and a measured build or reuse benefit justifies it.

## 5. Files intentionally not selected for immediate splitting

The following files are large but currently cohesive enough to retain:

- `VisionPipelineGapEdgePairTool.cs`: candidate detection, pair scoring, metrics,
  and semantic drawings implement one bounded algorithm contract;
- `VisionPipelineGeometryMeasureService.cs`: feature resolution, seven geometry
  modes, fail-closed validation, metrics, and drawings share one execution
  contract;
- `VisionPipelineFixtureFrameService.cs`: producer/consumer definition,
  normalization, frame publication, validation, and metrics are one fixture
  contract;
- `OpenVisionPipelineReviewView.xaml.cs`: most code is view-local selection,
  hit-testing, drawing, and event bridging;
- `OpenVisionShellHostRecipeCommandSurface.cs`: previous P95-P103 work already
  extracted several presenters; the remaining file is an active integration
  boundary. Extract only a complete owner, not isolated helper methods.

They can be reconsidered when a concrete defect, repeated edit conflict, or
independent test boundary appears.

## 6. MVVM readability and immediate refactor cuts (requested)

User-requested readability goals are explicit ownership, reduced file intent mixing,
and predictable state flow while keeping behavior stable.

| File | Why it is hard to understand today | Refactor target |
| --- | --- | --- |
| `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs` | Runtime bootstrap combines scenario discovery, orchestration, native input simulation, artifact writing, and assertions in one path | Split into dispatch entry + scenario families + shared artifact helpers |
| `UI\Menu\Wpf\OpenVisionShellHostView.xaml` | Shell, Recipe Manager, and legacy control state are mixed in one visual surface | Extract Recipe Manager and legacy-rail replacement views into dedicated `Views` with dedicated `ViewModel`/Presenter contracts |
| `UI\Menu\Wpf\OpenVisionShellHostView.xaml.cs` | View lifecycle logic is coupled to orchestration and command decisions | Keep as shell wrapper code-behind only; move orchestration to command presenters and a dedicated shell coordinator |
| `UI\Menu\Wpf\OpenVisionShellHostRecipeCommandSurface.cs` | High cognitive load by mixing recipe, sample, layer, and execution flows | Split by complete owner only (recipe workflow, sample/workspace workflow, execution/preview workflow) after behavior evidence is preserved |
| `UI\VisionTest\Wpf\Learn\OpenVisionLearnWindow.xaml.cs` | One class owns many independent topic state machines and animation/interaction branches | Keep host as selector; move one topic at a time into dedicated topic presenter/controller classes only when a topic change is active |
| `UI\Menu\Wpf\Recipe\PropertyGrid\VisionPipelineStepPropertyMapper.cs` | One class performs all tool-family conversions and context filtering | Keep `P1` decomposition and move to tool-family adapters behind a context model |
| `tools/PipelineViewerScreenshotSmoke/Program.cs`, `tools/VisionRecipeRunnerSmoke/Program.cs` | Scenario and support logic are tightly coupled in executable roots | Split into `Scenarios` + `Support` modules under a stable CLI contract |

### MVVM 가독성 분해 우선순위(사용자 요청 반영)

| Slice | 제약/대상 | 핵심 분해 목표 | 기대 효과 |
| --- | --- | --- | --- |
| P0 | `Program` + `OpenVisionLabDirectSmokeRunner` | 조건부 컴파일된 smoke 호스트 + 시나리오 레지스트리 | 제품 빌드에서 smoke 코드 분리 |
| P1 | `UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineStepPropertyMapper.cs` | 툴 패밀리별 어댑터 분해 | 매퍼 책임 분산, 매핑 회귀 영향 구간 축소 |
| P2 | `UI/Menu/Wpf/OpenVisionShellHostView.xaml` + `...RecipeCommandSurface.cs` | Shell-Recipe 이원 분리, Recipe Manager 뷰/컨트롤러 경계 정리 | 화면 변경의 책임 추적성 향상 |
| P3 | `tools/PipelineViewerScreenshotSmoke/Program.cs`, `tools/VisionRecipeRunnerSmoke/Program.cs` | 시나리오별 모듈 + 공용 지원 모듈 | 런너 유지보수성 향상, 시나리오 추가/수정 비용 감소 |

Target refactor conventions (for active new work):

- `View` owns XAML and binding-only events.
- `ViewModel` owns feature state and command enablement.
- `Presenter/Controller` owns orchestration and non-trivial side effects.
- `Document` owns persistence and load/save contracts.
- Any non-feature helper that is not lifecycle-critical should live in `Support` helpers.

Acceptance pattern for each MVVM cut:

1. No observed behavior regression for existing user paths.
2. No Preview/Run side-effect from view/model migration itself.
3. zero unintended layer/routing/active-layer changes from state migration.
4. focused before/after evidence for changed UI surfaces.

## 7. Implementation sequence

1. Release/smoke compile isolation.
2. Move Pipeline PropertyGrid mapping out of Core.
3. Remove the remaining Core-to-dialog/static-UI dependencies.
4. Split mapper ownership by tool family and remove static context.
5. Split smoke programs by scenario family.
6. Extract Shell resources, retire the proven-unused legacy rail, and extract
   Recipe Manager view.
7. Split Learn topics only when a concrete Learn change is active.
8. Optionally normalize physical root names in one mechanical, reviewed change.

Do not combine steps 1, 2, 5, and 6 into one commit. Each changes a different
verification boundary and must be independently reversible.

## 8. Verification matrix

Every implementation slice must run the smallest relevant focused checks plus
the final shared checks.

Shared checks:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --no-build --project `
  "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" `
  -c Debug -- "C:\Git\OpenVisionLab_Dev"
git diff --check
```

Additional checks by slice:

| Slice | Required evidence |
| --- | --- |
| Smoke compile isolation | Debug/Release MSBuild Compile-item proof, scenario-list parity, three representative smoke runs |
| Property mapper | all ToolType round trips, Line pair persistence, geometry/point dropdowns, zero-Run UI smoke |
| Core dependency cleanup | source rule proving no WPF/dialog/static UI under Core |
| Shell/Recipe view | fresh before/after screenshots, all Recipe tabs, drag/open/close, layer/route/Preview/Run counters |
| Smoke decomposition | identical CLI names/exit codes and representative report/artifact comparison |
| Learn split | per-topic play/step/reset, localization, close/dispose, fresh visual evidence |
| Root migration | solution/project references, scripts, readiness source paths, current docs links, full build |

## 9. Recommended implementation priorities

1. Isolate direct smoke code from normal product builds
   Recommended model: `GPT-5.3-Codex`
   Reasoning effort: `high`
2. Move and decompose Pipeline PropertyGrid mapping; remove Core UI dependencies
   Recommended model: `GPT-5.3-Codex`
   Reasoning effort: `high`
3. Split smoke runners by scenario family without changing CLI contracts
   Recommended model: `codex-mini-latest`
   Reasoning effort: `medium`
4. Extract Shell resources and Recipe Manager view with visual/state proof
   Recommended model: `GPT-5.3-Codex`
   Reasoning effort: `high`
5. Split Learn topics when a concrete Learn change is active
   Recommended model: `GPT-5.3-Codex`
   Reasoning effort: `high`
6. Normalize numbered root folders after the previous gates pass and the user
   explicitly approves the broad path move
   Recommended model: `GPT-5.3-Codex`
   Reasoning effort: `medium`

There is no separate product-feature priority after P235. These are
user-requested maintenance priorities; they do not authorize new feature or
inspection-validation work.

## 10. Durable closure record

Status: In-progress (design slice)
Scope: Current-source structure/refactor audit and phased target design only
Acceptance criteria:

- current structure and largest responsibility surfaces inventoried: pass;
- actual production/test and Core/WPF coupling verified: pass;
- intended owners, call paths, state owners, removed couplings, and gates
  specified for each selected phase: pass;
- current build/readiness baseline recorded: pass.

Verification:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`:
  pass, 0 warnings, 0 errors;
- `OpenVisionReadinessCheck`: pass, all 12 contracts.

Evidence:

- this document;
- current files and MSBuild Compile-item output checked on 2026-07-24.

Boundary / next dependency:

- no source refactor has been implemented or visually tested;
- implementation should begin with P0 only after the user approves that slice;
- the final broad root rename requires separate explicit approval after P0-P3.
