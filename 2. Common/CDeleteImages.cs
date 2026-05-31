using ADOX;
using Lib.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using static OpenVisionLab.CPropertySocket;

namespace OpenVisionLab
{
    public class PropertyDelete
    {
        [CategoryAttribute("Delete Image"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "DeletImage";
        [CategoryAttribute("Delete Image"), DescriptionAttribute(""), DisplayNameAttribute("Image_Folder_Path")]
        public string ImageFolderPath { get; set; } = Application.StartupPath + "\\Image";
        [CategoryAttribute("Delete Image"), DescriptionAttribute(""), DisplayNameAttribute("Drive_Path")]
        public string DrivePath { get; set; } = "D";
        [CategoryAttribute("Delete Image"), DescriptionAttribute(""), DisplayNameAttribute("Drive_Volum")]
        public int DriveVolum { get; set; } = 80;
        [CategoryAttribute("Delete Image"), DescriptionAttribute(""), DisplayNameAttribute("Delete_Image_Day")]
        public int DeleteImageDay { get; set; } = 7;
        [CategoryAttribute("Delete Image"), DescriptionAttribute(""), DisplayNameAttribute("Use_Delete_Image")]
        public bool UseDeleteImage { get; set; } = false;

        [CategoryAttribute("Delete Log"), DescriptionAttribute(""), DisplayNameAttribute("Use_Delete_Log")]
        public bool UseDeleteLog { get; set; } = false;

        [CategoryAttribute("Delete Log"), DescriptionAttribute(""), DisplayNameAttribute("Delete_Log_Day")]
        public int DeleteLogDay { get; set; } = 90;       

        public PropertyDelete LoadConfig(string RecipeName)
        {
            string Path = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\" + NAME + ".xml";
            PropertyDelete newData = null;

            if (File.Exists(Path))
            {
                newData = SerializeHelper.FromXmlFile<PropertyDelete>(Path);
                if (newData != null)
                    return newData;
            }
            this.SaveConfig(RecipeName);
            return newData;
        }

        public void SaveConfig(string RecipeName)
        {
            string Path = Application.StartupPath + "\\RECIPE\\" + RecipeName + "\\" + NAME + ".xml";
            SerializeHelper.ToXmlFile(Path, this);
        }
    }


    public class CDeleteImages : CSeqBase
    {
        public EventHandler<EventArgs> EventDriveVolume;

        public PropertyDelete Property { get; set; } = new PropertyDelete();


        public CDeleteImages() 
        {
            this.Name = nameof(CDeleteImages);
            this.TimeMS = 5000;
        }

        public override void Run()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Property.ImageFolderPath);
            try
            {                
                if (EventDriveVolume != null) { EventDriveVolume(null, null); }
                DriveInfo drv = new DriveInfo(Property.DrivePath);
                double lTotalDrive = ((double)drv.AvailableFreeSpace / (double)drv.TotalSize) * 100;
                double dTotalDrive = 100 - lTotalDrive;

                string strDeleteLogPath = Application.StartupPath + "\\Log";
                DirectoryInfo directoryInfoLog = new DirectoryInfo(strDeleteLogPath);

                if (Property.UseDeleteLog) { DeleteLog(directoryInfoLog); }

                if (!Property.UseDeleteImage) { return; }
                if (dTotalDrive > Property.DriveVolum) { DeleteImageFile(directoryInfo); }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");                
            }
        }        
        private void DeleteLog(DirectoryInfo dir)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int nDeleteDay = Property.DeleteLogDay;
                DirectoryInfo[] existingFolders = dir.GetDirectories();

                int nIndex = -1;

                foreach (var existingFolder in existingFolders)
                {
                    nIndex++;
                    if (existingFolder == null)
                    {
                        continue;
                    }

                    if (existingFolder.Exists)
                    {
                        DeleteLog(existingFolder);

                        existingFolders[nIndex] = null;
                    }
                }

                if (dir.Exists)
                {
                    FileInfo[] files = dir.GetFiles();
                    foreach (var file in files)
                    {
                        DateTime dateTimeFile = DateTime.Parse(file.LastWriteTime.ToString());

                        TimeSpan TS = DateTime.Now - dateTimeFile;

                        int diffDay = TS.Days;

                        // 날짜 빼야함
                        if (diffDay >= nDeleteDay)
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(file.Name, ".log"))
                            {
                                string path = dir.FullName + "\\" + file.Name;
                                File.Delete(path);
                            }
                        }
                    }
                }

                sw.Restart();
            }
            catch (Exception Desc)
            {
                //ILogger.Add(LOG_TYPE.ERROR, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        /// <summary>
        /// 지정된 경로의 폴더를 검색하여 이미지를 삭제하는 메서드
        /// </summary>
        /// <param name="existingDir"></param>
        /// <param name="copyDir"></param>
        private void DeleteImageFile(DirectoryInfo dir)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                int DeleteDay = Property.DeleteImageDay;
                DirectoryInfo[] existingFolders = dir.GetDirectories();

                int nIndex = -1;

                foreach (var existingFolder in existingFolders)
                {
                    nIndex++;
                    if (existingFolder == null)
                    {
                        continue;
                    }

                    if (existingFolder.Exists)
                    {
                        DeleteImageFile(existingFolder);
                        existingFolders[nIndex] = null;

                        //break;
                    }
                }

                if (dir.Exists)
                {
                    FileInfo[] files = dir.GetFiles();
                    foreach (var file in files)
                    {
                        DateTime dateTimeFile = DateTime.Parse(file.LastWriteTime.ToString());

                        TimeSpan TS = DateTime.Now - dateTimeFile;

                        int diffDay = TS.Days;

                        DriveInfo drv = new DriveInfo(Property.DrivePath);
                        double lTotalDrive = ((double)drv.AvailableFreeSpace / (double)drv.TotalSize) * 100;
                        double dTotalDrive = 100 - lTotalDrive;

                        if (diffDay > DeleteDay || dTotalDrive > Property.DriveVolum)
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(file.Name, ".jpg") || System.Text.RegularExpressions.Regex.IsMatch(file.Name, ".bmp")
                                || System.Text.RegularExpressions.Regex.IsMatch(file.Name, ".png")
                                || System.Text.RegularExpressions.Regex.IsMatch(file.Name, ".Jpeg"))
                            {
                                string path = dir.FullName + "\\" + file.Name;
                                File.Delete(path);
                            }
                        }
                    }
                }
                CLOG.NORMAL( "[OK] Delete Time {0}", sw.Elapsed.TotalSeconds.ToString());
                sw.Restart();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
            }
        }

        public void SetDeletePath()
        {
            string Path = "";
            if(OpenFolderPath(out Path))
            {
                Property.ImageFolderPath = Path;
            }
        }

        private bool OpenFolderPath(out string dirPath)
        {
            dirPath = "";
            try
            {
                CLOG.NORMAL( $"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        dirPath = fbd.SelectedPath;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }
    }
}
