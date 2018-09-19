using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POpusCodec.Enums
{
    internal enum OpusCtlSetRequest : int
    {
        Application = 4000,
        Bitrate = 4002,
        MaxBandwidth = 4004,
        VBR = 4006,
        Bandwidth = 4008,
        Complexity = 4010,
        InbandFec = 4012,
        PacketLossPercentage = 4014,
        Dtx = 4016,
        VBRConstraint = 4020,
        ForceChannels = 4022,
        Signal = 4024,
        Gain = 4034,
        LsbDepth = 4036
    }
}
