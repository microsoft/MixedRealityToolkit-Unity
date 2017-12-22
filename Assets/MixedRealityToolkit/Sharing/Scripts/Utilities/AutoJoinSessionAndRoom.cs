// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using System;
using System.Collections;
using UnityEngine;

namespace MixedRealityToolkit.Sharing.Utilities
{
    /// <summary>
    /// Utility class for automatically joining shared sessions without needing to go through a lobby.
    /// </summary>
    public class AutoJoinSessionAndRoom : Singleton<AutoJoinSessionAndRoom>
    {
        /// <summary>
        /// Some room ID for indicating which room we are in.
        /// </summary>
        private long roomID = 1;

        /// <summary>
        /// Time to wait before attempting to reconnect.
        /// </summary>
        public float Timeout = 1f;

        private static bool ShouldLocalUserCreateRoom
        {
            get
            {
                if (!SharingStage.IsInitialized || SharingStage.Instance.SessionUsersTracker == null)
                {
                    return false;
                }

                long localUserId;
                using (User localUser = SharingStage.Instance.Manager.GetLocalUser())
                {
                    localUserId = localUser.GetID();
                }

                for (int i = 0; i < SharingStage.Instance.SessionUsersTracker.CurrentUsers.Count; i++)
                {
                    if (SharingStage.Instance.SessionUsersTracker.CurrentUsers[i].GetID() < localUserId)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private void Start()
        {
            // SharingStage should be valid at this point, but we may not be connected.
            if (SharingStage.Instance.IsConnected)
            {
                SharingManagerConnected();
            }
            else
            {
                SessionTrackerDisconnected();
            }
        }

        protected override void OnDestroy()
        {
            if (SharingStage.Instance != null)
            {
                SharingStage.Instance.SharingManagerConnected -= SharingManagerConnected;
                SharingStage.Instance.SessionsTracker.ServerDisconnected -= SessionTrackerDisconnected;
            }

            StopCoroutine(AutoConnect());

            base.OnDestroy();
        }

        /// <summary>
        /// Called when the sharing stage connects to a server.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Events Arguments.</param>
        private void SharingManagerConnected(object sender = null, EventArgs e = null)
        {
            SharingStage.Instance.SharingManagerConnected -= SharingManagerConnected;
            SharingStage.Instance.SessionsTracker.ServerDisconnected += SessionTrackerDisconnected;
        }

        /// <summary>
        /// Called when the Session Tracker connects to a server.
        /// </summary>
        private void SessionTrackerDisconnected()
        {
            SharingStage.Instance.SharingManagerConnected += SharingManagerConnected;
            SharingStage.Instance.SessionsTracker.ServerDisconnected -= SessionTrackerDisconnected;

            if (SharingStage.Instance.ClientRole == ClientRole.Primary)
            {
                StartCoroutine(AutoConnect());
            }
        }

        private IEnumerator AutoConnect()
        {
            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.Log("[AutoJoinSessionAndRoom] Attempting to connect...");
            }

            yield return new WaitForSeconds(Timeout);

            if (!SharingStage.Instance.SessionsTracker.IsServerConnected)
            {
                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.LogWarning("[AutoJoinSessionAndRoom] Disconnected from server. Waiting for a connection... ");
                }

                while (!SharingStage.Instance.SessionsTracker.IsServerConnected)
                {
                    yield return null;
                }

                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.Log("[AutoJoinSessionAndRoom] Connected!");
                }
            }

            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.LogFormat("[AutoJoinSessionAndRoom] Looking for {0}...", SharingStage.Instance.SessionName);

                Debug.LogFormat("[AutoJoinSessionAndRoom] Successfully connected to server with {0} Sessions.",
                    SharingStage.Instance.SessionsTracker.Sessions.Count.ToString());
            }

            yield return new WaitForEndOfFrame();

            bool sessionExists = false;

            for (int i = 0; i < SharingStage.Instance.SessionsTracker.Sessions.Count; ++i)
            {
                if (SharingStage.Instance.SessionsTracker.Sessions[i].GetName().GetString() == SharingStage.Instance.SessionName)
                {
                    sessionExists = true;
                    if (SharingStage.Instance.ShowDetailedLogs)
                    {
                        Debug.LogFormat("[AutoJoinSessionAndRoom] Joining session {0}...", SharingStage.Instance.SessionName);
                    }

                    yield return SharingStage.Instance.SessionsTracker.JoinSession(SharingStage.Instance.SessionsTracker.Sessions[i]);

                    yield return new WaitForEndOfFrame();
                    break;
                }
            }

            if (!sessionExists)
            {
                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.LogFormat("[AutoJoinSessionAndRoom] Didn't find session {0}, making a new one...", SharingStage.Instance.SessionName);
                }

                if (!SharingStage.Instance.SessionsTracker.CreateSession(SharingStage.Instance.SessionName))
                {
                    yield break;
                }

                yield return new WaitForEndOfFrame();

                while (SharingStage.Instance.SessionsTracker.GetCurrentSession() == null)
                {
                    yield return null;
                }
            }

            while (SharingStage.Instance.SessionsTracker.GetCurrentSession().GetMachineSessionState() != MachineSessionState.JOINED)
            {
                yield return null;
            }

            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.LogFormat("[AutoJoinSessionAndRoom] Joined session {0} successfully!", SharingStage.Instance.SessionName);
            }

            yield return new WaitForEndOfFrame();

            if (SharingStage.Instance.CurrentRoomManager.GetRoomCount() == 0)
            {
                // If we are the user with the lowest user ID, we will create the room.
                if (ShouldLocalUserCreateRoom)
                {
                    if (SharingStage.Instance.ShowDetailedLogs)
                    {
                        Debug.LogFormat("[AutoJoinSessionAndRoom] Creating room {0}...", SharingStage.Instance.RoomName);
                    }

                    // To keep anchors alive even if all users have left the session...
                    // Pass in true instead of false in CreateRoom.
                    SharingStage.Instance.CurrentRoomManager.CreateRoom(
                        new XString(SharingStage.Instance.RoomName),
                        roomID,
                        SharingStage.Instance.KeepRoomAlive);
                }
            }
            else if (SharingStage.Instance.CurrentRoomManager.GetRoomCount() > 0)
            {
                if (SharingStage.Instance.CurrentRoom != null)
                {
                    SharingStage.Instance.CurrentRoomManager.LeaveRoom();
                }

                yield return new WaitForEndOfFrame();

                // Look through the existing rooms and join the one that matches the room name provided.
                for (int i = 0; i < SharingStage.Instance.CurrentRoomManager.GetRoomCount(); i++)
                {
                    if (SharingStage.Instance.CurrentRoomManager.GetRoom(i).GetName().GetString().Equals(SharingStage.Instance.RoomName, StringComparison.OrdinalIgnoreCase))
                    {
                        SharingStage.Instance.CurrentRoomManager.JoinRoom(SharingStage.Instance.CurrentRoomManager.GetRoom(i));

                        if (SharingStage.Instance.ShowDetailedLogs)
                        {
                            Debug.LogFormat("[AutoJoinSessionAndRoom] Joining room {0}...", SharingStage.Instance.CurrentRoomManager.GetRoom(i).GetName().GetString());
                        }

                        break;
                    }
                }
            }

            while (SharingStage.Instance.CurrentRoom == null)
            {
                yield return null;
            }

            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.LogFormat("[AutoJoinSessionAndRoom] Joined room {0} successfully!", SharingStage.Instance.CurrentRoom.GetName().GetString());
            }

            yield return new WaitForEndOfFrame();

            SharingWorldAnchorManager.Instance.AttachAnchor(gameObject);
        }
    }
}
