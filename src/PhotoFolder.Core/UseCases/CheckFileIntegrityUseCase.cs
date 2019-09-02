#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Core.Interfaces.UseCases;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoFolder.Core.Dto.Services;

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
            var fileInformation = message.FileInformation;

            await using var context = message.FileBaseContext.PhotoDirectory.GetDataContext();

            var issues = new List<IFileIssue>(3);
            foreach (var validator in _fileIntegrityValidators)
            {
                issues.AddRange(await validator.CheckForIssues(fileInformation, message.FileBaseContext, context));
            }

            return new CheckFileIntegrityResponse(fileInformation, issues);
        }
    }
}
