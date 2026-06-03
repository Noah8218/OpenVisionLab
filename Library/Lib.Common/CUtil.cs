using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Lib.Common
{
    public class CUtil
    {
        [DllImport("gdi32.dll")] public static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);
        [DllImport("user32.dll")] public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SetSystemTime([In] SystemTime st);

        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        /// <summary>
        /// 사용 예 => ParseEnum<(enum 타입)>(cbType.SelectedItem.ToString())
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static bool LoadFolderPath(out string strdirPath)
        {
            strdirPath = "";
            try
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        strdirPath = fbd.SelectedPath;
                    }
                }

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");                
                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");                
                return false;
            }
        }

        public static string LoadImageFilePath()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = Application.StartupPath;
                ofd.Filter = "Images Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string strFilePath = ofd.FileName;
                    CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                    return strFilePath;
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return "";
            }

            return "";
        }

        public static string[] LoadImagesFilePath()
        {
            string[] Images = null;
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = Application.StartupPath;
                ofd.Filter = "Images Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Images = ofd.FileNames;
                    CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                    return Images;
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return Images;
            }

            return Images;
        }

        public static string LoadFilePath()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = Application.StartupPath;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string strFilePath = ofd.FileName;
                    CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                    return strFilePath;
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return "";
            }

            return "";
        }

        public static string SaveImageFilePath()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.InitialDirectory = Application.StartupPath;
                sfd.Filter = "Images Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string strFilePath = sfd.FileName;
                    CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
                    return strFilePath;
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return "";
            }

            return "";
        }

        /// <summary>
        /// 원본/대상 폴더의 파일들을 비교하여 데이터를 backup합니다.
        /// </summary>
        /// <param name="existingDir"></param>
        /// <param name="copyDir"></param>
        public static void SynchFolder(DirectoryInfo existingDir, DirectoryInfo copyDir)
        {
            try
            {
                // 각각의 폴더에 있는 파일을 얻습니다.
                FileInfo[] existingFiles = existingDir.GetFiles(); // 원본
                FileInfo[] copyFiles = copyDir.GetFiles(); // 대상 파일

                bool findFile = false;
                int nIndex = 0;

                #region 파일 비교
                foreach (var existingFile in existingFiles)
                {
                    findFile = false;
                    nIndex = -1;
                    foreach (var copyFile in copyFiles)
                    {
                        nIndex++;

                        if (copyFile == null)
                        {
                            continue;
                        }

                        // 두 파일의 이름이 같다면
                        if (existingFile.Name == copyFile.Name)
                        {
                            findFile = true;

                            // 두 파일의 마지막 쓰기 시간이 틀리다면
                            if (existingFile.LastWriteTime != copyFile.LastWriteTime)
                            {
                                try
                                {
                                    if (existingFile.LastWriteTime > copyFile.LastWriteTime)
                                    {
                                        File.Copy(existingFile.FullName, copyFile.FullName, true);
                                    }
                                }
                                catch (Exception Desc)
                                {
                                    Console.WriteLine(Desc.Message);
                                }

                                copyFiles[nIndex] = null;

                                break;
                            }
                        }
                    }

                    // 원본에는 있는데, 대상 폴더에 없는 경우에는 무조건 복사
                    if (!findFile)
                    {
                        try
                        {
                            String path = copyDir.FullName + "\\" + existingFile.Name;
                            existingFile.CopyTo(path);
                        }
                        catch (Exception Desc)
                        {
                            Console.WriteLine(Desc.Message);
                        }
                    }
                }
                #endregion

                #region 폴더 비교
                DirectoryInfo[] existingFolders = existingDir.GetDirectories();
                DirectoryInfo[] copyFolders = copyDir.GetDirectories();

                foreach (var existingFolder in existingFolders)
                {
                    findFile = false;
                    nIndex = -1;

                    foreach (var copyFolder in copyFolders)
                    {
                        nIndex++;

                        if (copyFolder == null)
                        {
                            continue;
                        }

                        // 폴더가 있다면
                        if (existingFolder.Name == copyFolder.Name)
                        {
                            findFile = true;

                            // 재귀함수를 호출하여 폴더안에 폴더를 검사
                            // 재귀함수이기에 첫번째부터 진행하였던 파일들을 다시 검사
                            // 매개변수는 foreach문으로 처음에 가져왔던 폴더들로 다시 진행
                            SynchFolder(existingFolder, copyFolder);

                            copyFolders[nIndex] = null;

                            //break;
                        }
                    }

                    // 원본에는 있는데, 대상 폴더에 없는 경우에는 무조건 복사
                    if (!findFile)
                    {
                        try
                        {
                            string path = copyDir.FullName + "\\" + existingFolder.Name;
                            Directory.CreateDirectory(path);
                            SynchFolder(existingFolder, new DirectoryInfo(path));
                        }
                        catch (Exception Desc)
                        {
                            Console.WriteLine(Desc.Message);
                        }
                    }
                }
            }
            #endregion
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public static bool OpenCheckForm(Form form)
        {
            try
            {
                foreach (Form frm in Application.OpenForms)
                {
                    if (frm.Name == form.Name)
                    {
                        frm.Activate();
                        return false;
                    }
                }
                return true;
            }
            catch { return false; }           
        }

        public static bool OpenCheckForm(string strFormName)
        {
            try
            {
                foreach (Form frm in Application.OpenForms)
                {
                    if (frm.Name == strFormName)
                    {
                        //frm.Activate();
                        return false;
                    }
                }
                return true;
            }
            catch { return false; }           
        }

        public static Control[] GetControlsWinform(Control con)
        {
            var conList = new List<Control>();

            foreach (Control control in con.Controls)
            {
                //컨트롤 속성으로 찾는 방법
                if (control is Button)
                {
                    int nSize = 30;

                    //15 로 전달 되어 있는 인자 -> 실제 모서리 둥글게 표현 하는 인자
                    IntPtr ip = CreateRoundRectRgn(0, 0, control.Width, control.Height, nSize, nSize);
                    SetWindowRgn(control.Handle, ip, true);
                    conList.Add(control);
                }
                ////컨트롤 이름으로 찾는 방법
                //if (control.Name == "그리드뷰")
                //    conList.Add(control);

                //주석
                if (control.Controls.Count > 0)
                    conList.AddRange(GetControlsWinform(control));
            }

            return conList.ToArray();
        }

        public static void CaptureScreen(string outputFilename)
        {
            try
            {
                Bitmap FullScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                Graphics g = Graphics.FromImage(FullScreen);
                g.CopyFromScreen(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));
                g.Dispose();
                g = null;
                FullScreen.Save(outputFilename);
            }
            catch (Exception Desc)
            {
                 CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public static bool InitDirectory(string strFolderName)
        {
            try
            {
                string strFolderPath = Application.StartupPath + "\\" + strFolderName + "\\";
                DirectoryInfo dirRecipe = new DirectoryInfo(strFolderPath);
                if (dirRecipe.Exists == false) dirRecipe.Create();

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public static double DrivePercent(string strTargetDriver, out double TotalSize, out double AvaliableSize)
        {
            double dPercent = 0;

            TotalSize = 0.0D;
            AvaliableSize = 0.0D;

            try
            {
                System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
                foreach (System.IO.DriveInfo drive in drives)
                {
                    if (drive.Name == strTargetDriver)
                    {
                        // 드라이브 전체 용량
                        TotalSize = drive.TotalSize / 1000000.0D / 1024.0D;
                        AvaliableSize = drive.AvailableFreeSpace / 1000000.0D / 1024.0D;

                        // 사용중인 용량 ( 전체 용량 - 사용 가능한 용량 )
                        double dUsedSize = (int)((drive.TotalSize - drive.AvailableFreeSpace) / 1000000 / 1024.0D);

                        dPercent = dUsedSize / TotalSize * 100.0D;
                    }
                }
            }
            catch (Exception e)
            {
            }

            return dPercent;
        }

        public static string[] AvalibleComports()
        {
            return SerialPort.GetPortNames();
        }
    }
}
