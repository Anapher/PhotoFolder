using System.Collections.Generic;

namespace PhotoFolder.Application.Dto.WorkerRequests
{
    public class ExecuteOperationsRequest
    {
        public ExecuteOperationsRequest(IReadOnlyList<IFileOperation> operations, bool removeFilesFromOutside, string photoDirectoryRoot)
        {
            Operations = operations;
            RemoveFilesFromOutside = removeFilesFromOutside;
            PhotoDirectoryRoot = photoDirectoryRoot;
        }

        public IReadOnlyList<IFileOperation> Operations { get; }
        public bool RemoveFilesFromOutside { get; }
        public string PhotoDirectoryRoot { get; }
    }
}
