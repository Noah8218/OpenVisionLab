# OpenVisionLab

OpenVisionLab은 C#과 OpenCvSharp 기반으로 Rule-base 머신비전 검사 알고리즘을 테스트하고, ROI/전처리/결과 시각화를 하나의 UI에서 검증하기 위한 Windows Desktop 애플리케이션입니다.

## 개발 환경

- Language: C#
- Framework: .NET 8 Windows Desktop (`net8.0-windows`)
- UI: Windows Forms / WPF interop
- Vision Library: OpenCvSharp
- Rendering: SharpGL 기반 OpenGL image canvas
- IDE: Visual Studio 2022
- Platform: Windows / x64

## 주요 기능

- 길이 측정, 교차점 검출, 패턴 매칭, Blob, Contour 등 머신비전 검사 흐름 검증
- Morphology, Filter, Edge Detection, Histogram, HSV 등 이미지 전처리 테스트
- 레이어 기반 이미지 표시와 검사 결과 비교
- OpenGL 기반 이미지 표시, 확대/축소, ROI 편집
- Vision Test 폼을 통한 검사 파라미터 조정과 결과 표시

## 현재 리팩토링 방향

- 메인 프로젝트는 .NET 8 Windows Desktop 기반으로 마이그레이션되었습니다.
- 기존 ImageBox/GDI 중심 표시 경로는 `Library/OpenVisionLab.ImageCanvas` 기반 OpenGL 뷰어로 전환 중입니다.
- 이미지/레이어 상태는 `OpenVisionLab.ImageSpace.Core`, 표시 관련 공용 기능은 `OpenVisionLab.Display.Core`로 분리하는 방향을 유지합니다.
- ROI 편집은 `RoiImageCanvasView`와 `RoiImageCanvasViewModel` 기준으로 관리합니다.

자세한 인계 내용과 남은 작업은 `docs/CODEX_HANDOFF.md`를 기준으로 확인하세요.
