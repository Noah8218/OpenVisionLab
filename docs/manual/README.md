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

배포본의 `Guide`에는 두 HTML과 `guide-manifest.json`이 함께 들어갑니다.
앱은 선택한 언어의 파일과 해시를 확인한 뒤 엽니다. 사용자 매뉴얼의 편집·생성과
화면 검수 도구는 Dev에서 관리하며, 공개 빌드는 검증된 생성 결과를 패키징합니다.
