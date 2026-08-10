using System;
using System.Collections.Generic;
using System.IO;

namespace OpenVisionLab
{
    internal static class OpenVisionImageDirectoryResolver
    {
        private static string lastImageDirectory;

        public static string ResolveOpenImageDirectory(string lastDirectory)
        {
            if (IsDirectory(lastDirectory))
            {
                return lastDirectory;
            }

            if (IsDirectory(lastImageDirectory))
            {
                return lastImageDirectory;
            }

            string sampleDirectory = ResolveSampleImageDirectory();
            if (IsDirectory(sampleDirectory))
            {
                return sampleDirectory;
            }

            string pictureDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (IsDirectory(pictureDirectory))
            {
                return pictureDirectory;
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }

        public static void RememberImagePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            try
            {
                string directory = Directory.Exists(path)
                    ? Path.GetFullPath(path)
                    : Path.GetDirectoryName(Path.GetFullPath(path));
                if (IsDirectory(directory))
                {
                    lastImageDirectory = directory;
                }
            }
            catch
            {
            }
        }

        private static string ResolveSampleImageDirectory()
        {
            foreach (string root in EnumerateSearchRoots())
            {
                foreach (string sampleName in new[] { "Sample", "Samples", "samples" })
                {
                    string candidate = Path.Combine(root, sampleName);
                    if (IsDirectory(candidate))
                    {
                        return candidate;
                    }
                }
            }

            return null;
        }

        private static IEnumerable<string> EnumerateSearchRoots()
        {
            HashSet<string> visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string start in new[] { AppDomain.CurrentDomain.BaseDirectory, Directory.GetCurrentDirectory() })
            {
                if (!IsDirectory(start))
                {
                    continue;
                }

                DirectoryInfo directory = new DirectoryInfo(start);
                while (directory != null)
                {
                    if (visited.Add(directory.FullName))
                    {
                        yield return directory.FullName;
                    }

                    directory = directory.Parent;
                }
            }
        }

        private static bool IsDirectory(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && Directory.Exists(path);
        }
    }
}
