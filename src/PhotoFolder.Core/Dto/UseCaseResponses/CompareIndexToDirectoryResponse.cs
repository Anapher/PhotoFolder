using PhotoFolder.Core.Dto.Services;
using System.Collections.Generic;

namespace PhotoFolder.Core.Dto.UseCaseResponses
{
    public class CompareIndexToDirectoryResponse
    {
        public CompareIndexToDirectoryResponse(IEnumerable<IFileInfo> newFiles, IEnumerable<IFileInfo> removedFiles)
        {
            NewFiles = newFiles;
            RemovedFiles = removedFiles;
        }

        public IEnumerable<IFileInfo> NewFiles { get; }
        public IEnumerable<IFileInfo> RemovedFiles { get; }
    }
}
