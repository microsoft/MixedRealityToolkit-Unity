// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Dwell
{
    /// <summary>
    /// Example script to demonstrate a toggle button using dwell
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/ToggleDwellSample")]
    public class ToggleDwellSample : BaseDwellSample
    {
        [SerializeField]
        private TextMeshProUGUI dwellStatus = null;

        [SerializeField]
        private Image buttonBackground = null;

        private bool isDwellEnabled = true;

        [SerializeField]
        private Color dwellOnColor = Color.white;

        [SerializeField]
        private Color dwellOffColor = Color.cyan;

        [SerializeField]
        private Color dwellIntendedColor = Color.cyan;

        [SerializeField]
        private float dwellVisualCancelDurationInFrames = 60;

        private float cancelStartScale = 0;

        private void Update()
        {
            if (IsDwelling)
            {
                float value = DwellHandler.DwellProgress;
                dwellVisualImage.transform.localScale = new Vector3(value, value, value);
            }
            else if (!IsDwelling && dwellVisualImage.transform.localScale.x > 0)
            {
                float value = Mathf.Clamp(dwellVisualImage.transform.localScale.x - (cancelStartScale / dwellVisualCancelDurationInFrames), 0f, 1f);
                dwellVisualImage.transform.localScale = new Vector3(value, value, value);
            }
        }

        /// <inheritdoc/>
        public override void DwellIntended(IMixedRealityPointer pointer)
        {
            buttonBackground.color = dwellIntendedColor;
            dwellVisualImage.transform.localScale = Vector3.zero;
        }

        /// <inheritdoc/>
        public override void DwellStarted(IMixedRealityPointer pointer)
        {
            base.DwellStarted(pointer);
            buttonBackground.color = dwellIntendedColor;
        }

        /// <inheritdoc/>
        public override void DwellCanceled(IMixedRealityPointer pointer)
        {
            base.DwellCanceled(pointer);
            buttonBackground.color = isDwellEnabled ? this.dwellOnColor : this.dwellOffColor;
            cancelStartScale = dwellVisualImage.transform.localScale.x;
        }

        /// <inheritdoc/>
        public override void DwellCompleted(IMixedRealityPointer pointer)
        {
            base.DwellCompleted(pointer);
            dwellVisualImage.transform.localScale = Vector3.zero;
        }

        /// <inheritdoc/>
        public override void ButtonExecute()
        {
            isDwellEnabled = !isDwellEnabled;
            dwellStatus.text = isDwellEnabled ? "On" : "Off";

            // swap the button background and dwell visuals overlay color
            buttonBackground.color = isDwellEnabled ? this.dwellOnColor : this.dwellOffColor;
            dwellVisualImage.color = isDwellEnabled ? this.dwellOffColor : this.dwellOnColor;
        }
    }
}
