using Microsoft.Extensions.DependencyInjection;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Application.Dto.WorkerStates;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Interfaces.UseCases;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Application.Utilities;
using PhotoFolder.Core.Interfaces.Services;

namespace PhotoFolder.Application.Workers
{
    public class CheckFilesWorker : ICheckFilesWorker
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IFileBaseContextFactory _fileBaseContextFactory;

        public CheckFilesWorker(IServiceProvider serviceProvider, IFileBaseContextFactory fileBaseContextFactory)
        {
            State = new CheckFilesState();
            _serviceProvider = serviceProvider;
            _fileBaseContextFactory = fileBaseContextFactory;
        }

        public CheckFilesState State { get; }

        public async Task<FileCheckReport> Execute(CheckFilesRequest request, CancellationToken cancellationToken = default)
        {
            var directory = request.Directory;
            var fileInfos = request.Files;

            // get all files from the repository
            var context = await _fileBaseContextFactory.BuildContext(directory);
            State.Status = CheckFilesStatus.Querying;

            var result = new ConcurrentBag<IFileIssue>();
            var counter = -1;

            State.TotalFiles = context.IndexedFiles.Count;

            await TaskCombinators.ThrottledAsync(fileInfos, async (fileInformation, token) =>
            {
                var useCase = _serviceProvider.GetRequiredService<ICheckFileIntegrityUseCase>();
                var response = await useCase.Handle(new CheckFileIntegrityRequest(fileInformation, context));
                if (useCase.HasError)
                {
                    State.Errors.Add(fileInformation, useCase.Error!);
                }
                else
                {
                    foreach (var fileIssue in response!.Issues)
                        result.Add(fileIssue);
                }

                State.FilesProcessed = Interlocked.Increment(ref counter);
            }, cancellationToken);

            return new FileCheckReport(result.Distinct(new EqualityComparerByValue<IFileIssue, string>(x => x.Identity)).ToList());
        }
    }
}
