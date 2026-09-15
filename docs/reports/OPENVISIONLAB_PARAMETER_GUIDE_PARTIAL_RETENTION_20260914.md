# OpenVisionLab Parameter Guide Partial retention and catalog boundary

Date: 2026-09-14 KST  
Issue: `PL-0055`  
Scope: `VisionToolParameterGuideView.xaml.cs` and the existing parameter-guide
composition path

## Decision

`VisionToolParameterGuideView.xaml.cs` remains a Partial. It is the required
XAML visual adapter: it projects a `VisionToolParameterGuideContent` snapshot,
updates the named controls and automation properties, creates related-property
buttons, and forwards those buttons through the supplied callback. It does not
own selection policy, PropertyGrid policy, persistence, algorithm execution,
file I/O, or a disposable lifetime that can be extracted safely.

The audit did find a real boundary defect in the existing catalog owner. A
global `DynamicPropertyGridTypeDescriptionProvider` can hide a dependent public
property from `TypeDescriptor` while the parameter guide still needs to explain
why that property is inactive. `VisionToolParameterGuideCatalog` now keeps the
normal `TypeDescriptor` path and uses a narrow public-instance reflection
fallback only when that descriptor is unavailable. The fallback creates a
descriptor from the property attributes, so localization and value formatting
continue through the catalog without leaking PropertyGrid visibility policy into
the View.

This is a bounded owner correction, not a mechanical Partial split. The View
remains the presentation owner; the catalog remains the guidance-policy owner.

## Owner and call-path map

| Concern | Current owner | Proof / boundary |
| --- | --- | --- |
| XAML namescope, text/visibility/brush projection, automation text, related button presentation | `VisionToolParameterGuideView` Partial | `ShowPrompt`, `ShowContent`, `SetCompactMode`, `PopulateRelatedButtons` |
| Selected object/property and related-property navigation | `VisionToolParameterGuidePresenter` | `SelectObject`/`SelectProperty` -> `Refresh` -> `FocusRelatedProperty` |
| Definitions, localization, applicability, fallback wording, public-property descriptor recovery | `VisionToolParameterGuideCatalog` | `Resolve` -> `GetPropertyDescriptor` -> `ResolveApplicability` |
| PropertyGrid focus/value events, language refresh, disposal | `VisionToolCustomParameterGuideBinder` | event attach/detach, language controller, `Dispose` |
| Standard PropertyGrid selection/value wiring and presenter lifetime | `VisionToolPropertyGridHost` | selected-property/value events and presenter disposal |
| Floating-window ownership, positioning, re-entry, close/unsubscribe | `VisionToolParameterGuideSidecarController` | sidecar creation, owner resolution, close/unload cleanup |
| Guide instance, visibility and shared single-input composition | `VisionToolSingleInputPropertyToolShell` | `ParameterGuide` property and sidecar composition |
| Mutable guide control state | `VisionToolParameterGuideView` control fields only; policy state remains presenter/catalog | no business-state writer in View |
| View lifetime | `VisionToolSingleInputPropertyToolShell` and sidecar controller | shell composition and deterministic sidecar release |

The concrete call path is:

```text
PropertyGrid focus/value event
  -> VisionToolCustomParameterGuideBinder
  -> VisionToolParameterGuidePresenter.SelectObject/SelectProperty
  -> VisionToolParameterGuideCatalog.Resolve
  -> VisionToolParameterGuideView.ShowContent
  -> related button callback
  -> binder/property-grid focus navigation
```

## Binding and public contract

The XAML contract retains the `VisionToolParameterGuide` automation root,
`guideExpander`, coverage/title/identity/applicability/summary/impact/best/risk/
check/related automation identifiers, wrapping text, vertical scrolling, and
the compact `116D` versus normal `184D` height projection. The View's internal
test facade still reads rendered text and expansion state; the presenter and
shell contracts remain unchanged.

The View has no direct `PropertyGrid`, `Window`, `OpenVisionFloatingToolWindow`,
`OpenCvSharp`, file-dialog, persistence, Recipe execution, algorithm, or timer
coupling. Introducing another ViewModel, service, interface, forwarding Partial,
or wrapper would duplicate an existing owner and would not create an independent
state, lifetime, or test seam.

## Shortest reading order for a new developer

1. `VisionToolSingleInputPropertyToolShell.xaml(.cs)` — find the guide instance,
   visibility, density, and sidecar composition.
2. `VisionToolPropertyGridHost.cs` — find selection/value events and presenter
   lifetime.
3. `VisionToolCustomParameterGuideBinder.cs` — find focus, mouse/keyboard,
   language refresh, and disposal wiring.
4. `VisionToolParameterGuidePresenter.cs` — follow selected object/property to
   content projection and related-property navigation.
5. `VisionToolParameterGuideCatalog.cs` — read definitions, applicability,
   localization, fallback text, and the descriptor fallback.
6. `VisionToolParameterGuideView.xaml(.cs)` — inspect only the final XAML
   projection and callback adapter.

One useful search is `VisionToolParameterGuidePresenter`; it leads to each
owner above without starting from the View's generated Partial declaration.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\parameter-guide-partial-retention-20260914`

| Check | Result |
| --- | --- |
| `VisionToolParameterGuidePartialBoundaryContract` Debug | 14/14 PASS |
| `VisionToolParameterGuidePartialBoundaryContract` Release | 14/14 PASS |
| `VisionRecipeRunnerSmoke` Debug build | 0 errors; 2 existing MSB3270 architecture warnings |
| `VisionRecipeRunnerSmoke` Release build | 0 errors; 2 existing MSB3270 architecture warnings |
| Parameter guide WPF precheck Debug (`p257`, `p259`, `p260`) | all `OK`; layout/text/internal diagnostics `0` |
| Parameter guide WPF precheck Release (`p257`, `p259`, `p260`) | all `OK`; layout/text/internal diagnostics `0` |
| Dynamic monitor/window probe Debug | PASS; selected `\\.\DISPLAY2`, one intersecting window |
| Dynamic monitor/window probe Release | PASS; selected `\\.\DISPLAY2`, one intersecting window |
| Pipeline smoke Debug build | 0 errors; 2 existing MSB3270 architecture warnings |
| Pipeline smoke Release build | 0 errors; 3 warnings (2 existing MSB3270 plus existing `CS8600` at `Program.cs:10181`) |

The original failing p257 assertion was corrected to accept the localized
Korean inactive wording (`꺼져`) as well as English `inactive`. The catalog
fallback then fixed the reproduced runtime case where hidden PropertyGrid
descriptors prevented inactive unique-margin/contour guidance from rendering.

## Unverified boundary

This slice does not prove every theme, DPI level (100/125/150/175/200%),
keyboard/input state, native Tool/file association, camera/SDK/GPU path, actual
inspection execution/persistence, or long-running native runtime. The current
status is therefore: **소스 코드 기준 검토 완료 / 대표 WPF Runtime UI 및
동적 모니터 배치 검증 완료 / 전체 Runtime 행렬은 미검증**.

## Reopen rule

Do not split or rename this Partial merely because it is still a Partial or the
file grows. Reopen only for a new requirement, a reproducible defect, a failed
completion criterion, or a changed dependency/lifetime boundary. A future
change to guidance policy belongs in the catalog/presenter seam; a future
change to visual projection belongs in the existing View contract.
