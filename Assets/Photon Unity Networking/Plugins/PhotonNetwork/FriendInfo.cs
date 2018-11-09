// ----------------------------------------------------------------------------
// <copyright file="FriendInfo.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2013 Exit Games GmbH
// </copyright>
// <summary>
//   Collection of values related to a user / friend.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------


/// <summary>
/// Used to store info about a friend's online state and in which room he/she is.
/// </summary>
public class FriendInfo
{
    [System.Obsolete("Use UserId.")]
    public string Name { get { return this.UserId; } }
    public string UserId { get; internal protected set; }

    public bool IsOnline { get; internal protected set; }
    public string Room { get; internal protected set; }
    public bool IsInRoom { get { return IsOnline && !string.IsNullOrEmpty(this.Room); } }

    public override string ToString()
    {
        return string.Format("{0}\t is: {1}", this.UserId, (!this.IsOnline) ? "offline" : this.IsInRoom ? "playing" : "on master");
    }
}
