# OpenVisionLab OVL-06 행동 회귀·프로세스 복구 baseline

작성일: 2026-09-07 KST  
대상: `C:\Git\2D\Dev`  
상태: **Complete (OVL-06a slice)**

## 이번 slice의 범위

GPT Pro 분석의 다음 리팩토링 후보를 현재 Dev 상태와 대조해 다음 순서로
고정했다.

1. **OVL-06a 저장 lifecycle 프로세스 경계 회귀** — 이번에 구현.
2. **OVL-07 Validation 소유권 추출** — 기존 facade·presenter·execution owner를
   다시 조사한 뒤 한 상태 변경 owner가 입증될 때 진행.
3. **OVL-08 PropertyGrid 업무 정책 분리** — 기존 Abstractions/options로
   표현 가능한지 확인한 뒤 최소 경계만 진행.
4. **OVL-09 Learn 주제 수명 분리** — Threshold 한 주제의 timer·활성/닫힘
   경계를 먼저 검증.

OVL-06b 전수 정량 감사, 전체 WPF theme/Wide/Compact/DPI 행렬, 비협조 native
worker 강제 종료, 하드웨어·현장 자격 검증은 이번 slice에 포함하지 않는다.

## 변경 내용

`tools/VisionRecipeRunnerSmoke/Program.cs`에 다음 두 실행 경계를 추가했다.

- `--pipeline-persistence-process-recovery-contract [evidenceDirectory]`
  - 부모가 D: 테스트 데이터 루트와 고유 Recipe를 만들고, rename 6단계와
    delete 5단계의 11개 사례를 순회한다.
  - 자식 프로세스는 기존 `VisionPipelineStorage` failure-injection hook을
    사용해 journal 단계 직후 marker를 남기고 대기한다.
  - 부모는 marker를 확인한 뒤 자식을 `Kill(entireProcessTree: true)`하고,
    같은 데이터 루트에서 storage를 재오픈해 prior-state rollback 또는
    completed-state adoption을 검사한다.
  - 재오픈 뒤 journal, lifecycle backup, atomic temporary file이 남지 않는지
    검사하고 복구 시간과 프로세스 수치를 기록한다.
- `--pipeline-persistence-process-recovery-probe`는 위 부모 검사의 자식 전용
  경계다. apphost 실행 파일과 `dotnet <assembly>.dll` 실행을 모두 지원하도록
  자기 자신 런처를 구성했다.

제품 저장 포맷, journal schema, 복구 정책, Preview/Run 경로는 변경하지 않았다.
새 harness는 이미 존재하는 same-process 계약을 프로세스 경계까지 확장한다.

## 구조·호출 경로 증거

기존 경로:

```text
VisionRecipeRunnerSmoke → VisionPipelineStorage lifecycle operation
                         → same-process recovery check
```

새 경로:

```text
parent runner → child probe → durable journal stage → parent process kill
             → parent storage reopen → recovery state / artifact cleanup check
```

복구 판단과 저장 책임은 기존 `VisionPipelineStorage`에 남기고, 프로세스 종료와
재오픈을 재현하는 책임만 smoke runner의 부모/자식 경계로 분리했다. 따라서
검증 경로가 실제로 프로세스 경계를 통과하며, 생산 코드의 ownership을 복제하는
새 service나 partial은 추가하지 않았다.

## 합격 기준과 결과

| 기준 | 결과 | 근거 |
| --- | --- | --- |
| rename 6 journal 단계의 프로세스 종료 후 복구 | 통과 | `Cases=11`, rename 6건 모두 rollback 5건 또는 completed 1건 |
| delete 5 적용 단계의 프로세스 종료 후 복구 | 통과 | delete 5건 모두 rollback 4건 또는 completed 1건 |
| child 종료가 실제 kill 경계를 통과 | 통과 | `ChildLaunches=11`, `ChildKills=11`, 각 case `childExit=-1` |
| 복구 결과가 prior 또는 completed 중 하나로 일관됨 | 통과 | 각 case active pointer·XML 이름·prior SHA-256 확인 |
| journal/backup/tmp 정리 | 통과 | 11건 모두 재오픈 후 잔여 파일 0 |
| 기존 same-process 계약 보존 | 통과 | PL-0009 recovery contract PASS |
| storage path 경계 보존 | 통과 | PL-0007 storage path contract PASS |

## 검증

모든 출력과 테스트 데이터는 D:에 남겼다.

- [VisionRecipeRunnerSmoke Release build — 0 warnings / 0 errors](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl06-process-recovery-20260907/build-runner-release-final-final.log)
- [apphost process recovery contract — 11/11](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl06-process-recovery-20260907/run-final/pipeline_persistence_process_recovery_contract.txt)
- [dotnet DLL process recovery contract — 11/11](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl06-process-recovery-20260907/run-dotnet-r2/pipeline_persistence_process_recovery_contract.txt)
- [same-process lifecycle recovery contract](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl06-process-recovery-20260907/run-final/same-process/pipeline_persistence_recovery_contract.txt)
- [storage path contract](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl06-process-recovery-20260907/run-final/storage-path/recipe_storage_path_contract.txt)
- [documentation index validation](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl06-process-recovery-20260907/documentation-index-final.log)
- [git diff --check](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl06-process-recovery-20260907/git-diff-check-final.log)

최종 apphost 실행 수치는 `AverageRecoveryMs=13.182`, `MaximumRecoveryMs=37`,
`Recovered=11/11`이다. DLL-host 실행도 `Recovered=11/11`로 통과했다.

## 경계

- 이번 결과는 journal이 durable stage까지 기록된 뒤 프로세스가 중단되는
  rename/delete 경계만 증명한다. 전원 차단, 파일 시스템·디스크 고장,
  비협조 native worker의 강제 종료는 증명하지 않는다.
- 전체 WPF theme/state/Wide/Compact/100–200% DPI/pressed/focus/popup 행렬과
  장시간·peak-memory 정량 감사는 실행하지 않았다.
- 변경은 Dev에만 남겼으며 `C:\Git\2D\Original`은 수정하지 않았다. commit,
  push, release, deployment는 수행하지 않았다.

## 완료 기록

Status: Complete  
Scope: OVL-06a 프로세스 종료·재오픈 저장 lifecycle regression harness와 문서 색인  
Acceptance criteria: 11/11 process recovery, 기존 두 계약 PASS, build 0/0, D: evidence 저장  
Verification: Release build, apphost 실행, `dotnet` DLL 실행, same-process recovery, storage path, diff check  
Evidence: 위 D: evidence 경로와 이 보고서  
Boundary / next dependency: OVL-06b 전수 정량 감사와 OVL-07 Validation 소유권 추출은 별도 승인·검증 slice
