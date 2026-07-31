namespace OpenVisionLab
{
    [System.Xml.Serialization.XmlRoot("CData")]
    public class DataState
    {
        public DataState LoadConfig(string recipeName)
        {
            return RecipeDataStorage.Load(recipeName, this);
        }

        public void SaveConfig(string recipeName)
        {
            RecipeDataStorage.Save(recipeName, this);
        }
    }
}
