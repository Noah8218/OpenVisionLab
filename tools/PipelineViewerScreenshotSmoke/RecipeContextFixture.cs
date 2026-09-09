using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;

internal static class RecipeContextFixture
{
    internal static VisionPipeline CreatePipeline(string name, int stepCount)
    {
        VisionPipeline pipeline = new() { Name = name };
        for (int index = 0; index < stepCount; index++)
        {
            pipeline.Steps.Add(new VisionPipelineStep
            {
                Name = $"{name}_Step_{index + 1}",
                ToolType = "Threshold",
                InputLayer = index == 0 ? "Main" : $"{name}_Preview_{index}",
                OutputLayer = $"{name}_Preview_{index + 1}"
            });
        }

        return pipeline;
    }
}
