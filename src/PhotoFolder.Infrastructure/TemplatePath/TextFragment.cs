namespace PhotoFolder.Infrastructure.TemplatePath
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
