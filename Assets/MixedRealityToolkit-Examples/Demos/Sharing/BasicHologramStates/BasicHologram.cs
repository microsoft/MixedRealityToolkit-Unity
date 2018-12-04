using Pixie.Initialization;
using Pixie.StateControl;
using UnityEngine;

namespace Pixie.Demos
{
    public class BasicHologram : MonoBehaviour
    {
        public short HologramID { get { return hologramID; } }

        [SerializeField]
        private short hologramID = -1;

        private IAppStateReadOnly appState;

        private void Start()
        {
            ComponentFinder.FindInScenes<IAppStateReadOnly>(out appState);
        }

        public void Update()
        {
            // Wait until the state has been created
            if (!appState.Initialized || !appState.StateExists<BasicHologramState>(hologramID))
                return;

            BasicHologramState state = appState.GetState<BasicHologramState>(hologramID);
            GetComponent<Renderer>().material.color = state.Color;
        }
    }
}