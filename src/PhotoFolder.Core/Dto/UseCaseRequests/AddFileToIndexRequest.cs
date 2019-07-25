using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.Gateways;
using System.Collections.Immutable;

namespace PhotoFolder.Core.Dto.UseCaseRequests
{
    public class AddFileToIndexRequest : IUseCaseRequest<AddFileToIndexResponse>
    {
        public AddFileToIndexRequest(string filename, IPhotoDirectory directory,
           IImmutableList<FileInformation>? removedFiles = null)
        {
            Filename = filename;
            Directory = directory;

            RemovedFiles = removedFiles ?? ImmutableList<FileInformation>.Empty;
        }

        public string Filename { get; }
        public IPhotoDirectory Directory { get; }

        public IImmutableList<FileInformation> RemovedFiles { get; }
    }
}
