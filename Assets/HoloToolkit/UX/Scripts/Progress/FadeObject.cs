// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// This class describes and performs a fade.
    /// It can be used to create a fadeIn or a fadeOut or both.
    /// Additionally, it can be set to automatically begin when script awakens.
    /// </summary>
    public class FadeObject : MonoBehaviour
    {
        [SerializeField]
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

        private float fadeCounter = 0;
        private Color cachedColor;
        private bool fadingIn = true;

        //cache material to prevent memory leak
        private Material cachedMaterial;

        private void Awake()
        {
            cachedMaterial = this.GetComponent<Renderer>().material;
            cachedColor = cachedMaterial.color;
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
        /// <param name="resetFade">should value return to original</param>
        public void FadeIn(bool resetFade)
        {
            if (cachedMaterial != null)
            {
                cachedColor = cachedMaterial.color;
            }

            if (resetFade && cachedMaterial)
            {
                cachedColor.a = 0;
                cachedMaterial.color = cachedColor;
            }

            fadeCounter = 0;
            fadingIn = true;
        }

        /// <summary>
        /// Resets the color to original before fade effect
        /// </summary>
        /// <param name="value">value to set the alpha to</param>
        public void ResetFade(float value)
        {
            if (cachedMaterial != null)
            {
                cachedColor = cachedMaterial.color;
                cachedColor.a = value;
                cachedMaterial.color = cachedColor;
            }

            fadeCounter = 0;
        }

        /// <summary>
        /// start the Fade out effect.
        /// </summary>
        /// <param name="resetStartValue">should original value be reset</param>
        public void FadeOut(bool resetStartValue)
        {
            if (cachedMaterial != null)
            {
                cachedColor = cachedMaterial.color;
            }

            if (resetStartValue && cachedMaterial)
            {
                cachedColor.a = 1;
                cachedMaterial.color = cachedColor;
            }

            fadeCounter = 0;
            fadingIn = false;
        }

        private void Update()
        {
            if (fadeCounter < FadeTime)
            {
                fadeCounter += Time.deltaTime;
                if (fadeCounter > FadeTime)
                {
                    fadeCounter = FadeTime;
                }

                float percent = fadeCounter / FadeTime;

                if (!fadingIn)
                {
                    percent = 1 - percent;
                    if (percent < cachedColor.a)
                    {
                        cachedColor.a = percent;
                    }
                }
                else
                {
                    if (percent > cachedColor.a)
                    {
                        cachedColor.a = percent;
                    }
                }

                if (cachedMaterial != null)
                {
                    cachedMaterial.color = cachedColor;
                }
            }
        }

        /// <summary>
        /// Event handler when object is deleted
        /// This cleans up cached material.
        /// </summary>
        public void OnDestroy()
        {
            Destroy(cachedMaterial);
        }
    }
}