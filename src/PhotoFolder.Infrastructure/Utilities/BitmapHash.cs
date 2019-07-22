using System.Drawing;
using System.Linq;

namespace PhotoFolder.Infrastructure.Utilities
{
    public static class BitmapHash
    {
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
