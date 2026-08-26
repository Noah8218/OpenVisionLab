using System;
using System.Drawing;
using System.Threading;

namespace OpenVisionLab.ImageSpace.Core
{
    public sealed class ImageSpaceImageLease : IDisposable
    {
        private ImageSpaceImage owner;

        internal ImageSpaceImageLease(ImageSpaceImage owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public Bitmap Image => owner?.Image ?? throw new ObjectDisposedException(nameof(ImageSpaceImageLease));

        public void Dispose()
        {
            ImageSpaceImage current = Interlocked.Exchange(ref owner, null);
            current?.Release();
        }
    }

    internal sealed class ImageSpaceImage
    {
        private Bitmap image;
        private int referenceCount = 1;

        public ImageSpaceImage(Bitmap image)
        {
            this.image = image ?? throw new ArgumentNullException(nameof(image));
        }

        public Bitmap Image => Volatile.Read(ref image)
            ?? throw new ObjectDisposedException(nameof(ImageSpaceImage));

        public bool References(Bitmap candidate) => ReferenceEquals(Volatile.Read(ref image), candidate);

        public ImageSpaceImageLease Acquire()
        {
            while (true)
            {
                int count = Volatile.Read(ref referenceCount);
                if (count == 0)
                {
                    return null;
                }

                if (Interlocked.CompareExchange(ref referenceCount, count + 1, count) == count)
                {
                    return new ImageSpaceImageLease(this);
                }
            }
        }

        public void Release()
        {
            if (Interlocked.Decrement(ref referenceCount) != 0)
            {
                return;
            }

            Interlocked.Exchange(ref image, null)?.Dispose();
        }
    }
}
