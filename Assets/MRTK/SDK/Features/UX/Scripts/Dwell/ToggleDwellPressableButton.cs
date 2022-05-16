// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Dwell
{
    /// <summary>
    /// Example script to demonstrate a toggle button using dwell
    /// This script uses _BorderWidth property of Mixed Reality Standard Shader
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/ToggleDwellPressableButton")]
    public class ToggleDwellPressableButton : BaseDwellPressableButton
    {
        [SerializeField]
        private TextMeshPro dwellStatus = null;

        [SerializeField]
        private MeshRenderer buttonBackground = null;

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
                dwellVisualImage.material.SetFloat("_BorderWidth", value);
            }
            else if (!IsDwelling && dwellVisualImage.transform.localScale.x > 0)
            {
                float value = Mathf.Clamp(dwellVisualImage.transform.localScale.x - (cancelStartScale / dwellVisualCancelDurationInFrames), 0f, 1f);
                dwellVisualImage.material.SetFloat("_BorderWidth", value);
            }
        }

        /// <inheritdoc/>
        public override void DwellIntended(IMixedRealityPointer pointer)
        {
            buttonBackground.material.color = dwellIntendedColor;
        }

        /// <inheritdoc/>
        public override void DwellStarted(IMixedRealityPointer pointer)
        {
            base.DwellStarted(pointer);
            buttonBackground.material.color = dwellIntendedColor;
        }

        /// <inheritdoc/>
        public override void DwellCanceled(IMixedRealityPointer pointer)
        {
            base.DwellCanceled(pointer);
            buttonBackground.material.color = isDwellEnabled ? this.dwellOnColor : this.dwellOffColor;
            cancelStartScale = dwellVisualImage.material.GetFloat("_BorderWidth");
        }

        /// <inheritdoc/>
        public override void DwellCompleted(IMixedRealityPointer pointer)
        {
            base.DwellCompleted(pointer);
            buttonBackground.material.color = isDwellEnabled ? this.dwellOnColor : this.dwellOffColor;
        }

        /// <inheritdoc/>
        public override void ButtonExecute()
        {
            isDwellEnabled = !isDwellEnabled;
            dwellStatus.text = isDwellEnabled ? "On" : "Off";

            // swap the button background and dwell visuals overlay color
            buttonBackground.material.color = isDwellEnabled ? this.dwellOnColor : this.dwellOffColor;
            dwellVisualImage.material.color = isDwellEnabled ? this.dwellOffColor : this.dwellOnColor;
        }
    }
}
