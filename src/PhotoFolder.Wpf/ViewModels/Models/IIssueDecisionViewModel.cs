using System.Collections.Generic;
using System.ComponentModel;
using PhotoFolder.Application.Dto;
using PhotoFolder.Core.Dto.Services;

namespace PhotoFolder.Wpf.ViewModels.Models
{
    public interface IIssueDecisionViewModel : INotifyPropertyChanged
    {
        bool IsRecommended { get; }
        IReadOnlyList<IFileOperation> Operations { get; }
        IFileIssue Issue { get; }

        bool UpdateDeletedFiles(IReadOnlyList<FileInformation> deletedFiles);
    }
}