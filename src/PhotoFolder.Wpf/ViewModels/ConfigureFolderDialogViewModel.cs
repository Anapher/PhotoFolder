using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace PhotoFolder.Wpf.ViewModels
{
    public class ConfigureFolderDialogViewModel : BindableBase, IDialogAware
    {
        public string Title { get; } = "Test";
        public string PathTemplate { get; set; } = "{date:yyyy}/{date:MM.dd} - {eventName}/{filename}";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return false;
        }

        public void OnDialogClosed()
        {
            RequestClose?.Invoke(new DialogResult());
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
