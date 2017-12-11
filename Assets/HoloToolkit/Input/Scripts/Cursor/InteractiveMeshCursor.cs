using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// A cursor that looks and acts more like the shell cursor.
    /// A two part cursor with visual feedback for all cursor states
    /// </summary>
    public class InteractiveMeshCursor : Cursor
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
        private float mTimer = 0;

        private bool mHasHover = false;
        private bool mHasHand = false;
        private bool mIsDown = false;
        private Vector3 mBaseScale = new Vector3(1, 1, 1);
        private Vector3 mTargetScale;
        private bool mIsVisible = true;

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
            if (mHasHand != this.IsHandVisible || mIsDown != this.IsInputSourceDown || mHasHover != (this.TargetedObject != null))
            {
                mTimer = 0;
            }

            mHasHand = this.IsHandVisible;
            mIsDown = this.IsInputSourceDown;
            mHasHover = this.TargetedObject != null;

            mTargetScale = mBaseScale * DefaultScale;
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
                    mTargetScale = mBaseScale * DownScale;
                    break;
                case CursorStateEnum.InteractHover:
                    showRing = true;
                    mTargetScale = mBaseScale * UpScale;
                    break;
                case CursorStateEnum.Select:
                    mTargetScale = mBaseScale * UpScale;
                    break;
                case CursorStateEnum.Release:
                    break;
                case CursorStateEnum.Contextual:
                    break;
                default:
                    break;
            }

            if (!mIsVisible)
            {
                return;
            }

            Ring.SetActive(showRing);
            Dot.SetActive(!showRing);

            // added observation of CursorModifier
            if (TargetedCursorModifier != null && mHasHover)
            {
                ElementVisibility(!TargetedCursorModifier.GetCursorVisibility());
            }
        }

        /// <summary>
        /// scale the cursor elements
        /// </summary>
        protected override void UpdateCursorTransform()
        {
            base.UpdateCursorTransform();

            // animate scale of ring and dot
            if (mTimer < ScaleTime)
            {
                mTimer += Time.deltaTime;
                if (mTimer > ScaleTime)
                {
                    mTimer = ScaleTime;
                }

                Ring.transform.localScale = Vector3.Lerp(mBaseScale * DefaultScale, mTargetScale, mTimer/ScaleTime);
                Dot.transform.localScale = Vector3.Lerp(mBaseScale * DefaultScale, mTargetScale, mTimer / ScaleTime);
            }

            // handle scale of main cursor go
            float distance = Vector3.Distance(GazeManager.Instance.GazeOrigin, transform.position);
            float smoothscaling = 1 - DefaultCursorDistance * DistanceScaleFactor;
            transform.localScale = mAwakeScale * (distance * DistanceScaleFactor + smoothscaling);
        }

        /// <summary>
        /// override the base class for custom visibility
        /// </summary>
        /// <param name="visible"></param>
        public override void SetVisibility(bool visible)
        {
            base.SetVisibility(visible);

            mIsVisible = visible;
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
