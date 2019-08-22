using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using PhotoFolder.Wpf.ViewModels.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionManagerListViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;
        private DecisionManagerContext? _decisionContext;
        private ListCollectionView? _decisions;
        private DelegateCommand<IssueDecisionWrapperViewModel>? _openFileCommand;
        private DelegateCommand<IssueDecisionWrapperViewModel>? _revealFileInFolderCommand;
        private DelegateCommand<IssueDecisionWrapperViewModel>? _openAssistantCommand;

        public DecisionManagerListViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

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

        public DelegateCommand<IssueDecisionWrapperViewModel> OpenFileCommand
        {
            get
            {
                return _openFileCommand ??= new DelegateCommand<IssueDecisionWrapperViewModel>(parameter =>
                {
                    if (_decisionContext == null) throw new InvalidOperationException();

                    var absolutePath = _decisionContext.PhotoDirectory.GetAbsolutePath(parameter.Decision.Issue.File);
                    Process.Start(new ProcessStartInfo(absolutePath) {UseShellExecute = true});
                });
            }
        }

        public DelegateCommand<IssueDecisionWrapperViewModel> RevealFileInFolderCommand
        {
            get
            {
                return _revealFileInFolderCommand ??= new DelegateCommand<IssueDecisionWrapperViewModel>(parameter =>
                {
                    if (_decisionContext == null) throw new InvalidOperationException();

                    var absolutePath = _decisionContext.PhotoDirectory.GetAbsolutePath(parameter.Decision.Issue.File);
                    Process.Start("explorer.exe", $"/select, \"{absolutePath}\"");
                });
            }
        }

        public DelegateCommand<IssueDecisionWrapperViewModel> OpenAssistantCommand
        {
            get
            {
                return _openAssistantCommand ??= new DelegateCommand<IssueDecisionWrapperViewModel>(parameter =>
                {
                    if (DecisionContext == null) throw new InvalidOperationException();

                    var parameters = new DialogParameters {{"decision", parameter}, {"decisionManagerContext", DecisionContext}};
                    _dialogService.ShowDialog("DecisionAssistant", parameters, _ => {});
                });
            }
        }

        public void Initialize(DecisionManagerContext context)
        {
            DecisionContext = context;

            Decisions = new ListCollectionView(context.Issues);

            Decisions.SortDescriptions.Add(new SortDescription("Decision.Issue.File.Filename", ListSortDirection.Ascending));
            Decisions.SortDescriptions.Add(new SortDescription("IsDeleteDecision", ListSortDirection.Descending));
        }
    }
}