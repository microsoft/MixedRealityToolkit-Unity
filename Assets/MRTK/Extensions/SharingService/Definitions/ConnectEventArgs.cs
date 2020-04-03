// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    /// <summary>
    /// Struct describing a status change event.
    /// </summary>
    public struct StatusEventArgs
    {
        /// <summary>
        /// The current status of the device.
        /// </summary>
        public ConnectStatus Status;

        /// <summary>
        /// The current role of the device.
        /// </summary>
        public AppRole AppRole;

        /// <summary>
        /// Optional message for debug purposes.
        /// </summary>
        public string Message;
    }
}