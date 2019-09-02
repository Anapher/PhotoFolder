using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PhotoFolder.Application.Dto;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Wpf.ViewModels.Models;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels.DecisionManager
{
    public class DecisionManagerContext : BindableBase
    {
        private QuickAction? _currentQuickAction;
        private bool _removeFilesFromOutside;
        private IssueDecisionWrapperViewModel? _selectedIssue;

        public DecisionManagerContext(List<IssueDecisionWrapperViewModel> issues, IPhotoDirectory photoDirectory, Action resyncDatabaseAction)
        {
            Issues = issues;
            PhotoDirectory = photoDirectory;
            ResyncDatabaseAction = resyncDatabaseAction;

            OnUpdateOperations();

            foreach (var viewModel in Issues)
            {
                viewModel.PropertyChanged += ViewModelOnPropertyChanged;
                viewModel.Decision.PropertyChanged += DecisionOnPropertyChanged;
            }

            IssueFilter = new[] {typeof(InvalidFileLocationIssue), typeof(DuplicateFilesIssue), typeof(SimilarFilesIssue), typeof(FormerlyDeletedIssue)}
                .Select(x => new Checkable<Type>(x, true)).ToList();
        }

        public List<IssueDecisionWrapperViewModel> Issues { get; }
        public IPhotoDirectory PhotoDirectory { get; }
        public Action ResyncDatabaseAction { get; }
        public ISet<string> IgnoredIssues { get; } = new HashSet<string>();

        public IReadOnlyList<Checkable<Type>> IssueFilter { get; }

        public IEnumerable<IssueDecisionWrapperViewModel> ActiveIssues => Issues.Where(x => !IgnoredIssues.Contains(x.Decision.Issue.Identity));
        public bool IsBatchOperationActive { get; set; }

        public bool RemoveFilesFromOutside
        {
            get => _removeFilesFromOutside;
            set => SetProperty(ref _removeFilesFromOutside, value);
        }

        public IssueDecisionWrapperViewModel? SelectedIssue
        {
            get => _selectedIssue;
            set => SetProperty(ref _selectedIssue, value);
        }

        public QuickAction? CurrentQuickAction
        {
            get => _currentQuickAction;
            set => SetProperty(ref _currentQuickAction, value);
        }

        private void DecisionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IIssueDecisionViewModel.Operations))
                OnUpdateOperations();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IssueDecisionWrapperViewModel.Execute))
            {
                OnUpdateOperations();

                if (!IsBatchOperationActive)
                    CurrentQuickAction = QuickActionFactory.SetExecute((IssueDecisionWrapperViewModel) sender);
            }
        }

        private void OnUpdateOperations()
        {
            var operations = Issues.Where(x => x.Execute).SelectMany(x => x.Decision.Operations).ToList();

            var deletedFiles = operations.OfType<DeleteFileOperation>().Select(x => x.File).ToList();
            foreach (var issue in Issues)
                issue.UpdateDeletedFiles(deletedFiles);
        }
    }
}
