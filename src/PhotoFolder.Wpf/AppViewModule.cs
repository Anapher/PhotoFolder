using PhotoFolder.Wpf.ViewModels;
using PhotoFolder.Wpf.Views;
using Prism.Ioc;
using Prism.Regions;

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

            regionManager.RegisterViewWithRegion(RegionNames.PhotoFolderWidgets, typeof(PhotoFolderStatisticsView));
            regionManager.RegisterViewWithRegion(RegionNames.PhotoFolderWidgets, typeof(PhotoFolderImportView));

            regionManager.RegisterViewWithRegion(RegionNames.DecisionManagerMenu, typeof(DecisionManagerMenuView));
            regionManager.RegisterViewWithRegion(RegionNames.DecisionManagerList, typeof(DecisionManagerListView));
            regionManager.RegisterViewWithRegion(RegionNames.DecisionManagerSelectionDetails, typeof(DecisionManagerDetailsView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<ConfigureFolderDialog, ConfigureFolderDialogViewModel>("ConfigureFolder");
            containerRegistry.RegisterDialog<DecisionManagerDialog, DecisionManagerViewModel>("DecisionManager");
        }
    }
}
