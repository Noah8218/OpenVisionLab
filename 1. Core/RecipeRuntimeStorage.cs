using System;

namespace OpenVisionLab
{
    internal static class RecipeRuntimeStorage
    {
        public static void Load(
            string recipeName,
            Func<DataState> dataAccessor,
            Action<DataState> dataSetter,
            Func<VisionToolRepository> visionToolAccessor)
        {
            VisionToolRepository visionTools = GetVisionTools(visionToolAccessor);
            visionTools.LoadTools(recipeName);

            DataState data = GetData(dataAccessor).LoadConfig(recipeName);
            dataSetter(data);
        }

        public static void Save(
            string recipeName,
            Func<DataState> dataAccessor,
            Func<VisionToolRepository> visionToolAccessor)
        {
            GetVisionTools(visionToolAccessor).SaveTools(recipeName);
            GetData(dataAccessor).SaveConfig(recipeName);
        }

        private static DataState GetData(Func<DataState> dataAccessor)
        {
            if (dataAccessor == null)
            {
                throw new InvalidOperationException("Recipe data runtime is not configured.");
            }

            return dataAccessor();
        }

        private static VisionToolRepository GetVisionTools(Func<VisionToolRepository> visionToolAccessor)
        {
            if (visionToolAccessor == null)
            {
                throw new InvalidOperationException("Vision tool runtime is not configured.");
            }

            return visionToolAccessor();
        }
    }
}
