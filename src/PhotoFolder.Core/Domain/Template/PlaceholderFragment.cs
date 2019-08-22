namespace PhotoFolder.Core.Domain.Template
{
    public class PlaceholderFragment : ITemplateFragment
    {
        public PlaceholderFragment(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
