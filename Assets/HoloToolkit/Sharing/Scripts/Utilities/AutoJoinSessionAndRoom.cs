// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine;

namespace HoloToolkit.Sharing.Utilities
{
    /// <summary>
    /// Utility class for automatically joining shared sessions without needing to go through a lobby.
    /// </summary>
    public class AutoJoinSessionAndRoom : Singleton<AutoJoinSessionAndRoom>
    {
        private ServerSessionsTracker sessionsTracker;

        /// <summary>
        /// Some room ID for indicating which room we are in.
        /// </summary>
        private long roomID = 1;

        private Coroutine autoConnect;

        private static bool ShouldLocalUserCreateRoom
        {
            get
            {
                if (SharingStage.Instance == null || SharingStage.Instance.SessionUsersTracker == null)
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
                Connected();
            }
            else
            {
                Disconnected();
            }
        }

        protected override void OnDestroy()
        {
            if (SharingStage.Instance != null)
            {
                SharingStage.Instance.SharingManagerConnected -= Connected;
                SharingStage.Instance.SharingManagerDisconnected -= Disconnected;
            }

            if (autoConnect != null)
            {
                StopCoroutine(autoConnect);
            }

            base.OnDestroy();
        }

        /// <summary>
        /// Called when the sharing stage connects to a server.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Events Arguments.</param>
        private void Connected(object sender = null, EventArgs e = null)
        {
            StopCoroutine(autoConnect);
            SharingStage.Instance.SharingManagerConnected -= Connected;
            SharingStage.Instance.SharingManagerDisconnected += Disconnected;
        }

        /// <summary>
        /// Called when the sharing stage connects to a server.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Events Arguments.</param>
        private void Disconnected(object sender = null, EventArgs e = null)
        {
            Debug.LogError("[AutoJoinSession] Disconnected!");
            SharingStage.Instance.SharingManagerDisconnected -= Disconnected;
            SharingStage.Instance.SharingManagerConnected += Connected;

            if (SharingStage.Instance.ClientRole == ClientRole.Primary)
            {
                autoConnect = StartCoroutine(AutoConnect());
            }
        }

        private IEnumerator AutoConnect()
        {
            Debug.LogWarning("[AutoJoinSession] Attempting to connect...");
            if (SharingStage.Instance.SessionsTracker == null)
            {
                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.LogWarning("[AutoJoinSession] Sharing Manager is not ready!  Attempting to reinitialize...");
                }

                SharingStage.Instance.ConnectToServer();
            }

            while (SharingStage.Instance.SessionsTracker == null)
            {
                yield return null;
            }

            // Get the ServerSessionsTracker to use later.
            sessionsTracker = SharingStage.Instance.SessionsTracker;

            if (sessionsTracker.IsServerConnected)
            {
                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.LogFormat("[AutoJoinSession] Looking for {0}...", SharingStage.Instance.SessionName);

                    Debug.LogFormat("[AutoJoinSession] Successfully connected to server with {0} Sessions.",
                        sessionsTracker.Sessions.Count.ToString());
                }
            }
            else
            {
                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.LogWarning("[AutoJoinSession] Disconnected from server. Waiting for a connection... ");
                }

                while (!sessionsTracker.IsServerConnected)
                {
                    yield return null;
                }

                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.LogWarning("[AutoJoinSession] Connected!");
                }
            }

            // If we are a Primary Client and can join sessions...
            // Check to see if we aren't already in the desired session
            Session currentSession = sessionsTracker.GetCurrentSession();

            // We're not in a valid session.
            if (currentSession == null)
            {
                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.Log("[AutoJoinSession] Didn't find the session, making a new one...");
                }

                yield return sessionsTracker.CreateSession(new XString(SharingStage.Instance.SessionName));

                currentSession = sessionsTracker.GetCurrentSession();
            }

            // We're already in the Session we're searching for.
            if (currentSession != null && currentSession.GetName().GetString() == SharingStage.Instance.SessionName)
            {
                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.LogFormat("[AutoJoinSession] We're already in the session we're attempting to join.");
                }

                autoConnect = null;
                yield break;
            }

            // We're already connected to the session.
            if (currentSession != null && currentSession.GetMachineSessionState() != MachineSessionState.DISCONNECTED)
            {
                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.LogFormat("[AutoJoinSession] We're joining or we've already joined the session.");
                }

                autoConnect = null;
                yield break;
            }

            for (int i = 0; i < sessionsTracker.Sessions.Count; ++i)
            {
                if (sessionsTracker.Sessions[i].GetName().GetString() != SharingStage.Instance.SessionName)
                {
                    continue;
                }

                if (SharingStage.Instance.ShowDetailedLogs)
                {
                    Debug.LogFormat("[AutoJoinSession] Joining session {0}...", SharingStage.Instance.SessionName);
                }

                yield return sessionsTracker.JoinSession(sessionsTracker.Sessions[i]);
            }

            while (currentSession != null && currentSession.GetMachineSessionState() != MachineSessionState.JOINED)
            {
                yield return null;
            }

            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.LogFormat("[AutoJoinSession] Joined session {0} successfully!", SharingStage.Instance.SessionName);
            }

            var roomManager = SharingStage.Instance.Manager.GetRoomManager();

            // First check if there is a current room
            var currentRoom = roomManager.GetCurrentRoom();

            if (currentRoom == null || roomManager.GetRoomCount() == 0)
            {
                // If we are the user with the lowest user ID, we will create the room.
                if (ShouldLocalUserCreateRoom)
                {
                    if (SharingStage.Instance.ShowDetailedLogs)
                    {
                        Debug.LogFormat("[AutoJoinSession] Creating room {0}...", SharingStage.Instance.RoomName);
                    }

                    // To keep anchors alive even if all users have left the session...
                    // Pass in true instead of false in CreateRoom.
                    currentRoom = roomManager.CreateRoom(
                        new XString(SharingStage.Instance.RoomName),
                        roomID,
                        SharingStage.Instance.KeepRoomAlive);
                }
            }
            else if (roomManager.GetRoomCount() > 0)
            {
                // Look through the existing rooms and join the one that matches the room name provided.
                for (int i = 0; i < roomManager.GetRoomCount(); i++)
                {
                    if (roomManager.GetRoom(i).GetName().GetString().Equals(SharingStage.Instance.RoomName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        currentRoom = roomManager.GetRoom(i);
                        roomManager.JoinRoom(currentRoom);

                        if (SharingStage.Instance.ShowDetailedLogs)
                        {
                            Debug.LogFormat("[AutoJoinSession] Joining room {0}...", currentRoom.GetName().GetString());
                        }

                        break;
                    }
                }
            }

            if (currentRoom == null)
            {
                Debug.LogError("[AutoJoinSession] Unable to create or join a room!");
                yield break;
            }

            while (currentRoom != SharingStage.Instance.CurrentRoom)
            {
                yield return null;
            }

            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.LogFormat("[AutoJoinSession] Joined room {0} successfully!", currentRoom.GetName().GetString());
            }

            SharingWorldAnchorManager.Instance.AttachAnchor(gameObject);

            autoConnect = null;
        }
    }
}
