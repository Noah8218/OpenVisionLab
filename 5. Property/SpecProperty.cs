using System.ComponentModel;

namespace OpenVisionLab
{
    [System.Xml.Serialization.XmlRoot("CPropertySpec")]
    public class SpecProperty
    {
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "";    

        [CategoryAttribute("Length"), DescriptionAttribute(""), DisplayNameAttribute("DIST_MIN_MM")]
        public double DIST_MIN_MM { get; set; } = 0.5;

        [CategoryAttribute("Length"), DescriptionAttribute(""), DisplayNameAttribute("DIST_MAX_MM")]
        public double DIST_MAX_MM { get; set; } = 2;

        public SpecProperty(string strName)
        {
            NAME = strName;
        }

        public SpecProperty()
        {
            
        }

        public SpecProperty LoadConfig(string RecipeName)
        {
            string strPath = RecipeWorkspaceService.GetRecipeFilePath(RecipeName, NAME);
            SpecProperty loaded = SerializeHelper.LoadOrCreateXmlFile(strPath, this, out _);
            if (string.IsNullOrWhiteSpace(loaded.NAME))
            {
                loaded.NAME = NAME;
            }

            return loaded;
        }

        public void SaveConfig(string RecipeName)
        {
            string strPath = RecipeWorkspaceService.GetRecipeFilePath(RecipeName, NAME);
            SerializeHelper.SaveXmlFile(strPath, this);
        }

    }
}
