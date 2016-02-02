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
        if (this.sessionMgr != null)
        {
            // Check to see if we aren't already in the desired session
            Session currentSession = this.sessionMgr.GetCurrentSession();

            if (currentSession == null ||                                                       // We aren't in any session
                currentSession.GetName().GetString() != this.SessionName ||                     // We're in the wrong session
                currentSession.GetMachineSessionState() == MachineSessionState.DISCONNECTED)    // We aren't joined or joining the right session
            {
                for (int i = 0; i < this.sessionMgr.GetSessionCount(); ++i)
                {
                    Session s = this.sessionMgr.GetSession(i);
                    if (s.GetName().GetString() == this.SessionName)
                    {
                        s.Join();
                        break;
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
