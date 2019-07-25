using PhotoFolder.Core.Domain;

namespace PhotoFolder.Core.Dto.Services
{
    public interface IFileContentInfo
    {
        long Length { get; }
        Hash Hash { get; }
        PhotoProperties? PhotoProperties { get; }
    }
}
