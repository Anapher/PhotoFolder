using System.Collections.Generic;
using PhotoFolder.Core.Dto.Services;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels.Models
{
    public class IssueDecisionWrapperViewModel : BindableBase
    {
        private bool _execute;
        private bool _isVisible;

        public IssueDecisionWrapperViewModel(IIssueDecisionViewModel decision)
        {
            Decision = decision;
            IsDeleteDecision = decision is DeleteFilesDecisionViewModel;
        }

        public IIssueDecisionViewModel Decision { get; }
        public bool IsDeleteDecision { get; }

        public bool Execute
        {
            get => _execute;
            set => SetProperty(ref _execute, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            private set=> SetProperty(ref _isVisible, value);
        }

        public void UpdateDeletedFiles(IReadOnlyList<FileInformation> deletedFiles)
        {
            IsVisible = Decision.UpdateDeletedFiles(deletedFiles);
        }
    }
}