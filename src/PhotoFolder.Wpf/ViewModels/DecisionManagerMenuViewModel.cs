using System.Linq;
using Prism.Commands;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionManagerMenuViewModel : BindableBase
    {
        private DelegateCommand? _applyCopyMoveActionsCommand;
        private DelegateCommand? _applyRecommendedActionsCommand;

        private DelegateCommand? _applyRemoveActionsCommand;
        private DecisionManagerContext? _decisionContext;
        private DelegateCommand? _unsetAllActionsCommand;

        public DecisionManagerContext? DecisionContext
        {
            get => _decisionContext;
            set => SetProperty(ref _decisionContext, value);
        }

        public DelegateCommand ApplyRecommendedActionsCommand
        {
            get
            {
                return _applyRecommendedActionsCommand ??= new DelegateCommand(() =>
                {
                    if (DecisionContext == null) return;

                    foreach (var decision in DecisionContext.Decisions)
                        decision.SelectedDecision = decision.RecommendedDecision;
                });
            }
        }

        public DelegateCommand UnsetAllActionsCommand
        {
            get
            {
                return _unsetAllActionsCommand ??= new DelegateCommand(() =>
                {
                    if (DecisionContext == null) return;

                    foreach (var decision in DecisionContext.Decisions)
                        decision.SelectedDecision = FileDecision.None;
                });
            }
        }

        public DelegateCommand ApplyCopyMoveActionsCommand
        {
            get
            {
                return _applyCopyMoveActionsCommand ??= new DelegateCommand(() =>
                {
                    if (DecisionContext == null) return;

                    foreach (var decision in DecisionContext.Decisions.Where(x =>
                        x.RecommendedDecision == FileDecision.Move))
                        decision.SelectedDecision = FileDecision.Move;
                });
            }
        }

        public DelegateCommand ApplyRemoveActionsCommand
        {
            get
            {
                return _applyRemoveActionsCommand ??= new DelegateCommand(() =>
                {
                    if (DecisionContext == null) return;

                    foreach (var decision in DecisionContext.Decisions.Where(x =>
                        x.RecommendedDecision == FileDecision.Delete))
                        decision.SelectedDecision = FileDecision.Delete;
                });
            }
        }

        public void Initialize(DecisionManagerContext context)
        {
            DecisionContext = context;
        }
    }
}