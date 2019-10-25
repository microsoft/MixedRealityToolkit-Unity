// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A cursor that looks and acts more like the shell cursor.
    /// A two part cursor with visual feedback for all cursor states
    /// </summary>
    public class InteractiveMeshCursor : BaseCursor
    {
        [SerializeField]
        [Tooltip("The ring or outer element")]
        private GameObject ring = null;

        [SerializeField]
        [Tooltip("Inner cursor element")]
        private GameObject dot = null;

        [SerializeField]
        [Tooltip("The scale factor to soften the distance scaling, we want the cursor to scale in the distance, but not disappear.")]
        private float distanceScaleFactor = 0.3f;

        [SerializeField]
        [Tooltip("The scale both elements will be at their default state")]
        private float defaultScale = 0.75f;

        [SerializeField]
        [Tooltip("The scale both elements will when pressed")]
        private float downScale = 0.5f;

        [SerializeField]
        [Tooltip("The scale both elements will a hand is visible")]
        private float upScale = 1;

        [SerializeField]
        [Tooltip("Time to scale between states")]
        private float scaleTime = 0.5f;

        /// <summary>
        /// internal state and element management
        /// </summary>
        private float timer = 0;

        private bool isVisible = true;
        private bool hasHover = false;
        private bool hasHand = false;
        private bool isDown = false;

        private float ringDotTargetScale;
        private Vector3 initialScale;

        private void Awake()
        {
            initialScale = transform.localScale;
        }

        /// <summary>
        /// Decide which element (ring or dot) should be visible and at what scale
        /// </summary>
        public override void OnCursorStateChange(CursorStateEnum state)
        {
            base.OnCursorStateChange(state);

            // the cursor state has changed, reset the animation timer
            if (hasHand != IsSourceDetected || isDown != IsPointerDown || hasHover != (TargetedObject != null))
            {
                timer = 0;
            }

            hasHand = IsSourceDetected;
            isDown = IsPointerDown;
            hasHover = TargetedObject != null;

            ringDotTargetScale = defaultScale;
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
                    ringDotTargetScale = downScale;
                    break;
                case CursorStateEnum.InteractHover:
                    showRing = true;
                    ringDotTargetScale = upScale;
                    break;
                case CursorStateEnum.Select:
                    ringDotTargetScale = upScale;
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

            ring.SetActive(showRing);
            dot.SetActive(!showRing);

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
            if (timer < scaleTime)
            {
                timer += Time.deltaTime;
                if (timer > scaleTime)
                {
                    timer = scaleTime;
                }

                Vector3 useScale = Vector3.one * Mathf.Lerp(defaultScale, ringDotTargetScale, timer / scaleTime);
                ring.transform.localScale = useScale;
                dot.transform.localScale = useScale;
            }

            // handle scale of main cursor go
            float distance = Vector3.Distance(CoreServices.InputSystem.GazeProvider.GazeOrigin, transform.position);
            float smoothScaling = 1 - DefaultCursorDistance * distanceScaleFactor;
            transform.localScale = initialScale * (distance * distanceScaleFactor + smoothScaling);
        }

        /// <summary>
        /// override the base class for custom visibility
        /// </summary>
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
        private void ElementVisibility(bool visible)
        {
            if (ring != null)
            {
                ring.SetActive(visible);
            }

            if (dot != null)
            {
                dot.SetActive(visible);
            }
        }
    }
}
