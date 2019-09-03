using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Application.Dto.WorkerStates;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Application.Utilities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Services;

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

            var fileInfos = new FileInformation[request.Files.Count];
            var counter = -1; // as we always increment and the first index is zero

            await TaskCombinators.ThrottledAsync(request.Files, async (filename, _) =>
            {
                var index = Interlocked.Increment(ref counter);
                try
                {
                    var file = request.Directory.GetFile(filename);
                    if (file == null) return; // not found

                    fileInfos[index] = await _fileInformationLoader.Load(file);
                }
                finally
                {
                    State.Progress = (double) counter / request.Files.Count;
                }
            }, cancellationToken, 4);

            State.Status = ImportFilesStatus.Querying;
            _checkFilesWorker.State.PropertyChanged += (_, __) => State.Progress = _checkFilesWorker.State.Progress;

            return await _checkFilesWorker.Execute(new CheckFilesRequest(fileInfos.Where(x => x != null).ToList(), request.Directory), cancellationToken);
        }
    }
}
