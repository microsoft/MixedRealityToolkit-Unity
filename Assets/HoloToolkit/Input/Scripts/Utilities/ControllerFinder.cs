// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// ControllerFinder is a base class providing simple event handling for getting/releasing MotionController Transforms
    /// </summary>
    public abstract class ControllerFinder : MonoBehaviour
    {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        public MotionControllerInfo.ControllerElementEnum Element
        {
            get { return element; }
            set { element = value; }
        }

        [SerializeField]
        private MotionControllerInfo.ControllerElementEnum element = MotionControllerInfo.ControllerElementEnum.PointingPose;

        public InteractionSourceHandedness Handedness
        {
            get { return handedness; }
            set { handedness = value; }
        }

        [SerializeField]
        private InteractionSourceHandedness handedness = InteractionSourceHandedness.Left;

        public Transform ElementTransform { get { return elementTransform; } private set { elementTransform = value; } }
        private Transform elementTransform;

        protected MotionControllerInfo ControllerInfo;

        private bool started = false;
#endif

        protected virtual void OnEnable()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (!MotionControllerVisualizer.ConfirmInitialized())
            {
                // The motion controller visualizer singleton could not be found.
                return;
            }

            if (started)
            {
                CheckModelAlreadyLoaded();
            }

            MotionControllerVisualizer.Instance.OnControllerModelLoaded += AddControllerTransform;
            MotionControllerVisualizer.Instance.OnControllerModelUnloaded += RemoveControllerTransform;
#endif
        }

        protected virtual void Start()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            CheckModelAlreadyLoaded();
            started = true;
#endif
        }

        protected virtual void OnDisable()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (MotionControllerVisualizer.IsInitialized)
            {
                MotionControllerVisualizer.Instance.OnControllerModelLoaded -= AddControllerTransform;
                MotionControllerVisualizer.Instance.OnControllerModelUnloaded -= RemoveControllerTransform;
            }
#endif
        }

        protected virtual void OnDestroy()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (MotionControllerVisualizer.IsInitialized)
            {
                MotionControllerVisualizer.Instance.OnControllerModelLoaded -= AddControllerTransform;
                MotionControllerVisualizer.Instance.OnControllerModelUnloaded -= RemoveControllerTransform;
            }
#endif
        }

        /// <summary>
        /// Allows the object to change which controller it tracks, based on handedness.
        /// </summary>
        /// <param name="newHandedness">The new handedness to track. Does nothing if the handedness doesn't change.</param>
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        public void ChangeHandedness(InteractionSourceHandedness newHandedness)
        {
            if (newHandedness != handedness)
            {
                RemoveControllerTransform(ControllerInfo);
                handedness = newHandedness;
                CheckModelAlreadyLoaded();
            }
        }
#endif

        protected virtual void AddControllerTransform(MotionControllerInfo newController)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (newController.Handedness == handedness && !newController.Equals(ControllerInfo))
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

        /// <summary>
        /// Look if the controller was already loaded. This could happen if the
        /// GameObject was instantiated at runtime and the model loaded event has already fired.
        /// </summary>
        private void CheckModelAlreadyLoaded()
        {
            if (!MotionControllerVisualizer.ConfirmInitialized())
            {
                // The motion controller visualizer singleton could not be found.
                return;
            }

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            MotionControllerInfo newController;
            if (MotionControllerVisualizer.Instance.TryGetControllerModel(handedness, out newController))
            {
                AddControllerTransform(newController);
            }
#endif
        }
    }
}