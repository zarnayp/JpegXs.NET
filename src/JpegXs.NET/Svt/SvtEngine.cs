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
    internal class SvtEngine : ISvtEngine
    {
        public IEncoder CreateEncoder(uint width, uint height, uint numberOfThreads, uint bppNumerator, Format format)
        {
            var encoder = new SvtJpegEncoder();
            var result = encoder.Initialize(width, height, numberOfThreads, bppNumerator, format);

            if(result.Success != true)
            {
                throw new ArgumentException($"Unable to create encoder for given arguments. Inner error: {result.ErrorMessage}"); // TODO: not only due to wrong arguments it may be not success. Throw particular .net exception for such cases
            }

            return encoder;
        }
    }
}
