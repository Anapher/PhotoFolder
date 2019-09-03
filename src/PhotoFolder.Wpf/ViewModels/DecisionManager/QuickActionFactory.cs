using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Wpf.ViewModels.Models;

namespace PhotoFolder.Wpf.ViewModels.DecisionManager
{
    public static class QuickActionFactory
    {
        public static QuickAction SetExecute(IssueDecisionWrapperViewModel viewModel)
        {
            var execute = viewModel.Execute;
            var description = GetIssueTitle(execute ? "Execute" : "Don't execute", viewModel.Decision.Issue);

            return new QuickAction(viewModel, description, ActionForEveryViewModel(x => x.Execute = execute), GetDecisionFilters(viewModel.Decision).ToList());
        }

        public static QuickAction Create(IssueDecisionWrapperViewModel viewModel, string actionName, Action<IEnumerable<IssueDecisionWrapperViewModel>> action)
        {
            var description = GetIssueTitle(actionName, viewModel.Decision.Issue);

            return new QuickAction(viewModel, description, action, GetDecisionFilters(viewModel.Decision).ToList());
        }

        private static Action<IEnumerable<IssueDecisionWrapperViewModel>> ActionForEveryViewModel(Action<IssueDecisionWrapperViewModel> action)
        {
            return models =>
            {
                foreach (var model in models)
                    action(model);
            };
        }

        private static IEnumerable<IQuickActionFileFilter> GetDecisionFilters(IIssueDecisionViewModel decision)
        {
            yield return GetFileLocationFilter(decision);

            if (decision is InvalidLocationFileDecisionViewModel invalidLocationIssue)
                yield return GetTargetPathFilter(invalidLocationIssue);

            yield return GetCreatedOnFilter(decision);
        }

        private static IQuickActionFileFilter GetFileLocationFilter(IIssueDecisionViewModel decision)
        {
            var directory = Path.GetDirectoryName(decision.Issue.File.Filename);
            var displayDirectory = Path.GetDirectoryName(decision.Issue.File.RelativeFilename ?? decision.Issue.File.Filename);
            if (displayDirectory == string.Empty)
                displayDirectory = "/";

            return new DelegateQuickActionFileFilter($"from \"{displayDirectory}\"",
                list => list.Where(x => Path.GetDirectoryName(x.Decision.Issue.File.Filename) == directory));
        }

        private static IQuickActionFileFilter GetTargetPathFilter(InvalidLocationFileDecisionViewModel decision)
        {
            var directory = Path.GetDirectoryName(decision.TargetPath);

            return new DelegateQuickActionFileFilter($"with target \"{directory}\"",
                list => list.Where(x =>
                    x.Decision is InvalidLocationFileDecisionViewModel invalidLocation && Path.GetDirectoryName(invalidLocation.TargetPath) == directory));
        }

        private static IQuickActionFileFilter GetCreatedOnFilter(IIssueDecisionViewModel decision)
        {
            var createdOn = decision.Issue.File.FileCreatedOn.Date;

            return new DelegateQuickActionFileFilter($"created on \"{createdOn:d}\"",
                list => list.Where(x => x.Decision.Issue.File.FileCreatedOn.Date == createdOn));
        }

        private static string GetIssueTitle(string action, IFileIssue issue)
        {
            var issueName = GetIssueName(issue);
            return $"{action} all {issueName} issues...";
        }

        private static string GetIssueName(IFileIssue issue)
        {
            switch (issue)
            {
                case InvalidFileLocationIssue _:
                    return "Wrong Location";
                case SimilarFilesIssue _:
                    return "Similar Files";
                case DuplicateFilesIssue _:
                    return "Duplicate Files";
                case FormerlyDeletedIssue _:
                    return "Formerly Deleted Files";
                default:
                    throw new ArgumentOutOfRangeException(nameof(issue));
            }
        }
    }
}
