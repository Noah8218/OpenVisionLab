# OpenVisionLab N-image 검증 이미지 경계 리팩터링

작성일: 2026-09-13 KST  
작업 원장: `PL-0031`  
대상: `C:\Git\2D\Dev`

## 사용자 목표와 범위

이전 Sample Picker 경계에 이어 N-image 검증 화면의 이미지 미리보기 경계를
확인했다. 컨트롤러가 선택·검증 상태와 binding을 계속 소유하되, 이미 존재하는
`OpenVisionBitmapImagePreviewFactory`가 파일 기반 `BitmapImage` 디코드,
`OnLoad`, `Freeze`, 누락·손상 경로의 `null` 정책을 소유하도록 이동했다.
새 interface, manager, wrapper, registry, Partial은 추가하지 않았다.

## Refactor proof plan

### 현재 구조

- 현재 책임 owner: `VisionToolNImageVerificationController.LoadBitmap`
- 현재 호출 경로: N-image Window의 `SelectedSourceImage`/
  `SelectedDrawingImage` binding -> `SelectedRow` 변경 -> controller의
  `LoadSelectedEvidence()` -> 직접 `File.Exists`/`FileStream`/
  `BitmapImage` 생성
- 현재 dependency direction: 검증 controller가 파일 존재 확인, stream decode,
  `BitmapCacheOption.OnLoad`, stream-independent `Freeze`를 직접 소유
- mutable state writer: `VisionToolNImageVerificationController.SelectedRow`
  및 실행 session이 선택된 증거 경로를 결정
- lifetime owner: controller가 선택된 `BitmapImage` 값을 교체하고 Window가
  zoom controller만 해제; OnLoad/frozen 결과는 파일 stream과 분리됨

### 의도한 구조

- 이미지 디코드 owner: 기존 `OpenVisionBitmapImagePreviewFactory`
- 호출 경로: Window binding -> controller `LoadSelectedEvidence()` ->
  `LoadBitmap(path)` facade ->
  `OpenVisionBitmapImagePreviewFactory.TryCreateFromPath(path, 0)`
- dependency direction: controller는 선택된 경로와 binding 상태를 유지하고,
  기존 factory가 파일 기반 WPF preview 정책을 담당
- 보존 계약: `SelectedSourceImage`/`SelectedDrawingImage` 이름과
  `BitmapImage` 타입, missing/corrupt `null`, OnLoad/frozen semantics 유지

## 구현 결과

- `VisionToolNImageVerificationController`의 `LoadBitmap`는 기존 factory 호출
  하나만 남기고 직접 `FileStream`, `BitmapImage` 초기화, `OnLoad`, `Freeze`를
  제거했다.
- `OpenVisionBitmapImagePreviewFactory.TryCreateFromPath`의 반환 타입을
  `BitmapImage`로 구체화했다. 기존 `BitmapSource` 소비자는 암시적 업캐스트로
  계속 동작하며, 기존 `CreateFromPath` throwing 계약은 유지했다.
- 선택·검증 상태와 Window의 zoom/lifetime 계약은 변경하지 않았다.

## 구조적 증거

- old coupling absence: controller에 `new FileStream`, `BitmapCacheOption.OnLoad`,
  `image.StreamSource`가 없다.
- new owner: factory가 `BitmapImage` path decode, `OnLoad`, `Freeze`를 갖는다.
- binding contract: 두 selected preview property가 계속 `BitmapImage`다.
- no speculative structure: 새 프로젝트·서비스·인터페이스·래퍼·Partial 없음.

## 검증

- `ToolNImageVerificationImageBoundaryContract`: Debug 6/6 PASS.
- `VisionRecipeRunnerSmoke` Debug build: 경고 0, 오류 0.
- 증거 파일:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\tool-n-image-verification-image-boundary-20260913\tool-n-image-verification-image-boundary-contract.txt`

Release solution/Smoke builds, readiness, documentation index, issue-ledger, and
final diff checks are recorded in the phase summary after this bounded slice was
closed.
Full WPF theme/DPI/input/monitor, hardware/GPU/SDK, long-running native shutdown,
and third-party DLL redistribution rights remain unverified.
