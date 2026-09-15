# OVL-03 Runtime 로그 버퍼와 UI 소비량 리팩토링

작성일: 2026-09-07 KST  
상태: **Complete** (OVL-03 독립 slice)

## 범위

실행 로그의 화면용 sink에 메모리 상한과 소비 배치 상한을 도입했다. 파일 appender와 기존 로그 파일 형식은 건드리지 않아 영속 로그 경로와 화면용 버퍼 정책을 분리했다.

변경 파일:

- `src/Libraries/OpenVisionLab.Logging/Model/RuntimeLogSink.cs`
- `src/Libraries/OpenVisionLab.Logging/Model/RuntimeLogStream.cs`
- `src/Libraries/OpenVisionLab.Logging.Controls/ViewModel/LogPanelViewModel.cs`
- `tools/PipelineViewerScreenshotSmoke/Program.cs`

## 구조 증거

이전에는 `RuntimeLogSink.Append`가 `ConcurrentQueue<string>`에 제한 없이 추가하고 `ReadBuffer()`가 전체 큐를 하나의 문자열로 합친 뒤 `RuntimeLogStream.GetLogs()`가 다시 줄 단위로 분할했다.

현재는 다음 경계로 바뀌었다.

```text
log4net event
  → RuntimeLogSink: 최대 4,096개 / 1,048,576자 화면 버퍼
  → ReadEntries: 호출당 최대 250개
  → RuntimeLogStream.GetLogs: 문자열 재분할 없이 항목 배열 반환
  → LogPanelViewModel: 250개 단위로 UI 반영
```

상한을 넘으면 오래된 화면 항목을 제거하고 `DroppedLogCount`를 누적한다. 한 항목이 문자 상한보다 크면 해당 항목을 화면 버퍼에 넣지 않고 같은 카운터에 기록한다. `LogPanelViewModel.SummaryText`는 기존 로그 수와 함께 생략 건수를 표시한다.

## 합격 기준과 결과

| 기준 | 결과 | 근거 |
| --- | --- | --- |
| 병렬 producer에서도 화면 버퍼가 항목 상한을 넘지 않음 | 통과 | `logging_buffer_contract=OK`에서 8개 producer가 8,000건을 공급하고 drain 결과를 상한과 비교 |
| 소비 재개 시 전체 큐를 한 번에 처리하지 않음 | 통과 | `ReadEntries()` 배치가 250건 이하인지 검사 |
| 문자 상한을 넘는 큰 단일 메시지가 생략 건수에 반영됨 | 통과 | 1MB 초과 메시지 입력 후 `DroppedLogCount` 증가 검사 |
| UI에서 생략 건수가 보임 | 통과 | `LogPanelViewModel.DroppedLogText` 검사와 화면 증거 |
| 동시 Dispose 이후 runtime stream이 다시 읽히지 않음 | 통과 | 16개 동시 Dispose 후 `GetLogs()`/`GetLog()` 빈 결과 검사 |
| 기존 로그 패널과 Native Tool 경로 회귀 없음 | 통과 | `log_panel_contract_check`, OVL-01 수명 smoke, `wpf_shell_host_native_tool` |

## 검증

Focused build:

```text
dotnet build .\tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
```

결과: 오류 0개. `tools/PipelineViewerScreenshotSmoke/Program.cs:10713`의 기존 nullable 경고 1개만 남았다.

Runtime smoke는 `TEMP`, `TMP`, `OPENVISIONLAB_DATA_ROOT`를 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl03-20260907` 아래로 지정했다.

```text
logging_buffer_contract=OK|check=OK|...
log_panel_contract_check=OK|check=OK|...
wpf_native_tool_document_language_lifetime=OK|check=OK|...
wpf_shell_host_native_tool=OK|check=OK|...
```

증거:

- [로그 버퍼 contract 로그](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl03-20260907/logging-buffer-final-9.log)
- [로그 패널 기존 contract 로그](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl03-20260907/log-panel-existing-final-10.log)
- [OVL-01 수명 회귀 로그](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl03-20260907/native-lifetime-final-11.log)
- [Native Tool 회귀 로그](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl03-20260907/native-tool-final-11.log)
- [생략 건수 표시 화면](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl03-20260907/logging-buffer-final-9/logging_buffer_contract.png)

## 경계

화면용 sink의 상한은 현재 UI 안정성 정책이다. 영속 파일 로그의 오류 보존, 프로세스 kill/restart 복구, 장시간 메모리 계측은 이번 slice에서 새로 검증하지 않았다. WPF smoke는 정상 상태의 실제 렌더링과 배치·생략 문구를 확인했지만 전체 theme/Wide·Compact/DPI 매트릭스와 desktop EXE 모니터 검증은 수행하지 않았다.

기존 Dev worktree의 미커밋 변경은 보존했고 `C:\Git\2D\Original`은 수정하지 않았다. commit/push는 수행하지 않았다.

다음 프로젝트 우선순위는 OVL-02 공개 TCP identity와 중복 실행 진입 검증이다.  
Recommended model: `gpt-5.6-terra`  
Reasoning effort: `high`
