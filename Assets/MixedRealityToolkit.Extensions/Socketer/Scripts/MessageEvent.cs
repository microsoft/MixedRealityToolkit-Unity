// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer
{
    public class MessageEvent
    {
        /// <summary>
        /// The host that this message came from.  In some unusual situations, such as when crossing network boundaries using NAT, this value may not be reliable.
        /// An example is when one client is on corpnet wireless and the other is on corpnet wired.  In these situations, it may be better to either use TCP
        /// (where you might not care who exactly a message is from, since a TCP Listener can send to all automatically), or to send your own IP (using GetLocalIPAddress)
        /// as a message and have the Lister create a new Sender to reply using that value.
        /// </summary>
        public string SourceHost;
        /// <summary>
        /// The message, as bytes.
        /// </summary>
        public byte[] Message;
        /// <summary>
        /// The ID of the client who sent the message
        /// </summary>
        public int SourceId;
        /// <summary>
        /// The message, as a string.
        /// </summary>
        /// <returns></returns>
        public string GetMessageString()
        {
            return System.Text.Encoding.UTF8.GetString(Message);
        }
    }
}
