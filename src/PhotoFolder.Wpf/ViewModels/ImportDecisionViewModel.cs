using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.UseCaseResponses;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels
{
    public interface IFileAnalysis
    {
        string Description { get; }

        bool IsSafeAction { get; }
        FileAction Action { get; }
    }

    public interface IFileDecisionViewModel
    {
        IImmutableList<FileDecision> PossibleDecisions { get; }
        FileDecision RecommendedDecision { get; }

        string Description { get; }
        bool IsInPhotoDirectory { get; }

        FileDecision SelectedDecision { get; set; }
        FileInformation File { get; }

        string DirectoryPathTemplate { get; }
        string? TargetPath { get; set; }
        IImmutableList<string> RecommendedPaths { get; }
    }

    public class EqualFileDecisionViewModel : BindableBase, IFileDecisionViewModel
    {
        public EqualFileDecisionViewModel(IReadOnlyList<IFileDecisionViewModel> children)
        {
            Children = children;
        }

        public IReadOnlyList<IFileDecisionViewModel> Children { get; }

        public IImmutableList<FileDecision> PossibleDecisions { get; } = ImmutableList<FileDecision>.Empty;
        public FileDecision RecommendedDecision { get; } = FileDecision.None;

        public string Description => $"{Children.Count} equal files";
        public bool IsInPhotoDirectory => Children.First().IsInPhotoDirectory;
        public FileDecision SelectedDecision { get; set; }
        public FileInformation File => Children.First().File;
        public string DirectoryPathTemplate => Children.First().DirectoryPathTemplate;

        public string? TargetPath { get; set; }
        public IImmutableList<string> RecommendedPaths { get; } = ImmutableList<string>.Empty;
    }

    public class FileDecisionViewModel : BindableBase, IFileDecisionViewModel
    {
        private string? _targetPath;
        private FileDecision _selectedDecision = FileDecision.None;

        private static readonly IImmutableList<FileDecision> RootDecisions =
            new[] {FileDecision.None, FileDecision.DecideLater}.ToImmutableList();

        public FileDecisionViewModel(FileInformation file, string directoryPathTemplate, IReadOnlyList<IFileAnalysis> analysis, bool isInPhotoDirectory)
        {
            IsInPhotoDirectory = isInPhotoDirectory;
            File = file;
            DirectoryPathTemplate = directoryPathTemplate;

            PossibleDecisions = RootDecisions.AddRange(analysis.Select(x => (FileDecision) x.Action).Distinct());
            RecommendedDecision = analysis.Where(x => x.IsSafeAction).Select(x => (FileDecision?) x.Action)
                .FirstOrDefault() ?? FileDecision.None;

            var wrongPlaced = analysis.OfType<WrongPlacedFileAnalysis>().FirstOrDefault();
            if (wrongPlaced != null)
            {
                RecommendedPaths = wrongPlaced.RecommendedPaths;
                TargetPath = RecommendedPaths.FirstOrDefault();
            }
            else
                RecommendedPaths = ImmutableList<string>.Empty;

            if (analysis.OfType<FileDuplicationAnalysis>().Any())
                Description = isInPhotoDirectory ? "Duplicate" : "Already exists";
            else if (analysis.OfType<SimilarFilesAnalysis>().Any())
                Description = "Similar file found";
            else if (wrongPlaced != null)
                Description = isInPhotoDirectory ? "Wrong Location" : "Import";
            else Description = "Unknown Analysis";
        }

        public IImmutableList<FileDecision> PossibleDecisions { get; }
        public FileDecision RecommendedDecision { get; }
        public string Description { get; }
        public bool IsInPhotoDirectory { get; }
        public FileInformation File { get; }
        public string DirectoryPathTemplate { get; }
        public IImmutableList<string> RecommendedPaths { get; }

        public string? TargetPath
        {
            get => _targetPath;
            set => SetProperty(ref _targetPath, value);
        }

        public FileDecision SelectedDecision
        {
            get => _selectedDecision;
            set => SetProperty(ref _selectedDecision, value);
        }
    }

    /// <summary>
    ///     The file is not in the directory, but equal files already exist
    /// </summary>
    public class FileDuplicationAnalysis : IFileAnalysis
    {
        public FileDuplicationAnalysis(IReadOnlyList<FileLocation> equalFiles)
        {
            if (!equalFiles.Any()) throw new ArgumentException("Must have at least one equal file");

            EqualFiles = equalFiles;
        }

        public IReadOnlyList<FileLocation> EqualFiles { get; }
        public string Description => $"File was already found at {EqualFiles.Count} location{EqualFiles.GetSuffix()}.";
        public bool IsSafeAction { get; } = true;
        public FileAction Action { get; } = FileAction.Delete;
    }

    public class SimilarFilesAnalysis : IFileAnalysis
    {
        public SimilarFilesAnalysis(IReadOnlyList<SimilarFile> similarFiles)
        {
            SimilarFiles = similarFiles;
        }

        public IReadOnlyList<SimilarFile> SimilarFiles { get; }

        public string Description =>
            $"{SimilarFiles.Count} similar file{SimilarFiles.GetSuffix()} {(SimilarFiles.Count == 1 ? "was" : "were")} found (greatest similarity: {SimilarFiles.Max(x => x.Similarity)}).";

        public FileAction Action { get; } = FileAction.Delete;
        public bool IsSafeAction { get; } = false;
    }

    public class WrongPlacedFileAnalysis : IFileAnalysis
    {
        public WrongPlacedFileAnalysis(IImmutableList<string> recommendedPaths)
        {
            RecommendedPaths = recommendedPaths;
        }

        public IImmutableList<string> RecommendedPaths { get; }

        public string Description { get; } = $"The file should be relocated.";
        public bool IsSafeAction { get; } = true;
        public FileAction Action { get; } = FileAction.Move;
    }

    public static class CollectionStringExtensions
    {
        public static string GetSuffix<T>(this IReadOnlyList<T> collection) => collection.Count == 1 ? "" : "s";
    }

    public enum FileDecision
    {
        Delete,
        Move,

        None = 50,
        DecideLater
    }

    public enum FileLocationType
    {
        CorrectPath,
        Outside,
        WrongPlaced
    }

    public enum FileAction
    {
        Delete,
        Move,
    }
}