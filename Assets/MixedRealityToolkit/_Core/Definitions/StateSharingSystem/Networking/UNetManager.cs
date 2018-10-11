using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Networking
{
    public class UNetManager : NetworkManager, IServerConnection, IClientConnection
    {
        public static List<NetworkConnection> NetworkConnections { get { return networkConnectionsList; } }

        private static List<NetworkConnection> networkConnectionsList = new List<NetworkConnection>();

        [SerializeField]
        private bool useLocalHost = false;
        [SerializeField]
        private string serverIP;

        bool IServerConnection.IsConnected
        {
            get
            {
                return NetworkServer.active;
            }
        }

        bool IClientConnection.IsConnected
        {
            get
            {
                return IsClientConnected();
            }
        }

        bool IClientConnection.ClientSceneReady
        {
            get
            {
                return ClientScene.ready;
            }
        }

        bool IServerConnection.AllClientsReady
        {
            get
            {
                foreach (NetworkConnection conn in networkConnectionsList)
                {
                    if (!conn.isReady)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        bool IClientConnection.UseLocalHost
        {
            get { return useLocalHost; }
            set { useLocalHost = value; }
        }

        string IClientConnection.ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; }
        }

        void IServerConnection.StartServer()
        {
            base.StartServer();
        }

        void IServerConnection.SetAllClientsNotReady()
        {
            NetworkServer.SetAllClientsNotReady();
        }

        void IClientConnection.SetClientSceneReady()
        {
            ClientScene.Ready(client.connection);
        }

        void IClientConnection.StartClient()
        {
            Debug.Log("Starting client...");
            this.networkAddress = useLocalHost ? "localhost" : serverIP;
            base.StartClient();
        }

        void IClientConnection.GetStatsOut(out int numMsgs, out int numBufferedMsgs, out int numBytes, out int lastBufferedPerSecond)
        {
            client.GetStatsOut(out numMsgs, out numBufferedMsgs, out numBytes, out lastBufferedPerSecond);
        }

        void IClientConnection.GetStatsIn(out int numMsgs, out int numBytes)
        {
            client.GetStatsIn(out numMsgs, out numBytes);
        }

        void IClientConnection.ForceDisconnect()
        {
            if (IsClientConnected() || NetworkClient.active)
            {
                StopClient();
            }
        }

        void IServerConnection.ForceDisconnect()
        {
            if (NetworkServer.active)
            {
                StopServer();
            }
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            Debug.Log("OnClientConnect in UNetManager");
            base.OnClientConnect(conn);
            // The default OnClientConnect automatically sets connection to Ready
            // This results in scene objects being spawned incorrectly
            // Manual client ready commands are now used
            networkConnectionsList.Add(conn);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            Debug.Log("Client disconnected");
            base.OnClientDisconnect(conn);

            networkConnectionsList.Remove(conn);
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            Debug.Log("OnServerConnect in UNetManater");
            base.OnServerConnect(conn);
            networkConnectionsList.Add(conn);
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);

            Debug.Log("Spawning objects");
            NetworkServer.SpawnObjects();
        }

        public override void OnStartClient(NetworkClient client)
        {
            base.OnStartClient(client);
        }
        
        public override void OnStartServer()
        {
            Debug.Log("Started server");
            base.OnStartServer();
        }

        public override void OnStopServer()
        {
            Debug.Log("Stopped server");
            base.OnStopServer();
        }

        private void OnDestroy()
        {
            Debug.Log("OnDestroy in UNetManager");
            if (IsClientConnected())
            {
                Debug.Log("STOPPING CLIENT");
                StopClient();
            }
        }
    }
}