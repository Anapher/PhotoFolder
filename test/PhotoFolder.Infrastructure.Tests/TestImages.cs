using System.IO;
using System.Reflection;

namespace PhotoFolder.Infrastructure.Tests
{
    public static class TestImages
    {
        public static Stream Flora => ReadEmbeddedResource("flora.jpg");
        public static Stream FloraCompressed => ReadEmbeddedResource("flora_compressed.jpg");
        public static Stream Lando => ReadEmbeddedResource("lando.jpg");

        private static Stream ReadEmbeddedResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream($"PhotoFolder.Infrastructure.Tests.Resources.{name}");
        }
    }
}
