using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels;

namespace PhotoFolder.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for DecisionManagerMenuView.xaml
    /// </summary>
    public partial class DecisionManagerMenuView
    {
        public DecisionManagerMenuView()
        {
            InitializeComponent();

            this.UseRegionContext<DecisionManagerMenuViewModel, DecisionManagerContext>((vm, context) =>
                vm.Initialize(context));
        }
    }
}