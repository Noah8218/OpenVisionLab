using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KtemVisionSystem
{
    public class CCIM
    {
        //private ICGlobal.Instance CGlobal.Instance = ICGlobal.Instance.Instance;

        private CTCPAsync m_TCPIPServerAsync = new CTCPAsync();
        public CTCPAsync TCPIPServerAsync
        {
            get => m_TCPIPServerAsync;
            set => m_TCPIPServerAsync = value;
        }

        public CStatusQuery StatusQuery = new CStatusQuery();
        public CFormatJob JobChangeFormat = new CFormatJob();
        public CResultQuery ResultQuery = new CResultQuery();

        // 이놈이 True면 메거진이 이동이 완료가 된거기에 Unloading 시퀀스를 타도 무방함
        private bool m_bRequestEnd = false;
        public bool CanUnloading
        {
            get => m_bRequestEnd;
            set => m_bRequestEnd = value;
        }

        private bool m_bStartLoader = false;
        public bool StartLoader
        {
            get => m_bStartLoader;
            set => m_bStartLoader = value;
        }

        private bool m_bByPass = false;
        public bool ByPass
        {
            get => m_bByPass;
            set => m_bByPass = value;
        }


        public CCIM()
        {
            // 통신에 대한 설정
            m_TCPIPServerAsync.IsStringData = true;                    // 문자열로 데이터를 처리한다.
            m_TCPIPServerAsync.IsStringUnicode = false;                 // ASCII 
            m_TCPIPServerAsync.IsAutoConnectTry = true;                 // 연결이 끊어질 경우 자동으로 Retry를 수행한다.

            m_TCPIPServerAsync.nID = 2;                                 // 통신 ID는 1
            m_TCPIPServerAsync.sName = "Vision_TCP";                     // Vision 과의 연결 TCP/IP 통신

            // m_socketClient.Connect(sAddress, Convert.ToInt32(sPort));

            m_TCPIPServerAsync.SetCallbackReceive(OnServerReceiveFunction);
            m_TCPIPServerAsync.SetCallbackConnect(OnServerConnectFunction);
            m_TCPIPServerAsync.SetCallbackDisconnect(OnServerDisconnectFunction);

            m_TCPIPServerAsync.LoadConfig();
            m_TCPIPServerAsync.SetListen();
            
        }

        private string m_strRecvBuffer = "";
        private void OnServerReceiveFunction(IAsyncResult ar)
        {
            byte[] byData;
            string sMsg;

            while (m_TCPIPServerAsync.GetByteData(out byData))
            {
                sMsg = Encoding.ASCII.GetString(byData, 0, byData.Length);

                //m_strRecvBuffer += sMsg;

                RecvieMeaasge(sMsg);           // 수신된 데이터 처리

                //if (CGlobal.Instance.System.UseCim)
                //{
                //    RecvieMeaasge(sMsg);           // 수신된 데이터 처리
                //}
                //else
                //{
                //    CLogger.WriteLog(LOG.Comm, "CIM NOT USE");
                //    CLogger.WriteLog(LOG.NORMAL, "[Success] Recive Message --> {0}", sMsg);
                //}
            }

        }

        // Clinet가 연결에 성공했을 때
        private void OnServerConnectFunction(IAsyncResult ar)
        {
            CTCPAsync socket = (ar.AsyncState as CTCPAsync);
            CLOG.COMM($"---- Server connect success ID:{socket.nID}, Name:{socket.sName}");
        }

        // Clinet가 연결이 끊어졌을 때
        private void OnServerDisconnectFunction(IAsyncResult ar)
        {
            CTCPAsync socket = (ar.AsyncState as CTCPAsync);
            CLOG.COMM($"**** Server Disconnect ID:{socket.nID}, Name:{socket.sName}");

            // m_log2.Write($"**** Server Disconnect ID:{socket.nID}, Name:{socket.sName}");
        }


        public void Close()
        {
            //m_TCPIPServerAsync.StopThread();
        }

        public bool RecvieMeaasge(string strRecvMessage)
        {
            try
            {
                if (strRecvMessage.Length < 0)
                {
                    CLOG.ABNORMAL( "[FAILED] Recive Message");
                    return false;
                }
                else
                {
                    CLOG.COMM("CIM RECV <== {0}", strRecvMessage);
                }

                //int nPos = m_strRecvBuffer.IndexOf('\n');
                //if (nPos > -1)
                //{
                //    string strValidPacket = m_strRecvBuffer.Substring(0, nPos);
                //    m_strRecvBuffer = m_strRecvBuffer.Substring(nPos + 1, m_strRecvBuffer.Length - (nPos + 1));
                //}

                string[] strMessage = strRecvMessage.Split('\n');

                //for (int i = 0; i < strMessage.Length; i++)
                //{
                //    if (strMessage[i] == "")
                //    {
                //        continue;
                //    }

                //    string[] strArrParshingMessage = strMessage[i].Split(',');
                //    string strMainMessage = "";
                //    string strMiddleMessage = "";

                //    if (strArrParshingMessage.Length == 0)
                //    {
                //        CLog.Error( "Empty Message");
                //        return false;
                //    }

                //    // 만약 Length가 1개일 경우
                //    if (strArrParshingMessage.Length == 1)
                //    {
                //        strMainMessage = strArrParshingMessage[0];
                //    }
                //    else
                //    {
                //        strMainMessage = strArrParshingMessage[0];
                //        strMiddleMessage = strArrParshingMessage[1];
                //    }

                //    switch (strMainMessage)
                //    {
                //        case DEFINE.EQSendRequest:
                //            RespondRequest();
                //            break;
                //        case DEFINE.EQReadyQuery:
                //            // EQ -> QC로 부터 제품을 보낼테니 받을수 있냐는 명령어임
                //            // ReqReciveStrip 플래그는 QC에서 제품 로딩 시퀀스를 진행하여 제품을 받을 수 있는 Z축을 내릴때까지 기다리는 플래그임
                //            bool bExists = false;


                //            if (CGlobal.Instance.System.Recipe.StripFormat.LotID != "" 
                //                || CGlobal.Instance.System.Recipe.StripFormat.MgzNo != "" 
                //                || CGlobal.Instance.System.Recipe.StripFormat.SlotNo != "" 
                //                || CGlobal.Instance.System.Recipe.StripFormat.SlotName != ""
                //                || CGlobal.Instance.System.Recipe.StripFormat.GroupName != "" 
                //                || CGlobal.Instance.System.Recipe.StripFormat.DeviceName != "" 
                //                || CGlobal.Instance.System.Recipe.StripFormat.StripName != "")
                //            {
                //                bExists = true;
                //            }

                //            if (bExists) StatusQuery.StripExist = CStatusQuery.QCStripExist.EXISTS;                           
                                
                //            //if (CGlobal.Instance.System.Mode == ISystem.MODE.AUTO
                //            //&& (CGlobal.Instance.SeqMotion.m_StepMain == CSeqMotion.SEQ_MAIN.LOAD_WAIT_STRIP
                //            //|| CGlobal.Instance.SeqMotion.m_StepMain == CSeqMotion.SEQ_MAIN.LOAD_WAIT_STRIP_CHECK)
                //            //&& StatusQuery.StripExist == CStatusQuery.QCStripExist.NA)
                //            //{
                //            //    StartLoader = true;
                //            //    RespondReadyQuery(true);
                //            //}
                //            //else
                //            //{
                //            //    RespondReadyQuery(false);
                //            //}
                //            break;
                //        case DEFINE.EQSendOut:
                //            switch (strMiddleMessage)
                //            {
                //                case DEFINE.BYPASS:
                //                    ByPass = true;
                //                    RespondByPassOut(strArrParshingMessage);
                //                    //ResponsStatus();
                //                    break;
                //                case DEFINE.TEST:
                //                    ByPass = false;
                //                    RespondByPassOut(strArrParshingMessage);
                //                    //ResponsStatus();
                //                    break;
                //            }
                //            break;
                //        case DEFINE.EQSendEnd:
                //            RespondTransferEnd();
                //            ResponsStatus();
                //            break;

                //        case DEFINE.EQAbortTransfer:
                //            RespondAbortTransfer();
                //            break;
                //        case DEFINE.QCSendRequest:
                //            if (strArrParshingMessage.Length == 1)
                //            {
                //                SendRequestACK();

                //                // 20210209 Noah 양책임님 요청사항으로 QCSendRequest,ACK 다음에 제품을 내보낼 수 있는 명령어인
                //                // QCReadyQuery를 또 다시 보낼 수 있도록 수정
                //                if (ByPass)
                //                {
                //                    SendMessage("QCReadyQuery,GOOD");
                //                }
                //                else
                //                {
                //                    if (ResultQuery.Judgement)
                //                    {
                //                        SendMessage("QCReadyQuery,GOOD");
                //                        CLog.Info( string.Format("[Send] {0},{1}", "QCReadyQuery", "GOOD"));
                //                    }
                //                    else
                //                    {
                //                        SendMessage("QCReadyQuery,NG");
                //                        CLog.Info( string.Format("[Send] {0},{1}", "QCReadyQuery", "NG"));
                //                    }
                //                }

                //                CLog.Info( "[Recive] {0},{1}", DEFINE.QCSendRequest, strMainMessage);
                //            }

                //            if (String.Equals(strMiddleMessage, DEFINE.ACK, StringComparison.OrdinalIgnoreCase))
                //            {
                //                SendMessage("QCReadyQuery,GOOD");
                //                CLog.Info( "[Recive] {0},{1}", DEFINE.QCSendRequest, strMiddleMessage);
                //            }
                //            else
                //            {
                //                CLog.Error( "[Failed] Failed Recive ACK");
                //            }
                //            break;
                //        case DEFINE.QCReadyQuery:
                //            if (String.Equals(strMiddleMessage, DEFINE.OK, StringComparison.OrdinalIgnoreCase))
                //            {
                //                CanUnloading = true;

                //                // 20210601 Noah Choi 이재현 수석님 요청사항으로 NG를 수신시 OK를 받을 때까지 명령어 재송신
                //                if (ResultQuery.Judgement)
                //                {
                //                    SendByPassOut(true);
                //                }
                //                else
                //                {
                //                    SendByPassOut(false);
                //                }

                //                CLog.Info( "[Recive] {0},{1}", DEFINE.QCReadyQuery, strMiddleMessage);
                //            }
                //            else if (String.Equals(strMiddleMessage, DEFINE.NG, StringComparison.OrdinalIgnoreCase))
                //            {
                //                // 20210209 Noah Choi 양책임님 요청사항으로 NG시에도 제품을 내보낼수 있도록 수정
                //                //RequestEnd = true;

                //                //SendByPassOut(false);
                //                // 20210601 Noah Choi 이재현 수석님 요청사항으로 NG를 수신시 OK를 받을 때까지 명령어 재송신
                //                if (ByPass)
                //                {
                //                    SendMessage("QCReadyQuery,GOOD");
                //                }
                //                else
                //                {
                //                    if (ResultQuery.Judgement)
                //                    {
                //                        SendMessage("QCReadyQuery,GOOD");
                //                        CLog.Info( string.Format("[Send] {0},{1}", "QCReadyQuery", "GOOD"));
                //                    }
                //                    else
                //                    {
                //                        SendMessage("QCReadyQuery,NG");
                //                        CLog.Info( string.Format("[Send] {0},{1}", "QCReadyQuery", "NG"));
                //                    }
                //                }
                                

                //                Thread.Sleep(1000);

                //                CLog.Info( "[Recive] {0},{1}", DEFINE.QCReadyQuery, strMiddleMessage);
                //            }
                //            else
                //            {
                //                if (ResultQuery.Judgement)
                //                {
                //                    SendMessage("QCReadyQuery,GOOD");
                //                    CLog.Info( string.Format("[Send] {0},{1}", "QCReadyQuery", "GOOD"));
                //                }
                //                else
                //                {
                //                    SendMessage("QCReadyQuery,NG");
                //                    CLog.Info( string.Format("[Send] {0},{1}", "QCReadyQuery", "NG"));
                //                }

                //                Thread.Sleep(1000);

                //                CLog.Info( "[Recive] {0},{1}", DEFINE.QCReadyQuery, strMiddleMessage);
                //                CLog.Error( "[Failed] Failed Recive ACK");
                //            }
                //            break;
                //        case DEFINE.QCSendOut:
                //            if (String.Equals(strMiddleMessage, DEFINE.ACK, StringComparison.OrdinalIgnoreCase))
                //            {
                //                //SendTransferEnd();
                //                //CGlobal.Instance.SeqMotion.CanMoveToUnloading = true;
                //                CLog.Info( "[Recive] {0},{1}", DEFINE.QCSendOut, strMiddleMessage);
                //            }
                //            else
                //            {
                //                CLog.Error( "[Failed] Failed Recive ACK");
                //            }
                //            break;
                //        case DEFINE.QCSendEnd:
                //            if (String.Equals(strMiddleMessage, DEFINE.ACK, StringComparison.OrdinalIgnoreCase))
                //            {
                //                CLog.Info( "[Recive] {0},{1}", DEFINE.QCSendEnd, strMiddleMessage);
                //            }
                //            else
                //            {
                //                CLog.Error( "[Failed] Failed Recive ACK");
                //            }
                //            break;
                //        case DEFINE.QCAbortTransfer:
                //            if (String.Equals(strMiddleMessage, DEFINE.ACK, StringComparison.OrdinalIgnoreCase))
                //            {
                //                CLog.Info( "[Recive] {0},{1}", DEFINE.QCAbortTransfer, strMiddleMessage);
                //            }
                //            else
                //            {
                //                CLog.Error( "[Failed] Failed Recive ACK");
                //            }
                //            break;
                //        case DEFINE.AutoRun:
                //            RespondAutoRun();
                //            break;
                //        case DEFINE.AutoStop:
                //            RespondAutoStop();
                //            break;
                //        case DEFINE.ErrorReset:
                //            RespondAbNormalReset();
                //            break;
                //        case DEFINE.LotEnd:
                //            // 20210209 양책임님 요청사항으로 일단 주석처리
                //            // 사용상 문제가 될 수 있다고 빼달라고 하심
                //            //RespondLotEnd();
                //            CGlobal.Instance.iData.ResetCount();
                //            break;
                //        case DEFINE.DoorLock:
                //            switch (strMiddleMessage)
                //            {
                //                case DEFINE.ON:
                //                    RespondDoorLockOn();
                //                    break;
                //                case DEFINE.OFF:
                //                    RespondDoorLockOff();
                //                    break;
                //            }
                //            break;
                //        case DEFINE.JobChange:
                //            // 비전 사용유무는 어떻게 사용되는지?
                //            try
                //            {
                //                RespondJobCahnge(strArrParshingMessage);
                //            }
                //            catch (Exception ex)
                //            {
                //                string strRequest = "JobChange,NG" + "\n";

                //                CLog.COMM("[Send] ==> {0}", strRequest);

                //                m_TCPIPServerAsync.Send(strRequest);

                //                CLog.Error( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                //                return false;
                //            }

                //            break;
                //        case DEFINE.EmgStop:
                //            RespondEmergencyStop();
                //            break;
                //        case DEFINE.Buzzer:
                //            switch (strMiddleMessage)
                //            {
                //                case DEFINE.ON:
                //                    RespondBuzzer(true);
                //                    break;
                //                case DEFINE.OFF:
                //                    RespondBuzzer(false);
                //                    break;
                //            }
                //            break;
                //        case DEFINE.StatusQuery:
                //            // 나머지 상태 2개도 표시해줄 방안 모색
                //            ResponsStatus();
                //            break;
                //        case DEFINE.ResultQuery:
                //            // 20210916 noah 비전 결과 데이터를 넣어줘야함
                //            ResponsResultQuery("");
                //            break;
                //        default:
                //            break;
                //    }
                //}

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool SendMessage(string strSendMessage)
        {
            try
            {
                string[] strArrParshingMessage = strSendMessage.Replace("\n", "").Split(',');
                string strMainMessage = "";
                string strMiddleMessage = "";

                // 레시피나 데이터가 많이 들어올 경우 처리방법 모색해야함                

                if (strArrParshingMessage.Length == 0)
                {
                    CLOG.ABNORMAL( "Empty Message");
                    return false;
                }

                // 만약 Length가 1개일 경우
                if (strArrParshingMessage.Length == 1)
                {
                    strMainMessage = strArrParshingMessage[0];
                }
                else
                {
                    strMainMessage = strArrParshingMessage[0];
                    strMiddleMessage = strArrParshingMessage[1];
                }

                //switch (strMainMessage)
                //{
                //    case DEFINE.QCSendRequest:
                //        SendRequest();
                //        break;
                //    case DEFINE.QCReadyQuery:
                //        switch (strMiddleMessage)
                //        {
                //            case DEFINE.GOOD:
                //                SendReadyQuery(true);
                //                break;
                //            case DEFINE.NG:
                //                SendReadyQuery(false);
                //                break;
                //        }
                //        break;
                //    case DEFINE.QCSendOut:
                //        switch (strMiddleMessage)
                //        {
                //            case DEFINE.GOOD:
                //                SendByPassOut(true);
                //                break;
                //            case DEFINE.NG:
                //                SendByPassOut(false);
                //                break;
                //        }
                //        break;
                //    case DEFINE.QCSendEnd:
                //        SendTransferEnd();
                //        break;
                //    case DEFINE.QCAbortTransfer:
                //        SendAbortTransfer();
                //        break;
                //    default:
                //        break;
                //}

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }


        public bool SendRequest()
        {
            try
            {
                string strRequest = "QCSendRequest" + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                CLOG.COMM("[Send] ==> {0}", strRequest);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool SendRequestACK()
        {
            try
            {
                string strRequest = "QCSendRequest,ACK" + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                CLOG.COMM("[Send] ==> {0}", strRequest);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool RespondRequest()
        {
            try
            {
                string strRequest = "EQSendRequest,ACK" + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                CLOG.COMM("[Send] ==> {0}", strRequest);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool SendReadyQuery(bool bReady)
        {
            try
            {
                string strRequest = "";
                if (bReady)
                {
                    strRequest = "QCReadyQuery,GOOD" + "\n";
                }
                else
                {
                    strRequest = "QCReadyQuery,NG" + "\n";
                }

                m_TCPIPServerAsync.Send(strRequest);

                CLOG.COMM("[Send] ==> {0}", strRequest);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool RespondReadyQuery(bool bReady)
        {
            try
            {
                string strRequest = "";
                if (bReady)
                {
                    strRequest = "EQReadyQuery,OK" + "\n";
                }
                else
                {
                    strRequest = "EQReadyQuery,NG" + "\n";
                }

                m_TCPIPServerAsync.Send(strRequest);

                CLOG.COMM("[Send] ==> {0}", strRequest);
                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool RespondByPassOut(string[] strArrStrip)
        {
            try
            {
                if (strArrStrip.Length > 9)
                {
                    CLOG.ABNORMAL( "[Failed] Recive ByPass Length Under");
                    return false;
                }

                //CGlobal.Instance.System.Recipe.StripFormat.LotID = strArrStrip[2];
                //CGlobal.Instance.System.Recipe.StripFormat.MgzNo = strArrStrip[3];
                //CGlobal.Instance.System.Recipe.StripFormat.SlotNo = strArrStrip[4];
                //CGlobal.Instance.System.Recipe.StripFormat.SlotName = strArrStrip[5];
                //CGlobal.Instance.System.Recipe.StripFormat.GroupName = strArrStrip[6];
                //CGlobal.Instance.System.Recipe.StripFormat.DeviceName = strArrStrip[7];
                //CGlobal.Instance.System.Recipe.StripFormat.StripName = strArrStrip[8];

                //string strLotId = CGlobal.Instance.System.Recipe.StripFormat.LotID;
                //string strMgzNo = CGlobal.Instance.System.Recipe.StripFormat.MgzNo;
                //string strSlotNo = CGlobal.Instance.System.Recipe.StripFormat.SlotNo.ToString();
                //string strSlotName = CGlobal.Instance.System.Recipe.StripFormat.SlotName;
                //string strGroupName = CGlobal.Instance.System.Recipe.StripFormat.GroupName;
                //string strDeviceName = CGlobal.Instance.System.Recipe.StripFormat.DeviceName;
                //string strStripName = CGlobal.Instance.System.Recipe.StripFormat.StripName;

                //List<string> listStrip = new List<string>();
                //listStrip.Add(strLotId + ",");
                //listStrip.Add(strMgzNo + ",");
                //listStrip.Add(strSlotNo + ",");
                //listStrip.Add(strSlotName + ",");
                //listStrip.Add(strGroupName + ",");
                //listStrip.Add(strDeviceName + ",");
                //listStrip.Add(strStripName);

                //CGlobal.Instance.System.Recipe.StripFormat.SaveConfig(CGlobal.Instance.System.Recipe.Name);

                //string strRequest = "EQSendOut,ACK" + "\n";
                //string strRequestLog = "EQSendOut,ACK,";

                //m_TCPIPServerAsync.Send(strRequest);

                //for (int i = 0; i < listStrip.Count; i++)
                //{
                //    strRequestLog = strRequestLog + listStrip[i];
                //}
                //strRequestLog = strRequestLog + "\n";

                //CLog.COMM("[Send] ==> {0}", strRequestLog);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool SendByPassOut(bool bGood)
        {
            try
            {
                //string strLotId = CGlobal.Instance.System.Recipe.StripFormat.LotID;
                //string strMgzNo = CGlobal.Instance.System.Recipe.StripFormat.MgzNo;
                //string strSlotNo = CGlobal.Instance.System.Recipe.StripFormat.SlotNo;
                //string strSlotName = CGlobal.Instance.System.Recipe.StripFormat.SlotName;
                //string strGroupName = CGlobal.Instance.System.Recipe.StripFormat.GroupName;
                //string strDeviceName = CGlobal.Instance.System.Recipe.StripFormat.DeviceName;
                //string strStripName = CGlobal.Instance.System.Recipe.StripFormat.StripName;

                //List<string> listStrip = new List<string>();
                //listStrip.Add(strLotId + ",");
                //listStrip.Add(strMgzNo + ",");
                //listStrip.Add(strSlotNo + ",");
                //listStrip.Add(strSlotName + ",");
                //listStrip.Add(strGroupName + ",");
                //listStrip.Add(strDeviceName + ",");
                //listStrip.Add(strStripName);

                //string strRequest = "";
                //if (bGood)
                //{
                //    // 추후에 "[StripData]" 부분 수정해줘야함
                //    strRequest = "QCSendOut,GOOD,";

                //}
                //else
                //{
                //    // 추후에 "[StripData]" 부분 수정해줘야함
                //    strRequest = "QCSendOut,NG,";
                //}

                //for (int i = 0; i < listStrip.Count; i++)
                //{
                //    strRequest = strRequest + listStrip[i];
                //}
                //strRequest = strRequest + "\n";

                //m_TCPIPServerAsync.Send(strRequest);
                
                //CLog.COMM("[Send] ==> {0}", strRequest);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool SendTransferEnd()
        {
            try
            {
                // 20210209 엔드 푸쉬로 제품을 내보낼 때 스트립 정보도 지우는데 값을 지우기전 500ms 정도 쉬어야 문제가 없음
                Thread.Sleep(500);
                //CGlobal.Instance.System.Recipe.StripFormat.LotID = "";
                //CGlobal.Instance.System.Recipe.StripFormat.MgzNo = "";
                //CGlobal.Instance.System.Recipe.StripFormat.SlotNo = "";
                //CGlobal.Instance.System.Recipe.StripFormat.SlotName = "";
                //CGlobal.Instance.System.Recipe.StripFormat.GroupName = "";
                //CGlobal.Instance.System.Recipe.StripFormat.DeviceName = "";
                //CGlobal.Instance.System.Recipe.StripFormat.StripName = "";

                //CGlobal.Instance.System.Recipe.StripFormat.SaveConfig(CGlobal.Instance.System.Recipe.Name);

                //if (CGlobal.Instance.System.Recipe.StripFormat.LotID == "" &&
                //    CGlobal.Instance.System.Recipe.StripFormat.MgzNo == "" &&
                //    CGlobal.Instance.System.Recipe.StripFormat.SlotNo == "" &&
                //    CGlobal.Instance.System.Recipe.StripFormat.SlotName == "" &&
                //    CGlobal.Instance.System.Recipe.StripFormat.GroupName == "" &&
                //    CGlobal.Instance.System.Recipe.StripFormat.DeviceName == "" &&
                //    CGlobal.Instance.System.Recipe.StripFormat.StripName == "")
                //{
                //    StatusQuery.StripExist = CStatusQuery.QCStripExist.NA;
                //}


                string strRequest = "QCSendEnd" + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                CLOG.COMM("[Send] ==> {0}", strRequest);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool RespondTransferEnd()
        {
            try
            {
                string strRequest = "EQSendEnd,ACK" + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                //CGlobal.Instance.SeqMotion.CanMoveToStation = true;
                // 20210916 noah 시퀀스와 엮인 부분 확인해야함
                //CGlobal.Instance.SeqManager.Loader.ReqReciveEQSendEnd = true;
                
                CLOG.COMM("[Send] ==> {0}", strRequest);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool SendAbortTransfer()
        {
            try
            {
                string strRequest = "QCAbortTransfer" + "\n";

                if (StatusQuery.StripExist == CStatusQuery.QCStripExist.EXISTS)
                {
                    m_TCPIPServerAsync.Send(strRequest);
                    CLOG.COMM("[Send] ==> {0}", strRequest);
                }

                //CGlobal.Instance.System.Recipe.StripFormat.LotID = "";
                //CGlobal.Instance.System.Recipe.StripFormat.MgzNo = "";
                //CGlobal.Instance.System.Recipe.StripFormat.SlotNo = "";
                //CGlobal.Instance.System.Recipe.StripFormat.SlotName = "";
                //CGlobal.Instance.System.Recipe.StripFormat.GroupName = "";
                //CGlobal.Instance.System.Recipe.StripFormat.DeviceName = "";
                //CGlobal.Instance.System.Recipe.StripFormat.StripName = "";

                //CGlobal.Instance.System.Recipe.StripFormat.SaveConfig(CGlobal.Instance.System.Recipe.Name);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool RespondAbortTransfer()
        {
            try
            {
                //CGlobal.Instance.System.Recipe.StripFormat.LotID = "";
                //CGlobal.Instance.System.Recipe.StripFormat.MgzNo = "";
                //CGlobal.Instance.System.Recipe.StripFormat.SlotNo = "";
                //CGlobal.Instance.System.Recipe.StripFormat.SlotName = "";
                //CGlobal.Instance.System.Recipe.StripFormat.GroupName = "";
                //CGlobal.Instance.System.Recipe.StripFormat.DeviceName = "";
                //CGlobal.Instance.System.Recipe.StripFormat.StripName = "";

                //CGlobal.Instance.System.Recipe.StripFormat.SaveConfig(CGlobal.Instance.System.Recipe.Name);

                string strRequest = "EQAbortTransfer,ACK" + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                CLOG.COMM("[Send] ==> {0}", strRequest);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool RespondAutoRun()
        {
            try
            {
                if (CGlobal.Inst.System.Mode != CSystem.MODE.AUTO) CGlobal.Inst.System.Mode = CSystem.MODE.AUTO;

                string strRequest = "AutoRun,ACK" + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                ResponsStatus();

                CLOG.COMM("[Send] ==> {0}", strRequest);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool RespondAutoStop()
        {
            try
            {
                CGlobal.Inst.System.Mode = CSystem.MODE.READY;

                string strRequest = "AutoStop,ACK" + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                CLOG.COMM("[Send] ==> {0}", strRequest);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool RespondAbNormalReset()
        {
            try
            {
                //if (CGlobal.Instance.iSystem.Mode == ISystem.MODE.ALARM_CYCLE)


                // 20210916 Noah Cim으로부터 에러클리어 요청이 오면 클리어하는 항목임
                // 현재 상황에서는 어떻게 에러클리어할지 확인해야함

                CGlobal.Inst.System.Mode = CSystem.MODE.READY;
                string strRequest = "AbNormalReset,ACK" + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                //ResponsStatus();

                CLOG.COMM("[Send] ==> {0}", strRequest);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool RespondLotEnd()
        {
            try
            {
                int nTimeOutCheck = Environment.TickCount;
                int nTimeOut = 100000;

                while (true)
                {
                    Thread.Sleep(10);

                    if ((nTimeOutCheck - Environment.TickCount) > nTimeOut)
                    {
                        CLOG.ABNORMAL( "LotEnd Faile");
                        CLOG.ABNORMAL( "Exsit Strip!!");

                        //ResponsStatus();
                        break;
                    }

                }

                string strRequest = "LotEnd,ACK" + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                //ResponsStatus();

                CLOG.COMM("[Send] ==> {0}", strRequest);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }

        }

        public bool RespondDoorLockOn()
        {
            try
            {


                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool RespondDoorLockOff()
        {
            try
            {

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool RespondJobCahnge(string[] strArrJobName)
        {
            try
            {
                if (strArrJobName.Length > 4)
                {
                    CLOG.ABNORMAL( "[Failed] Recive Job Data Length Under");
                    return false;
                }

                JobChangeFormat.GroupName = strArrJobName[1];
                JobChangeFormat.DeviceName = strArrJobName[2];
                //if (strArrJobName[3] == DEFINE.ON)
                //{
                //    JobChangeFormat.UseVision = true;
                //}
                //else
                //{
                //    JobChangeFormat.UseVision = false;
                //}

                string strRecipeName = JobChangeFormat.GroupName + "." + JobChangeFormat.DeviceName;
                //string strPrevRecipe = CGlobal.Inst.System.Recipe.Name;

                //CGlobal.Inst.System.Recipe.Name = strRecipeName;
                //CGlobal.Instance.iSystem.Recipe.SetRecipe(strPrevRecipe, strRecipeName);
                //CGlobal.Inst.System.LastRecipe = strRecipeName;
                //CGlobal.Inst.System.Recipe.SaveTools();
                CGlobal.Inst.System.SaveConfig();
                //CGlobal.Inst.System.Notice = string.Format("Change the Recipe {0} ==> {1}", strPrevRecipe, strRecipeName);

                string strRequest = "JobChange,OK" + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                CLOG.COMM("[Send] ==> {0}", strRequest);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                string strRequest = "JobChange,NG" + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                CLOG.COMM("[Send] ==> {0}", strRequest);

                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool RespondEmergencyStop()
        {
            try
            {
                string strRequest = "EmgStop,ACK" + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                CLOG.COMM("[Send] ==> {0}", strRequest);

                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool RespondBuzzer(bool bBuzzerOn)
        {
            try
            {
                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool ResponsStatus()
        {
            try
            {
                Thread.Sleep(300);

                //int nQCStatus = (int)StatusQuery.Status;
                int nQCStatus = 3;
                int nExistStrip = (int)StatusQuery.StripExist;
                int nSaftyExhaust = (int)StatusQuery.END_PUSHER;

                string strRequest = "Status," + nQCStatus.ToString() + "," + nExistStrip + "," + nSaftyExhaust + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                //CLog.COMM("[Send] ==> {0}", strRequest);
                //CLog.Info( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        public bool ResponsResultQuery(string strResultData)
        {
            try
            {
                string[] strArrParshingMessage = strResultData.Replace("\n", "").Split(',');

                string strRequest = "ResultQuery,";

                for (int i = 0; i < strArrParshingMessage.Length; i++)
                {
                    strRequest = strRequest + strArrParshingMessage[i] + ",";
                }
                strRequest = strRequest.Substring(0, strRequest.Length - 1);

                strRequest = strRequest + "\n";

                m_TCPIPServerAsync.Send(strRequest);

                CLOG.COMM("[Send] ==> {0}", strRequest);
                CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }
    }
}
