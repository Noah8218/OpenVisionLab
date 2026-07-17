using System;
using System.Reflection;
using System.Xml.Serialization;
using Lib.Common;

namespace OpenVisionLab
{
    public class RecipeState
    {        
        [XmlIgnore] public EventHandler<EventArgs> EventChangedRecipe;

        [XmlIgnore]
        public EventHandler<EventArgs> EventChagedRecipe
        {
            get => EventChangedRecipe;
            set => EventChangedRecipe = value;
        }

        [XmlIgnore] private string m_strName = "";
        [XmlIgnore] private Func<DataState> dataAccessor;
        [XmlIgnore] private Action<DataState> dataSetter;
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

                m_strName = recipeName;
                UpdateModelInfo();

                if (string.IsNullOrWhiteSpace(m_strName))
                {
                    return;
                }

                RecipeWorkspaceService.EnsureVisionWorkspace(m_strName);
                LoadTools();
                EventChangedRecipe?.Invoke(this, EventArgs.Empty);
            }
        }

        public RecipeState() { }

        public void SetRuntime(Func<DataState> dataAccessor, Action<DataState> dataSetter, Func<VisionToolRepository> visionToolAccessor)
        {
            this.dataAccessor = dataAccessor ?? this.dataAccessor;
            this.dataSetter = dataSetter ?? this.dataSetter;
            this.visionToolAccessor = visionToolAccessor ?? this.visionToolAccessor;
        }

        public string ModelNo { get; set; } = "";
        public string ModelName { get; set; } = "";

        public bool LoadTools()
        {
                        
            RecipeRuntimeStorage.Load(Name, dataAccessor, dataSetter, visionToolAccessor);

            return true;
        
        }

        public bool SaveTools()
        {
                        
            RecipeRuntimeStorage.Save(Name, dataAccessor, visionToolAccessor);
            return true;
        
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
