using OpenVisionLab._1._Core;
using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using Lib.Common;
using OpenVisionLab.MessageDialogs;
using System.IO;
using System.Text;
using System.Windows.Controls.WpfPropertyGrid;
using System.Windows;
using System.Windows.Controls.WpfPropertyGrid.Controls;

namespace OpenVisionLab
{
    public static class PropertyGridEditorFactory
    {
        private static readonly PropertyGridEditorRuntime runtime = new PropertyGridEditorRuntime(new PropertyGridImageEditorService(() => DisplayManagerService.Default));

        public static IDisplayManager RuntimeContext
        {
            get => runtime.ImageEditorService.RuntimeContext;
            set => runtime.ImageEditorService.RuntimeContext = value;
        }

        public static void SetRuntimeContext(Func<IDisplayManager> contextAccessor)
        {
            runtime.SetRuntimeContext(contextAccessor);
        }

        public static void SetRecipeNameContext(Func<string> recipeNameAccessor)
        {
            runtime.SetRecipeNameContext(recipeNameAccessor);
        }

        public static void SetSourceLayerContext(Func<string> sourceLayerNameAccessor)
        {
            runtime.SetSourceLayerContext(sourceLayerNameAccessor);
        }

        public static string GetRecipeName()
        {
            return runtime.RecipeNameAccessor?.Invoke() ?? string.Empty;
        }

        internal static void SetImageEditorService(IPropertyGridImageEditorService service)
        {
            runtime.SetImageEditorService(service);
        }

        private static void LogEditorException(string editorName, Exception exception)
        {
            try
            {
                string directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "OpenVisionLab",
                    "Logs");
                Directory.CreateDirectory(directory);
                File.AppendAllText(
                    Path.Combine(directory, "property-grid-editors.log"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    + " ["
                    + editorName
                    + "] "
                    + exception
                    + Environment.NewLine,
                    Encoding.UTF8);
            }
            catch
            {
            }
        }

        private static bool HasValidSelectedRegion(OpenCvSharp.Rect selectedRegion)
        {
            return selectedRegion.Width > 0 && selectedRegion.Height > 0;
        }

        private static OpenCvSharp.Rect NormalizeRectForImage(OpenCvSharp.Rect rect, Mat sourceImage)
        {
            if (sourceImage == null || sourceImage.Empty())
            {
                return new OpenCvSharp.Rect();
            }

            if (!HasValidSelectedRegion(rect))
            {
                return new OpenCvSharp.Rect(0, 0, Math.Max(1, sourceImage.Width), Math.Max(1, sourceImage.Height));
            }

            int x = Math.Max(0, rect.X);
            int y = Math.Max(0, rect.Y);
            int right = Math.Min(sourceImage.Width, rect.X + rect.Width);
            int bottom = Math.Min(sourceImage.Height, rect.Y + rect.Height);
            int width = Math.Max(0, right - x);
            int height = Math.Max(0, bottom - y);
            return width > 0 && height > 0 ? new OpenCvSharp.Rect(x, y, width, height) : new OpenCvSharp.Rect(0, 0, sourceImage.Width, sourceImage.Height);
        }

        public static void ChangeBrowsability(object pThis, string pProperty, bool pBrowsable)
        {
            PropertyDescriptor pdDescriptor = TypeDescriptor.GetProperties(pThis.GetType())[pProperty];
            BrowsableAttribute baAttribute = (BrowsableAttribute)pdDescriptor.Attributes[typeof(BrowsableAttribute)];
            FieldInfo fiBrowsable = baAttribute.GetType().GetField("browsable", BindingFlags.NonPublic | BindingFlags.Instance);
            fiBrowsable.SetValue(baAttribute, pBrowsable);
        }

        public class ListTypeConverter : TypeConverter
        {
            public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return true; }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                List<PropertyDescriptor> list = new List<PropertyDescriptor>();
                IEnumerable members = value as IEnumerable;
                if (members != null)
                {
                    foreach (var member in members)
                    {
                        list.Add(new MemberDescriptor(member, list.Count));
                    }
                }
                return new PropertyDescriptorCollection(list.ToArray());
            }

            private class MemberDescriptor : SimplePropertyDescriptor
            {
                public object Member { get; private set; }
                public MemberDescriptor(object member, int index)
                    : base(member.GetType(), $"[{index}]", typeof(string))//展開時[0]の所をどう出すか
                {
                    Member = member;
                }
                public override object GetValue(object component)
                {
                    return Member;
                }
                public override void SetValue(object component, object value)
                {
                    Member = value;
                }
            }
        }

        public class WpgSliderEditor : Editor
        {
            // wpf 슬라이더 value가 기본.. double임 
            // 변수가 int형이면 작동 안함..
            public WpgSliderEditor()
            {
                if (this.InlineTemplate == null) { this.InlineTemplate = EditorKeys.SliderEditorKey; }                
            }
        }

        public class WpgThresholdEditor : Editor
        {
            public WpgThresholdEditor()
            {
                if (this.InlineTemplate == null) { this.InlineTemplate = EditorKeys.ThresholdEditorKey; }
            }
        }

        public class WpgRangeEditor : Editor
        {
            public WpgRangeEditor()
            {
                if (this.InlineTemplate == null) { this.InlineTemplate = EditorKeys.RangeEditorKey; }
            }
        }

        public class WpgMetricRangeEditor : Editor
        {
            public WpgMetricRangeEditor()
            {
                if (this.InlineTemplate == null) { this.InlineTemplate = EditorKeys.MetricRangeEditorKey; }
            }
        }

        public class WpgDoubleEditor : Editor
        {
            // wpf 슬라이더 value가 기본.. double임 
            // 변수가 int형이면 작동 안함..
            public WpgDoubleEditor()
            {
                if (this.InlineTemplate == null) { this.InlineTemplate = EditorKeys.DoubleEditorKey; }
            }
        }

        public class WpgColorEditor : PropertyEditor
        {
            public WpgColorEditor()
            {
                if (this.InlineTemplate == null) { this.InlineTemplate = EditorKeys.BrushEditorKey; }
            }
        }

        public class WpgMatchEditor : PropertyEditor
        {
            public WpgMatchEditor()
            {
                if (this.InlineTemplate == null) { this.InlineTemplate = EditorKeys.FilePathPickerEditorKey; }
            }

            public override void ShowDialog(PropertyItemValue propertyValue, IInputElement commandSource)
            {
                if (propertyValue == null) return;
                if (propertyValue.ParentProperty.IsReadOnly) return;

                try
                {
                    Mat sourceImage = runtime.ImageEditorService.GetSourceImage();
                    if (sourceImage == null || sourceImage.Empty())
                    {
                        VisionMessageBox.Warning("Template registration", "Load an image before registering a template.");
                        return;
                    }

                    string existingPatternPath = propertyValue.StringValue;
                    OpenCvSharp.Rect templateRoi = runtime.ImageEditorService.LoadTemplateRoi(sourceImage, existingPatternPath);
                    using IPropertyGridImageEditView imageEdit = runtime.ImageEditorService.CreateImageEditView(sourceImage, new Rectangle(templateRoi.X, templateRoi.Y, templateRoi.Width, templateRoi.Height), "TRAIN");
                    if (imageEdit is IPropertyGridTemplateImageEditView templateImageEdit)
                    {
                        templateImageEdit.TemplateRotationDegrees = runtime.ImageEditorService.LoadTemplateRotationDegrees(existingPatternPath);
                    }

                    imageEdit.LoadPatternPreviewImage(existingPatternPath);
                    if (imageEdit.ShowDialog())
                    {
                        if (!HasValidSelectedRegion(imageEdit.SelectedRegion)) { return; }

                        double rotationDegrees = imageEdit is IPropertyGridTemplateImageEditView acceptedTemplateImageEdit
                            ? acceptedTemplateImageEdit.TemplateRotationDegrees
                            : 0D;
                        string Path = runtime.ImageEditorService.SaveTemplateImage(sourceImage, imageEdit.SelectedRegion, rotationDegrees);
                        if (string.IsNullOrWhiteSpace(Path)) { return; }
                        propertyValue.StringValue = Path;
                    }
                }
                catch (Exception ex)
                {
                    VisionMessageBox.Error("Template registration", "Cannot open the template editor.", ex.Message);
                }
            }
        }

        public class WpgROIEditor : PropertyEditor
        {
            public WpgROIEditor()
            {
                if (this.InlineTemplate == null) { this.InlineTemplate = EditorKeys.FilePathPickerEditorKey; }
            }

            public override void ShowDialog(PropertyItemValue propertyValue, IInputElement commandSource)
            {
                if (propertyValue == null) return;
                if (propertyValue.ParentProperty.IsReadOnly) return;

                try
                {
                    OpenCvSharp.Rect rect = propertyValue.Value is OpenCvSharp.Rect typedRect ? typedRect : new OpenCvSharp.Rect();

                    Mat sourceImage = runtime.ImageEditorService.GetSourceImage();
                    if (sourceImage == null || sourceImage.Empty())
                    {
                        VisionMessageBox.Warning("ROI registration", "Load an image before editing ROI.");
                        return;
                    }

                    rect = NormalizeRectForImage(rect, sourceImage);
                    Rectangle r = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                    using IPropertyGridImageEditView imageEdit = runtime.ImageEditorService.CreateImageEditView(sourceImage, r, "ROI");
                    if (imageEdit.ShowDialog())
                    {
                        OpenCvSharp.Rect selectedRegion = NormalizeRectForImage(imageEdit.SelectedRegion, sourceImage);
                        if (!HasValidSelectedRegion(selectedRegion)) { return; }

                        propertyValue.Value = selectedRegion;
                    }
                }
                catch (Exception ex)
                {
                    LogEditorException("ROI", ex);
                    VisionMessageBox.Warning("ROI registration", "ROI editor could not be opened. " + ex.GetBaseException().Message);
                }
            }
        }

        public class WpgMultiROIEditor : PropertyEditor
        {
            public WpgMultiROIEditor()
            {
                if (this.InlineTemplate == null) { this.InlineTemplate = EditorKeys.ComplexPropertyEditorKey; }
            }

            public override void ShowDialog(PropertyItemValue propertyValue, IInputElement commandSource)
            {
                if (propertyValue == null) return;
                if (propertyValue.ParentProperty.IsReadOnly) return;

                try
                {
                    Mat sourceImage = runtime.ImageEditorService.GetSourceImage();
                    if (sourceImage == null || sourceImage.Empty())
                    {
                        VisionMessageBox.Warning("Mask registration", "Load an image before editing masks.");
                        return;
                    }

                    List<OpenCvSharp.Rect> regions = propertyValue.Value as List<OpenCvSharp.Rect> ?? new List<OpenCvSharp.Rect>();
                    using IPropertyGridImageEditView imageEdit = runtime.ImageEditorService.CreateImageEditView(sourceImage, regions, "MULTI_ROI");
                    if (imageEdit.ShowDialog())
                    {
                        propertyValue.Value = imageEdit.SelectedRegions;
                    }
                }
                catch (Exception ex)
                {
                    VisionMessageBox.Error("Mask registration", "Cannot open the masking editor.", ex.Message);
                }
            }
        }
    }
}
