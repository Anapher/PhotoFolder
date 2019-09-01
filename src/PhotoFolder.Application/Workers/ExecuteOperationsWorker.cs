using System.Collections.Concurrent;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PhotoFolder.Application.Dto;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Application.Dto.WorkerStates;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Application.Utilities;

namespace PhotoFolder.Application.Workers
{
    public class ExecuteOperationsWorker : IExecuteOperationsWorker
    {
        private readonly IFileSystem _fileSystem;

        public ExecuteOperationsWorker(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public ExecuteOperationsState State { get; } = new ExecuteOperationsState();

        public async Task<ExecuteOperationsResponse> Execute(ExecuteOperationsRequest request, CancellationToken cancellationToken = default)
        {
            var deletedDirectories = new ConcurrentDictionary<string, byte>(); // hashset
            State.TotalOperations = request.Operations.Count;

            var result = await TaskCombinators.ThrottledCatchErrorsAsync(request.Operations, (operation, _) =>
            {
                try
                {
                    if (operation is DeleteFileOperation deleteFileOperation)
                    {
                        if (operation.File.RelativeFilename == null && !request.RemoveFilesFromOutside) return Task.CompletedTask;

                        _fileSystem.File.Delete(deleteFileOperation.File.Filename);
                        deletedDirectories.TryAdd(_fileSystem.Path.GetDirectoryName(operation.File.Filename), default);
                    }
                    else if (operation is MoveFileOperation moveFileOperation)
                    {
                        var targetPath = _fileSystem.Path.Combine(request.PhotoDirectoryRoot, moveFileOperation.TargetPath);

                        _fileSystem.Directory.CreateDirectory(_fileSystem.Path.GetDirectoryName(targetPath));

                        if (operation.File.RelativeFilename == null && !request.RemoveFilesFromOutside)
                            _fileSystem.File.Copy(moveFileOperation.File.Filename, targetPath, false);
                        else
                        {
                            _fileSystem.File.Move(moveFileOperation.File.Filename, targetPath);
                            deletedDirectories.TryAdd(_fileSystem.Path.GetDirectoryName(operation.File.Filename), default);
                        }
                    }
                }
                finally
                {
                    State.OnOperationProcessed();
                }

                return Task.CompletedTask;
            }, cancellationToken);

            foreach (var deletedDirectory in deletedDirectories.Keys)
            {
                if (!_fileSystem.Directory.EnumerateFiles(deletedDirectory, "*", SearchOption.AllDirectories).Any())
                    _fileSystem.Directory.Delete(deletedDirectory, true);
            }

            return new ExecuteOperationsResponse(result);
        }
    }
}
