using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Core.Interfaces.Gateways;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PhotoFolder.Wpf.Services;
using PhotoFolder.Wpf.ViewModels.Models;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionManagerViewModel : BindableBase, IDialogAware
    {
        private DecisionManagerContext? _context;

        public string Title { get; } = "You Decide!";

        public DecisionManagerContext? Context
        {
            get => _context;
            set => SetProperty(ref _context, value);
        }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var report = parameters.GetValue<FileCheckReport>("report");
            var photoDirectory = parameters.GetValue<IPhotoDirectory>("photoDirectory");

            var issues = ImportDecisionFactory.Create(report, photoDirectory).Select(x => new IssueDecisionWrapperViewModel(x)).ToList();
            Context = new DecisionManagerContext(issues, photoDirectory);
        }
    }

    public class DecisionManagerContext : BindableBase
    {
        private IssueDecisionWrapperViewModel? _selectedIssue;
        private bool _removeFilesFromOutside;

        public DecisionManagerContext(List<IssueDecisionWrapperViewModel> issues, IPhotoDirectory photoDirectory)
        {
            Issues = issues;
            PhotoDirectory = photoDirectory;

            OnUpdateOperations();

            foreach (var viewModel in Issues)
            {
                viewModel.PropertyChanged += ViewModelOnPropertyChanged;
                viewModel.Decision.PropertyChanged += DecisionOnPropertyChanged;
            }
        }

        public List<IssueDecisionWrapperViewModel> Issues { get; }
        public IPhotoDirectory PhotoDirectory { get; }

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

        private void DecisionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IIssueDecisionViewModel.Operations))
                OnUpdateOperations();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IssueDecisionWrapperViewModel.Execute))
                OnUpdateOperations();
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
