# OpenVisionLab Matching scale 검증·성능 후속 연구 — Die Pad 188 r2

## 상태

`Complete` — 사용자가 요청한 범위인 (1) 신규 해시 기반 NG-only held-out
sanity check와 (2) 동일 500장에 대한 `FIND_SCALE_STEP=0.10` 성능 후보
실행 및 비교를 완료했습니다. `qualification=false`이며, 제품 기본값·배포
승인을 의미하지 않습니다.

## 결정 요약

- 정확도 우선 후보는 계속 `FIND_SCALE_MIN=0.90`, `MAX=1.10`,
  `STEP=0.05`로 유지합니다.
- `STEP=0.10`은 평균 실행 시간을 `3,754.370 ms`에서 `2,441.768 ms`로
  `34.96%` 줄였지만, 평균 `ScoreMax`가 `93.084`에서 `91.548`로
  `1.536`점 낮아졌고 `ScoreMax >= 90` 행이 `389`에서 `340`으로
  감소했습니다.
- 두 후보 모두 `500/500` 실행 성공, `ResultCount=1`, `StepStatus=OK`였지만,
  결과 이미지 `142/500`이 달라졌습니다. 따라서 실행 상태가 녹색으로
  남는 것만으로 box geometry가 보존됐다고 볼 수 없습니다.
- 결론적으로 `STEP=0.10`은 전역 정확도 기본값으로 `AUTO_REJECTED`하고,
  추후 명시적인 속도 예산·geometry 허용오차가 승인될 때만 별도 성능
  프로파일로 재검토합니다.

## 고정 입력과 중복 감사

기존 선택 corpus와 동일한 `500`장 목록(`NG 250 + OK 250`), 사용자 승인
템플릿, ROI, angle search, matcher, score floor를 고정했습니다.

| 항목 | 값 |
| --- | --- |
| Template | `die_pad_188_material_only_expanded_r2.png` |
| Template SHA-256 | `DBF8E0A0470B24F80162FF6128C0B07A9A9C28B777C843B709CFFC38146574E6` |
| Locked ROI | `(169,172,188,142)`, teaching angle `0°` |
| Matcher | `CCoeffNormed`, `SCORE_MIN=0.60`, `NUM_MATCH=3` |
| Angle search | `-10..+10°`, step `0.1°` |
| Existing 500 image-list SHA-256 | `9243089DF5E4CA7E2FEB13C569CED961856A69587DC8E37E647B9645B2770D0B` |

`E:\라벨테스트`의 die-pad `all_images`는 총 `550`장입니다. 기존 500장과
SHA-256을 대조한 결과, 외부 corpus의 신규 content는 NG `25`장뿐이었고
외부 OK `25`장은 기존 500장과 내용이 중복되었습니다. 따라서 OK 중복을
독립 held-out으로 가장하지 않았습니다.

- 전체 감사: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\all-images-hash-audit.csv`
- 감사 CSV SHA-256: `1D433CC5EF72EB3A6D0DDCABCFC2EDFFFF798E26FC19B4DAF3ECC6CA64EF9ABF`
- 신규 NG 목록: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\heldout-novel-ng-25-20260829\image-list-25.txt`
- 신규 NG 목록 SHA-256: `D1F6355E64DF6E9487369F7064D6556A4C20B1CA4E6F9E1B8B441EC1AEB9D653`

## 신규 NG 25장 sanity check

선택된 정확도 후보(`STEP=0.05`)를 신규 content NG 25장에 실행했습니다.
이는 NG-only이므로 full held-out qualification이 아니라 독립 content에 대한
sanity check입니다.

| 항목 | 결과 |
| --- | ---: |
| Rows | 25 |
| Pipeline/Step 성공 | 25/25 |
| Error | 0 |
| ResultCount=1 | 25/25 |
| 평균 ScoreMax | 92.887 |
| ScoreMax 최소/최대 | 73.509 / 98.874 |
| ScoreMax >= 90 | 20/25 |
| 평균 elapsed | 4,380.993 ms |
| 중앙값 / p95 / 최대 | 4,165.848 / 5,510.713 / 5,548.104 ms |

현재 실행에서 다음 overlay를 열어 확인했습니다. 모두 두 Pad, hole 순서,
L자 trace, 우측 vertical trace, 하단/우측 물리 경계에 박스가 대응했고,
낮은 score라는 이유만으로 위치가 틀렸다고 판정하지 않았습니다.

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\heldout-novel-ng-25-20260829\evidence\runs\runs\NG_die_pad_001_ng\01_matching_overlay.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\heldout-novel-ng-25-20260829\evidence\runs\runs\NG_die_pad_003_ng\01_matching_overlay.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\heldout-novel-ng-25-20260829\evidence\runs\runs\NG_die_pad_007_ng\01_matching_overlay.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\heldout-novel-ng-25-20260829\evidence\runs\runs\NG_die_pad_010_ng\01_matching_overlay.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\heldout-novel-ng-25-20260829\evidence\runs\runs\NG_die_pad_020_ng\01_matching_overlay.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\heldout-novel-ng-25-20260829\evidence\runs\runs\NG_die_pad_025_ng\01_matching_overlay.png`

Evidence row CSV:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\heldout-novel-ng-25-20260829\evidence\runs\evidence_rows.csv`

SHA-256: `A86EF964E1631756B23D15C11643694403940EBFC391D7A22F3F35BED8CE347B`.
Source/result/overlay는 각각 `25/25`개이고 source 및 overlay path와 hash가
서로 유일하며, source hash mismatch는 `0`개였습니다.

## `STEP=0.10` full-corpus 성능 후보

후보 pipeline은 정확도 후보에서 scale step만 바꿨습니다.

| 후보 | Pipeline SHA-256 | Image-list SHA-256 |
| --- | --- | --- |
| `0.90..1.10 / 0.10` | `824A5BE8820F32CF9405AAFD468A0195B222BF66AFC82529CAB5B37C750A5692` | `9243089DF5E4CA7E2FEB13C569CED961856A69587DC8E37E647B9645B2770D0B` |

실행 evidence:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-0p90-1p10-step0p10-full-20260829`

| 지표 | 정확도 후보 `STEP=0.05` | 성능 후보 `STEP=0.10` | 변화 |
| --- | ---: | ---: | ---: |
| Rows / pipeline pass | 500 / 500 | 500 / 500 | 동일 |
| StepStatus / ResultCount | OK 500 / 1 500 | OK 500 / 1 500 | 동일 |
| 평균 ScoreMax | 93.084 | 91.548 | -1.536 |
| ScoreMax >= 90 | 389 | 340 | -49 |
| 평균 elapsed | 3,754.370 ms | 2,441.768 ms | -34.96% |
| 중앙값 elapsed | 3,735.557 ms | 2,263.956 ms | -39.38% |
| p95 elapsed | 4,214.527 ms | 3,671.325 ms | -12.88% |
| 최대 elapsed | 6,579.764 ms | 4,385.455 ms | -33.35% |

행 단위 비교 결과:

- `358/500`은 ScoreMax와 결과 이미지 hash가 동일했습니다.
- `132/500`은 성능 후보 ScoreMax가 낮아졌고, `10/500`은 높아졌습니다.
- 결과 이미지 hash는 `142/500`개가 달라졌습니다.
- 두 후보 사이 pipeline status, step status, result count, error code의
  변화는 `0`개였습니다.

실제 overlay의 green-pixel extent도 일부 변경되었습니다. 이 값은 자동
정답 oracle이 아니라 렌더링된 geometry 변화의 보조 증거입니다.

| 샘플 | `STEP=0.05` extent | `STEP=0.10` extent |
| --- | --- | --- |
| `OK_die_pad_079_ok` | `195,193..385,349` | `201,195..379,344` |
| `OK_die_pad_004_ok` | `180,209..362,361` | `185,212..357,356` |
| `NG_die_pad_240_ng` | `152,144..344,302` | `157,146..339,299` |
| `NG_die_pad_219_ng` | `165,177..345,327` | `169,180..341,325` |
| `OK_die_pad_041_ok` | `222,201..404,352` | `229,203..399,348` |

대표 성능 후보 overlay는 다음 current-run 파일입니다.

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-0p90-1p10-step0p10-full-20260829\evidence\runs\runs\NG_die_pad_188_ng\01_matching_overlay.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-0p90-1p10-step0p10-full-20260829\evidence\runs\runs\NG_die_pad_111_ng\01_matching_overlay.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-0p90-1p10-step0p10-full-20260829\evidence\runs\runs\OK_die_pad_013_ok\01_matching_overlay.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-0p90-1p10-step0p10-full-20260829\evidence\runs\runs\OK_die_pad_079_ok\01_matching_overlay.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-0p90-1p10-step0p10-full-20260829\evidence\runs\runs\OK_die_pad_004_ok\01_matching_overlay.png`

후보 evidence row CSV SHA-256:

`C5F1020AA63E7A6A2123421B8FC21A61896820C5D259FD77CCF48B411E8DE677`.

Batch CSV SHA-256:

`D100465ABB407FAFDA103BCEAD047F7B1D9F628C6263588449D8ED5C340DDA21`.

Before/after composite images (left `STEP=0.05`, right `STEP=0.10`) are kept
under the current candidate folder:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-0p90-1p10-step0p10-full-20260829\comparison\before-after_OK_079.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-0p90-1p10-step0p10-full-20260829\comparison\before-after_OK_004.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-0p90-1p10-step0p10-full-20260829\comparison\before-after_NG_240.png`

## 재사용 규칙

이번 결과를 반영해
`C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\detection-power-study.md`
에 다음 제약을 추가했습니다.

- 선택된 scale 범위 뒤의 coarse step은 별도 성능 후보로 취급한다.
- 같은 full corpus에서 per-image score, count, status, 실제 overlay geometry를
  비교한다.
- `PipelineSuccess`/`StepStatus`/`ResultCount`가 같아도 score 또는 box가
  변하면 정확도 기본값으로 승격하지 않는다.
- 시간 절약만으로 coarse step을 자동 선택하지 않고, finer step을 정확도
  기본값으로 유지한다.

Skill 검증:

```text
Skill is valid!
PASS: template registration manifest regression cases
```

## 범위 경계

- 신규 OK content가 없어 정식 NG/OK held-out qualification은 완료되지
  않았습니다. 외부 OK 중복을 독립 근거로 사용하지 않았습니다.
- `STEP=0.10`은 실행 가능성 및 시간 개선은 확인했지만, geometry/score
  parity가 보장되지 않아 전역 정확도 기본값으로 채택하지 않았습니다.
- 이번 작업은 Dev의 테스트 evidence와 문서/skill reference만 변경했습니다.
  애플리케이션 source, 원본 repository, branch push, tag, release, 설치,
  rollout, deployment, EXE launch smoke는 수행하지 않았습니다.

## Evidence inventory

- Summary JSON: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-step-heldout-performance-summary.json`
  (SHA-256 `AC202B7A02802744643DC726505BC05336739BF0D8A4FDFDCF16D95408A23317`)
- Candidate manifest: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-0p90-1p10-step0p10-full-20260829\candidate-manifest.json`
  (SHA-256 `E182B9112BD9B6327FF1928EB189C878201382E6924F84C6A93A0226CA2B68AD`)
- Per-row comparison: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-step-comparison.csv`
  (SHA-256 `B9332E3BE82318A7B95997B173A4C1DD0B582FC8B7E38E4BA70399667F85DCC5`)
