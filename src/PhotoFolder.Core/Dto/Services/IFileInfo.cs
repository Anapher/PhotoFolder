namespace PhotoFolder.Core.Dto.Services
{
    public interface IFileInfo
    {
        string Filename { get; }
        long Length { get; }
    }
}
