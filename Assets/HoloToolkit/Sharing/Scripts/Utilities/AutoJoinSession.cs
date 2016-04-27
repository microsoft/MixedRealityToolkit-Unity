// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Sharing
{
    public class AutoJoinSession : MonoBehaviour
    {
        // The name of the session to join
        public string SessionName = "Default";

        // local cached pointer to the SessionManager
        private SessionManager sessionManager;

        void Start()
        {
            // Get the SessionManager to use later.  Note that if this processes takes the role of a secondary client,
            // then the SessionManager will always be null
            if (SharingStage.Instance != null && SharingStage.Instance.Manager != null)
            {
                this.sessionManager = SharingStage.Instance.Manager.GetSessionManager();
            }
        }

        void Update()
        {
            // Get an instance of the SessionManager if one does not exist.
            if (sessionManager == null && SharingStage.Instance != null && SharingStage.Instance.Manager != null)
            {
                this.sessionManager = SharingStage.Instance.Manager.GetSessionManager();
            }

            // If we are a Primary Client and can join sessions...
            if (this.sessionManager != null && sessionManager.GetSessionCount() > 0)
            {
                // Check to see if we aren't already in the desired session
                Session currentSession = this.sessionManager.GetCurrentSession();

                if (currentSession == null ||                                                       // We aren't in any session
                    currentSession.GetName().GetString() != this.SessionName ||                     // We're in the wrong session
                    currentSession.GetMachineSessionState() == MachineSessionState.DISCONNECTED)    // We aren't joined or joining the right session
                {
                    Debug.Log("Session conn " + sessionManager.IsServerConnected() + " sessions: " + sessionManager.GetSessionCount());
                    Debug.Log("Looking for " + SessionName);
                    bool sessionFound = false;

                    for (int i = 0; i < this.sessionManager.GetSessionCount(); ++i)
                    {
                        Session s = this.sessionManager.GetSession(i);
                        Debug.Log(string.Format("session {0}", s.GetName().GetString()));

                        if (s.GetName().GetString() == this.SessionName)
                        {
                            s.Join();
                            sessionFound = true;
                            break;
                        }
                    }
                    if (sessionManager.IsServerConnected() && !sessionFound)
                    {
                        Debug.Log("Didn't find session, making a new one");
                        sessionManager.CreateSession(new XString(SessionName));

                        for (int i = 0; i < this.sessionManager.GetSessionCount(); ++i)
                        {
                            Session s = this.sessionManager.GetSession(i);
                            if (s.GetName().GetString() == this.SessionName)
                            {
                                s.Join();
                                Debug.Log("Joining our new session");
                                sessionFound = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}