using System.Collections.Generic;
using System.Linq;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Specifications.FileInformation;
using PhotoFolder.Wpf.Extensions;
using PhotoFolder.Wpf.Services;
using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionManagerMenuViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;
        private readonly IWindowService _windowService;
        private DelegateCommand? _applyCopyMoveActionsCommand;
        private DelegateCommand? _applyRecommendedActionsCommand;
        private DelegateCommand? _applyRemoveActionsCommand;
        private DecisionManagerContext? _decisionContext;
        private DelegateCommand? _unsetAllActionsCommand;
        private AsyncDelegateCommand? _reviewAndExecuteCommand;

        public DecisionManagerMenuViewModel(IDialogService dialogService, IWindowService windowService)
        {
            _dialogService = dialogService;
            _windowService = windowService;
        }

        public DecisionManagerContext? DecisionContext
        {
            get => _decisionContext;
            set => SetProperty(ref _decisionContext, value);
        }

        public DelegateCommand ApplyRecommendedActionsCommand
        {
            get
            {
                return _applyRecommendedActionsCommand ??= new DelegateCommand(() =>
                {
                    if (DecisionContext == null) return;

                    foreach (var issue in DecisionContext.Issues.Where(x => x.Decision.IsRecommended))
                        issue.Execute = true;
                });
            }
        }

        public DelegateCommand UnsetAllActionsCommand
        {
            get
            {
                return _unsetAllActionsCommand ??= new DelegateCommand(() =>
                {
                    if (DecisionContext == null) return;

                    foreach (var issue in DecisionContext.Issues)
                        issue.Execute = false;
                });
            }
        }

        public DelegateCommand ApplyCopyMoveActionsCommand
        {
            get
            {
                return _applyCopyMoveActionsCommand ??= new DelegateCommand(() =>
                {
                    if (DecisionContext == null) return;

                    foreach (var issue in DecisionContext.Issues.Where(x => x.Decision is InvalidLocationFileDecisionViewModel))
                        issue.Execute = true;
                });
            }
        }

        public DelegateCommand ApplyRemoveActionsCommand
        {
            get
            {
                return _applyRemoveActionsCommand ??= new DelegateCommand(() =>
                {
                    if (DecisionContext == null) return;

                    foreach (var issue in DecisionContext.Issues.Where(x => x.Decision is DuplicateFileDecisionViewModel))
                        issue.Execute = true;
                });
            }
        }

        public AsyncDelegateCommand ReviewAndExecuteCommand
        {
            get
            {
                return _reviewAndExecuteCommand ??= new AsyncDelegateCommand(async () =>
                {
                    if (DecisionContext == null) return;

                    IReadOnlyList<IndexedFile> indexedFiles;
                    using (var appDbContext = DecisionContext.PhotoDirectory.GetDataContext())
                        indexedFiles = await appDbContext.FileRepository.GetAllBySpecs(new IncludeFileLocationsSpec());

                    var operations = OperationMapFactory.Create(DecisionContext.Issues.Where(x => x.Execute).Select(x => x.Decision), DecisionContext.RemoveFilesFromOutside,
                        indexedFiles).ToList();

                    if (!operations.Any())
                    {
                        _windowService.ShowError("The selected issues don't result in any operations.");
                        return;
                    }

                    var parameters = new DialogParameters
                    {
                        {"photoDirectory", DecisionContext.PhotoDirectory},
                        {"operations", operations},
                        {"removeFilesFromOutside", DecisionContext.RemoveFilesFromOutside}
                    };

                    _dialogService.ShowDialog("ReviewOperations", parameters, result =>
                    {
                        if (result.Result == ButtonResult.OK)
                            DecisionContext.ResyncDatabaseAction();
                    });
                });
            }
        }

        public void Initialize(DecisionManagerContext context)
        {
            DecisionContext = context;
        }
    }
}