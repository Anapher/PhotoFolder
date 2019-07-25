using PhotoFolder.Core;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace PhotoFolder.Infrastructure.Tests
{
    public class TestImage
    {
        public static TestImage Flora => new TestImage("flora_sonyalpha.jpg", 1200, 900, "D784CE783098E15496EEC82585F37E0B73BBA78503A656C8E1A55F1F6B723C4A", new DateTimeOffset(2010, 5, 9, 10, 30, 45, TimeSpan.FromHours(2)));
        public static TestImage FloraCompressed => new TestImage("flora_sonyalpha_compressed.jpg", 1200, 900, "22DB71BE2CE4B71E8F08A7858E9F37222DFE41C9976209603ACA9E43F7BE7A68", new DateTimeOffset(2010, 5, 9, 10, 30, 45, TimeSpan.FromHours(2)));
        public static TestImage Lando => new TestImage("lando_sonyalpha.jpg", 1200, 800, "4A97356B90A4773EF69E7516D0FEE90356E1435BFC5DB6AC77D306C950DDB0D2", new DateTimeOffset(2018, 1, 20, 12, 56, 17, TimeSpan.FromHours(1)));
        public static TestImage HansZimmer => new TestImage("hanszimmer_htcu11.jpg", 1200, 675, "43BCDC45439F4DCE549A5F6C29BE1218A5398A849904DFC114574FB86F409DBB", new DateTimeOffset(2019, 3, 30, 19, 2, 22, TimeSpan.FromHours(1)));
        public static TestImage Egypt => new TestImage("egypt_sonyz3.jpg", 1200, 675, "8CB06F038DA283D50AACDA14FC08AF25C77D757D69DAE24AFC248EA457DB2D2B", new DateTimeOffset(2015, 10, 25, 7, 45, 7, TimeSpan.FromHours(1)));

        public static TestImage[] AllDistinct => new[] { Flora, Lando, HansZimmer, Egypt };
        public static TestImage[] All => AllDistinct.Concat(new[] { FloraCompressed }).ToArray();

        public static TheoryData<TestImage> AllDistinctTheoryData => new TheoryData<TestImage> { Flora, Lando, HansZimmer, Egypt };
        public static TheoryData<TestImage> AllTheoryData => new TheoryData<TestImage> { Flora, Lando, HansZimmer, Egypt, FloraCompressed };

        private readonly string _filename;

        private TestImage(string filename, int width, int height, string hash, DateTimeOffset createdOn)
        {
            _filename = filename;
            Width = width;
            Height = height;
            Hash = Hash.Parse(hash);
            CreatedOn = createdOn;
        }

        public int Width { get; }
        public int Height { get; }
        public Hash Hash { get; }
        public DateTimeOffset CreatedOn { get; }

        public Stream GetStream() =>
            ReadEmbeddedResource($"PhotoFolder.Infrastructure.Tests.Resources.{_filename}");

        private static Stream ReadEmbeddedResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(name);
        }
    }
}
