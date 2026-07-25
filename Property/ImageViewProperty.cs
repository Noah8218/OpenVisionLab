using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using Lib.Common;

namespace OpenVisionLab
{
    [XmlRoot("PROPERTY")]
    public class ImageViewProperty
    {
        [XmlIgnore]
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "";

        [CategoryAttribute("Matrix"), DescriptionAttribute(""), DisplayNameAttribute("Rows")]
        public int ROWS { get; set; } = 10;

        [CategoryAttribute("Matrix"), DescriptionAttribute(""), DisplayNameAttribute("Columns")]
        public int COLUMNS { get; set; } = 10;

        [XmlIgnore]
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("EMPTY")]
        public string EMPTY { get; set; } = "EMPTY";

        public ImageViewProperty()
        {
        }

        public ImageViewProperty(string strName)
        {
            NAME = strName;
        }


        #region CONFIG BY XML              
        public bool LoadConfig(string strRecipeName)
        {
                        string strPath = RecipeWorkspaceService.GetRecipeFilePath(strRecipeName, NAME);
            string name = NAME;
            ImageViewProperty loaded = SerializeHelper.LoadOrCreateXmlFile(strPath, this, out bool isLoaded);
            ROWS = loaded.ROWS;
            COLUMNS = loaded.COLUMNS;
            NAME = name;

            return isLoaded;
        
        }
        public bool SaveConfig(string strRecipeName)
        {
            string strPath = RecipeWorkspaceService.GetRecipeFilePath(strRecipeName, NAME);
            return SerializeHelper.SaveXmlFile(strPath, this);
        }
        #endregion

    }
}
