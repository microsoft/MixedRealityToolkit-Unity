// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.SharingWithUNET
{
    /// <summary>
    /// It is nice to know what is going on with the networking scene sometimes.
    /// </summary>
    public class SharedAnchorDebugText : MonoBehaviour
    {

        /// <summary>
        /// Set in the editor with the network discovery object since
        /// we query that object for much of our text.  
        /// </summary>
        public NetworkDiscoveryWithAnchors NetworkDiscoveryObject;

        /// <summary>
        /// The text mesh we will write to.
        /// </summary>
        private TextMesh textMesh;

        /// <summary>
        /// These next variables are the parameters that are use to make
        /// up the debug string.  We keep a cached copy so that we don't  
        /// rebuild the string every frame.
        /// </summary>
        private bool wasClient = false;
        private bool wasServer = false;
        private string ServerIp = "";
        private bool anchorEstablished = false;
        private bool wasImporting = false;
        private bool wasDownloading = false;
        private string anchorName = "";

        /// <summary>
        /// The anchor manager so we can query the state of anchor creation
        /// </summary>
        private UNetAnchorManager anchorManager;

        private void Start()
        {
            textMesh = GetComponent<TextMesh>();
            anchorManager = UNetAnchorManager.Instance;
            UpdateText();
        }

        private void Update()
        {
            if (anchorManager == null)
            {
                anchorManager = UNetAnchorManager.Instance;
            }

            bool dirty = false;
            if (wasClient != NetworkDiscoveryObject.isClient)
            {
                Debug.Log("Was client changed to " + NetworkDiscoveryObject.isClient);
                wasClient = NetworkDiscoveryObject.isClient;
                dirty = true;
            }

            if (wasServer != NetworkDiscoveryObject.isServer)
            {
                wasServer = NetworkDiscoveryObject.isServer;
                dirty = true;
            }

            if (ServerIp != NetworkDiscoveryObject.ServerIp)
            {
                ServerIp = NetworkDiscoveryObject.ServerIp;
                dirty = true;
            }

            // Anchor manger doesn't come online until we connect
            if (anchorManager != null)
            {

                if (anchorEstablished != anchorManager.AnchorEstablished)
                {
                    anchorEstablished = anchorManager.AnchorEstablished;
                    dirty = true;
                }

                if (anchorName != anchorManager.AnchorName)
                {
                    anchorName = anchorManager.AnchorName;
                    dirty = true;
                }

                if (wasImporting != anchorManager.ImportInProgress)
                {
                    wasImporting = anchorManager.ImportInProgress;
                    dirty = true;
                }

                if (wasDownloading != anchorManager.DownloadingAnchor)
                {
                    wasDownloading = anchorManager.DownloadingAnchor;
                    dirty = true;
                }
            }

            if (dirty)
            {
                UpdateText();
            }
        }

        private void UpdateText()
        {
            textMesh.text = string.Format(
                "{0}{1}{2}\n{3}{4}\n",
                wasClient ? "Client\n" : "",
                wasServer ? "Server\n" : "",
                ServerIp,
                anchorEstablished ? "Anchored Here\n" : (wasImporting ? "Importing\n" : (wasDownloading ? "Downloading\n" : "Not Anchored\n")),
                anchorName);
        }
    }
}