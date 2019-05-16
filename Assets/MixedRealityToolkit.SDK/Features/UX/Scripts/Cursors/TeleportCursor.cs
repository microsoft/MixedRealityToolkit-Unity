// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Teleport
{
    public class TeleportCursor : AnimatedCursor, IMixedRealityTeleportHandler
    {
        [SerializeField]
        [Tooltip("Arrow Transform to point in the Teleporting direction.")]
        private Transform arrowTransform = null;

        private Vector3 cursorOrientation = Vector3.zero;

        #region IMixedRealityCursor Implementation

        /// <inheritdoc />
        public override IMixedRealityPointer Pointer
        {
            get { return pointer; }
            set
            {
                Debug.Assert(value.GetType() == typeof(TeleportPointer) ||
                             value.GetType() == typeof(ParabolicTeleportPointer),
                    "Teleport Cursor's Pointer must derive from a TeleportPointer type.");

                pointer = (TeleportPointer)value;
                pointer.BaseCursor = this;
                RegisterManagers();
            }
        }

        private TeleportPointer pointer;

        /// <inheritdoc />
        public override Vector3 Position => PrimaryCursorVisual.position;

        /// <inheritdoc />
        public override Quaternion Rotation => arrowTransform.rotation;

        /// <inheritdoc />
        public override Vector3 LocalScale => PrimaryCursorVisual.localScale;

        /// <inheritdoc />
        public override CursorStateEnum CheckCursorState()
        {
            if (CursorState != CursorStateEnum.Contextual)
            {
                if (pointer.IsInteractionEnabled)
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

        /// <inheritdoc />
        protected override void UpdateCursorTransform()
        {
            if (Pointer == null)
            {
                Debug.LogError($"[TeleportCursor.{name}] No Pointer has been assigned!");
                Destroy(gameObject);
                return;
            }

            FocusDetails focusDetails;

            if (!InputSystem.FocusProvider.TryGetFocusDetails(Pointer, out focusDetails))
            {
                if (InputSystem.FocusProvider.IsPointerRegistered(Pointer))
                {
                    Debug.LogError($"{gameObject.name}: Unable to get focus details for {pointer.GetType().Name}!");
                }
                else
                {
                    Debug.LogError($"{pointer.GetType().Name} has not been registered!");
                    Destroy(gameObject);
                }
                return;
            }

            if (pointer.Result == null) { return; }

            transform.position = pointer.Result.Details.Point;

            Vector3 forward = CameraCache.Main.transform.forward;
            forward.y = 0f;

            // Smooth out rotation just a tad to prevent jarring transitions
            PrimaryCursorVisual.rotation = Quaternion.Lerp(PrimaryCursorVisual.rotation, Quaternion.LookRotation(forward.normalized, Vector3.up), 0.5f);

            // Point the arrow towards the target orientation
            cursorOrientation.y = pointer.PointerOrientation;
            arrowTransform.eulerAngles = cursorOrientation;
        }

        #endregion IMixedRealityCursor Implementation

        #region IMixedRealityTeleportHandler Implementation

        /// <inheritdoc />
        public void OnTeleportRequest(TeleportEventData eventData)
        {
            OnCursorStateChange(CursorStateEnum.Observe);
        }

        /// <inheritdoc />
        public void OnTeleportStarted(TeleportEventData eventData)
        {
            OnCursorStateChange(CursorStateEnum.Release);
        }

        /// <inheritdoc />
        public void OnTeleportCompleted(TeleportEventData eventData) { }

        /// <inheritdoc />
        public void OnTeleportCanceled(TeleportEventData eventData)
        {
            OnCursorStateChange(CursorStateEnum.Release);
        }

        #endregion IMixedRealityTeleportHandler Implementation
    }
}
