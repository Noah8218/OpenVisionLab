using System.Globalization;

namespace OpenVisionLab
{
    internal enum OpenVisionRecipeWorkspaceLifecycleOperation
    {
        Create,
        Duplicate,
        Rename,
        Delete
    }

    // Owns Recipe Manager lifecycle result-to-status projection without owning UI state.
    internal sealed class OpenVisionRecipeWorkspaceLifecycleProjectionOwner
    {
        internal OpenVisionRecipeWorkspaceLifecycleProjection Project(
            OpenVisionRecipeWorkspaceLifecycleOperation operation,
            OpenVisionRecipeWorkspaceResult result,
            string deletedRecipeName = null)
        {
            if (!result.Succeeded)
            {
                return OpenVisionRecipeWorkspaceLifecycleProjection.Failure(
                    BuildFailureStatus(operation));
            }

            string displayName = operation == OpenVisionRecipeWorkspaceLifecycleOperation.Delete
                ? deletedRecipeName
                : result.RecipeName;
            return OpenVisionRecipeWorkspaceLifecycleProjection.Success(
                result.RecipeName,
                BuildSuccessStatus(operation, displayName));
        }

        private static string BuildFailureStatus(
            OpenVisionRecipeWorkspaceLifecycleOperation operation)
        {
            switch (operation)
            {
                case OpenVisionRecipeWorkspaceLifecycleOperation.Duplicate:
                    return OpenVisionRecipeText.Local(
                        "레시피 복제에 실패했습니다.",
                        "Duplicate failed.");
                case OpenVisionRecipeWorkspaceLifecycleOperation.Rename:
                    return OpenVisionRecipeText.Local(
                        "이름 변경에 실패했습니다.",
                        "Rename failed.");
                case OpenVisionRecipeWorkspaceLifecycleOperation.Delete:
                    return OpenVisionRecipeText.Local(
                        "삭제에 실패했습니다.",
                        "Delete failed.");
                default:
                    return string.Empty;
            }
        }

        private static string BuildSuccessStatus(
            OpenVisionRecipeWorkspaceLifecycleOperation operation,
            string recipeName)
        {
            string format = operation switch
            {
                OpenVisionRecipeWorkspaceLifecycleOperation.Create =>
                    OpenVisionRecipeText.Local("생성됨: {0}", "Created: {0}"),
                OpenVisionRecipeWorkspaceLifecycleOperation.Duplicate =>
                    OpenVisionRecipeText.Local("복제됨: {0}", "Duplicated: {0}"),
                OpenVisionRecipeWorkspaceLifecycleOperation.Rename =>
                    OpenVisionRecipeText.Local("이름 변경됨: {0}", "Renamed: {0}"),
                OpenVisionRecipeWorkspaceLifecycleOperation.Delete =>
                    OpenVisionRecipeText.Local("삭제됨: {0}", "Deleted: {0}"),
                _ => string.Empty
            };
            return string.IsNullOrEmpty(format)
                ? string.Empty
                : string.Format(
                    CultureInfo.CurrentCulture,
                    format,
                    recipeName ?? string.Empty);
        }
    }

    internal sealed class OpenVisionRecipeWorkspaceLifecycleProjection
    {
        private OpenVisionRecipeWorkspaceLifecycleProjection(
            bool succeeded,
            string recipeName,
            string statusText)
        {
            Succeeded = succeeded;
            RecipeName = recipeName ?? string.Empty;
            StatusText = statusText ?? string.Empty;
        }

        internal bool Succeeded { get; }
        internal string RecipeName { get; }
        internal string StatusText { get; }

        internal static OpenVisionRecipeWorkspaceLifecycleProjection Success(
            string recipeName,
            string statusText)
        {
            return new OpenVisionRecipeWorkspaceLifecycleProjection(
                true,
                recipeName,
                statusText);
        }

        internal static OpenVisionRecipeWorkspaceLifecycleProjection Failure(
            string statusText)
        {
            return new OpenVisionRecipeWorkspaceLifecycleProjection(
                false,
                string.Empty,
                statusText);
        }
    }
}
