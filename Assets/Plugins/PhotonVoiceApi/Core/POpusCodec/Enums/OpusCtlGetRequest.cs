using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POpusCodec.Enums
{
    internal enum OpusCtlGetRequest : int
    {
        Application = 4001,
        Bitrate = 4003,
        MaxBandwidth = 4005,
        VBR = 4007,
        Bandwidth = 4009,
        Complexity = 4011,
        InbandFec = 4013,
        PacketLossPercentage = 4015,
        Dtx = 4017,
        VBRConstraint = 4021,
        ForceChannels = 4023,
        Signal = 4025,
        LookAhead = 4027,
        SampleRate = 4029,
        FinalRange = 4031,
        Pitch = 4033,
        Gain = 4035,
        LsbDepth = 4037,
        LastPacketDurationRequest = 4039
    }
}
