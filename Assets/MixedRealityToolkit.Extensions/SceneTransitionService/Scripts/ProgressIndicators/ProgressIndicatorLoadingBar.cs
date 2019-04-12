// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    /// <summary>
    /// Displays progress bar and (optionally) percentage text.
    /// </summary>
    [ExecuteAlways]
    public class ProgressIndicatorLoadingBar : MonoBehaviour, IProgressIndicator
    {
        const float SmoothProgressSpeed = 0.25f;

        public ProgressIndicatorState State { get { return state; } }
        public float Progress { set { targetProgress = value; } }
        public string Message { set { messageText.text = value; } }

        // The animated progress bar object
        [SerializeField]
        private Transform progressBar = null;

        // The message text used by the 'Visible' message style
        [SerializeField]
        private TextMeshPro messageText = null;

        // The progress text used by all non-'None' progress styles
        [SerializeField]
        private TextMeshPro progressText = null;

        [Tooltip("Format to use when converting progress number to string.")]
        [SerializeField]
        private string progressStringFormat = "{0:P0}";

        [Tooltip("Whether to display the percentage as text in addition to the loading bar.")]
        [SerializeField]
        private bool displayPercentage = true;
        
        private float smoothProgress = 0f;
        private ProgressIndicatorState state = ProgressIndicatorState.Closed;

        /// <summary>
        /// Target for smooth progress, 0-1. Serialized to test in editor.
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        private float targetProgress = 0f;
        
        public async Task OpenAsync()
        {
            gameObject.SetActive(true);

            state = ProgressIndicatorState.Opening;

            await Task.Yield();

            state = ProgressIndicatorState.Displaying;
        }

        public async Task CloseAsync()
        {
            state = ProgressIndicatorState.Closing;

            await Task.Yield();

            state = ProgressIndicatorState.Closed;

            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (state != ProgressIndicatorState.Displaying)
                return;

            smoothProgress = Mathf.Lerp(smoothProgress, targetProgress, SmoothProgressSpeed);

            if (smoothProgress > 0.99f)
                smoothProgress = 1f;

            progressBar.localScale = new Vector3(smoothProgress, 1f, 1f);

            progressText.gameObject.SetActive(displayPercentage);
            progressText.text = string.Format(progressStringFormat, smoothProgress);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
