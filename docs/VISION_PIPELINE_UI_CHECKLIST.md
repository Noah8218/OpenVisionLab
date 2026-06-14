# Vision Pipeline UI Checklist

Pipeline Form은 단순한 실행 버튼 모음이 아니라 검사 Recipe 편집기여야 한다. 사용자는 각 Step을 클릭했을 때 “무엇을 입력으로 받아, 어떤 파라미터로 처리하고, 어디에 결과를 냈는지”를 즉시 이해할 수 있어야 한다.

## 1. Step Tree

Step Tree는 실행 순서를 보여준다.

- Step 번호가 보인다.
- ToolType이 보인다.
- 실행 상태가 보인다.
- Disabled Step은 일반 Step과 시각적으로 구분된다.
- 실패/타임아웃/취소 상태는 성공 상태보다 강하게 보인다.

권장 표시:

```text
01 Threshold [Passed]
02 Morphology [Passed]
03 Contour [Failed]
```

## 2. Step 상세 패널

Step을 선택하면 아래 정보가 한 화면에 보여야 한다.

- Step 이름
- ToolType
- 상태
- Input Layer -> Output Layer
- 실행 시간
- 메시지
- Acceptance 요약
- 주요 Metric

이 영역은 PropertyGrid와 역할이 다르다. PropertyGrid는 수정용이고, Step 상세 패널은 결과 해석용이다.

## 3. PropertyGrid

PropertyGrid는 현재 선택 Step의 수정 가능한 설정만 보여야 한다.

- `Name`, `Input Layer`, `Output Layer`, `Enabled`는 상단 Step 그룹
- Tool별 파라미터는 Tool 그룹
- Acceptance 설정은 Acceptance 그룹
- 숨겨야 하는 내부 필드는 보이지 않게 처리
- 콤보박스/슬라이더는 작은 버튼을 눌러야만 열리는 구조를 피한다.

## 4. Preview

Preview는 Step 실행 결과를 확인하는 영역이다.

- 실행 전: Placeholder와 “No Result” 상태 표시
- 실행 중: Running 상태 표시
- 실행 성공: Output 이미지 표시
- 실행 실패: 실패 상태와 메시지 표시
- Acceptance가 있으면 OK/NG Badge 표시

가능하면 Step별 결과 이미지는 캐시되어야 한다. 사용자가 다른 Step을 클릭했다가 돌아와도 결과를 다시 볼 수 있어야 한다.

## 5. Run Log

Run Log는 실행 이력이다. 디버그 콘솔처럼 길게 쌓는 것보다 사용자가 판단할 수 있는 사건 중심으로 보여준다.

- Validation 결과
- Step 시작
- Step 완료
- Step 실패
- Pipeline 완료
- 저장/로드 결과

메인 로그와 연결될 때는 `LogLevel.Inspect`를 사용한다.

## 6. Layer / Result

Pipeline 실행 후 사용자가 가장 자주 확인하는 것은 결과 레이어다.

- Output Layer 이름
- 이미지 크기
- 현재 표시 여부
- 어떤 Step이 만든 결과인지
- OK/NG 또는 Metric 요약

레이어를 클릭하면 메인 Viewer가 해당 레이어로 전환되는 흐름이 자연스럽다.

## 7. UX 완료 기준

Pipeline Form은 아래 기준을 만족하면 1차 완료로 본다.

- Step 하나를 추가하고 바로 파라미터를 수정할 수 있다.
- Run 후 각 Step의 상태와 결과 이미지를 확인할 수 있다.
- 실패한 Step이 어디인지 즉시 보인다.
- Recipe를 저장/로드해도 Step과 파라미터가 유지된다.
- Sample/History/Batch로 검증 흐름이 이어진다.

이 기준을 만족한 뒤에 LLM Recipe 생성, DLL Runner, 샘플 배포 작업으로 넘어가는 것이 좋다.
