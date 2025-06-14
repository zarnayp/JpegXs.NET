﻿// Copyright (c) 2025
// Released under the BSD 2-Clause License.
// See https://opensource.org/licenses/BSD-2-Clause for details.

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
