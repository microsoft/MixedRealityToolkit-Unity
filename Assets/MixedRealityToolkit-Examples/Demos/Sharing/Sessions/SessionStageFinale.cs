using Pixie.AppSystems.Sessions;
using Pixie.AppSystems.TimeSync;
using Pixie.StateControl;
using System.Collections;
using UnityEngine;

namespace Pixie.Demos
{
    public class SessionStageFinale : SessionStageBase
    {
        public override void OnStageEnter(ITimeSource timeSource, IAppStateReadWrite appState) { }

        public override IEnumerator TransitionIn(ITimeSource timeSource, IAppStateReadWrite appState)
        {
            foreach (SessionHologramState state in appState.GetStates<SessionHologramState>())
            {
                SessionHologramState newState = state;
                newState.Color = Color.white;
                appState.SetState<SessionHologramState>(newState);
            }

            WaitForSeconds wfs = new WaitForSeconds(1f);
            while (wfs.MoveNext())
                yield return null;
        }

        public override IEnumerator RunStage(ITimeSource timeSource, IAppStateReadWrite appState)
        {
            while (state == AppSystems.StageStateEnum.Running)
                yield return null;
        }

        public override IEnumerator TransitionOut(ITimeSource timeSource, IAppStateReadWrite appState)
        {
            WaitForSeconds wfs = new WaitForSeconds(1);
            while (wfs.MoveNext())
                yield return null;
        }

        public override void OnStageExit(ITimeSource timeSource, IAppStateReadWrite appState) { }
    }
}