using PhotoFolder.Infrastructure.Utilities;
using Xunit;

namespace PhotoFolder.Infrastructure.Tests.Utilities
{
    public class BinaryUtilsTests
    {
        [Theory]
        [InlineData(0b0111011, 5)]
        [InlineData(0b0, 0)]
        [InlineData(0b1, 1)]
        public void TestSumSetBits(int data, int expected)
        {
            var count = BinaryUtils.SumSetBits(data);
            Assert.Equal(expected, count);
        }

        [Theory]
        [InlineData(0b0, 0b0, 0)]
        [InlineData(0b1, 0b1, 0)]
        [InlineData(0b0, 0b1, 1)]
        [InlineData(0b1, 0b0, 1)]
        [InlineData(0b01010101, 0b110110, 4)]
        public void TestGetDifferentBits(byte b1, byte b2, int expected)
        {
            var count = BinaryUtils.GetDifferentBits(b1, b2);
            Assert.Equal(expected, count);
        }

        public static readonly TheoryData<byte[], byte[], float> ComputeByteArrayEqualityTestData = new TheoryData<byte[], byte[], float>
        {
            {new byte[] {1}, new byte[] {1}, 1 },
            {new byte[] {0}, new byte[] {0}, 1 },
            {new byte[] {0b00000000}, new byte[] {0b11111111}, 0 },
            {new byte[] {0b00100010}, new byte[] {0b11111111}, 0.25f },
            {new byte[] {0b00100010}, new byte[] {0b01111011}, 0.5f },
            {new byte[] {0b00100010, 0b11001100}, new byte[] { 0b01111011, 0b11001100 }, 0.75f },

        };

        [Theory]
        [MemberData(nameof(ComputeByteArrayEqualityTestData))]
        public void TestComputeByteArrayEquality(byte[] data1, byte[] data2, float expected)
        {
            var result = BinaryUtils.ComputeByteArrayEquality(data1, data2);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestRotateMatrix()
        {
            ushort[] matrix = new ushort[] {
                0b0001000100110001,
                0b0110011101011101,
                0b0011101010001010,
                0b0111010111111101,
                0b0001110101011101,
                0b1010010000110011,
                0b0110101111000100,
                0b0111001110000111,
                0b0001010001100101,
                0b1100111100111110,
                0b0110001110011100,
                0b0100011001100000,
                0b0001111101010110,
                0b0001011111110001,
                0b1001100100101101,
                0b0001010010010010
            };

            ushort[] expectedResult = new ushort[]
            {
                0b0100001000100000,
                0b0000111011001010,
                0b0000010011101110,
                0b1111000110011101,
                0b0101001001010100,
                0b1011101100111010,
                0b0011111011000110,
                0b0111011011011011,
                0b1010010011001100,
                0b0011100101011010,
                0b0110101100101001,
                0b1011011000111011,
                0b0100011000011110,
                0b0101011111011010,
                0b1001001010100100,
                0b0110000110111011,
            };

            var result = BinaryUtils.RotateMatrix(matrix);
            Assert.Equal(expectedResult, result);
        }
    }
}
