using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels.Models
{
    public class Checkable<T> : BindableBase
    {
        private bool _isChecked;

        public Checkable(T value, bool isChecked)
        {
            Value = value;
            IsChecked = isChecked;
        }

        public Checkable(T value) : this(value, false)
        {
        }

        public T Value { get; }

        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }
    }
}