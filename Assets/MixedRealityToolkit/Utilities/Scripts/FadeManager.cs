// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using System;
using UnityEngine;

#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#else
using UnityEngine.VR;
using UnityEngine.VR.WSA;
#endif
#endif

namespace MixedRealityToolkit.Utilities
{
    public class FadeManager : Singleton<FadeManager>
    {
        [Tooltip("If true, the FadeManager will update the shared material. Useful for fading multiple cameras that each render different layers.")]
        public bool FadeSharedMaterial;

        private Material fadeMaterial;
        private Color fadeColor = Color.black;

        private enum FadeState
        {
            idle = 0,
            fadingOut,
            FadingIn
        }

        public bool Busy
        {
            get
            {
                return currentState != FadeState.idle;
            }
        }

        private FadeState currentState;
        private float startTime;
        private float fadeOutTime;
        private Action fadeOutAction;
        private float fadeInTime;
        private Action fadeInAction;

        protected override void Awake()
        {
            // We want to check before calling base Awake
#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
            if (!HolographicSettings.IsDisplayOpaque)
#else
            if (VRDevice.isPresent)
#endif
            {
                Destroy(gameObject);
                return;
            }
#endif

            base.Awake();

            currentState = FadeState.idle;
            fadeMaterial = FadeSharedMaterial
                ? GetComponentInChildren<MeshRenderer>().sharedMaterial
                : GetComponentInChildren<MeshRenderer>().material;
        }

        private void Update()
        {
            if (Busy)
            {
                CalculateFade();
            }
        }

        private void CalculateFade()
        {
            float actionTime = currentState == FadeState.fadingOut ? fadeOutTime : fadeInTime;
            float timeBusy = Time.realtimeSinceStartup - startTime;
            float timePercentUsed = timeBusy / actionTime;
            if (timePercentUsed >= 1.0f)
            {
                Action callback = currentState == FadeState.fadingOut ? fadeOutAction : fadeInAction;
                if (callback != null)
                {
                    callback();
                }

                fadeColor.a = currentState == FadeState.fadingOut ? 1 : 0;
                fadeMaterial.color = fadeColor;

                currentState = currentState == FadeState.fadingOut ? FadeState.FadingIn : FadeState.idle;
                startTime = Time.realtimeSinceStartup;
            }
            else
            {
                fadeColor.a = currentState == FadeState.fadingOut ? timePercentUsed : 1 - timePercentUsed;
                fadeMaterial.color = fadeColor;
            }
        }

        protected override void OnDestroy()
        {
            if (fadeMaterial != null && !FadeSharedMaterial)
            {
                Destroy(fadeMaterial);
            }

            base.OnDestroy();
        }

        public bool DoFade(float _fadeOutTime, float _fadeInTime, Action _fadedOutAction, Action _fadedInAction)
        {
            if (Busy)
            {
                Debug.Log("Already fading");
                return false;
            }

            fadeOutTime = _fadeOutTime;
            fadeOutAction = _fadedOutAction;
            fadeInTime = _fadeInTime;
            fadeInAction = _fadedInAction;

            startTime = Time.realtimeSinceStartup;
            currentState = FadeState.fadingOut;
            return true;
        }
    }
}
