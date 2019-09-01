using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Infrastructure.Consts;
using PhotoFolder.Infrastructure.Photos;
using PhotoFolder.Infrastructure.Services;
using PhotoFolder.Wpf.Extensions;
using PhotoFolder.Wpf.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoFolder.Wpf.ViewModels
{
    public class OpenFolderViewModel : BindableBase
    {
        private string _folderPath;
        private DelegateCommand? _choseFolderPathCommand;
        private DelegateCommand? _openFolderCommand;
        private readonly IWindowService _windowService;
        private readonly IFileSystem _fileSystem;
        private readonly IDialogService _dialogService;
        private readonly IPhotoDirectoryCreator _photoDirectoryCreator;
        private readonly IPhotoDirectoryLoader _photoDirectoryLoader;
        private readonly IRegionManager _regionManager;
        private readonly IAppSettingsProvider _appSettingsProvider;

        public OpenFolderViewModel(IWindowService windowService, IDialogService dialogService, IFileSystem fileSystem,
            IPhotoDirectoryCreator photoDirectoryCreator, IPhotoDirectoryLoader photoDirectoryLoader, IRegionManager regionManager, IAppSettingsProvider appSettingsProvider)
        {
            _windowService = windowService;
            _fileSystem = fileSystem;
            _dialogService = dialogService;
            _photoDirectoryCreator = photoDirectoryCreator;
            _photoDirectoryLoader = photoDirectoryLoader;
            _regionManager = regionManager;
            _appSettingsProvider = appSettingsProvider;

            _folderPath = _appSettingsProvider.Current.LatestPhotoFolder ?? string.Empty;
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
                return _choseFolderPathCommand ??= new DelegateCommand(() => {
                    if (_windowService.ShowFolderBrowserDialog(new FolderBrowserDialogOptions
                    {
                        Description = "Please select your photo folder.",
                        ShowNewFolderButton = false
                    }, out var selectedPath))
                    {
                        FolderPath = selectedPath;
                    }
                });
            }
        }

        public DelegateCommand OpenFolderCommand
        {
            get
            {
                return _openFolderCommand ??= new DelegateCommand(() => {
                    var folder = _fileSystem.DirectoryInfo.FromDirectoryName(FolderPath);
                    if (!folder.Exists)
                    {
                        _windowService.ShowMessage("The selected folder does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var configFile = _fileSystem.FileInfo.FromFileName(_fileSystem.Path.Combine(folder.FullName, PhotoFolderConsts.ConfigFileName));
                    if (!configFile.Exists)
                    {
                        var parameters = new DialogParameters { { "path", folder.FullName } };
                        _dialogService.ShowDialog("ConfigureFolder", parameters, CreateFolderConfig(folder.FullName));
                    }
                    else
                    {
                        LoadFolder(folder.FullName);
                    }
                });
            }
        }

        private Action<IDialogResult> CreateFolderConfig(string path)
        {
            return async dialogResult =>
            {
                if (dialogResult.Result != ButtonResult.OK) return;

                var templateString = dialogResult.Parameters.GetValue<string>("templateString");
                try
                {
                    await _photoDirectoryCreator.Create(path, new PhotoDirectoryConfig(templateString));
                }
                catch (Exception e)
                {
                    _windowService.ShowError(e);
                    return;
                }

                await LoadFolder(path);
            };
        }

        private async Task LoadFolder(string path)
        {
            IPhotoDirectory photoDirectory;
            try
            {
                photoDirectory = await _photoDirectoryLoader.Load(path);
            }
            catch (Exception e)
            {
                _windowService.ShowError(e);
                return;
            }

            _appSettingsProvider.Save(_appSettingsProvider.Current.SetLatestPhotoFolder(path));

            var parameters = new NavigationParameters { { "photoDirectory", photoDirectory } };
            _regionManager.RequestNavigate(RegionNames.MainView, "SynchronizeFolderView", parameters);
        }
    }
}
