using PhotoFolder.Wpf.Extensions;
using PhotoFolder.Wpf.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.IO.Abstractions;
using System.Windows;

namespace PhotoFolder.Wpf.ViewModels
{
    public class OpenFolderViewModel : BindableBase
    {
        private string _folderPath = string.Empty;
        private DelegateCommand? _choseFolderPathCommand;
        private DelegateCommand? _loadFolderCommand;
        private readonly IWindowService _windowService;
        private readonly IFileSystem _fileSystem;
        private readonly IDialogService _dialogService;

        public OpenFolderViewModel(IWindowService windowService, IDialogService dialogService, IFileSystem fileSystem)
        {
            _windowService = windowService;
            _fileSystem = fileSystem;
            _dialogService = dialogService;
        }

        public string FolderPath
        {
            get => _folderPath;
            set => SetProperty(ref _folderPath, value);
        }

        public DelegateCommand ChoseFolderPathCommand
        {
            get
            {
                return _choseFolderPathCommand ?? (_choseFolderPathCommand = new DelegateCommand(() => {
                    if (_windowService.ShowFolderBrowserDialog(new FolderBrowserDialogOptions
                    {
                        Description = "Please select your photo folder.",
                        ShowNewFolderButton = false
                    }, out var selectedPath))
                    {
                        FolderPath = selectedPath;
                    }
                }));
            }
        }

        public DelegateCommand LoadFolderCommand
        {
            get
            {
                return _loadFolderCommand ?? (_loadFolderCommand = new DelegateCommand(() => {
                    var folder = _fileSystem.DirectoryInfo.FromDirectoryName(FolderPath);
                    if (!folder.Exists)
                    {
                        _windowService.ShowMessage("The selected folder does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var configFile = _fileSystem.FileInfo.FromFileName(_fileSystem.Path.Combine(folder.FullName, ".photofolder.settings.json"));
                    if (!configFile.Exists)
                    {

                    }
                }));
            }
        }
    }
}
