// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Sharing;
using UnityEngine;
using Random = System.Random;

namespace MixedRealityToolkit.Examples.Sharing
{
    /// <summary>
    /// Test class for demonstrating creating rooms and anchors.
    /// </summary>
    public class RoomTest : MonoBehaviour
    {
        private RoomManagerAdapter listener;
        private RoomManager roomMgr;

        private string roomName = "New Room";
        private Vector2 scrollViewVector = Vector2.zero;
        private int padding = 4;
        private int areaWidth = 400;
        private int areaHeight = 300;
        private int buttonWidth = 80;
        private int lineHeight = 20;

        private Vector2 anchorScrollVector = Vector2.zero;
        private string anchorName = "New Anchor";
        private readonly byte[] anchorTestData = new byte[5 * 1024 * 1024]; // 5 meg test buffer

        private void Start()
        {
            for (int i = 0; i < anchorTestData.Length; ++i)
            {
                anchorTestData[i] = (byte)(i % 256);
            }

            SharingStage stage = SharingStage.Instance;
            if (stage != null)
            {
                SharingManager sharingMgr = stage.Manager;
                if (sharingMgr != null)
                {
                    roomMgr = sharingMgr.GetRoomManager();

                    listener = new RoomManagerAdapter();
                    listener.RoomAddedEvent += OnRoomAdded;
                    listener.RoomClosedEvent += OnRoomClosed;
                    listener.UserJoinedRoomEvent += OnUserJoinedRoom;
                    listener.UserLeftRoomEvent += OnUserLeftRoom;
                    listener.AnchorsChangedEvent += OnAnchorsChanged;
                    listener.AnchorsDownloadedEvent += OnAnchorsDownloaded;
                    listener.AnchorUploadedEvent += OnAnchorUploadComplete;

                    roomMgr.AddListener(listener);
                }
            }
        }

        private void OnDestroy()
        {
            roomMgr.RemoveListener(listener);
            listener.Dispose();
        }

        private void OnGUI()
        {
            // Make a background box
            scrollViewVector = GUI.BeginScrollView(
                new Rect(25, 25, areaWidth, areaHeight),
                scrollViewVector,
                new Rect(0, 0, areaWidth, areaHeight));

            if (roomMgr != null)
            {
                SessionManager sessionMgr = SharingStage.Instance.Manager.GetSessionManager();
                if (sessionMgr != null)
                {
                    roomName = GUI.TextField(
                            new Rect(buttonWidth + padding, 0, areaWidth - (buttonWidth + padding), lineHeight),
                            roomName);

                    if (GUI.Button(new Rect(0, 0, buttonWidth, lineHeight), "Create"))
                    {
                        Random rnd = new Random();

                        Room newRoom = roomMgr.CreateRoom(roomName, rnd.Next(), false);
                        if (newRoom == null)
                        {
                            Debug.LogWarning("Cannot create room");
                        }
                    }

                    Room currentRoom = roomMgr.GetCurrentRoom();

                    for (int i = 0; i < roomMgr.GetRoomCount(); ++i)
                    {
                        Room room = roomMgr.GetRoom(i);

                        int vOffset = (padding + lineHeight) * (i + 1);
                        int hOffset = 0;

                        bool keepOpen = GUI.Toggle(new Rect(hOffset, vOffset, lineHeight, lineHeight), room.GetKeepOpen(), "");
                        room.SetKeepOpen(keepOpen);

                        hOffset += lineHeight + padding;

                        if (currentRoom != null && room.GetID() == currentRoom.GetID())
                        {
                            if (GUI.Button(new Rect(hOffset, vOffset, buttonWidth, lineHeight), "Leave"))
                            {
                                roomMgr.LeaveRoom();
                            }
                        }
                        else
                        {
                            if (GUI.Button(new Rect(hOffset, vOffset, buttonWidth, lineHeight), "Join"))
                            {
                                if (!roomMgr.JoinRoom(room))
                                {
                                    Debug.LogWarning("Cannot join room");
                                }
                            }
                        }

                        hOffset += buttonWidth + padding;

                        GUI.Label(new Rect(hOffset, vOffset, areaWidth - (buttonWidth + padding), lineHeight),
                            room.GetName().GetString());
                    }
                }
            }

            // End the ScrollView
            GUI.EndScrollView();

            if (roomMgr != null)
            {
                Room currentRoom = roomMgr.GetCurrentRoom();

                if (currentRoom != null)
                {
                    // Display option to upload anchor
                    anchorScrollVector = GUI.BeginScrollView(
                        new Rect(areaWidth + 50, 25, areaWidth, areaHeight),
                        anchorScrollVector,
                        new Rect(0, 0, areaWidth, areaHeight));

                    anchorName =
                        GUI.TextField(
                            new Rect(
                                buttonWidth + padding,
                                0,
                                areaWidth - (buttonWidth + padding),
                                lineHeight),
                            anchorName);

                    if (GUI.Button(new Rect(0, 0, buttonWidth, lineHeight), "Create"))
                    {
                        if (!roomMgr.UploadAnchor(currentRoom, anchorName, anchorTestData, anchorTestData.Length))
                        {
                            Debug.LogError("Failed to start anchor upload");
                        }
                    }

                    for (int i = 0; i < currentRoom.GetAnchorCount(); ++i)
                    {
                        int vOffset = (padding + lineHeight) * (i + 1);

                        XString currentRoomAnchor = currentRoom.GetAnchorName(i);

                        GUI.Label(
                            new Rect(
                                buttonWidth + padding,
                                vOffset,
                                areaWidth - (buttonWidth + padding),
                                lineHeight),
                            currentRoomAnchor);

                        if (GUI.Button(new Rect(0, vOffset, buttonWidth, lineHeight), "Download"))
                        {
                            if (!roomMgr.DownloadAnchor(currentRoom, currentRoomAnchor))
                            {
                                Debug.LogWarning("Failed to start anchor download");
                            }
                        }
                    }

                    GUI.EndScrollView();
                }
            }
        }

        private void OnRoomAdded(Room newRoom)
        {
            Debug.LogFormat("Room {0} added", newRoom.GetName().GetString());
        }

        private void OnRoomClosed(Room room)
        {
            Debug.LogFormat("Room {0} closed", room.GetName().GetString());
        }

        private void OnUserJoinedRoom(Room room, int user)
        {
            User joinedUser = SharingStage.Instance.SessionUsersTracker.GetUserById(user);
            Debug.LogFormat("User {0} joined Room {1}", joinedUser.GetName(), room.GetName().GetString());
        }

        private void OnUserLeftRoom(Room room, int user)
        {
            User leftUser = SharingStage.Instance.SessionUsersTracker.GetUserById(user);
            Debug.LogFormat("User {0} left Room {1}", leftUser.GetName(), room.GetName().GetString());
        }

        private void OnAnchorsChanged(Room room)
        {
            Debug.LogFormat("Anchors changed for Room {0}", room.GetName().GetString());
        }

        private void OnAnchorsDownloaded(bool successful, AnchorDownloadRequest request, XString failureReason)
        {
            if (successful)
            {
                Debug.LogFormat("Anchors download succeeded for Room {0}", request.GetRoom().GetName().GetString());
            }
            else
            {
                Debug.LogFormat("Anchors download failed: {0}", failureReason.GetString());
            }
        }

        private void OnAnchorUploadComplete(bool successful, XString failureReason)
        {
            if (successful)
            {
                Debug.Log("Anchors upload succeeded");
            }
            else
            {
                Debug.LogFormat("Anchors upload failed: {0}", failureReason.GetString());
            }
        }
    }
}
