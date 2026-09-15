# OpenVisionLab Project Document Audit And Priority Decision

- Date: 2026-08-30 KST
- Status: **Complete**
- Repository: `C:\Git\2D\Dev`
- Scope: inspect the Dev project's document control plane and all textual
documentation, reconcile it with Git/source evidence, correct the current path
and documentation authorities, determine the next priorities, and verify the
existing dirty implementation slices without committing or crossing into the
Original repository.

## Audit Coverage

- 1,148 document-area files were enumerated.
- 740 text, schema, and manifest files were read programmatically: approximately
  14.95 MB and 100,235 lines.
- 408 supporting media assets were inventoried: 402 PNG, three GIF, and three
  MP4 files. They were not all visually replayed; dated reports and current
  screenshots remain the authority for claims about their content.
- No text read errors or replacement-character corruption were observed.
- Structured JSON/XML/YAML/XSD inputs were parsed where applicable. The apparent
  root `docs/VISION_PIPELINE_RECIPE_SCHEMA.xsd` parse exception was a Markdown
  compatibility redirect; its canonical contract schema parsed successfully.
- `tools/TestDocumentationIndex.ps1` passed with 107 indexed paths, 12 routes,
  and 102 root redirects.

## Reconciled Project Assessment

### Product identity

OpenVisionLab is an OpenCvSharp4 deterministic rule-based vision recipe
workbench. Its shortest normal workflow is sample image -> PropertyGrid teaching
-> Pipeline -> explicit Preview/Run -> drawing/metric/layer review -> N-sample
validation -> saved Recipe. LLM/XML and Codex skills are optional maintenance or
teaching aids, not runtime detectors or product prerequisites.

### Evidence-based maturity

The bounded workbench workflow is broadly connected and has substantial source,
build, public-sample, persistence/provenance, resource-lifetime, focused WPF,
and RC evidence. The correct assessment is RC/pre-production within the recorded
environments, not commercial GA. Installer/signing/update, current full UI
matrix, multi-PC/hardware, calibrated metrology, and field robustness remain
unproven.

### Commercial lesson and excluded scope

Retain a short teach/run/review/validate/save operator path, explicit reasons,
visible evidence, Recipe management, and deterministic replay. Do not convert
commercial comparisons into camera, lighting, PLC/I/O, MES, account, cloud,
deployment-controller, or equipment-integration scope.

## Conflicts Found And Resolved

1. The physical Git root was nested at
   `C:\Git\2D\Dev\OpenVisionLab_Dev` while current rules claimed
   `C:\Git\2D\Dev`. It was flattened by same-volume rename with exact Git/file
   equivalence evidence. See
   `OPENVISIONLAB_DEV_REPOSITORY_PATH_MIGRATION_20260830.md`.
2. The former current handoff had grown to 2,133 lines and mixed live status with
   chronology. It is preserved unchanged in
   `docs/admin/archive/OPENVISIONLAB_CURRENT_HANDOFF_HISTORY_20260830.md`; the new
   live handoff is compact and activation-gated.
3. The documentation map was dated 2026-08-28 and omitted the 2026-08-30
   Matching reports. It now lists the path migration, skill split, Takt study,
   and this audit.
4. Current commands in the restart prompt, product target, public-sample policy,
   and manual builder still used the old Dev path. Current authorities now use
   `C:\Git\2D\Dev`; historical execution commands remain preserved with an
   explicit relocation note.
5. `CHANGELOG.md` incorrectly claimed no user-visible change after rc.1 despite
   post-candidate feature/fix history. `Unreleased` now summarizes the actual Dev
   additions, changes, fixes, and release boundary.

## Existing Dirty Worktree Review

The pre-migration dirty state was preserved exactly. Its implementation changes
fall into three coherent responsibilities:

1. Public sample and N-image correctness: resolve repo-relative Matching
   dependencies when a catalog pipeline is opened or rerun, and classify Tool
   execution failure as `ERROR` instead of ungated success.
2. Batch evidence identity: include source SHA-256 in same-basename run folders
   and retain Matching plus EdgeBasedMatching overlay evidence.
3. Cross-repository consumer scope: keep generic 2D published-consumer smoke
   usable without locator artifacts while allowing locator evidence to be an
   explicit required flag; align the Dev script default path with the canonical
   root.

Static review found no new blocking correctness defect in these slices. Existing
user changes and release drafts were preserved; nothing was staged, committed,
or pushed.

## Verification Performed

| Check | Result | Evidence or boundary |
| --- | --- | --- |
| `git diff --check` | PASS | LF/CRLF notices only |
| `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"` | PASS, 0 warnings / 0 errors | Current Dev root; output junctions and TEMP/TMP are D-backed |
| `dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Release` | PASS, 0 warnings / 0 errors | Current dirty runner/cross-repo code compiled |
| `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` | PASS, 1 existing nullable warning / 0 errors | Warning CS8600 at unchanged line 10707 |
| OpenVision readiness | PASS, 13/13 | Current root `C:\Git\2D\Dev` |
| N-image contract | PASS, 6 tools x 30 images | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\priority-review-20260830\nimage-contract` |
| Focused WPF sample-open + N-image targets | PASS | `...\wpf-focused\run-2`; selected `\\.\DISPLAY2`, windows `-1900,385..-300,1285` and `-1900,385..-520,1225` |
| Sample-open from non-repository CWD | PASS | `...\wpf-nonrepo-cwd\evidence`; process CWD was D: |
| Same-basename/EdgeBased batch evidence | PASS | two unique SHA-suffixed runs and two `_edgebasedmatching_overlay.png` files under `...\batch-mini\evidence` |
| Documentation JSON/index/diff | PASS | 107 paths / 12 routes / 102 redirects |
| Skill registry + template registration regression | PASS | Registry validated against `C:\Git\2D\Dev` |
| `RunTwoDIntegrationCrossRepoSmoke.ps1` parser | PASS, 519 tokens | External Machine Studio execution intentionally not rerun |

The first external monitor-placement wrapper attempt raced a window closing and
discarded its own result; no process remained. The corrected wrapper tolerated
closed-window races, placed two current windows inside the selected monitor, and
produced the successful `run-2` evidence above. This was a harness wrapper issue,
not a product smoke failure.

The two successful screenshots were inspected directly. The expected sample
image, sample workflow bar, N-image table, result panes, and detail panel were
rendered; no blank capture was observed. This does not substitute for the full
theme/state/layout/DPI/performance matrix.

## Priority Order And Activation Conditions

1. PCB Matching Takt qualification — blocked until the operator declares
   measurement scope, positive budget/unit, p95, hard maximum, warm-up/repeat
   policy, and supplies held-out/physical correspondence review. Current status:
   `WAIT_TAKT_BUDGET`. | Recommended model: none until inputs exist | Reasoning
   effort: none until inputs exist.
2. `locator-relative-blob-v1` qualification — blocked until an operator approves
   or replaces the physical locator and defines downstream ROI/tolerance. |
   Recommended model: none until the decision exists | Reasoning effort: none
   until the decision exists.
3. `CVR-00` novice validation — blocked until three independent first-time
   participants and unedited observations exist. | Recommended model: none until
   observations exist | Reasoning effort: none until observations exist.
4. RC2 candidate/publication — blocked until a separately reviewed clean
   candidate and explicit release-stage authorization exist. `PL-0012`,
   `PL-0011`, publication, and deployment remain distinct. | Recommended model:
   none until prerequisite and authorization exist | Reasoning effort: none
   until the prerequisite exists.

## Boundaries

- The full WPF normal/hover/pressed/focus/disabled/validation/popup,
  light/dark, Wide/Compact, 100/125/150/175/200% DPI, resize, keyboard, and
  performance matrix was not run. For those states: `소스 코드 기준 검토 완료 /
  실제 Runtime UI 검증 필요`.
- The external Machine Studio producer/consumer workflow was not rerun because
  this task is Dev-only. Its C# path compiled and the PowerShell script parsed;
  new cross-process behavior remains outside this task's fresh runtime evidence.
- Four already-prunable legacy cleancheck worktree registrations were observed
  and left untouched. Pruning them can remove their detached administrative HEAD
  references and was not required for the canonical-root correction.
- Supporting media were inventoried, not all replayed. Original, commit, push,
  tag, release publication, deployment, and installation were untouched.

## Closure Record

- Status: **Complete**
- Scope: all textual project-document audit, current-priority decision, exact Dev path correction, live documentation control-plane repair, and proportionate verification of the preserved dirty slices
- Acceptance criteria: textual documents covered -> PASS; product/maturity/scope reconciled -> PASS; canonical physical Dev root -> PASS; current authorities aligned -> PASS; priorities activation-gated with model guidance -> PASS; dirty slices build/focused smoke -> PASS
- Verification: exact checks and results are listed above
- Evidence: this report, the migration report, compact current handoff, and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\priority-review-20260830`
- Boundary / next dependency: no autonomous product task is unblocked; the exact operator/data/release prerequisites are listed in priority order above
