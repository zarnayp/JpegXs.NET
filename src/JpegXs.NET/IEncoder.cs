using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegXs.NET
{
    public interface IEncoder
    {
        Result Encode(byte[] inputComponent1, byte[] inputComponent2, byte[] inputComponent3, uint width, uint height, byte[] output);

        void Encode(IntPtr input, int width, int height, IntPtr output);
    }
}
