using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels;
using PhotoFolder.Wpf.ViewModels.Models;

namespace PhotoFolder.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for DecisionInvalidLocationView.xaml
    /// </summary>
    public partial class DecisionInvalidLocationView
    {
        public DecisionInvalidLocationView()
        {
            InitializeComponent();

            this.UseRegionContext<DecisionInvalidLocationViewModel, DecisionAssistantContext>((vm, context) => vm.Initialize(context));
        }
    }
}