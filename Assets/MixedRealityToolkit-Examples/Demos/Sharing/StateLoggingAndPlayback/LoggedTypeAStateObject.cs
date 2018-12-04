using Pixie.AppSystems.StateObjects;
using UnityEngine;

namespace Pixie.Demos
{
    public class LoggedTypeAStateObject : StateObject<LoggedTypeAState>, IStateListener<LoggedTypeAState>, IAppListener<LoggedTypeAState>
    {
        public override bool IsUserType { get { return false; } }

        #region IStateListener

        public void OnStateInitialize(IStateObject<LoggedTypeAState> stateObj, LoggedTypeAState initState)
        {
            transform.position = stateObj.CurrentState.TargetPosition;
            posLastUpdate = stateObj.CurrentState.TargetPosition;
            timeLastUpdate = Time.time;
        }

        public void OnStateChange(IStateObject<LoggedTypeAState> stateObj, LoggedTypeAState oldState, LoggedTypeAState newState)
        {
            posLastUpdate = transform.position;
            timeLastUpdate = Time.time;
        }

        #endregion

        #region IAppListener

        public void OnUpdateApp(IStateObject<LoggedTypeAState> stateObj) { }

        public void OnUpdateAppServer(IStateObject<LoggedTypeAState> stateObj)
        {
            float lerpAmount = Mathf.Clamp01(Time.time - timeLastUpdate);
            transform.position = Vector3.Lerp(posLastUpdate, stateObj.CurrentState.TargetPosition, lerpAmount);

            // On the server, we update our positions every so often
            if (Time.time > serverTimeNextUpdate)
            {
                serverTimeLastUpdate = Time.time;
                serverTimeNextUpdate = Time.time + Random.Range(0.5f, 2f);

                LoggedTypeAState currentState = stateObj.CurrentState;
                currentState.TargetPosition = Random.onUnitSphere * serverRoamRange;
                ChangeState(currentState);
            }
        }

        public void OnUpdateAppClient(IStateObject<LoggedTypeAState> stateObj)
        {
            float lerpAmount = Mathf.Clamp01(Time.time - timeLastUpdate);
            transform.position = Vector3.Lerp(posLastUpdate, stateObj.CurrentState.TargetPosition, lerpAmount);
        }

        #endregion

        // Server vars
        private float serverTimeNextUpdate;
        private float serverTimeLastUpdate;
        private float serverRoamRange = 2f;
        // Shared vars
        private Vector3 posLastUpdate;
        private float timeLastUpdate;
    }
}