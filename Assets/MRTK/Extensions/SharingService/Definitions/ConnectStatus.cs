using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    [Serializable]
    public enum ConnectStatus : byte
    {
        /// <summary>
        /// The sharing service has not connected.
        /// </summary>
        NotConnected = 1,
        /// <summary>
        /// The sharing service is attempting to connect.
        /// </summary>
        Connecting = 2,
        /// <summary>
        /// The sharing service has connected.
        /// </summary>
        Connected = 4,
    }
}