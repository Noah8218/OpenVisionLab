# Pipeline: Tool 연결·저장·검토

## 목적

여러 Tool의 입력/출력 Layer를 연결하고 같은 순서와 설정으로 반복 실행합니다.

## 작성 순서

1. 상단에서 Recipe를 선택하거나 새로 만듭니다.
2. 첫 Tool을 개별 Preview합니다.
3. 결과가 맞으면 `파이프라인에 추가·저장`을 누릅니다.
4. 다음 Tool 입력을 이전 출력 Layer로 선택합니다.
5. 각 Tool을 개별 Preview한 뒤 저장합니다.
6. `Pipeline 보기`를 엽니다.
7. Step 번호와 `입력 -> 출력` route를 확인합니다.
8. 아직 실행하지 않은 Step이 `WAIT`인지 확인합니다.
9. `Run Review`를 직접 누릅니다.
10. Step 1부터 출력 이미지와 Metric을 확인합니다.
11. NG면 첫 NG Step에서 멈춰 입력 Layer와 설정을 봅니다.
12. Good/Bad를 같은 Pipeline으로 실행합니다.
13. 저장 후 재시작하고 Step/route/설정 복원을 확인합니다.
14. 다시 명시적으로 Run Review합니다.

## Review에서 확인할 것

- Step 수와 실행 순서
- 각 입력 Layer의 생성 Step
- OK/NG/WAIT/오류 구분
- 첫 실패 Step
- 선택 Step의 입력/출력 이미지
- 드로잉, Metric, object row, reject 이유
- Good/Bad와 Validation Set 결과

Pipeline 정의가 유효하다는 것과 최종 검사 결과가 OK라는 것은 다릅니다.
