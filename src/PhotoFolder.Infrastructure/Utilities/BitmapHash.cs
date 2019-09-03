using System;
using System.Diagnostics;
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
            var hashResult = new byte[35];

            using (var hashBmp = ScaleImage(source, 16))
            {
                var (pixelBrightness, averageColor) = GetImageData(hashBmp);
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

                hashResult[32] = averageColor.R;
                hashResult[33] = averageColor.G;
                hashResult[34] = averageColor.B;
            }

            return hashResult;
        }

        /// <summary>
        ///     Rotate the bitmap hash by 90° to the right
        /// </summary>
        public static byte[] RotateBitmapHash(ReadOnlySpan<byte> bitmapHash)
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

        private static (float[] pixelBrightnesses, Color averageColor) GetImageData(Bitmap source)
        {
            var result = new float[source.Width * source.Height];

            long r = 0, g = 0, b = 0;

            for (int i = 0; i < source.Height; i++)
            {
                for (int j = 0; j < source.Width; j++)
                {
                    var position = i * source.Width + j;
                    var pixel = source.GetPixel(i, j);

                    r += pixel.R;
                    g += pixel.G;
                    b += pixel.B;

                    result[position] = pixel.GetBrightness();
                }
            }

            var totalPixels = source.Width * source.Height;
            var averageColor = Color.FromArgb((int) (r / totalPixels), (int) (g / totalPixels), (int) (b / totalPixels));

            return (result, averageColor);
        }
    }
}
