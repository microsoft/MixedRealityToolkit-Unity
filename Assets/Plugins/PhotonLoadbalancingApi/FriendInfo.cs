// ----------------------------------------------------------------------------
// <copyright file="FriendInfo.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2013 Exit Games GmbH
// </copyright>
// <summary>
//   Collection of values related to a user / friend.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_0 || UNITY_5_1 || UNITY_6_0
#define UNITY
#endif

namespace ExitGames.Client.Photon.LoadBalancing
{
    //#if UNITY_EDITOR || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    //#endif


    /// <summary>
    /// Used to store info about a friend's online state and in which room he/she is.
    /// </summary>
    public class FriendInfo
    {
        public string Name { get; internal protected set; }
        public bool IsOnline { get; internal protected set; }
        public string Room { get; internal protected set; }
        public bool IsInRoom { get { return IsOnline && !string.IsNullOrEmpty(this.Room); } }

        public override string ToString()
        {
        return string.Format("{0}\t is: {1}", this.Name, (!this.IsOnline) ? "offline" : this.IsInRoom ? "playing" : "on master");
        }
    }
}
