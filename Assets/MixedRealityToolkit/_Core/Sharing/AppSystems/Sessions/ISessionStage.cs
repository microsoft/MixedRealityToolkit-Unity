using Pixie.AppSystems.TimeSync;
using Pixie.StateControl;
using System.Collections;

namespace Pixie.AppSystems.Sessions
{
    public interface ISessionStage : ISessionStageBase
    {
        IEnumerator Run(ITimeSource timeSource, IAppStateReadWrite appState);

        void ForceComplete();
    }
}