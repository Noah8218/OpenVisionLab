using Lib.Common;
using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public class Images
    {
        public Bitmap BmpImage { get; set; } = new Bitmap(10, 10);
        public Mat MatImage { get; set; } = new Mat();
        public string ImagePath { get; set; } = Application.StartupPath + "\\Image";
        public ImageFormat imageFormat { get; set; } = ImageFormat.Jpeg;
        public ImageLibType Type { get; set; } = ImageLibType.BMP;

        public enum ImageLibType
        {
            BMP,
            MAT,
            MIL,
            EVISION,
            COGNEX
        }

        public Images(Bitmap Image, string Path, ImageFormat imageFormat)
        {
            this.BmpImage = Image;
            this.ImagePath = Path;
            this.imageFormat = imageFormat;
            Type = ImageLibType.BMP;
        }

        public Images(Mat Image, string Path, ImageFormat imageFormat)
        {
            this.MatImage = Image;
            this.ImagePath = Path;
            this.imageFormat = imageFormat;
            Type = ImageLibType.MAT;
        }
    }

    public class PropertySaves
    {
        [CategoryAttribute("Save Image"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "SaveImages";
        [CategoryAttribute("Save Image"), DescriptionAttribute(""), DisplayNameAttribute("Use_Save_OK")]
        public bool UseSaveOK { get; set; } = true;
        [CategoryAttribute("Save Image"), DescriptionAttribute(""), DisplayNameAttribute("Use_Save_NG")]
        public bool UseSaveNG { get; set; } = true;

        [CategoryAttribute("Save Image"), DescriptionAttribute(""), DisplayNameAttribute("Use_Save_Crop")]
        public bool UseSaveCrop { get; set; } = true;


        // 클래스 직렬화
        // 
        public PropertySaves LoadConfig(string RecipeName)
        {
            string Path = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\" + NAME + ".xml";
            PropertySaves newData = null;

            if (File.Exists(Path))
            {
                newData = SerializeHelper.FromXmlFile<PropertySaves>(Path);
                if (newData != null)
                    return newData;
            }
            this.SaveConfig(RecipeName);
            return newData = this.LoadConfig(RecipeName);
        }

        public void SaveConfig(string RecipeName)
        {
            string Path = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\" + NAME + ".xml";
            SerializeHelper.ToXmlFile(Path, this);
        }
    }

    public class CSaveImages : CSeqBase
    {
        public PropertySaves Property = new PropertySaves();

        public ConcurrentQueue<Images> Queues = new ConcurrentQueue<Images>();        

        public CSaveImages()
        {
            base.Name = nameof(CSaveImages);
            base.TimeMS = 1;
        }

        public override void Run()
        {
            if (Queues.TryDequeue(out Images Queue))
            {
                try
                {
                    switch(Queue.Type)
                    {
                        case Images.ImageLibType.MAT:
                            Cv2.ImWrite(Queue.ImagePath, Queue.MatImage);
                            Queue.MatImage.Dispose();
                            break;
                        case Images.ImageLibType.BMP:
                            Queue.BmpImage.Save(Queue.ImagePath, Queue.imageFormat);
                            Queue.BmpImage.Dispose();
                            break;
                    }
                }
                catch (Exception Desc)
                {
                    CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                }
            }            
        }
    }
}
