# 오픈비전 랩 (OpenVision Lab)

OpenVisionLab은 산업용 머신비전 검사 개발 과정에서 사용되는 Rule-base 알고리즘을 C#과 OpenCvSharp 기반으로 검증하는 테스트 플랫폼입니다.

ROI 설정, 전처리, 파라미터 튜닝, 그리고 결과 시각화를 하나의 UI에서 통합하여 수행할 수 있도록 개발되었습니다. 비전 검사 시스템 개발 시 알고리즘의 신뢰성을 빠르게 확보하고, 최적의 파라미터 튜닝에 소요되는 시간을 단축하는 것을 목표로 합니다.

산업 현장에서 필수적인 '길이 측정, 교차점 검출, 패턴 매칭, Blob, Contour' 등의 핵심 알고리즘을 제대로 익히면 수많은 비전 문제를 해결할 수 있습니다. 이 프로그램을 통해 많은 분이 비전 기술에 입문하고 활용하시길 바랍니다.

## 개발 환경 및 기술 스택

OpenVisionLab의 메인 프로젝트는 최신 `.NET 8` 기반으로 마이그레이션되었으며, 대용량 이미지 처리 성능 향상을 위해 뷰어를 OpenGL로 업그레이드했습니다.

- **Language:** C#
- **Framework:** .NET 8 Windows Desktop (`net8.0-windows`)
- **UI:** Windows Forms / WPF interop
- **Rendering Engine:** **OpenGL (GDI+에서 업그레이드)**
  - 고해상도 이미지 및 결과 시각화 시 끊김 없는 렌더링 및 CPU 부하 감소
- **Vision Library:** OpenCvSharp4
- **IDE:** Visual Studio 2022
- **Platform:** Windows / x64

## 주요 기능

### 1. 직관적인 다중 뷰어 디스플레이 (OpenGL 기반)
원본 검사 이미지, 이진화(Threshold) 전처리 결과, 최종 알고리즘 적용 결과를 한 화면에서 비교 분석할 수 있습니다. OpenGL 도입으로 고해상도 이미지에서도 부드러운 확대/축소 및 이동을 지원합니다.

### 2. 실시간 파라미터 튜닝
알고리즘별 설정값을 UI에서 직접 조정하고, 그에 따른 결과를 OpenGL 뷰어를 통해 즉각적으로 확인하여 디버깅 효율을 극대화했습니다.

### 3. 핵심 비전 검사 알고리즘 지원

* **길이 검사:** 지정 영역 내 객체의 치수 및 길이 측정.
* **교차점 검사 (얼라인먼트):** 라인/엣지 검출을 통한 정밀 교차점 좌표 산출.
* **패턴 검사:** 각도 보정 알고리즘을 강화하여 회전된 이미지에서도 강건한 패턴 매칭 수행.
* **Blob / Contour 검사:** 형태학적 특징 기반의 객체 검출 및 윤곽선 분석.

### 4. 내장 이미지 프로세싱(전처리) 모듈
검사 품질 향상을 위해 다양한 전처리 툴을 제공합니다.
* Morphology (Erode, Dilate, Open, Close)
* Image Filters (Blur, Gaussian, Median)
* Edge Detection (Canny, Sobel)
* Histogram 처 및 Thresholding

---

## 실행 화면

### 길이 검사
![_20260603_083856](https://github.com/user-attachments/assets/4f6e92a4-584c-47bc-8949-dfc55a9510db)

### 교차점 검사 (fitLine)
![비전 테스트 프로그램(룰베이스)_fitLine PNG](https://github.com/user-attachments/assets/7c5e4d3d-ca32-4f29-83c1-269d2b1b1489)

### 패턴 검사 (회전 검출 및 각도 보정)
![비전 테스트 프로그램(룰베이스)_패턴매칭_회전검출 PNG](https://github.com/user-attachments/assets/21aea53f-0a3b-4f35-adde-66ad05ccf662)
![비전 테스트 프로그램(룰베이스)_패턴 등록 PNG](https://github.com/user-attachments/assets/b847b595-e4c7-41cc-87f3-e08036d9aa35)

### Blob 검사
![비전 테스트 프로그램(룰베이스)_blob PNG](https://github.com/user-attachments/assets/f51b188b-a72e-42c0-a9bf-7af0a845b1d8)

### Contour 검사
![_20260603_092304](https://github.com/user-attachments/assets/c364daab-4b0a-4237-8592-62fa4a9c8d88)

### 이미지 프로세싱 (전처리 모듈)
![비전 테스트 프로그램(룰베이스)_이미지 프로세싱 PNG](https://github.com/user-attachments/assets/2c486b55-7bfd-4638-9db2-19a41801284f)
