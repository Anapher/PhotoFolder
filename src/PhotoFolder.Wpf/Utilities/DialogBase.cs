using System;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace PhotoFolder.Wpf.Utilities
{
    public abstract class DialogBase : BindableBase, IDialogAware
    {
        private string _title = string.Empty;
        public virtual bool CanCloseDialog() => true;

        public virtual void OnDialogClosed()
        {
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public event Action<IDialogResult> RequestClose;

        protected void OnRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }
    }
}
