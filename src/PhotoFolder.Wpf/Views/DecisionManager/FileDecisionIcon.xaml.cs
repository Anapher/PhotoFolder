using System.Windows;
using PhotoFolder.Wpf.ViewModels;

namespace PhotoFolder.Wpf.Views.DecisionManager
{
    /// <summary>
    ///     Interaction logic for FileDecisionIcon.xaml
    /// </summary>
    public partial class FileDecisionIcon
    {
        public static readonly DependencyProperty DecisionProperty = DependencyProperty.Register("Decision",
            typeof(FileDecision), typeof(FileDecisionIcon), new PropertyMetadata(default(FileDecision)));

        public static readonly DependencyProperty MoveFilesFromOutsideProperty =
            DependencyProperty.Register("MoveFilesFromOutside", typeof(bool), typeof(FileDecisionIcon),
                new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsInPhotoDirectoryProperty =
            DependencyProperty.Register("IsInPhotoDirectory", typeof(bool), typeof(FileDecisionIcon),
                new PropertyMetadata(default(bool)));

        public FileDecisionIcon()
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