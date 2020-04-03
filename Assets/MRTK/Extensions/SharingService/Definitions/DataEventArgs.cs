// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    /// <summary>
    /// Struct describing a data received event.
    /// </summary>
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