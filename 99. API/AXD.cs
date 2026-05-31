/****************************************************************************
*****************************************************************************
**
** File Name
** ---------
**
** AXD.CS
**
** COPYRIGHT (c) AJINEXTEK Co., LTD
**
*****************************************************************************
*****************************************************************************
**
** Description
** -----------
** Ajinextek Digital Library Header File
** 
**
*****************************************************************************
*****************************************************************************
**
** Source Change Indices
** ---------------------
** 
** (None)
**
**
*****************************************************************************
*****************************************************************************
**
** Website
** ---------------------
**
** http://www.ajinextek.com
**
*****************************************************************************
*****************************************************************************
*/

using System;
using System.Runtime.InteropServices;

public class CAXD
{
//========== 보드 및 모듈 정보 =================================================================================

    // DIO 모듈이 있는지 확인
    [DllImport("AXL.dll")] public static extern uint AxdInfoIsDIOModule(ref uint upStatus);
    
    // DIO 모듈 No 확인
    [DllImport("AXL.dll")] public static extern uint AxdInfoGetModuleNo(int lBoardNo, int lModulePos, ref int lpModuleNo);
    
    // DIO 입출력 모듈의 개수 확인
    [DllImport("AXL.dll")] public static extern uint AxdInfoGetModuleCount(ref int lpModuleCount);
    
    // 지정한 모듈의 입력 접점 개수 확인
    [DllImport("AXL.dll")] public static extern uint AxdInfoGetInputCount(int lModuleNo, ref int lpCount);
    
    // 지정한 모듈의 출력 접점 개수 확인
    [DllImport("AXL.dll")] public static extern uint AxdInfoGetOutputCount(int lModuleNo, ref int lpCount);
    
    // 지정한 모듈 번호로 베이스 보드 번호, 모듈 위치, 모듈 ID 확인
    [DllImport("AXL.dll")] public static extern uint AxdInfoGetModule(int lModuleNo, ref int lpBoardNo, ref int lpModulePos, ref uint upModuleID);

    // 해당 모듈이 제어가 가능한 상태인지 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxdInfoGetModuleStatus(int lModuleNo);

//========== 인터럽트 설정 확인 =================================================================================

    // 지정한 모듈에 인터럽트 메시지를 받아오기 위하여 윈도우 메시지, 콜백 함수 또는 이벤트 방식을 사용
    //========= 인터럽트 관련 함수 ======================================================================================
    // 콜백 함수 방식은 이벤트 발생 시점에 즉시 콜백 함수가 호출 됨으로 가장 빠르게 이벤트를 통지받을 수 있는 장점이 있으나
    // 콜백 함수가 완전히 종료 될 때까지 메인 프로세스가 정체되어 있게 된다.
    // 즉, 콜백 함수 내에 부하가 걸리는 작업이 있을 경우에는 사용에 주의를 요한다. 
    // 이벤트 방식은 쓰레드등을 이용하여 인터럽트 발생여부를 지속적으로 감시하고 있다가 인터럽트가 발생하면 
    // 처리해주는 방법으로, 쓰레드 등으로 인해 시스템 자원을 점유하고 있는 단점이 있지만
    // 가장 빠르게 인터럽트를 검출하고 처리해줄 수 있는 장점이 있다.
    // 일반적으로는 많이 쓰이지 않지만, 인터럽트의 빠른처리가 주요 관심사인 경우에 사용된다. 
    // 이벤트 방식은 이벤트의 발생 여부를 감시하는 특정 쓰레드를 사용하여 메인 프로세스와 별개로 동작되므로
    // MultiProcessor 시스템등에서 자원을 가장 효율적으로 사용할 수 있게 되어 특히 권장하는 방식이다.
    // 인터럽트 메시지를 받아오기 위하여 윈도우 메시지 또는 콜백 함수를 사용한다.
    // (메시지 핸들, 메시지 ID, 콜백함수, 인터럽트 이벤트)
    //    hWnd            : 윈도우 핸들, 윈도우 메세지를 받을때 사용. 사용하지 않으면 NULL을 입력.
    //    uMessage        : 윈도우 핸들의 메세지, 사용하지 않거나 디폴트값을 사용하려면 0을 입력.
    //    proc            : 인터럽트 발생시 호출될 함수의 포인터, 사용하지 않으면 NULL을 입력.
    //    pEvent          : 이벤트 방법사용시 이벤트 핸들
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptSetModule(int lModuleNo, IntPtr hWnd, uint uMessage, CAXHS.AXT_INTERRUPT_PROC pProc, ref uint pEvent);
    
    // 지정한 모듈의 인터럽트 사용 유무 설정
    //======================================================//
    // uUse        : DISABLE(0)    // 인터럽트 해제
    //             : ENABLE(1)     // 인터럽트 설정
    //======================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptSetModuleEnable(int lModuleNo, uint uUse);
    
    // 지정한 모듈의 인터럽트 사용 유무 확인
    //======================================================//
    // *upUse      : DISABLE(0)    // 인터럽트 해제
    //             : ENABLE(1)     // 인터럽트 설정
    //======================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptGetModuleEnable(int lModuleNo, ref uint upUse);
    
    // 인터럽트 발생 위치 확인
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptRead(ref int lpModuleNo, ref uint upFlag);

//========== 인터럽트 상승 / 하강 에지 설정 확인 =================================================================================
    // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 bit 단위로 상승 또는 하강 에지 값을 설정
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uMode        : DOWN_EDGE(0)
    //              : UP_EDGE(1)
    // uValue       : DISABLE(0)
    //              : ENABLE(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptEdgeSetBit(int lModuleNo, int lOffset, uint uMode, uint uValue);
    
    // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 byte 단위로 상승 또는 하강 에지 값을 설정
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uMode        : DOWN_EDGE(0)
    //              : UP_EDGE(1)
    // uValue       : 0x00 ~ 0x0FF ('1'로 Setting 된 부분 인터럽트 설정)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptEdgeSetByte(int lModuleNo, int lOffset, uint uMode, uint uValue);
    
    // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 word 단위로 상승 또는 하강 에지 값을 설정
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uMode        : DOWN_EDGE(0)
    //              : UP_EDGE(1)
    // uValue       : 0x00 ~ 0x0FFFF ('1'로 Setting 된 부분 인터럽트 설정)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptEdgeSetWord(int lModuleNo, int lOffset, uint uMode, uint uValue);
    
    // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 double word 단위로 상승 또는 하강 에지 값을 설정
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uMode        : DOWN_EDGE(0)
    //              : UP_EDGE(1)
    // uValue       : 0x00 ~ 0x0FFFFFFFF ('1'로 Setting 된 부분 인터럽트 설정)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptEdgeSetDword(int lModuleNo, int lOffset, uint uMode, uint uValue);
    
    // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 bit 단위로 상승 또는 하강 에지 값을 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uMode        : DOWN_EDGE(0)
    //              : UP_EDGE(1)
    // *upValue     : 0x00 ~ 0x0FF ('1'로 Setting 된 부분 인터럽트 설정)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptEdgeGetBit(int lModuleNo, int lOffset, uint uMode, ref uint upValue);
    
    // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 byte 단위로 상승 또는 하강 에지 값을 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uMode        : DOWN_EDGE(0)
    //              : UP_EDGE(1)
    // *upValue     : 0x00 ~ 0x0FF ('1'로 Setting 된 부분 인터럽트 설정)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptEdgeGetByte(int lModuleNo, int lOffset, uint uMode, ref uint upValue);
    
    // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 word 단위로 상승 또는 하강 에지 값을 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uMode        : DOWN_EDGE(0)
    //              : UP_EDGE(1)
    // *upValue     : 0x00 ~ 0x0FFFFFFFF ('1'로 Setting 된 부분 인터럽트 설정)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptEdgeGetWord(int lModuleNo, int lOffset, uint uMode, ref uint upValue);
    
    // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 double word 단위로 상승 또는 하강 에지 값을 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uMode        : DOWN_EDGE(0)
    //              : UP_EDGE(1)
    // *upValue     : 0x00 ~ 0x0FFFFFFFF ('1'로 Setting 된 부분 인터럽트 설정)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptEdgeGetDword(int lModuleNo, int lOffset, uint uMode, ref uint upValue);
    
    // 전체 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 bit 단위로 상승 또는 하강 에지 값을 설정
    //===============================================================================================//
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uMode        : DOWN_EDGE(0)
    //              : UP_EDGE(1)
    // uValue       : DISABLE(0)
    //              : ENABLE(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptEdgeSet(int lOffset, uint uMode, uint uValue);
    
    // 전체 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위정에서 bit 단위로 상승 또는 하강 에지 값을 확인
    //===============================================================================================//
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uMode        : DOWN_EDGE(0)
    //              : UP_EDGE(1)
    // *upValue     : DISABLE(0)
    //              : ENABLE(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptEdgeGet(int lOffset, uint uMode, ref uint upValue);

//========== 입출력 레벨 설정 확인 =================================================================================
//==입력 레벨 설정 확인
    // 지정한 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uLevel       : LOW(0)
    //              : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiLevelSetInportBit(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uLevel       : 0x00 ~ 0x0FF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiLevelSetInportByte(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uLevel       : 0x00 ~ 0x0FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiLevelSetInportWord(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 double word 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uLevel       : 0x00 ~ 0x0FFFFFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiLevelSetInportDword(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // *upLevel     : LOW(0)
    //              : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiLevelGetInportBit(int lModuleNo, int lOffset, ref uint upLevel);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // *upLevel     : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiLevelGetInportByte(int lModuleNo, int lOffset, ref uint upLevel);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // *upLevel     : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiLevelGetInportWord(int lModuleNo, int lOffset, ref uint upLevel);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 double word 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // *upLevel     : 0x00 ~ 0x0FFFFFFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiLevelGetInportDword(int lModuleNo, int lOffset, ref uint upLevel);
    
    // 전체 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lOffset      : 입력 접점에 대한 Offset 위치
    // uLevel       : LOW(0)
    //              : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiLevelSetInport(int lOffset, uint uLevel);
    
    // 전체 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // *upLevel     : LOW(0)
    //              : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiLevelGetInport(int lOffset, ref uint upLevel);
    
//==출력 레벨 설정 확인
    // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // uLevel       : LOW(0)
    //              : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoLevelSetOutportBit(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // uLevel       : 0x00 ~ 0x0FF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoLevelSetOutportByte(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // uLevel       : 0x00 ~ 0x0FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoLevelSetOutportWord(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 double word 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // uLevel       : 0x00 ~ 0x0FFFFFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoLevelSetOutportDword(int lModuleNo, int lOffset, uint uLevel);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // *upLevel     : LOW(0)
    //              : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoLevelGetOutportBit(int lModuleNo, int lOffset, ref uint upLevel);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // uLevel       : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoLevelGetOutportByte(int lModuleNo, int lOffset, ref uint upLevel);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // uLevel       : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoLevelGetOutportWord(int lModuleNo, int lOffset, ref uint upLevel);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 double word 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // uLevel       : 0x00 ~ 0x0FFFFFFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoLevelGetOutportDword(int lModuleNo, int lOffset, ref uint upLevel);
    
    // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lOffset      : 출력 접점에 대한 Offset 위치
    // uLevel       : LOW(0)
    //              : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoLevelSetOutport(int lOffset, uint uLevel);
    
    // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
    //===============================================================================================//
    // lOffset      : 출력 접점에 대한 Offset 위치
    // *upLevel     : LOW(0)
    //              : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoLevelGetOutport(int lOffset, ref uint upLevel);
    
//========== 입출력 포트 쓰기 읽기 =================================================================================
//==출력 포트 쓰기
    // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 출력
    //===============================================================================================//
    // lOffset      : 출력 접점에 대한 Offset 위치
    // uLevel       : LOW(0)
    //              : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoWriteOutport(int lOffset, uint uValue);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 출력
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // uLevel       : LOW(0)
    //              : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoWriteOutportBit(int lModuleNo, int lOffset, uint uValue);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터를 출력
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // uValue       : 0x00 ~ 0x0FF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoWriteOutportByte(int lModuleNo, int lOffset, uint uValue);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터를 출력
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // uValue       : 0x00 ~ 0x0FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoWriteOutportWord(int lModuleNo, int lOffset, uint uValue);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 double word 단위로 데이터를 출력
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // uValue       : 0x00 ~ 0x0FFFFFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoWriteOutportDword(int lModuleNo, int lOffset, uint uValue);
    
//==출력 포트 읽기    
    // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
    //===============================================================================================//
    // lOffset      : 출력 접점에 대한 Offset 위치
    // *upLevel     : LOW(0)
    //              : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoReadOutport(int lOffset, ref uint upValue);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // *upLevel     : LOW(0)
    //              : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoReadOutportBit(int lModuleNo, int lOffset, ref uint upValue);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // *upValue     : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoReadOutportByte(int lModuleNo, int lOffset, ref uint upValue);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // *upValue     : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoReadOutportWord(int lModuleNo, int lOffset, ref uint upValue);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 double word 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // *upValue     : 0x00 ~ 0x0FFFFFFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoReadOutportDword(int lModuleNo, int lOffset, ref uint upValue);
    
//==입력 포트 일기    
    // 전체 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
    //===============================================================================================//
    // lOffset      : 입력 접점에 대한 Offset 위치
    // *upValue     : LOW(0)
    //              : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiReadInport(int lOffset, ref uint upValue);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // *upValue     : LOW(0)
    //              : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiReadInportBit(int lModuleNo, int lOffset, ref uint upValue);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 byte 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // *upValue     : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiReadInportByte(int lModuleNo, int lOffset, ref uint upValue);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 word 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // *upValue     : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiReadInportWord(int lModuleNo, int lOffset, ref uint upValue);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 double word 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // *upValue     : 0x00 ~ 0x0FFFFFFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiReadInportDword(int lModuleNo, int lOffset, ref uint upValue);

    //== MLII 용 M-Systems DIO(R7 series) 전용 함수.
    // 지정한 모듈에 장착된 입력 접점용 확장 기능 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 입력 접점에 대한 Offset 위치(0~15)
    // *upValue    : LOW(0)
    //             : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdReadExtInportBit(int lModuleNo, int lOffset, ref uint upValue);

    // 지정한 모듈에 장착된 입력 접점용 확장 기능 모듈의 Offset 위치에서 byte 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 입력 접점에 대한 Offset 위치(0~1)
    // *upValue    : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdReadExtInportByte(int lModuleNo, int lOffset, ref uint upValue);

    // 지정한 모듈에 장착된 입력 접점용 확장 기능 모듈의 Offset 위치에서 word 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 입력 접점에 대한 Offset 위치(0)
    // *upValue    : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdReadExtInportWord(int lModuleNo, int lOffset, ref uint upValue);

    // 지정한 모듈에 장착된 입력 접점용 확장 기능 모듈의 Offset 위치에서 dword 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 입력 접점에 대한 Offset 위치(0)
    // *upValue    : 0x00 ~ 0x00000FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdReadExtInportDword(int lModuleNo, int lOffset, ref uint upValue);

    // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 출력 접점에 대한 Offset 위치(0~15)
    // *upValue    : LOW(0)
    //             : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdReadExtOutportBit(int lModuleNo, int lOffset, ref uint upValue);

    // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 byte 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 출력 접점에 대한 Offset 위치(0~1)
    // *upValue    : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdReadExtOutportByte(int lModuleNo, int lOffset, ref uint upValue);

    // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 word 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 출력 접점에 대한 Offset 위치(0)
    // *upValue    : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdReadExtOutportWord(int lModuleNo, int lOffset, ref uint upValue);

    // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 dword 단위로 데이터를 읽기
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 출력 접점에 대한 Offset 위치(0)
    // *upValue    : 0x00 ~ 0x00000FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdReadExtOutportDword(int lModuleNo, int lOffset, ref uint upValue);

    // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 bit 단위로 데이터 출력
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 출력 접점에 대한 Offset 위치
    // uValue      : LOW(0)
    //             : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdWriteExtOutportBit(int lModuleNo, int lOffset, uint uValue);

    // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 byte 단위로 데이터 출력
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 출력 접점에 대한 Offset 위치(0~1)
    // uValue      : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdWriteExtOutportByte(int lModuleNo, int lOffset, uint uValue);

    // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 word 단위로 데이터 출력
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 출력 접점에 대한 Offset 위치(0)
    // uValue    : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdWriteExtOutportWord(int lModuleNo, int lOffset, uint uValue);

    // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 dword 단위로 데이터 출력
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 출력 접점에 대한 Offset 위치(0)
    // uValue    : 0x00 ~ 0x00000FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdWriteExtOutportDword(int lModuleNo, int lOffset, uint uValue);

    // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 입력 접점에 대한 Offset 위치(0~15)
    // uLevel      : LOW(0)
    //             : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdLevelSetExtportBit(int lModuleNo, int lOffset, uint uLevel);

    // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 입력 접점에 대한 Offset 위치(0~1)
    // uLevel      : 0x00 ~ 0xFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdLevelSetExtportByte(int lModuleNo, int lOffset, uint uLevel);

    // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 입력 접점에 대한 Offset 위치(0)
    // uLevel      : 0x00 ~ 0xFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdLevelSetExtportWord(int lModuleNo, int lOffset, uint uLevel);

    // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 dword 단위로 데이터 레벨을 설정
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 입력 접점에 대한 Offset 위치(0)
    // uLevel      : 0x00 ~ 0x0000FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdLevelSetExtportDword(int lModuleNo, int lOffset, uint uLevel);

    // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 bit 단위로 데이터 레벨 확인
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 입력 접점에 대한 Offset 위치(0~15)
    // *upLevel      : LOW(0)
    //             : HIGH(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdLevelGetExtportBit(int lModuleNo, int lOffset, ref uint upLevel);

    // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 byte 단위로 데이터 레벨 확인
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 입력 접점에 대한 Offset 위치(0~1)
    // *upLevel      : 0x00 ~ 0xFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdLevelGetExtportByte(int lModuleNo, int lOffset, ref uint upLevel);

    // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 word 단위로 데이터 레벨 확인
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 입력 접점에 대한 Offset 위치(0)
    // *upLevel      : 0x00 ~ 0xFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdLevelGetExtportWord(int lModuleNo, int lOffset, ref uint upLevel);

    // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 dword 단위로 데이터 레벨 확인
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // lOffset     : 입력 접점에 대한 Offset 위치(0)
    // *upLevel      : 0x00 ~ 0x0000FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdLevelGetExtportDword(int lModuleNo, int lOffset, ref uint upLevel);
    
//========== 고급 함수 =================================================================================
    // 지정한 입력 접점 모듈의 Offset 위치에서 신호가 Off에서 On으로 바뀌었는지 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // *upValue     : FALSE(0)
    //              : TRUE(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiIsPulseOn(int lModuleNo, int lOffset, ref uint upValue);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 신호가 On에서 Off으로 바뀌었는지 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // *upValue     : FALSE(0)
    //              : TRUE(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiIsPulseOff(int lModuleNo, int lOffset, ref uint upValue);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 신호가 count 만큼 호출될 동안 On 상태로 유지하는지 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 입력 접점에 대한 Offset 위치
    // lCount       : 0 ~ 0x7FFFFFFF(2147483647)
    // *upValue     : FALSE(0)
    //              : TRUE(1)
    // lStart       : 1(최초 호출)
    //              : 0(반복 호출)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiIsOn(int lModuleNo, int lOffset, int lCount, ref uint upValue, int lStart);
    
    // 지정한 입력 접점 모듈의 Offset 위치에서 신호가 count 만큼 호출될 동안 Off 상태로 유지하는지 확인
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // lCount       : 0 ~ 0x7FFFFFFF(2147483647)
    // *upValue     : FALSE(0)
    //              : TRUE(1)
    // lStart       : 1(최초 호출)
    //              : 0(반복 호출)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdiIsOff(int lModuleNo, int lOffset, int lCount, ref uint upValue, int lStart);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 설정한 mSec동안 On을 유지하다가 Off 시킴
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // lCount       : 0 ~ 0x7FFFFFFF(2147483647)
    // lmSec        : 1 ~ 30000
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoOutPulseOn(int lModuleNo, int lOffset, int lmSec);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 설정한 mSec동안 Off를 유지하다가 On 시킴
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // lCount       : 0 ~ 0x7FFFFFFF(2147483647)
    // lmSec        : 1 ~ 30000
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoOutPulseOff(int lModuleNo, int lOffset, int lmSec);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 설정한 횟수, 설정한 간격으로 토글한 후 원래의 출력상태를 유지함
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // lInitState   : Off(0)
    //              : On(1)
    // lmSecOn      : 1 ~ 30000
    // lmSecOff     : 1 ~ 30000
    // lCount       : 1 ~ 0x7FFFFFFF(2147483647)
    //              : -1 무한 토글
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoToggleStart(int lModuleNo, int lOffset, int lInitState, int lmSecOn, int lmSecOff, int lCount);
    
    // 지정한 출력 접점 모듈의 Offset 위치에서 토글중인 출력을 설정한 신호 상태로 정지 시킴
    //===============================================================================================//
    // lModuleNo    : 모듈 번호
    // lOffset      : 출력 접점에 대한 Offset 위치
    // uOnOff       : Off(0)
    //              : On(1)
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoToggleStop(int lModuleNo, int lOffset, uint uOnOff);

    // 지정한 출력 모듈의 Network이 끊어 졌을 경우 출력 상태를 출력 Byte 단위로 설정한다.
    //===============================================================================================//
    // lModuleNo   : 모듈 번호
    // dwSize      : 설정 할 Byte 수(ex. RTEX-DB32 : 2, RTEX-DO32 : 4)
    // dwaSetValue : 설정 할 변수 값(Default는 Network 끊어 지기 전 상태 유지)
    //             : 0 --> Network 끊어 지기 전 상태 유지
    //             : 1 --> On
    //             : 2 --> Off
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxdoSetNetworkErrorAct(int lModuleNo, uint dwSize, ref uint dwaSetValue);

    [DllImport("AXL.dll")] public static extern uint AxdSetContactNum(int lModuleNo, uint dwInputNum, uint dwOutputNum);

    [DllImport("AXL.dll")] public static extern uint AxdGetContactNum(int lModuleNo, ref uint dwpInputNum, ref uint dwpOutputNum);
}

