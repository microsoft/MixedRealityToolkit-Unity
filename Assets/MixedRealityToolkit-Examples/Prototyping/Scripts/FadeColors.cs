// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace MixedRealityToolkit.Examples.Prototyping
{
    /// <summary>
    /// Fades colors (alpha value) on a material or TextMesh, requires a transparent material
    /// </summary>
    public class FadeColors : MonoBehaviour
    {
        /// <summary>
        /// The selectable ease types for the animation
        ///     Linear: steady progress
        ///     EaseIn: ramp up in speed
        ///     EaseOut: ramp down in speed
        ///     EaseInOut: ramp up then down in speed
        /// </summary>
        public enum LerpTypes { Linear, EaseIn, EaseOut, EaseInOut }

        [Tooltip("Fade ease type")]
        public GameObject TargetObject;

        [Tooltip("Fade ease type")]
        public LerpTypes LerpType;

        [Tooltip("Time to fade in seconds")]
        public float LerpTime = 1f;

        [Tooltip("Run by default? or Status")]
        public bool IsRunning = false;

        [Tooltip("Animation is complete!")]
        public UnityEvent OnComplete;

        /// <summary>
        /// Return the current alpha value
        /// </summary>
        public float GetCurrentAlpha { get { return mCurrentAlpha; } }

        // animation ticker
        private float mLerpTimeCounter;

        // color and alpha values
        private Color mCachedColor;
        private float mCurrentAlpha;

        // component targets
        private Material mMaterial;
        private TextMesh mTextMesh;

        // are we fading in or out?
        private bool mIsFadingIn = true;

        /// <summary>
        /// Get the renderer or text mesh
        /// </summary>
        private void Awake()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }

            Renderer renderer = GetComponent<Renderer>();
            mTextMesh = GetComponent<TextMesh>();

            if (renderer != null)
            {
                mMaterial = renderer.material;
            }
            else if (mTextMesh == null)
            {
                Debug.LogError("Renderer and TextMesh not found!");
                Destroy(this);
            }

            CacheColor();
        }

        /// <summary>
        /// Fade out
        /// </summary>
        public void FadeOut( bool resetStartValue = false)
        {
            CacheColor();
            mCurrentAlpha = mCachedColor.a;
            mLerpTimeCounter = LerpTime - LerpTime * mCurrentAlpha;
            IsRunning = true;
            mIsFadingIn = false;
            
            if (resetStartValue && mMaterial != null)
            {
                mCachedColor.a = 1;
                mMaterial.color = mCachedColor;
            }
        }

        /// <summary>
        /// Fade In
        /// </summary>
        public void FadeIn(bool resetStartValue = false)
        {
            CacheColor();
            mCurrentAlpha = mCachedColor.a;
            mLerpTimeCounter = LerpTime * mCurrentAlpha;
            IsRunning = true;
            mIsFadingIn = true;

            if (resetStartValue && mMaterial != null)
            {
                mCachedColor.a = 0;
                mMaterial.color = mCachedColor;
            }
        }

        /// <summary>
        /// Update the color of the component
        /// </summary>
        /// <param name="color"></param>
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
                mMaterial.color = mCachedColor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void ResetFade(float value)
        {
            CacheColor();

            if (mMaterial != null)
            {
                mCachedColor.a = value;
                mMaterial.color = mCachedColor;
            }

            mLerpTimeCounter = LerpTime * mCurrentAlpha;
        }

        /// <summary>
        /// stop the currently running animation
        /// </summary>
        public void StopRunning()
        {
            IsRunning = false;
        }

        /// <summary>
        /// Save the color so we can animate the alpha value
        /// </summary>
        private void CacheColor()
        {
            if (mTextMesh != null)
            {
                mCachedColor = mTextMesh.color;
            }
            else
            {
                if (mMaterial != null)
                {
                    mCachedColor = mMaterial.color;
                }
                else
                {
                    mCachedColor = new Color();
                }
            }
        }

        /// <summary>
        /// Apply the color and alpha to the material or TextMesh
        /// </summary>
        /// <param name="percent"></param>
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
                mMaterial.color = mCachedColor;
            }
        }

        // easing functions
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

        /// <summary>
        /// Animate the fading
        /// </summary>
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
            if (mMaterial != null)
            {
                Destroy(mMaterial);
            }
        }
    }
}
