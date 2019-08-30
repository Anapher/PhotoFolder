using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Domain.Template;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Wpf.ViewModels.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionInvalidLocationViewModel : BindableBase
    {
        private readonly IPathUtils _pathUtils;
        private DelegateCommand? _applyCommand;
        private string? _customPath;
        private InvalidLocationFileDecisionViewModel? _decision;
        private DecisionScope _decisionScope;
        private IssueDecisionWrapperViewModel? _decisionWrapper;
        private string? _filePathRegex;
        private TemplateString? _filePathTemplate;
        private bool _isNewPathValid;
        private InvalidFileLocationIssue? _issue;
        private string? _newPath;
        private PathConfigurator _pathConfigurator;
        private IList<KeyValueViewModel>? _pathVariables;
        private FilenameSuggestion? _selectedSuggestion;
        private IReadOnlyList<FilenameSuggestion>? _suggestions;

        public DecisionInvalidLocationViewModel(IPathUtils pathUtils)
        {
            _pathUtils = pathUtils;
        }

        public DecisionAssistantContext? Context { get; private set; }

        public IssueDecisionWrapperViewModel? DecisionWrapper
        {
            get => _decisionWrapper;
            set => SetProperty(ref _decisionWrapper, value);
        }

        public InvalidLocationFileDecisionViewModel? Decision
        {
            get => _decision;
            set => SetProperty(ref _decision, value);
        }

        public InvalidFileLocationIssue? Issue
        {
            get => _issue;
            set => SetProperty(ref _issue, value);
        }

        public FilenameSuggestion? SelectedSuggestion
        {
            get => _selectedSuggestion;
            set
            {
                if (SetProperty(ref _selectedSuggestion, value))
                {
                    PathConfigurator = PathConfigurator.Suggestions;
                    UpdateNewPath();
                }
            }
        }

        public IList<KeyValueViewModel>? PathVariables
        {
            get => _pathVariables;
            set => SetProperty(ref _pathVariables, value);
        }

        public string? CustomPath
        {
            get => _customPath;
            set
            {
                if (SetProperty(ref _customPath, value))
                {
                    PathConfigurator = PathConfigurator.Custom;
                    UpdateNewPath();
                }
            }
        }

        public TemplateString? FilePathTemplate
        {
            get => _filePathTemplate;
            set => SetProperty(ref _filePathTemplate, value);
        }

        public string? FilePathRegex
        {
            get => _filePathRegex;
            set => SetProperty(ref _filePathRegex, value);
        }

        public string? NewPath
        {
            get => _newPath;
            set
            {
                if (SetProperty(ref _newPath, value) && value != null && FilePathRegex != null)
                    IsNewPathValid = Regex.IsMatch(value, FilePathRegex);
            }
        }

        public bool IsNewPathValid
        {
            get => _isNewPathValid;
            set => SetProperty(ref _isNewPathValid, value);
        }

        public PathConfigurator PathConfigurator
        {
            get => _pathConfigurator;
            set
            {
                if (SetProperty(ref _pathConfigurator, value))
                    UpdateNewPath();
            }
        }

        public DecisionScope DecisionScope
        {
            get => _decisionScope;
            set => SetProperty(ref _decisionScope, value);
        }

        public IReadOnlyList<FilenameSuggestion>? Suggestions
        {
            get => _suggestions;
            set => SetProperty(ref _suggestions, value);
        }

        public DelegateCommand ApplyCommand
        {
            get
            {
                return _applyCommand ??= new DelegateCommand(() =>
                {
                    if (Decision == null) throw new InvalidOperationException();
                    if (Context == null) throw new InvalidOperationException();
                    if (NewPath == null) throw new InvalidOperationException();
                    if (Issue == null) throw new InvalidOperationException();

                    Decision.TargetPath = NewPath;

                    if (DecisionScope != DecisionScope.CurrentIssue)
                    {
                        IEnumerable<IssueDecisionWrapperViewModel> decisions;
                        var targetDirectory = Issue.DirectoryPathTemplate.ToString();

                        switch (DecisionScope)
                        {
                            case DecisionScope.SameSource:
                                var currentFileDirectory = _pathUtils.GetDirectoryName(Issue.File.Filename);
                                decisions = Context.DecisionManagerContext.Issues.Where(x =>
                                    x.Decision.Issue is InvalidFileLocationIssue issue && issue.DirectoryPathTemplate.ToString() == targetDirectory &&
                                    _pathUtils.GetDirectoryName(issue.File.Filename) == currentFileDirectory);
                                break;
                            case DecisionScope.SameTarget:
                                decisions = Context.DecisionManagerContext.Issues.Where(x =>
                                    x.Decision.Issue is InvalidFileLocationIssue issue && issue.DirectoryPathTemplate.ToString() == targetDirectory);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        foreach (var wrapper in decisions)
                        {
                            var decision = (InvalidLocationFileDecisionViewModel) wrapper.Decision;
                            var issue = (InvalidFileLocationIssue) decision.Issue;

                            decision.TargetPath = _pathUtils.Combine(_pathUtils.GetDirectoryName(NewPath), issue.CorrectFilename);
                            wrapper.Execute = true;
                        }
                    }

                    Context.CloseDialogAction();
                }).ObservesCanExecute(() => IsNewPathValid);
            }
        }

        public void Initialize(DecisionAssistantContext context)
        {
            Context = context;
            DecisionWrapper = context.DecisionWrapper;
            Decision = (InvalidLocationFileDecisionViewModel) context.DecisionWrapper.Decision;
            Issue = (InvalidFileLocationIssue) context.DecisionWrapper.Decision.Issue;

            FilePathTemplate = context.DecisionManagerContext.PhotoDirectory.GetFilenameTemplate(Issue.File);
            FilePathRegex = FilePathTemplate.ToRegexPattern();

            PathVariables = FilePathTemplate.Fragments.OfType<PlaceholderFragment>().Select(x => new KeyValueViewModel(x.Value)).ToList();
            foreach (var viewModel in PathVariables)
                viewModel.PropertyChanged += PathVariableOnPropertyChanged;

            CustomPath = Decision.TargetPath;

            Suggestions = Issue.Suggestions.Select(x => x.Filename).Concat(Context.DecisionManagerContext.Issues.Select(x => x.Decision)
                    .OfType<InvalidLocationFileDecisionViewModel>()
                    .Where(x => ((InvalidFileLocationIssue) x.Issue).DirectoryPathTemplate.ToString() == Issue.DirectoryPathTemplate.ToString())
                    .Select(x => _pathUtils.Combine(_pathUtils.GetDirectoryName(x.TargetPath), Issue.CorrectFilename)).ToArray()).Distinct()
                .Select(x => new FilenameSuggestion(x, x)).ToList();

            SelectedSuggestion = Suggestions.FirstOrDefault(x => x.Filename == Decision.TargetPath) ?? Suggestions.First();
        }

        private void PathVariableOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PathConfigurator = PathConfigurator.Configure;
            UpdateNewPath();
        }

        private void UpdateNewPath()
        {
            switch (PathConfigurator)
            {
                case PathConfigurator.Suggestions:
                    NewPath = SelectedSuggestion?.Filename;
                    break;
                case PathConfigurator.Configure:
                    if (PathVariables == null) throw new InvalidOperationException();
                    if (FilePathTemplate == null) throw new InvalidOperationException();
                    if (Context == null) throw new InvalidOperationException();

                    var path = FilePathTemplate!.ToString(PathVariables.ToDictionary(x => x.Key, x => x.Value));
                    NewPath = Context.DecisionManagerContext.PhotoDirectory.ClearPath(path);
                    break;
                case PathConfigurator.Custom:
                    NewPath = CustomPath;
                    break;
            }
        }
    }

    public enum PathConfigurator
    {
        Suggestions,
        Configure,
        Custom
    }

    public class KeyValueViewModel : BindableBase
    {
        private string _value = string.Empty;

        public KeyValueViewModel(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }

    public enum DecisionScope
    {
        CurrentIssue,
        SameSource,
        SameTarget
    }
}