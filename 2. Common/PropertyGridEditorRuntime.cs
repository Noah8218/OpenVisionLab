using OpenVisionLab._1._Core;
using System;

namespace OpenVisionLab
{
    internal sealed class PropertyGridEditorRuntime
    {
        public IPropertyGridImageEditorService ImageEditorService { get; private set; }
        public Func<string> RecipeNameAccessor { get; private set; } = () => string.Empty;

        public PropertyGridEditorRuntime(IPropertyGridImageEditorService imageEditorService)
        {
            SetImageEditorService(imageEditorService);
        }

        public void SetRuntimeContext(Func<IDisplayManager> contextAccessor)
        {
            ImageEditorService.SetRuntimeContext(contextAccessor);
        }

        public void SetRecipeNameContext(Func<string> recipeNameAccessor)
        {
            RecipeNameAccessor = recipeNameAccessor ?? (() => string.Empty);
            ImageEditorService.SetRecipeNameContext(RecipeNameAccessor);
        }

        public void SetImageEditorService(IPropertyGridImageEditorService service)
        {
            ImageEditorService = service ?? new PropertyGridImageEditorService(() => DisplayManagerService.Default);
            ImageEditorService.SetRecipeNameContext(RecipeNameAccessor);
        }
    }
}
