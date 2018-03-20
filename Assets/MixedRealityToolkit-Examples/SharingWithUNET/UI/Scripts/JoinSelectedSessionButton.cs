// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using UnityEngine;

namespace MixedRealityToolkit.SharingWithUNET
{

    public class JoinSelectedSessionButton : MonoBehaviour, IInputClickHandler
    {
        /// <summary>
        /// Shader property id for the text color so we can change it when selected.
        /// </summary>
        private int textColorId;

        /// <summary>
        /// The text mesh so we can change the text to show the session name.
        /// </summary>
        private TextMesh textMesh;

        /// <summary>
        /// The material for the text so we can change the text color.
        /// </summary>
        private Material textMaterial;

        /// <summary>
        /// The control that manages which session is selected.
        /// </summary>
        private ScrollingSessionListUIController scrollingUIControl;

        /// <summary>
        /// Script which manages finding and joining sessions.
        /// </summary>
        private NetworkDiscoveryWithAnchors networkDiscovery;

        private void Start()
        {
            scrollingUIControl = ScrollingSessionListUIController.Instance;
            textMesh = transform.parent.GetComponentInChildren<TextMesh>();
            textMaterial = textMesh.GetComponent<MeshRenderer>().material;
            textColorId = Shader.PropertyToID("_Color");
            textMaterial.SetColor(textColorId, Color.grey);
            networkDiscovery = NetworkDiscoveryWithAnchors.Instance;
        }

        private void Update()
        {
            // If we are the client and have a selected session make the button text blue, 
            // otherwise make the button text grey to indicate that the button is inactive.
            if (networkDiscovery.running && networkDiscovery.isClient)
            {
                if (scrollingUIControl.SelectedSession != null)
                {
                    textMaterial.SetColor(textColorId, Color.blue);
                }
                else
                {
                    textMaterial.SetColor(textColorId, Color.grey);
                }
            }
            else
            {
                textMaterial.SetColor(textColorId, Color.grey);
            }
        }

        /// <summary>
        /// Called by unity when the object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            if (textMaterial != null)
            {
                Destroy(textMaterial);
                textMaterial = null;
            }
        }

        /// <summary>
        /// When the button is clicked try to join the selected session
        /// </summary>
        /// <param name="eventData"></param>
        public void OnInputClicked(InputClickedEventData eventData)
        {
            ScrollingSessionListUIController.Instance.JoinSelectedSession();
            eventData.Use();
        }
    }
}
