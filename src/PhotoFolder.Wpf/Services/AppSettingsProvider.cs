using Newtonsoft.Json;
using System;
using System.IO.Abstractions;

namespace PhotoFolder.Wpf.Services
{
    public class AppSettingsProvider : IAppSettingsProvider
    {
        private readonly IFileSystem _fileSystem;
        private AppSettings? _settings;
        private readonly string _filename;

        public AppSettingsProvider(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _filename = _fileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PhotoFolder", "settings.json");
        }

        public AppSettings Current { get => _settings ?? (_settings = Load()); }

        public void Save(AppSettings appSettings)
        {
            var content = JsonConvert.SerializeObject(appSettings);
            _fileSystem.Directory.CreateDirectory(_fileSystem.Path.GetDirectoryName(_filename));
            _fileSystem.File.WriteAllText(_filename, content);

            _settings = appSettings;
        }

        private AppSettings Load()
        {
            var settingsFile = _fileSystem.FileInfo.FromFileName(_filename);
            if (settingsFile.Exists)
            {
                return JsonConvert.DeserializeObject<AppSettings>(_fileSystem.File.ReadAllText(_filename));
            }
            else
            {
                return new AppSettings();
            }
        }
    }
}
