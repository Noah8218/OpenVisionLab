# OpenVisionLab Rule-Based Teaching Skill — Die Pad 실사용 검증

Date: 2026-08-27 KST
Repository: `C:\Git\OpenVisionLab_Dev`
Status: **Complete — bounded Skill application evidence**
Skill: `openvisionlab-rule-based-teaching`

## 1. 검증 목적

새로 만든 Codex Skill이 실제 OpenVisionLab 샘플에서 다음을 수행하는지
확인했다.

- 안정적인 template/locator 기준점을 고르는가
- SourceFrame과 LocatorFrame을 섞지 않는가
- 고정 inspection ROI와 결정론적 tool path를 명시하는가
- LLM/Skill이 자동 회전·샘플별 튜닝·무증거 좌표를 만들지 않는가
- 현재 runtime drawing과 teaching plan이 같은 geometry를 가리키는가
- locator가 거부한 샘플을 성공으로 포장하지 않는가

이번 검증은 skill guidance와 현재 deterministic runner의 연결 검증이다.
새 알고리즘, WPF 변경, XML import, release, qualification은 포함하지 않았다.

## 2. 입력과 실행

입력 subset은 `E:\라벨테스트`의 다음 family다.

```text
E:\라벨테스트\EasyMatch_Die_Pad_500(1)\EasyMatch_Die_Pad_500
source_file=Die Pad 1.bmp
rows=122 (train 82 / val 27 / test 13; role labels OK 62 / NG 60)
```

현재 소스에서 다음 명령을 실행했다.

```text
dotnet build tools\LocatorRelativeBlobSkillSmoke\LocatorRelativeBlobSkillSmoke.csproj -c Debug

dotnet run --project tools\LocatorRelativeBlobSkillSmoke\LocatorRelativeBlobSkillSmoke.csproj \
  --no-build -c Debug -- \
  --locator-relative-blob-corpus-pilot \
  D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-e-die-pad-20260827 \
  D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-teaching-skill-die-pad-20260827-210908
```

빌드는 오류 없이 통과했고 nullable 경고 8개가 남았다. 실행 결과는 기존
native batch를 확인한 뒤 122행을 다시 실행하고, 유효한 locator만 packet을
만들어 CandidateId-only compile/replay를 수행했다.

## 3. Skill teaching decision

### Reference와 orientation

- Reference: `die_pad_001_ok.jpg`, 512 x 512
- Source SHA-256:
  `C2D57383408D4208A950A03FC2CEFA33DB47EEEB1F57CA597AFA1FDE718AF7F9`
- 현재 이미지는 수평/수직 기준 방향으로 보인다.
- coarse orientation은 `0°`를 provisional proposal로 기록했으며
  `orientationSelection=PENDING`이다. 자동으로 확정하지 않았다.
- 90/180/270 회전이나 원본 덮어쓰기는 하지 않았다.

### Template와 기준점

- Template ROI: `190,220,175,130`
- Template SHA-256:
  `FA9EE37A82EB4C0035CC6474627BF5A627B8A36EB39054A085A1347D3276D0B0`
- Reference template center proposal: `(277.5,285)` in `LocatorFrame`
- Current retained runtime candidate: `locator-1`, `(278,286)`, angle `1°`,
  scale `1`, bounds `(191,221,174,130)`
- Candidate score: `0.9488943219184875`
- Reported score margin: `94.88943219184875`

Template는 두 개의 오른쪽 Die Pad와 연결 trace를 함께 포함한다. 결함 표시,
글자, 단일 임의 pixel을 기준점으로 사용하지 않았다.

## 4. 결정론적 경로

```text
Matching(NUM_MATCH=2, score/margin gate)
  -> Matching(NUM_MATCH=1, publish LocatorFrame)
  -> RotateScale(NormalizeImage)
  -> Threshold(Binary, 170)
  -> Blob(fixed ROI 190,220,175,130, area 700..1300)
```

`ResultCount`는 현재 plan에서 measurement-only다. corpus의 OK/NG role label은
결함 판정 truth가 아니며, 이 실행으로 Good/Bad qualification을 주장하지 않는다.

## 5. 실행 결과

| 항목 | 결과 |
| --- | ---: |
| 전체 rows | 122 |
| hash-verified packets | 120 |
| compiled replay | 120 |
| replay outcome match | 120 / 120 |
| locator reject | 2 |
| runtime exception | 0 |
| Qualification | false |

Locator reject는 다음과 같다.

- `train_NG_die_pad_026_ng`
- `val_NG_die_pad_198_ng`

이 두 행은 packet/compile 대상에서 제외하고 reject 이유를 보존했다. native
overlay에 후보가 그려져 보여도 현재 skill/packet gate를 통과하지 않으면
성공으로 바꾸지 않는다.

## 6. 현재 teaching plan과 시각 근거

현재 bundle:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rule-based-teaching-skill-die-pad-20260827-210908\teaching-plan`

- `detection-point-teaching-plan.json`: Skill 출력 계약과 실제 좌표/해시/도구
  계획
- `teaching-report.md`: operator용 판단·다음 행동
- `reference-source.jpg`: reference source copy
- `taught-template.png`: template copy
- `reference-locator-overlay.png`: selected locator candidate/current bounds
- `boundary-ng-blob-overlay.png`: 같은 고정 ROI의 다른 NG-labelled sample
- `locator-rejected-boundary-overlay.png`: gate reject 경계 사례

plan JSON은 parse되고 다음을 다시 확인했다.

- Skill ID와 `READY_FOR_OPERATOR_REVIEW` 상태
- 5-Step tool plan
- orientation `0` + operator selection `PENDING`
- source/template/overlay 5개 SHA-256
- fixed ROI bounds와 LocatorFrame
- operator-owned gate와 measurement-only count

현재 Skill 출력은 operator 검토 전 proposal이다. operator가 orientation, template,
ROI, 실제 Good/NG gate를 확인한 뒤에만 기존 explicit Pipeline Review를 실행한다.

## 7. 남은 한계

- 이 실행은 `locator-relative-blob-v1` pilot이며 qualification이 아니다.
- 현재 accepted row는 한 후보만 보존하는 경우가 많아 reported ScoreMargin이
  실제 두 번째 후보와의 경쟁 margin임을 아직 증명하지 못한다.
- 초록 Matching overlay와 높은 score만으로 template-to-source 실제 구조
  대응을 확정할 수 없다. 대표 샘플 source patch/template/blend 재검토에서
  가림과 추가 구조가 확인된 행은 `WAIT`로 남긴다. 상세 결과는
  `OPENVISIONLAB_RULE_BASED_TEACHING_VISUAL_CORRESPONDENCE_REVIEW_20260827.md`
  에 기록한다.
- orientation `0°`는 Skill이 관찰한 provisional proposal이지 operator 승인값이
  아니다.
- Blob `ResultCount`는 측정값이며 defect truth가 아니다.
- Skill이 실제 provider/외부 LLM 호출을 수행하거나 XML을 자동 import하지 않는다.
- 제품 코드와 WPF UI는 이 검증에서 변경하지 않았다.
