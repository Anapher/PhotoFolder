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
    }
}
