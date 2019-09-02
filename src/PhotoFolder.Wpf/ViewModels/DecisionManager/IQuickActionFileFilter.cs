using System.Collections.Generic;
using PhotoFolder.Wpf.ViewModels.Models;

namespace PhotoFolder.Wpf.ViewModels.DecisionManager
{
    public interface IQuickActionFileFilter
    {
        string Description { get; }
        IEnumerable<IssueDecisionWrapperViewModel> Filter(IEnumerable<IssueDecisionWrapperViewModel> viewModels);
    }
}