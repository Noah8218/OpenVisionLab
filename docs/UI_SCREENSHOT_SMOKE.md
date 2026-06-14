# UI Screenshot Smoke

OpenVisionLab UI changes should be checked with a repeatable screenshot pass when direct manual testing is not available.

## Run

Full precheck:

```powershell
.\tools\RunUiPrecheck.ps1
```

This runs the OpenVisionLab build first, then runs the screenshot smoke, and writes:

```text
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_smoke\ui_precheck_report.md
```

Screenshot-only pass:

```powershell
.\tools\RunUiScreenshotSmoke.ps1
```

The script builds the screenshot smoke project and writes PNG files to:

```text
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_smoke
```

Current targets:

- `pipeline_viewer.png`: pipeline result detail viewer with overlay list and selected overlay state.
- `ai_recipe_form.png`: AI Recipe import dialog for pasted/generated pipeline XML.
- `pipeline_form.png`: pipeline editor form layout.
- `threshold_form.png`: threshold teaching form layout.
- `main_workspace.png`: main vision workspace with a synthetic Main layer image.

Console output includes a basic image check:

```text
pipeline_form=OK|check=OK|colors=52|flat=70%|layout=0|text=0|size=1280x760|...
```

- `check=OK`: image was created and has enough sampled color variation to avoid obvious blank captures.
- `check=WARN`: image was created, but sampled color variation is low.
- `check=NG`: image is missing, too small, nearly blank, or nearly fully saturated.
- `flat`: sampled tile percentage that appears mostly single-color. High values can indicate an overly empty capture.
- `layout`: count of visible WinForms controls outside their parent client area.
- `text`: count of visible button/label/combo text entries that likely do not fit their control width.

When `layout` or `text` is non-zero, a sibling issue file is written:

```text
pipeline_form.issues.txt
```

## Review Checklist

- No blank or black-only form unless the target intentionally has no image.
- Toolbars and footer buttons are not clipped.
- Combo boxes, numeric inputs, and buttons fit their row.
- Result/overlay panels have readable headers and selected state.
- Scrollbar presence is acceptable only when the form is intentionally scrollable.

## Development Gate

Run the full precheck before handing off UI-heavy changes:

```powershell
.\tools\RunUiPrecheck.ps1 -FailOnWarn
```

Use the generated report as the first review artifact. If `-FailOnWarn` is too strict for a work-in-progress branch, run without it and record any warning in the handoff note.

For pipeline or overlay viewer changes, inspect these images first:

- `pipeline_form.png`
- `pipeline_viewer.png`
- `ai_recipe_form.png`
- `main_workspace.png`

The goal is to catch obvious clipping, blank captures, and unreadable controls before manual UI testing.

## Notes

Most targets use WinForms `DrawToBitmap`. `main_workspace.png` uses live screen capture because DockPanel-hosted document content does not render reliably through `DrawToBitmap`.
