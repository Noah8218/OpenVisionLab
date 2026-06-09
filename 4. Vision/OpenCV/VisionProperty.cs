using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.WpfPropertyGrid;
using System.Windows.Forms;
using System.Xml.Linq;

namespace OpenVisionLab.Vision._1._Tools.OpenCV
{
    [CategoryOrder("Parameter", 0)]
    [CategoryOrder("Normalize", 1)]
    [System.Xml.Serialization.XmlRoot("CPropertyVision")]
    public class VisionProperty
    {
        public VisionProperty(string strName) : base()
        {
            NAME = strName;
        }

        public VisionProperty() { }    

        [PropertyOrder(0)]
        [Browsable(true)]
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]        
        public string NAME { get; set; } = "";

        [PropertyOrder(1)]
        [Browsable(true)]
        [CategoryAttribute("Normalize"), DescriptionAttribute("이미지에 노말라이즈를 적용 후 검사할지 결정합니다."), DisplayNameAttribute("USE NORMALIZE")]
        public bool USE_NORMALIZE { get; set; } = true;

        [PropertyOrder(2)]
        [Browsable(true)]
        [CategoryAttribute("Normalize"), DescriptionAttribute("노말라이즈 최소 gv값입니다."), DisplayNameAttribute("Alpha")]
        public int Alpha { get; set; } = 30;

        [PropertyOrder(3)]
        [Browsable(true)]
        [CategoryAttribute("Normalize"), DescriptionAttribute("노말라이즈 최대 gv값입니다."), DisplayNameAttribute("Beta")]
        public int Beta { get; set; } = 255;

        public VisionProperty DeepCopy()
        {
            VisionProperty temp = (VisionProperty)this.MemberwiseClone();
            return temp;
        }

        #region CONFIG BY XML              
        public VisionProperty LoadConfig(string RecipeName)
        {
            string strPath = RecipeWorkspaceService.GetVisionConfigPath(RecipeName, NAME);
            VisionProperty loaded = SerializeHelper.LoadOrCreateXmlFile(strPath, this, out _);
            loaded.NAME = NAME;
            return loaded;
        }

        public void SaveConfig(string RecipeName)
        {
            string strPath = RecipeWorkspaceService.GetVisionConfigPath(RecipeName, NAME);
            SerializeHelper.SaveXmlFile(strPath, this);
        }
        #endregion
    }
}
