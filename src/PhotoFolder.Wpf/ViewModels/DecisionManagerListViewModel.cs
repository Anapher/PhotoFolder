using System.Windows.Data;
using Prism.Commands;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionManagerListViewModel : BindableBase
    {
        private DelegateCommand<IFileDecisionViewModel>? _applyRecommendedActionCommand;
        private DecisionManagerContext? _decisionContext;
        private ListCollectionView? _decisions;

        public ListCollectionView? Decisions
        {
            get => _decisions;
            private set => SetProperty(ref _decisions, value);
        }

        public DecisionManagerContext? DecisionContext
        {
            get => _decisionContext;
            set => SetProperty(ref _decisionContext, value);
        }

        public DelegateCommand<IFileDecisionViewModel> ApplyRecommendedActionCommand
        {
            get
            {
                return _applyRecommendedActionCommand ??= new DelegateCommand<IFileDecisionViewModel>(parameter =>
                {
                    parameter.SelectedDecision = parameter.RecommendedDecision;
                });
            }
        }

        public void Initialize(DecisionManagerContext context)
        {
            Decisions = new ListCollectionView(context.Decisions);
            DecisionContext = context;
        }
    }
}