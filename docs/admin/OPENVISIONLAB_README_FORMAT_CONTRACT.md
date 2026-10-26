# OpenVisionLab README format contract

This contract preserves this repository's existing 2D README presentation.
Read it with the actual [English README](../../README.md) and
[Korean README](../../README.ko.md) before changing a README, its linked
presentation documents or its media.

## Published baseline and change boundary

The reference is this repository's README at
[`38a016136e72ef856f3dbec3e29229a182f7d268`](https://github.com/Noah8218/OpenVisionLab/commit/38a016136e72ef856f3dbec3e29229a182f7d268),
inspected on 2026-10-26. The structure below is a dated reference, not a
replacement README or a template for every OpenVisionLab product.

For an approved feature change or factual correction, edit the smallest relevant
paragraph, row, command or link. Preserve the existing common structure,
section order, tables, language links, media positions and detail destinations.
Read the target repository's source and README; another repository's layout,
version string or screenshots do not prove equivalent features here.
Do not rewrite this README wholesale to match another project or branch.

Changes to the format itself require explicit user confirmation of the proposed
before/after structure before implementation. This includes adding, removing,
renaming or reordering sections, moving the opening/language links/media, and
moving detail between documents. A general continuation request, a new model or
elapsed time is not format approval. Record an accepted format change in this
contract when it is implemented.

## Opening and section order

Keep the `OpenVisionLab` title, Korean-language link and brief product
introduction in their current order. The root README is English, with
`[한국어](README.ko.md)` near the top; the Korean file keeps
`[English](README.md)` in the equivalent position. Preserve this repository's
opening rather than adding another project's badge row or hero arrangement.

Keep the level-two headings in their existing order in both languages:

| Order | English | Korean |
| --- | --- | --- |
| 1 | Version | 버전 |
| 2 | Quick Overview | 개요 |
| 3 | Build and Check a Recipe | 레시피 구성과 확인 |
| 4 | One Image, Every Inspection Stage | 한 영상에서 모든 검사 단계 확인 |
| 5 | User Manual | 사용 설명서 |
| 6 | Tool Setup and Results | 툴 설정과 결과 |
| 7 | Chained Processing | 연속 처리 |
| 8 | Start Here: Choose the Project | 시작하기: 프로젝트 선택 |
| 9 | Build From A Fresh GitHub Clone | 새 GitHub 클론에서 빌드하기 |
| 10 | Requirements and Running the Application | 요구사항과 애플리케이션 실행 |
| 11 | Product Scope | 제품 범위 |
| 12 | Main Features | 주요 기능 |
| 13 | Public Samples | 공개 샘플 |
| 14 | Tool Architecture | 툴 구조 |
| 15 | Pipeline Review | 파이프라인 검토 |
| 16 | Build | 빌드 |
| 17 | Validation | 검증 |
| 18 | Continuous Integration | 지속적 통합 |
| 19 | Releases | 릴리스 |
| 20 | Roadmap | 로드맵 |
| 21 | Scope and Limitations | 범위와 한계 |
| 22 | Documentation | 문서 |
| 23 | License and Copyright | 라이선스와 저작권 |
| 24 | Summary | 요약 |

Keep the current version and recent real version history in the Version
section, and preserve the Visual Studio, command-line and optional Windows
Sandbox subsections inside the fresh-clone instructions. Keep the source-entry,
feature and sample-pair tables, commands, workflow diagrams and attribution
block with their existing subjects. Do not invent missing version history.

## Media placement and detailed documentation

README media live under `docs/assets/demo/readme-workflows/`. Both languages
keep the same media sequence and equivalent explanations:

| Position | Asset |
| --- | --- |
| Start of Recipe walkthrough, before its numbered steps | `recipe-teaching.gif` |
| Start of inspection-stage comparison | `inspection-stages.png` |
| Tool settings introduction, followed by its MP4 link | `tool-settings.gif`, `tool-settings.mp4` |
| Chained-processing example, followed by its MP4 link | `preprocessing-chains.gif`, `preprocessing-chains.mp4` |
| Following particle-count example in Chained Processing | `sample-inspections.gif` |
| Requirements/run section, before the workspace-area explanation | `main-workspace.png` |
| Public sample introduction, before the sample-pair table | `sample-catalog.png` |
| End of Public Samples, with the Good/Bad comparison | `recipe-validation.gif` |
| End of Pipeline Review | `pipeline-results.png` |

This baseline retains the complete source-build, architecture, validation, CI
and release guidance in the README sections above. Preserve that boundary;
moving it into a shorter overview and new build/development documents is a
format change requiring user confirmation.

Keep longer subject references linked through the existing
[manual](../manual/README.md), [tutorial](../learn/OPENVISIONLAB_TUTORIAL.md),
[Learn Mode](../learn/README.md), [sample guide](../samples/public/README.md),
[documentation index](../README.md), [source owner map](CODEBASE_STRUCTURE.md),
[stable contracts](../OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md),
[version policy](../OPENVISIONLAB_RELEASE_VERSION_POLICY.md) and
[changelog](../../CHANGELOG.md). Do not duplicate those documents in the introduction
or add links to files that exist only in another repository.

## Language, facts and supported scope

Use natural, direct English and Korean. Describe the operator's action and the
image, measurement or result to compare. Avoid inflated promotion, literal
translation and internal agent/planning narration. Keep actual UI names,
identifiers, commands, metrics, sample names, dates, attribution and support
qualifications. Translate meaning without broadening or weakening it.

Preserve the rule-based Recipe workbench identity, optional LLM/XML assistance,
explicit execution and Good/Bad versus OK/NG distinction. Keep compatibility
requirements and validation limitations factual. Public copy describes user
capabilities; internal platform non-goal boilerplate belongs in contributor
or product contracts rather than the public introduction.

Verify versions against this repository's canonical project metadata and real
history. Keep a source version distinct from any separately published ZIP;
do not invent a release, supported feature or passed validation. Publish only
behavior and media supported by this repository's source/build evidence.
Never copy development-only claims into the public README or infer parity
from matching product version strings.

## UI media standard

When adding or replacing UI media, capture the actual current build containing
the described behavior. Maximize the application within the selected monitor's
working area. Record source/build identity, resolution, working area and scale;
follow the applicable Full HD and monitor-selection rules. Existing recordings
demonstrate their captured build and do not verify later UI changes. A rule-only
edit does not require replacing unchanged media.

Keep captures free of click rings, pointer emphasis, keyboard/mouse HUDs,
recording controls and added manipulation callouts. Preserve the application's
real ROI, detection drawings and result overlays when they explain inspection
evidence. Do not fabricate or composite product UI. This README-specific rule
takes precedence over the older generic preference for annotated tutorial images.

Make GIFs quick to watch by trimming idle waits and keeping transitions brisk,
while leaving labels, parameter changes and results readable. Preserve action
order and failure/qualification context. Keep MP4 alternatives with their GIFs;
do not imply that accelerated playback measures execution performance.

## Review before publishing

1. Compare both languages to the accepted baseline. Confirm the opening,
   heading order, tables, media placement and detail destinations. Identify
   any separately approved format change.
2. Resolve relative links from each document's directory. Check exact filename
   case, heading anchors, translated headings and all referenced files. Confirm
   media exist, are tracked in the intended commit and decode; review GIF
   playback and MP4 links when those assets change.
3. Inspect rendered Markdown in GitHub or a compatible renderer at desktop and
   narrow reading widths. Check the language links, tables, code blocks,
   English/Korean wrapping, clipped or overflowing text, image scale and GIF
   readability. Open relevant links and anchors. File checks do not establish
   visual quality; state any visual check that was not performed.
4. Check facts and supported scope against this repository; preserve license
   and NOTICE references and scan the complete README for internal non-goal
   boilerplate. Run `git diff --check` and affected documentation checks.
5. Integrate changes into the existing AGENTS without replacing unrelated rules,
   then re-read the saved AGENTS and this contract. Global instructions remain
   with their designated owner. Commit and non-force push only the authorized
   file allowlist; verify the remote SHA/diff and associated CI. Report billing
   or runner blocks separately without changing CI settings or rerunning a
   billing-blocked job.
