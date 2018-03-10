// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.InteractiveElements
{
    public class FadeObject : MonoBehaviour
    {
        private float fadeTime = 0.5f;
        private bool autoFadeIn = false;

        private float mFadeCounter = 0;
        private Color mCachedColor;
        private bool mFadingIn = true;
        private bool mIsFading = false;

        //cache material to prevent memory leak
        private Material mCachedMaterial;

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

        public void OnDestroy()
        {
            Destroy(mCachedMaterial);
        }
    }
}