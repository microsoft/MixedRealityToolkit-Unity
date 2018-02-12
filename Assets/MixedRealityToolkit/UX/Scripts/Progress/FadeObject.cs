using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using MixedRealityToolkit.InputModule;

#if UNITY_WSA || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif


namespace MixedRealityToolkit.Examples.InteractiveElements
{
    public class FadeObject : MonoBehaviour
    {
        public float FadeTime = 0.5f;
        public bool AutoFadeIn = false;

        private float mFadeCounter = 0;
        private Color mCachedColor;
        private bool mFadingIn = true;
        private Renderer mRenderer;
        private bool mIsFading = false;

        // Use this for initialization
        private void Awake()
        {
            mRenderer = this.GetComponent<Renderer>();
            mCachedColor = mRenderer.material.color;
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
            if (mRenderer != null)
            {
                mCachedColor = mRenderer.material.color;
            }

            if (resetStartValue)
            {
                mCachedColor.a = 0;
                mRenderer.material.color = mCachedColor;
            }

            mFadeCounter = 0;
            mFadingIn = true;
        }

        public void ResetFade(float value)
        {
            if (mRenderer != null)
            {
                mCachedColor = mRenderer.material.color;
                mCachedColor.a = value;
                mRenderer.material.color = mCachedColor;
            }

            mFadeCounter = 0;
        }

        public void FadeOut(bool resetStartValue)
        {
            if (mRenderer != null)
            {
                mCachedColor = mRenderer.material.color;
            }

            if (resetStartValue)
            {
                mCachedColor.a = 1;
                mRenderer.material.color = mCachedColor;
            }

            mFadeCounter = 0;
            mFadingIn = false;
        }

        // Update is called once per frame
        void Update()
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
                        mCachedColor.a = percent;
                }
                else
                {
                    if (percent > mCachedColor.a)
                        mCachedColor.a = percent;
                }

                if (mRenderer != null)
                {
                    mRenderer.material.color = mCachedColor;
                }
            }
        }
    }
}