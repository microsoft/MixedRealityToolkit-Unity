using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POpusCodec.Enums
{
    public enum Bandwidth : int
    {
        /// <summary>
        /// Up to 4Khz
        /// </summary>
        Narrowband = 1101,
        /// <summary>
        /// Up to 6Khz
        /// </summary>
        Mediumband = 1102,
        /// <summary>
        /// Up to 8Khz
        /// </summary>
        Wideband = 1103,
        /// <summary>
        /// Up to 12Khz
        /// </summary>
        SuperWideband = 1104,
        /// <summary>
        /// Up to 20Khz (High Definition)
        /// </summary>
        Fullband = 1105
    }
}
