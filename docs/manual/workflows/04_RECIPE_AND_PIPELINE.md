# Recipe와 Pipeline 만들기

## 역할

- Recipe: 저장 단위입니다.
- Pipeline: Recipe 안에서 실행되는 Step 목록입니다.
- Step: 한 Tool과 파라미터, 입력/출력 Layer입니다.

## 새 Recipe 순서

1. 상단 Recipe 선택 영역에서 새 Recipe를 만들고 이름을 확인합니다.
2. 이미지 또는 공개 샘플을 엽니다.
3. 첫 Tool을 열고 입력 `Main`과 별도 출력 Layer를 지정합니다.
4. 명시적으로 Preview하고 결과를 확인합니다.
5. `파이프라인에 추가·저장`을 누릅니다.
6. 다음 Tool의 입력을 이전 Step의 출력 Layer로 선택합니다.
7. 같은 방법으로 Preview 후 추가·저장합니다.
8. `Pipeline 보기`를 엽니다.
9. 모든 Step의 `입력 -> 출력` 경로를 위에서 아래로 읽습니다.
10. `Run Review`를 직접 누릅니다.

예시:

```text
01 Threshold : Main -> Threshold_Preview
02 Blob      : Threshold_Preview -> Blob_Preview
```

## 재시작 확인

1. Recipe와 Pipeline을 저장합니다.
2. 프로그램을 종료하고 다시 엽니다.
3. 같은 Recipe를 선택합니다.
4. Step과 route가 남아 있는지 확인합니다.
5. Step이 `WAIT`인 것을 확인합니다. 복원은 실행이 아닙니다.
6. 이미지를 준비하고 `Run Review`를 직접 누릅니다.

## route 문제 확인

- 입력 없음: 이전 출력 이름과 현재 입력 이름을 비교합니다.
- 오래된 결과: Run Review를 눌렀는지 확인합니다.
- 잘못된 결과: 첫 NG Step부터 입력 이미지를 확인합니다.
- 분기: 같은 원본에서 독립 검사를 할 때만 같은 입력 Layer를 사용합니다.
