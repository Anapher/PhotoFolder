using System.Windows;
using PhotoFolder.Wpf.ViewModels.Models;

namespace PhotoFolder.Wpf.Views.Decision
{
    /// <summary>
    ///     Interaction logic for DecisionOperationView.xaml
    /// </summary>
    public partial class DecisionOperationView
    {
        public static readonly DependencyProperty DecisionViewModelProperty = DependencyProperty.Register("DecisionViewModel", typeof(IIssueDecisionViewModel),
            typeof(DecisionOperationView), new PropertyMetadata(default(IIssueDecisionViewModel)));

        public static readonly DependencyProperty RemoveFilesFromOutsideProperty = DependencyProperty.Register("RemoveFilesFromOutside", typeof(bool),
            typeof(DecisionOperationView), new PropertyMetadata(default(bool)));

        public DecisionOperationView()
        {
            InitializeComponent();
        }

        public bool RemoveFilesFromOutside
        {
            get => (bool) GetValue(RemoveFilesFromOutsideProperty);
            set => SetValue(RemoveFilesFromOutsideProperty, value);
        }

        public IIssueDecisionViewModel DecisionViewModel
        {
            get => (IIssueDecisionViewModel) GetValue(DecisionViewModelProperty);
            set => SetValue(DecisionViewModelProperty, value);
        }
    }
}