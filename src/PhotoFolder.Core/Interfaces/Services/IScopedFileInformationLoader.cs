namespace PhotoFolder.Core.Interfaces.Services
{
    public interface IScopedFileInformationLoader
    {
        IFileInformationLoader Build();
    }
}
