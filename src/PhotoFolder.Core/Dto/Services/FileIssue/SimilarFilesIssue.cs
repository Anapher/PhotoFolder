using PhotoFolder.Core.Dto.UseCaseResponses;
using System.Collections.Generic;
using System.Linq;
using PhotoFolder.Core.Extensions;

namespace PhotoFolder.Core.Dto.Services.FileIssue
{
    public class SimilarFilesIssue : IFileIssue
    {
        public SimilarFilesIssue(FileInformation file, IEnumerable<SimilarFile> similarFiles)
        {
            SimilarFiles = similarFiles;
            File = file;
        }

        public IEnumerable<SimilarFile> SimilarFiles { get; }
        public FileInformation File { get; }
        public IEnumerable<FileInformation> RelevantFiles => SimilarFiles.Select(x => x.FileInfo);

        public string Identity => string.Join(";", RelevantFiles.Concat(File.Yield()).Select(x => x.Hash.ToString()).OrderBy(x => x));
    }
}
