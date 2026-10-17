# UI Screenshot Smoke

Updated: 2026-09-24 KST

목적은 변경한 UI에 맞는 검사를 선택하고 증거의 한계를 구분하는 것입니다.
등록된 target·suite·assertion의 원본은
[runner](../../tools/PipelineViewerScreenshotSmoke/Program.cs)와 각 contract입니다.
대상별 동작 설명을 여기에 복제하지 않습니다.

## Run

먼저 변경한 View/컨테이너·공유 스타일의 소비자와 관련 안정 계약을 찾습니다.
현재 workspace에서 빌드한 runner의 목록을 조회하고 필요한 target만 선택합니다.

```powershell
# 이미 현재 소스로 빌드한 runner의 등록 목록 조회. --list는 UI 검증이 아님.
dotnet .\tools\PipelineViewerScreenshotSmoke\bin\x64\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll --list

# 예: 라우팅 변경의 focused suite. 실행 스크립트가 restore/build 후 검사.
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiScreenshotSmoke.ps1 -Platform x64 -Suite route -OutputDir "$env:TEMP\OpenVisionLab-UiReview\route"

# 예: 한 회귀 target만 검사. 기존 실행의 증거 폴더를 덮어쓰지 않음.
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiScreenshotSmoke.ps1 -Platform x64 -Targets wpf_shell_host_pipeline_review -OutputDir "$env:TEMP\OpenVisionLab-UiReview\pipeline"
```

명령의 target과 출력 폴더는 실제 작업에 맞게 정합니다. 설치된 MSBuild 등 실행 전제와
선택 옵션은 [실행 스크립트](../../tools/RunUiScreenshotSmoke.ps1)가 원본입니다.
증거·임시 데이터는 소스와 사용자 데이터에서 분리합니다. 기존 사용자 Recipe/CONFIG를 검사 입력으로 사용하지 않습니다.

## Focused Suites

- 레이아웃·문구·바인딩의 국소 변경은 해당 target과 공유 스타일의 대표 소비자를 선택합니다.
- 라우팅, 파라미터, Preview, Recipe 전환은 해당 정상/실패/취소/복귀 계약까지 포함합니다.
- 자동 미리보기라는 suite 이름만으로 기본 실행 정책을 판단하지 않습니다.
  도구별 opt-in·티칭 미리보기와 명시적 검사를 현재 계약·테스트에서 구분합니다.
- 과거 이름이 유지된 target은 함수의 실제 assertion을 확인합니다.
  예를 들어 레이어 탭 표시와 사용자가 요청한 비교 분할은 다른 행동입니다.
- 넓은 UI 변경이나 여러 focused 실패로 영향 범위가 불명확할 때만 전체 검사를 선택합니다.
  성능·대형 이미지 검사는 관련 요구나 측정 근거가 있을 때만 실행합니다.

## Review Checklist

해당 UI의 안정 계약과 다음 항목에 따라 검사 범위를 선택합니다.

- 현재 소스의 in-process View 렌더링, 실제 EXE 조작, 독립 사용자 관찰은 서로 다른 증거입니다.
- 실제 EXE 증거는 저장소의 clean-runtime 규칙으로 빌드 identity를 확인하고,
  실행 시 모니터·창 위치를 확인하고 기록한 뒤 얻습니다.
- 바뀐 컨트롤의 정상/hover/실제 pressed/focus/선택/disabled/오류와 popup 상태,
  대표 긴 입력, 키보드·명령·양방향 binding, 지원 theme/layout/DPI를 해당 범위에서 확인합니다.
- OpenGL 영역이 이미지에 보이지 않으면 sidecar 수치만으로 가독성·배치가 검증됐다고 하지 않습니다.
  sidecar는 보조 진단이며 실제 화면 확인이 남았음을 기록합니다.
- 변경 전후 증거와 실행본을 명시하고, 과거 자료는 기준선이라고 표시합니다.
  성공 흐름만으로 오류 복구·저장/재열기·다른 화면의 회귀까지 통과했다고 하지 않습니다.

## Development Gate

관련 focused 검사 실패는 그대로 남깁니다. fixture/도구 제한과 제품 결함을 구분하되
assertion이나 합격 기준을 완화하지 않습니다. 새로운 결함과 미검증 경계는 해당 변경의 검증 결과에
기록하고, 실제 실행한 검사·증거 위치·안 한 검사를 구분해 종료합니다.
소스 검토만 했다면 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`로 기록합니다.

전체 검사나 별도 platform gate가 필요한 경우에는
[UI precheck](../../tools/RunUiPrecheck.ps1)와
[platform precheck](../../tools/RunVisionPlatformPrecheck.ps1)의 현재 옵션을 확인합니다.
이 문서는 전체 검사나 제품 UI 변경을 새로 승인하지 않습니다.
