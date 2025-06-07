// Copyright (c) 2025
// Released under the BSD 2-Clause License.
// See https://opensource.org/licenses/BSD-2-Clause for details.

using System.Diagnostics;
using JpegXs.NET.Svt;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JpegXs.NET.Tests
{
    public class SvtJpgEncoderDecoderTests
    {
        private static void GetComponentsFromFile(string filePath, int width, int height, out byte[] red, out byte[] green, out byte[] blue)
        {
            red = new byte[height * width];
            green = new byte[height * width];
            blue = new byte[height * width];

            byte[] rgb = File.ReadAllBytes(filePath);

            int it = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    red[y * width + x] = rgb[it++];
                    green[y * width + x] = rgb[it++];
                    blue[y * width + x] = rgb[it++];
                }
            }
        }

        private static void SaveToFileFromComponents(string filePath, int width, int height, byte[] red, byte[] green, byte[] blue)
        {
            byte[] pixelData = new byte[width * height * 3];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    int pixelIndex = y * width * 3 + x * 3;

                    pixelData[pixelIndex] = red[index];
                    pixelData[pixelIndex + 1] = green[index];
                    pixelData[pixelIndex + 2] = blue[index];
                }
            }

            File.WriteAllBytes(filePath, pixelData);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SvtJpgEncoderDecoderTest_EncodeDecode()
        {
            // Encode
            const int width = 1024;
            const int height = 768;

            byte[] red1, red2;
            byte[] green1, green2;
            byte[] blue1, blue2;

            GetComponentsFromFile("sampleRGB1.bin", width, height,out red1,out green1,out blue1);
            GetComponentsFromFile("sampleRGB2.bin", width, height, out red2, out green2, out blue2);

            var encoder = new SvtJpegEncoder();
            encoder.Initialize((uint)width, (uint)height, 10, 6, Format.PLANAR_YUV444_OR_RGB);

            byte[] output1 = new byte[width * height * 3];
            byte[] output2 = new byte[width * height * 3];

            var sw = Stopwatch.StartNew();
            var result = encoder.Encode(red1, green1, blue1, width, height, output1);
            sw.Stop();
            Console.WriteLine($"Encoding in {sw.ElapsedMilliseconds} ms");

            sw = Stopwatch.StartNew();
            result = encoder.Encode(red2, green2, blue2, width, height, output2);
            sw.Stop();
            Console.WriteLine($"Encoding in {sw.ElapsedMilliseconds} ms");

            Assert.IsTrue(result.Success);

            // Decode
            byte[] input1 = new byte[result.UsedSize];
            Array.Copy(output1, 0, input1, 0, result.UsedSize);
            byte[] input2 = new byte[result.UsedSize];
            Array.Copy(output2, 0, input2, 0, result.UsedSize);

            byte[] redOut = new byte[height * width];
            byte[] greenOut = new byte[height * width];
            byte[] blueOut = new byte[height * width];

            var decoder = new SvtJpegDecoder();

            sw = Stopwatch.StartNew();
            decoder.Decode(input1, redOut, greenOut, blueOut);
            sw.Stop();
            Console.WriteLine($"First decoding in {sw.ElapsedMilliseconds} ms");

            sw = Stopwatch.StartNew();
            decoder.Decode(input2, redOut, greenOut, blueOut);
            sw.Stop();
            Console.WriteLine($"Second decoding in {sw.ElapsedMilliseconds} ms");

            Assert.IsTrue(result.Success);

            SaveToFileFromComponents("outRGB.bin", width, height, redOut, greenOut, blueOut);
        }
    }
}