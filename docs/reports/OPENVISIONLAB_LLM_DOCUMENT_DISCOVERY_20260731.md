# OpenVisionLab LLM Document Discovery

Updated: 2026-07-31 KST

## Status

Complete

## Scope

기존 문서와 증거 경로를 이동하거나 삭제하지 않고, 사람과 LLM이 현재 권위 문서와 작업별 상세 문서를 빠르게 찾는 탐색 계층을 추가했다.

## Implemented

- `docs/README.md`를 단일 사람/LLM 진입점으로 재작성했다.
- 최소 시작 문서, 권위 순서, 작업별 읽기 경로, 폴더 의미, 루트 리다이렉트 규칙, 검색 예시를 한 곳에 모았다.
- `docs/LLM_DOCUMENT_INDEX.json`에 권위 문서와 10개 작업 의도별 canonical `read` 경로를 기계 판독 형식으로 고정했다.
- `tools/TestDocumentationIndex.ps1`가 색인 JSON, 중복 route/rank, 경로 이탈, 누락 파일, canonical 대신 리다이렉트 사용, `docs/` 루트의 모든 호환 리다이렉트를 검사하도록 했다.
- `AGENTS.md`의 시작 순서를 새 진입점/색인 우선으로 바꾸고, 전체 연대기와 LLM 문서는 관련 작업에서만 읽도록 범위를 좁혔다.
- 상세 문서 지도는 전체 등록부 역할을 유지하며 새 진입점과 색인을 안내한다.

## Acceptance Criteria

| Criterion | Result | Evidence |
| --- | --- | --- |
| 단일 문서 진입점 | Pass | `docs/README.md` |
| 권위/현재/역사 구분 | Pass | README 권위 표와 역사 문서 규칙 |
| 작업별 LLM 라우팅 | Pass | `docs/LLM_DOCUMENT_INDEX.json`의 10개 route |
| canonical 경로만 색인 | Pass | `tools/TestDocumentationIndex.ps1` |
| 기존 루트 링크 호환 유지 | Pass | 기존 리다이렉트는 이동/삭제하지 않고 전수 검사 |
| 기존 의미·증거 경로 보존 | Pass | 문서 본문/증거 파일의 물리 이동 없음 |

## Verification

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/TestDocumentationIndex.ps1
# DocumentationIndex=PASS IndexedPaths=37 Routes=10 RootRedirects=99

git diff --check
# Pass (whitespace errors 없음)
```

## Evidence

- `docs/README.md`
- `docs/LLM_DOCUMENT_INDEX.json`
- `tools/TestDocumentationIndex.ps1`
- `docs/admin/OPENVISIONLAB_DOCUMENTATION_MAP.md`
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`

## Boundary / Next Dependency

이 작업은 탐색과 문서 소유권을 정리한 것이며, 수백 개 역사 문서를 요약·삭제하거나 제품 기능/소스 구조를 변경하지 않는다. 새 핵심 문서가 생길 때 색인 route와 검증을 함께 갱신해야 한다.

## Closure Record

```text
Status: Complete
Scope: LLM-first document entrypoint, canonical route index, redirect/index validation
Acceptance criteria: single entrypoint -> pass; authority/history split -> pass; 10 task routes -> pass; 37 indexed paths and 99 redirects -> pass
Verification: TestDocumentationIndex.ps1 PASS; git diff --check PASS
Evidence: docs/README.md; docs/LLM_DOCUMENT_INDEX.json; tools/TestDocumentationIndex.ps1; this report
Boundary / next dependency: no historical document deletion or product/runtime change; update the index only when a durable repeated entrypoint changes
```
