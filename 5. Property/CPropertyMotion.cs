using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Lib.Common;
using OpenCvSharp;

namespace OpenVisionLab
{
    public class CPropertyMotion
    {
        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "";

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("DESC")]
        public string DESC { get; set; } = "";

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("SECTION")]
        public string SECTION { get; set; } = "";

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("AXIS_NO")]
        public int AXIS_NO { get; set; } = 0;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("AXIS_NAME")]
        public string AXIS_NAME { get; set; } = "";

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("POSITION_NAME")]
        public string POSITION_NAME { get; set; } = "";

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("POSITION")]
        public double POSITION { get; set; } = 0;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("DESC")]
        public double PreviousPos { get; set; } = 0;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("SPEED_FAST")]
        public double SPEED_FAST { get; set; } = 100;        

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("SPEED_SLOW")]
        public double SPEED_SLOW { get; set; } = 50;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("RPM")]
        public double SPEED_RPM { get; set; } = 10;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("RPM 최저 제한 속도")]
        public double RPM_SPEED_MIN { get; set; } = 0;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("RPM 최대 제한 속도")]
        public double RPM_SPEED_MAX { get; set; } = 0;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("SPEED 최대 제한 속도")]
        public double POSITION_SPEED_MAX { get; set; } = 0;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("SPEED 최저 제한 속도")]
        public double POSITION_SPEED_MIN { get; set; } = 0;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("ACCEL_TIME")]
        public double ACCEL_TIME { get; set; } = 100;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("DECEL_TIME")]
        public double DECEL_TIME { get; set; } = 100;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("DELAY_BEFORE")]
        public int DELAY_BEFORE { get; set; } = 100;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("DELAY_AFTER")]
        public int DELAY_AFTER { get; set; } = 100;

        [CategoryAttribute("SEQUENCE"), DescriptionAttribute(""), DisplayNameAttribute("TIME_OUT")]
        public int TIME_OUT { get; set; } = 20000;

        public int MOVE_START_TIME = 0;
        public bool IS_COMPLETE_INPOS = false;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("ACTUAL_POS")]
        public double ACTUAL_POS { get; set; } = 0;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("COMMAND_POS")]
        public double COMMAND_POS { get; set; } = 0;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("ACTUAL_POS_Pre")]
        public double ACTUAL_POS_Pre { get; set; } = 0;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("COMMAND_POS_Pre")]
        public double COMMAND_POS_Pre { get; set; } = 0;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("ACTUAL_POS_TEMP")]
        public double ACTUAL_POS_TEMP { get; set; } = 0;

        [CategoryAttribute("PARAMETER"), DescriptionAttribute(""), DisplayNameAttribute("COMMAND_POS_TEMP")]
        public double COMMAND_POS_TEMP { get; set; } = 0;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("EMPTY")]
        public string EMPTY { get; set; } = "EMPTY";

        public List<CSignal> Inputs_Ref = new List<CSignal>();
        public List<CSignal> Outputs_Ref = new List<CSignal>();

        public CPropertyMotion(string strName)
        {
            NAME = strName;
        }

        public CPropertyMotion(string strSection, string strName, int AxisNo,  string strDesc = "")
        {
            SECTION = strSection;
            NAME = strName;
            AXIS_NO = AxisNo;
            DESC = strDesc;            
        }

        #region CONFIG BY XML              
        private string m_XMLName = "PROPERTY_MOTION";
        public bool LoadConfig(string strRecipe)
        {
            try
            {
                string strPath =  $"{Application.StartupPath}\\RECIPE\\{strRecipe}\\MOTION\\" + NAME + ".xml";

                if (File.Exists(strPath))
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);

                    try
                    {
                        while (xmlReader.Read())
                        {
                            if (xmlReader.NodeType == XmlNodeType.Element)
                            {
                                switch (xmlReader.Name)
                                {
                                    case "NAME": if (xmlReader.Read()) NAME = xmlReader.Value; break;
                                    case "AXIS_NO": if (xmlReader.Read()) AXIS_NO = int.Parse(xmlReader.Value); break;
                                    case "AXIS_NAME": if (xmlReader.Read()) AXIS_NAME = xmlReader.Value; break;
                                    case "POSITION_NAME": if (xmlReader.Read()) POSITION_NAME = xmlReader.Value; break;
                                    case "POSITION": if (xmlReader.Read()) POSITION = double.Parse(xmlReader.Value); break;
                                    case "PreviousPos": if (xmlReader.Read()) PreviousPos = double.Parse(xmlReader.Value); break;
                                    case "SPEED_FAST": if (xmlReader.Read()) SPEED_FAST = double.Parse(xmlReader.Value); break;
                                    case "SPEED_SLOW": if (xmlReader.Read()) SPEED_SLOW = double.Parse(xmlReader.Value); break;
                                    case "RPM_SPEED_MAX": if (xmlReader.Read()) RPM_SPEED_MAX = double.Parse(xmlReader.Value); break;
                                    case "RPM_SPEED_MIN": if (xmlReader.Read()) RPM_SPEED_MIN = double.Parse(xmlReader.Value); break;
                                    case "SPEED_RPM": if (xmlReader.Read()) SPEED_RPM = double.Parse(xmlReader.Value); break;
                                    case "POSITION_SPEED_MAX": if (xmlReader.Read()) POSITION_SPEED_MAX = double.Parse(xmlReader.Value); break;
                                    case "POSITION_SPEED_MIN": if (xmlReader.Read()) POSITION_SPEED_MIN = double.Parse(xmlReader.Value); break;
                                    case "DELAY_BEFORE": if (xmlReader.Read()) DELAY_BEFORE = int.Parse(xmlReader.Value); break;
                                    case "DELAY_AFTER": if (xmlReader.Read()) DELAY_AFTER = int.Parse(xmlReader.Value); break;
                                    case "ACCEL_TIME": if (xmlReader.Read()) ACCEL_TIME = double.Parse(xmlReader.Value); break;
                                    case "DECEL_TIME": if (xmlReader.Read()) DECEL_TIME = double.Parse(xmlReader.Value); break;
                                    case "ACTUAL_POS": if (xmlReader.Read()) ACTUAL_POS = double.Parse(xmlReader.Value); break;
                                    case "COMMAND_POS": if (xmlReader.Read()) COMMAND_POS = double.Parse(xmlReader.Value); break;
                                    case "ACTUAL_POS_TEMP": if (xmlReader.Read()) ACTUAL_POS_TEMP = double.Parse(xmlReader.Value); break;
                                    case "COMMAND_POS_TEMP": if (xmlReader.Read()) COMMAND_POS_TEMP = double.Parse(xmlReader.Value); break;
                                    case "ACTUAL_POS_Pre": if (xmlReader.Read()) ACTUAL_POS_Pre = double.Parse(xmlReader.Value); break;
                                    case "COMMAND_POS_Pre": if (xmlReader.Read()) COMMAND_POS_Pre = double.Parse(xmlReader.Value); break;
                                }

                                CLOG.CONFIG($"LOAD CONFIG ==> {nameof(xmlReader.Name)} : {xmlReader.Value}");
                            }
                            else
                            {
                                if (xmlReader.NodeType == XmlNodeType.EndElement)
                                {
                                    if (xmlReader.Name == m_XMLName) break;
                                }
                            }
                        }
                    }
                    catch (Exception Desc)
                    {
                        CCommon.ShowdialogMessageBox("ALARM", "레시피 로딩에 실패했습니다. 즉시 프로그램을 종료해주세요.");
                        CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                        xmlReader.Close();
                    }

                    xmlReader.Close();
                }
                else
                {
                    SaveConfig(strRecipe);
                    return false;
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return false;
            }
            return true;
        }

        public bool ManualLoadConfig(string strPath)
        {
            try
            {
                if (File.Exists(strPath))
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);

                    try
                    {
                        while (xmlReader.Read())
                        {
                            if (xmlReader.NodeType == XmlNodeType.Element)
                            {
                                switch (xmlReader.Name)
                                {
                                    case "NAME": if (xmlReader.Read()) NAME = xmlReader.Value; break;
                                    case "AXIS_NO": if (xmlReader.Read()) AXIS_NO = int.Parse(xmlReader.Value); break;
                                    case "AXIS_NAME": if (xmlReader.Read()) AXIS_NAME = xmlReader.Value; break;
                                    case "POSITION_NAME": if (xmlReader.Read()) POSITION_NAME = xmlReader.Value; break;
                                    case "POSITION": if (xmlReader.Read()) POSITION = double.Parse(xmlReader.Value); break;
                                    case "PreviousPos": if (xmlReader.Read()) PreviousPos = double.Parse(xmlReader.Value); break;
                                    case "SPEED_FAST": if (xmlReader.Read()) SPEED_FAST = double.Parse(xmlReader.Value); break;
                                    case "SPEED_SLOW": if (xmlReader.Read()) SPEED_SLOW = double.Parse(xmlReader.Value); break;
                                    case "RPM_SPEED_MAX": if (xmlReader.Read()) RPM_SPEED_MAX = double.Parse(xmlReader.Value); break;
                                    case "RPM_SPEED_MIN": if (xmlReader.Read()) RPM_SPEED_MIN = double.Parse(xmlReader.Value); break;
                                    case "SPEED_RPM": if (xmlReader.Read()) SPEED_RPM = int.Parse(xmlReader.Value); break;
                                    case "POSITION_SPEED_MAX": if (xmlReader.Read()) POSITION_SPEED_MAX = double.Parse(xmlReader.Value); break;
                                    case "POSITION_SPEED_MIN": if (xmlReader.Read()) POSITION_SPEED_MIN = double.Parse(xmlReader.Value); break;
                                    case "DELAY_BEFORE": if (xmlReader.Read()) DELAY_BEFORE = int.Parse(xmlReader.Value); break;
                                    case "DELAY_AFTER": if (xmlReader.Read()) DELAY_AFTER = int.Parse(xmlReader.Value); break;
                                    case "ACCEL_TIME": if (xmlReader.Read()) ACCEL_TIME = double.Parse(xmlReader.Value); break;
                                    case "DECEL_TIME": if (xmlReader.Read()) DECEL_TIME = double.Parse(xmlReader.Value); break;
                                    case "ACTUAL_POS": if (xmlReader.Read()) ACTUAL_POS = double.Parse(xmlReader.Value); break;
                                    case "COMMAND_POS": if (xmlReader.Read()) COMMAND_POS = double.Parse(xmlReader.Value); break;
                                    case "ACTUAL_POS_TEMP": if (xmlReader.Read()) ACTUAL_POS_TEMP = double.Parse(xmlReader.Value); break;
                                    case "COMMAND_POS_TEMP": if (xmlReader.Read()) COMMAND_POS_TEMP = double.Parse(xmlReader.Value); break;
                                    case "ACTUAL_POS_Pre": if (xmlReader.Read()) ACTUAL_POS_Pre = double.Parse(xmlReader.Value); break;
                                    case "COMMAND_POS_Pre": if (xmlReader.Read()) COMMAND_POS_Pre = double.Parse(xmlReader.Value); break;
                                }
                            }
                            else
                            {
                                if (xmlReader.NodeType == XmlNodeType.EndElement)
                                {
                                    if (xmlReader.Name == m_XMLName) break;
                                }
                            }
                        }
                    }
                    catch (Exception Desc)
                    {
                        CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                        xmlReader.Close();
                    }

                    xmlReader.Close();
                }
                else
                {
                    ManualSaveConfig(strPath);
                    return false;
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
                return false;
            }
            return true;
        }
        public bool SaveConfig(string strRecipe)
        {
            string strPath = $"{Application.StartupPath}\\RECIPE\\{strRecipe}\\MOTION\\" + NAME + ".xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            XmlWriter xmlWriter = XmlWriter.Create(strPath, settings);
            try
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("PROPERTY");

                //CLog.CONFIG($"==================== UPDETED RECIPE : {Global.Recipe.Name} ====================");
                WriteParameter(xmlWriter, nameof(AXIS_NO), AXIS_NO);
                WriteParameter(xmlWriter, nameof(AXIS_NAME), AXIS_NAME);
                WriteParameter(xmlWriter, nameof(POSITION_NAME), POSITION_NAME);
                WriteParameter(xmlWriter, nameof(POSITION), POSITION);
                WriteParameter(xmlWriter, nameof(PreviousPos), PreviousPos);
                WriteParameter(xmlWriter, nameof(SPEED_FAST), SPEED_FAST);
                WriteParameter(xmlWriter, nameof(SPEED_SLOW), SPEED_SLOW);
                WriteParameter(xmlWriter, nameof(ACCEL_TIME), ACCEL_TIME);
                WriteParameter(xmlWriter, nameof(DECEL_TIME), DECEL_TIME);
                WriteParameter(xmlWriter, nameof(RPM_SPEED_MAX), RPM_SPEED_MAX);
                WriteParameter(xmlWriter, nameof(RPM_SPEED_MIN), RPM_SPEED_MIN);
                WriteParameter(xmlWriter, nameof(SPEED_RPM), SPEED_RPM);
                WriteParameter(xmlWriter, nameof(POSITION_SPEED_MAX), POSITION_SPEED_MAX);
                WriteParameter(xmlWriter, nameof(POSITION_SPEED_MIN), POSITION_SPEED_MIN);
                WriteParameter(xmlWriter, nameof(DELAY_BEFORE), DELAY_BEFORE);
                WriteParameter(xmlWriter, nameof(DELAY_AFTER), DELAY_AFTER);
                WriteParameter(xmlWriter, nameof(ACTUAL_POS), ACTUAL_POS);
                WriteParameter(xmlWriter, nameof(COMMAND_POS), COMMAND_POS);
                WriteParameter(xmlWriter, nameof(ACTUAL_POS_TEMP), ACTUAL_POS_TEMP);
                WriteParameter(xmlWriter, nameof(COMMAND_POS_TEMP), COMMAND_POS_TEMP);
                WriteParameter(xmlWriter, nameof(ACTUAL_POS_Pre), ACTUAL_POS_Pre);
                WriteParameter(xmlWriter, nameof(COMMAND_POS_Pre), COMMAND_POS_Pre);
                CLOG.CONFIG("============================================================");

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }

            return true;
        }

        public bool WriteParameter(XmlWriter xmlWriter, string strParamName, object ob)
        {
            try
            {
                xmlWriter.WriteElementString(strParamName, ob.ToString());
                CLOG.CONFIG($"SAVE CONFIG ==> {NAME} {strParamName} : {ob.ToString()}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
            }

            return true;
        }

        public bool ManualSaveConfig(string strPath)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            XmlWriter xmlWriter = XmlWriter.Create(strPath, settings);
            try
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("PROPERTY");
                xmlWriter.WriteElementString("AXIS_NO", AXIS_NO.ToString());
                xmlWriter.WriteElementString("AXIS_NAME", AXIS_NAME.ToString());
                xmlWriter.WriteElementString("POSITION_NAME", POSITION_NAME.ToString());
                xmlWriter.WriteElementString("POSITION", POSITION.ToString());
                xmlWriter.WriteElementString("PreviousPos", PreviousPos.ToString());
                xmlWriter.WriteElementString("SPEED_FAST", SPEED_FAST.ToString());
                xmlWriter.WriteElementString("SPEED_SLOW", SPEED_SLOW.ToString());
                xmlWriter.WriteElementString("ACCEL_TIME", ACCEL_TIME.ToString());
                xmlWriter.WriteElementString("DECEL_TIME", DECEL_TIME.ToString());
                xmlWriter.WriteElementString("RPM_SPEED_MAX", RPM_SPEED_MAX.ToString());
                xmlWriter.WriteElementString("RPM_SPEED_MIN", RPM_SPEED_MIN.ToString());
                xmlWriter.WriteElementString("SPEED_RPM", SPEED_RPM.ToString());
                xmlWriter.WriteElementString("POSITION_SPEED_MAX", POSITION_SPEED_MAX.ToString());
                xmlWriter.WriteElementString("POSITION_SPEED_MIN", POSITION_SPEED_MIN.ToString());
                xmlWriter.WriteElementString("DELAY_BEFORE", DELAY_BEFORE.ToString());
                xmlWriter.WriteElementString("DELAY_AFTER", DELAY_AFTER.ToString());
                xmlWriter.WriteElementString("ACTUAL_POS", ACTUAL_POS.ToString());
                xmlWriter.WriteElementString("COMMAND_POS", COMMAND_POS.ToString());
                xmlWriter.WriteElementString("ACTUAL_POS_TEMP", ACTUAL_POS_TEMP.ToString());
                xmlWriter.WriteElementString("COMMAND_POS_TEMP", COMMAND_POS_TEMP.ToString());
                xmlWriter.WriteElementString("ACTUAL_POS_Pre", ACTUAL_POS_Pre.ToString());
                xmlWriter.WriteElementString("COMMAND_POS_Pre", COMMAND_POS_Pre.ToString());

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[ERROR] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Ex ==> {Desc.Message}");
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }

            return true;
        }
        #endregion

    }
}
