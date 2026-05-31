using Lib.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace OpenVisionLab
{
    public static class SerializeHelper
    {
        public static string ToString<T>(this T toSerialize)
        {
            //XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            //MemoryStream ms = new MemoryStream();
            //XmlTextWriter xmlTextWriter = new XmlTextWriter(ms, Encoding.UTF8);

            //xmlTextWriter.Formatting = Formatting.Indented;

            //xmlSerializer.Serialize(xmlTextWriter, toSerialize);

            return Encoding.UTF8.GetString(ToByte<T>(toSerialize));
        }

        public static byte[] ToByte<T>(this T toSerialize)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

                MemoryStream ms = new MemoryStream();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(ms, Encoding.UTF8);

                xmlTextWriter.Formatting = Formatting.Indented;

                xmlSerializer.Serialize(xmlTextWriter, toSerialize);

                return ((MemoryStream)xmlTextWriter.BaseStream).ToArray();
            }
            catch (Exception Desc)
            {
                //VTS.Logger.Error(ex, string.Format("[VTS.MySerialize.ToByte] error. type({0})", typeof(T).ToString()));

                CLOG.ABNORMAL( $"[{MethodBase.GetCurrentMethod().ReflectedType.Name}]==>{MethodBase.GetCurrentMethod().Name}]   Ex = {Desc.Message}");

            }

            return null;
        }

        public static T Deserialize<T>(string xml)
        {
            try
            {
                if (!string.IsNullOrEmpty(xml))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    byte[] byteArr = System.Text.Encoding.UTF8.GetBytes(xml);

                    using (MemoryStream ms = new MemoryStream(byteArr))
                    {
                        XmlTextReader xmlTextReader = new XmlTextReader(ms);
                        return (T)serializer.Deserialize(xmlTextReader);
                    }

                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[{MethodBase.GetCurrentMethod().ReflectedType.Name}]==>{MethodBase.GetCurrentMethod().Name}]   Ex = {Desc.Message}");
            }

            return default(T);

            //XmlReaderSettings settings = new XmlReaderSettings();
            // No settings need modifying here

            //using (StringReader textReader = new StringReader(xml))
            //{
            //    using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
            //    {
            //        return (T)serializer.Deserialize(xmlReader);
            //    }
            //}
        }

        public static T FromXmlFile<T>(string path)
        {
            try
            {
                using (
                    System.IO.StreamReader sr = new StreamReader(path)
                )
                {
                    XmlSerializer reader =
                    new XmlSerializer(typeof(T));

                    T np = (T)reader.Deserialize(sr);

                    return np;
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[{MethodBase.GetCurrentMethod().ReflectedType.Name}]==>{MethodBase.GetCurrentMethod().Name}], type({typeof(T).ToString()}), path={path}, Ex = {Desc.Message}");
                //VTS.Logger.Error(ex, string.Format(
                //    "[VTS.MySerialize.FromXmlFile] error. type({0}), path={1}",
                //    typeof(T).ToString(), path));
                CCommon.ShowMessageBox("EXCEPTION", string.Format($"[{MethodBase.GetCurrentMethod().ReflectedType.Name}]==>{MethodBase.GetCurrentMethod().Name}], type({typeof(T).ToString()}), path={path}, Ex = {Desc.Message}"), FormMessageBox.MESSAGEBOX_TYPE.Waring);
            }

            return default(T);
        }

        public static bool ToXmlFile<T>(string path, T val)
        {
            try
            {
                using (Stream savestream = new FileStream(path, FileMode.Create))
                {
                    XmlSerializer writer = new XmlSerializer(val.GetType());
                    writer.Serialize(savestream, val);

                }
                return true;
            }
            catch (Exception Desc)
            {                
                CLOG.ABNORMAL( $"[{MethodBase.GetCurrentMethod().ReflectedType.Name}]==>{MethodBase.GetCurrentMethod().Name}], type({typeof(T).ToString()}), path={path}, Ex = {Desc.Message}");

                //CCommon.ShowMessageBox("EXCEPTION", string.Format($"[{MethodBase.GetCurrentMethod().ReflectedType.Name}]==>{MethodBase.GetCurrentMethod().Name}], type({typeof(T).ToString()}), path={path}, Ex = {Desc.Message}"), FormMessageBox.MESSAGEBOX_TYPE.Waring);
                //VTS.Logger.Error(ex, string.Format(
                //    "[VTS.MySerialize.ToXmlFile] error. type({0}), path={1}",
                //    typeof(T).ToString(), path));
            }
            return false;
        }


    }

}
