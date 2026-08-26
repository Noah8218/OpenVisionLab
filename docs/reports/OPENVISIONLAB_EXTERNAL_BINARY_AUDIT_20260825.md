# OpenVisionLab 외부 바이너리 감사

Date: 2026-08-25 KST  
Repository: C:\Git\OpenVisionLab_Dev  
Branch: codex/public-sample-ux-docs  
HEAD used for the audit: 827a22e92eba94445e98d1143b94e8d3ea4619b7

## 목적과 범위

이번 감사는 PL-0005의 첫 번째 경계를 처리한다.

- dll 아래 추적된 DLL의 경로, 현재 작업트리 상태, 길이, SHA-256을 기록한다.
- 직접 참조, 확인된 동적 참조, clean runtime 복사 여부, publish 관찰 여부를 분리한다.
- 라이선스·정확한 바이너리 출처·재배포 notice가 확인되지 않은 파일은 삭제하지 않고 blocked로 분류한다.
- 새 DLL이 manifest에 등록되지 않으면 외부 참조 gate가 실패하도록 한다.

이 문서는 법률 자문이나 release 허가가 아니다. release admission은 각 항목의
releasePolicy와 별도 release gate를 함께 통과해야 한다.

## 기준선 분리

감사 시작 시 git status --short -- dll에서 확인된 DLL 관련 dirty 상태는 다음 세
파일의 삭제뿐이었다.

- dll/EmguCV/Emgu.CV.UI.dll
- dll/EmguCV/Emgu.CV.World.dll
- dll/EmguCV/cvextern.dll

이 삭제 상태는 기존 reliability bundle의 일부로 보존했다. git ls-files -v dll
검사에서 skip-worktree 또는 assume-unchanged 항목은 확인되지 않았다. 동반 XML
파일의 물리 바이트 길이는 text=auto 정규화 전후와 달랐지만 Git blob hash는
동일하여 이번 DLL 기준선에 변경으로 포함하지 않았다. PL-0003의 stale WPF smoke
assertion은 이번 감사와 섞지 않았다.

## 인벤토리 결과

OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json은 24개의 추적 DLL 경로를 기록한다.

- 현재 작업트리에 존재: 21개 DLL
- 현재 작업트리에서 삭제됨: 3개 legacy Emgu DLL
- 삭제된 3개도 HEAD blob 기준 길이와 SHA-256을 manifest에 기록함
- 현재 dll 물리 파일 전체: 25개
  - DLL 21개
  - 동반 XML 3개
  - SDK provenance JSON 1개
- 현재 manifest 분류:
  - runtime-required: 9개
  - runtime-conditional: 1개
  - direct-reference runtime 후보: 2개
  - repository-only: 9개
  - legacy-forbidden: 3개

상세 경로, 길이, SHA-256, 참조, license evidence, release 정책은 다음 manifest가
기계 판독 가능한 기준이다.

docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json

### 현재 runtime에 복사된 항목

현재 소스에서 새로 생성한 Debug clean runtime은 다음 manifest 항목을 실제로
복사했다.

- OpenVisionLab-Vision-SDK의 SDK 3.0 관리 DLL 5개
- dll/OpenCVSharp/OpenCvSharpExtern.dll
- dll/System.Windows.Controls.WpfPropertyGrid.dll
- dll/System.Windows.Controls.WpfPropertyGrid.xml
- dll/SharpGL/SharpGL.dll
- dll/SharpGL/SharpGL.WinForms.dll
- dll/FontAwesome.Sharp.dll
- dll/Vila.Core.dll
- dll/OpenCVSharp/opencv_ffmpeg400_64.dll

Vila.Core.dll은 현재 src/OpenVisionLab/OpenVisionLab.csproj의 직접 참조로
복사되지만 소스 코드 사용처는 확인되지 않았다. FontAwesome.Sharp.dll도 직접
참조와 runtime 복사는 확인되었지만 소스 심볼 사용처는 확인되지 않았다. 두 파일의
참조 제거는 별도 clean build·EXE smoke·release publish 실험으로 분리한다.

opencv_ffmpeg400_64.dll은 OpenVisionLab.ImageCanvas.csproj의 조건부 Content이며
현재 기본값이 true이므로 clean runtime에 포함된다. 제품 방향상 카메라·비디오
플랫폼은 범위 밖이고, 이 exact binary의 build/FFmpeg notice/재배포 근거는 현재
확정되지 않았으므로 blocked로 남겼다.

### 현재 clean runtime에 복사되지 않은 후보

다음 항목은 현재 source build의 clean runtime에 포함되지 않았다.

- CircularProgressBar.dll
- Cyotek.Windows.Forms.ImageBox.dll
- MaterialDesign/MaterialDesignColors.dll
- MaterialDesign/MaterialDesignThemes.Wpf.dll
- Matrox.MatroxImagingLibrary.dll
- SharpGL/SharpGL.SceneGraph.dll
- TabControl.dll
- WinFormAnimation.dll

MaterialDesign converter의 유일한 source hit는 현재 compile에서 제외된 파일에
있다. 이를 unused로 확정하여 삭제하지는 않았다. SharpGL.SceneGraph.dll도
현재 직접 참조와 clean runtime 복사가 확인되지 않았지만, SharpGL dependency
graph를 별도 확인하기 전에는 삭제하지 않는다. Matrox, EzBasicAxl, TabControl,
CircularProgressBar, WinFormAnimation은 현재 source/runtime 참조와 재배포
근거가 모두 부족하다.

## 라이선스와 재배포 판단

공식 upstream 페이지에서 확인된 permissive license는 manifest에 URL로 기록했다.

- FontAwesome.Sharp: Apache-2.0
- SharpGL: MIT
- Cyotek ImageBox: MIT
- MaterialDesignInXAMLToolkit: MIT
- OpenCvSharp: Apache-2.0

이 확인은 upstream 프로젝트의 license 존재를 확인한 것이며, 저장소에 있는
정확한 prebuilt DLL이 해당 source/version에서 생성되었다는 provenance를 자동으로
증명하지 않는다. 다음 항목은 현재 근거가 부족하여 release를 막는다.

- WPG-CUSTOM prepared DLL/XML의 정확한 source/license/redistribution terms
- Vila.Core.dll의 source/license
- opencv_ffmpeg400_64.dll의 exact build와 FFmpeg notice
- EzBasicAxl.dll, Matrox.MatroxImagingLibrary.dll, TabControl.dll,
  CircularProgressBar.dll, WinFormAnimation.dll의 source/license
- repository-only open-source 후보의 exact binary provenance
- root NOTICE의 third-party binary별 attribution 완결 여부

현재 root NOTICE는 OpenVisionLab attribution을 담고 있지만, 위 외부 DLL별
attribution 목록을 완결한 third-party notice로 보지 않는다.

## Gate 변경과 결과

변경 파일:

- tools/TestExternalReferences.ps1
- docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json
- docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_REFERENCE_POLICY.md

외부 참조 gate는 이제 다음을 검사한다.

1. manifest JSON schema와 중복 경로
2. 모든 추적 DLL 경로의 manifest 등록
3. dll 아래 실제 DLL의 manifest 등록
4. 현재 존재하는 등록 DLL의 길이와 SHA-256
5. forbidden DLL 재도입
6. required/allow 항목의 누락

알려진 blocked 항목은 현재 repository 분류 결과로 출력하지만, 그것만으로
기존 Dev 작업트리를 삭제하거나 깨뜨리지는 않는다. blocked 항목을 release output에
포함시키는 것은 별도 release gate에서 허용되지 않아야 한다.

D: staging mutation test는 현재 dll tree를 복제한 뒤 manifest에 없는
dll/Unexpected.dll을 추가했다. 이 staging에서 gate는 exit code 1로 종료했고,
유일한 핵심 failure는 "Unallowlisted DLL under dll root:
dll\Unexpected.dll"이었다. Dev repository에는 이 테스트 파일을 만들지 않았다.

## 현재 검증

실행한 명령과 결과:

    powershell -NoProfile -ExecutionPolicy Bypass -File tools\BuildCleanRuntime.ps1 -Mode Dev -Configuration Debug -Platform AnyCPU -OutputDir artifacts\pl0005_binary_audit_20260825\clean_runtime
    PASS: 0 warnings, 0 errors

    powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1 -Configuration Debug -OutputPath artifacts\pl0005_binary_audit_20260825\external_references.txt
    PASS: manifest/hash/forbidden/unallowlisted checks

    dotnet publish src\OpenVisionLab\OpenVisionLab.csproj -c Release -p:Platform=AnyCPU -r win-x64 --self-contained false -p:PublishDir=D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_binary_audit_20260825\release_publish\
    PASS: fresh framework-dependent publish artifact created; 72 files and OpenVisionLab.exe present
    Observed blocked payload: Vila.Core.dll, opencv_ffmpeg400_64.dll
    Observed forbidden legacy payload: none

실제 artifact는 repository의 기존 junction을 통해 다음 D: 경로에 물리적으로
생성되었다.

D:\OpenVisionLab_Data\Dev\artifacts\pl0005_binary_audit_20260825\clean_runtime

Fresh Release publish artifact:

D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_binary_audit_20260825\release_publish

Release publish identity checks for OpenVisionLab.exe, Vila.Core.dll,
opencv_ffmpeg400_64.dll, OpenCvSharpExtern.dll, and
System.Windows.Controls.WpfPropertyGrid.dll matched the current manifest where
applicable. This is publish evidence, not release-candidate approval.

현재 dist/OpenVisionLab은 이 감사의 증거로 사용하지 않았다. 그 디렉터리의
manifest는 2026-07-19 산출물이며 cvextern.dll, Lib.Common.dll,
Lib.OpenCV.dll, Lib.OpenCV.Blob.dll, OpenCvSharp.Extensions.dll 등을 포함한
이전 layout의 stale output이다. 삭제·덮어쓰기는 수행하지 않았다.

## PL-0005 판정

    Status: Incomplete
    Scope: 현재 Dev 작업트리의 dll inventory, hash identity, 참조·복사 관찰, 외부 DLL allowlist gate
    Acceptance criteria:
      C1 inventory/length/SHA/reference/build-publish/license record -> PASS for the current classified scope; unresolved license/provenance entries are explicit BLOCKED
      C2 retained binary notice/license evidence -> BLOCKED by WPG, Vila, FFmpeg, proprietary/unknown artifacts, and incomplete third-party NOTICE
      C3 deletion/clean release publish proof -> PARTIAL; fresh Release publish passed to the D: artifact, but release admission remains blocked by known blocked payloads and unresolved notices
      C4 new unallowlisted DLL gate -> PASS; D: staging mutation test exited 1 for dll/Unexpected.dll
    Verification: Debug clean runtime build passed 0 warnings/0 errors; TestExternalReferences passed; fresh framework-dependent Release publish produced 72 files with no forbidden legacy payload; no tag, commit, push, or release publication performed
    Evidence: docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json; artifacts/pl0005_binary_audit_20260825/clean_runtime/clean_runtime_manifest.json; artifacts/pl0005_binary_audit_20260825/external_references.txt; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_binary_audit_20260825\release_publish
    Boundary / next dependency: resolve WPG/Vila/FFmpeg/proprietary binary provenance and third-party notice decisions, then run the repository distribution gate before closing PL-0005

## 다음 우선순위

1. PL-0005 blocker resolution and fresh Release publish evidence | Recommended model: gpt-5.6-luna | Reasoning effort: high
2. PL-0006 BitmapImageConverter failure-safe boundary | Recommended model: gpt-5.6-luna | Reasoning effort: high
3. PL-0007 path boundary audit | Recommended model: gpt-5.6-luna | Reasoning effort: high

PL-0003은 현재 reliability bundle과 분리된 상태로 유지한다. P256과 CVR-00의
기존 product-priority 상태는 변경하지 않았다.
