using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Core.Interfaces.Gateways;
using Prism.Services.Dialogs;
using System.Linq;
using PhotoFolder.Wpf.Services;
using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels.DecisionManager;
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
}
