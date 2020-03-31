using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    public struct DataEventArgs
    {
        /// <summary>
        /// The type of the data.
        /// </summary>
        public int Type;
        /// <summary>
        /// The device ID of the data's sender.
        /// </summary>
        public short Sender;
        /// <summary>
        /// Serialized data.
        /// </summary>
        public Byte[] Data;
    }
}