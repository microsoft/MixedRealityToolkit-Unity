// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Waits for a controller to be instantiated, then attaches itself to a specified element
    /// </summary>
    public class AttachToController : ControllerFinder
    {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        [Header("AttachToController Elements")]
        [SerializeField]
        protected new InteractionSourceHandedness handedness = InteractionSourceHandedness.Left;
#endif
        [SerializeField]
        protected new MotionControllerInfo.ControllerElementEnum element = MotionControllerInfo.ControllerElementEnum.PointingPose;

        public bool SetChildrenInactiveWhenDetached = true;

        [SerializeField]
        protected Vector3 positionOffset = Vector3.zero;

        [SerializeField]
        protected Vector3 rotationOffset = Vector3.zero;

        [SerializeField]
        protected Vector3 scale = Vector3.one;

        [SerializeField]
        protected bool setScaleOnAttach = false;

        public bool IsAttached { get; private set; }

        protected virtual void OnAttachToController() { }
        protected virtual void OnDetachFromController() { }

        protected override void OnEnable()
        {
            SetChildrenActive(false);

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

        protected override void AddControllerTransform(MotionControllerInfo newController)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (!IsAttached && newController.Handedness == handedness)
            {
                base.AddControllerTransform(newController);

                SetChildrenActive(true);

                // Parent ourselves under the element and set our offsets
                transform.parent = ElementTransform;
                transform.localPosition = positionOffset;
                transform.localEulerAngles = rotationOffset;

                if (setScaleOnAttach)
                {
                    transform.localScale = scale;
                }

                // Announce that we're attached
                OnAttachToController();

                IsAttached = true;
            }
#endif
        }

        protected override void RemoveControllerTransform(MotionControllerInfo oldController)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (IsAttached && oldController.Handedness == handedness)
            {
                base.RemoveControllerTransform(oldController);

                OnDetachFromController();

                transform.parent = null;

                SetChildrenActive(false);

                IsAttached = false;
            }
#endif
        }

        private void SetChildrenActive(bool isActive)
        {
            if (SetChildrenInactiveWhenDetached)
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(isActive);
                }
            }
        }
    }
}