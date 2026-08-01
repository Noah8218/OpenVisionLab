# Tool 설정과 명시적 Preview

## 공통 순서

1. 이미지와 입력 Layer를 준비합니다.
2. 왼쪽 Tool 검색 또는 목록에서 목적에 맞는 Tool을 엽니다.
3. 입력 Layer와 출력 Layer를 먼저 확인합니다.
4. 처음에는 `기본` preset이 있으면 사용합니다.
5. PropertyGrid에서 가장 중요한 값과 ROI만 조정합니다.
6. 설정 화면의 대상/배경/방향 설명을 읽습니다.
7. `미리보기 실행`을 직접 누릅니다.
8. 결과 Layer에서 드로잉이 실제 대상을 가리키는지 봅니다.
9. Metric이 드로잉과 같은 결과를 설명하는지 봅니다.
10. 실패하면 입력 -> ROI -> 기본 파라미터 -> 고급 파라미터 순서로 확인합니다.
11. 결과가 맞을 때만 `파이프라인에 추가·저장`을 누릅니다.

## 실행되지 않는 동작

다음 동작은 설정만 바꾸며 Preview나 Pipeline을 실행하지 않습니다.

- Tool 열기/닫기/우측 고정
- preset 선택
- PropertyGrid 값 변경
- 입력/출력 Layer 선택
- ROI 표시, 결과 표시, boolean 표시 옵션 변경
- 출력 Layer 생성

## 결과 확인 순서

1. 상태가 Preview OK/NG인지
2. 출력 이미지가 갱신됐는지
3. box, 선, 중심, ROI가 실제 대상 위에 있는지
4. ResultCount, Area, Score, Distance 같은 Metric이 맞는지
5. 처리 시간이 갑자기 증가하지 않았는지

`Preview OK`는 Tool 실행이 완료됐다는 뜻입니다. 최종 검사 `결과 OK/NG`와
같은 뜻이 아닙니다.
