// Copyright (c) 2025
// Released under the BSD 2-Clause License.
// See https://opensource.org/licenses/BSD-2-Clause for details.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JpegXs.NET.Svt;

namespace JpegXs.NET
{
    internal class SvtJpegEncoder : IEncoder
    {

#if WINDOWS
        private const string DllName = "SvtJpegxs.dll";
#else
        private const string DllName = "SvtJpegxs.so";
#endif

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SvtJxsErrorType svt_jpeg_xs_encoder_load_default_parameters(
            ulong version_api_major,
            ulong version_api_minor,
            ref svt_jpeg_xs_encoder_api_t enc_api);


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SvtJxsErrorType svt_jpeg_xs_encoder_init(
            ulong version_api_major,
            ulong version_api_minor,
            ref svt_jpeg_xs_encoder_api_t enc_api);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SvtJxsErrorType svt_jpeg_xs_encoder_send_picture(
            ref svt_jpeg_xs_encoder_api_t enc_api,
            ref svt_jpeg_xs_frame_t enc_input,
            byte blocking_flag);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SvtJxsErrorType svt_jpeg_xs_encoder_get_packet(
            ref svt_jpeg_xs_encoder_api_t enc_api,
            ref svt_jpeg_xs_frame_t enc_output,
            byte blocking_flag);

        private svt_jpeg_xs_encoder_api_t enc;


        internal SvtJpegEncoder()
        {
        }

        internal InitializeResult Initialize(uint width, uint height, uint numberOfThreads, uint bppNumerator, Format format)
        {
            this.enc = new svt_jpeg_xs_encoder_api_t();
            var error = svt_jpeg_xs_encoder_load_default_parameters(SvtConst.MajorVersion, SvtConst.MinorVersion, ref this.enc);

            if (error != SvtJxsErrorType.SvtJxsErrorNone)
            {
                return InitializeResult.Fail($"Failed to load default parameters: {error.ToString()}");
            }

            this.enc.source_width = width;
            this.enc.source_height = height;
            this.enc.input_bit_depth = 8;  // TODO: remove hardcode
            this.enc.colour_format = ColourFormat.COLOUR_FORMAT_PLANAR_YUV444_OR_RGB; // TODO: remove hardcode
            this.enc.bpp_numerator = bppNumerator;
            this.enc.threads_num = numberOfThreads;

            error = svt_jpeg_xs_encoder_init(SvtConst.MajorVersion, SvtConst.MinorVersion, ref this.enc);

            if (error != SvtJxsErrorType.SvtJxsErrorNone)
            {
                return InitializeResult.Fail($"Failed to init encoder: {error.ToString()}");
            }

            return InitializeResult.Ok();
        }


        public unsafe Result Encode(byte[] inputComponent1, byte[] inputComponent2, byte[] inputComponent3, uint width, uint height, byte[] output)
        {
            uint usedSize = 0;

            fixed (byte* ptr1 = inputComponent1, ptr2 = inputComponent2, ptr3 = inputComponent3, ptrOut = output)
            {
                var in_buf = new svt_jpeg_xs_image_buffer_t();

                uint pixel_size = enc.input_bit_depth <= 8 ? 1u : 2u;
                in_buf.stride[0] = width;
                in_buf.stride[1] = width;
                in_buf.stride[2] = width;
                in_buf.alloc_size[0] = in_buf.stride[0] * height * pixel_size;
                in_buf.alloc_size[1] = in_buf.stride[1] * height * pixel_size;
                in_buf.alloc_size[2] = in_buf.stride[2] * height * pixel_size;
                in_buf.data_yuv[0] = (IntPtr)ptr1;
                in_buf.data_yuv[1] = (IntPtr)ptr2;
                in_buf.data_yuv[2] = (IntPtr)ptr3;

                var out_buf = new svt_jpeg_xs_bitstream_buffer_t();
                out_buf.allocation_size = (uint)output.Length;
                out_buf.used_size = 0;
                out_buf.buffer = (IntPtr)ptrOut;

                var enc_input = new svt_jpeg_xs_frame_t();
                enc_input.bitstream = out_buf;
                enc_input.image = in_buf;
                enc_input.user_prv_ctx_ptr = IntPtr.Zero;

                var error = svt_jpeg_xs_encoder_send_picture(ref this.enc, ref enc_input, 1);

                if(error != SvtJxsErrorType.SvtJxsErrorNone)
                {
                    return Result.Fail($"Failed with error: {error.ToString()}"); // TODO: specify what kind of error
                }

                var enc_output = new svt_jpeg_xs_frame_t();
                error = svt_jpeg_xs_encoder_get_packet(ref this.enc, ref enc_output, 1);

                if (error != SvtJxsErrorType.SvtJxsErrorNone)
                {
                    return Result.Fail("Failed"); // TODO: specify what kind of error
                }

                usedSize = enc_output.bitstream.used_size;

            }

            return Result.Ok(usedSize);
        }

        public void Encode(nint input, int width, int height, nint output)
        {
            throw new NotImplementedException();
        }
    }

}
