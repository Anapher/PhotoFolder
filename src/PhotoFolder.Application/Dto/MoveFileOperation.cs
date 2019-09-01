using PhotoFolder.Core.Dto.Services;

namespace PhotoFolder.Application.Dto
{
    public class MoveFileOperation : IFileOperation
    {
        public MoveFileOperation(FileInformation file, string targetPath)
        {
            File = file;
            TargetPath = targetPath;
        }

        public FileInformation File { get; }
        public string TargetPath { get; }
    }
}