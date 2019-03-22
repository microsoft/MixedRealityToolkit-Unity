// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TabViewManager.cs" company="Exit Games GmbH">
// </copyright>
// <summary>
//  Output detailed information about Pun Current states, using the old Unity UI framework.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Output detailed information about Pun Current states, using the old Unity UI framework.
    /// </summary>
    public class StatesGui : MonoBehaviour
    {
        public Rect GuiOffset = new Rect(250, 0, 300, 300);
        public bool DontDestroy = true;
        public bool ServerTimestamp;
        public bool DetailedConnection;
        public bool Server;
        public bool AppVersion;
        public bool UserId;
        public bool Room;
        public bool RoomProps;
        public bool LocalPlayer;
        public bool PlayerProps;
        public bool Others;
        public bool Buttons;
        public bool ExpectedUsers;

        private Rect GuiRect = new Rect();
        private static StatesGui Instance;

        void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(this.gameObject);
                return;
            }
            if (DontDestroy)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        void OnDisable()
        {
            if (DontDestroy && Instance == this)
            {
                Instance = null;
            }

        }

        void OnGUI()
        {
            Rect GuiOffsetRuntime = new Rect(this.GuiOffset);

            if (GuiOffsetRuntime.x < 0)
            {
                GuiOffsetRuntime.x = Screen.width - GuiOffsetRuntime.width;
            }
            GuiRect.xMin = GuiOffsetRuntime.x;
            GuiRect.yMin = GuiOffsetRuntime.y;
            GuiRect.xMax = GuiOffsetRuntime.x + GuiOffsetRuntime.width;
            GuiRect.yMax = GuiOffsetRuntime.y + GuiOffsetRuntime.height;
            GUILayout.BeginArea(GuiRect);

            GUILayout.BeginHorizontal();
            if (this.ServerTimestamp)
            {
                GUILayout.Label((((double)PhotonNetwork.ServerTimestamp) / 1000d).ToString("F3"));
            }

            if (Server)
            {
                GUILayout.Label(PhotonNetwork.ServerAddress + " " + PhotonNetwork.Server);
            }
            if (DetailedConnection)
            {
                GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
            }
            if (AppVersion)
            {
                GUILayout.Label(PhotonNetwork.NetworkingClient.AppVersion);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (UserId)
            {
                GUILayout.Label("UID: " + ((PhotonNetwork.AuthValues != null) ? PhotonNetwork.AuthValues.UserId : "no UserId"));
                GUILayout.Label("UserId:" + PhotonNetwork.LocalPlayer.UserId);
            }
            GUILayout.EndHorizontal();

            if (Room)
            {
                if (PhotonNetwork.InRoom)
                {
                    GUILayout.Label(this.RoomProps ? PhotonNetwork.CurrentRoom.ToStringFull() : PhotonNetwork.CurrentRoom.ToString());
                }
                else
                {
                    GUILayout.Label("not in room");
                }
            }



            if (this.LocalPlayer)
            {
                GUILayout.Label(PlayerToString(PhotonNetwork.LocalPlayer));
            }
            if (Others)
            {
                foreach (Player player in PhotonNetwork.PlayerListOthers)
                {
                    GUILayout.Label(PlayerToString(player));
                }
            }
            if (ExpectedUsers)
            {
                if (PhotonNetwork.InRoom)
                {
                    int countExpected = (PhotonNetwork.CurrentRoom.ExpectedUsers != null) ? PhotonNetwork.CurrentRoom.ExpectedUsers.Length : 0;

                    GUILayout.Label("Expected: " + countExpected + " " +
                                   ((PhotonNetwork.CurrentRoom.ExpectedUsers != null) ? string.Join(",", PhotonNetwork.CurrentRoom.ExpectedUsers) : "")
                                    );

                }
            }


            if (Buttons)
            {
                if (!PhotonNetwork.IsConnected && GUILayout.Button("Connect"))
                {
                    PhotonNetwork.ConnectUsingSettings();
                }
                GUILayout.BeginHorizontal();
                if (PhotonNetwork.IsConnected && GUILayout.Button("Disconnect"))
                {
                    PhotonNetwork.Disconnect();
                }
                if (PhotonNetwork.IsConnected && GUILayout.Button("Close Socket"))
                {
                    PhotonNetwork.NetworkingClient.LoadBalancingPeer.StopThread();
                }
                GUILayout.EndHorizontal();
                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && GUILayout.Button("Leave"))
                {
                    PhotonNetwork.LeaveRoom();
                }
                if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom && GUILayout.Button("Join Random"))
                {
                    PhotonNetwork.JoinRandomRoom();
                }
                if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom && GUILayout.Button("Create Room"))
                {
                    PhotonNetwork.CreateRoom(null);
                }
            }

            GUILayout.EndArea();
        }

        private string PlayerToString(Player player)
        {
            if (PhotonNetwork.NetworkingClient == null)
            {
                Debug.LogError("nwp is null");
                return "";
            }
            return string.Format("#{0:00} '{1}'{5} {4}{2} {3} {6}", player.ActorNumber + "/userId:<" + player.UserId + ">", player.NickName, player.IsMasterClient ? "(master)" : "", this.PlayerProps ? player.CustomProperties.ToStringFull() : "", (PhotonNetwork.LocalPlayer.ActorNumber == player.ActorNumber) ? "(you)" : "", player.UserId, player.IsInactive ? " / Is Inactive" : "");
        }
    }
}