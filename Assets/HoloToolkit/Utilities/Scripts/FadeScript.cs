using System;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace HoloToolkit.Unity
{
    public class FadeScript : SingleInstance<FadeScript>
    {
        Material fadeMaterial;
        Color fadeColor = Color.black;

        enum FadeState
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

        FadeState currentState;
        float startTime;
        float fadeOutTime;
        Action fadeOutAction;
        float fadeInTime;
        Action fadeInAction;

        void Start()
        {
            if (!HolographicSettings.IsDisplayOpaque)
            {
                GetComponentInChildren<MeshRenderer>().enabled = false;
                Debug.Log("Removing unnecessary full screen effect from HoloLens");
                return;
            }

            currentState = FadeState.idle;
            fadeMaterial = GetComponentInChildren<MeshRenderer>().material;
        }

        void Update()
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

        void CalculateFade()
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
            if (fadeMaterial != null)
            {
                Destroy(fadeMaterial);
            }

            base.OnDestroy();
        }

        public bool DoFade(float fadeOutTime, float fadeInTime, Action FadedOutAction, Action FadedInAction)
        {
            if (Busy)
            {
                Debug.Log("Already fading");
                return false;
            }

            this.fadeOutTime = fadeOutTime;
            fadeOutAction = FadedOutAction;
            this.fadeInTime = fadeInTime;
            fadeInAction = FadedInAction;

            startTime = Time.realtimeSinceStartup;
            currentState = FadeState.fadingOut;
            return true;
        }
    }
}