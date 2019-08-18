using System.Windows;
using System.Windows.Controls;
using PhotoFolder.Wpf.ViewModels;

namespace PhotoFolder.Wpf.Views.DecisionManager
{
    /// <summary>
    ///     Interaction logic for FileDecisionText.xaml
    /// </summary>
    public partial class FileDecisionText : UserControl
    {
        public static readonly DependencyProperty DecisionProperty = DependencyProperty.Register("Decision",
            typeof(FileDecision), typeof(FileDecisionText), new PropertyMetadata(default(FileDecision)));

        public static readonly DependencyProperty MoveFilesFromOutsideProperty =
            DependencyProperty.Register("MoveFilesFromOutside", typeof(bool), typeof(FileDecisionText),
                new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsInPhotoDirectoryProperty =
            DependencyProperty.Register("IsInPhotoDirectory", typeof(bool), typeof(FileDecisionText),
                new PropertyMetadata(default(bool)));

        public FileDecisionText()
        {
            InitializeComponent();
        }

        public FileDecision Decision
        {
            get => (FileDecision) GetValue(DecisionProperty);
            set => SetValue(DecisionProperty, value);
        }

        public bool MoveFilesFromOutside
        {
            get => (bool) GetValue(MoveFilesFromOutsideProperty);
            set => SetValue(MoveFilesFromOutsideProperty, value);
        }

        public bool IsInPhotoDirectory
        {
            get => (bool) GetValue(IsInPhotoDirectoryProperty);
            set => SetValue(IsInPhotoDirectoryProperty, value);
        }
    }
}