//------------------------------------------------------------------------------ -
//MRTK - Quest
//https ://github.com/provencher/MRTK-Quest
//------------------------------------------------------------------------------ -
//
//MIT License
//
//Copyright(c) 2020 Eric Provencher
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files(the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions :
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//------------------------------------------------------------------------------ -


using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Teleport;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Custom teleport cursor built for MRTK-Quest
    /// </summary>
    public class CustomTeleportCursorHandler : MonoBehaviour, IMixedRealityTeleportHandler
    {
        private Vector3 cursorOrientation = Vector3.zero;

        [SerializeField]
        private CustomTeleportPointer pointer = null;

        [Header("Transform References")]
        [SerializeField]
        [Tooltip("Visual that is displayed when cursor is active normally")]
        protected Transform PrimaryCursorVisual = null;

        [SerializeField]
        [Tooltip("Ring visual on cursor")]
        protected Transform RingCursorVisual = null;

        [SerializeField]
        [Tooltip("Arrow Transform to point in the Teleporting direction.")]
        private Transform arrowTransform = null;


        #region IMixedRealityCursor Implementation

        /// <inheritdoc />
        public Vector3 Position => PrimaryCursorVisual.position;

        /// <inheritdoc />
        public Quaternion Rotation => arrowTransform.rotation;

        /// <inheritdoc />
        public Vector3 LocalScale => PrimaryCursorVisual.localScale;

        public CursorStateEnum CursorState { get; private set; } = CursorStateEnum.None;

        private float scaleTarget = 0f;
        private float scaleOrigin = 0f;
        private Vector3 startScale = Vector3.one;
        private float scaleSmoothTime = 0f;

        /// <inheritdoc />
        public CursorStateEnum CheckCursorState()
        {
            if (pointer.IsInteractionEnabled)
            {
                switch (pointer.TeleportSurfaceResult)
                {
                    case TeleportSurfaceResult.None:
                        return CursorStateEnum.Release;
                    case TeleportSurfaceResult.Invalid:
                        return CursorStateEnum.Observe;
                    case TeleportSurfaceResult.HotSpot:
                    case TeleportSurfaceResult.Valid:
                        return CursorStateEnum.ObserveHover;
                    default:
                        break;
                }
            }
            return CursorStateEnum.Release;
        }

        private bool CanUpdateCursor => pointer.IsActive && pointer.IsInteractionEnabled && pointer.Result != null;

        private void Awake()
        {
            startScale = PrimaryCursorVisual.localScale;
        }

        private void LateUpdate()
        {
            if (!CanUpdateCursor)
            {
                SetRenderersActive(false);
                ResetCursor();
                return;
            }

            UpdateCursorState();
            SetRenderersActive(true);
            UpdateCursorTransform();
        }

        private void SetRenderersActive(bool isActive)
        {
            if (PrimaryCursorVisual != null)
            {
                PrimaryCursorVisual.gameObject.SetActive(isActive);
            }
        }

        /// <summary>
        /// Internal update to check for cursor state changes
        /// </summary>
        private void UpdateCursorState()
        {
            CursorStateEnum newState = CheckCursorState();
            if (CursorState != newState)
            {
                OnCursorStateChange(newState);
            }
        }

        /// <inheritdoc />
        protected void UpdateCursorTransform()
        {
            transform.position = pointer.Result.Details.Point;

            Vector3 forward = CameraCache.Main.transform.forward;
            forward.y = 0f;

            // Smooth out rotation just a tad to prevent jarring transitions
            PrimaryCursorVisual.rotation = Quaternion.Lerp(PrimaryCursorVisual.rotation, Quaternion.LookRotation(forward.normalized, Vector3.up), 0.5f);

            // Smooth in cursor scale
            scaleSmoothTime += Time.deltaTime * 4f;

            float scaleAlpha = Mathf.SmoothStep(scaleOrigin, scaleTarget, Mathf.Clamp01(scaleSmoothTime));
            PrimaryCursorVisual.localScale = startScale * scaleAlpha;

            // Spin ring gently
            RingCursorVisual.localRotation = Quaternion.Euler(Mathf.Repeat(Time.time * 0.25f, 1f) * Vector3.up * 360f);

            // Point the arrow towards the target orientation
            cursorOrientation.y = pointer.PointerOrientation;
            arrowTransform.eulerAngles = cursorOrientation;
        }

        /// <summary>
        /// Override OnCursorState change to set the correct animation state for the cursor.
        /// </summary>
        public void OnCursorStateChange(CursorStateEnum state)
        {
            CursorState = state;
            scaleSmoothTime = 0f;
            switch (state)
            {
                case CursorStateEnum.Observe:
                case CursorStateEnum.ObserveHover:
                case CursorStateEnum.Interact:
                case CursorStateEnum.InteractHover:
                    scaleTarget = 1f;
                    scaleOrigin = 0f;
                    break;
                case CursorStateEnum.None:
                case CursorStateEnum.Release:
                case CursorStateEnum.Contextual:
                default:
                    scaleTarget = 0f;
                    scaleOrigin = PrimaryCursorVisual.localScale.x / startScale.x;
                    break;
            }
        }
        #endregion IMixedRealityCursor Implementation

        #region IMixedRealityTeleportHandler Implementation

        private void ResetCursor()
        {
            PrimaryCursorVisual.localScale = Vector3.zero;
            OnCursorStateChange(CursorStateEnum.Release);
        }

        /// <inheritdoc />
        public void OnTeleportRequest(TeleportEventData eventData)
        {
            ResetCursor();
        }

        /// <inheritdoc />
        public void OnTeleportStarted(TeleportEventData eventData)
        {
            ResetCursor();
        }

        /// <inheritdoc />
        public void OnTeleportCompleted(TeleportEventData eventData)
        {
            ResetCursor();
        }

        /// <inheritdoc />
        public void OnTeleportCanceled(TeleportEventData eventData)
        {
            ResetCursor();
        }
        #endregion IMixedRealityTeleportHandler Implementation
    }
}
