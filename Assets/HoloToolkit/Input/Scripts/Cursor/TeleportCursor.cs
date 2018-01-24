// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
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
            if (!canUpdateTransform) { return; }

            transform.position = pointer.TeleportTargetPosition;
            Quaternion rotation;

            // This is kind of a cheat - if the result is valid we can be reasonably sure the dot will permit using the camera's forward
            if (pointer.TeleportSurfaceResult == TeleportSurfaceResult.Valid)
            {
                Vector3 forward = CameraCache.Main.transform.forward;
                forward.y = 0f;
                rotation = Quaternion.LookRotation(forward.normalized, pointer.TeleportTargetNormal);
            }
            else
            {
                // Otherwise, just use the navigation normal directly
                rotation = Quaternion.FromToRotation(Vector3.up, pointer.TeleportTargetNormal);
            }

            // Smooth out rotation just a tad to prevent jarring transitions
            PrimaryCursorVisual.rotation = Quaternion.Lerp(PrimaryCursorVisual.rotation, rotation, 0.5f);

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

        #endregion

        #region ITeleport Implementation

        public void OnTeleportIntent(TeleportEventData eventData)
        {
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
