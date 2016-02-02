using UnityEngine;
using HoloToolkit.XTools;

public class AutoJoinSession : MonoBehaviour
{
    // The name of the session to join
    public string SessionName = "Default";

    // local cached pointer to the SessionManager
    private SessionManager sessionMgr;

    void Start()
    {
        // Get the SessionManager to use later.  Note that if this processes takes the role of a secondary client,
        // then the SessionManager will always be null
        if (XToolsStage.Instance != null && XToolsStage.Instance.Manager != null)
        {
            this.sessionMgr = XToolsStage.Instance.Manager.GetSessionManager();
        }
    }

    void Update()
    {
        // If we are a Primary Client and can join sessions...
        if (this.sessionMgr != null && sessionMgr.GetSessionCount() > 0)
        {
            // Check to see if we aren't already in the desired session
            Session currentSession = this.sessionMgr.GetCurrentSession();

            if (currentSession == null ||                                                       // We aren't in any session
                currentSession.GetName().GetString() != this.SessionName ||                     // We're in the wrong session
                currentSession.GetMachineSessionState() == MachineSessionState.DISCONNECTED)    // We aren't joined or joining the right session
            {
                Debug.Log("Session conn " + sessionMgr.IsServerConnected() + " sessions: " + sessionMgr.GetSessionCount());
                Debug.Log("Looking for " + SessionName);
                bool sessionFound = false;

                for (int i = 0; i < this.sessionMgr.GetSessionCount(); ++i)
                {
                    Session s = this.sessionMgr.GetSession(i);
                    Debug.Log(string.Format("session {0}", s.GetName().GetString()));

                    if (s.GetName().GetString() == this.SessionName)
                    {
                        s.Join();
                        sessionFound = true;
                        break;
                    }
                }
                if (sessionMgr.IsServerConnected() && !sessionFound)
                {
                    Debug.Log("Didn't find session, making a new one");
                    sessionMgr.CreateSession(new XString(SessionName));

                    for (int i = 0; i < this.sessionMgr.GetSessionCount(); ++i)
                    {
                        Session s = this.sessionMgr.GetSession(i);
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
        else
        {
            if (XToolsStage.Instance != null && XToolsStage.Instance.Manager != null)
            {
                this.sessionMgr = XToolsStage.Instance.Manager.GetSessionManager();                
            }
        }
    }
}
