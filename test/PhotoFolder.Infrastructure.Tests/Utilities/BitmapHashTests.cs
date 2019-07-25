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
            using (var imageStream = TestImage.Flora.GetStream())
            using (var bmp = (Bitmap) Image.FromStream(imageStream))
            {
                var hash = BitmapHash.Compute(bmp);
                Assert.NotNull(hash);
            }
        }

        [Fact]
        public void TestCompareUnrelatedPictures()
        {
            using (var imageStream1 = TestImage.Flora.GetStream())
            using (var bmp1 = (Bitmap) Image.FromStream(imageStream1))
            using (var imageStream2 = TestImage.Lando.GetStream())
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
            using (var imageStream1 = TestImage.Flora.GetStream())
            using (var bmp1 = (Bitmap)Image.FromStream(imageStream1))
            using (var imageStream2 = TestImage.FloraCompressed.GetStream())
            using (var bmp2 = (Bitmap)Image.FromStream(imageStream2))
            {
                var hash1 = BitmapHash.Compute(bmp1);
                var hash2 = BitmapHash.Compute(bmp2);

                var diff = BinaryUtils.ComputeByteArrayEquality(hash1, hash2);
                Assert.True(diff > 0.9);
            }
        }

        [Fact]
        public void TestRotateBitmapHash()
        {
            byte[] matrix = new byte[] {
                0b00010001, 0b00110001,
                0b01100111, 0b01011101,
                0b00111010, 0b10001010,
                0b01110101, 0b11111101,
                0b00011101, 0b01011101,
                0b10100100, 0b00110011,
                0b01101011, 0b11000100,
                0b01110011, 0b10000111,
                0b00010100, 0b01100101,
                0b11001111, 0b00111110,
                0b01100011, 0b10011100,
                0b01000110, 0b01100000,
                0b00011111, 0b01010110,
                0b00010111, 0b11110001,
                0b10011001, 0b00101101,
                0b00010100, 0b10010010
            };

            byte[] expectedResult = new byte[]
            {
                0b01000010, 0b00100000,
                0b00001110, 0b11001010,
                0b00000100, 0b11101110,
                0b11110001, 0b10011101,
                0b01010010, 0b01010100,
                0b10111011, 0b00111010,
                0b00111110, 0b11000110,
                0b01110110, 0b11011011,
                0b10100100, 0b11001100,
                0b00111001, 0b01011010,
                0b01101011, 0b00101001,
                0b10110110, 0b00111011,
                0b01000110, 0b00011110,
                0b01010111, 0b11011010,
                0b10010010, 0b10100100,
                0b01100001, 0b10111011,
            };

            var result = BitmapHash.RotateBitmapHash(matrix);
            Assert.Equal(expectedResult, result);
        }
    }
}
