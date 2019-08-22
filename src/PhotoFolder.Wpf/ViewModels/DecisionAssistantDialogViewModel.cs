using System;
using System.Linq;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Wpf.ViewModels.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionAssistantDialogViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand? _applyCommand;
        private DecisionAssistantContext? _context;
        private string _title = string.Empty;

        public DecisionAssistantContext? Context
        {
            get => _context;
            set => SetProperty(ref _context, value);
        }

        public DelegateCommand? ApplyCommand
        {
            get => _applyCommand;
            set => SetProperty(ref _applyCommand, value);
        }

        public bool CanCloseDialog() => true;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public event Action<IDialogResult> RequestClose;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var viewModel = parameters.GetValue<IssueDecisionWrapperViewModel>("decision");
            var decisionManagerContext = parameters.GetValue<DecisionManagerContext>("decisionManagerContext");

            Context = new DecisionAssistantContext(viewModel, decisionManagerContext, CloseDialogAction);

            switch (viewModel.Decision.Issue)
            {
                case InvalidFileLocationIssue invalidLocation:
                    Title = $"Invalid location ({invalidLocation.File.Filename})";
                    break;
                case DuplicateFilesIssue duplicateFiles:
                    Title = $"{duplicateFiles.RelevantFiles.Count() + 1} duplicate files found (hash: {duplicateFiles.File.Hash})";
                    break;
                case SimilarFilesIssue similarFiles:
                    Title = $"{similarFiles.RelevantFiles.Count() + 1} similar files found";
                    break;
            }
        }

        private void CloseDialogAction()
        {
            RequestClose?.Invoke(new DialogResult());
        }
    }

    public class DecisionAssistantContext
    {
        public DecisionAssistantContext(IssueDecisionWrapperViewModel decisionWrapper, DecisionManagerContext decisionManagerContext, Action closeDialogAction)
        {
            DecisionWrapper = decisionWrapper;
            DecisionManagerContext = decisionManagerContext;
            CloseDialogAction = closeDialogAction;
        }

        public IssueDecisionWrapperViewModel DecisionWrapper { get; }
        public DecisionManagerContext DecisionManagerContext { get; }
        public Action CloseDialogAction { get; }
    }

    public class ApplyDecisionSettings
    {
        public ApplyDecisionSettings(Action<IIssueDecisionViewModel> apply)
        {
            Apply = apply;
        }

        public Action<IIssueDecisionViewModel> Apply { get; }
    }
}