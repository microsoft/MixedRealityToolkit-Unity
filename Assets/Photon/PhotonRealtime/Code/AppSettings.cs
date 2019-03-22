// -----------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>Settings for Photon application(s) and the server to connect to.</summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif


namespace Photon.Realtime
{
    using System;
    using ExitGames.Client.Photon;

    #if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    #endif


    /// <summary>
    /// Settings for Photon application(s) and the server to connect to.
    /// </summary>
    /// <remarks>
    /// This is Serializable for Unity, so it can be included in ScriptableObject instances.
    /// </remarks>
    #if !NETFX_CORE || SUPPORTED_UNITY
    [Serializable]
    #endif
    public class AppSettings
    {
        /// <summary>AppId for Realtime or PUN.</summary>
        public string AppIdRealtime;
        /// <summary>AppId for the Chat Api.</summary>
        public string AppIdChat;
        /// <summary>AppId for use in the Voice Api.</summary>
        public string AppIdVoice;

        /// <summary>The AppVersion can be used to identify builds and will split the AppId distinct "Virtual AppIds" (important for matchmaking).</summary>
        public string AppVersion;


        /// <summary>If false, the app will attempt to connect to a Master Server (which is obsolete but sometimes still necessary).</summary>
        /// <remarks>if true, Server points to a NameServer (or is null, using the default), else it points to a MasterServer.</remarks>
        public bool UseNameServer = true;

        /// <summary>Can be set to any of the Photon Cloud's region names to directly connect to that region.</summary>
        /// <remarks>if this IsNullOrEmpty() AND UseNameServer == true, use BestRegion. else, use a server</remarks>
        public string FixedRegion;

        /// <summary>The address (hostname or IP) of the server to connect to.</summary>
        public string Server;
        /// <summary>If not null, this sets the port of the first Photon server to connect to (that will "forward" the client as needed).</summary>
        public int Port;
        /// <summary>The network level protocol to use.</summary>
        public ConnectionProtocol Protocol = ConnectionProtocol.Udp;

        /// <summary>If true, the client will request the list of currently available lobbies.</summary>
        public bool EnableLobbyStatistics;
        /// <summary>Log level for the network lib.</summary>
        public DebugLevel NetworkLogging = DebugLevel.ERROR;

        /// <summary>If true, the Server field contains a Master Server address (if any address at all).</summary>
        public bool IsMasterServerAddress { get { return !this.UseNameServer; } }
        /// <summary>If true, the client should fetch the region list from the Name Server and find the one with best ping.</summary>
        /// <remarks>See "Best Region" in the online docs.</remarks>
        public bool IsBestRegion { get { return this.UseNameServer && string.IsNullOrEmpty(this.FixedRegion); } }
        /// <summary>If true, the default nameserver address for the Photon Cloud should be used.</summary>
        public bool IsDefaultNameServer { get { return this.UseNameServer && string.IsNullOrEmpty(this.Server); } }
        /// <summary>If true, the default ports for a protocol will be used.</summary>
        public bool IsDefaultPort { get { return this.Port <= 0; } }

        /// <summary>ToString but with more details.</summary>
        public string ToStringFull()
        {
            return string.Format("IsBestRegion: {0} IsDefaultNameServer: {1} IsDefaultPort: {2}", this.IsBestRegion, this.IsDefaultNameServer, this.IsDefaultPort);
        }
    }
}