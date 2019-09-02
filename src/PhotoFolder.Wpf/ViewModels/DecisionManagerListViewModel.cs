using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using PhotoFolder.Core.Extensions;
using PhotoFolder.Wpf.ViewModels.DecisionManager;
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
        private DelegateCommand<IssueDecisionWrapperViewModel>? _ignoreIssueCommand;

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
            private set => SetProperty(ref _decisionContext, value);
        }

        public DelegateCommand<IssueDecisionWrapperViewModel> OpenFileCommand
        {
            get
            {
                return _openFileCommand ??= new DelegateCommand<IssueDecisionWrapperViewModel>(parameter =>
                {
                    if (_decisionContext == null) throw new InvalidOperationException();

                    var absolutePath = parameter.Decision.Issue.File.Filename;
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

                    var absolutePath = parameter.Decision.Issue.File.Filename.Replace('/', '\\');
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

        public DelegateCommand<IssueDecisionWrapperViewModel> IgnoreIssueCommand
        {
            get
            {
                return _ignoreIssueCommand ??= new DelegateCommand<IssueDecisionWrapperViewModel>(parameter =>
                {
                    if (DecisionContext == null) throw new InvalidOperationException();

                    IgnoreFiles(parameter.Yield());
                    DecisionContext.CurrentQuickAction = QuickActionFactory.Create(parameter, "Ignore", IgnoreFiles);
                });
            }
        }

        private void IgnoreFiles(IEnumerable<IssueDecisionWrapperViewModel> models)
        {
            var memoryManager = DecisionContext!.PhotoDirectory.MemoryManager;
            var result = memoryManager.DirectoryMemory.IgnoredIssues.ToHashSet();

            foreach (var model in models)
            {
                if (model.Decision.Issue.File.RelativeFilename == null) continue;

                result.Add(model.Decision.Issue.Identity);
                DecisionContext.IgnoredIssues.Add(model.Decision.Issue.Identity);
            }

            memoryManager.Update(memoryManager.DirectoryMemory.SetIgnoredIssues(result.ToImmutableHashSet()));
            Decisions?.Refresh();
        }

        public void Initialize(DecisionManagerContext context)
        {
            DecisionContext = context;

            Decisions = new ListCollectionView(context.Issues) {Filter = Filter};

            Decisions.SortDescriptions.Add(new SortDescription("Decision.Issue.File.Filename", ListSortDirection.Ascending));
            Decisions.SortDescriptions.Add(new SortDescription("IsDeleteDecision", ListSortDirection.Descending));

            foreach (var checkable in context.IssueFilter)
                checkable.PropertyChanged += (sender, args) => Decisions.Refresh();
        }

        private bool Filter(object obj)
        {
            if (DecisionContext == null) return true;

            var viewModel = (IssueDecisionWrapperViewModel) obj;
            if (DecisionContext.IgnoredIssues.Contains(viewModel.Decision.Issue.Identity)) return false;
            if (!DecisionContext.IssueFilter.First(x => x.Value == viewModel.Decision.Issue.GetType()).IsChecked) return false;

            return true;
        }
    }
}