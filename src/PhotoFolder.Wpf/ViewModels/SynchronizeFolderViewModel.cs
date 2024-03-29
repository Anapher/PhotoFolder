﻿using System;
using System.Threading.Tasks;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Dto.WorkerStates;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Wpf.Extensions;
using PhotoFolder.Wpf.Services;
using Prism.Mvvm;
using Prism.Regions;

namespace PhotoFolder.Wpf.ViewModels
{
    public class SynchronizeFolderViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly PhotoFolderSynchronizationEvent _synchronizationEvent;
        private readonly ISynchronizeIndexWorker _synchronizeIndexWorker;
        private readonly IWindowService _windowService;

        public SynchronizeFolderViewModel(ISynchronizeIndexWorker synchronizeIndexWorker, IWindowService windowService,
            IRegionManager regionManager, PhotoFolderSynchronizationEvent synchronizationEvent)
        {
            _synchronizeIndexWorker = synchronizeIndexWorker;
            _windowService = windowService;
            _regionManager = regionManager;
            _synchronizationEvent = synchronizationEvent;
        }

        public SynchronizeIndexState State => _synchronizeIndexWorker.State;

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var photoDirectory = navigationContext.Parameters.GetValue<IPhotoDirectory>("photoDirectory");
            ExecuteWorker(photoDirectory);
        }

        private async Task ExecuteWorker(IPhotoDirectory photoDirectory)
        {
            try
            {
                await _synchronizeIndexWorker.Execute(new SynchronizeIndexRequest(photoDirectory));
            }
            catch (Exception e)
            {
                _windowService.ShowError(e);
                _regionManager.RequestNavigate(RegionNames.MainView, "OpenFolderView");
                return;
            }

            _synchronizationEvent.OnPhotoFolderSynchronized();

            var parameters = new NavigationParameters {{"photoDirectory", photoDirectory}};
            _regionManager.RequestNavigate(RegionNames.MainView, "PhotoFolderView", parameters);
        }
    }
}