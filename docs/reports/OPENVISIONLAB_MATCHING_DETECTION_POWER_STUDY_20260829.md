# OpenVisionLab Matching 검출력 연구 — Die Pad 188 r2

## 상태

`Complete` — 사용자 승인으로 수행한 고정 corpus 기반의 범위 제한 연구가 완료되었습니다. 이 문서는 운영 배포 승인이나 제품 qualification을 의미하지 않습니다.

## 결론

같은 원본·템플릿·ROI·각도 규칙을 유지한 채 `Matching.USE_FIND_SCALE=true`를 전역으로 켜고 `FIND_SCALE_MIN=0.90`, `FIND_SCALE_MAX=1.10`, `FIND_SCALE_STEP=0.05`를 적용한 후보를 연구용 전역 후보로 선택했습니다.

- 기존 r2 고정 배율: `445/500` 검출, `55` NoResult
- 선택 후보: `500/500` 검출, `0` NoResult
- 선택 후보는 기존 445개 성공 행을 모두 보존했고, 손실 55개를 추가로 복구했습니다.
- `SCORE_MIN=0.60`, `CCoeffNormed`, angle search `-10..+10°/0.1°`, `NUM_MATCH=3`, Canny 미사용은 그대로 유지했습니다.
- 대표 성공·저점수·결함·복구 오버레이에서 모두 동일한 두 Pad, hole 순서, L/right Trace, 하단·우측 물리 경계에 박스가 대응했습니다.
- 단점은 평균 elapsed가 약 `600 ms`에서 `3,754 ms`로 증가한 것입니다. 따라서 정확도 후보로는 유효하지만, 제품 기본값으로 반영하기 전 성능 예산과 held-out 검증이 필요합니다.

`ScoreMax >= 90`은 이 연구의 1차 기준이 아닙니다. 선택 후보의 `ScoreMax >= 90`은 `389/500`이고, 낮은 점수 행도 실제 위치가 맞는 경우가 확인되었습니다. 점수만 90 이상으로 강제하면 검출력과 결함 샘플의 위치 증거를 잃을 수 있습니다.

## 고정 입력

| 항목 | 값 |
| --- | --- |
| Dataset | `E:\라벨테스트\EasyMatch_Die_Pad_500(1)\EasyMatch_Die_Pad_500` |
| Corpus | 500 images (`NG 250 + OK 250`) |
| Image-list SHA-256 | `9243089DF5E4CA7E2FEB13C569CED961856A69587DC8E37E647B9645B2770D0B` |
| Template | `die_pad_188_material_only_expanded_r2.png` |
| Template SHA-256 | `DBF8E0A0470B24F80162FF6128C0B07A9A9C28B777C843B709CFFC38146574E6` |
| Registration manifest SHA-256 | `B60348FF5B94F30C22684839EF2CF177409FB702A111B85F8EF476833A780E28` |
| Locked ROI | `(x=169, y=172, width=188, height=142)`, teaching angle `0°` |
| Source image SHA-256 | `4B34BCA9F3DC042DAB834FA7772EF8AED11459EAE077736C096038D5D84912EC` |

The template is the user-approved same-source `188` material-boundary expansion. No other source image, per-image ROI, or per-image parameter was introduced.

## Candidate comparison

All full-corpus candidates used the same 500-image list and produced per-image source/result/overlay evidence.

| Candidate | Found | NoResult | Mean score | Score >=90 | Mean elapsed | Decision |
| --- | ---: | ---: | ---: | ---: | ---: | --- |
| r2 fixed scale, `SCORE_MIN=0.60` | 445 | 55 | 82.396 | 152 | 599.696 ms | baseline |
| `SCORE_MIN=0.55` (scale off) | 480 | 20 | 80.728 | 152 | 879.877 ms | reject as threshold-only fix |
| Canny 30/60 (scale off) | 9 | 491 | 65.403 | 0 | 399.359 ms | reject; destroys useful grayscale structure |
| scale 0.95..1.05 / 0.05 | 494 | 6 | 87.801 | 279 | 2,422.823 ms | narrower diagnostic |
| **scale 0.90..1.10 / 0.05** | **500** | **0** | **93.084** | **389** | **3,754.370 ms** | **study-level AUTO_SELECTED** |

The selected candidate had `0` rows found by the baseline but missing in the candidate, and `55` rows found only by the candidate. The 0.95..1.05 candidate had 6 remaining NoResult rows; all 6 were recovered by the wider 0.90..1.10 candidate.

### NG/OK split for selected candidate

| Corpus label | Rows | Found | NoResult | Mean score | Score >=90 |
| --- | ---: | ---: | ---: | ---: | ---: |
| NG | 250 | 250 | 0 | 92.540 | 183 |
| OK | 250 | 250 | 0 | 93.630 | 206 |

`NG/OK` filenames are corpus labels only; they were not used as a correctness oracle for template location.

## Visual correspondence evidence

The current selected-candidate overlays are under:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-0p90-1p10-full\evidence\runs`

Representative files reviewed in the current run:

- `NG_die_pad_188_ng\01_matching_overlay.png` — user reference; two right Pads, traces, and lower/right boundary are enclosed.
- `OK_die_pad_019_ok\01_matching_overlay.png` — rotated sample; the box follows the sample orientation and the same physical feature set.
- `OK_die_pad_207_ok\01_matching_overlay.png` — high-score ordinary success.
- `NG_die_pad_222_ng\01_matching_overlay.png` — difficult/defective sample; box remains on the intended structure.
- `NG_die_pad_036_ng\01_matching_overlay.png`, `NG_die_pad_045_ng\01_matching_overlay.png`, `OK_die_pad_002_ok\01_matching_overlay.png`, `OK_die_pad_232_ok\01_matching_overlay.png` — examples recovered from the former 55-row loss set.
- `NG_die_pad_111_ng`, `NG_die_pad_211_ng`, `NG_die_pad_243_ng`, `OK_die_pad_013_ok`, `OK_die_pad_159_ok`, `OK_die_pad_168_ok` — the six rows still missed by the narrow scale candidate and recovered by the wider range; all six boxes were visually on the same target structure.

The selected run contains `500/500` source images, `500/500` runtime result images, and `500/500` matching overlays, each with a unique path and source hash.

## Exact selected recipe candidate

Selected candidate pipeline:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-0p90-1p10-full\pipelines\pipeline.xml`

- Pipeline SHA-256: `F1156AF2C852D0C053811AB30EDE217FF0F246876CF8B53F8CA19342ECFB5694`
- `MATCH_MODE=CCoeffNormed`
- `SCORE_MIN=0.6`
- `NUM_MATCH=3`
- `USE_FIND_ANGLE=True`, `FIND_ANGLE_MIN=-10`, `FIND_ANGLE_MAX=10`, `FIND_ANGLE=0.1`
- `USE_FIND_SCALE=True`, `FIND_SCALE_MIN=0.9`, `FIND_SCALE_MAX=1.1`, `FIND_SCALE_STEP=0.05`
- `USE_CANNY=False`

Batch evidence:

- `evidence_rows.csv` SHA-256: `FF0C1FD459BC77E5AD131B8FC51D5EAF21B9970032E2230DCD0B4C5A8F95097F`
- `batch.csv` SHA-256: `3D1DBDCBD1D47B969FC9FE22ED08D9B9E1B6D823348FB84847DBCFAA447CA769`

The run used `VisionRecipeRunnerSmoke.dll --batch-evidence` with the frozen list and produced `BatchRows=500`, `BatchCompleted=500`, `BatchPipelinePasses=500`, `BatchMissingImages=0`.

## Skill change

The rule-based teaching skill now contains a reusable detection-power study route:

- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\SKILL.md`
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\detection-power-study.md`

The added rule is conditional, not a hardcoded Die Pad value: after source/ROI/angle/outer-boundary lock, repeated fixed-scale NoResult must trigger a finite global scale-sensitivity study before lowering `SCORE_MIN` or changing the template. Scale is global only, never a per-image retry; full-corpus rerun and visual correspondence gates are required.

Validation passed:

```text
Skill is valid!
PASS: template registration manifest regression cases
```

## Boundary and next action

- This is a study-level automatic selection with `qualification=false`; every overlay was generated, but not every one was individually human-reviewed and no held-out corpus was supplied.
- No application source, original repository, branch, tag, release, deployment, installation, or EXE launch was changed or executed.
- Before making the scale candidate an operating default, run a held-out visual review and decide whether the roughly 6.3× mean-time cost is acceptable or whether a bounded performance optimization is authorized.
