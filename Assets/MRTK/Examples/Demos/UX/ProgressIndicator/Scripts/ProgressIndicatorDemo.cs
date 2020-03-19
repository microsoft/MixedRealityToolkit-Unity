// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.UI;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Demo class for IProgressIndicator examples
    /// </summary>
    public class ProgressIndicatorDemo : MonoBehaviour
    {
        [SerializeField, Header("Indicators")]
        private GameObject progressIndicatorLoadingBarGo = null;
        [SerializeField]
        private GameObject progressIndicatorRotatingObjectGo = null;
        [SerializeField]
        private GameObject progressIndicatorRotatingOrbsGo = null;

        [SerializeField, Header("Editor Keyboard Controls")]
        private KeyCode toggleBarKey = KeyCode.Alpha1;
        [SerializeField]
        private KeyCode toggleRotatingKey = KeyCode.Alpha2;
        [SerializeField]
        private KeyCode toggleOrbsKey = KeyCode.Alpha3;

        [SerializeField, Header("Settings")]
        private string[] loadingMessages = new string[] { 
            "First Loading Message",
            "Loading Message 1", 
            "Loading Message 2", 
            "Loading Message 3", 
            "Final Loading Message" };

        [SerializeField, Range(1f, 10f)]
        private float loadingTime = 5f;

        private IProgressIndicator progressIndicatorLoadingBar;
        private IProgressIndicator progressIndicatorRotatingObject;
        private IProgressIndicator progressIndicatorRotatingOrbs;

        /// <summary>
        /// Target method for demo button
        /// </summary>
        public void OnClickBar()
        {
            HandleButtonClick(progressIndicatorLoadingBar);
        }

        /// <summary>
        /// Target method for demo button
        /// </summary>
        public void OnClickRotating()
        {
            HandleButtonClick(progressIndicatorRotatingObject);
        }

        /// <summary>
        /// Target method for demo button
        /// </summary>
        public void OnClickOrbs()
        {
            HandleButtonClick(progressIndicatorRotatingOrbs);
        }

        private async void HandleButtonClick(IProgressIndicator indicator)
        {
            // If the indicator is opening or closing, wait for that to finish before trying to open / close it
            // Otherwise the indicator will display an error and take no action
            await indicator.AwaitTransitionAsync();

            switch (indicator.State)
            {
                case ProgressIndicatorState.Closed:
                    OpenProgressIndicator(indicator);
                    break;

                case ProgressIndicatorState.Open:
                    await indicator.CloseAsync();
                    break;
            }
        }

        private void OnEnable()
        {
            progressIndicatorLoadingBar = progressIndicatorLoadingBarGo.GetComponent<IProgressIndicator>();
            progressIndicatorRotatingObject = progressIndicatorRotatingObjectGo.GetComponent<IProgressIndicator>();
            progressIndicatorRotatingOrbs = progressIndicatorRotatingOrbsGo.GetComponent<IProgressIndicator>();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(toggleBarKey))
            {
                HandleButtonClick(progressIndicatorLoadingBar);
            }

            if (UnityEngine.Input.GetKeyDown(toggleRotatingKey))
            {
                HandleButtonClick(progressIndicatorRotatingObject);
            }

            if (UnityEngine.Input.GetKeyDown(toggleOrbsKey))
            {
                HandleButtonClick(progressIndicatorRotatingOrbs);
            }
        }

        private async void OpenProgressIndicator(IProgressIndicator indicator)
        {
            await indicator.OpenAsync();

            float timeStarted = Time.time;
            while (Time.time < timeStarted + loadingTime)
            {
                float normalizedProgress = Mathf.Clamp01((Time.time - timeStarted) / loadingTime);
                indicator.Progress = normalizedProgress;
                indicator.Message = loadingMessages[Mathf.FloorToInt(normalizedProgress * loadingMessages.Length)];

                await Task.Yield();

                switch (indicator.State)
                {
                    case ProgressIndicatorState.Open:
                        break;

                    default:
                        // The indicator was closed
                        return;
                }
            }

            await indicator.CloseAsync();
        }
    }
}