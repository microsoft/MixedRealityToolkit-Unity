using System;
using UnityEngine;

#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#else
using UnityEngine.VR.WSA;
#endif
#endif

namespace HoloToolkit.Unity
{
    public class FadeScript : Singleton<FadeScript>
    {
        [Tooltip("If true, the FadeScript will update the shared material. Useful for fading multiple cameras that each render different layers.")]
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

        private void Start()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (!HolographicSettings.IsDisplayOpaque)
            {
                GetComponentInChildren<MeshRenderer>().enabled = false;
                Debug.Log("Removing unnecessary full screen effect from HoloLens");
                return;
            }
#endif

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

            if (Input.GetKeyUp(KeyCode.F))
            {
                DoFade(3, 3, () => { Debug.Log("Done fading out"); }, () => { Debug.Log("Done fading in"); });
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