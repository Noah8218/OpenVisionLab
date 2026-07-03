# OpenVisionLab AI Recipe Interactive Edit Plan

Updated: 2026-06-16

AI Recipe의 목표는 LLM이 만든 XML을 그대로 신뢰하는 것이 아닙니다. 목표는 LLM이 제안한 Pipeline을 OpenVisionLab이 검증하고, 실패한 지점을 사용자가 빠르게 고칠 수 있게 만드는 것입니다.

## Current State

현재 AI Recipe 흐름은 다음 수준까지 도달했습니다.

- 이미지와 요구사항을 기반으로 VisionPipeline XML을 Import할 수 있다.
- XML 유효성 검증을 수행한다.
- Run Preview로 실제 이미지에서 실행한다.
- 실패 시 첫 실패 Step을 찾는다.
- ErrorCode, DiagnosticHint, SuggestedFix를 제공한다.
- 직접 영향을 받는 dependent Step을 표시한다.
- Copy AI Feedback으로 LLM에게 재시도 요청할 정보를 제공한다.
- Safe Fix grid에서 적용 가능한 구조/파라미터 수정 후보를 체크하고 적용할 수 있다.
- `Apply Fix`, `Apply + Preview` 흐름으로 XML을 직접 고치지 않고 재검증할 수 있다.
- Acceptance limit 변경 후보는 기본 선택하지 않고, 사용자가 결과를 확인한 뒤 명시적으로 선택해야 한다.

현재 한계:

- Safe mechanical fix 중심이며, ROI 이동/확장이나 score/acceptance 완화 같은 의미 변경은 자동 적용하지 않는다.
- 실패 유형별 전용 editor는 아직 제한적이다.
- Good/Bad sample pair 기반으로 “이 수정이 실제로 맞는지” 자동 판정하는 흐름은 더 필요하다.

## Target UX

실패한 Recipe를 열었을 때 사용자는 아래 순서로 움직여야 합니다.

```text
Run Preview NG
  -> First Failed Step 자동 선택
  -> 실패 이유 요약
  -> 수정 후보 표시
  -> 사용자 직접 수정 또는 LLM 수정 요청
  -> Preview 재실행
  -> OK면 Save
```

## Edit Surface 1: Failed Step Quick Fix

첫 번째 구현 대상입니다.

표시 항목:

- Failed Step name
- ToolType
- ErrorCode
- ResultStatus
- Input Layer
- Output Layer
- DiagnosticHint
- SuggestedFix
- Patch Proposal

수정 가능한 항목:

- Input Layer
- Output Layer
- 핵심 파라미터
  - Threshold: Threshold, RangeMin, RangeMax, Invert, Adaptive block size/weight
  - Morphology: Operator, KernelWidth, KernelHeight, Iterations
  - Contour/Blob: MinArea, MaxArea, DetectMode, ROI
  - LineGauge: threshold, ROI, expected count/range

동작:

- Apply Patch: 현재 Pipeline Step에만 적용한다.
- Apply And Preview: 적용 후 Run Preview를 다시 실행한다.
- Copy AI Feedback: 성공 Step은 유지하고 첫 실패 Step과 dependent Step만 수정하라는 피드백을 복사한다.

## Edit Surface 2: Layer Flow Fix

Layer Flow 오류는 파라미터 오류보다 먼저 봐야 합니다.

표시 항목:

- Expected previous output
- Current input layer
- Branch status
- Branch reason
- Direct dependents

버튼:

- `Use Previous Output`
- `Keep Branch`
- `Rename Output Layer`
- `Preview Input`
- `Preview Output`

규칙:

- 기본은 previous output을 읽는 체인입니다.
- Main 또는 이전 이전 레이어를 읽으면 Branch로 표시합니다.
- Branch는 허용하지만 사용자가 의도 확인을 해야 합니다.

## Edit Surface 3: LLM Retry Scope

LLM에게 다시 요청할 때는 범위를 제한해야 합니다.

LLM feedback에는 반드시 포함합니다.

- Do not rewrite successful previous steps.
- Keep stable output layer names unless the layer flow is wrong.
- Fix the first failed step first.
- Only update direct dependent steps when necessary.
- Preserve final OverlayMerge review image when branch results must be inspected together.

## Contract Checks

AI Recipe interactive edit은 다음 smoke/contract가 필요합니다.

1. Failed preview selects first failed row.
2. Failed step quick fix panel shows ErrorCode, Hint, Fix, Patch Proposal.
3. `Use Previous Output` changes only selected Step input.
4. `Apply And Preview` reruns preview and keeps successful prior steps stable.
5. `Copy AI Feedback` includes first failed step and direct dependents.
6. Final summary preview remains available after successful retry.

## Implementation Order

1. Closed: read-only failed-step quick-fix context.
2. Closed: `Use Previous Output`, `Keep Branch`, layer flow controls.
3. Closed / Watch: safe mechanical parameter fixes for threshold/range/kernel/scale/sampling.
4. Closed / Watch: Apply Fix without rerun.
5. Closed / Watch: Apply + Preview.
6. Next: failed-step quick fix contract should verify safe-fix grid state and default selection policy.
7. Next: Good/Bad pair validation should guard any acceptance/score/ROI tuning suggestion.

## Non-goals

- LLM이 최종 승인 없이 자동으로 Recipe를 저장하지 않는다.
- 성공한 이전 Step을 임의로 재작성하지 않는다.
- 모든 Tool parameter를 한 번에 interactive editor로 만들지 않는다.
- AI Recipe가 Tool Form이나 Pipeline Form의 수동 검증을 대체하지 않는다.
