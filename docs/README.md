# OpenVisionLab Documentation

이 저장소의 문서는 아래 분류로 이동해 두었습니다. 루트 `docs`는 카테고리 안내용 인덱스만 유지하고, 실제 본문은 하위 폴더에 위치합니다.

## 핵심 참조 순서

1. `AGENTS.md` (운영 규칙)
2. `docs/OPENVISIONLAB_CURRENT_HANDOFF.md` (현재 상태/다음 우선순위)
3. `docs/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`
4. `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
5. `docs/OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md` + `docs/OPENVISIONLAB_LLM_TOOL_CATALOG.json`
6. `docs/OPENVISIONLAB_PUBLIC_SAMPLE_ASSET_POLICY.md`, `docs/OPENVISIONLAB_EXTERNAL_REFERENCE_POLICY.md`, `docs/OPENVISIONLAB_RELEASE_VERSION_POLICY.md`

현재 작업 우선순위는 문서 위치 정리와 규칙 기반 UI/파이프라인 이해를 방해하지 않는 범위에서의 MVVM 가독성 개선입니다.

## docs 폴더 구조

- `docs/admin/` : 운영 문서, handoff, 정책, 진행 관리용 메모
- `docs/contracts/` : 기능/동작 계약, LLM/툴 스펙, 안정성 계약
- `docs/research/` : 경쟁사/현황/참고조사 자료
- `docs/roadmap/` : 제품 방향, 단계 계획, 이력/요약
- `docs/analysis/` : 성능/실험 분석 노트
- `docs/runbooks/` : 실행 절차, Smoke/Test 가이드
- `docs/learn/` : 사용자 튜토리얼
- `docs/reports/` : 실행 결과 리포트(데이터/회수)
- `docs/evidence/` : 검증 로그/리포트 원문
- `docs/assets/` : 문서 관련 부속 자료

## 루트에 남아 있는 파일 규칙

루트 `docs`에 남아 있는 `*.md` 중 대부분은 **이동 안내 스텁**입니다.
실제 본문은 상기 하위 폴더로 이동되어 링크만 유지합니다.

- 스텁 형식인지 빠르게 확인하려면 본문 맨 위에 `# 이동 안내` 문구가 있는지 확인하세요.
- 본문이 길거나 핵심 운영/계약 문서는 하위 폴더로 이동 후 스텁으로 유지합니다.

## 빠른 작업 체크리스트

1. 작업 시작 전 `git status --short` / `git log --oneline -5` 확인
2. 문서 수정은 `docs/admin/OPENVISIONLAB_DOCUMENTATION_MAP.md`의 우선순위를 준수
3. 코드 변경은 해당 뷰/도메인 문서의 최신 계약 문서와 함께 변경
4. 중요한 구조 변경은 `OPENVISIONLAB_SOURCE_OWNERSHIP_REFACTOR_PROOF_20260717.md`에 반영
