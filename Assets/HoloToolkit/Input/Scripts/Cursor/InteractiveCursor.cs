using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// A cursor that looks and acts more like the shell cursor.
    /// A two part cursor with visual feedback for all cursor states
    /// </summary>
    public class InteractiveCursor : Cursor
    {
        [Tooltip("The ring or outer element")]
        public GameObject Ring;

        [Tooltip("Inner cursor element")]
        public GameObject Dot;

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
        private bool mRingVisible = false;
        private bool mScaleDown = false;
        private float mTimer = 0;

        private bool mHasHover = false;
        private bool mHasHand = false;
        private bool mIsDown = false;
        private Vector3 mBaseScale = new Vector3(1, 1, 1);
        private Vector3 mTargetScale;
        private bool mIsVisible = true;

        /// <summary>
        /// Decide which element (ring or dot) should be visible and at what scale
        /// </summary>
        /// <param name="state"></param>
        public override void OnCursorStateChange(CursorStateEnum state)
        {
            base.OnCursorStateChange(state);

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

            if (TargetedCursorModifier != null && mHasHover)
            {
                Ring.SetActive(!TargetedCursorModifier.GetCursorVisibility());
                Dot.SetActive(!TargetedCursorModifier.GetCursorVisibility());
            }
        }

        /// <summary>
        /// scale the cursor elements
        /// </summary>
        protected override void UpdateCursorTransform()
        {
            base.UpdateCursorTransform();

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
        }

        /// <summary>
        /// override the base class for custom visibility
        /// </summary>
        /// <param name="visible"></param>
        public override void SetVisiblity(bool visible)
        {
            //base.SetVisiblity(visible);
            mIsVisible = visible;

            if (visible)
            {
                OnCursorStateChange(CursorState);
            }
            else
            {
                if (Ring != null && Dot != null)
                {
                    Ring.SetActive(visible);
                    Dot.SetActive(visible);
                }
            }
        }
    }
}
