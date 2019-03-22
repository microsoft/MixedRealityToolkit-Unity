// ----------------------------------------------------------------------------------------------------------------------
// <summary>The Photon Chat Api enables clients to connect to a chat server and communicate with other clients.</summary>
// <remarks>ChatClient is the main class of this api.</remarks>
// <copyright company="Exit Games GmbH">Photon Chat Api - Copyright (C) 2014 Exit Games GmbH</copyright>
// ----------------------------------------------------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif

namespace Photon.Chat
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using ExitGames.Client.Photon;

    #if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    #endif


    /// <summary>
    /// Provides basic operations of the Photon Chat server. This internal class is used by public ChatClient.
    /// </summary>
    public class ChatPeer : PhotonPeer
    {
        /// <summary>Name Server Host Name for Photon Cloud. Without port and without any prefix.</summary>
        public const string NameServerHost = "ns.exitgames.com";

        /// <summary>Name Server for HTTP connections to the Photon Cloud. Includes prefix and port.</summary>
        public const string NameServerHttp = "http://ns.exitgamescloud.com:80/photon/n";

        /// <summary>Name Server port per protocol (the UDP port is different than TCP, etc).</summary>
        private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>() { { ConnectionProtocol.Udp, 5058 }, { ConnectionProtocol.Tcp, 4533 }, { ConnectionProtocol.WebSocket, 9093 }, { ConnectionProtocol.WebSocketSecure, 19093 } }; //, { ConnectionProtocol.RHttp, 6063 } };

        /// <summary>Name Server Address for Photon Cloud (based on current protocol). You can use the default values and usually won't have to set this value.</summary>
        public string NameServerAddress { get { return this.GetNameServerAddress(); } }

        virtual internal bool IsProtocolSecure { get { return this.UsedProtocol == ConnectionProtocol.WebSocketSecure; } }

        /// <summary> Chat Peer constructor. </summary>
        /// <param name="listener">Chat listener implementation.</param>
        /// <param name="protocol">Protocol to be used by the peer.</param>
        public ChatPeer(IPhotonPeerListener listener, ConnectionProtocol protocol) : base(listener, protocol)
        {
            this.ConfigUnitySockets();
        }



        // Sets up the socket implementations to use, depending on platform
        [Conditional("SUPPORTED_UNITY")]
        private void ConfigUnitySockets()
        {
            Type websocketType = null;
            #if UNITY_XBOXONE && !UNITY_EDITOR
            websocketType = Type.GetType("ExitGames.Client.Photon.SocketWebTcpNativeDynamic, PhotonWebSocket", false);
            if (websocketType == null)
            {
                websocketType = Type.GetType("ExitGames.Client.Photon.SocketWebTcpNativeDynamic, Assembly-CSharp-firstpass", false);
            }
            if (websocketType == null)
            {
                websocketType = Type.GetType("ExitGames.Client.Photon.SocketWebTcpNativeDynamic, Assembly-CSharp", false);
            }
            if (websocketType == null)
            {
                UnityEngine.Debug.LogError("UNITY_XBOXONE is defined but peer could not find SocketWebTcpNativeDynamic. Check your project files to make sure the native WSS implementation is available. Won't connect.");
            }
            #else
            // to support WebGL export in Unity, we find and assign the SocketWebTcp class (if it's in the project).
            // alternatively class SocketWebTcp might be in the Photon3Unity3D.dll
            websocketType = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, PhotonWebSocket", false);
            if (websocketType == null)
            {
                websocketType = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, Assembly-CSharp-firstpass", false);
            }
            if (websocketType == null)
            {
                websocketType = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, Assembly-CSharp", false);
            }
            #endif

            if (websocketType != null)
            {
                this.SocketImplementationConfig[ConnectionProtocol.WebSocket] = websocketType;
                this.SocketImplementationConfig[ConnectionProtocol.WebSocketSecure] = websocketType;
            }

            #if NET_4_6 && (UNITY_EDITOR || !ENABLE_IL2CPP)
            this.SocketImplementationConfig[ConnectionProtocol.Udp] = typeof(SocketUdpAsync);
            this.SocketImplementationConfig[ConnectionProtocol.Tcp] = typeof(SocketTcpAsync);
            #endif
        }


        /// <summary>
        /// Gets the NameServer Address (with prefix and port), based on the set protocol (this.UsedProtocol).
        /// </summary>
        /// <returns>NameServer Address (with prefix and port).</returns>
        private string GetNameServerAddress()
        {
            var protocolPort = 0;
            ProtocolToNameServerPort.TryGetValue(this.TransportProtocol, out protocolPort);

            switch (this.TransportProtocol)
            {
                case ConnectionProtocol.Udp:
                case ConnectionProtocol.Tcp:
                    return string.Format("{0}:{1}", NameServerHost, protocolPort);
                #if RHTTP
                case ConnectionProtocol.RHttp:
                    return NameServerHttp;
                #endif
                case ConnectionProtocol.WebSocket:
                    return string.Format("ws://{0}:{1}", NameServerHost, protocolPort);
                case ConnectionProtocol.WebSocketSecure:
                    return string.Format("wss://{0}:{1}", NameServerHost, protocolPort);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary> Connects to NameServer. </summary>
        /// <returns>If the connection attempt could be sent.</returns>
        public bool Connect()
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "Connecting to nameserver " + this.NameServerAddress);
            }

            return this.Connect(this.NameServerAddress, "NameServer");
        }

        /// <summary> Authenticates on NameServer. </summary>
        /// <returns>If the authentication operation request could be sent.</returns>
        public bool AuthenticateOnNameServer(string appId, string appVersion, string region, AuthenticationValues authValues)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
            }

            var opParameters = new Dictionary<byte, object>();

            opParameters[ParameterCode.AppVersion] = appVersion;
            opParameters[ParameterCode.ApplicationId] = appId;
            opParameters[ParameterCode.Region] = region;

            if (authValues != null)
            {
                if (!string.IsNullOrEmpty(authValues.UserId))
                {
                    opParameters[ParameterCode.UserId] = authValues.UserId;
                }

                if (authValues != null && authValues.AuthType != CustomAuthenticationType.None)
                {
                    opParameters[ParameterCode.ClientAuthenticationType] = (byte) authValues.AuthType;
                    if (!string.IsNullOrEmpty(authValues.Token))
                    {
                        opParameters[ParameterCode.Secret] = authValues.Token;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(authValues.AuthGetParameters))
                        {
                            opParameters[ParameterCode.ClientAuthenticationParams] = authValues.AuthGetParameters;
                        }
                        if (authValues.AuthPostData != null)
                        {
                            opParameters[ParameterCode.ClientAuthenticationData] = authValues.AuthPostData;
                        }
                    }
                }
            }

            return this.SendOperation(ChatOperationCode.Authenticate, opParameters, new SendOptions() { Reliability = true, Encrypt = this.IsEncryptionAvailable });
        }
    }

    /// <summary>
    /// Options for optional "Custom Authentication" services used with Photon. Used by OpAuthenticate after connecting to Photon.
    /// </summary>
    public enum CustomAuthenticationType : byte
    {
        /// <summary>Use a custom authentification service. Currently the only implemented option.</summary>
        Custom = 0,

        /// <summary>Authenticates users by their Steam Account. Set auth values accordingly!</summary>
        Steam = 1,

        /// <summary>Authenticates users by their Facebook Account. Set auth values accordingly!</summary>
        Facebook = 2,

        /// <summary>Authenticates users by their Oculus Account and token.</summary>
        Oculus = 3,

        /// <summary>Authenticates users by their PSN Account and token.</summary>
        PlayStation = 4,

        /// <summary>Authenticates users by their Xbox Account and XSTS token.</summary>
        Xbox = 5,

        /// <summary>Disables custom authentification. Same as not providing any AuthenticationValues for connect (more precisely for: OpAuthenticate).</summary>
        None = byte.MaxValue
    }



    /// <summary>
    /// Container for user authentication in Photon. Set AuthValues before you connect - all else is handled.
    /// </summary>
    /// <remarks>
    /// On Photon, user authentication is optional but can be useful in many cases.
    /// If you want to FindFriends, a unique ID per user is very practical.
    ///
    /// There are basically three options for user authentification: None at all, the client sets some UserId
    /// or you can use some account web-service to authenticate a user (and set the UserId server-side).
    ///
    /// Custom Authentication lets you verify end-users by some kind of login or token. It sends those
    /// values to Photon which will verify them before granting access or disconnecting the client.
    ///
    /// The Photon Cloud Dashboard will let you enable this feature and set important server values for it.
    /// https://dashboard.photonengine.com
    /// </remarks>
    public class AuthenticationValues
    {
        /// <summary>See AuthType.</summary>
        private CustomAuthenticationType authType = CustomAuthenticationType.None;

        /// <summary>The type of custom authentication provider that should be used. Currently only "Custom" or "None" (turns this off).</summary>
        public CustomAuthenticationType AuthType
        {
            get { return authType; }
            set { authType = value; }
        }

        /// <summary>This string must contain any (http get) parameters expected by the used authentication service. By default, username and token.</summary>
        /// <remarks>Standard http get parameters are used here and passed on to the service that's defined in the server (Photon Cloud Dashboard).</remarks>
        public string AuthGetParameters { get; set; }

        /// <summary>Data to be passed-on to the auth service via POST. Default: null (not sent). Either string or byte[] (see setters).</summary>
        public object AuthPostData { get; private set; }

        /// <summary>After initial authentication, Photon provides a token for this client / user, which is subsequently used as (cached) validation.</summary>
        public string Token { get; set; }

        /// <summary>The UserId should be a unique identifier per user. This is for finding friends, etc..</summary>
        public string UserId { get; set; }


        /// <summary>Creates empty auth values without any info.</summary>
        public AuthenticationValues()
        {
        }

        /// <summary>Creates minimal info about the user. If this is authenticated or not, depends on the set AuthType.</summary>
        /// <param name="userId">Some UserId to set in Photon.</param>
        public AuthenticationValues(string userId)
        {
            this.UserId = userId;
        }

        /// <summary>Sets the data to be passed-on to the auth service via POST.</summary>
        /// <param name="stringData">String data to be used in the body of the POST request. Null or empty string will set AuthPostData to null.</param>
        public virtual void SetAuthPostData(string stringData)
        {
            this.AuthPostData = (string.IsNullOrEmpty(stringData)) ? null : stringData;
        }

        /// <summary>Sets the data to be passed-on to the auth service via POST.</summary>
        /// <param name="byteData">Binary token / auth-data to pass on.</param>
        public virtual void SetAuthPostData(byte[] byteData)
        {
            this.AuthPostData = byteData;
        }

        /// <summary>Adds a key-value pair to the get-parameters used for Custom Auth.</summary>
        /// <remarks>This method does uri-encoding for you.</remarks>
        /// <param name="key">Key for the value to set.</param>
        /// <param name="value">Some value relevant for Custom Authentication.</param>
        public virtual void AddAuthParameter(string key, string value)
        {
            string ampersand = string.IsNullOrEmpty(this.AuthGetParameters) ? "" : "&";
            this.AuthGetParameters = string.Format("{0}{1}{2}={3}", this.AuthGetParameters, ampersand, System.Uri.EscapeDataString(key), System.Uri.EscapeDataString(value));
        }
        /// <summary>
        /// Transform this object into string.
        /// </summary>
        /// <returns>string representation of this object.</returns>
        public override string ToString()
        {
            return string.Format("AuthenticationValues UserId: {0}, GetParameters: {1} Token available: {2}", UserId, this.AuthGetParameters, Token != null);
        }
    }

    /// <summary>Class for constants. Codes for parameters of Operations and Events.</summary>
    public class ParameterCode
    {
        /// <summary>(224) Your application's ID: a name on your own Photon or a GUID on the Photon Cloud</summary>
        public const byte ApplicationId = 224;
        /// <summary>(221) Internally used to establish encryption</summary>
        public const byte Secret = 221;
        /// <summary>(220) Version of your application</summary>
        public const byte AppVersion = 220;
        /// <summary>(217) This key's (byte) value defines the target custom authentication type/service the client connects with. Used in OpAuthenticate</summary>
        public const byte ClientAuthenticationType = 217;
        /// <summary>(216) This key's (string) value provides parameters sent to the custom authentication type/service the client connects with. Used in OpAuthenticate</summary>
        public const byte ClientAuthenticationParams = 216;
        /// <summary>(214) This key's (string or byte[]) value provides parameters sent to the custom authentication service setup in Photon Dashboard. Used in OpAuthenticate</summary>
        public const byte ClientAuthenticationData = 214;
        /// <summary>(210) Used for region values in OpAuth and OpGetRegions.</summary>
        public const byte Region = 210;
        /// <summary>(230) Address of a (game) server to use.</summary>
        public const byte Address = 230;
        /// <summary>(225) User's ID</summary>
        public const byte UserId = 225;
    }

    /// <summary>
    /// ErrorCode defines the default codes associated with Photon client/server communication.
    /// </summary>
    public class ErrorCode
    {
        /// <summary>(0) is always "OK", anything else an error or specific situation.</summary>
        public const int Ok = 0;

        // server - Photon low(er) level: <= 0

        /// <summary>
        /// (-3) Operation can't be executed yet (e.g. OpJoin can't be called before being authenticated, RaiseEvent cant be used before getting into a room).
        /// </summary>
        /// <remarks>
        /// Before you call any operations on the Cloud servers, the automated client workflow must complete its authorization.
        /// In PUN, wait until State is: JoinedLobby or ConnectedToMaster
        /// </remarks>
        public const int OperationNotAllowedInCurrentState = -3;

        /// <summary>(-2) The operation you called is not implemented on the server (application) you connect to. Make sure you run the fitting applications.</summary>
        public const int InvalidOperationCode = -2;

        /// <summary>(-1) Something went wrong in the server. Try to reproduce and contact Exit Games.</summary>
        public const int InternalServerError = -1;

        // server - PhotonNetwork: 0x7FFF and down
        // logic-level error codes start with short.max

        /// <summary>(32767) Authentication failed. Possible cause: AppId is unknown to Photon (in cloud service).</summary>
        public const int InvalidAuthentication = 0x7FFF;

        /// <summary>(32766) GameId (name) already in use (can't create another). Change name.</summary>
        public const int GameIdAlreadyExists = 0x7FFF - 1;

        /// <summary>(32765) Game is full. This rarely happens when some player joined the room before your join completed.</summary>
        public const int GameFull = 0x7FFF - 2;

        /// <summary>(32764) Game is closed and can't be joined. Join another game.</summary>
        public const int GameClosed = 0x7FFF - 3;

        /// <summary>(32762) Not in use currently.</summary>
        public const int ServerFull = 0x7FFF - 5;

        /// <summary>(32761) Not in use currently.</summary>
        public const int UserBlocked = 0x7FFF - 6;

        /// <summary>(32760) Random matchmaking only succeeds if a room exists thats neither closed nor full. Repeat in a few seconds or create a new room.</summary>
        public const int NoRandomMatchFound = 0x7FFF - 7;

        /// <summary>(32758) Join can fail if the room (name) is not existing (anymore). This can happen when players leave while you join.</summary>
        public const int GameDoesNotExist = 0x7FFF - 9;

        /// <summary>(32757) Authorization on the Photon Cloud failed becaus the concurrent users (CCU) limit of the app's subscription is reached.</summary>
        /// <remarks>
        /// Unless you have a plan with "CCU Burst", clients might fail the authentication step during connect.
        /// Affected client are unable to call operations. Please note that players who end a game and return
        /// to the master server will disconnect and re-connect, which means that they just played and are rejected
        /// in the next minute / re-connect.
        /// This is a temporary measure. Once the CCU is below the limit, players will be able to connect an play again.
        ///
        /// OpAuthorize is part of connection workflow but only on the Photon Cloud, this error can happen.
        /// Self-hosted Photon servers with a CCU limited license won't let a client connect at all.
        /// </remarks>
        public const int MaxCcuReached = 0x7FFF - 10;

        /// <summary>(32756) Authorization on the Photon Cloud failed because the app's subscription does not allow to use a particular region's server.</summary>
        /// <remarks>
        /// Some subscription plans for the Photon Cloud are region-bound. Servers of other regions can't be used then.
        /// Check your master server address and compare it with your Photon Cloud Dashboard's info.
        /// https://cloud.photonengine.com/dashboard
        ///
        /// OpAuthorize is part of connection workflow but only on the Photon Cloud, this error can happen.
        /// Self-hosted Photon servers with a CCU limited license won't let a client connect at all.
        /// </remarks>
        public const int InvalidRegion = 0x7FFF - 11;

        /// <summary>
        /// (32755) Custom Authentication of the user failed due to setup reasons (see Cloud Dashboard) or the provided user data (like username or token). Check error message for details.
        /// </summary>
        public const int CustomAuthenticationFailed = 0x7FFF - 12;
    }

}
