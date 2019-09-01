using System;
using System.Linq;
using Humanizer;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels.Models;
using Prism.Services.Dialogs;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionAssistantDialogViewModel : DialogBase
    {
        private DecisionAssistantContext? _context;

        public DecisionAssistantContext? Context
        {
            get => _context;
            set => SetProperty(ref _context, value);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            var viewModel = parameters.GetValue<IssueDecisionWrapperViewModel>("decision");
            var decisionManagerContext = parameters.GetValue<DecisionManagerContext>("decisionManagerContext");

            Context = new DecisionAssistantContext(viewModel, decisionManagerContext, CloseDialogAction);

            switch (viewModel.Decision.Issue)
            {
                case InvalidFileLocationIssue invalidLocation:
                    Title = $"Invalid location ({invalidLocation.File.RelativeFilename ?? invalidLocation.File.Filename})";
                    break;
                case DuplicateFilesIssue duplicateFiles:
                    Title = $"{duplicateFiles.RelevantFiles.Count() + 1} duplicate files found (hash: {duplicateFiles.File.Hash})";
                    break;
                case SimilarFilesIssue similarFiles:
                    Title = $"{similarFiles.RelevantFiles.Count() + 1} similar files found";
                    break;
                case FormerlyDeletedIssue formerlyDeleted:
                    Title = $"File was deleted {formerlyDeleted.DeletedFileInfo.DeletedAt.Humanize()}. Import it again?";
                    break;
            }
        }

        private void CloseDialogAction()
        {
            OnRequestClose(new DialogResult());
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
}