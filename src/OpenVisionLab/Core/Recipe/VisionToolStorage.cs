using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal static class VisionToolStorage
    {
        public const int DefaultToolSetCount = 1;

        public static void Load(VisionToolRepository repository, string recipeName)
        {
            VisionToolRepository loadedRepository = new VisionToolRepository();
            try
            {
                LoadInto(loadedRepository, recipeName);
                repository.ReplaceLoadedState(loadedRepository);
            }
            catch
            {
                DisposeFailedLoadTemplates(loadedRepository);
                throw;
            }
        }

        private static void LoadInto(VisionToolRepository repository, string recipeName)
        {
            Reset(repository);

            for (int i = 0; i < DefaultToolSetCount; i++)
            {
                repository.Blobs.Add(new BlobProperty($"Blob_{i + 1}").LoadConfig(recipeName));
                repository.Contours.Add(new ContourProperty($"Contour_{i + 1}").LoadConfig(recipeName));
                repository.Lines_L.Add(new LineGaugeProperty($"Line(L)_{i + 1}").LoadConfig(recipeName));
                repository.Lines_R.Add(new LineGaugeProperty($"Line(R)_{i + 1}").LoadConfig(recipeName));
                repository.Lines_TOP.Add(new LineGaugeProperty($"Line(TOP)_{i + 1}").LoadConfig(recipeName));
                repository.Matchings.Add(new MatchingProperty($"Matching_{i + 1}").LoadConfig(recipeName));
                repository.EdgeBasedMatchings.Add(new EdgeBasedMatchingProperty($"EdgeBasedMatching_{i + 1}").LoadConfig(recipeName));
                repository.Features.Add(new FeatureMatchingProperty($"Feature_{i + 1}").LoadConfig(recipeName));
            }

            repository.PropertyVision = repository.PropertyVision.LoadConfig(recipeName);
        }

        public static void Save(VisionToolRepository repository, string recipeName)
        {
            string visionDirectory = Path.GetDirectoryName(
                RecipeWorkspaceService.GetVisionConfigPath(recipeName, "__save_transaction__"));
            string backupDirectory = Path.Combine(
                Path.GetDirectoryName(visionDirectory) ?? visionDirectory,
                $".VISION-save-{Guid.NewGuid():N}");

            try
            {
                List<string> snapshotFiles = CreateSnapshot(visionDirectory, backupDirectory);
                try
                {
                    SaveInto(repository, recipeName);
                }
                catch (Exception saveException)
                {
                    try
                    {
                        RestoreSnapshot(visionDirectory, backupDirectory, snapshotFiles);
                    }
                    catch (Exception rollbackException)
                    {
                        throw new IOException(
                            "Vision Tool save failed and its file snapshot could not be restored.",
                            new AggregateException(saveException, rollbackException));
                    }

                    throw;
                }
            }
            finally
            {
                if (Directory.Exists(backupDirectory))
                {
                    Directory.Delete(backupDirectory, recursive: true);
                }
            }
        }

        private static void SaveInto(VisionToolRepository repository, string recipeName)
        {
            foreach (BlobProperty property in repository.Blobs)
            {
                property.SaveConfig(recipeName);
            }

            foreach (LineGaugeProperty property in repository.Lines_L)
            {
                property.SaveConfig(recipeName);
            }

            foreach (LineGaugeProperty property in repository.Lines_R)
            {
                property.SaveConfig(recipeName);
            }

            foreach (LineGaugeProperty property in repository.Lines_TOP)
            {
                property.SaveConfig(recipeName);
            }

            foreach (ContourProperty property in repository.Contours)
            {
                property.SaveConfig(recipeName);
            }

            foreach (FeatureMatchingProperty property in repository.Features)
            {
                property.SaveConfig(recipeName);
            }

            foreach (MatchingProperty property in repository.Matchings)
            {
                property.SaveConfig(recipeName);
            }

            foreach (EdgeBasedMatchingProperty property in repository.EdgeBasedMatchings)
            {
                property.SaveConfig(recipeName);
            }

            repository.PropertyVision.SaveConfig(recipeName);
        }

        private static List<string> CreateSnapshot(string visionDirectory, string backupDirectory)
        {
            Directory.CreateDirectory(backupDirectory);
            List<string> snapshotFiles = Directory.GetFiles(
                    visionDirectory,
                    "*",
                    SearchOption.TopDirectoryOnly)
                .ToList();
            foreach (string sourcePath in snapshotFiles)
            {
                File.Copy(
                    sourcePath,
                    Path.Combine(backupDirectory, Path.GetFileName(sourcePath)),
                    overwrite: false);
            }

            return snapshotFiles;
        }

        private static void RestoreSnapshot(
            string visionDirectory,
            string backupDirectory,
            IReadOnlyCollection<string> snapshotFiles)
        {
            HashSet<string> originalPaths = new HashSet<string>(
                snapshotFiles,
                StringComparer.OrdinalIgnoreCase);
            foreach (string currentPath in Directory.GetFiles(
                         visionDirectory,
                         "*",
                         SearchOption.TopDirectoryOnly))
            {
                if (!originalPaths.Contains(currentPath))
                {
                    File.Delete(currentPath);
                }
            }

            foreach (string originalPath in snapshotFiles)
            {
                string backupPath = Path.Combine(
                    backupDirectory,
                    Path.GetFileName(originalPath));
                if (!FilesAreEqual(originalPath, backupPath))
                {
                    File.Copy(backupPath, originalPath, overwrite: true);
                }
            }
        }

        private static void DisposeFailedLoadTemplates(VisionToolRepository repository)
        {
            foreach (MatchingProperty property in repository.Matchings)
            {
                property.ImageTemplate?.Dispose();
            }

            foreach (EdgeBasedMatchingProperty property in repository.EdgeBasedMatchings)
            {
                property.ImageTemplate?.Dispose();
            }

            foreach (FeatureMatchingProperty property in repository.Features)
            {
                property.ImageTemplate?.Dispose();
            }
        }

        private static bool FilesAreEqual(string leftPath, string rightPath)
        {
            if (!File.Exists(leftPath) || !File.Exists(rightPath))
            {
                return false;
            }

            FileInfo left = new FileInfo(leftPath);
            FileInfo right = new FileInfo(rightPath);
            if (left.Length != right.Length)
            {
                return false;
            }

            const int BufferSize = 81920;
            byte[] leftBuffer = new byte[BufferSize];
            byte[] rightBuffer = new byte[BufferSize];
            using FileStream leftStream = new FileStream(leftPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using FileStream rightStream = new FileStream(rightPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            while (true)
            {
                int leftRead = leftStream.Read(leftBuffer, 0, leftBuffer.Length);
                int rightRead = rightStream.Read(rightBuffer, 0, rightBuffer.Length);
                if (leftRead != rightRead)
                {
                    return false;
                }

                if (leftRead == 0)
                {
                    return true;
                }

                for (int index = 0; index < leftRead; index++)
                {
                    if (leftBuffer[index] != rightBuffer[index])
                    {
                        return false;
                    }
                }
            }
        }

        private static void Reset(VisionToolRepository repository)
        {
            repository.Blobs.Clear();
            repository.Contours.Clear();
            repository.Lines_L.Clear();
            repository.Lines_R.Clear();
            repository.Lines_TOP.Clear();
            repository.Matchings.Clear();
            repository.EdgeBasedMatchings.Clear();
            repository.Features.Clear();
        }
    }
}
