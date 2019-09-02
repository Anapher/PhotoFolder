using System;

namespace PhotoFolder.Infrastructure.Utilities
{
    public static class BinaryUtils
    {
        /// <summary>
        ///     Return amount number of bits set to 1 of the value
        /// </summary>
        /// <param name="n">The value to scan</param>
        public static int SumSetBits(int n)
        {
            // Java: use >>> instead of >>
            // C or C++: use uint32_t
            n = n - ((n >> 1) & 0x55555555);
            n = (n & 0x33333333) + ((n >> 2) & 0x33333333);
            return (((n + (n >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }
        //public static int SumSetBits(int n)
        //{
        //    var count = 0;

        //    while (n > 0)
        //    {
        //        if ((n & 1) == 1)
        //            count++;

        //        n = (byte) (n >> 1);
        //    }

        //    return count;
        //}

        /// <summary>
        ///     Get the amount of bits with the same position but set to a different value in the two bytes
        /// </summary>
        public static int GetDifferentBits(byte b1, byte b2)
        {
            var diff = b1 ^ b2;
            return SumSetBits(diff);
        }

        /// <summary>
        ///     Compute the equality of two byte arrays by inspecting the bits (return equal bits / total bits)
        /// </summary>
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

        /// <summary>
        ///     Rotate the given 16x16 matrix by 90° degree to the right (bit wise)
        /// </summary>
        public static ushort[] RotateMatrix(ushort[] matrix)
        {
            if (matrix.Length != 16)
                throw new ArgumentException("This method only accepts 16x16 matrices");

            // Transponding
            ushort[] result = new ushort[16];

            for (int i = 0; i < matrix.Length; i++)
            {
                var current = matrix[i];

                for (int j = 0; j < result.Length; j++)
                {
                    result[j] |= (ushort)(((current >> (result.Length - j - 1)) & 1) << (matrix.Length - i - 1));
                }
            }

            // Reverse
            for (int i = 0; i < result.Length; i++)
            {
                ushort x = result[i];
                ushort y = 0;
                for (int j = 0; j < sizeof(ushort) * 8; j++)
                {
                    y <<= 1;    // Shift accumulated result left
                    y |= (ushort)(x & 1); // Set the least significant bit if it is set in the input value
                    x >>= 1;    // Shift input value right
                }

                result[i] = y;
            }

            return result;
        }
    }
}
