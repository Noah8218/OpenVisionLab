# OpenVisionLab 사용자 매뉴얼 / User Manual

이 폴더가 한국어·영어 배포용 사용자 매뉴얼의 원본입니다.

- `workflows`, `tools`, `reference`: 한국어 원문 26장
- `en/workflows`, `en/tools`, `en/reference`: 같은 순서와 범위의 영어 원문 26장
- `assets/ui`: 한국어 현재 UI 캡처
- `assets/ui/en`: 영어 현재 UI 캡처
- `manual-manifest.json`, `manual-visuals.json`: 한국어 장 순서와 화면 번호
- `manual-manifest.en.json`, `manual-visuals.en.json`: 영어 장 순서와 화면 번호
- `generated/OpenVisionLab_User_Manual.ko.html`: 한국어 배포본
- `generated/OpenVisionLab_User_Manual.en.html`: 영어 배포본
- `generated/guide-manifest.json`: 두 파일의 언어·파일명·SHA-256 계약

프로그램의 Guide는 클릭 시점의 `OpenVisionLanguageService.CurrentLanguage`를
사용합니다. 한국어 UI에서는 `.ko.html`, 영어 UI에서는 `.en.html`만 엽니다.
선택 언어 파일이 없거나 해시가 다르면 다른 언어로 대신 열지 않고 오류로
종료합니다.

화면 안내 규칙:

1. 각 장은 실제 OpenVisionLab 화면을 먼저 보여줍니다.
2. 화면 위 번호와 바로 아래 번호 설명은 반드시 일치해야 합니다.
3. 번호는 기본 작업 순서인 입력 -> 설정 -> 명시적 Preview/Run -> 결과 확인을 따릅니다.
4. 오래된 캡처나 임의로 만든 UI 이미지를 현재 화면으로 사용하지 않습니다.
5. 원본 캡처를 바꾸면 `manual-visuals.json`의 번호 위치도 다시 확인합니다.
6. 각 언어 매뉴얼의 캡처와 번호 설명은 같은 언어여야 합니다.
7. 두 매뉴얼은 장 순서, Tool 범위, 작업 계약이 같아야 합니다.

생성 명령:

```powershell
dotnet run --project tools/OpenVisionUserManualBuilder/OpenVisionUserManualBuilder.csproj -- C:\Git\OpenVisionLab_Dev
```

원본 Markdown 또는 UI 캡처를 수정한 뒤에는 생성 명령, readiness, clean runtime
빌드를 실행합니다. 배포본의 `Guide`에 두 HTML과 매니페스트가 있는지 확인하고,
한국어 → 영어 → 한국어 전환 후 Guide가 각각 같은 언어 파일을 여는지 검증합니다.
