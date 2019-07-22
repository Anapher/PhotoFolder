using System;

namespace PhotoFolder.Core.Domain
{
    public class PhotoProperties
    {
        public PhotoProperties(Hash bitmapHash, DateTimeOffset dateTaken, int width, int height)
        {
            BitmapHash = bitmapHash;
            DateTaken = dateTaken;
            Width = width;
            Height = height;
        }

        private PhotoProperties()
        {
        }

        public Hash BitmapHash { get; private set; }
        public DateTimeOffset DateTaken { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}
