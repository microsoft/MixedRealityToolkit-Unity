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

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// ControllerFinder is a base class providing simple event handling for getting/releasing MotionController Transforms
    /// </summary>
    public abstract class ControllerFinder : MonoBehaviour
    {
        public MotionControllerInfo.ControllerElementEnum Element { get { return element; } }

        [Header("Controller Attachment Options")]
        [SerializeField]
        protected MotionControllerInfo.ControllerElementEnum element = MotionControllerInfo.ControllerElementEnum.PointingPose;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        public InteractionSourceHandedness Handedness { get { return handedness; } set { handedness = value; } }

        [SerializeField]
        protected InteractionSourceHandedness handedness = InteractionSourceHandedness.Left;
#endif

        public Transform ElementTransform { get { return elementTransform; } private set { elementTransform = value; } }
        private Transform elementTransform;

        protected MotionControllerInfo ControllerInfo;

        protected virtual void OnEnable()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            // Look if the controller has loaded.
            if (MotionControllerVisualizer.Instance.TryGetControllerModel(handedness, out ControllerInfo))
            {
                AddControllerTransform(ControllerInfo);
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
                ControllerInfo = newController;
                // update elementTransform for consumption
                ControllerInfo.TryGetElement(element, out elementTransform);
                ElementTransform = elementTransform;
            }
#endif
        }

        protected virtual void RemoveControllerTransform(MotionControllerInfo oldController)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (oldController.Handedness == handedness)
            {
                ControllerInfo = null;
                ElementTransform = null;
            }
#endif
        }
    }
}