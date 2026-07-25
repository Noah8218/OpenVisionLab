# Next Codex Prompt

아래 프롬프트를 다음 Codex 대화창에 그대로 붙여 넣어 이어서 진행하세요.

```text
작업 위치는 우선 C:\Git\OpenVisionLab_Dev 입니다.

주의: C:\Git\OpenVisionLab_Dev에서 구현/검증을 진행하고, 안정화한 변경을 기존 OpenVisionLab repo인 C:\Git\OpenVisionLab에 검토 후 반영/커밋하는 흐름입니다. 원본 반영은 변경 범위와 공개 안전성을 확인한 뒤 patch/cherry-pick/import 방식으로 진행하고, 무작정 대량 덮어쓰기는 하지 마세요.

먼저 아래 문서를 읽고 현재 상태를 파악한 뒤 작업을 시작해 주세요.

1. C:\Git\OpenVisionLab_Dev\AGENTS.md
2. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_NEXT_SESSION_HANDOFF.md
3. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md
4. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_PRODUCT_IDENTITY_AND_ROADMAP.md
5. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_PUBLIC_SAMPLE_ASSET_POLICY.md
6. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_EXTERNAL_REFERENCE_POLICY.md
7. C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_RELEASE_VERSION_POLICY.md

현재 상태:

- Dev 작업 결과는 원본 GitHub repo의 main에 병합되었습니다.
- 병합 커밋은 `60c6e32 Import public-safe OpenVisionLab handoff (#1)` 입니다.
- 공개용 브랜치 `codex/public-safe-handoff-20260703`에서 main으로 PR merge가 완료되었습니다.
- 실제 구현/검증 작업은 우선 `C:\Git\OpenVisionLab_Dev`에서 진행합니다.
- `C:\Git\OpenVisionLab`은 기존 OpenVisionLab 원본 repo이며, Dev에서 안정화한 변경을 검토한 뒤 반영/커밋하는 대상입니다.
- GitHub Desktop에 `Stashed changes`가 보이면 Restore하지 마세요. `.codex`, `tmp` 중심의 과거 로컬 검증 산출물일 가능성이 큽니다.
- 사용자가 명시적으로 `PUSH`를 요청하지 않는 이상 `git push`를 실행하지 마세요. 커밋 후에도 푸시는 별도 요청이 있을 때만 진행합니다.
- 완료는 말로 판단하지 말고 실제 명령 통과로 판단하세요. 변경 범위에 맞는 build/test/readiness/lint/typecheck를 실행하고 결과를 보고하세요.
- 모르는 것은 추측하지 말고 관련 파일, 테스트, 로그, 스크린샷, 명령 출력을 열어 근거를 확인하세요.
- 간단 수정은 낮은 reasoning effort로 빠르게 처리하고, 설계/MVVM/도킹/성능/대규모 변경은 높은 reasoning effort로 가정과 검증 범위를 먼저 정리하세요.

OpenVisionLab의 정체성:

- OpenVisionLab은 OpenCvSharp4 기반 rule-based vision workbench입니다.
- 초보자도 샘플을 따라 하며 Threshold, Blob, Contour, Line/Length, Matching, EdgeBasedMatching, Feature/Shape 계열 검사를 익히고, 숙련자는 PropertyGrid 기반 파라미터와 Pipeline/Recipe XML로 검사를 구성하는 프로그램입니다.
- 카메라/조명/PLC/I/O 통합 장비 플랫폼이 아니라, 이미지 기반 룰베이스 알고리즘 검증/학습/레시피 구성 도구입니다.

반드시 지킬 안정 계약:

- 알고리즘 툴은 PropertyGrid 기반 구조를 유지합니다.
- View code-behind에는 업무 로직을 최소화하고 ViewModel, Controller, Presenter, Behavior, Converter, Runtime/Service로 분리합니다.
- Output 레이어 생성이 Input 레이어를 자동 변경하면 안 됩니다.
- Boolean visibility toggle만으로 Preview/Run이 실행되면 안 됩니다.
- 레이어 생성/삭제/이미지 로드는 자동 Run을 유발하면 안 됩니다.
- Viewer zoom/pan/drag, ROI overlay, template editor, 레이어 비교/도킹 기능을 제거하지 마세요.
- 안정 계약 문서에 기록된 동작은 임의로 바꾸지 마세요.
- `Dirkster.AvalonDock` PackageReference를 `OpenVisionLab.csproj`에 직접 추가하지 마세요.
- root `Sample/` 또는 SDK 샘플을 공개 경로에 다시 넣지 마세요.
- `dll\Library-Noah\OpenCvSharpExtern.dll`을 다시 추가하지 마세요. native runtime은 `dll\OpenCVSharp\OpenCvSharpExtern.dll`만 공유 사용합니다.

먼저 실행할 확인:

```powershell
cd C:\Git\OpenVisionLab
git fetch origin
git status --short
git log --oneline -5
cd C:\Git\OpenVisionLab_Dev
git status --short
git log --oneline -5
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
```

변경 범위에 별도 테스트/린트/타입체크가 있으면 해당 명령도 완료 조건에 포함합니다.

다음 우선순위:

1. MainView/Product sample review 실제 사용자 흐름 자체 평가
2. 부족하면 in-app guide, result/failure explanation, sample review affordance를 큰 단위로 개선
3. Product sample catalog 품질 감사와 현장감 있는 샘플 보강 여부 판단
4. Tool View code-behind 축소와 공통 runtime/controller/template 정리
5. Pipeline/Recipe operator review UX 보강

UI/UX 변경 시 반드시 같은 시나리오로 이전/이후 캡처를 남기고 비교해 주세요. 이전 캡처를 재사용하지 말고 현재 EXE/current build 기준으로 캡처합니다.

작업 완료 후 한국어로 아래 항목을 보고해 주세요.

- 변경한 파일
- 구조 변경 내용
- 검증 결과
- 남은 위험/후속 작업
- 다음 우선순위
```
