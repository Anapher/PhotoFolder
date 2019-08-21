#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Core.Interfaces.UseCases;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoFolder.Core.UseCases
{
    public class CheckFileIntegrityUseCase : UseCaseStatus<CheckFileIntegrityResponse>, ICheckFileIntegrityUseCase
    {
        private readonly IEnumerable<IFileIntegrityValidator> _fileIntegrityValidators;

        public CheckFileIntegrityUseCase(IEnumerable<IFileIntegrityValidator> fileIntegrityValidators)
        {
            _fileIntegrityValidators = fileIntegrityValidators;
        }

        public async Task<CheckFileIntegrityResponse?> Handle(CheckFileIntegrityRequest message)
        {
            var directory = message.PhotoDirectory;
            var fileInformation = message.FileInformation;
            var indexedFiles = message.IndexedFiles;

            var issues = _fileIntegrityValidators.SelectMany(x => x.CheckForIssues(fileInformation, directory, indexedFiles)).ToList();
            return new CheckFileIntegrityResponse(fileInformation, issues);
        }
    }
}
