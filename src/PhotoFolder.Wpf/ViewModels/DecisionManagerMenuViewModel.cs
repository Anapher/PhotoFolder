using System.Linq;
using PhotoFolder.Wpf.ViewModels.Models;
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

                    foreach (var issue in DecisionContext.Issues.Where(x => x.Decision.IsRecommended))
                        issue.Execute = true;
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

                    foreach (var issue in DecisionContext.Issues)
                        issue.Execute = false;
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

                    foreach (var issue in DecisionContext.Issues.Where(x => x.Decision is InvalidLocationFileDecisionViewModel))
                        issue.Execute = true;
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
                });
            }
        }

        public void Initialize(DecisionManagerContext context)
        {
            DecisionContext = context;
        }
    }
}