using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Wpf.ViewModels;
using Prism.Common;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotoFolder.Wpf.Views
{
    /// <summary>
    /// Interaction logic for PhotoFolderImportView.xaml
    /// </summary>
    public partial class PhotoFolderImportView : UserControl
    {
        private readonly ObservableObject<object> _regionContext;

        public PhotoFolderImportView()
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

            var viewModel = (PhotoFolderImportViewModel)DataContext;
            if (viewModel != null)
            {
                viewModel.Initialize((IPhotoDirectory)_regionContext.Value);
            }
        }
    }
}
