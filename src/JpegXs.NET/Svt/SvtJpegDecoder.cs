// Copyright (c) 2025
// Released under the BSD 2-Clause License.
// See https://opensource.org/licenses/BSD-2-Clause for details.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JpegXs.NET.Svt
{
    internal class SvtJpegDecoder : IDecoder
    {

#if WINDOWS
        private const string DllName = "SvtJpegxs.dll";
#else
        private const string DllName = "libSvtJpegxs.so.0.10.0";
#endif

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SvtJxsErrorType svt_jpeg_xs_decoder_get_single_frame_size(
            IntPtr bitstream_buf,
            UIntPtr bitstream_buf_size,
            ref svt_jpeg_xs_image_config out_image_config,
            out uint frame_size,
            uint fast_search);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SvtJxsErrorType svt_jpeg_xs_decoder_init(
            ulong version_api_major,
            ulong version_api_minor,
            ref svt_jpeg_xs_decoder_api dec_api,
            IntPtr bitstream_buf,
            UIntPtr codestream_size,
            ref svt_jpeg_xs_image_config out_image_config);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SvtJxsErrorType svt_jpeg_xs_decoder_send_frame(
           ref svt_jpeg_xs_decoder_api dec_api,
           ref svt_jpeg_xs_frame_t dec_input,
           byte blocking_flag);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SvtJxsErrorType svt_jpeg_xs_decoder_get_frame(
            ref svt_jpeg_xs_decoder_api dec_api,
            ref svt_jpeg_xs_frame_t dec_output,
            byte blocking_flag);

        private svt_jpeg_xs_decoder_api decoder;
        private svt_jpeg_xs_image_config initializedImageConfig;
        private bool initialized = false;

        public SvtJpegDecoder()
        {
            this.decoder = new svt_jpeg_xs_decoder_api
            {
                verbose = VerboseMessages.VERBOSE_SYSTEM_INFO,
                threads_num = 10,
                use_cpu_flags = CPU_FLAGS.CPU_FLAGS_ALL // TODO: remove hardcode
            };
        }

        private unsafe SvtJxsErrorType ReadHeader(byte* ptrIn, out uint frameSize, out svt_jpeg_xs_image_config imageConfig)
        {
            const int tmpBufferSize = 1000;
            var ptrTmp = stackalloc byte[tmpBufferSize]; // TODO: do not allocate always

            Buffer.MemoryCopy(ptrIn, ptrTmp, tmpBufferSize, tmpBufferSize);

            frameSize = 0;
            imageConfig = new svt_jpeg_xs_image_config();

            var initError = svt_jpeg_xs_decoder_get_single_frame_size(
                (IntPtr)ptrTmp, tmpBufferSize, ref imageConfig, out frameSize, 1);

            if (initError != SvtJxsErrorType.SvtJxsErrorNone)
            {
                return initError;
            }

            return SvtJxsErrorType.SvtJxsErrorNone;
        }

        private unsafe SvtJxsErrorType Initialize(byte* ptrIn, uint frameSizeIn, out svt_jpeg_xs_image_config imageConfig)
        {
            uint frameSize = 0;

            var error = ReadHeader(ptrIn, out frameSize, out imageConfig);

            if (frameSize == 0 || frameSizeIn != frameSize)
            {
                this.initializedImageConfig = imageConfig;
                return SvtJxsErrorType.SvtJxsErrorCorruptFrame;
            }

            if(imageConfig.height == this.initializedImageConfig.height
                && imageConfig.height == this.initializedImageConfig.height
                && imageConfig.width == this.initializedImageConfig.width
                && imageConfig.bit_depth == this.initializedImageConfig.bit_depth
                && imageConfig.format == this.initializedImageConfig.format
                && imageConfig.components_num == this.initializedImageConfig.components_num
                ) // TODO: is it all what needs to be compared
            {
                return SvtJxsErrorType.SvtJxsErrorNone;
            }

            this.initializedImageConfig = imageConfig;

            var bitstream = new svt_jpeg_xs_bitstream_buffer_t
            {
                buffer = (IntPtr)ptrIn,
                allocation_size = frameSize,
                used_size = frameSize,
            };

            error = svt_jpeg_xs_decoder_init(
                SvtConst.MajorVersion, SvtConst.MinorVersion, ref this.decoder, bitstream.buffer, frameSize, ref imageConfig);

            if (error != SvtJxsErrorType.SvtJxsErrorNone)
            {
                return error;
            }

            if (imageConfig.components_num != 3)  // TODO: limited to 3 componets
            {
                return error;
            }

            return SvtJxsErrorType.SvtJxsErrorNone;
        }

        public void Decode(byte[] input, byte[] outputComponent1, byte[] outputComponent2, byte[] outputComponent3)
        {
            DecodeInternal(true, input, outputComponent1, outputComponent2, outputComponent3);
        }

        // skip header read for better performance
        public void DecodeSame(byte[] input, byte[] outputComponent1, byte[] outputComponent2, byte[] outputComponent3)
        {
            DecodeInternal(false, input, outputComponent1, outputComponent2, outputComponent3);
        }

        private unsafe void DecodeInternal(bool reinit,byte[] input, byte[] outputComponent1, byte[] outputComponent2, byte[] outputComponent3)
        {
            fixed (byte* ptrIn = input, ptr1 = outputComponent1, ptr2 = outputComponent2, ptr3 = outputComponent3)
            {
                svt_jpeg_xs_image_config imageConfig;

                if (!this.initialized || reinit)
                {
                    Initialize(ptrIn, (uint)input.Length, out imageConfig);
                    initialized = true;
                }
                else
                {
                    imageConfig = initializedImageConfig;
                }

                var bitstream = new svt_jpeg_xs_bitstream_buffer_t
                {
                    buffer = (IntPtr)ptrIn,
                    allocation_size = (uint)input.Length,
                    used_size = (uint)input.Length,
                };

                uint pixel_size = imageConfig.bit_depth <= 8 ? 1u : 2u;

                svt_jpeg_xs_image_buffer_t imageBuffer = new svt_jpeg_xs_image_buffer_t();
                imageBuffer.stride[0] = imageConfig.components[0].width;
                imageBuffer.stride[1] = imageConfig.components[1].width;
                imageBuffer.stride[2] = imageConfig.components[2].width;
                imageBuffer.alloc_size[0] = imageBuffer.stride[0] * imageConfig.components[0].height * pixel_size;
                imageBuffer.alloc_size[1] = imageBuffer.stride[1] * imageConfig.components[1].height * pixel_size;
                imageBuffer.alloc_size[2] = imageBuffer.stride[2] * imageConfig.components[2].height * pixel_size;

                if (imageBuffer.alloc_size[0] > outputComponent1.Length ||
                    imageBuffer.alloc_size[1] > outputComponent2.Length ||
                    imageBuffer.alloc_size[2] > outputComponent3.Length)
                {
                    return; // TODO: return error with details
                }


                imageBuffer.data_yuv[0] = (IntPtr)ptr1;
                imageBuffer.data_yuv[1] = (IntPtr)ptr2;
                imageBuffer.data_yuv[2] = (IntPtr)ptr3;

                svt_jpeg_xs_frame_t decInput = new svt_jpeg_xs_frame_t
                {
                    bitstream = bitstream,
                    image = imageBuffer
                };

                var error = svt_jpeg_xs_decoder_send_frame(ref this.decoder, ref decInput, 1 /*blocking*/);

                if (error != SvtJxsErrorType.SvtJxsErrorNone)
                {
                    return; // TODO: return error with details
                }

                svt_jpeg_xs_frame_t decOutput = new svt_jpeg_xs_frame_t();
                error = svt_jpeg_xs_decoder_get_frame(ref this.decoder, ref decOutput, 1 /*blocking*/);

                if (error != SvtJxsErrorType.SvtJxsErrorNone)
                {
                    return; // TODO: return error with details
                }

            }
        }
    }
}
