// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public class InteractiveCursor : Cursor
    {
        [Tooltip ("Model to show when collision or source is detected")]
        public GameObject Ring;
        [Tooltip ("Default cursor model")]
        public GameObject Dot;

        [Tooltip ("Scale when no source or interaction is detected")]
        public float DefaultScale = 0.75f;
        [Tooltip ("Scale when a tap or gesture is detected")]
        public float DownScale = 0.5f;
        [Tooltip ("Scale when a source is detected")]
        public float UpScale = 1;
        [Tooltip ("The lerp time for scaling")]
        public float ScaleTime = 0.5f;

        /// <summary>
        /// Chached properties for tracking input states over time
        /// </summary>
        private bool mHasHover = false;
        private bool mHasHand = false;
        private bool mIsDown = false;

        /// <summary>
        /// Used for lerping the scale in responce to gesture or source
        /// </summary>
        private float mTimer = 0;

        /// <summary>
        /// Base scale for converting the scale values as floats to vector3
        /// </summary>
        private Vector3 mBaseScale = Vector3.one;

        /// <summary>
        /// Cached scale value to lerp toward
        /// </summary>
        private Vector3 mTargetScale;

        /// <summary>
        /// Cursor change events for deriving interaction states
        /// </summary>
        /// <param name="state"></param>
        public override void OnCursorStateChange(CursorStateEnum state)
        {
            base.OnCursorStateChange(state);

            // check for state changes - reset scale timer
            if (mHasHand != this.IsHandVisible || mIsDown != this.IsInputSourceDown || mHasHover != (this.TargetedObject != null))
            {
                mTimer = 0;
            }

            mHasHand = this.IsHandVisible;
            mIsDown = this.IsInputSourceDown;
            mHasHover = this.TargetedObject != null;

            // determine if the ring or dot should be visible
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
            
            Ring.SetActive(showRing);
            Dot.SetActive(!showRing);
            
        }

        /// <summary>
        /// lerp the scale for the ring and dot to the mTargetScale
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
    }
}
