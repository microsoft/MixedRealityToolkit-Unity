// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Allows users of the RoomManager to register to receive event callbacks without
    /// having their classes inherit directly from RoomManagerListener
    /// </summary>
    public class RoomManagerAdapter : RoomManagerListener
    {
        public event System.Action<Room> RoomAddedEvent;
        public event System.Action<Room> RoomClosedEvent;
        public event System.Action<Room, int> UserJoinedRoomEvent;
        public event System.Action<Room, int> UserLeftRoomEvent;
        public event System.Action<Room> AnchorsChangedEvent;
        public event System.Action<bool, AnchorDownloadRequest, XString> AnchorsDownloadedEvent;
        public event System.Action<bool, XString> AnchorUploadedEvent;

        public RoomManagerAdapter() { }

        public override void OnRoomAdded(Room newRoom)
        {
            Profile.BeginRange("OnRoomAdded");
            if (this.RoomAddedEvent != null)
            {
                this.RoomAddedEvent(newRoom);
            }
            Profile.EndRange();
        }

        public override void OnRoomClosed(Room room)
        {
            Profile.BeginRange("OnRoomClosed");
            if (this.RoomClosedEvent != null)
            {
                this.RoomClosedEvent(room);
            }
            Profile.EndRange();
        }

        public override void OnUserJoinedRoom(Room room, int user)
        {
            Profile.BeginRange("OnUserJoinedRoom");
            if (this.UserJoinedRoomEvent != null)
            {
                this.UserJoinedRoomEvent(room, user);
            }
            Profile.EndRange();
        }

        public override void OnUserLeftRoom(Room room, int user)
        {
            Profile.BeginRange("OnUserLeftRoom");
            if (this.UserLeftRoomEvent != null)
            {
                this.UserLeftRoomEvent(room, user);
            }
            Profile.EndRange();
        }

        public override void OnAnchorsChanged(Room room)
        {
            Profile.BeginRange("OnAnchorsChanged");
            if (this.AnchorsChangedEvent != null)
            {
                this.AnchorsChangedEvent(room);
            }
            Profile.EndRange();
        }

        public override void OnAnchorsDownloaded(bool successful, AnchorDownloadRequest request, XString failureReason)
        {
            Profile.BeginRange("OnAnchorsDownloaded");
            if (this.AnchorsDownloadedEvent != null)
            {
                this.AnchorsDownloadedEvent(successful, request, failureReason);
            }
            Profile.EndRange();
        }

        public override void OnAnchorUploadComplete(bool successful, XString failureReason)
        {
            Profile.BeginRange("OnAnchorUploadComplete");
            if (this.AnchorUploadedEvent != null)
            {
                this.AnchorUploadedEvent(successful, failureReason);
            }
            Profile.EndRange();
        }
    }
}