// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.SharingWithUNET
{
    /// <summary>
    /// Represents a button on a list of sessions.  Tapping the button indicates the selected session
    /// </summary>
    public class SessionListButton : MonoBehaviour, IInputClickHandler
    {
        /// <summary>
        /// Information about the session attached to this button
        /// </summary>
        private NetworkDiscoveryWithAnchors.SessionInfo SessionInfo;

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
        private ScrollingSessionListUIController scrollingUIController;

        /// <summary>
        /// When the control gets started we need to do some setup.
        /// </summary>
        private void Awake()
        {
            textMesh = gameObject.GetComponentInChildren<TextMesh>();
            textMaterial = textMesh.GetComponent<MeshRenderer>().material;
            textColorId = Shader.PropertyToID("_Color");
            scrollingUIController = ScrollingSessionListUIController.Instance;
            if (scrollingUIController == null)
            {
                Debug.Log("without a scrolling UI control, this button can't work");
                Destroy(this);
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
        /// Sets the session information associated with this button
        /// </summary>
        /// <param name="sessionInfo">The session info</param>
        public void SetSessionInfo(NetworkDiscoveryWithAnchors.SessionInfo sessionInfo)
        {
            SessionInfo = sessionInfo;
            if (SessionInfo != null)
            {
                textMesh.text = string.Format("{0}\n{1}", SessionInfo.SessionName, SessionInfo.SessionIp);
                if (SessionInfo == scrollingUIController.SelectedSession)
                {
                    textMaterial.SetColor(textColorId, Color.blue);

                    textMesh.color = Color.blue;
                }
                else
                {
                    textMaterial.SetColor(textColorId, Color.yellow);
                    textMesh.color = Color.yellow;
                }
            }
        }

        /// <summary>
        /// When the user clicks a session this will route that information to the 
        /// scrolling UI control so it knows which session is selected.
        /// </summary>
        /// <param name="eventData">Information about the click.</param>
        public void OnInputClicked(InputClickedEventData eventData)
        {
            scrollingUIController.SetSelectedSession(SessionInfo);
            eventData.Use();
        }
    }
}
