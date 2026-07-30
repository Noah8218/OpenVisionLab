using Lib.Common;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace OpenVisionLab
{
    internal enum XmlFileLoadDisposition
    {
        Loaded,
        CreatedDefaultForMissingFile,
        ReplacedInvalidFile
    }

    internal sealed class XmlFileLoadResult
    {
        public XmlFileLoadResult(
            XmlFileLoadDisposition disposition,
            string sourcePath,
            string backupPath,
            string errorMessage)
        {
            Disposition = disposition;
            SourcePath = sourcePath ?? string.Empty;
            BackupPath = backupPath ?? string.Empty;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        public XmlFileLoadDisposition Disposition { get; }

        public string SourcePath { get; }

        public string BackupPath { get; }

        public string ErrorMessage { get; }
    }

    public static class SerializeHelper
    {
        public static bool TryLoadFromXmlFile<T>(string path, out T value)
        {
            return TryLoadFromXmlFile(path, out value, out _);
        }

        private static bool TryLoadFromXmlFile<T>(
            string path,
            out T value,
            out Exception loadException)
        {
            value = default(T);
            loadException = null;

            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    value = (T)serializer.Deserialize(stream);
                }

                return value != null;
            }
            catch (InvalidOperationException exception)
            {
                value = default(T);
                loadException = exception.GetBaseException();
                return false;
            }
            catch (XmlException exception)
            {
                value = default(T);
                loadException = exception.GetBaseException();
                return false;
            }
            catch (IOException exception)
            {
                value = default(T);
                loadException = exception.GetBaseException();
                return false;
            }
        }

        public static bool TryLoadFromXmlText<T>(string xmlText, out T value, out string errorMessage)
        {
            value = default(T);
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(xmlText))
            {
                errorMessage = "XML text is empty.";
                return false;
            }

            try
            {
                using (StringReader reader = new StringReader(xmlText))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    value = (T)serializer.Deserialize(reader);
                }

                return value != null;
            }
            catch (InvalidOperationException ex)
            {
                errorMessage = ex.GetBaseException().Message;
                value = default(T);
                return false;
            }
            catch (XmlException ex)
            {
                errorMessage = ex.GetBaseException().Message;
                value = default(T);
                return false;
            }
        }

        public static T LoadOrCreateXmlFile<T>(string path, T defaultValue, out bool loaded)
        {
            return LoadOrCreateXmlFile(
                path,
                defaultValue,
                out loaded,
                out _);
        }

        internal static T LoadOrCreateXmlFile<T>(
            string path,
            T defaultValue,
            out bool loaded,
            out XmlFileLoadResult loadResult)
        {
            if (TryLoadFromXmlFile(
                    path,
                    out T loadedValue,
                    out Exception loadException)
                && loadedValue != null)
            {
                loaded = true;
                loadResult = new XmlFileLoadResult(
                    XmlFileLoadDisposition.Loaded,
                    path,
                    string.Empty,
                    string.Empty);
                return loadedValue;
            }

            loaded = false;
            if (File.Exists(path))
            {
                string backupPath = BackupInvalidXmlFile(path);
                SaveXmlFile(path, defaultValue);
                loadResult = new XmlFileLoadResult(
                    XmlFileLoadDisposition.ReplacedInvalidFile,
                    path,
                    backupPath,
                    loadException?.Message);
                return defaultValue;
            }

            SaveXmlFile(path, defaultValue);
            loadResult = new XmlFileLoadResult(
                XmlFileLoadDisposition.CreatedDefaultForMissingFile,
                path,
                string.Empty,
                string.Empty);
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

            string tempPath = CreateTempPath(path);
            try
            {
                using (XmlWriter writer = XmlWriter.Create(tempPath, settings))
                {
                    XmlSerializer serializer = new XmlSerializer(GetXmlSerializerType(value));
                    serializer.Serialize(writer, value);
                }

                ReplaceFile(tempPath, path);
                return true;
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        private static Type GetXmlSerializerType<T>(T value)
        {
            return value == null ? typeof(T) : value.GetType();
        }

        private static string CreateTempPath(string path)
        {
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Directory.GetCurrentDirectory();
            }

            return Path.Combine(directory, $".{fileName}.{Guid.NewGuid():N}.tmp");
        }

        private static void ReplaceFile(string tempPath, string path)
        {
            if (File.Exists(path))
            {
                File.Replace(tempPath, path, null);
                return;
            }

            File.Move(tempPath, path);
        }

        private static string BackupInvalidXmlFile(string path)
        {
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);

            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Directory.GetCurrentDirectory();
            }

            string backupPath = Path.Combine(directory, $"{fileName}.invalid-{DateTime.Now:yyyyMMddHHmmssfff}{extension}");
            File.Move(path, backupPath);
            return backupPath;
        }
    }

}
