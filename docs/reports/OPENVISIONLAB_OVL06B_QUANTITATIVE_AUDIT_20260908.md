# OpenVisionLab OVL-06b 정량 감사 instrumentation — 2026-09-08

## Completion record

Status: Complete

Scope: `src`·`tools`의 C#·XAML 규모, partial/type 선언, 큰 파일, ViewModel의
직접 UI/파일 시스템 신호, Shell Run History 저장소 호출, 프로젝트 참조
순환을 한 번에 계측하고 D: 증거 파일로 남기는 읽기 전용 감사 도구를 추가했다.

Acceptance criteria:

- 반복 실행 가능한 단일 PowerShell 진입점이 존재한다 —
  `tools/RefactorAudit/Invoke-RefactorAudit.ps1`.
- `-Verify`가 C# 소스 존재, 프로젝트 순환 0, Shell Run History 직접 호출 0,
  알려진 `RoiImageCanvasViewModel` 결합 신호 보고를 확인한다.
- JSON·CSV·text 결과를 D: 출력 디렉터리에 생성하고, D: 이외 경로는
  `-AllowNonDOutput` 없이는 실패한다.
- 기존 Recipe/XML, Preview/Run, Layer, ImageSpace, SDK 또는 완료 owner의
  호출 경로를 변경하지 않는다.

## 현재 owner와 새 경계

- 현재 owner: 이전에는 수동 `rg` 명령과 감사용 D: 메모리가 각각 지표를
  계산했다.
- 새 owner: `Invoke-RefactorAudit.ps1`가 저장소 읽기, 지표 계산, 회귀 판정,
  증거 출력의 책임을 가진다.
- 호출 경로: `PowerShell script → src/tools source scan → JSON/CSV/text
  evidence`.
- 상태 소유권: 감사 실행 중에는 저장소를 읽기만 하며 제품 상태나 Recipe를
  변경하지 않는다. 출력 상태는 지정한 D: evidence root가 소유한다.

이 도구는 구조 변경을 대신하지 않는다. 예를 들어 `RoiImageCanvasViewModel`
의 UI/IO 신호는 숨기지 않고 OVL-11의 다음 독립 slice 대상으로 계속 보고한다.
Path helper도 기존 기준선의 직접 IO 파일 수(8개)와 일치하도록 같은 경계로
계수한다.

## 실행과 실제 결과

실행 명령:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File `
  .\tools\RefactorAudit\Invoke-RefactorAudit.ps1 `
  -RepositoryRoot C:\Git\2D\Dev `
  -OutputDirectory D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl06b-quantitative-audit-20260908 `
  -Verify
```

계약 출력은 다음과 같았다.

```text
REFACTOR_AUDIT=PASS|CSharpFiles=763|XamlFiles=58|PartialDeclarations=106|ViewModelUiIoFiles=2|ProjectCycles=0|ShellStorageCalls=0
```

세부 결과는 C# 763개/268,270줄/12,219,039 bytes, XAML 58개/24,624줄,
partial 106개, 1,000줄 이상 파일 28개, 2,000줄 이상 12개, 3,000줄 이상
8개, Shell 10개/10,367줄, ViewModel UI·대화상자 2개, 직접 IO 8개,
프로젝트 27개/참조 34개/순환 0개였다.

## 검증 증거

- `refactor-audit-contract.log`: 위 `-Verify` 실행 PASS.
- `invalid-output-check.log`: C: 출력 경로가 exit code 1로 거부되고 경로가
  생성되지 않음.
- `invalid-repo-check.log`: 존재하지 않는 저장소 경로가 exit code 1로
  거부되고 출력 경로가 생성되지 않음.
- `repeatability-check.log`, `repeatability-metrics.json`: 같은 소스에 대한
  두 번의 `-Verify` 실행에서 모든 구조 지표가 일치함.
- `baseline-comparison.txt`: 기존 수동 기준선의 C#·XAML·partial·Shell·cycle
  핵심 수치가 새 도구 결과와 일치함.
- `source-survey.json`, `large-files.csv`, `viewmodel-ui-io.csv`,
  `shell-storage-calls.csv`, `project-cycles.csv`,
  `project-cycle-check.txt`, `source-survey-summary.txt`: 기계 판독 및
  사람이 읽는 결과.
- `refactor-proof-plan.md`, `refactor-proof-report.md`, `structure-proof.txt`:
  현재/의도 owner, 호출 경로, 실패 조건과 보존 계약에 대한 구조 증명.
- `final-state.txt`: branch/HEAD, dirty status·staged count, 15분 heartbeat와
  외부 mutation 금지 상태.

문서·PowerShell 변경만 포함한 slice이므로 앱/Runner 빌드와 WPF runtime
qualification은 실행하지 않았다. 이 보고서는 그 미실행 범위를 완료 증거로
확장하지 않는다.

Boundary / next dependency: 다음 독립 slice는 `RoiImageCanvasViewModel`의
UI·IO 경계(OVL-11)다. 해당 변경 전까지 ViewModel 결합 신호는 의도적으로
남아 있으며, 감사 도구가 이를 회귀 없이 계속 보고해야 한다.
