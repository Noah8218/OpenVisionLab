# OpenVisionLab Undo/Redo Scope

Updated: 2026-06-18

이 문서는 Undo/Redo가 어디까지 닫혔는지 분리해서 관리한다. 같은 항목을 다시 막연한 안정화 과제로 올리지 않기 위한 기준 문서다.

## Closed Scope

| Area | Scope | Implementation | Verification |
| --- | --- | --- | --- |
| PropertyGrid | Tool Form의 WPG property 값 변경을 `Ctrl+Z`, `Ctrl+Y`, `Ctrl+Shift+Z`로 되돌린다. | `Library/RJControls/RJForms/VisionTestForm.cs`, `Library/OpenVisionLab.History/PropertyGridUndoBinder.cs` | `HistoryContract=OK`, build OK |
| Pipeline | Step 추가, 삭제, 이동, 복제, 활성화, acceptance preset, Step property 변경을 되돌린다. | `0. UI/6) Vision Test/FormVision_Pipeline.cs` | platform precheck OK |
| ROI edit | ROI draw, move, delete, paste를 snapshot으로 되돌린다. | `0. UI/2) POPUP/FormImageEditView.cs`, `Library/OpenVisionLab.ImageCanvas` | `HistoryContract=OK` |
| Layer image | 기존 레이어의 image buffer 변경을 snapshot으로 되돌린다. | `1. Core/DisplayLayerImageHistoryService.cs`, `1. Core/DisplayManagerService.cs` | build OK, `HistoryContract=OK` |
| Layer create/delete | 새 레이어 생성과 사용자 닫기 삭제를 같은 layer history stack에서 되돌린다. 복원 시 원래 title, index, image, ROI, TrainROI를 복원한다. | `DisplayLayerSnapshot`, `DisplayLayerPresenter`, `DisplayManagerService`, `ImageSpaceService` | build OK, `HistoryContract=OK` |

## Intentional Limits

- DockPanel의 정확한 visual docking layout까지 되돌리지는 않는다. 레이어 문서가 복원되고, title/index/image/ROI 상태가 복원되는 것을 1차 완료 기준으로 둔다.
- 앱 시작 시 생성되는 기본 `Main` placeholder layer는 사용자 Undo stack에 남기지 않는다.
- 대용량 이미지는 history snapshot 한도를 넘으면 기록하지 않는다. 이는 메모리 보호 정책이다.
- Acceptance 기준 완화, ROI 자동 이동, score threshold 완화 같은 AI Recipe 의미 변경은 자동 Undo 대상이 아니라 사용자 확인 후 적용하는 편집 작업이다.

## Reopen Conditions

- 레이어 생성 후 `Ctrl+Z`가 레이어를 제거하지 못하는 경우.
- 레이어 삭제 후 `Ctrl+Z`가 원래 레이어 title/image/index를 복원하지 못하는 경우.
- Tool Form property 변경 후 `Ctrl+Z`가 실제 검사 preview에 반영되지 않는 경우.
- ROI 편집 후 `Ctrl+Z`가 ROI geometry를 복원하지 못하는 경우.
- Pipeline Step 편집 후 Undo/Redo가 Step Flow와 property grid를 동기화하지 못하는 경우.
