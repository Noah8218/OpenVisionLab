using OpenVisionLab.Core;
using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
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

        internal static readonly DependencyProperty EditActionCallbackProperty = DependencyProperty.RegisterAttached(
            "EditActionCallback", typeof(Action<string>), typeof(PropertyGridEditorFactory), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        internal static void SetEditActionCallback(DependencyObject owner, Action<string> value) => owner.SetValue(EditActionCallbackProperty, value);
        internal static Action<string> GetEditActionCallback(DependencyObject owner) => owner?.GetValue(EditActionCallbackProperty) as Action<string>;

        internal static readonly DependencyProperty ImageEditHostProperty = DependencyProperty.RegisterAttached("ImageEditHost", typeof(Func<IPropertyGridImageEditView, DependencyObject, bool>), typeof(PropertyGridEditorFactory), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        internal static readonly DependencyProperty ImageEditContextProperty = DependencyProperty.RegisterAttached("ImageEditContext", typeof(Func<bool>), typeof(PropertyGridEditorFactory), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        internal static void SetImageEditHost(DependencyObject owner, Func<IPropertyGridImageEditView, DependencyObject, bool> value) => owner.SetValue(ImageEditHostProperty, value);
        internal static void SetImageEditContext(DependencyObject owner, Func<bool> value) => owner.SetValue(ImageEditContextProperty, value);

        private static Func<IPropertyGridImageEditView, DependencyObject, bool> ResolveImageEditHost(DependencyObject caller)
        {
            var host = caller?.GetValue(ImageEditHostProperty) as Func<IPropertyGridImageEditView, DependencyObject, bool>;
            for (System.Windows.Window window = caller == null ? null : System.Windows.Window.GetWindow(caller); host == null && window != null; window = window.Owner)
            {
                host = window.GetValue(ImageEditHostProperty) as Func<IPropertyGridImageEditView, DependencyObject, bool>;
            }
            return host;
        }

        internal static bool ShowImageEdit(IPropertyGridImageEditView editor, DependencyObject caller)
        {
            var host = ResolveImageEditHost(caller);
            if (GetEditActionCallback(caller) == null || host == null) { return editor.ShowDialog(); }
            var current = caller.GetValue(ImageEditContextProperty) as Func<bool>;
            return current?.Invoke() != false && host(editor, caller) && current?.Invoke() != false;
        }

        private static readonly DependencyProperty ImageEditAutoScopeProperty = DependencyProperty.RegisterAttached("ImageEditAutoScope", typeof(ImageEditAutoScope), typeof(PropertyGridEditorFactory), new PropertyMetadata(null));

        internal static IDisposable BeginImageEditWriteScope(DependencyObject caller)
        {
            if (GetEditActionCallback(caller) == null || ResolveImageEditHost(caller) == null) { return null; }
            DependencyObject owner = caller;
            while (owner.ReadLocalValue(EditActionCallbackProperty) == DependencyProperty.UnsetValue)
            {
                DependencyObject parent = owner is FrameworkElement element ? element.Parent ?? System.Windows.Media.VisualTreeHelper.GetParent(element) : null;
                if (parent == null) { break; }
                owner = parent;
            }
            var state = owner.GetValue(ImageEditAutoScopeProperty) as ImageEditAutoScope;
            if (state == null || !state.MatchesCurrentContext()) { state = new ImageEditAutoScope(owner); owner.SetValue(ImageEditAutoScopeProperty, state); }
            state.Enter(); return new ImageEditAutoLease(state);
        }

        private sealed class ImageEditAutoLease : IDisposable
        {
            private ImageEditAutoScope state;
            internal ImageEditAutoLease(ImageEditAutoScope state) { this.state = state; }
            public void Dispose() { var value = state; state = null; value?.Leave(); }
        }

        private sealed class ImageEditAutoScope
        {
            private readonly DependencyObject owner;
            private readonly object originalLocal;
            private readonly System.Windows.Data.BindingBase originalBinding;
            private readonly bool originalAllow;
            private readonly Func<bool> originalContext;
            private int depth;
            internal ImageEditAutoScope(DependencyObject owner)
            {
                this.owner = owner; originalContext = owner.GetValue(ImageEditContextProperty) as Func<bool>; originalLocal = owner.ReadLocalValue(VisionToolDebouncedPreviewScheduler.AllowAutoPreviewProperty);
                originalBinding = System.Windows.Data.BindingOperations.GetBindingBase(owner, VisionToolDebouncedPreviewScheduler.AllowAutoPreviewProperty);
                originalAllow = VisionToolDebouncedPreviewScheduler.GetAllowAutoPreview(owner);
            }
            internal bool MatchesCurrentContext() => ReferenceEquals(originalContext, owner.GetValue(ImageEditContextProperty));
            internal void Enter()
            {
                if (depth++ == 0 && originalBinding != null) { System.Windows.Data.BindingOperations.ClearBinding(owner, VisionToolDebouncedPreviewScheduler.AllowAutoPreviewProperty); }
                owner.SetValue(VisionToolDebouncedPreviewScheduler.AllowAutoPreviewProperty, false); VisionToolDebouncedPreviewScheduler.CancelPending(owner);
            }
            internal void Leave()
            {
                if (--depth > 0) { return; }
                if (!ReferenceEquals(owner.GetValue(ImageEditAutoScopeProperty), this)) { return; }
                owner.ClearValue(ImageEditAutoScopeProperty);
                if (!MatchesCurrentContext() || originalContext?.Invoke() == false)
                {
                    if (owner.GetValue(ImageEditContextProperty) == null
                        && System.Windows.Data.BindingOperations.GetBindingBase(owner, VisionToolDebouncedPreviewScheduler.AllowAutoPreviewProperty) == null
                        && owner.ReadLocalValue(VisionToolDebouncedPreviewScheduler.AllowAutoPreviewProperty) is false)
                    {
                        owner.ClearValue(VisionToolDebouncedPreviewScheduler.AllowAutoPreviewProperty);
                    }
                    return;
                }
                VisionToolDebouncedPreviewScheduler.CancelPending(owner);
                if (originalLocal == DependencyProperty.UnsetValue) { owner.ClearValue(VisionToolDebouncedPreviewScheduler.AllowAutoPreviewProperty); }
                else if (originalBinding != null) { System.Windows.Data.BindingOperations.SetBinding(owner, VisionToolDebouncedPreviewScheduler.AllowAutoPreviewProperty, originalBinding); }
                else { owner.SetCurrentValue(VisionToolDebouncedPreviewScheduler.AllowAutoPreviewProperty, originalAllow); }
            }
        }

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

        internal static string SaveTemplateImageForTeaching(Mat sourceImage, OpenCvSharp.Rect selectedRegion)
        {
            return runtime.ImageEditorService.SaveTemplateImage(sourceImage, selectedRegion);
        }

        internal static string SelectTemplate(bool fromFile, string existingPath, System.Windows.Window owner = null, bool forWorkbench = false, Action<string> editAction = null, DependencyObject editOwner = null)
        {
            using IDisposable imageEditScope = fromFile ? null : BeginImageEditWriteScope(editOwner);
            try
            {
                if (fromFile)
                {
                    Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
                    {
                        Title = OpenVisionLanguageService.T("Workbench.Tool.Template"),
                        Filter = "BMP/PNG/JPEG/TIFF|*.bmp;*.png;*.jpg;*.jpeg;*.tif;*.tiff",
                        CheckFileExists = true,
                        Multiselect = false
                    };
                    bool? accepted = owner == null ? dialog.ShowDialog() : dialog.ShowDialog(owner);
                    return accepted == true ? dialog.FileName : null;
                }

                using Mat sourceImage = runtime.ImageEditorService.GetSourceImage();
                if (sourceImage == null || sourceImage.Empty())
                {
                    VisionMessageBox.Warning(forWorkbench ? OpenVisionLanguageService.T("Workbench.Tool.Template") : "Template registration",
                        forWorkbench ? OpenVisionLanguageService.T("Workbench.Viewer.NoImage") : "Load an image before registering a template.");
                    return null;
                }
                OpenCvSharp.Rect roi = runtime.ImageEditorService.LoadTemplateRoi(sourceImage, existingPath);
                using IPropertyGridImageEditView imageEdit = runtime.ImageEditorService.CreateImageEditView(sourceImage,
                    new Rectangle(roi.X, roi.Y, roi.Width, roi.Height), "TRAIN");
                if (owner != null && imageEdit is System.Windows.Window window) { window.Owner = owner; }
                if (imageEdit is IPropertyGridTemplateImageEditView templateImageEdit)
                {
                    templateImageEdit.TemplateRotationDegrees = runtime.ImageEditorService.LoadTemplateRotationDegrees(existingPath);
                }
                imageEdit.LoadPatternPreviewImage(existingPath);
                editAction?.Invoke("Workbench.Action.TemplateEditing");
                if (!ShowImageEdit(imageEdit, editOwner)) { editAction?.Invoke("Workbench.Action.RoiEditCanceled"); return null; }
                if (!HasValidSelectedRegion(imageEdit.SelectedRegion)) { editAction?.Invoke("Workbench.Status.Error"); return null; }
                double rotation = imageEdit is IPropertyGridTemplateImageEditView acceptedTemplateImageEdit
                    ? acceptedTemplateImageEdit.TemplateRotationDegrees : 0D;
                string path = runtime.ImageEditorService.SaveTemplateImage(sourceImage, imageEdit.SelectedRegion, rotation);
                if (string.IsNullOrWhiteSpace(path)) { editAction?.Invoke("Workbench.Status.Error"); }
                return path;
            }
            catch (Exception exception)
            {
                editAction?.Invoke("Workbench.Status.Error");
                VisionMessageBox.Error(forWorkbench ? OpenVisionLanguageService.T("Workbench.Tool.Template") : "Template registration",
                    forWorkbench ? OpenVisionLanguageService.T("Workbench.Status.Error") : "Cannot open the template editor.", exception.Message);
                return null;
            }
        }

        private static void LogEditorException(string editorName, Exception exception)
        {
            try
            {
                string directory = Path.Combine(
                    AppPathService.LogRootDirectory,
                    "Diagnostics");
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
                    : base(member.GetType(), $"[{index}]", typeof(string))//������[0]��ᶪ�ɪ��󪹪�
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
            // wpf �����̴� value�� �⺻.. double��
            // ������ int���̸� �۵� ����..
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
            // wpf �����̴� value�� �⺻.. double��
            // ������ int���̸� �۵� ����..
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
                if (propertyValue == null || propertyValue.ParentProperty.IsReadOnly) { return; }
                Action<string> editAction = GetEditActionCallback(commandSource as DependencyObject);
                using IDisposable imageEditScope = BeginImageEditWriteScope(commandSource as DependencyObject);
                string path = SelectTemplate(false, propertyValue.StringValue, editAction: editAction, editOwner: commandSource as DependencyObject);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    try { propertyValue.StringValue = path; editAction?.Invoke("Workbench.Action.TemplateRegistered"); }
                    catch { editAction?.Invoke("Workbench.Status.Error"); throw; }
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

                Action<string> editAction = GetEditActionCallback(commandSource as DependencyObject);
                using IDisposable imageEditScope = BeginImageEditWriteScope(commandSource as DependencyObject);
                try
                {
                    OpenCvSharp.Rect rect = propertyValue.Value is OpenCvSharp.Rect typedRect ? typedRect : new OpenCvSharp.Rect();

                    using Mat sourceImage = runtime.ImageEditorService.GetSourceImage();
                    if (sourceImage == null || sourceImage.Empty())
                    {
                        VisionMessageBox.Warning("ROI registration", "Load an image before editing ROI.");
                        return;
                    }

                    rect = NormalizeRectForImage(rect, sourceImage);
                    Rectangle r = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                    using IPropertyGridImageEditView imageEdit = runtime.ImageEditorService.CreateImageEditView(sourceImage, r, "ROI");
                    editAction?.Invoke("Workbench.Action.RoiEditing");
                    if (ShowImageEdit(imageEdit, commandSource as DependencyObject))
                    {
                        OpenCvSharp.Rect selectedRegion = NormalizeRectForImage(imageEdit.SelectedRegion, sourceImage);
                        if (!HasValidSelectedRegion(selectedRegion)) { editAction?.Invoke("Workbench.Status.Error"); return; }

                        propertyValue.Value = selectedRegion;
                        editAction?.Invoke("Workbench.Action.RoiApplied");
                    }
                    else { editAction?.Invoke("Workbench.Action.RoiEditCanceled"); }
                }
                catch (Exception ex)
                {
                    editAction?.Invoke("Workbench.Status.Error");
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

                Action<string> editAction = GetEditActionCallback(commandSource as DependencyObject);
                using IDisposable imageEditScope = BeginImageEditWriteScope(commandSource as DependencyObject);
                try
                {
                    using Mat sourceImage = runtime.ImageEditorService.GetSourceImage();
                    if (sourceImage == null || sourceImage.Empty())
                    {
                        VisionMessageBox.Warning("Mask registration", "Load an image before editing masks.");
                        return;
                    }

                    List<OpenCvSharp.Rect> regions = propertyValue.Value as List<OpenCvSharp.Rect> ?? new List<OpenCvSharp.Rect>();
                    using IPropertyGridImageEditView imageEdit = runtime.ImageEditorService.CreateImageEditView(sourceImage, regions, "MULTI_ROI");
                    editAction?.Invoke("Workbench.Action.RoiEditing");
                    if (ShowImageEdit(imageEdit, commandSource as DependencyObject))
                    {
                        propertyValue.Value = imageEdit.SelectedRegions;
                        editAction?.Invoke("Workbench.Action.RoiApplied");
                    }
                    else { editAction?.Invoke("Workbench.Action.RoiEditCanceled"); }
                }
                catch (Exception ex)
                {
                    editAction?.Invoke("Workbench.Status.Error");
                    VisionMessageBox.Error("Mask registration", "Cannot open the masking editor.", ex.Message);
                }
            }
        }
    }
}
