// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Sharing
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

        private bool sessionCreationRequested = false;
        private string previousSessionName;

        void Start()
        {
            // Get the ServerSessionsTracker to use later.  Note that if this processes takes the role of a secondary client,
            // then the sessionsTracker will always be null
            if (SharingStage.Instance != null && SharingStage.Instance.Manager != null)
            {
                this.sessionsTracker = SharingStage.Instance.SessionsTracker;
            }
        }

        void Update()
        {
            if (previousSessionName != SessionName)
            {
                sessionCreationRequested = false;
                previousSessionName = SessionName;
            }

            // If we are a Primary Client and can join sessions...
            if (this.sessionsTracker != null && this.sessionsTracker.Sessions.Count > 0)
            {
                // Check to see if we aren't already in the desired session
                Session currentSession = this.sessionsTracker.GetCurrentSession();

                if (currentSession == null ||                                                       // We aren't in any session
                    currentSession.GetName().GetString() != this.SessionName ||                     // We're in the wrong session
                    currentSession.GetMachineSessionState() == MachineSessionState.DISCONNECTED)    // We aren't joined or joining the right session
                {
                    Debug.Log("Session conn " + this.sessionsTracker.IsServerConnected + " sessions: " + this.sessionsTracker.Sessions.Count);
                    Debug.Log("Looking for " + SessionName);
                    bool sessionFound = false;

                    for (int i = 0; i < this.sessionsTracker.Sessions.Count; ++i)
                    {
                        Session session = this.sessionsTracker.Sessions[i];

                        if (session.GetName().GetString() == this.SessionName)
                        {
                            this.sessionsTracker.JoinSession(session);
                            sessionFound = true;
                            break;
                        }
                    }
                    if (this.sessionsTracker.IsServerConnected && !sessionFound && !sessionCreationRequested)
                    {
                        Debug.Log("Didn't find session, making a new one");
                        if (this.sessionsTracker.CreateSession(new XString(SessionName)))
                        {
                            sessionCreationRequested = true;
                        }
                    }
                }
            }            
        }
    }
}