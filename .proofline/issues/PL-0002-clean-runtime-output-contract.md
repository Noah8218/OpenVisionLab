---
{
  "id": "PL-0002",
  "type": "maintenance",
  "status": "resolved",
  "title": "Define a permanent Dev and release clean-runtime output contract",
  "discovered_while": "P132 direct-smoke Contour runtime investigation",
  "description": "P132 proved that a new empty runtime output runs the current Contour replay correctly, while the retained bin\\Debug directory contains legacy files and cannot be treated as deployment evidence. The user approved P133: timestamped artifacts output for Dev evidence, dist\\OpenVisionLab for a Release package, and retention of the existing bin\\Debug local recipe workspace.",
  "evidence": [
    {
      "kind": "P132 resolution boundary",
      "location": "tools\\BuildCleanRuntime.ps1",
      "note": "Creates a fresh artifacts runtime without overwriting an existing directory."
    },
    {
      "kind": "runtime contrast",
      "location": "artifacts\\p132_direct_smoke_contour_host_20260719",
      "note": "The fresh runtime passed Good and controlled NG; retained bin\\Debug terminated in FindContours."
    }
  ],
  "risk": "medium",
  "impact": "The approved script contract prevents stale-output ambiguity for its Dev and Release paths. Direct ad-hoc launches of retained bin\\Debug remain unsupported as current-runtime or release evidence.",
  "suggested_next_step": "Resolve LLM template-dependency portability from the approved dist\\OpenVisionLab package without changing the retained bin\\Debug workspace policy.",
  "completion_criteria": [
    "The approved Dev and release runtime roots are documented.",
    "A clean package/runtime build contains the required managed and native files and rejects stale-output ambiguity.",
    "Direct LLM XML replay, normal Recipe Manager smoke, and dependency checks pass from the approved runtime root.",
    "Existing local recipe workspace retention or migration behavior is explicitly verified."
  ],
  "linked_context": {
    "task": "P132 direct-smoke Contour runtime investigation",
    "files": [
      "tools/BuildCleanRuntime.ps1",
      "docs/OPENVISIONLAB_CURRENT_HANDOFF.md",
      "docs/OPENVISIONLAB_RELEASE_VERSION_POLICY.md"
    ]
  },
  "work_log": [
    {
      "at": "2026-07-19T00:30:24+09:00",
      "status": "blocked",
      "summary": "Recorded the package/output decision required after P132 established a safe verification runtime without deleting the retained legacy output.",
      "evidence": [
        "artifacts/p132_direct_smoke_contour_host_20260719/clean_runtime_script_final/clean_runtime_manifest.json"
      ]
    },
    {
      "at": "2026-07-19T00:50:00+09:00",
      "status": "resolved",
      "summary": "The user approved timestamped artifacts Dev output, dist\\OpenVisionLab Release output, and retention of the existing bin\\Debug local recipe workspace. The implemented build/publish modes reject stale destinations and passed the agreed runtime checks.",
      "evidence": [
        "artifacts/openvisionlab_clean_runtime_20260719_004600/clean_runtime_manifest.json",
        "dist/OpenVisionLab/clean_runtime_manifest.json",
        "artifacts/p133_clean_runtime_output_contract_20260719/release_recipe_manager_tabs/report.txt",
        "artifacts/p133_clean_runtime_output_contract_20260719/release_good/report.txt",
        "artifacts/p133_clean_runtime_output_contract_20260719/release_bad_expected_ng/report.txt"
      ]
    }
  ],
  "resolved_evidence": [
    "User-approved Dev artifacts, Release dist\\OpenVisionLab, and retained bin\\Debug workspace contract in this chat.",
    "tools/BuildCleanRuntime.ps1 -Mode Dev created artifacts/openvisionlab_clean_runtime_20260719_004600 with required runtime hashes.",
    "tools/BuildCleanRuntime.ps1 -Mode Release published dist/OpenVisionLab with required runtime hashes.",
    "artifacts/p133_clean_runtime_output_contract_20260719/release_recipe_manager_tabs/report.txt",
    "artifacts/p133_clean_runtime_output_contract_20260719/release_good/report.txt",
    "artifacts/p133_clean_runtime_output_contract_20260719/release_bad_expected_ng/report.txt"
  ],
  "created_at": "2026-07-19T00:30:24+09:00",
  "updated_at": "2026-07-19T00:50:00+09:00"
}
---

# PL-0002 개발·배포용 깨끗한 런타임 출력 계약 정의

## 설명

P132는 새 빈 런타임 출력이 현재 Contour 직접 재생을 올바르게 실행하고, 보존된 `bin\Debug`는 레거시 파일 때문에 배포 근거가 될 수 없음을 확인했다. `BuildCleanRuntime`은 검증용 `artifacts` 런타임을 안전하게 만들지만, 영구 개발·배포 출력 위치와 기존 작업공간 보존 정책은 아직 결정되지 않았다.

## 근거

- `tools\BuildCleanRuntime.ps1`는 기존 출력 폴더를 덮어쓰지 않는 새 런타임을 만든다.
- `artifacts\p132_direct_smoke_contour_host_20260719`에서 새 런타임은 Good과 통제된 NG를 통과했고, 기존 `bin\Debug`는 FindContours 접근 위반으로 끝났다.

## 영향

향후 릴리스 또는 증거 명령이 보존된 `bin\Debug`를 사용하면 오래된 런타임 파일을 읽을 수 있다.

## 다음 단계

사용자가 개발 런타임 루트, 배포 패키지 루트, 기존 `bin\Debug` 레시피 작업공간의 보존 또는 보관 정책을 결정해야 한다. 그 뒤에만 가장 작은 출력 전환과 회귀 검증을 구현한다. `bin\Debug`를 일괄 삭제하지 않는다.

## 완료 기준

- 승인된 개발 및 배포 런타임 루트가 문서화된다.
- 깨끗한 패키지/런타임 빌드가 필요한 관리·네이티브 파일을 포함하고 오래된 출력 혼동을 막는다.
- 승인 런타임에서 LLM XML 직접 재생, Recipe Manager 스모크, 의존성 검사가 통과한다.
- 기존 로컬 레시피 작업공간의 보존 또는 이전 방식이 명시적으로 검증된다.

## 작업 기록

- 2026-07-19 00:30 KST: P132의 안전한 검증 런타임 이후 필요한 배포/출력 결정을 차단 항목으로 기록했다.

## 해결 근거

아직 해결하지 않음.

## 참조

- `tools\BuildCleanRuntime.ps1`
- `docs\OPENVISIONLAB_CURRENT_HANDOFF.md`
- `docs\OPENVISIONLAB_RELEASE_VERSION_POLICY.md`

## Resolution (2026-07-19)

P133 records the user's explicit decision: Dev evidence uses a new timestamped `artifacts\openvisionlab_clean_runtime_<timestamp>` runtime, Release output uses a new `dist\OpenVisionLab` package, and the existing `bin\Debug` workspace remains unchanged. Both outputs passed direct Good/expected-NG XML replay; the Release package also passed Recipe Manager smoke. The remaining package-path work is limited to LLM template-dependency portability and is not part of this resolved output-contract issue.
