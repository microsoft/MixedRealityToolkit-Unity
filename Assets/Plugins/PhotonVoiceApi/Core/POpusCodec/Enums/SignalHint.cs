using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POpusCodec.Enums
{
    public enum SignalHint : int
    {
        /// <summary>
        /// (default) 
        /// </summary>
        Auto = -1000,
        /// <summary>
        /// Bias thresholds towards choosing LPC or Hybrid modes
        /// </summary>
        Voice = 3001,
        /// <summary>
        /// Bias thresholds towards choosing MDCT modes. 
        /// </summary>
        Music = 3002
    }
}
