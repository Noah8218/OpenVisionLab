# Validation Set, Run History, Qualified Snapshot

이 기능은 한 쌍보다 많은 이미지를 검증하는 숙련 사용자 흐름입니다.

## Validation Set

1. Recipe Manager에서 현재 Recipe를 선택합니다.
2. Local Validation Set을 만들고 이름을 지정합니다.
3. 이미지마다 예상 역할 `OK` 또는 `NG`를 지정합니다.
4. 같은 경로를 중복 등록하지 않았는지 확인합니다.
5. 누락 경로가 있으면 선택한 한 항목만 명시적으로 복구합니다.
6. Pipeline과 기준을 저장합니다.
7. Validation 실행을 직접 시작합니다.
8. 예상/실제/판정과 첫 실패 Step을 검토합니다.

## Run History

1. 완료된 실행을 선택합니다.
2. 사용한 Recipe, Pipeline, 입력 이미지, 실행 시간을 확인합니다.
3. Step별 결과와 저장된 드로잉을 확인합니다.
4. 현재 Recipe와 과거 실행을 혼동하지 않습니다.
5. 필요한 경우 검토용 보고서를 별도 명시적 명령으로 내보냅니다.

## Qualified Snapshot

1. Validation Set 전체 실행이 완료된 기록을 선택합니다.
2. 범위, 메모, 의존 파일 상태를 확인합니다.
3. 명시적으로 snapshot을 생성합니다.
4. hash와 검증 상태를 확인합니다.
5. 수정이 필요하면 원본 snapshot을 바꾸지 말고 working copy를 만듭니다.

Snapshot은 카메라 교정, 생산 조건, 현장 성능을 자동으로 보증하지 않습니다.
