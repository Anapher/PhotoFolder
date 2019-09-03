using PhotoFolder.Core.Domain;
using PhotoFolder.Infrastructure.Photos;
using Xunit;

namespace PhotoFolder.Infrastructure.Tests.Photos
{
    public class BitmapHashComparerTests
    {
        [Fact]
        public void TestCompareRotatedPicture()
        {
            byte[] picture = {
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
                0b00010100, 0b10010010,
                0, 0, 0, // color hash
            };

            byte[] pictureRotated = {
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
                0, 0, 0, // color hash
            };

            var comparer = new BitmapHashComparer();

            var context = comparer.CreateContext(new Hash(picture));
            var result = comparer.Compare(context, new Hash(pictureRotated));
            Assert.Equal(1F, result);

            // swap arguments, should produce same result
            context = comparer.CreateContext(new Hash(pictureRotated));
            result = comparer.Compare(context, new Hash(picture));
            Assert.Equal(1F, result);
        }
    }
}
