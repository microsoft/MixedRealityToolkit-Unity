// ----------------------------------------------------------------------------------------------------------------------
// <summary>The Photon Chat Api enables clients to connect to a chat server and communicate with other clients.</summary>
// <remarks>ChatClient is the main class of this api.</remarks>
// <copyright company="Exit Games GmbH">Photon Chat Api - Copyright (C) 2014 Exit Games GmbH</copyright>
// ----------------------------------------------------------------------------------------------------------------------

namespace Photon.Chat
{
    /// <summary>
    /// Wraps up internally used constants in Photon Chat events. You don't have to use them directly usually.
    /// </summary>
    public class ChatEventCode
    {
        /// <summary>(0) Event code for messages published in public channels.</summary>
        public const byte ChatMessages = 0;
        /// <summary>(1) Not Used. </summary>
        public const byte Users = 1;// List of users or List of changes for List of users
        /// <summary>(2) Event code for messages published in private channels</summary>
        public const byte PrivateMessage = 2;
        /// <summary>(3) Not Used. </summary>
        public const byte FriendsList = 3;
        /// <summary>(4) Event code for status updates. </summary>
        public const byte StatusUpdate = 4;
        /// <summary>(5) Event code for subscription acks. </summary>
        public const byte Subscribe = 5;
        /// <summary>(6) Event code for unsubscribe acks. </summary>
        public const byte Unsubscribe = 6;
    }
}
