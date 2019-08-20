using System.Collections.Generic;
using PhotoFolder.Core.Dto.Services;

namespace PhotoFolder.Application.Dto.WorkerResponses
{
    public class FileCheckReport
    {
        public FileCheckReport(IReadOnlyList<IFileIssue> issues)
        {
            Issues = issues;
        }

        public IReadOnlyList<IFileIssue> Issues { get; }
    }
}
