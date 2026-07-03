using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Media;

namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockingGuideZoneContent
    {
        public PackIconMaterialKind IconKind { get; set; }

        public double IconSize { get; set; } = 24D;

        public Brush IconBrush { get; set; } = Brushes.LightBlue;

        public string Title { get; set; } = string.Empty;

        public Brush TitleBrush { get; set; } = Brushes.White;

        public FontWeight TitleFontWeight { get; set; } = FontWeights.SemiBold;

        public string Detail { get; set; } = string.Empty;

        public Brush DetailBrush { get; set; } = Brushes.LightGray;

        public double DetailFontSize { get; set; } = 10D;
    }
}
