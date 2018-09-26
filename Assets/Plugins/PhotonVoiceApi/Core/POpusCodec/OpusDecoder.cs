using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POpusCodec.Enums;
using System.Runtime.InteropServices;

namespace POpusCodec
{
    public class OpusDecoder : IDisposable
    {
        private IntPtr _handle = IntPtr.Zero;
        private string _version = string.Empty;
        private const int MaxFrameSize = 5760;

        // makes sense if OpusEncoder.UseInbandFEC and OpusEncoder.ExpectedPacketLossPercentage are set
        // TODO: to implement FEC, decoder normally should decode previous frame while saving current for next decode() call (see opus_demo.c)
#pragma warning disable 414 // "not used" warning
        private bool _previousPacketInvalid = true;
#pragma warning restore 414

        private int _channelCount;

        private static readonly float[] EmptyBufferFloat = new float[] { };
        private static readonly short[] EmptyBufferShort = new short[] { };

        public string Version
        {
            get
            {
                return _version;
            }
        }

        private Bandwidth? _previousPacketBandwidth = null;

        public Bandwidth? PreviousPacketBandwidth
        {
            get
            {
                return _previousPacketBandwidth;
            }
        }

        public OpusDecoder(SamplingRate outputSamplingRateHz, Channels numChannels)
        {
            if ((outputSamplingRateHz != SamplingRate.Sampling08000)
                && (outputSamplingRateHz != SamplingRate.Sampling12000)
                && (outputSamplingRateHz != SamplingRate.Sampling16000)
                && (outputSamplingRateHz != SamplingRate.Sampling24000)
                && (outputSamplingRateHz != SamplingRate.Sampling48000))
            {
                throw new ArgumentOutOfRangeException("outputSamplingRateHz", "Must use one of the pre-defined sampling rates (" + outputSamplingRateHz + ")");
            }
            if ((numChannels != Channels.Mono)
                && (numChannels != Channels.Stereo))
            {
                throw new ArgumentOutOfRangeException("numChannels", "Must be Mono or Stereo");
            }

            _channelCount = (int)numChannels;
            _handle = Wrapper.opus_decoder_create(outputSamplingRateHz, numChannels);
            _version = Marshal.PtrToStringAnsi( Wrapper.opus_get_version_string());

            if (_handle == IntPtr.Zero)
            {
                throw new OpusException(OpusStatusCode.AllocFail, "Memory was not allocated for the encoder");
            }
        }
        
        private float[] bufferFloat; // allocated for exactly 1 frame size as first valid frame received

        // pass null to indicate packet loss
        public float[] DecodePacketFloat(byte[] packetData)
        {
            if (this.bufferFloat == null && packetData == null)
            {
                return EmptyBufferFloat;
            }
            
            int numSamplesDecoded = 0;

            float[] buf;
            if (this.bufferFloat == null)
            {
                buf = new float[MaxFrameSize * _channelCount];                
            }
            else
            {
                buf = this.bufferFloat;
            }

            numSamplesDecoded = Wrapper.opus_decode(_handle, packetData, buf, 0, _channelCount);

            if (packetData == null)
            {
                _previousPacketInvalid = false;
            }
            else
            { 
                int bandwidth = Wrapper.opus_packet_get_bandwidth(packetData);
                _previousPacketInvalid = bandwidth == (int)OpusStatusCode.InvalidPacket;
            }

            if (numSamplesDecoded == 0)
                return EmptyBufferFloat;

            if (this.bufferFloat == null)
            {
                this.bufferFloat = new float[numSamplesDecoded * _channelCount];
                Buffer.BlockCopy(buf, 0, this.bufferFloat, 0, numSamplesDecoded * sizeof(float));
            }
            return this.bufferFloat;
        }

        private short[] bufferShort; // allocated for exactly 1 frame size as first valid frame received

        // pass null to indicate packet loss
        public short[] DecodePacketShort(byte[] packetData)
        {
            if (this.bufferShort == null && packetData == null)
            {
                return EmptyBufferShort;
            }

            int numSamplesDecoded = 0;

            short[] buf;
            if (this.bufferShort == null)
            {
                buf = new short[MaxFrameSize * _channelCount];
            }
            else
            {
                buf = this.bufferShort;
            }

            numSamplesDecoded = Wrapper.opus_decode(_handle, packetData, buf, 0, _channelCount);

            if (packetData == null)
            {
                _previousPacketInvalid = false;
            }
            else
            {
                int bandwidth = Wrapper.opus_packet_get_bandwidth(packetData);
                _previousPacketInvalid = bandwidth == (int)OpusStatusCode.InvalidPacket;
            }

            if (numSamplesDecoded == 0)
                return EmptyBufferShort;

            if (this.bufferShort == null)
            {
                this.bufferShort = new short[numSamplesDecoded * _channelCount];
                Buffer.BlockCopy(buf, 0, this.bufferShort, 0, numSamplesDecoded * sizeof(short));
            }
            return this.bufferShort;
        }
        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                Wrapper.opus_decoder_destroy(_handle);
                _handle = IntPtr.Zero;
            }
        }
    }
}
