using System.Collections.Generic;

namespace PhotoFolder.Core.Dto.Services.FileIssue
{
    public class DuplicateFilesIssue : IFileIssue
    {
        public DuplicateFilesIssue(FileInformation file, IEnumerable<FileInformation> relevantFiles)
        {
            File = file;
            RelevantFiles = relevantFiles;
        }

        public FileInformation File { get; }
        public IEnumerable<FileInformation> RelevantFiles { get; }
        public string Identity => File.Hash.ToString();
    }
}
