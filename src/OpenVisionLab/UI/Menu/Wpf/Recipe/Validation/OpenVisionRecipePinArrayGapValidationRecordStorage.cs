using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace OpenVisionLab
{
    [XmlRoot("PinRowEdgeGapValidationRecord")]
    public sealed class OpenVisionRecipePinArrayGapValidationRecord
    {
        [XmlAttribute]
        public string SkillVersion { get; set; } = OpenVisionRecipePinArrayGapValidationRecordStorage.SkillVersion;

        [XmlAttribute]
        public string RecipeName { get; set; } = string.Empty;

        [XmlAttribute]
        public string PipelineName { get; set; } = string.Empty;

        [XmlAttribute]
        public DateTime FrozenUtc { get; set; }

        public string PipelineXmlSha256 { get; set; } = string.Empty;

        public double DistancePxRangeMaximum { get; set; }

        [XmlElement("Row")]
        public List<OpenVisionRecipePinArrayGapRowIdentity> Rows { get; set; }
            = new List<OpenVisionRecipePinArrayGapRowIdentity>();

        public OpenVisionRecipePinArrayGapValidationSplitIdentity Train { get; set; }
            = new OpenVisionRecipePinArrayGapValidationSplitIdentity();

        public OpenVisionRecipePinArrayGapValidationSplitIdentity Validation { get; set; }
            = new OpenVisionRecipePinArrayGapValidationSplitIdentity();

        public OpenVisionRecipePinArrayGapValidationSplitIdentity Test { get; set; }
            = new OpenVisionRecipePinArrayGapValidationSplitIdentity();
    }

    public sealed class OpenVisionRecipePinArrayGapValidationSplitIdentity
    {
        [XmlAttribute]
        public string Role { get; set; } = string.Empty;

        [XmlAttribute]
        public string SetName { get; set; } = string.Empty;

        [XmlAttribute]
        public int ImageCount { get; set; }

        public string ContentSha256 { get; set; } = string.Empty;
    }

    public sealed class OpenVisionRecipePinArrayGapRowIdentity
    {
        [XmlAttribute]
        public int Index { get; set; }

        [XmlAttribute]
        public string Roi { get; set; } = string.Empty;

        [XmlAttribute]
        public int DarkThreshold { get; set; }

        [XmlAttribute]
        public double MinDarkCoverageRatio { get; set; }

        [XmlAttribute]
        public int MinPinWidth { get; set; }

        [XmlAttribute]
        public int MaxPinBreakWidth { get; set; }

        [XmlAttribute]
        public int MinGapWidth { get; set; }
    }

    internal static class OpenVisionRecipePinArrayGapValidationRecordStorage
    {
        internal const string SkillVersion = "v1";
        private const string FileName = "pin-row-edge-gap-v1.xml";
        private const double DoubleTolerance = 0.000000001D;

        public static bool TrySave(
            string recipeName,
            string pipelineXmlText,
            OpenVisionRecipeValidationSetOption train,
            OpenVisionRecipeValidationSetOption validation,
            OpenVisionRecipeValidationSetOption test,
            out OpenVisionRecipePinArrayGapValidationRecord record,
            out string error)
        {
            if (!TryCreateRecord(recipeName, pipelineXmlText, train, validation, test, out record, out error))
            {
                return false;
            }

            try
            {
                if (!SerializeHelper.SaveXmlFile(GetPath(recipeName), record))
                {
                    error = "PinArrayGap validation record could not be saved.";
                    return false;
                }

                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.GetBaseException().Message;
                return false;
            }
        }

        public static bool TryLoad(
            string recipeName,
            out OpenVisionRecipePinArrayGapValidationRecord record,
            out string error)
        {
            record = null;
            if (!TryValidateRecipeName(recipeName, out string normalizedRecipeName, out error))
            {
                return false;
            }

            string path;
            try
            {
                path = GetPath(normalizedRecipeName);
            }
            catch (Exception ex)
            {
                error = ex.GetBaseException().Message;
                return false;
            }

            if (!File.Exists(path))
            {
                error = "PinArrayGap validation record does not exist: " + path;
                return false;
            }

            if (!SerializeHelper.TryLoadFromXmlFile(path, out record)
                || !IsRecordShapeValid(record)
                || !string.Equals(record.RecipeName, normalizedRecipeName, StringComparison.OrdinalIgnoreCase))
            {
                record = null;
                error = "PinArrayGap validation record is invalid: " + path;
                return false;
            }

            error = string.Empty;
            return true;
        }

        public static bool TryMatchesCurrent(
            string recipeName,
            string pipelineXmlText,
            OpenVisionRecipeValidationSetOption train,
            OpenVisionRecipeValidationSetOption validation,
            OpenVisionRecipeValidationSetOption test,
            OpenVisionRecipePinArrayGapValidationRecord record,
            out bool matches,
            out string error)
        {
            matches = false;
            if (!IsRecordShapeValid(record))
            {
                error = "PinArrayGap validation record is missing or invalid.";
                return false;
            }

            if (!TryCreateRecord(
                    recipeName,
                    pipelineXmlText,
                    train,
                    validation,
                    test,
                    out OpenVisionRecipePinArrayGapValidationRecord current,
                    out error))
            {
                return false;
            }

            matches = RecordsMatch(record, current);
            error = string.Empty;
            return true;
        }

        private static string GetPath(string recipeName)
        {
            return Path.Combine(
                RecipeWorkspaceService.GetRecipeWorkspaceDirectory(recipeName),
                "VISION",
                "IntentSkills",
                FileName);
        }

        private static bool TryCreateRecord(
            string recipeName,
            string pipelineXmlText,
            OpenVisionRecipeValidationSetOption train,
            OpenVisionRecipeValidationSetOption validation,
            OpenVisionRecipeValidationSetOption test,
            out OpenVisionRecipePinArrayGapValidationRecord record,
            out string error)
        {
            record = null;
            if (!TryValidateRecipeName(recipeName, out string normalizedRecipeName, out error)
                || !TryReadPipelineIdentity(
                    pipelineXmlText,
                    out string pipelineName,
                    out string pipelineXmlSha256,
                    out double rangeMaximum,
                    out List<OpenVisionRecipePinArrayGapRowIdentity> rows,
                    out error)
                || !TryCreateSplitIdentity("Train", train, out OpenVisionRecipePinArrayGapValidationSplitIdentity trainIdentity, out HashSet<string> trainPaths, out error)
                || !TryCreateSplitIdentity("Validation", validation, out OpenVisionRecipePinArrayGapValidationSplitIdentity validationIdentity, out HashSet<string> validationPaths, out error)
                || !TryCreateSplitIdentity("Test", test, out OpenVisionRecipePinArrayGapValidationSplitIdentity testIdentity, out HashSet<string> testPaths, out error))
            {
                return false;
            }

            if (trainPaths.Overlaps(validationPaths)
                || trainPaths.Overlaps(testPaths)
                || validationPaths.Overlaps(testPaths))
            {
                error = "Train, Validation, and Test image paths must be pairwise disjoint.";
                return false;
            }

            record = new OpenVisionRecipePinArrayGapValidationRecord
            {
                SkillVersion = SkillVersion,
                RecipeName = normalizedRecipeName,
                PipelineName = pipelineName,
                FrozenUtc = DateTime.UtcNow,
                PipelineXmlSha256 = pipelineXmlSha256,
                DistancePxRangeMaximum = rangeMaximum,
                Rows = rows,
                Train = trainIdentity,
                Validation = validationIdentity,
                Test = testIdentity
            };
            error = string.Empty;
            return true;
        }

        private static bool TryValidateRecipeName(string recipeName, out string normalized, out string error)
        {
            normalized = recipeName?.Trim() ?? string.Empty;
            if (!RecipeWorkspaceService.IsValidRecipeName(normalized))
            {
                error = "A valid recipe name is required.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool TryReadPipelineIdentity(
            string pipelineXmlText,
            out string pipelineName,
            out string pipelineXmlSha256,
            out double rangeMaximum,
            out List<OpenVisionRecipePinArrayGapRowIdentity> rows,
            out string error)
        {
            pipelineName = string.Empty;
            pipelineXmlSha256 = string.Empty;
            rangeMaximum = 0D;
            rows = new List<OpenVisionRecipePinArrayGapRowIdentity>();

            if (!SerializeHelper.TryLoadFromXmlText(
                    pipelineXmlText,
                    out VisionPipeline pipeline,
                    out string loadError)
                || pipeline == null)
            {
                error = "PinArrayGap pipeline XML could not be loaded: " + loadError;
                return false;
            }

            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
                pipeline,
                new[] { "Main" });
            if (!validation.Success)
            {
                error = "PinArrayGap pipeline is not runnable: " + validation.FormatErrors();
                return false;
            }

            pipelineName = pipeline.Name?.Trim() ?? string.Empty;
            if (pipelineName.Length == 0)
            {
                error = "PinArrayGap pipeline name is required.";
                return false;
            }

            List<VisionPipelineStep> enabledSteps = (pipeline.Steps ?? new List<VisionPipelineStep>())
                .Where(step => step != null && step.Enabled)
                .ToList();
            if (enabledSteps.Count == 0
                || enabledSteps.Any(step => !string.Equals(step.ToolType, "PinArrayGap", StringComparison.OrdinalIgnoreCase)))
            {
                error = "Every enabled pipeline Step must use ToolType=PinArrayGap.";
                return false;
            }

            HashSet<string> outputLayers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int index = 0; index < enabledSteps.Count; index++)
            {
                VisionPipelineStep step = enabledSteps[index];
                string outputLayer = step.OutputLayer?.Trim() ?? string.Empty;
                if (!string.Equals(step.InputLayer?.Trim(), "Main", StringComparison.OrdinalIgnoreCase)
                    || outputLayer.Length == 0
                    || !outputLayers.Add(outputLayer))
                {
                    error = "Every enabled PinArrayGap Step must branch from InputLayer=Main to a unique non-empty OutputLayer.";
                    return false;
                }

                if (index > 0
                    && (!TryGetParameter(step, "ALLOW_BRANCH_INPUT", out string allowBranchInput)
                        || !string.Equals(allowBranchInput, "true", StringComparison.OrdinalIgnoreCase)))
                {
                    error = "Every PinArrayGap row after the first must set ALLOW_BRANCH_INPUT=true.";
                    return false;
                }

                if (!step.UseAcceptance
                    || !step.ExpectedSuccess
                    || !string.Equals(step.AcceptanceMetricName, VisionPipelineKnownMetrics.DistancePxRange, StringComparison.OrdinalIgnoreCase)
                    || step.UseAcceptanceMetricMinimum
                    || !step.UseAcceptanceMetricMaximum
                    || double.IsNaN(step.AcceptanceMetricMaximum)
                    || double.IsInfinity(step.AcceptanceMetricMaximum)
                    || step.AcceptanceMetricMaximum <= 0D)
                {
                    error = "Every enabled PinArrayGap Step must use the same positive DistancePxRange maximum-only acceptance gate.";
                    return false;
                }

                if (index == 0)
                {
                    rangeMaximum = step.AcceptanceMetricMaximum;
                }
                else if (Math.Abs(step.AcceptanceMetricMaximum - rangeMaximum) > DoubleTolerance)
                {
                    error = "Every enabled PinArrayGap Step must use the same DistancePxRange maximum.";
                    return false;
                }

                if (!TryCreateRowIdentity(step, index + 1, out OpenVisionRecipePinArrayGapRowIdentity row, out error))
                {
                    return false;
                }

                rows.Add(row);
            }

            pipelineXmlSha256 = ComputeSha256(pipelineXmlText);
            error = string.Empty;
            return true;
        }

        private static bool TryCreateRowIdentity(
            VisionPipelineStep step,
            int index,
            out OpenVisionRecipePinArrayGapRowIdentity row,
            out string error)
        {
            row = null;
            if (!TryGetParameter(step, "USE_ROI", out string useRoi)
                || !string.Equals(useRoi, "true", StringComparison.OrdinalIgnoreCase)
                || !TryGetParameter(step, "CvROI", out string roiText)
                || !TryParseRoi(roiText, out string roi)
                || !TryGetIntegerParameter(step, "DarkThreshold", 0, 255, out int darkThreshold)
                || !TryGetDoubleParameter(step, "MinDarkCoverageRatio", 0D, 1D, out double coverageRatio)
                || !TryGetIntegerParameter(step, "MinPinWidth", 1, int.MaxValue, out int minimumPinWidth)
                || !TryGetIntegerParameter(step, "MaxPinBreakWidth", 0, int.MaxValue, out int maximumBreakWidth)
                || !TryGetIntegerParameter(step, "MinGapWidth", 1, int.MaxValue, out int minimumGapWidth))
            {
                error = "PinArrayGap row " + index.ToString(CultureInfo.InvariantCulture)
                    + " requires USE_ROI=true, a valid CvROI, and all five valid detection parameters.";
                return false;
            }

            row = new OpenVisionRecipePinArrayGapRowIdentity
            {
                Index = index,
                Roi = roi,
                DarkThreshold = darkThreshold,
                MinDarkCoverageRatio = coverageRatio,
                MinPinWidth = minimumPinWidth,
                MaxPinBreakWidth = maximumBreakWidth,
                MinGapWidth = minimumGapWidth
            };
            error = string.Empty;
            return true;
        }

        private static bool TryCreateSplitIdentity(
            string role,
            OpenVisionRecipeValidationSetOption option,
            out OpenVisionRecipePinArrayGapValidationSplitIdentity identity,
            out HashSet<string> paths,
            out string error)
        {
            identity = null;
            paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            OpenVisionRecipeValidationSet set = option?.Set;
            string setName = set?.Name?.Trim() ?? string.Empty;
            List<OpenVisionRecipeValidationSetImage> images = set?.Images;
            if (setName.Length == 0 || images == null || images.Count == 0)
            {
                error = role + " validation set must be non-empty.";
                return false;
            }

            List<ValidationImageIdentity> imageIdentities = new List<ValidationImageIdentity>(images.Count);
            foreach (OpenVisionRecipeValidationSetImage image in images)
            {
                if (image == null
                    || !TryNormalizeExistingPath(image.Path, out string path)
                    || !TryNormalizeExpected(image.Expected, out string expected)
                    || !TryComputeFileSha256(path, out string fileSha256))
                {
                    error = role + " validation set contains an unreadable/missing file or invalid expected outcome.";
                    return false;
                }

                paths.Add(path);
                imageIdentities.Add(new ValidationImageIdentity(
                    path,
                    expected,
                    NormalizeNewLines(image.Notes ?? string.Empty),
                    fileSha256));
            }

            imageIdentities = imageIdentities
                .OrderBy(image => image.Path, StringComparer.OrdinalIgnoreCase)
                .ThenBy(image => image.Path, StringComparer.Ordinal)
                .ThenBy(image => image.Expected, StringComparer.Ordinal)
                .ThenBy(image => image.Notes, StringComparer.Ordinal)
                .ToList();

            StringBuilder canonical = new StringBuilder();
            foreach (ValidationImageIdentity image in imageIdentities)
            {
                AppendHashField(canonical, image.Path);
                AppendHashField(canonical, image.Expected);
                AppendHashField(canonical, image.Notes);
                AppendHashField(canonical, image.FileSha256);
                canonical.Append('\n');
            }

            identity = new OpenVisionRecipePinArrayGapValidationSplitIdentity
            {
                Role = role,
                SetName = setName,
                ImageCount = images.Count,
                ContentSha256 = ComputeSha256(canonical.ToString())
            };
            error = string.Empty;
            return true;
        }

        private static bool TryNormalizeExistingPath(string path, out string normalized)
        {
            normalized = string.Empty;
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            try
            {
                normalized = Path.GetFullPath(path.Trim());
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException)
            {
                normalized = string.Empty;
                return false;
            }

            return File.Exists(normalized);
        }

        private static bool TryNormalizeExpected(string expected, out string normalized)
        {
            if (string.Equals(expected?.Trim(), OpenVisionRecipeValidationSetImage.ExpectedOk, StringComparison.OrdinalIgnoreCase))
            {
                normalized = OpenVisionRecipeValidationSetImage.ExpectedOk;
                return true;
            }

            if (string.Equals(expected?.Trim(), OpenVisionRecipeValidationSetImage.ExpectedNg, StringComparison.OrdinalIgnoreCase))
            {
                normalized = OpenVisionRecipeValidationSetImage.ExpectedNg;
                return true;
            }

            normalized = string.Empty;
            return false;
        }

        private static bool TryGetIntegerParameter(
            VisionPipelineStep step,
            string key,
            int minimum,
            int maximum,
            out int value)
        {
            value = 0;
            return TryGetParameter(step, key, out string text)
                && int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
                && value >= minimum
                && value <= maximum;
        }

        private static bool TryGetDoubleParameter(
            VisionPipelineStep step,
            string key,
            double exclusiveMinimum,
            double inclusiveMaximum,
            out double value)
        {
            value = 0D;
            return TryGetParameter(step, key, out string text)
                && double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                && !double.IsNaN(value)
                && !double.IsInfinity(value)
                && value > exclusiveMinimum
                && value <= inclusiveMaximum;
        }

        private static bool TryGetParameter(VisionPipelineStep step, string key, out string value)
        {
            value = string.Empty;
            if (step?.Parameters == null)
            {
                return false;
            }

            foreach (KeyValuePair<string, string> parameter in step.Parameters)
            {
                if (string.Equals(parameter.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    value = parameter.Value?.Trim() ?? string.Empty;
                    return value.Length > 0;
                }
            }

            return false;
        }

        private static bool TryParseRoi(string text, out string normalized)
        {
            normalized = string.Empty;
            string[] values = (text ?? string.Empty)
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != 4
                || !int.TryParse(values[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int x)
                || !int.TryParse(values[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)
                || !int.TryParse(values[2].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int width)
                || !int.TryParse(values[3].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int height)
                || x < 0
                || y < 0
                || width <= 0
                || height <= 0)
            {
                return false;
            }

            normalized = string.Join(",", new[] { x, y, width, height }.Select(value => value.ToString(CultureInfo.InvariantCulture)));
            return true;
        }

        private static bool IsRecordShapeValid(OpenVisionRecipePinArrayGapValidationRecord record)
        {
            return record != null
                && string.Equals(record.SkillVersion, SkillVersion, StringComparison.Ordinal)
                && !string.IsNullOrWhiteSpace(record.RecipeName)
                && !string.IsNullOrWhiteSpace(record.PipelineName)
                && record.FrozenUtc != default
                && IsSha256(record.PipelineXmlSha256)
                && record.DistancePxRangeMaximum > 0D
                && !double.IsNaN(record.DistancePxRangeMaximum)
                && !double.IsInfinity(record.DistancePxRangeMaximum)
                && record.Rows != null
                && record.Rows.Count > 0
                && record.Rows.Select((row, index) => IsRowShapeValid(row, index + 1)).All(valid => valid)
                && IsSplitShapeValid(record.Train, "Train")
                && IsSplitShapeValid(record.Validation, "Validation")
                && IsSplitShapeValid(record.Test, "Test");
        }

        private static bool IsRowShapeValid(OpenVisionRecipePinArrayGapRowIdentity row, int expectedIndex)
        {
            return row != null
                && row.Index == expectedIndex
                && TryParseRoi(row.Roi, out _)
                && row.DarkThreshold >= 0
                && row.DarkThreshold <= 255
                && row.MinDarkCoverageRatio > 0D
                && row.MinDarkCoverageRatio <= 1D
                && !double.IsNaN(row.MinDarkCoverageRatio)
                && !double.IsInfinity(row.MinDarkCoverageRatio)
                && row.MinPinWidth > 0
                && row.MaxPinBreakWidth >= 0
                && row.MinGapWidth > 0;
        }

        private static bool IsSplitShapeValid(
            OpenVisionRecipePinArrayGapValidationSplitIdentity split,
            string role)
        {
            return split != null
                && string.Equals(split.Role, role, StringComparison.Ordinal)
                && !string.IsNullOrWhiteSpace(split.SetName)
                && split.ImageCount > 0
                && IsSha256(split.ContentSha256);
        }

        private static bool RecordsMatch(
            OpenVisionRecipePinArrayGapValidationRecord saved,
            OpenVisionRecipePinArrayGapValidationRecord current)
        {
            return string.Equals(saved.SkillVersion, current.SkillVersion, StringComparison.Ordinal)
                && string.Equals(saved.RecipeName, current.RecipeName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(saved.PipelineName, current.PipelineName, StringComparison.Ordinal)
                && string.Equals(saved.PipelineXmlSha256, current.PipelineXmlSha256, StringComparison.Ordinal)
                && Math.Abs(saved.DistancePxRangeMaximum - current.DistancePxRangeMaximum) <= DoubleTolerance
                && SplitsMatch(saved.Train, current.Train)
                && SplitsMatch(saved.Validation, current.Validation)
                && SplitsMatch(saved.Test, current.Test)
                && RowsMatch(saved.Rows, current.Rows);
        }

        private static bool SplitsMatch(
            OpenVisionRecipePinArrayGapValidationSplitIdentity saved,
            OpenVisionRecipePinArrayGapValidationSplitIdentity current)
        {
            return string.Equals(saved.Role, current.Role, StringComparison.Ordinal)
                && string.Equals(saved.SetName, current.SetName, StringComparison.Ordinal)
                && saved.ImageCount == current.ImageCount
                && string.Equals(saved.ContentSha256, current.ContentSha256, StringComparison.Ordinal);
        }

        private static bool RowsMatch(
            IReadOnlyList<OpenVisionRecipePinArrayGapRowIdentity> saved,
            IReadOnlyList<OpenVisionRecipePinArrayGapRowIdentity> current)
        {
            if (saved == null || current == null || saved.Count != current.Count)
            {
                return false;
            }

            for (int index = 0; index < saved.Count; index++)
            {
                OpenVisionRecipePinArrayGapRowIdentity left = saved[index];
                OpenVisionRecipePinArrayGapRowIdentity right = current[index];
                if (left == null
                    || right == null
                    || left.Index != right.Index
                    || !string.Equals(left.Roi, right.Roi, StringComparison.Ordinal)
                    || left.DarkThreshold != right.DarkThreshold
                    || Math.Abs(left.MinDarkCoverageRatio - right.MinDarkCoverageRatio) > DoubleTolerance
                    || left.MinPinWidth != right.MinPinWidth
                    || left.MaxPinBreakWidth != right.MaxPinBreakWidth
                    || left.MinGapWidth != right.MinGapWidth)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsSha256(string value)
        {
            return value != null
                && value.Length == 64
                && value.All(character => (character >= '0' && character <= '9') || (character >= 'a' && character <= 'f'));
        }

        private static string ComputeSha256(string value)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value ?? string.Empty)))
                .ToLowerInvariant();
        }

        private static bool TryComputeFileSha256(string path, out string value)
        {
            value = string.Empty;
            try
            {
                using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                value = Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
                return value.Length == 64;
            }
            catch
            {
                return false;
            }
        }

        private static void AppendHashField(StringBuilder builder, string value)
        {
            string text = value ?? string.Empty;
            builder.Append(text.Length.ToString(CultureInfo.InvariantCulture));
            builder.Append(':');
            builder.Append(text);
        }

        private static string NormalizeNewLines(string value)
        {
            return (value ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n');
        }

        private sealed class ValidationImageIdentity
        {
            public ValidationImageIdentity(string path, string expected, string notes, string fileSha256)
            {
                Path = path;
                Expected = expected;
                Notes = notes;
                FileSha256 = fileSha256;
            }

            public string Path { get; }

            public string Expected { get; }

            public string Notes { get; }

            public string FileSha256 { get; }
        }
    }
}
