using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace VisionUiContractCheck
{
    internal static class Program
    {
        private const string PropertyEditorAttributeName = "System.Windows.Controls.WpfPropertyGrid.PropertyEditorAttribute";
        private const string ThresholdEditorAttributeName = "System.Windows.Controls.WpfPropertyGrid.ThresholdEditorAttribute";
        private const string RangeEditorAttributeName = "System.Windows.Controls.WpfPropertyGrid.RangeEditorAttribute";
        private const string MetricRangeEditorAttributeName = "System.Windows.Controls.WpfPropertyGrid.MetricRangeEditorAttribute";

        private static string _assemblyDirectory = string.Empty;
        private static AssemblyDependencyResolver _dependencyResolver;

        private static int Main(string[] args)
        {
            try
            {
                _assemblyDirectory = ResolveAssemblyDirectory(args);
                string appAssemblyPath = Path.Combine(_assemblyDirectory, "OpenVisionLab.dll");
                _dependencyResolver = new AssemblyDependencyResolver(appAssemblyPath);
                AssemblyLoadContext.Default.Resolving += ResolveFromAssemblyDirectory;

                Assembly appAssembly = LoadRequiredAssembly("OpenVisionLab.dll");
                Assembly abstractionsAssembly = LoadRequiredAssembly("PropertyGrid.Abstractions.dll");
                LoadRequiredAssembly("WpfPropertyGridBridge.dll");

                AssertDisplayOptions(abstractionsAssembly);
                AssertThresholdEditorContract(appAssembly);
                AssertRangeEditorContract(appAssembly, "ContourProperty", "MIN_AREA", "MIN_AREA", "MAX_AREA", "Contour area range");
                AssertRangeEditorContract(appAssembly, "BlobProperty", "MIN_AREA", "MIN_AREA", "MAX_AREA", "Blob area range");
                AssertRangeEditorContract(appAssembly, "MatchingProperty", "FIND_ANGLE_MIN", "FIND_ANGLE_MIN", "FIND_ANGLE_MAX", "Matching angle range");
                AssertRangeEditorContract(appAssembly, "MatchingProperty", "CANNY_LOW", "CANNY_LOW", "CANNY_HIGH", "Matching canny range");
                AssertRangeEditorContract(appAssembly, "MeanProperty", "MEAN_MIN", "MEAN_MIN", "MEAN_MAX", "Mean range");
                AssertPipelineLayerSelectorContract(appAssembly);
                AssertPipelineMetricRangeContract(appAssembly);
                AssertNativeToolPrewarmContract(appAssembly);
                AssertNativeToolNavigationContract(appAssembly);

                Console.WriteLine("VisionUiContract=OK");
                Console.WriteLine("AssemblyDirectory=" + _assemblyDirectory);
                Console.WriteLine("PropertyGridDisplayOptions=OK");
                Console.WriteLine("ThresholdEditorContract=OK");
                Console.WriteLine("RangeEditorContract=OK");
                Console.WriteLine("PipelineLayerSelectorContract=OK");
                Console.WriteLine("PipelineMetricRangeContract=OK");
                Console.WriteLine("NativeToolPrewarmContract=OK");
                Console.WriteLine("NativeToolNavigationContract=OK");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("VisionUiContract=NG");
                Console.Error.WriteLine(ex.Message);
                if (ex is ReflectionTypeLoadException reflectionLoadException)
                {
                    foreach (Exception loaderException in reflectionLoadException.LoaderExceptions.Where(item => item != null))
                    {
                        Console.Error.WriteLine(loaderException.GetBaseException().Message);
                    }
                }
                return 1;
            }
        }

        private static string ResolveAssemblyDirectory(string[] args)
        {
            if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                return Path.GetFullPath(args[0]);
            }

            string repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
            string x64Debug = Path.Combine(repoRoot, "bin", "x64", "Debug");
            if (File.Exists(Path.Combine(x64Debug, "OpenVisionLab.dll")))
            {
                return x64Debug;
            }

            return Path.Combine(repoRoot, "bin", "Debug");
        }

        private static Assembly ResolveFromAssemblyDirectory(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            string resolvedPath = _dependencyResolver != null ? _dependencyResolver.ResolveAssemblyToPath(assemblyName) : null;
            if (!string.IsNullOrWhiteSpace(resolvedPath) && File.Exists(resolvedPath))
            {
                return context.LoadFromAssemblyPath(resolvedPath);
            }

            string assemblyPath = Path.Combine(_assemblyDirectory, assemblyName.Name + ".dll");
            if (File.Exists(assemblyPath))
            {
                return context.LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        private static Assembly LoadRequiredAssembly(string fileName)
        {
            string path = Path.Combine(_assemblyDirectory, fileName);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Required assembly was not found.", path);
            }

            return AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        }

        private static void AssertDisplayOptions(Assembly abstractionsAssembly)
        {
            Type optionsType = abstractionsAssembly.GetType("OpenVisionLab.PropertyGrid.PropertyGridDisplayOptions")
                ?? throw new InvalidOperationException("PropertyGridDisplayOptions type was not found.");

            object toolForm = optionsType.GetProperty("ToolForm", BindingFlags.Public | BindingFlags.Static)?.GetValue(null)
                ?? throw new InvalidOperationException("PropertyGridDisplayOptions.ToolForm was not found.");
            object pipeline = optionsType.GetProperty("Pipeline", BindingFlags.Public | BindingFlags.Static)?.GetValue(null)
                ?? throw new InvalidOperationException("PropertyGridDisplayOptions.Pipeline was not found.");

            double toolNameWidth = GetDoubleProperty(toolForm, "PropertyNameColumnWidth");
            double toolEditorWidth = GetDoubleProperty(toolForm, "EditorColumnMinWidth");
            double pipelineNameWidth = GetDoubleProperty(pipeline, "PropertyNameColumnWidth");
            double pipelineEditorWidth = GetDoubleProperty(pipeline, "EditorColumnMinWidth");

            if (toolNameWidth < 140 || toolEditorWidth < 160)
            {
                throw new InvalidOperationException("ToolForm PropertyGrid display options are too narrow for the shared WPG editor layout.");
            }

            if (pipelineNameWidth < 140 || pipelineEditorWidth < 320)
            {
                throw new InvalidOperationException("Pipeline PropertyGrid display options are too narrow for step editor rows.");
            }
        }

        private static void AssertThresholdEditorContract(Assembly appAssembly)
        {
            PropertyInfo property = FindType(appAssembly, "OpenCvPropertyBase").GetProperty("THRESHOLD")
                ?? throw new InvalidOperationException("OpenCvPropertyBase.THRESHOLD was not found.");

            CustomAttributeData editor = FindAttribute(property, PropertyEditorAttributeName);
            Type editorType = editor.ConstructorArguments[0].Value as Type;
            string editorTypeName = editorType != null ? editorType.Name : string.Empty;
            if (!string.Equals(editorTypeName, "WpgThresholdEditor", StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"OpenCvPropertyBase.THRESHOLD should use WpgThresholdEditor. Actual={editorTypeName}");
            }

            CustomAttributeData threshold = FindAttribute(property, ThresholdEditorAttributeName);
            double minimum = ToDouble(threshold.ConstructorArguments[0].Value);
            double maximum = ToDouble(threshold.ConstructorArguments[1].Value);
            double tick = ToDouble(threshold.ConstructorArguments[2].Value);
            string invertPropertyName = threshold.ConstructorArguments.Count >= 5
                ? Convert.ToString(threshold.ConstructorArguments[4].Value) ?? string.Empty
                : string.Empty;

            if (minimum != 0 || maximum != 255 || tick != 1)
            {
                throw new InvalidOperationException("Threshold editor range should stay 0..255 with tick 1.");
            }

            if (!string.Equals(invertPropertyName, "USE_BITWISENOT", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Threshold editor should keep the Invert property linked to USE_BITWISENOT.");
            }
        }

        private static void AssertRangeEditorContract(
            Assembly appAssembly,
            string ownerTypeName,
            string propertyName,
            string expectedMinimumProperty,
            string expectedMaximumProperty,
            string label)
        {
            PropertyInfo property = FindType(appAssembly, ownerTypeName).GetProperty(propertyName)
                ?? throw new InvalidOperationException($"{label}: property '{propertyName}' was not found on {ownerTypeName}.");

            CustomAttributeData editor = FindAttribute(property, PropertyEditorAttributeName);
            Type editorType = editor.ConstructorArguments[0].Value as Type;
            string editorTypeName = editorType != null ? editorType.Name : string.Empty;
            if (!string.Equals(editorTypeName, "WpgRangeEditor", StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"{label}: property should use WpgRangeEditor. Actual={editorTypeName}");
            }

            CustomAttributeData range = FindAttribute(property, RangeEditorAttributeName);
            string minPropertyName = Convert.ToString(range.ConstructorArguments[4].Value) ?? string.Empty;
            string maxPropertyName = Convert.ToString(range.ConstructorArguments[5].Value) ?? string.Empty;
            if (!string.Equals(minPropertyName, expectedMinimumProperty, StringComparison.Ordinal)
                || !string.Equals(maxPropertyName, expectedMaximumProperty, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"{label}: range should link {expectedMinimumProperty}/{expectedMaximumProperty}. Actual={minPropertyName}/{maxPropertyName}");
            }

            object rangeAttribute = property.GetCustomAttributes(inherit: true)
                .FirstOrDefault(attribute => string.Equals(attribute.GetType().FullName, RangeEditorAttributeName, StringComparison.Ordinal))
                ?? throw new InvalidOperationException($"{label}: range editor attribute could not be instantiated.");
            string invertPropertyName = Convert.ToString(rangeAttribute.GetType().GetProperty("InvertPropertyName")?.GetValue(rangeAttribute)) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(invertPropertyName)
                || string.Equals(invertPropertyName, propertyName, StringComparison.Ordinal)
                || string.Equals(invertPropertyName, expectedMinimumProperty, StringComparison.Ordinal)
                || string.Equals(invertPropertyName, expectedMaximumProperty, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"{label}: range editor must not fall back to the range property as an invert checkbox. Actual={invertPropertyName}");
            }

            if (FindType(appAssembly, ownerTypeName).GetProperty(invertPropertyName, BindingFlags.Instance | BindingFlags.Public) != null)
            {
                throw new InvalidOperationException($"{label}: range editor unexpectedly links invert to existing property '{invertPropertyName}'.");
            }
        }

        private static void AssertPipelineLayerSelectorContract(Assembly appAssembly)
        {
            Type[] pipelineTypes = GetPipelineStepPropertyTypes(appAssembly);
            if (pipelineTypes.Length < 8)
            {
                throw new InvalidOperationException($"Pipeline step property types were not found. Count={pipelineTypes.Length}");
            }

            foreach (Type type in pipelineTypes)
            {
                AssertLayerConverter(type, "InputLayer");
                AssertLayerConverter(type, "OutputLayer");
            }
        }

        private static void AssertLayerConverter(Type type, string propertyName)
        {
            PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException($"{type.Name}.{propertyName} was not found.");
            CustomAttributeData converter = property.CustomAttributes.FirstOrDefault(attribute =>
                string.Equals(attribute.AttributeType.FullName, typeof(System.ComponentModel.TypeConverterAttribute).FullName, StringComparison.Ordinal))
                ?? throw new InvalidOperationException($"{type.Name}.{propertyName} should use PipelineLayerNameConverter.");
            string converterTypeName = Convert.ToString(converter.ConstructorArguments.FirstOrDefault().Value) ?? string.Empty;
            if (!converterTypeName.EndsWith("PipelineLayerNameConverter", StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"{type.Name}.{propertyName} should use PipelineLayerNameConverter. Actual={converterTypeName}");
            }
        }

        private static void AssertPipelineMetricRangeContract(Assembly appAssembly)
        {
            foreach (Type type in GetPipelineStepPropertyTypes(appAssembly))
            {
                PropertyInfo property = type.GetProperty("AcceptanceMetricMinimum", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    ?? throw new InvalidOperationException($"{type.Name}.AcceptanceMetricMinimum was not found.");

                CustomAttributeData editor = FindAttribute(property, PropertyEditorAttributeName);
                Type editorType = editor.ConstructorArguments[0].Value as Type;
                string editorTypeName = editorType != null ? editorType.Name : string.Empty;
                if (!string.Equals(editorTypeName, "WpgMetricRangeEditor", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException($"{type.Name}.AcceptanceMetricMinimum should use WpgMetricRangeEditor. Actual={editorTypeName}");
                }

                CustomAttributeData range = FindAttribute(property, MetricRangeEditorAttributeName);
                string useMinPropertyName = Convert.ToString(range.ConstructorArguments[1].Value) ?? string.Empty;
                string minPropertyName = Convert.ToString(range.ConstructorArguments[2].Value) ?? string.Empty;
                string useMaxPropertyName = Convert.ToString(range.ConstructorArguments[3].Value) ?? string.Empty;
                string maxPropertyName = Convert.ToString(range.ConstructorArguments[4].Value) ?? string.Empty;
                if (!string.Equals(useMinPropertyName, "UseAcceptanceMetricMinimum", StringComparison.Ordinal)
                    || !string.Equals(minPropertyName, "AcceptanceMetricMinimum", StringComparison.Ordinal)
                    || !string.Equals(useMaxPropertyName, "UseAcceptanceMetricMaximum", StringComparison.Ordinal)
                    || !string.Equals(maxPropertyName, "AcceptanceMetricMaximum", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"{type.Name}.AcceptanceMetricMinimum should link metric min/max acceptance flags. "
                        + $"Actual={useMinPropertyName}/{minPropertyName}/{useMaxPropertyName}/{maxPropertyName}");
                }
            }
        }

        private static void AssertNativeToolPrewarmContract(Assembly appAssembly)
        {
            Type policyType = FindType(appAssembly, "OpenVisionNativeToolPrewarmPolicy");
            MethodInfo getDefaultMenus = policyType.GetMethod("GetDefaultMenus", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException("OpenVisionNativeToolPrewarmPolicy.GetDefaultMenus was not found.");
            MethodInfo getMenus = policyType.GetMethod("GetMenus", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException("OpenVisionNativeToolPrewarmPolicy.GetMenus was not found.");
            _ = policyType.GetMethod("GetLayoutWarmSize", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException("OpenVisionNativeToolPrewarmPolicy.GetLayoutWarmSize was not found.");
            object defaultPrewarmMenusObject = getDefaultMenus.Invoke(null, Array.Empty<object>())
                ?? throw new InvalidOperationException("OpenVisionNativeToolPrewarmPolicy.GetDefaultMenus returned null.");
            object basePrewarmMenusObject = getMenus.Invoke(null, new object[] { null })
                ?? throw new InvalidOperationException("OpenVisionNativeToolPrewarmPolicy.GetMenus(null) returned null.");

            string[] registeredMenus = GetNativeToolRegistrationMenuNames(appAssembly);
            string[] prewarmMenus = ToMenuNames(basePrewarmMenusObject).ToArray();
            string[] priorityMenus = { "Blob", "Matching", "EdgeBasedMatching", "FeatureMatching", "Line", "Contour" };

            if (registeredMenus.Length < 10)
            {
                throw new InvalidOperationException($"Native tool registry has too few menus. Count={registeredMenus.Length}");
            }

            AssertPrewarmMenuSetMatchesRegistered("Native tool base prewarm", prewarmMenus, registeredMenus);
            AssertPrewarmMenuSetMatchesRegistered("Native tool default prewarm", ToMenuNames(defaultPrewarmMenusObject).ToArray(), registeredMenus);

            if (!prewarmMenus.Take(priorityMenus.Length).SequenceEqual(priorityMenus, StringComparer.Ordinal))
            {
                throw new InvalidOperationException(
                    "Native tool prewarm should prioritize heavier PropertyGrid/inspection tools. "
                    + "ActualFirst=" + string.Join(",", prewarmMenus.Take(priorityMenus.Length)));
            }
        }

        private static void AssertPrewarmMenuSetMatchesRegistered(string label, string[] prewarmMenus, string[] registeredMenus)
        {
            string[] prewarmDistinct = prewarmMenus.Distinct(StringComparer.Ordinal).OrderBy(name => name, StringComparer.Ordinal).ToArray();
            if (prewarmMenus.Length != prewarmDistinct.Length)
            {
                throw new InvalidOperationException(label + " contains duplicate menus.");
            }

            string[] missingFromPrewarm = registeredMenus.Except(prewarmDistinct, StringComparer.Ordinal).ToArray();
            string[] unknownPrewarmMenus = prewarmDistinct.Except(registeredMenus, StringComparer.Ordinal).ToArray();
            if (missingFromPrewarm.Length != 0 || unknownPrewarmMenus.Length != 0)
            {
                throw new InvalidOperationException(
                    label + " menus should match registered document factories. "
                    + "MissingFromPrewarm=" + string.Join(",", missingFromPrewarm)
                    + " UnknownPrewarmMenus=" + string.Join(",", unknownPrewarmMenus));
            }
        }

        private static void AssertNativeToolNavigationContract(Assembly appAssembly)
        {
            string[] registeredMenus = GetNativeToolRegistrationMenuNames(appAssembly);
            string[] shellMenus = GetShellNavigationMenuNames(appAssembly).ToArray();
            string[] shellDistinct = shellMenus.Distinct(StringComparer.Ordinal).OrderBy(name => name, StringComparer.Ordinal).ToArray();
            string[] nativeShellMenus = shellDistinct.Where(name => !string.Equals(name, "Pipeline", StringComparison.Ordinal)).ToArray();

            if (shellMenus.Length != shellDistinct.Length)
            {
                throw new InvalidOperationException("Shell navigation contains duplicate tool menu entries.");
            }

            if (!shellDistinct.Contains("Pipeline", StringComparer.Ordinal))
            {
                throw new InvalidOperationException("Shell navigation must expose the Pipeline tool.");
            }

            string[] missingFromShell = registeredMenus.Except(nativeShellMenus, StringComparer.Ordinal).ToArray();
            string[] unknownShellMenus = nativeShellMenus.Except(registeredMenus, StringComparer.Ordinal).ToArray();
            if (missingFromShell.Length != 0 || unknownShellMenus.Length != 0)
            {
                throw new InvalidOperationException(
                    "Native tool registry and shell navigation should stay in sync for new tool additions. "
                    + "MissingFromShell=" + string.Join(",", missingFromShell)
                    + " UnknownShellMenus=" + string.Join(",", unknownShellMenus));
            }
        }

        private static string[] GetNativeToolRegistrationMenuNames(Assembly appAssembly)
        {
            Type registryType = FindType(appAssembly, "OpenVisionNativeToolRegistry");
            FieldInfo registrationsField = registryType.GetField("Registrations", BindingFlags.Static | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException("OpenVisionNativeToolRegistry.Registrations was not found.");
            object registrations = registrationsField.GetValue(null)
                ?? throw new InvalidOperationException("OpenVisionNativeToolRegistry.Registrations is null.");
            return ToRegistrationMenuNames(registrations).Distinct(StringComparer.Ordinal).OrderBy(name => name, StringComparer.Ordinal).ToArray();
        }

        private static IEnumerable<string> GetShellNavigationMenuNames(Assembly appAssembly)
        {
            Type catalogType = FindType(appAssembly, "OpenVisionShellCommandCatalog");
            MethodInfo createNavigationGroups = catalogType.GetMethod("CreateNavigationGroups", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException("OpenVisionShellCommandCatalog.CreateNavigationGroups was not found.");
            object groups = createNavigationGroups.Invoke(null, Array.Empty<object>())
                ?? throw new InvalidOperationException("OpenVisionShellCommandCatalog.CreateNavigationGroups returned null.");

            if (groups is not IEnumerable groupItems)
            {
                throw new InvalidOperationException("OpenVisionShellCommandCatalog.CreateNavigationGroups should return enumerable groups.");
            }

            foreach (object group in groupItems)
            {
                object items = group.GetType().GetProperty("Items")?.GetValue(group)
                    ?? throw new InvalidOperationException("Shell navigation group does not expose Items.");
                if (items is not IEnumerable navItems)
                {
                    throw new InvalidOperationException("Shell navigation group Items should be enumerable.");
                }

                foreach (object item in navItems)
                {
                    object menu = item.GetType().GetProperty("Menu")?.GetValue(item)
                        ?? throw new InvalidOperationException("Shell navigation item does not expose Menu.");
                    string name = Convert.ToString(menu) ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        yield return name;
                    }
                }
            }
        }

        private static IEnumerable<string> ToRegistrationMenuNames(object enumerable)
        {
            if (enumerable is not IEnumerable items)
            {
                throw new InvalidOperationException("Expected an enumerable native tool registration collection.");
            }

            foreach (object item in items)
            {
                object menu = item.GetType().GetProperty("Menu")?.GetValue(item)
                    ?? throw new InvalidOperationException("Native tool registration does not expose Menu.");
                string name = Convert.ToString(menu) ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    yield return name;
                }
            }
        }

        private static IEnumerable<string> ToMenuNames(object enumerable)
        {
            if (enumerable is not IEnumerable items)
            {
                throw new InvalidOperationException("Expected an enumerable menu collection.");
            }

            foreach (object item in items)
            {
                string name = Convert.ToString(item) ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    yield return name;
                }
            }
        }

        private static Type[] GetPipelineStepPropertyTypes(Assembly appAssembly)
        {
            return GetLoadableTypes(appAssembly)
                .Where(type => type.Name.StartsWith("Pipeline", StringComparison.Ordinal)
                    && type.Name.EndsWith("Property", StringComparison.Ordinal)
                    && type.GetProperty("InputLayer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null
                    && type.GetProperty("OutputLayer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null
                    && type.GetProperty("AcceptanceMetricMinimum", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null)
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToArray();
        }

        private static Type[] GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(type => type != null).ToArray();
            }
        }

        private static Type FindType(Assembly assembly, string typeName)
        {
            string[] candidateNames =
            {
                typeName,
                "OpenVisionLab." + typeName,
                "OpenVisionLab.Vision._1._Tools.OpenCV." + typeName
            };

            foreach (string candidateName in candidateNames)
            {
                Type type = assembly.GetType(candidateName, throwOnError: false);
                if (type != null)
                {
                    return type;
                }
            }

            throw new InvalidOperationException($"{typeName} type was not found.");
        }

        private static CustomAttributeData FindAttribute(PropertyInfo property, string attributeTypeName)
        {
            return property.CustomAttributes.FirstOrDefault(attribute => string.Equals(attribute.AttributeType.FullName, attributeTypeName, StringComparison.Ordinal))
                ?? throw new InvalidOperationException($"{property.DeclaringType?.Name}.{property.Name} should use {attributeTypeName}.");
        }

        private static double GetDoubleProperty(object target, string propertyName)
        {
            object value = target.GetType().GetProperty(propertyName)?.GetValue(target)
                ?? throw new InvalidOperationException($"{propertyName} was not found.");
            return Convert.ToDouble(value);
        }

        private static double ToDouble(object value)
        {
            return Convert.ToDouble(value);
        }
    }
}
