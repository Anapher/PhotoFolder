using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels;

namespace PhotoFolder.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for DecisionDuplicateFilesView.xaml
    /// </summary>
    public partial class DecisionDuplicateFilesView
    {
        public DecisionDuplicateFilesView()
        {
            InitializeComponent();

            this.UseRegionContext<DecisionDuplicateFilesViewModel, DecisionAssistantContext>((vm, context) => vm.Initialize(context));
        }
    }
}