# 오픈비전 랩 (OpenVision Lab)
OpenCV 기반 룰베이스 비전 알고리즘 검증 툴

 - 프로젝트 개요
길이 측정, 교차점 검출, 패턴 매칭, Blob, Contour 등 다양한 룰베이스 비전 검사 알고리즘을 UI 기반으로 테스트하고 파라미터를 튜닝할 수 있는 검증 툴입니다. 비전 검사 시스템 개발 과정에서 알고리즘의 신뢰성을 빠르게 확보하고, 최적의 파라미터 튜닝에 소요되는 시간을 대폭 단축하기 위해 개발되었습니다.

 - 주요 기능
직관적인 다중 뷰어 디스플레이: 원본 검사 이미지, 이진화(Threshold) 전처리 결과, 그리고 최종 알고리즘 적용 결과를 한 화면에서 다중 뷰어로 비교 분석할 수 있습니다.

실시간 파라미터 튜닝: 알고리즘별 설정값을 UI에서 직접 조정하고, 그에 따른 결과를 즉각적으로 확인하여 디버깅 효율을 높입니다.
 - 핵심 비전 검사 알고리즘 지원:
길이 검사: 지정 영역의 치수 및 길이 측정.
교차점 검사 (얼라인 전용): 정밀한 라인/엣지 검출을 통한 교차점 좌표 산출 및 얼라인먼트.
패턴 검사: 각도 보정 알고리즘을 추가 보완하여 회전된 이미지에서도 강건한 패턴 매칭 수행.
Blob / Contour 검사: 형태학적 특징 기반의 객체 검출 및 윤곽선 분석.
이미지 프로세싱(전처리) 모듈 내장:
Morphology, Filter, Edge Detection (Canny 등), Histogram 처리 등 검사 전 품질을 높이기 위한 다양한 전처리 툴을 제공합니다.

 - 길이검사
<img width="1920" height="1080" alt="_20260603_083611" src="https://github.com/user-attachments/assets/f527ddb5-f3d4-46a6-8498-9f7501557db0" />

 - 교차점 검사
<img width="1920" height="1040" alt="비전 테스트 프로그램(룰베이스)_fitLine PNG" src="https://github.com/user-attachments/assets/7c5e4d3d-ca32-4f29-83c1-269d2b1b1489" />

 - 패턴 검사(각도 보정 알고리즘 추가 보완 개발)
<img width="1920" height="1040" alt="비전 테스트 프로그램(룰베이스)_패턴매칭_회전검출 PNG" src="https://github.com/user-attachments/assets/21aea53f-0a3b-4f35-adde-66ad05ccf662" />
<img width="1269" height="885" alt="비전 테스트 프로그램(룰베이스)_패턴 등록 PNG" src="https://github.com/user-attachments/assets/b847b595-e4c7-41cc-87f3-e08036d9aa35" />

 - Blob 검사
<img width="1920" height="1040" alt="비전 테스트 프로그램(룰베이스)_blob PNG" src="https://github.com/user-attachments/assets/f51b188b-a72e-42c0-a9bf-7af0a845b1d8" />

 - Contour 검사
<img width="1920" height="1040" alt="비전 테스트 프로그램(룰베이스)_컨투어 PNG" src="https://github.com/user-attachments/assets/f1a5d5f5-42a7-400b-9cbd-841a157fa1cb" />

 - 이미지 프로세싱
<img width="1920" height="1040" alt="비전 테스트 프로그램(룰베이스)_이미지 프로세싱 PNG" src="https://github.com/user-attachments/assets/2c486b55-7bfd-4638-9db2-19a41801284f" />





