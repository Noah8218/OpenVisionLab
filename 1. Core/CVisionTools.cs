using OpenVisionLab.Vision._1._Tools.OpenCV;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace OpenVisionLab
{
    public class LinesProperties
    {
        public string Name = "";
        public CPropertyLineGuage Lines_L = new CPropertyLineGuage();
        public CPropertyLineGuage Lines_R = new CPropertyLineGuage();
    }

    public static class CVisionTools
    {
        public static List<LinesProperties> Lines = new List<LinesProperties>();
        public static List<CPropertyBlob> Blobs = new List<CPropertyBlob>();
        public static List<CPropertyContour> Contours = new List<CPropertyContour>();
        public static List<CPropertyLineGuage> Lines_L = new List<CPropertyLineGuage>();
        public static List<CPropertyLineGuage> Lines_R = new List<CPropertyLineGuage>();
        public static List<CPropertyLineGuage> Lines_TOP = new List<CPropertyLineGuage>();
        public static List<CPropertyMatching> Matchings = new List<CPropertyMatching>();

        public static CPropertyVision PropertyVision = new CPropertyVision("VisionPara");

        public static List<CPropertyFeatureMatching> Features = new List<CPropertyFeatureMatching>();

        public static bool LoadTools(string Name)
        {
            try
            {
                Blobs.Clear();
                Contours.Clear();
                Lines_L.Clear();
                Lines_R.Clear();
                Lines_TOP.Clear();
                Matchings.Clear();
                Features.Clear();

                for (int i = 0; i < CGlobal.Inst.Device.CAMERA_COUNT; i++)
                {                    
                    Blobs.Add(new CPropertyBlob($"Blob_{i + 1}"));
                    Contours.Add(new CPropertyContour($"Contour_{i + 1}"));
                    Lines_L.Add(new CPropertyLineGuage($"Line(L)_{i + 1}"));
                    Lines_R.Add(new CPropertyLineGuage($"Line(R)_{i + 1}"));
                    Lines_TOP.Add(new CPropertyLineGuage($"Line(TOP)_{i + 1}"));
                    Matchings.Add(new CPropertyMatching($"Matching_{i + 1}"));
                    Features.Add(new CPropertyFeatureMatching($"Feature_{i + 1}"));
                }

                for (int i = 0; i < CGlobal.Inst.Device.CAMERA_COUNT; i++)
                {
                    Blobs[i] = Blobs[i].LoadConfig(Name);
                    Contours[i] = Contours[i].LoadConfig(Name);
                    Lines_L[i] = Lines_L[i].LoadConfig(Name);
                    Lines_R[i] = Lines_R[i].LoadConfig(Name);
                    Lines_TOP[i] = Lines_TOP[i].LoadConfig(Name);
                    Matchings[i] = Matchings[i].LoadConfig(Name);
                    Features[i] = Features[i].LoadConfig(Name);
                }

                PropertyVision = PropertyVision.LoadConfig(Name);
            }
            catch (Exception Desc)
            {
                MessageBox.Show(Desc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public static bool SaveTools(string Name)
        {
            try
            {
                for (int i = 0; i < CGlobal.Inst.Device.CAMERA_COUNT; i++)
                {
                    Blobs[i].SaveConfig(Name);
                    Lines_L[i].SaveConfig(Name);
                    Lines_R[i].SaveConfig(Name);
                    Lines_TOP[i].SaveConfig(Name);
                    Contours[i].SaveConfig(Name);
                    Features[i].SaveConfig(Name);
                }
            }
            catch (Exception Desc)
            {
                MessageBox.Show(Desc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}
