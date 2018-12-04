using Pixie.AppSystems.Sessions;
using Pixie.AppSystems.TimeSync;
using Pixie.StateControl;
using System.Collections;
using UnityEngine;

namespace Pixie.Demos
{
    public class SessionStageRepeating : SessionStageBase
    {
        [Header("SessionStageRepeating settings:")]
        [SerializeField]
        private Color color1;
        [SerializeField]
        private Color color2;

        public override void OnStageEnter(ITimeSource timeSource, IAppStateReadWrite appState) { }

        public override IEnumerator TransitionIn(ITimeSource timeSource, IAppStateReadWrite appState)
        {
            foreach (SessionHologramState state in appState.GetStates<SessionHologramState>())
            {
                SessionHologramState newState = state;
                newState.Color = color1;
                appState.SetState<SessionHologramState>(newState);
            }

            WaitForSeconds wfs = new WaitForSeconds(1f);
            while (wfs.MoveNext())
                yield return null;
        }

        public override IEnumerator RunStage(ITimeSource timeSource, IAppStateReadWrite appState)
        {
            while (state == AppSystems.StageStateEnum.Running)
            {
                foreach (SessionHologramState state in appState.GetStates<SessionHologramState>())
                {
                    SessionHologramState newState = state;
                    newState.Color = (Random.value > 0.5f) ? color1 : color2;
                    appState.SetState<SessionHologramState>(newState);

                    WaitForSeconds wfs = new WaitForSeconds(0.25f);
                    while (wfs.MoveNext())
                        yield return null;
                }

                yield return null;
            }
        }

        public override IEnumerator TransitionOut(ITimeSource timeSource, IAppStateReadWrite appState)
        {
            yield break;
        }

        public override void OnStageExit(ITimeSource timeSource, IAppStateReadWrite appState) { }
    }
}
