using Lib.Common;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace OpenVisionLab
{
    public static class SerializeHelper
    {
        public static bool TryLoadFromXmlFile<T>(string path, out T value)
        {
            value = default(T);

                        if (!File.Exists(path))
            {
                return false;
            }

            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                value = (T)serializer.Deserialize(stream);
            }

            return value != null;
        
        }

        public static T LoadOrCreateXmlFile<T>(string path, T defaultValue, out bool loaded)
        {
            if (TryLoadFromXmlFile(path, out T loadedValue) && loadedValue != null)
            {
                loaded = true;
                return loadedValue;
            }

            loaded = false;
            if (!File.Exists(path))
            {
                SaveXmlFile(path, defaultValue);
            }

            return defaultValue;
        }

        public static bool SaveXmlFile<T>(string path, T value)
        {
                        string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                NewLineOnAttributes = true
            };

            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, value);
            }

            return true;
        
        }

    }

}
