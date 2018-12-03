// ----------------------------------------------------------------------------------------------------------------------
// <summary>The Photon Chat Api enables clients to connect to a chat server and communicate with other clients.</summary>
// <remarks>ChatClient is the main class of this api.</remarks>
// <copyright company="Exit Games GmbH">Photon Chat Api - Copyright (C) 2014 Exit Games GmbH</copyright>
// ----------------------------------------------------------------------------------------------------------------------

namespace Photon.Chat
{
    /// <summary>
    /// Wraps up codes for parameters (in operations and events) used internally in Photon Chat. You don't have to use them directly usually.
    /// </summary>
    public class ChatParameterCode
    {
        /// <summary>(0) Array of chat channels.</summary>
        public const byte Channels = 0;
        /// <summary>(1) Name of a single chat channel.</summary>
        public const byte Channel = 1;
        /// <summary>(2) Array of chat messages.</summary>
        public const byte Messages = 2;
        /// <summary>(3) A single chat message.</summary>
        public const byte Message = 3;
        /// <summary>(4) Array of names of the users who sent the array of chat mesages.</summary>
        public const byte Senders = 4;
        /// <summary>(5) Name of a the user who sent a chat message.</summary>
        public const byte Sender = 5;
        /// <summary>(6) Not used.</summary>
        public const byte ChannelUserCount = 6;
        /// <summary>(225) Name of user to send a (private) message to.</summary><remarks>The code is used in LoadBalancing and copied over here.</remarks>
        public const byte UserId = 225;
        /// <summary>(8) Id of a message.</summary>
        public const byte MsgId = 8;
        /// <summary>(9) Not used.</summary>
        public const byte MsgIds = 9;
        /// <summary>(221) Secret token to identify an authorized user.</summary><remarks>The code is used in LoadBalancing and copied over here.</remarks>
        public const byte Secret = 221;
        /// <summary>(15) Subscribe operation result parameter. A bool[] with result per channel.</summary>
        public const byte SubscribeResults = 15;

        /// <summary>(10) Status</summary>
        public const byte Status = 10;
        /// <summary>(11) Friends</summary>
        public const byte Friends = 11;
        /// <summary>(12) SkipMessage is used in SetOnlineStatus and if true, the message is not being broadcast.</summary>
        public const byte SkipMessage = 12;

        /// <summary>(14) Number of message to fetch from history. 0: no history. 1 and higher: number of messages in history. -1: all history.</summary>
        public const byte HistoryLength = 14;

        /// <summary>(21) WebFlags object for changing behaviour of webhooks from client.</summary>
        public const byte WebFlags = 21;
    }
}