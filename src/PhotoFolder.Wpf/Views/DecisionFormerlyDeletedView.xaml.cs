using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels;

namespace PhotoFolder.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for DecisionFormerlyDeletedView.xaml
    /// </summary>
    public partial class DecisionFormerlyDeletedView
    {
        public DecisionFormerlyDeletedView()
        {
            InitializeComponent();

            this.UseRegionContext<DecisionFormerlyDeletedViewModel, DecisionAssistantContext>((vm, context) => vm.Initialize(context));
        }
    }
}
