// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.Gaze;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Cursors
{
    /// <summary>
    /// A cursor that looks and acts more like the shell cursor.
    /// A two part cursor with visual feedback for all cursor states
    /// </summary>
    public class InteractiveMeshCursor : BaseCursor
    {
        [Tooltip("The ring or outer element")]
        public GameObject Ring;

        [Tooltip("Inner cursor element")]
        public GameObject Dot;

        [Tooltip("Point light")]
        public GameObject Light;

        [Tooltip("The scale factor to soften the distance scaling, we want the cursor to scale in the distance, but not disappear.")]
        public float DistanceScaleFactor = 0.3f;

        [Tooltip("The scale both elements will be at their default state")]
        public float DefaultScale = 0.75f;

        [Tooltip("The scale both elements will when pressed")]
        public float DownScale = 0.5f;

        [Tooltip("The scale both elements will a hand is visible")]
        public float UpScale = 1;

        [Tooltip("Time to scale between states")]
        public float ScaleTime = 0.5f;

        /// <summary>
        /// internal state and element management
        /// </summary>
        private float timer = 0;

        private bool isVisible = true;
        private bool hasHover = false;
        private bool hasHand = false;
        private bool isDown = false;

        private readonly Vector3 baseScale = Vector3.one;
        private Vector3 targetScale;

        /// <summary>
        /// The Current Input System for this Input Source.
        /// </summary>
        private static IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>());
        private static IMixedRealityInputSystem inputSystem = null;

        private Vector3 mAwakeScale;

        private void Awake()
        {
            mAwakeScale = transform.localScale;
        }

        /// <summary>
        /// Decide which element (ring or dot) should be visible and at what scale
        /// </summary>
        /// <param name="state"></param>
        public override void OnCursorStateChange(CursorStateEnum state)
        {
            base.OnCursorStateChange(state);

            // the cursor state has changed, reset the animation timer
            if (hasHand != IsHandDetected || isDown != IsPointerDown || hasHover != (TargetedObject != null))
            {
                timer = 0;
            }

            hasHand = IsHandDetected;
            isDown = IsPointerDown;
            hasHover = TargetedObject != null;

            targetScale = baseScale * DefaultScale;
            bool showRing = false;

            switch (state)
            {
                case CursorStateEnum.None:
                    break;
                case CursorStateEnum.Observe:
                    break;
                case CursorStateEnum.ObserveHover:
                    showRing = true;
                    break;
                case CursorStateEnum.Interact:
                    showRing = true;
                    targetScale = baseScale * DownScale;
                    break;
                case CursorStateEnum.InteractHover:
                    showRing = true;
                    targetScale = baseScale * UpScale;
                    break;
                case CursorStateEnum.Select:
                    targetScale = baseScale * UpScale;
                    break;
                case CursorStateEnum.Release:
                    break;
                case CursorStateEnum.Contextual:
                    break;
            }

            if (!isVisible)
            {
                return;
            }

            Ring.SetActive(showRing);
            Dot.SetActive(!showRing);

            // added observation of CursorModifier
            if (Pointer.CursorModifier != null && hasHover)
            {
                ElementVisibility(!Pointer.CursorModifier.GetCursorVisibility());
            }
        }

        /// <summary>
        /// scale the cursor elements
        /// </summary>
        protected override void UpdateCursorTransform()
        {
            base.UpdateCursorTransform();

            // animate scale of ring and dot
            if (timer < ScaleTime)
            {
                timer += Time.deltaTime;
                if (timer > ScaleTime)
                {
                    timer = ScaleTime;
                }

                Ring.transform.localScale = Vector3.Lerp(baseScale * DefaultScale, targetScale, timer / ScaleTime);
                Dot.transform.localScale = Vector3.Lerp(baseScale * DefaultScale, targetScale, timer / ScaleTime);
            }

            // handle scale of main cursor go
            float distance = Vector3.Distance(InputSystem.GazeProvider.GazeOrigin, transform.position);
            float smoothScaling = 1 - DefaultCursorDistance * DistanceScaleFactor;
            transform.localScale = mAwakeScale * (distance * DistanceScaleFactor + smoothScaling);
        }

        /// <summary>
        /// override the base class for custom visibility
        /// </summary>
        /// <param name="visible"></param>
        public override void SetVisibility(bool visible)
        {
            base.SetVisibility(visible);

            isVisible = visible;
            ElementVisibility(visible);

            if (visible)
            {
                OnCursorStateChange(CursorState);
            }
        }

        /// <summary>
        /// controls the visibility of cursor elements in one place
        /// </summary>
        /// <param name="visible"></param>
        private void ElementVisibility(bool visible)
        {
            if (Ring != null)
            {
                Ring.SetActive(visible);
            }

            if (Dot != null)
            {
                Dot.SetActive(visible);
            }

            if (Light != null)
            {
                Light.SetActive(visible);
            }
        }
    }
}
