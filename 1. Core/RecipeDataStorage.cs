using Lib.Common;
using System.IO;

namespace OpenVisionLab
{
    internal static class RecipeDataStorage
    {
        public static CData Load(string recipeName, CData defaultData)
        {
            string path = RecipeWorkspaceService.GetVisionDataPath(recipeName);

            if (File.Exists(path))
            {
                CData loadedData = SerializeHelper.FromXmlFile<CData>(path);
                if (loadedData != null)
                {
                    return loadedData;
                }
            }

            CData data = defaultData ?? new CData();
            Save(recipeName, data);
            return data;
        }

        public static void Save(string recipeName, CData data)
        {
            string path = RecipeWorkspaceService.GetVisionDataPath(recipeName);
            SerializeHelper.ToXmlFile(path, data ?? new CData());
        }
    }
}
