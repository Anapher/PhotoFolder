using PhotoFolder.Core.Dto.Services;

namespace PhotoFolder.Application.Dto
{
    public interface IFileOperation
    {
        FileInformation File { get; }
    }
}