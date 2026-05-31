using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KtemVisionSystem
{
    public partial class CMOTION_AJIN
    {
        private bool m_bIsOpen = false;
        public bool IsOpen
        {
            get { return m_bIsOpen; }
            set { m_bIsOpen = value; }
        }

        private int m_nAxisCount = 0;
        public int AxisCount
        {
            get { return m_nAxisCount; }
            set { m_nAxisCount = value; }
        }

        private uint m_unModuleID = 0;
        public uint ModuleID
        {
            get { return m_unModuleID; }
            set { m_unModuleID = value; }
        }

        private int m_nBoardNO = 0;
        public int BoardNO
        {
            get { return m_nBoardNO; }
            set { m_nBoardNO = value; }
        }

        private int m_nBoardPos = 0;
        public int BoardPos
        {
            get { return m_nBoardPos; }
            set { m_nBoardPos = value; }
        }

        #region ENUM REGION
        public enum InputEncoderMode
        {
            ObverseUpDownMode, //정방향 Up/Down
            ObverseSqr1Mode, //정방향 1체배
            ObverseSqr2Mode, //정방향 2체배
            ObverseSqr4Mode, //정방향 4체배
            ReverseUpDownMode, //역방향 Up/Down
            ReverseSqr1Mode, //역방향 1체배
            ReverseSqr2Mode, //역방향 2체배
            ReverseSqr4Mode    //역방향 4체배
        }
        public enum OutputEncoderMode
        {
            OneHighLowHigh, //1펄스 방식, PULSE(Active High), 정방향(DIR=Low)  / 역방향(DIR=High)
            OneHighHighLow, //1펄스 방식, PULSE(Active High), 정방향(DIR=High) / 역방향(DIR=Low)
            OneLowLowHigh, //1펄스 방식, PULSE(Active Low),  정방향(DIR=Low)  / 역방향(DIR=High)
            OneLowHighLow, //1펄스 방식, PULSE(Active Low),  정방향(DIR=High) / 역방향(DIR=Low)
            TwoCcwCwHigh, //2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active High     
            TwoCcwCwLow, //2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active Low     
            TwoCwCcwHigh, //2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active High
            TwoCwCcwLow, //2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active Low
            TwoPhase, //2상(90' 위상차),  PULSE lead DIR(CW: 정방향), PULSE lag DIR(CCW:역방향)
            TwoPhaseReverse   //2상(90' 위상차),  PULSE lead DIR(CCW: 정방향), PULSE lag DIR(CW:역방향)
        }

        public enum Status_StopMode
        {
            EmergencyStop, // 급정지
            SlowDownStop // 감속 정지
        }

        public enum Status_LevelMode
        {
            Low, // B접점,
            Hght, // A접점
            Unused, // 사용 안함
            Used // 현 상태 유지
        }

        enum Status_Move_Direction
        {
            DIR_CCW = 0, // -방향(반시계)
            DIR_CW = 1 // +방향(시계)
        }

        enum Status_Home_Detect_Signal_Def
        {
            PosEndLimit, // +Elm(End limit) + 방향 리미트 센서 신호            
            NegEndLimit, // -Elm(End limit) 방향 리미트 센서 신호
            PosSloLimit, // +Slm(Slow Down limit) 신호 사용하지 않음            
            NegSloLimit, // -Slm(Slow Down limit) 신호 사용하지 않음
            HomeSensor, // IN0(ORG) 원점 센서 신호
            EncodZPhase, // IN1(Z상 ) Encoder Z 상 신호            
            UniInput02, // IN2(범용 ) 범용 입력 2 번 신호
            UniInput03 // IN3(범용 ) 범용 입력 3 번 신호
        }

        public enum Status_Motion_Profile_Mode
        {
            SymTrapezoideMode, // 대칭 Trapezode
            AsymTrapezoideMode, // 비대칭 Trapezode
            QuasiSCurveMode, // 대칭Quasi S Curve (PCI N404/804 는 지원 안 함
            SymSCurveMode, // 대칭S Curve
            AsymSCurveMode // 비대칭S Curve
        }
        #endregion


        public CMOTION_AJIN()
        {
        }


        public bool Init()
        {
            try
            {
                string strAxis = "";

                // 7 은 IRQ 를 뜻한다 . PCI 에서 자동으로 IRQ 가 설정된다
                if (CAXL.AxlOpen(7) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS) return false;
                if (CAXM.AxmInfoGetAxisCount(ref m_nAxisCount) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS) return false;

                //for (int i = 0; i < m_nAxisCount; i++)
                //{
                //    if (CAXM.AxmSignalSetStop(i, (uint)AXT_MOTION_STOPMODE.EMERGENCY_STOP, (uint)AXT_MOTION_LEVEL_MODE.LOW) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS) return false;
                //    //파라미터 세팅 필요, 무빙 가속도/감속도, 홈 가속도/감속도

                //    //// uAbsRelMode : POS_ABS_MODE '0' - Absolute coordinates 
                //    //// POS_REL_MODE '1' - Relative coordinates 
                //    //dwResult = AxmMotSetAbsRelMode(lAxisNo, POS_REL_MODE);
                //    //if (dwResult != AXT_RT_SUCCESS)
                //    //    Console.WriteLine("Error: {0:D4}, AxmMotSetAbsRelMode", dwResult);
                //}

                    //// Start Move 
                    //dwResult = AxmMoveStartPos(lAxisNo, dDistance, dVelocity, dAcceleration, dDeceleration);
                    //if (dwResult != AXT_RT_SUCCESS)
                    //    Console.WriteLine("Error: {0:D4}, AxmMoveStartPos", dwResult);

                    //do
                    //{
                    //    System.Threading.Thread.Sleep(500);
                    //    // Read Command position 
                    //    dwResult = AxmStatusGetCmdPos(0, dPosition);
                    //    if (dwResult != AXT_RT_SUCCESS)
                    //        Console.WriteLine("Error: {0:D4}, AxmStatusGetCmdPos", dwResult);
                    //    Console.WriteLine(" Command position = {0:F3}", dPosition);

                    //    // Read Motion status 
                    //    dwResult = AxmStatusReadInMotion(0, dwStatus);
                    //    if (dwResult != AXT_RT_SUCCESS)
                    //        Console.WriteLine("Error: {0:D4}, AxmStatusReadInMotion", dwResult);
                    //}
                    //while ((dwStatus));

                    //// Confirm move 
                    //if (Convert.ToInt32(dDistance) == Convert.ToInt32(dPosition))
                    //    Console.WriteLine("Move success.");
                    //else
                    //    Console.WriteLine("Move failure.");                

                //++ 지정한 Mot파일의 설정값들로 모션보드의 설정값들을 일괄변경 적용합니다.
                //if (CAXM.AxmMotLoadParaAll(szFilePath) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS) MessageBox.Show("Mot File Not Found.");

                m_bIsOpen = true;
                return m_bIsOpen;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                //CUtil.ShowMessageBox("EXCEPTION", ex.Message, FormMessageBox.MESSAGEBOX_TYPE.Waring);
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                if (CAXL.AxlClose() != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                {
                    //비정상적으로 종료
                    CLOG.ABNORMAL( "Close Fail..!! ");
                    return false;
                }

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
