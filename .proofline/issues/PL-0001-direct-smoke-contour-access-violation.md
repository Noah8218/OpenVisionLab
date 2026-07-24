---
{
  "id": "PL-0001",
  "type": "bug",
  "status": "resolved",
  "title": "Direct smoke Contour replay requires a clean current runtime",
  "discovered_while": "P130 local Bent Pin field-pilot candidate preparation",
  "description": "The retained bin\\Debug output contains legacy runtime files that make llm-xml-image-run terminate in OpenCvSharp FindContours. P132 proved the same freshly built OpenVisionLab executable passes the Contour Good and controlled-NG direct replays when built into a new empty runtime directory. tools\\BuildCleanRuntime.ps1 now creates that safe evidence runtime without deleting or modifying the existing bin\\Debug workspace.",
  "evidence": [
    {
      "kind": "reproduction",
      "location": "bin\\Debug\\OpenVisionLab.exe --smoke llm-xml-image-run",
      "note": "Both the app host and dotnet OpenVisionLab.dll terminated with -1073741819 in OpenCvSharp.NativeMethods.imgproc_findContours1_vector."
    },
    {
      "kind": "resolution",
      "location": "artifacts\\p132_direct_smoke_contour_host_20260719\\clean_runtime_script_final",
      "note": "The clean current runtime built by tools\\BuildCleanRuntime.ps1 passed llm-xml-image-run for the approved Bent Pin Good and controlled NG inputs; its manifest records managed/native runtime hashes."
    },
    {
      "kind": "control",
      "location": "artifacts\\p132_direct_smoke_contour_host_20260719\\control_wpf_contour_current_source",
      "note": "Current-source WPF Contour Tool and Pipeline Review controls passed."
    }
  ],
  "risk": "medium",
  "impact": "Contour-based LLM XML replay evidence can now use a fresh current runtime produced by the checked-in builder. The retained bin\\Debug directory remains unsuitable as deployment evidence until a separate output/package contract is approved; P132 did not delete or rewrite it.",
  "suggested_next_step": "Keep direct replay evidence on tools\\BuildCleanRuntime.ps1. Before changing the default output or deployment layout, obtain an explicit Dev/release package and legacy-workspace retention decision, then address it as PL-0002.",
  "completion_criteria": [
    "A minimal direct-smoke Contour repro completes without an access violation.",
    "The repro's Good and controlled NG results match the expected acceptance outcomes.",
    "The passing console and WPF Contour controls still pass after the fix."
  ],
  "linked_context": {
    "task": "P132 direct-smoke Contour runtime investigation",
    "files": [
      "OpenVisionLabDirectSmokeRunner.cs",
      "tools/BuildCleanRuntime.ps1",
      "docs/samples/BentPin_ShaftContour.pipeline.xml"
    ],
    "commands": [
      "powershell -NoProfile -ExecutionPolicy Bypass -File tools/BuildCleanRuntime.ps1 -OutputDir artifacts/<new-runtime>",
      "<new-runtime>/OpenVisionLab.exe --smoke llm-xml-image-run ..."
    ]
  },
  "work_log": [
    {
      "at": "2026-07-18T23:43:02+09:00",
      "status": "open",
      "summary": "Recorded the repeated direct-smoke crash and the passing controls without changing product or native-runtime code.",
      "evidence": [
        "OpenCvSharp.NativeMethods.imgproc_findContours1_vector access-violation stack from the direct-smoke repro",
        "P130 local Good/Bad console replay and WPF control artifacts"
      ]
    },
    {
      "at": "2026-07-19T00:30:24+09:00",
      "status": "resolved",
      "summary": "Confirmed that the access violation is specific to the retained polluted bin\\Debug output, then added a non-destructive clean-runtime builder and verified direct Contour Good/controlled-NG replay from its fresh output.",
      "evidence": [
        "artifacts/p132_direct_smoke_contour_host_20260719/clean_runtime_script_final/clean_runtime_manifest.json",
        "artifacts/p132_direct_smoke_contour_host_20260719/script_runtime_good/report.txt",
        "artifacts/p132_direct_smoke_contour_host_20260719/script_runtime_bad_expected_ng/report.txt",
        "artifacts/p132_direct_smoke_contour_host_20260719/control_wpf_contour_current_source/wpf_shell_host_contour_tool.png"
      ]
    }
  ],
  "resolved_evidence": [
    "tools/BuildCleanRuntime.ps1 creates a new empty artifacts runtime and refuses an existing output directory.",
    "Fresh current OpenVisionLab.exe passed the Bent Pin Good direct replay with ActualRunSuccess=True.",
    "Fresh current OpenVisionLab.exe passed the Bent Pin expected-NG direct replay with ActualRunSuccess=False and BoundsWidthMax=26 > 18.",
    "Current-source WPF Contour Tool and Pipeline Review controls passed during P132."
  ],
  "created_at": "2026-07-18T23:43:02+09:00",
  "updated_at": "2026-07-19T00:30:24+09:00"
}
---

# PL-0001 Contour 직접 재생용 깨끗한 현재 런타임

## 설명

보존된 `bin\Debug`에는 레거시 런타임 파일이 섞여 있어 `llm-xml-image-run`의 Contour 실행이 OpenCvSharp `FindContours` 접근 위반으로 끝났다. P132는 새 빈 출력 폴더에 빌드한 동일 소스의 `OpenVisionLab.exe`가 Good과 통제된 NG 직접 재생을 모두 통과함을 확인했다. `tools\BuildCleanRuntime.ps1`는 기존 `bin\Debug` 작업공간을 삭제하거나 수정하지 않고 이 검증용 런타임을 만든다.

## 근거

- 기본 `bin\Debug`와 `dotnet OpenVisionLab.dll` 재현은 모두 `OpenCvSharp.NativeMethods.imgproc_findContours1_vector`에서 `-1073741819`로 끝났다.
- 새 런타임의 Good 직접 재생은 `ActualRunSuccess=True`, `BoundsWidthMax=14`로 통과했다.
- 같은 런타임의 Bent NG 직접 재생은 `ActualRunSuccess=False`, `BoundsWidthMax=26 > 18`의 의도된 검사 NG로 스모크 자체는 통과했다.
- 현재 소스 WPF Contour Tool 및 Pipeline Review 제어 경로도 통과했다.

## 영향

Contour 기반 LLM XML 이미지 재생 근거는 이제 새 런타임 빌드 도구로 만들 수 있다. 기존 `bin\Debug`는 별도 배포·작업공간 계약이 결정되기 전까지 현재 런타임 또는 배포 근거로 사용하지 않는다.

## 다음 단계

직접 재생 근거에는 `tools\BuildCleanRuntime.ps1`를 사용한다. 기본 출력 또는 배포 레이아웃 변경은 개발/배포 패키지 위치와 기존 작업공간 보존 정책을 사용자가 결정한 뒤 PL-0002로 진행한다.

## 완료 기준

- 새 현재 런타임의 최소 직접 스모크 Contour 재현이 접근 위반 없이 끝났다.
- Good 및 통제된 NG 결과가 예상 수용 결과와 일치했다.
- 현재 소스 WPF Contour 제어 경로가 통과했다.

## 작업 기록

- 2026-07-18 23:43 KST: 직접 스모크 재현과 통과 제어 경로를 기록했다. 제품 또는 네이티브 런타임 코드는 변경하지 않았다.
- 2026-07-19 00:30 KST: 기본 출력의 오염을 원인으로 확인하고, 기존 출력에 손대지 않는 새 런타임 빌드 도구를 추가했다. 새 현재 EXE의 Good/통제된 NG 직접 재생과 WPF Contour 제어를 확인했다.

## 해결 근거

`tools\BuildCleanRuntime.ps1`가 새 출력 폴더와 manifest를 만들고, 그 EXE가 Good/통제된 NG 직접 재생을 통과했다. 기본 `bin\Debug`의 장기 배포 계약은 별도 PL-0002로 남긴다.

## 참조

- `tools\BuildCleanRuntime.ps1`
- `docs\OPENVISIONLAB_CURRENT_HANDOFF.md`
