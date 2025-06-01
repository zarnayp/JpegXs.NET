using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegXs.NET
{
    public interface IDecoder
    {
        void Decode(byte[] input, byte[] outputComponent1, byte[] outputComponent2, byte[] outputComponent3);

    }
}
