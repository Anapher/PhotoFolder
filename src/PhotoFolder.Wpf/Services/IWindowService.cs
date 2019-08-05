using System;
using System.Windows;

namespace PhotoFolder.Wpf.Services
{
    public interface IWindowService
    {
        bool ShowFolderBrowserDialog(FolderBrowserDialogOptions options, out string selectedPath);
        bool ShowFileSelectionDialog(string filter, out string[] selectedFiles);

        /// <summary>
        ///     Displays a message box.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">
        ///     One of the <see cref="MessageBoxButton" /> values that specifies which buttons to display in the
        ///     message box.
        /// </param>
        /// <param name="icon">
        ///     One of the <see cref="MessageBoxImage" /> values that specifies which icon to display in the message
        ///     box.
        /// </param>
        /// <param name="defResult">
        ///     One of the <see cref="MessageBoxResult" /> values that specifies the default button for the
        ///     message box.
        /// </param>
        /// <param name="options">
        ///     One of the <see cref="MessageBoxOptions" /> values that specifies which display and association
        ///     options will be used for the message box. You may pass in 0 if you wish to use the defaults.
        /// </param>
        /// <returns>One of the <see cref="MessageBoxResult" /> values</returns>
        MessageBoxResult ShowMessageBox(string? text, string? caption, MessageBoxButton buttons, MessageBoxImage icon,
            MessageBoxResult defResult, MessageBoxOptions options);
    }

    public class FolderBrowserDialogOptions
    {
        public string? Description { get; set; }
        public Environment.SpecialFolder RootFolder { get; set; }
        public bool ShowNewFolderButton { get; set; }
    }
}
