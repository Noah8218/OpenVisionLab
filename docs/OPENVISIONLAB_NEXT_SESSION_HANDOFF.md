# OpenVisionLab Next Session Handoff

Updated: 2026-07-03

이 문서는 긴 Codex 대화를 다음 세션에서 바로 이어가기 위한 최소 인수인계 문서입니다. 공개 repo에 남길 수 있도록 개인 목적 표현이나 로컬 실험 산출물 경로를 넣지 않습니다.

## 현재 기준

- 작업 기준 repo: `C:\Git\OpenVisionLab_Dev`
- 작업 기준 branch: 현재 Dev 작업 브랜치
- 원본 반영 대상 repo: `C:\Git\OpenVisionLab`
- 원본 반영 대상 branch: `main`
- GitHub merge 완료 커밋: `60c6e32 Import public-safe OpenVisionLab handoff (#1)`
- 구현/검증은 우선 Dev에서 진행합니다.
- Dev에서 안정화한 변경만 원본 repo에 반영하고 별도 커밋합니다.
- 원본 반영은 변경 범위와 공개 안전성을 확인한 뒤 patch/cherry-pick/import 방식으로 진행합니다. 무작정 대량 덮어쓰기는 하지 않습니다.

## 병합 직후 상태

- WPF/MVVM 기반 MainView/ShellHost 전환과 도킹 UX 정리 결과가 원본 GitHub repo의 `main`에 병합되었습니다.
- 공개용 synthetic product sample catalog가 포함되어 있습니다.
- Product sample picker/review UX와 Good/Bad opposite-reference 흐름이 포함되어 있습니다.
- 공개 README, 튜토리얼, sample asset policy, external reference policy, release policy가 정리되었습니다.
- 중복 대형 native DLL `dll\Library-Noah\OpenCvSharpExtern.dll`은 제거되었습니다.
- native OpenCvSharp runtime은 `dll\OpenCVSharp\OpenCvSharpExtern.dll`만 공유 사용합니다.
- 비공개 작업 로그와 과거 벤치마크 분석 문서는 공개 repo에서 제거되었습니다.

## GitHub Desktop Stash 주의

GitHub Desktop에 `Stashed changes`가 보일 수 있습니다.

- 확인된 stash 성격: `.codex`, `tmp`, 일부 로컬 검증 산출물 중심
- 공개 main에 병합된 내용이 아닙니다.
- `Restore`하지 마세요.
- 정리하려면 내용을 확인한 뒤 `Discard`만 고려하세요.

## Git 작업 원칙

- 기본 작업 위치는 `C:\Git\OpenVisionLab_Dev`입니다.
- `C:\Git\OpenVisionLab`은 기존 OpenVisionLab 원본 repo이며, Dev에서 안정화한 변경을 반영/커밋하는 대상입니다.
- 사용자가 명시적으로 `PUSH`를 요청하지 않는 이상 `git push`를 실행하지 않습니다.
- 문서/코드 변경 후 커밋이 필요하면 먼저 변경 범위와 검증 결과를 확인하고 커밋합니다.
- 커밋 후에도 푸시는 별도 요청이 있을 때만 진행합니다.
- GitHub Desktop에서 stash가 보이더라도 임의로 Restore하지 않습니다.

## 작업 협약

루트 `AGENTS.md`를 먼저 읽고 따릅니다. 핵심은 다음과 같습니다.

- 완료는 말이 아니라 명령 결과로 판단합니다. 변경 범위에 맞는 build/test/readiness/lint/typecheck를 실행하고 결과를 남깁니다.
- 모르면 추측하지 않습니다. 관련 파일, 테스트, 로그, 스크린샷, 명령 출력을 열어 근거를 확인합니다.
- 코드 작성 전 목표, 가정, 검증 방법을 먼저 정리합니다.
- 단순한 해결을 우선하고, 요청하지 않은 기능/추상화/에러 처리를 추가하지 않습니다.
- 요청 범위만 정밀하게 수정하고 관련 없는 파일을 건드리지 않습니다.
- 넓은 요청은 "어떤 명령과 smoke를 통과시킬 것인가" 같은 실행 가능한 목표로 바꿉니다.
- 간단 수정은 낮은 reasoning effort로 빠르게 처리하고, 설계/MVVM/도킹/성능/대규모 변경은 높은 reasoning effort로 검토합니다.

## 제품 정체성

OpenVisionLab은 OpenCvSharp4 기반 rule-based vision workbench입니다.

목표는 초보자가 샘플을 따라 하며 검사 원리를 익히고, 숙련자가 PropertyGrid 기반 파라미터와 Pipeline/Recipe XML로 반복 가능한 검사 레시피를 구성하는 것입니다.

범위에 포함되는 것:

- Threshold, Blob, Contour, Line/Length, Matching, EdgeBasedMatching, Feature/Shape 계열 검사
- Good/Bad 샘플 기반 검증
- 결과 이미지, overlay, metric, log를 통한 설명 가능한 판단
- Pipeline/Recipe 저장, 실행, 비교

범위가 아닌 것:

- 카메라/조명/PLC/I/O 통합 장비 플랫폼
- 외부 SDK 샘플 자산을 그대로 공개 배포하는 샘플 저장소

## 안정 계약

다음 계약은 유지해야 합니다.

- 알고리즘 툴은 PropertyGrid 기반 구조를 유지합니다.
- 모델 Property를 PropertyGrid `SelectedObject`에 넣으면 UI가 자동 생성되는 구조가 제품 방향입니다.
- View code-behind의 업무 로직은 ViewModel, Controller, Presenter, Behavior, Converter, Runtime/Service로 분리합니다.
- Output 레이어 생성이 Input 레이어를 자동 변경하면 안 됩니다.
- Boolean visibility toggle만으로 Preview/Run이 실행되면 안 됩니다.
- 레이어 생성/삭제/이미지 로드는 자동 Run을 유발하면 안 됩니다.
- Viewer zoom/pan/drag, ROI overlay, template editor, 레이어 비교/도킹 기능을 제거하지 않습니다.
- 안정 계약 문서에 기록된 동작은 임의로 바꾸지 않습니다.
- `Dirkster.AvalonDock` 직접 참조를 `OpenVisionLab.csproj`에 추가하지 않습니다.
- AvalonDock 패키지 소유권은 `Library\OpenVisionLab.Docking.Controls`에 둡니다.
- root `Sample/` 또는 SDK 샘플을 공개 경로에 다시 넣지 않습니다.
- `dll\Library-Noah\OpenCvSharpExtern.dll`을 다시 추가하지 않습니다.

## 다음 세션 시작 체크

다음 세션에서는 먼저 병합 후 main 상태를 확인합니다.

```powershell
cd C:\Git\OpenVisionLab
git fetch origin
git status --short
git log --oneline -5
cd C:\Git\OpenVisionLab_Dev
git status --short
git log --oneline -5
```

기대 상태:

- branch: `main`
- worktree: clean
- `origin/main`에 `60c6e32` 또는 그 이후 커밋 존재

## 최소 검증

병합 직후 또는 다음 작업 전 다음 검증을 권장합니다.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
```

UI 경로를 수정했다면 관련 WPF/EXE smoke를 추가합니다. Product sample review를 건드렸다면 Product sample review smoke를 우선 실행합니다.

## 다음 우선순위

1. MainView/Product sample review 실제 사용자 흐름 자체 평가
   - 샘플 선택
   - Good 먼저 실행
   - 같은 PairGroup의 Bad 실행
   - 같은 pipeline/metric/log/overlay 비교
   - 실패 원인 해석
   - Markdown 없이 앱 안에서 다음 행동을 이해할 수 있는지 확인

2. 부족한 부분을 큰 단위로 개선
   - in-app guide
   - result/failure explanation
   - sample review affordance
   - 결과 비교/로그 요약

3. Product sample catalog 품질 감사
   - Battery/Display/Semiconductor 도메인별 현장감 확인
   - 저대비, 조명 불균일, 회전, 스케일, 노이즈, 오염, 버 사례 보강 여부 판단

4. Tool View code-behind 축소
   - PropertyGrid 기반은 유지
   - 반복 preview scheduling, parameter commit, summary update를 공통 runtime/controller/template로 이동

5. Pipeline/Recipe operator review UX 보강
   - step별 input/output image
   - branch reason
   - expected metric
   - actual metric
   - OK/NG 판단 근거

## UI/UX 변경 보고 규칙

UI/UX를 수정하면 같은 시나리오로 이전/이후를 비교해야 합니다.

- 변경 전 캡처
- 변경 후 캡처
- 어떤 문제를 줄였는지
- 검증 명령과 결과
- 남은 위험

이전 캡처를 재사용하지 말고, 가능한 현재 EXE/current build 기준으로 캡처합니다.
