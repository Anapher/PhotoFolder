using System;
using System.Drawing;
using System.Linq;

namespace PhotoFolder.Infrastructure.Utilities
{
    public static class BitmapHash
    {
        /// <summary>
        ///     Compute the bitmap hash
        /// </summary>
        public static byte[] Compute(Bitmap source)
        {
            var hashResult = new byte[32];

            using (var hashBmp = ScaleImage(source, 16))
            {
                var pixelBrightness = GetImageBrightness(hashBmp);
                var averageBrightness = pixelBrightness.Sum(x => x) / pixelBrightness.Length;

                for (int i = 0; i < pixelBrightness.Length; i++)
                {
                    var currentByteIndex = i / 8;
                    var currentBitIndex = i % 8;

                    // reduce colors to true / false
                    var isPixelSet = pixelBrightness[i] < averageBrightness;

                    if (isPixelSet)
                        hashResult[currentByteIndex] = (byte) (hashResult[currentByteIndex] | (1 << currentBitIndex));
                }
            }

            return hashResult;
        }

        /// <summary>
        ///     Rotate the bitmap hash by 90° to the right
        /// </summary>
        public static byte[] RotateBitmapHash(byte[] bitmapHash)
        {
            if (bitmapHash.Length != 32)
                throw new ArgumentException("The bitmap hash must be 256 bit sized.");

            var matrix = new ushort[16];
            for (int i = 0; i < 16; i++)
                matrix[i] = (ushort) (bitmapHash[i * 2] << 8 | bitmapHash[(i * 2) + 1]);

            var rotatedMatrix = BinaryUtils.RotateMatrix(matrix);
            var result = new byte[32];
            for (int i = 0; i < 16; i++)
            {
                var n = rotatedMatrix[i];
                result[i * 2] = (byte)(n >> 8);
                result[(i * 2) + 1] = (byte)(n & byte.MaxValue);
            }

            return result;
        }

        private static Bitmap ScaleImage(Bitmap source, int length)
        {
            return new Bitmap(source, length, length);
        }

        private static float[] GetImageBrightness(Bitmap source)
        {
            var result = new float[source.Width * source.Height];

            for (int i = 0; i < source.Height; i++)
            {
                for (int j = 0; j < source.Width; j++)
                {
                    var positon = i * source.Width + j;
                    var brightness = source.GetPixel(i, j).GetBrightness();

                    result[positon] = brightness;
                }
            }

            return result;
        }
    }
}
