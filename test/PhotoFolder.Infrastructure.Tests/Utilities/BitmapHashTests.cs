using PhotoFolder.Infrastructure.Utilities;
using System.Drawing;
using Xunit;

namespace PhotoFolder.Infrastructure.Tests.Utilities
{
    public class BitmapHashTests
    {
        [Fact]
        public void TestComputeHash()
        {
            using (var imageStream = TestImages.Flora)
            using (var bmp = (Bitmap) Image.FromStream(imageStream))
            {
                var hash = BitmapHash.Compute(bmp);
                Assert.NotNull(hash);
            }
        }

        [Fact]
        public void TestCompareUnrelatedPictures()
        {
            using (var imageStream1 = TestImages.Flora)
            using (var bmp1 = (Bitmap) Image.FromStream(imageStream1))
            using (var imageStream2 = TestImages.Lando)
            using (var bmp2 = (Bitmap) Image.FromStream(imageStream2))
            {
                var hash1 = BitmapHash.Compute(bmp1);
                var hash2 = BitmapHash.Compute(bmp2);

                var diff = BinaryUtils.ComputeByteArrayEquality(hash1, hash2);
                Assert.True(diff < 0.9);
            }
        }

        [Fact]
        public void TestCompareCompressedPicture()
        {
            using (var imageStream1 = TestImages.Flora)
            using (var bmp1 = (Bitmap)Image.FromStream(imageStream1))
            using (var imageStream2 = TestImages.FloraCompressed)
            using (var bmp2 = (Bitmap)Image.FromStream(imageStream2))
            {
                var hash1 = BitmapHash.Compute(bmp1);
                var hash2 = BitmapHash.Compute(bmp2);

                var diff = BinaryUtils.ComputeByteArrayEquality(hash1, hash2);
                Assert.True(diff > 0.9);
            }
        }
    }
}
