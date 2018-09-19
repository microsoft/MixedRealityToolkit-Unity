using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using POpusCodec.Enums;

namespace POpusCodec
{
    internal class Wrapper
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_encoder_get_size(Channels channels);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern OpusStatusCode opus_encoder_init(IntPtr st, SamplingRate Fs, Channels channels, OpusApplicationType application);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr opus_get_version_string();

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_encode(IntPtr st, short[] pcm, int frame_size, byte[] data, int max_data_bytes);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_encode_float(IntPtr st, float[] pcm, int frame_size, byte[] data, int max_data_bytes);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_encoder_ctl_set(IntPtr st, OpusCtlSetRequest request, int value);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_encoder_ctl_get(IntPtr st, OpusCtlGetRequest request, ref int value);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_decoder_ctl_set(IntPtr st, OpusCtlSetRequest request, int value);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_decoder_ctl_get(IntPtr st, OpusCtlGetRequest request, ref int value);

		[DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_decoder_get_size(Channels channels);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern OpusStatusCode opus_decoder_init(IntPtr st, SamplingRate Fs, Channels channels);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_decode(IntPtr st, byte[] data, int len, short[] pcm, int frame_size, int decode_fec);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_decode_float(IntPtr st, byte[] data, int len, float[] pcm, int frame_size, int decode_fec);

//        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
//        private static extern int opus_decode(IntPtr st, IntPtr data, int len, short[] pcm, int frame_size, int decode_fec);

//        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
//        private static extern int opus_decode_float(IntPtr st, IntPtr data, int len, float[] pcm, int frame_size, int decode_fec);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int opus_packet_get_bandwidth(byte[] data);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int opus_packet_get_nb_channels(byte[] data);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr opus_strerror(OpusStatusCode error);

#else
        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_encoder_get_size(Channels channels);

        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern OpusStatusCode opus_encoder_init(IntPtr st, SamplingRate Fs, Channels channels, OpusApplicationType application);

        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr opus_get_version_string();

        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_encode(IntPtr st, short[] pcm, int frame_size, byte[] data, int max_data_bytes);

        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_encode_float(IntPtr st, float[] pcm, int frame_size, byte[] data, int max_data_bytes);

        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_encoder_ctl_set(IntPtr st, OpusCtlSetRequest request, int value);

        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_encoder_ctl_get(IntPtr st, OpusCtlGetRequest request, ref int value);

        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_decoder_ctl_set(IntPtr st, OpusCtlSetRequest request, int value);

        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_decoder_ctl_get(IntPtr st, OpusCtlGetRequest request, ref int value);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_decoder_get_size(Channels channels);

        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern OpusStatusCode opus_decoder_init(IntPtr st, SamplingRate Fs, Channels channels);

        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_decode(IntPtr st, byte[] data, int len, short[] pcm, int frame_size, int decode_fec);

        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int opus_decode_float(IntPtr st, byte[] data, int len, float[] pcm, int frame_size, int decode_fec);

//        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
//        private static extern int opus_decode(IntPtr st, IntPtr data, int len, short[] pcm, int frame_size, int decode_fec);

//        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
//        private static extern int opus_decode_float(IntPtr st, IntPtr data, int len, float[] pcm, int frame_size, int decode_fec);

        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int opus_packet_get_bandwidth(byte[] data);

        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int opus_packet_get_nb_channels(byte[] data);

        [DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr opus_strerror(OpusStatusCode error);
#endif
        public static IntPtr opus_encoder_create(SamplingRate Fs, Channels channels, OpusApplicationType application)
        {
            int size = Wrapper.opus_encoder_get_size(channels);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            OpusStatusCode statusCode = Wrapper.opus_encoder_init(ptr, Fs, channels, application);

            try
            {
                HandleStatusCode(statusCode);
            }
            catch (Exception ex)
            {
                if (ptr != IntPtr.Zero)
                {
                    Wrapper.opus_encoder_destroy(ptr);
                    ptr = IntPtr.Zero;
                }

                throw ex;
            }

            return ptr;
        }


        public static int opus_encode(IntPtr st, short[] pcm, int frame_size, byte[] data)
        {
            if (st == IntPtr.Zero)
                throw new ObjectDisposedException("OpusEncoder");

            int payloadLength = opus_encode(st, pcm, frame_size, data, data.Length);

            if (payloadLength <= 0)
            {
                HandleStatusCode((OpusStatusCode)payloadLength);
            }

            return payloadLength;
        }

        public static int opus_encode(IntPtr st, float[] pcm, int frame_size, byte[] data)
        {
            if (st == IntPtr.Zero)
                throw new ObjectDisposedException("OpusEncoder");

            int payloadLength = opus_encode_float(st, pcm, frame_size, data, data.Length);

            if (payloadLength <= 0)
            {
                HandleStatusCode((OpusStatusCode)payloadLength);
            }

            return payloadLength;
        }

        public static void opus_encoder_destroy(IntPtr st)
        {
            Marshal.FreeHGlobal(st);
        }

        public static int get_opus_encoder_ctl(IntPtr st, OpusCtlGetRequest request)
        {
            if (st == IntPtr.Zero)
                throw new ObjectDisposedException("OpusEncoder");

            int value = 0;
            OpusStatusCode statusCode = (OpusStatusCode)opus_encoder_ctl_get(st, request, ref value);

            HandleStatusCode(statusCode);

            return value;
        }

        public static void set_opus_encoder_ctl(IntPtr st, OpusCtlSetRequest request, int value)
        {
            if (st == IntPtr.Zero)
                throw new ObjectDisposedException("OpusEncoder");

            OpusStatusCode statusCode = (OpusStatusCode)opus_encoder_ctl_set(st, request, value);

            HandleStatusCode(statusCode);
        }

        public static int get_opus_decoder_ctl(IntPtr st, OpusCtlGetRequest request)
        {
            if (st == IntPtr.Zero)
                throw new ObjectDisposedException("OpusDcoder");

            int value = 0;
            OpusStatusCode statusCode = (OpusStatusCode)opus_decoder_ctl_get(st, request, ref value);

            HandleStatusCode(statusCode);

            return value;
        }

        public static void set_opus_decoder_ctl(IntPtr st, OpusCtlSetRequest request, int value)
        {
            if (st == IntPtr.Zero)
                throw new ObjectDisposedException("OpusDecoder");

            OpusStatusCode statusCode = (OpusStatusCode)opus_decoder_ctl_set(st, request, value);

            HandleStatusCode(statusCode);
        }
        public static IntPtr opus_decoder_create(SamplingRate Fs, Channels channels)
        {
            int size = Wrapper.opus_decoder_get_size(channels);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            OpusStatusCode statusCode = Wrapper.opus_decoder_init(ptr, Fs, channels);

            try
            {
                HandleStatusCode(statusCode);
            }
            catch (Exception ex)
            {
                if (ptr != IntPtr.Zero)
                {
                    Wrapper.opus_decoder_destroy(ptr);
                    ptr = IntPtr.Zero;
                }

                throw ex;
            }

            return ptr;
        }

        public static void opus_decoder_destroy(IntPtr st)
        {
            Marshal.FreeHGlobal(st);
        }

        public static int opus_decode(IntPtr st, byte[] data, short[] pcm, int decode_fec, int channels)
        {
            if (st == IntPtr.Zero)
                throw new ObjectDisposedException("OpusDecoder");

            int numSamplesDecoded = 0;

            if (data != null)
            {
                numSamplesDecoded = opus_decode(st, data, data.Length, pcm, pcm.Length / channels, decode_fec);
            }
            else
            {
                numSamplesDecoded = opus_decode(st, null, 0, pcm, pcm.Length / channels, decode_fec);
            }

            if (numSamplesDecoded == (int)OpusStatusCode.InvalidPacket)
                return 0;

            if (numSamplesDecoded <= 0)
            {
                HandleStatusCode((OpusStatusCode)numSamplesDecoded);
            }

            return numSamplesDecoded;
        }

        public static int opus_decode(IntPtr st, byte[] data, float[] pcm, int decode_fec, int channels)
        {
            if (st == IntPtr.Zero)
                throw new ObjectDisposedException("OpusDecoder");

            int numSamplesDecoded = 0;

            if (data != null)
            {
                numSamplesDecoded = opus_decode_float(st, data, data.Length, pcm, pcm.Length / channels, decode_fec);
            }
            else
            {
                numSamplesDecoded = opus_decode_float(st, null, 0, pcm, pcm.Length / channels, decode_fec);
            }

            if (numSamplesDecoded == (int)OpusStatusCode.InvalidPacket)
                return 0;

            if (numSamplesDecoded <= 0)
            {
                HandleStatusCode((OpusStatusCode)numSamplesDecoded);
            }

            return numSamplesDecoded;
        }

        private static void HandleStatusCode(OpusStatusCode statusCode)
        {
            if (statusCode != OpusStatusCode.OK)
            {
                throw new OpusException(statusCode, Marshal.PtrToStringAnsi(opus_strerror(statusCode)));
            }
        }
    }
}
