// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Examples.Prototyping
{

    public class FadeMaterialColor : MonoBehaviour
    {

        public enum LerpTypes { Linear, EaseIn, EaseOut, EaseInOut }
        public GameObject TargetObject;
        public LerpTypes LerpType;
        public float LerpTime = 1f;
        public bool IsRunning = false;
        public UnityEvent OnComplete;

        public float GetCurrentAlpha { get { return mCurrentAlpha; } }

        private float mLerpTimeCounter;
        private Color mCachedColor;
        private float mCurrentAlpha;

        private Renderer mRenderer;
        private TextMesh mTextMesh;
        private bool mIsFadingIn;

        // Use this for initialization
        private void Awake()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }

            mRenderer = GetComponent<Renderer>();
            mTextMesh = GetComponent<TextMesh>();
        }

        public void FadeOut()
        {
            CacheColor();
            mCurrentAlpha = mCachedColor.a;
            mLerpTimeCounter = LerpTime - LerpTime * mCurrentAlpha;
            IsRunning = true;
            mIsFadingIn = false;
        }

        public void FadeIn()
        {
            CacheColor();
            mCurrentAlpha = mCachedColor.a;
            mLerpTimeCounter = LerpTime * mCurrentAlpha;
            IsRunning = true;
            mIsFadingIn = true;
        }

        public void ResetColor(Color color)
        {
            mCachedColor = color;
            mCurrentAlpha = mCachedColor.a;
            mLerpTimeCounter = LerpTime * mCurrentAlpha;

            if (mTextMesh != null)
            {
                mTextMesh.color = mCachedColor;
            }
            else
            {
                mRenderer.material.color = mCachedColor;
            }
        }

        public void StopRunning()
        {
            IsRunning = false;
        }

        private void CacheColor()
        {
            if (mTextMesh != null)
            {
                mCachedColor = mTextMesh.color;
            }
            else
            {
                mCachedColor = mRenderer.material.color;
            }
        }

        private void SetColor(float percent)
        {
            float newAlpha = 0;
            if (!mIsFadingIn)
            {
                percent = 1 - percent;
            }

            switch (LerpType)
            {
                case LerpTypes.Linear:
                    newAlpha = percent;
                    break;
                case LerpTypes.EaseIn:
                    newAlpha = QuadEaseIn(0, 1, percent);
                    break;
                case LerpTypes.EaseOut:
                    newAlpha = QuadEaseOut(0, 1, percent);
                    break;
                case LerpTypes.EaseInOut:
                    newAlpha = QuadEaseInOut(0, 1, percent);
                    break;
                default:
                    break;
            }

            mCachedColor.a = newAlpha;

            if (mTextMesh != null)
            {
                mTextMesh.color = mCachedColor;
            }
            else
            {
                mRenderer.material.color = mCachedColor;
            }
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
            if (IsRunning)
            {
                mLerpTimeCounter += Time.deltaTime;
                float percent = mLerpTimeCounter / LerpTime;

                SetColor(percent);

                if (percent >= 1)
                {
                    IsRunning = false;
                    OnComplete.Invoke();
                }
            }
        }

        private void OnDestroy()
        {
            if (mRenderer != null)
            {
                Destroy(mRenderer.material);
            }
        }
    }
}
