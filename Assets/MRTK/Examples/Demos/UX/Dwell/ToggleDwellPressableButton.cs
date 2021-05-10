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
    /// This script uses _BorderWidth property of Mixed Reality Standard Shader
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/ToggleDwellPressableButton")]
    public class ToggleDwellPressableButton : BaseDwellPressableButton
    {
        [SerializeField]
        private TextMeshPro dwellStatus = null;

        [SerializeField]
        private GameObject buttonBackground = null;

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
                dwellVisualImage.GetComponentInChildren<MeshRenderer>().material.SetFloat("_BorderWidth", value);
            }
            else if (!IsDwelling && dwellVisualImage.transform.localScale.x > 0)
            {
                float value = Mathf.Clamp(dwellVisualImage.transform.localScale.x - (cancelStartScale / dwellVisualCancelDurationInFrames), 0f, 1f);
                dwellVisualImage.GetComponentInChildren<MeshRenderer>().material.SetFloat("_BorderWidth", value);
            }
        }

        /// <inheritdoc/>
        public override void DwellIntended(IMixedRealityPointer pointer)
        {
            buttonBackground.GetComponent<MeshRenderer>().material.color = dwellIntendedColor;
        }

        /// <inheritdoc/>
        public override void DwellStarted(IMixedRealityPointer pointer)
        {
            base.DwellStarted(pointer);
            buttonBackground.GetComponent<MeshRenderer>().material.color = dwellIntendedColor;
        }

        /// <inheritdoc/>
        public override void DwellCanceled(IMixedRealityPointer pointer)
        {
            base.DwellCanceled(pointer);
            buttonBackground.GetComponent<MeshRenderer>().material.color = isDwellEnabled ? this.dwellOnColor : this.dwellOffColor;
            cancelStartScale = dwellVisualImage.GetComponentInChildren<MeshRenderer>().material.GetFloat("_BorderWidth");
        }

        /// <inheritdoc/>
        public override void DwellCompleted(IMixedRealityPointer pointer)
        {
            base.DwellCompleted(pointer);
            buttonBackground.GetComponent<MeshRenderer>().material.color = isDwellEnabled ? this.dwellOnColor : this.dwellOffColor;
            //dwellVisualImage.transform.localScale = Vector3.zero;
        }

        /// <inheritdoc/>
        public override void ButtonExecute()
        {
            isDwellEnabled = !isDwellEnabled;
            dwellStatus.text = isDwellEnabled ? "On" : "Off";

            // swap the button background and dwell visuals overlay color
            buttonBackground.GetComponent<MeshRenderer>().material.color = isDwellEnabled ? this.dwellOnColor : this.dwellOffColor;
            dwellVisualImage.GetComponentInChildren<MeshRenderer>().material.color = isDwellEnabled ? this.dwellOffColor : this.dwellOnColor;

            var foundObjects = FindObjectsOfType<DwellHandler>();

            foreach (var obj in foundObjects)
            {
                obj.enabled = isDwellEnabled;
            }

            DwellHandler.enabled = true;
        }
    }
}
