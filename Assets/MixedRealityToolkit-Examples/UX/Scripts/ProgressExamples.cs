// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.UX.Progress;
using System.Collections;
using UnityEngine;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#endif


namespace MixedRealityToolkit.Examples.UX
{
    public class ProgressExamples : MonoBehaviour
    {
        [SerializeField]
        private GameObject objectToScaleBasedOnHMD = null;

        [SerializeField]
        private Vector3 scaleIfImmersive = new Vector3(1.3f, 1.3f, 1f);

        [Header("How long to spend on each stage of loading")]
        [SerializeField]
        private float leadInTime = 1.5f;

        [SerializeField]
        private float loadingTime = 5f;

        [SerializeField]
        private float finishTime = 1.5f;

        [Header("Set these to override the defaults set in the ProgressIndicator prefab")]
        [SerializeField]
        private GameObject loadingPrefab = null;

        [SerializeField]
        private Texture2D loadingIcon = null;

        [Header("Messages displayed during loading")]
        [SerializeField]
        private string leadInMessage = "Lead in Message";
        [SerializeField]
        private string loadTextMessage = "Loading with message only";
        [SerializeField]
        private string loadOrbsMessage = "Loading with Orbs";
        [SerializeField]
        private string loadIconMessage = "Loading with Icon";
        [SerializeField]
        private string loadPrefabMessage = "Loading with Prefab";
        [SerializeField]
        private string loadProgressMessage = "Loading with Progress";
        [SerializeField]
        private string loadProgressBarMessage = "Loading with Bar";
        [SerializeField]
        private string finishMessage = "Finished!";

        [SerializeField]
        private GameObject buttonCollection = null;

        public float LeadInTime
        {
            get
            {
                return leadInTime;
            }

            set
            {
                leadInTime = value;
            }
        }

        public float LoadingTime
        {
            get
            {
                return loadingTime;
            }

            set
            {
                loadingTime = value;
            }
        }

        public float FinishTime
        {
            get
            {
                return finishTime;
            }

            set
            {
                finishTime = value;
            }
        }

        public GameObject LoadingPrefab
        {
            get
            {
                return loadingPrefab;
            }

            set
            {
                loadingPrefab = value;
            }
        }

        public Texture2D LoadingIcon
        {
            get
            {
                return loadingIcon;
            }

            set
            {
                loadingIcon = value;
            }
        }

        public GameObject ButtonCollection
        {
            get
            {
                return buttonCollection;
            }

            set
            {
                buttonCollection = value;
            }
        }

        public string LeadInMessage
        {
            get
            {
                return leadInMessage;
            }

            set
            {
                leadInMessage = value;
            }
        }

        public string LoadTextMessage
        {
            get
            {
                return loadTextMessage;
            }

            set
            {
                loadTextMessage = value;
            }
        }

        public string LoadOrbsMessage
        {
            get
            {
                return loadOrbsMessage;
            }

            set
            {
                loadOrbsMessage = value;
            }
        }

        public string LoadIconMessage
        {
            get
            {
                return loadIconMessage;
            }

            set
            {
                loadIconMessage = value;
            }
        }

        public string LoadPrefabMessage
        {
            get
            {
                return loadPrefabMessage;
            }

            set
            {
                loadPrefabMessage = value;
            }
        }

        public string LoadProgressMessage
        {
            get
            {
                return loadProgressMessage;
            }

            set
            {
                loadProgressMessage = value;
            }
        }

        public string LoadProgressBarMessage
        {
            get
            {
                return loadProgressBarMessage;
            }

            set
            {
                loadProgressBarMessage = value;
            }
        }

        public string FinishMessage
        {
            get
            {
                return finishMessage;
            }

            set
            {
                finishMessage = value;
            }
        }

        private void Start()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (objectToScaleBasedOnHMD)
            {
                if (HolographicSettings.IsDisplayOpaque)
                {
                    objectToScaleBasedOnHMD.transform.localScale = scaleIfImmersive;
                }
            }
#endif
        }

        public void LaunchProgress(IndicatorStyleEnum indicatorStyle, ProgressStyleEnum progressStyle)
        {
            if (ProgressIndicator.Instance.IsLoading)
            {
                return;
            }

            switch (indicatorStyle)
            {
                case IndicatorStyleEnum.None:
                    //progressbar examples all assume IndicatorStyleEnum = None
                    switch (progressStyle)
                    {
                        case ProgressStyleEnum.Percentage:
                            ProgressIndicator.Instance.Open(
                                IndicatorStyleEnum.None,
                                ProgressStyleEnum.Percentage,
                                MessageStyleEnum.Visible,
                                LeadInMessage);
                            StartCoroutine(LoadOverTime(LoadProgressMessage));
                            break;

                        case ProgressStyleEnum.ProgressBar:
                            ProgressIndicator.Instance.Open(
                                IndicatorStyleEnum.None,
                                ProgressStyleEnum.ProgressBar,
                                MessageStyleEnum.Visible,
                                LeadInMessage);
                            StartCoroutine(LoadOverTime(LoadProgressBarMessage));
                            break;

                        case ProgressStyleEnum.None:
                            ProgressIndicator.Instance.Open(
                            IndicatorStyleEnum.None,
                            ProgressStyleEnum.None,
                            MessageStyleEnum.Visible,
                            LeadInMessage);
                            StartCoroutine(LoadOverTime(LoadTextMessage));
                            break;
                    }
                    break;

                case IndicatorStyleEnum.AnimatedOrbs:
                    ProgressIndicator.Instance.Open(
                             IndicatorStyleEnum.AnimatedOrbs,
                             ProgressStyleEnum.None,
                             MessageStyleEnum.Visible,
                             LeadInMessage);
                    StartCoroutine(LoadOverTime(LoadOrbsMessage));
                    break;

                case IndicatorStyleEnum.StaticIcon:
                    ProgressIndicator.Instance.Open(
                        IndicatorStyleEnum.StaticIcon,
                        ProgressStyleEnum.None,
                        MessageStyleEnum.Visible,
                        LeadInMessage,
                        null);
                    StartCoroutine(LoadOverTime(LoadIconMessage));
                    break;

                case IndicatorStyleEnum.Prefab:
                    ProgressIndicator.Instance.Open(
                        IndicatorStyleEnum.Prefab,
                        ProgressStyleEnum.None,
                        MessageStyleEnum.Visible,
                        LeadInMessage,
                        LoadingPrefab);
                    StartCoroutine(LoadOverTime(LoadPrefabMessage));
                    break;
            }
        }

        protected IEnumerator LoadOverTime(string message)
        {
            yield return new WaitForSeconds(LeadInTime);

            // While we're in the loading period, update progress and message in 1/4 second intervals
            // Displayed progress is smoothed out so you don't have to update every frame
            float startTime = Time.time;
            while (Time.time < startTime + LoadingTime)
            {
                // Progress must be a number from 0-1 (it will be clamped)
                // It will be formatted according to 'ProgressFormat' (0.0 by default) and followed with a '%' character 
                float progress = (Time.time - startTime) / LoadingTime;
                ProgressIndicator.Instance.SetMessage(message);
                ProgressIndicator.Instance.SetProgress(progress);
                yield return new WaitForSeconds(Random.Range(0.15f, 0.5f));
            }

            // Give the user a final notification that loading has finished (optional)
            ProgressIndicator.Instance.SetMessage(FinishMessage);
            ProgressIndicator.Instance.SetProgress(1f);
            yield return new WaitForSeconds(FinishTime);

            // Close the loading dialog
            // ProgressIndicator.Instance.IsLoading will report true until its 'Closing' animation has ended
            // This typically takes about 1 second
            ProgressIndicator.Instance.Close();
            while (ProgressIndicator.Instance.IsLoading)
            {
                yield return null;
            }
        }
    }
}
