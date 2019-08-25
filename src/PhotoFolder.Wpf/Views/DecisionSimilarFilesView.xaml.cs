using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels;

namespace PhotoFolder.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for DecisionSimilarFilesView.xaml
    /// </summary>
    public partial class DecisionSimilarFilesView
    {
        public DecisionSimilarFilesView()
        {
            InitializeComponent();

            this.UseRegionContext<DecisionSimilarFilesViewModel, DecisionAssistantContext>((vm, context) => vm.Initialize(context));
        }
    }
}
