using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OpenVisionLab
{
    internal enum VisionPipelineXmlSchemaIssueKind
    {
        None,
        InvalidXml,
        UnsupportedRoot,
        UnsupportedSchemaVersion,
        FutureSchemaVersion,
        UnknownCriticalElement,
        UnknownCriticalAttribute,
        UnsupportedNamespace
    }

    internal sealed class VisionPipelineXmlSchemaInspection
    {
        public VisionPipelineXmlSchemaIssueKind IssueKind { get; set; }

        public int SchemaVersion { get; set; }

        public bool IsSupported => IssueKind == VisionPipelineXmlSchemaIssueKind.None;

        public bool PreserveOriginal => IssueKind == VisionPipelineXmlSchemaIssueKind.FutureSchemaVersion
            || IssueKind == VisionPipelineXmlSchemaIssueKind.UnsupportedSchemaVersion
            || IssueKind == VisionPipelineXmlSchemaIssueKind.UnknownCriticalElement
            || IssueKind == VisionPipelineXmlSchemaIssueKind.UnknownCriticalAttribute
            || IssueKind == VisionPipelineXmlSchemaIssueKind.UnsupportedNamespace;

        public string ErrorMessage { get; set; } = string.Empty;
    }

    internal sealed class VisionPipelineXmlSchemaException : InvalidOperationException
    {
        public VisionPipelineXmlSchemaException(
            VisionPipelineXmlSchemaIssueKind issueKind,
            string message,
            bool preserveOriginal)
            : base(message)
        {
            IssueKind = issueKind;
            PreserveOriginal = preserveOriginal;
        }

        public VisionPipelineXmlSchemaIssueKind IssueKind { get; }

        public bool PreserveOriginal { get; }
    }

    internal static class VisionPipelineXmlSchemaPolicy
    {
        internal const int CurrentSchemaVersion = 1;
        internal const string OptionalExtensionNamespace = "urn:openvisionlab:extension";

        private const string XmlSchemaInstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";
        private const string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";

        private static readonly HashSet<string> RootElements = CreateSet(
            "Name",
            "Steps",
            "SchemaVersion",
            "Version",
            "Extensions");

        private static readonly HashSet<string> StepElements = CreateSet(
            "Name",
            "ToolType",
            "Enabled",
            "InputLayer",
            "OutputLayer",
            "UseAcceptance",
            "ExpectedSuccess",
            "MaxElapsedMilliseconds",
            "RequiredMessageText",
            "AcceptanceMetricName",
            "UseAcceptanceMetricMinimum",
            "AcceptanceMetricMinimum",
            "UseAcceptanceMetricMaximum",
            "AcceptanceMetricMaximum",
            "Parameters",
            "Extensions");

        private static readonly HashSet<string> StepsElements = CreateSet(
            "Step",
            "Extensions");

        private static readonly HashSet<string> ParametersElements = CreateSet(
            "Parameter",
            "Extensions");

        private static readonly HashSet<string> ParameterElements = CreateSet(
            "Key",
            "Value",
            "Extensions");

        public static VisionPipelineXmlSchemaInspection Inspect(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return Failure(
                    VisionPipelineXmlSchemaIssueKind.InvalidXml,
                    "Pipeline XML is empty.");
            }

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null,
                    IgnoreComments = false,
                    IgnoreWhitespace = false
                };
                using MemoryStream stream = new MemoryStream(bytes, writable: false);
                using XmlReader reader = XmlReader.Create(stream, settings);
                XDocument document = XDocument.Load(
                    reader,
                    LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
                return InspectDocument(document);
            }
            catch (Exception exception) when (exception is XmlException || exception is InvalidOperationException)
            {
                return Failure(
                    VisionPipelineXmlSchemaIssueKind.InvalidXml,
                    "Pipeline XML is not well-formed: " + exception.GetBaseException().Message);
            }
            catch (IOException exception)
            {
                return Failure(
                    VisionPipelineXmlSchemaIssueKind.InvalidXml,
                    "Pipeline XML could not be read: " + exception.GetBaseException().Message);
            }
        }

        public static VisionPipelineXmlSchemaInspection Inspect(string xmlText)
        {
            if (string.IsNullOrWhiteSpace(xmlText))
            {
                return Failure(
                    VisionPipelineXmlSchemaIssueKind.InvalidXml,
                    "Pipeline XML is empty.");
            }

            return Inspect(Encoding.UTF8.GetBytes(xmlText));
        }

        public static VisionPipelineXmlSchemaInspection InspectFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return Failure(
                    VisionPipelineXmlSchemaIssueKind.InvalidXml,
                    "Pipeline XML file was not found.");
            }

            try
            {
                return Inspect(File.ReadAllBytes(path));
            }
            catch (IOException exception)
            {
                return Failure(
                    VisionPipelineXmlSchemaIssueKind.InvalidXml,
                    "Pipeline XML could not be read: " + exception.GetBaseException().Message);
            }
        }

        public static void ThrowIfExecutionBlocked(byte[] bytes)
        {
            ThrowIfExecutionBlocked(Inspect(bytes));
        }

        public static void ThrowIfExecutionBlocked(string xmlText)
        {
            ThrowIfExecutionBlocked(Inspect(xmlText));
        }

        private static void ThrowIfExecutionBlocked(VisionPipelineXmlSchemaInspection inspection)
        {
            if (inspection == null || inspection.IsSupported || !inspection.PreserveOriginal)
            {
                return;
            }

            throw new VisionPipelineXmlSchemaException(
                inspection.IssueKind,
                inspection.ErrorMessage,
                inspection.PreserveOriginal);
        }

        private static VisionPipelineXmlSchemaInspection InspectDocument(XDocument document)
        {
            XElement root = document?.Root;
            if (root == null)
            {
                return Failure(
                    VisionPipelineXmlSchemaIssueKind.InvalidXml,
                    "Pipeline XML has no document root.");
            }

            if (!string.Equals(root.Name.LocalName, "VisionPipeline", StringComparison.Ordinal))
            {
                return Failure(
                    VisionPipelineXmlSchemaIssueKind.UnsupportedRoot,
                    "Pipeline XML root must be the unqualified VisionPipeline element.");
            }

            if (!string.IsNullOrEmpty(root.Name.NamespaceName))
            {
                return Failure(
                    VisionPipelineXmlSchemaIssueKind.UnsupportedNamespace,
                    "Pipeline XML root uses an unsupported namespace; execution is blocked and the original XML is preserved.");
            }

            VisionPipelineXmlSchemaInspection attributeResult = ValidateAttributes(root, "VisionPipeline");
            if (!attributeResult.IsSupported)
            {
                return attributeResult;
            }

            if (!TryReadSchemaVersion(root, out int schemaVersion, out VisionPipelineXmlSchemaInspection versionResult))
            {
                return versionResult;
            }

            if (schemaVersion > CurrentSchemaVersion)
            {
                return Failure(
                    VisionPipelineXmlSchemaIssueKind.FutureSchemaVersion,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Pipeline XML schema version {0} is newer than the supported version {1}; execution is blocked and the original XML is preserved.",
                        schemaVersion,
                        CurrentSchemaVersion));
            }

            if (schemaVersion < CurrentSchemaVersion)
            {
                return Failure(
                    VisionPipelineXmlSchemaIssueKind.UnsupportedSchemaVersion,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Pipeline XML schema version {0} is not supported; execution is blocked and the original XML is preserved.",
                        schemaVersion));
            }

            VisionPipelineXmlSchemaInspection elementResult = ValidateChildren(
                root,
                RootElements,
                "VisionPipeline");
            if (!elementResult.IsSupported)
            {
                return elementResult;
            }

            return new VisionPipelineXmlSchemaInspection
            {
                IssueKind = VisionPipelineXmlSchemaIssueKind.None,
                SchemaVersion = schemaVersion
            };
        }

        private static VisionPipelineXmlSchemaInspection ValidateChildren(
            XElement parent,
            IReadOnlySet<string> allowedNames,
            string path)
        {
            foreach (XElement child in parent.Elements())
            {
                if (IsOptionalExtension(child))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(child.Name.NamespaceName))
                {
                    return Failure(
                        VisionPipelineXmlSchemaIssueKind.UnsupportedNamespace,
                        $"Pipeline XML element '{BuildPath(path, child.Name.LocalName)}' uses an unsupported namespace; execution is blocked and the original XML is preserved.");
                }

                if (!allowedNames.Contains(child.Name.LocalName))
                {
                    return Failure(
                        VisionPipelineXmlSchemaIssueKind.UnknownCriticalElement,
                        $"Pipeline XML contains unknown critical element '{BuildPath(path, child.Name.LocalName)}'; execution is blocked and the original XML is preserved.");
                }

                VisionPipelineXmlSchemaInspection attributeResult = ValidateAttributes(
                    child,
                    BuildPath(path, child.Name.LocalName));
                if (!attributeResult.IsSupported)
                {
                    return attributeResult;
                }

                IReadOnlySet<string> childAllowedNames = ResolveAllowedChildren(child.Name.LocalName);
                if (childAllowedNames == null)
                {
                    if (child.Elements().Any(element => !IsOptionalExtension(element)))
                    {
                        return Failure(
                            VisionPipelineXmlSchemaIssueKind.UnknownCriticalElement,
                            $"Pipeline XML element '{BuildPath(path, child.Name.LocalName)}' contains an unknown critical child; execution is blocked and the original XML is preserved.");
                    }

                    continue;
                }

                VisionPipelineXmlSchemaInspection nestedResult = ValidateChildren(
                    child,
                    childAllowedNames,
                    BuildPath(path, child.Name.LocalName));
                if (!nestedResult.IsSupported)
                {
                    return nestedResult;
                }
            }

            return new VisionPipelineXmlSchemaInspection
            {
                IssueKind = VisionPipelineXmlSchemaIssueKind.None
            };
        }

        private static VisionPipelineXmlSchemaInspection ValidateAttributes(XElement element, string path)
        {
            foreach (XAttribute attribute in element.Attributes())
            {
                if (attribute.IsNamespaceDeclaration
                    || string.Equals(attribute.Name.NamespaceName, XmlnsNamespace, StringComparison.Ordinal)
                    || string.Equals(attribute.Name.NamespaceName, XmlSchemaInstanceNamespace, StringComparison.Ordinal)
                    || string.Equals(attribute.Name.NamespaceName, OptionalExtensionNamespace, StringComparison.Ordinal))
                {
                    continue;
                }

                if (element.Parent == null && IsVersionName(attribute.Name.LocalName))
                {
                    continue;
                }

                return Failure(
                    VisionPipelineXmlSchemaIssueKind.UnknownCriticalAttribute,
                    $"Pipeline XML contains unknown critical attribute '{attribute.Name.LocalName}' on '{path}'; execution is blocked and the original XML is preserved.");
            }

            return new VisionPipelineXmlSchemaInspection
            {
                IssueKind = VisionPipelineXmlSchemaIssueKind.None
            };
        }

        private static bool TryReadSchemaVersion(
            XElement root,
            out int schemaVersion,
            out VisionPipelineXmlSchemaInspection failure)
        {
            schemaVersion = CurrentSchemaVersion;
            failure = null;
            List<string> values = root.Attributes()
                .Where(attribute => !attribute.IsNamespaceDeclaration && IsVersionName(attribute.Name.LocalName))
                .Select(attribute => attribute.Value)
                .Concat(root.Elements()
                    .Where(element => IsVersionName(element.Name.LocalName))
                    .Select(element => element.Value))
                .ToList();
            if (values.Count == 0)
            {
                return true;
            }

            if (values.Any(value => !int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)))
            {
                failure = Failure(
                    VisionPipelineXmlSchemaIssueKind.UnsupportedSchemaVersion,
                    "Pipeline XML schema version must be a positive invariant integer; execution is blocked and the original XML is preserved.");
                return false;
            }

            int parsed = int.Parse(values[0], CultureInfo.InvariantCulture);
            if (values.Any(value => int.Parse(value, CultureInfo.InvariantCulture) != parsed))
            {
                failure = Failure(
                    VisionPipelineXmlSchemaIssueKind.UnsupportedSchemaVersion,
                    "Pipeline XML contains conflicting schema version values; execution is blocked and the original XML is preserved.");
                return false;
            }

            schemaVersion = parsed;
            return true;
        }

        private static IReadOnlySet<string> ResolveAllowedChildren(string elementName)
        {
            return elementName switch
            {
                "Steps" => StepsElements,
                "Step" => StepElements,
                "Parameters" => ParametersElements,
                "Parameter" => ParameterElements,
                _ => null
            };
        }

        private static bool IsOptionalExtension(XElement element)
        {
            return string.Equals(element.Name.NamespaceName, OptionalExtensionNamespace, StringComparison.Ordinal)
                || (string.IsNullOrEmpty(element.Name.NamespaceName)
                    && string.Equals(element.Name.LocalName, "Extensions", StringComparison.Ordinal));
        }

        private static bool IsVersionName(string name)
        {
            return string.Equals(name, "SchemaVersion", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "Version", StringComparison.OrdinalIgnoreCase);
        }

        private static string BuildPath(string parent, string child)
        {
            return parent + "/" + child;
        }

        private static HashSet<string> CreateSet(params string[] values)
        {
            return new HashSet<string>(values, StringComparer.Ordinal);
        }

        private static VisionPipelineXmlSchemaInspection Failure(
            VisionPipelineXmlSchemaIssueKind issueKind,
            string message)
        {
            return new VisionPipelineXmlSchemaInspection
            {
                IssueKind = issueKind,
                ErrorMessage = message ?? string.Empty
            };
        }
    }
}
