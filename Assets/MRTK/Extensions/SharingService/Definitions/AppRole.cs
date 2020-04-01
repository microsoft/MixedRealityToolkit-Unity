using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    [Serializable]
    public enum AppRole : byte
    {
        /// <summary>
        /// The sharing app's role has not yet been defined.
        /// </summary>
        None = 0,  
        /// <summary>
        /// The app is considered a client.
        /// </summary>
        Client = 1,
        /// <summary>
        /// The app is considered a server. This should be used for dedicated server setups.
        /// </summary>
        Server = 2,
        /// <summary>
        /// The app is considered both host and server.
        /// In Photon this is equivalent to Master Client.
        /// </summary>
        Host = Client | Server,
    }
}