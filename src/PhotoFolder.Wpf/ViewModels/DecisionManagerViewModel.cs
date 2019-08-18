using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Core.Interfaces.Gateways;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using PhotoFolder.Wpf.Services;

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

            var decisions = ImportDecisionFactory.Create(report, photoDirectory);
            Context = new DecisionManagerContext(decisions);
        }
    }

    public class DecisionManagerContext : BindableBase
    {
        private IFileDecisionViewModel? _selectedDecision;
        private bool _moveFilesFromOutside;

        public DecisionManagerContext(List<IFileDecisionViewModel> decisions)
        {
            Decisions = decisions;
        }

        public List<IFileDecisionViewModel> Decisions { get; }

        public bool MoveFilesFromOutside
        {
            get => _moveFilesFromOutside;
            set => SetProperty(ref _moveFilesFromOutside, value);
        }

        public IFileDecisionViewModel? SelectedDecision
        {
            get => _selectedDecision;
            set => SetProperty(ref _selectedDecision, value);
        }
    }
}
