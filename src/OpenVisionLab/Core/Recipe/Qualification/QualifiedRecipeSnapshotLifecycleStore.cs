using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OpenVisionLab
{
    internal sealed class QualifiedRecipeSnapshotLifecycleStore
    {
        private readonly string _rootDirectory;
        private readonly QualifiedRecipeSnapshotStore _snapshotStore;

        internal QualifiedRecipeSnapshotLifecycleStore(
            string qualifiedRecipeRoot,
            QualifiedRecipeSnapshotStore snapshotStore)
        {
            _rootDirectory = Path.Combine(
                Path.GetFullPath(qualifiedRecipeRoot),
                "lifecycle");
            _snapshotStore = snapshotStore
                ?? throw new ArgumentNullException(nameof(snapshotStore));
        }

        internal bool TryAppend(
            string snapshotId,
            QualifiedRecipeSnapshotLifecycleAction action,
            string reason,
            string relatedSnapshotId,
            DateTime occurredAtUtc,
            out QualifiedRecipeSnapshotLifecycleEvent created,
            out string error)
        {
            created = null;
            error = string.Empty;
            string normalizedId =
                QualifiedRecipeSnapshotPreflight.NormalizeSha(snapshotId);
            if (!_snapshotStore.Verify(normalizedId).Success)
            {
                error = "Lifecycle event requires an existing valid snapshot.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                error = "Lifecycle reason is required.";
                return false;
            }

            string related =
                QualifiedRecipeSnapshotPreflight.NormalizeSha(relatedSnapshotId);
            if (action == QualifiedRecipeSnapshotLifecycleAction.Superseded)
            {
                if (string.Equals(normalizedId, related, StringComparison.Ordinal)
                    || !_snapshotStore.Verify(related).Success)
                {
                    error = "Superseded requires a different existing valid related snapshot.";
                    return false;
                }
            }
            else if (!string.IsNullOrWhiteSpace(related))
            {
                error = "Revoked does not accept a related snapshot.";
                return false;
            }

            QualifiedRecipeSnapshotLifecycleState state = Load(normalizedId);
            if (!state.Success)
            {
                error = string.Join(Environment.NewLine, state.Errors);
                return false;
            }

            if (state.Events.Count > 0)
            {
                error = "Snapshot lifecycle is already terminal: " + state.State;
                return false;
            }

            QualifiedRecipeSnapshotLifecycleEvent lifecycleEvent =
                new QualifiedRecipeSnapshotLifecycleEvent
                {
                    Sequence = 1,
                    SnapshotId = normalizedId,
                    Action = action.ToString(),
                    OccurredAtUtc = occurredAtUtc.ToUniversalTime().ToString("o"),
                    Reason = reason.Trim(),
                    RelatedSnapshotId = related,
                    PreviousEventSha256 = string.Empty
                };
            lifecycleEvent.EventSha256 = ComputeEventSha256(lifecycleEvent);

            string eventDirectory = GetEventDirectory(normalizedId);
            Directory.CreateDirectory(eventDirectory);
            string eventPath = Path.Combine(
                eventDirectory,
                lifecycleEvent.Sequence.ToString("D6", CultureInfo.InvariantCulture)
                + "_" + lifecycleEvent.EventSha256 + ".xml");
            try
            {
                SaveCreateNew(eventPath, lifecycleEvent);
            }
            catch (Exception ex)
            {
                error = ex.GetBaseException().Message;
                return false;
            }

            QualifiedRecipeSnapshotLifecycleState reloaded = Load(normalizedId);
            if (!reloaded.Success || reloaded.Events.Count != 1)
            {
                error = "Lifecycle event reload verification failed: "
                    + string.Join(" | ", reloaded.Errors);
                return false;
            }

            created = lifecycleEvent;
            return true;
        }

        internal QualifiedRecipeSnapshotLifecycleState Load(string snapshotId)
        {
            QualifiedRecipeSnapshotLifecycleState state =
                new QualifiedRecipeSnapshotLifecycleState();
            string normalizedId =
                QualifiedRecipeSnapshotPreflight.NormalizeSha(snapshotId);
            if (!QualifiedRecipeSnapshotPreflight.IsSha256(normalizedId))
            {
                state.Errors.Add("Lifecycle snapshot ID is invalid.");
                return state;
            }

            string eventDirectory = GetEventDirectory(normalizedId);
            if (!Directory.Exists(eventDirectory))
            {
                return state;
            }

            List<string> files = Directory.EnumerateFiles(
                    eventDirectory,
                    "*.xml",
                    SearchOption.TopDirectoryOnly)
                .OrderBy(path => Path.GetFileName(path), StringComparer.Ordinal)
                .ToList();
            string previousHash = string.Empty;
            for (int index = 0; index < files.Count; index++)
            {
                if (!SerializeHelper.TryLoadFromXmlFile(
                        files[index],
                        out QualifiedRecipeSnapshotLifecycleEvent lifecycleEvent)
                    || lifecycleEvent == null)
                {
                    state.Errors.Add(
                        "Lifecycle event XML is invalid: " + Path.GetFileName(files[index]));
                    continue;
                }

                int expectedSequence = index + 1;
                string computedHash = ComputeEventSha256(lifecycleEvent);
                if (lifecycleEvent.SchemaVersion != 1
                    || lifecycleEvent.Sequence != expectedSequence
                    || !string.Equals(
                        lifecycleEvent.SnapshotId,
                        normalizedId,
                        StringComparison.Ordinal)
                    || !string.Equals(
                        lifecycleEvent.PreviousEventSha256,
                        previousHash,
                        StringComparison.Ordinal)
                    || !string.Equals(
                        lifecycleEvent.EventSha256,
                        computedHash,
                        StringComparison.Ordinal)
                    || !Path.GetFileName(files[index]).StartsWith(
                        expectedSequence.ToString("D6", CultureInfo.InvariantCulture)
                        + "_" + computedHash,
                        StringComparison.Ordinal))
                {
                    state.Errors.Add(
                        "Lifecycle event hash/sequence chain is invalid: "
                        + Path.GetFileName(files[index]));
                    continue;
                }

                if (!Enum.TryParse(
                        lifecycleEvent.Action,
                        ignoreCase: false,
                        out QualifiedRecipeSnapshotLifecycleAction action)
                    || string.IsNullOrWhiteSpace(lifecycleEvent.Reason))
                {
                    state.Errors.Add("Lifecycle event action/reason is invalid.");
                    continue;
                }

                if (action == QualifiedRecipeSnapshotLifecycleAction.Superseded)
                {
                    if (!QualifiedRecipeSnapshotPreflight.IsSha256(
                            lifecycleEvent.RelatedSnapshotId)
                        || string.Equals(
                            lifecycleEvent.RelatedSnapshotId,
                            normalizedId,
                            StringComparison.Ordinal))
                    {
                        state.Errors.Add("Superseded lifecycle relation is invalid.");
                        continue;
                    }

                    state.State = "Superseded";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(lifecycleEvent.RelatedSnapshotId))
                    {
                        state.Errors.Add("Revoked lifecycle event has an unexpected relation.");
                        continue;
                    }

                    state.State = "Revoked";
                }

                if (state.Events.Count > 0)
                {
                    state.Errors.Add("Lifecycle contains more than one terminal event.");
                    continue;
                }

                state.Events.Add(lifecycleEvent);
                previousHash = lifecycleEvent.EventSha256;
            }

            return state;
        }

        private string GetEventDirectory(string snapshotId)
        {
            return Path.Combine(_rootDirectory, snapshotId + ".events");
        }

        private static string ComputeEventSha256(
            QualifiedRecipeSnapshotLifecycleEvent lifecycleEvent)
        {
            string canonical = string.Join(
                "\n",
                lifecycleEvent.SchemaVersion.ToString(CultureInfo.InvariantCulture),
                lifecycleEvent.Sequence.ToString(CultureInfo.InvariantCulture),
                lifecycleEvent.SnapshotId ?? string.Empty,
                lifecycleEvent.Action ?? string.Empty,
                lifecycleEvent.OccurredAtUtc ?? string.Empty,
                lifecycleEvent.Reason ?? string.Empty,
                lifecycleEvent.RelatedSnapshotId ?? string.Empty,
                lifecycleEvent.PreviousEventSha256 ?? string.Empty);
            return Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
        }

        private static void SaveCreateNew<T>(string path, T value)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                NewLineOnAttributes = true
            };
            using FileStream stream = new FileStream(
                path,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None);
            using XmlWriter writer = XmlWriter.Create(stream, settings);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, value);
        }
    }
}
