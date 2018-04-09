// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    public class FadeObject : MonoBehaviour
    {
        private float fadeTime = 0.5f;

        /// <summary>
        /// How long will the Fade effect last
        /// </summary>
        public float FadeTime
        {
            get
            {
                return fadeTime;
            }

            set
            {
                fadeTime = value;
            }
        }

        private bool autoFadeIn = false;

        /// <summary>
        /// Does the Fade effect occure automatically
        /// </summary>
        public bool AutoFadeIn
        {
            get
            {
                return autoFadeIn;
            }

            set
            {
                autoFadeIn = value;
            }
        }

        private float mFadeCounter = 0;
        private Color mCachedColor;
        private bool mFadingIn = true;
        private bool mIsFading = false;

        //cache material to prevent memory leak
        private Material mCachedMaterial;

        private void Awake()
        {
            mCachedMaterial = this.GetComponent<Renderer>().material;
            mCachedColor = mCachedMaterial.color;
        }

        private void Start()
        {
            if (AutoFadeIn)
            {
                FadeIn(true);
            }
        }

        private void OnEnable()
        {
            if (AutoFadeIn)
            {
                FadeIn(true);
            }
        }

        /// <summary>
        /// Begins the Fade in effect
        /// </summary>
        /// <param name="resetStartValue">should value return to original</param>
        public void FadeIn(bool resetStartValue)
        {
            if (mCachedMaterial != null)
            {
                mCachedColor = mCachedMaterial.color;
            }

            if (resetStartValue && mCachedMaterial)
            {
                mCachedColor.a = 0;
                mCachedMaterial.color = mCachedColor;
            }

            mFadeCounter = 0;
            mFadingIn = true;
        }

        /// <summary>
        /// Resets the color to original before fade effect
        /// </summary>
        /// <param name="value">value to set the alpha to</param>
        public void ResetFade(float value)
        {
            if (mCachedMaterial != null)
            {
                mCachedColor = mCachedMaterial.color;
                mCachedColor.a = value;
                mCachedMaterial.color = mCachedColor;
            }

            mFadeCounter = 0;
        }

        /// <summary>
        /// start the Fade out effect.
        /// </summary>
        /// <param name="resetStartValue">should original value be reset</param>
        public void FadeOut(bool resetStartValue)
        {
            if (mCachedMaterial != null)
            {
                mCachedColor = mCachedMaterial.color;
            }

            if (resetStartValue && mCachedMaterial)
            {
                mCachedColor.a = 1;
                mCachedMaterial.color = mCachedColor;
            }

            mFadeCounter = 0;
            mFadingIn = false;
        }

        private void Update()
        {
            if (mFadeCounter < FadeTime)
            {
                mFadeCounter += Time.deltaTime;
                if (mFadeCounter > FadeTime)
                {
                    mFadeCounter = FadeTime;
                }

                float percent = mFadeCounter / FadeTime;

                if (!mFadingIn)
                {
                    percent = 1 - percent;
                    if (percent < mCachedColor.a)
                    {
                        mCachedColor.a = percent;
                    }
                }
                else
                {
                    if (percent > mCachedColor.a)
                    {
                        mCachedColor.a = percent;
                    }
                }

                if (mCachedMaterial != null)
                {
                    mCachedMaterial.color = mCachedColor;
                }
            }
        }

        /// <summary>
        /// Event handler when object is deleted
        /// This cleans up cached material.
        /// </summary>
        public void OnDestroy()
        {
            Destroy(mCachedMaterial);
        }
    }
}