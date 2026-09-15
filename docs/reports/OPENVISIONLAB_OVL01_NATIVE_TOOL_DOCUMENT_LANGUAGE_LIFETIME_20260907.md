# OVL-01 네이티브 Tool 문서 언어 이벤트 수명 리팩토링

작성일: 2026-09-07 KST  
상태: **Complete** (OVL-01 독립 slice)

## 범위

`OpenVisionNativeToolDocument`가 성공한 Preview마다 `OpenVisionLanguageService.LanguageChanged`를 다시 구독하던 경로를 제거했다. 문서 생성 시의 최초 구독과 `Dispose()` 시의 해제는 기존 owner와 순서를 유지한다. 화면 배치나 Preview/Run 명시 동작은 변경하지 않았다.

변경 파일:

- `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativeToolDocument.cs`
- `tools/PipelineViewerScreenshotSmoke/Program.cs`

## 구조 증거

리팩토링 전 흐름은 다음과 같았다.

```text
Document 생성 → LanguageChanged += handler
성공 Preview → LanguageChanged += handler (반복 추가)
Dispose      → LanguageChanged -= handler (1회 해제)
```

따라서 한 문서의 성공 Preview가 반복될수록 static event invocation list에 같은 문서 handler가 누적되고, 단일 `Dispose()`가 그 중 하나만 제거할 수 있었다.

현재 흐름은 다음과 같다.

```text
Document 생성 → LanguageChanged += handler (1회)
성공 Preview → 상태·레이어 갱신만 수행
Dispose      → LanguageChanged -= handler (1회)
```

즉, 성공 Preview 경로에서 수명 관리 책임을 제거하고 문서 생성/해제 경계에만 남겼다. 현재 소스 검색 결과는 `OpenVisionNativeToolDocument.cs` 안에서 해당 이벤트의 `+=` 1곳과 `-=` 1곳이며, 성공 경로의 추가 구독은 없다.

## 합격 기준과 결과

| 기준 | 결과 | 근거 |
| --- | --- | --- |
| 동일 HSV 문서에서 성공 Preview 100회 후 handler가 1개로 유지 | 통과 | `wpf_native_tool_document_language_lifetime=OK` |
| 첫 Preview와 반복 Preview의 실행 수가 정확히 100 증가 | 통과 | 전용 smoke가 `NativePreviewRunCount`를 검사 |
| 언어 전환이 Preview/Run을 추가 실행하지 않음 | 통과 | 전용 smoke가 언어 변경 전후 실행 수를 비교 |
| 호스트 종료 후 해당 문서 handler가 0개 | 통과 | 전용 smoke가 `CaptureWindowWithContent` 종료 후 invocation list를 검사 |
| 기존 Native Tool 화면 smoke 유지 | 통과 | `wpf_shell_host_native_tool=OK` |

## 검증

Focused build:

```text
dotnet build .\tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
```

결과: 오류 0개. `tools/PipelineViewerScreenshotSmoke/Program.cs:10708`의 기존 nullable 경고 1개만 남았다.

Runtime smoke는 `OPENVISIONLAB_DATA_ROOT`를 별도 D: 테스트 루트로 지정하고 prewarm을 비활성화하지 않은 상태에서 실행했다.

```text
dotnet .\tools\PipelineViewerScreenshotSmoke\bin\x64\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll --target wpf_native_tool_document_language_lifetime <D:\...\after-fix-current>
dotnet .\tools\PipelineViewerScreenshotSmoke\bin\x64\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll --target wpf_shell_host_native_tool <D:\...\after-fix-native-tool>
```

before/after 비교도 남겼다. 수정 전 전용 smoke는 `Runs=0->100, Subscribers=14->209`로 실패했고, 수정 후에는 같은 target이 `OK`로 종료했다.

증거:

- [수정 전 실패 로그](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl01-20260907/before-fix.log)
- [수정 후 수명 smoke 로그](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl01-20260907/after-fix-current.log)
- [수정 후 기존 Native Tool smoke 로그](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl01-20260907/after-fix-native-tool.log)
- [수정 후 수명 smoke 화면](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl01-20260907/after-fix-current/wpf_native_tool_document_language_lifetime.png)
- [수정 후 기존 Native Tool 화면](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl01-20260907/after-fix-native-tool/wpf_shell_host_native_tool.png)

## 경계

이번 검증은 직접 WPF smoke runner를 통한 Native Tool 수명·실행 회귀 확인이다. 전체 theme/Wide·Compact/DPI 매트릭스, 실제 desktop EXE 모니터 배치, 장시간 GC 수거 계측은 수행하지 않았다. 화면 레이아웃을 변경하지 않은 소스/수명 slice이므로 해당 범위는 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`로 남긴다.

기존 Dev worktree의 미커밋 변경은 보존했고 `C:\Git\2D\Original`은 수정하지 않았다. commit/push는 수행하지 않았다.

다음 프로젝트 우선순위는 OVL-03 로그 수집 경계 검토다.  
Recommended model: `gpt-5.6-terra`  
Reasoning effort: `medium`
