// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.InputModule.Utilities
{
    /// <summary>
    /// Waits for a controller to be instantiated, then attaches itself to a specified element
    /// </summary>
    public class AttachToController : ControllerFinder
    {
        public bool SetChildrenInactiveWhenDetached = true;

        [SerializeField]
        protected Vector3 PositionOffset = Vector3.zero;

        [SerializeField]
        protected Vector3 RotationOffset = Vector3.zero;

        [SerializeField]
        protected Vector3 ScaleOffset = Vector3.one;

        [SerializeField]
        protected bool SetScaleOnAttach = false;

        public bool IsAttached { get; private set; }

        protected virtual void OnAttachToController() { }
        protected virtual void OnDetachFromController() { }

        protected override void OnEnable()
        {
            SetChildrenActive(false);

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            // Look if the controller has loaded.
            if (MotionControllerVisualizer.Instance.TryGetControllerModel(Handedness, out ControllerInfo))
            {
                AddControllerTransform(ControllerInfo);
            }

            MotionControllerVisualizer.Instance.OnControllerModelLoaded += AddControllerTransform;
            MotionControllerVisualizer.Instance.OnControllerModelUnloaded += RemoveControllerTransform;
#endif 
        }

        protected override void AddControllerTransform(MotionControllerInfo newController)
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (!IsAttached && newController.Handedness == Handedness)
            {
                base.AddControllerTransform(newController);

                SetChildrenActive(true);

                // Parent ourselves under the element and set our offsets
                transform.parent = ElementTransform;
                transform.localPosition = PositionOffset;
                transform.localEulerAngles = RotationOffset;

                if (SetScaleOnAttach)
                {
                    transform.localScale = ScaleOffset;
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
            if (IsAttached && oldController.Handedness == Handedness)
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