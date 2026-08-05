# OpenVisionLab 문서 시작점

Updated: 2026-08-05 KST

이 파일은 사람과 LLM이 프로젝트 문서를 찾을 때 사용하는 단일 진입점입니다. 문서를 처음부터 전부 읽지 말고, 아래 최소 세트와 작업별 경로만 읽으세요.

기계 판독용 색인은 `docs/LLM_DOCUMENT_INDEX.json`, 전체 상세 등록부는 `docs/admin/OPENVISIONLAB_DOCUMENTATION_MAP.md`입니다.

## 30초 시작 순서

1. `AGENTS.md` — 저장소 규칙, 제품 경계, 변경/검증 계약
2. `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md` — 현재 상태, 최신 완료 근거, 실제 다음 우선순위만 담은 짧은 핸드오프
3. `docs/roadmap/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md` — 제품 정체성과 화면별 책임
4. `docs/contracts/openvisionlab/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md` — 회귀시키면 안 되는 동작

사용 방법이나 초보자 흐름을 찾는 경우에는 `docs/manual/README.md`를 먼저 읽습니다.
배포본의 상단 `Guide` 버튼도 이 원본에서 생성한 단일 HTML을 엽니다.

그 다음 `docs/LLM_DOCUMENT_INDEX.json`의 `routes`에서 현재 작업과 일치하는 항목만 추가로 읽습니다. 수백 KB의 `OPENVISIONLAB_NEXT_SESSION_HANDOFF.md`는 특정 P 번호나 과거 결정의 상세 근거가 필요할 때만 검색합니다.

## 문서 권위 순서

충돌할 때는 아래 순서를 따르고, 충돌을 숨기지 않습니다.

| 순위 | 문서 | 용도 |
| --- | --- | --- |
| 1 | `AGENTS.md` | 작업 규칙, 저장소/제품 경계, 검증 의무 |
| 2 | `docs/contracts/**` | 안정 동작, XML, 배포, 외부 참조 계약 |
| 3 | `docs/roadmap/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md` | 제품 정체성과 책임 소유권 |
| 4 | `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md` | 최신 상태, 증거, 다음 우선순위 |
| 5 | `docs/reports/**` | 특정 작업의 완료·실패·한계 증거 |
| 6 | `docs/admin/OPENVISIONLAB_NEXT_SESSION_HANDOFF.md` 및 과거 평가 | 상세 연대기와 역사적 문맥 |

오래된 완성도 백분율, 우선순위, 상용 비교 결론은 현재 사실로 재사용하지 않습니다. 현재 핸드오프와 최신 코드·테스트·스크린샷으로 다시 확인합니다.

## 작업별 빠른 경로

| 작업 | 먼저 읽을 문서 |
| --- | --- |
| 작업 시작/계속 | `AGENTS.md` → 현재 핸드오프 → 제품 목표 → 안정 계약 |
| 현재 상태/다음 작업 | 현재 핸드오프 → 최신 날짜의 관련 `docs/reports/` → 완료 추적기 |
| UI/운영 흐름 변경 | 안정 계약 → 사용자 중심 워크플로 보고서 → 관련 기능 계약 → UI smoke runbook |
| 초보자 매뉴얼/튜토리얼/Learn | `docs/manual/README.md` → `docs/manual/manual-visuals.json` → `docs/learn/OPENVISIONLAB_TUTORIAL.md` → 관련 Tool Learn 문서 |
| Recipe/Pipeline/XML 변경 | 안정 계약 → Vision Tool 계약/결과 계약 → LLM XML 가이드와 Tool Catalog(LLM 호환성이 관련될 때만) |
| 샘플/검증/외부 자산 | Public Sample 정책 → External Reference 정책 → 관련 검증 보고서 |
| 빌드/릴리스/배포 | Release Version 정책 → Source Build 보고서 → Runtime Data Root 계약/보고서 → Production Release Gate |
| 소스 구조/소유권 | `docs/admin/CODEBASE_STRUCTURE.md` → 구조 리팩터링 완료 기록 → 최신 Source Layout Migration 보고서 |
| 상용 제품 비교 | 현재 핸드오프 → Commercial Video Backlog/Queue → 과거 비교 문서(참고만) |
| 특정 `P###` 조사 | 현재 핸드오프에서 검색 → 완료 추적기 → 상세 세션 핸드오프 → 해당 보고서/증거 폴더 |

정확한 경로 목록과 작업별 `read` 배열은 `docs/LLM_DOCUMENT_INDEX.json`이 관리합니다.

## 폴더 의미

- `docs/admin/`: 현재 핸드오프, 문서 지도, 운영·구조 기록
- `docs/admin/archive/`, `docs/roadmap/archive/`: 현재 우선순위로 사용하지 않는 과거 핸드오프·계획 원문
- `docs/contracts/`: 현재 동작/정책/XML/API 계약
- `docs/roadmap/`: 제품 목표와 승인된 개발 큐
- `docs/reports/`: 작업 단위의 결과, 검증, 실패와 한계
- `docs/evidence/`: 원시 또는 상세 검증 자료
- `docs/research/`, `docs/analysis/`: 조사·비교·실험 분석(현재 권위가 아님)
- `docs/runbooks/`: 반복 가능한 실행/Smoke 절차
- `docs/learn/`: 사용자 학습 문서
- `docs/assets/`, `docs/samples/`: 문서 부속 자료와 샘플 메타데이터

## 루트 리다이렉트 규칙

`docs/` 바로 아래의 기존 문서 대부분은 이전 링크 호환을 위한 작은 이동 안내 파일입니다. LLM은 이동 안내에서 가리키는 하위 폴더의 본문을 읽어야 하며, 이동 안내 자체를 권위 문서로 인용하면 안 됩니다.

루트의 문서 진입점은 `README.md`와 `LLM_DOCUMENT_INDEX.json`입니다. 나머지 루트 문서는 이전 링크 호환용 이동 안내입니다. 배포용 튜토리얼은 `docs/learn/OPENVISIONLAB_TUTORIAL_PORTABLE.html`입니다.

## 검색 예시

```powershell
# 파일명 또는 문서 본문 찾기
rg --files docs | rg "CURRENT_HANDOFF|RUNTIME_DATA_ROOT|P276"
rg -n "P276|ResultCount|Preview/Run" AGENTS.md docs

# 최신 보고서 후보 보기
Get-ChildItem docs/reports -File | Sort-Object LastWriteTime -Descending | Select-Object -First 20

# 문서 색인과 모든 루트 리다이렉트 검증
powershell -NoProfile -ExecutionPolicy Bypass -File tools/TestDocumentationIndex.ps1
```

## 문서 갱신 규칙

1. 현재 상태나 우선순위가 바뀌면 `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`를 갱신합니다.
   최근 상태와 활성 조건만 남기고, 상세 명령과 긴 P 이력은 날짜가 있는
   `docs/reports/` 문서에 기록합니다. 완료 항목을 계속 누적해 현재
   핸드오프를 다시 연대기 문서로 만들지 않습니다.
2. 안정 동작이 바뀌면 해당 `docs/contracts/` 문서를 갱신합니다.
3. 완료/실패/검증 한계는 날짜가 있는 `docs/reports/` 문서에 기록합니다.
4. 새 문서가 반복 작업의 주요 입구라면 `docs/LLM_DOCUMENT_INDEX.json`의 관련 route에 추가합니다.
5. 전체 상세 등록이 필요하면 `docs/admin/OPENVISIONLAB_DOCUMENTATION_MAP.md`를 갱신합니다.
6. `tools/TestDocumentationIndex.ps1`가 통과해야 문서 정리가 완료됩니다.
