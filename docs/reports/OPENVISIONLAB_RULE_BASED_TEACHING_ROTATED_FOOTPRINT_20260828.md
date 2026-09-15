# OpenVisionLab Matching 회전 footprint 검증

검증일: 2026-08-28 KST
상태: `Complete` (019 Matching Step 1 표시·각도 진단 범위)
범위: `Blob` 결과와 전체 Pipeline 판정은 제외하고, Die Pad `019`의 Matching
Step 1 후보 위치·회전 표시·물리적 패턴 대응만 검토했다.

## 사용자 의도

019의 초록색 검출 박스가 패턴의 하단·우측 외곽을 실제 기울기까지 반영해
감싸야 한다. Score만으로 양호하다고 판단하지 않고, 두 Pad와 Hole 순서,
L자 Trace, 우측 수직 Trace 및 상대 외곽 위치가 일치하는지를 우선한다.

## 변경

- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - `DrawRectangleOverlay`가 `VisionRecipeOverlaySummary.Angle`을 무시하고
    축 정렬 `Cv2.Rectangle`만 그리던 문제를 수정했다.
  - 각도 값이 유효하고 0이 아니면 보고된 중심·Bounds·각도로 4개 꼭짓점을
    계산해 `Cv2.Polylines`로 회전 footprint를 그린다.
  - 각도가 0이거나 비정상이면 기존 축 정렬 사각형으로 안전하게 유지한다.
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\SKILL.md`
  - 비영(非零) 각도/배율 Matching은 `RotatedRect` 또는 4점 footprint를
    증거에 포함해야 하며, 축 정렬 Bounds만으로는 물리 대응을 승인하지 않는
    규칙을 추가했다.
  - 회전 이의가 있을 때 고정된 대표 샘플에서만 전역 fine-angle 진단을 하고,
    물리 edge/trace 대응을 Score보다 먼저 보도록 규정했다.
- 같은 규칙을 `references/visual-correspondence-review.md`와
  `references/detection-point-contract.md`에 반영했다.

## 증거

### 회전 footprint 렌더링

동일한 A 레시피와 122장 corpus를 최신 `Any CPU` Runner DLL로 실행했다.

- 실행 결과: `BatchRows=122`, `BatchCompleted=122`,
  `BatchPipelinePasses=49`, `BatchMissingImages=0`
- Pipeline SHA-256: `74DE3E2EF0C1212C72EB88ED7F73EC4221A29921D40380D0CB3458C9737ECC81`
- 최신 증거 CSV:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-rotated-overlay-20260828-rerun04\evidence\evidence_rows.csv`
- 증거 CSV SHA-256: `75F9A7AE7FA8C570A55292F032A30CAA660802057BC6A1451C0DF84A029F06F2`
- Batch CSV SHA-256: `D3912B1A086AEE7734D1F32FF4E77A84A6C457EB58106D428121A21945816F2A`
- 019 Step 1 오버레이:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-rotated-overlay-20260828-rerun04\evidence\runs\OK_die_pad_019_ok\01_matching_overlay.png`
- 019 Step 1 오버레이 SHA-256:
  `F7028D8C022F907294BD33C69E616472801E0EC1A73D3088FABD17CEF60BA9F5`
- 188 reference exemplar 오버레이:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-rotated-overlay-20260828-rerun04\evidence\runs\NG_die_pad_188_ng\01_matching_overlay.png`
- 188 오버레이 SHA-256:
  `FDC94C45620C7DA343F1BF8E2EAD787FB4A4BFA57356DFE3AE19AC8D4B8B28CC`
- 019 전/후 확대 비교:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-rotated-overlay-20260828-rerun04\comparison\019_rotated_overlay_before_after.png`
- 전/후 비교 SHA-256:
  `9C5102404599397677C0ABE2F85827493F40E39C6546F589C453720B3D6CFAA7`

019 Step 2의 보고 pose는 중심 `(321,242)`, `Angle=-0.5°`, `Scale=1.05`,
Score `90.742617845535`였다. 019의 하단 초록선은 기존 축 정렬 표시에서
좌·우 평균 y 차이 `0 px`였고, 각도 반영 후 약 `1.7 px`(반올림 `2 px`)가
되었다. 이는 `Angle=-0.5°` pose가 화면 footprint에
반영되었음을 확인하는 렌더링 증거다. 작은 각도이므로 전체 512×512 화면에서는
변화가 미세하며, 확대 비교 이미지를 함께 보존했다.

### 고정 전역 0.1° 진단

템플릿·검색 ROI·배율 범위·Score floor·기타 gates를 그대로 두고
`FIND_ANGLE=0.1`로 복사한 진단 XML을 사용했다. per-image 조정은 하지 않았다.

| 이미지 | 기존 전역 설정 | 0.1° 진단 | 해석 |
|---|---:|---:|---|
| `die_pad_019_ok.jpg` | `-0.5° / 90.7426` | `-0.4° / 90.6152` | 더 회전해도 개선 근거 없음; 기존 값 유지 |
| `die_pad_188_ng.jpg` | `0° / 93.2423` | `0.2° / 93.5339` | 미세 pose 변화는 관찰되지만 exemplar 판정은 물리 대응 우선 |
| `die_pad_149_ng.jpg` | `1.5° / 93.3334` | `1.6° / 93.1347` | 각도 증대가 일관된 개선 아님 |
| `die_pad_020_ok.jpg` | `0.5° / 94.9871` | `0.3° / 95.1626` | 샘플별 최적값이 달라 전역 per-image tuning 금지 |

진단 산출물:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-angle01-diagnostic-20260828-rerun02\pipeline-angle01.xml`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-angle01-diagnostic-20260828-rerun02\batch.csv`
- 진단 Batch CSV SHA-256: `2BE06FA768C3838346A0BE3B92793ED6A7E56447A8473B4C1AE6DF66CB7D3C12`

따라서 019에 대해 `FIND_ANGLE`을 임의로 더 크게 바꾸지 않았다. 확인된 개선은
각도-aware 증거 표시이며, 실제 레시피의 전역 각도 변경은 고정 corpus의 물리
대응 증거가 추가될 때만 별도 결정한다.

## 검증 명령

```powershell
dotnet build "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj" -c Debug -p:Platform="Any CPU"
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
python "C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py" "C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching"
```

결과: Runner 빌드 경고 0/오류 0, 솔루션 빌드 경고 0/오류 0, Skill validator
`Skill is valid!`.

## 경계

- 이번 변경은 증거 오버레이와 Teaching Skill 규칙만 변경했다. Matching 알고리즘,
  template, ROI, score gate, Blob, Pipeline XML의 전역 각도 값은 변경하지 않았다.
- `0.1°` 진단은 대표 4장에 대한 진단이며 전체 corpus qualification 또는 생산
  레시피 승인 근거가 아니다.
- EXE launch smoke, DPI/theme/monitor 검증, Release/배포, 원본 repository push는
  수행하지 않았다.
