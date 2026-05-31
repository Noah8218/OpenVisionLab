using System;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Lib.Common;

namespace OpenVisionLab
{
    public class CRecipe
    {        
        // xml 저장과 상관 없는 데이터 분리. => XmlIgnore
        // 설계에서 설정값과 runtime에 관리하는 data를 분리할 것.        
        [XmlIgnore] public EventHandler<EventArgs> EventChagedRecipe;

        // xml serialize는 property 만 가능합니다.
        // 변수 선언하고 alt + enter로 property를 자동 생성 하세요. => 필드 캡슐화
        // 간단한건 attribute로. 너무 많을 땐 element로
        // 필요없는건 XmlIgnore

        //int, double, bool 등 일반 자료형일 경우 애트리뷰트로 선언하는게 바람직
        // 예 <IData MAX_CELL_COUNT="10"/>
        //[XmlAttribute]

        /*
         * <root> // 앨리먼트
                 <Count> 1 </Count> // 애트리뷰트
            </root>
         * 
         * */

        // 복수일떄 s를 붙히는게 바람직
        //[XmlArray("HeadUseInfos")]
        //[XmlArrayItem("HeadUseInfo")]

        [XmlIgnore] private const string RECIPE_DIRECTORY = "RECIPE";

        [XmlIgnore] private string m_strName = "";

        [XmlIgnore] private string m_strNamePrev = "";

        [XmlIgnore]
        public string Name
        {
            get { return m_strName; }
            set
            {
                Task LoadToolsTask = null;
                if (m_strName != value)
                {
                    m_strNamePrev = m_strName;
                    m_strName = value;

                    try
                    {
                        ModelName = Name.Substring(0, Name.Length - 3);
                        ModelNo = Name.Substring(Name.Length - 3);
                    }
                    catch { }

                    CUtil.InitDirectory($"RECIPE\\{m_strName}\\VISION");
                    CUtil.InitDirectory($"RECIPE\\{m_strName}\\DEVICE");
                    CUtil.InitDirectory($"RECIPE\\{m_strName}\\MOTION");

                    LoadToolsTask = Task.Run(() =>
                    {
                        LoadTools();
                    });
                }

                if (m_strName != "")
                {
                    Task.WaitAll(LoadToolsTask);
                    InitDirectory(m_strName);

                    if (EventChagedRecipe != null) { EventChagedRecipe(null, null); }                  
                }               
            }
        }

        public CRecipe() { }        

        public string ModelNo { get; set; } = "";
        public string ModelName { get; set; } = "";

        public bool LoadTools()
        {
            try
            {                
                CVisionTools.LoadTools(Name);
                CGlobal.Inst.Device.LoadDevices(Name);
                CGlobal.Inst.Data = CGlobal.Inst.Data.LoadConfig(Name);

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public bool SaveTools()
        {
            try
            {                
                CVisionTools.SaveTools(Name);
                CGlobal.Inst.Data.SaveConfig(Name);
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public static bool InitDirectory(string strRecipeName)
        {
            try
            {
                string strRecipePath = Application.StartupPath + "\\" + RECIPE_DIRECTORY + "\\";
                DirectoryInfo dirRecipe = new DirectoryInfo(strRecipePath);
                if (dirRecipe.Exists == false) dirRecipe.Create();

                string strRecipeNamePath = Application.StartupPath + "\\" + RECIPE_DIRECTORY + "\\" + strRecipeName;
                DirectoryInfo dirRecipeName = new DirectoryInfo(strRecipeNamePath);
                if (dirRecipeName.Exists == false) dirRecipeName.Create();

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }        
    }
}
