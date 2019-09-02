using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels;

namespace PhotoFolder.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for PhotoFolderCheckView.xaml
    /// </summary>
    public partial class PhotoFolderCheckView
    {
        public PhotoFolderCheckView()
        {
            InitializeComponent();

            this.UseRegionContext<PhotoFolderCheckViewModel, IPhotoDirectory>((vm, context) => vm.Initialize(context));
        }
    }
}
