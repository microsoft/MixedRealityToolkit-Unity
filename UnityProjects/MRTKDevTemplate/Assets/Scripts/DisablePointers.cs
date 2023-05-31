using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;
using Microsoft.MixedReality.Toolkit.Subsystems;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Example script that turns pointers on and off
    /// by activating and deactivating the interactors.
    /// </summary>
    [RequireComponent(typeof(PointerBehaviorControls))]
    [AddComponentMenu("MRTK/Examples/Disable Pointers")]
    public class DisablePointers : MonoBehaviour
    {
        public PressableButton GazeToggle;
        public PressableButton GrabToggle;
        public PressableButton PokeToggle;
        public PressableButton HandRayToggle;
        public PressableButton ControllerRayToggle;

        void Start()
        {
            ResetExample();
        }

        public void ResetExample()
        {
            
        }

        void Update()
        {
            
        }
    }
}
