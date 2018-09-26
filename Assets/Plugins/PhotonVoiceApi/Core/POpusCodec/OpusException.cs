using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POpusCodec.Enums;

namespace POpusCodec
{
    public class OpusException : Exception
    {
        private OpusStatusCode _statusCode = OpusStatusCode.OK;

        public OpusStatusCode StatusCode
        {
            get
            {
                return _statusCode;
            }
        }

        public OpusException(OpusStatusCode statusCode, string message)
            : base(message)
        {
            _statusCode = statusCode;
        }
    }
}
