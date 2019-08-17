using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Core.Interfaces.Gateways;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoFolder.Wpf.ViewModels
{
    public class ImportDialogViewModel : BindableBase, IDialogAware
    {
        public string Title { get; } = "Import files";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var report = parameters.GetValue<FileCheckReport>("report");
            var photoDirectory = parameters.GetValue<IPhotoDirectory>("photoDirectory");
        }
    }
}
