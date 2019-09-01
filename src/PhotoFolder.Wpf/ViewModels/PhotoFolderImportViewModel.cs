using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Wpf.Extensions;
using PhotoFolder.Wpf.Services;
using PhotoFolder.Wpf.Utilities;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace PhotoFolder.Wpf.ViewModels
{
    public class PhotoFolderImportViewModel : BindableBase
    {
        private static readonly string[] ImageExtensions = {".bmp", ".jpg", ".png", ".gif"};
        private readonly IFileSystem _fileSystem;
        private readonly IDialogService _dialogService;
        private readonly IRegionManager _regionManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWindowService _windowService;

        private AsyncDelegateCommand? _openFilesCommand;
        private AsyncDelegateCommand? _openFolderCommand;
        private IPhotoDirectory? _photoDirectory;

        public PhotoFolderImportViewModel(IServiceProvider serviceProvider, IWindowService windowService,
            IFileSystem fileSystem, IDialogService dialogService, IRegionManager regionManager)
        {
            _serviceProvider = serviceProvider;
            _windowService = windowService;
            _fileSystem = fileSystem;
            _dialogService = dialogService;
            _regionManager = regionManager;
        }

        public AsyncDelegateCommand OpenFilesCommand =>
            _openFilesCommand ??= new AsyncDelegateCommand(async () =>
            {
                var result = _windowService.ShowFileSelectionDialog(
                    $"Image files|*{string.Join(";*", ImageExtensions)}|All files|*.*", out var filenames);
                if (!result) return;

                await ImportFiles(filenames);
            });

        public AsyncDelegateCommand OpenFolderCommand =>
            _openFolderCommand ??= new AsyncDelegateCommand(async () =>
            {
                var result = _windowService.ShowFolderBrowserDialog(
                    new FolderBrowserDialogOptions
                    {
                        ShowNewFolderButton = false, Description = "Select the folder to import"
                    }, out var path);
                if (!result) return;

                var directory = _fileSystem.DirectoryInfo.FromDirectoryName(path);
                if (!directory.Exists)
                {
                    _windowService.ShowError("The directory does not exist");
                    return;
                }

                var searchOption = SearchOption.TopDirectoryOnly;
                if (directory.EnumerateDirectories().Any())
                {
                    var msgResult = _windowService.ShowMessage(
                        "The selected directory contains subdirectories. Should the files from these also be included?",
                        "Include subdirectories?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question,
                        MessageBoxResult.Yes);

                    if (msgResult == MessageBoxResult.Cancel) return;
                    if (msgResult == MessageBoxResult.Yes) searchOption = SearchOption.AllDirectories;
                }

                var files = directory.EnumerateFiles("*", searchOption).ToList();

                if (!files.All(HasFileImageExtension))
                {
                    var msgResult = _windowService.ShowMessage(
                        $"Files were found that do not end in the known image extensions ({string.Join(", ", ImageExtensions)}). Do you want to filter the files and only include image files?",
                        "Filter", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);

                    if (msgResult == MessageBoxResult.Cancel) return;
                    if (msgResult == MessageBoxResult.Yes) files = files.Where(HasFileImageExtension).ToList();
                }

                await ImportFiles(files.Select(x => x.FullName).ToList());
            });

        private static bool HasFileImageExtension(IFileInfo fileInfo)
        {
            return ImageExtensions.Any(extension => fileInfo.Name.EndsWith(extension));
        }

        public void Initialize(IPhotoDirectory photoDirectory)
        {
            _photoDirectory = photoDirectory;
        }

        public async Task ImportFiles(IReadOnlyList<string> files)
        {
            if (_photoDirectory == null)
                throw new InvalidOperationException(
                    "The view model must first be initialized with the photo directory");

            var worker = _serviceProvider.GetRequiredService<IImportFilesWorker>();
            FileCheckReport report;
            try
            {
                report = await worker.Execute(new ImportFilesRequest(files, _photoDirectory));
            }
            catch (Exception e)
            {
                _windowService.ShowError(e);
                return;
            }

            var parameters = new DialogParameters {{"report", report}, {"photoDirectory", _photoDirectory}};

            _dialogService.ShowDialog("DecisionManager", parameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var parameters = new NavigationParameters { { "photoDirectory", _photoDirectory } };
                    _regionManager.RequestNavigate(RegionNames.MainView, "SynchronizeFolderView", parameters);
                }
            });
        }
    }
}