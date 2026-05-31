using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    // 이미지 관리 클래스
    public static class CImageManager
    {
        // 이미지 삭제 클래스
        public static CDeleteImages DelImages { get; set; } = new CDeleteImages();
        // 이미지 저장 클래스
        public static CSaveImages SaveImages { get; set; } = new CSaveImages();

        //public static OpenCvSharp.Mat OriSrc = new OpenCvSharp.Mat();        

        public static void LoadConfig(string Name)
        {
           SaveImages.Property = SaveImages.Property.LoadConfig(Name);
           DelImages.Property = DelImages.Property.LoadConfig(Name);
        }

        public static void SaveConfig(string Name)
        {
            SaveImages.Property.SaveConfig(Name);
            DelImages.Property.SaveConfig(Name);
        }

        public static void StartThread()
        {
            SaveImages.StartThread();
            DelImages.StartThread();
        }

        public static void Dispose()
        {
            SaveImages.StopThread();
            DelImages.StopThread();                   
        }
    }
}
