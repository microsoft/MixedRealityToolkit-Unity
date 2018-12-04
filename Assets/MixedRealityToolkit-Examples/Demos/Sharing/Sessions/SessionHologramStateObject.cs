using Pixie.AppSystems.StateObjects;
using UnityEngine;

namespace Pixie.Demos
{
    public class SessionHologramStateObject : StateObject<SessionHologramState>, IStateListener<SessionHologramState>
    {
        public override bool IsUserType { get { return false; } }

        [SerializeField]
        private Renderer renderer;
        
        public void OnStateInitialize(IStateObject<SessionHologramState> stateObj, SessionHologramState initState)
        {
            transform.position = initState.Position;
            renderer.material.color = initState.Color;
        }

        public void OnStateChange(IStateObject<SessionHologramState> stateObj, SessionHologramState oldState, SessionHologramState newState)
        {
            transform.position = newState.Position;
            renderer.material.color = newState.Color;
        }
    }
}