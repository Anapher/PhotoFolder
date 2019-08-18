using PhotoFolder.Core.Interfaces.Gateways;
using Prism.Mvvm;
using Prism.Regions;

namespace PhotoFolder.Wpf.ViewModels
{
    public class PhotoFolderViewModel : BindableBase, INavigationAware
    {
        private IPhotoDirectory? _photoDirectory;

        public IPhotoDirectory? PhotoDirectory
        {
            get => _photoDirectory;
            set => SetProperty(ref _photoDirectory, value);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            PhotoDirectory = navigationContext.Parameters.GetValue<IPhotoDirectory>("photoDirectory");
        }
    }
}