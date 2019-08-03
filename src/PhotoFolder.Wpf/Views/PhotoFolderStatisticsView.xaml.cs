using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Wpf.ViewModels;
using Prism.Common;
using Prism.Regions;
using System.ComponentModel;
using System.Windows.Controls;

namespace PhotoFolder.Wpf.Views
{
    /// <summary>
    /// Interaction logic for PhotoFolderStatisticsView.xaml
    /// </summary>
    public partial class PhotoFolderStatisticsView : UserControl
    {
        private readonly ObservableObject<object> _regionContext;

        public PhotoFolderStatisticsView()
        {
            InitializeComponent();

            _regionContext = RegionContext.GetObservableContext(this);
            _regionContext.PropertyChanged += RegionContext_PropertyChanged;

            DataContextChanged += (_, __) => UpdateRegionContext();
        }

        private void RegionContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateRegionContext();
        }

        private void UpdateRegionContext()
        {
            if (_regionContext.Value == null) return;

            var viewModel = (PhotoFolderStatisticsViewModel)DataContext;
            if (viewModel != null)
            {
                viewModel.Initialize((IPhotoDirectory) _regionContext.Value);
            }
        }
    }
}
