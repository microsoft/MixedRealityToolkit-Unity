// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.Focus;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.InputModule.Pointers;
using MixedRealityToolkit.InputModule.Utilities;
using System;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Cursors
{
    public class TeleportCursor : AnimatedCursor, ITeleportHandler
    {
        [SerializeField]
        [Tooltip("Arrow Transform to point in the Teleporting direction.")]
        private Transform arrowTransform;

        private TeleportPointer pointer;

        private bool canUpdateTransform;

        public override CursorStateEnum CheckCursorState()
        {
            if (CursorState != CursorStateEnum.Contextual)
            {
                if (pointer.InteractionEnabled)
                {
                    switch (pointer.TeleportSurfaceResult)
                    {
                        case TeleportSurfaceResult.None:
                            return CursorStateEnum.Release;
                        case TeleportSurfaceResult.Invalid:
                            return CursorStateEnum.ObserveHover;
                        case TeleportSurfaceResult.HotSpot:
                        case TeleportSurfaceResult.Valid:
                            return CursorStateEnum.ObserveHover;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return CursorStateEnum.Release;
            }

            return CursorStateEnum.Contextual;
        }

        protected override void UpdateCursorTransform()
        {
            Debug.Assert(Pointer != null, "No Pointer has been assigned!");

            FocusDetails focusDetails;
            if (!FocusManager.Instance.TryGetFocusDetails(Pointer, out focusDetails))
            {
                if (FocusManager.Instance.IsPointerRegistered(Pointer))
                {
                    Debug.LogErrorFormat("{0}: Unable to get focus details for {1}!", name, pointer.GetType().Name);
                }
                else
                {
                    Debug.LogErrorFormat("{0} has not been registered!", pointer.GetType().Name);
                }

                return;
            }

            if (!canUpdateTransform || pointer.Result == null) { return; }

            transform.position = pointer.TeleportTargetPosition;

            Vector3 forward = CameraCache.Main.transform.forward;
            forward.y = 0f;

            // Smooth out rotation just a tad to prevent jarring transitions
            PrimaryCursorVisual.rotation = Quaternion.Lerp(PrimaryCursorVisual.rotation, Quaternion.LookRotation(forward.normalized, Vector3.up), 0.5f);

            // Point the arrow towards the target orientation
            arrowTransform.eulerAngles = new Vector3(0f, pointer.PointerOrientation, 0f);
        }

        #region ICursor Implementation

        public override IPointer Pointer
        {
            get { return pointer; }
            set
            {
                Debug.Assert(value.GetType() == typeof(TeleportPointer) ||
                             value.GetType() == typeof(ParabolicTeleportPointer),
                    "Teleport Cursor's Pointer must derive from TeleportPointer type.");

                pointer = (TeleportPointer)value;
                pointer.BaseCursor = this;
                RegisterManagers();
            }
        }

        public override Vector3 Position { get { return PrimaryCursorVisual.position; } }
        public override Quaternion Rotation { get { return arrowTransform.rotation; } }
        public override Vector3 LocalScale { get { return PrimaryCursorVisual.localScale; } }

        #endregion ICursor Implementation

        #region ITeleport Implementation

        public void OnTeleportIntent(TeleportEventData eventData)
        {
            if (eventData.InputSource.Pointers == null) { return; }

            for (int i = 0; i < eventData.InputSource.Pointers.Length; i++)
            {
                if (eventData.InputSource.Pointers[i].PointerId == pointer.PointerId)
                {
                    canUpdateTransform = true;
                    OnCursorStateChange(CursorStateEnum.Observe);
                    break;
                }
            }
        }

        public void OnTeleportStarted(TeleportEventData eventData)
        {
            if (eventData.InputSource.Pointers == null) { return; }

            for (int i = 0; i < eventData.InputSource.Pointers.Length; i++)
            {
                if (eventData.InputSource.Pointers[i].PointerId == pointer.PointerId)
                {
                    canUpdateTransform = false;
                    OnCursorStateChange(CursorStateEnum.Release);
                    break;
                }
            }
        }

        public void OnTeleportCompleted(TeleportEventData eventData)
        {
            if (eventData.InputSource.Pointers == null) { return; }

            for (int i = 0; i < eventData.InputSource.Pointers.Length; i++)
            {
                if (eventData.InputSource.Pointers[i].PointerId == pointer.PointerId)
                {
                    canUpdateTransform = true;
                }
            }
        }

        public void OnTeleportCanceled(TeleportEventData eventData)
        {
            if (eventData.InputSource.Pointers == null) { return; }

            for (int i = 0; i < eventData.InputSource.Pointers.Length; i++)
            {
                if (eventData.InputSource.Pointers[i].PointerId == pointer.PointerId)
                {
                    canUpdateTransform = true;
                    OnCursorStateChange(CursorStateEnum.Release);
                    break;
                }
            }
        }

        #endregion ITeleport Implementation
    }
}
