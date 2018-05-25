// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity.Preview.SpectatorView
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
        /// SpectatorViewNetworkDiscovery
        /// </summary>
        [Tooltip("SpectatorViewNetworkDiscovery")]
        [SerializeField]
        private SpectatorViewNetworkDiscovery spectatorViewNetworkDiscovery;
        /// <summary>
        /// SpectatorViewNetworkDiscovery
        /// </summary>
        public SpectatorViewNetworkDiscovery SpectatorViewNetworkDiscovery
        {
            get
            {
                return spectatorViewNetworkDiscovery;
            }

            set
            {
                spectatorViewNetworkDiscovery = value;
            }
        }

        /// <summary>
        /// SpectatorViewNetworkManager
        /// </summary>
        [Tooltip("SpectatorViewNetworkManager")]
        [SerializeField]
        private SpectatorViewNetworkManager spectatorViewNetworkManager;
        /// <summary>
        /// SpectatorViewNetworkManager
        /// </summary>
        public SpectatorViewNetworkManager SpectatorViewNetworkManager
        {
            get
            {
                return spectatorViewNetworkManager;
            }

            set
            {
                spectatorViewNetworkManager = value;
            }
        }

        /// <summary>
        /// Object responsible for aligning holograms on mobile and HoloLens
        /// </summary>
        [Tooltip("Object responsible for aligning holograms on mobile and HoloLens")]
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
        /// Object to detect whether the world anchor has been located
        /// </summary>
        [Tooltip("Object to detect whether the world anchor has been located")]
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

            if (SpectatorViewNetworkDiscovery == null)
            {
                SpectatorViewNetworkDiscovery = FindObjectOfType<SpectatorViewNetworkDiscovery>();
            }

            if (SpectatorViewNetworkManager == null)
            {
                SpectatorViewNetworkManager = FindObjectOfType<SpectatorViewNetworkManager>();
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
            SpectatorViewNetworkDiscovery.OnHololensSessionFound += PromptConnecting;
            SpectatorViewNetworkDiscovery.OnHololensSessionFound += PromptAlmostThere;

            // First status
            if (Text != null)
            {
                Text.text = "Locating Floor...";
            }
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
            SpectatorViewNetworkDiscovery.OnHololensSessionFound -= PromptConnecting;
            SpectatorViewNetworkDiscovery.OnHololensSessionFound -= PromptAlmostThere;
        }
    }
}
