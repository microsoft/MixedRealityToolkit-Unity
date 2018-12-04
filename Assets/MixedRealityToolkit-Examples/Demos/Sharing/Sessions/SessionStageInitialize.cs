using Pixie.AppSystems.Sessions;
using Pixie.AppSystems.TimeSync;
using Pixie.StateControl;
using System.Collections;
using UnityEngine;

namespace Pixie.Demos
{
    public class SessionStageInitialize : SessionStageBase
    {
        [Header("Initialize settings")]
        [SerializeField]
        private int numHolograms = 5;
        [SerializeField]
        private float spawnRange = 2;

        public override void OnStageEnter(ITimeSource timeSource, IAppStateReadWrite appState) { }

        public override IEnumerator TransitionIn(ITimeSource timeSource, IAppStateReadWrite appState)
        {
            for (short hologramID = 0; hologramID < numHolograms; hologramID++)
            {
                SessionHologramState newHologramState = new SessionHologramState(hologramID);
                newHologramState.Position = Random.insideUnitSphere * spawnRange;
                appState.AddState<SessionHologramState>(newHologramState);

                WaitForSeconds wfs = new WaitForSeconds(0.5f);
                while (wfs.MoveNext())
                    yield return null;
            }
        }

        public override IEnumerator RunStage(ITimeSource timeSource, IAppStateReadWrite appState)
        {
            while (State == AppSystems.StageStateEnum.Running)
            {
                WaitForSeconds wfs = new WaitForSeconds(0.5f);
                while (wfs.MoveNext())
                    yield return null;
            }
        }

        public override IEnumerator TransitionOut(ITimeSource timeSource, IAppStateReadWrite appState)
        {
            WaitForSeconds wfs = new WaitForSeconds(0.5f);
            while (wfs.MoveNext())
                yield return null;
        }

        public override void OnStageExit(ITimeSource timeSource, IAppStateReadWrite appState) { }
    }
}