using PhotoFolder.Wpf.Utilities;
using System.Windows;

namespace PhotoFolder.Wpf.Services
{
    public class WindowService : IWindowService
    {
        public bool ShowFileSelectionDialog(string filter, out string[] selectedFiles)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog
            {
                Filter = filter,
                Multiselect = true
            };

            using (ofd)
            {
                var result = ofd.ShowDialog(new WindowWrapper(System.Windows.Application.Current.MainWindow));
                selectedFiles = ofd.FileNames;

                return result == System.Windows.Forms.DialogResult.OK ? true : false;
            }
        }

        public bool ShowFolderBrowserDialog(FolderBrowserDialogOptions options, out string selectedPath)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = options.Description,
                RootFolder = options.RootFolder,
                ShowNewFolderButton = options.ShowNewFolderButton,
                UseDescriptionForTitle = true
            };

            using (dialog)
            {
                var result = dialog.ShowDialog(new WindowWrapper(System.Windows.Application.Current.MainWindow));
                selectedPath = dialog.SelectedPath;

                return result == System.Windows.Forms.DialogResult.OK ? true : false;
            }
        }

        public MessageBoxResult ShowMessageBox(string? text, string? caption, MessageBoxButton buttons,
            MessageBoxImage icon, MessageBoxResult defResult, MessageBoxOptions options) =>
            MessageBoxEx.Show(System.Windows.Application.Current.MainWindow, text, caption, buttons, icon, defResult, options);
    }
}
