# OpenVisionLab Next Session Handoff

Updated: 2026-07-03 18:05 KST

이 문서는 다음 세션에서 바로 이어가기 위한 최소 인수인계 문서입니다. 작업은 항상 `C:\Git\OpenVisionLab_Dev`에서 먼저 구현/검증하고, 안정화한 변경만 `C:\Git\OpenVisionLab` 원본 repo에 reviewed patch/import 방식으로 반영합니다. 사용자가 명시적으로 `PUSH`를 요청하지 않는 이상 `git push`를 실행하지 않습니다.

## 현재 제품 방향

- OpenVisionLab은 OpenCvSharp4 기반 rule-based vision workbench입니다.
- 목표는 이미지 기반 룰베이스 알고리즘 학습, 검증, 레시피 구성입니다.
- 카메라, 조명, PLC, I/O 통합 장비 플랫폼이 아닙니다.
- 알고리즘 Tool은 PropertyGrid 기반 구조를 유지합니다.
- Preview/Run은 명시적 사용자 동작이어야 하며 layer create/delete/load-image, visibility toggle, output layer 생성만으로 자동 실행되면 안 됩니다.
- Viewer zoom/pan/drag, ROI overlay, template editor, layer compare/docking 기능은 유지해야 합니다.

## 원본 repo 최신 안정 커밋

`C:\Git\OpenVisionLab` main 기준 최근 안정 커밋:

- `e98a0b2 Clarify NG review fix guidance`
- `26f95f1 Document OpenVisionLab self evaluation`
- `95ed902 Clarify final review next action`
- `1250eb5 Clarify sample workflow next actions`
- `092e8f5 Use public-safe samples in review smoke`

## 2026-07-03 진행 완료

- Product sample catalog/native runner gate 안정화
  - `dll\OpenCVSharp\OpenCvSharpExtern.dll`를 공유 native runtime으로 사용합니다.
  - `dll\Library-Noah\OpenCvSharpExtern.dll`은 다시 추가하지 않습니다.
  - `RunVisionSampleCatalog.ps1` 최신 Dev evidence: `artifacts\self_evaluation_product_catalog_20260703_1750\sample_catalog_summary.json`
  - 결과: `GateStatus=OK`, `RunnableRows=168`, `RequiredRows=84`, `ExpectedFailureRows=84`, `OKRows=168`, `NGRows=0`
  - 품질 audit: `ProductSampleQualityAudit=PASS | PairRecords=84 OK=84 Review=0 Critical=0`
- 자체 평가 문서 추가
  - `docs\OPENVISIONLAB_SELF_EVALUATION_20260703.md`
  - 상용 제품 대비 평가는 제품 목표 기준 `4.0/5`, 산업용 통합 플랫폼 기준 `2.0/5`입니다.
  - 결론: 상용 장비 플랫폼처럼 넓히지 말고 PropertyGrid tool, layer route, Preview/Pipeline 분리, sample-backed review를 강화합니다.
- MainView/Product sample 흐름 개선
  - sample workflow detail에 다음 행동을 표시했습니다.
  - MainView sample strip의 Pipeline/첫 Step 버튼 가시성을 높였습니다.
  - 원본 커밋: `1250eb5`
- Pipeline Review operator guide 개선
  - 최종 OK 단계에서 더 이상 "다음 Step"으로 오해시키지 않고, 출력/지표/Good-Bad 쌍 비교 후 승인하도록 안내합니다.
  - NG 단계에서 `우선 확인:` 힌트를 추가해 Tool별로 먼저 볼 파라미터/라우트 범위를 안내합니다.
  - 원본 커밋: `95ed902`, `e98a0b2`
- public-safe smoke 정리
  - legacy-named sample review smoke가 root `Sample` 대신 public catalog 샘플을 사용합니다.
  - 원본 커밋: `092e8f5`
- Tool View controller 분리 상태 확인
  - Line/Arithmetic code-behind는 공통 controller를 사용합니다.
  - Dev에서 `wpf_layer_selection_arithmetic_tool`, `wpf_shell_host_line_tool`, `wpf_shell_host_line_presets` 통과 확인했습니다.

## 주요 검증 증거

- Build:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` 통과
- Readiness:
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` 통과
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"` 통과
- Public samples:
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` 통과
- Product catalog:
  - `artifacts\self_evaluation_product_catalog_20260703_1750\sample_catalog_summary.json`
  - `artifacts\product_sample_quality_audit\product_sample_quality_audit.md`
- UI before/after evidence:
  - Product focus before: `artifacts\mainview_product_flow_before_20260703_1735\wpf_shell_host_workspace_sample_product_focus_open.png`
  - Product focus after: `artifacts\mainview_product_flow_after_20260703_1745\wpf_shell_host_workspace_sample_product_focus_open.png`
  - Pipeline Review final before: `artifacts\mainview_product_review_before_20260703_1739\wpf_shell_host_workspace_product_sample_review.png`
  - Pipeline Review final after: `artifacts\pipeline_review_final_next_after_20260703_1746\wpf_shell_host_workspace_product_sample_review.png`
  - Pipeline Review NG before: `artifacts\pipeline_review_ng_guide_before_20260703_1754\wpf_shell_host_pipeline_review_ng.png`
  - Pipeline Review NG after: `artifacts\pipeline_review_ng_guide_after_20260703_1758\wpf_shell_host_pipeline_review_ng.png`
  - Original NG after: `C:\Git\OpenVisionLab\artifacts\original_pipeline_review_ng_guide_after_20260703_1800\wpf_shell_host_pipeline_review_ng.png`

## 다음 세션 시작 체크

```powershell
cd C:\Git\OpenVisionLab_Dev
git status --short
git log --oneline -5

cd C:\Git\OpenVisionLab
git fetch origin
git status --short
git log --oneline -5
```

## 다음 우선순위

1. Pipeline/Recipe operator review UX 추가 보강
   - NG 단계에서 failed step, metric, expected/actual, suggested action을 더 가까이 보여줄 수 있는지 current EXE 캡처로 점검합니다.
2. MainView/Product sample review 실제 흐름 재점검
   - Sample Picker -> Open Sample -> Pipeline Review -> Good/Bad counterpart 비교까지 한 번 더 실제 사용자 흐름으로 확인합니다.
3. Tool View code-behind 추가 축소
   - Line/Arithmetic 이후 남은 반복 wiring을 찾되, PropertyGrid 구조는 유지합니다.
4. Product sample catalog 보강 여부 판단
   - 현재 84 pair audit는 PASS입니다. 새 샘플 추가보다 대표 샘플의 설명/비교 affordance 강화가 우선입니다.

## 주의

- UI/UX를 수정하면 현재 EXE/current build 기준 before/after 캡처를 새로 남깁니다.
- 원본 repo에 Dev를 대량 덮어쓰지 않습니다.
- GitHub Desktop stash가 보이면 임의로 Restore하지 않습니다.
- 외부 SDK sample asset이나 `dll\Library-Noah\OpenCvSharpExtern.dll`을 public 경로에 다시 넣지 않습니다.
