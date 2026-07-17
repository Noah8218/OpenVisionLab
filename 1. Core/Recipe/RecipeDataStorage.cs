namespace OpenVisionLab
{
    internal static class RecipeDataStorage
    {
        public static DataState Load(string recipeName, DataState defaultData)
        {
            string path = RecipeWorkspaceService.GetVisionDataPath(recipeName);
            DataState data = defaultData ?? new DataState();
            return SerializeHelper.LoadOrCreateXmlFile(path, data, out _);
        }

        public static void Save(string recipeName, DataState data)
        {
            string path = RecipeWorkspaceService.GetVisionDataPath(recipeName);
            SerializeHelper.SaveXmlFile(path, data ?? new DataState());
        }
    }
}
