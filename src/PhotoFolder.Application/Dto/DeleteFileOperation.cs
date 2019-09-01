using PhotoFolder.Core.Dto.Services;

namespace PhotoFolder.Application.Dto
{
    public class DeleteFileOperation : IFileOperation
    {
        public DeleteFileOperation(FileInformation file)
        {
            File = file;
        }

        public FileInformation File { get; }
    }
}
