using UnityEngine;

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Component to pair with a remote client directly.
    /// One side should use the Receiver roll, the other side should use the Connector roll.
    /// RemoteAddress and RemotePort are used by the Connector roll, LocalPort is used by the Receiver
    /// </summary>
    public class DirectPairing : MonoBehaviour
    {
        public enum Role
        {
            Connector,
            Receiver
        };

        public Role PairingRole = Role.Connector;
        public string RemoteAddress = "localhost";
        public ushort RemotePort = 0x507B;
        public ushort LocalPort = 0x507B;
        public bool AutoReconnect = true;

        private SharingManager sharingMgr;
        private PairMaker pairMaker;
        private PairingAdapter pairingAdapter;
        private NetworkConnectionAdapter connectionAdapter;


        void Start()
        {
            if (this.PairingRole == Role.Connector)
            {
                this.pairMaker = new DirectPairConnector(this.RemoteAddress, this.RemotePort);
            }
            else
            {
                this.pairMaker = new DirectPairReceiver(this.LocalPort);
            }

            this.pairingAdapter = new PairingAdapter();
            this.pairingAdapter.SuccessEvent += OnPairingConnectionSucceeded;
            this.pairingAdapter.FailureEvent += OnPairingConnectionFailed;

            // Register to listen for disconnections, so we can reconnect automatically
            if (SharingStage.Instance != null)
            {
                this.sharingMgr = SharingStage.Instance.Manager;

                if (this.sharingMgr != null)
                {
                    this.connectionAdapter = new NetworkConnectionAdapter();
                    this.connectionAdapter.DisconnectedCallback += OnDisconnected;

                    NetworkConnection pairedConnection = this.sharingMgr.GetPairedConnection();
                    pairedConnection.AddListener((byte)MessageID.StatusOnly, this.connectionAdapter);
                }
            }

            StartPairing();
        }


        void OnDestroy()
        {
            if (this.sharingMgr != null)
            {
                PairingManager pairingMgr = sharingMgr.GetPairingManager();
                pairingMgr.CancelPairing();	// Safe to call, even if no pairing is in progress.  Will not cause a disconnect

                // Remove our listener from the paired connection
                NetworkConnection pairedConnection = sharingMgr.GetPairedConnection();
                pairedConnection.RemoveListener((byte)MessageID.StatusOnly, this.connectionAdapter);
            }
        }


        void OnPairingConnectionSucceeded()
        {
            Debug.Log("Direct Pairing Succeeded");
        }


        void OnPairingConnectionFailed(PairingResult result)
        {
            Debug.LogWarning("Direct pairing failed: " + result.ToString());

            if (this.AutoReconnect)
            {
                StartPairing();
            }
        }


        void OnDisconnected(NetworkConnection connection)
        {
            Debug.LogWarning("Remote client disconnected");

            if (this.AutoReconnect)
            {
                StartPairing();
            }
        }


        void StartPairing()
        {
            if (this.sharingMgr != null)
            {
                PairingManager pairingMgr = this.sharingMgr.GetPairingManager();

                PairingResult result = pairingMgr.BeginPairing(this.pairMaker, this.pairingAdapter);
                if (result != PairingResult.Ok)
                {
                    Debug.LogError("Failed to start pairing");
                }
            }
        }
    }
}