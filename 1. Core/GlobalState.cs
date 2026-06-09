using Lib.Common;
using System;
using System.Reflection;

namespace OpenVisionLab
{
    public static class AppVersion
    {
        public static string VERSION { get; set; } = "2.1.0";
        public static string DATETIME_UPDATED { get; set; } = "2026/06/08 /*18:00*/";
        public static string MANAGER { get; set; } = "NOAH";
    }

    // 애플리케이션 전역 상태 컨테이너입니다.
    // Recipe, System, Data, Vision Tool 저장소의 런타임 연결을 관리합니다.
    public class GlobalState
    {
        // 현재 레시피 상태입니다. 레시피가 변경되면 장치, 화면, 파라미터 관련 데이터가 함께 로드됩니다.
        public RecipeState Recipe { get; set; } = new RecipeState();

        // 메뉴, 권한, 창 상태 등 프로그램 실행 상태를 관리합니다.
        public SystemState System { get; set; } = new SystemState();

        // 검사에서 사용하는 이미지, 파라미터, 결과 데이터를 관리합니다.
        public DataState Data { get; set; } = new DataState();

        public VisionToolRepository VisionTools { get; } = new VisionToolRepository();

        public GlobalState()
        {
            Recipe.SetRuntime(() => Data, data => Data = data, () => VisionTools);
        }

        public bool Close()
        {
            System.SaveConfig();
            return true;
        }
    }
}
