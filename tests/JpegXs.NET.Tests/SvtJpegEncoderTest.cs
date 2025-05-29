using System.Drawing;

namespace JpegXs.NET.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SvtJpegEncoderTest_Encode()
        {
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

            var result = encoder.Encode(red, green, blue, 1024, 768, output);

            Assert.Pass();
        }
    }
}