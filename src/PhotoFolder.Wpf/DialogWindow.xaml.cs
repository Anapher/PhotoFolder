using Prism.Services.Dialogs;

namespace PhotoFolder.Wpf
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : IDialogWindow
    {
        public DialogWindow()
        {
            InitializeComponent();
        }

        public IDialogResult? Result { get; set; }
    }
}
