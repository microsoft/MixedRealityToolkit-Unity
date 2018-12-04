using Pixie.AppSystems.StateObjects;
using UnityEngine;

namespace Pixie.Demos
{
    public class LoggedTypeBStateObject : StateObject<LoggedTypeBState>, IStateListener<LoggedTypeBState>, IAppListener<LoggedTypeBState>
    {
        public override bool IsUserType { get { return false; } }

        #region IStateListener

        public void OnStateInitialize(IStateObject<LoggedTypeBState> stateObj, LoggedTypeBState initState)
        {
            transform.position = stateObj.CurrentState.TargetPosition;
            posLastUpdate = stateObj.CurrentState.TargetPosition;
            timeLastUpdate = Time.time;
        }

        public void OnStateChange(IStateObject<LoggedTypeBState> stateObj, LoggedTypeBState oldState, LoggedTypeBState newState)
        {
            posLastUpdate = transform.position;
            timeLastUpdate = Time.time;
        }

        #endregion

        #region IAppListener

        public void OnUpdateApp(IStateObject<LoggedTypeBState> stateObj) { }

        public void OnUpdateAppServer(IStateObject<LoggedTypeBState> stateObj)
        {
            float lerpAmount = Mathf.Clamp01(Time.time - timeLastUpdate);
            transform.position = Vector3.Lerp(posLastUpdate, stateObj.CurrentState.TargetPosition, lerpAmount);

            // On the server, we update our positions every so often
            if (Time.time > serverTimeNextUpdate)
            {
                serverTimeNextUpdate = Time.time + Random.Range(0.5f, 2f);

                LoggedTypeBState currentState = stateObj.CurrentState;
                currentState.TargetPosition = Random.onUnitSphere * serverRoamRange;
                ChangeState(currentState);
            }
        }

        public void OnUpdateAppClient(IStateObject<LoggedTypeBState> stateObj)
        {
            float lerpAmount = Mathf.Clamp01(Time.time - timeLastUpdate);
            transform.position = Vector3.Lerp(posLastUpdate, stateObj.CurrentState.TargetPosition, lerpAmount);
        }

        #endregion

        // Server vars
        private Vector3 serverPrevPosition;
        private float serverTimeNextUpdate;
        private float serverRoamRange = 2f;
        // Client vars
        private Vector3 posLastUpdate;
        private float timeLastUpdate;
    }
}