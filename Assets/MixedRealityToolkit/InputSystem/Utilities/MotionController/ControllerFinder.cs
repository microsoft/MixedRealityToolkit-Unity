// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace Microsoft.MixedReality.Toolkit.InputSystem.Utilities
{
    /// <summary>
    /// ControllerFinder is a base class providing simple event handling for getting/releasing MotionController Transforms
    /// </summary>
    public abstract class ControllerFinder : MotionControllerVisualizer
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
                if (handedness == (Handedness.Left & Handedness.Right) || handedness == Handedness.None)
                {
                    handedness = Handedness.Left;
                }

                return handedness;
            }
            set
            {
                if (value == (Handedness.Left & Handedness.Right) || value == Handedness.None)
                {
                    DebugUtilities.DebugLogWarning("Controller Finder must have a valid handedness.");
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

        public Transform ElementTransform { get; private set; }

        protected MotionControllerInfo ControllerInfo;

        #region Monobehaviour Implementation

        private void OnValidate()
        {
            if (handedness == (Handedness.Left & Handedness.Right) || handedness == Handedness.None)
            {
                DebugUtilities.DebugLogWarning($"{gameObject.name} Controller Finder must have a valid handedness.");
                handedness = Handedness.Left;
            }
        }

        protected virtual void OnEnable()
        {
#if UNITY_WSA
            // Look if the controller has loaded.
            if (TryGetControllerModel((InteractionSourceHandedness)Handedness, out ControllerInfo))
            {
                AddControllerTransform(ControllerInfo);
            }
#endif
            OnControllerModelLoaded += AddControllerTransform;
            OnControllerModelUnloaded += RemoveControllerTransform;
        }

        protected virtual void OnDisable()
        {
            OnControllerModelLoaded -= AddControllerTransform;
            OnControllerModelUnloaded -= RemoveControllerTransform;
        }

        protected virtual void OnDestroy()
        {
            OnControllerModelLoaded -= AddControllerTransform;
            OnControllerModelUnloaded -= RemoveControllerTransform;
        }

        #endregion Monobehaviour Implementation

        protected virtual void AddControllerTransform(MotionControllerInfo newController)
        {
#if UNITY_WSA
            if (newController.Handedness == (InteractionSourceHandedness)handedness)
            {
                Transform elementTransform;
                if (!newController.TryGetElement(element, out elementTransform))
                {
                    DebugUtilities.DebugLogError($"Unable to find element of type {element} under controller {newController.ControllerParent.name}; not attaching.");
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
#if UNITY_WSA
            if (oldController.Handedness == (InteractionSourceHandedness)handedness)
            {
                ControllerInfo = null;
                ElementTransform = null;
            }
#endif
        }
    }
}