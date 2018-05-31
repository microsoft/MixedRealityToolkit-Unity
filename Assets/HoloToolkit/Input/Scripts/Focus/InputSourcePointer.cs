// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Class implementing IPointingSource to demonstrate how to create a pointing source.
    /// This is consumed by SimpleSinglePointerSelector.
    /// </summary>
    public class InputSourcePointer : IPointingSource
    {
        public IInputSource InputSource { get; set; }

        public uint InputSourceId { get; set; }

        public BaseRayStabilizer RayStabilizer { get; set; }

        public bool OwnAllInput { get; set; }

        [Obsolete("Will be removed in a later version. Use Rays instead.")]
        public Ray Ray { get { return Rays[0]; } }

        public RayStep[] Rays
        {
            get
            {
                return rays;
            }
        }

        public PointerResult Result { get; set; }

        public float? ExtentOverride { get; set; }

        public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        public bool InteractionEnabled
        {
            get
            {
                return true;
            }
        }

        public bool FocusLocked { get; set; }

        public PointerLine PointerRay { get; set; }

        private RayStep[] rays = new RayStep[1] { new RayStep(Vector3.zero, Vector3.forward) };

        private bool selectPressed = false;

        [Obsolete("Will be removed in a later version. Use OnPreRaycast / OnPostRaycast instead.")]
        public void UpdatePointer()
        {
        }

        public virtual void OnPreRaycast()
        {
            if (InputSource == null)
            {
                rays[0] = default(RayStep);
            }
            else
            {
                Debug.Assert(InputSource.SupportsInputInfo(InputSourceId, SupportedInputInfo.Pointing), string.Format("{0} with id {1} does not support pointing!", InputSource, InputSourceId));

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
                // For visualization with controllers, we don't want to use the event-based data the InputManager has.
                // Instead, we query the source states manually here.
                InteractionSourceState[] currentReading = InteractionManager.GetCurrentReading();
                for (int i = 0; i < currentReading.Length; i++)
                {
                    InteractionSourceState sourceState = currentReading[i];

                    if (sourceState.source.id != InputSourceId)
                    {
                        continue;
                    }

                    selectPressed = sourceState.selectPressed;

                    Vector3 position;
                    Vector3 forward;

                    if (!sourceState.sourcePose.TryGetPosition(out position))
                    {
                        return;
                    }

                    if (!sourceState.sourcePose.TryGetForward(out forward, InteractionSourceNode.Pointer))
                    {
                        return;
                    }

                    if (CameraCache.Main.transform.parent != null)
                    {
                        position = CameraCache.Main.transform.parent.TransformPoint(position);
                        forward = CameraCache.Main.transform.parent.TransformDirection(forward);
                    }

                    rays[0].CopyRay(new Ray(position, forward), FocusManager.Instance.GetPointingExtent(this));
                }
#else
                Ray pointingRay;
                if (InputSource.TryGetPointingRay(InputSourceId, out pointingRay))
                {
                    rays[0].CopyRay(pointingRay, FocusManager.Instance.GetPointingExtent(this));
                }
#endif
            }

            if (RayStabilizer != null)
            {
                RayStabilizer.UpdateStability(rays[0].Origin, rays[0].Direction);
                rays[0].CopyRay(RayStabilizer.StableRay, FocusManager.Instance.GetPointingExtent(this));
            }
        }

        public virtual void OnPostRaycast()
        {
            if (PointerRay != null)
            {
                PointerRay.UpdateRenderedLine(rays, Result, selectPressed, FocusManager.Instance.GetPointingExtent(this));
            }
        }

        public bool OwnsInput(BaseEventData eventData)
        {
            return (OwnAllInput || InputIsFromSource(eventData));
        }

        public bool InputIsFromSource(BaseEventData eventData)
        {
            var inputData = (eventData as IInputSourceInfoProvider);

            return (inputData != null)
                && (inputData.InputSource == InputSource)
                && (inputData.SourceId == InputSourceId);
        }
    }
}
