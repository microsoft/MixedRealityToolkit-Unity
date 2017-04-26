// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Examples.Prototyping
{
    public class RotateToValue : MonoBehaviour
    {

        public enum LerpTypes { Linear, EaseIn, EaseOut, EaseInOut, Free }

        public GameObject TargetObject;
        public Quaternion TargetValue;
        public LerpTypes LerpType;
        public float LerpTime = 1f;
        public bool IsRunning = false;
        public bool ToLocalTransform = false;
        public UnityEvent OnComplete;

        private float mLerpTimeCounter;
        private Quaternion mStartValue;

        // Use this for initialization
        private void Awake()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }
            mStartValue = GetRotation();
        }

        public void StartRunning()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }

            mStartValue = GetRotation();
            mLerpTimeCounter = 0;
            IsRunning = true;
        }

        public void ResetTransform()
        {
            if (ToLocalTransform)
            {
                this.transform.localRotation = mStartValue;
            }
            else
            {
                this.transform.rotation = mStartValue;
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

        private Quaternion GetRotation()
        {
            return ToLocalTransform ? TargetObject.transform.localRotation : TargetObject.transform.rotation;
        }

        private Quaternion GetNewRotation(Quaternion currentRotation, float percent)
        {
            Quaternion newPosition = Quaternion.identity;
            switch (LerpType)
            {
                case LerpTypes.Linear:
                    newPosition = Quaternion.Lerp(mStartValue, TargetValue, percent);
                    break;
                case LerpTypes.EaseIn:
                    newPosition = Quaternion.Lerp(mStartValue, TargetValue, QuadEaseIn(0, 1, percent));
                    break;
                case LerpTypes.EaseOut:
                    newPosition = Quaternion.Lerp(mStartValue, TargetValue, QuadEaseOut(0, 1, percent));
                    break;
                case LerpTypes.EaseInOut:
                    newPosition = Quaternion.Lerp(mStartValue, TargetValue, QuadEaseInOut(0, 1, percent));
                    break;
                case LerpTypes.Free:
                    newPosition = Quaternion.Lerp(currentRotation, TargetValue, percent);
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
                    this.transform.localRotation = GetNewRotation(this.transform.localRotation, percent);
                }
                else
                {
                    this.transform.rotation = GetNewRotation(this.transform.rotation, percent);
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
                    this.transform.localRotation = GetNewRotation(this.transform.localRotation, LerpTime * Time.deltaTime);
                    IsRunning = this.transform.localRotation != TargetValue;
                }
                else
                {
                    this.transform.rotation = GetNewRotation(this.transform.rotation, LerpTime * Time.deltaTime);
                    IsRunning = this.transform.rotation != TargetValue;
                }

                if (IsRunning != wasRunning && !IsRunning)
                {
                    OnComplete.Invoke();
                }
            }
        }
    }
}
