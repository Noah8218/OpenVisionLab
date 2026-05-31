using OpenVisionLab._1._Core;
using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using Lib.Common;
using System.Windows.Controls.WpfPropertyGrid;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Controls.WpfPropertyGrid.Controls;

namespace OpenVisionLab
{
    public static class CPropertyGridEditor
    {
        public static void ChangeBrowsability(object pThis, string pProperty, bool pBrowsable)
        {
            PropertyDescriptor pdDescriptor = TypeDescriptor.GetProperties(pThis.GetType())[pProperty];
            BrowsableAttribute baAttribute = (BrowsableAttribute)pdDescriptor.Attributes[typeof(BrowsableAttribute)];
            FieldInfo fiBrowsable = baAttribute.GetType().GetField("browsable", BindingFlags.NonPublic | BindingFlags.Instance);
            fiBrowsable.SetValue(baAttribute, pBrowsable);
        }

        public class PathEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }
            public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = System.Windows.Forms.Application.StartupPath + "\\RECIPE\\" + CGlobal.Inst.Recipe.Name + "\\Template";
                string strFilePath = "";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    strFilePath = ofd.FileName;
                    CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                    value = strFilePath;
                }

                return value; // can also replace the wrapper object here
            }
        }

        public class ROIEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }
            public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
            {
                OpenCvSharp.Rect rect = (OpenCvSharp.Rect)value;
                Rectangle r = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

                IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                FormImageEditView FrmImageEdit = new FormImageEditView(Lib.Common.CImageConverter.ToBitmap(CDisplayManager.ImageSrc), r, "ROI");
                if (FrmImageEdit.ShowDialog() == DialogResult.OK)
                {
                    value = FrmImageEdit.SelectedRegion;
                }

                return value; // can also replace the wrapper object here
            }
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

        public class MULTIROIEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }
            public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                FormImageEditView FrmImageEdit = new FormImageEditView(Lib.Common.CImageConverter.ToBitmap(CDisplayManager.ImageSrc), (List<OpenCvSharp.Rect>)value, "MULTI_ROI");
                if (FrmImageEdit.ShowDialog() == DialogResult.OK)
                {
                    value = FrmImageEdit.SelectedRegions;
                }

                return value; // can also replace the wrapper object here
            }

            //public MULTIROIEditor(Type type) : base(type) { }

            //public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            //{
            //    object result = base.EditValue(context, provider, value);

            //                IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            //    FormImageEditView FrmImageEdit = new FormImageEditView(Lib.Common.CImageConverter.ToBitmap(CDisplayManager.ImageSrc), (List<Rect>)value, "MULTI_ROI");
            //    if (FrmImageEdit.ShowDialog() == DialogResult.OK)
            //    {
            //        value = FrmImageEdit.SelectedRegion;
            //    }

            //    // assign the temporary collection from the UI to the property
            //    //((ClassContainingStuffProperty)context.Instance).Stuff = (List<SomeType>)result;

            //    return result;
            //}
        }

        public class MatchEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }
            public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                FormImageEditView FrmImageEdit = new FormImageEditView(Lib.Common.CImageConverter.ToBitmap(CDisplayManager.ImageSrc), new Rectangle(), "TRAIN");
                if (FrmImageEdit.ShowDialog() == DialogResult.OK)
                {
                    CUtil.InitDirectory($"RECIPE\\{CGlobal.Inst.Recipe.Name}\\PATTERN");
                    string Path = System.Windows.Forms.Application.StartupPath + $"\\RECIPE\\{CGlobal.Inst.Recipe.Name}\\PATTERN\\{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.bmp";
                    Mat ImageTemplate = CDisplayManager.ImageSrc.SubMat(FrmImageEdit.SelectedRegion).Clone();
                    Cv2.ImWrite(Path, ImageTemplate);

                    value = Path;
                }

                return value; // can also replace the wrapper object here
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

                FormImageEditView FrmImageEdit = new FormImageEditView(Lib.Common.CImageConverter.ToBitmap(CDisplayManager.ImageSrc), new Rectangle(), "TRAIN");
                if (FrmImageEdit.ShowDialog() == DialogResult.OK)
                {
                    CUtil.InitDirectory($"RECIPE\\{CGlobal.Inst.Recipe.Name}\\PATTERN");
                    string Path = System.Windows.Forms.Application.StartupPath + $"\\RECIPE\\{CGlobal.Inst.Recipe.Name}\\PATTERN\\{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.bmp";
                    Mat ImageTemplate = CDisplayManager.ImageSrc.SubMat(FrmImageEdit.SelectedRegion).Clone();
                    Cv2.ImWrite(Path, ImageTemplate);

                    propertyValue.StringValue = Path;
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

                OpenCvSharp.Rect rect = (OpenCvSharp.Rect)propertyValue.Value;
                Rectangle r = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                
                FormImageEditView FrmImageEdit = new FormImageEditView(Lib.Common.CImageConverter.ToBitmap(CDisplayManager.ImageSrc), r, "ROI");
                if (FrmImageEdit.ShowDialog() == DialogResult.OK)
                {
                    propertyValue.Value = FrmImageEdit.SelectedRegion;
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

                FormImageEditView FrmImageEdit = new FormImageEditView(Lib.Common.CImageConverter.ToBitmap(CDisplayManager.ImageSrc), (List<OpenCvSharp.Rect>)propertyValue.Value, "MULTI_ROI");
                if (FrmImageEdit.ShowDialog() == DialogResult.OK)
                {
                    propertyValue.Value = FrmImageEdit.SelectedRegions;
                }                
            }
        }
    }
}
