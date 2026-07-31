using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal static class OpenVisionRecipePersistenceStatusPresenter
    {
        public static string CreateCompactText(
            VisionPipelinePersistenceState state)
        {
            if (state == null)
            {
                return string.Empty;
            }

            return state.Kind switch
            {
                VisionPipelinePersistenceStateKind.InvalidFileSubstituted =>
                    OpenVisionRecipeText.Local(
                        "저장본 손상 — 편집 가능한 기본 Pipeline으로 대체됨. 저장 후 다시 열어 확인하세요.",
                        "Saved Pipeline was damaged — an editable default was substituted. Save, then reopen to verify."),
                VisionPipelinePersistenceStateKind.LoadFailed =>
                    OpenVisionRecipeText.Local(
                        "저장본 읽기 실패 — 디스크 파일은 변경하지 않았습니다. 원인과 경로를 확인하세요.",
                        "Saved Pipeline could not be read — the disk file was not changed. Review the cause and path."),
                VisionPipelinePersistenceStateKind.SaveFailed =>
                    OpenVisionRecipeText.Local(
                        "Pipeline 저장 실패 — 현재 편집은 메모리에만 있으며 다시 열면 손실될 수 있습니다.",
                        "Pipeline save failed — current edits are memory-only and may be lost after reopen."),
                VisionPipelinePersistenceStateKind.SaveRecovered =>
                    OpenVisionRecipeText.Local(
                        "Pipeline 저장 복구 완료 — 현재 정의가 디스크에 기록되었습니다.",
                        "Pipeline save recovered — the current definition is persisted."),
                _ => string.Empty
            };
        }

        public static string CreateHelpText(
            VisionPipelinePersistenceState state)
        {
            if (state == null)
            {
                return string.Empty;
            }

            List<string> lines = new List<string>
            {
                CreateCompactText(state),
                OpenVisionRecipeText.Local("레시피: ", "Recipe: ")
                    + state.RecipeName,
                OpenVisionRecipeText.Local("파이프라인: ", "Pipeline: ")
                    + state.PipelineName,
                OpenVisionRecipeText.Local("저장 경로: ", "Saved path: ")
                    + state.SourcePath
            };

            if (!string.IsNullOrWhiteSpace(state.BackupPath))
            {
                lines.Add(
                    OpenVisionRecipeText.Local(
                        "보존된 이전 파일: ",
                        "Preserved prior file: ")
                    + state.BackupPath);
            }

            if (!string.IsNullOrWhiteSpace(state.ErrorMessage))
            {
                lines.Add(
                    OpenVisionRecipeText.Local("원인: ", "Cause: ")
                    + state.ErrorMessage);
            }

            if (state.IsFailure)
            {
                lines.Add(
                    OpenVisionRecipeText.Local(
                        "이 상태에서는 실행·검증·적격화 근거로 사용하지 마십시오. 원인을 해결한 뒤 명시적으로 저장하고 다시 여세요.",
                        "Do not use this state as Run, validation, or qualification evidence. Resolve the cause, explicitly save, and reopen."));
            }
            else
            {
                lines.Add(
                    OpenVisionRecipeText.Local(
                        "다시 열어 Recipe/Pipeline/Step이 동일한지 확인하십시오. 다음 일반 저장에서는 이 복구 알림이 반복되지 않습니다.",
                        "Reopen and verify the same Recipe/Pipeline/Steps. The next ordinary save will not repeat this recovery notice."));
            }

            return string.Join(Environment.NewLine, lines);
        }

        public static string CreateCompactText(
            RecipeDataPersistenceState state)
        {
            if (state == null)
            {
                return string.Empty;
            }

            return state.Kind switch
            {
                RecipeDataPersistenceStateKind.InvalidFileSubstituted =>
                    OpenVisionRecipeText.Local(
                        "Recipe 데이터 저장본 손상 — 편집 가능한 기본값으로 대체됨. 저장 후 다시 열어 확인하세요.",
                        "Saved Recipe data was damaged — editable defaults were substituted. Save, then reopen to verify."),
                RecipeDataPersistenceStateKind.LoadFailed =>
                    OpenVisionRecipeText.Local(
                        "Recipe 데이터 읽기 실패 — 디스크 파일은 변경하지 않았습니다. 원인과 경로를 확인하세요.",
                        "Recipe data could not be read — the disk file was not changed. Review the cause and path."),
                RecipeDataPersistenceStateKind.SaveFailed =>
                    OpenVisionRecipeText.Local(
                        "Recipe 데이터 저장 실패 — 현재 상태는 메모리에만 있으며 다시 열면 손실될 수 있습니다.",
                        "Recipe data save failed — current state is memory-only and may be lost after reopen."),
                RecipeDataPersistenceStateKind.SaveRecovered =>
                    OpenVisionRecipeText.Local(
                        "Recipe 데이터 저장 복구 완료 — 현재 상태가 디스크에 기록되었습니다.",
                        "Recipe data save recovered — the current state is persisted."),
                _ => string.Empty
            };
        }

        public static string CreateHelpText(
            RecipeDataPersistenceState state)
        {
            if (state == null)
            {
                return string.Empty;
            }

            List<string> lines = new List<string>
            {
                CreateCompactText(state),
                OpenVisionRecipeText.Local("레시피: ", "Recipe: ")
                    + state.RecipeName,
                OpenVisionRecipeText.Local(
                    "Recipe 데이터 경로: ",
                    "Recipe data path: ")
                    + state.SourcePath
            };

            if (!string.IsNullOrWhiteSpace(state.BackupPath))
            {
                lines.Add(
                    OpenVisionRecipeText.Local(
                        "보존된 이전 파일: ",
                        "Preserved prior file: ")
                    + state.BackupPath);
            }

            if (!string.IsNullOrWhiteSpace(state.ErrorMessage))
            {
                lines.Add(
                    OpenVisionRecipeText.Local("원인: ", "Cause: ")
                    + state.ErrorMessage);
            }

            lines.Add(
                state.IsFailure
                    ? OpenVisionRecipeText.Local(
                        "이 상태에서는 실행·검증·적격화 근거로 사용하지 마십시오. 원인을 해결한 뒤 명시적으로 저장하고 다시 여세요.",
                        "Do not use this state as Run, validation, or qualification evidence. Resolve the cause, explicitly save, and reopen.")
                    : OpenVisionRecipeText.Local(
                        "다시 열어 Recipe 상태가 동일한지 확인하십시오. 다음 일반 저장에서는 이 복구 알림이 반복되지 않습니다.",
                        "Reopen and verify the same Recipe state. The next ordinary save will not repeat this recovery notice."));

            return string.Join(Environment.NewLine, lines);
        }
    }
}
