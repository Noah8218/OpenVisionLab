using OpenVisionLab.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeToolPrewarmService
    {
        private readonly Dispatcher dispatcher;
        private readonly OpenVisionNativeToolDocumentCache documentCache;
        private readonly IDisplayManager displayManager;
        private readonly Func<bool> isHostLoaded;
        private readonly Action<OpenVisionNativeToolDocument> warmDocument;
        private readonly Queue<VISION_MENU> queue = new Queue<VISION_MENU>();
        private readonly Stopwatch stopwatch = new Stopwatch();
        private int creationBudget = int.MaxValue;
        private int createdThisRun;
        private bool started;
        private bool cancelled;

        public OpenVisionNativeToolPrewarmService(
            Dispatcher dispatcher,
            OpenVisionNativeToolDocumentCache documentCache,
            IDisplayManager displayManager,
            Func<bool> isHostLoaded,
            Action<OpenVisionNativeToolDocument> warmDocument)
        {
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.documentCache = documentCache ?? throw new ArgumentNullException(nameof(documentCache));
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.isHostLoaded = isHostLoaded ?? throw new ArgumentNullException(nameof(isHostLoaded));
            this.warmDocument = warmDocument ?? throw new ArgumentNullException(nameof(warmDocument));
        }

        public bool IsCompleted { get; private set; }

        public int CreatedCount { get; private set; }

        public long ElapsedMilliseconds => stopwatch.ElapsedMilliseconds;

        public void Start(IEnumerable<VISION_MENU> menus, int maxDocumentsToCreate = int.MaxValue)
        {
            if (started || !isHostLoaded())
            {
                return;
            }

            started = true;
            cancelled = false;
            IsCompleted = false;
            CreatedCount = documentCache.Count;
            createdThisRun = 0;
            creationBudget = Math.Max(0, maxDocumentsToCreate);
            queue.Clear();
            foreach (VISION_MENU menu in menus ?? Array.Empty<VISION_MENU>())
            {
                queue.Enqueue(menu);
            }

            stopwatch.Restart();
            // Warm the first registered heavy tool before early operator input; later tools stay sliced in Background.
            ScheduleNext(DispatcherPriority.Loaded);
        }

        public void Cancel()
        {
            cancelled = true;
            queue.Clear();
            stopwatch.Stop();
            started = false;
            IsCompleted = false;
        }

        private void ScheduleNext(DispatcherPriority priority = DispatcherPriority.Background)
        {
            dispatcher.BeginInvoke(new Action(PrewarmNext), priority);
        }

        private void PrewarmNext()
        {
            if (cancelled || !isHostLoaded())
            {
                return;
            }

            // Respect a creation budget: fast-start tools get cached first, less common tools can still be created on demand.
            while (queue.Count > 0 && createdThisRun < creationBudget)
            {
                VISION_MENU menu = queue.Dequeue();
                bool wasCached = documentCache.Contains(menu);
                int beforeCount = documentCache.Count;
                if (documentCache.TryGetOrCreate(menu, displayManager, out OpenVisionNativeToolDocument document))
                {
                    if (wasCached)
                    {
                        // Restarted prewarm must not rewarm documents already created by an operator click.
                        CreatedCount = documentCache.Count;
                        continue;
                    }

                    if (documentCache.Count > beforeCount)
                    {
                        createdThisRun++;
                    }

                    warmDocument(document);
                    CreatedCount = documentCache.Count;
                    break;
                }
            }

            if (queue.Count == 0 || createdThisRun >= creationBudget)
            {
                IsCompleted = true;
                stopwatch.Stop();
                return;
            }

            ScheduleNext();
        }
    }
}
