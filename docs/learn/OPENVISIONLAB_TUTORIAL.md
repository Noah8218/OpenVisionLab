# OpenVisionLab 처음 사용하기

Updated: 2026-08-01 KST

이 문서는 공개 샘플로 첫 결과를 확인하고, `Threshold -> Blob` Recipe를 저장하는 절차입니다.

## 먼저 알아둘 말

| 화면 용어 | 뜻 |
| --- | --- |
| Image | 검사할 사진 |
| Layer | 원본이나 중간 결과 이미지 |
| Tool | Threshold, Blob처럼 한 가지 작업을 하는 기능 |
| Preview | 현재 설정을 한 번 시험하는 실행 |
| Pipeline | Tool을 실행 순서대로 연결한 목록 |
| Recipe | 저장된 Pipeline과 설정 |
| Metric | 개수, 면적, 점수처럼 판정에 쓰는 숫자 |
| ROI | 검사할 이미지 영역 |

샘플 선택, Layer 선택, preset 적용, 파라미터 변경만으로는 실행되지 않습니다. `미리보기 실행`이나 `Run Review`를 직접 눌러야 결과가 바뀝니다.

## 1. 공개 Blob 샘플 열기

1. 시작 화면에서 `샘플 열기`를 누릅니다.
2. `Learn 경로`에서 `Blob`을 선택합니다.
3. 밝은 원형 입자가 있는 Good 샘플을 선택합니다. 검색할 때는 `Public_Blob_Particles_Good`를 입력해도 됩니다.
4. 기준이 `ResultCount 8~14`인지 확인합니다.
5. `이 샘플 열기`를 누릅니다.

성공하면 가운데에 입자 이미지가 보이고 현재 Layer는 `Main`입니다. 샘플을 열어도 검사는 아직 실행되지 않습니다.

## 2. Blob 결과 만들기

1. 왼쪽 Tool 목록에서 `Blob`을 엽니다.
2. 입력 Layer가 `Main`, 결과 Layer가 `Blob_Preview`인지 확인합니다.
3. `기본` preset을 누릅니다.
4. PropertyGrid에서 `기준값`을 `150`으로 입력합니다.
5. `기준값` 행을 선택하면 `Parameter Guide`에서 값의 뜻과 확인할 결과를 볼 수 있습니다.
6. `미리보기 실행`을 누릅니다.

정상 결과:

- 상태: `미리보기 OK`
- 검출: `12`
- 이미지: 밝은 입자 12개에 노란 상자와 중심 표시
- 결과 Layer: `Blob_Preview`

![Blob 미리보기 결과](../assets/tutorial/current/public_blob_particles_good_result.png)

결과가 다르면 아래 세 가지만 먼저 확인합니다.

| 증상 | 확인할 값 |
| --- | --- |
| 결과가 바뀌지 않음 | `미리보기 실행`을 눌렀는지 확인 |
| 입자가 너무 많거나 적음 | `기준값`, `최소 면적`, `최대 면적` |
| 엉뚱한 위치가 잡힘 | ROI와 입력 Layer |

후보 표에 제외된 행이 많이 보여도 실제 입자 수는 `허용됨`과 `ResultCount`로 판단합니다.

## 3. Good과 Bad 비교하기

1. 아래 샘플 안내에서 `Pipeline 보기`를 누릅니다.
2. Pipeline Review에서 `Run Review`를 누릅니다.
3. Good 결과의 `ResultCount`가 `8~14` 안인지 확인합니다.
4. `NG 기준 열기` 또는 짝 샘플 열기 기능으로 `Public_Blob_Particles_Sparse_Bad`를 엽니다.
5. 다시 `Run Review`를 누릅니다.

Bad 샘플은 입자가 3개이므로 정상 범위 `8~14`보다 적어 `결과 NG`가 됩니다.

`검증 OK`는 Pipeline 정의가 실행 가능한 상태라는 뜻입니다. `결과 OK/NG`는 이미지 검사 결과입니다.

## 4. 내 Recipe 만들기

새 Recipe에서는 아래 두 Step만 만듭니다.

```text
01 Threshold : Main -> Threshold_Preview
02 Blob_1    : Threshold_Preview -> Blob_Preview
```

### Threshold 저장

1. 상단 Recipe 선택 영역에서 새 Recipe를 만듭니다.
2. 공개 Blob Good 이미지를 다시 엽니다.
3. `Threshold` Tool을 엽니다.
4. 입력 Layer를 `Main`, 결과 Layer를 `Threshold_Preview`로 둡니다.
5. `미리보기 실행`으로 흰 입자와 검은 배경이 분리되는지 확인합니다.
6. `파이프라인에 추가·저장`을 누릅니다.

### Blob 저장

1. `Blob` Tool을 엽니다.
2. 입력 Layer를 `Threshold_Preview`로 선택합니다.
3. 결과 Layer는 `Blob_Preview`로 둡니다.
4. `미리보기 실행`으로 입자 상자와 개수를 확인합니다.
5. `파이프라인에 추가·저장`을 누릅니다.

### 저장 결과 확인

1. `Pipeline 보기`를 엽니다.
2. 두 Step의 입력과 출력이 위 경로와 같은지 확인합니다.
3. `Run Review`를 누릅니다.
4. 앱을 다시 열었을 때 두 Step이 남아 있는지 확인합니다.

다시 열었을 때 Step 상태가 `WAIT`인 것은 정상입니다. Recipe 복원만으로 실행되지 않으며 `Run Review`를 눌러야 결과가 생깁니다.

## 5. 화면이 예상과 다를 때

아래 순서대로 확인합니다.

1. 현재 이미지가 맞는지 확인합니다.
2. Tool의 입력 Layer를 확인합니다.
3. 이전 Step의 결과 Layer가 있는지 확인합니다.
4. Preview 또는 Run Review를 실제로 눌렀는지 확인합니다.
5. 결과 이미지의 상자와 중심이 대상 위에 있는지 확인합니다.
6. Metric이 목표 범위 안인지 확인합니다.

## 다음 문서

- [Blob](LEARN_BLOB.md)
- [Threshold](LEARN_THRESHOLD.md)
- [Pipeline과 Layer](LEARN_PIPELINE_LAYER_ROUTING.md)
- [Metric과 OK/NG](LEARN_METRICS_ACCEPTANCE.md)
- [전체 학습 순서](OPENVISIONLAB_LEARN_CURRICULUM.md)
