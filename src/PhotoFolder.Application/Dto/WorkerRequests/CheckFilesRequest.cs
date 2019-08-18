using PhotoFolder.Core.Interfaces.Gateways;
using System.Collections.Generic;
using PhotoFolder.Core.Dto.Services;

namespace PhotoFolder.Application.Dto.WorkerRequests
{
    public class CheckFilesRequest
    {
        public CheckFilesRequest(IReadOnlyList<FileInformation> files, IPhotoDirectory directory)
        {
            Files = files;
            Directory = directory;
        }

        public IReadOnlyList<FileInformation> Files { get; }
        public IPhotoDirectory Directory { get; }
    }
}
