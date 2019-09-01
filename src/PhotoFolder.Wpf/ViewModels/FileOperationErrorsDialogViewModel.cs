using System;
using System.Collections.Generic;
using Humanizer;
using PhotoFolder.Application.Dto;
using PhotoFolder.Wpf.Utilities;
using Prism.Services.Dialogs;

namespace PhotoFolder.Wpf.ViewModels
{
    public class FileOperationErrorsDialogViewModel : DialogBase
    {
        private IReadOnlyDictionary<IFileOperation, Exception>? _exceptions;
        private bool _removeFilesFromOutside;

        public IReadOnlyDictionary<IFileOperation, Exception>? Exceptions
        {
            get => _exceptions;
            private set => SetProperty(ref _exceptions, value);
        }

        public bool RemoveFilesFromOutside
        {
            get => _removeFilesFromOutside;
            set => SetProperty(ref _removeFilesFromOutside, value);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            Exceptions = parameters.GetValue<IReadOnlyDictionary<IFileOperation, Exception>>("errors");
            RemoveFilesFromOutside = parameters.GetValue<bool>("removeFilesFromOutside");
            Title = $"{"error".ToQuantity(Exceptions.Count)} occurred";
        }
    }
}
