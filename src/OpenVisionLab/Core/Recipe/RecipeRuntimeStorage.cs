using System;

namespace OpenVisionLab
{
    internal static class RecipeRuntimeStorage
    {
        public static bool Load(
            string recipeName,
            Func<DataState> dataAccessor,
            Action<DataState> dataSetter,
            Func<VisionToolRepository> visionToolAccessor)
        {
            VisionToolRepository visionTools = GetVisionTools(visionToolAccessor);
            if (!visionTools.LoadTools(recipeName))
            {
                return false;
            }

            DataState data = GetData(dataAccessor).LoadConfig(recipeName);
            dataSetter(data);
            return true;
        }

        public static bool Save(
            string recipeName,
            Func<DataState> dataAccessor,
            Func<VisionToolRepository> visionToolAccessor)
        {
            if (!GetVisionTools(visionToolAccessor).SaveTools(recipeName))
            {
                return false;
            }

            GetData(dataAccessor).SaveConfig(recipeName);
            return true;
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
