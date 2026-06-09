using System.Collections.Generic;
using System.Drawing;

namespace OpenVisionLab.ImageSpace.Core
{
    public sealed class ImageSpaceService : IImageSpace
    {
        private readonly List<ImageSpaceItem> items = new List<ImageSpaceItem>();
        private Bitmap activeImage;

        public void SetActiveImage(Bitmap image)
        {
            activeImage = image;
        }

        public Bitmap GetActiveImage()
        {
            return activeImage;
        }

        public void SetImage(int index, string title, Bitmap image)
        {
            ImageSpaceItem item = GetOrCreate(index);
            item.Title = title ?? string.Empty;
            item.Image = image;
        }

        public Bitmap GetImage(int index)
        {
            return GetOrNull(index)?.Image;
        }

        public Bitmap GetImage(string title)
        {
            return FindByTitle(title)?.Image;
        }

        public void SetRoi(int index, Rectangle roi)
        {
            GetOrCreate(index).Roi = roi;
        }

        public Rectangle GetRoi(int index)
        {
            return GetOrNull(index)?.Roi ?? Rectangle.Empty;
        }

        public Rectangle GetRoi(string title)
        {
            return FindByTitle(title)?.Roi ?? Rectangle.Empty;
        }

        public void SetTrainRoi(int index, Rectangle roi)
        {
            GetOrCreate(index).TrainRoi = roi;
        }

        public Rectangle GetTrainRoi(int index)
        {
            return GetOrNull(index)?.TrainRoi ?? Rectangle.Empty;
        }

        public Rectangle GetTrainRoi(string title)
        {
            return FindByTitle(title)?.TrainRoi ?? Rectangle.Empty;
        }

        public void MarkImageChanged(string title, bool changed)
        {
            ImageSpaceItem item = FindByTitle(title);
            if (item != null)
            {
                item.ImageChanged = changed;
            }
        }

        public bool IsImageChanged(string title)
        {
            return FindByTitle(title)?.ImageChanged ?? false;
        }

        public void AcceptImageChanged(string title)
        {
            MarkImageChanged(title, false);
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
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Title == title) return items[i];
            }

            return null;
        }
    }
}
