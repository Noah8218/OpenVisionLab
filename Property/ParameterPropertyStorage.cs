namespace OpenVisionLab
{
    internal static class ParameterPropertyStorage
    {
        public static ParameterProperty Load(ParameterProperty property, string recipeName)
        {
            string path = RecipeWorkspaceService.GetRecipeFilePath(recipeName, property.NAME);
            string name = property.NAME;
            ParameterProperty loaded = SerializeHelper.LoadOrCreateXmlFile(path, property, out _);
            loaded.NAME = name;
            LoadChildConfigs(loaded, recipeName);
            return loaded;
        }

        public static void Save(ParameterProperty property, string recipeName)
        {
            string path = RecipeWorkspaceService.GetRecipeFilePath(recipeName, property.NAME);
            SerializeHelper.SaveXmlFile(path, property);
            SaveChildConfigs(property, recipeName);
        }

        private static void SaveChildConfigs(ParameterProperty property, string recipeName)
        {
            property.Matching ??= new MatchingProperty();
            property.Matching.NAME = $"{property.NAME}-Matching";
            property.Matching.SaveConfig(recipeName);

            property.Blob ??= new BlobProperty();
            property.Blob.NAME = $"{property.NAME}-Blob";
            property.Blob.SaveConfig(recipeName);

            property.Line_1 ??= new LineGaugeProperty();
            property.Line_1.NAME = $"{property.NAME}-Line_1";
            property.Line_1.SaveConfig(recipeName);

            property.Line_2 ??= new LineGaugeProperty();
            property.Line_2.NAME = $"{property.NAME}-Line_2";
            property.Line_2.SaveConfig(recipeName);

            property.Mean ??= new MeanProperty();
            property.Mean.NAME = $"{property.NAME}-Mean";
            property.Mean.SaveConfig(recipeName);
        }

        private static void LoadChildConfigs(ParameterProperty property, string recipeName)
        {
            property.Matching ??= new MatchingProperty();
            property.Matching.NAME = $"{property.NAME}-Matching";
            property.Matching = (MatchingProperty)property.Matching.LoadConfig(recipeName);

            property.Blob ??= new BlobProperty();
            property.Blob.NAME = $"{property.NAME}-Blob";
            property.Blob = property.Blob.LoadConfig(recipeName);

            property.Line_1 ??= new LineGaugeProperty();
            property.Line_1.NAME = $"{property.NAME}-Line_1";
            property.Line_1 = (LineGaugeProperty)property.Line_1.LoadConfig(recipeName);

            property.Line_2 ??= new LineGaugeProperty();
            property.Line_2.NAME = $"{property.NAME}-Line_2";
            property.Line_2 = (LineGaugeProperty)property.Line_2.LoadConfig(recipeName);

            property.Mean ??= new MeanProperty();
            property.Mean.NAME = $"{property.NAME}-Mean";
            property.Mean = property.Mean.LoadConfig(recipeName);
        }
    }
}
