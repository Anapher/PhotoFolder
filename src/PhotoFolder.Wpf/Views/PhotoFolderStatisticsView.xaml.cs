using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels;

namespace PhotoFolder.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for PhotoFolderStatisticsView.xaml
    /// </summary>
    public partial class PhotoFolderStatisticsView
    {
        public PhotoFolderStatisticsView()
        {
            InitializeComponent();

            this.UseRegionContext<PhotoFolderStatisticsViewModel, IPhotoDirectory>((vm, context) =>
                vm.Initialize(context));
        }
    }
}