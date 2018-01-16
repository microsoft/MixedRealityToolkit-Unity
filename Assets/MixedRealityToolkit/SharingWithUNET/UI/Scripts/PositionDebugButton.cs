// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using UnityEngine;

namespace MixedRealityToolkit.SharingWithUNET
{
    public class PositionDebugButton : SingleInstance<PositionDebugButton>
    {

        public GameObject DisconnectedPosition { get; set; }
        public GameObject ConnectedPosition { get; set; }

        NetworkDiscoveryWithAnchors networkDisco;

        // Use this for initialization
        void Start()
        {
            transform.SetParent(DisconnectedPosition.transform, false);
            networkDisco = NetworkDiscoveryWithAnchors.Instance;
            networkDisco.ConnectionStatusChanged += NetworkDisco_ConnectionStatusChanged;
        }

        private void NetworkDisco_ConnectionStatusChanged(object sender, System.EventArgs e)
        {
            MoveToRightSpot();
        }

        void MoveToRightSpot()
        {
            GameObject parent = networkDisco.Connected ? ConnectedPosition : DisconnectedPosition;
            if (parent == null)
            {
                Invoke("MoveToRightSpot", 0.1f);
                return;
            }

            transform.SetParent(parent.transform, false);
            // this is a little hack because our parent might have disabled our renderers/colliders. 
            SetChildren(true);
        }

        void SetChildren(bool Enabled)
        {
            foreach (Renderer mr in GetComponentsInChildren<Renderer>())
            {
                mr.enabled = Enabled;
            }

            foreach (BoxCollider bc in GetComponentsInChildren<BoxCollider>())
            {
                bc.enabled = Enabled;
            }
        }
    }
}
