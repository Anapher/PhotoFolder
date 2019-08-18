using System.ComponentModel;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels;
using Prism.Common;
using Prism.Regions;

namespace PhotoFolder.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for PhotoFolderImportView.xaml
    /// </summary>
    public partial class PhotoFolderImportView
    {
        public PhotoFolderImportView()
        {
            InitializeComponent();

            this.UseRegionContext<PhotoFolderImportViewModel, IPhotoDirectory>((vm, context) => vm.Initialize(context));
        }
    }
}