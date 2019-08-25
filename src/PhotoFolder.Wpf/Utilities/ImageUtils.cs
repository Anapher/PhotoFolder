using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PhotoFolder.Wpf.Utilities
{
    public static class ImageUtils
    {
        public static Task<BitmapImage> LoadImageAsync(string filename)
        {
            return Task.Run(() =>
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.UriSource = new Uri(filename);
                img.EndInit();
                img.Freeze();

                return img;
            });
        }
    }
}
