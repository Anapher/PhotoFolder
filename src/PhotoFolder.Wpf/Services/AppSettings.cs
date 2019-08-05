using Newtonsoft.Json;

namespace PhotoFolder.Wpf.Services
{
    public class AppSettings
    {
        [JsonProperty]
        public string? LatestPhotoFolder { get; private set; }

        public AppSettings SetLatestPhotoFolder(string? path)
        {
            var newSettings = new AppSettings();
            newSettings.LatestPhotoFolder = path;

            return newSettings;
        }
    }
}
