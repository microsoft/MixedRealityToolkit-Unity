// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Examples.Prototyping
{
    public class ScaleToValue : MonoBehaviour
    {

        public enum LerpTypes { Linear, EaseIn, EaseOut, EaseInOut, Free }

        public GameObject TargetObject;
        public Vector3 TargetValue;
        public LerpTypes LerpType;
        public float LerpTime = 1f;
        public bool IsRunning = false;
        public UnityEvent OnComplete;

        private float mLerpTimeCounter;
        private Vector3 mStartValue;

        // Use this for initialization
        private void Awake()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }
            mStartValue = GetScale();
        }

        public void StartRunning()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }

            mStartValue = GetScale();
            mLerpTimeCounter = 0;
            IsRunning = true;
        }

        public void ResetTransform()
        {
            this.transform.localScale = mStartValue;
            IsRunning = false;
            mLerpTimeCounter = 0;
        }

        public void Reverse()
        {
            TargetValue = mStartValue;
            mStartValue = TargetValue;
            mLerpTimeCounter = 0;
            IsRunning = true;
        }

        public void StopRunning()
        {
            IsRunning = false;
        }

        private Vector3 GetScale()
        {
            return TargetObject.transform.localScale;
        }

        private Vector3 GetNewScale(Vector3 currentScale, float percent)
        {
            Vector3 newScale = Vector3.one;
            switch (LerpType)
            {
                case LerpTypes.Linear:
                    newScale = Vector3.Lerp(mStartValue, TargetValue, percent);
                    break;
                case LerpTypes.EaseIn:
                    newScale = Vector3.Lerp(mStartValue, TargetValue, QuadEaseIn(0, 1, percent));
                    break;
                case LerpTypes.EaseOut:
                    newScale = Vector3.Lerp(mStartValue, TargetValue, QuadEaseOut(0, 1, percent));
                    break;
                case LerpTypes.EaseInOut:
                    newScale = Vector3.Lerp(mStartValue, TargetValue, QuadEaseInOut(0, 1, percent));
                    break;
                case LerpTypes.Free:
                    newScale = Vector3.Lerp(currentScale, TargetValue, percent);
                    break;
                default:
                    break;
            }

            return newScale;
        }

        public static float QuadEaseIn(float s, float e, float v)
        {
            return e * (v /= 1) * v + s;
        }

        public static float QuadEaseOut(float s, float e, float v)
        {
            return -e * (v /= 1) * (v - 2) + s;
        }

        public static float QuadEaseInOut(float s, float e, float v)
        {
            if ((v /= 0.5f) < 1)
                return e / 2 * v * v + s;

            return -e / 2 * ((--v) * (v - 2) - 1) + s;
        }

        // Update is called once per frame
        private void Update()
        {
            if (IsRunning && LerpType != LerpTypes.Free)
            {

                mLerpTimeCounter += Time.deltaTime;
                float percent = mLerpTimeCounter / LerpTime;

                this.transform.localScale = GetNewScale(this.transform.localScale, percent);

                if (percent >= 1)
                {
                    IsRunning = false;
                    OnComplete.Invoke();
                }
            }
            else if (LerpType == LerpTypes.Free)
            {
                bool wasRunning = IsRunning;
                this.transform.localScale = GetNewScale(this.transform.localScale, LerpTime * Time.deltaTime);
                IsRunning = this.transform.localScale != TargetValue;

                if (IsRunning != wasRunning && !IsRunning)
                {
                    OnComplete.Invoke();
                }
            }
        }
    }
}
