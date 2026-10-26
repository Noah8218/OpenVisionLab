# OpenVisionLab README format contract

This contract preserves the user-reviewed 2D README presentation. Read it with
the actual [English README](../../README.md) and [Korean README](../../README.ko.md)
before editing any README, linked presentation document or README media.

## Reviewed baseline and change boundary

This reference records the shared 2D README structure approved for this
repository on 2026-10-26. The English and Korean READMEs use the concise
overview and separated guides below, with this repository's own source,
versions, history and media facts. Earlier long README layouts in Git
history are historical references, not an alternative active format.

For an approved feature change or factual correction, edit the smallest relevant
paragraph, row, command or link. Keep the existing shared structure, section
order, tables, media positions and division into detailed documents. Do not
rewrite the whole README, restore the former long README, or import another
project's headings merely to make the projects look identical.

Changing the format itself requires explicit user confirmation of the proposed
before/after structure before editing it. This includes adding, removing,
renaming or reordering sections, moving the language links or media, changing
the opening composition, or moving detail between documents. A general request
to continue, a model change or elapsed time does not approve a redesign.
Record the accepted format change in this contract when it is implemented.

## Opening and section order

Keep this opening sequence: product title, one-sentence description, Release /
Windows / .NET / License / CI badges, language switch, then the teaching GIF.
The root README is English. Keep `English | [한국어](README.ko.md)` near the top;
the Korean file keeps `[English](README.md) | 한국어` in the equivalent position.
Preserve the existing badge destinations; changing which repository a badge
reports is a separate factual decision, not an automatic Dev/public substitution.

The level-two headings appear in this order in both languages:

| Order | English | Korean |
| --- | --- | --- |
| 1 | Try it in 3 steps | 3단계로 시작하기 |
| 2 | What it does | 지원 작업 |
| 3 | Build and Check a Recipe | Recipe 구성과 확인 |
| 4 | One Image, Every Inspection Stage | 한 영상에서 모든 검사 단계 확인 |
| 5 | Tool Setup and Results | 툴 설정과 결과 |
| 6 | Chained Processing | 연속 처리 |
| 7 | Main Features | 주요 기능 |
| 8 | Public Samples | 공개 샘플 |
| 9 | Build from source | 소스에서 빌드 |
| 10 | Documentation | 문서 |
| 11 | Status and limitations | 현재 상태와 한계 |
| 12 | Part of OpenVisionLab | OpenVisionLab 프로젝트 |
| 13 | License | 라이선스 |

Keep the current-source version and Changelog link after the three-step start
and the published-download qualification. Keep the feature and sample-pair
tables, the family-project table and the license/attribution block in their
existing sections.

## Media placement and detailed documents

README media live under `docs/assets/demo/readme-workflows/`. Both languages
use the same media sequence and equivalent explanations:

| Position | Asset |
| --- | --- |
| Opening, after language links | `recipe-teaching.gif` |
| Recipe walkthrough, after the Good/Bad result example | `recipe-validation.gif` |
| Start of inspection-stage comparison | `inspection-stages.png` |
| Tool settings introduction, followed by its MP4 link | `tool-settings.gif`, `tool-settings.mp4` |
| Chained-processing example, followed by its MP4 link | `preprocessing-chains.gif`, `preprocessing-chains.mp4` |
| Following particle-count example in Chained Processing | `sample-inspections.gif` |
| Public sample introduction, before the sample-pair table | `sample-catalog.png` |

Keep the README focused on trying the application and seeing the inspection
workflow. Preserve these existing detail destinations and their reciprocal links:

- [BUILD.md](../runbooks/BUILD.md): published ZIP distinction, complete prerequisites,
  Visual Studio and command-line builds, optional Windows Sandbox, source-run
  procedure and workspace reference. `main-workspace.png` stays in its
  [workspace reference](../runbooks/BUILD.md#workspace-reference).
- [DEVELOPMENT.md](../runbooks/DEVELOPMENT.md): source entry points, architecture, inspection
  review, Pipeline Review, checks, CI and release/version references.
  `pipeline-results.png` stays with [Pipeline Review](../runbooks/DEVELOPMENT.md#pipeline-review).
- [CHANGELOG.md](../../CHANGELOG.md): detailed version history. Keep the concise
  current-version link in the README rather than copying the history back.
- [Manual](../manual/README.md), [tutorial](../learn/OPENVISIONLAB_TUTORIAL.md),
  [Learn Mode](../learn/README.md), [sample guide](../samples/public/README.md) and
  [documentation index](../README.md): their existing detailed subjects.

On 2026-10-26 the build/development guides were classified under the existing
`docs/runbooks/` directory. The README links directly to those canonical
guides. `docs/BUILD.md` and `docs/DEVELOPMENT.md` retain compatibility links
and section anchors for already-published URLs. This documented path change
preserves the approved section order and presentation.

## Language, facts and repository differences

Use direct, natural English and Korean. Explain what the operator does and
what result to inspect; avoid inflated promotion, literal translation and
internal agent or planning narration. Keep actual UI labels, identifiers,
commands, metrics, sample names, dates, attribution and support qualifications.
Translate meaning without silently broadening or weakening it.

Preserve the rule-based Recipe workbench identity, optional LLM/XML assistance,
explicit execution and Good/Bad versus OK/NG distinction. Keep compatibility
requirements and validation limitations accurate. Do not add internal platform
non-goal boilerplate to the public README.

Check versions against the target repository's canonical project metadata and
existing history. Distinguish current source from an older published ZIP; do not
invent a release, a supported feature or a passed validation. A feature, capture, command or source SHA from another branch does not
establish support here. Keep the approved common presentation while
checking this repository's facts independently; equal version strings
do not establish source parity.

## UI media standard

When adding or replacing UI media, capture the actual current build containing
the described behavior. Maximize the application within the selected monitor's
working area. Follow the repository's Full HD, scaling, monitor selection and
clean-runtime evidence rules and record the source/build identity and capture
conditions. Existing media remain historical demonstrations of their captured
build; this contract does not require recapturing unchanged media for a rule edit.

Keep captures free of click rings, pointer emphasis, keystroke/mouse HUDs,
recording controls and added manipulation callouts. Preserve the application's
real ROI, detection drawings and result overlays when they explain inspection
evidence. Do not fabricate or composite product UI. This README-specific rule
takes precedence over the older generic preference for annotated tutorial images.

Make GIFs quick to watch by trimming idle waits and keeping transitions brisk,
while leaving labels, parameter changes and results readable. Preserve action
order and failure/qualification context. Keep MP4 alternatives with their GIFs;
do not imply that accelerated playback measures execution performance.

## Review before publishing

1. Compare both languages against the last accepted baseline. Confirm section
   order, opening, tables, media positions and detail destinations. Identify any
   separately approved format change.
2. Resolve relative links from each document's own directory. Check file paths,
   exact filename case and heading anchors, including translated and renamed
   headings. Confirm media exist, are tracked for the intended commit and decode;
   check GIF playback and MP4 links when those assets change.
3. Review the rendered Markdown in GitHub or a compatible renderer at desktop
   and narrow reading widths. Inspect language links, badges, tables, code blocks,
   English/Korean wrapping, clipped or overflowing text, image scale and GIF
   readability. Open the relevant anchors and detail links. Path checks alone do
   not establish visual quality; report any unperformed visual check explicitly.
4. Verify facts and supported scope against this repository, preserve license
   and NOTICE references, scan the full README for internal non-goal boilerplate,
   and run `git diff --check` plus the affected documentation checks.
5. Integrate changes into the existing AGENTS without replacing unrelated rules;
   re-read the saved AGENTS and this contract. Global instructions remain with
   their designated owner. Use the exact authorized file allowlist for commits
   and non-force pushes, then verify the remote SHA/diff and associated CI.
   Report billing/runner blocks separately without changing CI settings or
   rerunning a billing-blocked job.
