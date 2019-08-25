using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Application.Dto.WorkerStates;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Specifications.FileInformation;
using PhotoFolder.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoFolder.Application.Workers
{
    public class CheckIndexWorker : ICheckIndexWorker
    {
        private readonly ICheckFilesWorker _checkFilesWorker;

        public CheckIndexWorker(ICheckFilesWorker checkFilesWorker)
        {
            _checkFilesWorker = checkFilesWorker;

            State = checkFilesWorker.State;
        }

        public CheckFilesState State { get; }

        public async Task<FileCheckReport> Execute(CheckIndexRequest request, CancellationToken cancellationToken = default)
        {
            // get all files from the repository
            IReadOnlyList<IndexedFile> indexedFiles;
            using (var context = request.Directory.GetDataContext())
            {
                indexedFiles = await context.FileRepository.GetAllBySpecs(new IncludeFileLocationsSpec());
            }

            var fileInfos = indexedFiles.SelectMany(x => x.Files.Select(y => x.ToFileInformation(request.Directory))).ToList();
            return await _checkFilesWorker.Execute(new CheckFilesRequest(fileInfos, request.Directory));
        }
    }
}
