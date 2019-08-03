using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Application.Dto.WorkerStates;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoFolder.Application.Workers
{
    public class ImportFilesWorker : IImportFilesWorker
    {
        private readonly ICheckFilesWorker _checkFilesWorker;
        private readonly IFileInformationLoader _fileInformationLoader;

        public ImportFilesWorker(ICheckFilesWorker checkFilesWorker, IFileInformationLoader fileInformationLoader)
        {
            _checkFilesWorker = checkFilesWorker;
            _fileInformationLoader = fileInformationLoader;
        }

        public ImportFilesState State { get; } = new ImportFilesState();

        public async Task<FileCheckReport> Execute(ImportFilesRequest request, CancellationToken cancellationToken = default)
        {
            State.Status = ImportFilesStatus.Scanning;

            var fileInfos = new List<FileInformation>(request.Files.Count);
            for (int i = 0; i < request.Files.Count; i++)
            {
                var filename = request.Files[i];
                var file = request.Directory.GetFile(filename);
                if (file == null) continue; // not found

                var fileInfo = await _fileInformationLoader.Load(file);
                fileInfos.Add(fileInfo);

                State.Progress = (double)i / request.Files.Count;
            }

            State.Status = ImportFilesStatus.Querying;
            _checkFilesWorker.State.PropertyChanged += (_, __) => State.Progress = _checkFilesWorker.State.Progress;

            return await _checkFilesWorker.Execute(new CheckFilesRequest(fileInfos, request.Directory));
        }
    }
}
