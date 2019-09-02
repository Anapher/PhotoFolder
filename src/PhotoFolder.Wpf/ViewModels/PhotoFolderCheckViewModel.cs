using System;
using System.Threading;
using System.Threading.Tasks;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Application.Dto.WorkerStates;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Core.Interfaces.Gateways;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace PhotoFolder.Wpf.ViewModels
{
    public class PhotoFolderCheckViewModel : BindableBase
    {
        private readonly ICheckIndexWorker _checkIndexWorker;
        private readonly IDialogService _dialogService;
        private readonly IRegionManager _regionManager;
        private FileCheckReport? _fileCheckReport;
        private IPhotoDirectory? _photoDirectory;
        private DelegateCommand? _resolveIssuesCommand;

        public PhotoFolderCheckViewModel(ICheckIndexWorker checkIndexWorker, IDialogService dialogService, IRegionManager regionManager)
        {
            _checkIndexWorker = checkIndexWorker;
            _dialogService = dialogService;
            _regionManager = regionManager;
            State = checkIndexWorker.State;
        }

        public CheckFilesState State { get; }

        public FileCheckReport? FileCheckReport
        {
            get => _fileCheckReport;
            private set => SetProperty(ref _fileCheckReport, value);
        }

        public DelegateCommand ResolveIssuesCommand
        {
            get
            {
                return _resolveIssuesCommand ??= new DelegateCommand(() =>
                {
                    var parameters = new DialogParameters
                    {
                        {"report", _fileCheckReport ?? throw new InvalidOperationException()},
                        {"photoDirectory", _photoDirectory ?? throw new InvalidOperationException()}
                    };

                    _dialogService.ShowDialog("DecisionManager", parameters, result =>
                    {
                        if (result.Result == ButtonResult.OK)
                        {
                            var parameters = new NavigationParameters {{"photoDirectory", _photoDirectory}};
                            _regionManager.RequestNavigate(RegionNames.MainView, "SynchronizeFolderView", parameters);
                        }
                    });
                });
            }
        }

        public async void Initialize(IPhotoDirectory photoDirectory)
        {
            _photoDirectory = photoDirectory;
            FileCheckReport = await Task.Run(() => _checkIndexWorker.Execute(new CheckIndexRequest(photoDirectory), CancellationToken.None));
        }
    }
}
