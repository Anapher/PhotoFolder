using PhotoFolder.Wpf.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace PhotoFolder.Wpf
{
    public class AppViewModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion(RegionNames.MainView, typeof(OpenFolderView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<OpenFolderView>();
        }
    }
}
