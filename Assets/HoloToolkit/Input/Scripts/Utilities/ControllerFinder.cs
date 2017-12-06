// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// ControllerFinder is a base class providing simple event handling for getting/releasing MotionController Transforms
    /// </summary>
    public class ControllerFinder : MonoBehaviour
    {
        #region public members
        public MotionControllerInfo.ControllerElementEnum Element { get { return element; } }
        public Transform ElementTransform { get; private set; }
        public bool IsAttached { get { return isAttached; } set { isAttached = value; } }
        #endregion

    #if UNITY_WSA && UNITY_2017_2_OR_NEWER
        public virtual InteractionSourceHandedness Handedness { get { return handedness; } set { handedness = value; } }
        protected InteractionSourceHandedness handedness = InteractionSourceHandedness.Left;
    
    #endif

        #region private members
        protected MotionControllerInfo controller;
        protected MotionControllerInfo.ControllerElementEnum element = MotionControllerInfo.ControllerElementEnum.PointingPose;
        protected Transform elementTransform;
        private bool isAttached = false;
        #endregion


        public virtual void OnEnable()
        {
    #if UNITY_WSA && UNITY_2017_2_OR_NEWER
            // Look if the controller has loaded.
            if (MotionControllerVisualizer.Instance.TryGetControllerModel(handedness, out controller))
            {
                AddControllerTransform(controller);
            }

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

        private void AddControllerTransform(MotionControllerInfo newController)
        {
            if (!isAttached && newController.Handedness == handedness)
            {
                if (!newController.TryGetElement(element, out elementTransform))
                {
                    Debug.LogError("Unable to find element of type " + element + " under controller " + newController.ControllerParent.name + "; not attaching.");
                    return;
                }
                controller = newController;
                // update elementTransform for cosnsumption
                controller.TryGetElement(element, out elementTransform);
                ElementTransform = elementTransform;

                isAttached = true;
            }
        }

        private void RemoveControllerTransform(MotionControllerInfo oldController)
        {
            if (isAttached && oldController.Handedness == handedness)
            {
                controller = null;
                isAttached = false;
            }

        }
    #endif
    }
}