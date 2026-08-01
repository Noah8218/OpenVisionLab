# 문제 해결 순서

## 결과가 바뀌지 않음

1. Preview 또는 Run Review를 눌렀는지 확인합니다.
2. 현재 입력 Layer가 맞는지 확인합니다.
3. 이전 Step의 출력 Layer가 생성됐는지 확인합니다.
4. 표시 옵션만 바꾼 것은 아닌지 확인합니다.

## 결과가 0개

1. 입력 이미지와 ROI
2. Threshold 방향과 범위
3. Area/Score/Contrast 최소값
4. template 또는 의존 파일
5. 검색 ROI, angle, scale 범위

## 결과가 너무 많음

1. ROI를 의도한 영역으로 좁힙니다.
2. 작은 노이즈와 반복 패턴을 드로잉에서 찾습니다.
3. Area/Width/Height 또는 Score를 조정합니다.
4. Good과 Bad 모두 다시 실행합니다.

## Pipeline NG

1. 첫 NG Step을 선택합니다.
2. 그 Step의 입력 Layer 이미지를 엽니다.
3. route를 확인합니다.
4. ROI와 핵심 파라미터를 확인합니다.
5. 수정 후 Run Review를 다시 누릅니다.

## Guide가 열리지 않음

`OpenVisionLab.exe`와 같은 배포 폴더의 `Guide` 안에 아래 두 파일이 있는지
확인합니다.

```text
Guide/OpenVisionLab_User_Manual.ko.html
Guide/OpenVisionLab_User_Manual.en.html
Guide/guide-manifest.json
```

파일이 없거나 손상 메시지가 나오면 `Guide` 폴더를 복원하거나 프로그램을
다시 받습니다. 저장소의 `docs` 파일을 직접 복사해 대체하지 않습니다.
