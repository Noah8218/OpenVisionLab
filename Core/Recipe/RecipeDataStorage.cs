using System;
using System.Collections.Generic;
using System.IO;

namespace OpenVisionLab
{
    internal static class RecipeDataStorage
    {
        private static readonly object persistenceStateSync =
            new object();
        private static readonly Dictionary<
            string,
            RecipeDataPersistenceState> persistenceStates =
                new Dictionary<string, RecipeDataPersistenceState>(
                    StringComparer.OrdinalIgnoreCase);

        public static DataState Load(
            string recipeName,
            DataState defaultData)
        {
            string path =
                RecipeWorkspaceService.GetVisionDataPath(recipeName);
            DataState data = defaultData ?? new DataState();
            try
            {
                if (SerializeHelper.TryLoadFromXmlFile(
                        path,
                        out DataState loaded,
                        out Exception loadException)
                    && loaded != null)
                {
                    return loaded;
                }

                if (!File.Exists(path))
                {
                    ClearPersistenceState(path);
                    SerializeHelper.SaveXmlFile(path, data);
                    return data;
                }

                RecipeDataPersistenceState previousState =
                    GetPersistenceState(path);
                string backupPath =
                    previousState?.Kind
                            == RecipeDataPersistenceStateKind
                                .InvalidFileSubstituted
                        && File.Exists(previousState.BackupPath)
                            ? previousState.BackupPath
                            : SerializeHelper.BackupInvalidXmlFile(
                                path);
                SetPersistenceState(
                    path,
                    new RecipeDataPersistenceState(
                        RecipeDataPersistenceStateKind
                            .InvalidFileSubstituted,
                        recipeName,
                        path,
                        backupPath,
                        loadException?.Message));
                return data;
            }
            catch (Exception ex)
            {
                SetPersistenceState(
                    path,
                    new RecipeDataPersistenceState(
                        RecipeDataPersistenceStateKind.LoadFailed,
                        recipeName,
                        path,
                        string.Empty,
                        ex.GetBaseException().Message));
                return data;
            }
        }

        public static void Save(string recipeName, DataState data)
        {
            string path =
                RecipeWorkspaceService.GetVisionDataPath(recipeName);
            RecipeDataPersistenceState previousState =
                GetPersistenceState(path);
            try
            {
                SerializeHelper.SaveXmlFile(
                    path,
                    data ?? new DataState());
                if (previousState?.IsFailure == true)
                {
                    SetPersistenceState(
                        path,
                        new RecipeDataPersistenceState(
                            RecipeDataPersistenceStateKind.SaveRecovered,
                            recipeName,
                            path,
                            previousState.BackupPath,
                            string.Empty));
                }
                else
                {
                    ClearPersistenceState(path);
                }
            }
            catch (Exception ex)
            {
                SetPersistenceState(
                    path,
                    new RecipeDataPersistenceState(
                        RecipeDataPersistenceStateKind.SaveFailed,
                        recipeName,
                        path,
                        previousState?.BackupPath,
                        ex.GetBaseException().Message));
                throw;
            }
        }

        internal static bool TryGetPersistenceState(
            string recipeName,
            out RecipeDataPersistenceState state)
        {
            state = GetPersistenceState(
                RecipeWorkspaceService.GetVisionDataPath(recipeName));
            return state != null;
        }

        private static RecipeDataPersistenceState GetPersistenceState(
            string path)
        {
            string key = Path.GetFullPath(path);
            lock (persistenceStateSync)
            {
                persistenceStates.TryGetValue(
                    key,
                    out RecipeDataPersistenceState state);
                return state;
            }
        }

        private static void SetPersistenceState(
            string path,
            RecipeDataPersistenceState state)
        {
            string key = Path.GetFullPath(path);
            lock (persistenceStateSync)
            {
                persistenceStates[key] = state;
            }
        }

        private static void ClearPersistenceState(string path)
        {
            string key = Path.GetFullPath(path);
            lock (persistenceStateSync)
            {
                persistenceStates.Remove(key);
            }
        }
    }
}
