# GPT Blob Particle Count Test

이 폴더만 사용하면 됩니다. 다른 OpenVisionLab 문서를 GPT에 첨부할 필요가 없습니다.

## 처음 요청하는 순서

1. GPT 대화를 새로 엽니다.
2. 아래 두 이미지를 함께 첨부합니다.
   - `Blob_Particles_Synthetic_OK.png`
   - `Blob_Particles_Synthetic_Sparse_NG.png`
3. `COPY_THIS_TO_GPT.txt`의 전체 내용을 한 번에 붙여넣습니다.
4. GPT가 반환한 응답을 수정하지 말고 그대로 Codex 대화에 전달합니다.

정상 응답은 `<?xml`로 시작하고 `</VisionPipeline>`으로 끝나야 합니다. 설명이나 Markdown 코드 블록이 함께 오더라도 사용자가 고치지 마십시오. 그 상태 자체가 실제 LLM 검증 데이터입니다.

## OpenVisionLab 검증이 실패한 경우

1. Codex가 제공하는 실제 Validation/Run 보고서를 받습니다.
2. `PASTE_VALIDATION_NG_BACK_TO_GPT.txt`의 자리표시자에 보고서를 넣습니다.
3. 처음 XML을 만든 같은 GPT 대화에 붙여넣습니다.
4. 수정 응답도 손대지 말고 그대로 Codex 대화에 전달합니다.

이 과정은 사람이 XML을 고쳐 성공시키는 시험이 아니라, GPT 초안과 실제 OpenVisionLab correction loop를 보존하는 시험입니다.
