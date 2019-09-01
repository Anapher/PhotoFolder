using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using PhotoFolder.Application.Dto;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Wpf.Services;
using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels.Models;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace PhotoFolder.Wpf.ViewModels
{
    public class ReviewOperationsDialogViewModel : DialogBase
    {
        private IReadOnlyList<FileOperationInfo>? _operations;
        private IPhotoDirectory? _photoDirectory;
        private bool _removeFilesFromOutside;
        private OperationStatistics? _statistics;
        private AsyncDelegateCommand? _executeCommand;

        public ReviewOperationsDialogViewModel(IExecuteOperationsWorker executeOperationsWorker)
        {
            ExecuteOperationsWorker = executeOperationsWorker;
        }

        public IExecuteOperationsWorker ExecuteOperationsWorker { get; }

        public IReadOnlyList<FileOperationInfo>? Operations
        {
            get => _operations;
            private set => SetProperty(ref _operations, value);
        }

        public OperationStatistics? Statistics
        {
            get => _statistics;
            private set => SetProperty(ref _statistics, value);
        }

        public bool RemoveFilesFromOutside
        {
            get => _removeFilesFromOutside;
            set => SetProperty(ref _removeFilesFromOutside, value);
        }

        public AsyncDelegateCommand ExecuteCommand
        {
            get
            {
                return _executeCommand ??= new AsyncDelegateCommand(async () =>
                {
                    var request = new ExecuteOperationsRequest(Operations.Select(x => x.Operation).ToList(), RemoveFilesFromOutside,
                        _photoDirectory!.RootDirectory);

                    await Task.Run(() => ExecuteOperationsWorker.Execute(request, CancellationToken.None));
                    OnRequestClose(new DialogResult(ButtonResult.OK));
                });
            }
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            _photoDirectory = parameters.GetValue<IPhotoDirectory>("photoDirectory");
            Operations = parameters.GetValue<IReadOnlyList<FileOperationInfo>>("operations");
            RemoveFilesFromOutside = parameters.GetValue<bool>("removeFilesFromOutside");
            Statistics = CreateStatistics(Operations, _removeFilesFromOutside);

            Title = $"Review {"operation".ToQuantity(Operations.Count)}";
        }

        private static OperationStatistics CreateStatistics(IReadOnlyList<FileOperationInfo> operations, bool deleteOutsideFiles)
        {
            var copyOperations = operations.Select(x => x.Operation).OfType<MoveFileOperation>()
                .Count(x => !deleteOutsideFiles && x.File.RelativeFilename == null);
            var moveOperations = operations.Select(x => x.Operation).OfType<MoveFileOperation>()
                .Count(x => x.File.RelativeFilename != null || deleteOutsideFiles);
            var deleteOperations = operations.Select(x => x.Operation).OfType<DeleteFileOperation>().Count();
            var deletedExternalFiles = operations.Select(x => x.Operation).OfType<DeleteFileOperation>().Count(x => x.File.RelativeFilename == null);
            var newFiles = operations.Count(x => x.FileBaseChange == FileBaseChange.NewFile);
            var deletedFilesFromPhotoDirectory = operations.Count(x => x.FileBaseChange == FileBaseChange.FileDeleted);

            return new OperationStatistics(copyOperations, moveOperations, deleteOperations, deletedExternalFiles, newFiles, deletedFilesFromPhotoDirectory);
        }
    }

    public class OperationStatistics
    {
        public OperationStatistics(int copyOperations, int moveOperations, int deleteOperations, int deletedExternalFiles, int newFiles,
            int deletedFilesOfPhotoDirectory)
        {
            CopyOperations = copyOperations;
            MoveOperations = moveOperations;
            DeleteOperations = deleteOperations;
            DeletedExternalFiles = deletedExternalFiles;
            NewFiles = newFiles;
            DeletedFilesOfPhotoDirectory = deletedFilesOfPhotoDirectory;
        }

        public int CopyOperations { get; }
        public int MoveOperations { get; }
        public int DeleteOperations { get; }
        public int DeletedExternalFiles { get; }
        public int NewFiles { get; }
        public int DeletedFilesOfPhotoDirectory { get; }
    }
}
