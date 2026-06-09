using OpenVisionLab.Vision._1._Tools.OpenCV;

namespace OpenVisionLab
{
    internal static class VisionToolStorage
    {
        public const int DefaultToolSetCount = 1;

        public static void Load(VisionToolRepository repository, string recipeName)
        {
            Reset(repository);

            for (int i = 0; i < DefaultToolSetCount; i++)
            {
                repository.Blobs.Add(new BlobProperty($"Blob_{i + 1}").LoadConfig(recipeName));
                repository.Contours.Add(new ContourProperty($"Contour_{i + 1}").LoadConfig(recipeName));
                repository.Lines_L.Add(new LineGaugeProperty($"Line(L)_{i + 1}").LoadConfig(recipeName));
                repository.Lines_R.Add(new LineGaugeProperty($"Line(R)_{i + 1}").LoadConfig(recipeName));
                repository.Lines_TOP.Add(new LineGaugeProperty($"Line(TOP)_{i + 1}").LoadConfig(recipeName));
                repository.Matchings.Add(new MatchingProperty($"Matching_{i + 1}").LoadConfig(recipeName));
                repository.Features.Add(new FeatureMatchingProperty($"Feature_{i + 1}").LoadConfig(recipeName));
            }

            repository.PropertyVision = repository.PropertyVision.LoadConfig(recipeName);
        }

        public static void Save(VisionToolRepository repository, string recipeName)
        {
            foreach (BlobProperty property in repository.Blobs)
            {
                property.SaveConfig(recipeName);
            }

            foreach (LineGaugeProperty property in repository.Lines_L)
            {
                property.SaveConfig(recipeName);
            }

            foreach (LineGaugeProperty property in repository.Lines_R)
            {
                property.SaveConfig(recipeName);
            }

            foreach (LineGaugeProperty property in repository.Lines_TOP)
            {
                property.SaveConfig(recipeName);
            }

            foreach (ContourProperty property in repository.Contours)
            {
                property.SaveConfig(recipeName);
            }

            foreach (FeatureMatchingProperty property in repository.Features)
            {
                property.SaveConfig(recipeName);
            }

            foreach (MatchingProperty property in repository.Matchings)
            {
                property.SaveConfig(recipeName);
            }

            repository.PropertyVision.SaveConfig(recipeName);
        }

        private static void Reset(VisionToolRepository repository)
        {
            repository.Blobs.Clear();
            repository.Contours.Clear();
            repository.Lines_L.Clear();
            repository.Lines_R.Clear();
            repository.Lines_TOP.Clear();
            repository.Matchings.Clear();
            repository.Features.Clear();
        }
    }
}
