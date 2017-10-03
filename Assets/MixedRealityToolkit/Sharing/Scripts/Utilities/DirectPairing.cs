// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Sharing.Utilities
{
    /// <summary>
    /// This class enables users to pair with a remote client directly.
    /// One side should use the Receiver role, the other side should use the Connector role.
    /// RemoteAddress and RemotePort are used by the Connector role, LocalPort is used by the Receiver.
    /// </summary>
    public class DirectPairing : MonoBehaviour
    {
        public enum Role
        {
            Connector,
            Receiver
        }

        public Role PairingRole = Role.Connector;
        public string RemoteAddress = "localhost";
        public ushort RemotePort = 0x507B;
        public ushort LocalPort = 0x507B;
        public bool AutoReconnect = true;

        private SharingManager sharingMgr;
        private PairMaker pairMaker;
        private PairingAdapter pairingAdapter;
        private NetworkConnectionAdapter connectionAdapter;

        private void Start()
        {
            if (PairingRole == Role.Connector)
            {
                pairMaker = new DirectPairConnector(RemoteAddress, RemotePort);
            }
            else
            {
                pairMaker = new DirectPairReceiver(LocalPort);
            }

            pairingAdapter = new PairingAdapter();
            pairingAdapter.SuccessEvent += OnPairingConnectionSucceeded;
            pairingAdapter.FailureEvent += OnPairingConnectionFailed;

            // Register to listen for disconnections, so we can reconnect automatically
            if (SharingStage.Instance != null)
            {
                sharingMgr = SharingStage.Instance.Manager;

                if (sharingMgr != null)
                {
                    connectionAdapter = new NetworkConnectionAdapter();
                    connectionAdapter.DisconnectedCallback += OnDisconnected;

                    NetworkConnection pairedConnection = sharingMgr.GetPairedConnection();
                    pairedConnection.AddListener((byte)MessageID.StatusOnly, connectionAdapter);
                }
            }

            StartPairing();
        }

        private void OnDestroy()
        {
            // SharingStage's OnDestroy() might execute first in the script order. Therefore we should check if
            // SharingStage.Instance still exists. Without the instance check, the execution of GetPairingManager
            // on a disposed sharing manager will crash the Unity Editor and application.
            if (SharingStage.Instance != null && sharingMgr != null)
            {
                PairingManager pairingMgr = sharingMgr.GetPairingManager();
                pairingMgr.CancelPairing(); // Safe to call, even if no pairing is in progress.  Will not cause a disconnect

                // Remove our listener from the paired connection
                NetworkConnection pairedConnection = sharingMgr.GetPairedConnection();
                pairedConnection.RemoveListener((byte)MessageID.StatusOnly, connectionAdapter);
            }
        }

        private void OnPairingConnectionSucceeded()
        {
            if (SharingStage.Instance.ShowDetailedLogs)
            {
                Debug.Log("Direct Pairing Succeeded");
            }
        }

        private void OnPairingConnectionFailed(PairingResult result)
        {
            Debug.LogWarning("Direct pairing failed: " + result);

            if (AutoReconnect)
            {
                Debug.LogWarning("Attempting to reconnect...");
                StartPairing();
            }
        }

        private void OnDisconnected(NetworkConnection connection)
        {
            Debug.LogWarning("Remote client disconnected");

            if (AutoReconnect)
            {
                StartPairing();
            }
        }

        private void StartPairing()
        {
            if (sharingMgr != null)
            {
                PairingManager pairingMgr = sharingMgr.GetPairingManager();

                PairingResult result = pairingMgr.BeginPairing(pairMaker, pairingAdapter);
                if (result != PairingResult.Ok)
                {
                    Debug.LogError("Failed to start pairing");
                }
            }
        }
    }
}