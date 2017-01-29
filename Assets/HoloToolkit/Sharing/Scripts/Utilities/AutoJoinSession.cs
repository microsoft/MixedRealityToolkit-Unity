// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Sharing.Utilities
{
    public class AutoJoinSession : MonoBehaviour
    {
        /// <summary>
        /// Name of the session to join.
        /// </summary>
        public string SessionName = "Default";

        /// <summary>
        /// Cached pointer to the sessions tracker.
        /// </summary>
        private ServerSessionsTracker sessionsTracker;

        private bool sessionCreationRequested;
        private string previousSessionName;

        private void Start()
        {
            // Get the ServerSessionsTracker to use later.  Note that if this processes takes the role of a secondary client,
            // then the sessionsTracker will always be null
            if (SharingStage.Instance != null && SharingStage.Instance.Manager != null)
            {
                sessionsTracker = SharingStage.Instance.SessionsTracker;
            }
        }

        private void Update()
        {
            if (previousSessionName != SessionName)
            {
                sessionCreationRequested = false;
                previousSessionName = SessionName;
            }

            // If we are a Primary Client and can join sessions...
            if (sessionsTracker != null && sessionsTracker.Sessions.Count > 0)
            {
                // Check to see if we aren't already in the desired session
                Session currentSession = sessionsTracker.GetCurrentSession();

                if (currentSession == null ||                                                    // We aren't in any session
                    currentSession.GetName().GetString() != SessionName ||                       // We're in the wrong session
                    currentSession.GetMachineSessionState() == MachineSessionState.DISCONNECTED) // We aren't joined or joining the right session
                {
                    Debug.LogFormat("Session conn {0} Sessions: {1}.", sessionsTracker.IsServerConnected.ToString(), sessionsTracker.Sessions.Count.ToString());
                    Debug.Log("Looking for " + SessionName);
                    bool sessionFound = false;

                    for (int i = 0; i < sessionsTracker.Sessions.Count; ++i)
                    {
                        Session session = sessionsTracker.Sessions[i];

                        if (session.GetName().GetString() == SessionName)
                        {
                            sessionsTracker.JoinSession(session);
                            sessionFound = true;
                            break;
                        }
                    }

                    if (sessionsTracker.IsServerConnected && !sessionFound && !sessionCreationRequested)
                    {
                        Debug.Log("Didn't find session, making a new one");
                        if (sessionsTracker.CreateSession(new XString(SessionName)))
                        {
                            sessionCreationRequested = true;
                        }
                    }
                }
            }
        }
    }
}