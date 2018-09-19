using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POpusCodec.Enums;
using System.Runtime.InteropServices;

namespace POpusCodec
{
    public class OpusEncoder : IDisposable
    {
        public const int BitrateMax = -1;

        private IntPtr _handle = IntPtr.Zero;
        private string _version = string.Empty;
        private const int RecommendedMaxPacketSize = 4000;
        private int _frameSizePerChannel = 960;
        private SamplingRate _inputSamplingRate = SamplingRate.Sampling48000;
        private Channels _inputChannels = Channels.Stereo;
        
        public SamplingRate InputSamplingRate
        {
            get
            {
                return _inputSamplingRate;
            }
        }

        public Channels InputChannels
        {
            get
            {
                return _inputChannels;
            }
        }


        private readonly byte[] writePacket = new byte[RecommendedMaxPacketSize];
        private static readonly ArraySegment<byte> EmptyBuffer = new ArraySegment<byte>(new byte[] { });

        public string Version
        {
            get
            {
                return _version;
            }
        }

        private Delay _encoderDelay = Delay.Delay20ms;

        /// <summary>
        /// Using a duration of less than 10 ms will prevent the encoder from using the LPC or hybrid modes. 
        /// </summary>
        public Delay EncoderDelay
        {
            set
            {
                _encoderDelay = value;
                _frameSizePerChannel = (int)((((int)_inputSamplingRate) / 1000) * ((decimal)_encoderDelay) / 2);
            }
            get
            {
                return _encoderDelay;
            }
        }

        public int FrameSizePerChannel
        {
            get
            {
                return _frameSizePerChannel;
            }
        }

        public int Bitrate
        {
            get
            {
                return Wrapper.get_opus_encoder_ctl(_handle, OpusCtlGetRequest.Bitrate);
            }
            set
            {
                Wrapper.set_opus_encoder_ctl(_handle, OpusCtlSetRequest.Bitrate, value);
            }
        }

        public Bandwidth MaxBandwidth
        {
            get
            {
                return (Bandwidth)Wrapper.get_opus_encoder_ctl(_handle, OpusCtlGetRequest.MaxBandwidth);
            }
            set
            {
                Wrapper.set_opus_encoder_ctl(_handle, OpusCtlSetRequest.MaxBandwidth, (int)value);
            }
        }

        public Complexity Complexity
        {
            get
            {
                return (Complexity)Wrapper.get_opus_encoder_ctl(_handle, OpusCtlGetRequest.Complexity);
            }
            set
            {
                Wrapper.set_opus_encoder_ctl(_handle, OpusCtlSetRequest.Complexity, (int)value);
            }
        }

        public int ExpectedPacketLossPercentage
        {
            get
            {
                return Wrapper.get_opus_encoder_ctl(_handle, OpusCtlGetRequest.PacketLossPercentage);
            }
            set
            {
                Wrapper.set_opus_encoder_ctl(_handle, OpusCtlSetRequest.PacketLossPercentage, value);
            }
        }

        public SignalHint SignalHint
        {
            get
            {
                return (SignalHint)Wrapper.get_opus_encoder_ctl(_handle, OpusCtlGetRequest.Signal);
            }
            set
            {
                Wrapper.set_opus_encoder_ctl(_handle, OpusCtlSetRequest.Signal, (int)value);
            }
        }

        public ForceChannels ForceChannels
        {
            get
            {
                return (ForceChannels)Wrapper.get_opus_encoder_ctl(_handle, OpusCtlGetRequest.ForceChannels);
            }
            set
            {
                Wrapper.set_opus_encoder_ctl(_handle, OpusCtlSetRequest.ForceChannels, (int)value);
            }
        }

        public bool UseInbandFEC
        {
            get
            {
                return Wrapper.get_opus_encoder_ctl(_handle, OpusCtlGetRequest.InbandFec) == 1;
            }
            set
            {
                Wrapper.set_opus_encoder_ctl(_handle, OpusCtlSetRequest.InbandFec, value ? 1 : 0);
            }
        }

        public bool UseUnconstrainedVBR
        {
            get
            {
                return Wrapper.get_opus_encoder_ctl(_handle, OpusCtlGetRequest.VBRConstraint) == 0;
            }
            set
            {
                Wrapper.set_opus_encoder_ctl(_handle, OpusCtlSetRequest.VBRConstraint, value ? 0 : 1);
            }
        }

        public bool DtxEnabled
        {
            get
            {
                return Wrapper.get_opus_encoder_ctl(_handle, OpusCtlGetRequest.Dtx) == 1;
            }
            set
            {
                Wrapper.set_opus_encoder_ctl(_handle, OpusCtlSetRequest.Dtx, value ? 1 : 0);
            }
        }

        //public OpusEncoder(SamplingRate inputSamplingRateHz, Channels numChannels)
        //    : this(inputSamplingRateHz, numChannels,  120000, OpusApplicationType.Audio, Delay.Delay20ms)
        //{ }

        //public OpusEncoder(SamplingRate inputSamplingRateHz, Channels numChannels, int bitrate)
        //    : this(inputSamplingRateHz, numChannels, bitrate, OpusApplicationType.Audio, Delay.Delay20ms)
        //{ }

        //public OpusEncoder(SamplingRate inputSamplingRateHz, Channels numChannels, int bitrate, OpusApplicationType applicationType)
        //    : this(inputSamplingRateHz, numChannels, bitrate, applicationType, Delay.Delay20ms)
        //{ }

        public OpusEncoder(SamplingRate inputSamplingRateHz, Channels numChannels, int bitrate, OpusApplicationType applicationType, Delay encoderDelay)
        {
            if ((inputSamplingRateHz != SamplingRate.Sampling08000)
                && (inputSamplingRateHz != SamplingRate.Sampling12000)
                && (inputSamplingRateHz != SamplingRate.Sampling16000)
                && (inputSamplingRateHz != SamplingRate.Sampling24000)
                && (inputSamplingRateHz != SamplingRate.Sampling48000))
            {
                throw new ArgumentOutOfRangeException("inputSamplingRateHz", "Must use one of the pre-defined sampling rates(" + inputSamplingRateHz + ")");
            }
            if ((numChannels != Channels.Mono)
                && (numChannels != Channels.Stereo))
            {
                throw new ArgumentOutOfRangeException("numChannels", "Must be Mono or Stereo");
            }
            if ((applicationType != OpusApplicationType.Audio)
                && (applicationType != OpusApplicationType.RestrictedLowDelay)
                && (applicationType != OpusApplicationType.Voip))
            {
                throw new ArgumentOutOfRangeException("applicationType", "Must use one of the pre-defined application types (" + applicationType + ")");
            }
            if ((encoderDelay != Delay.Delay10ms)
                && (encoderDelay != Delay.Delay20ms)
                && (encoderDelay != Delay.Delay2dot5ms)
                && (encoderDelay != Delay.Delay40ms)
                && (encoderDelay != Delay.Delay5ms)
                && (encoderDelay != Delay.Delay60ms))
            {
                throw new ArgumentOutOfRangeException("encoderDelay", "Must use one of the pre-defined delay values (" + encoderDelay + ")"); ;
            }

            _inputSamplingRate = inputSamplingRateHz;
            _inputChannels = numChannels;
            _handle = Wrapper.opus_encoder_create(inputSamplingRateHz, numChannels, applicationType);
            _version = Marshal.PtrToStringAnsi(Wrapper.opus_get_version_string());

            if (_handle == IntPtr.Zero)
            {
                throw new OpusException(OpusStatusCode.AllocFail, "Memory was not allocated for the encoder");
            }

            EncoderDelay = encoderDelay;
            Bitrate = bitrate;
        }

        public ArraySegment<byte> Encode(float[] pcmSamples)
        {
            int size = Wrapper.opus_encode(_handle, pcmSamples, _frameSizePerChannel, writePacket);

            if (size <= 1) //DTX. Negative already handled at this point
                return EmptyBuffer;
            else
                return new ArraySegment<byte>(writePacket, 0, size);
        }

        public ArraySegment<byte> Encode(short[] pcmSamples)
        {
            int size = Wrapper.opus_encode(_handle, pcmSamples, _frameSizePerChannel, writePacket);

            if (size <= 1) //DTX. Negative already handled at this point
                return EmptyBuffer;
            else
                return new ArraySegment<byte>(writePacket, 0, size);
        }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                Wrapper.opus_encoder_destroy(_handle);
                _handle = IntPtr.Zero;
            }
        }
    }
}
