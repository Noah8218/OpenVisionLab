//////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// C# Gaus Library
// 
// Gaus.Comm : TCP/IP Socket 통신 및 RS232/485 Serial 통신 class
//
// 2020-02-25, jhLee
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace KtemVisionSystem
{
    // Gaus TCP/IP Socket class
    // 비동기방식의 socket class로 Server와 Client를 동시에 지원한다.
    // 문자열 및 바이트 배열을 전송하고, 문자열은 Unicode와 ASCII를 지원한다.
    // OnConnect, OnDisconnect, OnReceive Callback 함수를 지원한다.
    // CGxLog를 통해 화면과 file로 전송데이터 및 이벤트/예외 내용을 저장 가능하다.

    public class CTCPAsync
    // Server socket class
    {
        public bool bConnected = false;

        public List<byte[]> listRcvData = new List<byte[]>();        // Byte 배열을 가지는 list queue

        public Socket socketServer = null;
        public Socket socketClient = null;                  // client Socket class

        public const int BufferSize = 4096;                    // Size of receive buffer.  
        public byte[] byBuffer = new byte[BufferSize];      // Receive buffer. 
        // public byte[] byPopData = null;                     // 수신 list에서 사용을 위해 빼어낸 데이터의 임시 보관용
        // public StringBuilder sb = new StringBuilder();      // Received data string. 

        // 송수신 데이터 길이
        public int nSendLength = 0;                         // 송신 완료 길이
        public int nRecvLength = 0;                         // 수신 완료 길이
        
        public string sMyName;
        public string sIPAddress = "";                      // 접속 할 IP 주소
        public int nPortNo = 5000;                          // 접속 할 Port 주소

        //d  IPAddress ipAddress;
        //d IPEndPoint localEPoint;
        //d public int nConnectDelay = 0;


        public IAsyncResult arConnect;                      // BeginConnect의 결과를 담아둔다.
        public int nAbnormalCount = 0;                      // 비정상 상태 반복횟수
        private bool bAutoConnectFlag = false;              // 자동 접속 동작을 활성화할 것인가 ?

        System.Timers.Timer tmrTryConnect = new System.Timers.Timer();  // 접속용 Timer

        // 비동기 처리시 각종 수행 완료 event
        public ManualResetEvent evtConnectDone;             // Connect 완료 event
        public ManualResetEvent evtSendDone;                // Send 완료 event
        public ManualResetEvent evtReceiveDone;             // Receive 완료 event
        public ManualResetEvent evtReceiveFlag;             // Receive 완료 event, 수신 데이터 처리를 위한 Event


        // 각종 Event callback 함수
        private AsyncCallback fnAcceptHandle = null;        // Accept event
        private AsyncCallback fnSendHandle = null;          // Send event
        private AsyncCallback fnReceiveHandle = null;       // Receive event
        private AsyncCallback fnConnectHandle = null;       // Connect event  


        // 최종 사용단에서 각 이벤트 발생시 처리 할 내용 지정
        private AsyncCallback fnCallbackConnect = null;     // 연결이 이루어질 경우 발생되는 callback 함수
        private AsyncCallback fnCallbackDisconnect = null;  // 연결이 끊어졌을 경우 발생되는 callback 함수
        private AsyncCallback fnCallbackReceive = null;     // 데이터가 수신되었을때 수행되는 callback 함수


        // Property

        private string m_strIp = "127.0.0.1";
        public string IP
        {
            get => m_strIp;
            set => m_strIp = value;
        }

        private int m_nPort = 5000;
        public int Port
        {
            get => m_nPort;
            set => m_nPort = value;
        }

        public string sName { get; set; } = "NONE";         // 객체 이름 지정
        public int nID { get; set; } = 0;                   // 구분 ID 지정

        public bool IsUnicode { get; set; } = false;        // 전송시 Unicode 문자열을 이용하는가 ? true:Unicode, false:ASCII

        // Log 기록관련
        public bool IsStringData { get; set; } = true;      // 전송데이터가 문자열인가 바이너리인가 ?
        public bool IsStringUnicode { get; set; } = false;  // 문자열을 Unicode로 송/수신 할것인가 ?
        public bool IsLogData { get; set; } = true;         // Send / Receive 동작시 문자열로 데이터를 log에 남길것인가 ?
        public bool IsLogLength { get; set; } = true;       // 송/수신 데이터 길이를 log 에 남길것인가 ?

        public bool IsLogException { get; set; } = true;    // 예외 발생을 log에 남길것인가 ?
        public bool IsLogEvent { get; set; } = true;        // Event 발생을 log에 남길것인가 ?
        public bool IsLogEventReceive { get; set; } = true;    // Receive event발생시 log에 남길것인가 ? (자주 발생되는 이벤트 로그기록)

        // 재접속 관련 설정
        public bool IsAutoConnectTry { get; set; } = false;     // 자동으로 재접속을 시도한다.
        public int ConnectTimeout { get; set; } = 1500;     // Client connect timeout
        public int ConnectDelay { get; set; } = 3000;       // 얼마의 시간 뒤에 다시 재접속을 시도할 것인가 ?


        public CTCPAsync() // 생성시 초기화
        {
            fnAcceptHandle = new AsyncCallback(OnAcceptCallback);
            fnSendHandle = new AsyncCallback(OnSendCallback);
            fnReceiveHandle = new AsyncCallback(OnReceiveCallback);
            fnConnectHandle = new AsyncCallback(OnConnectCallback);

            // 비동기 처리시 각종 수행 완료 event
            evtConnectDone = new ManualResetEvent(false);             // Connect 완료 event
            evtSendDone = new ManualResetEvent(false);                // Send 완료 event
            evtReceiveDone = new ManualResetEvent(false);             // Receive 완료 event
            evtReceiveFlag = new ManualResetEvent(false);             // Receive 완료 event, 수신 데이터 처리를 위한 Event

            // 접속 시도 Timer 기본 설정
            tmrTryConnect.Interval = 2000;                      // 이벤트 발생 주기
            tmrTryConnect.AutoReset = false;                    // 이벤트를 1번만 발생시킨다.
            tmrTryConnect.Elapsed += OnTryConnectEvent;         // 재접속 시도 이벤트 지정
        }

        public void Send(String data)
        {
            try
            {
                CLOG.COMM($"CIM SEND ==> {data}");
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                if (socketClient != null)
                {
                    socketClient.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), socketClient);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;

                int bytesSent = handler.EndSend(ar);
                //CLog.COMM("Sent {0} bytes to client.", bytesSent);                

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);                
            }
        }


        // 선두의 데이터를 바이트 배열로 받아온다. (원본 삭제)
        // return 데이터를 정상적으로 받아왔는지 여부, true:정상적으로 데이터를 취득하였다. false:데이터가 존재하지 않는다.
        public bool GetByteData(out byte[] OutData)
        {
            if (listRcvData.Count > 0)              // 수신 데이터가 존재하는가 ?
            {
                try
                {
                    OutData = new byte[listRcvData[0].Length];
                    Array.Copy(listRcvData[0], 0, OutData, 0, listRcvData[0].Length);       // 배열을 복가한다.
                    listRcvData.RemoveAt(0);                                                // 선두 데이터를 삭제한다.

                    return true;
                }
                catch (Exception ex)
                {
                    CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }

            OutData = null;
            return false;                           // 데이터가 존재하지 않는다.
        }


        // 선두의 데이터를 문자열로 받아온다. (원본 삭제)
        // return 데이터를 정상적으로 받아왔는지 여부, true:정상적으로 데이터를 취득하였다. false:데이터가 존재하지 않는다.
        public bool GetStringData(out string OutMsg)
        {
            if (listRcvData.Count > 0)              // 수신 데이터가 존재하는가 ?
            {
                try
                {
                    if (IsStringUnicode)
                    {
                        OutMsg = Encoding.Unicode.GetString(listRcvData[0], 0, listRcvData[0].Length);  // Unicode
                    }
                    else
                        OutMsg = Encoding.ASCII.GetString(listRcvData[0], 0, listRcvData[0].Length);    // ASCII

                    listRcvData.RemoveAt(0);                                                // 선두 데이터를 삭제한다.

                    return true;
                }
                catch (Exception ex)
                {
                    CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }

            OutMsg = "";
            return false;                           // 데이터가 존재하지 않는다.
        }


        // Event callback 함수 대입
        public void SetCallbackFunction(AsyncCallback fnConnect, AsyncCallback fnDisconnect, AsyncCallback fnReceive)
        {
            fnCallbackConnect = fnConnect;          // 연결이 이루어질 경우 발생되는 callback 함수
            fnCallbackDisconnect = fnDisconnect;    // 연결이 끊어졌을 경우 발생되는 callback 함수
            fnCallbackReceive = fnReceive;          // 데이터가 수신되었을때 수행되는 callback 함수
        }

        // 개별적인 callback 함수 대입
        // 연결이 된 경우 호출
        public void SetCallbackConnect(AsyncCallback fn)
        {
            fnCallbackConnect = fn;         // 연결이 이루어질 경우 발생되는 callback 함수
        }

        // 연결이 끊어진 경우 호출
        public void SetCallbackDisconnect(AsyncCallback fn)
        {
            fnCallbackDisconnect = fn;     // 연결이 끊어졌을 경우 발생되는 callback 함수
        }

        // 데이터가 수신된 경우 호출
        public void SetCallbackReceive(AsyncCallback fn)
        {
            fnCallbackReceive = fn;      // 데이터가 수신되었을때 수행되는 callback 함수
        }


        // Client socket이 재접속을 하도록 만들어주는 Timer event 생성
        private void OnTryConnectEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            CLOG.COMM("The Elapsed event was raised at {0:HH:mm:ss.fff}", e.SignalTime);            

            // Client socket이 초기화 되었다면 새로이 생성하고 연결을 시도한다.
            if (socketClient == null)
            {
                // 자동 연결에 대한 동작이 중지되었다면 아무런 수행도 하지 않는다.
                // (사용자가 명시적으로 ClientClose를 수행한 경우)
                if (bAutoConnectFlag == false) return;

                // 연결을 시도하고 연결 지령을 성공적으로 수행하지 못했다면
                if (Connect() == false)
                {
                    try
                    {
                        socketClient.Close();           // socket  객체를 닫고 초기화 해준다.
                    }
                    catch (Exception ex)
                    {
                        CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                    }

                    socketClient = null;

                    tmrTryConnect.Interval = ConnectDelay;      // 다시 연결을 지령하기위한 지연 시간
                }
                else // 연결 요청 성공
                    tmrTryConnect.Interval = ConnectTimeout;    // 연결대기 시간초과

                tmrTryConnect.Start();          // 다시 타이머를 동작시킨다.
            }
            else      // socket 객체가 생성되어있다면, 이전 단계에서 connect를 시도하였다.
            {
                // 아직까지 연결이 이루어지지 않았다면 Timeout으로 간주한다.
                if (IsConnected() == false)
                {
                    try
                    {
                        socketClient.Close();                       // 연결을 중지시킨다.
                    }
                    catch (Exception ex)
                    {
                        CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                    }
                }

                // Close 이후로는 Receive event가 발생하면서 Read size가 0이 되어 disconnect 처리하게 되어있다.
                // 그리고 이 Timer는 1회성 timer이므로 명시적으로 start 시켜주지 않으면 반복되지 않게된다.
            }
        }


        // 수신대기를 시작한다.
        //public bool SetListen()                              // Server 수신대기를 시작한다.
        //{
        //    return SetListen();                      // 내정된 Port로 listen을 수행한디ㅏ.
        //}

        // 지정 Port로 수신대기를 시작한다.
        public bool SetListen()                    // Server 수신대기를 시작한다.
        {
            IPAddress ipAddress = IPAddress.Parse(m_strIp);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);
            IPEndPoint localEPoint = new IPEndPoint(ipAddress, Port);         // 포트 지정

            try
            {
                socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);    // server Socket class
                socketServer.Bind(localEPoint);                           // 지정 포트로 바인딩 한다.
                socketServer.Listen(10);
                socketServer.BeginAccept(fnAcceptHandle, this); //  socketServer);   // Accept call back 함수를 지정한다.

                bConnected = true;

                CLOG.COMM("!!SetListen({ nPort}) ok");                
                return true;
            }
            catch (SocketException exp)
            {
                CLOG.COMM("Socket에 액세스하려고 시도하는 동안 오류가 발생했습니다.");                
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;
        }

        // Server의 연결 대기를 종료한다.
        public void StopListen()
        {
            try
            {
                if (socketClient != null)
                {
                    socketClient.Disconnect(false);  //  .BeginDisconnect(true, OnDisConnectCallback, socketClient);
                    //socketClient.Close();
                }

                socketServer.Close();
                //socketClient = null;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

        }

        // Client 연결을 닫는다.
        public void CloseClient()
        {
            try
            {
                bAutoConnectFlag = false;                       // 명시적으로 연결을 끊는경우에는 자동 재접속을 수행하지 않도록 한다.
                socketClient.Disconnect(false);                 // 전송중인 데이터를 모두 종료하고 연결을 끊는다.

                // 이후 Receive event가 발생하며 Reading 길이가 0인 경우가 발생하므로 이를 최종 Close 단계로 인식하면 된다.

                //d socketClient.Shutdown(SocketShutdown.Both);  // 즉시 연결을 끊는다. 
                //d socketClient.Close();
                //d socketClient = null;                
                CLOG.COMM("Close server connected Client ...");
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        // 연결 요청이 발생하면 호출될 콜백 함수
        public void OnAcceptCallback(IAsyncResult ar)
        {
            // 접속을 허용시킨다.
            try
            {
                // Get the socket that handles the client request.  
                socketClient = socketServer.EndAccept(ar);                // Accept를 끝낸다.
                socketClient.BeginReceive(byBuffer, 0, byBuffer.Length, 0, fnReceiveHandle, this);   // 수신 Callback 함수를 대입한다.

                evtConnectDone.Set();               // 연결이 설정되었다.

                if (fnCallbackConnect != null)
                {
                    fnCallbackConnect(ar);          // 연결이 이루어질 경우 발생되는 callback 함수 호출
                }

                // 연결이 끊어 진 뒤 다시 재연결이 가능하도록 Accept를 재가동 시킨다.
                socketServer.BeginAccept(fnAcceptHandle, this); //  socketServer);   // Accept call back 함수를 지정한다.
                CLOG.COMM("!! OnAcceptCallback() EndAccept : {socketClient.Connected}");                
                return;
            }
            catch (ObjectDisposedException exp)
            {
                CLOG.ABNORMAL( "Socket 개체가 닫혔습니다.");                
            }
            catch (ArgumentNullException exp)
            {
                CLOG.ABNORMAL( "asyncResult가 비어 있습니다.");                
            }
            catch (ArgumentException exp)
            {
                CLOG.ABNORMAL( "BeginAccept(AsyncCallback, Object)를 호출했지만 asyncResult가 만들어지지 않았습니다.");                                
            }
            catch (InvalidOperationException exp)
            {
                CLOG.ABNORMAL( "EndAccept(IAsyncResult) 메서드가 이미 호출되었습니다.");                
            }
            catch (SocketException exp)
            {
                CLOG.ABNORMAL( "Socket에 액세스하려고 시도하는 동안 오류가 발생했습니다.");                
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            evtConnectDone.Reset();         // 연결이 해제되었다.

            CLOG.COMM("---- OnAcceptCallback() EndAccept ----");            
        }


        // Client가 접속 할 주소를 지정한다.
        public void SetAddress(string sAddr, int nPort)
        {
            sIPAddress = sAddr;                      // 접속 할 IP 주소
            nPortNo = nPort;                         // 접속 할 Port 주소
        }


        // 기존에 설정된 주소로 접속을 시도한다.
        public bool Connect()
        {
            return Connect(sIPAddress, nPortNo);
        }


        // Client socket이 begin connect를 수행한다.
        public bool Connect(string sAddr, int nPort)
        {
            bAutoConnectFlag = true;

            if (socketClient == null)
            {
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);    // server Socket class
                CLOG.COMM("Create new stocket instance {0}:{1}",sAddr, nPort);                
            }

            try
            {
                CLOG.COMM("!! BeginConnect to {0}:{1}", sAddr, nPort);
                                
                arConnect = socketClient.BeginConnect(sAddr, nPort, fnConnectHandle, this); //  socketClient);
                
                return true;
            }
            catch (ArgumentNullException exp)
            {
                CLOG.ABNORMAL( "host이(가) null입니다.");                
            }
            catch (ArgumentOutOfRangeException exp)
            {
                CLOG.ABNORMAL( "포트 번호가 잘못되었습니다.");                
            }
            catch (SocketException exp)
            {
                CLOG.ABNORMAL( "소켓에 액세스하는 동안 오류가 발생했습니다.");               
            }
            catch (ObjectDisposedException exp)
            {
                CLOG.ABNORMAL( "Socket이 닫혔습니다.");                
            }
            catch (NotSupportedException exp)
            {
                CLOG.ABNORMAL( "이 메서드는 InterNetwork 또는 InterNetworkV6 제품군의 소켓에 유효합니다.");                
            }
            catch (InvalidOperationException exp)
            {
                CLOG.ABNORMAL( "Socket이 Listen(Int32)을 호출하여 수신 상태에 배치되었습니다.");                
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;       // 예외 발생시 수행 실패
        }


        public bool IsConnected()
        {
            if (socketClient == null) return false;

            try
            {
                return !((socketClient.Poll(1000, SelectMode.SelectRead) && (socketClient.Available == 0)) || !socketClient.Connected);
            }
            catch (SocketException exp)
            {
                CLOG.ABNORMAL( "소켓에 액세스하는 동안 오류가 발생했습니다.");                
            }
            catch (ObjectDisposedException exp)
            {
                CLOG.ABNORMAL( "Socket이 닫혔습니다.");                
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return false;
        }



        // 접속 시도에 대한 Callback event 발생
        public void OnConnectCallback(IAsyncResult ar)
        {
            try
            {
                socketClient.EndConnect(ar);            // 연결시도 종료
                socketClient.BeginReceive(byBuffer, 0, byBuffer.Length, 0, fnReceiveHandle, this); // socketClient);   // 수신 Callback 함수를 대입한다.

                tmrTryConnect.Stop();                   // Timeout 처리 및 재접속 timer 중지
                nAbnormalCount = 0;                     // 비정상 상태 반복횟수
                evtConnectDone.Set();                   // 연결이 설정되었다.

                if (fnCallbackConnect != null)
                {
                    fnCallbackConnect(ar);              // 연결이 이루어질 경우 발생되는 callback 함수
                }

                CLOG.COMM("!! OnConnectCallback(), connected : {socketClient.Connected}");                
                return;
            }
            catch (ArgumentNullException exp)
            {
                CLOG.ABNORMAL( "host이(가) null인 경우");                
            }
            catch (ArgumentOutOfRangeException exp)
            {
                CLOG.ABNORMAL( "포트 번호가 잘못된 경우");                
            }
            catch (SocketException exp)
            {
                CLOG.ABNORMAL( "소켓에 액세스하는 동안 오류가 발생했습니다.");                
            }
            catch (ObjectDisposedException exp)
            {
                CLOG.ABNORMAL( "Socket이 닫혔습니다.");                
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            // 정상적이지 않다.
            CLOG.ABNORMAL( "OnConnectCallback(), connected fail");            

            socketClient.Close();
            socketClient = null;
            evtConnectDone.Reset();         // 연결이 해제되었다.

            // 자동으로 재접속을 시도하기로 지정되었다면
            if (IsAutoConnectTry)
            {
                tmrTryConnect.Interval = ConnectDelay;      // 다시 연결을 지령하기위한 지연 시간
                tmrTryConnect.Start();          // 다시 타이머를 동작시킨다.
            }

        }


        // 데이터 수신이 이루어졌을때 발생하는 Callback 함수
        public void OnReceiveCallback(IAsyncResult ar)
        {
            nRecvLength = 0;  

            try
            {
                // 자료를 수신하고, 수신받은 바이트를 가져옵니다.
                nRecvLength = socketClient.EndReceive(ar);
            }
            catch (ArgumentNullException exp)
            {
                CLOG.ABNORMAL( "host이(가) null인 경우");
            }
            catch (ArgumentOutOfRangeException exp)
            {
                CLOG.ABNORMAL( "포트 번호가 잘못된 경우");
            }
            catch (SocketException exp)
            {
                CLOG.ABNORMAL( "소켓에 액세스하는 동안 오류가 발생했습니다.");
            }
            catch (ObjectDisposedException exp)
            {
                CLOG.ABNORMAL( "Socket이 닫혔습니다.");
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }


            // 수신받은 자료의 크기가 1 이상일 때에만 자료 처리
            if (nRecvLength > 0)
            {
                byte[] data = new byte[nRecvLength];            // 수신된 데이터 수 만큼 배열을 잡는다.

                Array.Copy(byBuffer, 0, data, 0, nRecvLength);  // 수신된 길이만큼 내용을 복사한다

                listRcvData.Add(data);                          // 복사된 수신 데이터를 list에 보관한다.

                
                if (IsLogData) // 송수신 메세지를 로그로 기록하라고 되어있다면
                {

                    if (IsStringData)      // 전송데이터가 문자열인가 바이너리인가 ?
                    {
                        string sMsg;

                        if (IsStringUnicode) // 문자열을 Unicode로 송/수신 할것인가 ?
                        {
                            sMsg = Encoding.Unicode.GetString(byBuffer, 0, nRecvLength);
                        }
                        else
                            sMsg = Encoding.ASCII.GetString(byBuffer, 0, nRecvLength);
                    }
                    else
                    {       // Binary data
                        StringBuilder sbMsg = new StringBuilder();

                        for (int i = 0; i < data.Length; i++)
                        {
                            sbMsg.Append($"{data[i]:X2} ");     // 2자리 16진수로 표시
                        }
                    }

                }

                // 수신데이터 처리를 위한 event set 최종 사용자가 사용한다.
                evtReceiveFlag.Set();

                // 수신완료 함수 수행
                if (fnCallbackReceive != null)
                {
                    fnCallbackReceive(ar);              // 데이터 수신이 이루어질 경우 발생되는 callback 함수
                }

                // byBuffer.Initialize(); -> 정상 동작하지 않음
                Array.Clear(byBuffer, 0x0, byBuffer.Length);            // Buffer Clear

                // 다음 수신을 대기한다.
                socketClient.BeginReceive(byBuffer, 0, byBuffer.Length, 0, fnReceiveHandle, this); // socketClient);
            }
            else 
            {
                // 수신된 Size가 0일 경우 연결이 끊어진 것이다.
                if (IsLogEvent) //log.Write($"!! OnReceiveCallback : receive {nRecvLength} Bytes -> Client close");

                evtConnectDone.Reset();         // 연결이 해제되었다.

                if (fnCallbackDisconnect != null)
                {
                    fnCallbackDisconnect(ar);              // 연결이 끊어질 경우 발생되는 callback 함수
                }

                socketClient.Close();
                socketClient = null;

                // 자동으로 재접속을 시도하기로 지정되었다면
                if (IsAutoConnectTry)
                {
                    tmrTryConnect.Interval = ConnectDelay;      // 다시 연결을 지령하기위한 지연 시간
                    tmrTryConnect.Start();          // 다시 타이머를 동작시킨다.
                }

            }
        }

        // 데이터 송신이 이루어졌을때 발생하는 Callback 함수
        public void OnSendCallback(IAsyncResult ar)
        {
            try
            {
                nSendLength = socketClient.EndSend(ar);                     // 송신 처리
                evtSendDone.Set();

               // if (IsLogEventReceive) log.Write($"!! OnSendCallback : {nSendLength}");
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);

                nSendLength = 0;                                            // 송신 실패
                                                                            // if (IsLogException) log.Write($"** OnSendCallback() SocketException : {exp.Message}");

                evtConnectDone.Reset();         // 연결이 해제되었다.

            }
        }


        // 문자열을 전송한다. (비동기 방식)
        public bool SendStringASync(string sMsg)
        {
            if (sMsg.Length <= 0) return false;                     // 전송 할 내용이 없다면 전송 실패


            byte[] data = IsUnicode ? Encoding.Unicode.GetBytes(sMsg) : Encoding.ASCII.GetBytes(sMsg);

            try
            {
                // 지정 데이터를 전송 시도한다.
                nSendLength = 0;
                evtSendDone.Reset();

                socketClient.BeginSend(data, 0, data.Length, 0, fnSendHandle, this);    // socketClient);

                nAbnormalCount = 0;                      // 비정상 상태 반복횟수 clear

                return true;
            }
            catch (ArgumentNullException exp)
            {
                CLOG.ABNORMAL( "host이(가) null인 경우");
            }
            catch (ArgumentOutOfRangeException exp)
            {
                CLOG.ABNORMAL( "포트 번호가 잘못된 경우");
            }
            catch (SocketException exp)
            {
                CLOG.ABNORMAL( "소켓에 액세스하는 동안 오류가 발생했습니다.");
            }
            catch (ObjectDisposedException exp)
            {
                CLOG.ABNORMAL( "Socket이 닫혔습니다.");
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            ++nAbnormalCount;                      // 비정상 상태 반복횟수
            return false;
        }


        // 문자열을 전송한다. (동기 방식)
        public bool SendString(string sMsg)
        {
            if (sMsg.Length <= 0) return false;                     // 전송 할 내용이 없다면 전송 실패

            byte[] data;

            if (IsStringUnicode) // 문자열을 Unicode로 송/수신 할것인가 ?
            {
                // sMsg = Encoding.Unicode.GetString(byBuffer, 0, nRecvLength);
                data = Encoding.Unicode.GetBytes(sMsg);
            }
            else
                data = Encoding.ASCII.GetBytes(sMsg);

            // byte[] data = Encoding.Unicode.GetBytes(sMsg);
            // byte[] data = Encoding.ASCII.GetBytes(sMsg);

            try
            {
                // 지정 데이터를 전송 시도한다.
                evtSendDone.Reset();
                nSendLength = socketClient.Send(data, data.Length, 0);  // , MSG_NOSIGNAL);

                nAbnormalCount = 0;                                   // 비정상 상태 반복횟수 clear
                evtSendDone.Set();

                if (IsLogData) // 송수신 메세지를 로그로 기록하라고 되어있다면
                {

                    if (IsStringData)      // 전송데이터가 문자열인가 바이너리인가 ?
                    {
                    }
                    else
                    {       // Binary data
                        StringBuilder sbMsg = new StringBuilder();

                        for (int i = 0; i < data.Length; i++)
                        {
                            sbMsg.Append($"{data[i]:X2} ");
                        }
                    }
                }

                return (nSendLength > 0);
            }
            catch (ArgumentNullException exp)
            {
                CLOG.ABNORMAL( "buffers은 null입니다.");                
            }
            catch (ArgumentException exp)
            {
                CLOG.ABNORMAL( "buffers가 비어 있는 경우");                
            }
            catch (SocketException exp)
            {
                CLOG.ABNORMAL( "소켓에 액세스하는 동안 오류가 발생했습니다.아래의 설명 부분을 참조하십시오.");                
            }
            catch (ObjectDisposedException exp)
            {
                CLOG.ABNORMAL( "Socket이 닫혔습니다.");                
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            ++nAbnormalCount;                      // 비정상 상태 반복횟수
            return false;
        }

        // 지정 Byte 배열의 데이터를 전송한다. (동기 방식)
        public bool Send(byte[] SendData)
        {
            if (SendData.Length <= 0) return false;                     // 전송 할 내용이 없다면 전송 실패

            try
            {
                // 지정 데이터를 전송 시도한다.
                evtSendDone.Reset();
                nSendLength = socketClient.Send(SendData, SendData.Length, 0);

                nAbnormalCount = 0;                                   // 비정상 상태 반복횟수 clear
                evtSendDone.Set();

                if (IsLogData) // 송수신 메세지를 로그로 기록하라고 되어있다면
                {
                    // Binary data
                    StringBuilder sbMsg = new StringBuilder();

                    for (int i = 0; i < SendData.Length; i++)
                    {
                        sbMsg.Append($"{SendData[i]:X2} ");
                    }
                }

                return (nSendLength > 0);
            }
            catch (ArgumentNullException exp)
            {
                CLOG.ABNORMAL( "buffers은 null입니다.");
            }
            catch (ArgumentException exp)
            {
                CLOG.ABNORMAL( "buffers가 비어 있는 경우");
            }
            catch (SocketException exp)
            {
                CLOG.ABNORMAL( "소켓에 액세스하는 동안 오류가 발생했습니다.아래의 설명 부분을 참조하십시오.");
            }
            catch (ObjectDisposedException exp)
            {
                CLOG.ABNORMAL( "Socket이 닫혔습니다.");
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            ++nAbnormalCount;                      // 비정상 상태 반복횟수
            return false;
        }

        #region CONFIG BY XML              
        private string m_XMLName = "TCP";
        public bool LoadConfig()
        {
            try
            {
                string strPath = Application.StartupPath + "\\" + m_XMLName + ".xml";

                if (File.Exists(strPath))
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);

                    try
                    {
                        LoadConfigFromXML(xmlReader);
                    }
                    catch (Exception ex)
                    {
                        CLOG.ABNORMAL( "SYSTEM Load Ex ==> {0}", ex.Message);
                        xmlReader.Close();
                    }

                    xmlReader.Close();
                }
                else
                {
                    SaveConfig();
                    return false;
                }
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
            return true;
        }

        public bool LoadConfigFromXML(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    CLOG.NORMAL( "CONFIG [{0}] ==> {1}", xmlReader.Name, xmlReader.Value);

                    switch (xmlReader.Name)
                    {
                        case "IP":
                            if (!xmlReader.Read()) return false;
                            IP = xmlReader.Value;
                            break;
                        case "Port":
                            if (!xmlReader.Read()) return false;
                            Port = int.Parse(xmlReader.Value);
                            break;
                    }
                }
                else
                {
                    if (xmlReader.NodeType == XmlNodeType.EndElement)
                    {
                        if (xmlReader.Name == m_XMLName) break;
                    }
                }
            }

            CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public bool SaveConfig()
        {
            string strPath = Application.StartupPath + "\\" + m_XMLName + ".xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            XmlWriter xmlWriter = XmlWriter.Create(strPath, settings);
            try
            {
                xmlWriter.WriteStartDocument();
                SaveConfigToXML(xmlWriter);
                xmlWriter.WriteEndDocument();
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }

            CLOG.NORMAL( "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public bool SaveConfigToXML(XmlWriter xmlWriter)
        {
            try
            {
                xmlWriter.WriteStartElement("SYSTEM");
                xmlWriter.WriteElementString("IP", IP);
                xmlWriter.WriteElementString("Port", Port.ToString());
                xmlWriter.WriteEndElement();
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "SYSTEM Save Ex ==> {0}", ex.Message);
            }

            return true;
        }
        #endregion

    } //of public class CGxSocket


}
