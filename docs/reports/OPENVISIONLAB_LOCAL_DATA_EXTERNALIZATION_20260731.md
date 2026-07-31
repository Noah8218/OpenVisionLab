# OpenVisionLab Local Data Externalization

Updated: 2026-07-31 KST

## Status

Complete

## Scope

`C:\Git\OpenVisionLab_Dev`의 Git 비추적 로컬 실행·빌드·테스트 데이터만
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev`로 옮기고, 기존 C: 경로에는
NTFS Junction을 유지했다. 제품 소스, Git 추적 검증 도구, 공개 샘플,
문서 자산, DLL 및 런타임 동작은 변경하지 않았다.

기존 `artifacts` Junction은 이미
`D:\OpenVisionLab_Data\Dev\artifacts`를 가리키므로 다시 이동하지 않았다.

## Externalized Data

- 루트 로컬 작업 데이터: `.codex`, `.codex-temp`, `.vs`, `tmp`
- 실행·배포 출력: `bin`, `dist`
- 빌드 중간 출력: 루트 및 각 `src`/`tools` 프로젝트의 `bin`/`obj`
- 테스트 이미지: Git ignored `Sample`
- 생성 샘플: `docs/samples/generated`
- 라이브러리 성능 측정 출력: `src/Libraries/*/artifacts`

최종 전수 결과:

- 외부화/Junction 대상: 68개 디렉터리
- 파일: 44,085개
- 바이트: 8,612,491,468 bytes (약 8.02 GiB)
- 최종 관측 C: 여유 공간: 약 144GB -> 163.67GB
- 최종 관측 D: 여유 공간: 약 604.2GB -> 584.89GB

드라이브 여유 공간 변화에는 같은 시간대의 빌드 서버 종료와 빌드/캐시
변화가 포함된다. 두 이동 단계의 초기 검증 합계는 19,391,395,096 bytes
(약 18.06 GiB)다.

## Preserved In Repository

- `tools/`의 Git 추적 테스트·검증 소스
- `docs/samples/`의 Git 추적 공개 카탈로그/샘플
- `docs/assets/` 문서 자산
- `dll/`과 모든 Git 추적 소스/프로젝트 파일
- `.git/` 저장소 메타데이터

이 파일들은 clean clone, GitHub Actions, source build와 공개 샘플 계약에
필요하므로 machine-local Junction 대상으로 취급하지 않는다.

## Reusable Command

```powershell
# 현재 상태 확인 및 누락된 로컬 생성물 외부화
powershell -NoProfile -ExecutionPolicy Bypass -File tools\Move-OpenVisionLabLocalData.ps1

# 변경 예상 경로만 확인
powershell -NoProfile -ExecutionPolicy Bypass -File tools\Move-OpenVisionLabLocalData.ps1 -WhatIf

# 필요할 때 C: 저장소로 복원
powershell -NoProfile -ExecutionPolicy Bypass -File tools\Move-OpenVisionLabLocalData.ps1 -RestoreToRepo
```

스크립트는 Git 추적 파일이 포함된 경로를 거부하고, 저장소/외부 루트
경계를 검사하며, 긴 Windows 경로를 지원한다. 부분 이동이 발견되면 동일
상대 경로 파일의 SHA-256을 확인한 뒤에만 병합하고 원본을 제거한다.

## Migration Findings

- 최초 이동에서 Hidden `.vs` 내용은 D:로 복사됐지만 빈 C: 디렉터리 제거가
  거부됐다. 원본 파일 0개/대상 33개를 확인한 뒤 빈 원본만 제거하고 Junction을
  만들었다.
- 명령 실행 제한으로 한 차례 프로세스 출력 연결이 종료됐지만 실제 이동
  프로세스는 계속 실행됐다. 종료를 확인한 뒤 전수 검증했다.
- 두 Smoke 출력 디렉터리에 긴 Windows 경로가 남아 일반 `Move-Item`이
  중단됐다. extended path + SHA-256 복구 경로로 남은 파일을 검증·병합했고,
  최종 68개 대상이 모두 Junction 상태가 됐다.

## Production Verification Clone Deletion Audit

화면에 제시된 C: Git 루트의 생산 검증 clone 8개도 확인했다.

- 총 25,455개 파일, 약 10.037GB다.
- 모두 동일 원격 저장소에서 만든 P273/P274 검증용 clone이다.
- 7개 worktree는 clean이다.
- `OpenVisionLab_Production_Verification_Final_20260730`만
  `tools/TestReleaseDistribution.ps1` 한 파일이 수정 상태지만, 조건식을
  한 줄로 합친 비의미적 서식 차이이며 현재 Dev에는 더 최신 버전이 있다.
- 해당 폴더를 사용하는 실행 중 프로세스는 없다.
- 아래 4개는 현재 보고서/핸드오프가 ZIP 또는 summary 증거 경로로 직접
  참조한다.
  - `OpenVisionLab_Production_DataRoot_RC_20260730`
  - `OpenVisionLab_Production_DataRoot_Repro_20260730`
  - `OpenVisionLab_Production_RC_Final_20260730`
  - `OpenVisionLab_Production_Repro_Final_20260730`
- 나머지 4개는 현재 Dev 문서에서 직접 참조하지 않는다.

판정 후 실행: 사용자가 승인한 안전안에 따라 8개 모두
`D:\OpenVisionLab-TestData\ProductionVerification_20260730`으로 이동하고
네 개 P273/P274 문서 참조를 갱신했다. 긴 경로 지원 복사와 파일별 SHA-256
일치 후에만 C: 원본을 제거했다.

```text
VerificationCloneMove=PASS Mode=Move Candidates=8 Moved=8 Existing=0 Files=25455 Bytes=10778903628
C:\Git source folders remaining: 0
Manifest: D:\OpenVisionLab-TestData\ProductionVerification_20260730\migration_manifest.json
Error log: empty
```

재실행/부분 이동 복구 스크립트:
`tools\Move-OpenVisionLabVerificationClones.ps1`.

## Verification

```text
Initial migration: Candidates=68 Files=44085 Bytes=8612491468
Post-build idempotent replay: LocalDataMove=PASS Mode=Externalize Candidates=68 Moved=0 Existing=68 Files=44090 Bytes=8612495039
Restore dry-run: LocalDataMove=PASS Mode=RestoreWhatIf Candidates=68 Moved=68 Existing=0 Files=44090 Bytes=8612495039
Verification clone move: VerificationCloneMove=PASS Mode=Move Candidates=8 Moved=8 Existing=0 Files=25455 Bytes=10778903628
Verification clone idempotent WhatIf: Candidates=8 Moved=0 Existing=8 Files=25455 Bytes=10778903628
Old C: production-clone evidence path references: 0
Clone manifest: 8 clones / 25455 files / 10778903628 bytes / 8 Moved states
Debug solution build: PASS, 0 warnings, 0 errors
OpenVisionLab readiness: PASS, 13/13 contracts
Built EXE through C: Junction: C:/D: physical paths both exist, 219648 bytes
Documentation index: PASS, IndexedPaths=38 Routes=10 RootRedirects=99
PowerShell parser: PASS
Git diff check: PASS
```

## Boundary / Next Dependency

이 설정은 현재 Windows 개발 PC의 로컬 저장공간 배치다. D:가 연결되지 않으면
Junction 대상의 실행·빌드·테스트 데이터에 접근할 수 없다. Git clone 자체와
추적 소스는 C: 저장소에 남아 있으며, 다른 PC는 이 Junction 구성을 자동으로
상속하지 않는다.

## Closure Record

```text
Status: Complete
Scope: ignored local execution/build/test data externalized to D: with compatible C: junctions; eight production verification clones archived to D: without C: copies
Acceptance criteria: tracked sources preserved -> pass; 68 local targets externalized -> pass; 8 verification clones moved -> pass; file/byte/SHA-256 verification -> pass; C: clone sources absent -> pass; build/readiness/index checks -> pass; explicit restore dry-run -> pass
Verification: Debug build 0/0; readiness 13/13; local idempotent 68/68; restore WhatIf 68/68; clone idempotent 8/8; old path references 0; document index and diff check pass
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev; D:\OpenVisionLab-TestData\ProductionVerification_20260730\migration_manifest.json; this report; tools\Move-OpenVisionLabLocalData.ps1; tools\Move-OpenVisionLabVerificationClones.ps1
Boundary / next dependency: D: must remain mounted; tracked repository payload remains on C:; production verification clones are archived under D:\OpenVisionLab-TestData\ProductionVerification_20260730
```
