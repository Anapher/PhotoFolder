namespace PhotoFolder.Wpf.Services
{
    public interface IAppSettingsProvider
    {
        AppSettings Current { get; }

        void Save(AppSettings appSettings);
    }
}