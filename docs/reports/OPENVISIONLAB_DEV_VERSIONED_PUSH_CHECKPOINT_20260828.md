# OpenVisionLab Dev 버전별 커밋·푸시 체크포인트

작성일: 2026-08-28 KST
상태: **Complete — Dev branch checkpoint**

## 대상과 버전 정책

- 저장소: `C:\Git\OpenVisionLab_Dev`
- 브랜치: `codex/public-sample-ux-docs`
- 원격: `https://github.com/Noah8218/OpenVisionLab_Dev.git`
- 원격 ref: `refs/heads/codex/public-sample-ux-docs`
- canonical application version: `2.1.0`
- 이 작업은 release/tag/deployment가 아닌 개발 체크포인트 푸시다. 따라서
  정상적인 개발 커밋마다 제품 버전을 올리지 않았다.
- 버전 식별자는 실제 변경 책임에 맞춰 사용했다.
  - `0.2.0-alpha.1`: 2D Image Buffer 교환 계약
  - `3.0.1-dev.20260826.candidate.2`: Vision SDK 객체 후보 계약
  - `2.1.0`: 애플리케이션, UI, ImageCanvas, locator, docking, teaching

## 원격에 반영된 순서

| 순서 | 버전 식별자 | 커밋 | 책임 범위 |
| ---: | --- | --- | --- |
| 1 | `2.1.0` | `54f219acf0006c565688780bcc9175b6d3376c2f` | Release precheck public-catalog alignment; 작업 시작 시 이미 앞선 Dev 체크포인트로 확인됨 |
| 2 | `0.2.0-alpha.1` | `37c8b113399cbc966ab6013517732e545a6d2689` | 2D image-buffer exchange 계약, 패키지/manifest, cross-repository smoke |
| 3 | `0.2.0-alpha.1` | `ee54d101155b873a0388ff0bb5f4762ef22ce426` | 2D exchange smoke 진입점과 사용법 보정 |
| 4 | `3.0.1-dev.20260826.candidate.2` | `f5f4c4ca798d062df72911584bae95f81b05e366` | SDK 객체 후보 metadata 및 one-pass 소비 구현 |
| 5 | `3.0.1-dev.20260826.candidate.2` | `617a60320f3635da0f9e0d860dc545304770b538` | Blob/Contour mask·Multi-ROI·Run History·public sample parity smoke |
| 6 | `2.1.0` | `352c6301dcf123aa161501fb5c0d24de32d834ba` | 독립 ImageCanvas external consumer와 cross-modal projection 표시 |
| 7 | `2.1.0` | `aebd5d660046f0896a8f874ea207cf4013c14aae` | evidence-constrained locator-relative Blob pilot |
| 8 | `2.1.0` | `29cd6f9f821bc22b26c5c6327289b26f3c11c558` | Pipeline Review image-first/compact 및 N-image UX 안정화 |
| 9 | `2.1.0` | `0404c28fd6a9b7ec795a7475377a6613e863c2a5` | docking close affordance와 layer-preservation smoke |
| 10 | `2.1.0` | `67b43e0053353937ab150afd5bdaf5feaadec036` | rule-based teaching 회전 overlay, Die Pad 증거 보고서/검증 스크립트 |

각 단계는 같은 remote branch에 순차적으로 `git push origin
codex/public-sample-ux-docs`로 반영되었고, 마지막 원격 head는 순서 10의
커밋과 일치한다.

## 검증과 증거

- 2D exchange consumer build: `dotnet build
  tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Release
  --no-restore --nologo /nodeReuse:false` — 경고 0, 오류 0.
- SDK parity smoke: `PL-0010 object-candidate parity contract passed.`
  Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\commit-checkpoints\sdk-candidate-parity-3.0.1-dev.20260826-candidate.2-20260828-r2`.
- Rule-based teaching batch: `BatchRows=122`, `BatchCompleted=122`,
  `BatchPipelinePasses=49`, `BatchMissingImages=0`.
  Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-rotated-overlay-20260828-rerun05`.
- Teaching PowerShell scripts 5개는 Windows PowerShell parser에서 모두
  `PASS`를 반환했다.
- Docking embedded EXE smoke와 close-path smoke는 동적 monitor 선택 및
  close/redock/last-document 경로를 포함해 `Result: PASS`로 종료했다.
  Evidence roots:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\commit-checkpoints\docking-2.1.0-20260828-embedded`
  및
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\commit-checkpoints\docking-close-2.1.0-20260828`.
- UI 관련 검증은 현재-source WPF smoke와 필요한 docking EXE smoke 범위만
  수행했다. 성능 테스트는 사용자 요청대로 수행하지 않았다.

## 경계와 잔여 상태

- 원본 `C:\Git\OpenVisionLab`의 branch/tag/release/deployment와
  `C:\Git\OpenVisionLab-Machine-Studio`는 이 체크포인트에서 변경하지 않았다.
- 제품 version source는 계속 `2.1.0`이다. SDK candidate의
  `3.0.1-dev.20260826.candidate.2`와 integration contract의
  `0.2.0-alpha.1`은 해당 배치의 dependency/version identity이지 제품
  release tag가 아니다.
- teaching evidence는 문서에 기록된 대로 `qualification=false` 및
  operator review 경계를 유지한다. 이 커밋은 검사 알고리즘의 현장 승인이나
  production qualification을 의미하지 않는다.
- `.proofline/drafts/`는 release-draft 작업공간으로 의도적으로 untracked
  상태를 유지했으며 소스 체크포인트에 넣지 않았다. 이 디렉터리를 제외한
  tracked 변경과 관련 보고서는 이 문서 커밋에서 정리한다.
- GitHub Actions는 `codex/**` push 트리거에 의해 실행될 수 있으나, 이
  체크포인트에서 Release publication 또는 deployment를 실행하지 않았다.
