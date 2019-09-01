namespace PhotoFolder.Core.Domain
{
    public class PhotoProperties
    {
        public PhotoProperties(Hash bitmapHash, int width, int height)
        {
            BitmapHash = bitmapHash;
            Width = width;
            Height = height;
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        private PhotoProperties()
        {
        }

#pragma warning enable CS8618

        public Hash BitmapHash { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}
