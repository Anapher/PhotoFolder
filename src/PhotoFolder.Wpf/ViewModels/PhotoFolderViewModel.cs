using PhotoFolder.Core.Interfaces.Gateways;
using Prism.Mvvm;
using Prism.Regions;

namespace PhotoFolder.Wpf.ViewModels
{
    public class PhotoFolderViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionContext;

        public PhotoFolderViewModel(IRegionManager regionContext)
        {
            _regionContext = regionContext;
        }

        private IPhotoDirectory? _photoDirectory;

        public IPhotoDirectory? PhotoDirectory
        {
            get { return _photoDirectory; }
            set => SetProperty(ref _photoDirectory, value);
        }


        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            PhotoDirectory = navigationContext.Parameters.GetValue<IPhotoDirectory>("photoDirectory");

            //_regionContext.Regions[RegionNames.PhotoFolderStatistics].Context = PhotoDirectory;
        }
    }
}
