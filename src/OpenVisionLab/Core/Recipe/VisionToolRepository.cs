using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    public class LinesProperties
    {
        public string Name = "";
        public LineGaugeProperty Lines_L = new LineGaugeProperty();
        public LineGaugeProperty Lines_R = new LineGaugeProperty();
    }

    public sealed class VisionToolRepository
    {
        public Exception LastStorageError { get; private set; }

        public List<LinesProperties> Lines { get; } = new List<LinesProperties>();
        public List<BlobProperty> Blobs { get; } = new List<BlobProperty>();
        public List<ContourProperty> Contours { get; } = new List<ContourProperty>();
        public List<LineGaugeProperty> Lines_L { get; } = new List<LineGaugeProperty>();
        public List<LineGaugeProperty> Lines_R { get; } = new List<LineGaugeProperty>();
        public List<LineGaugeProperty> Lines_TOP { get; } = new List<LineGaugeProperty>();
        public List<MatchingProperty> Matchings { get; } = new List<MatchingProperty>();
        public List<EdgeBasedMatchingProperty> EdgeBasedMatchings { get; } = new List<EdgeBasedMatchingProperty>();
        public VisionProperty PropertyVision { get; internal set; } = new VisionProperty("VisionPara");
        public List<FeatureMatchingProperty> Features { get; } = new List<FeatureMatchingProperty>();

        public bool LoadTools(string Name)
        {
            return TryLoadTools(Name, out _);
        }

        public bool TryLoadTools(string Name, out Exception error)
        {
            error = null;
            try
            {
                VisionToolStorage.Load(this, Name);
                LastStorageError = null;
                return true;
            }
            catch (Exception Desc)
            {
                error = Desc;
                LastStorageError = Desc;
                return false;
            }
        }

        public bool SaveTools(string Name)
        {
            return TrySaveTools(Name, out _);
        }

        public bool TrySaveTools(string Name, out Exception error)
        {
            error = null;
            try
            {
                VisionToolStorage.Save(this, Name);
                LastStorageError = null;
                return true;
            }
            catch (Exception Desc)
            {
                error = Desc;
                LastStorageError = Desc;
                return false;
            }
        }
    }
}
