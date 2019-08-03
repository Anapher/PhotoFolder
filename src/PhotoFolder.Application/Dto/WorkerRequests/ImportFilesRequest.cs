using PhotoFolder.Core.Interfaces.Gateways;
using System.Collections.Generic;

namespace PhotoFolder.Application.Dto.WorkerRequests
{
    public class ImportFilesRequest
    {
        public ImportFilesRequest(IReadOnlyList<string> files, IPhotoDirectory directory)
        {
            Files = files;
            Directory = directory;
        }

        public IReadOnlyList<string> Files { get; }
        public IPhotoDirectory Directory { get; }
    }
}
