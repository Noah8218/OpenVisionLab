# Vision Tool PropertyGrid Policy

OpenVisionLab의 알고리즘 검사 툴은 **Property 모델 우선** 구조를 유지한다.

## 핵심 원칙

- Blob, Contour, Line, Matching, FeatureMatching, EdgeBasedMatching 같은 알고리즘 검사 툴은 PropertyGrid를 사용한다.
- 새 파라미터는 해당 Tool Property 모델에 public property와 attribute를 추가하면 PropertyGrid UI에 자동으로 노출되는 구조를 유지한다.
- 알고리즘 검사 툴마다 별도 WPF 입력 패널을 직접 만드는 방식은 기본 방향이 아니다.
- ViewModel은 PropertyGrid를 대체하지 않는다. ViewModel은 실행 전 정규화, 요약, 템플릿 상태, 레이어/프리뷰 보조 상태처럼 화면 주변 로직을 분리하는 용도로만 사용한다.
- PropertyGrid editor, visibility rule, range/threshold editor, ROI/mask/template editor는 공통 bridge/editor 계층에서 고친다. 개별 알고리즘 View가 같은 editor를 복제하지 않는다.
- PropertyGrid가 편집하는 원본 Property 객체가 파라미터의 source of truth이다. 컨트롤 값을 따로 읽어서 임시 상태로 저장하거나, View 재생성 시 기본 생성자 값으로 되돌리면 안 된다.
- RangeEditor는 `MIN_AREA`/`MAX_AREA`, `FIND_ANGLE_MIN`/`FIND_ANGLE_MAX`처럼 두 모델 값을 하나의 UI 행에서 편집하는 공통 editor이다. Max 보조 속성은 XML 저장/실행 모델에는 남기되 PropertyGrid에 별도 행으로 중복 노출하지 않는다.
- RangeEditor의 Max 보조 속성 descriptor는 TypeDescriptor/원본 WPG PropertyItem 경로에서 제거하지 않는다. WPG RangeEditor는 `FIND_ANGLE_MAX`, `MAX_AREA`, `CANNY_HIGH` 같은 companion descriptor를 통해 Max endpoint를 읽고 쓰므로, 중복 행은 visual row만 숨기고 descriptor/model property는 유지한다.
- RangeEditor 숫자 TextBox는 operator가 값을 지우거나 `-`처럼 입력 중간 상태를 만드는 것을 허용해야 한다. TextBox는 `TextChanged`마다 즉시 모델에 커밋하지 말고 Enter 또는 focus lost 시 확정한다. Slider endpoint는 즉시 반영해도 된다.
- PropertyGrid 기반 툴은 preview/run 또는 pipeline 추가 직전에 현재 편집 중인 TextBox/ComboBox/Slider 값을 Property 모델에 커밋해야 한다. 사용자가 숫자 값을 입력한 뒤 focus를 옮기지 않고 바로 버튼을 눌러도 이전 값으로 실행되면 안 된다.
- PropertyGrid의 조건부 하위 파라미터 행은 부모 토글/선택 행과 시각적으로 구분되어야 한다. 이 스타일은 `WpfPropertyGridBridge` 공통 계층에서 유지하며 개별 툴 View에 복제하지 않는다.
- `USE_THRESHOLD`, `USE_ADAPTIVE_THRESHOLD`, `USE_APPROXPOLYDP`, `USE_FIND_ANGLE`, `USE_CANNY` 같은 표시/옵션 토글은 하위 행 표시만 변경한다. 토글만으로 preview/run이나 output layer 갱신이 발생하면 안 된다.
- Contour 표시 옵션은 안정화된 UX이다. `컨투어 표시` 바로 아래에 `표시 색상`, `선 두께`가 위치해야 하며, 기본 표시 색상은 `Aquamarine`이다.
- 툴별 마지막 파라미터와 ROI는 티칭 상태이다. 도구 창을 닫았다 다시 열거나 native tool document가 재생성되어도 같은 recipe/tool에서는 마지막 값이 유지되어야 한다.
- ROI(`CvROI`, `CvROIS`, `CvMASKS`)는 Property 모델과 함께 저장되어야 하며, input preview 이미지가 로드/갱신될 때 현재 ROI overlay가 표시되어야 한다.
- ROI overlay 표시는 실행 결과가 아니다. ROI 표시만으로 output layer 생성, output layer 선택, preview/run, pipeline append가 발생하면 안 된다.
- 기존 recipe XML 영구 저장 경로는 `VisionToolStorage`, `RecipeRuntimeStorage`, `OpenCvPropertyBase.LoadConfig/SaveConfig`에 이미 존재한다. WPF native tool 전환 시 이 경로를 우회해서 별도 `new ...Property(...)` 객체만 쓰면 저장 기능이 사라지는 회귀가 된다.
- WPF native PropertyGrid 툴은 가능한 경우 `VisionToolRepository`가 소유한 recipe Property 객체를 그대로 사용한다. 기본 1번 툴 이름은 `Blob_1`, `Contour_1`, `Line(L)_1`, `Line(R)_1`, `Matching_1`, `Feature_1`, `EdgeBasedMatching_1` 규칙을 유지한다.
- Threshold, Filter, Morphology, Arithmetic, SimplePreprocess처럼 custom WPF 컨트롤을 쓰는 툴도 recipe별 `*_ToolState.xml`로 마지막 설정값을 저장한다. PropertyGrid 기반이 아니어도 티칭 파라미터 persistence 계약은 동일하다.

## Line Tool Scan Line Contract

- Line 툴 PropertyGrid의 수직선 계열 보조 파라미터는 사용자에게 `Scan Line` 카테고리로 표시한다.
- 표시명은 `Scan direction`, `Scan interval`, `Use scan angle`, `Scan angle`, `Show scan line`을 유지한다.
- WPF PropertyGrid 표시 문구를 다시 `Vertical Line`으로 되돌리지 않는다. 기존 localization key는 레거시 호환 목적으로만 유지할 수 있다.
- 표시 문구를 맞춘다는 이유로 내부 호환 식별자 `VER_PRJ_DIR`, `POINT_RANGE`, `USE_MANUAL_ANGLE`, `MANUAL_ANGLE_VALUE`, `SHOW_VERTICAL_LINE`, `VerticalLineCalculator`, XML 이름, pipeline 파라미터 이름을 변경하지 않는다.
- Line 툴의 `Distance`는 scan-line 기반 교차 및 거리 측정 흐름이다. `Intersection`은 fit-line 간 교차점 계산 흐름이다. 새 명시 사양 없이 별도 `Scan Intersection` 모드로 분리하지 않는다.

## Matching Tool PropertyGrid Contract

- Matching 툴은 WPF native floating tool window에서도 PropertyGrid 기반 파라미터 편집 구조를 유지한다. 개별 WPF 수동 입력 패널로 대체하지 않는다.
- 템플릿 상태 행은 유지한다. 템플릿 미선택/등록 완료 상태와 템플릿 크기/파일명을 사용자가 확인할 수 있어야 한다.
- 템플릿 등록/편집 창은 active input image를 표시해야 하며, blank viewer로 열리면 안 된다. ROI/template rectangle은 클릭, 이동, 크기 조절이 가능해야 한다.
- `PATTERN_PATH` editor 버튼은 main/input image 로드 후에도 클릭 가능해야 한다.
- Matching은 기본적으로 manual preview이다. `AUTO_PREVIEW=false`에서는 템플릿 등록, 점수, 매칭 개수, 배율, 매칭 방식, 각도 범위/간격 변경만으로 preview/run이 실행되면 안 된다.
- `AUTO_PREVIEW=true`는 명시적 opt-in이다. 켠 뒤에도 `USE_FIND_ANGLE`, `USE_CANNY` 같은 표시/조건 토글은 하위 row 표시만 바꾸며 preview를 직접 실행하지 않는다.
- `USE_COARSE_TO_FINE_ANGLE_SEARCH`는 넓은 각도 범위 성능 개선용 명시적 opt-in이다. 기본값은 false이며, 사용자가 켜기 전까지 기존 exhaustive angle search 결과 흐름을 바꾸면 안 된다.
- `COARSE_ANGLE_STEP`, `COARSE_ANGLE_TOP_K`는 `USE_FIND_ANGLE=true` 및 `USE_COARSE_TO_FINE_ANGLE_SEARCH=true`일 때만 하위 옵션으로 표시한다. 이 표시 토글만으로 preview/run을 실행하면 안 된다.
- Coarse 옵션 설명과 Matching summary는 operator가 비용 차이를 이해할 수 있게 유지한다. Coarse가 켜지면 summary에 `Coarse step`, `top K`, `estimated/full candidates`가 보여야 한다.
- Matching result review는 `Template Match` label을 유지하고 Count, Score, Center, Box, Angle, Tact 정보를 표시한다. `Tact`는 ms 단위 실행시간으로 유지한다.
- 각도 탐색 결과의 match box/ROI overlay는 검출 각도를 반영해 회전되어야 한다. 텍스트 라벨은 판독성을 위해 화면 기준 upright로 둘 수 있다.
- `MAGNIFIATION` 내부 property 이름은 recipe/XML 호환 때문에 유지한다. 표시 설명은 image-pyramid 기반 scale search 개념을 설명한다.
- `FIND_ANGLE_MIN`/`FIND_ANGLE_MAX`는 하나의 RangeEditor row에서 편집한다. `FIND_ANGLE_MAX` descriptor/model property는 삭제하지 말고, 중복 visual row만 숨긴다.
- Matching 각도 RangeEditor TextBox는 지우기, `-` 입력, 부분 숫자 입력 같은 transient edit를 허용한다. Enter 또는 focus lost 전까지 즉시 이전 값으로 되돌리면 안 된다.
- Matching의 일반 숫자 TextBox(`SCORE_MIN`, `NUM_MATCH`, `MAGNIFIATION` 등)는 operator가 값을 입력하고 바로 `미리보기 실행` 또는 `파이프라인 추가`를 눌러도 현재 입력값으로 실행되어야 한다. focus lost를 기다리는 동작으로 되돌리지 않는다.

## EdgeBasedMatching PropertyGrid Contract

- EdgeBasedMatching remains a PropertyGrid-based algorithm tool. Do not replace it with a custom one-off WPF parameter panel.
- `USE_HYBRID_VERIFY` is an explicit opt-in option. The default is `false` so existing recipes keep the previous edge-score selection path.
- `HYBRID_VERIFY_TOP_N` and `HYBRID_VERIFY_IMAGE_WEIGHT` are child parameters of `USE_HYBRID_VERIFY`; they must be visible only when Hybrid verify is enabled.
- Hybrid verify re-ranks edge candidates with image-template similarity, preserves spatial grid candidates, and adds one image-matching proposal whose edge score is recomputed before selection. It must not change `SCORE_MIN` semantics, and the exposed result `Score` remains the edge score for recipe compatibility.
- `Search step=2 + Hybrid verify` is the validated safer path for similar-edge distractors. `Search step=4 + Refine position + Hybrid verify` also recovered the synthetic clutter sample on 2026-06-27, but `Search step=4 + Refine position` without Hybrid verify remains a speed option that needs sample validation.
- Coarse angle search, position refine, and hybrid verify are independent performance/robustness options. Do not auto-enable one just because another is enabled.
- EdgeBasedMatching phase timing is diagnostic-only. `CollectPhaseTimings` must remain off by default and must not affect PropertyGrid visibility, recipe XML, result score, or preview behavior.
- The current measured speed priority is edge candidate search first, Hybrid scaled image proposal second. Do not make PropertyGrid default changes to hide algorithm cost without benchmark evidence.

## WPF Tool View 구조

- 단일 입력 툴의 입력 레이어, 출력 레이어, 미리보기, 상태, 실행/파이프라인 버튼은 `VisionToolSingleInputPropertyToolShell`과 전용 Runtime이 맡는다.
- PropertyGrid 기반 툴은 `VisionToolSingleInputPropertyToolRuntime<TProperty>` 또는 matching 전용 Runtime을 사용한다.
- Threshold, Filter, Morphology처럼 전처리성 custom WPF 패널이 필요한 툴은 `VisionToolSingleInputCustomToolRuntime`을 사용한다.
- custom WPF 패널은 View 자신의 namescope에 `parameterContentHost`로 두고, Runtime이 Shell의 parameter slot으로 이동시킨다. Shell DP 안에 `x:Name`이 있는 컨트롤을 직접 넣으면 WPF namescope 충돌이 생길 수 있다.
- View 코드비하인드는 Shell 내부 컨트롤 이름을 직접 만지지 않는다. 레이어 목록, preview image, status, summary는 Runtime API를 통해 갱신한다.

## 예외

Threshold, Filter, Morphology 같은 전처리성 native WPF 툴은 사용성이 명확한 전용 WPF 컨트롤을 사용할 수 있다. 다만 알고리즘 검사 툴은 Property 모델을 추가하면 자동으로 UI가 생기는 구조가 OpenVisionLab의 유지보수 방향이다.

## 작업 전 체크

1. 알고리즘 검사 파라미터를 추가할 때는 먼저 Property 모델에 추가한다.
2. PropertyGrid에 표시할 이름, 설명, 순서, editor attribute를 Property 모델에 둔다.
3. UI 문제가 있으면 개별 툴 화면을 새로 만들기보다 PropertyGrid bridge/editor/visibility rule을 먼저 수정한다.
4. ViewModel로 옮길 때도 PropertyGrid가 편집하는 원본 Property 객체와 실행용 property 생성 계약을 보존한다.
5. Tool factory에서 PropertyGrid 기반 툴을 만들 때는 `VisionToolRepository`의 recipe Property를 우선 사용한다. 단순히 `new BlobProperty(...)`처럼 매번 새 기본값을 만들면 안 된다.
6. ROI editor를 수정할 때는 `docs/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`의 “Tool Parameter Persistence And ROI Teaching Memory” 계약을 함께 확인한다.
7. 새 WPF Tool View를 만들 때는 공통 Shell/Runtime을 먼저 사용 가능한지 확인하고, 직접 레이어 콤보나 preview 버튼 배선을 복제하지 않는다.
8. custom WPF 툴을 추가할 때는 `OpenVisionNativeToolSettingsStore.CreateConfigName(toolName)` 규칙으로 recipe XML 저장을 연결한다.

## 최근 검증

- 2026-06-24: Threshold, Filter, Morphology를 custom parameter host + `VisionToolSingleInputCustomToolRuntime` 구조로 전환했다.
- 검증: `dotnet build .\OpenVisionLab.sln -c Debug -p:Platform=x64 -m:1 -nr:false` 통과.
- 검증: `tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_threshold_tool,wpf_layer_selection_all_native_tools,wpf_tool_open_perf" -OutputDir "artifacts\ui_precheck_custom_shell_threshold_filter_morphology" -TimeoutSeconds 180` 통과.
- 2026-06-26: Matching RangeEditor의 Max endpoint 조절과 숫자 TextBox transient edit를 검증했다. `FIND_ANGLE_MAX` companion descriptor를 유지한 상태에서 visual duplicate row만 숨겨야 하며, 각도 TextBox는 빈 값으로 지워도 즉시 이전 값으로 되돌아가면 안 된다.
- 검증: `dotnet build .\src\OpenVisionLab\OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false` 통과.
- 검증: `tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_matching_tool" -FailOnWarn -OutputDir "artifacts\ui_precheck_matching_range_text_transient_20260626" -WpgCustomBuildEnabled false -TimeoutSeconds 420` 통과.
- 2026-06-26: Matching template editor image 표시, angle overlay 회전 표시, result review Tact, manual/auto preview option, RangeEditor Max endpoint, RangeEditor transient text edit를 Matching 안정 계약으로 묶었다.
- 검증: `artifacts\ui_precheck_matching_template_image_fixed_20260626_final`, `artifacts\ui_precheck_matching_rotated_overlay_20260626`, `artifacts\ui_precheck_matching_tact_label_ui_20260626`, `artifacts\ui_precheck_matching_auto_preview_option_20260626`, `artifacts\ui_precheck_matching_angle_max_range_20260626_visual_hidden`, `artifacts\ui_precheck_matching_range_text_transient_20260626` 모두 OK.
- 2026-06-26: PropertyGrid 편집 중인 일반 TextBox 값이 focus lost 없이도 preview/run 직전에 커밋되도록 검증했다. Matching `SCORE_MIN` 값을 TextBox에 입력만 한 상태에서 explicit preview를 실행해 새 값이 모델에 반영되는지 확인했다.
- 검증: `artifacts\ui_precheck_matching_text_commit_20260626_rerun` OK.
- 2026-06-26: 사용자가 Matching PropertyGrid UX를 직접 검증 완료했다. 콤보박스, RangeEditor, 일반 숫자 TextBox command-time commit, manual/auto preview 정책, 템플릿/결과 표시 안정화 항목은 새 회귀 증거 또는 명시적 재설계 요청이 없으면 다시 일반 리팩토링 대상으로 건드리지 않는다.
- 2026-06-26: Matching wide-angle 성능 개선을 위해 coarse-to-fine angle search를 옵션으로 추가했다. 기본 off, `Use angle search` 하위 표시, 명시적 opt-in, manual preview 기본 정책을 유지한다.
- 검증: `.codex\MatchingAngleBenchmark` 기준 Die Pad 1~4에서 exhaustive `-10..180 / step 0.1` 4258.3~5782.1 ms, coarse step 5/top K 3 옵션 481.7~781.0 ms로 동일 best score/angle 확인. `artifacts\ui_precheck_matching_coarse_angle_20260626_diag` OK.
