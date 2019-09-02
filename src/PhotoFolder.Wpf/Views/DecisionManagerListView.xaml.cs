using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels;
using PhotoFolder.Wpf.ViewModels.DecisionManager;

namespace PhotoFolder.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for DecisionManagerListView.xaml
    /// </summary>
    public partial class DecisionManagerListView
    {
        public DecisionManagerListView()
        {
            InitializeComponent();

            this.UseRegionContext<DecisionManagerListViewModel, DecisionManagerContext>((vm, context) =>
                vm.Initialize(context));
        }
    }
}