using Autofac;
using PhotoFolder.Application;
using PhotoFolder.Core;
using PhotoFolder.Infrastructure;
using PhotoFolder.Wpf.ViewModels;
using PhotoFolder.Wpf.Views;
using Prism.Ioc;
using Prism.Regions;
using PhotoFolder.Wpf.Extensions;

namespace PhotoFolder.Wpf
{
    public class AppViewModule : Prism.Modularity.IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion(RegionNames.MainView, typeof(OpenFolderView));
            regionManager.RegisterViewWithRegion(RegionNames.MainView, typeof(SynchronizeFolderView));
            regionManager.RegisterViewWithRegion(RegionNames.MainView, typeof(PhotoFolderView));

            regionManager.RegisterViewWithRegion(RegionNames.PhotoFolderStatistics, typeof(PhotoFolderStatisticsView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<OpenFolderView>();
            containerRegistry.RegisterForNavigation<SynchronizeFolderView>();
            containerRegistry.RegisterForNavigation<PhotoFolderView>();
            containerRegistry.RegisterForNavigation<PhotoFolderStatisticsView>();

            containerRegistry.RegisterDialog<ConfigureFolderDialog, ConfigureFolderDialogViewModel>("ConfigureFolder");
        }
    }
}
