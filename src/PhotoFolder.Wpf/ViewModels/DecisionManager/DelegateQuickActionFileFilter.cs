using System;
using System.Collections.Generic;
using PhotoFolder.Wpf.ViewModels.Models;

namespace PhotoFolder.Wpf.ViewModels.DecisionManager
{
    public class DelegateQuickActionFileFilter : IQuickActionFileFilter
    {
        private readonly Func<IEnumerable<IssueDecisionWrapperViewModel>, IEnumerable<IssueDecisionWrapperViewModel>> _delegateAction;

        public DelegateQuickActionFileFilter(string description,
            Func<IEnumerable<IssueDecisionWrapperViewModel>, IEnumerable<IssueDecisionWrapperViewModel>> delegateAction)
        {
            _delegateAction = delegateAction;
            Description = description;
        }

        public string Description { get; }

        public IEnumerable<IssueDecisionWrapperViewModel> Filter(IEnumerable<IssueDecisionWrapperViewModel> viewModels)
        {
            return _delegateAction(viewModels);
        }
    }
}
