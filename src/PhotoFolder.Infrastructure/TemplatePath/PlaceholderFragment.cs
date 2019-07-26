namespace PhotoFolder.Infrastructure.TemplatePath
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
