using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POpusCodec.Enums
{
    public enum OpusStatusCode : int
    {
        OK = 0,
        BadArguments = -1,
        BufferTooSmall = -2,
        InternalError = -3,
        InvalidPacket = -4,
        Unimplemented = -5,
        InvalidState = -6,
        AllocFail = -7
    }
}
