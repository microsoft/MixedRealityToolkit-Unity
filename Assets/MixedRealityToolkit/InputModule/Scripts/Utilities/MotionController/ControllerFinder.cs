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

namespace MixedRealityToolkit.InputModule.Utilities
{
    /// <summary>
    /// ControllerFinder is a base class providing simple event handling for getting/releasing MotionController Transforms
    /// </summary>
    public abstract class ControllerFinder : MonoBehaviour
    {
        public MotionControllerInfo.ControllerElementEnum Element
        {
            get { return element; }
            set { element = value; }
        }

        [Header("Controller Finder")]
        [SerializeField]
        private MotionControllerInfo.ControllerElementEnum element = MotionControllerInfo.ControllerElementEnum.PointingPose;

        public Handedness Handedness
        {
            get
            {
                if (handedness == Handedness.Both || handedness == Handedness.None)
                {
                    handedness = Handedness.Left;
                }

                return handedness;
            }
            set
            {
                if (value == Handedness.Both || value == Handedness.None)
                {
                    Debug.LogWarning("Controller Finder must have a valid handedness.");
                    handedness = Handedness.Left;
                }
                else
                {
                    handedness = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("Must be Left or Right.")]
        private Handedness handedness = Handedness.Left;

        public Transform ElementTransform { get { return elementTransform; } private set { elementTransform = value; } }
        private Transform elementTransform;

        protected MotionControllerInfo ControllerInfo;

        #region Monobehaviour Implementation

        private void OnValidate()
        {
            if (handedness == Handedness.Both || handedness == Handedness.None)
            {
                Debug.LogWarning(gameObject.name + " Controller Finder must have a valid handedness.");
                handedness = Handedness.Left;
            }
        }

        protected virtual void OnEnable()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            // Look if the controller has loaded.
            if (MotionControllerVisualizer.Instance.TryGetControllerModel((InteractionSourceHandedness)Handedness, out ControllerInfo))
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

        #endregion Monobehaviour Implementation

        protected virtual void AddControllerTransform(MotionControllerInfo newController)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (newController.Handedness == (InteractionSourceHandedness)handedness)
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
            if (oldController.Handedness == (InteractionSourceHandedness)handedness)
            {
                ControllerInfo = null;
                ElementTransform = null;
            }
#endif
        }
    }
}