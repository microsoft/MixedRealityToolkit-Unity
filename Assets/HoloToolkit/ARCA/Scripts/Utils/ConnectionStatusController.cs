// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.ARCapture
{
    /// <summary>
    /// Controls an on screen text field to display connection status to the user
    /// </summary>
    public class ConnectionStatusController : MonoBehaviour
    {
        /// <summary>
        /// UI Textfield to display status
        /// </summary>
        [SerializeField]
        [Tooltip("UI Textfield to display status")]
        private Text text;
        /// <summary>
        /// UI Textfield to display status
        /// </summary>
        public Text Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }

        /// <summary>
        /// ARCANetworkDiscovery
        /// </summary>
        [Tooltip("ARCANetworkDiscovery")]
        [SerializeField]
        private ARCANetworkDiscovery arcaNetworkDiscovery;
        /// <summary>
        /// ARCANetworkDiscovery
        /// </summary>
        public ARCANetworkDiscovery ARCANetworkDiscovery
        {
            get
            {
                return arcaNetworkDiscovery;
            }

            set
            {
                arcaNetworkDiscovery = value;
            }
        }

        /// <summary>
        /// ARCANetworkManager
        /// </summary>
        [Tooltip("ARCANetworkManager")]
        [SerializeField]
        private ARCANetworkManager arcaNetworkManager;
        /// <summary>
        /// ARCANetworkManager
        /// </summary>
        public ARCANetworkManager ARCANetworkManager
        {
            get
            {
                return arcaNetworkManager;
            }

            set
            {
                arcaNetworkManager = value;
            }
        }

        /// <summary>
        /// WorldSync
        /// </summary>
        [Tooltip("WorldSync")]
        [SerializeField]
        private WorldSync worldSync;
        /// <summary>
        /// WorldSync
        /// </summary>
        public WorldSync WorldSync
        {
            get
            {
                return worldSync;
            }

            set
            {
                worldSync = value;
            }
        }

        /// <summary>
        /// AnchorLocated
        /// </summary>
        [Tooltip("AnchorLocated")]
        [SerializeField]
        private AnchorLocated anchorLocated;
        /// <summary>
        /// AnchorLocated
        /// </summary>
        public AnchorLocated AnchorLocated
        {
            get
            {
                return anchorLocated;
            }

            set
            {
                anchorLocated = value;
            }
        }

        void Start ()
        {
            if (Text == null)
            {
                Text = GetComponent<Text>();
            }

            if (ARCANetworkDiscovery == null)
            {
                ARCANetworkDiscovery = FindObjectOfType<ARCANetworkDiscovery>();
            }

            if (ARCANetworkManager == null)
            {
                ARCANetworkManager = FindObjectOfType<ARCANetworkManager>();
            }

            if (WorldSync == null)
            {
                WorldSync = FindObjectOfType<WorldSync>();
            }

            if (AnchorLocated == null)
            {
                AnchorLocated = FindObjectOfType<AnchorLocated>();
            }

            // Suscribe to Anchor and Network events
            AnchorLocated.OnAnchorLocated += PromptShowToHoloLens;
            ARCANetworkDiscovery.OnHololensSessionFound += PromptConnecting;
            ARCANetworkDiscovery.OnHololensSessionFound += PromptAlmostThere;

            // First status
            Text.text = "Locating Floor...";
        }

        /// <summary>
        /// Sets text displayed on screen before marker detected
        /// </summary>
        private void PromptShowToHoloLens()
        {
            Text.text = "Show to HoloLens";
        }

        /// <summary>
        /// Sets text displayed on screen once marker has been detected,
        /// before mobile has connected to session
        /// </summary>
        private void PromptConnecting()
        {
            Text.text = "Connecting...";
        }

        /// <summary>
        /// Sets text displayed on screen once marker has been detected,
        /// mobile has connected to the HoloLens session, but before
        /// the world space has been synchronized
        /// </summary>
        private void PromptAlmostThere()
        {
            Text.text = "Almost there...";
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            AnchorLocated.OnAnchorLocated -= PromptShowToHoloLens;
            ARCANetworkDiscovery.OnHololensSessionFound -= PromptConnecting;
            ARCANetworkDiscovery.OnHololensSessionFound -= PromptAlmostThere;
        }
    }
}
