# OpenVisionLab Next Session Handoff

Updated: 2026-07-03 18:30 KST

이 문서는 다음 세션에서 바로 이어가기 위한 최소 인수인계입니다. 작업은 항상 `C:\Git\OpenVisionLab_Dev`에서 먼저 구현/검증하고, 안정화한 변경만 `C:\Git\OpenVisionLab` 원본 repo에 reviewed patch/import 방식으로 반영합니다. 사용자가 명시적으로 `PUSH`를 요청하지 않는 한 `git push`를 실행하지 않습니다.

## 현재 제품 방향

- OpenVisionLab은 OpenCvSharp4 기반 rule-based vision workbench입니다.
- 목적은 이미지 기반 룰베이스 알고리즘 학습, 검증, 레시피 구성입니다.
- 카메라, 조명, PLC, I/O 통합 장비 플랫폼이 아닙니다.
- 알고리즘 Tool은 PropertyGrid 기반 구조를 유지합니다.
- Preview/Run은 명시적 사용자 동작이어야 하며 layer create/delete/load-image, visibility toggle, output layer 생성만으로 자동 실행되면 안 됩니다.
- Viewer zoom/pan/drag, ROI overlay, template editor, layer compare/docking 기능은 유지해야 합니다.

## 원본 repo 최신 안정 커밋

`C:\Git\OpenVisionLab` main 기준 최근 안정 커밋:

- `b0da050 Allow product sample NG review smoke`
- `b011ee2 Expose pair review hint in sample workflow`
- `667e454 Update OpenVisionLab handoff status`
- `e98a0b2 Clarify NG review fix guidance`
- `26f95f1 Document OpenVisionLab self evaluation`

## 2026-07-03 진행 완료

- Product sample catalog/native runner gate 안정화
  - `RunVisionSampleCatalog.ps1` evidence: `artifacts\self_evaluation_product_catalog_20260703_1750\sample_catalog_summary.json`
  - 결과: `GateStatus=OK`, `RunnableRows=168`, `RequiredRows=84`, `ExpectedFailureRows=84`, `OKRows=168`, `NGRows=0`
  - 품질 audit: `ProductSampleQualityAudit=PASS | PairRecords=84 OK=84 Review=0 Critical=0`
- 자체 평가 문서 추가
  - `docs\OPENVISIONLAB_SELF_EVALUATION_20260703.md`
  - 타사 대비 결론: 제품 목표 기준 완성도 `4.0/5`, 산업용 통합 플랫폼 기준 `2.0/5`
  - 장점은 PropertyGrid tool, layer route 투명성, Preview/Pipeline 분리, sample-backed review입니다.
- MainView/Product sample 흐름 개선
  - 샘플을 연 직후 하단 workflow strip에 `Pipeline Review에서 NG/OK 기준 열기` 단서를 노출합니다.
  - 제품군 표기는 `Secondary Battery`, `Display`, `Semiconductor`처럼 짧게 줄여 중요 문구가 말줄임표 뒤로 밀리지 않게 했습니다.
  - 원본 커밋: `b011ee2`
- Pipeline Review operator guide 개선
  - 최종 OK 단계는 더 이상 "다음 Step"으로 오해시키지 않고 출력/지표/Good-Bad 쌍 비교 후 Pipeline 승인으로 안내합니다.
  - NG 단계는 `우선 확인:` 힌트를 Tool 유형별로 표시합니다.
  - 원본 커밋: `95ed902`, `e98a0b2`
- Product sample NG review smoke 수정
  - `wpf_shell_host_workspace_product_sample_review_ng`는 Product catalog 샘플을 쓰므로, Public 전용 SourceKind assertion을 재사용하지 않도록 수정했습니다.
  - 원본 커밋: `b0da050`
- public-safe smoke 정리
  - legacy root sample 의존 smoke는 public catalog 샘플로 대체되었습니다.
  - 원본 커밋: `092e8f5`
- Tool View controller 분리 상태 확인
  - Line/Arithmetic은 공통 controller를 사용합니다.
  - Dev에서 `wpf_layer_selection_arithmetic_tool`, `wpf_shell_host_line_tool`, `wpf_shell_host_line_presets` 통과 확인했습니다.

## 주요 검증 증거

- Dev build:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` 통과
- Original build:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` 통과
- Readiness:
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` 통과
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"` 통과
- Public samples:
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` 통과
- Product sample focus after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\sample_workflow_pair_hint_after2_20260703_1821\wpf_shell_host_workspace_sample_product_focus_open.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\original_sample_workflow_pair_hint_after_20260703_1825\wpf_shell_host_workspace_sample_product_focus_open.png`
- Product sample NG review after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\operator_review_pair_flow_after_fix_20260703_1833\wpf_shell_host_workspace_product_sample_review_ng.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\original_product_sample_review_ng_after_fix_20260703_1836\wpf_shell_host_workspace_product_sample_review_ng.png`
- Pair coverage/audit:
  - `wpf_shell_host_workspace_sample_pair_coverage` 통과
  - `wpf_shell_host_workspace_sample_bad_reference_audit` 통과

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
   - Product sample Good -> Review -> NG 기준 열기 -> Run Review 흐름을 current EXE 캡처로 반복 확인합니다.
   - NG 단계에서 failed metric, expected/actual, suggested action이 한 화면에서 충분히 읽히는지 봅니다.
2. MainView/Product sample review 실제 사용자 흐름 재점검
   - Sample Picker -> Open Sample -> Pipeline Review -> Good/Bad counterpart 비교까지 끊김이 없는지 확인합니다.
3. Tool View code-behind 추가 축소
   - Line/Arithmetic 이후 남은 반복 wiring을 찾되, PropertyGrid 구조를 흔드는 추상화는 피합니다.
4. Product sample catalog 보강 여부 판단
   - 현재 84 pair audit은 PASS입니다. 새 샘플 추가보다 대표 샘플 설명/비교 affordance 강화가 우선입니다.

## 주의

- UI/UX 수정 시 현재 EXE/current build 기준 before/after 캡처를 새로 남깁니다.
- `PipelineViewerScreenshotSmoke`의 여러 WPF target을 한 프로세스에서 suite로 실행하면 첫 target 이후 멈출 수 있습니다. Product/sample review 검증은 단일 target을 순차 실행하세요.
- UI smoke를 병렬 실행하면 `OpenCvSharpExtern.dll` 잠금 warning이 날 수 있습니다. WPF smoke는 병렬 실행하지 않는 편이 안전합니다.
- 원본 repo에 Dev를 대량 덮어쓰지 않습니다.
- GitHub Desktop stash가 보이면 임의로 Restore하지 않습니다.
- 외부 SDK sample asset이나 `dll\Library-Noah\OpenCvSharpExtern.dll`은 public 경로에 다시 넣지 않습니다.
