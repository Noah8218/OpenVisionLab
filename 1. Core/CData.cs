namespace OpenVisionLab
{
    public class CData
    {
        public CData LoadConfig(string recipeName)
        {
            return RecipeDataStorage.Load(recipeName, this);
        }

        public void SaveConfig(string recipeName)
        {
            RecipeDataStorage.Save(recipeName, this);
        }
    }
}
