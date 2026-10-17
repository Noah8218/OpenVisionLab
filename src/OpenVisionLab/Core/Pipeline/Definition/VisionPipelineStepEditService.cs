using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineStepEditResult
    {
        public VisionPipelineStepEditResult(VisionPipeline pipeline, VisionPipelineValidationResult validation, int affectedIndex, int selectedIndex)
        {
            Pipeline = pipeline;
            Validation = validation;
            AffectedIndex = affectedIndex;
            SelectedIndex = selectedIndex;
        }

        public VisionPipeline Pipeline { get; }
        public VisionPipelineValidationResult Validation { get; }
        public int AffectedIndex { get; }
        public int SelectedIndex { get; }
    }

    internal static class VisionPipelineStepEditService
    {
        public static VisionPipelineStepEditResult Insert(VisionPipeline pipeline, int index, VisionPipelineStep step, IEnumerable<string> sourceLayers = null)
        {
            if (step == null) { throw new ArgumentNullException(nameof(step)); }
            VisionPipeline copy = Copy(pipeline);
            if (index < 0 || index > copy.Steps.Count) { throw new ArgumentOutOfRangeException(nameof(index)); }
            VisionPipelineStep inserted = CopyValue(step);
            VisionPipelineAppendService.EnsureUniqueStepName(copy, inserted);
            copy.Steps.Insert(index, inserted);
            return Complete(pipeline, copy, sourceLayers, index, index);
        }

        public static VisionPipelineStepEditResult Move(VisionPipeline pipeline, int index, int targetIndex, IEnumerable<string> sourceLayers = null)
        {
            VisionPipeline copy = Copy(pipeline);
            RequireIndex(copy, index);
            RequireIndex(copy, targetIndex);
            VisionPipelineStep step = copy.Steps[index];
            copy.Steps.RemoveAt(index);
            copy.Steps.Insert(targetIndex, step);
            return Complete(pipeline, copy, sourceLayers, Math.Min(index, targetIndex), targetIndex);
        }

        public static VisionPipelineStepEditResult Delete(VisionPipeline pipeline, int index, IEnumerable<string> sourceLayers = null)
        {
            VisionPipeline copy = Copy(pipeline);
            RequireIndex(copy, index);
            copy.Steps.RemoveAt(index);
            return Complete(pipeline, copy, sourceLayers, index, Math.Min(index, copy.Steps.Count - 1));
        }

        public static VisionPipelineStepEditResult Duplicate(VisionPipeline pipeline, int index, IEnumerable<string> sourceLayers = null)
        {
            VisionPipeline copy = Copy(pipeline);
            RequireIndex(copy, index);
            VisionPipelineStep duplicated = CopyValue(copy.Steps[index]);
            VisionPipelineAppendService.EnsureUniqueStepName(copy, duplicated);
            copy.Steps.Insert(index + 1, duplicated);
            return Complete(pipeline, copy, sourceLayers, index + 1, index + 1);
        }

        public static VisionPipelineStepEditResult Rename(VisionPipeline pipeline, int index, string name, IEnumerable<string> sourceLayers = null)
        {
            VisionPipeline copy = Copy(pipeline);
            RequireIndex(copy, index);
            VisionPipelineStep step = copy.Steps[index];
            copy.Steps.RemoveAt(index);
            step.Name = name;
            VisionPipelineAppendService.EnsureUniqueStepName(copy, step);
            copy.Steps.Insert(index, step);
            return Complete(pipeline, copy, sourceLayers, index, index);
        }

        public static VisionPipelineStepEditResult SetEnabled(VisionPipeline pipeline, int index, bool enabled, IEnumerable<string> sourceLayers = null)
        {
            VisionPipeline copy = Copy(pipeline);
            RequireIndex(copy, index);
            copy.Steps[index].Enabled = enabled;
            return Complete(pipeline, copy, sourceLayers, index, index);
        }

        public static VisionPipelineStepEditResult Replace(VisionPipeline pipeline, int index, VisionPipelineStep step, IEnumerable<string> sourceLayers = null)
        {
            if (step == null) { throw new ArgumentNullException(nameof(step)); }
            VisionPipeline copy = Copy(pipeline);
            RequireIndex(copy, index);
            copy.Steps[index] = CopyStep(step);
            return Complete(pipeline, copy, sourceLayers, index, index);
        }

        public static VisionPipelineStep CopyStep(VisionPipelineStep step) => step == null ? null : CopyValue(step);

        public static VisionPipeline Copy(VisionPipeline pipeline)
        {
            if (pipeline == null) { throw new ArgumentNullException(nameof(pipeline)); }
            return CopyValue(pipeline);
        }

        public static VisionPipelineValidationResult Validate(VisionPipeline pipeline, IEnumerable<string> sourceLayers = null)
        {
            HashSet<string> outputs = new HashSet<string>(
                (pipeline?.Steps ?? new List<VisionPipelineStep>()).Where(step => step?.Enabled == true)
                    .Select(step => step.OutputLayer).Where(output => !string.IsNullOrWhiteSpace(output)),
                StringComparer.OrdinalIgnoreCase);
            IEnumerable<string> inputs = (sourceLayers ?? new[] { VisionRecipeRunner.DefaultInputLayer })
                .Where(layer => string.Equals(layer, VisionRecipeRunner.DefaultInputLayer, StringComparison.OrdinalIgnoreCase) || !outputs.Contains(layer));
            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, inputs);
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int index = 0; index < (pipeline?.Steps?.Count ?? 0); index++)
            {
                VisionPipelineStep step = pipeline.Steps[index];
                if (step?.Enabled != true || string.IsNullOrWhiteSpace(step.OutputLayer)) { continue; }
                string label = $"Step {index + 1} '{step.Name}'";
                if (string.Equals(step.OutputLayer.Trim(), VisionRecipeRunner.DefaultInputLayer, StringComparison.OrdinalIgnoreCase))
                {
                    validation.Errors.Add($"{label}: output layer must not be Main.");
                }
                else if (!seen.Add(step.OutputLayer))
                {
                    validation.Errors.Add($"{label}: output layer '{step.OutputLayer}' duplicates an earlier step.");
                }
            }

            return validation;
        }

        private static VisionPipelineStepEditResult Complete(VisionPipeline original, VisionPipeline copy, IEnumerable<string> sourceLayers, int affectedIndex, int selectedIndex)
        {
            HashSet<string> originalOutputs = new HashSet<string>(original.Steps.Where(step => step != null)
                .Select(step => step.OutputLayer).Where(output => !string.IsNullOrWhiteSpace(output)), StringComparer.OrdinalIgnoreCase);
            IEnumerable<string> inputs = (sourceLayers ?? new[] { VisionRecipeRunner.DefaultInputLayer })
                .Where(layer => string.Equals(layer, VisionRecipeRunner.DefaultInputLayer, StringComparison.OrdinalIgnoreCase) || !originalOutputs.Contains(layer));
            return new VisionPipelineStepEditResult(copy, Validate(copy, inputs), affectedIndex, selectedIndex);
        }

        private static void RequireIndex(VisionPipeline pipeline, int index)
        {
            if (index < 0 || index >= pipeline.Steps.Count) { throw new ArgumentOutOfRangeException(nameof(index)); }
            if (pipeline.Steps[index] == null) { throw new InvalidOperationException("The selected step is null."); }
        }

        private static T CopyValue<T>(T value)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, value);
            stream.Position = 0;
            return (T)serializer.Deserialize(stream);
        }
    }
}
