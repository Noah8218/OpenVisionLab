using System;
using System.Reflection;
using System.Xml.Serialization;
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

        [XmlIgnore] private string m_strName = "";

        [XmlIgnore] private string m_strNamePrev = "";
        [XmlIgnore] private Func<CData> dataAccessor;
        [XmlIgnore] private Action<CData> dataSetter;
        [XmlIgnore] private Func<VisionToolRepository> visionToolAccessor;

        [XmlIgnore]
        public string Name
        {
            get { return m_strName; }
            set
            {
                string recipeName = value ?? string.Empty;
                if (m_strName == recipeName)
                {
                    return;
                }

                m_strNamePrev = m_strName;
                m_strName = recipeName;
                UpdateModelInfo();

                if (string.IsNullOrWhiteSpace(m_strName))
                {
                    return;
                }

                RecipeWorkspaceService.EnsureVisionWorkspace(m_strName);
                LoadTools();
                EventChagedRecipe?.Invoke(this, EventArgs.Empty);
            }
        }

        public CRecipe() { }

        public void SetRuntime(Func<CData> dataAccessor, Action<CData> dataSetter, Func<VisionToolRepository> visionToolAccessor)
        {
            this.dataAccessor = dataAccessor ?? this.dataAccessor;
            this.dataSetter = dataSetter ?? this.dataSetter;
            this.visionToolAccessor = visionToolAccessor ?? this.visionToolAccessor;
        }

        public string ModelNo { get; set; } = "";
        public string ModelName { get; set; } = "";

        public bool LoadTools()
        {
            try
            {                
                GetVisionTools().LoadTools(Name);
                dataSetter(GetData().LoadConfig(Name));

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
                GetVisionTools().SaveTools(Name);
                GetData().SaveConfig(Name);
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        private CData GetData()
        {
            if (dataAccessor == null)
            {
                throw new InvalidOperationException("Recipe runtime is not configured.");
            }

            return dataAccessor();
        }

        private VisionToolRepository GetVisionTools()
        {
            if (visionToolAccessor == null)
            {
                throw new InvalidOperationException("Vision tool runtime is not configured.");
            }

            return visionToolAccessor();
        }

        private void UpdateModelInfo()
        {
            if (m_strName.Length < 3)
            {
                ModelName = m_strName;
                ModelNo = string.Empty;
                return;
            }

            ModelName = m_strName.Substring(0, m_strName.Length - 3);
            ModelNo = m_strName.Substring(m_strName.Length - 3);
        }
    }
}
