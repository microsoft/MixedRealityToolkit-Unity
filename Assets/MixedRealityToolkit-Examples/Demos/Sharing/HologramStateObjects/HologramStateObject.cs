using Pixie.AppSystems.StateObjects;
using UnityEngine;

namespace Pixie.Demos
{
    public class HologramStateObject : StateObject<HologramState>, IStateListener<HologramState>, IAppListener<HologramState>
    {
        public override bool IsUserType { get { return false; } }

        #region IStateListener

        public void OnStateInitialize(IStateObject<HologramState> stateObj, HologramState initState)
        {
            transform.position = stateObj.CurrentState.Position;
        }

        public void OnStateChange(IStateObject<HologramState> stateObj, HologramState oldState, HologramState newState)
        {
            clientPrevPosition = transform.position;
            clientTimeLastUpdate = Time.time;
        }

        #endregion

        #region IAppListener

        public void OnUpdateApp(IStateObject<HologramState> stateObj) { }

        public void OnUpdateAppServer(IStateObject<HologramState> stateObj)
        {
            // On the server, we update our positions every so often
            if (Time.time > serverTimeNextUpdate)
            {
                serverTimeLastUpdate = Time.time;
                serverTimeNextUpdate = Time.time + Random.Range(0.5f, 2f);
                serverPrevPosition = transform.position;

                HologramState currentState = stateObj.CurrentState;
                currentState.Position = Random.onUnitSphere * serverRoamRange;
                ChangeState(currentState);
            }

            float lerpAmount = Mathf.Clamp01(Time.time - serverTimeLastUpdate);
            transform.position = Vector3.Lerp(serverPrevPosition, stateObj.CurrentState.Position, lerpAmount);
        }

        public void OnUpdateAppClient(IStateObject<HologramState> stateObj)
        {
            float lerpAmount = Mathf.Clamp01(Time.time - clientTimeLastUpdate);
            transform.position = Vector3.Lerp(clientPrevPosition, stateObj.CurrentState.Position, lerpAmount);
        }

        #endregion

        // Server vars
        private Vector3 serverPrevPosition;
        private float serverTimeNextUpdate;
        private float serverTimeLastUpdate;
        private float serverRoamRange = 2f;
        // Client vars
        private Vector3 clientPrevPosition;
        private float clientTimeLastUpdate;
    }
}