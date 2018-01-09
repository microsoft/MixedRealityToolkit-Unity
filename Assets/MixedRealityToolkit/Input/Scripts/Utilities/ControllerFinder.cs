// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA.Input;
#endif
#endif

namespace MixedRealityToolkit.Input.Utilities
{
    /// <summary>
    /// ControllerFinder is a base class providing simple event handling for getting/releasing MotionController Transforms
    /// </summary>
    public class ControllerFinder : MonoBehaviour
    {
        #region public members
        public MotionControllerInfo.ControllerElementEnum Element { get { return element; } }
        public Transform ElementTransform { get { return elementTransform; } private set { elementTransform = value; } }
        #endregion

    #if UNITY_WSA && UNITY_2017_2_OR_NEWER
        public virtual InteractionSourceHandedness Handedness { get { return handedness; } set { handedness = value; } }
        protected InteractionSourceHandedness handedness = InteractionSourceHandedness.Left;
    
    #endif

        #region private members
        protected MotionControllerInfo controller;
        protected MotionControllerInfo.ControllerElementEnum element = MotionControllerInfo.ControllerElementEnum.PointingPose;
        private Transform elementTransform;
        #endregion


        protected virtual void OnEnable()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            // Look if the controller has loaded.
            if (MotionControllerVisualizer.Instance.TryGetControllerModel(handedness, out controller))
            {
                AddControllerTransform(controller);
            }
#endif
            MotionControllerVisualizer.Instance.OnControllerModelLoaded += AddControllerTransform;
            MotionControllerVisualizer.Instance.OnControllerModelUnloaded += RemoveControllerTransform;
        }

        protected virtual void OnDisable()
        {
            if (MotionControllerVisualizer.IsInitialized)
            {
                MotionControllerVisualizer.Instance.OnControllerModelLoaded -= AddControllerTransform;
                MotionControllerVisualizer.Instance.OnControllerModelUnloaded -= RemoveControllerTransform;
            }
        }

        protected virtual void OnDestroy()
        {
            if (MotionControllerVisualizer.IsInitialized)
            {
                MotionControllerVisualizer.Instance.OnControllerModelLoaded -= AddControllerTransform;
                MotionControllerVisualizer.Instance.OnControllerModelUnloaded -= RemoveControllerTransform;
            }
        }

        protected virtual void AddControllerTransform(MotionControllerInfo newController)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (newController.Handedness == handedness)
            {
                if (!newController.TryGetElement(element, out elementTransform))
                {
                    Debug.LogError("Unable to find element of type " + element + " under controller " + newController.ControllerParent.name + "; not attaching.");
                    return;
                }
                controller = newController;
                // update elementTransform for consumption
                controller.TryGetElement(element, out elementTransform);
                ElementTransform = elementTransform;
            }
#endif
        }

        protected virtual void RemoveControllerTransform(MotionControllerInfo oldController)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (oldController.Handedness == handedness)
            {
                controller = null;
                ElementTransform = null;
            }
#endif
        }
    }
}