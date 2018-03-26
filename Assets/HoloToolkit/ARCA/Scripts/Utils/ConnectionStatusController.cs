// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;
using UnityEngine.UI;

namespace ARCA
{
    public class ConnectionStatusController : MonoBehaviour
    {
        [Tooltip("UI Textfield to display status")]
        public Text Text;

        [Tooltip("ARCANetworkDiscovery")]
        public ARCANetworkDiscovery ARCANetworkDiscovery;

        [Tooltip("ARCANetworkManager")]
        public ARCANetworkManager ARCANetworkManager;

        [Tooltip("WorldSync")]
        public WorldSync WorldSync;

        [Tooltip("AnchorLocated")]
        public AnchorLocated AnchorLocated;

        // Use this for initialization
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

        private void PromptShowToHoloLens()
        {
            Text.text = "Show to HoloLens";
        }

        private void PromptConnecting()
        {
            Text.text = "Connecting...";
        }

        private void PromptAlmostThere()
        {
            Text.text = "Almost there...";
        }

        private void OnDestroy()
        {
            // Unsuscruibe from events
            AnchorLocated.OnAnchorLocated -= PromptShowToHoloLens;
            ARCANetworkDiscovery.OnHololensSessionFound -= PromptConnecting;
            ARCANetworkDiscovery.OnHololensSessionFound -= PromptAlmostThere;
        }
    }
}
