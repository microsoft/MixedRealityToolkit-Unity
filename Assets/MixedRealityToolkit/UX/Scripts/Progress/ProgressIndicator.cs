// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using UnityEngine;

namespace MixedRealityToolkit.UX.Progress
{
    /// <summary>
    /// Singleton for displaying a simple loading dialog
    /// Can be combined with a radial solver to keep locked to the HMD
    /// </summary>
    public class ProgressIndicator : Singleton<ProgressIndicator>
    {
        private const float SmoothProgressSpeed = 10f;

        public bool IsLoading
        {
            get
            {
                return Instance.gameObject.activeSelf;
            }
        }

        public float Progress
        {
            get
            {
                return smoothProgress;
            }
        }

        [SerializeField]
        private IndicatorStyleEnum defaultIndicatorStyle = IndicatorStyleEnum.AnimatedOrbs;

        [SerializeField]
        private ProgressStyleEnum defaultProgressStyle = ProgressStyleEnum.Percentage;

        //[SerializeField]
        //private MessageStyleEnum defaultMessageStyle = MessageStyleEnum.Visible;

        // The default prefab used by the 'Prefab' indicator style
        [SerializeField]
        private GameObject defaultPrefab = null;

        // The default icon used by the 'StaticIcon' indicator style
        [SerializeField]
        private GameObject defaultIconPrefab = null;

        [SerializeField]
        private GameObject defaultOrbsPrefab = null;

        // The progress bar container object
        [SerializeField]
        private GameObject progressBarContainer = null;

        // The animated progress bar object
        [SerializeField]
        private Transform progressBar = null;

        // The message text used by the 'Visible' message style
        [SerializeField]
        private TextMesh messageText = null;

        // The progress text used by all non-'None' progress styles
        [SerializeField]
        private TextMesh progressText = null;

        [SerializeField]
        private Animator animator = null;

        private float smoothProgress = 0f;
        private float targetProgress = 0f;
        private bool closing = false;
        private GameObject instantiatedCustomObject = null;

        /// <summary>
        /// Format to be used for the progress number
        /// </summary>
        public string ProgressFormat = "0.0";

        /// <summary>
        /// Opens the dialog with full custom options
        /// </summary>
        /// <param name="indicatorStyle"></param>
        /// <param name="progressStyle"></param>
        /// <param name="messageStyle"></param>
        /// <param name="message"></param>
        /// <param name="prefab"></param>
        public void Open(IndicatorStyleEnum indicatorStyle, ProgressStyleEnum progressStyle, MessageStyleEnum messageStyle, string message = "", GameObject prefab = null)
        {
            if (gameObject.activeSelf) { return; }

            // Make sure we aren't parented under anything
            transform.parent = null;

            // Turn our common objects on 
            closing = false;
            gameObject.SetActive(true);
            progressText.gameObject.SetActive(progressStyle == ProgressStyleEnum.Percentage);
            progressBarContainer.gameObject.SetActive(progressStyle == ProgressStyleEnum.ProgressBar);
            messageText.gameObject.SetActive(messageStyle != MessageStyleEnum.None);
            messageText.text = message;

            // Reset our loading progress
            smoothProgress = 0f;
            targetProgress = 0f;

            // Re-enable objects based on our style
            switch (indicatorStyle)
            {
                case IndicatorStyleEnum.None:
                    break;

                case IndicatorStyleEnum.StaticIcon:
                    // Instantiate our custom object under our animator
                    if (defaultIconPrefab == null)
                    {
                        Debug.LogError("No Icon prefab available in loading dialog, spawning without one");
                    }
                    else
                    {
                        instantiatedCustomObject = Instantiate(defaultIconPrefab);
                        instantiatedCustomObject.transform.localPosition = new Vector3(0.0f, 13.0f, 0.0f);
                        instantiatedCustomObject.transform.localRotation = Quaternion.identity;
                        instantiatedCustomObject.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);

                        instantiatedCustomObject.transform.Translate(messageText.transform.position);
                        instantiatedCustomObject.transform.SetParent(messageText.transform, false);
                    }
                    break;

                case IndicatorStyleEnum.AnimatedOrbs:
                    if (defaultOrbsPrefab != null)
                    {
                        instantiatedCustomObject = Instantiate(defaultOrbsPrefab);
                        instantiatedCustomObject.transform.localPosition = new Vector3(0.0f, 25.0f, 0.0f);
                        //instantiatedCustomObject.transform.localRotation = Quaternion.identity;
                        instantiatedCustomObject.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);

                        instantiatedCustomObject.transform.Translate(messageText.transform.position);
                        instantiatedCustomObject.transform.SetParent(messageText.transform, false);
                    }
                    break;

                case IndicatorStyleEnum.Prefab:
                    // Instantiate our custom object under our animator
                    if (defaultPrefab == null && prefab == null)
                    {
                        Debug.LogError("No prefab available in loading dialog, spawning without one");
                    }
                    else
                    {
                        instantiatedCustomObject = Instantiate(defaultPrefab);
                        instantiatedCustomObject.transform.localPosition = new Vector3(0.0f, 20.0f, 0.0f);
                        instantiatedCustomObject.transform.localRotation = Quaternion.identity;
                        instantiatedCustomObject.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);

                        instantiatedCustomObject.transform.Translate(messageText.transform.position);
                        instantiatedCustomObject.transform.SetParent(messageText.transform, false);
                    }
                    break;
            }

            animator.SetTrigger("Open");
        }

        /// <summary>
        /// Opens the dialog with default settings for indicator and progress
        /// </summary>
        /// <param name="message"></param>
        public void Open(string message)
        {
            Open(defaultIndicatorStyle, defaultProgressStyle, MessageStyleEnum.Visible, message, null);
        }

        /// <summary>
        /// Updates message.
        /// Has no effect until Open is called.
        /// </summary>
        /// <param name="message"></param>
        public void SetMessage(string message)
        {
            if (!gameObject.activeSelf)
                return;

            messageText.text = message;
        }

        /// <summary>
        /// Updates progress.
        /// Has no effect until Open is called.
        /// </summary>
        /// <param name="progress"></param>
        public void SetProgress(float progress)
        {
            targetProgress = Mathf.Clamp01(progress) * 100;
            // If progress is 100, assume we want to snap to that value
            if (targetProgress == 100)
            {
                smoothProgress = targetProgress;
                if (instantiatedCustomObject != null && instantiatedCustomObject.name.Contains("Orbs"))
                {
                    instantiatedCustomObject.GetComponent<ProgressIndicatorOrbsRotator>().Stop();
                }
            }
        }

        /// <summary>
        /// Initiates the process of closing the dialog.
        /// </summary>
        public void Close()
        {
            if (!gameObject.activeSelf)
                return;

            closing = true;
            progressText.gameObject.SetActive(false);
            messageText.gameObject.SetActive(false);
            animator.SetTrigger("Close");
        }

        private void Start()
        {
            gameObject.SetActive(false);
            progressText.gameObject.SetActive(false);
            messageText.gameObject.SetActive(false);
        }

        private void Update()
        {
            smoothProgress = Mathf.Lerp(smoothProgress, targetProgress, Time.deltaTime * SmoothProgressSpeed);
            progressBar.localScale = new Vector3(smoothProgress / 100, 1f, 1f);
            progressText.text = smoothProgress.ToString(ProgressFormat) + "%";
            // If we're closing, wait for the animator to reach the closed state

            if (closing)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Closed"))
                {
                    // Once we've reached the cloesd state shut down completely
                    closing = false;
                    transform.parent = null;
                    gameObject.SetActive(false);
                    // Destroy our custom object if we made one
                    if (instantiatedCustomObject != null)
                        Destroy(instantiatedCustomObject);
                }
            }
        }
    }
}
