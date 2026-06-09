using Lib.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using OpenVisionLab.Logging;

namespace OpenVisionLab
{
    [XmlRoot("SYSTEM")]
    public class SystemState
    {
        #region Events
        public event EventHandler EventChangedAuthorization;
        public event EventHandler EventChangedMenu;
        #endregion

        #region Enums
        public enum RunMode { READY, AUTO, ALARM, SIMULATION };
        public enum MenuKind { MAIN, VISION, MOTION };
        #endregion

        #region Fields
        private const string XmlName = "SYSTEM";
        private bool m_IsInitialized = false;
        private string m_strLastRecipe = "SETUP001";
        private string m_strNotice = "-";
        private RunMode m_eMode = RunMode.READY;
        private MenuKind m_SelectedMenu = MenuKind.MAIN;
        private DEFINE.AUTHORIZATION m_Authorization = DEFINE.AUTHORIZATION.OPERATOR;
        #endregion

        #region Properties
        [XmlIgnore]
        public RunMode Mode
        {
            get { return m_eMode; }
            set
            {
                m_eMode = value;
            }
        }

        [XmlIgnore]
        public MenuKind Menu
        {
            get { return m_SelectedMenu; }
            set
            {
                m_SelectedMenu = value;
                EventChangedMenu?.Invoke(this, EventArgs.Empty);
            }
        }

        public string LastRecipe
        {
            get => m_strLastRecipe;
            set
            {
                LastRecipeUpdateTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
                m_strLastRecipe = value;
            }
        }

        public string LastRecipeUpdateTime { get; set; }

        [Category("LogConfig")]
        [DisplayName("LogConfig")]
        public LogConfig LogConfig { get; set; } = new LogConfig();

        [XmlIgnore]
        public string Notice
        {
            get { return m_strNotice; }
            set
            {
                m_strNotice = value;
            }
        }

        [XmlIgnore]
        public DEFINE.AUTHORIZATION Authorization
        {
            get => m_Authorization;
            set
            {
                m_Authorization = value;
                EventChangedAuthorization?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Initialization
        public SystemState()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (m_IsInitialized)
            {
                return;
            }

            EnsureRuntimeDirectories();
            LoadConfig();
            m_IsInitialized = true;
        }

        public void ApplyLogConfig()
        {
            LogConfig = LogConfig?.Normalize() ?? new LogConfig().Normalize();
            Directory.CreateDirectory(LogConfig.LogDirectory);

            CLog.ApplyFilePolicy(
                LogConfig.LogDirectory,
                LogConfig.MaxBackupFileCount,
                LogConfig.MaximumFileSizeMB);

            CLog.ApplyRetentionPolicy(
                LogConfig.LogDirectory,
                LogConfig.RetentionDays);
        }

        private void EnsureRuntimeDirectories()
        {
            RecipeWorkspaceService.EnsureRoot();
            AppPathService.EnsureDirectory("CONFIG");
        }
        #endregion

        #region Config
        public bool LoadConfig()
        {
            string strPath = RecipeWorkspaceService.GetSystemConfigPath(XmlName);
            SystemStateConfig loaded = SerializeHelper.LoadOrCreateXmlFile(strPath, CreateConfig(), out bool isLoaded);
            LastRecipe = loaded.LastRecipe;
            LastRecipeUpdateTime = loaded.LastRecipeUpdateTime;
            LogConfig = loaded.LogConfig?.Normalize() ?? new LogConfig().Normalize();
            return isLoaded;
        }

        public bool SaveConfig()
        {
            string strPath = RecipeWorkspaceService.GetSystemConfigPath(XmlName);
            return SerializeHelper.SaveXmlFile(strPath, CreateConfig());
        }

        private SystemStateConfig CreateConfig()
        {
            return new SystemStateConfig
            {
                LastRecipe = LastRecipe,
                LastRecipeUpdateTime = LastRecipeUpdateTime,
                LogConfig = LogConfig?.Normalize() ?? new LogConfig().Normalize()
            };
        }
        #endregion
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LogConfig
    {
        #region Constants
        private const int DefaultMaximumFileSizeMB = 100;
        private const int DefaultMaxBackupFileCount = 10000;
        private const int DefaultRetentionDays = 180;
        #endregion

        #region Properties
        [Category("로그 경로")]
        [DisplayName("로그 저장 경로")]
        public string LogDirectory { get; set; } = GetDefaultLogDirectory();

        [Category("로그 보관 정책")]
        [DisplayName("로그 파일 최대 크기(MB)")]
        public int MaximumFileSizeMB { get; set; } = DefaultMaximumFileSizeMB;

        [Category("로그 보관 정책")]
        [DisplayName("최대 백업 파일 개수")]
        public int MaxBackupFileCount { get; set; } = DefaultMaxBackupFileCount;

        [Category("로그 보관 정책")]
        [DisplayName("로그 보관 기간(일)")]
        public int RetentionDays { get; set; } = DefaultRetentionDays;
        #endregion

        #region Methods
        public LogConfig Normalize()
        {
            if (string.IsNullOrWhiteSpace(LogDirectory))
            {
                LogDirectory = GetDefaultLogDirectory();
            }

            LogDirectory = Environment.ExpandEnvironmentVariables(LogDirectory.Trim());
            if (!Path.IsPathRooted(LogDirectory))
            {
                LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogDirectory);
            }

            LogDirectory = Path.GetFullPath(LogDirectory);
            MaximumFileSizeMB = Math.Max(1, MaximumFileSizeMB);
            MaxBackupFileCount = Math.Max(1, MaxBackupFileCount);
            RetentionDays = Math.Max(1, RetentionDays);
            return this;
        }

        public override string ToString()
        {
            return LogDirectory;
        }

        private static string GetDefaultLogDirectory()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
        }
        #endregion
    }

    [XmlRoot("SYSTEM")]
    public class SystemStateConfig
    {
        #region Properties
        public string LastRecipe { get; set; } = "SETUP001";
        public string LastRecipeUpdateTime { get; set; } = "";
        public LogConfig LogConfig { get; set; } = new LogConfig();
        #endregion
    }
}
