using RJCodeUI_M1.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    public static class DEFINE
    {
        //로딩레일리프트? 플리퍼?
        public const string ALARM_CODE_DEVICE_LOST = "-30";

        //TimeOut 100~
        public const string ALARM_TIME_OUT = "100";
        public const string ALARM_TIME_OUT_AXIS_MOVE = "110";
        public const string ALARM_TIME_OUT_AXIS_HOME = "120";
        public const string ALARM_RESET_TIME_OUT = "130";
        public const string ALARM_TIME_OUT_CONV = "140";

        //SYSTEM 300~
        public const string ALARM_SYSTEM = "300";
        public const string ALARM_REGIST_NULL = "310";
        public const string ALARM_MODE_MANAUL = "320";
        public const string ALARM_CODESYS_ERROR = "330";
        public const string ALARM_UI = "340";

        public const string ALARM_SYSTEM_EQ = "350";
        public const string ALARM_SYSTEM_MAIN_AIR = "351";
        public const string ALARM_SYSTEM_SMOKE = "352";
        public const string ALARM_SYSTEM_MAIN_POWER = "353";        
        public const string ALARM_SYSTEM_DOOR_OPEN = "355";

        public const string ALARM_SYSTEM_SERVO_POWER_AXIS_0 = "356";
        public const string ALARM_SYSTEM_SERVO_POWER_AXIS_1 = "357";
        public const string ALARM_SYSTEM_SERVO_POWER_AXIS_2 = "358";
        public const string ALARM_SYSTEM_SERVO_POWER_AXIS_3 = "359";

        public const string ALARM_SYSTEM_HOME = "360";

        //사용여부 확인
        public const string ALARM_DEVICE_CONNECTION = "340";

        //LOT OPEN  400~
        public const string ALARM_LOT_OPEN_FAIL = "400";

        public const string ALARM_모재부족 = "410";
        public const string ALARM_테이프부족 = "420";
        public const string ALARM_테이크업교체 = "430";
        

        // DoorOpen InterLock 500~
        public const string ALARM_INTERLOCK_DOOR = "500";
        public const string ALARM_INTERLOCK_DOOR_OPEN_TOP = "510";
        public const string ALARM_INTERLOCK_DOOR_OPEN_BTM = "520";

        //실린더 인터락
        public const string ALARM_INTERLOCK_CYL = "550";

        //커버 트레이 인터락
        public const string ALARM_UNLOADING_COVER_TRAY_NA = "560";

        //Vision 600~\
        //사용안함
        public const string ALARM_VISION_REJECT = "600";
        public const string AlARM_VISION_NG = "605";
        public const string ALARM_VISION_RETRY = "610";
        public const string ALARM_CODE_VISION_SOURCE_EMPTY = "620";
        public const string ALARM_CODE_VISION_TEMPLATE_EMPTY = "630";

        //Work Done Buffer 800~
        public const string ALARM_WORK_DONE_BUFFER = "800";

        //Work Picker 900~
        public const string ALARM_WORK_PICKER = "900";

        //Flipper 1  1000~
        public const string ALARM_FLIPPER_1ST = "1000";

        //Flipper 2  1100~
        public const string ALARM_FLIPPER_2ND = "1100";

        //REJECT 시퀀스
        public const string ALARM_REJECT_WORK_FLIPPER = "-10000";
        public const string ALARM_REJECT_WORK_RAIL = "-10010";

        public const string Axis_0 = "헤드 - Rotation (1:1)";
        public const string Axis_1 = "캡스탄 - Rotation (1:10)";
        public const string Axis_2 = "트래버스 - Y (1:10)";
        public const string Axis_3 = "테이크업 - Rotation (1:10)";

        public const string GRAPH_INDEX_NAME_A_ONE = "Thickness";
        public const string GRAPH_INDEX_NAME_A_TWO = "Pitch";
        public const string GRAPH_INDEX_NAME_B_ONE = "Angle";
        public const string GRAPH_INDEX_NAME_B_TWO = "INDEX #B-2";
        public const string GRAPH_INDEX_NAME_C_ONE = "INDEX #C-1";
        public const string GRAPH_INDEX_NAME_C_TWO = "INDEX #C-2";
        public const string GRAPH_INDEX_NAME_D_ONE = "INDEX #D-1";
        public const string GRAPH_INDEX_NAME_D_TWO = "INDEX #D-2";

        public const int GRAPH_INDEX_A_ONE = 0;
        public const int GRAPH_INDEX_A_TWO = 1;
        public const int GRAPH_INDEX_B_ONE = 2;
        public const int GRAPH_INDEX_B_TWO = 3;
        public const int GRAPH_INDEX_C_ONE = 4;
        public const int GRAPH_INDEX_C_TWO = 5;
        public const int GRAPH_INDEX_D_ONE = 6;
        public const int GRAPH_INDEX_D_TWO = 7;

        #region 카메라 인덱스
        public const int CAM_1 = 0;
        public const int CAM_2 = 1;
        public const int CAM_3 = 2;
        #endregion

        public const int Main = 0;
        public const int nThreshold = 1;

        public const int Vision_Morph = 0;
        public const int Vision_Filter = 1;
        public const int Vision_Arithmetic = 2;

        public const int Algorithm_Blob = 3;
        public const int Algorithm_Contour = 4;

        public enum MAIN_DOCK_FORM : int
        {
            MainSystem = 0,
            Defect = 1,
            Graph = 2,
            IO = 3,
            CSV = 4,
            LABEL = 5,
            LOG = 6,
            BUTTON = 7,
            CAM_TOP = 8,
            CAM_BOTTOM = 9,
            SEARCHDB = 10,
            SUMMARY =11,
            PLC = 12
        }

        public enum VISION_DOCK_FORM : int
        {
            System = 0,
            BLOB,
            LINE,
            CONTOUR,
            PROPERTY,
            THRESHOLD,
            TEACHING,
            LOG
        }

        public enum VISION_MENU
        {
            Morphology,
            Filter,
            Arithmetic,
            EdgeDetection,
            Blob,
            Contour,
            Matching,
            Line,
            RotateAndScale,
            Histogram,
            Mean,
            HSV,
            FeatureMatching,
            Pipeline
        }

        public enum PROCESS_TYPES
        {
            Bolt_tightened,
            Pin_Inspection,
            Connector_Inspection,
            ThermalGrease_Inspection
        }

        public enum PIN_INSPECTION_TYPES
        {
            Pin_SpecArea,
            Pin_Distance
        }

        public enum GRID_ROI_COLUMN : int
        {
            X = 1,
            Y = 2,
            WIDTH = 3,
            HEIGHT = 4,
            ALGORITHM = 5,
            USE_BITWISENOT = 6,
            THRESHOLD = 7,
            MIN_AREA = 8,
            MAX_AREA = 9,
            InspCount = 10,
            SCORE = 11,
            USE_ROI = 12

        }

        public enum Labeler : uint {No1 = 0, No2 = 1, No3 = 2, No4 = 3}

        public enum REEL_NO : uint {NO1 = 1, NO2 = 2, NO3 = 3, NO4 = 4 }

        public enum ALGORITHM : uint { BLOB, CONTOUR, MATCING, MEAN };

        public enum FLIP_TYPE : int { X, Y, XY };
        public enum FORM_RESULT : int { IDLE, SKIP, RETRY, REJECT };

        public enum RESULT : int
        {
            NA = 0,
            OK = 1,
            T_NG = 2,
            L_NG = 3,
            RVS = 4,
            NG,
            NG_Pin_SpecArea,
            NG_Pin_Distance

        }

        public enum AUTHORIZATION : uint { OPERATOR = 1, ENGINEER, MASTER };

        public const int FILRM_LEFT = 0;
        public const int FILRM_RIGHT = 1;
        public const int RECHECK_LEFT = 2;
        public const int RECHECK_RIGHT = 3;

        public const int GLASS_LEFT = 0;
        public const int GLASS_RIGHT = 1;

        public const double PIXEL_RESOLUTION_UM = 4.3125;
        public const double PIXEL_RESOLUTION_MM = 0.01750;
        public const double PIXEL_RESOLUTION_ETCHING_MM = 0.005;

        public static System.Drawing.Color MOUSEHOVER_COLOR = System.Drawing.Color.FromArgb(83, 97, 212);
        public static System.Drawing.Color BACK_COLOR = System.Drawing.Color.FromArgb(49, 42, 81);
        //public static System.Drawing.Color BACK_COLOR = System.Drawing.Color.FromArgb(24, 24, 36);

        public static System.Drawing.Color ButtonColorBlue = System.Drawing.Color.FromArgb(83, 97, 212);
        public static System.Drawing.Color ButtonColorRed = System.Drawing.Color.FromArgb(234, 79, 82);

        public static System.Drawing.Color COLOR_TEAL = System.Drawing.Color.FromArgb(0, 170, 173);
        public static System.Drawing.Color COLOR_RED = System.Drawing.Color.FromArgb(209, 17, 65);
        public static System.Drawing.Color COLOR_SECOND = System.Drawing.Color.FromArgb(41, 41, 76);

        public const string PATH_RECIPE = "RECIPE";

        public const int ROI_RB = 0;
        public const int ROI_LB = 1;
        public const int ROI_RT = 2;
        public const int ROI_LT = 3;

        public const string Grab = "GRAB";
        public const string Live = "LIVE";
        public const string Live_Stop = "LIVE STOP";
        public const string Image_Load = "IMAGE LOAD";
        public const string Image_Save = "IMAGE SAVE";

        public const string Line = "Line";
        public const string Blob = "Blob";
        public const string Pattern = "Pattern";
        public const string Filter = "Filter";
        public const string MultyPly = "MultyPly";
        public const string Threshold = "Threshold";
        public const string AdaptiveThreshold = "AdaptiveThreshold";

        public const int ImageW = 8192;
        public const int ImageH = 4000;        

        //public const int ALIGN_Panel_LEFT = 0;
        //public const int ALIGN_Panel_RIGHT = 1;
        public const int ALIGN_Filrm_LEFT = 0;
        public const int ALIGN_Filrm_RIGHT = 1;
        public const int ALIGN_FilrmReCheck_LEFT = 2;
        public const int ALIGN_FilrmReCheck_RIGHT =3;

        //슬라이딩 메뉴의 최대, 최소 폭 크기
        public static int MAX_SLIDING_WIDTH = 1841;
        public static int MIN_SLIDING_WIDTH = 1721;
        //슬라이딩 메뉴가 보이는/접히는 속도 조절
        public const int STEP_SLIDING = 10;
        //최초 슬라이딩 메뉴 크기
        public const int _posSliding = 200;

        public  enum MOVE_POS : int
        {
            POS_LOADER_LAPPING_R = 0,
            POS_MAIN_R = 1,
            POS_BACK_AND_FORTH_Y = 2,
            POS_LAPPING_R = 3
        }

        public enum TapeType
        {
            동,
            은,
            금
        }

    }
}
