# LLM XML Draft Workflow Refactor Proof (2026-07-25)

Status: Complete

## Scope

Move existing LLM XML draft load, review-bundle dry-run, validation, dependency inspection, and import lifecycle methods into a dedicated Recipe Command Surface partial. This is maintenance-only structure work; no LLM workflow, prompt, template, or acceptance behavior changes.

## Intended responsibility boundary

- `OpenVisionShellHostRecipeCommandSurface.LlmXmlDraftWorkflow.cs` owns draft file input, review-bundle dry-run state, validation/import readiness, dependency review state, and import persistence.
- `OpenVisionShellHostRecipeCommandSurface.Handlers.cs` retains Guided Setup template authoring, prompt composition, and unrelated recipe workflows.
- Existing command bindings, public test entry points, and explicit Preview/Run behavior remain unchanged.

## Acceptance criteria

- Extracted methods have a single implementation in the workflow partial.
- The existing LLM XML draft screenshot smoke passes from the current source.
- The Debug solution build and repository readiness check pass.

## Verification

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- Current-source `wpf_shell_host_recipe_manager_summary` smoke passed. Its established Recipe Manager path exercises XML draft load, validation, import-readiness, import, and the surrounding selection/summary workflow.
- The source search confirms that draft load, review-bundle dry-run, validation/import, dependency context, and import readiness implementations now reside only in `OpenVisionShellHostRecipeCommandSurface.LlmXmlDraftWorkflow.cs`.

## Evidence

- `artifacts\\maintenance_llm_xml_draft_workflow_refactor_20260725\\wpf_shell_host_recipe_manager_summary.png\\wpf_shell_host_recipe_manager_summary.png`
- `src\OpenVisionLab\UI\\Menu\\Wpf\\OpenVisionShellHostRecipeCommandSurface.LlmXmlDraftWorkflow.cs`

## Boundary

This is a responsibility split only. It does not add or expand LLM/provider behavior, modify prompts or starter templates, or change explicit Preview/Run, layer routing, XML validation, dependency-copy, or import semantics.
