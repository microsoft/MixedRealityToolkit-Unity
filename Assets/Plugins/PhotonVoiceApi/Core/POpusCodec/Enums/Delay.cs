using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POpusCodec.Enums
{
    /// <summary>
    /// Using a duration of less than 10 ms will prevent the encoder from using the LPC or hybrid modes. 
    /// </summary>
    public enum Delay
    {
        /// <summary>
        /// 2.5ms
        /// </summary>
        Delay2dot5ms = 5,
        /// <summary>
        /// 5ms
        /// </summary>
        Delay5ms = 10,
        /// <summary>
        /// 10ms
        /// </summary>
        Delay10ms = 20,
        /// <summary>
        /// 20ms
        /// </summary>
        Delay20ms = 40,
        /// <summary>
        /// 40ms
        /// </summary>
        Delay40ms = 80,
        /// <summary>
        /// 60ms
        /// </summary>
        Delay60ms = 120
    }
}
