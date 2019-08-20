using PhotoFolder.Core.Dto.Services;
using System.Collections.Generic;

namespace PhotoFolder.Core.Dto.UseCaseResponses
{
    public class CheckFileIntegrityResponse
    {
        public CheckFileIntegrityResponse(FileInformation file, IReadOnlyList<IFileIssue> issues)
        {
            File = file;
            Issues = issues;
        }

        public FileInformation File { get; }
        public IReadOnlyList<IFileIssue> Issues { get; }
    }

    public class SimilarFile
    {
        public SimilarFile(FileInformation fileInfo, float similarity)
        {
            FileInfo = fileInfo;
            Similarity = similarity;
        }

        public FileInformation FileInfo { get; }
        public float Similarity { get; }
    }
}
