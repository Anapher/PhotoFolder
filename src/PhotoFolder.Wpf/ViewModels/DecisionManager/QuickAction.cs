using System;
using System.Collections.Generic;
using PhotoFolder.Wpf.ViewModels.Models;

namespace PhotoFolder.Wpf.ViewModels.DecisionManager
{
    public class QuickAction
    {
        public QuickAction(IssueDecisionWrapperViewModel source, string description, Action<IEnumerable<IssueDecisionWrapperViewModel>> action,
            IReadOnlyList<IQuickActionFileFilter> filters)
        {
            Source = source;
            Description = description;
            Action = action;
            Filters = filters;
        }

        public string Description { get; }
        public IssueDecisionWrapperViewModel Source { get;}
        public Action<IEnumerable<IssueDecisionWrapperViewModel>> Action { get; }
        public IReadOnlyList<IQuickActionFileFilter> Filters { get; }
    }
}