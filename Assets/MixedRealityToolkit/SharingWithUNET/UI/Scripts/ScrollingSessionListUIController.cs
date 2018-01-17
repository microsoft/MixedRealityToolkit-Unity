// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using MixedRealityToolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MixedRealityToolkit.SharingWithUNET
{
    /// <summary>
    /// Controls a scrollable list of sessions.
    /// </summary>
    public class ScrollingSessionListUIController : SingleInstance<ScrollingSessionListUIController>
    {
        /// <summary>
        /// Object which controls finding sessions so we know what to put on our buttons.
        /// </summary>
        private NetworkDiscoveryWithAnchors networkDiscovery;

        /// <summary>
        /// Current list of sessions.
        /// TODO: Currently these don't clean up if a session goes away...
        /// </summary>
        private Dictionary<string, NetworkDiscoveryWithAnchors.SessionInfo> sessionList;

        /// <summary>
        /// Keeps track of the current index that is the 'top' of the UI list
        /// to enable scrolling.
        /// </summary>
        private int SessionIndex = 0;

        /// <summary>
        /// List of session controls that will have the session info on them.
        /// </summary>
        public SessionListButton[] SessionControls;

        /// <summary>
        /// Keeps track of the session the user has currently selected.
        /// </summary>
        public NetworkDiscoveryWithAnchors.SessionInfo SelectedSession { get; private set; }

        private void Start()
        {
            // On the immersive device the UI is best placed a little closer than on a HoloLens
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
            {
                gameObject.GetComponent<SimpleTagalong>().TagalongDistance = 1;
            }
#endif

            // Register for events when sessions are found / joined.
            networkDiscovery = NetworkDiscoveryWithAnchors.Instance;
            networkDiscovery.SessionListChanged += NetworkDiscovery_SessionListChanged;
            networkDiscovery.ConnectionStatusChanged += NetworkDiscovery_ConnectionStatusChanged;
            ScrollSessions(0);
        }

        /// <summary>
        /// When we are connected we want to disable the UI
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">the event data</param>
        private void NetworkDiscovery_ConnectionStatusChanged(object sender, EventArgs e)
        {
            gameObject.SetActive(networkDiscovery.running && !networkDiscovery.isServer);
            // sets the UI to be active when not connected and disabled when connected.
            //SetChildren(networkDiscovery.running && !networkDiscovery.isServer);
        }

        /// <summary>
        /// Called when a session is discovered
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">the event data</param>
        private void NetworkDiscovery_SessionListChanged(object sender, EventArgs e)
        {
            sessionList = networkDiscovery.remoteSessions;
            SessionIndex = Mathf.Min(SessionIndex, sessionList.Count);

            // this will force a recalculation of the buttons.
            ScrollSessions(0);
        }

        /// <summary>
        /// Sometimes it is useful to disable rendering 
        /// </summary>
        /// <param name="Enabled"></param>
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

        /// <summary>
        /// Updates which session is the 'top' session in the list, and sets the 
        /// session buttons accordingly
        /// </summary>
        /// <param name="Direction">are we scrolling up, down, or not scrolling</param>
        public void ScrollSessions(int Direction)
        {
            int sessionCount = sessionList == null ? 0 : sessionList.Count;
            // Update the session index
            SessionIndex = Mathf.Clamp(SessionIndex + Direction, 0, Mathf.Max(0, sessionCount - SessionControls.Length));

            // Update all of the controls
            for (int index = 0; index < SessionControls.Length; index++)
            {
                if (SessionIndex + index < sessionCount)
                {
                    SessionControls[index].gameObject.SetActive(true);
                    NetworkDiscoveryWithAnchors.SessionInfo sessionInfo = sessionList.Values.ElementAt(SessionIndex + index);
                    SessionControls[index].SetSessionInfo(sessionInfo);
                }
                else
                {
                    SessionControls[index].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Sets the selected session
        /// </summary>
        /// <param name="sessionInfo">The session to set as selected</param>
        public void SetSelectedSession(NetworkDiscoveryWithAnchors.SessionInfo sessionInfo)
        {
            SelectedSession = sessionInfo;
            // Recalculating the session list so we can update the text colors.  
            ScrollSessions(0);
        }

        /// <summary>
        /// Joins the selected session if there is a selected session.
        /// </summary>
        public void JoinSelectedSession()
        {
            if (SelectedSession != null && networkDiscovery.running)
            {
                networkDiscovery.JoinSession(SelectedSession);
            }
        }
    }
}