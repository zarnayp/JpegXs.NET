using System.Drawing;
using System.Drawing.Imaging;
using JpegXs.NET.Svt;

namespace JpegXs.NET.Tests
{
    public class SvtJpgEncoderDecoderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SvtJpgEncoderDecoderTest_EncodeDecode()
        {
            // Encode
            string filePath = "sample.bmp";

            using var bmp = new Bitmap(filePath);

            int width = bmp.Width;
            int height = bmp.Height;

            byte[] red = new byte[height * width];
            byte[] green = new byte[height * width];
            byte[] blue = new byte[height * width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bmp.GetPixel(x, y);

                    red[y * width + x] = pixel.R;
                    green[y * width + x] = pixel.G;
                    blue[y * width + x] = pixel.B;
                }
            }

            var encoder = new SvtJpegEncoder();

            byte[] output = new byte[width * height * 3];

            var result = encoder.Encode(red, green, blue, 1024, 764, output);

            Assert.IsTrue(result.Success);

            // Decode
            byte[] input = new byte[result.UsedSize];
            Array.Copy(output, 0, input, 0, result.UsedSize);

            byte[] redOut = new byte[height * width];
            byte[] greenOut = new byte[height * width];
            byte[] blueOut = new byte[height * width];

            var decoder = new SvtJpegDecoder();

            decoder.Decode(input, redOut, greenOut, blueOut);

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int stride = bitmapData.Stride;
            IntPtr ptr = bitmapData.Scan0;
            byte[] pixelData = new byte[stride * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    int pixelIndex = y * stride + x * 3;

                    pixelData[pixelIndex] = blueOut[index];
                    pixelData[pixelIndex + 1] = greenOut[index];
                    pixelData[pixelIndex + 2] =  redOut[index];
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(pixelData, 0, ptr, pixelData.Length);
            bitmap.UnlockBits(bitmapData);

            bitmap.Save("sampleTestOut.bmp", ImageFormat.Bmp);

            Assert.Pass();
        }
    }
}