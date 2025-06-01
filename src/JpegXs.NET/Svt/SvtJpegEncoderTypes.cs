using System;
using System.Runtime.InteropServices;

namespace JpegXs.NET.Svt
{
    public enum ColourFormat
    {
        COLOUR_FORMAT_INVALID = 0,
        COLOUR_FORMAT_PLANAR_YUV400 = 1,
        COLOUR_FORMAT_PLANAR_YUV420 = 2,        // planar: yuv420p, yuv420p10le, etc.
        COLOUR_FORMAT_PLANAR_YUV422 = 3,        // planar: yuv422p, yuv422p10le, etc.
        COLOUR_FORMAT_PLANAR_YUV444_OR_RGB = 4, // planar: yuv444p, rgbp, gbrp, yuv444p10le, gbrp10le, etc.
        COLOUR_FORMAT_PLANAR_4_COMPONENTS = 5,  // planar 4 components
        COLOUR_FORMAT_GRAY = 6,
        COLOUR_FORMAT_PLANAR_MAX,

        COLOUR_FORMAT_PACKED_MIN = 20,
        COLOUR_FORMAT_PACKED_YUV444_OR_RGB, // packed rgb/bgr, 8:8:8, 24bpp, RGBRGB... / BGRBGR...
        COLOUR_FORMAT_PACKED_MAX
    }

    [Flags]
    public enum CPU_FLAGS : ulong
    {
        CPU_FLAGS_C = 0,
        CPU_FLAGS_MMX = 1 << 0,
        CPU_FLAGS_SSE = 1 << 1,
        CPU_FLAGS_SSE2 = 1 << 2,
        CPU_FLAGS_SSE3 = 1 << 3,
        CPU_FLAGS_SSSE3 = 1 << 4,
        CPU_FLAGS_SSE4_1 = 1 << 5,
        CPU_FLAGS_SSE4_2 = 1 << 6,
        CPU_FLAGS_AVX = 1 << 7,
        CPU_FLAGS_AVX2 = 1 << 8,
        CPU_FLAGS_AVX512F = 1 << 9,
        CPU_FLAGS_AVX512CD = 1 << 10,
        CPU_FLAGS_AVX512DQ = 1 << 11,
        CPU_FLAGS_AVX512ER = 1 << 12,
        CPU_FLAGS_AVX512PF = 1 << 13,
        CPU_FLAGS_AVX512BW = 1 << 14,
        CPU_FLAGS_AVX512VL = 1 << 15,

        CPU_FLAGS_ALL = (CPU_FLAGS_AVX512VL << 1) - 1,
        CPU_FLAGS_INVALID = 1UL << (sizeof(ulong) * 8 - 1)
    }

    public enum VerboseMessages : uint
    {
        VERBOSE_NONE = 0,
        VERBOSE_ERRORS = 1,
        VERBOSE_SYSTEM_INFO = 2,
        VERBOSE_SYSTEM_INFO_ALL = 3,
        VERBOSE_WARNINGS = 4,
        VERBOSE_INFO_MULTITHREADING = 5,
        VERBOSE_INFO_FULL = 6
    };

    public enum SvtJxsErrorType : uint
    {
        SvtJxsErrorNone = 0,
        SvtJxsErrorInvalidApiVersion = 0x40001001,
        SvtJxsErrorCorruptFrame = 0x4000100C,
        SvtJxsErrorInsufficientResources = 0x80001000,
        SvtJxsErrorUndefined = 0x80001001,
        SvtJxsErrorInvalidComponent = 0x80001004,
        SvtJxsErrorBadParameter = 0x80001005,
        SvtJxsErrorDestroyThreadFailed = 0x80002012,
        SvtJxsErrorSemaphoreUnresponsive = 0x80002021,
        SvtJxsErrorDestroySemaphoreFailed = 0x80002022,
        SvtJxsErrorCreateMutexFailed = 0x80002030,
        SvtJxsErrorMutexUnresponsive = 0x80002031,
        SvtJxsErrorDestroyMutexFailed = 0x80002032,
        SvtJxsErrorNoErrorEmptyQueue = 0x80002033,
        SvtJxsErrorNoErrorFifoShutdown = 0x80002034,

        // Encoder errors
        SvtJxsErrorEncodeFrameError = 0x80002035,

        // Decoder errors
        SvtJxsErrorDecoderInvalidPointer = 0x80003000,
        SvtJxsErrorDecoderInvalidBitstream = 0x80003001,
        SvtJxsErrorDecoderInternal = 0x80003002,
        SvtJxsErrorDecoderBitstreamTooShort = 0x80003003,
        SvtJxsErrorDecoderConfigChange = 0x80003004,
        SvtJxsDecoderEndOfCodestream = 0x80003005,

        SvtJxsErrorMax = 0x7FFFFFFF
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct svt_jpeg_xs_encoder_api_t
    {
        // Mandatory fields
        public uint source_width;
        public uint source_height;
        public byte input_bit_depth;
        public ColourFormat colour_format;
        public uint bpp_numerator;
        public uint bpp_denominator;
        public uint ndecomp_v;
        public uint ndecomp_h;
        public uint quantization;
        public uint slice_height;
        public CPU_FLAGS use_cpu_flags;
        public uint threads_num;
        public byte cpu_profile;
        public uint print_bands_info;

        // Additional settings
        public uint coding_signs_handling;
        public uint coding_significance;
        public uint rate_control_mode;
        public uint coding_vertical_prediction_mode;
        public uint verbose;

        // Callbacks
        public IntPtr callback_send_data_available;
        public IntPtr callback_send_data_available_context;
        public IntPtr callback_get_data_available;
        public IntPtr callback_get_data_available_context;

        // Other settings
        public byte slice_packetization_mode;
        public IntPtr private_ptr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct svt_jpeg_xs_decoder_api
    {
        public CPU_FLAGS use_cpu_flags;
        public VerboseMessages verbose;
        public uint threads_num;
        public byte packetization_mode;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void callback_send_data_available(IntPtr decoder, IntPtr context);

        public callback_send_data_available callback_send_data_available_func;
        public IntPtr callback_send_data_available_context;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void callback_get_data_available(IntPtr decoder, IntPtr context);

        public callback_get_data_available callback_get_data_available_func;
        public IntPtr callback_get_data_available_context;

        public IntPtr private_ptr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct svt_jpeg_xs_bitstream_buffer_t
    {
        /// <summary>
        /// Bitstream buffer; this pointer can be changed during encoding process.
        /// Use release_ctx_ptr for deallocation.
        /// </summary>
        public IntPtr buffer;

        /// <summary>
        /// Bitstream buffer allocation size.
        /// </summary>
        public uint allocation_size;

        /// <summary>
        /// Bitstream buffer used size.
        /// </summary>
        public uint used_size;

        /// <summary>
        /// Pointer for memory management, used by pool allocator.
        /// </summary>
        public IntPtr release_ctx_ptr;

        /// <summary>
        /// Indicates if the buffer can be released by the user (1 = Yes).
        /// </summary>
        public byte ready_to_release;

        /// <summary>
        /// Indicates if this is the last packetization unit within the frame (1 = Yes).
        /// </summary>
        public byte last_packet_in_frame;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct svt_jpeg_xs_image_buffer_t
    {
        public svt_jpeg_xs_image_buffer_t()
        {
        }

        const int MAX_COMPONENTS_NUM = 4;

        /// <summary>
        /// Allocated Data buffer for components (pointers).
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_COMPONENTS_NUM)]
        public IntPtr[] data_yuv = new IntPtr[MAX_COMPONENTS_NUM];

        /// <summary>
        /// Greater than or equal to width. Not supported in decoder yet.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_COMPONENTS_NUM)]
        public uint[] stride = new uint[MAX_COMPONENTS_NUM];

        /// <summary>
        /// Used by Encoder and Decoder to validate that the buffer size is enough.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_COMPONENTS_NUM)]
        public uint[] alloc_size = new uint[MAX_COMPONENTS_NUM];

        /// <summary>
        /// Used by pool allocator, for different allocation free to use.
        /// </summary>
        public IntPtr release_ctx_ptr;

        /// <summary>
        /// Set by encoder/decoder. 1 indicates that buffer can be released by user.
        /// </summary>
        public byte ready_to_release;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct svt_jpeg_xs_frame_t
    {
        /// <summary>
        /// Common structure for encoder and decoder.
        /// Encoder: image is input, bitstream is output.
        /// Decoder: bitstream is input, image is output.
        /// </summary>
        public svt_jpeg_xs_image_buffer_t image; // YUV buffer

        /// <summary>
        /// Bitstream buffer.
        /// </summary>
        public svt_jpeg_xs_bitstream_buffer_t bitstream;

        /// <summary>
        /// Input user private context pointer, received in output.
        /// </summary>
        public IntPtr user_prv_ctx_ptr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct svt_jpeg_xs_image_config
    {
        const int MAX_COMPONENTS_NUM = 4;

        public uint width;
        public uint height;
        public byte bit_depth;
        public ColourFormat format;
        public byte components_num;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_COMPONENTS_NUM)]
        public Component[] components;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Component
    {
        public uint width;
        public uint height;
        public uint byte_size;
    }

    internal static class SvtConst
    {
        public const ulong MajorVersion = 0;
        public const ulong MinorVersion = 9;
    }
}
