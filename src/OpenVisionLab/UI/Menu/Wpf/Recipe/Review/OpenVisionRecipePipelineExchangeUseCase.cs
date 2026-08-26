using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipePipelineExchangeUseCase
    {
        public OpenVisionRecipePipelineExchangeResult Import(string recipeName, string sourcePath)
        {
            if (!VisionPipelineStorage.TryLoadFromFile(sourcePath, out VisionPipeline pipeline, out string message))
            {
                return OpenVisionRecipePipelineExchangeResult.Failure(message);
            }

            string basePipelineName = string.IsNullOrWhiteSpace(pipeline.Name)
                ? Path.GetFileNameWithoutExtension(sourcePath)
                : pipeline.Name.Trim();
            if (!TryCreateUniquePipelineName(
                    recipeName,
                    basePipelineName,
                    out string uniquePipelineName,
                    out string nameError))
            {
                return OpenVisionRecipePipelineExchangeResult.Failure(nameError);
            }

            pipeline.Name = uniquePipelineName;
            VisionPipelineStorage.Save(recipeName, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);
            return OpenVisionRecipePipelineExchangeResult.Success(pipeline.Name, string.Empty);
        }

        public OpenVisionRecipePipelineExchangeResult Export(string recipeName, string activePipelineName, string destinationPath)
        {
            VisionPipeline pipeline = VisionPipelineStorage.Load(recipeName, activePipelineName);
            if (!VisionPipelineStorage.TrySaveToFile(destinationPath, pipeline, out string message))
            {
                return OpenVisionRecipePipelineExchangeResult.Failure(message);
            }

            return OpenVisionRecipePipelineExchangeResult.Success(activePipelineName, destinationPath);
        }

        public OpenVisionRecipePipelineExchangeResult ExportReviewBundle(
            string recipeName,
            string activePipelineName,
            string destinationPath,
            IReadOnlyList<OpenVisionRecipeReviewReference> references)
        {
            VisionPipeline pipeline = VisionPipelineStorage.Load(recipeName, activePipelineName);
            if (!OpenVisionRecipeReviewBundleExporter.TryExport(
                destinationPath,
                recipeName,
                activePipelineName,
                pipeline,
                SerializePipelineToXmlText(pipeline),
                references ?? Array.Empty<OpenVisionRecipeReviewReference>(),
                out string message))
            {
                return OpenVisionRecipePipelineExchangeResult.Failure(message);
            }

            return OpenVisionRecipePipelineExchangeResult.Success(activePipelineName, message);
        }

        private static string SerializePipelineToXmlText(VisionPipeline pipeline)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(VisionPipeline));
            using StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            serializer.Serialize(writer, pipeline);
            return writer.ToString();
        }

        private static bool TryCreateUniquePipelineName(
            string recipeName,
            string requestedBaseName,
            out string pipelineName,
            out string error)
        {
            if (!RecipeWorkspaceService.TryNormalizeStoragePathSegment(
                    requestedBaseName,
                    VisionPipelineAppendService.DefaultPipelineName,
                    "Pipeline name",
                    out string normalizedBaseName,
                    out error))
            {
                pipelineName = string.Empty;
                return false;
            }

            HashSet<string> names = RecipeWorkspaceService.GetVisionPipelineNames(recipeName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            string candidate = normalizedBaseName;
            int suffix = 2;
            while (names.Contains(candidate))
            {
                candidate = normalizedBaseName + "_" + suffix;
                suffix++;
            }

            pipelineName = candidate;
            error = string.Empty;
            return true;
        }
    }

    internal sealed class OpenVisionRecipePipelineExchangeResult
    {
        private OpenVisionRecipePipelineExchangeResult(bool succeeded, string pipelineName, string detail)
        {
            Succeeded = succeeded;
            PipelineName = pipelineName ?? string.Empty;
            Detail = detail ?? string.Empty;
        }

        public bool Succeeded { get; }
        public string PipelineName { get; }
        public string Detail { get; }

        public static OpenVisionRecipePipelineExchangeResult Success(string pipelineName, string detail)
        {
            return new OpenVisionRecipePipelineExchangeResult(true, pipelineName, detail);
        }

        public static OpenVisionRecipePipelineExchangeResult Failure(string detail)
        {
            return new OpenVisionRecipePipelineExchangeResult(false, string.Empty, detail);
        }
    }
}
