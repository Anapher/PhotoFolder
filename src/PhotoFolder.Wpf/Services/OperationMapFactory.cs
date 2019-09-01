using System.Collections.Generic;
using System.Linq;
using PhotoFolder.Application.Dto;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Wpf.ViewModels.Models;

namespace PhotoFolder.Wpf.Services
{
    public static class OperationMapFactory
    {
        public static IEnumerable<FileOperationInfo> Create(IEnumerable<IIssueDecisionViewModel> issues, bool removeFilesFromOutside, IReadOnlyList<IndexedFile> indexedFiles)
        {
            // Filter operations, delete operations overwrite move operations
            var operationsQuery = issues.SelectMany(decision => decision.Operations.Select(operation => (decision.Issue, operation)))
                .GroupBy(x => x.operation.File.Filename).Select(x =>
                    x.Any(y => y.operation is DeleteFileOperation) ? x.First(y => y.operation is DeleteFileOperation) : x.First());

            // exclude files from outside if it is not configured
            if (!removeFilesFromOutside)
                operationsQuery = operationsQuery.Where(x => !(x.operation is DeleteFileOperation deleteOperation) || deleteOperation.File.RelativeFilename != null);

            var operations = operationsQuery.ToList();

            // apply the operations on an internal list so we can get the real changes
            var currentFileBase = indexedFiles.SelectMany(x => x.Files.Select(y => (y.RelativeFilename, x.Hash)))
                .ToDictionary(x => x.RelativeFilename, x => x.Hash);

            var newFileBase = currentFileBase.ToDictionary(x => x.Key, x => x.Value);

            foreach (var (_, operation) in operations.OrderByDescending(x => x.operation is DeleteFileOperation))
            {
                if (operation is DeleteFileOperation deleteOperation && deleteOperation.File.RelativeFilename != null)
                {
                    newFileBase.Remove(deleteOperation.File.RelativeFilename);
                }
                else if (operation is MoveFileOperation moveFileOperation && moveFileOperation.File.RelativeFilename == null)
                {
                    newFileBase[moveFileOperation.TargetPath] = moveFileOperation.File.Hash.ToString();
                }
            }

            var currentFiles = currentFileBase.Values.ToHashSet();
            var newFiles = newFileBase.Values.ToHashSet();

            // compile list
            return CompileList(operations, currentFiles, newFiles);
        }

        private static IEnumerable<FileOperationInfo> CompileList(IEnumerable<(IFileIssue, IFileOperation)> operations, ISet<string> currentFiles, ISet<string> newFiles)
        {
            foreach (var (issue, operation) in operations)
            {
                var existsCurrently = currentFiles.Contains(operation.File.Hash.ToString());
                var willExist = newFiles.Contains(operation.File.Hash.ToString());

                FileBaseChange change;
                if (existsCurrently == willExist)
                    change = FileBaseChange.NoChanges;
                else if (existsCurrently) // && !willExist
                    change = FileBaseChange.FileDeleted;
                else change = FileBaseChange.NewFile; // !existsCurrently && willExist

                yield return new FileOperationInfo(operation, issue, change);
            }
        }
    }

    public class FileOperationInfo
    {
        public FileOperationInfo(IFileOperation operation, IFileIssue issue, FileBaseChange fileBaseChange)
        {
            Operation = operation;
            Issue = issue;
            FileBaseChange = fileBaseChange;
        }

        public IFileOperation Operation { get; }
        public IFileIssue Issue { get; }
        public FileBaseChange FileBaseChange { get; }
    }

    public enum FileBaseChange
    {
        NoChanges,
        NewFile,
        FileDeleted
    }
}
