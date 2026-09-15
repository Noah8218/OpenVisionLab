# 룰베이스 스킬 프롬프트 반영

Date: 2026-09-08 KST
Status: Complete
Scope: 첨부 검사 설계 프롬프트를 기존 통합 teaching 스킬에 반영하고 XML 전달 후보의 버전 호환성을 보존한다.

## 작업 기준과 완료 조건

사용자가 제공한 20절 프롬프트와 “룰베이스 스킬을 이어나가”라는 요청을
기준으로 기존 스킬을 보완한다. 새 알고리즘 전문 스킬이나 제품 기능은 만들지 않는다.

- 20절 요구사항을 기존 지침/추가 참조에 대응시킨다.
- 현재 제품의 Tool·파라미터·좌표·판정 계약과 프롬프트 예제를 구분한다.
- 통합 스킬 `1.0.1`과 기존 `1.0.0`의 v1 envelope 호환성을 유지한다.
- Skill Creator, registry, XML 후보의 집중 회귀, 문서 색인, 독립 행동 평가를 통과한다.
- 수정 전후 파일·해시·검사 명령·평가 결과를 D 드라이브에 보존한다.

제품은 OpenCvSharp4 룰베이스 레시피 워크벤치다. 현재 핸드오프의 성숙도는
기록된 환경에 한해 RC/pre-production이며, 현장 장시간 신뢰성이나 상용 GA의
증거가 아니다. 명시적 실행, 짧은 티칭 흐름, 판정 이유와 도형/측정값 연결을
유지한다. 카메라·조명·PLC/I/O·장비 통합은 이번 범위 밖이다.

## 시작 상태와 재개 판단

- Dev HEAD: `d875559577c85984d54900df973a6fb35fb20146`.
- 기존 dirty 작업은 유지한다. 제품 코드·Original·commit·push는 대상이 아니다.
- 기존 통합 스킬 `1.0.0 active`, Matching `0.2.1 active`, XML handoff
  `0.1.12 candidate`를 확인했다.
- XML `0.1.12` 집중 보정은 2026-09-04에 이미 완료됐다. 마지막 전체 평가는
  `0.1.11`의 완료된 실패 평가이며 중단된 실행이 아니다: 구조 108/112,
  unsupported/invented 16건, red fail-closed 29/32, Reviewer 14/16.
- 이번 작업은 첨부 프롬프트에 해당하는 일반 검사 설계 지침을 이어간다.
  이전 frozen corpus·평가 산출물을 수정하거나 새 대규모 평가를 실행하지 않는다.

원문은 사용자 첨부 `pasted-text.txt`이며, 전체 경로와 SHA-256은 증거 폴더의
`before-manifest.json`에 보존했다. 프로젝트 전체 문서 탐색은
`docs/README.md`, `docs/LLM_DOCUMENT_INDEX.json`의 `start_or_continue`와
`codex_rule_based_skill_suite_governance`, 현재 핸드오프, 제품 목표, 관련
안정/Tool/result 계약 및 최근 스킬 보고서를 통해 수행했다. 모든 역사 문서를
현재 규칙으로 읽거나 전체 문서를 전수 재검증했다는 뜻은 아니다.

## 원문 20절 대응

| 원문 절 | 반영 위치/판단 |
| --- | --- |
| 1 최우선 원칙 | 기존 evidence/frame/recipe 계약 유지; 구현 책임·중복 방지 보완 |
| 2 검사 목적 | 목표·출력·단위·수용 기준 명시 |
| 3 이미지 특징 | 밝기/대비·형상·자세·노이즈·제품/불량 편차 관찰표 |
| 4 Tool 선택 | 실제 지원 확인, histogram/polarity, Blob 복수 특징, Contour/Edge/Filter 선택 근거 |
| 5 Pipeline | 최소 Tool chain, 중복 edge/threshold 처리 방지 |
| 6 ROI | 기존 detection-point/frame 계약 재사용; 소비자 frame에 맞는 변환 |
| 7 Parameter | 실제 key·owner·단위·근거·지원 범위 표 |
| 8 판정 구조 | 실행·검출·측정·수용 판정 분리 |
| 9 실패 이유 | 실제 오류 식별자와 설명 분리; 새 코드 발명 금지 |
| 10 Robustness | 작업에 맞는 변동 범위·실행/미실행 구분; 합성 변형 한계 |
| 11 FP/FN | 불량=positive 정의, 정상 오검출/불량 미검출·분모·오류 행 분리 |
| 12 Tuning | 분포·중첩·선정/검증/held-out 분리 |
| 13 재사용 | Tool/property/Pipeline/result owner 추적; 필요한 확장만 허용 |
| 14 Mat 수명 | 생성·차용·보관·Dispose·SubMat 부모·clone 수명 |
| 15 성능 | ROI→복사→색 변환→연산 수→알고리즘→병렬화; 비교 조건·시간 분포 |
| 16 Debug Image | 정확한 실행 그림·선택/탈락·검증용 보존; 상시 무제한 저장 금지 |
| 17 로그 | 기존 결과/로그 채널과 source/recipe/parameters/metrics/reason 연결 |
| 18 테스트 | Good/Bad/Boundary/absent/multiple/invalid/empty/parameter/variation 행 |
| 19 금지 | 기존 비조작·비과장 원칙 및 신규 구현/메모리 지침에 통합 |
| 20 출력 | 검사 설계용 10항목; 코드·실행 결과가 없으면 해당 상태를 명시 |

## 실제 적용과 호환성

- `openvisionlab-rule-based-teaching/SKILL.md`: version `1.0.1`, 조건부 참조
  및 검사 설계 출력 연결. 기존 owner·dispatch·권한 유지.
- `references/inspection-design-contract.md`: 원문을 제품 계약에 맞춰 반영한
  재사용 참조. 전문 Matching 정책과 machine envelope는 복제하지 않는다.
- `references/evidence-packet-contract.md`: `skillVersion`만 `1.0.1`로 동기화.
- XML 후보 검증기는 upstream `1.0.0`만 허용하므로, 새 통합 버전이 무조건
  거부되는 호환성 문제를 함께 수정한다. 후보 `0.1.13`은 명시적으로 검토한
  `1.0.0`/`1.0.1`만 허용하며 알 수 없는 버전과 모든 기존 의미/해시/권한
  오류를 계속 거부한다. v1 schema는 바뀌지 않는다.
- registry가 스킬 버전 소유자이며, 일반 envelope 예제와 후보
  SKILL/reference/validator/test를 동기화했다. 두 버전 변경은 기존 책임 안의
  호환 보완이므로 PATCH다. XML 후보는 explicit-only·candidate·unqualified다.

원문의 `.NET Framework 4.8 / C# 7.3`은 현재 앱 프로젝트의
`net8.0-windows7.0`와 다르므로 설치 지침으로 고정하지 않는다. `MinArea`,
`ThresholdMin`, `ObjectNotFound` 같은 설명용 이름은 실제 catalog/API/XML
이름이 아니다. Circularity 등 일반 OpenCV 개념을 지원된 product metric으로
발명하지 않는다. mm 예시는 실제 보정·Tool 근거 없이 사용하지 않는다.

## 검증과 증거

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-prompt-20260908`

| 검사 | 실제 결과 | 기록 |
| --- | --- | --- |
| 일반/후보 Skill Creator | 두 스킬 모두 `Skill is valid!`, exit 0 | `general-quick-validate.txt`, `candidate-quick-validate.txt` |
| XML 후보 회귀 | 29 tests, OK, exit 0 | `candidate-regression.txt` |
| 버전 신뢰 경계 | 1.0.0/1.0.1 허용, 미래/누락/잘못된 타입 버전 거부, per-image 변경 거부 | 신규 회귀 1건의 하위 사례 |
| 레지스트리 | PASS, errors=[] | `registry-validation.txt` |
| envelope/정책 | skillVersion 외 예제 JSON 동일; version·dispatch·권한·Matching entry·두 openai.yaml 보존; Python compile() PASS | `envelope-schema-equivalence.txt`, `verification-results.json` |
| 독립 설계 사례 | 원본 영상 없음·FP/FN/ERROR 분리·Circularity 비발명·24시간 미입증, PASS | `forward-design-request.json`, `forward-design-response.md`, `forward-design-verdict.json` |
| 독립 책임 분리 사례 | Matching 전문 소유권, 정규화/고정 ROI, 수치 미확정, 명시 실행 경계, PASS | `forward-routing-request.json`, `forward-routing-response.md`, `forward-routing-verdict.json` |
| 독립 변경 검토 | 추가 수정이 필요한 문제 없음 | `independent-diff-review.txt` |
| 문서 색인 | PASS, 185 paths / 13 routes / 102 redirects | `documentation-index-validation.txt` |

주요 검증 명령은 다음과 같다. `verification-results.json`에는 해석 없이 다시
실행할 수 있는 실제 Python 경로와 인자 배열이 보존되어 있다.

```powershell
python -X utf8 -B D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-skill-prompt-20260908\verify_skill_update.py
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestDocumentationIndex.ps1 -RepoRoot C:\Git\2D\Dev
```

검사 프로세스의 TEMP/TMP와 회귀 테스트 root는 위 D 드라이브 물리 디렉터리다.
Python bytecode 파일을 소스 폴더에 만들지 않았다. 원문/기존 파일은
`before-manifest.json`과 `before/`, 최종 수정 파일과 해시는
`changed-file-manifest.json`, 이번 작업만의 diff는 `current-task.diff`에 있다.
최종 whitespace/경로/해시 검사는 `final-checks.json`에 보존한다.

증거용 비교 도우미의 첫 실행은 `git diff --no-index`의 정상 차이 exit 1과
줄바꿈 안내를 실패로 분류했다. 깨끗한 차이(exit 1)와 실제 trailing whitespace
(exit 3)를 대조 파일로 확인해 분류를 수정했다. 최초 결과와 대조 근거는
`final-checks-initial-exit-code-classification.json`, `diff-exit-code-control.json`에
보존했다. 제품/스킬 코드의 실패는 아니며 Git 설정을 영구 변경하지 않았다.

독립 평가자는 초기화된 별도 context에서 실제 요청·스킬 경로·읽기 전용
저장소만 받았다. 의도한 답·수정 가설·다른 평가 결과는 제공하지 않았다.
응답과 별도의 부모 판정 파일을 보존했다. 두 사례는 설계/책임 분리 근거이며
XML 후보의 전체 Author/Reviewer 품질이나 영상 검사 성능을 증명하지 않는다.

제품 소스가 바뀌지 않아 제품 build, desktop EXE/WPF smoke, 실제 영상 실행,
Matching 이미지 검증, 전체 회귀 및 Round 1 벤치마크는 실행하지 않았다.
스킬 전용 변경의 검증 범위는 기존 governance 계약을 따른다.


## 종료 기록

Status: Complete
Scope: 통합 검사 설계 스킬 보강 및 XML 후보 버전 호환 보완.
Acceptance criteria: 20절 대응, 현재 계약에 맞춘 지침, 1.0.0/1.0.1 호환과 strict gate 보존, 형식/레지스트리/29개 회귀/독립 2사례/문서 검사 통과.
Verification: 위 표의 실제 명령과 결과 및 final-checks.json.
Evidence: 위 D 드라이브 before/after 해시, 실제 평가 응답·별도 판정, 실행 로그 및 현재 문서.
Boundary / next dependency: 실제 영상 검증, 24시간 운용/계측 증거, 대규모 XML
평가·활성화·제품 실행·Original·commit·push·release·deployment는 입증하지 않는다.
