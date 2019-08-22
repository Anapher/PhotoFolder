namespace PhotoFolder.Core.Domain.Template
{
    public class TextFragment : ITemplateFragment
    {
        public TextFragment(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
