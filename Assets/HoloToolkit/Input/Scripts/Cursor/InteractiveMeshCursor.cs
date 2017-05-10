//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public class InteractiveMeshCursor : Cursor
    {
        public GameObject Ring;
        public GameObject Dot;
        public float DistanceScaleFactor = 0.3f;
        public float DefaultScale = 0.75f;
        public float DownScale = 0.5f;
        public float UpScale = 1;
        public float ScaleTime = 0.5f;
        
        private float mTimer;
        private bool mHasHover;
        private bool mHasHand;
        private bool mIsDown;
        private Vector3 mBaseScale = new Vector3(1, 1, 1);
        private Vector3 mTargetScale;
        private Vector3 mAwakeScale;

        private void Awake()
        {
            mAwakeScale = transform.localScale;
        }

        public override void OnCursorStateChange(CursorStateEnum state)
        {
            base.OnCursorStateChange(state);

            if (mHasHand != IsHandVisible || mIsDown != IsInputSourceDown || mHasHover != (TargetedObject != null))
            {
                mTimer = 0;
            }

            mHasHand = IsHandVisible;
            mIsDown = IsInputSourceDown;
            mHasHover = TargetedObject != null;

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

            float distance = Vector3.Distance(GazeManager.Instance.GazeOrigin, transform.position);
            float smoothscaling = 1 - DefaultCursorDistance * DistanceScaleFactor;
            transform.localScale = mAwakeScale * (distance * DistanceScaleFactor + smoothscaling);
        }
    }
}
