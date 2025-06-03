// Copyright (c) 2025
// Released under the BSD 2-Clause License.
// See https://opensource.org/licenses/BSD-2-Clause for details.

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
