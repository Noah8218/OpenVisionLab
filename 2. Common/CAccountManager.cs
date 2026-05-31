using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using Lib.Common;

namespace OpenVisionLab
{
    public class CAccountManager
    {
        public Dictionary<string, CAccount> Accounts = new Dictionary<string, CAccount>();
        public CAccountManager() { }
        
        #region CONFIG BY XML              
        private string m_XMLName = "ACCOUNT";
        public bool LoadConfig()
        {
            try
            {
                string strPath = $"{Application.StartupPath}\\CONFIG\\ACCOUNT\\"+ m_XMLName + ".xml";

                if (File.Exists(strPath))
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);

                    try
                    {
                        Accounts.Clear();

                        while (xmlReader.Read())
                        {
                            if (xmlReader.NodeType == XmlNodeType.Element)
                            {
                                switch (xmlReader.Name)
                                {
                                    case "ACCOUNT_INFO": if (xmlReader.Read())
                                        {                           
                                            string strAccountInfo = xmlReader.Value;
                                            string[] strSplit1 = strAccountInfo.Split(';');

                                            for (int i = 0; i < strSplit1.Length; i++)
                                            {
                                                string[] strSplit2 = strSplit1[i].Split(',');

                                                if(strSplit2.Length == 3)
                                                {
                                                    CAccount account = new CAccount();
                                                    account.ID = strSplit2[0];
                                                    account.PASSWORD = strSplit2[1];
                                                    account.AUTHORIZATION = CUtil.ParseEnum<DEFINE.AUTHORIZATION>(strSplit2[2]);

                                                    Accounts.Add(account.ID, account);
                                                }
                                            }
                                        }
                                        break;
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
                    Accounts.Clear();

                    CAccount account = new CAccount();
                    account.ID = "MASTER";
                    account.PASSWORD = "0000";
                    account.AUTHORIZATION = DEFINE.AUTHORIZATION.MASTER;
                    Accounts.Add("MASTER", account);

                    account = new CAccount();
                    account.ID = "ENGINEER";
                    account.PASSWORD = "0000";
                    account.AUTHORIZATION = DEFINE.AUTHORIZATION.ENGINEER;
                    Accounts.Add("ENGINEER", account);

                    account = new CAccount();
                    account.ID = "OPERATOR";
                    account.PASSWORD = "0000";
                    account.AUTHORIZATION = DEFINE.AUTHORIZATION.OPERATOR;
                    Accounts.Add("OPERATOR", account);


                    SaveConfig();
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

        public bool SaveConfig()
        {
            CUtil.InitDirectory("CONFIG");
            CUtil.InitDirectory("CONFIG\\ACCOUNT\\");

            string strPath = $"{Application.StartupPath}\\CONFIG\\ACCOUNT\\" + m_XMLName + ".xml";

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

                string strAccountInfo = "";
                for (int i = 0; i < Accounts.Count; i++)
                {
                    CAccount account = Accounts.ElementAt(i).Value;
                    strAccountInfo += $"{account.ID},{account.PASSWORD},{account.AUTHORIZATION};";
                }

                xmlWriter.WriteElementString("ACCOUNT_INFO", strAccountInfo);

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
    public class CAccount
    {
        public string ID { get; set; } = "";
        public string PASSWORD { get; set; } = "";
        public DEFINE.AUTHORIZATION AUTHORIZATION { get; set; } = DEFINE.AUTHORIZATION.OPERATOR;
        public CAccount() { }       
    }
}
