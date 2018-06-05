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
        public MotionControllerInfo.ControllerElementEnum Element
        {
            get { return element; }
            set { element = value; }
        }

        [SerializeField]
        private MotionControllerInfo.ControllerElementEnum element = MotionControllerInfo.ControllerElementEnum.PointingPose;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        public InteractionSourceHandedness Handedness
        {
            get { return handedness; }
            set
            {
                // We need to refresh which controller we're attached to if we switch handedness.
                if (handedness != value)
                {
                    handedness = value;
                    RefreshControllerTransform();
                }
            }
        }

        [SerializeField]
        private InteractionSourceHandedness handedness = InteractionSourceHandedness.Unknown;
#endif

        public Transform ElementTransform { get; private set; }

        protected MotionControllerInfo ControllerInfo;

        protected virtual void OnEnable()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (!MotionControllerVisualizer.ConfirmInitialized())
            {
                // The motion controller visualizer singleton could not be found.
                return;
            }
#endif

            // Look if the controller has loaded.
            RefreshControllerTransform();

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            MotionControllerVisualizer.Instance.OnControllerModelLoaded += AddControllerTransform;
            MotionControllerVisualizer.Instance.OnControllerModelUnloaded += RemoveControllerTransform;
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
                Handedness = newHandedness;
            }
        }
#endif

        /// <summary>
        /// Looks to see if the controller model already exists and registers it if so.
        /// </summary>
        protected virtual void TryAndAddControllerTransform()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            // Look if the controller was already loaded. This could happen if the
            // GameObject was instantiated at runtime and the model loaded event has already fired.
            if (!MotionControllerVisualizer.ConfirmInitialized())
            {
                // The motion controller visualizer singleton could not be found.
                return;
            }

            MotionControllerInfo newController;
            if (MotionControllerVisualizer.Instance.TryGetControllerModel(handedness, out newController))
            {
                AddControllerTransform(newController);
            }
#endif
        }

        protected virtual void AddControllerTransform(MotionControllerInfo newController)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (newController.Handedness == handedness && !newController.Equals(ControllerInfo))
            {
                Transform elementTransform;
                if (!newController.TryGetElement(element, out elementTransform))
                {
                    Debug.LogError("Unable to find element of type " + element + " under controller " + newController.ControllerParent.name + "; not attaching.");
                    return;
                }

                ControllerInfo = newController;
                // Update ElementTransform for consumption
                ElementTransform = elementTransform;

                OnControllerFound();
            }
#endif
        }

        protected virtual void RemoveControllerTransform(MotionControllerInfo oldController)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (oldController.Handedness == handedness)
            {
                OnControllerLost();

                ControllerInfo = null;
                ElementTransform = null;
            }
#endif
        }

        protected virtual void RefreshControllerTransform()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (ControllerInfo != null)
            {
                RemoveControllerTransform(ControllerInfo);
            }

            TryAndAddControllerTransform();
#endif
        }

        /// <summary>
        /// Override this method to act when the correct controller is actually found.
        /// This provides similar functionality to overriding AddControllerTransform,
        /// without the overhead of needing to check that handedness matches.
        /// </summary>
        protected virtual void OnControllerFound() { }

        /// <summary>
        /// Override this method to act when the correct controller is actually lost.
        /// This provides similar functionality to overriding AddControllerTransform,
        /// without the overhead of needing to check that handedness matches.
        /// </summary>
        protected virtual void OnControllerLost() { }
    }
}