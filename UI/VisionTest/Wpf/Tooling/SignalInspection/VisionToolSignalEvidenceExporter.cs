using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenVisionLab
{
    internal static class VisionToolSignalEvidenceExporter
    {
        public static void ExportTsv(VisionToolSignalEvidence evidence, string path)
        {
            if (evidence == null)
            {
                throw new ArgumentNullException(nameof(evidence));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("An export path is required.", nameof(path));
            }

            string fullPath = Path.GetFullPath(path);
            string directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using StreamWriter writer = new StreamWriter(fullPath, false, new UTF8Encoding(false));
            WriteMetadata(writer, "EvidenceId", evidence.EvidenceId);
            WriteMetadata(writer, "SourceSha256", evidence.SourceSha256);
            WriteMetadata(writer, "ResultSha256", evidence.ResultSha256);
            WriteMetadata(writer, "Tool", evidence.ToolIdentity);
            WriteMetadata(writer, "InputLayer", evidence.InputLayer);
            WriteMetadata(writer, "Region", evidence.RegionDescription);
            WriteMetadata(writer, "Parameters", evidence.ParameterSummary);
            WriteMetadata(writer, "XAxis", evidence.XAxisLabel);
            WriteMetadata(writer, "YAxis", evidence.YAxisLabel);
            WriteMetadata(writer, "Guidance", evidence.Guidance);
            foreach (System.Collections.Generic.KeyValuePair<string, string> attribute in evidence.Attributes)
            {
                WriteMetadata(writer, "Attribute." + attribute.Key, attribute.Value);
            }

            foreach (VisionToolSignalMarker marker in evidence.Markers)
            {
                WriteMetadata(
                    writer,
                    "Marker." + marker.Id,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}|{1:0.###############}|Editable={2}",
                        marker.Name,
                        marker.X,
                        marker.IsEditable));
            }

            writer.Write(evidence.XAxisLabel);
            foreach (VisionToolSignalSeries series in evidence.Series)
            {
                writer.Write('\t');
                writer.Write(Escape(series.Name));
            }

            writer.WriteLine();
            int rowCount = evidence.Series.Max(series => series.Values.Count);
            for (int row = 0; row < rowCount; row++)
            {
                VisionToolSignalSeries xSeries = evidence.Series.First(series => row < series.Values.Count);
                writer.Write((xSeries.XStart + (row * xSeries.XStep)).ToString("0.###############", CultureInfo.InvariantCulture));
                foreach (VisionToolSignalSeries series in evidence.Series)
                {
                    writer.Write('\t');
                    if (row < series.Values.Count)
                    {
                        writer.Write(series.Values[row].ToString("0.###############", CultureInfo.InvariantCulture));
                    }
                }

                writer.WriteLine();
            }
        }

        private static void WriteMetadata(TextWriter writer, string key, string value)
        {
            writer.Write("#\t");
            writer.Write(key);
            writer.Write('\t');
            writer.WriteLine(Escape(value));
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty)
                .Replace("\t", " ", StringComparison.Ordinal)
                .Replace("\r", " ", StringComparison.Ordinal)
                .Replace("\n", " ", StringComparison.Ordinal);
        }
    }
}
