# OpenVisionLab 2D 입력 포맷 의미 대조 — 2026-09-15

## 결론

2D-029의 현재 owner는 유지하고 입력 경계를 명시했다. 검사 Runner는 원본 파일을
`ImreadModes.Unchanged`로 먼저 확인한 뒤 8-bit 입력만 기존
`BitmapImageConverter` 경로로 전달한다. 16-bit depth와 손상 파일은 계산 전에
명확한 오류로 끝난다. ImageCanvas의 `CanvasImageLoader`가 수행하는 16-bit → 8-bit
표시 변환과 BGRA alpha 제거는 display 전용이며 Runner 입력으로 재사용하지 않는다.

첫 전용 계약 실행에서는 GDI+ `new Bitmap(path)`가 16-bit PNG를 8-bit Bitmap처럼
노출해 `BitmapImageConverter`가 입력을 받아들이는 문제가 재현됐다. 따라서
Bitmap만 검사하는 것으로는 원본 파일 depth를 보장할 수 없었다. 수정은 기존
`VisionPipelineSampleCheckService` owner 안의 사전 검사 한 곳으로 제한했다.

## Owner와 실제 호출 경로

### 검사 Runner

`OpenVisionRecipeValidationSetRunner`/recipe review 호출부
→ `VisionPipelineSampleCheckService.RunSampleCheck*`
→ `ValidateInspectionImageFormat(ImreadModes.Unchanged)`
→ 기존 `BitmapImageConverter.ToMat(Bitmap)`
→ `VisionRecipeRunner.RunAsync`.

`ValidateInspectionImageFormat`가 원본 depth를 보존해 읽고 `CV_8U`가 아니면
`NotSupportedException`을 반환한다. 8-bit Gray/BGR/BGRA는 기존 converter의
channel 의미와 row-copy 경계를 그대로 사용한다. 새 normalization service,
algorithm, UI abstraction은 추가하지 않았다.

### ImageCanvas 표시

`RoiImageCanvasViewModel`
→ `CanvasImageLoader.LoadMatFromFile`
→ ImageCanvas texture/display 경로.

이 owner는 `ImreadModes.AnyColor`를 사용해 표시용 Mat을 만들며 현재 계약상
16-bit Gray는 high-byte 기준 8-bit로 표시되고 BGRA는 BGR로 표시된다. 이 Mat은
검사 Runner에 전달되지 않는다.

## 포맷 표

| 입력 | 검사 Runner | ImageCanvas 표시 | 의미 |
|---|---|---|---|
| 8-bit Gray | 지원, `CV_8UC1` | 지원, `CV_8UC1` | 픽셀 값 보존 |
| 8-bit BGR | 지원, `CV_8UC3` | 지원, `CV_8UC3` | BGR channel 순서 보존 |
| 8-bit BGRA | 지원, `CV_8UC4` | 표시 시 `CV_8UC3`로 alpha 제거 | 검사에서는 alpha 포함, 표시는 alpha 비소비 |
| 16-bit Gray | 계산 전 명시적 거절 | 표시 전용 `CV_8UC1`, `value >> 8` | 표시 축소를 검사 정규화로 간주하지 않음 |
| 손상/비디코드 파일 | 계산 전 `ERROR` | 별도 표시 성공을 주장하지 않음 | stale/빈 Mat으로 진행하지 않음 |

현재 표는 확인된 경계만 의미한다. 16-bit 검사 지원, arbitrary TIFF codec 조합,
camera/hardware decode, 모든 WPF 상태를 새로 보장하지 않는다.

## 변경 파일

- `src/OpenVisionLab/Core/Pipeline/Execution/VisionPipelineSampleCheckService.cs`
  - 기존 sample-check owner에 원본 depth/decode 사전 검사 추가.
- `tools/VisionRecipeRunnerSmoke/InputFormatMeaningContract.cs`
  - 8-bit Gray/BGR/BGRA round-trip, 16-bit 계산 전 거절, 손상 decode 거절,
    display-only 16-bit/alpha mapping을 고정.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - `--input-format-meaning-contract` dispatch/help.
- `docs/contracts/openvisionlab/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
  - Runner와 display 입력 의미를 별도 stable contract로 기록.

## 검증 증거

- Debug build: 0 errors, 기존 nullable warnings 16개.
- Release build: 0 errors, 기존 nullable warnings 16개.
- Debug `--input-format-meaning-contract`: 모든 case pass.
- Release `--input-format-meaning-contract`: 모든 case pass.
- Debug/Release `--bitmap-converter-contract`: 기존 5/5 regression pass.
- Debug/Release `--preview-run-reopen-equivalence-contract`: Mean, Blob, Contour,
  Matching, Line 각각 `PASS`.
- 증거 디렉터리:
  - 초기 결함 재현(수정 전, 16-bit Runner가 Bitmap 변환 후 수락):
    `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d029-debug-20260915\input-format-meaning\input_format_meaning_contract.txt`
  - `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d029-debug-20260915\input-format-meaning-final\`
  - `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d029-debug-20260915\bitmap-converter-regression\`
  - `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d029-debug-20260915\preview-run-reopen-regression\`
  - `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d029-release-20260915\input-format-meaning-final\`
  - `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d029-release-20260915\bitmap-converter-regression\`
  - `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d029-release-20260915\preview-run-reopen-regression\`

## 완료 경계

소스 코드와 headless contract 기준 검토 완료. 실제 WPF EXE의 표시 픽셀,
theme/layout/DPI/monitor/input interaction, camera/hardware, 장시간 운전은 이번
slice에서 검증하지 않았다.
