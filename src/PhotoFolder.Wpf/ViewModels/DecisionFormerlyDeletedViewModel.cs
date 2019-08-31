using Humanizer;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Wpf.ViewModels.Models;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionFormerlyDeletedViewModel : BindableBase
    {
        private FormerlyDeletedFileDecisionViewModel? _decision;
        private FormerlyDeletedIssue? _issue;
        private string? _relativeDeletedAt;

        public FormerlyDeletedFileDecisionViewModel? Decision
        {
            get => _decision;
            private set => SetProperty(ref _decision, value);
        }

        public FormerlyDeletedIssue? Issue
        {
            get => _issue;
            private set => SetProperty(ref _issue, value);
        }

        public string? RelativeDeletedAt
        {
            get => _relativeDeletedAt;
            private set => SetProperty(ref _relativeDeletedAt, value);
        }

        public void Initialize(DecisionAssistantContext context)
        {
            Decision = (FormerlyDeletedFileDecisionViewModel) context.DecisionWrapper.Decision;
            Issue = (FormerlyDeletedIssue) Decision.Issue;

            RelativeDeletedAt = Issue.DeletedFileInfo.DeletedAt.Humanize();
        }
    }
}
