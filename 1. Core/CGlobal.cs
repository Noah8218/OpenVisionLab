using OpenVisionLab._1._Core;
using Lib.Common;
using System;
using System.Reflection;

namespace OpenVisionLab
{
    public static class CVersion
    {
        #region VERSION HISTORY
        //VER 1.0.0 ==> 2022/08/04 초기 싱글톤 패턴을 적용 --> 현장 검증은 어느정도 완료        
        #endregion
        public static string APP_NAME { get; set; } = "KTEM_VISION";
        public static string VERSION { get; set; } = "1.0.5";
        public static string DATETIME_UPDATED { get; set; } = "2023/05/09 /*20:00*/";
        public static string MANAGER { get; set; } = "NOAH";
    }

    // Global 클래스
    // 해당 클래스안에 장치,레시피,검사 등 전역적으로 접근 가능하도록 UI 구성
    // CGlobal.Inst.... 
    public class CGlobal
    {               
        // 싱글톤(객체 접근시에만 객체를 생성)->지연 생성
        private static readonly Lazy<CGlobal> instance = new Lazy<CGlobal>(() => new CGlobal());

        public static CGlobal Inst
        {
            get { return instance.Value; }
        }
        
        // 레시피 관리 클래스(실행폴더//Recipe)
        // 레시피가 변경되면, 장치,스팩,파라미터 등 관련 값들을 Load
        public CRecipe Recipe { get; set; } = new CRecipe();
        // 모드, 권한, 창 변경 등 System 관련 클래스
        public CSystem System { get; set; } = new CSystem();
        // 장치(카메라,io,조명,모션) 등 관리 클래스
        public CDevice Device { get; set; } = new CDevice();
        // 상시로 스레드가 돌면서 큐에 이미지가 들어오면 검사
        // 유동적으로 알아서 변경 사용
        //public CSeqVision SeqVision { get; set; } = new CSeqVision();        
        // 스팩,파라미터, 판정값등 각종 검사에 사용되는 값들을 관리
        public CData Data { get; set; } = new CData();
        
        public CGlobal() { }        

        public bool Close()
        {
            try
            {
                System.Close();
                Device.Close();                                
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }
    }
}
