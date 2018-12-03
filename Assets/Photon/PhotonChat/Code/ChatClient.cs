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
    using System.Collections.Generic;
    using ExitGames.Client.Photon;

    #if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    #endif


    /// <summary>Central class of the Photon Chat API to connect, handle channels and messages.</summary>
    /// <remarks>
    /// This class must be instantiated with a IChatClientListener instance to get the callbacks.
    /// Integrate it into your game loop by calling Service regularly. If the target platform supports Threads/Tasks,
    /// set UseBackgroundWorkerForSending = true, to let the ChatClient keep the connection by sending from
    /// an independent thread.
    ///
    /// Call Connect with an AppId that is setup as Photon Chat application. Note: Connect covers multiple
    /// messages between this client and the servers. A short workflow will connect you to a chat server.
    ///
    /// Each ChatClient resembles a user in chat (set in Connect). Each user automatically subscribes a channel
    /// for incoming private messages and can message any other user privately.
    /// Before you publish messages in any non-private channel, that channel must be subscribed.
    ///
    /// PublicChannels is a list of subscribed channels, containing messages and senders.
    /// PrivateChannels contains all incoming and sent private messages.
    /// </remarks>
    public class ChatClient : IPhotonPeerListener
    {
        const int FriendRequestListMax = 1024;

        /// <summary>The address of last connected Name Server.</summary>
        public string NameServerAddress { get; private set; }

        /// <summary>The address of the actual chat server assigned from NameServer. Public for read only.</summary>
        public string FrontendAddress { get; private set; }

        /// <summary>Region used to connect to. Currently all chat is done in EU. It can make sense to use only one region for the whole game.</summary>
        private string chatRegion = "EU";

        /// <summary>Settable only before you connect! Defaults to "EU".</summary>
        public string ChatRegion
        {
            get { return this.chatRegion; }
            set { this.chatRegion = value; }
        }

        /// <summary>Current state of the ChatClient. Also use CanChat.</summary>
        public ChatState State { get; private set; }

        /// <summary> Disconnection cause. Check this inside <see cref="IChatClientListener.OnDisconnected"/>. </summary>
        public ChatDisconnectCause DisconnectedCause { get; private set; }
        /// <summary>
        /// Checks if this client is ready to send messages.
        /// </summary>
        public bool CanChat
        {
            get { return this.State == ChatState.ConnectedToFrontEnd && this.HasPeer; }
        }
        /// <summary>
        /// Checks if this client is ready to publish messages inside a public channel.
        /// </summary>
        /// <param name="channelName">The channel to do the check with.</param>
        /// <returns>Whether or not this client is ready to publish messages inside the public channel with the specified channelName.</returns>
        public bool CanChatInChannel(string channelName)
        {
            return this.CanChat && this.PublicChannels.ContainsKey(channelName) && !this.PublicChannelsUnsubscribing.Contains(channelName);
        }

        private bool HasPeer
        {
            get { return this.chatPeer != null; }
        }

        /// <summary>The version of your client. A new version also creates a new "virtual app" to separate players from older client versions.</summary>
        public string AppVersion { get; private set; }

        /// <summary>The AppID as assigned from the Photon Cloud.</summary>
        public string AppId { get; private set; }


        /// <summary>Settable only before you connect!</summary>
        public AuthenticationValues AuthValues { get; set; }

        /// <summary>The unique ID of a user/person, stored in AuthValues.UserId. Set it before you connect.</summary>
        /// <remarks>
        /// This value wraps AuthValues.UserId.
        /// It's not a nickname and we assume users with the same userID are the same person.</remarks>
        public string UserId
        {
            get
            {
                return (this.AuthValues != null) ? this.AuthValues.UserId : null;
            }
            private set
            {
                if (this.AuthValues == null)
                {
                    this.AuthValues = new AuthenticationValues();
                }
                this.AuthValues.UserId = value;
            }
        }

        /// <summary>If greater than 0, new channels will limit the number of messages they cache locally.</summary>
        /// <remarks>
        /// This can be useful to limit the amount of memory used by chats.
        /// You can set a MessageLimit per channel but this value gets applied to new ones.
        ///
        /// Note:
        /// Changing this value, does not affect ChatChannels that are already in use!
        /// </remarks>
        public int MessageLimit;
        /// <summary> Public channels this client is subscribed to. </summary>
        public readonly Dictionary<string, ChatChannel> PublicChannels;
        /// <summary> Private channels in which this client has exchanged messages. </summary>
        public readonly Dictionary<string, ChatChannel> PrivateChannels;

        // channels being in unsubscribing process
        // items will be removed on successful unsubscription or subscription (the latter required after attempt to unsubscribe from not existing channel)
        private readonly HashSet<string> PublicChannelsUnsubscribing;

        private readonly IChatClientListener listener = null;
        /// <summary> The Chat Peer used by this client. </summary>
        public ChatPeer chatPeer = null;
        private const string ChatAppName = "chat";
        private bool didAuthenticate;

        private int? statusToSetWhenConnected;
        private object messageToSetWhenConnected;

        private int msDeltaForServiceCalls = 50;
        private int msTimestampOfLastServiceCall;

        /// <summary>Defines if a background thread will call SendOutgoingCommands, while your code calls Service to dispatch received messages.</summary>
        /// <remarks>
        /// The benefit of using a background thread to call SendOutgoingCommands is this:
        ///
        /// Even if your game logic is being paused, the background thread will keep the connection to the server up.
        /// On a lower level, acknowledgements and pings will prevent a server-side timeout while (e.g.) Unity loads assets.
        ///
        /// Your game logic still has to call Service regularly, or else incoming messages are not dispatched.
        /// As this typicalls triggers UI updates, it's easier to call Service from the main/UI thread.
        /// </remarks>
        public bool UseBackgroundWorkerForSending { get; set; }

        /// <summary>Exposes the TransportProtocol of the used PhotonPeer. Settable while not connected.</summary>
        public ConnectionProtocol TransportProtocol
        {
            get { return this.chatPeer.TransportProtocol; }
            set
            {
                if (this.chatPeer == null || this.chatPeer.PeerState != PeerStateValue.Disconnected)
                {
                    this.listener.DebugReturn(DebugLevel.WARNING, "Can't set TransportProtocol. Disconnect first! " + ((this.chatPeer != null) ? "PeerState: " + this.chatPeer.PeerState : "The chatPeer is null."));
                    return;
                }
                this.chatPeer.TransportProtocol = value;
            }
        }

        /// <summary>Defines which IPhotonSocket class to use per ConnectionProtocol.</summary>
        /// <remarks>
        /// Several platforms have special Socket implementations and slightly different APIs.
        /// To accomodate this, switching the socket implementation for a network protocol was made available.
        /// By default, UDP and TCP have socket implementations assigned.
        ///
        /// You only need to set the SocketImplementationConfig once, after creating a PhotonPeer
        /// and before connecting. If you switch the TransportProtocol, the correct implementation is being used.
        /// </remarks>
        public Dictionary<ConnectionProtocol, Type> SocketImplementationConfig
        {
            get { return this.chatPeer.SocketImplementationConfig; }
        }

        /// <summary>
        /// Chat client constructor.
        /// </summary>
        /// <param name="listener">The chat listener implementation.</param>
        /// <param name="protocol">Connection protocol to be used by this client. Default is <see cref="ConnectionProtocol.Udp"/>.</param>
        public ChatClient(IChatClientListener listener, ConnectionProtocol protocol = ConnectionProtocol.Udp)
        {
            this.listener = listener;
            this.State = ChatState.Uninitialized;

            this.chatPeer = new ChatPeer(this, protocol);
            this.chatPeer.SerializationProtocolType = SerializationProtocol.GpBinaryV18;

            this.PublicChannels = new Dictionary<string, ChatChannel>();
            this.PrivateChannels = new Dictionary<string, ChatChannel>();

            this.PublicChannelsUnsubscribing = new HashSet<string>();
        }

        /// <summary>
        /// Connects this client to the Photon Chat Cloud service, which will also authenticate the user (and set a UserId).
        /// </summary>
        /// <param name="appId">Get your Photon Chat AppId from the <a href="https://dashboard.photonengine.com">Dashboard</a>.</param>
        /// <param name="appVersion">Any version string you make up. Used to separate users and variants of your clients, which might be incompatible.</param>
        /// <param name="authValues">Values for authentication. You can leave this null, if you set a UserId before. If you set authValues, they will override any UserId set before.</param>
        /// <returns></returns>
        public bool Connect(string appId, string appVersion, AuthenticationValues authValues)
        {
            this.chatPeer.TimePingInterval = 3000;
            this.DisconnectedCause = ChatDisconnectCause.None;

            this.AuthValues = authValues;

            this.AppId = appId;
            this.AppVersion = appVersion;
            this.didAuthenticate = false;
            this.chatPeer.QuickResendAttempts = 2;
            this.chatPeer.SentCountAllowance = 7;

            // clean all channels
            this.PublicChannels.Clear();
            this.PrivateChannels.Clear();
            this.PublicChannelsUnsubscribing.Clear();

            #if UNITY_WEBGL
            if (this.TransportProtocol == ConnectionProtocol.Tcp || this.TransportProtocol == ConnectionProtocol.Udp)
            {
                this.listener.DebugReturn(DebugLevel.WARNING, "WebGL requires WebSockets. Switching TransportProtocol to WebSocketSecure.");
                this.TransportProtocol = ConnectionProtocol.WebSocketSecure;
            }
            #endif

            this.NameServerAddress = this.chatPeer.NameServerAddress;
            bool isConnecting = this.chatPeer.Connect();
            if (isConnecting)
            {
                this.State = ChatState.ConnectingToNameServer;
            }

            if (this.UseBackgroundWorkerForSending)
            {
                SupportClass.StartBackgroundCalls(this.SendOutgoingInBackground, this.msDeltaForServiceCalls, "ChatClient Service Thread");
            }

            return isConnecting;
        }

        /// <summary>
        /// Connects this client to the Photon Chat Cloud service, which will also authenticate the user (and set a UserId).
        /// This also sets an online status once connected. By default it will set user status to <see cref="ChatUserStatus.Online"/>.
        /// See <see cref="SetOnlineStatus(int,object)"/> for more information.
        /// </summary>
        /// <param name="appId">Get your Photon Chat AppId from the <a href="https://dashboard.photonengine.com">Dashboard</a>.</param>
        /// <param name="appVersion">Any version string you make up. Used to separate users and variants of your clients, which might be incompatible.</param>
        /// <param name="authValues">Values for authentication. You can leave this null, if you set a UserId before. If you set authValues, they will override any UserId set before.</param>
        /// <param name="status">User status to set when connected. Predefined states are in class <see cref="ChatUserStatus"/>. Other values can be used at will.</param>
        /// <param name="message">Optional status Also sets a status-message which your friends can get.</param>
        /// <returns>If the connection attempt could be sent at all.</returns>
        public bool ConnectAndSetStatus(string appId, string appVersion, AuthenticationValues authValues,
            int status = ChatUserStatus.Online, object message = null)
        {
            statusToSetWhenConnected = status;
            messageToSetWhenConnected = message;
            return Connect(appId, appVersion, authValues);
        }

        /// <summary>
        /// Must be called regularly to keep connection between client and server alive and to process incoming messages.
        /// </summary>
        /// <remarks>
        /// This method limits the effort it does automatically using the private variable msDeltaForServiceCalls.
        /// That value is lower for connect and multiplied by 4 when chat-server connection is ready.
        /// </remarks>
        public void Service()
        {
            // Dispatch until every already-received message got dispatched
            while (this.HasPeer && this.chatPeer.DispatchIncomingCommands())
            {
            }

            // if there is no background thread for sending, Service() will do that as well, in intervals
            if (!this.UseBackgroundWorkerForSending)
            {
                if (Environment.TickCount - this.msTimestampOfLastServiceCall > this.msDeltaForServiceCalls || this.msTimestampOfLastServiceCall == 0)
                {
                    this.msTimestampOfLastServiceCall = Environment.TickCount;

                    while (this.HasPeer && this.chatPeer.SendOutgoingCommands())
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Called by a separate thread, this sends outgoing commands of this peer, as long as it's connected.
        /// </summary>
        /// <returns>True as long as the client is not disconnected.</returns>
        private bool SendOutgoingInBackground()
        {
            while (this.HasPeer && this.chatPeer.SendOutgoingCommands())
            {
            }

            return this.State != ChatState.Disconnected;
        }

        /// <summary> Obsolete: Better use UseBackgroundWorkerForSending and Service(). </summary>
        [Obsolete("Better use UseBackgroundWorkerForSending and Service().")]
        public void SendAcksOnly()
        {
            if (this.HasPeer) this.chatPeer.SendAcksOnly();
        }


        /// <summary>
        /// Disconnects from the Chat Server by sending a "disconnect command", which prevents a timeout server-side.
        /// </summary>
        public void Disconnect()
        {
            if (this.HasPeer && this.chatPeer.PeerState != PeerStateValue.Disconnected)
            {
                this.chatPeer.Disconnect();
            }
        }

        /// <summary>
        /// Locally shuts down the connection to the Chat Server. This resets states locally but the server will have to timeout this peer.
        /// </summary>
        public void StopThread()
        {
            if (this.HasPeer)
            {
                this.chatPeer.StopThread();
            }
        }

        /// <summary>Sends operation to subscribe to a list of channels by name.</summary>
        /// <param name="channels">List of channels to subscribe to. Avoid null or empty values.</param>
        /// <returns>If the operation could be sent at all (Example: Fails if not connected to Chat Server).</returns>
        public bool Subscribe(string[] channels)
        {
            return this.Subscribe(channels, 0);
        }

        /// <summary>
        /// Sends operation to subscribe to a list of channels by name and possibly retrieve messages we did not receive while unsubscribed.
        /// </summary>
        /// <param name="channels">List of channels to subscribe to. Avoid null or empty values.</param>
        /// <param name="lastMsgIds">ID of last message received per channel. Useful when re subscribing to receive only messages we missed.</param>
        /// <returns>If the operation could be sent at all (Example: Fails if not connected to Chat Server).</returns>
        public bool Subscribe(string[] channels, int[] lastMsgIds)
        {
            if (!this.CanChat)
            {
                if (this.DebugOut >= DebugLevel.ERROR)
                {
                    this.listener.DebugReturn(DebugLevel.ERROR, "Subscribe called while not connected to front end server.");
                }
                return false;
            }

            if (channels == null || channels.Length == 0)
            {
                if (this.DebugOut >= DebugLevel.WARNING)
                {
                    this.listener.DebugReturn(DebugLevel.WARNING, "Subscribe can't be called for empty or null channels-list.");
                }
                return false;
            }

            for (int i = 0; i < channels.Length; i++)
            {
                if (string.IsNullOrEmpty(channels[i]))
                {
                    if (this.DebugOut >= DebugLevel.ERROR)
                    {
                        this.listener.DebugReturn(DebugLevel.ERROR, string.Format("Subscribe can't be called with a null or empty channel name at index {0}.", i));
                    }
                    return false;
                }
            }

            if (lastMsgIds == null || lastMsgIds.Length != channels.Length)
            {
                if (this.DebugOut >= DebugLevel.ERROR)
                {
                    this.listener.DebugReturn(DebugLevel.ERROR, "Subscribe can't be called when \"lastMsgIds\" array is null or does not have the same length as \"channels\" array.");
                }
                return false;
            }

            Dictionary<byte, object> opParameters = new Dictionary<byte, object>
            {
                { ChatParameterCode.Channels, channels },
                { ChatParameterCode.MsgIds,  lastMsgIds},
                { ChatParameterCode.HistoryLength, -1 } // server will decide how many messages to send to client
            };

            return this.chatPeer.SendOperation(ChatOperationCode.Subscribe, opParameters, new SendOptions { Reliability = true });
        }

        /// <summary>
        /// Sends operation to subscribe client to channels, optionally fetching a number of messages from the cache.
        /// </summary>
        /// <remarks>
        /// Subscribes channels will forward new messages to this user. Use PublishMessage to do so.
        /// The messages cache is limited but can be useful to get into ongoing conversations, if that's needed.
        /// </remarks>
        /// <param name="channels">List of channels to subscribe to. Avoid null or empty values.</param>
        /// <param name="messagesFromHistory">0: no history. 1 and higher: number of messages in history. -1: all available history.</param>
        /// <returns>If the operation could be sent at all (Example: Fails if not connected to Chat Server).</returns>
        public bool Subscribe(string[] channels, int messagesFromHistory)
        {
            if (!this.CanChat)
            {
                if (this.DebugOut >= DebugLevel.ERROR)
                {
                    this.listener.DebugReturn(DebugLevel.ERROR, "Subscribe called while not connected to front end server.");
                }
                return false;
            }

            if (channels == null || channels.Length == 0)
            {
                if (this.DebugOut >= DebugLevel.WARNING)
                {
                    this.listener.DebugReturn(DebugLevel.WARNING, "Subscribe can't be called for empty or null channels-list.");
                }
                return false;
            }

            return this.SendChannelOperation(channels, (byte)ChatOperationCode.Subscribe, messagesFromHistory);
        }

        /// <summary>Unsubscribes from a list of channels, which stops getting messages from those.</summary>
        /// <remarks>
        /// The client will remove these channels from the PublicChannels dictionary once the server sent a response to this request.
        ///
        /// The request will be sent to the server and IChatClientListener.OnUnsubscribed gets called when the server
        /// actually removed the channel subscriptions.
        ///
        /// Unsubscribe will fail if you include null or empty channel names.
        /// </remarks>
        /// <param name="channels">Names of channels to unsubscribe.</param>
        /// <returns>False, if not connected to a chat server.</returns>
        public bool Unsubscribe(string[] channels)
        {
            if (!this.CanChat)
            {
                if (this.DebugOut >= DebugLevel.ERROR)
                {
                    this.listener.DebugReturn(DebugLevel.ERROR, "Unsubscribe called while not connected to front end server.");
                }
                return false;
            }

            if (channels == null || channels.Length == 0)
            {
                if (this.DebugOut >= DebugLevel.WARNING)
                {
                    this.listener.DebugReturn(DebugLevel.WARNING, "Unsubscribe can't be called for empty or null channels-list.");
                }
                return false;
            }

            foreach (string ch in channels)
            {
                this.PublicChannelsUnsubscribing.Add(ch);
            }
            return this.SendChannelOperation(channels, ChatOperationCode.Unsubscribe, 0);
        }

        /// <summary>Sends a message to a public channel which this client subscribed to.</summary>
        /// <remarks>
        /// Before you publish to a channel, you have to subscribe it.
        /// Everyone in that channel will get the message.
        /// </remarks>
        /// <param name="channelName">Name of the channel to publish to.</param>
        /// <param name="message">Your message (string or any serializable data).</param>
        /// <param name="forwardAsWebhook">Optionally, public messages can be forwarded as webhooks. Configure webhooks for your Chat app to use this.</param>
        /// <returns>False if the client is not yet ready to send messages.</returns>
        public bool PublishMessage(string channelName, object message, bool forwardAsWebhook = false)
        {
            return this.publishMessage(channelName, message, true, forwardAsWebhook);
        }

        internal bool PublishMessageUnreliable(string channelName, object message, bool forwardAsWebhook = false)
        {
            return this.publishMessage(channelName, message, false, forwardAsWebhook);
        }

        private bool publishMessage(string channelName, object message, bool reliable, bool forwardAsWebhook = false)
        {
            if (!this.CanChat)
            {
                if (this.DebugOut >= DebugLevel.ERROR)
                {
                    this.listener.DebugReturn(DebugLevel.ERROR, "PublishMessage called while not connected to front end server.");
                }
                return false;
            }

            if (string.IsNullOrEmpty(channelName) || message == null)
            {
                if (this.DebugOut >= DebugLevel.WARNING)
                {
                    this.listener.DebugReturn(DebugLevel.WARNING, "PublishMessage parameters must be non-null and not empty.");
                }
                return false;
            }

            Dictionary<byte, object> parameters = new Dictionary<byte, object>
                {
                    { (byte)ChatParameterCode.Channel, channelName },
                    { (byte)ChatParameterCode.Message, message }
                };
            if (forwardAsWebhook)
            {
                parameters.Add(ChatParameterCode.WebFlags, (byte)0x1);
            }

            return this.chatPeer.SendOperation(ChatOperationCode.Publish, parameters, new SendOptions() { Reliability = reliable });
        }

        /// <summary>
        /// Sends a private message to a single target user. Calls OnPrivateMessage on the receiving client.
        /// </summary>
        /// <param name="target">Username to send this message to.</param>
        /// <param name="message">The message you want to send. Can be a simple string or anything serializable.</param>
        /// <param name="forwardAsWebhook">Optionally, private messages can be forwarded as webhooks. Configure webhooks for your Chat app to use this.</param>
        /// <returns>True if this clients can send the message to the server.</returns>
        public bool SendPrivateMessage(string target, object message, bool forwardAsWebhook = false)
        {
            return this.SendPrivateMessage(target, message, false, forwardAsWebhook);
        }

        /// <summary>
        /// Sends a private message to a single target user. Calls OnPrivateMessage on the receiving client.
        /// </summary>
        /// <param name="target">Username to send this message to.</param>
        /// <param name="message">The message you want to send. Can be a simple string or anything serializable.</param>
        /// <param name="encrypt">Optionally, private messages can be encrypted. Encryption is not end-to-end as the server decrypts the message.</param>
        /// <param name="forwardAsWebhook">Optionally, private messages can be forwarded as webhooks. Configure webhooks for your Chat app to use this.</param>
        /// <returns>True if this clients can send the message to the server.</returns>
        public bool SendPrivateMessage(string target, object message, bool encrypt, bool forwardAsWebhook)
        {
            return this.sendPrivateMessage(target, message, encrypt, true, forwardAsWebhook);
        }

        internal bool SendPrivateMessageUnreliable(string target, object message, bool encrypt, bool forwardAsWebhook = false)
        {
            return this.sendPrivateMessage(target, message, encrypt, false, forwardAsWebhook);
        }

        private bool sendPrivateMessage(string target, object message, bool encrypt, bool reliable, bool forwardAsWebhook = false)
        {
            if (!this.CanChat)
            {
                if (this.DebugOut >= DebugLevel.ERROR)
                {
                    this.listener.DebugReturn(DebugLevel.ERROR, "SendPrivateMessage called while not connected to front end server.");
                }
                return false;
            }

            if (string.IsNullOrEmpty(target) || message == null)
            {
                if (this.DebugOut >= DebugLevel.WARNING)
                {
                    this.listener.DebugReturn(DebugLevel.WARNING, "SendPrivateMessage parameters must be non-null and not empty.");
                }
                return false;
            }

            Dictionary<byte, object> parameters = new Dictionary<byte, object>
                {
                    { ChatParameterCode.UserId, target },
                    { ChatParameterCode.Message, message }
                };
            if (forwardAsWebhook)
            {
                parameters.Add(ChatParameterCode.WebFlags, (byte)0x1);
            }

            return this.chatPeer.SendOperation(ChatOperationCode.SendPrivate, parameters, new SendOptions() { Reliability = reliable, Encrypt = encrypt });
        }

        /// <summary>Sets the user's status (pre-defined or custom) and an optional message.</summary>
        /// <remarks>
        /// The predefined status values can be found in class ChatUserStatus.
        /// State ChatUserStatus.Invisible will make you offline for everyone and send no message.
        ///
        /// You can set custom values in the status integer. Aside from the pre-configured ones,
        /// all states will be considered visible and online. Else, no one would see the custom state.
        ///
        /// The message object can be anything that Photon can serialize, including (but not limited to)
        /// Hashtable, object[] and string. This value is defined by your own conventions.
        /// </remarks>
        /// <param name="status">Predefined states are in class ChatUserStatus. Other values can be used at will.</param>
        /// <param name="message">Optional string message or null.</param>
        /// <param name="skipMessage">If true, the message gets ignored. It can be null but won't replace any current message.</param>
        /// <returns>True if the operation gets called on the server.</returns>
        private bool SetOnlineStatus(int status, object message, bool skipMessage)
        {
            if (!this.CanChat)
            {
                if (this.DebugOut >= DebugLevel.ERROR)
                {
                    this.listener.DebugReturn(DebugLevel.ERROR, "SetOnlineStatus called while not connected to front end server.");
                }
                return false;
            }

            Dictionary<byte, object> parameters = new Dictionary<byte, object>
                {
                    { ChatParameterCode.Status, status },
                };

            if (skipMessage)
            {
                parameters[ChatParameterCode.SkipMessage] = true;
            }
            else
            {
                parameters[ChatParameterCode.Message] = message;
            }

            return this.chatPeer.SendOperation(ChatOperationCode.UpdateStatus, parameters, new SendOptions() { Reliability = true });
        }

        /// <summary>Sets the user's status without changing your status-message.</summary>
        /// <remarks>
        /// The predefined status values can be found in class ChatUserStatus.
        /// State ChatUserStatus.Invisible will make you offline for everyone and send no message.
        ///
        /// You can set custom values in the status integer. Aside from the pre-configured ones,
        /// all states will be considered visible and online. Else, no one would see the custom state.
        ///
        /// This overload does not change the set message.
        /// </remarks>
        /// <param name="status">Predefined states are in class ChatUserStatus. Other values can be used at will.</param>
        /// <returns>True if the operation gets called on the server.</returns>
        public bool SetOnlineStatus(int status)
        {
            return this.SetOnlineStatus(status, null, true);
        }

        /// <summary>Sets the user's status without changing your status-message.</summary>
        /// <remarks>
        /// The predefined status values can be found in class ChatUserStatus.
        /// State ChatUserStatus.Invisible will make you offline for everyone and send no message.
        ///
        /// You can set custom values in the status integer. Aside from the pre-configured ones,
        /// all states will be considered visible and online. Else, no one would see the custom state.
        ///
        /// The message object can be anything that Photon can serialize, including (but not limited to)
        /// Hashtable, object[] and string. This value is defined by your own conventions.
        /// </remarks>
        /// <param name="status">Predefined states are in class ChatUserStatus. Other values can be used at will.</param>
        /// <param name="message">Also sets a status-message which your friends can get.</param>
        /// <returns>True if the operation gets called on the server.</returns>
        public bool SetOnlineStatus(int status, object message)
        {
            return this.SetOnlineStatus(status, message, false);
        }

        /// <summary>
        /// Adds friends to a list on the Chat Server which will send you status updates for those.
        /// </summary>
        /// <remarks>
        /// AddFriends and RemoveFriends enable clients to handle their friend list
        /// in the Photon Chat server. Having users on your friends list gives you access
        /// to their current online status (and whatever info your client sets in it).
        ///
        /// Each user can set an online status consisting of an integer and an arbitratry
        /// (serializable) object. The object can be null, Hashtable, object[] or anything
        /// else Photon can serialize.
        ///
        /// The status is published automatically to friends (anyone who set your user ID
        /// with AddFriends).
        ///
        /// Photon flushes friends-list when a chat client disconnects, so it has to be
        /// set each time. If your community API gives you access to online status already,
        /// you could filter and set online friends in AddFriends.
        ///
        /// Actual friend relations are not persistent and have to be stored outside
        /// of Photon.
        /// </remarks>
        /// <param name="friends">Array of friend userIds.</param>
        /// <returns>If the operation could be sent.</returns>
        public bool AddFriends(string[] friends)
        {
            if (!this.CanChat)
            {
                if (this.DebugOut >= DebugLevel.ERROR)
                {
                    this.listener.DebugReturn(DebugLevel.ERROR, "AddFriends called while not connected to front end server.");
                }
                return false;
            }

            if (friends == null || friends.Length == 0)
            {
                if (this.DebugOut >= DebugLevel.WARNING)
                {
                    this.listener.DebugReturn(DebugLevel.WARNING, "AddFriends can't be called for empty or null list.");
                }
                return false;
            }
            if (friends.Length > FriendRequestListMax)
            {
                if (this.DebugOut >= DebugLevel.WARNING)
                {
                    this.listener.DebugReturn(DebugLevel.WARNING, "AddFriends max list size exceeded: " + friends.Length + " > " + FriendRequestListMax);
                }
                return false;
            }

            Dictionary<byte, object> parameters = new Dictionary<byte, object>
                {
                    { ChatParameterCode.Friends, friends },
                };

            return this.chatPeer.SendOperation(ChatOperationCode.AddFriends, parameters, new SendOptions() { Reliability = true });
        }

        /// <summary>
        /// Removes the provided entries from the list on the Chat Server and stops their status updates.
        /// </summary>
        /// <remarks>
        /// Photon flushes friends-list when a chat client disconnects. Unless you want to
        /// remove individual entries, you don't have to RemoveFriends.
        ///
        /// AddFriends and RemoveFriends enable clients to handle their friend list
        /// in the Photon Chat server. Having users on your friends list gives you access
        /// to their current online status (and whatever info your client sets in it).
        ///
        /// Each user can set an online status consisting of an integer and an arbitratry
        /// (serializable) object. The object can be null, Hashtable, object[] or anything
        /// else Photon can serialize.
        ///
        /// The status is published automatically to friends (anyone who set your user ID
        /// with AddFriends).
        ///
        /// Photon flushes friends-list when a chat client disconnects, so it has to be
        /// set each time. If your community API gives you access to online status already,
        /// you could filter and set online friends in AddFriends.
        ///
        /// Actual friend relations are not persistent and have to be stored outside
        /// of Photon.
        ///
        /// AddFriends and RemoveFriends enable clients to handle their friend list
        /// in the Photon Chat server. Having users on your friends list gives you access
        /// to their current online status (and whatever info your client sets in it).
        ///
        /// Each user can set an online status consisting of an integer and an arbitratry
        /// (serializable) object. The object can be null, Hashtable, object[] or anything
        /// else Photon can serialize.
        ///
        /// The status is published automatically to friends (anyone who set your user ID
        /// with AddFriends).
        ///
        ///
        /// Actual friend relations are not persistent and have to be stored outside
        /// of Photon.
        /// </remarks>
        /// <param name="friends">Array of friend userIds.</param>
        /// <returns>If the operation could be sent.</returns>
        public bool RemoveFriends(string[] friends)
        {
            if (!this.CanChat)
            {
                if (this.DebugOut >= DebugLevel.ERROR)
                {
                    this.listener.DebugReturn(DebugLevel.ERROR, "RemoveFriends called while not connected to front end server.");
                }
                return false;
            }

            if (friends == null || friends.Length == 0)
            {
                if (this.DebugOut >= DebugLevel.WARNING)
                {
                    this.listener.DebugReturn(DebugLevel.WARNING, "RemoveFriends can't be called for empty or null list.");
                }
                return false;
            }
            if (friends.Length > FriendRequestListMax)
            {
                if (this.DebugOut >= DebugLevel.WARNING)
                {
                    this.listener.DebugReturn(DebugLevel.WARNING, "RemoveFriends max list size exceeded: " + friends.Length + " > " + FriendRequestListMax);
                }
                return false;
            }

            Dictionary<byte, object> parameters = new Dictionary<byte, object>
                {
                    { ChatParameterCode.Friends, friends },
                };

            return this.chatPeer.SendOperation(ChatOperationCode.RemoveFriends, parameters, new SendOptions() { Reliability = true });
        }

        /// <summary>
        /// Get you the (locally used) channel name for the chat between this client and another user.
        /// </summary>
        /// <param name="userName">Remote user's name or UserId.</param>
        /// <returns>The (locally used) channel name for a private channel.</returns>
        public string GetPrivateChannelNameByUser(string userName)
        {
            return string.Format("{0}:{1}", this.UserId, userName);
        }

        /// <summary>
        /// Simplified access to either private or public channels by name.
        /// </summary>
        /// <param name="channelName">Name of the channel to get. For private channels, the channel-name is composed of both user's names.</param>
        /// <param name="isPrivate">Define if you expect a private or public channel.</param>
        /// <param name="channel">Out parameter gives you the found channel, if any.</param>
        /// <returns>True if the channel was found.</returns>
        public bool TryGetChannel(string channelName, bool isPrivate, out ChatChannel channel)
        {
            if (!isPrivate)
            {
                return this.PublicChannels.TryGetValue(channelName, out channel);
            }
            else
            {
                return this.PrivateChannels.TryGetValue(channelName, out channel);
            }
        }

        /// <summary>
        /// Simplified access to all channels by name. Checks public channels first, then private ones.
        /// </summary>
        /// <param name="channelName">Name of the channel to get.</param>
        /// <param name="channel">Out parameter gives you the found channel, if any.</param>
        /// <returns>True if the channel was found.</returns>
        public bool TryGetChannel(string channelName, out ChatChannel channel)
        {
            bool found = false;
            found = this.PublicChannels.TryGetValue(channelName, out channel);
            if (found) return true;

            found = this.PrivateChannels.TryGetValue(channelName, out channel);
            return found;
        }

        /// <summary>
        /// Sets the level (and amount) of debug output provided by the library.
        /// </summary>
        /// <remarks>
        /// This affects the callbacks to IChatClientListener.DebugReturn.
        /// Default Level: Error.
        /// </remarks>
        public DebugLevel DebugOut
        {
            set { this.chatPeer.DebugOut = value; }
            get { return this.chatPeer.DebugOut; }
        }

        #region Private methods area

        #region IPhotonPeerListener implementation

        void IPhotonPeerListener.DebugReturn(DebugLevel level, string message)
        {
            this.listener.DebugReturn(level, message);
        }

        void IPhotonPeerListener.OnEvent(EventData eventData)
        {
            switch (eventData.Code)
            {
                case ChatEventCode.ChatMessages:
                    this.HandleChatMessagesEvent(eventData);
                    break;
                case ChatEventCode.PrivateMessage:
                    this.HandlePrivateMessageEvent(eventData);
                    break;
                case ChatEventCode.StatusUpdate:
                    this.HandleStatusUpdate(eventData);
                    break;
                case ChatEventCode.Subscribe:
                    this.HandleSubscribeEvent(eventData);
                    break;
                case ChatEventCode.Unsubscribe:
                    this.HandleUnsubscribeEvent(eventData);
                    break;
            }
        }

        void IPhotonPeerListener.OnOperationResponse(OperationResponse operationResponse)
        {
            switch (operationResponse.OperationCode)
            {
                case (byte)ChatOperationCode.Authenticate:
                    this.HandleAuthResponse(operationResponse);
                    break;

                // the following operations usually don't return useful data and no error.
                case (byte)ChatOperationCode.Subscribe:
                case (byte)ChatOperationCode.Unsubscribe:
                case (byte)ChatOperationCode.Publish:
                case (byte)ChatOperationCode.SendPrivate:
                default:
                    if ((operationResponse.ReturnCode != 0) && (this.DebugOut >= DebugLevel.ERROR))
                    {
                        if (operationResponse.ReturnCode == -2)
                        {
                            this.listener.DebugReturn(DebugLevel.ERROR, string.Format("Chat Operation {0} unknown on server. Check your AppId and make sure it's for a Chat application.", operationResponse.OperationCode));
                        }
                        else
                        {
                            this.listener.DebugReturn(DebugLevel.ERROR, string.Format("Chat Operation {0} failed (Code: {1}). Debug Message: {2}", operationResponse.OperationCode, operationResponse.ReturnCode, operationResponse.DebugMessage));
                        }
                    }
                    break;
            }
        }

        void IPhotonPeerListener.OnStatusChanged(StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.Connect:
                    if (!this.chatPeer.IsProtocolSecure)
                    {
                        this.chatPeer.EstablishEncryption();
                    }
                    else
                    {
                        if (!this.didAuthenticate)
                        {
                            this.didAuthenticate = this.chatPeer.AuthenticateOnNameServer(this.AppId, this.AppVersion, this.chatRegion, this.AuthValues);
                            if (!this.didAuthenticate)
                            {
                                if (this.DebugOut >= DebugLevel.ERROR)
                                {
                                    ((IPhotonPeerListener)this).DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + this.State);
                                }
                            }
                        }
                    }

                    if (this.State == ChatState.ConnectingToNameServer)
                    {
                        this.State = ChatState.ConnectedToNameServer;
                        this.listener.OnChatStateChange(this.State);
                    }
                    else if (this.State == ChatState.ConnectingToFrontEnd)
                    {
                        this.AuthenticateOnFrontEnd();
                    }
                    break;
                case StatusCode.EncryptionEstablished:
                    // once encryption is availble, the client should send one (secure) authenticate. it includes the AppId (which identifies your app on the Photon Cloud)
                    if (!this.didAuthenticate)
                    {
                        this.didAuthenticate = this.chatPeer.AuthenticateOnNameServer(this.AppId, this.AppVersion, this.chatRegion, this.AuthValues);
                        if (!this.didAuthenticate)
                        {
                            if (this.DebugOut >= DebugLevel.ERROR)
                            {
                                ((IPhotonPeerListener)this).DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + this.State);
                            }
                        }
                    }
                    break;
                case StatusCode.EncryptionFailedToEstablish:
                    this.State = ChatState.Disconnecting;
                    this.chatPeer.Disconnect();
                    break;
                case StatusCode.Disconnect:
                    if (this.State == ChatState.Authenticated)
                    {
                        this.ConnectToFrontEnd();
                    }
                    else
                    {
                        this.State = ChatState.Disconnected;
                        this.listener.OnChatStateChange(ChatState.Disconnected);
                        this.listener.OnDisconnected();
                    }
                    break;
            }
        }

        #if SDK_V4
        void IPhotonPeerListener.OnMessage(object msg)
        {
            // in v4 interface IPhotonPeerListener
            return;
        }
        #endif

        #endregion

        private bool SendChannelOperation(string[] channels, byte operation, int historyLength)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object> { { (byte)ChatParameterCode.Channels, channels } };

            if (historyLength != 0)
            {
                opParameters.Add((byte)ChatParameterCode.HistoryLength, historyLength);
            }

            return this.chatPeer.SendOperation(operation, opParameters, new SendOptions() { Reliability = true });
        }

        private void HandlePrivateMessageEvent(EventData eventData)
        {
            //Console.WriteLine(SupportClass.DictionaryToString(eventData.Parameters));

            object message = (object)eventData.Parameters[(byte)ChatParameterCode.Message];
            string sender = (string)eventData.Parameters[(byte)ChatParameterCode.Sender];
            int msgId = (int)eventData.Parameters[ChatParameterCode.MsgId];

            string channelName;
            if (this.UserId != null && this.UserId.Equals(sender))
            {
                string target = (string)eventData.Parameters[(byte)ChatParameterCode.UserId];
                channelName = this.GetPrivateChannelNameByUser(target);
            }
            else
            {
                channelName = this.GetPrivateChannelNameByUser(sender);
            }

            ChatChannel channel;
            if (!this.PrivateChannels.TryGetValue(channelName, out channel))
            {
                channel = new ChatChannel(channelName);
                channel.IsPrivate = true;
                channel.MessageLimit = this.MessageLimit;
                this.PrivateChannels.Add(channel.Name, channel);
            }

            channel.Add(sender, message, msgId);
            this.listener.OnPrivateMessage(sender, message, channelName);
        }

        private void HandleChatMessagesEvent(EventData eventData)
        {
            object[] messages = (object[])eventData.Parameters[(byte)ChatParameterCode.Messages];
            string[] senders = (string[])eventData.Parameters[(byte)ChatParameterCode.Senders];
            string channelName = (string)eventData.Parameters[(byte)ChatParameterCode.Channel];
            int lastMsgId = (int)eventData.Parameters[ChatParameterCode.MsgId];

            ChatChannel channel;
            if (!this.PublicChannels.TryGetValue(channelName, out channel))
            {
                if (this.DebugOut >= DebugLevel.WARNING)
                {
                    this.listener.DebugReturn(DebugLevel.WARNING, "Channel " + channelName + " for incoming message event not found.");
                }
                return;
            }

            channel.Add(senders, messages, lastMsgId);
            this.listener.OnGetMessages(channelName, senders, messages);
        }

        private void HandleSubscribeEvent(EventData eventData)
        {
            string[] channelsInResponse = (string[])eventData.Parameters[ChatParameterCode.Channels];
            bool[] results = (bool[])eventData.Parameters[ChatParameterCode.SubscribeResults];

            for (int i = 0; i < channelsInResponse.Length; i++)
            {
                if (results[i])
                {
                    string channelName = channelsInResponse[i];
                    if (!this.PublicChannels.ContainsKey(channelName))
                    {
                        ChatChannel channel = new ChatChannel(channelName);
                        channel.MessageLimit = this.MessageLimit;
                        this.PublicChannels.Add(channel.Name, channel);
                    }
                }
            }

            this.listener.OnSubscribed(channelsInResponse, results);
        }

        private void HandleUnsubscribeEvent(EventData eventData)
        {
            string[] channelsInRequest = (string[])eventData[ChatParameterCode.Channels];
            for (int i = 0; i < channelsInRequest.Length; i++)
            {
                string channelName = channelsInRequest[i];
                this.PublicChannels.Remove(channelName);
                this.PublicChannelsUnsubscribing.Remove(channelName);
            }

            this.listener.OnUnsubscribed(channelsInRequest);
        }

        private void HandleAuthResponse(OperationResponse operationResponse)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.listener.DebugReturn(DebugLevel.INFO, operationResponse.ToStringFull() + " on: " + this.chatPeer.NameServerAddress);
            }

            if (operationResponse.ReturnCode == 0)
            {
                if (this.State == ChatState.ConnectedToNameServer)
                {
                    this.State = ChatState.Authenticated;
                    this.listener.OnChatStateChange(this.State);

                    if (operationResponse.Parameters.ContainsKey(ParameterCode.Secret))
                    {
                        if (this.AuthValues == null)
                        {
                            this.AuthValues = new AuthenticationValues();
                        }
                        this.AuthValues.Token = operationResponse[ParameterCode.Secret] as string;

                        this.FrontendAddress = (string)operationResponse[ParameterCode.Address];

                        // we disconnect and status handler starts to connect to front end
                        this.chatPeer.Disconnect();
                    }
                    else
                    {
                        if (this.DebugOut >= DebugLevel.ERROR)
                        {
                            this.listener.DebugReturn(DebugLevel.ERROR, "No secret in authentication response.");
                        }
                    }
                    if (operationResponse.Parameters.ContainsKey(ParameterCode.UserId))
                    {
                        string incomingId = operationResponse.Parameters[ParameterCode.UserId] as string;
                        if (!string.IsNullOrEmpty(incomingId))
                        {
                            this.UserId = incomingId;
                            this.listener.DebugReturn(DebugLevel.INFO, string.Format("Received your UserID from server. Updating local value to: {0}", this.UserId));
                        }
                    }
                }
                else if (this.State == ChatState.ConnectingToFrontEnd)
                {
                    this.State = ChatState.ConnectedToFrontEnd;
                    this.listener.OnChatStateChange(this.State);
                    this.listener.OnConnected();
                    if (statusToSetWhenConnected.HasValue)
                    {
                        SetOnlineStatus(statusToSetWhenConnected.Value, messageToSetWhenConnected);
                        statusToSetWhenConnected = null;
                    }
                }
            }
            else
            {
                //this.listener.DebugReturn(DebugLevel.INFO, operationResponse.ToStringFull() + " NS: " + this.NameServerAddress + " FrontEnd: " + this.frontEndAddress);

                switch (operationResponse.ReturnCode)
                {
                    case ErrorCode.InvalidAuthentication:
                        this.DisconnectedCause = ChatDisconnectCause.InvalidAuthentication;
                        break;
                    case ErrorCode.CustomAuthenticationFailed:
                        this.DisconnectedCause = ChatDisconnectCause.CustomAuthenticationFailed;
                        break;
                    case ErrorCode.InvalidRegion:
                        this.DisconnectedCause = ChatDisconnectCause.InvalidRegion;
                        break;
                    case ErrorCode.MaxCcuReached:
                        this.DisconnectedCause = ChatDisconnectCause.MaxCcuReached;
                        break;
                    case ErrorCode.OperationNotAllowedInCurrentState:
                        this.DisconnectedCause = ChatDisconnectCause.OperationNotAllowedInCurrentState;
                        break;
                }

                if (this.DebugOut >= DebugLevel.ERROR)
                {
                    this.listener.DebugReturn(DebugLevel.ERROR, "Authentication request error: " + operationResponse.ReturnCode + ". Disconnecting.");
                }


                this.State = ChatState.Disconnecting;
                this.chatPeer.Disconnect();
            }
        }

        private void HandleStatusUpdate(EventData eventData)
        {
            string user = (string)eventData.Parameters[ChatParameterCode.Sender];
            int status = (int)eventData.Parameters[ChatParameterCode.Status];

            object message = null;
            bool gotMessage = eventData.Parameters.ContainsKey(ChatParameterCode.Message);
            if (gotMessage)
            {
                message = eventData.Parameters[ChatParameterCode.Message];
            }

            this.listener.OnStatusUpdate(user, status, gotMessage, message);
        }

        private void ConnectToFrontEnd()
        {
            this.State = ChatState.ConnectingToFrontEnd;

            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.listener.DebugReturn(DebugLevel.INFO, "Connecting to frontend " + this.FrontendAddress);
            }

            #if UNITY_WEBGL
            if (this.TransportProtocol == ConnectionProtocol.Tcp || this.TransportProtocol == ConnectionProtocol.Udp)
            {
                this.listener.DebugReturn(DebugLevel.WARNING, "WebGL requires WebSockets. Switching TransportProtocol to WebSocketSecure.");
                this.TransportProtocol = ConnectionProtocol.WebSocketSecure;
            }
            #endif

            this.chatPeer.Connect(this.FrontendAddress, ChatAppName);
        }

        private bool AuthenticateOnFrontEnd()
        {
            if (this.AuthValues != null)
            {
                if (string.IsNullOrEmpty(AuthValues.Token))
                {
                    if (this.DebugOut >= DebugLevel.ERROR)
                    {
                        this.listener.DebugReturn(DebugLevel.ERROR, "Can't authenticate on front end server. Secret is not set");
                    }
                    return false;
                }
                else
                {
                    Dictionary<byte, object> opParameters = new Dictionary<byte, object> { { (byte)ChatParameterCode.Secret, this.AuthValues.Token } };
                    return this.chatPeer.SendOperation(ChatOperationCode.Authenticate, opParameters, new SendOptions() { Reliability = true });
                }
            }
            else
            {
                if (this.DebugOut >= DebugLevel.ERROR)
                {
                    this.listener.DebugReturn(DebugLevel.ERROR, "Can't authenticate on front end server. Authentication Values are not set");
                }
                return false;
            }
        }

        #endregion
    }
}
