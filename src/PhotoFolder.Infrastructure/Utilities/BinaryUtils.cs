using System;

namespace PhotoFolder.Infrastructure.Utilities
{
    public static class BinaryUtils
    {
        public static int SumSetBits(int n)
        {
            var count = 0;

            while (n > 0)
            {
                if ((n & 1) == 1)
                    count++;

                n = (byte) (n >> 1);
            }

            return count;
        }

        public static int GetDifferentBits(byte b1, byte b2)
        {
            var diff = b1 ^ b2;
            return SumSetBits(diff);
        }

        public static float ComputeByteArrayEquality(byte[] data1, byte[] data2)
        {
            if (data1.Length != data2.Length)
                throw new ArgumentException("The hashes must have the same length.");

            var length = data1.Length;
            var differences = 0;

            for (int i = 0; i < length; i++)
            {
                var byte1 = data1[i];
                var byte2 = data2[i];

                differences += GetDifferentBits(byte1, byte2);
            }

            var totalBits = length * 8;
            var sameBits = totalBits - differences;
            return (float) sameBits / totalBits;
        }
    }
}
