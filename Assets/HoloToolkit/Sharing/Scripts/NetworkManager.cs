using UnityEngine;
using HoloToolkit.Unity;

namespace HoloToolkit.Sharing
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        /// <summary>
        /// Manager object for networking component
        /// </summary>
        public XToolsManager XToolsManager
        {
            get; set;
        }

        /// <summary>
        /// Session service connection object
        /// </summary>
        public NetworkConnection ServerConnection
        {
            get; set;
        }

        /// <summary>
        /// Helper object that we use to route incoming message callbacks
        /// </summary>
        public NetworkConnectionAdapter ConnectionAdapter
        {
            get; set;
        }

        void Awake()
        {
            InitializeXTools();
        }

        void InitializeXTools()
        {
            NetworkStage xtoolsStage = NetworkStage.Instance;
            if (xtoolsStage != null)
            {
                this.XToolsManager = xtoolsStage.Manager;
                this.ServerConnection = xtoolsStage.Manager.GetServerConnection();
                this.ConnectionAdapter = new NetworkConnectionAdapter();
            }
        }
    }
}