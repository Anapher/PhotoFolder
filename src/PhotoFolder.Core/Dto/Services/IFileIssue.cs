using System.Collections.Generic;

namespace PhotoFolder.Core.Dto.Services
{
    public interface IFileIssue
    {
        string Identity { get; }
        FileInformation File { get; }
        IEnumerable<FileInformation> RelevantFiles { get; }
    }
}
