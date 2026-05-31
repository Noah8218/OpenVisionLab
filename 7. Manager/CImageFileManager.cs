using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KtemVisionSystem
{
    // 이미지 관리 클래스
    public class CImageFileManager: IDisposable
    {
        public CDeleteImages DelImages { get; set; } = new CDeleteImages();
        public CSaveImages SaveImages { get; set; } = new CSaveImages();

        public void LoadConfig(string Name)
        {
           SaveImages.Property = SaveImages.Property.LoadConfig(Name);
           DelImages.Property = DelImages.Property.LoadConfig(Name);
        }

        public void SaveConfig(string Name)
        {
            SaveImages.Property.SaveConfig(Name);
            DelImages.Property.SaveConfig(Name);
        }

        public CImageFileManager()
        {
            SaveImages.StartThreadSaveImages();
            DelImages.StartThreadDeleteImage();
        }

        public void Dispose()
        {
            SaveImages.StopThreadSaveImages();
            DelImages.StopThreadDeleteImage();                   
        }
    }
}
