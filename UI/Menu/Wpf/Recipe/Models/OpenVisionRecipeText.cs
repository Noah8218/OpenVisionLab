namespace OpenVisionLab
{
    internal static class OpenVisionRecipeText
    {
        public static string Local(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean ? korean : english;
        }
    }
}
