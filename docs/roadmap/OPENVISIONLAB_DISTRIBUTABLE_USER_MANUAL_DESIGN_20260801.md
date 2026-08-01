# OpenVisionLab 배포형 사용자 매뉴얼 설계

Updated: 2026-08-01 KST

Implementation status: completed in P281. See
`docs/reports/OPENVISIONLAB_DISTRIBUTABLE_USER_MANUAL_20260801.md` for the
packaging, numbered UI figure, responsive layout, and copied-EXE verification.

## P282 언어별 배포 계약 보완

P281의 단일 한국어 파일 경로는 P282에서 언어별 경로로 대체되었습니다.
현재 계약은 아래 세 파일입니다.

- `Guide\OpenVisionLab_User_Manual.ko.html`
- `Guide\OpenVisionLab_User_Manual.en.html`
- `Guide\guide-manifest.json` (schema 2, 두 언어 파일과 SHA-256)

Guide는 클릭 시점의 프로그램 언어를 읽어 같은 언어 파일만 엽니다. 선택한
언어의 파일·매니페스트 항목·해시·HTML 언어 표식 중 하나라도 맞지 않으면
다른 언어로 대신 열지 않습니다. 아래의 단일 파일명과 schema 1 예시는 P280/
P281 설계 이력을 설명하는 이전 계약입니다. 현재 구현과 검증에는
`docs/manual/README.md`와
`docs/reports/OPENVISIONLAB_LOCALIZED_USER_MANUAL_20260801.md`를 사용합니다.

## 결정

`Guide` 버튼은 저장소의 문서 경로를 찾아 올라가지 않습니다. 배포물에
포함된 단일 오프라인 HTML 매뉴얼을 먼저 열어야 합니다. 개발 환경에서는
소스 문서를 보조 경로로 사용할 수 있지만, 이동 안내 파일은 실행 후보에서
제외합니다.

사용자 매뉴얼은 기능 목록을 나열하는 문서가 아닙니다. 사용자의 작업 순서인
`이미지 준비 -> Tool 설정 -> 명시적 Preview -> 드로잉/Metric 확인 -> Pipeline
저장 -> 명시적 Run Review -> Good/Bad 검증 -> 재시작 후 재실행`을 먼저
설명하고, 각 Tool 문서는 이 흐름 안에서 어디에 쓰는지를 설명합니다.

## 현재 상태와 문제

| 항목 | 현재 | 문제 |
| --- | --- | --- |
| Guide 탐색 | 현재 폴더와 EXE 폴더에서 최대 8단계 상위 폴더의 `docs`를 검색 | 개발 저장소 구조에 의존하며 다른 위치에 복사한 배포물에서는 성립하지 않음 |
| 이전 경로 | `docs/OPENVISIONLAB_TUTORIAL*.html`도 후보 | 해당 파일은 HTML 매뉴얼이 아니라 `Moved to canonical location` 이동 안내 Markdown |
| 배포 파일 | P279 클린 런타임에 Tutorial/Guide 파일 0개 | 저장소 밖으로 복사하면 실제 매뉴얼을 찾을 수 없음 |
| 첫 사용 문서 | Blob 중심 1개 예제로 기본 흐름 설명 | 시작에는 적합하지만 전체 기능 선택과 사용 순서를 설명하지 못함 |
| 기능 문서 | `docs/learn/LEARN_*.md`에 개념과 일부 실행 순서가 분산 | 화면 이름, 선행 조건, 저장, Good/Bad 검증, 실패 복구 형식이 문서마다 다름 |
| 검증 | 소스 문자열과 링크 존재 여부 중심 | 이동된 배포 폴더에서 Guide 버튼이 실제 매뉴얼을 여는지는 증명하지 못함 |

P279의 소스 변경은 `docs/learn`을 이전 루트 경로보다 먼저 찾도록 했지만,
배포물에 매뉴얼을 넣지 않았으므로 배포 문제를 해결한 것으로 볼 수 없습니다.

## 목표 사용자

### 처음 사용하는 작업자

- 이미지와 Layer의 차이를 모릅니다.
- 어떤 Tool부터 선택해야 하는지 모릅니다.
- 설정 변경과 실행이 별개라는 사실을 처음 배웁니다.
- 결과 숫자만 보지 않고 드로잉이 실제 대상을 가리키는지 확인해야 합니다.

### Recipe 작성자

- 여러 Tool을 순서대로 연결합니다.
- 각 Step의 입력/출력 Layer를 확인합니다.
- Good/Bad 샘플과 Validation Set으로 같은 Recipe를 검증합니다.
- 저장, 재시작, `WAIT`, 명시적 Run Review의 관계를 알아야 합니다.

### 검토자

- Pipeline Review, Object Results, Fixture/Geometry/Scale Review, Run History,
  Qualified Snapshot을 사용합니다.
- 실행 성공과 검사 OK/NG를 구분하고 근거 이미지와 Metric을 함께 봅니다.

## 배포 구조

배포 폴더는 아래 구조를 고정합니다.

```text
OpenVisionLab/
  OpenVisionLab.exe
  OpenVisionLab.dll
  Guide/
    OpenVisionLab_User_Manual.html
    guide-manifest.json
```

`OpenVisionLab_User_Manual.html`은 CSS, 검색 스크립트, 필요한 이미지를 모두
내장한 단일 파일입니다. 인터넷 연결, 저장소의 `docs` 폴더, 사용자 TEMP,
브라우저 확장 기능에 의존하지 않습니다.

`guide-manifest.json`은 아래 항목만 가집니다.

```json
{
  "schemaVersion": 1,
  "manualVersion": "2026-08-01",
  "applicationVersion": "2.1.0",
  "file": "OpenVisionLab_User_Manual.html",
  "sha256": "<package-time hash>",
  "language": ["ko", "en"]
}
```

### 경로 해결 순서

1. `AppContext.BaseDirectory\Guide\OpenVisionLab_User_Manual.html`
2. 개발 실행일 때만 저장소의
   `docs\manual\generated\OpenVisionLab_User_Manual.html`
3. 없음 또는 손상이면 안내 대화상자 표시

다음 경로는 열지 않습니다.

- `docs\OPENVISIONLAB_TUTORIAL.html`
- `docs\OPENVISIONLAB_TUTORIAL_PORTABLE.html`
- `Moved to canonical location` 본문을 가진 파일
- 네트워크 URL

개발 보조 경로는 `.git`과 `OpenVisionLab.sln`이 함께 있는 저장소 루트에서만
허용합니다. 일반 배포 실행에서는 부모 디렉터리를 8단계 탐색하지 않습니다.

### 파일 검증

Guide를 열기 전에 다음 조건을 확인합니다.

- 파일 존재
- `data-openvisionlab-manual-version` 표식 존재
- `Moved to canonical location` 문구 없음
- manifest의 파일명과 SHA-256 일치
- 파일 크기 0이 아니며 읽기 가능

검증 실패 메시지는 저장소 이동을 설명하지 않습니다.

```text
사용 설명서 파일이 없거나 손상되었습니다.
OpenVisionLab의 Guide 폴더를 복원하거나 프로그램을 다시 받으십시오.
확인한 경로: <path>
```

## 문서 소유 구조

```text
docs/manual/
  README.md
  manual-manifest.json
  workflows/
    01_FIRST_START.md
    02_IMAGE_SAMPLE_AND_LAYER.md
    03_DIRECT_TOOL_TEACHING.md
    04_RECIPE_AND_PIPELINE.md
    05_GOOD_BAD_VALIDATION.md
    06_VALIDATION_SET_AND_HISTORY.md
  tools/
    THRESHOLD.md
    FILTER.md
    MORPHOLOGY.md
    ARITHMETIC.md
    EDGE_DETECTION.md
    ROTATE_SCALE.md
    AFFINE_TRANSFORM.md
    HISTOGRAM.md
    HSV.md
    MEAN.md
    BLOB.md
    CONTOUR.md
    LINE.md
    MATCHING.md
    EDGE_BASED_MATCHING.md
    FEATURE_MATCHING.md
    PIPELINE.md
  reference/
    TROUBLESHOOTING.md
    GLOSSARY.md
    LIMITS_AND_EVIDENCE.md
  generated/
    OpenVisionLab_User_Manual.html
```

기존 `docs/learn/LEARN_*.md`는 개념 학습 자료로 유지합니다. 사용자 매뉴얼은
정확한 클릭 순서와 작업 완료 조건을 소유합니다. 같은 알고리즘 이론을 두
곳에 복사하지 않고 Tool 문서에서 관련 Learn 문서로 연결합니다.

`manual-manifest.json`은 문서 순서, HTML anchor, 관련 `VISION_MENU`, 관련 Learn
문서, 공개 샘플을 정의합니다. 생성기는 이 manifest 순서대로 Markdown을
Markdig로 변환하고 하나의 HTML로 합칩니다.

## 매뉴얼 정보 구조

### 1부: 15분 첫 실행

1. 프로그램 실행과 언어 선택
2. `샘플 열기`
3. `Public_Blob_Particles_Good` 선택
4. Blob Tool 열기
5. 입력 `Main`, 출력 `Blob_Preview` 확인
6. `기본` preset과 기준값 확인
7. `미리보기 실행`
8. 12개 드로잉과 `ResultCount` 비교
9. `파이프라인에 추가·저장`
10. Pipeline Review에서 명시적으로 `Run Review`
11. Bad 쌍 샘플을 같은 Pipeline으로 실행
12. 저장 후 재시작, `WAIT` 확인, 다시 `Run Review`

### 2부: 공통 작업

- 내 이미지와 공개 샘플 열기
- Image, Layer, ROI, Preview, Pipeline, Recipe, Metric 용어
- Tool 검색, 열기, 우측 고정, 다시 띄우기
- Layer 생성, 이름 변경, 삭제, 입력/출력 선택, 비교
- Recipe 생성/선택과 Tool 설정 복원
- Pipeline Step 추가·저장과 route 확인
- Pipeline Review 실행과 Step별 문제 찾기
- Good/Bad 쌍 비교
- Validation Set, Run History, Qualified Snapshot
- 결과 보고서와 검토 근거 보존

### 3부: 목적에 맞는 Tool 선택

| 하려는 작업 | 먼저 볼 Tool |
| --- | --- |
| 밝고 어두운 영역 분리 | Threshold |
| 노이즈 제거·선명화 | Filter |
| 작은 점 제거·영역 연결 | Morphology |
| 두 Layer 합성·차이·논리 연산 | Arithmetic |
| 경계선 찾기 | Edge Detection |
| 회전·크기 정규화 | Rotate / Scale |
| 세 점 기준 좌표 정규화 | Affine Transform |
| 밝기 분포·대비 확인 | Histogram |
| 색 범위 분리 | HSV |
| 평균 밝기 판정 | Mean |
| 연결 영역 개수·면적·크기 | Blob |
| 외곽 형상·윤곽선 | Contour |
| 엣지·길이·거리·교차점 | Line |
| 고정된 회색조 모양 위치 찾기 | Matching |
| 윤곽 중심 위치 찾기 | Edge Based Matching |
| 텍스처 특징으로 위치 찾기 | Feature Matching |
| 여러 Tool 연결·저장·검증 | Pipeline |

### 4부: 기능별 따라 하기

모든 Tool 문서는 아래 순서를 사용합니다.

1. 이 Tool을 쓰는 목적
2. 사용하면 안 되는 경우
3. 준비할 이미지와 선행 Layer
4. 사용할 공개 샘플
5. Tool을 여는 정확한 위치
6. 입력/출력 Layer
7. 먼저 설정할 최소 파라미터
8. 필요할 때만 펼칠 고급 파라미터
9. `미리보기 실행`
10. 이미지 드로잉에서 볼 것
11. Metric과 OK/NG 해석
12. 실패할 때 확인할 순서
13. Pipeline 추가·저장
14. Good/Bad 재실행
15. 완료 체크리스트와 한계

## 17개 기능의 작업 순서

| 기능 | 설정 순서 | Preview 후 확인 |
| --- | --- | --- |
| Threshold | 입력/출력 Layer -> Binary 방식 -> 기준값/Max -> ROI | 대상과 배경 분리, 흰색/검은색 방향, 출력 Layer |
| Filter | 입력/출력 -> Filter 종류 -> Kernel/Sigma -> ROI | 노이즈 감소와 경계 보존, 과도한 흐림 |
| Morphology | 이진 입력 -> Open/Close/Erode/Dilate -> Kernel -> 반복 | 작은 노이즈 제거, 끊김 연결, 대상 크기 변화 |
| Arithmetic | Layer A -> Layer B -> 연산 종류 -> 계수/오프셋 | 두 Layer 정렬, 포화, 예상한 합/차/마스크 |
| Edge Detection | 방식 -> 임계값/Kernel -> ROI -> 출력 | 실제 경계 위의 edge, 잡음 edge, 누락된 경계 |
| Rotate / Scale | 출력 크기/경계 -> 각도 -> X/Y 배율 | 잘림, 빈 경계, 대상 위치와 downstream ROI |
| Affine Transform | Source/Destination 3점 -> 출력 크기 -> 면적/유효 픽셀 gate | 삼각형 대응, 2x3 행렬, 변환 프레임, 유효 픽셀 |
| Histogram | 입력/ROI -> 채널 -> 평활화/정규화 옵션 | 분포, 평균, 대비 변화, 포화 영역 |
| HSV | H/S/V 범위 -> ROI -> 마스크 옵션 | 선택 영역 비율과 실제 색 영역의 일치 |
| Mean | 입력/ROI -> 채널 -> 허용 범위 | `MeanValueAvg`, 범위, ROI가 의도한 면을 포함하는지 |
| Blob | Threshold 방식 -> ROI -> Area -> Width/Height -> 표시 | object box/중심, accepted/rejected 개수와 이유 |
| Contour | Threshold -> Retrieval/Approximation -> Area/크기 -> 표시 | 실제 외곽선, object 개수, reject 이유 |
| Line | `Purpose` 선택 -> Line A/B -> ROI -> 방향/극성/대비/간격 -> scale | 피팅 edge, 길이 또는 A/B 거리/교차점, 지지점 |
| Matching | Template ROI -> Search ROI -> Score/Count -> 필요 시 Angle/Scale | 대상 위 box, score, count, 반복 패턴/대안 margin |
| Edge Based Matching | Template -> Edge Model/Canny -> Search/Score -> 선택적 Angle/Scale/Refine | 윤곽 box, score, ambiguity, 모델 edge 일치 |
| Feature Matching | 특징 Template -> Ratio -> RANSAC -> 검색 조건 | inlier 기하, 대상 box, 잘못 연결된 특징점 |
| Pipeline | Recipe 선택 -> Step 추가 -> 입출력 route -> 저장 -> Run Review | Step별 OK/NG/WAIT, 첫 실패 Step, 최종 Layer/Metric |

## Tool 문서 예시: Blob

```text
목표: 밝은 입자의 개수를 센다.
준비: Public_Blob_Particles_Good, 입력 Layer Main.

1. 왼쪽 Tool 목록에서 Blob을 연다.
2. 입력 Main, 출력 Blob_Preview를 확인한다.
3. 기본 preset을 선택한다. 이 동작은 실행하지 않는다.
4. 기준값 150, Area 범위를 확인한다.
5. 미리보기 실행을 누른다.
6. 12개 입자 위의 box/중심과 ResultCount=12가 일치하는지 본다.
7. 바깥 타원이나 노이즈가 reject된 이유를 object row에서 확인한다.
8. 맞으면 파이프라인에 추가·저장을 누른다.
9. Sparse Bad를 같은 Pipeline으로 Run Review한다.
10. ResultCount=3이 8..14 밖이라 NG인지 확인한다.
```

완료 조건은 “실행됨”이 아닙니다. 드로잉이 실제 입자를 가리키고 Metric이
그 드로잉을 설명하며 Good/Bad가 의도한 이유로 분리되어야 합니다.

## 화면 설계

단일 HTML은 다음 고정 요소를 가집니다.

- 왼쪽: 단계/기능 목차와 현재 위치
- 위: 검색, 한국어/English, 글자 크기, 처음으로
- 본문 상단: `준비 -> 설정 -> 실행 -> 확인 -> 저장 -> 검증` 진행 표시
- 각 단계: `할 일`, `화면에서 보일 것`, `다르면 확인할 것`
- Tool 페이지 하단: 이전 기능, 다음 기능, 관련 Learn, 공개 샘플
- 고급 기능: 기본 흐름과 분리된 `숙련 사용자` 표식
- 모든 이미지: 실제 UI 전체 문맥, 현재 빌드 버전, 대체 텍스트

검색은 같은 HTML에 포함된 제목, 화면 용어, 파라미터, Metric, 오류 문구만
검색합니다. 외부 서버나 분석 스크립트를 사용하지 않습니다.

## 생성과 배포 계약

1. `manual-manifest.json`의 모든 문서를 읽습니다.
2. Markdig로 Markdown을 HTML로 변환합니다.
3. 내부 anchor와 관련 문서 링크를 검증합니다.
4. 이미지를 data URI로 내장합니다.
5. `data-openvisionlab-manual-version`과 앱 버전을 기록합니다.
6. portable HTML과 manifest SHA-256을 생성합니다.
7. MSBuild가 두 파일을 `Guide` 폴더로 복사합니다.
8. `CopyToOutputDirectory`와 `CopyToPublishDirectory`를 모두 검증합니다.

배포물은 소스 Markdown, 저장소 경로, 테스트 영상, 내부 보고서를 포함하지
않습니다.

## 검증 기준

### 빠른 자동 검증

- 17개 `VISION_MENU` 항목이 매뉴얼 Tool chapter에 정확히 한 번 매핑됨
- 모든 manifest 경로와 HTML anchor 존재
- 모든 로컬 이미지/링크 해결
- portable HTML에 외부 이미지 의존성 0개
- `Moved to canonical location` 문구 0개
- output/publish 폴더에 Guide HTML과 manifest 존재
- manifest SHA-256과 실제 파일 일치
- 임시 외부 폴더에서 resolver가 packaged Guide만 선택
- Guide 열기가 Preview/Run, Layer, Recipe, Pipeline을 변경하지 않음

### 실제 EXE 검증

배포 폴더를 저장소 밖의
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\guide_distribution_smoke\<run>`으로
복사한 뒤 실행합니다. 가장 왼쪽 모니터에서 Guide를 직접 호출하고 브라우저가
packaged HTML을 열었는지 확인합니다. 일반 검증은 빠른 UI Automation을
사용하며 사람처럼 움직이는 커서는 외부 공개용 튜토리얼·소개·홍보 영상을
따로 만들 때만 사용합니다.

### 사용자 이해 검증

CVR-00의 독립 초보자 3명에게 아래 과제를 제공합니다.

1. Guide만 보고 공개 Blob Good을 실행한다.
2. Bad 쌍을 같은 Pipeline으로 실행하고 NG 이유를 설명한다.
3. 새 `Threshold -> Blob` Recipe를 저장한다.
4. 재시작 후 `WAIT`를 확인하고 명시적으로 Run Review한다.
5. 목적 카드만 보고 Mean, Line, Matching 중 알맞은 Tool을 고른다.

관찰 항목은 잘못 누른 버튼, 멈춘 단계, 용어 오해, 결과 해석 오류, 도움 요청,
완료 시간입니다. 에이전트 녹화는 이 검증을 대체하지 않습니다.

## 구현 순서

1. 배포형 Guide 경로 계약과 패키지 파일 포함 | Recommended model: `gpt-5.6-terra` | Reasoning effort: `medium`
2. 매뉴얼 manifest/생성기와 공통 작업 6개 문서 | Recommended model: `gpt-5.6-terra` | Reasoning effort: `medium`
3. 17개 Tool 문서를 동일 템플릿으로 작성하고 현재 UI와 대조 | Recommended model: `gpt-5.6-terra` | Reasoning effort: `medium`
4. 독립 초보자 3명 검증 | Prerequisite: 세 명의 실제 첫 사용자와 미편집 관찰 기록; prerequisite 전에는 모델 토큰을 사용하지 않음

## 완료 기록

```text
Status: Complete
Scope: 배포형 Guide 경로, 오프라인 패키지 구조, 전체 사용자 매뉴얼 정보 구조, 17개 기능 순서, 생성/검증 계약 설계
Acceptance criteria: 현재 결함의 직접 원인 확인 -> pass; 배포 독립 경로 계약 -> pass; 초보자 공통 흐름 -> pass; 17개 기능 순서 -> pass; 구현/검증 gate -> pass
Verification: 현재 resolver 소스 확인; docs-root 이동 안내 파일 확인; P279 clean runtime 내 Tutorial/Guide 파일 0개 확인; 17개 NavigationGroups와 Learn 문서 대조
Evidence: src/OpenVisionLab/UI/Menu/Wpf/Shell/Commands/OpenVisionShellHostCommandController.cs; src/OpenVisionLab/OpenVisionLab.csproj; artifacts/p279_beginner_tutorial_v2_20260801/clean_runtime; docs/roadmap/OPENVISIONLAB_DISTRIBUTABLE_USER_MANUAL_DESIGN_20260801.md
Boundary / next dependency: 설계만 완료; 배포 파일 포함, resolver 수정, 매뉴얼 생성기와 본문 작성은 아직 구현되지 않음
```
