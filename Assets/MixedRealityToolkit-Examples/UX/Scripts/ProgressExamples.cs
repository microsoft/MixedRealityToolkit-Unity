// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using MixedRealityToolkit.UX.Progress;
using MixedRealityToolkit.InputModule;
using MixedRealityToolkit.InputModule.InputSources;
using UnityEngine.XR.WSA.Input;
using MixedRealityToolkit.Utilities.Solvers;
using UnityEngine.XR.WSA;

namespace MixedRealityToolkit.Examples.UX
{
    public class ProgressExamples : MonoBehaviour
    {
        public GameObject SceneObject;

        [Header("How long to spend on each stage of loading")]
        public float LeadInTime = 1.5f;
        public float LoadingTime = 5f;
        public float FinishTime = 1.5f;

        [Header("Set these to override the defaults set in the ProgressIndicator prefab")]
        public GameObject LoadingPrefab = null;
        public Texture2D LoadingIcon = null;

        [Header("Messages displayed during loading")]
        public string LeadInMessage = "Lead in Message";
        public string LoadTextMessage = "Loading with message only";
        public string LoadOrbsMessage = "Loading with Orbs";
        public string LoadIconMessage = "Loading with Icon";
        public string LoadPrefabMessage = "Loading with Prefab";
        public string LoadProgressMessage = "Loading with Progress";
        public string LoadProgressBarMessage = "Loading with Bar";
        public string FinishMessage = "Finished!";

        private void Start()
        {
            // Optimize the content for immersive headset
            if (HolographicSettings.IsDisplayOpaque)
            {
               GameObject buttonCollection = GameObject.Find("ButtonCollection");
               buttonCollection.transform.localScale = new Vector3(1.3f, 1.3f, 1.0f);
            }
        }

        public void LaunchProgress(GameObject obj)
        {
            Debug.Log("Loading with button " + obj.name);

            if (ProgressIndicator.Instance.IsLoading)
                return;

            switch (obj.name)
            {
                case "ButtonLoadText":
                    ProgressIndicator.Instance.Open(
                            ProgressIndicator.IndicatorStyleEnum.None,
                            ProgressIndicator.ProgressStyleEnum.None,
                            ProgressIndicator.MessageStyleEnum.Visible,
                            LeadInMessage);
                    StartCoroutine(LoadOverTime(LoadTextMessage));
                    break;

                case "ButtonLoadDefault":
                    ProgressIndicator.Instance.Open(
                             ProgressIndicator.IndicatorStyleEnum.AnimatedOrbs,
                             ProgressIndicator.ProgressStyleEnum.None,
                             ProgressIndicator.MessageStyleEnum.Visible,
                             LeadInMessage);
                    StartCoroutine(LoadOverTime(LoadOrbsMessage));
                    break;

                case "ButtonLoadIcon":
                    ProgressIndicator.Instance.Open(
                        ProgressIndicator.IndicatorStyleEnum.StaticIcon,
                        ProgressIndicator.ProgressStyleEnum.None,
                        ProgressIndicator.MessageStyleEnum.Visible,
                        LeadInMessage,
                        null);
                    StartCoroutine(LoadOverTime(LoadIconMessage));
                    break;

                case "ButtonLoadPrefab":
                    ProgressIndicator.Instance.Open(
                        ProgressIndicator.IndicatorStyleEnum.Prefab,
                        ProgressIndicator.ProgressStyleEnum.None,
                        ProgressIndicator.MessageStyleEnum.Visible,
                        LeadInMessage,
                        LoadingPrefab);
                    StartCoroutine(LoadOverTime(LoadPrefabMessage));
                    break;

                case "ButtonLoadProgress":
                    ProgressIndicator.Instance.Open(
                        ProgressIndicator.IndicatorStyleEnum.None,
                        ProgressIndicator.ProgressStyleEnum.Percentage,
                        ProgressIndicator.MessageStyleEnum.Visible,
                        LeadInMessage);
                    StartCoroutine(LoadOverTime(LoadProgressMessage));
                    break;

                case "ButtonLoadProgressBar":
                    ProgressIndicator.Instance.Open(
                        ProgressIndicator.IndicatorStyleEnum.None,
                        ProgressIndicator.ProgressStyleEnum.ProgressBar,
                        ProgressIndicator.MessageStyleEnum.Visible,
                        LeadInMessage);
                    StartCoroutine(LoadOverTime(LoadProgressBarMessage));
                    break;

                default:
                    break;
            }
        }
        protected IEnumerator LoadOverTime(string message)
        {
            // Wait for lead in time to end (optional)
            float startTime = Time.time;
            yield return new WaitForSeconds(LeadInTime);

            // Progress must be a number from 0-1 (it will be clamped)
            // It will be formatted according to 'ProgressFormat' (0.0 by default) and followed with a '%' character 
            float progress = 0f;
            // While we're in the loading period, update progress and message in 1/4 second intervals
            // Displayed progress is smoothed out so you don't have to update every frame
            startTime = Time.time;
            while (Time.time < startTime + LoadingTime)
            {
                progress = (Time.time - startTime) / LoadingTime;
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

            yield break;
        }
    }
}
