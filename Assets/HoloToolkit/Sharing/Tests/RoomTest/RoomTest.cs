using UnityEngine;
using HoloToolkit.Sharing;

public class RoomTest : MonoBehaviour
{
    private RoomManagerAdapter listener;
    private RoomManager roomMgr;

    private string roomName = "New Room";
    private Vector2 scrollViewVector = Vector2.zero;
    private int Padding = 4;
    private int AreaWidth = 400;
    private int AreaHeight = 300;
    private int ButtonWidth = 80;
    private int LineHeight = 20;

    private Vector2 anchorScrollVector = Vector2.zero;
    private string anchorName = "New Anchor";
    private byte[] anchorTestData = new byte[5 * 1024 * 1024];  // 5 meg test buffer

    void Start ()
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
                this.roomMgr = sharingMgr.GetRoomManager();

                this.listener = new RoomManagerAdapter();
                this.listener.RoomAddedEvent += OnRoomAdded;
                this.listener.RoomClosedEvent += OnRoomClosed;
                this.listener.UserJoinedRoomEvent += OnUserJoinedRoom;
                this.listener.UserLeftRoomEvent += OnUserLeftRoom;
                this.listener.AnchorsChangedEvent += OnAnchorsChanged;
                this.listener.AnchorsDownloadedEvent += OnAnchorsDownloaded;
                this.listener.AnchorUploadedEvent += OnAnchorUploadComplete;

                this.roomMgr.AddListener(this.listener);
            }
        }
    }

    private void OnDestroy()
    {
        this.roomMgr.RemoveListener(this.listener);
        this.listener.Dispose();
    }


    void OnGUI()
    {
        // Make a background box
        this.scrollViewVector = GUI.BeginScrollView(new Rect(25, 25, this.AreaWidth, this.AreaHeight), scrollViewVector, new Rect(0, 0, this.AreaWidth, this.AreaHeight));

        if (this.roomMgr != null)
        {
            SessionManager sessionMgr = SharingStage.Instance.Manager.GetSessionManager();
            if (sessionMgr != null)
            {
                this.roomName = GUI.TextField(new Rect(this.ButtonWidth + this.Padding, 0, this.AreaWidth - (this.ButtonWidth + this.Padding), this.LineHeight), this.roomName);

                if (GUI.Button(new Rect(0, 0, this.ButtonWidth, this.LineHeight), "Create"))
                {
                    System.Random rnd = new System.Random();

                    Room newRoom = roomMgr.CreateRoom(this.roomName, rnd.Next(), false);
                    if (newRoom == null)
                    {
                        Debug.LogWarning("Cannot create room");
                    }
                }

                Room currentRoom = roomMgr.GetCurrentRoom();

                for (int i = 0; i < roomMgr.GetRoomCount(); ++i)
                {
                    Room r = roomMgr.GetRoom(i);

                    int vOffset = (this.Padding + this.LineHeight) * (i + 1);
                    int hOffset = 0;

                    bool keepOpen = GUI.Toggle(new Rect(hOffset, vOffset, this.LineHeight, this.LineHeight), r.GetKeepOpen(), "");
                    r.SetKeepOpen(keepOpen);

                    hOffset += this.LineHeight + this.Padding;

                    if (currentRoom != null && r.GetID() == currentRoom.GetID())
                    {
                        if (GUI.Button(new Rect(hOffset, vOffset, this.ButtonWidth, this.LineHeight), "Leave"))
                        {
                            roomMgr.LeaveRoom();
                        }
                    }
                    else
                    {
                        if (GUI.Button(new Rect(hOffset, vOffset, this.ButtonWidth, this.LineHeight), "Join"))
                        {
                            if (!roomMgr.JoinRoom(r))
                            {
                                Debug.LogWarning("Cannot join room");
                            }
                        }
                    }

                    hOffset += this.ButtonWidth + this.Padding;

                    GUI.Label(new Rect(hOffset, vOffset, this.AreaWidth - (this.ButtonWidth + this.Padding), this.LineHeight), r.GetName().GetString());

                }
            }
        }

        // End the ScrollView
        GUI.EndScrollView();

        {
            Room currentRoom = roomMgr.GetCurrentRoom();

            if (currentRoom != null)
            {
                // Display option to upload anchor
                this.anchorScrollVector = GUI.BeginScrollView(new Rect(this.AreaWidth + 50, 25, this.AreaWidth, this.AreaHeight), anchorScrollVector, new Rect(0, 0, this.AreaWidth, this.AreaHeight));

                this.anchorName = GUI.TextField(new Rect(this.ButtonWidth + this.Padding, 0, this.AreaWidth - (this.ButtonWidth + this.Padding), this.LineHeight), this.anchorName);

                if (GUI.Button(new Rect(0, 0, this.ButtonWidth, this.LineHeight), "Create"))
                {
                    if (!this.roomMgr.UploadAnchor(currentRoom, this.anchorName, anchorTestData, anchorTestData.Length))
                    {
                        Debug.LogError("Failed to start anchor upload");
                    }
                }

                for (int i = 0; i < currentRoom.GetAnchorCount(); ++i)
                {
                    int vOffset = (this.Padding + this.LineHeight) * (i + 1);

                    XString name = currentRoom.GetAnchorName(i);

                    GUI.Label(new Rect(this.ButtonWidth + this.Padding, vOffset, this.AreaWidth - (this.ButtonWidth + this.Padding), this.LineHeight), name);

                    if (GUI.Button(new Rect(0, vOffset, this.ButtonWidth, this.LineHeight), "Download"))
                    {
                        if (!roomMgr.DownloadAnchor(currentRoom, name))
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
        Debug.Log(string.Format("Room {0} added", newRoom.GetName().GetString()));
    }

    private void OnRoomClosed(Room room)
    {
        Debug.Log(string.Format("Room {0} closed", room.GetName().GetString()));
    }

    private void OnUserJoinedRoom(Room room, int user)
    {
        Debug.Log(string.Format("User {0} joined Room {1}", user, room.GetName().GetString()));
    }

    private void OnUserLeftRoom(Room room, int user)
    {
        Debug.Log(string.Format("User {0} left Room {1}", user, room.GetName().GetString()));
    }

    private void OnAnchorsChanged(Room room)
    {
        Debug.Log(string.Format("Anchors changed for Room {0}", room.GetName().GetString()));
    }

    private void OnAnchorsDownloaded(bool successful, AnchorDownloadRequest request, XString failureReason)
    {
        if (successful)
        {
            Debug.Log(string.Format("Anchors download succeeded for Room {0}", request.GetRoom().GetName().GetString()));
        }
        else
        {
            Debug.Log(string.Format("Anchors download failed: {0}", failureReason.GetString()));
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
            Debug.Log(string.Format("Anchors upload failed: {0}", failureReason.GetString()));
        }
    }
}
