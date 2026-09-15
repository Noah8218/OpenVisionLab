# OpenVisionLab OVL-09 Learn topic-selection/presentation composition — 2026-09-08

## Status

Complete for one independently verifiable topic-presentation boundary. This
slice did not reset, clean, stage, commit, push, touch the Original checkout,
release, or deploy.

## Scope and ownership

Before this slice, `OpenVisionLearnWindow.xaml.cs` used a long chain of
topic-index conditions to decide the selected topic title, subtitle, practice
text, panel visibility, practice-expander state, and guide refresh calls. The
same View also applied those decisions to its child Views and controls.

`OpenVisionLearnTopicPresentationPolicy` now resolves the normalized catalog
entry into immutable `OpenVisionLearnTopicPresentation` state. The policy is
WPF-independent and owns only topic presentation decisions. The Window remains
the View owner: it applies title/subtitle/practice/panel/expander state to
controls, calls the existing topic View `SelectTopic` methods, and dispatches
guide flags to existing View/Presenter methods. Topic-specific UI and
calculation ownership was not repartitioned.

The existing `OpenVisionLearnTopicCatalog`, Recipe/XML contracts, explicit
Preview/Run behavior, Layer/ImageSpace routing, SDK calls, and public test
facade remain unchanged.

## Call path

`TopicList_SelectionChanged` -> `UpdateSelectedTopic` ->
`OpenVisionLearnTopicPresentationPolicy.Resolve` -> Window applies immutable
presentation state -> existing topic View selection -> existing guide
View/Presenter refresh methods.

## Changed files

- `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnTopicPresentationPolicy.cs`
  - New WPF-independent policy, immutable presentation state, and guide-update
    flags for all 17 catalog topics.
- `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnWindow.xaml.cs`
  - Replaced inline topic decision chain with policy resolution and a small
    state-application method; existing child View/Presenter calls are reused.
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`
- `docs/admin/CODEBASE_STRUCTURE.md`
- `docs/admin/OPENVISIONLAB_DOCUMENTATION_MAP.md`
- `docs/LLM_DOCUMENT_INDEX.json`
- `docs/reports/OPENVISIONLAB_REFACTOR_PROGRAM_AUDIT_20260908.md`

## Verification evidence

Evidence root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl09-topic-composition-20260908`.

- `topic-policy-contract\run-output\topic-policy-contract.txt`:
  `TOPIC_POLICY_CONTRACT=PASS`. The pure contract checks invalid-index
  fallback, exact title/subtitle/practice strings, panel flags, expander
  state, guide flags, animation legend, and all 17 catalog topics.
- `topic-window-contract\run-output\topic-window-contract.txt`:
  `TOPIC_WINDOW_CONTRACT=PASS`. The STA WPF harness opens the actual Learn
  Window, selects every topic, and checks the rendered named controls, child
  View visibility, panel state, expander, legend, title, subtitle, and
  practice text against the policy.
- `structure-proof.txt`:
  `TopicPolicyStructureProof=True`, with no WPF references in the policy and
  the Window using the policy/catalog call path.
- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj --configuration Debug --no-restore -p:Platform=x64`:
  passed with 0 warnings and 0 errors.
- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj --configuration Release --no-restore -p:Platform=x64`:
  passed with 0 warnings and 0 errors.
- `dotnet build tools/ImageCanvasExternalConsumerSmoke/ImageCanvasExternalConsumerSmoke.csproj --configuration Release --no-restore`:
  passed with 0 warnings and 0 errors.
- Focused Learn UI smoke targets Brightness, Threshold, and Matching passed
  (`OK|check=OK`) under `ui-after-representative`; a fresh Threshold capture
  also passed under `ui-after-threshold`.
- `ui-precheck-after-debug` and `ui-precheck-after-release` both returned
  `OK 1 / WARN 0 / NG 0` for the Learn Threshold target.
- `Invoke-RefactorAudit.ps1 ... -Verify` returned
  `REFACTOR_AUDIT=PASS|CSharpFiles=772|XamlFiles=58|PartialDeclarations=106|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
  Its `quantitative-audit-after-topic-policy\source-survey-summary.txt`
  records 268,718 C# lines and 12,237,951 C# bytes.

## UI qualification boundary

No XAML or shared style changed in this slice. Fresh before/after Threshold
captures are present in the evidence root, and the representative focused
smoke plus Debug/Release precheck passed. The full physical desktop matrix
(focus, hover, pressed, selected, disabled, popup, themes, Wide/Compact
layouts, 100/125/150/175/200% DPI, monitor placement, resize and close order)
was not run; those states remain unverified. Baseline broader Learn screenshot
smoke attempts reported existing unrelated preconditions: internal engineering
copy in `LEARN_EDGE_BASED_MATCHING.md`, missing `Routing safety checklist` in
the Layer/Recipe guide, missing animated outlier gate in Metrics/Acceptance,
and missing expected practice guidance in Color/HSV. Those expectations were
not relaxed or deleted.

## Junior developer assessment

**PASS for this boundary.** The readable path is now
`TopicList_SelectionChanged -> UpdateSelectedTopic -> policy.Resolve -> Window
applies state -> existing topic View/Presenter guide owner`. The policy has no
control or Window dependency, while the Window visibly owns control updates;
the existing catalog and topic owners remain easy to locate. The remaining
Directory policy and physical runtime qualification are separate tasks.

## No-reopen rule and next slice

Completed Learn host ownership and topic View/Presenter owners are not to be
split again without a reproduced defect, changed requirement, or proven
responsibility conflict. The next independently verifiable slice is the
remaining `RoiImageCanvasViewModel.Commands.cs` Directory policy owner.

Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
