using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KtemVisionSystem
{
    public class CTimeOutManager
    {

        #region LIST
        public Dictionary<string, CPropertyTimeOut> List = new Dictionary<string, CPropertyTimeOut>();

        public CPropertyTimeOut LOADING_FEED_SEPARATION;
        public CPropertyTimeOut LOADING_FEED_CONV_SENSING;
        public CPropertyTimeOut LOADING_IN_SENSING;
        public CPropertyTimeOut LOADING_IN_STOPPER;
        public CPropertyTimeOut LOADING_OUT_SENSING;
        public CPropertyTimeOut LOADING_OUT_STOPPER;
        public CPropertyTimeOut LOADING_LIFT_UP;
        public CPropertyTimeOut LOADING_LIFT_DOWN;
        public CPropertyTimeOut UNLOADING_LIFT_UP;
        public CPropertyTimeOut UNLOADING_LIFT_DOWN;
        public CPropertyTimeOut UNLOADING_OUT_SENSING;
        public CPropertyTimeOut UNLOADING_STACK_SENSING;
        public CPropertyTimeOut UNLOADING_STACK_LIFT_UP;
        public CPropertyTimeOut UNLOADING_STACK_LIFT_DOWN;
        public CPropertyTimeOut WORK_DONE_FEED_FWD;
        public CPropertyTimeOut WORK_DONE_LOCK_FWD;
        public CPropertyTimeOut WORK_DONE_LOCK_BKD;
        public CPropertyTimeOut WORK_DONE_LIFT_UP;
        public CPropertyTimeOut WORK_DONE_LIFT_DOWN;
        public CPropertyTimeOut WORK_DONE_IN_SENSING;
        public CPropertyTimeOut WORK_DONE_BUFFER_OUT_SENSING;
        public CPropertyTimeOut METAL_TRAY_LOADING_FWD;
        public CPropertyTimeOut METAL_TRAY_LOADING_BKD;
        public CPropertyTimeOut METAL_TRAY_UNLOADING_FWD;
        public CPropertyTimeOut METAL_TRAY_UNLOADING_BKD;
        public CPropertyTimeOut METAL_TRAY_UNLOADING_UP;
        public CPropertyTimeOut METAL_TRAY_UNLOADING_DOWN;
        public CPropertyTimeOut METAL_TRAY_ID_POS_SENSING;
        public CPropertyTimeOut METAL_TRAY_OUT_SENSING;
        public CPropertyTimeOut METAL_TRAY_LIFT_UP;
        public CPropertyTimeOut METAL_TRAY_LIFT_DOWN;
        public CPropertyTimeOut TOP_COVER_LOADING_FWD;
        public CPropertyTimeOut TOP_COVER_LOADING_BKD;
        public CPropertyTimeOut TOP_COVER_UNLOADING_FWD;
        public CPropertyTimeOut TOP_COVER_UNLOADING_BKD;
        public CPropertyTimeOut TOP_COVER_UNLOADING_UP;
        public CPropertyTimeOut TOP_COVER_UNLOADING_DOWN;
        public CPropertyTimeOut TOP_COVER_ID_POS_SENSING;
        public CPropertyTimeOut TOP_COVER_OUT_SENSING;
        public CPropertyTimeOut TOP_COVER_LIFT_UP;
        public CPropertyTimeOut TOP_COVER_LIFT_DOWN;
        public CPropertyTimeOut WORK_PICKER_GRIP;
        public CPropertyTimeOut WORK_PICKER_UNGRIP;
        public CPropertyTimeOut LOADING_FLIPPER_LIFT_UP;
        public CPropertyTimeOut LOADING_FLIPPER_LIFT_DOWN;
        public CPropertyTimeOut LOADING_FLIPPER_CLAMP_FWD;
        public CPropertyTimeOut LOADING_FLIPPER_CLAMP_BKD;
        public CPropertyTimeOut LOADING_FLIPPER_NARROW;
        public CPropertyTimeOut LOADING_FLIPPER_WIDE;
        public CPropertyTimeOut LOADING_FLIPPER_ROTATE_0;
        public CPropertyTimeOut LOADING_FLIPPER_ROTATE_180;
        public CPropertyTimeOut LOADING_VISION_WAIT;
        public CPropertyTimeOut UNLOADING_FLIPPER_LIFT_UP;
        public CPropertyTimeOut UNLOADING_FLIPPER_LIFT_DOWN;
        public CPropertyTimeOut UNLOADING_FLIPPER_CLAMP_FWD;
        public CPropertyTimeOut UNLOADING_FLIPPER_CLAMP_BKD;
        public CPropertyTimeOut UNLOADING_FLIPPER_NARROW;
        public CPropertyTimeOut UNLOADING_FLIPPER_WIDE;
        public CPropertyTimeOut UNLOADING_FLIPPER_ROTATE_0;
        public CPropertyTimeOut UNLOADING_FLIPPER_ROTATE_180;
        public CPropertyTimeOut UNLOADING_VISION_WAIT;
        #endregion

        public CTimeOutManager()
        {
        }

        public bool Init()
        {
            try
            {
                List.Clear();

                LOADING_FEED_SEPARATION = new CPropertyTimeOut("LOADING_FEED_SEPARATION");
                List.Add("LOADING_FEED_SEPARATION", LOADING_FEED_SEPARATION);
                LOADING_FEED_CONV_SENSING = new CPropertyTimeOut("LOADING_FEED_CONV_SENSING");
                List.Add("LOADING_FEED_CONV_SENSING", LOADING_FEED_CONV_SENSING);
                LOADING_IN_SENSING = new CPropertyTimeOut("LOADING_IN_SENSING");
                List.Add("LOADING_IN_SENSING", LOADING_IN_SENSING);
                LOADING_IN_STOPPER = new CPropertyTimeOut("LOADING_IN_STOPPER");
                List.Add("LOADING_IN_STOPPER", LOADING_IN_STOPPER);
                LOADING_OUT_SENSING = new CPropertyTimeOut("LOADING_OUT_SENSING");
                List.Add("LOADING_OUT_SENSING", LOADING_OUT_SENSING);
                LOADING_OUT_STOPPER = new CPropertyTimeOut("LOADING_OUT_STOPPER");
                List.Add("LOADING_OUT_STOPPER", LOADING_OUT_STOPPER);
                LOADING_LIFT_UP = new CPropertyTimeOut("LOADING_LIFT_UP");
                List.Add("LOADING_LIFT_UP", LOADING_LIFT_UP);
                LOADING_LIFT_DOWN = new CPropertyTimeOut("LOADING_LIFT_DOWN");
                List.Add("LOADING_LIFT_DOWN", LOADING_LIFT_DOWN);
                UNLOADING_LIFT_UP = new CPropertyTimeOut("UNLOADING_LIFT_UP");
                List.Add("UNLOADING_LIFT_UP", UNLOADING_LIFT_UP);
                UNLOADING_LIFT_DOWN = new CPropertyTimeOut("UNLOADING_LIFT_DOWN");
                List.Add("UNLOADING_LIFT_DOWN", UNLOADING_LIFT_DOWN);
                UNLOADING_OUT_SENSING = new CPropertyTimeOut("UNLOADING_OUT_SENSING");
                List.Add("UNLOADING_OUT_SENSING", UNLOADING_OUT_SENSING);
                UNLOADING_STACK_SENSING = new CPropertyTimeOut("UNLOADING_STACK_SENSING");
                List.Add("UNLOADING_STACK_SENSING", UNLOADING_STACK_SENSING);
                UNLOADING_STACK_LIFT_UP = new CPropertyTimeOut("UNLOADING_STACK_LIFT_UP");
                List.Add("UNLOADING_STACK_LIFT_UP", UNLOADING_STACK_LIFT_UP);
                UNLOADING_STACK_LIFT_DOWN = new CPropertyTimeOut("UNLOADING_STACK_LIFT_DOWN");
                List.Add("UNLOADING_STACK_LIFT_DOWN", UNLOADING_STACK_LIFT_DOWN);
                WORK_DONE_FEED_FWD = new CPropertyTimeOut("WORK_DONE_FEED_FWD");
                List.Add("WORK_DONE_FEED_FWD", WORK_DONE_FEED_FWD);
                WORK_DONE_LOCK_FWD = new CPropertyTimeOut("WORK_DONE_LOCK_FWD");
                List.Add("WORK_DONE_LOCK_FWD", WORK_DONE_LOCK_FWD);
                WORK_DONE_LOCK_BKD = new CPropertyTimeOut("WORK_DONE_LOCK_BKD");
                List.Add("WORK_DONE_LOCK_BKD", WORK_DONE_LOCK_BKD);
                WORK_DONE_LIFT_UP = new CPropertyTimeOut("WORK_DONE_LIFT_UP");
                List.Add("WORK_DONE_LIFT_UP", WORK_DONE_LIFT_UP);
                WORK_DONE_LIFT_DOWN = new CPropertyTimeOut("WORK_DONE_LIFT_DOWN");
                List.Add("WORK_DONE_LIFT_DOWN", WORK_DONE_LIFT_DOWN);
                WORK_DONE_IN_SENSING = new CPropertyTimeOut("WORK_DONE_IN_SENSING");
                List.Add("WORK_DONE_IN_SENSING", WORK_DONE_IN_SENSING);
                WORK_DONE_BUFFER_OUT_SENSING = new CPropertyTimeOut("WORK_DONE_BUFFER_OUT_SENSING");
                List.Add("WORK_DONE_BUFFER_OUT_SENSING", WORK_DONE_BUFFER_OUT_SENSING);
                METAL_TRAY_LOADING_FWD = new CPropertyTimeOut("METAL_TRAY_LOADING_FWD");
                List.Add("METAL_TRAY_LOADING_FWD", METAL_TRAY_LOADING_FWD);
                METAL_TRAY_LOADING_BKD = new CPropertyTimeOut("METAL_TRAY_LOADING_BKD");
                List.Add("METAL_TRAY_LOADING_BKD", METAL_TRAY_LOADING_BKD);
                METAL_TRAY_UNLOADING_FWD = new CPropertyTimeOut("METAL_TRAY_UNLOADING_FWD");
                List.Add("METAL_TRAY_UNLOADING_FWD", METAL_TRAY_UNLOADING_FWD);
                METAL_TRAY_UNLOADING_BKD = new CPropertyTimeOut("METAL_TRAY_UNLOADING_BKD");
                List.Add("METAL_TRAY_UNLOADING_BKD", METAL_TRAY_UNLOADING_BKD);
                METAL_TRAY_UNLOADING_UP = new CPropertyTimeOut("METAL_TRAY_UNLOADING_UP");
                List.Add("METAL_TRAY_UNLOADING_UP", METAL_TRAY_UNLOADING_UP);
                METAL_TRAY_UNLOADING_DOWN = new CPropertyTimeOut("METAL_TRAY_UNLOADING_DOWN");
                List.Add("METAL_TRAY_UNLOADING_DOWN", METAL_TRAY_UNLOADING_DOWN);
                METAL_TRAY_ID_POS_SENSING = new CPropertyTimeOut("METAL_TRAY_ID_POS_SENSING");
                List.Add("METAL_TRAY_ID_POS_SENSING", METAL_TRAY_ID_POS_SENSING);
                METAL_TRAY_OUT_SENSING = new CPropertyTimeOut("METAL_TRAY_OUT_SENSING");
                List.Add("METAL_TRAY_OUT_SENSING", METAL_TRAY_OUT_SENSING);
                METAL_TRAY_LIFT_UP = new CPropertyTimeOut("METAL_TRAY_LIFT_UP");
                List.Add("METAL_TRAY_LIFT_UP", METAL_TRAY_LIFT_UP);
                METAL_TRAY_LIFT_DOWN = new CPropertyTimeOut("METAL_TRAY_LIFT_DOWN");
                List.Add("METAL_TRAY_LIFT_DOWN", METAL_TRAY_LIFT_DOWN);
                TOP_COVER_LOADING_FWD = new CPropertyTimeOut("TOP_COVER_LOADING_FWD");
                List.Add("TOP_COVER_LOADING_FWD", TOP_COVER_LOADING_FWD);
                TOP_COVER_LOADING_BKD = new CPropertyTimeOut("TOP_COVER_LOADING_BKD");
                List.Add("TOP_COVER_LOADING_BKD", TOP_COVER_LOADING_BKD);
                TOP_COVER_UNLOADING_FWD = new CPropertyTimeOut("TOP_COVER_UNLOADING_FWD");
                List.Add("TOP_COVER_UNLOADING_FWD", TOP_COVER_UNLOADING_FWD);
                TOP_COVER_UNLOADING_BKD = new CPropertyTimeOut("TOP_COVER_UNLOADING_BKD");
                List.Add("TOP_COVER_UNLOADING_BKD", TOP_COVER_UNLOADING_BKD);
                TOP_COVER_UNLOADING_UP = new CPropertyTimeOut("TOP_COVER_UNLOADING_UP");
                List.Add("TOP_COVER_UNLOADING_UP", TOP_COVER_UNLOADING_UP);
                TOP_COVER_UNLOADING_DOWN = new CPropertyTimeOut("TOP_COVER_UNLOADING_DOWN");
                List.Add("TOP_COVER_UNLOADING_DOWN", TOP_COVER_UNLOADING_DOWN);
                TOP_COVER_ID_POS_SENSING = new CPropertyTimeOut("TOP_COVER_ID_POS_SENSING");
                List.Add("TOP_COVER_ID_POS_SENSING", TOP_COVER_ID_POS_SENSING);
                TOP_COVER_OUT_SENSING = new CPropertyTimeOut("TOP_COVER_OUT_SENSING");
                List.Add("TOP_COVER_OUT_SENSING", TOP_COVER_OUT_SENSING);
                TOP_COVER_LIFT_UP = new CPropertyTimeOut("TOP_COVER_LIFT_UP");
                List.Add("TOP_COVER_LIFT_UP", TOP_COVER_LIFT_UP);
                TOP_COVER_LIFT_DOWN = new CPropertyTimeOut("TOP_COVER_LIFT_DOWN");
                List.Add("TOP_COVER_LIFT_DOWN", TOP_COVER_LIFT_DOWN);
                WORK_PICKER_GRIP = new CPropertyTimeOut("WORK_PICKER_GRIP");
                List.Add("WORK_PICKER_GRIP", WORK_PICKER_GRIP);
                WORK_PICKER_UNGRIP = new CPropertyTimeOut("WORK_PICKER_UNGRIP");
                List.Add("WORK_PICKER_UNGRIP", WORK_PICKER_UNGRIP);
                LOADING_FLIPPER_LIFT_UP = new CPropertyTimeOut("LOADING_FLIPPER_LIFT_UP");
                List.Add("LOADING_FLIPPER_LIFT_UP", LOADING_FLIPPER_LIFT_UP);
                LOADING_FLIPPER_LIFT_DOWN = new CPropertyTimeOut("LOADING_FLIPPER_LIFT_DOWN");
                List.Add("LOADING_FLIPPER_LIFT_DOWN", LOADING_FLIPPER_LIFT_DOWN);
                LOADING_FLIPPER_CLAMP_FWD = new CPropertyTimeOut("LOADING_FLIPPER_CLAMP_FWD");
                List.Add("LOADING_FLIPPER_CLAMP_FWD", LOADING_FLIPPER_CLAMP_FWD);
                LOADING_FLIPPER_CLAMP_BKD = new CPropertyTimeOut("LOADING_FLIPPER_CLAMP_BKD");
                List.Add("LOADING_FLIPPER_CLAMP_BKD", LOADING_FLIPPER_CLAMP_BKD);
                LOADING_FLIPPER_NARROW = new CPropertyTimeOut("LOADING_FLIPPER_NARROW");
                List.Add("LOADING_FLIPPER_NARROW", LOADING_FLIPPER_NARROW);
                LOADING_FLIPPER_WIDE = new CPropertyTimeOut("LOADING_FLIPPER_WIDE");
                List.Add("LOADING_FLIPPER_WIDE", LOADING_FLIPPER_WIDE);
                LOADING_FLIPPER_ROTATE_0 = new CPropertyTimeOut("LOADING_FLIPPER_ROTATE_0");
                List.Add("LOADING_FLIPPER_ROTATE_0", LOADING_FLIPPER_ROTATE_0);
                LOADING_FLIPPER_ROTATE_180 = new CPropertyTimeOut("LOADING_FLIPPER_ROTATE_180");
                List.Add("LOADING_FLIPPER_ROTATE_180", LOADING_FLIPPER_ROTATE_180);
                LOADING_VISION_WAIT = new CPropertyTimeOut("LOADING_VISION_WAIT");
                List.Add("LOADING_VISION_WAIT", LOADING_VISION_WAIT);
                UNLOADING_FLIPPER_LIFT_UP = new CPropertyTimeOut("UNLOADING_FLIPPER_LIFT_UP");
                List.Add("UNLOADING_FLIPPER_LIFT_UP", UNLOADING_FLIPPER_LIFT_UP);
                UNLOADING_FLIPPER_LIFT_DOWN = new CPropertyTimeOut("UNLOADING_FLIPPER_LIFT_DOWN");
                List.Add("UNLOADING_FLIPPER_LIFT_DOWN", UNLOADING_FLIPPER_LIFT_DOWN);
                UNLOADING_FLIPPER_CLAMP_FWD = new CPropertyTimeOut("UNLOADING_FLIPPER_CLAMP_FWD");
                List.Add("UNLOADING_FLIPPER_CLAMP_FWD", UNLOADING_FLIPPER_CLAMP_FWD);
                UNLOADING_FLIPPER_CLAMP_BKD = new CPropertyTimeOut("UNLOADING_FLIPPER_CLAMP_BKD");
                List.Add("UNLOADING_FLIPPER_CLAMP_BKD", UNLOADING_FLIPPER_CLAMP_BKD);
                UNLOADING_FLIPPER_NARROW = new CPropertyTimeOut("UNLOADING_FLIPPER_NARROW");
                List.Add("UNLOADING_FLIPPER_NARROW", UNLOADING_FLIPPER_NARROW);
                UNLOADING_FLIPPER_WIDE = new CPropertyTimeOut("UNLOADING_FLIPPER_WIDE");
                List.Add("UNLOADING_FLIPPER_WIDE", UNLOADING_FLIPPER_WIDE);
                UNLOADING_FLIPPER_ROTATE_0 = new CPropertyTimeOut("UNLOADING_FLIPPER_ROTATE_0");
                List.Add("UNLOADING_FLIPPER_ROTATE_0", UNLOADING_FLIPPER_ROTATE_0);
                UNLOADING_FLIPPER_ROTATE_180 = new CPropertyTimeOut("UNLOADING_FLIPPER_ROTATE_180");
                List.Add("UNLOADING_FLIPPER_ROTATE_180", UNLOADING_FLIPPER_ROTATE_180);
                UNLOADING_VISION_WAIT = new CPropertyTimeOut("UNLOADING_VISION_WAIT");
                List.Add("UNLOADING_VISION_WAIT", UNLOADING_VISION_WAIT);

                for (int i = 0; i < List.Count; i++)
                {
                    List.ElementAt(i).Value.LoadConfig();
                }

                return true;
            }
            catch (Exception ex)
            {
                CLOG.ALARM("[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);                
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                for (int i = 0; i < List.Count; i++)
                {
                    List.ElementAt(i).Value.SaveConfig();
                }

                return true;
            }
            catch (Exception ex)
            {
                CLOG.ALARM("[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }
    }
}
