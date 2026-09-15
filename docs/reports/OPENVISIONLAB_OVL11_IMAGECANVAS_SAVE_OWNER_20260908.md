# OpenVisionLab OVL-11 ImageCanvas 저장 owner — 2026-09-08

## Completion record

Status: Complete

Scope: `RoiImageCanvasViewModel.SaveCurrentImage(string)`에서 Mat 파일 저장
정책만 추출했다. 대화상자, ContextMenu, 포인터·키보드 입력, texture 업로드와
기존 외부 consumer 계약은 이 slice에서 변경하지 않았다.

Acceptance criteria:

- 기존 public `SaveCurrentImage`와 모든 호출자는 유지된다.
- 확장자 기본값, 디렉터리 생성, save override 호출, 빈 Mat 거부,
  `Cv2.ImWrite` 동작이 유지된다.
- Mat의 생성·수명·Dispose 소유권은 ViewModel에 남고 새 owner는 Mat을 보관하거나
  해제하지 않는다.
- ImageCanvas Debug/Release 빌드와 기존 owned-Mat 저장 smoke가 통과한다.

## 구조 변경

- 이전 owner: `RoiImageCanvasViewModel`이 확장자·경로·디렉터리·OpenCV 쓰기를
  직접 소유했다.
- 새 owner: `CanvasImageSaver`가 동기 Mat 파일 저장 정책을 소유한다.
- 호출 경로: 기존 command/presenter/smoke caller →
  `RoiImageCanvasViewModel.SaveCurrentImage` →
  `CanvasImageSaver.SaveMat` → 기존 callback 또는 `Cv2.ImWrite`.
- 의존 방향: ViewModel → ImageCanvas 저장 owner → OpenCvSharp/System.IO.
  새 owner는 앱·WPF View·Recipe·Layer·ImageSpace·SDK를 참조하지 않는다.
- 상태/data: 현재 Mat과 callback 수명은 ViewModel이 계속 소유하며,
  `CanvasImageSaver`는 호출 중에만 동작한다.

## 보존한 동작

빈 경로는 즉시 `false`를 반환한다. 경로 확장자가 없으면 `.png`를 붙이고,
부모 디렉터리를 만든 뒤 callback을 먼저 호출한다. callback이 없을 때만 빈 Mat을
거부하고 `Cv2.ImWrite`를 호출한다. 이 순서는 기존 구현과 동일하다.

## 실제 검증

- ImageCanvas Debug/Release build: 경고 0, 오류 0.
- 기존 `ImageCanvasExternalConsumerSmoke` Release build: 경고 0, 오류 0으로
  public `RoiImageCanvasViewModel.SaveCurrentImage` 소비 경로의 컴파일 호환성을
  확인했다.
- `RunUiPrecheck.ps1 -Targets wpf_imagecanvas_owned_mat_load` Debug:
  `OK 1 / WARN 0 / NG 0`, 실제 owned-Mat load → Dispose → save 경로 통과.
- 같은 target Release: `OK 1 / WARN 0 / NG 0`.
- 구조 검색에서 `RoiImageCanvasViewModel`의 `Cv2.ImWrite`,
  `Directory.CreateDirectory`, `EnsureImageFileExtension`가 제거되고,
  동일 호출이 `CanvasImageSaver`에 존재함을 확인했다.
- 변경 후 정량 감사 `Invoke-RefactorAudit.ps1 -Verify`도 통과했다
  (764 C# 파일, 58 XAML 파일, partial 106개, project cycle 0개).
- Recipe/XML, Preview/Run, Layer, ImageSpace, SDK, 기존 외부 consumer 공개
  계약은 변경하지 않았다.

증거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-image-save-owner-20260908`
의 `imagecanvas-build-*-final.log`, `ui-precheck-*-final/ui_precheck_report.md`,
`structure-proof.txt`, `refactor-proof-report.md`,
`quantitative-audit-after-save` 및 smoke 출력.

이번 변경은 저장 owner만 다루므로 대화상자·ContextMenu의 full WPF 상태,
지원 theme/layout/DPI matrix는 아직 검증하지 않았다. 해당 결합은 같은 OVL-11의
다음 독립 slice에서 다룬다.

Boundary / next dependency: `RoiImageCanvasViewModel.Commands.cs`에 남은
`OpenFileDialog`·`SaveFileDialog`와 `ContextMenu` 직접 참조를 명시적 View/dialog
host로 이동하는 작업이 다음 우선순위다. 기존 owner를 재분할하거나 partial 파일을
추가하지 않는다.
