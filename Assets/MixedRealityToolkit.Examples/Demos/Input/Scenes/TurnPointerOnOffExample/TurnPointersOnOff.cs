using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This example demonstrates how to turn pointers on and off by 
    /// specifying custom behaviors.
    /// </summary>
    public class TurnPointersOnOff : MonoBehaviour
    {
        List<Tuple<Type, Interactable>> pointerToggles = new List<Tuple<Type, Interactable>>();

        public void SetRayEnabled(bool isEnabled)
        {
            PointerUtils.SetRayPointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.Off,
                Handedness.Any);
        }

        public void SetGazeEnabled(bool isEnabled)
        {
            PointerUtils.SetGGVBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.Off);
        }

        public void SetGrabEnabled(bool isEnabled)
        {
            PointerUtils.SetGrabPointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.Off, Handedness.Any);
        }

        public void SetPokeEnabled(bool isEnabled)
        {
            PointerUtils.SetPokePointerBehavior(isEnabled ? PointerBehavior.Default : PointerBehavior.Off, Handedness.Any);
        }

        public void SetVR()
        {
            PointerUtils.SetPokePointerBehavior(PointerBehavior.Off, Handedness.Any);
            PointerUtils.SetGrabPointerBehavior(PointerBehavior.Off, Handedness.Any);
            PointerUtils.SetRayPointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetGGVBehavior(PointerBehavior.Off);
        }

        public void SetFingerOnly()
        {
            PointerUtils.SetPokePointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetGrabPointerBehavior(PointerBehavior.Off, Handedness.Any);
            PointerUtils.SetRayPointerBehavior(PointerBehavior.Off, Handedness.Any);
            PointerUtils.SetGGVBehavior(PointerBehavior.Off);
        }

        public void SetHoloLens1()
        {
            PointerUtils.SetPokePointerBehavior(PointerBehavior.Off, Handedness.Any);
            PointerUtils.SetGrabPointerBehavior(PointerBehavior.Off, Handedness.Any);
            PointerUtils.SetRayPointerBehavior(PointerBehavior.Off, Handedness.Any);
            PointerUtils.SetGGVBehavior(PointerBehavior.Default);
        }

        public void SetHoloLens2()
        {
            PointerUtils.SetPokePointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetGrabPointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetRayPointerBehavior(PointerBehavior.Default, Handedness.Any);
            PointerUtils.SetGGVBehavior(PointerBehavior.Off);
        }

        void Start()
        {
            IMixedRealityCapabilityCheck capabilityChecker = CoreServices.InputSystem as IMixedRealityCapabilityCheck;
            if (capabilityChecker != null)
            {
                if (capabilityChecker.CheckCapability(MixedRealityCapability.ArticulatedHand))
                {
                    SetHoloLens2();
                }
                else if (capabilityChecker.CheckCapability(MixedRealityCapability.MotionController))
                {
                    SetVR();
                }
                else
                {
                    SetHoloLens1();
                }
            }
            else
            {
                Debug.LogWarning("Input system does not implement IMixedRealityCapabilityCheck, not setting to any preset interaction");
            }

            PopulatePointerToggleList();
        }

        private void PopulatePointerToggleList()
        {
            foreach (var icl in FindObjectsOfType<Interactable>())
            {
                if (icl.Dimensions == 2)
                {
                    if (icl.gameObject.name.Contains("Ray"))
                    {
                        pointerToggles.Add(new Tuple<Type, Interactable>(typeof(LinePointer), icl));
                    }
                    else if (icl.gameObject.name.Contains("Poke"))
                    {
                        pointerToggles.Add(new Tuple<Type, Interactable>(typeof(PokePointer), icl));
                    }
                    else if (icl.gameObject.name.Contains("Grab"))
                    {
                        pointerToggles.Add(new Tuple<Type, Interactable>(typeof(SpherePointer), icl));
                    }
                    else if (icl.gameObject.name.Contains("Gaze"))
                    {
                        pointerToggles.Add(new Tuple<Type, Interactable>(typeof(GGVPointer), icl));
                    }
                }
            }
        }

        void Update()
        {
            foreach (var tuple in pointerToggles)
            {
                tuple.Item2.SetToggled(PointerUtils.GetPointerBehavior(tuple.Item1, Handedness.Any) != PointerBehavior.Off);
            }
        }

    }

}