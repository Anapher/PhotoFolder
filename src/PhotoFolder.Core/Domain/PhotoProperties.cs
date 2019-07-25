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

        private PhotoProperties()
        {
        }

        public Hash BitmapHash { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}
