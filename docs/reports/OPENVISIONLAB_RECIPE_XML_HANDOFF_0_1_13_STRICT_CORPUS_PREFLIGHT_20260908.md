# Recipe XML Handoff 0.1.13 평가 자료 동결 및 사전 검사

Date: 2026-09-08 KST
Status: Complete

## 완료 범위

일반 검사 설계 스킬 `1.0.1`과 XML 전달 후보 `0.1.13`에 맞춘 새 Round 1
평가 자료의 보정·동결·정적 사전 검사를 완료했다. 실제 Authors/Reviewers 평가는
실행하지 않았다. 후보는 explicit-only, inactive, unqualified 상태를 유지한다.

- 새 identity: `openvisionlab-rule-based-round1-v0_1_13-strict-corpus-20260908`
- 자료 위치: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v0_1_13-strict-corpus-20260908`
- 준비/검증 스크립트 및 원본 보존 근거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-corpus-preflight-20260908`
- 입력 버전: Dev HEAD `d875559577c85984d54900df973a6fb35fb20146`, 기존 dirty worktree 보존.
- 분류: `EXPOSED_SELECTION_REGRESSION_ONLY`. 새로운 비공개 평가나 인간 대비 성능 자료가 아니다.

현재 제품은 OpenCvSharp4 기반 결정적 룰베이스 Recipe workbench이며, 기록된 환경의
RC/pre-production 근거를 가진다. 이 검사는 현장 성능·상용 완성도·계측 정확도의
증거를 추가하지 않는다. 명시적 실행, 결과 근거 보존, 동일 Recipe의 재현성을
유지하며 camera/lighting/PLC/I/O/MES/deployment 영역은 포함하지 않는다.

## 확인된 입력 문제와 보정

| 대상 | 보정 및 근거 |
| --- | --- |
| 일반 사례 7개 | S1-01..03, S2-01..04의 깨진 한국어를 `rule-based-skill-round1-20260831`의 정상 `operatorIntent`와 정확히 일치하도록 복원. 영상 해시와 수치 lock의 동등성을 먼저 확인했다. |
| 공유 mask 사례 | S4-03 공개 요청에 하나의 Threshold 170 mask와 두 Blob 소비자, 뒤쪽 `ALLOW_BRANCH_INPUT=true`를 명시. 기존 비공개 기대 및 공개 보정 계약에 맞추며 ROI/area/ratio 값을 바꾸지 않았다. |
| 지원 기능 누락 | RT19를 미등록 Tool 생성 요청과 구분되는 기능 검토 요청으로 정리하고 기대 상태를 `WAIT`로 한정했다. RT17의 명시적인 미등록 Tool 생성은 `REJECTED`로 한정했다. |
| 이미지별 교체 | RT32에 특정 이미지에만 template/ROI를 교체한다는 조건을 명시해 전역 graph 변경과 구분했다. |
| 공개/비공개 연결 | 32개 공격 사례의 요청·입력을 공개 파일에서 비공개 설계에 다시 투영했다. RT04 비공개 입력의 오래된 `actualSha256`도 보정했다. 공격용 `providedSha256`은 그대로 유지했다. |
| 공개 판정 규칙 | 기존 공개 보정 계약의 상태/이유 패턴과 소유권·직렬화 규칙을 `frozen/public/authoring-policy.md`로 고정하고 양쪽 Author protocol에서 읽도록 연결했다. case ID별 정답은 공개하지 않았다. |
| 실행 의존 파일 | 설치된 세 스킬과 정적 검사 프로그램·runtime을 D:에 복사했다. 평가 지시와 감사 스크립트는 이 복사본을 가리키며, 스킬/검사 프로그램의 바이트 동일성과 runtime 전체 목록을 해시로 보존했다. |

정책 근거는
`docs/roadmap/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_CORRECTION_CONTRACT_20260904.md`다.
`repairs.json`에 사례별 원문 경로와 SHA-256을 기록했다. 일반 사례 16개의 기대
내용과 모든 영상·수치 lock은 유지했다. 새 기준 자료의 경로·연결 해시·handoff
버전은 새 identity에 맞추었으며, XML 바이트와 보호되는 상태·자격·검증·동작 의미는
유지했다. 기존 0.1.11 자료 1,800개 파일은 경로·길이·SHA-256 비교가 모두 일치한다.

## 최종 동결

| 항목 | 값 |
| --- | --- |
| `freeze-record.json` SHA-256 | `17A025D198AD978BB0161EBCC16579763B68BF8DEE8633D260A7D366B34D6AAB` |
| 동결 시각 UTC | `2026-09-07T22:22:44.344845Z` |
| 기존 0.1.11 freeze SHA-256 | `0DF579083B7F72C473A2C97B5C875FBE97FEB152101E9615421C4CAC308845D8` |
| 준비 수 | 일반 16개 × 5회 = 80, 공격 32개 × 1회 = 32, 합계 112 |
| Author 설정 | `gpt-5.6-luna`, `medium`, 900초, 매회 독립 context |
| Reviewer 설정 | `gpt-5.6-sol`, `high`, 900초, 16개 독립 context |
| 실패 분모 | 누락·타임아웃·모델 오류를 분모에 유지 |

최종 동결 이후 `prepare`를 실행했다. 생성된 112개 메타데이터 모두 최종 freeze와
정확히 일치하며, `attempts`에는 메타데이터 외 파일이 없다. 새 Author 결과,
Reviewer 결과, 점수, 실행 중 감사 프로세스는 없다. 기존 `S2-04-05`의 900초
TIMEOUT과 이전 품질 FAIL은 유지된다. backend health는 검사하지 않았다.

## 실행한 검증

다음 근거는 새 자료의 `preflight`에 있다. 실제 명령·인자·작업 폴더·종료 코드는
`commands.json`, 사전 준비 코드는 별도 근거 폴더의 `prepare_corpus.py`,
`verify_corpus.py`, `repair_preflight_order.py`, `final_checks.py`에 보존했다.

| 검사 | 결과 | 근거 |
| --- | --- | --- |
| 세 스킬 형식 | PASS | `*-quick-validate.txt` |
| XML 후보 회귀 | 29 tests, OK | `candidate-regression.txt` |
| harness/runner 회귀 | 15 + 1 tests, OK | `harness-regression.txt`, `runner-regression.txt` |
| 기준 handoff / 보호 필드 투영 | 모두 PASS, errors=[] | `baseline-direct-validator.txt`, `baseline-projection-validator.txt` |
| 정적 XML 호환성 | 13 XML roots, 1 recipe XML PASS | `baseline-static-compatibility.txt` |
| Python 문법 | 5개 파일 PASS, bytecode 생성 없음 | `syntax-validation.json` |
| 계약/자체 검사/prepare | PASS, 112 records | `contract-validation.json`, `self-test.txt`, `prepare.txt` |
| 감사 분류 자체 검사 | 합성 입력 12개 PASS | `audit-self-test.txt` |
| 최종 자료 무결성 | 20개 gate PASS; 경로/해시 273쌍 및 112개 메타데이터 일치 | `final-gate.json` |
| 레지스트리·문서·작업 범위 | PASS | 별도 근거 폴더의 `documentation-checks.json`, `documentation-index.txt`, `registry-validation.txt` |

정적 검사 명령의 첫 인자는 복사된 runtime 디렉터리, 두 번째 인자는 기준 XML이
있는 디렉터리다. 제품 desktop EXE 실행 없이 serializer 호환성을 확인했다.
현재 제품 소스를 새로 빌드한 결과라고 주장하지 않는다. 제품 build, WPF UI,
실제 이미지 검사, 장시간 운용, 본 평가·감사 세션·점수 계산은 실행하지 않았다.

### 사전 검사 중 수정한 검증 문제

Windows `Path` 정렬과 계약의 ordinal 문자열 정렬이 달라 최초 runtime 목록 검사가
실패했다. 메타데이터 생성 전에 정렬과 종속 해시를 고치고 다시 검사했다.
최초 기록은 `initial-ordering-failure-*`, 수정 근거는 `ordering-repair.json`에 있다.
최종 freeze 뒤 입력을 다시 고치지 않았다.

설치 스킬 비교의 첫 검사에서는 원래 복사에서 제외한 기존 `__pycache__` 4개를
포함해 불일치가 발생했다. 비교에도 동일 제외 규칙을 적용했고 스킬 소스 바이트는
일치한다. 첫 결과는 `initial-cache-inventory-failure.json`에 남겼다.
레지스트리 갱신 전 검사에서는 동일한 근거 키 두 개가 중복된 것을 확인해 하나씩만
유지했다. 버전·dispatch·권한은 바꾸지 않았다.
감사 자체 검사의 첫 호출은 기본 `AuditRoot`가 빈 문자열로 전달되어 인자 바인딩이
실패했다. 실제 실행 wrapper가 사용하는 명시적 `-AuditRoot`로 12개 분류 검사를
통과했다. `invocation-corrections.json`에 두 호출을 구분했다.

## 종료 및 다음 경계

Status: Complete
Scope: 0.1.13 새 평가 자료 보정·동결·정적 사전 검사 및 112개 메타데이터 준비.
Acceptance criteria: 새 identity와 원본 보존, 공개/비공개 정책 일치, 집중 검증 통과,
최종 freeze-before-prepare, 평가 결과 없음, 재사용 가능한 근거/문서 연결 모두 충족.
Verification: 위 표의 실제 검사와 로그.
Evidence: 이 문서와 두 D: 근거 폴더.
Boundary / next dependency: 본 평가 실행의 별도 승인과 직전 backend/의존 파일
상태 확인이 필요하다. 900초 조건을 바꾸려면 실행 전 새로운 동결 결정을 기록해야
한다. 이 사전 검사는 후보 활성화·현장 자격·제품 실행·Original 변경·commit/push를
승인하거나 입증하지 않는다.

승인된 본 평가를 시작하기 직전에 다음 자료 검사를 재사용할 수 있다.

```powershell
python -X utf8 -B D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-corpus-preflight-20260908\final_checks.py
```

동일 작업을 반복하지 말고, 위 최종 freeze와 현재 의존 파일의 유효성을 먼저
확인한다. 제품의 별도 우선순위는 현재 handoff의 Learn Matching 상태/표현 책임
분리이며 이 스킬 작업이 그 상태를 대체하지 않는다.

다음 스킬 우선순위: 본 평가 112회 및 독립 검토 16회 실행 여부 결정.
승인과 backend 전제 확보 전에는 평가를 실행하지 않는다. 실행 관리 시
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
