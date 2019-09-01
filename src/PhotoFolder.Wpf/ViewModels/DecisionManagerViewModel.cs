using System;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Core.Interfaces.Gateways;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PhotoFolder.Application.Dto;
using PhotoFolder.Wpf.Services;
using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels.Models;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionManagerViewModel : DialogBase
    {
        private DecisionManagerContext? _context;

        public DecisionManagerContext? Context
        {
            get => _context;
            set => SetProperty(ref _context, value);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            var report = parameters.GetValue<FileCheckReport>("report");
            var photoDirectory = parameters.GetValue<IPhotoDirectory>("photoDirectory");

            var issues = ImportDecisionFactory.Create(report, photoDirectory).Select(x => new IssueDecisionWrapperViewModel(x)).ToList();
            Context = new DecisionManagerContext(issues, photoDirectory, ResyncDatabaseAction);

            Title = "You Decide!";
        }

        private void ResyncDatabaseAction()
        {
            OnRequestClose(new DialogResult(ButtonResult.OK));
        }
    }

    public class DecisionManagerContext : BindableBase
    {
        private IssueDecisionWrapperViewModel? _selectedIssue;
        private bool _removeFilesFromOutside;

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
        }

        public List<IssueDecisionWrapperViewModel> Issues { get; }
        public IPhotoDirectory PhotoDirectory { get; }
        public Action ResyncDatabaseAction { get; }

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
