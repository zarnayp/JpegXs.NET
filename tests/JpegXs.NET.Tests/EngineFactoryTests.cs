// Copyright (c) 2025
// Released under the BSD 2-Clause License.
// See https://opensource.org/licenses/BSD-2-Clause for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegXs.NET.Tests
{
    public class EngineFactoryTests
    {
        [Test]
        public void EngineFactoryTest_CreateEncoder()
        {
            var svtEngine = EngineFactory.CreateEngine<ISvtEngine>();

            var encoder = svtEngine?.CreateEncoder(100, 100, 5, 5, Format.PLANAR_YUV444_OR_RGB);

            Assert.IsNotNull(encoder);
        }

        [Test]
        public void EngineFactoryTest_CreateEncoderWithBadAttributes()
        {
            var svtEngine = EngineFactory.CreateEngine<ISvtEngine>();

            Assert.Throws<ArgumentException> ( () => svtEngine?.CreateEncoder(100, 100, 5, 0, Format.PLANAR_YUV444_OR_RGB) ) ;

        }
    }
}
