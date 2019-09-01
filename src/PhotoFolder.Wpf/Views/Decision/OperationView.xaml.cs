using System.Windows;
using System.Windows.Controls;
using PhotoFolder.Application.Dto;
using PhotoFolder.Wpf.ViewModels.Models;

namespace PhotoFolder.Wpf.Views.Decision
{
    /// <summary>
    ///     Interaction logic for OperationView.xaml
    /// </summary>
    public partial class OperationView : UserControl
    {
        public static readonly DependencyProperty FileOperationProperty = DependencyProperty.Register("FileOperation", typeof(IFileOperation),
            typeof(OperationView), new PropertyMetadata(default(IFileOperation)));

        public static readonly DependencyProperty RemoveFilesFromOutsideProperty =
            DependencyProperty.Register("RemoveFilesFromOutside", typeof(bool), typeof(OperationView), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(OperationView), new PropertyMetadata(default(bool)));

        public OperationView()
        {
            InitializeComponent();
        }

        public IFileOperation FileOperation
        {
            get => (IFileOperation) GetValue(FileOperationProperty);
            set => SetValue(FileOperationProperty, value);
        }

        public bool RemoveFilesFromOutside
        {
            get => (bool) GetValue(RemoveFilesFromOutsideProperty);
            set => SetValue(RemoveFilesFromOutsideProperty, value);
        }

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
    }
}
