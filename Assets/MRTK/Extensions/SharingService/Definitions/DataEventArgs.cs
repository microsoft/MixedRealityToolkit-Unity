using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    public struct DataEventArgs
    {
        /// <summary>
        /// The type of the data.
        /// </summary>
        public short Type;
        /// <summary>
        /// The device ID of the data's sender.
        /// </summary>
        public short Sender;
        /// <summary>
        /// Serialized data. May be null.
        /// </summary>
        public byte[] Data;
    }
}