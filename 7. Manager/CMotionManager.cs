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
    public class CMotionManager
    {        
        private CMOTION_AJIN mAxises = new CMOTION_AJIN();

        #region POSITION
        public CPropertyMotion POS_LOADER_LAPPING_R            = new CPropertyMotion("WORK_PICKER", "POS_LOADER_LAPPING_R",0, DEFINE.Axis_0);
        public CPropertyMotion POS_MAIN_R                      = new CPropertyMotion("WORK_PICKER", "POS_MAIN_R",1, DEFINE.Axis_1);
        public CPropertyMotion POS_BACK_AND_FORTH_Y            = new CPropertyMotion("WORK_PICKER", "POS_BACK_AND_FORTH_Y",2, DEFINE.Axis_2);
        public CPropertyMotion POS_LAPPING_R                   = new CPropertyMotion("WORK_PICKER", "POS_LAPPING_R",3, DEFINE.Axis_3);    
        //POS_섹션_트레이_위치_PICK/PLACE_WAIT/WORK
        //MT : METAL TRAY
        //CT : CONVERT TRAY
        //OL : OVER LAP TRAY
        //JT : JEDEC TRAY
        //LD : LOAD CONVEYOR
        //ULD : UNLOAD CONVEYOR
        //WDB : WORK DONE BUFFER CONV
        //LDV : LOAD VISION
        //ULDV : UNLOAD VISION       
        #endregion

        public Dictionary<string, CPropertyMotion> Positions = new Dictionary<string, CPropertyMotion>();
        public Dictionary<int, CAXIS_AJIN> Axises = new Dictionary<int, CAXIS_AJIN>();

        public CAXIS_AJIN AxisWork_Loader_LappingR   { get; set; } = new CAXIS_AJIN(0, "AxisWork_Loader_LappingR", CAXIS_AJIN.AxisMode.Rotate);
        public CAXIS_AJIN AxisWork_MainR { get; set; } = new CAXIS_AJIN(1, "AxisWork_MainR", CAXIS_AJIN.AxisMode.Rotate);
        public CAXIS_AJIN AxisWork_BackAndForthY      { get; set; } = new CAXIS_AJIN(2, "AxisWork_BackAndForthY", CAXIS_AJIN.AxisMode.Position);
        public CAXIS_AJIN AxisWork_LappingR      { get; set; } = new CAXIS_AJIN(3, "AxisWork_LappingR", CAXIS_AJIN.AxisMode.Rotate);   

        public CMOTION_AJIN_HOME HOME { get; set; } = null;


        public CMotionManager()
        {
            HOME = new CMOTION_AJIN_HOME(this);            
        }

        public bool IsOpen
        {
            get => mAxises.IsOpen;
        }

        public bool IsHomeComplete
        {
            get
            {
                if (AxisWork_Loader_LappingR == null) return false;
                if (AxisWork_MainR == null)               return false;
                if (AxisWork_BackAndForthY == null)                    return false;
                if (AxisWork_LappingR == null)                    return false;

                if (AxisWork_Loader_LappingR.HomeComplete == false) return false;
                if (AxisWork_MainR.HomeComplete == false) return false;
                if (AxisWork_BackAndForthY.HomeComplete == false)      return false;
                if (AxisWork_LappingR.HomeComplete == false)      return false;
                return true;
            }                
        }

        public bool Init()
        {
            try
            {
                if ( !mAxises.Init( ) ) return false;

                if (mAxises.IsOpen)
                {
                    string strMotFilePath = $"{Application.StartupPath}\\CONFIG\\DEVICE\\motion_5G.mot";
                    bool bResult = LoadConfig_Motor(strMotFilePath);

                    if (!AxisWork_Loader_LappingR.Init()) CUtil.ShowMessageBox("ERROR", "AxisWorkLoadingLiftZ Initialization Failed");
                    if (!AxisWork_MainR.Init())     CUtil.ShowMessageBox("ERROR", "AxisMetalTrayBufferZ Initialization Failed");
                    if (!AxisWork_BackAndForthY.Init())      CUtil.ShowMessageBox("ERROR", "AxisWorkPickerY Initialization Failed");                    
                    if (!AxisWork_LappingR.Init())      CUtil.ShowMessageBox("ERROR", "AxisWorkPickerZ Initialization Failed");

                    Axises.Clear();

                    Axises.Add((int)0, AxisWork_Loader_LappingR);
                    Axises.Add((int)1, AxisWork_MainR);
                    Axises.Add((int)2, AxisWork_BackAndForthY);
                    Axises.Add((int)3, AxisWork_LappingR);

                    AxisWork_Loader_LappingR.PulsePermm = 0.001D;
                    AxisWork_MainR.PulsePermm = 0.001D;
                    AxisWork_BackAndForthY.PulsePermm = 0.001D;   
                    AxisWork_LappingR.PulsePermm = 0.001D;

                    //CAXM.AxmMotSetMaxVel(0, 100000);
                    //CAXM.AxmMotSetMaxVel(1, 100000);
                    //CAXM.AxmMotSetMaxVel(3, 100000);

                    double dMax = 0;
                    CAXM.AxmMotGetMaxVel(0, ref dMax);
                    CAXM.AxmMotGetMaxVel(1, ref dMax);
                    CAXM.AxmMotGetMaxVel(3, ref dMax);

                    double dEnd = 0;

                    CAXM.AxmMotGetEndVel(0, ref dEnd);
                    CAXM.AxmMotGetEndVel(1, ref dEnd);
                    CAXM.AxmMotGetEndVel(3, ref dEnd);
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                CLOG.ALARM("[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);                
                return false;
            }
        }

        public bool LoadConfig_Motor(string strPath)
        {
            try
            {
                string strReason = "";
                uint uRet = 0;
                uRet = CAXM.AxmMotLoadParaAll(strPath);

                if (uRet != 0)
                {
                    //strReason = GetReturnCode(uRet);
                    //CLog.MOTION("[Fail] --> {0}", strReason);
                    return false;
                }

                CLOG.MOTION("[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }


        public bool LoadPositions(string strRecipeName)
        {
            try
            {
                Positions.Clear();

                Positions.Add(AxisWork_Loader_LappingR.AxisName, POS_LOADER_LAPPING_R);       
                Positions.Add(AxisWork_MainR.AxisName, POS_MAIN_R);       
                Positions.Add(AxisWork_BackAndForthY.AxisName, POS_BACK_AND_FORTH_Y);       
                Positions.Add(AxisWork_LappingR.AxisName, POS_LAPPING_R);

                
                //CAXM.AxmStatusSetActPos(POS_MAIN_R.AXIS_NO, POS_MAIN_R.ACTUAL_POS);
                //CAXM.AxmStatusSetActPos(POS_BACK_AND_FORTH_Y.AXIS_NO, POS_BACK_AND_FORTH_Y.ACTUAL_POS);
                //CAXM.AxmStatusSetActPos(POS_LAPPING_R.AXIS_NO, POS_LAPPING_R.ACTUAL_POS);

                
                //CAXM.AxmStatusSetCmdPos(POS_MAIN_R.AXIS_NO, POS_MAIN_R.COMMAND_POS);
                //CAXM.AxmStatusSetCmdPos(POS_BACK_AND_FORTH_Y.AXIS_NO, POS_BACK_AND_FORTH_Y.COMMAND_POS);
                //CAXM.AxmStatusSetCmdPos(POS_LAPPING_R.AXIS_NO, POS_LAPPING_R.COMMAND_POS);

                for (int i = 0; i < Positions.Count; i++)
                {
                    Positions.ElementAt(i).Value.LoadConfig(strRecipeName);                    
                    CAXM.AxmStatusSetActPos(Positions.ElementAt(i).Value.AXIS_NO, Positions.ElementAt(i).Value.ACTUAL_POS_Pre);
                    CAXM.AxmStatusSetCmdPos(Positions.ElementAt(i).Value.AXIS_NO, Positions.ElementAt(i).Value.COMMAND_POS_Pre);
                    Axises[i].ActualPos_Temp = (long)Positions.ElementAt(i).Value.ACTUAL_POS_TEMP;
                    Axises[i].CommanPos_Temp = (long)Positions.ElementAt(i).Value.COMMAND_POS_TEMP;
                    Axises[i].ActualPos = (long) Positions.ElementAt(i).Value.ACTUAL_POS_Pre + (long)Positions.ElementAt(i).Value.ACTUAL_POS_TEMP;
                    Axises[i].Status.ActualPos = (long)Positions.ElementAt(i).Value.ACTUAL_POS_Pre + (long)Positions.ElementAt(i).Value.ACTUAL_POS_TEMP;
                    Axises[i].Status.CommandPos = (long)Positions.ElementAt(i).Value.COMMAND_POS_Pre + (long)Positions.ElementAt(i).Value.COMMAND_POS_TEMP;

                }
            }
            catch (Exception ex)
            {
                CLOG.ALARM("[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

            return true;
        }

        public bool Close()
        {
            try
            {
                HomeStop();

                HOME.StopThreadAllHome();

                if (mAxises.IsOpen)
                {
                    AxisWork_Loader_LappingR.Close();     
                    AxisWork_MainR.Close();   
                    AxisWork_BackAndForthY.Close();        
                    AxisWork_LappingR.Close();           
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                CLOG.ALARM("[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);                
                return false;
            }
        }

        public bool SetSpeed(CAXIS_AJIN axis, int nSpeed)
        {
            try
            {
                axis.MoveVelocity = nSpeed;

                CLOG.NORMAL( $"{axis.AxisName} SPEED : {nSpeed}");
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ALARM("[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool SetAccel(CAXIS_AJIN axis, double dAccel)
        {
            try
            {
                axis.Accel = dAccel;

                CLOG.NORMAL( $"{axis.AxisName} ACCEL : {dAccel}");
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ALARM("[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool SetDecel(CAXIS_AJIN axis, double dDecel)
        {
            try
            {
                axis.Decel= dDecel;

                CLOG.NORMAL( $"{axis.AxisName} DECEL : {dDecel}");
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ALARM("[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }


        public void AllStop()
        {
            try
            {
                for (int i = 0; i < Axises.Count; i++)
                {
                    Axises.ElementAt(i).Value.StopThreadHome();
                    Axises.ElementAt(i).Value.EStop();
                    //Axises.ElementAt(i).Value.Emergency();

                    Axises.ElementAt(i).Value.HomeComplete = false;
                }
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }           
        }

        public bool HomeStop()
        {
            try
            {
                AxisWork_Loader_LappingR.StopThreadHome();
                AxisWork_MainR.StopThreadHome();
                AxisWork_BackAndForthY.StopThreadHome();
                AxisWork_LappingR.StopThreadHome();

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }  
    }
}
