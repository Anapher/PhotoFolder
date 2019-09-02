using PhotoFolder.Core.Domain;
using System.Collections.Generic;
using System.Linq;

namespace PhotoFolder.Core.Dto.Services.FileIssue
{
    public class FormerlyDeletedIssue : IFileIssue
    {
        public FormerlyDeletedIssue(FileInformation file, DeletedFileInfo deletedFileInfo)
        {
            File = file;
            DeletedFileInfo = deletedFileInfo;
        }

        public string Identity => $"FormerlyDeleted:{File.Hash}";

        public FileInformation File { get; }
        public DeletedFileInfo DeletedFileInfo { get; }

        public IEnumerable<FileInformation> RelevantFiles => Enumerable.Empty<FileInformation>();
    }
}
