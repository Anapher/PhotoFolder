using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels;

namespace PhotoFolder.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for DecisionManagerDetailsView.xaml
    /// </summary>
    public partial class DecisionManagerDetailsView
    {
        public DecisionManagerDetailsView()
        {
            InitializeComponent();

            this.UseRegionContext<DecisionManagerDetailsViewModel, DecisionManagerContext>((vm, context) =>
                vm.Initialize(context));
        }
    }
}