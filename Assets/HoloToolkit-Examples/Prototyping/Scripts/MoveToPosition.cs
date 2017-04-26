// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Examples.Prototyping
{

    public class MoveToPosition : MonoBehaviour
    {

        public enum LerpTypes { Linear, EaseIn, EaseOut, EaseInOut, Free }

        public GameObject TargetObject;
        public Vector3 TargetValue;
        public LerpTypes LerpType;
        public float LerpTime = 1f;
        public bool IsRunning = false;
        public bool ToLocalTransform = false;
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
            mStartValue = GetPosition();
        }

        public void StartRunning()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }

            mStartValue = GetPosition();
            mLerpTimeCounter = 0;
            IsRunning = true;
        }

        public void ResetTransform()
        {
            if (ToLocalTransform)
            {
                this.transform.localPosition = mStartValue;
            }
            else
            {
                this.transform.position = mStartValue;
            }
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

        private Vector3 GetPosition()
        {
            return ToLocalTransform ? TargetObject.transform.localPosition : TargetObject.transform.position;
        }

        private Vector3 GetNewPosition(Vector3 currentPosition, float percent)
        {
            Vector3 newPosition = Vector3.zero;
            switch (LerpType)
            {
                case LerpTypes.Linear:
                    newPosition = Vector3.Lerp(mStartValue, TargetValue, percent);
                    break;
                case LerpTypes.EaseIn:
                    newPosition = Vector3.Lerp(mStartValue, TargetValue, QuadEaseIn(0, 1, percent));
                    break;
                case LerpTypes.EaseOut:
                    newPosition = Vector3.Lerp(mStartValue, TargetValue, QuadEaseOut(0, 1, percent));
                    break;
                case LerpTypes.EaseInOut:
                    newPosition = Vector3.Lerp(mStartValue, TargetValue, QuadEaseInOut(0, 1, percent));
                    break;
                case LerpTypes.Free:
                    newPosition = Vector3.Lerp(currentPosition, TargetValue, percent);
                    break;
                default:
                    break;
            }

            return newPosition;
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

                if (ToLocalTransform)
                {
                    this.transform.localPosition = GetNewPosition(this.transform.localPosition, percent);
                }
                else
                {
                    this.transform.position = GetNewPosition(this.transform.position, percent);
                }

                if (percent >= 1)
                {
                    IsRunning = false;
                    OnComplete.Invoke();
                }
            }
            else if (LerpType == LerpTypes.Free)
            {
                bool wasRunning = IsRunning;
                if (ToLocalTransform)
                {
                    this.transform.localPosition = GetNewPosition(this.transform.localPosition, LerpTime * Time.deltaTime);
                    IsRunning = this.transform.localPosition != TargetValue;
                }
                else
                {
                    this.transform.position = GetNewPosition(this.transform.position, LerpTime * Time.deltaTime);
                    IsRunning = this.transform.localPosition != TargetValue;
                }

                if (IsRunning != wasRunning && !IsRunning)
                {
                    OnComplete.Invoke();
                }
            }
        }
    }
}
