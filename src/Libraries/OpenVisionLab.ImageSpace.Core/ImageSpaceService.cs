using System;
using System.Collections.Generic;
using System.Drawing;

namespace OpenVisionLab.ImageSpace.Core
{
    public sealed class ImageSpaceService : IImageSpace
    {
        private readonly object sync = new object();
        private readonly List<ImageSpaceItem> items = new List<ImageSpaceItem>();
        private Bitmap activeImage;
        private bool disposed;

        public void SetActiveImage(Bitmap image)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                activeImage = image;
            }
        }

        public Bitmap GetActiveImage()
        {
            lock (sync)
            {
                ThrowIfDisposed();
                return activeImage;
            }
        }

        public void SetImage(int index, string title, Bitmap image)
        {
            ImageSpaceImage previous;
            lock (sync)
            {
                ThrowIfDisposed();
                ImageSpaceItem item = GetOrCreate(index);
                item.Title = title ?? string.Empty;
                previous = item.Image;
                if (previous?.References(image) == true)
                {
                    return;
                }

                item.Image = image == null ? null : new ImageSpaceImage(image);
                if (previous?.References(activeImage) == true)
                {
                    activeImage = image;
                }
            }

            previous?.Release();
        }

        public void InsertImage(int index, string title, Bitmap image)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                int insertIndex = index < 0 ? items.Count : index;
                while (items.Count < insertIndex)
                {
                    items.Add(new ImageSpaceItem());
                }

                if (insertIndex > items.Count)
                {
                    insertIndex = items.Count;
                }

                items.Insert(insertIndex, new ImageSpaceItem
                {
                    Title = title ?? string.Empty,
                    Image = image == null ? null : new ImageSpaceImage(image)
                });
            }
        }

        public Bitmap GetImage(int index)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                return GetOrNull(index)?.Image?.Image;
            }
        }

        public Bitmap GetImage(string title)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                return FindByTitle(title)?.Image?.Image;
            }
        }

        public ImageSpaceImageLease AcquireImage(int index)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                return GetOrNull(index)?.Image?.Acquire();
            }
        }

        public ImageSpaceImageLease AcquireImage(string title)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                return FindByTitle(title)?.Image?.Acquire();
            }
        }

        public void RemoveImage(int index)
        {
            ImageSpaceImage removed;
            lock (sync)
            {
                ThrowIfDisposed();
                if (index < 0 || index >= items.Count)
                {
                    return;
                }

                ImageSpaceItem item = items[index];
                removed = item.Image;
                if (removed?.References(activeImage) == true)
                {
                    activeImage = null;
                }

                items.RemoveAt(index);
            }

            removed?.Release();
        }

        public void RemoveImage(string title)
        {
            int index;
            lock (sync)
            {
                ThrowIfDisposed();
                index = FindIndexByTitle(title);
            }

            if (index >= 0)
            {
                RemoveImage(index);
            }
        }

        public void SetRoi(int index, Rectangle roi)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                GetOrCreate(index).Roi = roi;
            }
        }

        public Rectangle GetRoi(int index)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                return GetOrNull(index)?.Roi ?? Rectangle.Empty;
            }
        }

        public Rectangle GetRoi(string title)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                return FindByTitle(title)?.Roi ?? Rectangle.Empty;
            }
        }

        public void SetTrainRoi(int index, Rectangle roi)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                GetOrCreate(index).TrainRoi = roi;
            }
        }

        public Rectangle GetTrainRoi(int index)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                return GetOrNull(index)?.TrainRoi ?? Rectangle.Empty;
            }
        }

        public Rectangle GetTrainRoi(string title)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                return FindByTitle(title)?.TrainRoi ?? Rectangle.Empty;
            }
        }

        public void MarkImageChanged(string title, bool changed)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                ImageSpaceItem item = FindByTitle(title);
                if (item != null)
                {
                    item.ImageChanged = changed;
                }
            }
        }

        public bool IsImageChanged(string title)
        {
            lock (sync)
            {
                ThrowIfDisposed();
                return FindByTitle(title)?.ImageChanged ?? false;
            }
        }

        public void AcceptImageChanged(string title)
        {
            MarkImageChanged(title, false);
        }

        public void Dispose()
        {
            List<ImageSpaceImage> ownedImages = new List<ImageSpaceImage>();
            lock (sync)
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                foreach (ImageSpaceItem item in items)
                {
                    if (item.Image != null)
                    {
                        ownedImages.Add(item.Image);
                    }
                }

                items.Clear();
                activeImage = null;
            }

            foreach (ImageSpaceImage image in ownedImages)
            {
                image.Release();
            }
        }

        private ImageSpaceItem GetOrCreate(int index)
        {
            while (items.Count <= index)
            {
                items.Add(new ImageSpaceItem());
            }

            return items[index];
        }

        private ImageSpaceItem GetOrNull(int index)
        {
            if (index < 0 || index >= items.Count) return null;
            return items[index];
        }

        private ImageSpaceItem FindByTitle(string title)
        {
            int index = FindIndexByTitle(title);
            return index >= 0 ? items[index] : null;
        }

        private int FindIndexByTitle(string title)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Title == title) return i;
            }

            return -1;
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(ImageSpaceService));
            }
        }
    }
}
