/****************************************************************************
*****************************************************************************
**
** File Name
** ----------
**
** AXA.CS
**
** COPYRIGHT (c) AJINEXTEK Co., LTD
**
*****************************************************************************
*****************************************************************************
**
** Description
** -----------
** Ajinextek Analog Library Header File
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

public class CAXA
{
//========== 보드 및 모듈 정보 확인 함수 =============================================================
    //AIO 모듈이 있는지 확인한다    
    [DllImport("AXL.dll")] public static extern uint AxaInfoIsAIOModule(ref uint upStatus);
    
    //모듈 No를 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxaInfoGetModuleNo(int lBoardNo, int lModulePos, ref int lpModuleNo);
    
    //AIO 모듈의 개수를 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxaInfoGetModuleCount(ref int lpModuleCount);
    
    //지정한 모듈의 입력 채널 수를 확인한다
    [DllImport("AXL.dll")] public static extern uint AxaInfoGetInputCount(int lModuleNo, ref int lpCount);
    
    //지정한 모듈의 출력 채널 수를 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxaInfoGetOutputCount(int lModuleNo, ref int lpCount);

    //지정한 모듈의 첫 번째 채널 번호를 확인한다.(입력 전용,출력 전용 모듈용)
    [DllImport("AXL.dll")] public static extern uint AxaInfoGetChannelNoOfModuleNo(int lModuleNo, ref int lpChannelNo);
    
    //지정한 모듈의 첫 번째 입력 채널 번호를 확인한다.(입력 모듈, 입력/출력 통합 모듈용)
    [DllImport("AXL.dll")] public static extern uint AxaInfoGetChannelNoAdcOfModuleNo(int lModuleNo, ref int lpChannelNo);

    //지정한 모듈의 첫 번째 출력 채널 번호를 확인한다.(출력 모듈, 입력/출력 통합 모듈용)
    [DllImport("AXL.dll")] public static extern uint AxaInfoGetChannelNoDacOfModuleNo(int lModuleNo, ref int lpChannelNo);
    
    //지정한 모듈 번호로 베이스 보드 번호, 모듈 위치, 모듈 ID를 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxaInfoGetModule(int lModuleNo, ref int lpBoardNo, ref int lpModulePos, ref uint upModuleID);
    
    // 해당 모듈이 제어가 가능한 상태인지 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxaInfoGetModuleStatus(int lModuleNo);
    
//========== 입력 모듈 정보 검색 함수 ================================================================
    //지정한 입력 채널 번호로 모듈 번호를 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxaiInfoGetModuleNoOfChannelNo(int lChannelNo, ref int lpModuleNo);
    
    //아날로그 입력 모듈의 전체 채널 개수를 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxaiInfoGetChannelCount(ref int lpChannelCount);

    //========== 입력 모듈 인터럽트/채널 이벤트 설정 및 확인 함수 ====================================
    //지정한 채널에 이벤트 메시지를 받아오기 위하여 윈도우 메시지, 콜백 함수 또는 이벤트 방식을 사용한다. H/W 타이머(Timer Trigger Mode, External Trigger Mode)를 이용, 연속적 데이터 수집 동작시(AxaStartMultiChannelAdc 참조)에 사용한다.
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
    [DllImport("AXL.dll")] public static extern uint AxaiEventSetChannel(int lModuleNo, IntPtr hWnd, uint uMessage, CAXHS.AXT_INTERRUPT_PROC pProc, ref uint pEvent);
    
    //지정한 입력 채널에 이벤트 사용 유무를 설정한다.
    //======================================================
    // uUse        : DISABLE(0)    // 이벤트 해제
    //             : ENABLE(1)     // 이벤트 설정
    //======================================================
    [DllImport("AXL.dll")] public static extern uint AxaiEventSetChannelEnable(int lChannelNo, uint uUse);
    
    //지정한 입력 채널의 이벤트 사용 유무를 확인한다.
    //======================================================
    // *upUse      : DISABLE(0)    // 이벤트 해제
    //             : ENABLE(1)     // 이벤트 설정
    //======================================================
    [DllImport("AXL.dll")] public static extern uint AxaiEventGetChannelEnable(int lChannelNo, ref uint upUse);
    
    //지정한 여러 입력 채널에 이벤트 사용 유무를 설정한다.
    //======================================================
    // lSize       : 사용 할 입력 채널의 갯수
    // lpChannelNo : 사용할 채널 번호의 배열
    // uUse        : DISABLE(0)    // 이벤트 해제
    //             : ENABLE(1)     // 이벤트 설정
    //======================================================
    [DllImport("AXL.dll")] public static extern uint AxaiEventSetMultiChannelEnable(int lSize, int[] lpChannelNo, uint uUse);
    
    //지정한 입력 채널에 이벤트 종류를 설정한다.
    //======================================================
    // uMask       : DATA_EMPTY(1) --> 버퍼에 데이터가 없을 때
    //             : DATA_MANY(2)  --> 버퍼에 데이터가 상한 설정 값보다 많아질 때
    //             : DATA_SMALL(3) --> 버퍼에 데이터가 하한 설정 값보다 적어질 때
    //             : DATA_FULL(4)  --> 버퍼에 데이터가 꽉 찼을 때
    //======================================================
    [DllImport("AXL.dll")] public static extern uint AxaiEventSetChannelMask(int lChannelNo, uint uMask);
    
    //지정한 입력 채널에 이벤트 종류를 확인한다.
    //======================================================
    // *upMask     : DATA_EMPTY(1) --> 버퍼에 데이터가 없을 때
    //             : DATA_MANY(2)  --> 버퍼에 데이터가 상한 설정 값보다 많아질 때
    //             : DATA_SMALL(3) --> 버퍼에 데이터가 하한 설정 값보다 적어질 때
    //             : DATA_FULL(4)  --> 버퍼에 데이터가 꽉 찼을 때
    //======================================================
    [DllImport("AXL.dll")] public static extern uint AxaiEventGetChannelMask(int lChannelNo, ref uint upMask);
    
    //지정한 여러 입력 채널에 이벤트 종류를 설정한다.
    //======================================================
    // lSize       : 사용 할 입력 채널의 갯수
    // lpChannelNo : 사용할 채널 번호의 배열
    // uMask       : DATA_EMPTY(1) --> 버퍼에 데이터가 없을 때
    //             : DATA_MANY(2)  --> 버퍼에 데이터가 상한 설정 값보다 많아질 때
    //             : DATA_SMALL(3) --> 버퍼에 데이터가 하한 설정 값보다 적어질 때
    //             : DATA_FULL(4)  --> 버퍼에 데이터가 꽉 찼을 때
    //======================================================
    [DllImport("AXL.dll")] public static extern uint AxaiEventSetMultiChannelMask(int lSize, int[] lpChannelNo, uint uMask);
    
    //이벤트 발생 위치를 확인한다.
    //======================================================
    // *upMode     : AIO_EVENT_DATA_UPPER(1) --> 버퍼에 데이터가 상한 설정 값보다 많아질 때
    //             : AIO_EVENT_DATA_LOWER(2) --> 버퍼에 데이터가 하한 설정 값보다 적어질 때
    //             : AIO_EVENT_DATA_FULL(3)  --> 버퍼에 데이터가 꽉 찼을 때
    //             : AIO_EVENT_DATA_EMPTY(4) --> 버퍼에 데이터가 없을 때
    //======================================================
    [DllImport("AXL.dll")] public static extern uint AxaiEventRead(ref int lpChannelNo, ref uint upMode);
    
    //지정한 모듈의 인터럽트 마스크를 설정한다. 이 함수는 연속적 신호감시를 할 경우에 하드웨어(모듈)의 FIFO 에서 사용자가 
    //지정한 크기의 버퍼로 내부 인터럽트를 통한 데이터 이동 시점을 지정하기 위해 사용된다. (SIO-AI4RB는 지원하지 않는다.)
    //==================================================================================================//
    // uMask       : SCAN_END(1)       --> 셋팅된 채널 모두  ADC 변환이 한번 이루어 질 때 마다 인터럽트가 발생
    //             : FIFO_HALF_FULL(2) --> 모듈내의 FIFO가 HALF이상 찼을 경우 내부 인터럽트 발생
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiInterruptSetModuleMask(int lModuleNo, uint uMask);
    
    //지정한 모듈의 인터럽트 마스크를 확인한다.
    //==================================================================================================//
    // *upMask     : SCAN_END(1)       --> 셋팅된 채널 모두  ADC 변환이 한번 이루어 질 때 마다 인터럽트가 발생
    //             : FIFO_HALF_FULL(2) --> 모듈내의 FIFO가 HALF이상 찼을 경우 내부 인터럽트 발생
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiInterruptGetModuleMask(int lModuleNo, ref uint upMask);
    
//========== 입력 모듈 파라미터 설정 및 확인 함수 ========================================================================
    //지정한 입력 채널에 입력 전압 범위를 설정한다.
    //==================================================================================================//
    // AI4RB
    // dMinVolt    : -10V/-5V로 설정 가능
    // dMaxVolt    : 10V/5V/로 설정 가능
    //
    // AI16Hx
    // dMinVolt    : -10V 고정
    // dMaxVolt    : 10V 고정
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiSetRange(int lChannelNo, double dMinVolt, double dMaxVolt);
    
    //지정한 입력 채널의 입력 전압 범위를 확인한다.
    //==================================================================================================//
    // AI4RB
    // *dpMinVolt  : -10V/-5V로 설정 가능
    // *dpMaxVolt  : 10V/5V/로 설정 가능
    //
    // AI16Hx
    // *dpMaxVolt  : -10V 고정
    // *dpMaxVolt  : 10V 고정
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiGetRange(int lChannelNo, ref double dpMinVolt, ref double dpMaxVolt);

    //지정한 여러 입력 모듈에 허용 입력 전압 범위를 설정한다.
    //==================================================================================================//
    // lModuleNo   : 사용할 입력 모듈 번호
    //
    // RTEX AI16F
    // Mode -5~+5  : dMinVolt = -5, dMaxVolt = +5
    // Mode -10~+10: dMinVolt = -10, dMaxVolt = +10
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiSetRangeModule(int lModuleNo, double dMinVolt, double dMaxVolt);

    //지정한 여러 입력 모듈에 허용 입력 전압 범위를 확인한다.
    //==================================================================================================//
    // lModuleNo   : 사용할 입력 모듈 번호
    //
    // RTEX AI16F
    // *dMinVolt   : -5V, -10V
    // *dMaxVolt   : +5V, +10V
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiGetRangeModule(int lModuleNo, ref double dMinVolt, ref double dMaxVolt);
    
    //지정한 여러 입력 채널에 허용 입력 전압 범위를 설정한다.
    //==================================================================================================//
    // lSize        : 사용할 입력 채널의 개수
    // *lpChannelNo : 사용할 채널 번호의 배열
    //
    // AI4RB
    // dMinVolt    : -10V/-5V로 설정 가능
    // dMaxVolt    : 10V/5V/로 설정 가능
    //
    // AI16Hx
    // dMinVolt    : -10V
    // dMaxVolt    : 10V
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiSetMultiRange(int lSize, int[] lpChannelNo, double dMinVolt, double dMaxVolt);
    
    //지정한 입력 모듈에 트리거 모드를 설정한다.
    //==================================================================================================//
    // uTriggerMode : NORMAL_MODE(1)   --> 사용자가 원하는 시점에 A/D변환하는 Software Trigger 방식 
    //              : TIMER_MODE(2)    --> H/W의 내부 클럭을 이용해서 A/D변환하는 Trigger 방식
    //              : EXTERNAL_MODE(3) --> 외부 입력단자의 클럭을 이용해서 A/D변환하는 Trigger 방식
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiSetTriggerMode(int lModuleNo, uint uTriggerMode);
    
    //지정한 모듈에 트리거 모드를 확인한다.
    //==================================================================================================//
    // *upTriggerMode : NORMAL_MODE(1)   --> 사용자가 원하는 시점에 A/D변환하는 Software Trigger 방식 
    //                : TIMER_MODE(2)    --> H/W의 내부 클럭을 이용해서 A/D변환하는 Trigger 방식
    //                : EXTERNAL_MODE(3) --> 외부 입력단자의 클럭을 이용해서 A/D변환하는 Trigger 방식
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiGetTriggerMode(int lModuleNo, ref uint upTriggerMode);
    
    //지정한 입력모듈의 Offset을 mVolt 단위(mV)로 설정한다. 최대 -100~100mVolt
    //==================================================================================================//
    // dMiliVolt    : -100 ~ 100 
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiSetModuleOffsetValue(int lModuleNo, double dMiliVolt);
    
    //지정한 입력모듈의 Offset 값을 확인한다. mVolt 단위(mV)
    //==================================================================================================//
    // *dpMiliVolt  : -100 ~ 100 
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiGetModuleOffsetValue(int lModuleNo, ref double dpMiliVolt); 

    //========== 입력 모듈 A/D 변환 함수
    //==Software Trigger Mode 함수
    //사용자가 지정한 입력 채널에 아날로그 입력 값을 A/D변환한 후 전압 값으로 반환한다.이 함수를 사용하기 전에 AxaSetTriggerModeAdc 함수를 사용하여 Normal Trigger Mode로 설정되어 있어야 한다.
    [DllImport("AXL.dll")] public static extern uint AxaiSwReadVoltage(int lChannelNo, ref double dpVolt);
    
    //지정한 입력 채널에 아날로그 입력 값을 Digit 값으로 반환한다. Normal Trigger Mode로 설정되어 있어야 한다.
    [DllImport("AXL.dll")] public static extern uint AxaiSwReadDigit(int lChannelNo, ref uint upDigit);
    
    //지정한 여러 입력 채널에 아날로그 입력 값을 전압 값으로 반환한다. Normal Trigger Mode로 설정되어 있어야 한다.
    [DllImport("AXL.dll")] public static extern uint AxaiSwReadMultiVoltage(int lSize, int[] lpChannelNo, double[] dpVolt);
    
    //지정한 여러 입력 채널에 아날로그 입력 값을 Digit 값으로 반환한다. Normal Trigger Mode로 설정되어 있어야 한다.
    [DllImport("AXL.dll")] public static extern uint AxaiSwReadMultiDigit(int lSize, int[] lpChannelNo, uint[] upDigit);
    
    //지정한 여러 입력 채널에 Immediate모드를 사용하기 위해 설정 값을 설정한다. 이 함수를 사용하기 전에 AxaSetTriggerModeAdc 함수를 사용하여 Timer Trigger Mode로 설정되어 있어야 한다.
    [DllImport("AXL.dll")] public static extern uint AxaiHwSetMultiAccess(int lSize, int[] lpChannelNo, int[] lpWordSize);
    
    //지정한 개수만큼 A/D변환 후 전압 값을 반환한다. 이 함수를 사용하기 전에 AxaiHwSetMultiAccess함수를 이용 설정값을 지정해야 하며 , AxaSetTriggerModeAdc 함수를 사용하여 Timer Trigger Mode로 설정되어 있어야 한다.
    // [DllImport("AXL.dll")] public static extern uint AxaiHwStartMultiAccess(ref double[] dpBuffer);
    [DllImport("AXL.dll")] public static extern uint AxaiHwStartMultiAccess(double[,] dpBuffer);
    
    //지정한 모듈에 샘플링 간격을 주파수 단위로 설정한다.
    //==================================================================================================//
    // dSampleFreq    : 10 ~ 100000 
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiHwSetSampleFreq(int lModuleNo, double dSampleFreq);
    
    //지정한 모듈에 샘플링 간격을 주파수 단위로 설정된 값을 확인한다.
    //==================================================================================================//
    // *dpSampleFreq  : 10 ~ 100000 
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiHwGetSampleFreq(int lModuleNo, ref double dpSampleFreq);
    
    //지정한 모듈에 샘플링 간격을 시간 단위(uSec)로 설정한다.
    //==================================================================================================//
    // dSamplePeriod  : 100000 ~ 1000000000
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiHwSetSamplePeriod(int lModuleNo, double dSamplePeriod);
    
    //지정한 모듈에 샘플링 간격을 시간 단위(uSec)로 설정된 값을 확인한다.
    //==================================================================================================//
    // *dpSamplePeriod: 100000 ~ 1000000000
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiHwGetSamplePeriod(int lModuleNo, ref double dpSamplePeriod);
    
    //지정한 입력 채널에 버퍼가 Full로 찼을 때 관리 방식을 설정한다.
    //==================================================================================================//
    // uFullMode      : NEW_DATA_KEEP(0) --> 새로운 데이터 유지
    //                : CURR_DATA_KEEP(1) --> 이전 데이터 유지
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiHwSetBufferOverflowMode(int lChannelNo, uint uFullMode);
    
    //지정한 입력 채널이 버퍼가 Full로 찼을 때 관리 방식을 확인한다.
    //==================================================================================================//
    // *upFullMode    : NEW_DATA_KEEP(0) --> 새로운 데이터 유지
    //                : CURR_DATA_KEEP(1) --> 이전 데이터 유지
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiHwGetBufferOverflowMode(int lChannelNo, ref uint upFullMode);
    
    //지정한 여러 입력 채널에 버퍼가 Full로 찼을 때 관리 방식을 설정한다.
    //==================================================================================================//
    // uFullMode      : NEW_DATA_KEEP(0) --> 새로운 데이터 유지
    //                : CURR_DATA_KEEP(1) --> 이전 데이터 유지
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiHwSetMultiBufferOverflowMode(int lSize, int[] lpChannelNo, uint uFullMode);
    
    //지정한 입력 채널에 버퍼의 상한 값과 하한 값을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxaiHwSetLimit(int lChannelNo, int lLowLimit, int lUpLimit);
    
    //지정한 입력 채널에 버퍼의 상한 값과 하한 값을 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxaiHwGetLimit(int lChannelNo, ref int lpLowLimit, ref int lpUpLimit);
    
    //지정한 여러 입력 채널에 버퍼의 상한 값과 하한 값을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxaiHwSetMultiLimit(int lSize, int[] lpChannelNo, int lLowLimit, int lUpLimit);
    
    //지정한 여러 입력 채널에 H/W타이머를 이용한 A/D변환을 시작한다.
    [DllImport("AXL.dll")] public static extern uint AxaiHwStartMultiChannel(int lSize, int[] lpChannelNo, int lBuffSize);
    
    //지정한 여러 입력 채널에 A/D변환을 시작 후 지정한 개수만큼 필터 처리해서 전압으로 반환한다.
    //==================================================================================================//
    // lSize          : 사용할 입력 채널의 개수
    // *lpChannelNo   : 사용할 채널 번호의 배열
    // lFilterCount   : Filtering할 데이터의 개수
    // lBuffSize      : 각 채널에 할당되는 버퍼의 개수
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiHwStartMultiFilter(int lSize, int[] lpChannelNo, int lFilterCount, int lBuffSize);
    
    //H/W타이머를 이용한 연속 신호 A/D변환을 중지한다.
    [DllImport("AXL.dll")] public static extern uint AxaiHwStopMultiChannel(int lModuleNo);
    
    //지정한 입력 채널의 메모리 버퍼에 데이터가 몇 개인지 검사한다.
    [DllImport("AXL.dll")] public static extern uint AxaiHwReadDataLength(int lChannelNo, ref int lpDataLength);
    
    //지정한 입력 채널에 H/W타이머를 이용하여 A/D변환된 값을 전압 값으로 읽는다.
    [DllImport("AXL.dll")] public static extern uint AxaiHwReadSampleVoltage(int lChannelNo, ref int lpSize, ref double dpVolt);
    
    //지정한 입력 채널에 H/W타이머를 이용하여 A/D변환된 값을 Digit 값으로 읽는다.
    [DllImport("AXL.dll")] public static extern uint AxaiHwReadSampleDigit(int lChannelNo, ref int lpSize, ref uint upDigit);

//========== 입력 모듈 버퍼 상태 체크 함수 ===============================================================================
    //지정한 입력 채널의 메모리 버퍼에 데이터가 없는 지 검사한다.
    //==================================================================================================//
    // *upEmpty       : FALSE(0) --> 메모리 버퍼에 데이터가 있을 경우
    //                : TRUE(1)  --> 메모리 버퍼에 데이터가 없을 경우
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiHwIsBufferEmpty(int lChannelNo, ref uint upEmpty);
    
    //지정한 입력 채널의 메모리 버퍼에 설정되어 있는 상한 값보다 데이터가 많은 지 검사한다
    //==================================================================================================//
    // *upUpper       : FALSE(0) --> 메모리 버퍼에 데이터가 상한 값보다 적을 경우
    //                : TRUE(1)  --> 메모리 버퍼에 데이터가 상한 값보다 많을 경우
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiHwIsBufferUpper(int lChannelNo, ref uint upUpper);
    
    //지정한 입력 채널의 메모리 버퍼에 설정되어 있는 하한 값보다 데이터가 적은 지 검사한다.
    //==================================================================================================//
    // *upLower       : FALSE(0) --> 메모리 버퍼에 데이터가 하한 값보다 많을 경우
    //                : TRUE(1)  --> 메모리 버퍼에 데이터가 하한 값보다 적을 경우
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaiHwIsBufferLower(int lChannelNo, ref uint upLower);

//==External Trigger Mode 함수
    //지정한 입력모듈의 선택된 채널들의 외부 트리거 모드를 시작한다.
    //==================================================================================================//
    // lSize          : 지정한 입력 모듈에서 외부트리거를 사용 할 채널갯수
    // *lpChannelPos  : 지정한 입력 모듈에서 외부트리거를 사용 할 채널의 Index
    [DllImport("AXL.dll")] public static extern uint AxaiExternalStartADC(int lModuleNo, int lSize, ref int lpChannelPos);

    //지정한 입력모듈의 외부트리거 모드를 정지한다.  
    [DllImport("AXL.dll")] public static extern uint AxaiExternalStopADC(int lModuleNo);

    //지정한 입력모듈의 Fifo상태를 반환한다.
    //==================================================================================================//
    // *dwpStatus     : FIFO_DATA_EXIST(0)
    //                : FIFO_DATA_EMPTY(1)
    //                : FIFO_DATA_HALF(2)
    //                : FIFO_DATA_FULL(6)
    //==================================================================================================//    
    [DllImport("AXL.dll")] public static extern uint AxaiExternalReadFifoStatus(int lModuleNo, ref uint upStatus);

    //지정한 입력모듈의 외부신호에 의해 변환된 A/D값을 읽어옴.
    // lSize          : 지정한 입력 모듈에서 변환된 A/D값을 읽어올 채널의 갯수(AxaiExternalStartADC에 사용한 채널갯수와 동일 해야됨)
    // *lpChannelPos  : 지정한 입력 모듈에서 변환된 A/D값을 읽어올 채널의 Index(AxaiExternalStartADC에 사용한 채널의 Index와 동일 해야됨)
    // lDataSize      : 외부트리거에 의해 A/D변환된 값을 한번에 읽어 올 최대 데이타의 갯수
    // lBuffSize      : 외부에서(사용자 Program) 할당한 Data Buffer의 Size
    // lStartDataPos  : 외부에서(사용자 Program) 할당한 Data Buffer에 저장 시작 할 위치 
    // *dpVolt[]      : A/D변환된 값을 할당 받을 2차원 배열 포인트(dpVlot[Channel][Count])
    // *lpRetDataSize : A/D변환된 값이 Data Buffer에 실제 할당된 갯수
    // *dwpStatus     : A/D변환된 값을 Fifo(H/W Buffer)로 부터 읽을 때 Fifo상태를 반환함.
    [DllImport("AXL.dll")] public static extern uint AxaiExternalReadVoltage(int lModuleNo, int lSize, ref int lpChannelPos, int lDataSize, int lBuffSize, int lStartDataPos, double[,] dpVolt, ref int lpRetDataSize, ref uint upStatus);
	
//========== 출력 모듈 정보 검색 함수 ====================================================================================
    //지정한 출력 채널 번호로 모듈 번호를 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxaoInfoGetModuleNoOfChannelNo(int lChannelNo, ref int lpModuleNo);
    
    //아날로그 출력 모듈의 전체 채널 개수를 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxaoInfoGetChannelCount(ref int lpChannelCount);

//========== 출력 모듈 설정 및 확인 함수 =================================================================================
    //지정한 출력 채널에 출력 전압 범위를 설정한다
    //==================================================================================================//
    // AXT_SIO_RAO4RB
    // dMinVolt    : -10V
    // dMaxVolt    : 10V
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaoSetRange(int lChannelNo, double dMinVolt, double dMaxVolt);
    
    //지정한 출력 채널의 출력 전압 범위를 확인한다.
    //==================================================================================================//
    // AXT_SIO_RAO4RB
    // *dpMinVolt    : -10V
    // *dpMaxVolt    : 10V
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaoGetRange(int lChannelNo, ref double dpMinVolt, ref double dpMaxVolt);
    
    //지정한 여러 출력 채널에 출력 전압 범위를 설정한다.
    //==================================================================================================//
    // AO4R, AO2Hx
    // dMinVolt    : -10V
    // dMaxVolt    : 10V
    //==================================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaoSetMultiRange(int lSize, int[] lpChannelNo, double dMinVolt, double dMaxVolt);
    
    //지정한 출력 채널에 입력된 전압이 출력 된다.
    [DllImport("AXL.dll")] public static extern uint AxaoWriteVoltage(int lChannelNo, double dVolt);
    
    //지정한 출력 채널에 입력된 전압이 출력 된다.
    [DllImport("AXL.dll")] public static extern uint AxaoWriteDigit(int lChannelNo, uint uDigit);


    //지정한 여러 출력 채널에 입력된 전압이 출력 된다.
    [DllImport("AXL.dll")] public static extern uint AxaoWriteMultiVoltage(int lSize, int[] lpChannelNo, double[] dpVolt);
    
    //지정한 여러 출력 채널에 입력된 전압이 출력 된다.
    [DllImport("AXL.dll")] public static extern uint AxaoWriteMultiDigit(int lSize, int[] lpChannelNo, uint[] upDigit);

    //지정한 출력 채널에 출력되는 전압 값을 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxaoReadVoltage(int lChannelNo, ref double dpVolt);

    //지정한 출력 채널에 출력되는 전압 값을 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxaoReadDigit(int lChannelNo, ref uint upDigit);
    
    //지정한 여러 출력 채널에 출력되는 전압 값을 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxaoReadMultiVoltage(int lSize, int[] lpChannelNo, double[] dpVolt);

    //지정한 여러 출력 채널에 출력되는 전압 값을 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxaoReadMultiDigit(int lSize, int[] lpChannelNo, uint[] upDigit);


    //============================ AXA User Define Pattern Generator ============================
    // Pattern Generator 관련 함수들은 PCI-R32IO_RTEX_LCM 에서만 지원
    // Channel User Define Pattern Generator를 설정 하는 함수 이다.
    // AxaoPgSetUserInterval에 설정딘 시간 마다 Pattern을 순차적으로 출력한다.
    // lLoopCnt       : '0'(입력된 패턴을 무한대로 반복), 'value' : 지정된 횟수 만큼 입력 패턴 출력 후 마지막 패턴 유지
    //                : (MAX : 60000)
    // lPatternSize   : 입력 패턴 갯수(MAX : 8192) , PCI-R
    [DllImport("AXL.dll")] public static extern uint AxaoPgSetUserPatternGenerator(int lChannelNo, int lLoopCnt, int lPatternSize, double[] dpPattern);
    //AxaoPgSetUserPatternGenerator와 동일 기능, Digit 값으로 표현
    [DllImport("AXL.dll")] public static extern uint AxaoPgSetUserPatternGeneratorDigit(int lChannelNo, int lLoopCnt, int lPatternSize, uint[] pDigitPattern);

    // User Define Pattern Generator를 확인 하는 함수 이다.
    [DllImport("AXL.dll")] public static extern uint AxaoPgGetUserPatternGenerator(int lChannelNo, ref int lpLoopCnt, ref int lpPatternSize, double[] dpPattern);
    // User Define Pattern Generator를 확인 하는 함수 이다. Digit값으로 확인
    [DllImport("AXL.dll")] public static extern uint AxaoPgGetUserPatternGeneratorDigit(int lChannelNo, ref int lpLoopCnt, ref int lpPatternSize, uint[] pDigitPattern);

    // 해당 Channel의 Pattern Generator Interval을 설정 하는 함수 이다.
    // 단위는 us(기본 Resolution : 500uSec)
    [DllImport("AXL.dll")] public static extern uint AxaoPgSetUserInterval(int lChannelNo, double dInterval);

    // 해당 Channel의 Pattern Generator Interval을 확인 하는 함수 이다.
    [DllImport("AXL.dll")] public static extern uint AxaoPgGetUserInterval(int lChannelNo, ref double dpInterval);

    // 해당 Channel의 Pattern Index / Loop Cnt를 확인 하는 함수 이다.
    // Status의 경우 아래의 Status를 확인 할 수 있다.
    // lpIndexNum : 현재 User Pattern의 Index
    // lpLoopCnt : 현재 구동중인 Loop 횟수
    // dwpInBusy : Pattern Generator의 구동 유무
    [DllImport("AXL.dll")] public static extern uint AxaoPgGetStatus(int lChannelNo, ref int lpIndexNum, ref int lpLoopCnt, ref uint dwpInBusy);

    // 해당 Channel의 User Define Pattern Generator를 시작 하는 함수이다. (AO 출력 시작)
    // Start Channel 번호를 배열로 입력
    // 입력된 채널에 대하여 동시에 패턴 생성 기능을 시작한다.
    [DllImport("AXL.dll")] public static extern uint AxaoPgSetUserStart(int[] lpChannelNo, int lSize);

    // 해당 Channel의 User Define Pattern Generator를 정지 하는 함수이다. (AO 출력 정지)
    // 출력 정지시 출력값은 0Volt로 전환된다.
    [DllImport("AXL.dll")] public static extern uint AxaoPgSetUserStop(int[] lpChannelNo, int lSize);

    // Pattern Data를 Clear 하는 함수. (0x00으로 모든 영역을 Reset)
    [DllImport("AXL.dll")] public static extern uint AxaPgSetUserDataReset(int lChannelNo);

    // 지정한 출력 모듈의 Network이 끊어 졌을 경우 출력 상태를 채널 별로 설정하는 함수이다.
    //===============================================================================================//
    // lChannelNo  : 채널 번호(분산형 슬레이브 제품만 지원 함)
    // dwSetValue  : 설정 할 변수 값(Default는 Network 끊어 지기 전 상태 유지)
    //             : 0 --> Network 끊어 지기 전 상태 유지
    //             : 1 --> Analog Max
    //             : 2 --> Analog MIN
    //             : 3 --> User Vaule(Default user value는 0V로 설정됨, AxaoSetNetworkErrorUserValue() 함수로 변경가능)
    //             : 4 --> Analog 0 V
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaoSetNetworkErrorAct(int lChannelNo, uint dwSetValue);

    // 지정한 출력 모듈의 Network이 끊어 졌을 경우 출력 상태를 출력 Byte 단위로 설정한다.
    //===============================================================================================//
    // lChannelNo  : 채널 번호(분산형 슬레이브 제품만 지원 함)
    // dVolt       : 사용자 정의 아날로그 출력 전압 값
    //===============================================================================================//
    [DllImport("AXL.dll")] public static extern uint AxaoSetNetworkErrorUserValue(int lChannelNo, double dVolt);

}

