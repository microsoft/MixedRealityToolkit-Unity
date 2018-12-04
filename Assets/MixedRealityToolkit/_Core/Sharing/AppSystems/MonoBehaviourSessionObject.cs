using Pixie.Core;

namespace Pixie.AppSystems
{
    public class MonoBehaviourSessionObject : MonoBehaviourSharingApp, ISessionObject
    {
        public virtual void OnSessionEnd() { }

        public virtual void OnSessionStageBegin() { }

        public virtual void OnSessionStageEnd() { }

        public virtual void OnSessionStart() { }

        public virtual void OnSessionUpdate(SessionState sessionState) { }
    }
}