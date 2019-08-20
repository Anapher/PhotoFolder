using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.Gateways;
using System.Collections.Generic;

namespace PhotoFolder.Core.Dto.UseCaseRequests
{
    public class CheckFileIntegrityRequest : IUseCaseRequest<CheckFileIntegrityResponse>
    {
        public CheckFileIntegrityRequest(FileInformation fileInformation, IReadOnlyList<IndexedFile> indexedFiles, IPhotoDirectory photoDirectory)
        {
            FileInformation = fileInformation;
            IndexedFiles = indexedFiles;
            PhotoDirectory = photoDirectory;
        }

        public FileInformation FileInformation { get; }
        public IReadOnlyList<IndexedFile> IndexedFiles { get; }
        public IPhotoDirectory PhotoDirectory { get; }
    }
}
