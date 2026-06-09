using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
        public List<LinesProperties> Lines { get; } = new List<LinesProperties>();
        public List<BlobProperty> Blobs { get; } = new List<BlobProperty>();
        public List<ContourProperty> Contours { get; } = new List<ContourProperty>();
        public List<LineGaugeProperty> Lines_L { get; } = new List<LineGaugeProperty>();
        public List<LineGaugeProperty> Lines_R { get; } = new List<LineGaugeProperty>();
        public List<LineGaugeProperty> Lines_TOP { get; } = new List<LineGaugeProperty>();
        public List<MatchingProperty> Matchings { get; } = new List<MatchingProperty>();
        public VisionProperty PropertyVision { get; internal set; } = new VisionProperty("VisionPara");
        public List<FeatureMatchingProperty> Features { get; } = new List<FeatureMatchingProperty>();

        public bool LoadTools(string Name)
        {
            try
            {
                VisionToolStorage.Load(this, Name);
            }
            catch (Exception Desc)
            {
                MessageBox.Show(Desc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public bool SaveTools(string Name)
        {
            try
            {
                VisionToolStorage.Save(this, Name);
            }
            catch (Exception Desc)
            {
                MessageBox.Show(Desc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}
