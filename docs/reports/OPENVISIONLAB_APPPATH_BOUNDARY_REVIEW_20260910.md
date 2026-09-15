# OpenVisionLab AppPathService 런타임 경로 경계 검토

검토일: 2026-09-10 KST
대상 저장소: `C:\Git\2D\Dev`
검토 시작 branch: `codex/public-sample-ux-docs`
검토 시작 SHA: `1528d3b869ce67f439ac28fc2b8565cde58c28b2`
Target Framework: `net8.0-windows7.0`
제품 버전: `2.2.0-dev.2`

## 범위와 결론

이번 slice의 범위는 이미 존재하는 `AppPathService`의 런타임 데이터 경로
조합 경계다. 새로운 서비스, 인터페이스, Provider, Manager를 추가하지 않고
기존 static 소유자 안에서 경로 조합의 공통 진입점을 보정했다.

상태: **Complete (bounded runtime path containment correction)**

`Combine`, `CombineInstallation`, `EnsureDirectory`,
`GetCaptureFilePath`, `GetTestConfigPath`가 루트 밖으로 나가는 입력을 같은
검사 경로에서 거부한다. 정상적인 하위 경로 결과와 기존 런타임 데이터 루트
초기화·마이그레이션 동작은 유지했다.

WPF metadata parameter의 portable 경계는 소스 기준으로 재확인했지만, 현재
host 분리 요구나 재현 결함이 없어 추가 추출은 하지 않았다. 실제 WPF 실행,
다중 DPI, 장시간 native resource 운전은 이 문서의 완료 범위가 아니다.

## 기존 구조와 재현

### 현재 소유자

- `src/OpenVisionLab/Common/Runtime/AppPathService.cs`가 `DataRootDirectory`,
  `InstallationRootDirectory`, `CONFIG`, `CAPTURE`, `RECIPE`, `TEST`, `Log`
  경로와 초기화/마이그레이션 상태를 process-wide `Lazy<RuntimeDataPathState>`로
  소유한다.
- `src/OpenVisionLab/Program.cs`의 `Main`이 첫 진입점에서
  `AppPathService.Initialize()`를 호출한다.
- `src/OpenVisionLab/App/Bootstrap/OpenVisionLabApplication.cs`는 같은
  소유자의 데이터 루트와 마이그레이션 알림을 읽어 애플리케이션 초기화를
  계속한다.
- Recipe 저장은 `src/OpenVisionLab/Core/Recipe/RecipeWorkspaceService.cs`가
  별도의 세그먼트 검증과 저장 경계를 소유한다. 이번 변경이 그 책임을
  중복하지 않는다.

### 이전 호출 경로

기존 `Combine`/`CombineInstallation`은 `Path.Combine` 결과를 그대로 반환했다.
따라서 `..` 세그먼트 또는 외부 rooted 경로가 전달되면 의도한 루트 밖의
경로가 만들어졌다. D: 격리 harness의 변경 전 실행 결과는 다음과 같다.

```text
DataRoot=...\apppath-boundary-review-20260910\data
Traversal=...\apppath-boundary-review-20260910\escaped
Rooted=...\apppath-boundary-review-20260910\rooted
TraversalUnderRoot=False
RootedUnderRoot=False
```

원본 harness와 출력은
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\apppath-boundary-review-20260910\baseline-harness-output.txt`
에 보관했다.

## 변경 후 구조와 호출 경로

### 의도한 소유자

기존 `AppPathService`를 계속 런타임 경로 소유자로 유지하고,
`CombineUnderRoot`를 private 공통 경계로 두었다. 입력 세그먼트에 `.` 또는
`..`가 있으면 거부하고, `Path.GetFullPath` 후
`IsSameOrChildPath(root, combined)`를 다시 확인한다. 루트 밖이면
`InvalidOperationException`을 호출자에게 전달한다.

### 실제 호출 경로

```text
Program.Main
  -> AppPathService.Initialize()
  -> RuntimeState.Value / CreateRuntimeState()
  -> AppPathService.Combine*()
  -> CombineUnderRoot()
  -> IsSameOrChildPath()
```

`EnsureDirectory`는 기존처럼 `Combine`을 호출하므로 디렉터리 생성도 같은
경계 검사를 사용한다. Capture/Test config convenience API도 직접
`Path.Combine`하지 않고 같은 helper를 사용한다.

### 상태·수명·공개 계약

- mutable state: `Lazy<RuntimeDataPathState>`와 process environment 변수이며,
  소유자는 `AppPathService`다.
- lifetime/release owner: process 종료까지 유지되는 static runtime state다.
  이번 변경으로 `Mat`, `Bitmap`, 파일 스트림 등의 수명은 바뀌지 않는다.
- public contract: 기존 public 메서드와 반환 형식, 정상 하위 경로, 데이터 루트
  환경 변수, 설치 루트 fallback 계약을 유지한다. 잘못된 traversal/rooted
  입력은 이전에 예외 없이 허용되던 경계 입력이며 이제 명시적으로 실패한다.
- preserved dependency fallback: `ResolveExistingDataOrInstallationPath`는
  package-relative sample의 기존 `..` fallback 의미를 보존해야 하므로 이번
  수정에서 변경하지 않았다. 이 API는 “기존 파일을 data/install에서 찾고
  없으면 data 경로를 반환”하는 별도 의도다.
- legacy public types: `CPropertyImageView`, `CPropertyLot`, `CPropertySocket`은
  현재 저장소 내부 caller가 없는 legacy public 선언이다. 외부 binary/reflection
  계약 증거 없이 삭제하거나 새 저장 owner로 이동하지 않았다.

## 파일별 변경

| 파일 | 변경 목적 | 기존 동작 보존 |
| --- | --- | --- |
| `src/OpenVisionLab/Common/Runtime/AppPathService.cs` | 기존 경로 조합 API를 private 루트 경계 helper로 통합하고 escape를 거부 | 데이터 루트 초기화/마이그레이션, 정상 경로, 설치 fallback, 공개 API 이름 유지 |
| `tools/VisionRecipeRunnerSmoke/AppPathBoundaryContract.cs` | 8개 정상/실패 경계 검사를 D: 격리 runner 계약으로 추가 | 제품 실행 경로에는 영향 없음 |
| `tools/VisionRecipeRunnerSmoke/Program.cs` | `--app-path-boundary-contract` 명령을 기존 smoke entrypoint에 연결 | 기존 smoke 명령과 기본 실행 흐름 유지 |
| `docs/reports/OPENVISIONLAB_APPPATH_BOUNDARY_REVIEW_20260910.md` | owner, call path, 보존 계약, 검증과 미검증 경계를 기록 | 문서 탐색용 변경 |
| `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md` | 현재 우선순위와 완료 증거 갱신 | 제품 동작 변경 없음 |
| `docs/admin/OPENVISIONLAB_DOCUMENTATION_MAP.md` | 상세 문서 registry에 이 보고서 등록 | 제품 동작 변경 없음 |
| `docs/LLM_DOCUMENT_INDEX.json` | status/folder route에서 보고서를 찾을 수 있게 등록 | 문서 경로 외 변경 없음 |

## 집중 검증

모든 test output과 로그는 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\apppath-boundary-contract-20260910` 아래에 저장했다.

| 검사 | 결과 | 증거 |
| --- | --- | --- |
| 변경 전 격리 harness | 루트 밖 traversal/rooted 결과 재현 | `apppath-boundary-review-20260910/baseline-harness-output.txt` |
| VisionRecipeRunnerSmoke Debug build | PASS, warning 0/error 0 | `build-debug.log` |
| `--app-path-boundary-contract` Debug | PASS, `Passed=8 Failed=0` | `debug/app-path-boundary-contract.txt`, `debug/stdout.log` |
| VisionRecipeRunnerSmoke Release build | PASS, warning 0/error 0 | `build-release.log` |
| `--app-path-boundary-contract` Release | PASS, `Passed=8 Failed=0` | `release/app-path-boundary-contract.txt`, `release/stdout.log` |
| Solution Debug build | PASS, warning 0/error 0 | `build-solution-debug.log` |
| Solution Release build | PASS, warning 0/error 0 | `build-solution-release.log` |
| OpenVisionReadinessCheck Debug | PASS, 13/13 checks | `readiness-debug.log` |
| OpenVisionReadinessCheck Release | PASS, 13/13 checks | `readiness-release.log` |
| Refactor audit `-Verify` | PASS; C# 845, XAML 60, partial 110, project cycle 0, shell storage 0 | `refactor-audit/audit.log` |
| AppPath caller / WPF bridge source search | 기존 runtime caller와 legacy public `Path.Combine` 선언을 확인하고 WPF metadata bridge의 AppPath/file I/O 결합이 없음을 확인 | `source-path-search.txt` |
| Documentation index / JSON | PASS, `IndexedPaths=278 Routes=15 RootRedirects=102` | `documentation-index.log` |

경계 계약의 8개 검사는 정상 runtime/install 경로 2개와 traversal, nested
traversal, external rooted, directory, capture title, test config name의 거부
6개로 구성된다.

## 미검증·후속 경계

- 실제 WPF 창, supported theme, Wide/Compact layout, 100/125/150/175/200%
  DPI, monitor placement, pointer/keyboard 상태는 실행하지 않았다.
- 실제 카메라/SDK/장시간 native resource 운전과 강제 종료 중 파일 쓰기는
  검증하지 않았다.
- `ResolveExistingDataOrInstallationPath`의 외부 absolute 허용은 package와
  sample 호환 계약이므로 유지했다. 이를 제거하려면 host/package 요구사항과
  기존 consumer 증거가 먼저 필요하다.
- `CPropertyImageView`/`CPropertyLot`/`CPropertySocket` legacy public API의
  외부 사용 여부는 저장소 밖 정보이므로 삭제하지 않았다.

이번 변경에 대한 UI 결론은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

## 개발자 탐색 순서

경로 문제를 처음 추적하는 개발자는 다음 순서로 읽는다.

1. `docs/README.md`의 Start Here와 이 보고서의 범위를 확인한다.
2. `src/OpenVisionLab/Program.cs`에서 `Main`과 `Initialize` 호출을 찾는다.
3. `src/OpenVisionLab/Common/Runtime/AppPathService.cs`에서 `RuntimeState`,
   `DataRootDirectory`, `CombineUnderRoot`, `ResolveExisting...`를 읽는다.
4. Recipe 파일 경로면 `src/OpenVisionLab/Core/Recipe/RecipeWorkspaceService.cs`로
   이동해 저장 세그먼트 검증과 호출자를 확인한다.
5. 경계 회귀는 `tools/VisionRecipeRunnerSmoke/Program.cs`의
   `--app-path-boundary-contract` dispatch와
   `AppPathBoundaryContract.cs`의 8개 검사를 실행한다.

현재 경계의 단일 runtime owner, 실제 call path, mutable-state write owner,
공개 binding/API 계약, 최소 reading order, 실행된 focused checks는 위에
명시했다. 별도 host 분리나 재사용 package가 요구되지 않는 한 이 owner를
다시 분리하지 않는다.
