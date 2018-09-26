// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InRoomtime.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


//#define GUI_ENABLED


using System.Collections;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// This component establishes a common, shared room-start-time and RoomTime and RoomTimestamp. Both go up without wrapping around (for ~48 days).
/// </summary>
/// <remarks>
/// When entering a new room, this script will take a moment to establish the start timestamp. While this is done, all values are 0.
/// Uses a Custom Property in a room to sync a start time for a multiplayer game.
/// 
/// The internally used roomStartTimestamp is only valid in "this" room and only on the one Game Server where
/// it was established initially. This means: <b>This is not useful for asynchronous gameplay!</b>
/// </remarks>
public class InRoomTime : MonoBehaviour
{
    private int roomStartTimestamp;
    private const string StartTimeKey = "#rt"; // the name of our "room time" custom property.


    /// <summary>A common, synced timer as double (similar to Unity's Time.time) for a room, starting close to 0 and not wrapping around.</summary>
    /// <remarks>When IsoomTimeSet is false, RoomTimestamp and RoomTime will both be zero.</remarks>
    public double RoomTime
    {
        get
        {
            uint u = (uint)this.RoomTimestamp;
            double t = u;
            return t/1000;
        }
    }

    /// <summary>A common, synced timer for a room, starting close to 0 and not wrapping around.</summary>
    /// <remarks>When IsoomTimeSet is false, RoomTimestamp and RoomTime will both be zero.</remarks>
    public int RoomTimestamp
    {
        get { return PhotonNetwork.inRoom ? PhotonNetwork.ServerTimestamp - this.roomStartTimestamp : 0; }
    }

    /// <summary>True if the client is in a room and if that room's start time is defined (by any player).</summary>
    /// <remarks>When IsoomTimeSet is false, RoomTimestamp and RoomTime will both be zero.</remarks>
    public bool IsRoomTimeSet
    {
        get { return PhotonNetwork.inRoom && PhotonNetwork.room.customProperties.ContainsKey(StartTimeKey); }
    }



    internal IEnumerator SetRoomStartTimestamp()
    {
        //Debug.Log("SetRoomStartTimestamp() IsRoomTimeSet: " + IsRoomTimeSet + " PhotonNetwork.isMasterClient: " + PhotonNetwork.isMasterClient);
        if (IsRoomTimeSet || !PhotonNetwork.isMasterClient)
        {
            //Debug.Log("Not setting time.");
            yield break;
        }


        // in some cases, when you enter a room, the server time is not available immediately.
        if (PhotonNetwork.ServerTimestamp == 0)
        {
            yield return 0;
        }

        ExitGames.Client.Photon.Hashtable startTimeProp = new Hashtable(); // only use ExitGames.Client.Photon.Hashtable for Photon
        startTimeProp[StartTimeKey] = PhotonNetwork.ServerTimestamp;

        //Debug.Log("Setting roomStartTimestamp property to: " + startTimeProp[StartTimeKey]);
        PhotonNetwork.room.SetCustomProperties(startTimeProp); // implement OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged) to get this change everywhere
    }


    /// <summary>Called by PUN when this client entered a room (no matter if joined or created).</summary>
    public void OnJoinedRoom()
    {
        StartCoroutine("SetRoomStartTimestamp");
    }

    /// <remarks>
    /// In theory, the client which created the room might crash/close before it sets the start time.
    /// Just to make extremely sure this never happens, a new masterClient will check if it has to
    /// start a new round.
    /// </remarks>
    public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        StartCoroutine("SetRoomStartTimestamp");
    }

    /// <summary>Called by PUN when new properties for the room were set (by any client in the room).</summary>
    public void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(StartTimeKey))
        {
            this.roomStartTimestamp = (int)propertiesThatChanged[StartTimeKey];
            //Debug.Log("Got prop for roomStartTimestamp: " + roomStartTimestamp);
        }
    }


#if GUI_ENABLED

    public Rect TextPos = new Rect(0, 150, 200, 300); // default gui position. inspector overrides this!

    public void OnGUI()
    {
        // simple gui for output
        GUILayout.BeginArea(TextPos);
        GUILayout.Label(string.Format("RoomTime: {0:0.000}", RoomTime));
        GUILayout.Label(string.Format("RoomTimestamp: {0:0.000}", RoomTimestamp));
        GUILayout.EndArea();
    }
#endif
}
