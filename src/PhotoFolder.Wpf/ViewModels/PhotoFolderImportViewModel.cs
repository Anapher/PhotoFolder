using Microsoft.Extensions.DependencyInjection;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Wpf.Extensions;
using PhotoFolder.Wpf.Services;
using PhotoFolder.Wpf.Utilities;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoFolder.Wpf.ViewModels
{
    public class PhotoFolderImportViewModel : BindableBase
    {
        private readonly IServiceProvider _serviceProvider;
        private IPhotoDirectory? _photoDirectory;
        private readonly IWindowService _windowService;
        private static readonly string[] ImageExtensions = new[] { ".bmp", ".jpg", ".png", ".gif" };

        public PhotoFolderImportViewModel(IServiceProvider serviceProvider, IWindowService windowService, IFileSystem fileSystem)
        {
            _serviceProvider = serviceProvider;
            _windowService = windowService;
            _fileSystem = fileSystem;
        }

        private AsyncDelegateCommand? _openFilesCommand;

        public AsyncDelegateCommand OpenFilesCommand
        {
            get => _openFilesCommand ?? (_openFilesCommand = new AsyncDelegateCommand(async () => {
                var result = _windowService.ShowFileSelectionDialog($"Image files|*{string.Join(";*", ImageExtensions)}|All files|*.*", out var filenames);
                if (!result) return;

                await ImportFiles(filenames);
            }));
        }

        private AsyncDelegateCommand? _openFolderCommand;
        private readonly IFileSystem _fileSystem;

        public AsyncDelegateCommand OpenFolderCommand
        {
            get => _openFolderCommand ?? (_openFolderCommand = new AsyncDelegateCommand(async () => {
                var result = _windowService.ShowFolderBrowserDialog(new FolderBrowserDialogOptions
                {
                    ShowNewFolderButton = false,
                    Description = "Select the folder to import"
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
                        "Include subdirectories?", System.Windows.MessageBoxButton.YesNoCancel,
                        System.Windows.MessageBoxImage.Question, System.Windows.MessageBoxResult.Yes);

                    if (msgResult == System.Windows.MessageBoxResult.Cancel) return;
                    if (msgResult == System.Windows.MessageBoxResult.Yes) searchOption = SearchOption.AllDirectories;
                }

                var files = directory.EnumerateFiles("*", searchOption);

                if (!files.All(HasFileImageExtension))
                {
                    var msgResult = _windowService.ShowMessage(
                    $"Files were found that do not end in the known image extensions ({string.Join(", ", ImageExtensions)}). Do you want to filter the files and only include image files?",
                    "Filter", System.Windows.MessageBoxButton.YesNoCancel,
                    System.Windows.MessageBoxImage.Question, System.Windows.MessageBoxResult.Yes);

                    if (msgResult == System.Windows.MessageBoxResult.Cancel) return;
                    if (msgResult == System.Windows.MessageBoxResult.Yes)
                    {
                        files = files.Where(HasFileImageExtension);
                    }
                }

                await ImportFiles(files.Select(x => x.FullName).ToList());
            }));
        }

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
                throw new InvalidOperationException("The view model must first be initialized with the photo directory");

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
        }
    }
}
